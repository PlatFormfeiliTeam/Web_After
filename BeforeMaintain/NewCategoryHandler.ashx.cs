using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Web_After.Common;

namespace Web_After
{
    /// <summary>
    /// NewCategoryHandler 的摘要说明
    /// </summary>
    public class NewCategoryHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(getCate());
        }

        public string getCate()
        {
            string result = ""; string sql = string.Empty;
            sql = @"select t.* from NEWSCATEGORY t where t.PID is null order by sortindex";

            result = "[";
            DataTable dt = DBMgr.GetDataTable(sql);
            int i = 0;
            string children = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                children = getchildren(dr["id"].ToString());
                result += "{ID:'" + dr["id"] + "',NAME:'" + dr["NAME"] + "',PID:'" + dr["PID"] + "',leaf:'" + dr["ISLEAF"] + "',checked:false,children:" + children + "}";

                if (i != dt.Rows.Count - 1)
                {
                    result += ",";
                }
                i++;
            }
            result += "]";
            return result;
        }

        private string getchildren(string id)
        {
            string sql = string.Empty;

            string children = "[";
            sql = @"select t.* from NEWSCATEGORY t where  t.PID ='" + id + "' order by sortindex";

            DataTable dt = DBMgr.GetDataTable(sql);
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string tmp_children = getchildren(dr["id"].ToString());

                children += "{ID:'" + dr["id"] + "',NAME:'" + dr["NAME"] + "',PID:'" + dr["PID"] + "',leaf:'" + dr["ISLEAF"] + "',checked:false,children:" + tmp_children + "}";

                if (i != dt.Rows.Count - 1)
                {
                    children += ",";
                }
                i++;
            }
            children += "]";
            return children;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}