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

namespace Web_After.BasicManager.InspInfor
{
    public partial class Base_Container : System.Web.UI.Page
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
                        save(Request["formdata"]);
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
            Sql.Base_Container bc = new Sql.Base_Container();
            DataTable dt = bc.LoaData(table, strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();

        }

        public void save(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            Sql.Base_Container bcsql = new Sql.Base_Container();
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
                    List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("CODE"), json.Value<string>("NAME"),
                        json.Value<string>("HSCODE"));

                    repeat = bcsql.Check_Repeat(retunRepeat);
                    if (repeat == "")
                    {
                        //insert数据向表base_company当是5时插入成功
                        bcsql.insert_base_container(json, stopman);
                        repeat = "5";

                    }
            }
            else
            {
                List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("CODE"),
                        json.Value<string>("NAME"),
                        json.Value<string>("HSCODE"));
                repeat = bcsql.Check_Repeat(retunRepeat);
                if (repeat == "")
                {
                    DataTable dt = bcsql.LoadDataById(json.Value<string>("ID"));
                    int i = bcsql.update_base_container(json, stopman);
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

        /// <summary>
        /// 导入excel数据
        /// </summary>
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


            Dictionary<int, List<int>> result = upload_base_company(newfile, fileName, action, json_formdata);
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

        public Dictionary<int, List<int>> upload_base_company(string newfile, string fileName, string action, JObject json_formdata)
        {
            Sql.Base_Container bc = new Sql.Base_Container();
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


                //集装箱规格编码                       
                string CODE = stringList[0];
                //集装箱规格名称                     
                string NAME = stringList[1];
                //企业英文名称                 
                string HSCODE = stringList[2];
                //备注
                string REMARK = stringList[3];
                string ENABLED = stringList[4] == "是" ? "1" : "0";

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
                List<int> inlist = bc.CheckRepeat("", CODE,NAME, HSCODE);
                string check_repeat = bc.Check_Repeat(inlist);

                if (check_repeat == "")
                {
                    bc.insert_base_container_excel(CODE, NAME, HSCODE, ENABLED, REMARK, stopman, STARTDATE, ENDDATE);
                    count = count + 1;
                }
                else
                {
                    repeatListerror.Add(i + 2);
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

            string strWhere = string.Empty;
            string combo_ENABLED_S2 = Request["combo_ENABLED_S"];
            if (combo_ENABLED_S2 == "null")
            {
                combo_ENABLED_S2 = String.Empty;
            }
            if (!string.IsNullOrEmpty(Request["CODE_S"]))
            {
                strWhere = strWhere + " and t1.code like '%" + Request["CODE_S"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["CNNAME_S"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["CNNAME_S"] + "%'";
            }
            if (!string.IsNullOrEmpty(combo_ENABLED_S2))
            {
                strWhere = strWhere + " and t1.enabled='" + combo_ENABLED_S2 + "'";
            }
            Sql.Base_Container bc = new Sql.Base_Container();
            DataTable dt = bc.export_base_container(strWhere);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("集装箱规格");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("集装箱规格代码");
            row1.CreateCell(1).SetCellValue("集装箱规格名称");
            row1.CreateCell(2).SetCellValue("集装箱HS编码");
            row1.CreateCell(3).SetCellValue("集装箱商品名称");
            row1.CreateCell(4).SetCellValue("检验检验类别");
            row1.CreateCell(5).SetCellValue("海关监管条件");
            row1.CreateCell(6).SetCellValue("启用情况");
            row1.CreateCell(7).SetCellValue("启用时间");
            row1.CreateCell(8).SetCellValue("维护人");
            row1.CreateCell(9).SetCellValue("维护时间");
            row1.CreateCell(10).SetCellValue("停用人");
            row1.CreateCell(11).SetCellValue("停用时间");
            row1.CreateCell(12).SetCellValue("备注");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["NAME"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["HSCODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["HSNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["INSPECTION"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["DECLARATION"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["STARTDATE"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["CREATEMANNAME"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["CREATEDATE"].ToString());
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["STOPMANNAME"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["ENDDATE"].ToString());
                rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["REMARK"].ToString());
            }
            try
            {
                // 输出Excel
                string filename = "集装箱规格.xls";
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