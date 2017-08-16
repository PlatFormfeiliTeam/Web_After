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

namespace Web_After.BasicManager
{
    public partial class UserList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"];
            switch (action)
            {
                case "loadchildaccount":
                    loadChildAccount();
                    break;
                case "loaduser":
                    loadUser();
                    break;
                case "inipsd":
                    inipsd();
                    break;
                case "enabled":
                    enabled();
                    break;
                case "delete":
                    deleteData();
                    break;
                case "Ini_Base_Data":
                    Ini_Base_Data();
                    break;
                case "save":                    
                    save(Request["formdata"]);
                    break;
            }
        }

        /// <summary>
        /// 获取子节点
        /// </summary>
        private void loadChildAccount()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string id = Request["id"];
            string sql = @"SELECT * FROM SYS_USER WHERE PARENTID = " + id + " order by CREATETIME ASC";
            DataTable dt = DBMgr.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{innerrows:" + json + "}");
            Response.End();
        }
        /// <summary>
        /// 查询
        /// </summary>
        private void loadUser()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string groupid = Request["groupid"];
            string where = "";
            if (!string.IsNullOrEmpty(Request["NAME_S"]))
            {
                where += " and NAME like '%" + Request["NAME_S"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["REALNAME_S"]))
            {
                where += " and REALNAME like '%" + Request["REALNAME_S"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["POSITIONID_S"]))
            {
                where += " and POSITIONID =" + Request["POSITIONID_S"];
            }
            //主账号
            string sql = @"SELECT * FROM SYS_USER WHERE TYPE = 1 " + where;
            int totalProperty = 0;
            sql = Extension.GetPageSql(sql, "CREATETIME", "desc", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }
        private void inipsd()
        {
            string id = Request["id"];
            string name = Request["name"];
            string sql = "update sys_user set points=0,PASSWORD='{0}' where id='{1}'";
            sql = string.Format(sql, Extension.ToSHA1(name), id);
            Response.Write(DBMgr.ExecuteNonQuery(sql));
            Response.End();
        }
        /// <summary>
        /// 启禁用
        /// </summary>
        private void enabled()
        {
            string id = Request["id"];
            string flag = Request["flag"];
            string sql = "update sys_user set enabled='{0}' where id='{1}'";
            string str = DBMgr.ExecuteNonQuery(string.Format(sql, flag, id)) > 0 ? "true" : "false";
            Response.Write("{\"success\":" + str + "}");
            Response.End();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        private void deleteData()
        {
            string id = Request["ID"];
            string sql = "delete from sys_user where id='{0}'";
            string str = DBMgr.ExecuteNonQuery(string.Format(sql, id)) > 0 ? "true" : "false";
            Response.Write("{\"success\":" + str + "}");
            Response.End();
        }

        private void Ini_Base_Data()
        {
            string sql = "";

            string json_ksdh = "";
            sql = "select id code,name from cusdoc.sys_customer where enabled=1";
            json_ksdh = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));

            Response.Write("{ksdh:" + json_ksdh + "}");
            Response.End();
        }

        private void save(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);

            string sql = "";
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                sql = @"insert into sys_user(id,name,realname,telephone,mobilephone,email,customerid,companyids,positionid,enabled,remark,createtime,type,password) 
                                        values(sys_user_id.nextval,'{0}','{1}','{2}', '{3}','{4}',{5},{6},'{7}','{8}','{9}',sysdate,1,'{10}')";
                sql = string.Format(sql,
                    json.Value<string>("NAME"),
                    json.Value<string>("REALNAME"),
                    json.Value<string>("TELEPHONE"),
                    json.Value<string>("MOBILEPHONE"),
                    json.Value<string>("EMAIL"),
                    json.Value<string>("CUSTOMERID"),
                    "(select NAME from cusdoc.sys_customer where code='" + json.Value<string>("CUSTOMERID") + "')",
                    json.Value<string>("POSITIONID"),
                    json.Value<string>("ENABLED"),
                    json.Value<string>("REMARK"),
                    json.Value<string>("NAME").ToSHA1());
            }
            else
            {
                sql = @"update sys_user set name='{0}',realname='{1}',telephone='{2}',mobilephone='{3}',email='{4}',customerid={5},companyids='{6}',
                                positionid='{7}',enabled='{8}',remark='{9}' where id={10}";
                sql = string.Format(sql,
                    json.Value<string>("NAME"),
                    json.Value<string>("REALNAME"),
                    json.Value<string>("TELEPHONE"),
                    json.Value<string>("MOBILEPHONE"),
                    json.Value<string>("EMAIL"),
                    json.Value<string>("CUSTOMERID"),
                    "(select id from cusdoc.sys_customer where code='" + json.Value<string>("CUSTOMERID") + "')",
                   json.Value<string>("POSITIONID"),
                    json.Value<string>("ENABLED"),
                    json.Value<string>("REMARK"),
                    json.Value<string>("ID"));
            }

            int i = DBMgr.ExecuteNonQuery(sql);

            string response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
            Response.Write(response);
            Response.End();
        }
    }
}