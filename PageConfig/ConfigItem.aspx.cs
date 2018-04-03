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
    public partial class ConfigItem : System.Web.UI.Page
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
                    case "loadbasebusitype":
                        GetBusitype();
                        break;
                    case "loadbasebusidetail":
                        GetBusiDetail();
                        break;
                    case "delete":
                        //Delete();
                        break;

                }
            }
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
            WEB_CUSTOMSCOST en = JsonToEntity(json);
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

        /// <summary>
        /// 新增一笔记录
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public int AddConfig(WEB_CUSTOMSCOST en)
        {
            string sqlStr = @"insert into WEB_CUSTOMSCOST (Id,Busitypecode,Busitypename,Busiitemcode,Busiitemname,Originname,Configname,Createuserid,Createusername)
                                         values(web_customsconfig_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";

            sqlStr = string.Format(sqlStr, en.BUSITYPECODE, en.BUSITYPENAME, en.BUSIITEMCODE, en.BUSIITEMNAME, en.ORIGINNAME, en.CONFIGNAME, en.CREATEUSERID, en.CREATEUSERNAME);
            int i = DBMgr.ExecuteNonQuery(sqlStr);
            return i;
        }

        public int UpdateConfig(WEB_CUSTOMSCOST en)
        {
            string sqlStr = @"update WEB_CUSTOMSCOST  set Busitypecode='{0}',Busitypename='{1}',Busiitemcode='{2}',Busiitemname='{3}',Originname='{4}',Configname='{5}',Createuserid='{6}',Createusername='{7}' where id='{8}'";

            sqlStr = string.Format(sqlStr, en.BUSITYPECODE, en.BUSITYPENAME, en.BUSIITEMCODE, en.BUSIITEMNAME, en.ORIGINNAME, en.CONFIGNAME, en.CREATEUSERID, en.CREATEUSERNAME,en.ID);
            int i = DBMgr.ExecuteNonQuery(sqlStr);
            return i;
        }

        public string CanUpdateOrInsert(WEB_CUSTOMSCOST en)
        {
            string repeat = "";
            string strWhere = "";
            if (en.ID > 0)
            {
                strWhere = " and t1.id not in ('" + en.ID + "') ";
            }
            string sqlStr = "select * from WEB_CUSTOMSCOST t1 where t1.BUSITYPECODE='{0}' and t1.BUSIITEMCODE='{1}' and t1.ORIGINNAME='{2}' " + strWhere;
            sqlStr = string.Format(sqlStr, en.BUSITYPECODE, en.BUSIITEMCODE, en.ORIGINNAME);
            DataTable dt = DBMgr.GetDataTable(sqlStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                repeat += "不能重复";
            }
            return repeat;
        }

        public WEB_CUSTOMSCOST JsonToEntity(JObject json)
        {
            WEB_CUSTOMSCOST en = new WEB_CUSTOMSCOST();
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
                string sqlStr = "select * from web_customsconfig t1 where t1.busitypecode='" + en.BUSITYPECODE + "'";
                DataTable dt = DBMgr.GetDataTable(sqlStr);
                en.BUSITYPENAME = dt.Rows[0]["busitypename"].ToString();
                en.BUSIITEMCODE = json.Value<string>("BUSIITEMCODE");
                sqlStr = "select * from web_customsconfig t1 where t1.busiitemcode='" + en.BUSIITEMCODE + "'";
                dt = DBMgr.GetDataTable(sqlStr);
                en.BUSIITEMNAME = dt.Rows[0]["busiitemname"].ToString();
                en.CONFIGNAME = json.Value<string>("CONFIGNAME");
                en.ORIGINNAME = json.Value<string>("ORIGINNAME");
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
        /// <summary>
        /// 加载数据
        /// </summary>
        public void loadData()
        {
            //业务类型
            string busitypecode = Request["SEARCH_BUSITYPE"];
            //业务细项
            string busidetailcode = Request["SEARCH_BUSIDETAIL"];
            string strWhere = "where 1=1 ";
            if (!string.IsNullOrEmpty(busitypecode))
            {
                strWhere += " and t1.BUSITYPECODE like '%" + busitypecode + "%' ";
            }
            if (!string.IsNullOrEmpty(busidetailcode))
            {
                strWhere += " and t1.BUSIITEMCODE like '%" + busidetailcode + "%' ";
            }


            string sqlStr = "select t1.* from WEB_CUSTOMSCOST t1 " + strWhere;

            sqlStr = Extension.GetPageSql(sqlStr, "t1.BUSITYPECODE", "", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            DataTable loDataSet = DBMgr.GetDataTable(sqlStr);
            string json = JsonConvert.SerializeObject(loDataSet, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }

        /// <summary>
        /// 业务类型
        /// </summary>
        public void GetBusitype()
        {
            string sql = "";
            string busitype = "[]";
            sql = "select distinct t1.busitypecode as code, t1.busitypename||'('||t1.busitypecode||')' as name from web_customsconfig t1 where t1.enable='1'";
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
            sql = "select  t1.busiitemcode as code, t1.busiitemname||'('||t1.busiitemcode||')' as name,t1.busitypecode as busitype from web_customsconfig t1 where t1.enable='1'";
            busidetail = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            Response.Write("{" + "busidetail:" + busidetail + "}");
            Response.End();
        }
    }
}