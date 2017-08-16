using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.BeforeMaintain
{
    public partial class CarouselAdList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sql = "";
            string action = Request["action"];
            DataTable dt;
            switch (action)
            {
                case "select":
                    sql = @"select ID,IMGURL,LINKURL,DESCRIPTION,STATUS,FILENAME,SORTINDEX from web_banner order by SORTINDEX";
                    dt = DBMgr.GetDataTable(sql);
                    string json = JsonConvert.SerializeObject(dt);
                    Response.Write("{rows:" + json + "}");
                    Response.End();
                    break;
                case "delete":
                    //先删除对应的本地文件
                    sql = "select * from web_banner where id='" + Request["id"] + "'";
                    dt = DBMgr.GetDataTable(sql);
                    string path = Server.MapPath(dt.Rows[0]["IMGURL"] + "");
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    sql = @"delete from web_banner where id = '" + Request["id"] + "'";
                    DBMgr.ExecuteNonQuery(sql);
                    break;
                case "save":
                    save(Request["formdata"]);
                    break;
            }
        }

        private void save(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            string result = "{success:true}";

            string sql = "";

            string DESCRIPTION = json.Value<string>("DESCRIPTION");
            string STATUS = json.Value<string>("STATUS");
            string SORTINDEX = json.Value<string>("SORTINDEX");

            if (!string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                HttpPostedFile postedFile = Request.Files["IMGURL"];//获取上传信息对象  
                string fileName = Path.GetFileName(postedFile.FileName);
                System.Drawing.Image image = System.Drawing.Image.FromStream(postedFile.InputStream);
                int hig = image.Height;
                int wid = image.Width;

                if ((hig > 458 && hig < 462) && (wid > 1918 && wid < 1922))
                {
                    string savepath = Server.MapPath(@"\FileUpload\Banner\");
                    string strGuid = Guid.NewGuid().ToString();
                    string IMGURL = @"\FileUpload\Banner\" + strGuid + "_" + fileName;
                    sql = @"insert into WEB_BANNER (ID,IMGURL,DESCRIPTION,STATUS,FILENAME,SORTINDEX) values ('" + strGuid + "','" + IMGURL + "','" + DESCRIPTION + "','" + STATUS + "','" + fileName + "','" + SORTINDEX + "')";
                    DBMgr.ExecuteNonQuery(sql);
                    postedFile.SaveAs(savepath + strGuid + "_" + fileName);//保存

                }
                else
                {
                    result = "{success:false}";
                }  

            }
            else
            {
                sql = @"update WEB_BANNER set DESCRIPTION='" + DESCRIPTION + "',STATUS='" + STATUS + "',SORTINDEX='" + SORTINDEX + "' where id = '" + json.Value<string>("ID") + "'";
                DBMgr.ExecuteNonQuery(sql);
            }

            Response.Write(result);
            Response.End();

        }


    }
}