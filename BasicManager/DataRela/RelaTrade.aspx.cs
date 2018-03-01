using Aspose.Cells;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.BasicManager.DataRela
{
    public partial class RelaTrade : System.Web.UI.Page
    {
        IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
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
                        loadData();
                        break;
                    case "save":
                        save(Request["formdata"]);
                        break;
                    case "export":
                        //export();
                        break;
                    case "add":
                        ImportExcelData();
                        break;
                    case "Ini_Base_Data":
                        Ini_Base_Data();
                        break;
                }
            }
        }

        public void Ini_Base_Data()
        {
            string sql = "";
            string DECLTRADE = "[]";//报关
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')'  as NAME FROM base_decltradeway where CODE is not null and enabled=1";
            DECLTRADE = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            string INSPTRADE = "[]";//报检
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')' as NAME FROM base_tradeway where CODE is not null and enabled=1";
            INSPTRADE = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            Response.Write("{DECLTRADE:" + DECLTRADE + ",INSPTRADE:" + INSPTRADE + "}");
            Response.End();
        }


        private void loadData()
        {
            string strWhere = " where 1=1 ";
            if (!string.IsNullOrEmpty(Request["DECLTRADECODE"]))
            {
                strWhere = strWhere + " and t1.decltrade like '%" + Request["DECLTRADECODE"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["DECLTRADENAME"]))
            {
                strWhere = strWhere + " and t2.name like '%" + Request["DECLTRADENAME"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.RelaTrade bc = new Sql.RelaTrade();
            DataTable dt = bc.LoaData(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }

        public void save(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            Sql.RelaTrade bcsql = new Sql.RelaTrade();
            //禁用人
            string stopman = "";
            //返回重复结果
            string repeat = "";
            //返回前端的值
            string response = "";

            if (json.Value<string>("ENABLED") == "1")
            {
                stopman = "";
            }
            else
            {
                FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
                string userName = identity.Name;
                JObject json_user = Extension.Get_UserInfo(userName);
                stopman = (string)json_user.GetValue("ID");
            }

            if (String.IsNullOrEmpty(json.Value<string>("ID")))
            {
                List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("DECLTRADE"), json.Value<string>("INSPTRADE"));
                if (retunRepeat.Count > 0)
                {
                    repeat = "此报关报检贸易方式已经有对应关系存在，请检查";
                }
                else
                {
                    int i = bcsql.insert_relaTrade(json, stopman);
                    repeat = "5";
                }
            }
            else
            {
                List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("DECLCOUNTRY"), json.Value<string>("INSPCOUNTRY"));
                if (retunRepeat.Count > 0)
                {
                    repeat = "此报关报检贸易方式已经有对应关系存在，请检查";
                }
                else
                {
                    DataTable dt = bcsql.LoadDataById(json.Value<string>("ID"));
                    int i = bcsql.update_relaTrade(json, stopman);
                    if (i > 0)
                    {
                        bcsql.insert_base_alterrecord(json, dt);
                    }
                    repeat = "5";
                }
            }

            response = "{\"success\":\"" + repeat + "\"}";

            Response.Write(response);
            Response.End();
        }

        public void ImportExcelData()
        {
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

            //本机不加Server.MapPath
            //postedFile.SaveAs(newfile);

            //npoi的方法
            //DataTable dt = NPOIHelper.RenderDataTableFromExcel(newfile, ".xls", 0, 0);
            //int bCount = dt.Rows.Count;
            //string a = dt.Rows[0][0].ToString();


            Dictionary<int, List<int>> result = upload_RelaCountry(newfile, fileName, action, json_formdata);
            //成功的条数
            List<int> success = result[1];
            //失败列
            List<int> error = result[2];
            string errorcount = "";
            for (int i = 0; i < error.Count; i++)
            {
                errorcount = errorcount + error[i] + ",";
            }

            //返回失败信息
            string responseerrorlist = "";
            //返回成功信息
            string responsesuccesslist = "";
            if (error.Count > 0)
            {
                responseerrorlist = "插入失败的行数为：" + errorcount;
            }

            if (success.Count > 0)
            {
                responsesuccesslist = "成功插入" + success[0] + "行!";
            }
            reponseresult = responsesuccesslist + responseerrorlist;

            //失败的条数
            //********************************************************************

            string response = "{\"success\":\"" + reponseresult + "\"}";
            Response.Write(response);
            Response.End();
        }

        public Dictionary<int, List<int>> upload_RelaCountry(string newfile, string fileName, string action, JObject json_formdata)
        {
            Sql.RelaTrade bc = new Sql.RelaTrade();
            DataTable dtExcel = GetExcelData_Table(Server.MapPath(newfile), 0);
            //DataTable dtExcel = GetExcelData_Table(newfile, 0);
            List<string> stringList = new List<string>();
            //停用人
            string stopman = "";

            //存放成功信息
            List<int> repeatListsuccess = new List<int>();
            //存放失败条数
            List<int> repeatListerror = new List<int>();
            //记住insert成功的条数
            int count = 0;
            //返回信息
            Dictionary<int, List<int>> dcInts = new Dictionary<int, List<int>>();

            for (int i = 0; i < dtExcel.Rows.Count; i++)
            {

                for (int j = 0; j < dtExcel.Columns.Count; j++)
                {


                    stringList.Add(dtExcel.Rows[i][j].ToString());


                }
                //报关         
                string DECLTRADE = stringList[0];
                //报检          
                string INSPTRADE = stringList[2];

                string REMARK = stringList[4];
                //string ENABLED = stringList[4] == "是" ? "1" : "0";
                string ENABLED = "1";
                //维护人
                string CREATEMANNAME = json_formdata.Value<string>("CREATEMANNAME");
                //启用时间
                string STARTDATE = json_formdata.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json_formdata.Value<string>("STARTDATE");

                //停用日期
                string ENDDATE = json_formdata.Value<string>("ENDDATE") == ""
                    ? DateTime.MaxValue.ToShortDateString()
                    : json_formdata.Value<string>("ENDDATE");

                if (ENABLED == "1")
                {
                    stopman = "";
                }
                else
                {
                    FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
                    string userName = identity.Name;
                    JObject json_user = Extension.Get_UserInfo(userName);
                    stopman = (string)json_user.GetValue("ID");
                }
                //导入判断条件
                List<int> inlist = bc.CheckRepeat("", DECLTRADE, INSPTRADE);

                if (inlist.Count > 0)
                {
                    repeatListerror.Add(i + 2);

                }
                else
                {
                    bc.insert_rela_trade_excel(DECLTRADE, INSPTRADE, ENABLED, REMARK, stopman, STARTDATE, ENDDATE);
                    count = count + 1;
                }

                //清除
                stringList.Clear();

            }
            repeatListsuccess.Add(count);
            dcInts.Add(1, repeatListsuccess);
            dcInts.Add(2, repeatListerror);
            return dcInts;
        }


        public DataTable GetExcelData_Table(string filePath, int sheetPoint)
        {
            Workbook book = new Workbook(filePath);
            //book.Open(filePath);
            Worksheet sheet = book.Worksheets[sheetPoint];
            Cells cells = sheet.Cells;
            DataTable dt_Import = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);//获取excel中的数据保存到一个datatable中
            return dt_Import;

        }

        public void export()
        {
            string strWhere = " where 1=1 ";
            if (!string.IsNullOrEmpty(Request["DECLTRADE"]))
            {
                strWhere = strWhere + " and t1.decltrade like '%" + Request["DECLTRADE"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["DECLTRADENAME"]))
            {
                strWhere = strWhere + " and t2.name like '%" + Request["DECLTRADENAME"] + "%'";
            }
            string combo_ENABLED_S2 = Request["combo_ENABLED_S"];
            if (combo_ENABLED_S2 == "null")
            {
                combo_ENABLED_S2 = String.Empty;
            }

            if (!string.IsNullOrEmpty(combo_ENABLED_S2))
            {
                strWhere = strWhere + " and t1.enabled='" + combo_ENABLED_S2 + "'";
            }
            Sql.RelaTrade bc = new Sql.RelaTrade();

            DataTable dt = bc.export_rela_trade(strWhere);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("贸易方式对应关系");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("报关贸易方式代码");
            row1.CreateCell(1).SetCellValue("报关贸易方式名称");
            row1.CreateCell(2).SetCellValue("报检贸易方式代码");
            row1.CreateCell(3).SetCellValue("报检贸易方式名称");
            row1.CreateCell(4).SetCellValue("启用情况");
            row1.CreateCell(5).SetCellValue("启用时间");
            row1.CreateCell(6).SetCellValue("维护人");
            row1.CreateCell(7).SetCellValue("维护时间");
            row1.CreateCell(8).SetCellValue("停用人");
            row1.CreateCell(9).SetCellValue("停用时间");
            row1.CreateCell(10).SetCellValue("备注");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["DECLTRADE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["DECLTRADENAME"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["INSPTRADE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["INSPTRADENAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["STARTDATE"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["CREATEMANNAME"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["CREATEDATE"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["STOPMANNAME"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["ENDDATE"].ToString());
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["REMARK"].ToString());
            }
            try
            {
                // 输出Excel
                string filename = "贸易方式对应关系.xls";
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
    }
}