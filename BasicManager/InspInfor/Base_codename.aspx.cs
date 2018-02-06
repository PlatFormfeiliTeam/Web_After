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

namespace Web_After.BasicManager.InspInfor
{
    public partial class Base_codename : System.Web.UI.Page
    {
        IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
        int totalProperty = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string action = Request["action"];
                string table = Request["table"];
                iso.DateTimeFormat = "yyyy-MM-dd";
                switch (action)
                {
                    case "loadData":
                        loadData(table);
                        break;
                    case "save":
                        save(table,Request["formdata"]);
                        break;
                    case "export":
                        export(table);
                        break;
                    case "add":
                        ImportExcelData(table);
                        break;
                    
                }
            }
        }

        private void loadData(string table)
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["CODE_S"]))
            {
                strWhere = strWhere + " and t1.Code like '%" + Request["CODE_S"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["CNNAME_S"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["CNNAME_S"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.Base_codename bc = new Sql.Base_codename();
            DataTable dt = bc.LoaData(table,strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
            
        }


        private void save(string table,string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            //返回界面值
            string response = "";
            Sql.Base_codename bc = new Sql.Base_codename();
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                if (bc.check_repeat(table, json) > 0)
                {
                    response = "{\"success\":\"4\"}";
                }
                else
                {
                    int i = bc.insertTable(table, json);
                    if (i > 0)
                    {
                        response = "{\"success\":\"5\"}";
                    }
                }

            }
            else
            {
                if (bc.update_check_repeat(table,json) > 0)
                {

                    response = "{\"success\":\"4\"}";
                }
                else
                {
                    //获取修改之前的datatable
                    DataTable dt = bc.getBeforeChangeData(table, json);

                    int i = bc.updataTable(table, json);
                    if (i > 0)
                    {
                        //插入修改信息到表内
                        int j = bc.insertAlterRecordTable(dt, table, json);
                        if (j>0)
                        {
                            response = "{\"success\":\"5\"}";
                        }
                        
                    }
                }
            }
            
            
            Response.Write(response);
            Response.End();
            
        }

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

        private void export(string table)
        {
            string strWhere = string.Empty;
            string combo_ENABLED_S = Request["combo_ENABLED_S"];
            if (combo_ENABLED_S == "null")
            {
                combo_ENABLED_S = String.Empty;
            }
            if (!string.IsNullOrEmpty(Request["CODE_S"]))
            {
                strWhere = strWhere + " and t1.Code like '%" + Request["CODE_S"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["CNNAME_S"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["CNNAME_S"] + "%'";
            }
            if (!string.IsNullOrEmpty(combo_ENABLED_S))
            {
                strWhere = strWhere + " and t1.enabled='" + combo_ENABLED_S + "'";
            }

            strWhere = strWhere + " order by t1.code asc";
            Sql.Base_codename bc = new Sql.Base_codename();
            Switch_helper_Base_codename sc = new Switch_helper_Base_codename();
            List<string> ColumnName = sc.getColum(table);
            DataTable dt = bc.ExportTable(table, strWhere);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet(sc.getExcelName(table));
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);

            row1.CreateCell(0).SetCellValue(ColumnName[0]);
            row1.CreateCell(1).SetCellValue(ColumnName[1]);
           
            row1.CreateCell(2).SetCellValue("启用情况");
            row1.CreateCell(3).SetCellValue("启用时间");
            row1.CreateCell(4).SetCellValue("维护人");
            row1.CreateCell(5).SetCellValue("维护时间");
            row1.CreateCell(6).SetCellValue("停用人");
            row1.CreateCell(7).SetCellValue("停用时间");
            row1.CreateCell(8).SetCellValue("备注");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i][ColumnName[2]].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i][ColumnName[3]].ToString());                
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["STARTDATE"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CREATEMANNAME"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CREATEDATE"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["STOPMANNAME"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["ENDDATE"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["REMARK"].ToString());
            }
            try
            {
                // 输出Excel
                string filename = sc.getExcelName(table)+".xls";
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

        private void ImportExcelData(string table)
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
            Dictionary<int, List<int>> result = upload_base_company(newfile, fileName, action, json_formdata,table);

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
            JObject json_formdata,string table)
        {
            Base_Company_Method bcm = new Base_Company_Method();

            DataTable dtExcel = bcm.GetExcelData_Table(Server.MapPath(newfile), 0);
            List<string> stringList = new List<string>();

            //记住发生错误的行数
            List<int> errorlines = new List<int>();

            //记住插入成功的个数
            int count = 0;
            Sql.Base_codename bc = new Sql.Base_codename();
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
                //代码                           //名字
                string code = stringList[0];    string name = stringList[1];
                //是否启用                                         //备注
                string enabled = stringList[2] == "是" ? "1" : "0"; string remark = stringList[3];
                
                //启用日期
                string startdate = json_formdata.Value<string>("STARTDATE");
                //停用日期
                string enddate = json_formdata.Value<string>("ENDDATE");

                string formdata = "{\"CODE\":\""+code+"\",\"NAME\":\""+name+"\",\"ENABLED\":\""+enabled+"\",\"REMARK\":\""+remark+"\",\"STARTDATE\":\""+startdate+"\",\"ENDDATE\":\""+enddate+"\"}";
                
                JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
                if (bc.check_repeat(table, json) > 0 || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
                {
                    errorlines.Add(i + 2);
                }
                else
                {
                    bc.insertTable(table, json);
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