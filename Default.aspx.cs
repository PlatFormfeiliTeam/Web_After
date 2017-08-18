using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;
namespace Web_After
{
    public partial class SysFrame : System.Web.UI.Page
    {
        public string userName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            FormsIdentity identity = User.Identity as FormsIdentity;
            if (identity == null)
            {
                return;
            }
            userName = identity.Name;
            string id = Request["id"];
            string strAction = Request["action"];

            if (strAction == "logout")
            {
                FormsAuthentication.SignOut();
            }

            //人员权限
            string sql = "";

            if (!string.IsNullOrEmpty(id))
            {
                sql = @"select ModuleID,Name,IsLeaf,Url,icon from sysmodule 
                        where parentid ='{0}' 
                            and ModuleID in (select distinct moduleid from sys_moduleuser_back where userid in (select id from sys_user where name = '{1}')) order by SortIndex";

                if (id == "-1")
                {
                    sql = string.Format(sql, "91a0657f-1939-4528-80aa-91b202a593ac", userName);  
                }
                else
                {
                    sql = string.Format(sql, id, userName);  
                }
                              
                DataTable ents = DBMgr.GetDataTable(sql);

                int i = 0;
                string result = "[";
                foreach (DataRow smEnt in ents.Rows)
                {
                    result += "{id:'" + smEnt["ModuleID"] + "',name:'" + smEnt["Name"] + "',leaf:'" + smEnt["IsLeaf"] + "',url:'" + smEnt["Url"] + "',iconCls:'no-icon',iconmy:'" + smEnt["ICON"] + "'}";

                    if (i != ents.Rows.Count - 1)
                    {
                        result += ",";
                    }
                    i++;
                }
                result += "]";
                Response.Write(result);
                Response.End();
            }

        }
    }
}