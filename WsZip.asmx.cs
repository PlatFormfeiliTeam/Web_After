using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using Web_After.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace Web_After
{
    /// <summary>
    /// WsZip 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WsZip : System.Web.Services.WebService
    {

        [WebMethod]
        public string getZipFile(string filedata, string customer, string judge)
        {
            string dir = @"d:/ftpserver/";
            string tmp_dir = @"d:/ftpserver/declare_tmp_zip/";
            if (!Directory.Exists(tmp_dir))
            {
                Directory.CreateDirectory(tmp_dir);
            }
            try
            {
                JArray j_array = JArray.Parse(filedata);
                
                string codelist="(";
                int i = 0;
                foreach (JObject jo in j_array)
                {
                    codelist += "'" + jo["CODE"] + "'";
                    if (i != j_array.Count - 1)
                    {
                       codelist +=",";
                    }
                    i++;
                    
                }
                codelist += ")";

                string sql = @"select t.* from list_attachment t where t.filetype='61' and t.declcode in " + codelist + " order by pgindex asc,uploadtime asc";
                DataTable dt = DBMgr.GetDataTable(sql);


                string filepath = string.Empty;
                //MemoryStream ms = new MemoryStream();
                ZipEntryFactory zipEntryFactory = new ZipEntryFactory();
                //ZipFile zipFile = new ZipFile(ms);
                string filename = DateTime.Now.ToString("yyyyMMddhhmmssff") + ".zip";
                string newfilename = string.Empty;
                string sourcefile = string.Empty;
                string filepath_mask = string.Empty;
                string busitype = string.Empty;
                using (ZipOutputStream outPutStream = new ZipOutputStream(System.IO.File.Create(tmp_dir + filename)))
                {
                  
                    outPutStream.SetLevel(5);
                    ZipEntry entry = null;
                    byte[] buffer = null;
                    //foreach (DataRow dr in dt.Rows)
                    foreach (JObject jo in j_array)
                    {

                        DataRow[] drs=dt.Select("DECLCODE='"+jo["CODE"]+"'");
                        busitype = jo["BUSITYPE"].ToString();
                        sourcefile = drs[0]["FILENAME"].ToString();
                        filepath = dir + sourcefile;

                        
                        
                        filepath_mask = AddBackground(filepath, "企业留存联", busitype, "", customer,judge);
                        
                        
                        
                        newfilename = jo["DECLARATIONCODE"].ToString() + sourcefile.Substring(sourcefile.LastIndexOf("."));
                        buffer = new byte[4096];
                        entry = zipEntryFactory.MakeFileEntry(newfilename);
                        outPutStream.PutNextEntry(entry);
                        using (FileStream fileStream = File.OpenRead(filepath_mask))
                        {
                            StreamUtils.Copy(fileStream, outPutStream, buffer);

                        }
                    }
                    outPutStream.Finish();
                    outPutStream.Close();

                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                return  "/file/declare_tmp_zip/" + filename;
            }
            catch (Exception)
            {

                return "error";
            }
           


        }


        public string AddBackground(string filename, string printtmp, string busitype, string decltype,string customer,string judge)
        {
            string tmp_dir = @"d:/ftpserver/declare_tmp_zip/";
            string outname = Guid.NewGuid() + "";
            string destFile = tmp_dir+outname + ".pdf";
            if (judge == "1")
            {
                File.Copy(filename, destFile);
                return destFile;
            }
            
            DataTable dt_mask = new DataTable(); int top_int = 0, right_int = 0, buttom_int = 0, left_int = 0;

            string sql = "select POSITIONWEBTOP,POSITIONWEBRIGHT,POSITIONWEBBUTTOM,POSITIONWEBLEFT from config_watermark where CUSTOMER='" + customer + "'";         
            dt_mask = DBMgr.GetDataTable(sql);
            if (dt_mask.Rows.Count > 0)
            {
                top_int = Convert.ToInt32(dt_mask.Rows[0]["POSITIONWEBTOP"].ToString() == "" ? "0" : dt_mask.Rows[0]["POSITIONWEBTOP"].ToString());
                right_int = Convert.ToInt32(dt_mask.Rows[0]["POSITIONWEBRIGHT"].ToString() == "" ? "0" : dt_mask.Rows[0]["POSITIONWEBRIGHT"].ToString());
                buttom_int = Convert.ToInt32(dt_mask.Rows[0]["POSITIONWEBBUTTOM"].ToString() == "" ? "0" : dt_mask.Rows[0]["POSITIONWEBBUTTOM"].ToString());
                left_int = Convert.ToInt32(dt_mask.Rows[0]["POSITIONWEBLEFT"].ToString() == "" ? "0" : dt_mask.Rows[0]["POSITIONWEBLEFT"].ToString());
            }  

            Image img = null;
            if (busitype == "11" || busitype == "21" || busitype == "31" || busitype == "41" || busitype == "51")
            {
                if (printtmp == "海关作业联")
                {
                    img = Image.GetInstance(Server.MapPath("/FileUpload/进口-海关作业联.png"));
                }
                if (printtmp == "企业留存联")
                {
                    img = Image.GetInstance(Server.MapPath("/FileUpload/进口-企业留存联.png"));
                }
                if (printtmp == "海关核销联")
                {
                    img = Image.GetInstance(Server.MapPath("/FileUpload/进口-海关核销联.png"));
                }
            }
            else
            {
                if (printtmp == "海关作业联")
                {
                    img = Image.GetInstance(Server.MapPath("/FileUpload/出口-海关作业联.png"));
                }
                if (printtmp == "企业留存联")
                {
                    img = Image.GetInstance(Server.MapPath("/FileUpload/出口-企业留存联.png"));
                }
                if (printtmp == "海关核销联")
                {
                    img = Image.GetInstance(Server.MapPath("/FileUpload/出口-海关核销联.png"));
                }
            }
            
            FileStream stream = new FileStream(destFile, FileMode.Create, FileAccess.ReadWrite);
            byte[] pwd = System.Text.Encoding.Default.GetBytes(ConfigurationManager.AppSettings["PdfPwd"]);//密码 
            PdfReader reader = new PdfReader(filename, pwd);

            iTextSharp.text.Rectangle psize = reader.GetPageSize(1);
            var imgWidth = psize.Width + right_int;
            var imgHeight = psize.Height - top_int + buttom_int;
            img.ScaleAbsolute(imgWidth, imgHeight);
            img.SetAbsolutePosition(0 + left_int, 0 - buttom_int);//坐标是从左下角开始算的，注意 

            PdfStamper stamper = new PdfStamper(reader, stream);    //read pdf stream 
            int totalPage = reader.NumberOfPages;
            for (int current = 1; current <= totalPage; current++)
            {
                var canvas = stamper.GetUnderContent(current);
                var page = stamper.GetImportedPage(reader, current);
                canvas.AddImage(img);
            }
            stamper.Close();
            reader.Close();
            return destFile;
        }
    }
}
