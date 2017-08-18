using Newtonsoft.Json;
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
    public partial class GWYRoleManage : System.Web.UI.Page
    {
        string companyid = string.Empty; string roleid = string.Empty;
        string sql = string.Empty;
        DataTable configDT = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"]; roleid = Request["roleid"];
            switch (action)
            {
                case "loadManager":
                    loadManager();
                    break;
                case "loadmenu":
                    loadMenu();
                    break;
                case "saveMenuConfig":
                    saveMenuConfig();
                    break;
            }
        }
        private void loadManager()
        {
            //是单证服务单位的主账号
            string sql = "select * from cusdoc.sys_customer where docservicecompany=1";
            DataTable dt = DBMgr.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt);
            Response.Write("{rows:" + json + "}");
            Response.End();
        }
        private void loadMenu()
        {
            companyid = Request["roleid"].ToString2();
            string sql = "select * from t_menu order by menuid";
            DataTable dt = DBMgr.GetDataTable(sql);
            sql = "select * from t_rolemenu";
            configDT = DBMgr.GetDataTable(sql);
            string result = "[";
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
                    string check = isCheck(dr["menuid"].ToString2());
                    if (children.Length > 1)
                    {
                        result += "{id:'" + dr["menuid"] + "',name:'" + dr["menuname"] + "',checked:" + check + ",leaf:false,parentid:'" + dr["parentmenuid"] + "',frmname:'" + dr["frmname"] + "',assemblyname:'" + dr["assemblyname"] + "',args:'" + dr["args"] + "',remark:'" + dr["remark"] + "',children:[" + children + "]},";
                    }
                    else
                    {
                        result += "{id:'" + dr["menuid"] + "',name:'" + dr["menuname"] + "',checked:" + check + ",leaf:true,parentid:'" + dr["parentmenuid"] + "',frmname:'" + dr["frmname"] + "',assemblyname:'" + dr["assemblyname"] + "',args:'" + dr["args"] + "',remark:'" + dr["remark"] + "',children:[]},";
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
        private string isCheck(string menuid)
        {
            DataRow[] rows = configDT.Select("menuid='" + menuid + "' and companyid='" + companyid + "'");
            if (rows.Length > 0)
                return "true";
            else
                return "false";
        }
        private void saveMenuConfig()
        {
            string companyid = Request["roleid"];
            string moduleids = Request["moduleids"];
            string sql = "delete from t_rolemenu where companyid='" + companyid + "'";
            DBMgr.ExecuteNonQuery(sql);
            string flag = "true";
            if (!string.IsNullOrEmpty(moduleids))
            {
                string[] ids = moduleids.Split(',');
                List<string> sqls = new List<string>();
                foreach (string id in ids)
                {
                    if (!string.IsNullOrEmpty(id) && id != "-1")
                    {
                        sql = "insert into t_rolemenu(menuid,companyid) values('{0}','{1}')";
                        sqls.Add(string.Format(sql, id, companyid));
                    }
                }
                flag = DBMgr.ExecuteNonQuery(sqls) > 0 ? "true" : "false";
            }
            Response.Write("{\"success\":" + flag + "}");
            Response.End();
        }
    }
}