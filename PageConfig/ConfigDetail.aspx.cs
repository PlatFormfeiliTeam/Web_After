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
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;
using Web_After.PageConfig.PageconfigEntity;

namespace Web_After.PageConfig
{
    public partial class ConfigDetail : System.Web.UI.Page
    {
        string parentid = "";
        WEB_PAGECONFIG parentInfo = null;
        IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
        int totalProperty = 0;
        protected void Page_Load(object sender, EventArgs e)
        {

            parentid = Request["parentid"];
            parentInfo = GetParentInfo(parentid);

            if (!IsPostBack)
            {
                string action = Request["action"];
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
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
                        //Ini_Base_Data();
                        break;
                    case "loadbasebusitype":
                        GetBusitype();
                        break;
                    case "loadbasebusidetail":
                        GetBusiDetail();
                        break;
                    case "loadbasecustomercode":
                        GetCustomerCode();
                        break;
                    case "loadparentinfo":
                        GetParentInfoForSeach();
                        break;
                }
            }
        }

        public void GetParentInfoForSeach()
        {
            parentid = Request["parentid"];
            string parentinfo = "[]";
            parentInfo = GetParentInfo(parentid);
            parentinfo = JsonConvert.SerializeObject(parentInfo);
            Response.Write("{" + "parentinfo:" + parentinfo + "}");
            Response.End();
        }

        /// <summary>
        /// 获取主配置的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WEB_PAGECONFIG GetParentInfo(string id)
        {
            WEB_PAGECONFIG en = new WEB_PAGECONFIG();
            string sqlStr = "select * from web_pageconfig t1 where t1.id='{0}'";
            sqlStr = string.Format(sqlStr, id);
            DataTable dt = DBMgr.GetDataTable(sqlStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                en.ID = Convert.ToInt32(dt.Rows[0]["ID"].ToString());
                en.CODE = dt.Rows[0]["CODE"].ToString();
                en.NAME = dt.Rows[0]["NAME"].ToString();
                en.PAGENAME = dt.Rows[0]["PAGENAME"].ToString();
                en.CONFIGCONTENT = dt.Rows[0]["CONFIGCONTENT"].ToString();
                string[] array = en.CONFIGCONTENT.Split(';');
                en.BUSITYPE = array[0].Split('=')[1];
                en.BUSIDETAIL = array[1].Split('=')[1];
                en.CUSTOMERCODE = dt.Rows[0]["CUSTOMERCODE"].ToString();
                return en;
            }
            else
            {
                return null;
            }
        }

        #region

        /// <summary>
        /// 业务细项
        /// </summary>
        public void GetBusiDetail()
        {
            string sql = "";
            string busidetail = "[]";
            sql = "select  t1.busiitemcode as code, t1.busiitemname||t1.busiitemcode as name,t1.busitypecode as busitype from web_customsconfig t1 where t1.enable='1'";
            busidetail = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            Response.Write("{" + "busidetail:" + busidetail + "}");
            Response.End();
        }

        /// <summary>
        /// 业务类型
        /// </summary>
        public void GetBusitype()
        {
            string sql = "";
            string busitype = "[]";
            sql = "select distinct t1.busitypecode as code, t1.busitypename||t1.busitypecode as name from web_customsconfig t1 where t1.enable='1'";
            busitype = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            Response.Write("{" + "busitype:" + busitype + "}");
            Response.End();
        }

        public void GetCustomerCode()
        {
            string sql = "";
            string customercode = "[]";
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')'  as NAME FROM sys_customer where CODE is not null and enabled=1";
            customercode = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            Response.Write("{" + "customercode:" + customercode + "}");
            Response.End();
        }
        #endregion
    }
}