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
    public partial class FtpSetting : System.Web.UI.Page
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
                }
            }

        }



        private void loadData()
        {
            string strWhere = " where 1=1 ";
            if (!string.IsNullOrEmpty(Request["PROFILENAMECODE"]))
            {
                strWhere = strWhere + " and t1.profilename like '%" + Request["PROFILENAMECODE"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.FtpSetting bc = new Sql.FtpSetting();
            DataTable dt = bc.LoaData(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }

        public void save(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            Sql.FtpSetting bcsql = new Sql.FtpSetting();
            //禁用人
            string stopman = "";
            //返回重复结果
            string repeat = "";
            //返回前端的值
            string response = "";



            if (String.IsNullOrEmpty(json.Value<string>("ID")))
            {
                List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("PROFILENAME"));
                if (retunRepeat.Count > 0)
                {
                    repeat = "此FTP设置应存在，请检查";
                }
                else
                {
                    int i = bcsql.insert_Ftpsetting(json);
                    repeat = "5";
                }
            }
            else
            {
                List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("PROFILENAME"));
                if (retunRepeat.Count > 0)
                {
                    repeat = "此FTP设置应存在，请检查";
                }
                else
                {
                    DataTable dt = bcsql.LoadDataById(json.Value<string>("ID"));
                    int i = bcsql.update_Ftpsetting(json);
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
            string strWhere = " where 1=1  ";
            if (!string.IsNullOrEmpty(Request["PROFILENAMECODE"]))
            {
                strWhere = strWhere + " and t1.profilename like '%" + Request["PROFILENAMECODE"] + "%'";
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
            Sql.FtpSetting bc = new Sql.FtpSetting();

            DataTable dt = bc.export_rela_ftpsetting(strWhere);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("通道FTP配置");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("配置方案名称");
            row1.CreateCell(1).SetCellValue("FTP服务器URI");
            row1.CreateCell(2).SetCellValue("端口号");
            row1.CreateCell(3).SetCellValue("FTP用户名");
            row1.CreateCell(4).SetCellValue("启用情况");
            row1.CreateCell(5).SetCellValue("FTP密码");
            row1.CreateCell(6).SetCellValue("通道名称");
            row1.CreateCell(7).SetCellValue("发送文件类型");
            row1.CreateCell(8).SetCellValue("适用关区");
            row1.CreateCell(9).SetCellValue("申报类型");


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["PROFILENAME"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["URI"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["PORT"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["USERNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["PASSWORD"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["CHANNELNAME"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["FILETYPE"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["CUSTOMDISTRICTCODE"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["ENTRUSTTYPE"].ToString());
            }
            try
            {
                // 输出Excel
                string filename = "通道FTP配置.xls";
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