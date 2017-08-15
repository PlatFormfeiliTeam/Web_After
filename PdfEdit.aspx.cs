using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;
using System.Web.Services;
using iTextSharp.text.pdf;
using StackExchange.Redis;
using System.IO;
namespace Web_After
{
    public partial class PdfEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"];
            string filetype = Request["filetype"];
            string ordercode = Request["ordercode"];
            string fileid = Request["fileid"];
            string userid = Request["userid"];
            string sql = "", json = "";
            string splitfilename = ""; string filestatus = "";
            DataTable dt;
            PdfReader pdfReader;
            IDatabase db = SeRedis.redis.GetDatabase();
            FileInfo fi;
            switch (action)
            {
                case "loadform":
                    string result = "", result_file = "";

                    sql = @"SELECT CODE,BUSITYPE,BUSIUNITNAME,FILESTATUS,ASSOCIATENO,(select name from cusdoc.sys_busitype where code=a.busitype)  as BUSITYPENAME 
                            ,(case FILESTATUS when 1 then '已拆分' else '未拆分' end) as FILESTATUSDESC
                            FROM list_order a WHERE CODE = '" + ordercode + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count == 1)
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0]["ASSOCIATENO"].ToString()))
                        {
                            sql = @"SELECT  CODE,BUSITYPE,BUSIUNITNAME,FILESTATUS,ASSOCIATENO,(select name from cusdoc.sys_busitype where code=a.busitype)  as BUSITYPENAME 
                            ,(case FILESTATUS when 1 then '已拆分' else '未拆分' end) as FILESTATUSDESC 
                            FROM list_order a WHERE CODE != '" + ordercode + "' and ASSOCIATENO='" + dt.Rows[0]["ASSOCIATENO"] + "'";
                            DataTable dt_gl = DBMgr.GetDataTable(sql);
                            if (dt_gl.Rows.Count == 1)
                            {
                                dt.Rows[0]["ASSOCIATENO"] = dt_gl.Rows[0]["CODE"];
                            }
                        }

                    }
                    result = JsonConvert.SerializeObject(dt).TrimStart('[').TrimEnd(']');

                    sql = "select * from list_attachment where ordercode='" + ordercode + "' and filetype=44 order by uploadtime asc";
                    DataTable dt_file = DBMgr.GetDataTable(sql);
                    result_file = JsonConvert.SerializeObject(dt_file);

                    Response.Write("{\"formdata\":" + result + ",\"filedata\":" + result_file + "}");
                    Response.End();
                    break;
                case "loadpdf":
                    sql = "select * from list_attachment where id='" + fileid + "'";
                    dt = DBMgr.GetDataTable(sql);
                    splitfilename = dt.Rows[0]["FILENAME"].ToString();filestatus = dt.Rows[0]["SPLITSTATUS"].ToString();//0 未拆分  1 已拆分 
                    if (filestatus == "" || filestatus == "0")  //如果未拆分,初始化拆分明细界面内容并写入缓存
                    {
                        //插入待压缩文件的记录【新的压缩方式】 因为工具端上传的文件是没有压缩日志的
                        sql = "select t.* from pdfshrinklog t where t.attachmentid='" + fileid + "'";
                        dt = DBMgr.GetDataTable(sql);
                        if (dt.Rows.Count == 0)
                        {
                            sql = "insert into pdfshrinklog (id,attachmentid) values (pdfshrinklog_id.nextval,'" + fileid + "')";
                            DBMgr.ExecuteNonQuery(sql);
                        }
                        pdfReader = new PdfReader(@"d:\ftpserver\" + splitfilename);
                        int totalPages = pdfReader.NumberOfPages;
                        sql = "select * from sys_filetype where parentfiletypeid=44  order by sortindex asc";//取该文件类型下面所有的子类型
                        dt = DBMgr.GetDataTable(sql);
                        //构建页码表格数据
                        DataTable dt2 = new DataTable();
                        DataColumn dc = new DataColumn("ID");
                        dt2.Columns.Add(dc);
                        for (int k = 0; k < dt.Rows.Count; k++)
                        {
                            dc = new DataColumn("c-" + dt.Rows[k]["FILETYPEID"] + "|" + dt.Rows[k]["FILETYPENAME"]);
                            dt2.Columns.Add(dc);
                        }
                        for (int i = 1; i <= totalPages; i++)
                        {
                            DataRow dr = dt2.NewRow();
                            dr["ID"] = i;
                            dt2.Rows.Add(dr);
                        }
                        json = JsonConvert.SerializeObject(dt2);
                        //订单文件拆分明细保存至缓存数据库 并设置过期时间是24小时
                        //db.StringSet("new:" + ordercode + ":" + fileid + ":splitdetail", json, TimeSpan.FromMinutes(1440));
                    }
                    else//如果已拆分 直接读取缓存数据库
                    {
                        //if (db.KeyExists("new:" + ordercode + ":" + fileid + ":splitdetail"))
                        //{
                        //    json = db.StringGet("new:" + ordercode + ":" + fileid + ":splitdetail");
                        //}
                        //else
                        //{
                            pdfReader = new PdfReader(@"d:\ftpserver\" + splitfilename);
                            int totalPages = pdfReader.NumberOfPages;
                            sql = "select * from sys_filetype where parentfiletypeid=44 order by sortindex asc";//取该文件类型下面所有的子类型
                            dt = DBMgr.GetDataTable(sql);
                            //构建页码表格数据
                            DataTable dt2 = new DataTable();
                            DataColumn dc = new DataColumn("ID");
                            dt2.Columns.Add(dc);
                            for (int k = 0; k < dt.Rows.Count; k++)
                            {
                                dc = new DataColumn("c-" + dt.Rows[k]["FILETYPEID"] + "|" + dt.Rows[k]["FILETYPENAME"]);
                                dt2.Columns.Add(dc);
                            }
                            for (int i = 1; i <= totalPages; i++)
                            {
                                DataRow dr = dt2.NewRow();
                                dr["ID"] = i;
                                foreach (DataRow tmp in dt.Rows)//一个子类型是一列  取每一列的值
                                {
                                    sql = "select pages from list_attachmentdetail where ordercode='" + ordercode + "' and attachmentid=" + fileid + " and filetypeid=" + tmp["FILETYPEID"];
                                    DataTable sub_dt = DBMgr.GetDataTable(sql);
                                    if (sub_dt.Rows.Count > 0)
                                    {
                                        string[] tmparray = sub_dt.Rows[0]["PAGES"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        if (tmparray.Contains<string>(i + ""))
                                        {
                                            dr["c-" + tmp["FILETYPEID"] + "|" + tmp["FILETYPENAME"]] = "√";
                                        }
                                        else
                                        {
                                            dr["c-" + tmp["FILETYPEID"] + "|" + tmp["FILETYPENAME"]] = "";
                                        }
                                    }
                                }
                                dt2.Rows.Add(dr);
                            }
                            json = JsonConvert.SerializeObject(dt2);
                            //db.StringSet("new:" + ordercode + ":" + fileid + ":splitdetail", json, TimeSpan.FromMinutes(1440));
                        //}
                    }
                    //如果是已经拆分好的 需要调出所有拆分好的文件类型 filetype:'" + filetypename + "'
                    sql = @"select a.id,a.filetypeid,b.filetypename from LIST_ATTACHMENTDETAIL a left join sys_filetype
                          b on a.filetypeid=b.filetypeid where a.attachmentid='" + fileid + "' order by b.sortindex asc";
                    dt = DBMgr.GetDataTable(sql);
                    string json_type = JsonConvert.SerializeObject(dt);
                    Response.Write("{\"success\":true,\"src\":\"" + splitfilename + "\",\"rows\":" + json + ",\"fileid\":" + fileid + ",\"filestatus\":'" + filestatus + "',\"result\":" + json_type + "}");
                    Response.End();
                    break;
                case "loadfile":
                    sql = "select * from list_attachmentdetail where id='" + fileid + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        Response.Write("{\"success\":true,\"src\":\"" + dt.Rows[0]["SOURCEFILENAME"] + "\"}");
                        Response.End();
                    }
                    break;
                case "cancelsplit":
                    //删除文件明细
                    sql = "select * from list_attachmentdetail where ordercode='" + ordercode + "' and attachmentid=" + fileid;
                    dt = DBMgr.GetDataTable(sql);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (File.Exists(@"d:/ftpserver/" + dt.Rows[i]["SOURCEFILENAME"]))
                        {
                            File.Delete(@"d:/ftpserver/" + dt.Rows[i]["SOURCEFILENAME"]);
                        }
                        sql = "delete from list_attachmentdetail where id=" + dt.Rows[i]["ID"];
                        DBMgr.ExecuteNonQuery(sql);
                    }
                    sql = "update LIST_ATTACHMENT set SPLITSTATUS=0 where id=" + fileid;
                    DBMgr.ExecuteNonQuery(sql);
                    //20160922赵艳提出 拆分完，需要更新订单表的 拆分人和时间,和文件状态
                    sql = "update LIST_ORDER set FILESTATUS=0,FILESPLITEUSERNAME=null,FILESPLITEUSERID=null,FILESPLITTIME=null where code='" + ordercode + "'";
                    DBMgr.ExecuteNonQuery(sql);
                    //db.KeyDelete("new:" + ordercode + ":" + fileid + ":splitdetail");
                    Response.Write("{\"success\":true}");
                    Response.End();
                    break;
            }
        }



    }
}