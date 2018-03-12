using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Web_After.Common;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using Web_After.BasicManager;
using Newtonsoft.Json;

namespace Web_After.Sql
{
    public class RelaContainer
    {
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as containersizename,t5.name as containertypename from rela_container t1 left join 
sys_user t2 on t1.createman=t2.id left join sys_user t3 on t1.stopman=t3.id left join base_containersize t4 on t4.code=t1.containersize left join base_containertype t5 on 
t1.containertype=t5.code {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.containersize", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }


        public string CheckRepeat(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            string repeat = "";
            string strWhere = "";
            string ID = json.Value<string>("ID");
            if (string.IsNullOrEmpty(ID))
            {
                strWhere = "";
            }
            else
            {
                strWhere = " and id not in('" + ID + "')";
            }
            string CONTAINERSIZE = json.Value<string>("CONTAINERSIZE");
            string CONTAINERTYPE = json.Value<string>("CONTAINERTYPE");
            string FORMATCODE = json.Value<string>("FORMATCODE");
            string sqlStr = @"select * from rela_container t1 where t1.containersize='{0}' and t1.containertype='{1}' and t1.formatcode='{2}' " + strWhere;
            sqlStr = string.Format(sqlStr, CONTAINERSIZE, CONTAINERTYPE, FORMATCODE);
            DataTable dt = DBMgrBase.GetDataTable(sqlStr);
            if (dt.Rows.Count > 0)
            {
                repeat = "此配置已存在，请检查";
            }
            return repeat;
        }

        public DataTable LoadDataById(string id)
        {
            string sql = @"select * from rela_container t1 where t1.id='{0}'";
            sql = string.Format(sql, id);
            return DBMgrBase.GetDataTable(sql);
        }

        public int insert_base_alterrecord(JObject json, DataTable dt)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql = @"insert into base_alterrecord(id,
                                tabid,tabkind,alterman,
                                reason,contentes,alterdate) 
                                values(base_alterrecord_id.nextval,
                                '{0}','{1}','{2}',
                                '{3}','{4}',sysdate)";
            sql = String.Format(sql,
                                json.Value<string>("ID"), (int)Base_YearKindEnum.Rela_Container, json_user.GetValue("ID"),
                                json.Value<string>("REASON"), getChange(dt, json));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public string getChange(DataTable dt, JObject json)
        {
            string str = "";

            if (dt.Rows[0]["containersize"] != json.Value<string>("CONTAINERSIZE"))
            {
                str += "集装箱尺寸：" + dt.Rows[0]["containersize"] + "——>" + json.Value<string>("CONTAINERSIZE") + "。";
            }

            if (dt.Rows[0]["containertype"] != json.Value<string>("CONTAINERTYPE"))
            {
                str += "集装箱类型：" + dt.Rows[0]["containertype"] + "——>" + json.Value<string>("CONTAINERTYPE") + "。";
            }

            if (dt.Rows[0]["formatcode"] != json.Value<string>("FORMATCODE"))
            {
                str += "集装箱规格：" + dt.Rows[0]["formatcode"] + "——>" + json.Value<string>("FORMATCODE") + "。";
            }

            if (dt.Rows[0]["enabled"] != json.Value<string>("ENABLED"))
            {
                str += "启用：" + dt.Rows[0]["enabled"] + "——>" + json.Value<string>("ENABLED") + "。";
            }

            if (dt.Rows[0]["remark"] != json.Value<string>("REMARK"))
            {
                str += "备注：" + dt.Rows[0]["remark"] + "——>" + json.Value<string>("REMARK") + "。";
            }
            if (dt.Rows[0]["StartDate"] != json.Value<string>("STARTDATE"))
            {
                str += "开始时间：" + dt.Rows[0]["StartDate"] + "——>" + json.Value<string>("STARTDATE") + "。";
            }
            if (dt.Rows[0]["EndDate"] != json.Value<string>("ENDDATE"))
            {
                str += "停用时间：" + dt.Rows[0]["EndDate"] + "——>" + json.Value<string>("ENDDATE") + "。";
            }
            return str;

        }

        public void insert_rela_container(JObject json, string stopman)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string formatCode = json.Value<string>("FORMATCODE");
            string sql = "select * from base_containerstandard t1 where t1.code='" + formatCode + "' and t1.enabled='1'";
            DataTable dt_containerstandard = DBMgrBase.GetDataTable(sql);

            string sqlStr = @"insert into rela_container (id,containersize,containertype,formatcode,formatname,containerhs,enabled,remark,createman,stopman,startdate,enddate,createdate)
                                         values (rela_container_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}',
                                         '{7}','{8}',to_date('{9}','yyyy-mm-dd hh24:mi:ss'),to_date('{10}','yyyy-mm-dd hh24:mi:ss'),sysdate)";
            sqlStr = string.Format(sqlStr, json.Value<string>("CONTAINERSIZE"),
                 json.Value<string>("CONTAINERTYPE"),
                 json.Value<string>("FORMATCODE"),
                 dt_containerstandard.Rows[0]["name"],
                 dt_containerstandard.Rows[0]["hscode"],
                 json.Value<string>("ENABLED"),
                 json.Value<string>("REMARK"),
                 json_user.GetValue("ID"),
                 stopman,
                 json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
                 json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"));
            int i = DBMgrBase.ExecuteNonQuery(sqlStr);
        }


        public int update_rela_container(JObject json, string stopman)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string formatCode = json.Value<string>("FORMATCODE");
            string sql = "select * from base_containerstandard t1 where t1.code='" + formatCode + "' and t1.enabled='1'";
            DataTable dt_containerstandard = DBMgrBase.GetDataTable(sql);

            string sqlStr = @"update rela_container set containersize='{0}',containertype='{1}',formatcode='{2}',formatname='{3}',containerhs='{4}',
                                         enabled='{5}',remark='{6}',createman='{7}',stopman='{8}',startdate=to_date('{9}','yyyy-mm-dd hh24:mi:ss'),enddate=to_date('{10}','yyyy-mm-dd hh24:mi:ss')
                                         where id='{11}'";
            sqlStr = string.Format(sqlStr, json.Value<string>("CONTAINERSIZE"),
                 json.Value<string>("CONTAINERTYPE"),
                 json.Value<string>("FORMATCODE"),
                 dt_containerstandard.Rows[0]["name"],
                 dt_containerstandard.Rows[0]["hscode"],
                 json.Value<string>("ENABLED"),
                 json.Value<string>("REMARK"),
                 json_user.GetValue("ID"),
                 stopman,
                 json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
                 json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
                 json.Value<string>("ID"));
            int i = DBMgrBase.ExecuteNonQuery(sqlStr);
            return i;
        }

        public string CheckRepeat(string containerSize, string containerType, string formatCode)
        {
            string sqlStr = @"select * from rela_container t1 where t1.containersize='{0}' and t1.containertype='{1}' and t1.formatcode='{2}'";
            string repeat = "";
            sqlStr = string.Format(sqlStr, containerSize, containerType, formatCode);
            DataTable dt = DBMgrBase.GetDataTable(sqlStr);
            if (dt.Rows.Count > 0)
            {
                repeat = "此配置已存在，请检查";
            }
            return repeat;
        }

        public void insert_rela_container(string CONTAINERSIZE, string CONTAINERTYPE, string FORMATCODE, string FORMATNAME, string CONTAINERHS, string ENABLED, string REMARK, string stopman, string STARTDATE, string ENDDATE)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string sqlStr = @"insert into rela_container (id,containersize,containertype,formatcode,formatname,containerhs,enabled,remark,createman,stopman,startdate,enddate,createdate)
                                         values (rela_container_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}',
                                         '{7}','{8}',to_date('{9}','yyyy-mm-dd hh24:mi:ss'),to_date('{10}','yyyy-mm-dd hh24:mi:ss'),sysdate)";
            sqlStr = string.Format(sqlStr, CONTAINERSIZE,
                 CONTAINERTYPE,
                 FORMATCODE,
                 FORMATNAME,
                 CONTAINERHS,
                 ENABLED,
                 REMARK,
                 json_user.GetValue("ID"),
                 stopman,
                 STARTDATE,
                 ENDDATE);
            int i = DBMgrBase.ExecuteNonQuery(sqlStr);
        }


        public DataTable export_rela_container(string strWhere)
        {
            string sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as containersizename,t5.name as containertypename from rela_container t1 left join 
                                     sys_user t2 on t1.createman=t2.id left join sys_user t3 on t1.stopman=t3.id left join base_containersize t4 on t4.code=t1.containersize left join base_containertype t5 on 
                                     t1.containertype=t5.code " + strWhere;
            return DBMgrBase.GetDataTable(sql);
        }
    }
}