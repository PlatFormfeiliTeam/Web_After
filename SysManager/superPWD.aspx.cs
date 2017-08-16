using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.SysManager
{
    public partial class superPWD : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"];
            switch (action)
            {
                case "show":
                    DataTable dt = DBMgr.GetDataTable("select PWD from sys_superpwd");
                    string json = JsonConvert.SerializeObject(dt);
                    Response.Write(json);
                    Response.End();
                    break;

                case "update":
                    string pwd = Request["pwd"];
                    DBMgr.ExecuteNonQuery("update  sys_superpwd set PWD='" + pwd + "'");
                    Response.Write("{success:true}");
                    Response.End();
                    break;

            }
        }
    }
}