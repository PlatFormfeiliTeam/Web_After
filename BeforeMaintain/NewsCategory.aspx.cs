using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.BeforeMaintain
{
    public partial class NewsCategory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FormsIdentity identity = User.Identity as FormsIdentity;
            if (identity == null)
            {
                return;
            }

        }



        [WebMethod]
        public static string CateManager(string action, string json)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            JObject joc = (JObject)JsonConvert.DeserializeObject(json);
            string sql = string.Empty;
            string haveChild = "false";
            string haveNews = "false";
            if (action == "create")
            {
                sql = @"insert into NEWSCATEGORY (ID,PID,NAME,REMARK,CREATETIME,CREATEID,ISLEAF,SORTINDEX) 
                          values (NEWSCATEGORY_ID.nextval,'{0}','{1}','{2}',sysdate,'{3}',1,'{4}')";
                sql = string.Format(sql, joc.Value<string>("PID"), joc.Value<string>("NAME"), joc.Value<string>("REMARK"), json_user.GetValue("ID"), joc.Value<string>("SORTINDEX"));
                DBMgr.ExecuteNonQuery(sql);
                string sql_up = "update NEWSCATEGORY set isleaf=null where id='" + joc.Value<string>("PID") + "'";
                DBMgr.ExecuteNonQuery(sql_up);


            }
            if (action == "delete")
            {

                sql = "select * from web_notice where type='" + joc.Value<string>("ID") + "'";
                int i = DBMgr.GetDataTable(sql).Rows.Count;
                if (i <= 0)
                {
                    sql = @"delete from NEWSCATEGORY where id='" + joc.Value<string>("ID") + "' and ISLEAF=1";
                    int j = DBMgr.ExecuteNonQuery(sql);
                    if (j <= 0)
                    {
                        //有其它子类，请先移除
                        haveChild = "true";
                    }
                    else
                    {
                        sql = "select * from NEWSCATEGORY where pid='" + joc.Value<string>("PID") + "'";
                        int k = DBMgr.GetDataTable(sql).Rows.Count;
                        if (k <= 0)
                        {
                            DBMgr.ExecuteNonQuery("update  NEWSCATEGORY set ISLEAF=1 where id='" + joc.Value<string>("PID") + "'");
                        }
                    }
                }
                else
                {
                    //此类别下有新闻不能删除
                    haveNews = "true";

                }
            }
            if (action == "update")
            {
                sql = @"update NEWSCATEGORY set NAME='{0}',REMARK='{1}',SORTINDEX='{2}',CREATETIME=sysdate,CREATEID='{3}'  WHERE ID='{4}'";
                sql = string.Format(sql, joc.Value<string>("NAME"), joc.Value<string>("REMARK"), joc.Value<string>("SORTINDEX"), json_user.GetValue("ID"), joc.Value<string>("ID"));
                DBMgr.ExecuteNonQuery(sql);
            }




            //joc.Remove("ID");
            //joc.Add("ID", newid);
            //joc.Add("leaf", 1);

            //  return "{success:true,data:" + joc + "}";
            return "{success:true,haveChild:" + haveChild + ",haveNews:" + haveNews + "}";

        }

        [WebMethod]
        public static string getCate(string id)
        {
            string result = ""; string sql = string.Empty;
            if (string.IsNullOrEmpty(id))
            {
                sql = @"select t.* from NEWSCATEGORY t where t.PID is null order by sortindex";
            }
            else
            {
                sql = @"select t.* from NEWSCATEGORY t where t.PID='" + id + "' order by sortindex";
            }

            result = "[";
            DataTable dt = DBMgr.GetDataTable(sql);
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ISLEAF"] + "" == "1")
                {
                    result += "{ID:'" + dr["id"] + "',NAME:'" + dr["NAME"] + "',PID:'" + dr["PID"] + "',leaf:'" + dr["ISLEAF"] + "',REMARK:'" + dr["REMARK"] + "',SORTINDEX:'" + dr["SORTINDEX"] + "',children:[]}";
                }
                else
                {
                    result += "{ID:'" + dr["id"] + "',NAME:'" + dr["NAME"] + "',PID:'" + dr["PID"] + "',leaf:'" + dr["ISLEAF"] + "',REMARK:'" + dr["REMARK"] + "',SORTINDEX:'" + dr["SORTINDEX"] + "'}";
                }


                if (i != dt.Rows.Count - 1)
                {
                    result += ",";
                }
                i++;
            }
            result += "]";
            return result;
        }
    }
}