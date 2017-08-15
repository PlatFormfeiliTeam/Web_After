using Web_After.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_After
{
    public partial class PdfTab : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ordercode = Request["ordercode"];
            string action = Request["action"];
            string json = "";
            string sql = "";
            DataTable dt;
            switch (action)
            {
                case "load":
                    //取出该订单下所有上传的pdf文件 先按类型排序 再按文件上传时间排序
                    //20161027 去掉条件：t.confirmstatus=1 and 
                    sql = @"select t.ID,t.FILENAME,t.FILETYPE,f.filetypename from list_attachment t  
                          left join sys_filetype f on t.filetype=f.filetypeid
                          where lower(t.FILESUFFIX)='pdf' and instr(ordercode,'" + ordercode + "')>0 order by t.FILETYPE asc";
                    dt = DBMgr.GetDataTable(sql);
                    sql = @"select t.ID,t.SOURCEFILENAME,t.FILETYPEID,f.FILETYPENAME from list_attachmentdetail t  
                          left join sys_filetype f on t.FILETYPEID=f.filetypeid
                          where instr(ordercode,'" + ordercode + "')>0 order by t.FILETYPEID asc";
                    DataTable dt_detail = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt_detail.Rows)
                        {
                            DataRow nr = dt.NewRow();
                            nr["ID"] = dr["ID"];
                            nr["FILENAME"] = "/" + dr["SOURCEFILENAME"];
                            nr["FILETYPE"] = dr["FILETYPEID"];
                            nr["FILETYPENAME"] = dr["FILETYPENAME"];
                            dt.Rows.Add(nr);
                        }
                        json = JsonConvert.SerializeObject(dt);
                        Response.Write("{\"success\":true,\"rows\":" + json + "}");
                    }
                    else
                    {
                        Response.Write("{\"success\":false}");
                    }
                    Response.End();
                    break;
            }
        }
    }
}