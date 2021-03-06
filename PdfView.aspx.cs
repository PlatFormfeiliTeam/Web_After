﻿using Web_After.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_After
{
    public partial class PdfView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"];
            string filetype = Request["filetype"];
            string ordercode = Request["ordercode"];
            string fileid = Request["fileid"];
            string userid = Request["userid"] == "null" ? "" : Request["userid"];            
            string json = "";
            string sql = "";
            DataTable dt;
            PdfReader pdfReader;
            IDatabase db = SeRedis.redis.GetDatabase();
            FileInfo fi;

            string username = ""; 
            DataTable dt_user = DBMgr.GetDataTable("select * from Sys_User where ID='" + userid + "'");
            if (dt_user.Rows.Count>0)
            {
                username = dt_user.Rows[0]["REALNAME"] + "";
            }

            switch (action)
            {
                case "merge":
                    string splitfilename = "";
                    string filestatus = "";
                    string fileids = Request["fileids"].Replace("[", "").Replace("]", "");
                    string[] fileidarray = fileids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    IList<string> pathlist = new List<string>();
                    //如果传输到这个页面的比如订单文件大于等于两个，需要将这文件合并后再输出 
                    if (fileidarray.Length > 1)
                    {
                        for (int i = 0; i < fileidarray.Length; i++)
                        {
                            sql = "select * from list_attachment where ID=" + fileidarray[i];
                            dt = DBMgr.GetDataTable(sql);
                            if (dt.Rows.Count > 0)
                            {
                                if (File.Exists(@"d:\ftpserver\" + dt.Rows[0]["FILENAME"] + ""))
                                {
                                    pathlist.Add(dt.Rows[0]["FILENAME"] + "");
                                }
                            }
                        }
                        string orifilename = Guid.NewGuid().ToString();
                        splitfilename = "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/" + orifilename + ".pdf";
                        MergePDFFiles(pathlist, @"d:\ftpserver\" + splitfilename);
                        sql = @"insert into list_attachment (id,FILENAME,ORIGINALNAME,UPLOADTIME,FILETYPE,ORDERCODE,FILESUFFIX,uploaduserid,ISUPLOAD)
                        values (list_attachment_id.nextval,'{0}','{1}',sysdate,'{2}','{3}','{4}','{5}','1')";
                        sql = string.Format(sql, splitfilename, orifilename + ".pdf", 44, ordercode, "pdf", userid);
                        DBMgr.ExecuteNonQuery(sql);
                        //合并完成后数据库删除原有文件，插入新的文件记录"/" + ordercode
                        try
                        {
                            for (int k = 0; k < fileidarray.Length; k++)
                            {
                                sql = "delete from list_attachment where id= '" + fileidarray[k] + "'";
                                DBMgr.ExecuteNonQuery(sql);
                            }
                            for (int j = 0; j < pathlist.Count; j++)
                            {
                                File.Delete(@"d:\ftpserver\" + pathlist[j]);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    Response.Write("{success:true}");
                    Response.End();
                    break;
                case "loadpdf":
                    sql = "select * from list_attachment where id='" + Request["fileid"] + "'";
                    dt = DBMgr.GetDataTable(sql);
                    splitfilename = dt.Rows[0]["FILENAME"] + "";
                    fileid = Request["fileid"];
                    filestatus = dt.Rows[0]["SPLITSTATUS"] + "";//0 未拆分  1 已拆分 
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
                        pdfReader.Close(); pdfReader.Dispose();
                        sql = "select * from sys_filetype where parentfiletypeid=44  order by sortindex asc";//取该文件类型下面所有的子类型
                        dt = DBMgr.GetDataTable(sql);
                        //构建页码表格数据
                        DataTable dt2 = new DataTable();
                        DataColumn dc = new DataColumn("ID");
                        dt2.Columns.Add(dc);
                        for (int k = 0; k < dt.Rows.Count; k++)
                        {
                            dc = new DataColumn("c-" + dt.Rows[k]["FILETYPEID"] + "@" + dt.Rows[k]["FILETYPENAME"]);
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
                        db.StringSet(ordercode + ":" + fileid + ":splitdetail", json, TimeSpan.FromMinutes(1440));
                    }
                    else//如果已拆分 直接读取缓存数据库
                    {
                        if (db.KeyExists(ordercode + ":" + fileid + ":splitdetail"))
                        {
                            json = db.StringGet(ordercode + ":" + fileid + ":splitdetail");
                        }
                        else
                        {
                            pdfReader = new PdfReader(@"d:\ftpserver\" + splitfilename);
                            int totalPages = pdfReader.NumberOfPages;
                            pdfReader.Close(); pdfReader.Dispose();
                            sql = "select * from sys_filetype where parentfiletypeid=44 order by sortindex asc";//取该文件类型下面所有的子类型
                            dt = DBMgr.GetDataTable(sql);
                            //构建页码表格数据
                            DataTable dt2 = new DataTable();
                            DataColumn dc = new DataColumn("ID");
                            dt2.Columns.Add(dc);
                            for (int k = 0; k < dt.Rows.Count; k++)
                            {
                                dc = new DataColumn("c-" + dt.Rows[k]["FILETYPEID"] + "@" + dt.Rows[k]["FILETYPENAME"]);
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
                                            dr["c-" + tmp["FILETYPEID"] + "@" + tmp["FILETYPENAME"]] = "√";
                                        }
                                        else
                                        {
                                            dr["c-" + tmp["FILETYPEID"] + "@" + tmp["FILETYPENAME"]] = "";
                                        }
                                    }
                                }
                                dt2.Rows.Add(dr);
                            }
                            json = JsonConvert.SerializeObject(dt2);
                            db.StringSet(ordercode + ":" + fileid + ":splitdetail", json, TimeSpan.FromMinutes(1440));
                        }
                    }
                    //sql = "select * from sys_filetype where filetypeid=" + filetype;
                    //dt = DBMgr.GetDataTable(sql);
                    //string filetypename = dt.Rows[0]["FILETYPENAME"] + "";
                    //如果是已经拆分好的 需要调出所有拆分好的文件类型 filetype:'" + filetypename + "'
                    sql = @"select a.id,a.filetypeid,b.filetypename from LIST_ATTACHMENTDETAIL a left join sys_filetype
                          b on a.filetypeid=b.filetypeid where a.attachmentid='" + fileid + "' and a.ordercode='" + ordercode + "' order by b.sortindex asc";
                    dt = DBMgr.GetDataTable(sql);
                    string json_type = JsonConvert.SerializeObject(dt);
                    Response.Write(@"{success:true,src:'\/file\/" + splitfilename + "',rows:" + json + ",fileid:" + fileid + ",filestatus:'" + filestatus + "',result:" + json_type + "}");
                    Response.End();
                    break;
                case "split":
                    int filepages = 0;
                    string data = Request["pages"];
                    JArray jsonarray = JsonConvert.DeserializeObject<JArray>(data);
                    db.StringSet(ordercode + ":" + fileid + ":splitdetail", data);
                    sql = "select * from list_attachment where ID='" + fileid + "'";
                    dt = DBMgr.GetDataTable(sql);
                    //2016-6-16压缩改用pdfshrink在后台执行                   
                    string compressname = "";
                    //如果pdfshrink压缩文件存在               
                    if (File.Exists(@"d:\ftpserver\" + (dt.Rows[0]["FILENAME"] + "").Replace(".pdf", "").Replace(".PDF", "") + "-web.pdf"))
                    {
                        compressname = @"d:\ftpserver\" + (dt.Rows[0]["FILENAME"] + "").Replace(".pdf", "").Replace(".PDF", "") + "-web.pdf";
                    }
                    else
                    {
                        if (File.Exists(@"d:\Compress\" + fileid + ".pdf"))//如果代码压缩的文件存在
                        {
                            compressname = @"d:\Compress\" + fileid + ".pdf";
                        }
                        else
                        {
                            fi = new FileInfo(@"D:\ftpserver\" + dt.Rows[0]["FILENAME"]);
                            CompressPdf(fileid, fi);//不存在则生成压缩文件再进行拆分  
                            compressname = @"d:\Compress\" + fileid + ".pdf";
                        }
                    }
                    if (File.Exists(compressname))//如果压缩文件存在
                    {
                        try
                        {
                            if ((new FileInfo(@"D:\ftpserver\" + dt.Rows[0]["FILENAME"])).Length / 1024 == (new FileInfo(compressname)).Length / 1024)//压缩文件根源文件大小一样
                            {
                                //没压缩成功  新增压缩任务
                                DataTable dt_shrink = new DataTable();
                                dt_shrink = DBMgr.GetDataTable("select * from pdfshrinklog where attachmentid='" + fileid + "' and ISCOMPRESS=0");
                                if (dt_shrink.Rows.Count <= 0)
                                {
                                    sql = "insert into pdfshrinklog (id,attachmentid) values (pdfshrinklog_id.nextval,'" + fileid + "')";
                                    DBMgr.ExecuteNonQuery(sql);
                                }
                                Response.Write("{success:false}");//没压缩成功
                            }
                            else
                            {
                                pdfReader = new PdfReader(compressname); filepages = pdfReader.NumberOfPages;
                                sql = "select * from sys_filetype where parentfiletypeid=" + filetype;//取该文件类型下面所有的子类型
                                dt = DBMgr.GetDataTable(sql);
                                IList<Int32> pagelist;
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    int rotation = 0;
                                    pagelist = new List<Int32>();
                                    foreach (JObject jo in jsonarray)
                                    {
                                        if (jo.Value<string>("c-" + dt.Rows[i]["FILETYPEID"] + "@" + dt.Rows[i]["FILETYPENAME"]) == "√")
                                        {
                                            pagelist.Add(jo.Value<Int32>("ID"));//统计出该子类型下面所有的页码
                                        }
                                    }
                                    if (pagelist.Count > 0)
                                    {
                                        string new_name = DateTimeToUnixTimestamp(DateTime.Now) + "";
                                        if (!Directory.Exists(@"d:/ftpserver/" + dt.Rows[i]["FILETYPEID"] + @"/" + ordercode))
                                        {
                                            Directory.CreateDirectory(@"d:/ftpserver/" + dt.Rows[i]["FILETYPEID"] + @"/" + ordercode);
                                        }
                                        string newfilename = @"d:/ftpserver/" + dt.Rows[i]["FILETYPEID"] + @"/" + ordercode + @"/" + ordercode + "_" + new_name + ".pdf";
                                        FileStream fs = new FileStream(newfilename, FileMode.Create);
                                        Document newDocument = new Document();
                                        PdfWriter pdfWriter = PdfWriter.GetInstance(newDocument, fs);
                                        pdfWriter.CloseStream = true;
                                        newDocument.Open();
                                        PdfContentByte pdfContentByte = pdfWriter.DirectContent;
                                        foreach (Int32 page in pagelist)
                                        {
                                            newDocument.SetPageSize(pdfReader.GetPageSizeWithRotation(page));
                                            newDocument.NewPage();
                                            PdfImportedPage importedPage = pdfWriter.GetImportedPage(pdfReader, page);
                                            rotation = pdfReader.GetPageRotation(page);
                                            pdfContentByte.AddTemplate(importedPage, 0, 0);
                                            if (rotation == 90 || rotation == 270)
                                            {
                                                pdfContentByte.AddTemplate(importedPage, 0, -1f, 1f, 0, 0, pdfReader.GetPageSizeWithRotation(page).Height);
                                            }
                                            else
                                            {
                                                pdfContentByte.AddTemplate(importedPage, 1f, 0, 0, 1f, 0, 0);
                                            }
                                        }
                                        fs.Flush();
                                        newDocument.Close();
                                        sql = "insert into LIST_ATTACHMENTDETAIL (id,sourcefilename,filename,attachmentid,filetypeid,splitetime,ordercode,pages) values (list_attachmentdetail_id.nextval,'{0}','{1}','{2}','{3}',sysdate,'{4}','{5}')";
                                        sql = String.Format(sql, dt.Rows[i]["FILETYPEID"] + @"/" + ordercode + @"/" + ordercode + "_" + new_name + ".pdf", ordercode + "_" + new_name + ".pdf", fileid, dt.Rows[i]["FILETYPEID"], ordercode, string.Join(",", pagelist.ToArray()));
                                        DBMgr.ExecuteNonQuery(sql);
                                    }
                                }
                                pdfReader.Close(); pdfReader.Dispose();

                                //拆分完成后更新主文件的状态,同时将拆分好的类型送到页面形成按钮便于查看
                                sql = "update LIST_ATTACHMENT set SPLITSTATUS=1,CONFIRMSTATUS=1,FILEPAGES=" + filepages + " where id=" + fileid;
                                DBMgr.ExecuteNonQuery(sql);

                                DataTable dt_list_order = DBMgr.GetDataTable("select * from LIST_ORDER where  code='" + ordercode + "'");
                                sql = "update LIST_ORDER set FILESTATUS=1,FILESPLITTIME=sysdate,FILEPAGES=" + filepages
                                    + ",FILESPLITEUSERID='" + userid + "',FILESPLITEUSERNAME='" + username + "' where code='" + ordercode + "'";
                                int resultcode = DBMgr.ExecuteNonQuery(sql);
                                if (resultcode > 0)
                                {
                                    //若正常拆分在字段修改历史记录表中记录
                                    sql = "insert into list_updatehistory(id,ordercode,type,userid, updatetime, oldfield,newfield,name,fieldname,code,field)"
                                        + " values(LIST_UPDATEHISTORY_ID.nextval,'" + ordercode + "','1','" + userid + "',sysdate,'" + dt_list_order.Rows[0]["FILESTATUS"] + "','1','" + username + "','业务—文件状态-WEB','"
                                        + ordercode + "','FILESTATUS')";
                                    DBMgr.ExecuteNonQuery(sql);
                                }
                                sql = "select a.id,a.filetypeid,b.filetypename from LIST_ATTACHMENTDETAIL a left join sys_filetype b on a.filetypeid=b.filetypeid where a.ordercode='" + ordercode + "' order by b.sortindex asc";
                                dt = DBMgr.GetDataTable(sql);
                                json = JsonConvert.SerializeObject(dt);
                                Response.Write("{success:true,result:" + json + "}");
                            }
                        }
                        catch (Exception ex)
                        {
                            string error = "{\"ordercode\":\"" + ordercode + "\",\"fileid\":\"" + fileid + "\",\"error\":\"" + ex.Message + "\"}";
                            db.ListRightPush("spliterror", error);
                            Response.Write("{success:false,result:[" + error + "]}");//拆分异常
                        }

                    }
                    else
                    {
                        Response.Write("{success:false}");//压缩文件不存在 
                    }
                    Response.End();
                    break;
                case "cancelsplit":
                    //删除文件明细
                    string fileids_temp = "";
                    sql = "select * from list_attachmentdetail where ordercode='" + ordercode + "'";
                    dt = DBMgr.GetDataTable(sql);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (!fileids_temp.Contains(dt.Rows[i]["attachmentid"].ToString()))
                        {
                            fileids_temp = fileids_temp + dt.Rows[i]["attachmentid"].ToString() + ",";
                            db.KeyDelete(ordercode + ":" + fileid + ":splitdetail");
                        }
                        if (File.Exists(@"d:/ftpserver/" + dt.Rows[i]["SOURCEFILENAME"]))
                        {
                            File.Delete(@"d:/ftpserver/" + dt.Rows[i]["SOURCEFILENAME"]);
                        }
                        sql = "delete from list_attachmentdetail where id=" + dt.Rows[i]["ID"];
                        DBMgr.ExecuteNonQuery(sql);
                    }

                    if (fileids_temp != "")
                    {
                        fileids_temp = fileids_temp.Substring(0, fileids_temp.Length - 1);
                        sql = "update LIST_ATTACHMENT set SPLITSTATUS=0 where id in(" + fileids_temp + ")";
                        DBMgr.ExecuteNonQuery(sql);
                    }

                    DataTable dt_list_order_c = DBMgr.GetDataTable("select * from LIST_ORDER where  code='" + ordercode + "'");

                    //20160922赵艳提出 拆分完，需要更新订单表的 拆分人和时间,和文件状态
                    sql = "update LIST_ORDER set FILESTATUS=0,FILEPAGES=null,FILESPLITEUSERNAME=null,FILESPLITEUSERID=null,FILESPLITTIME=null where code='" + ordercode + "'";
                    DBMgr.ExecuteNonQuery(sql);

                    sql = "insert into list_updatehistory(id,ordercode,type,userid, updatetime, oldfield,newfield,name,fieldname,code,field)"
                        + " values(LIST_UPDATEHISTORY_ID.nextval,'" + ordercode + "','1','" + userid + "',sysdate,'" + dt_list_order_c.Rows[0]["FILESTATUS"] + "','0','" + username + "','业务—文件状态-WEB','"
                        + ordercode + "','FILESTATUS')";
                    DBMgr.ExecuteNonQuery(sql);

                    Response.Write("{success:true}");
                    Response.End();
                    break;
                case "loadfile":
                    sql = "select * from list_attachmentdetail where id='" + fileid + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        Response.Write(@"{success:true,src:'/file/" + dt.Rows[0]["SOURCEFILENAME"] + "'}");
                        Response.End();
                    }
                    break;
                case "adjustpage":
                    int currentPage = Convert.ToInt32(Request["currentpage"]);
                    string direction = Request["direction"];
                    sql = "select * from list_attachment where ID=" + fileid;
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        if (File.Exists(@"d:\ftpserver\" + dt.Rows[0]["FILENAME"] + ""))
                        {
                            string newid = Guid.NewGuid().ToString();
                            string outFile = dt.Rows[0]["FILETYPE"] + @"/" + ordercode + @"/" + newid + ".pdf";
                            AdjustPage(dt.Rows[0]["FILENAME"] + "", currentPage, direction, @"d:/ftpserver/" + outFile);
                            //删除老文件 并更新该记录的FILENAME字段
                            //2016-8-17测试发现调整页码顺序重新生成文件，删除时有可能文件正在使用中,故原始文件暂不需要删除 下面这句话会报错
                            //File.Delete(@"d:/ftpserver/" + dt.Rows[0]["FILENAME"]);                           
                            sql = "update list_attachment set FileName='" + outFile + "',fileguid='" + newid + "',originalname='" + newid + ".pdf'  where id=" + fileid;
                            DBMgr.ExecuteNonQuery(sql);
                            Response.Write(@"{success:true,src:'/file/" + outFile + "'}");
                            Response.End();
                        }
                    }
                    break;
                case "compress":
                    string ser_path = Server.MapPath(Request["path"]);
                    fi = new FileInfo(ser_path);
                    CompressPdf(fileid, fi);
                    break;
                case "loadform":
                    sql = "SELECT * FROM list_order WHERE CODE = '" + ordercode + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (!string.IsNullOrEmpty(dt.Rows[0]["ASSOCIATENO"].ToString()))
                    {
                        sql = "SELECT * FROM list_order WHERE CODE != '" + ordercode + "' and ASSOCIATENO='" + dt.Rows[0]["ASSOCIATENO"] + "'";
                        DataTable dt_gl = DBMgr.GetDataTable(sql);
                        if (dt_gl.Rows.Count > 0) { dt.Rows[0]["ASSOCIATENO"] = dt_gl.Rows[0]["CODE"]; }
                        else { dt.Rows[0]["ASSOCIATENO"] = ""; }                        
                    }
                    string result = JsonConvert.SerializeObject(dt).Replace("[", "").Replace("]", "");
                    sql = "select * from list_attachment where ordercode='" + ordercode + "' and filetype=44 order by uploadtime asc";
                    DataTable dt_file = DBMgr.GetDataTable(sql);
                    string result_file = JsonConvert.SerializeObject(dt_file);
                    Response.Write("{formdata:" + result + ",filedata:" + result_file + "}");
                    Response.End();
                    break;
                case "loadattach":
                    sql = "select * from list_attachment where ordercode='" + ordercode + "' and filetype=44 order by uploadtime asc";
                    DataTable dt_attach = DBMgr.GetDataTable(sql);
                    string result_attach = JsonConvert.SerializeObject(dt_attach);
                    Response.Write("{filedata:" + result_attach + "}");
                    Response.End();
                    break;
                case "delete"://删除及明细
                    try
                    {
                        string del_fileids = Request["fileids"].Replace("[", "").Replace("]", "");
                        string[] del_fileidarray = del_fileids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < del_fileidarray.Length; i++)
                        {
                            sql = "select * from list_attachment where ID=" + del_fileidarray[i];
                            dt = DBMgr.GetDataTable(sql);
                            if (dt.Rows.Count > 0)
                            {
                                //删除明细
                                sql = "select * from list_attachmentdetail where ordercode='" + ordercode + "' and attachmentid=" + del_fileidarray[i];
                                DataTable dt_detail = DBMgr.GetDataTable(sql);
                                for (int j = 0; j < dt_detail.Rows.Count; j++)
                                {
                                    if (File.Exists(@"d:/ftpserver/" + dt_detail.Rows[j]["SOURCEFILENAME"]))
                                    {
                                        File.Delete(@"d:/ftpserver/" + dt_detail.Rows[j]["SOURCEFILENAME"]);
                                    }
                                    sql = "delete from list_attachmentdetail where id=" + dt_detail.Rows[j]["ID"];
                                    DBMgr.ExecuteNonQuery(sql);
                                }

                                //删除                               
                                if (File.Exists(@"d:/ftpserver/" + dt.Rows[0]["FILENAME"]))
                                {                                    
                                    File.Delete(@"d:/ftpserver/" + dt.Rows[0]["FILENAME"]);   
                                }

                                sql = "delete from list_attachment where id= '" + del_fileidarray[i] + "'";
                                DBMgr.ExecuteNonQuery(sql);

                                db.KeyDelete(ordercode + ":" + del_fileidarray[i] + ":splitdetail");
                            }
                        }

                        sql = "update LIST_ORDER set FILESTATUS=0,FILEPAGES=null,FILESPLITEUSERNAME=null,FILESPLITEUSERID=null,FILESPLITTIME=null where code='" + ordercode + "'";
                        DBMgr.ExecuteNonQuery(sql);

                        Response.Write("{success:true}");
                    }
                    catch (Exception ex)
                    {
                        Response.Write("{success:false,error:\"" + ex.Message + "\"}");
                    } 

                    Response.End();
                    break;
            }
        }
        private static System.Drawing.Image ShrinkImage(System.Drawing.Image sourceImage, float scaleFactor)
        {
            int newWidth = Convert.ToInt32(sourceImage.Width * scaleFactor);
            int newHeight = Convert.ToInt32(sourceImage.Height * scaleFactor);

            var thumbnailBitmap = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(thumbnailBitmap))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                System.Drawing.Rectangle imageRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
                g.DrawImage(sourceImage, imageRectangle);
            }
            return thumbnailBitmap;
        }
        //Standard image save code from MSDN, returns a byte array
        private static byte[] ConvertImageToBytes(System.Drawing.Image image, long compressionLevel)
        {
            if (compressionLevel < 0)
            {
                compressionLevel = 0;
            }
            else if (compressionLevel > 100)
            {
                compressionLevel = 100;
            }
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, compressionLevel);
            myEncoderParameters.Param[0] = myEncoderParameter;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, jgpEncoder, myEncoderParameters);
                return ms.ToArray();
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        private void CompressPdf(string fileId, FileInfo fi)
        {
            //ITextSharp是一个生成Pdf文件的开源项目
            PdfReader reader = new PdfReader(fi.FullName);
            if (File.Exists(@"D:\Compress\" + fileId + ".pdf"))//先判断对应文件ID的压缩文件是否存在  如果存在,将其删除
            {
                File.Delete(@"D:\Compress\" + fileId + ".pdf");
            }
            string newPath = @"D:\Compress\" + fileId + ".pdf";
            using (FileStream fs = new FileStream(newPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (PdfStamper stamper = new PdfStamper(reader, fs))
                {
                    #region
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfDictionary page = reader.GetPageN(i);
                        PdfDictionary resources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
                        PdfDictionary xobject = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT));
                        if (xobject != null)
                        {
                            PdfObject obj;
                            foreach (PdfName name in xobject.Keys)
                            {
                                obj = xobject.Get(name);
                                if (obj.IsIndirect())
                                {
                                    //Get the current key as a PDF object
                                    PdfDictionary imgObject = (PdfDictionary)PdfReader.GetPdfObject(obj);
                                    //See if its an image
                                    if (imgObject.Get(PdfName.SUBTYPE).Equals(PdfName.IMAGE))
                                    {
                                        //NOTE: There's a bunch of different types of filters, I'm only handing the simplest one here which is basically raw JPG, you'll have to research others
                                        if (imgObject.Get(PdfName.FILTER).Equals(PdfName.DCTDECODE))
                                        {
                                            //Get the raw bytes of the current image
                                            byte[] oldBytes = PdfReader.GetStreamBytesRaw((PRStream)imgObject);
                                            //Will hold bytes of the compressed image later
                                            byte[] newBytes;
                                            //Wrap a stream around our original image
                                            using (MemoryStream sourceMS = new MemoryStream(oldBytes))
                                            {
                                                //Convert the bytes into a .Net image
                                                using (System.Drawing.Image oldImage = Bitmap.FromStream(sourceMS))
                                                {
                                                    //Shrink the image to 90% of the original
                                                    using (System.Drawing.Image newImage = ShrinkImage(oldImage, 40 / 100f))
                                                    {
                                                        //Convert the image to bytes using JPG at 85%
                                                        newBytes = ConvertImageToBytes(newImage, 35);
                                                    }
                                                }
                                            }
                                            //Create a new iTextSharp image from our bytes
                                            iTextSharp.text.Image compressedImage = iTextSharp.text.Image.GetInstance(newBytes);
                                            //Kill off the old image
                                            PdfReader.KillIndirect(obj);
                                            //Add our image in its place
                                            stamper.Writer.AddDirectImageSimple(compressedImage, (PRIndirectReference)obj);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            reader.Close(); reader.Dispose();
        }
        protected void MergePDFFiles(IList<string> fileList, string outMergeFile)
        {
            string filedir = outMergeFile.Substring(0, outMergeFile.LastIndexOf('/'));
            if (!Directory.Exists(filedir))
            {
                Directory.CreateDirectory(filedir);
            }

            int rotation = 0;
            PdfReader reader;
            Document document = new Document();
            // Define the output place, and add the document to the streamPageSize.A3
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outMergeFile, FileMode.Create));
            document.Open(); // Open document
            // PDF ContentByte
            PdfContentByte cb = writer.DirectContent;
            // PDF import page
            PdfImportedPage newPage;
            for (int i = 0; i < fileList.Count; i++)
            {
                reader = new PdfReader(@"d:\ftpserver\" + fileList[i]);
                int iPageNum = reader.NumberOfPages;
                for (int j = 1; j <= iPageNum; j++)
                {
                    //2016-6-15发现一个新的问题不是所有过来的pdf文件都是A4大小的,需要根据原始文档的大小做合并操作 
                    document.SetPageSize(reader.GetPageSizeWithRotation(j));
                    document.NewPage();
                    newPage = writer.GetImportedPage(reader, j);
                    cb.AddTemplate(newPage, 0, 0);
                    rotation = reader.GetPageRotation(j);
                    switch (rotation)
                    {
                        case 90:
                            cb.AddTemplate(newPage, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(j).Height);
                            break;
                        case 180:
                            cb.AddTemplate(newPage, -1f, 0, 0, -1f, reader.GetPageSizeWithRotation(j).Width, reader.GetPageSizeWithRotation(j).Height);
                            break;
                        case 270:
                            cb.AddTemplate(newPage, 0, 1f, -1f, 0, reader.GetPageSizeWithRotation(j).Width, 0);
                            break;
                        default:
                            cb.AddTemplate(newPage, 1f, 0, 0, 1f, 0, 0);//等同于
                            break;
                    }
                }
            }
            document.Close();
        }
        protected void AdjustPage(string filename, int currentPage, string direction, string outFile)
        {
            PdfReader reader;
            Document document = new Document();
            // Define the output place, and add the document to the stream
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outFile, FileMode.Create));
            // Open document
            document.Open();
            // PDF ContentByte
            PdfContentByte cb = writer.DirectContent;
            // PDF import page
            PdfImportedPage newPage;
            reader = new PdfReader(@"d:\ftpserver\" + filename);
            int iPageNum = reader.NumberOfPages;
            for (int j = 1; j <= iPageNum; j++)
            {
                if (direction == "down")
                {
                    if (currentPage == j)
                    {
                        document.NewPage();
                        newPage = writer.GetImportedPage(reader, j + 1);
                        cb.AddTemplate(newPage, 0, 0);
                        document.NewPage();
                        newPage = writer.GetImportedPage(reader, j);
                        cb.AddTemplate(newPage, 0, 0);
                        j++;
                    }
                    else
                    {
                        document.NewPage();
                        newPage = writer.GetImportedPage(reader, j);
                        cb.AddTemplate(newPage, 0, 0);
                    }
                }
                if (direction == "up")
                {
                    if (currentPage == j + 1)
                    {
                        document.NewPage();
                        newPage = writer.GetImportedPage(reader, j + 1);
                        cb.AddTemplate(newPage, 0, 0);
                        document.NewPage();
                        newPage = writer.GetImportedPage(reader, j);
                        cb.AddTemplate(newPage, 0, 0);
                        j++;
                    }
                    else
                    {
                        document.NewPage();
                        newPage = writer.GetImportedPage(reader, j);
                        cb.AddTemplate(newPage, 0, 0);
                    }
                }
                //if (direction == "delete")  by panhuaguo 2016-4-19 梁总决定不让删除原始文件的某一页
                //{
                //    if (currentPage != j)
                //    {
                //        document.NewPage();
                //        newPage = writer.GetImportedPage(reader, j);
                //        cb.AddTemplate(newPage, 0, 0);
                //    }
                //}
            }
            document.Close();
        }
        /// <summary>
        /// 日期转换成unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
            return Convert.ToInt64((dateTime - start).TotalSeconds);
        }
    }
}