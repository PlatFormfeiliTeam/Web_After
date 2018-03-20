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
                    case "gettablename":
                        GetTableName();
                        break;
                    case "getfieldname":
                        GetFieldName();
                        break;
                    case "getorderno":
                        GetOrderNo();
                        break;
                    case "delete":
                        Delete();
                        break;
                    case "moveup":
                        MoveUp();
                        break;
                    case "movedown":
                        MoveDown();
                        break;
                }
            }
        }

        public void MoveDown()
        {
            string repeat = "";
            string response = "";
            string id = Request["deleterecord"];
            parentid = Request["parentid"];

            string GetAllRecordStr = "select * from web_pageconfig_detail t1 where t1.parentid='{0}'";
            GetAllRecordStr = string.Format(GetAllRecordStr, parentid);
            DataTable dt = DBMgr.GetDataTable(GetAllRecordStr);
            string GetCurrentRecord="select * from web_pageconfig_detail t1 where t1.parentid='{0}' and t1.id='{1}'";
            GetCurrentRecord = string.Format(GetCurrentRecord, parentid, id);
            DataTable dtCurrent = DBMgr.GetDataTable(GetCurrentRecord);
            bool canMove = false;
            if(dt!=null&&dt.Rows.Count>0&&dtCurrent!=null&&dtCurrent.Rows.Count>0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                  if(Convert.ToInt32( dt.Rows[i]["orderno"].ToString()) >Convert.ToInt32(dtCurrent.Rows[0]["orderno"].ToString()))
                  {
                      canMove = true;
                      break;
                  }
                }
            }
            else
            {
                repeat = "下移失败";
            }
            if (canMove)
            {
                string orderNo = dtCurrent.Rows[0]["orderno"].ToString();
                string updateMax = "update web_pageconfig_detail set orderno='" + orderNo + "' where orderno ='"+(Convert.ToInt32(orderNo)+1)+"'";
                DBMgr.ExecuteNonQuery(updateMax);
                string updateCurrent = "update web_pageconfig_detail set orderno='" + (Convert.ToInt32(orderNo) + 1) + "' where id='" + id + "'";
                DBMgr.ExecuteNonQuery(updateCurrent);
                repeat = "5";
            }
            else
            {
                repeat = "最后一条记录不能下移";
            }

            response = "{\"success\":\"" + repeat + "\"}";
            Response.Write(response);
            Response.End();
        }

        public void MoveUp()
        {
            string repeat = "";
            string response = "";
            string id = Request["deleterecord"];
            parentid = Request["parentid"];
            string sqlStr = "select * from web_pageconfig_detail t1 where t1.parentid='{0}' and t1.id='{1}'";
            sqlStr = string.Format(sqlStr, parentid, id);
            DataTable dt = DBMgr.GetDataTable(sqlStr);
           
            if (dt != null && dt.Rows.Count > 0)
            {
                string orderNo = dt.Rows[0]["orderno"].ToString();
                string updateMinStr = "update web_pageconfig_detail set orderno='"+orderNo+"' where parentid='"+parentid+"' and orderno='"+(Convert.ToInt32(orderNo)-1)+"'";
                DBMgr.ExecuteNonQuery(updateMinStr);
                string updateStr = "update web_pageconfig_detail set orderno='"+(Convert.ToInt32(orderNo)-1)+"' where id='"+id+"'";
                DBMgr.ExecuteNonQuery(updateStr);
                repeat = "5";
            }
            else
            {
                repeat = "异常，无法上移";
            }
            response = "{\"success\":\"" + repeat + "\"}";
            Response.Write(response);
            Response.End();
        }

        public void Delete()
        {
            string repeat = "";
            string response = "";
            string id = Request["deleterecord"];
            parentid = Request["parentid"];
            try
            {
                //获得小于它orderno的所有记录，然后要依次减去1;
                string sqlStr = "select * from web_pageconfig_detail t1 where t1.parentid='{0}' and t1.id='{1}'";
                sqlStr = string.Format(sqlStr, parentid, id);
                DataTable dt = DBMgr.GetDataTable(sqlStr);
                string orderno = dt.Rows[0]["orderno"].ToString();
                sqlStr = "select * from web_pageconfig_detail t1 where t1.parentid='{0}' and t1.orderno>'{1}'";
                sqlStr = string.Format(sqlStr, parentid, orderno);
                DataTable dt1 = DBMgr.GetDataTable(sqlStr);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        string updateStr = "update web_pageconfig_detail set orderno='" + (Convert.ToInt32(dt1.Rows[i]["orderno"].ToString()) - 1) + "' where id='" + dt1.Rows[i]["id"].ToString() + "'";
                        DBMgr.ExecuteNonQuery(updateStr);
                    }
                }
                sqlStr = "delete from web_pageconfig_detail t1 where t1.id='" + id + "'";
                DBMgr.ExecuteNonQuery(sqlStr);
                repeat = "5";
            }
            catch
            {
                repeat = "删除失败";
            }
            
            response = "{\"success\":\"" + repeat + "\"}";
            Response.Write(response);
            Response.End();
        }

        /// <summary>
        /// 保存更新数据
        /// </summary>
        /// <param name="formdata"></param>
        public void save(string formdata)
        {
            parentid = Request["parentid"];
            string response = "";
            string repeat = "";
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            WEB_PAGECONFIG_DETAIL en = JsonToEntity(json);
            if(!string.IsNullOrEmpty(parentid)){
                en.PARENTID = Convert.ToInt32(parentid);
            }
            
            if (en != null)
            {
                if (en.ID < 0)
                {
                    //新增
                    string sqlStr = @"insert into web_pageconfig_detail (id,parentid,orderno,name,controltype,isselect,selectcontent,configtype,tablecode,fieldcode,tablename,fieldname,createtime,userid,username,enabled)
                                                 values (web_pageconfig_detail_id.nextval,'{0}','{1}','{2}','{3}','0','{4}','{5}','{6}','{7}','{8}','{9}',
                                                 sysdate,'{10}','{11}','{12}')";
                    sqlStr = string.Format(sqlStr, en.PARENTID, en.ORDERNO, en.NAME, en.CONTROLTYPE, en.SELECTCONTENT,
                       en.CONFIGTYPE, en.TABLECODE, en.FIELDCODE, en.TABLENAME, en.FIELDNAME, en.USERID, en.USERNAME, en.ENABLED);
                    int i = DBMgr.ExecuteNonQuery(sqlStr);
                    if (i > 0)
                    {
                        repeat = "5";
                    }
                    else
                    {
                        repeat = "新增配置失败";
                    }
                }
                else
                {
                    //更新
                    string sqlStr = @"update web_pageconfig_detail set name='{0}',controltype='{1}',selectcontent='{2}',configtype='{3}',tablecode='{4}',fieldcode='{5}',tablename='{6}',fieldname='{7}',
                                                 userid='{8}',username='{9}',enabled='{10}' where id='{11}'";
                    sqlStr = string.Format(sqlStr, en.NAME,en.CONTROLTYPE,en.SELECTCONTENT,en.CONFIGTYPE,en.TABLECODE,en.FIELDCODE,
                        en.TABLENAME,en.FIELDNAME,en.USERID,en.USERNAME,en.ENABLED,en.ID);
                    int i = DBMgr.ExecuteNonQuery(sqlStr);
                    if (i > 0)
                    {
                        repeat = "5";
                    }
                    else
                    {
                        repeat = "更新配置失败";
                    }
                }
            }
            else
            {
                repeat = "json转换出错";
            }

            response = "{\"success\":\"" + repeat + "\"}";
            Response.Write(response);
            Response.End();
            
        }



        /// <summary>
        /// 新增记录时，序号从数据库算出来
        /// </summary>
        public void GetOrderNo()
        {
            string orderno = "[]";
            parentid = Request["parentid"];
            string sqlStr = "select * from web_pageconfig_detail t1 where t1.parentid='{0}'";
            sqlStr = string.Format(sqlStr, parentid);
            DataTable dt = DBMgr.GetDataTable(sqlStr);
            orderno = "[{\"orderno\":\"" + (dt.Rows.Count + 1) + "\"}]";
            Response.Write("{" + "orderno:" + orderno + "}");
            Response.End();
        }

        /// <summary>
        /// 加载对应的细项配置
        /// </summary>
        public void loadData()
        {
            parentid = Request["parentid"];
            string sqlStr = "select * from web_pageconfig_detail t1 where t1.parentid='{0}'";
            sqlStr = string.Format(sqlStr, parentid);
            sqlStr = Extension.GetPageSql(sqlStr, "t1.orderno", "", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            DataTable loDataSet = DBMgr.GetDataTable(sqlStr);
            string json = JsonConvert.SerializeObject(loDataSet, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
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
        public void GetFieldName()
        {
            string sql = "";
            string fieldname = "[]";
            sql = "select t1.code,t1.name||'('||t1.code||')' as name,t1.tablename from web_fieldconfig t1";
            fieldname = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            Response.Write("{" + "fieldname:" + fieldname + "}");
            Response.End();
        }
        /// <summary>
        /// 获取表名
        /// </summary>
        public void GetTableName()
        {
            string sql = "";
            string tablename = "[]";
            sql = "select t1.code,t1.name||'('||t1.code||')' as name from web_tableconfig t1";
            tablename = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            Response.Write("{" + "tablename:" + tablename + "}");
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

        public WEB_PAGECONFIG_DETAIL JsonToEntity(JObject json)
        {
            WEB_PAGECONFIG_DETAIL en = new WEB_PAGECONFIG_DETAIL();
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
                if (!string.IsNullOrEmpty(json.Value<string>("ENABLED")))
                {
                    en.ENABLED = Convert.ToInt32(json.Value<string>("ENABLED"));
                }
                else
                {
                    en.ENABLED = 1;
                }
                FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
                string userName = identity.Name;
                JObject json_user = Extension.Get_UserInfo(userName);
                en.USERID = (Int32)json_user.GetValue("ID");
                en.USERNAME = (string)json_user.GetValue("REALNAME");
                en.REASON = json.Value<string>("REASON");
                en.NAME = json.Value<string>("NAME");
                en.FIELDCODE = json.Value<string>("FIELDCODE");
                en.TABLECODE = json.Value<string>("TABLECODE");
                en.SELECTCONTENT = json.Value<string>("SELECTCONTENT");
                if (!string.IsNullOrEmpty(json.Value<string>("ORDERNO")))
                {
                    en.ORDERNO = Convert.ToInt32(json.Value<string>("ORDERNO"));
                }
                en.CONTROLTYPE = json.Value<string>("CONTROLTYPE");
                en.CONFIGTYPE = json.Value<string>("CONFIGTYPE");
                if (en.CONTROLTYPE == "下拉框")
                {
                    en.SELECTCONTENT = en.SELECTCONTENT.Replace("；",";");
                }
                else
                {
                    en.SELECTCONTENT = "";
                }
                en.TABLENAME = GetTableNameWithCode(en.TABLECODE);
                en.FIELDNAME = GetFieldNameWithCode(en.FIELDCODE, en.TABLECODE);
            }
            catch
            {
                return null;
            }
            return en;
        }

        public string GetTableNameWithCode(string tablecode)
        {
            string tableName = "";
            string sqlStr = "select t1.name from web_tableconfig t1 where t1.code='" + tablecode + "'";
            DataTable dt = DBMgr.GetDataTable(sqlStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                tableName = dt.Rows[0]["name"].ToString();
            }
            return tableName;
        }

        public string GetFieldNameWithCode(string fieldcode,string tablecode)
        {
            string tableName = "";
            string sqlStr = "select t1.name from web_fieldconfig t1 where t1.code='" + fieldcode + "' and t1.tablename='" + tablecode + "'";
            DataTable dt = DBMgr.GetDataTable(sqlStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                tableName = dt.Rows[0]["name"].ToString();
            }
            return tableName;
        }
    }
}