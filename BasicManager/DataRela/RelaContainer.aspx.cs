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
    public partial class RelaContainer : System.Web.UI.Page
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
                        export();
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
            string CONTAINERSIZE = "[]";
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')'  as NAME FROM base_containersize where CODE is not null and enabled=1";
            CONTAINERSIZE = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
            string CONTAINERTYPE = "[]";
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')'  as NAME FROM base_containertype where CODE is not null and enabled=1";
            CONTAINERTYPE = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
            string CONTAINERSTANDARD = "[]";
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')'  as NAME FROM base_containerstandard where CODE is not null and enabled=1";
            CONTAINERSTANDARD = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            Response.Write("{CONTAINERSIZE:" + CONTAINERSIZE + ",CONTAINERTYPE:" + CONTAINERTYPE + ",CONTAINERSTANDARD:" + CONTAINERSTANDARD + "}");
            Response.End();
        }

        private void loadData()
        {
            string strWhere = " where 1=1 ";
            if (!string.IsNullOrEmpty(Request["SEARCHCONTAINERTYPE"]))
            {
                strWhere = strWhere + " and t5.name like '%" + Request["SEARCHCONTAINERTYPE"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["SEARCHFORTNAME"]))
            {
                strWhere = strWhere + " and t1.FormatName like '%" + Request["SEARCHFORTNAME"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.RelaContainer bc = new Sql.RelaContainer();
            DataTable dt = bc.LoaData(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
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

        public void save(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            Sql.RelaContainer bcsql = new Sql.RelaContainer();
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

            //插入
            if (String.IsNullOrEmpty(json.Value<string>("ID")))
            {
                repeat = bcsql.CheckRepeat(formdata);
                if (repeat == "")
                {
                    //insert数据向表base_company当是5时插入成功
                    bcsql.insert_rela_container(json, stopman);
                    repeat = "5";
                }
            }
            else
            {
                //更新
                repeat = bcsql.CheckRepeat(formdata);
                if (repeat == "")
                {
                    //insert数据向表base_company当是5时插入成功
                    DataTable dt = bcsql.LoadDataById(json.Value<string>("ID"));
                    int i = bcsql.update_rela_container(json, stopman);
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
            //postedFile.SaveAs(Server.MapPath(newfile));

            //本机不加Server.MapPath
            postedFile.SaveAs(newfile);

            //npoi的方法
            //DataTable dt = NPOIHelper.RenderDataTableFromExcel(newfile, ".xls", 0, 0);
            //int bCount = dt.Rows.Count;
            //string a = dt.Rows[0][0].ToString();


            Dictionary<int, List<int>> result = upload_RelaContainer(newfile, fileName, action, json_formdata);
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
        public Dictionary<int, List<int>> upload_RelaContainer(string newfile, string fileName, string action, JObject json_formdata)
        {
            Sql.RelaContainer bc = new Sql.RelaContainer();
            //DataTable dtExcel = GetExcelData_Table(Server.MapPath(newfile), 0);
            DataTable dtExcel = GetExcelData_Table(newfile, 0);
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
                //       
                string CONTAINERSIZE = stringList[0];
                //              
                string CONTAINERTYPE = stringList[1];

                string FORMATCODE = stringList[2];

                string FORMATNAME = stringList[3];

                string CONTAINERHS = stringList[4];

                string REMARK = stringList[5];

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
                string check_repeat = bc.CheckRepeat(CONTAINERSIZE, CONTAINERTYPE, FORMATCODE);

                if (check_repeat == "")
                {
                    bc.insert_rela_container(CONTAINERSIZE, CONTAINERTYPE, FORMATCODE, FORMATNAME, CONTAINERHS, ENABLED, REMARK, stopman, STARTDATE, ENDDATE);
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
            string strWhere = " where 1=1 ";
            string combo_ENABLED_S2 = Request["combo_ENABLED_S"];
            if (combo_ENABLED_S2 == "null")
            {
                combo_ENABLED_S2 = String.Empty;
            }
            if (!string.IsNullOrEmpty(Request["SEARCHCONTAINERTYPE"]))
            {
                strWhere = strWhere + " and t5.name like '%" + Request["SEARCHCONTAINERTYPE"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["SEARCHFORTNAME"]))
            {
                strWhere = strWhere + " and t1.FormatName like '%" + Request["SEARCHFORTNAME"] + "%'";
            }

            if (!string.IsNullOrEmpty(combo_ENABLED_S2))
            {
                strWhere = strWhere + " and t1.enabled='" + combo_ENABLED_S2 + "'";
            }
            Sql.RelaContainer bc = new Sql.RelaContainer();

            DataTable dt = bc.export_rela_container(strWhere);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("集装箱对应关系");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("集装箱尺寸");
            row1.CreateCell(1).SetCellValue("集装箱类型");
            row1.CreateCell(2).SetCellValue("集装箱规格");
            row1.CreateCell(3).SetCellValue("集装箱规格名称");
            row1.CreateCell(4).SetCellValue("集装箱HS编码");
            row1.CreateCell(5).SetCellValue("启用情况");
            row1.CreateCell(6).SetCellValue("启用时间");
            row1.CreateCell(7).SetCellValue("维护人");
            row1.CreateCell(8).SetCellValue("维护时间");
            row1.CreateCell(9).SetCellValue("停用人");
            row1.CreateCell(10).SetCellValue("停用时间");
            row1.CreateCell(11).SetCellValue("备注");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CONTAINERSIZE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["CONTAINERTYPE"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["FORMATCODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["FORMATNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CONTAINERHS"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["STARTDATE"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["CREATEMANNAME"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["CREATEDATE"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["STOPMANNAME"].ToString());
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["ENDDATE"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["REMARK"].ToString());
            }
            try
            {
                // 输出Excel
                string filename = "集装箱对应关系.xls";
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


    }

}