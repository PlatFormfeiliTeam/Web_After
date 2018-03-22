using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Web_After.BasicManager.InspInfor
{
    public partial class busi_RecordInfor : System.Web.UI.Page
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
                        loadData();
                        break;
                }
            }
        }

        private void loadData()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["CodeBase"]))
            {
                strWhere = strWhere + " and t1.code like '%" + Request["CodeBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["RecordBase"]))
            {
                strWhere = strWhere + " and t1.busiunit like '%" + Request["RecordBase"] + "%'";
            }

            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.busi_RecordInfor bc = new Sql.busi_RecordInfor();
            DataTable dt = bc.LoaData(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }
    }
}