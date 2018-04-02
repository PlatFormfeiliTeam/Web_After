using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Web_After.BasicManager.BasicManager;
using Web_After.BasicManager.DeclInfor;
using Web_After.Common;
using Web_After.model;

namespace Web_After.BasicManager.InspInfor
{
    public partial class busi_SpecialHsConvernet : System.Web.UI.Page
    {
        IsoDateTimeConverter iso = new IsoDateTimeConverter();
        int totalProperty = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string action = Request["action"];
                iso.DateTimeFormat = "yyyy-MM-dd";
                switch (action)
                {
                    case "loadData":
                        loaddata();
                        break;
                    case "getCheckBoxData":
                        getCountry();
                        break;
                    case "save":
                        save();
                        break;
                    case "export":
                        export();
                        break;
                    case "add":
                        ImportExcelData();
                        break;
                }
            }
        }

        private void loaddata()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["CodeBase"]))
            {
                strWhere = strWhere + " and t1.code like '%" + Request["CodeBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["RecordBase"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["RecordBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.busi_SpecialHsConvernet bs = new Sql.busi_SpecialHsConvernet();
            DataTable dt = bs.loaddata(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();

        }

        //获取国家
        private void getCountry()
        {
            string sql = String.Empty;
            sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
              from base_country t1
              left join sys_user t2
                on t1.createman = t2.id
              left join sys_user t3
                on t1.stopman = t3.id
                where t1.enabled = '1'";
            DataTable dt = DBMgrBase.GetDataTable(sql);
            string country = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{COUNTRY:" + country + "}");
            Response.End();
        }

        //获取登录人
        public string Username()
        {
            string userName = "";
            FormsIdentity identity = User.Identity as FormsIdentity;
            if (identity == null)
            {
                return "";
            }
            return userName = identity.Name;
        }

        private void save()
        {
            string formdata = Request["formdata"];
            string country = Request["country"];
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            Sql.busi_SpecialHsConvernet bsh = new Sql.busi_SpecialHsConvernet();
            string response = String.Empty;
            Base_Codename_Method bm = new Base_Codename_Method();
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                //查询是否有重复值
                if (json.Value<string>("ENABLED") == "1")
                {
                    if (bsh.check_repeat(json).Rows.Count > 0)
                    {
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        int i = bsh.inser_specialhsconvert(json, country);
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }
                }
                else
                {
                    int i = bsh.inser_specialhsconvert(json, country);
                    response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }
            }
            else
            {
                if (json.Value<string>("ENABLED") == "1")
                {
                    if (bsh.check_update_repeat(json).Rows.Count > 0)
                    {
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        DataTable dt = bsh.before_data(json);
                        int i = bsh.update_base_specialhsconvert(json, country);
                        if (i > 0)
                        {
                            string content = bm.getbasespecialhsconvert(dt, json, country);
                            bsh.insert_alert_record(json, content);
                            response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                        }
                    }
                }
                else
                {
                        DataTable dt = bsh.before_data(json);
                        int i = bsh.update_base_specialhsconvert(json, country);
                        if (i > 0)
                        {
                            string content = bm.getbasespecialhsconvert(dt, json, country);
                            bsh.insert_alert_record(json, content);
                            response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                        }
                }
            }
            Response.Write(response);
            Response.End();
        }

        //导出
        private void export()
        {
            string strWhere = "";
            string combo_ENABLED_S2 = Request["combo_ENABLED_S"];
            if (combo_ENABLED_S2 == "null")
            {
                combo_ENABLED_S2 = String.Empty;
            }
            if (!string.IsNullOrEmpty(Request["CodeBase"]))
            {
                strWhere = strWhere + " and t1.code like '%" + Request["CodeBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["RecordBase"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["RecordBase"] + "%'";
            }

            if (!string.IsNullOrEmpty(combo_ENABLED_S2))
            {
                strWhere = strWhere + " and t1.enabled='" + combo_ENABLED_S2 + "'";
            }

            Sql.busi_SpecialHsConvernet bsh = new Sql.busi_SpecialHsConvernet();
            DataTable dt =  bsh.export_base_specialhsconvert(strWhere);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("特殊商品单位换算");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("商品HS编码");
            row1.CreateCell(1).SetCellValue("附加码");
            row1.CreateCell(2).SetCellValue("商品名称");
            row1.CreateCell(3).SetCellValue("类型");
            row1.CreateCell(4).SetCellValue("国家");
            row1.CreateCell(5).SetCellValue("启用/禁用");
            row1.CreateCell(6).SetCellValue("维护人");
            row1.CreateCell(7).SetCellValue("停用人");
            row1.CreateCell(8).SetCellValue("启用时间");
            row1.CreateCell(9).SetCellValue("停用时间");
            row1.CreateCell(10).SetCellValue("维护时间");
            row1.CreateCell(11).SetCellValue("备注");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["EXTRACODE"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["NAME"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["TYPE"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["COUNTRY"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["CREATEMANNAME"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["STOPMANNAME"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["STARTDATE"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["ENDDATE"].ToString());
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CREATEDATE"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["REMARK"].ToString());
            }
            try
            {
                // 输出Excel
                string filename = "备案信息.xls";
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

        //导入
        private void ImportExcelData()
        {
            Base_Company_Method bcm = new Base_Company_Method();
            string formdata = Request["formdata"];
            string action = Request["action"];
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
            Dictionary<int, List<int>> result = upload_base_company(newfile, fileName, action, json_formdata);

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

        public Dictionary<int, List<int>> upload_base_company(string newfile, string fileName, string action,
            JObject json_formdata)
        {
            Base_Company_Method bcm = new Base_Company_Method();
            Switch_helper_Base_blend_web sw = new Switch_helper_Base_blend_web();
            DataTable dtExcel = bcm.GetExcelData_Table(Server.MapPath(newfile), 0);
            List<string> stringList = new List<string>();

            //记住发生错误的行数
            List<int> errorlines = new List<int>();

            //记住插入成功的个数
            int count = 0;

            //Sql.busi_RecordInfor bc = new Sql.busi_RecordInfor();
            Sql.busi_SpecialHsConvernet bsh = new Sql.busi_SpecialHsConvernet();
            //插入成功的个数(返回放入dictionary)
            List<int> successinsert = new List<int>();
            //返回值
            Dictionary<int, List<int>> returndic = new Dictionary<int, List<int>>();
            for (int i = 0; i < dtExcel.Rows.Count; i++)
            {

                for (int j = 0; j < dtExcel.Columns.Count; j++)
                {
                    stringList.Add(dtExcel.Rows[i][j].ToString());
                }
                //商品HS编码                           //附加码
                string CODE = stringList[0]; string EXTRACODE = stringList[1];
                //是否启用
                string ENABLED = stringList[5] == "是" ? "1" : "0";
                //商品名称                                        
                string NAME = stringList[2];
                //类型
                string TYPE = stringList[3];
                //国家
                string COUNTRY = stringList[4];
                //备注
                string REMARK = stringList[6];
                //启用日期
                string startdate = json_formdata.Value<string>("STARTDATE");
                //停用日期
                string enddate = json_formdata.Value<string>("ENDDATE");

                string formdata = "{\"CODE\":\"" + CODE + "\",\"EXTRACODE\":\"" + EXTRACODE + "\",\"ENABLED\":\"" + ENABLED + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\"," +
                                  "\"NAME\":\"" + NAME + "\",\"TYPE\":\"" + TYPE + "\",\"COUNTRY\":\"" + COUNTRY + "\",\"REMARK\":\"" + REMARK + "\"}";
                JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
                if (bsh.check_repeat(json).Rows.Count > 0 || string.IsNullOrEmpty(json.Value<string>("CODE")))
                {
                    errorlines.Add(i + 2);
                }
                else
                {
                    bsh.inser_specialhsconvert(json, COUNTRY);
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