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
    public partial class GWYButtonManage : System.Web.UI.Page
    {
        string companyid = string.Empty; string roleid = string.Empty;
        string sql = string.Empty;
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
                case "saveButtonConfig":
                    saveButtonConfig();
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
            string strSql = "select * from t_formbutton where enabled='1' order by formname";
            DataTable dt = DBMgr.GetDataTable(strSql);
            string configSql = "select * from t_roleformbutton";
            DataTable configDT = DBMgr.GetDataTable(configSql);
            string result = "[";
            string formName = "";
            foreach (DataRow dr in dt.Rows)
            {
                if (formName == dr["formname"].ToString2())
                    continue;
                string children = getChildren(dt, dr["formname"].ToString2(), configDT);
                string check = isCheck_form(configDT, dr["formname"].ToString2());
                if (children.Length > 1)
                {
                    result += "{name:'" + dr["formcaption"] + "',leaf:false,ParentID:'root',code:'" + dr["formname"] + "',children:" + children + ",checked:" + check + "},";
                }
                else
                {
                    result += "{name:'" + dr["formcaption"] + "',leaf:true,ParentID:'root',code:'" + dr["formname"] + "',checked:" + check + "},";
                }
                formName = dr["formname"].ToString2();
            }
            if (result.Length > 1) result = result.Substring(0, result.Length - 1) + "]";
            Response.Write(result);
            Response.End();
        }
        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string getChildren(DataTable dt, string name, DataTable cdt)
        {
            string str = "[";
            DataRow[] rows = dt.Select("formname='" + name + "'");
            foreach (DataRow dr in rows)
            {
                string check = isCheck_btn(cdt, dr["buttonname"].ToString2(), dr["formname"].ToString2());
                str += "{name:'" + dr["buttoncaption"] + "',leaf:true,ParentID:'" + dr["formname"] + "',code:'" + dr["buttonname"] + "',checked:" + check + "},";
            }
            if (str.Length > 1) str = str.Substring(0, str.Length - 1);
            str += "]";
            return str;
        }
        private string isCheck_form(DataTable dt, string name)
        {
            DataRow[] rows = dt.Select("formname='" + name + "' and companyid='" + companyid + "'");
            if (rows.Length > 0) return "true";
            else return "false";
        }
        private string isCheck_btn(DataTable dt, string btnname, string formname)
        {
            DataRow[] rows = dt.Select("buttonname='" + btnname + "' and formname='" + formname + "' and companyid='" + companyid + "'");
            if (rows.Length > 0) return "true";
            else return "false";
        }
        private void saveButtonConfig()
        {
            string companyid = Request["roleid"];
            string moduleids = Request["moduleids"];
            string sql = "delete from t_roleformbutton where companyid='" + companyid + "'";
            DBMgr.ExecuteNonQuery(sql);
            string flag = "true";
            if (!string.IsNullOrEmpty(moduleids))
            {
                string[] ids = moduleids.Split(',');
                List<string> sqls = new List<string>();
                foreach (string id in ids)
                {
                    string formname = string.Empty, buttonname = string.Empty;
                    string[] names = id.Split('|');
                    if (names.Length > 0)
                        formname = names[0];
                    if (names.Length > 1)
                        buttonname = names[1];
                    if (string.IsNullOrEmpty(formname) || formname.ToLower() == "root")
                        continue;
                    sql = "insert into t_roleformbutton(formname,buttonname,companyid) values('{0}','{1}','{2}')";
                    sqls.Add(string.Format(sql, formname, buttonname, companyid));
                }
                flag = DBMgr.ExecuteNonQuery(sqls) > 0 ? "true" : "false";
            }
            Response.Write("{\"success\":" + flag + "}");
            Response.End();
        }
    }
}