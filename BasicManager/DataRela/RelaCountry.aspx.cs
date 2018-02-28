﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.BasicManager.DataRela
{
    public partial class RelaCountry : System.Web.UI.Page
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

        private void Ini_Base_Data()
        {
            string sql = "";
            string DECLCOUNTRY = "[]";//报关国家码
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')'  as NAME FROM base_country where CODE is not null and enabled=1";
            DECLCOUNTRY = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            string INSPCOUNTRY = "[]";//报检国家码
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')' as NAME FROM base_inspcountry where CODE is not null and enabled=1";
            INSPCOUNTRY = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            Response.Write("{DECLCOUNTRY:" + DECLCOUNTRY + ",INSPCOUNTRY:" + INSPCOUNTRY + "}");
            Response.End();
        }

        private void loadData()
        {
            string strWhere = " where 1=1 ";
            if (!string.IsNullOrEmpty(Request["DECLCOUNTRYCODE"]))
            {
                strWhere = strWhere + " and t1.declcountry like '%" + Request["DECLCOUNTRYCODE"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["DECLCOUNTRYNAME"]))
            {
                strWhere = strWhere + " and t2.name like '%" + Request["DECLCOUNTRYNAME"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.RelaCountry bc = new Sql.RelaCountry();
            DataTable dt = bc.LoaData(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
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
    }
}