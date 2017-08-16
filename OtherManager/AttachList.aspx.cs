using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.OtherManager
{
    public partial class AttachList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"];
            int totalProperty = 0;
            string json = string.Empty; string sql = ""; DataTable dt; string where = "";

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            if (!string.IsNullOrEmpty(Request["ORDERCODE"]))
            {
                where += " and ORDERCODE like '%" + Request["ORDERCODE"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["SPLITSTATUS"]))
            {
                where += " and SPLITSTATUS = '" + Request["SPLITSTATUS"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["FILETYPE"]))
            {
                where += " and FILETYPE = '" + Request["FILETYPE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["START_DATE"]))
            {
                where += " and UPLOADTIME>=to_date('" + Request["START_DATE"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["END_DATE"]))
            {
                where += " and UPLOADTIME<=to_date('" + Request["END_DATE"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }

            switch (action)
            {
                case "loadattach":
                    sql = @"SELECT * FROM List_Attachment where ORDERCODE is not null and (abolishstatus=0 or abolishstatus is null) " + where;
                    sql = Extension.GetPageSql(sql, "UPLOADTIME", "desc", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
                    dt = DBMgr.GetDataTable(sql);
                    json = JsonConvert.SerializeObject(dt, iso);
                    Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
                    Response.End();
                    break;
                case "export":
                    sql = @"SELECT * FROM List_Attachment where ORDERCODE is not null and (abolishstatus=0 or abolishstatus is null) " + where + " order by UPLOADTIME desc";
                    dt = DBMgr.GetDataTable(sql);

                    //创建Excel文件的对象
                    NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                    //添加一个导出成功sheet
                    NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("文件信息");
                    NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                    row1.CreateCell(0).SetCellValue("业务编号"); row1.CreateCell(1).SetCellValue("页数"); row1.CreateCell(2).SetCellValue("上传时间");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                        rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["ORDERCODE"].ToString());
                        rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["FILEPAGES"].ToString());
                        rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["UPLOADTIME"].ToString());
                    }
                    // 输出Excel
                    string filename = "文件信息.xls";
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", Server.UrlEncode(filename)));
                    Response.Clear();

                    MemoryStream ms = new MemoryStream();
                    book.Write(ms);
                    Response.BinaryWrite(ms.GetBuffer());
                    Response.End();
                    break;
            }
        }
    }
}