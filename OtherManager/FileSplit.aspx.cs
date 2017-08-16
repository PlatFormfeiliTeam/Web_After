using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.OtherManager
{
    public partial class FileSplit : System.Web.UI.Page
    {
        IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
        int totalProperty = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"];
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            switch (action)
            {
                case "Ini_Base_Data":
                    Ini_Base_Data();
                    break;
                case "loadData":
                    loadData();
                    break;
                case "save":
                    save(Request["formdata"]);
                    break;
                case "delete":
                    deleteData();
                    break;
            }
        }

        //基础资料 20170713
        public void Ini_Base_Data()
        {
            string sql = "";

            string json_jydw = "";//经营单位、申报单位
            sql = "SELECT CODE,NAME||'('||CODE||')' NAME FROM cusdoc.BASE_COMPANY where CODE is not null and enabled=1";
            json_jydw = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));

            string json_wtdw = "[]";//委托单位
            sql = "SELECT CODE,NAME||'('||CODE||')' NAME FROM cusdoc.sys_customer where enabled=1";
            json_wtdw = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));

            string json_wjlx = "[]";//文件类型
            sql = "SELECT to_char(FILETYPEID) CODE,FILETYPENAME NAME FROM sys_filetype where parentfiletypeid=44  order by sortindex asc";
            json_wjlx = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));

            Response.Write("{jydw:" + json_jydw + ",wtdw:" + json_wtdw + ",wjlx:" + json_wjlx + "}");
            Response.End();
        }

        private void loadData()
        {
            string strWhere = string.Empty;

            if (!string.IsNullOrEmpty(Request["BUSIUNITCODE_S"]))
            {
                strWhere = " and instr(BUSIUNITCODE,'" + Request["BUSIUNITCODE_S"] + "')>0";
            }
            if (!string.IsNullOrEmpty(Request["CUSTOMERCODE_S"]))
            {
                strWhere = " and instr(CUSTOMERCODE,'" + Request["CUSTOMERCODE_S"] + "')>0";
            }
            if (!string.IsNullOrEmpty(Request["REPUNITCODE_S"]))
            {
                strWhere = " and REPUNITCODE='" + Request["REPUNITCODE_S"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["BUSITYPE_S"]))
            {
                strWhere = " and BUSITYPE='" + Request["BUSITYPE_S"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["FILETYPE_S"]))
            {
                strWhere = " and FILETYPE='" + Request["FILETYPE_S"] + "'";
            }
            string sql = "select * from config_filesplit where 1=1 " + strWhere;
            sql = Extension.GetPageSql(sql, "ID", "desc", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            DataTable dt = DBMgr.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }

        private void save(string formdata)
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.Current.User.Identity.Name);
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);

            string sql = "";
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                sql = @"insert into config_filesplit(id,
                                    busiunitcode,customercode,repunitcode,busitype,filetype,createuserid, 
                                    createusername,createtime
                                ) values(config_filesplit_id.nextval
                                    ,'{0}','{1}','{2}', '{3}','{4}','{5}'
                                    ,'{6}',sysdate)";
                sql = string.Format(sql
                        , json.Value<string>("BUSIUNITCODE"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("REPUNITCODE"), json.Value<string>("BUSITYPE"), json.Value<string>("FILETYPE"), json_user.Value<string>("ID")
                        , json_user.Value<string>("REALNAME")
                    );
            }
            else
            {
                sql = @"update config_filesplit set busiunitcode='{0}',customercode='{1}',repunitcode='{2}',busitype='{3}',filetype='{4}',createuserid='{5}'
                                    ,createusername='{6}',createtime=sysdate
                                where id={7}";
                sql = string.Format(sql
                        , json.Value<string>("BUSIUNITCODE"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("REPUNITCODE"), json.Value<string>("BUSITYPE"), json.Value<string>("FILETYPE"), json_user.Value<string>("ID")
                        , json_user.Value<string>("REALNAME"), json.Value<string>("ID")
                   );
            }

            int i = DBMgr.ExecuteNonQuery(sql);

            string response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
            Response.Write(response);
            Response.End();
        }

        private void deleteData()
        {
            string id = Request["ID"];
            string sql = "delete from config_filesplit where id='{0}'";
            string str = DBMgr.ExecuteNonQuery(string.Format(sql, id)) > 0 ? "true" : "false";
            Response.Write("{\"success\":" + str + "}");
            Response.End();
        }
    }
}