using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.BasicManager.DeclInfor;
using Web_After.Common;
using Web_After.model;

namespace Web_After.CustomerMaintain
{
    public partial class CustomerManage : System.Web.UI.Page
    {
        IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
        int totalProperty = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string action = Request["action"];
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                switch (action)
                {
                    case "loadData":
                        loadData();
                        break;
                    case "delete":
                        deleteData();
                        break;
                    case "export":
                        exportData();
                        break;
                    case "save":
                        save(Request["formdata"]);
                        break;
                    case "add":
                        ImportExcelData();
                        break;

                }
            }

        }
        /// <summary>
        /// 加载数据
        /// </summary>
        private void loadData()
        {
            string strWhere = string.Empty;

            if (!string.IsNullOrEmpty(Request["CODE_S"]))
            {
                strWhere = " and instr(code,'" + Request["CODE_S"] + "')>0";
            }
            if (!string.IsNullOrEmpty(Request["CNNAME_S"]))
            {
                strWhere = " and (instr(name,'" + Request["CNNAME_S"] + "')>0 or instr(chineseabbreviation, '" + Request["CNNAME_S"] + "')>0)";
            }
            if (!string.IsNullOrEmpty(Request["ENGLISHNAME_S"]))
            {
                strWhere = " and instr(englishname,'" + Request["ENGLISHNAME_S"] + "')>0";
            }
            if (!string.IsNullOrEmpty(Request["HSCODE_S"]))
            {
                strWhere = " and hscode='" + Request["HSCODE_S"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["CIQCODE_S"]))
            {
                strWhere = " and ciqcode='" + Request["CIQCODE_S"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = " and enabled='" + Request["ENABLED_S"] + "'";
            }
            string sql = "select * from cusdoc.sys_customer where 1=1 " + strWhere;
            sql = Extension.GetPageSql(sql, "ID", "desc", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            DataTable dt = DBMgr.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        private void deleteData()
        {
            string id = Request["ID"];
            string sql = "delete from cusdoc.sys_customer where id='{0}'";
            string str = DBMgr.ExecuteNonQuery(string.Format(sql, id)) > 0 ? "true" : "false";
            Response.Write("{\"success\":" + str + "}");
            Response.End();
        }
        private void exportData()
        {
            string sql = @"SELECT * FROM cusdoc.sys_customer order by id desc";
            DataTable dt = DBMgr.GetDataTable(sql);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("客商信息");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("代码");
            row1.CreateCell(1).SetCellValue("海关代码");
            row1.CreateCell(2).SetCellValue("国检代码");
            row1.CreateCell(3).SetCellValue("中文名称");
            row1.CreateCell(4).SetCellValue("中文简称");
            row1.CreateCell(5).SetCellValue("中文地址");
            row1.CreateCell(6).SetCellValue("英文名称");
            row1.CreateCell(7).SetCellValue("英文地址");
            row1.CreateCell(8).SetCellValue("是否启用");
            row1.CreateCell(9).SetCellValue("备注");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["HSCODE"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CIQCODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["NAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CHINESEABBREVIATION"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CHINESEADDRESS"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["ENGLISHNAME"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["ENGLISHADDRESS"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["REMARK"].ToString());
            }
            try
            {
                // 输出Excel
                string filename = "客商信息.xls";
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", Server.UrlEncode(filename)));
                Response.Clear();

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                Response.BinaryWrite(ms.GetBuffer());
                Response.End();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private void save(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);

            string sql = ""; DataTable dt_valid_name = new DataTable();
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                sql = @"insert into cusdoc.sys_customer(id
                                    ,code,name,chineseabbreviation,chineseaddress,hscode,ciqcode
                                    ,englishname,englishaddress,iscustomer,isshipper,iscompany
                                    ,logicauditflag,docservicecompany,enabled,remark,SOCIALCREDITNO
                                    ,TOOLVERSION,isreceiver
                                ) values(cusdoc.sys_customer_id.nextval
                                    ,'{0}','{1}','{2}', '{3}','{4}','{5}'
                                    ,'{6}','{7}','{8}','{9}','{10}'
                                    ,'{11}','{12}','{13}','{14}','{15}'
                                    ,'{16}','{17}')";
                sql = string.Format(sql
                        , json.Value<string>("CODE").ToUpper(), json.Value<string>("NAME"), json.Value<string>("CHINESEABBREVIATION"), json.Value<string>("CHINESEADDRESS"), json.Value<string>("HSCODE"), json.Value<string>("CIQCODE")
                        , json.Value<string>("ENGLISHNAME"), json.Value<string>("ENGLISHADDRESS"), GetChk(json.Value<string>("ISCUSTOMER")), GetChk(json.Value<string>("ISSHIPPER")), GetChk(json.Value<string>("ISCOMPANY"))
                        , GetChk(json.Value<string>("LOGICAUDITFLAG")), GetChk(json.Value<string>("DOCSERVICECOMPANY")), json.Value<string>("ENABLED"), json.Value<string>("REMARK"), json.Value<string>("SOCIALCREDITNO")
                        , json.Value<string>("TOOLVERSION"),GetChk(json.Value<string>("ISRECEIVER"))
                    );

                dt_valid_name = DBMgr.GetDataTable("select * from cusdoc.sys_customer where lower(code)='" + json.Value<string>("CODE").ToLower() + "'");
            }
            else
            {
                sql = @"update cusdoc.sys_customer set code='{0}',name='{1}',chineseabbreviation='{2}',chineseaddress='{3}',hscode='{4}',ciqcode='{5}'
                                    ,englishname='{6}',englishaddress='{7}',iscustomer='{8}',isshipper='{9}',iscompany='{10}'
                                    ,logicauditflag='{11}',docservicecompany='{12}',enabled='{13}',remark='{14}',SOCIALCREDITNO='{15}' 
                                    ,TOOLVERSION='{16}',isreceiver='{17}' 
                                where id={18}";
                sql = string.Format(sql
                        , json.Value<string>("CODE").ToUpper(), json.Value<string>("NAME"), json.Value<string>("CHINESEABBREVIATION"), json.Value<string>("CHINESEADDRESS"), json.Value<string>("HSCODE"), json.Value<string>("CIQCODE")
                        , json.Value<string>("ENGLISHNAME"), json.Value<string>("ENGLISHADDRESS"), GetChk(json.Value<string>("ISCUSTOMER")), GetChk(json.Value<string>("ISSHIPPER")), GetChk(json.Value<string>("ISCOMPANY"))
                        , GetChk(json.Value<string>("LOGICAUDITFLAG")), GetChk(json.Value<string>("DOCSERVICECOMPANY")), json.Value<int>("ENABLED"), json.Value<string>("REMARK"), json.Value<string>("SOCIALCREDITNO")
                        , json.Value<string>("TOOLVERSION"),GetChk(json.Value<string>("ISRECEIVER")), json.Value<string>("ID")
                   );
                dt_valid_name = DBMgr.GetDataTable("select * from cusdoc.sys_customer where lower(code)='" + json.Value<string>("CODE").ToLower() + "' and id!=" + json.Value<string>("ID"));
            }
            string response = "";
             //验证用户是否重复
            if (dt_valid_name != null && dt_valid_name.Rows.Count != 0)
            {
                response = "{\"success\":false,\"flag\":1}";
            }
            else
            {
                int i = DBMgr.ExecuteNonQuery(sql);
                response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
            }
            Response.Write(response);
            Response.End();
        }

        public string GetChk(string check_val)
        {
            return check_val == "on" ? "1" : "0";
        }

        public void ImportExcelData()
        {
            Base_Company_Method bcm = new Base_Company_Method();
            string formdata = Request["formdata"]; string action = Request["action"];
            JObject json_formdata = (JObject)JsonConvert.DeserializeObject(formdata);
            string reponseresult = "";
            HttpPostedFile postedFile = Request.Files["UPLOADFILE"];//获取上传信息对象  
            string fileName = Path.GetFileName(postedFile.FileName);
            if (!Directory.Exists("/FileUpload/PreData"))
            {
                Directory.CreateDirectory("/FileUpload/PreData");
            }
            string newfile = @"/FileUpload/PreData/" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + fileName;
            postedFile.SaveAs(Server.MapPath(newfile));
            Dictionary<int,List<int>> result = upload_base_company(newfile, fileName, action, json_formdata);

            List<int> succInts = result[1];
            List<int> errorInts = result[2];
            string errorStr = "";
            for (int i = 0; i < errorInts.Count; i++)
            {
                errorStr = errorStr + errorInts[i] + ",";
            }

            //返回失败信息
            string responseerrorlist = "";
            //返回成功信息
            string responsesuccesslist = "";


            if (errorInts.Count > 0)
            {
                responseerrorlist = "插入失败的行数为：" + errorStr;
            }

            if (succInts.Count > 0)
            {
                responsesuccesslist = "成功插入" + succInts[0] + "行!";
            }
            reponseresult = responsesuccesslist + responseerrorlist;

            string response = "{\"success\":\"" + reponseresult + "\"}";
            Response.Write(response);
            Response.End();

        }

        public Dictionary<int,List<int>> upload_base_company(string newfile, string fileName, string action,
            JObject json_formdata)
        {
            Base_Company_Method bcm = new Base_Company_Method();
            Sql.CustomerManage cm = new Sql.CustomerManage();
            CustomerEn cus = new CustomerEn();
            DataTable dtExcel = bcm.GetExcelData_Table(Server.MapPath(newfile), 0);
            List<string> stringList = new List<string>();

            //记住发生错误的行数
            List<int> errorlines = new List<int>();

            //记住插入成功的个数
            int count = 0;

            //插入成功的个数(返回放入dictionary)
            List<int> successinsert = new List<int>();

            

            //返回值
            Dictionary<int,List<int>> returndic = new Dictionary<int, List<int>>();

            for (int i = 0; i < dtExcel.Rows.Count; i++)
            {

                for (int j = 0; j < dtExcel.Columns.Count; j++)
                {
                    stringList.Add(dtExcel.Rows[i][j].ToString());
                }

                //客户编码                      //海关编码
                string code = stringList[0];   string HSCODE = stringList[1];
                //国检编码                       //中文名称
                string CIQCODE = stringList[2]; string CHINESEABBREVIATION = stringList[3];
                //中文简称                         //中文地址
                string name = stringList[4];   string CHINESEADDRESS = stringList[5];
                //英文名称                               //英文地址
                string ENGLISHNAME = stringList[6];     string ENGLISHADDRESS = stringList[7];
                //是否启用                                       //备注
                string enabled = stringList[8]=="是"?"1":"0";  string remark = stringList[9];

                //需要验证客户编码是否重复，客户编码是为空，中文简称不能为空
                cus.Code = code; cus.HSCode = HSCODE;
                cus.CIQCode = CIQCODE; cus.ChineseAbbreviation = CHINESEABBREVIATION;
                cus.name = name; cus.ChineseAddress = CHINESEADDRESS;
                cus.EnglishName = ENGLISHNAME; cus.EnglishAddress = ENGLISHADDRESS;
                cus.Enabled = enabled.ToInt(); cus.Remark = remark;
                int p = cm.before_import_check(code).Rows.Count;
                if (cm.before_import_check(code).Rows.Count>0 || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
                {
                    errorlines.Add(i+2);
                }
                else
                {
                    cm.insert_import_sys_customer(cus);
                    count = count + 1;
                }
                stringList.Clear();
            }
            successinsert.Add(count);
            returndic.Add(1, successinsert);
            returndic.Add(2, errorlines);
            return returndic;
        }
    }
}