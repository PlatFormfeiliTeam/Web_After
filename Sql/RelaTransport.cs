using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Web_After.Common;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using Web_After.BasicManager;

namespace Web_After.Sql
{
    public class RelaTransport
    {
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,t2.name as decltransportname,t3.name as insptransportname,t4.name as createmanname,t5.name as stopmanname from rela_transport t1 left join base_transport t2 on 
t1.decltransport = t2.code left join base_inspconveyance t3 on t1.insptransport = t3.code  left join sys_user t4 on t1.createman=t4.id left join sys_user t5 on t1.stopman=t5.id    {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.decltransport", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }

        public List<int> CheckRepeat(string id, string declcountry, string inspcountry)
        {
            string strWhere = String.Empty;
            if (string.IsNullOrEmpty(id))
            {
                strWhere = "";
            }
            else
            {
                strWhere = " and id not in('" + id + "')";
            }
            List<int> addList = new List<int>();
            Sql.Base_Company bc = new Sql.Base_Company();
            //对应关系重复返回值为1
            if (check_transport_repeat(declcountry, inspcountry, strWhere).Rows.Count > 0)
            {
                addList.Add(1);
            }
            return addList;
        }

        public DataTable check_transport_repeat(string declcountry, string inspcountry, string strWhere)
        {
            string sql = @"select * from rela_transport where decltransport='{0}' and insptransport='{1}' " + strWhere;
            sql = string.Format(sql, declcountry, inspcountry);
            return DBMgrBase.GetDataTable(sql);
        }


        public int insert_relaTransport(JObject json, string stopman)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            //            string sql = @"insert into rela_country (id,declcountry,inspcountry,createman,stopman,createdate,startdate,enddate,enabled,remark,yearid)
            //values(rela_country_id.nextval,'{0-declcountry}','{1-inspcountry}','{2-createman}','{3-stopman}',sysdate,to_date('4-startdate','yyyy-mm-dd hh24:mi:ss'),
            //to_date('5-enddate','yyyy-mm-dd hh24:mi:ss'),'{6-enabled}','{7-remark}','')";
            string sql = @"insert into rela_transport (id,decltransport,insptransport,createman,stopman,createdate,startdate,enddate,enabled,remark)
                                  values(rela_country_id.nextval,'{0}','{1}','{2}','{3}',sysdate,to_date('{4}','yyyy-mm-dd hh24:mi:ss'),
                                  to_date('{5}','yyyy-mm-dd hh24:mi:ss'),'{6}','{7}')";
            sql = string.Format(sql, json.Value<string>("DECLTRANSPORT"), json.Value<string>("INSPTRANSPORT"), json_user.GetValue("ID"), stopman,
                json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
                 json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
                 json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public DataTable LoadDataById(string id)
        {
            string sql = @"select * from rela_transport t1 where t1.id='{0}'";
            sql = string.Format(sql, id);
            return DBMgrBase.GetDataTable(sql);
        }

        public int update_relaPackage(JObject json, string stopman)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql = @"update rela_transport set decltransport='{0}',insptransport='{1}',createman='{2}',stopman='{3}',createdate=sysdate,
                                 startdate =to_date('{4}','yyyy-mm-dd hh24:mi:ss'),enddate=to_date('{5}','yyyy-mm-dd hh24:mi:ss'),enabled='{6}',remark='{7}'
                                 where id='{8}'";
            sql = string.Format(sql, json.Value<string>("DECLTRANSPORT"), json.Value<string>("INSPTRANSPORT"), json_user.GetValue("ID"), stopman,
                 json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
                 json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
                 json.Value<string>("ENABLED"), json.Value<string>("REMARK"), json.Value<string>("ID"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
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
                                json.Value<string>("ID"), (int)Base_YearKindEnum.Insp_ContainerStandard, json_user.GetValue("ID"),
                                json.Value<string>("REASON"), getChange(dt, json));
            int i = DBMgrBase.ExecuteNonQuery(sql);

            return i;

        }

        public string getChange(DataTable dt, JObject json)
        {
            string str = "";

            if (dt.Rows[0]["decltransport"] != json.Value<string>("DECLTRANSPORT"))
            {
                str += "报关运输方式代码：" + dt.Rows[0]["decltransport"] + "——>" + json.Value<string>("DECLTRANSPORT") + "。";
            }

            if (dt.Rows[0]["insptransport"] != json.Value<string>("INSPTRANSPORT"))
            {
                str += "报关运输方式代码：" + dt.Rows[0]["insptransport"] + "——>" + json.Value<string>("INSPTRANSPORT") + "。";
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


        public void insert_rela_transport_excel(string DECLTRANSPORT, string INSPTRANSPORT, string ENABLED, string REMARK, string stopman, string STARTDATE, string ENDDATE)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql = @"insert into rela_transport (id,decltransport,insptransport,createman,stopman,createdate,startdate,enddate,enabled,remark)
                                  values(rela_country_id.nextval,'{0}','{1}','{2}','{3}',sysdate,to_date('{4}','yyyy-mm-dd hh24:mi:ss'),
                                  to_date('{5}','yyyy-mm-dd hh24:mi:ss'),'{6}','{7}')";
            sql = string.Format(sql, DECLTRANSPORT, INSPTRANSPORT, json_user.GetValue("ID"), stopman,
                STARTDATE, ENDDATE, ENABLED, REMARK);
            int i = DBMgrBase.ExecuteNonQuery(sql);
        }

        public DataTable export_rela_package(string strWhere)
        {
            string sql = @"select t1.*,t2.name as decltransportname,t3.name as insptransportname,t4.name as createmanname,t5.name as stopmanname from rela_transport t1 left join base_transport t2 on 
t1.decltransport = t2.code left join base_inspconveyance t3 on t1.insptransport = t3.code  left join sys_user t4 on t1.createman=t4.id left join sys_user t5 on t1.stopman=t5.id  {0}";
            sql = string.Format(sql, strWhere);
            return DBMgrBase.GetDataTable(sql);
        }

    }
}