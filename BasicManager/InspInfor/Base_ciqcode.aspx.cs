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
using Org.BouncyCastle.Bcpg.OpenPgp;
using Web_After.BasicManager.DeclInfor;
using Web_After.Common;
using Web_After.model;
using Web_After.Sql;

namespace Web_After.BasicManager.InspInfor
{
    public partial class Base_ciqcode : System.Web.UI.Page
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
                        loadDate();
                        break;
                    case "save":
                        save(Request["formdata"]);
                        break;
                    case "change":
                        change();
                        break;
                    case "MaintainloadData":
                        MaintainloadData();
                        break;
                    case "MaintainSave":
                        MaintainSave(Request["formdata"]);
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

        
        private void loadDate()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["CiqCodeBase"]))
            {
                strWhere = strWhere + " and t1.Name like '%" + Request["CiqCodeBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["start_date"]))
            {
                strWhere = strWhere + " and t1.startDate >= to_date('" + Request["start_date"] + "','yyyy-mm-dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(Request["end_date"]))
            {
                strWhere = strWhere + " and t1.endDate < to_date('" + Request["end_date"] + "','yyyy-mm-dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Basic_ciqcode bc = new Basic_ciqcode();
            DataTable dt = bc.LoaData(strWhere, "ID", "desc", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }
        //新建
        private void save(string formdata)
        {
            


            //停用人
            string stopman=String.Empty;
            //返回给前端的值
            string response = "";
            //从前端获取值
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            //sql语句执行类
            Basic_ciqcode bc = new Basic_ciqcode();
            //判断当启用情况是否的时候把停用人的id传给base_year
            if (json.Value<string>("ENABLED")=="1")
            {
                stopman = "";
            }
            else
            {
                FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
                string userName = identity.Name;
                JObject json_user = Extension.Get_UserInfo(userName);
                stopman = (string) json_user.GetValue("ID");
            }

            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {

                //查询base_year的数据根据name
                DataSet baDataSet = bc.check_base_year(json);
                if (baDataSet.Tables[0].Rows.Count > 0)
                {
                    //当数据有重复时success返回值为4
                    response = "{\"success\":\"4\"}";
                }
                else
                {
                    //insert数据
                    int i = bc.insert_base_year(json);
                    response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }


            }
            else
            {

                //保存修改之前的信息
                string beforechanges = Session["beforechangeSession"].ToString();
                JObject beforechangesjson = (JObject)JsonConvert.DeserializeObject(beforechanges);
                //更新时查看规则名是否已经使用
                DataSet checkDataSet = bc.check_base_year_by_idandname(json);
                if (checkDataSet.Tables[0].Rows.Count > 0)
                {
                    response = "{\"success\":\"4\"}";
                }
                else
                {
                    //判断修改原因是否为空
                    if (!string.IsNullOrEmpty(json.Value<string>("REASON")))
                    {
                        string a = json.Value<string>("ID");
                        //点击修改更新base_year
                        int j = bc.update_base_year(json, stopman);
                        bc.insert_base_alterrecord(json, getChange(beforechangesjson, json));
                        response = "{\"success\":" + (j > 0 ? "true" : "false") + "}";
                    }
                    
                }

                
            }

            Response.Write(response);
            Response.End();
        }

        //获取维护人的姓名
        public string  Username()
        {
            string userName = "";
            FormsIdentity identity = User.Identity as FormsIdentity;
            if (identity == null)
            {
                return "";
            }
            return userName = identity.Name;
        }

        //保存修改记录
        public void change()
        {
            
            string beforechange = Request["formdata"];
            Session["beforechangeSession"] = beforechange;           
        }

        //修改内容
        private string getChange(JObject beforechangesjson,JObject afterJObject)
        {
            string str = "";
            if (beforechangesjson.Value<string>("NAME") != afterJObject.Value<string>("NAME"))
            {
                str = "规则名称：" + beforechangesjson.Value<string>("NAME") + "——>" + afterJObject.Value<string>("NAME") + "。";
            }

            if (beforechangesjson.Value<string>("STARTDATE") != afterJObject.Value<string>("STARTDATE"))
            {
                str += "开始时间：" + beforechangesjson.Value<string>("STARTDATE") + "——>" + afterJObject.Value<string>("STARTDATE") + "。";
            }
            if (beforechangesjson.Value<string>("ENDDATE") != afterJObject.Value<string>("ENDDATE"))
            {
                str += "停用时间：" + beforechangesjson.Value<string>("ENDDATE") + "——>" + afterJObject.Value<string>("ENDDATE") + "。";
            }
            return str;
        }


        //CIQ代码维护界面数据加载
        private void MaintainloadData()
        {
            string ID = Request["id"];
            string strWhere = string.Empty;
            Basic_ciqcode bc = new Basic_ciqcode();

            if (!string.IsNullOrEmpty(Request["CiqCode"]))
            {
                strWhere = strWhere + " and t1.ciq like '%" + Request["CiqCode"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ciqChineseCode"]))
            {
                strWhere = strWhere + " and t1.ciqname like '%" + Request["ciqChineseCode"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S2"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S2"] + "'";
            }
            DataTable dt = bc.MaintainloadData(ID, strWhere, "desc", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }


        //新增CIQ代码
        public void MaintainSave(string formdata)
        {
            string yearid = Request["ID"];
            //从前端获取值
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            
            Basic_ciqcode bc = new Basic_ciqcode();
            
            
            //返回的值
            string responseStr = "";
            if (json.Value<string>("ID")=="")
            {
                DataTable dt = bc.check_repeat_base_ciqcode(json, yearid);
                if (dt.Rows.Count > 0)
                {
                    //当返回是4的时候有重复值
                    responseStr = "{\"success\":\"4\"}";
                }
                else
                {
                    //insert数据到表base_ciqcode
                    int i = bc.insert_base_ciqcode(json, yearid);
                    responseStr = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }
            }
            else
            {
                DataTable dt1 = bc.update_check_repeat_base_ciqcode(json);
                if (dt1.Rows.Count>0)
                {
                    responseStr = "{\"success\":\"4\"}";
                }
                else
                {

                    DataTable getchanTable = bc.GetChangeDataTable(json);
                    string content = getchange(getchanTable,json);
                    int i = bc.update_base_ciqcode(json);
                    if (i>0)
                    {
                        bc.saveChangeBaseCiqCode(json, content);
                    }
                    responseStr = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }
            }
                                   
            Response.Write(responseStr);
            Response.End();
        }


        //保存修改之前的ciq数据
        public string getchange(DataTable dt,JObject json)
        {
            string str = "";
            if (dt.Rows[0]["ciq"] != json.Value<string>("CIQ"))
            {
                str += "CIQ代码：" + dt.Rows[0]["ciq"] + "——>" + json.Value<string>("CIQ") + "。";
            }
            if (dt.Rows[0]["ciqname"] != json.Value<string>("CIQNAME"))
            {
                str += "CIQ中文名：" + dt.Rows[0]["ciqname"] + "——>" + json.Value<string>("CIQNAME") + "。";
            }
            if (dt.Rows[0]["enabled"] != json.Value<string>("ENABLED"))
            {
                str += "是否启用：" + dt.Rows[0]["enabled"] + "——>" + json.Value<string>("ENABLED") + "。";
            }
            if (dt.Rows[0]["startdate"] != json.Value<string>("STARTDATE"))
            {
                str += "启用日期：" + dt.Rows[0]["startdate"] + "——>" + json.Value<string>("STARTDATE") + "。";
            }
            if (dt.Rows[0]["enddate"] != json.Value<string>("ENDDATE"))
            {
                str += "停用日期：" + dt.Rows[0]["enddate"] + "——>" + json.Value<string>("ENDDATE") + "。";
            }
            if (dt.Rows[0]["remark"] != json.Value<string>("REMARK"))
            {
                str += "备注：" + dt.Rows[0]["remark"] + "——>" + json.Value<string>("REMARK") + "。";
            }

            return str;
        }

        
        //导出
        public void export()
        {
            Basic_ciqcode bc = new Basic_ciqcode();
            string strWhere = String.Empty;
            string yearid = Request["id"];
            string combo_ENABLED_S2 = Request["combo_ENABLED_S2"];
            if (combo_ENABLED_S2 == "null")
            {
                combo_ENABLED_S2 = String.Empty;
            }
            if (!string.IsNullOrEmpty(Request["CiqCode"]))
            {
                strWhere = strWhere + " and t1.ciq like '%" + Request["CiqCode"] + "%'";
            }

            if (!string.IsNullOrEmpty(combo_ENABLED_S2))
            {
                strWhere = strWhere + " and t1.enabled='" + combo_ENABLED_S2 + "'";
            }

            if (!string.IsNullOrEmpty(Request["ciqChineseCode"]))
            {
                strWhere = strWhere + " and t1.ciqname like '%" + Request["ciqChineseCode"] + "%'";
            }

            DataTable dt = bc.exoprt_base_ciqcode(strWhere, yearid);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("CIQ代码");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("CIQ代码");
            row1.CreateCell(1).SetCellValue("CIQ中文名");
            row1.CreateCell(2).SetCellValue("代码库");
            row1.CreateCell(3).SetCellValue("维护人");
            row1.CreateCell(4).SetCellValue("停用人");
            row1.CreateCell(5).SetCellValue("启用时间");
            row1.CreateCell(6).SetCellValue("停用时间");
            row1.CreateCell(7).SetCellValue("维护时间");
            row1.CreateCell(8).SetCellValue("启用情况");
            row1.CreateCell(9).SetCellValue("备注");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CIQ"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["CIQNAME"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["YEARNAME"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CREATEMANNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["STOPMANNAME"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["STARTDATE"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["ENDDATE"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["CREATEDATE"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["ENABLED"].ToString()=="1"?"是":"否");
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["REMARK"].ToString());
                
            }
            try
            {
                // 输出Excel
                string filename = "CIQ代码.xls";
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
        public void ImportExcelData()
        {
            //获取yearid
            string yearid = Request["id"];
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
            Dictionary<int,List<int>> result = upload_base_company(newfile, fileName, action, json_formdata,yearid);

            List<int> successInts = result[1];
            List<int> errorlines = result[2];

            string errorStr = "";
            for (int i = 0; i < errorlines.Count; i++)
            {
                errorStr = errorStr + errorlines[i] + ",";
            }


            //返回失败信息
            string responseerrorlist = "";
            //返回成功信息
            string responsesuccesslist = "";

            
            if (errorlines.Count > 0)
            {
                responseerrorlist = "插入失败的行数为：" + errorStr;
            }

            if (successInts.Count > 0)
            {
                responsesuccesslist = "成功插入" + successInts[0] + "行!";
            }
            reponseresult = responsesuccesslist + responseerrorlist;

            string response = "{\"success\":\"" + reponseresult + "\"}";
            Response.Write(response);
            Response.End();
        }

        public Dictionary<int,List<int>> upload_base_company(string newfile, string fileName, string action, JObject json_formdata,string yearid)
        {
            Base_Company_Method bcm = new Base_Company_Method();
            DataTable dtExcel = bcm.GetExcelData_Table(Server.MapPath(newfile), 0);
            
            Basic_ciqcode  bc = new Basic_ciqcode();
            //判断是否有重复的ciq代码
            //excel中得到的数据是：CIQ代码，CIQ中文名，启用情况，备注

            List<string> insert_base_ciqcode = new List<string>();
            
            //记住成功插入的条数
            int countsuccess = 0;

            //记住失败的行数
            List<int> errorlines = new List<int>();

            //记住成功的个数
            List<int> successInts = new List<int>();

            //返回值
            Dictionary<int,List<int>> retundDictionary = new Dictionary<int, List<int>>();
            for (int i = 0; i < dtExcel.Rows.Count; i++)
            {

                for (int j = 0; j < dtExcel.Columns.Count; j++)
                {
                    insert_base_ciqcode.Add(dtExcel.Rows[i][j].ToString());
                }
                //ciq代码                                   //ciq中文名
                string ciq = insert_base_ciqcode[0];       string ciqname = insert_base_ciqcode[1];
                //启用情况                                                     //备注
                string ENABLED = insert_base_ciqcode[2] == "是" ? "1" : "0";  string remark = insert_base_ciqcode[3];

                if (bc.Before_import_check(ciq).Rows.Count > 0 || string.IsNullOrEmpty(ciq))
                {
                    errorlines.Add(i+2);
                }
                else
                {
                    bc.import_base_ciqcode(json_formdata,ciq,ciqname,ENABLED,remark,yearid);
                    countsuccess = countsuccess + 1;
                }

                insert_base_ciqcode.Clear();
            }

            successInts.Add(countsuccess);
            retundDictionary.Add(1,successInts);
            retundDictionary .Add(2,errorlines);
            return retundDictionary;

        }
    }
}