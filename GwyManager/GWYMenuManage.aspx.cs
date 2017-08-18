using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.GwyManager
{
    public partial class GWYMenuManage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string action = Request["action"];
                switch (action)
                {
                    case "loadMenu":
                        loadMenu();
                        break;
                    case "add":
                        save();
                        break;
                    case "update":
                        update();
                        break;
                    case "delete":
                        delete();
                        break;
                }
            }

        }
        private void loadMenu()
        {
            string sql = "select * from t_menu order by menuid";
            DataTable dt = DBMgr.GetDataTable(sql);
            string result = "[";
            //["id", "name", "leaf", "ParentID","frmname","assemblyname"] });
            result += getChildren(dt, "-1");
            result = result + "]";
            Response.Write(result);
            Response.End();
        }
        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string getChildren(DataTable dt, string parentid)
        {
            string result = "";
            try
            {
                DataRow[] rows = dt.Select("parentmenuid='" + parentid + "'");
                foreach (DataRow dr in rows)
                {
                    string children = getChildren(dt, dr["menuid"].ToString2());
                    if (children.Length > 1)
                    {
                        result += "{id:'" + dr["menuid"] + "',name:'" + dr["menuname"] + "',leaf:false,parentid:'" + dr["parentmenuid"] + "',frmname:'" + dr["frmname"] + "',assemblyname:'" + dr["assemblyname"] + "',args:'" + dr["args"] + "',remark:'" + dr["remark"] + "',children:[" + children + "]},";
                    }
                    else
                    {
                        result += "{id:'" + dr["menuid"] + "',name:'" + dr["menuname"] + "',leaf:true,parentid:'" + dr["parentmenuid"] + "',frmname:'" + dr["frmname"] + "',assemblyname:'" + dr["assemblyname"] + "',args:'" + dr["args"] + "',remark:'" + dr["remark"] + "',children:[]},";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (result.Length > 0) result = result.Substring(0, result.Length - 1);
            return result;
        }
        private void save()
        {
            try
            {
                string id = Request["ID"];
                JObject json = (JObject)JsonConvert.DeserializeObject(Request["json"]);
                string sql = "insert into t_menu(menuid,menuname,frmname,parentmenuid,assemblyname,args,remark) values(t_menu_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}')";
                sql = string.Format(sql, json.Value<string>("name"), json.Value<string>("frmname"), id, json.Value<string>("assemblyname"), json.Value<string>("args"), json.Value<string>("remark"));
                string flag = DBMgr.ExecuteNonQuery(sql) > 0 ? "true" : "false";
                Response.Write("{\"success\":" + flag + "}");
                Response.End();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void update()
        {
            try
            {
                string id = Request["ID"];
                JObject json = (JObject)JsonConvert.DeserializeObject(Request["json"]);
                string sql = "update t_menu set menuname='{0}',frmname='{1}',assemblyname='{2}',args='{3}',remark='{4}' where menuid='{5}'";
                sql = string.Format(sql, json.Value<string>("name"), json.Value<string>("frmname"), json.Value<string>("assemblyname"), json.Value<string>("args"), json.Value<string>("remark"), id);
                string flag = DBMgr.ExecuteNonQuery(sql) > 0 ? "true" : "false";
                Response.Write("{\"success\":" + flag + "}");
                Response.End();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void delete()
        {
            string id = Request["ID"];
            List<string> sqls = new List<string>();
            sqls.Add("delete from t_menu where menuid ='" + id + "'");
            sqls.Add("delete from t_menu where parentmenuid ='" + id + "'");
            string flag = DBMgr.ExecuteNonQuery(sqls) > 0 ? "true" : "false";
            Response.Write("{\"success\":" + flag + "}");
            Response.End();
        }
    }
}