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
using Web_After.BasicManager.BasicManager;
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
                    case "MaintainloadData":
                        MaintainloadData();
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
            string response = String.Empty;
            Sql.Base_year by = new Sql.Base_year();
            Base_Codename_Method bcm = new Base_Codename_Method();
            //从前端获取值
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                //查询是否有重复值
                if (json.Value<string>("ENABLED") == "1")
                {
                    DataSet  dt = by.check_base_year(json);
                    if (dt.Tables[0].Rows.Count>0)
                    {
                        //当数据有重复时success返回值为4
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        int i = by.insertTable(json);
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }

                }
                else
                {
                    
                    int i = by.insertTable(json);
                    response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }
            }
            else
            {
                //查询是否有重复值
                if (json.Value<string>("ENABLED") == "1")
                {
                    DataSet dt = by.check_base_year_by_idandname(json);
                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        //当数据有重复时success返回值为4
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        DataTable data = by.getBeforeChangData(json);
                        string content = bcm.getChangeBase_year(data, json);
                        int i = by.update_base_year(json);
                        if (i>0)
                        {
                            //插入修改记录
                            by.insert_base_alterrecord(json, content);
                        }
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }
                }
                else
                {
                    DataTable data = by.getBeforeChangData(json);
                    string content = bcm.getChangeBase_year(data, json);
                    int i = by.update_base_year(json);
                    if (i>0)
                    {
                        //插入修改记录
                        by.insert_base_alterrecord(json, content);
                    }
                    response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }
            }
            Response.Write(response);
            Response.End();
        }

        //CIQ代码维护界面数据加载
        private void MaintainloadData()
        {
            string ID = Request["id"];
            string strWhere = string.Empty;
            Sql.Base_year by = new Sql.Base_year();

            if (!string.IsNullOrEmpty(Request["HsCodeSearch"]))
            {
                strWhere = strWhere + " and t1.hscode like '%" + Request["HsCodeSearch"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["HsNameSearch"]))
            {
                strWhere = strWhere + " and t1.hsname like '%" + Request["HsNameSearch"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S2"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S2"] + "'";
            }
            DataTable dt = by.MaintainloadData(ID, strWhere, "", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }
    }
}