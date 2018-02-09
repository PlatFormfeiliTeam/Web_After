using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Web_After.Sql;

namespace Web_After.BasicManager.InspInfor
{
    public partial class Base_year : System.Web.UI.Page
    {
        IsoDateTimeConverter iso = new IsoDateTimeConverter();
        int totalProperty = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                string action = Request["action"];
                iso.DateTimeFormat = "yyyy-MM-dd";


                switch (action)
                {
                    case "loadData":
                        loadDate();
                        break;
                    case "save":
                        save(Request["formdata"]);
                        break;
                }
            }
            
        }

        private void loadDate()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["HsCodeBase"]))
            {
                strWhere = strWhere + " and t1.Name like '%" + Request["HsCodeBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["start_date"]))
            {
                strWhere = strWhere + " and t1.startDate >= to_date('" + Request["start_date"] + "','yyyy-mm-dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(Request["end_date"]))
            {
                strWhere = strWhere + " and t1.endDate < to_date('" + Request["end_date"] + "','yyyy-mm-dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.Base_year by = new Sql.Base_year();
            DataTable dt = by.LoaData(strWhere, "ID", "desc", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();

        }

        public string Username()
        {
            string userName = "";
            FormsIdentity identity = User.Identity as FormsIdentity;
            if (identity == null)
            {
                return "";
            }
            return userName = identity.Name;
        }

        private void save(string formdata)
        {
            //从前端获取值
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);

        }
    }
}