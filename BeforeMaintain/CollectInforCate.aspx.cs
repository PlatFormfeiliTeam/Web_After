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
    public partial class CollectInforCate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sql = "";
            string action = Request["action"];
            string id = string.Empty; string ICON = string.Empty; string formdata = "";
            DataTable dt; string json = "";
            string webpath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;

            switch (action)
            {
                case "select":
                    sql = @"select ID,NAME,DESCRIPTION,ICON,SORTINDEX from list_collect_infor_cate where ISINVALID=0 order by SORTINDEX";
                    dt = DBMgr.GetDataTable(sql);
                    json = JsonConvert.SerializeObject(dt);
                    Response.Write("{rows:" + json + "}");
                    Response.End();
                    break;
                case "loaddetail":
                    id = Request["id"];
                    sql = @"SELECT * FROM list_collect_infor WHERE rid_type = " + id + " order by CREATEDATE ASC";
                    dt = DBMgr.GetDataTable(sql);
                    json = JsonConvert.SerializeObject(dt);
                    Response.Write("{innerrows:" + json + "}");
                    Response.End();
                    break;
                case "delete":
                    id = Request["id"]; ICON = Request["ICON"];
                    sql = @"delete from list_collect_infor where id in (" + id + ")";
                    DBMgr.ExecuteNonQuery(sql);

                    sql = @"delete from list_collect_infor_byuser where type='news' and rid in (" + id + ")";
                    DBMgr.ExecuteNonQuery(sql);

                    if (File.Exists(webpath + ICON)) { File.Delete(webpath + ICON); }

                    Response.Write("{success:true}");
                    Response.End();
                    break;
                case "save":
                    formdata = Request["formdata"];
                    JObject jo = (JObject)JsonConvert.DeserializeObject(formdata);
                    id = jo.Value<string>("ID"); string NAME = jo.Value<string>("NAME");
                    string DESCRIPTION = jo.Value<string>("DESCRIPTION"); string SORTINDEX = jo.Value<string>("SORTINDEX");

                    sql = @"update list_collect_infor_cate set NAME='" + NAME + "',DESCRIPTION='" + DESCRIPTION + "',SORTINDEX='" + SORTINDEX + "' where id=" + id;
                    DBMgr.ExecuteNonQuery(sql);

                    sql = @"update list_collect_infor set TYPE='" + NAME + "' where rid_type=" + id;
                    DBMgr.ExecuteNonQuery(sql);

                    Response.Write("{success:true}");
                    Response.End();
                    break;
                case "saveinner":
                    saveinner(Request["formdata"]);
                    break;
            }
        }

        private void saveinner(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            string RID_TYPE = json.Value<string>("RID_TYPE"); string URL = json.Value<string>("URL"); string NAME = json.Value<string>("NAME");
            string ISINVALID = json.Value<string>("ISINVALID"); string ICON = string.Empty; string ICON_OLD = json.Value<string>("ICON_OLD");

            string result = "{success:true}"; string fileName = ""; string strGuid = ""; string savepath = "";
            string webpath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;

            string sql = "";

            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                HttpPostedFile postedFile = Request.Files["ICON"];//获取上传信息对象  
                fileName = Path.GetFileName(postedFile.FileName);
                System.Drawing.Image image = System.Drawing.Image.FromStream(postedFile.InputStream);

                if ((image.Height >= 58 && image.Height <= 62) && (image.Width >= 44 && image.Width <= 48))
                {
                    savepath = Server.MapPath(@"/FileUpload/InforCate/");
                    strGuid = Guid.NewGuid().ToString();
                    ICON = @"/FileUpload/InforCate/" + strGuid + "_" + fileName;
                    sql = @"insert into list_collect_infor (ID,ICON,URL,NAME,ISINVALID,CREATEDATE,RID_TYPE,TYPE) 
                                values (LIST_COLLECT_INFOR_ID.nextval,'" + ICON + "','" + URL + "','" + NAME + "','" + ISINVALID + "',sysdate,'" + RID_TYPE
                                        + "',(select name from list_collect_infor_cate where id='" + RID_TYPE + "'))";
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
                sql = @"update list_collect_infor set URL='" + URL + "',NAME='" + NAME + "',ISINVALID='" + ISINVALID + "'";

                HttpPostedFile postedFile_update = Request.Files["ICON"];//获取上传信息对象  
                if (postedFile_update.FileName != "")
                {
                    fileName = Path.GetFileName(postedFile_update.FileName);
                    System.Drawing.Image image_update = System.Drawing.Image.FromStream(postedFile_update.InputStream);
                    if ((image_update.Height >= 58 && image_update.Height <= 62) && (image_update.Width >= 44 && image_update.Width <= 48))
                    {
                        savepath = Server.MapPath(@"/FileUpload/InforCate/");
                        strGuid = Guid.NewGuid().ToString();
                        ICON = @"/FileUpload/InforCate/" + strGuid + "_" + fileName;
                        sql = sql + ",ICON='" + ICON + "'";

                        sql = sql + " where id = '" + json.Value<string>("ID") + "'";
                        DBMgr.ExecuteNonQuery(sql);

                        if (File.Exists(webpath + ICON_OLD)) { File.Delete(webpath + ICON_OLD); }
                        postedFile_update.SaveAs(savepath + strGuid + "_" + fileName); //保存

                    }
                    else
                    {
                        result = "{success:false}";
                    }
                }
                else
                {
                    sql = sql + " where id = '" + json.Value<string>("ID") + "'";
                    DBMgr.ExecuteNonQuery(sql);
                }
            }

            Response.Write(result);
            Response.End();
        }


    }
}