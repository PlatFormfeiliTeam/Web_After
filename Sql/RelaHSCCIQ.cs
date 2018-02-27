using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using Web_After.BasicManager;
using Web_After.Common;

namespace Web_After.Sql
{
    public class RelaHSCCIQ
    {
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,t2.hsname as hsname,t3.ciqname,t4.name as createmanname,t5.name as stopmanname from rela_hsciq t1 left join base_insphs t2 on 
                                   t1.hscode=(t2.hscode||t2.extracode) left join base_ciqcode t3 on t1.ciqcode=t3.ciq  left join sys_user t4 on t1.createman=t4.id left join sys_user t5 on t1.stopman=t5.id {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.hscode", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }

        //判断内部编码,海关编码,社会信用代码是否有重复
        public List<int> CheckRepeat(string id, string hscode, string ciqcode)
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
            if (check_hscode_repeat(hscode, ciqcode, strWhere).Rows.Count > 0)
            {
                addList.Add(1);
            }
            return addList;
        }

        public int insert_rela_hs_ciq(JObject json, string stopman)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
//            string sql = @"insert into rela_hsciq (id,hscode,ciqcode,createman,stopman,createdate,startdate,enddate,yearid,enabled,remark)
//values (rela_hsciq_id.nextval,'{0-hscode}','{1-ciqcode}','{2-createman}','{3-stopman}',to_date('{4-createdate}','yyyy-mm-dd hh24:mi:ss'),to_date('{5-startdate}','yyyy-mm-dd hh24:mi:ss'),
//to_date('{6-enddate}','yyyy-mm-dd hh24:mi:ss'),'','{7-enabled}','{8-remark}')";
            string sql = @"insert into rela_hsciq (id,hscode,ciqcode,createman,stopman,createdate,startdate,enddate,yearid,enabled,remark)
                                   values (rela_hsciq_id.nextval,'{0}','{1}','{2}','{3}',sysdate,to_date('{4}','yyyy-mm-dd hh24:mi:ss'),
                                   to_date('{5}','yyyy-mm-dd hh24:mi:ss'),'','{6}','{7}')";
            sql = string.Format(sql, json.Value<string>("HSCODE"), json.Value<string>("CIQCODE"), json_user.GetValue("ID"), stopman,
                json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
                 json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
                  json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public int update_rela_hs_ciq(JObject json, string stopman)
        {
            int i = 0;
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql = @"update rela_hsciq set hscode='{0}',ciqcode='{1}',createman='{2}',stopman='{3}',createdate=sysdate, startdate=to_date('{4}','yyyy-mm-dd hh24:mi:ss'),
                                  enddate=to_date('{5}','yyyy-mm-dd hh24:mi:ss'),enabled='{6}',remark='{7}' where id='{8}'";
            sql = string.Format(sql,json.Value<string>("HSCODE"),json.Value<string>("CIQCODE"),json_user.GetValue("ID"),stopman,
                json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
                 json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
                  json.Value<string>("ENABLED"), json.Value<string>("REMARK"), json.Value<string>("ID")
                );
             i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public DataTable check_hscode_repeat(string hscode, string ciqcode, string strWhere)
        {
            string sql = "select * from rela_hsciq t1 where t1.hscode='{0}' and t1.ciqcode='{1}'";
            sql = string.Format(sql, hscode, ciqcode, strWhere);
            DataTable check_table = DBMgrBase.GetDataTable(sql);
            return check_table;
        }

        public string Check_Repeat(List<int> retunRepeat)
        {
            string repeat = "";
            for (int i = 0; i < retunRepeat.Count; i++)
            {
                if (retunRepeat[i] == 1)
                {
                    repeat = repeat + "HS与CIQ对应关系已存在,";
                }

            }
            return repeat;

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

        public DataTable LoadDataById(string id)
        {
            string sql = "select * from rela_hsciq t1 where t1.id='{0}'";
            sql = string.Format(sql, id);
            return DBMgrBase.GetDataTable(sql);
        }

        public string getChange(DataTable dt, JObject json)
        {
            string str = "";

            if (dt.Rows[0]["hscode"] != json.Value<string>("HSCODE"))
            {
                str += "hs代码：" + dt.Rows[0]["hscode"] + "——>" + json.Value<string>("HSCODE") + "。";
            }

            if (dt.Rows[0]["ciqcode"] != json.Value<string>("CIQCODE"))
            {
                str += "ciq代码：" + dt.Rows[0]["ciqcode"] + "——>" + json.Value<string>("CIQCODE") + "。";
            }

            if (dt.Rows[0]["enabled"] != json.Value<string>("ENABLED"))
            {
                str += "启用：" + dt.Rows[0]["name"] + "——>" + json.Value<string>("NAME") + "。";
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

    }
}