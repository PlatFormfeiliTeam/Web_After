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
    public partial class MainConfig : System.Web.UI.Page
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
                    case "add":
                        //ImportExcelData();
                        break;
                    case "Ini_Base_Data":
                        Ini_Base_Data();
                        break;
                    case "loadbasebusitype":
                        GetBusitype();
                        break;
                    case "loadbasebusidetail":
                        GetBusiDetail();
                        break;
                }
            }
        }

        [WebMethod]
        public  static string EnableConfig(string id)
        {
            string[] array = id.Split(';');
            foreach (var item in array)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    string sqlStr = "update web_pageconfig t1 set t1.enabled='1' where t1.id='{0}'";
                    sqlStr = string.Format(sqlStr, item);
                    DBMgr.ExecuteNonQuery(sqlStr);
                    sqlStr = "update web_pageconfig_detail t1 set t1.enabled='1' where t1.parentid='{0}'";
                    sqlStr = string.Format(sqlStr, item);
                    DBMgr.ExecuteNonQuery(sqlStr);
                }
            }
            return "success";
        }

        [WebMethod]
        public static string DisableConfig(string id)
        {
            string[] array = id.Split(';');
            foreach (var item in array)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    string sqlStr = "update web_pageconfig t1 set t1.enabled='0' where t1.id='{0}'";
                    sqlStr = string.Format(sqlStr, item);
                    DBMgr.ExecuteNonQuery(sqlStr);
                    sqlStr = "update web_pageconfig_detail t1 set t1.enabled='0' where t1.parentid='{0}'";
                    sqlStr = string.Format(sqlStr, item);
                    DBMgr.ExecuteNonQuery(sqlStr);
                }
            }
            return "success";
        }

        /// <summary>
        /// 保存或更新数据
        /// </summary>
        /// <param name="formdata"></param>
        public void save(string formdata)
        {
            string response="";
            string repeat="";
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            WEB_PAGECONFIG en = JsonToEntity(json);
            if (en == null)
            {
                repeat = "保存失败，JSON数据转换出现问题";
            }
            if (en.ID<0)
            {
                //新增
                repeat =CanUpdateOrInsert(en);
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
        public int AddConfig(WEB_PAGECONFIG en)
        {
            string sqlStr = @"insert into web_pageconfig (ID,code,name,pagename,configcontent,customercode,createtime,enabled,userid,username)
                                         values (web_pageconfig_id.nextval,'{0}','{1}','{2}','{3}','{4}',sysdate,'{5}','{6}','{7}')";

            sqlStr = string.Format(sqlStr, en.CODE, en.NAME, en.PAGENAME, en.CONFIGCONTENT, en.CUSTOMERCODE, en.ENABLED, en.USERID, en.USERNAME);
            int i = DBMgr.ExecuteNonQuery(sqlStr);
            return i;
        }


        public int UpdateConfig(WEB_PAGECONFIG en)
        {
            string sqlStr = @"update web_pageconfig set code='{0}',name='{1}',pagename='{2}',configcontent='{3}',customercode='{4}',
                                         enabled='{5}',userid='{6}',username='{7}' where id='{8}'";
            sqlStr = string.Format(sqlStr,en.CODE,en.NAME,en.PAGENAME,en.CONFIGCONTENT,en.CUSTOMERCODE,en.ENABLED,en.USERID,en.USERNAME,en.ID);
            int i = DBMgr.ExecuteNonQuery(sqlStr);
            return i;
        }

        /// <summary>
        /// 判断是否可以将此条记录放入数据库中
        /// </summary>
        /// <returns></returns>
        public string CanUpdateOrInsert(WEB_PAGECONFIG en)
        {
            string result = "";
            string strWhere = "";
            if (en.ID > 0)
            {
                strWhere = " and t1.id not in ('" + en.ID + "') ";
            }
            string sqlStr = "select * from web_pageconfig t1 where t1.customercode='{0}' and (t1.code='{1}' or t1.name='{2}') "+strWhere;
            sqlStr = string.Format(sqlStr, en.CUSTOMERCODE, en.CODE, en.NAME);
            DataTable dt = DBMgr.GetDataTable(sqlStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                result += "同一客户代码下配置的代码或名称发生了重复";
            }
            sqlStr = "select * from web_pageconfig t1 where t1.customercode='{0}' and t1.pagename='{1}' and t1.configcontent='{2}' " + strWhere;
            sqlStr = string.Format(sqlStr, en.CUSTOMERCODE, en.PAGENAME, en.CONFIGCONTENT);
            dt = DBMgr.GetDataTable(sqlStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                result += "同一客户代码下相同页面存在重复";
            }
            return result;
        }
         
        #region 基础信息
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

        /// <summary>
        /// 获取初始化的信息
        /// </summary>
        public void Ini_Base_Data()
        {
            string sql = "";
            string customercode = "[]";//客商代码
            sql = "SELECT CODE as CODE,NAME||'('||CODE||')'  as NAME FROM sys_customer where CODE is not null and enabled=1";
            customercode = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));

            Response.Write("{customercode:" + customercode + "}");
            Response.End();

        }
        #endregion

        /// <summary>
        /// 加载数据
        /// </summary>
        public void loadData()
        {
            //适用页面
            string pagename = Request["SEARCH_PAGE"];
            //业务类型
            string busitype = Request["SEARCH_BUSITYPE"];
            //业务细项
            string busidetail = Request["SEARCH_BUSIDETAIL"];
            //名称
            string name = Request["SEARCH_NAME"];
            //编号
            string code = Request["SEARCH_CODE"];
            //适用客商
            string customercode = Request["SEARCH_ACCOUNT"];
            //是否启用 1-启用，0-禁用
            string enabled = Request["SEARCH_ENABLED"];
            string strWhere = "where 1=1 ";
            if (!string.IsNullOrEmpty(pagename))
            {
                strWhere += " and t1.pagename like '%" + pagename + "%' ";
            }
            if (!string.IsNullOrEmpty(busitype))
            {
                strWhere += " and t1.configcontent like '%" + busitype + "%' ";
            }
            if (!string.IsNullOrEmpty(busidetail))
            {
                strWhere += " and t1.configcontent like '%" + busidetail + "%' ";
            }
            if (!string.IsNullOrEmpty(name))
            {
                strWhere += " and t1.name like '%" + name + "%' ";
            }
            if (!string.IsNullOrEmpty(code))
            {
                strWhere += " and t1.code like '%" + code + "%' ";
            }
            if (!string.IsNullOrEmpty(customercode))
            {
                strWhere += " and t1.customercode like '%" + customercode + "%' ";
            }
            if (!string.IsNullOrEmpty(enabled))
            {
                strWhere += " and t1.enabled = '" + enabled + "' ";
            }
            string sqlStr = "select t1.* from web_pageconfig t1 " + strWhere;

            sqlStr = Extension.GetPageSql(sqlStr, "t1.code", "", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            DataTable loDataSet = DBMgr.GetDataTable(sqlStr);
            loDataSet = ProcessDataTable(loDataSet);
            string json = JsonConvert.SerializeObject(loDataSet, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }

        #region 工具方法
        //将配置内容拆开成业务类型和业务细项
        public DataTable ProcessDataTable(DataTable dt)
        {
            dt.Columns.Add("BUSITYPE", typeof(string));
            dt.Columns.Add("BUSIDETAIL", typeof(string));
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["CONFIGCONTENT"] != null && (!string.IsNullOrEmpty(dr["CONFIGCONTENT"].ToString())))
                {
                    string configContent = (string)dr["CONFIGCONTENT"];
                    string[] array = configContent.Split(';');

                    dr["BUSITYPE"] = array[0].Split('=')[1];
                    dr["BUSIDETAIL"] = array[1].Split('=')[1];
                }
            }
            return dt;
        }

        public WEB_PAGECONFIG JsonToEntity(JObject json)
        {
            WEB_PAGECONFIG en = new WEB_PAGECONFIG();
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
                en.CODE = json.Value<string>("CODE");
                en.NAME = json.Value<string>("NAME");
                en.PAGENAME = json.Value<string>("PAGENAME");
                en.CUSTOMERCODE = json.Value<string>("CUSTOMERCODE");
                if (!string.IsNullOrEmpty(json.Value<string>("ENABLED")))
                {
                    en.ENABLED = Convert.ToInt32(json.Value<string>("ENABLED"));
                }
                else
                {
                    en.ENABLED = 1;
                }
               
                en.BUSITYPE = json.Value<string>("BUSITYPE");
                en.BUSIDETAIL = json.Value<string>("BUSIDETAIL");
                en.CONFIGCONTENT = "业务类型=" + en.BUSITYPE + ";业务细项=" + en.BUSIDETAIL;
                FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
                string userName = identity.Name;
                JObject json_user = Extension.Get_UserInfo(userName);
                en.USERID = (Int32)json_user.GetValue("ID");
                en.USERNAME =(string) json_user.GetValue("REALNAME");
                en.REASON = json.Value<string>("REASON");
                return en;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}