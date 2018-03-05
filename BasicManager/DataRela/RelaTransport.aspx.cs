using Aspose.Cells;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.BasicManager.DataRela
{
    public partial class RelaTransport : System.Web.UI.Page
    {

        IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
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
                    case "save":
                        //save(Request["formdata"]);
                        break;
                    case "export":
                        //export();
                        break;
                    case "add":
                        //ImportExcelData();
                        break;
                    case "Ini_Base_Data":
                        Ini_Base_Data();
                        break;
                }
            }
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

        public void Ini_Base_Data()
        {
            string sql = "";
            string DECTRANSPORT = "[]";//报关
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')'  as NAME FROM base_transport where CODE is not null and enabled=1";
            DECTRANSPORT = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            string INSPTRANSPORT = "[]";//报检
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')' as NAME FROM base_inspconveyance where CODE is not null and enabled=1";
            INSPTRANSPORT = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            Response.Write("{DECTRANSPORT:" + DECTRANSPORT + ",INSPTRANSPORT:" + INSPTRANSPORT + "}");
            Response.End();
        }

        private void loadData()
        {
            string strWhere = " where 1=1 ";
            if (!string.IsNullOrEmpty(Request["DECLTRANSPORTCODE"]))
            {
                strWhere = strWhere + " and t1.decltransport like '%" + Request["DECLTRANSPORTCODE"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["DECLTRANSPORTNAME"]))
            {
                strWhere = strWhere + " and t2.name like '%" + Request["DECLTRANSPORTNAME"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.RelaTransport bc = new Sql.RelaTransport();
            DataTable dt = bc.LoaData(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }


    }
}