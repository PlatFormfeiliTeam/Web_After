using System;
using System.Data;
using Newtonsoft.Json.Linq;
using Web_After.BasicManager;
using Web_After.BasicManager.BasicManager;
using Web_After.Common;

namespace Web_After.Sql
{

    public class busi_SpecialHsConvernet
    {
        Base_Codename_Method bcm = new Base_Codename_Method();
        string stopman = "";
        string createman = "";
        string startdate = "";
        string enddate = "";
        public DataTable loaddata(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
              from base_specialhsconvert t1
              left join sys_user t2
                on t1.createman = t2.id
              left join sys_user t3
                on t1.stopman = t3.id where 1 = 1 {0}";
            sql = String.Format(sql,strWhere);
            sql = Extension.GetPageSql2(sql, "t1.code", "desc", ref totalProperty, start, limit);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //新增查询是否重复
        public DataTable check_repeat(JObject json)
        {
            string sql = @"select * from base_specialhsconvert where code = '{0}'";
            sql = String.Format(sql,json.Value<string>("CODE"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }
        //新增
        public int inser_specialhsconvert(JObject json,string country)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"insert into base_specialhsconvert(id,code,name,extracode,country,type,remark,enabled,createman,stopman,createdate,startdate,enddate) 
 values(base_specialhsconvert_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',sysdate,to_date('{9}','yyyy/mm/dd hh24:mi:ss'),
to_date('{10}','yyyy/mm/dd hh24:mi:ss'))";
            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("EXTRACODE"), country, json.Value<string>("TYPE"),
                json.Value<string>("REMARK"), json.Value<string>("ENABLED"), createman, stopman, startdate, enddate);
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        //更新是否重复
        public DataTable check_update_repeat(JObject json)
        {
            string sql = @"select * from base_specialhsconvert where code = '{0}' and id not in ('{1}')";
            sql = String.Format(sql,json.Value<string>("CODE"),json.Value<string>("ID"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }
        //更新
        public int update_base_specialhsconvert(JObject json,string country)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"update base_specialhsconvert set code='{0}', name='{1}', remark='{2}',startdate=to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
            enddate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),extracode='{5}',country='{6}',type='{7}',enabled='{8}' where id='{9}'";
            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"),startdate,enddate,
                json.Value<string>("EXTRACODE"), country, json.Value<string>("TYPE"), json.Value<string>("ENABLED"), json.Value<string>("ID"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        //插入修改记录
        public int insert_alert_record(JObject json,string contents)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"insert into base_alterrecord(id,
                            tabid,tabkind,alterman,
                            reason,contentes,alterdate) values(base_alterrecord_id.nextval,
                            '{0}','{1}','{2}',
                            '{3}','{4}',sysdate)";
            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_SpecialHSConvert, createman, json.Value<string>("REASON"), contents);
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        //根据id来找出
        public DataTable before_data(JObject json)
        {
            string sql = @"select * from base_specialhsconvert where id = '{0}'";
            sql = String.Format(sql,json.Value<string>("ID"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //导出
        public DataTable export_base_specialhsconvert(string strwhere)
        {
            string sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
              from base_specialhsconvert t1
              left join sys_user t2
                on t1.createman = t2.id
              left join sys_user t3
                on t1.stopman = t3.id where 1 = 1 {0}";
            sql = String.Format(sql, strwhere);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }
    }
}