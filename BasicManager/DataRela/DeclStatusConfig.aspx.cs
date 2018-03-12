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
    public partial class DeclStatusConfig : System.Web.UI.Page
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
                        //ImportExcelData();
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
            string DECLSTATUS = "[]";
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')'  as NAME FROM base_declstatus where CODE is not null and enabled=1";
            DECLSTATUS = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            Response.Write("{DECLSTATUS:" + DECLSTATUS + "}");
            Response.End();
        }

        private void loadData()
        {
            string strWhere = " where 1=1 ";
            if (!string.IsNullOrEmpty(Request["STATUSCODE"]))
            {
                strWhere = strWhere + " and t1.code like '%" + Request["STATUSCODE"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["STATUSNAME"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["STATUSNAME"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.DeclStatusConfig bc = new Sql.DeclStatusConfig();
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
            Sql.DeclStatusConfig bcsql = new Sql.DeclStatusConfig();
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
                List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("CODE"));
                if (retunRepeat.Count > 0)
                {
                    repeat = "相同状态代码已存在，请检查";
                }
                else
                {
                    int i = bcsql.insert_statusconfig(json, stopman);
                    repeat = "5";
                }
            }
            else
            {
                List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("CODE"));
                if (retunRepeat.Count > 0)
                {
                    repeat = "相同状态代码已存在，请检查";
                }
                else
                {
                    DataTable dt = bcsql.LoadDataById(json.Value<string>("ID"));
                    int i = bcsql.update_statusconfig(json, stopman);
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


        public void export()
        {
            string strWhere = " where 1=1 ";
            if (!string.IsNullOrEmpty(Request["STATUSCODE"]))
            {
                strWhere = strWhere + " and t1.code like '%" + Request["STATUSCODE"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["STATUSNAME"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["STATUSNAME"] + "%'";
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
            Sql.DeclStatusConfig bc = new Sql.DeclStatusConfig();

            DataTable dt = bc.export_rela_declstatus(strWhere);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("回执状态");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("回执状态码");
            row1.CreateCell(1).SetCellValue("回执状态名称");
            row1.CreateCell(2).SetCellValue("业务状态");
            row1.CreateCell(3).SetCellValue("所属类型");
            row1.CreateCell(4).SetCellValue("显示描述");
            row1.CreateCell(5).SetCellValue("序号");
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
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["BUSISTATUSNAME"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["TYPECLASS"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["DESCRIPTION"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["ORDERNO"].ToString());
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
                string filename = "回执状态.xls";
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