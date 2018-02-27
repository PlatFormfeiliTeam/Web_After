using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
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
    public partial class RelaHSCIQ : System.Web.UI.Page
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
                        save(Request["formdata"]);
                        break;
                    case "export":
                        //export();
                        break;
                    case "add":
                        //ImportExcelData();
                    case "Ini_Base_Data":
                        Ini_Base_Data();
                        break;

                }
            }
        }

        private void Ini_Base_Data()
        {
            string sql = "";
            string HS = "[]";
            sql = "SELECT HSCODE||EXTRACODE as CODE,HSNAME||'('||HSCODE||EXTRACODE||')'  as NAME FROM base_insphs where HSCODE is not null and enabled=1";
            HS = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            string CIQ = "[]";//委托单位
            sql = "SELECT CIQ as CODE,CIQNAME||'('||CIQ||')' as NAME FROM base_ciqcode where enabled=1";
            CIQ = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            Response.Write("{HS:" + HS + ",CIQ:" + CIQ +"}");
            Response.End();
        }
        private void loadData()
        {
            string strWhere = " where t1.enabled=1 and t2.enabled=1 ";
            if (!string.IsNullOrEmpty(Request["CODE_HS"]))
            {
                strWhere = strWhere + " and t1.hscode like '%" + Request["CODE_HS"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["NAME_HS"]))
            {
                strWhere = strWhere + " and t2.hsname like '%" + Request["NAME_HS"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["CODE_CIQ"]))
            {
                strWhere = strWhere + " and t1.ciqcode like '%" + Request["CODE_CIQ"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["NAME_CIQ"]))
            {
                strWhere = strWhere + " and t3.ciqname like '%" + Request["NAME_CIQ"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.RelaHSCCIQ bc = new Sql.RelaHSCCIQ();
            DataTable dt = bc.LoaData(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }

        public void save(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            Sql.RelaHSCCIQ bcsql = new Sql.RelaHSCCIQ();
            //禁用人
            string stopman = "";
            //返回重复结果
            string repeat = "";
            //返回前端的值
            string response = "";

            if (json.Value<string>("ENABLED") == "1")
            {
                stopman = "";
            }
            else
            {
                FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
                string userName = identity.Name;
                JObject json_user = Extension.Get_UserInfo(userName);
                stopman = (string)json_user.GetValue("ID");
            }

            //插入
            if (String.IsNullOrEmpty(json.Value<string>("ID")))
            {
                List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("HSCODE"), json.Value<string>("CIQCODE"));
                repeat = bcsql.Check_Repeat(retunRepeat);
                if (repeat == "")
                {
                    //insert数据向表base_company当是5时插入成功
                    bcsql.insert_rela_hs_ciq(json, stopman);
                    repeat = "5";
                }
            }
            else
            {
                //更新
                List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("HSCODE"), json.Value<string>("CIQCODE"));
                repeat = bcsql.Check_Repeat(retunRepeat);
                if (repeat == "")
                {
                    //insert数据向表base_company当是5时插入成功
                    DataTable dt = bcsql.LoadDataById(json.Value<string>("ID"));
                    int i  = bcsql.update_rela_hs_ciq(json, stopman);
                    if (i > 0)
                    {
                        bcsql.insert_base_alterrecord(json, dt);
                    }
                    repeat = "5";
                }
            }

            response = "{\"success\":\"" + repeat + "\"}";

            Response.Write(response);
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