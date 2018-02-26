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

namespace Web_After.BasicManager.InspInfor
{
    public partial class Base_Container : System.Web.UI.Page
    {
        IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
        int totalProperty = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string action = Request["action"];
                string table = Request["table"];
                iso.DateTimeFormat = "yyyy-MM-dd";
                switch (action)
                {
                    case "loadData":
                        loadData(table);
                        break;
                    case "save":
                        save(Request["formdata"]);
                        break;
                    case "export":
                        //export(table);
                        break;
                    case "add":
                        //ImportExcelData(table);
                        break;

                }
            }
        }

        private void loadData(string table)
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["CODE_S"]))
            {
                strWhere = strWhere + " and t1.Code like '%" + Request["CODE_S"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["CNNAME_S"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["CNNAME_S"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.Base_Container bc = new Sql.Base_Container();
            DataTable dt = bc.LoaData(table, strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();

        }

        public void save(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            Sql.Base_Container bcsql = new Sql.Base_Container();
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

            if (String.IsNullOrEmpty(json.Value<string>("ID")))
            {
                    List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("CODE"), json.Value<string>("NAME"),
                        json.Value<string>("HSCODE"));

                    repeat = bcsql.Check_Repeat(retunRepeat);
                    if (repeat == "")
                    {
                        //insert数据向表base_company当是5时插入成功
                        bcsql.insert_base_container(json, stopman);
                        repeat = "5";

                    }
            }
            else
            {
                List<int> retunRepeat = bcsql.CheckRepeat(json.Value<string>("ID"), json.Value<string>("CODE"),
                        json.Value<string>("NAME"),
                        json.Value<string>("HSCODE"));
                repeat = bcsql.Check_Repeat(retunRepeat);
                if (repeat == "")
                {
                    DataTable dt = bcsql.LoadDataById(json.Value<string>("ID"));
                    int i = bcsql.update_base_container(json, stopman);
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