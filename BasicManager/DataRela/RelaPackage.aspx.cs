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
    public partial class RelaPackage : System.Web.UI.Page
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
                        //loadData();
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


        public void Ini_Base_Data()
        {
            string sql = "";
            string DECLPACKAGE = "[]";//报关包装类型
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')'  as NAME FROM base_packing where CODE is not null and enabled=1";
            DECLPACKAGE = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            string INSPPACKAGE = "[]";//报检包装类型
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')' as NAME FROM base_insppackage where CODE is not null and enabled=1";
            INSPPACKAGE = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            Response.Write("{DECLPACKAGE:" + DECLPACKAGE + ",INSPCOUNTRY:" + INSPPACKAGE + "}");
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
    }
}