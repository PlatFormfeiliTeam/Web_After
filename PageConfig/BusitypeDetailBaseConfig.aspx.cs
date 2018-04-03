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
    public partial class BusitypeDetailBaseConfig : System.Web.UI.Page
    {
        IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
        int totalProperty = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string action = Request["action"];
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
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
                    case "Ini_Base_Data":
                        //Ini_Base_Data();
                        break;
                    case "loadbasebusitype":
                        GetBusitype();
                        break;
                    case "loadbasebusidetail":
                        GetBusiDetail();
                        break;
                    case "delete":
                        Delete();
                        break;

                }
            }

        }

        [WebMethod]
        public static string EnableConfig(string id)
        {
            string[] array = id.Split(';');
            foreach (var item in array)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    string sqlStr = "update web_customsconfig t1 set t1.enable='1' where t1.id='{0}'";
                    sqlStr = string.Format(sqlStr, item);
                    DBMgr.ExecuteNonQuery(sqlStr);
                }
            }
            return "成功";
        }

        [WebMethod]
        public static string DisableConfig(string id)
        {
            string[] array = id.Split(';');
            foreach (var item in array)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    string sqlStr = "update web_customsconfig t1 set t1.enable='0' where t1.id='{0}'";
                    sqlStr = string.Format(sqlStr, item);
                    DBMgr.ExecuteNonQuery(sqlStr);
                }
            }
            return "成功";
        }

        public void Delete()
        {
            string repeat = "";
            string response = "";

            string id = Request["deleteid"];
            try
            {
                string sqlStr = "delete from web_customsconfig where id='" + id + "'";
                DBMgr.ExecuteNonQuery(sqlStr);
                repeat = "5";
            }
            catch
            {
                repeat = "删除异常";
            }
            response = "{\"success\":\"" + repeat + "\"}";
            Response.Write(response);
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
        /// 加载数据
        /// </summary>
        public void loadData()
        {
            //业务类型
            string busitype = Request["SEARCH_BUSITYPE"];
            //业务细项
            string busidetail = Request["SEARCH_BUSIDETAIL"];

            //是否启用 1-启用，0-禁用
            string enabled = Request["SEARCH_ENABLED"];
            string strWhere = "where 1=1 ";

            if (!string.IsNullOrEmpty(busitype))
            {
                strWhere += " and t1.busitypecode like '%" + busitype + "%' ";
            }
            if (!string.IsNullOrEmpty(busidetail))
            {
                strWhere += " and t1.busiitemcode like '%" + busidetail + "%' ";
            }
            if (!string.IsNullOrEmpty(enabled))
            {
                strWhere += " and t1.enable = '" + enabled + "' ";
            }
            string sqlStr = "select t1.* from web_customsconfig t1 " + strWhere;

            sqlStr = Extension.GetPageSql(sqlStr, "t1.busitypecode", "", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            DataTable loDataSet = DBMgr.GetDataTable(sqlStr);
            string json = JsonConvert.SerializeObject(loDataSet, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }

        /// <summary>
        /// 保存或更新数据
        /// </summary>
        /// <param name="formdata"></param>
        public void save(string formdata)
        {
            string response = "";
            string repeat = "";
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            WEB_CUSTOMSCONFIG en = JsonToEntity(json);
            if (en == null)
            {
                repeat = "保存失败，JSON数据转换出现问题";
            }
            else if (en.ID < 0)
            {
                //新增
                repeat = CanUpdateOrInsert(en);
                if (string.IsNullOrEmpty(repeat))
                {
                    int i = AddConfig(en);
                    if (i > 0)
                    {
                        repeat = "5";//代表成功
                    }
                }
            }
            else
            {
                //更新
                repeat = CanUpdateOrInsert(en);
                if (string.IsNullOrEmpty(repeat))
                {
                    int i = UpdateConfig(en);
                    if (i > 0)
                    {
                        repeat = "5";//代表成功
                    }
                }
            }
            response = "{\"success\":\"" + repeat + "\"}";
            Response.Write(response);
            Response.End();
        }

        public int AddConfig(WEB_CUSTOMSCONFIG en)
        {
            string sqlStr = @"insert into web_customsconfig (ID,busitypecode,busitypename,busiitemcode,busiitemname,createuserid,createusername,enable,starttime)
                                         values (web_customsconfig_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}',sysdate)";
            sqlStr = string.Format(sqlStr, en.BUSITYPECODE, en.BUSITYPENAME, en.BUSIITEMCODE, en.BUSIITEMNAME, en.CREATEUSERID,
                en.CREATEUSERNAME, en.ENABLE);
            return DBMgr.ExecuteNonQuery(sqlStr);
        }

        public int UpdateConfig(WEB_CUSTOMSCONFIG en)
        {
            string sqlStr = @"update web_customsconfig set buistypecode='{0}',busitypename='{1}',
                                         busiitemcode='{2}' , busiitemname='{3}' ,enable='{4}', createuserid='{5}',createusername='{6}',
                                         starttime=sysdate where id='{7}'";
            sqlStr = string.Format(sqlStr, en.BUSITYPECODE, en.BUSITYPENAME, en.BUSIITEMCODE, en.BUSIITEMNAME,
                en.ENABLE, en.CREATEUSERID, en.CREATEUSERNAME, en.ID);
            return DBMgr.ExecuteNonQuery(sqlStr);
        }

        public WEB_CUSTOMSCONFIG JsonToEntity(JObject json)
        {
            WEB_CUSTOMSCONFIG en = new WEB_CUSTOMSCONFIG();
            try
            {
                if (!string.IsNullOrEmpty(json.Value<string>("ID")))
                {
                    en.ID = Convert.ToInt32(json.Value<string>("ID"));
                }
                else
                {
                    en.ID = -1;
                }

                en.BUSITYPECODE = json.Value<string>("BUSITYPECODE");
                if (!string.IsNullOrEmpty(en.BUSITYPECODE))
                {
                    string sqlStr = "select busitypename from web_customsconfig t1 where t1.busitypecode='" + en.BUSITYPECODE + "'";
                    DataTable dt = DBMgr.GetDataTable(sqlStr);
                    en.BUSITYPENAME = dt.Rows[0]["busitypename"].ToString();
                }
               
                en.BUSIITEMCODE = json.Value<string>("BUSIITEMCODE");
                en.BUSIITEMNAME = json.Value<string>("BUSIITEMNAME");

                if (!string.IsNullOrEmpty(json.Value<string>("ENABLE")))
                {
                    en.ENABLE = Convert.ToInt32(json.Value<string>("ENABLE"));
                }
                else
                {
                    en.ENABLE = 1;
                }

                FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
                string userName = identity.Name;
                JObject json_user = Extension.Get_UserInfo(userName);
                en.CREATEUSERID = (Int32)json_user.GetValue("ID");
                en.CREATEUSERNAME = (string)json_user.GetValue("REALNAME");
                en.REASON = json.Value<string>("REASON");
                return en;
            }
            catch
            {
                return null;
            }
        }

        public string CanUpdateOrInsert(WEB_CUSTOMSCONFIG en)
        {
            string repeat = "";
            string strWhere = "";
            if (en.ID > 0)
            {
                strWhere = " and t1.id not in ('" + en.ID + "') ";
            }
            string sqlStr = "select * from web_customsconfig t1 where t1.busitypecode='"+en.BUSITYPECODE+"' and t1.busiitemcode='"+en.BUSIITEMCODE+"'";
            sqlStr += strWhere;
            DataTable dt = DBMgr.GetDataTable(sqlStr);
            if (dt.Rows.Count > 0)
            {
                repeat = "此业务类型下已存在改业务细项";
            }
            return repeat;
        }


    }
}