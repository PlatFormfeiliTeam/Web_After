using System;
using System.Data;
using Newtonsoft.Json.Linq;
using Web_After.BasicManager;
using Web_After.BasicManager.BasicManager;
using Web_After.Common;

namespace Web_After.Sql
{
    public class busi_UnitConvert
    {
        Base_Codename_Method bcm = new Base_Codename_Method();
        string stopman = "";
        string createman = "";
        string startdate = "";
        string enddate = "";
        public DataTable load(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,
                           t2.name as createmanname,
                           t3.name as stopmanname,
                           t4.name as unitname1,
                           t5.name as unitname2
                      from sys_unitconvert t1
                      left join sys_user t2
                        on t1.createman = t2.id
                      left join sys_user t3
                        on t1.stopman = t3.id
                      left join base_declproductunit t4
                        on t1.unitcode1 = t4.code
                      left join base_declproductunit t5
                        on t1.unitcode2 = t5.code where 1 = 1 {0}";
            sql = String.Format(sql,strWhere);
            sql = Extension.GetPageSql2(sql, "t1.unitcode1", "asc", ref totalProperty, start, limit);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //新增查找是否重复
        public DataTable check_repeat(JObject json)
        {
            string sql = @"select *　from sys_unitconvert where unitcode1 = '{0}' and unitcode2 = '{1}' and convertrate = '{2}'";
            sql = String.Format(sql,json.Value<string>("UNITCODE1"),json.Value<string>("UNITCODE2"),json.Value<string>("CONVERTRATE"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //新增
        public int insert_table(JObject json)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"insert into sys_unitconvert(id,unitcode1,unitcode2,ConvertRate,remark,enabled,createman,stopman,createdate,startdate,enddate) values(
            sys_unitconvert_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}',sysdate,to_date('{7}','yyyy/mm/dd hh24:mi:ss'),
            to_date('{8}','yyyy/mm/dd hh24:mi:ss'))";
            sql = String.Format(sql, json.Value<string>("UNITCODE1"), json.Value<string>("UNITCODE2"),json.Value<string>("CONVERTRATE"),json.Value<string>("REMARK"),
                json.Value<string>("ENABLED"),createman,stopman,startdate,enddate);
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        //更新时检查是否重复
        public DataTable check_update_repeat(JObject json)
        {
            string sql = @"select *　from sys_unitconvert where unitcode1 = '{0}' and unitcode2 = '{1}' and convertrate = '{2}' and id not in ('{3}')";
            sql = String.Format(sql, json.Value<string>("UNITCODE1"), json.Value<string>("UNITCODE2"), json.Value<string>("CONVERTRATE"),json.Value<string>("ID"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //更新
        public int update_unitconvert(JObject json)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"update sys_unitconvert set unitcode1='{0}',unitcode2='{1}', convertrate='{2}', remark='{3}',startdate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),
enddate=to_date('{5}','yyyy/mm/dd hh24:mi:ss'),enabled='{6}' where id='{7}'";
            sql = String.Format(sql,json.Value<string>("UNITCODE1"), json.Value<string>("UNITCODE2"), json.Value<string>("CONVERTRATE"),json.Value<string>("REMARK"),startdate,enddate,
                json.Value<string>("ENABLED"),json.Value<string>("ID"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        //根据id来找出
        public DataTable befor_change(JObject json)
        {
            string sql = @"select * from sys_unitconvert where id = '{0}'";
            sql = String.Format(sql,json.Value<string>("ID"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }
        //保存修改记录
        public int insert_alert_data(JObject json,string contents)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"insert into base_alterrecord(id,
                            tabid,tabkind,alterman,
                            reason,contentes,alterdate) values(base_alterrecord_id.nextval,
                            '{0}','{1}','{2}',
                            '{3}','{4}',sysdate)";
            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_UnitConvert, createman, json.Value<string>("REASON"), contents);
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        //导出
        public DataTable export(string strWhere)
        {
            string sql = @"select t1.*,
                           t2.name as createmanname,
                           t3.name as stopmanname,
                           t4.name as unitname1,
                           t5.name as unitname2
                      from sys_unitconvert t1
                      left join sys_user t2
                        on t1.createman = t2.id
                      left join sys_user t3
                        on t1.stopman = t3.id
                      left join base_declproductunit t4
                        on t1.unitcode1 = t4.code
                      left join base_declproductunit t5
                        on t1.unitcode2 = t5.code where 1 = 1 {0}";
            sql = String.Format(sql,strWhere);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //根据名字获取单位code
        public DataTable getUnitName(string unitname)
        {
            string sql = @"select * from base_declproductunit where name = '{0}'";
            sql = String.Format(sql,unitname.Trim());
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

    }
}