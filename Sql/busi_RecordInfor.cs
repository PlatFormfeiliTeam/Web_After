using System;
using System.Data;
using Newtonsoft.Json.Linq;
using Web_After.BasicManager;
using Web_After.BasicManager.BasicManager;
using Web_After.Common;

namespace Web_After.Sql
{
    public class busi_RecordInfor
    {
        Base_Codename_Method bcm = new Base_Codename_Method();
        string stopman = "";
        string createman = "";
        string startdate = "";
        string enddate = "";
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,
                       t2.name as createmanname,
                       t3.name as stopmanname,
                       T4.NAME AS TRADENAME,
                       T5.NAME AS EXEMPTINGNAME,
                       t6.name as busiunitname
                      from SYS_RECORDINFO t1
                      LEFT JOIN sys_user t2
                        on t1.createman = t2.id
                      LEFT JOIN sys_user t3
                        on t1.stopman = t3.id
                      LEFT JOIN BASE_DECLTRADEWAY t4
                        ON t1.TRADE = t4.CODE
                       AND t4.enabled = 1
                      LEFT JOIN BASE_EXEMPTINGNATURE t5
                        ON t1.EXEMPTING = t5.CODE
                       AND t5.enabled = 1
                      left join base_company t6
                        on t1.busiunit = t6.code
                        where 1 = 1 {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.code", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
            
        }

        public DataSet check_sys_recordinfo(JObject json)
        {
            string sql = @"select * from SYS_RECORDINFO where code = '{0}'";
            sql = string.Format(sql, json.Value<string>("CODE"));
            return DBMgrBase.GetDataSet(sql);
        }

        public int insert_sys_recordinfo(JObject json)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string select_sql = @"select * from base_company where code = '" + json.Value<string>("RECEIVEUNIT") + "'";
            DataTable dt = DBMgrBase.GetDataTable(select_sql);
            string receiveunitname = dt.Rows[0]["name"].ToString();
            string sql = @"insert into sys_recordinfo(id,code,bookattribute,busiunit,receiveunit,trade,exempting,ismodel,remark,enabled,createman,stopman,createdate,startdate,enddate,receiveunitname)
            values(sys_recordinfo_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',sysdate,to_date('{11}','yyyy/mm/dd hh24:mi:ss'),
            to_date('{12}','yyyy/mm/dd hh24:mi:ss'),'{13}')";
            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("BOOKATTRIBUTE"), json.Value<string>("BUSIUNIT"), json.Value<string>("RECEIVEUNIT"),
                json.Value<string>("TRADE"), json.Value<string>("EXEMPTING"), json.Value<string>("ISMODEL"), json.Value<string>("REMARK"), json.Value<string>("ENABLED"),
                createman,stopman,startdate,enddate,receiveunitname);
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public DataTable update_check_sys_recordinfo(JObject json)
        {
            string sql = @"select * from sys_recordinfo where code = '{0}' and id not in ('{1}')";
            sql = String.Format(sql,json.Value<string>("CODE"),json.Value<string>("ID"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        public int update_sys_recordinfo(JObject json)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string select_sql = @"select * from base_company where code = '" + json.Value<string>("RECEIVEUNIT") + "'";
            DataTable dt = DBMgrBase.GetDataTable(select_sql);
            string receiveunitname = dt.Rows[0]["name"].ToString();
            string sql = @"update sys_recordinfo set code='{0}', bookattribute='{1}', busiunit='{2}', receiveunit='{3}', trade='{4}', exempting='{5}', remark='{6}', startdate=to_date('{7}','yyyy/mm/dd hh24:mi:ss'),
enddate=to_date('{8}','yyyy/mm/dd hh24:mi:ss'),ismodel='{9}',stopman='{10}',enabled='{11}',receiveunitname = '{12}' where id='{13}'";
            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("BOOKATTRIBUTE"), json.Value<string>("BUSIUNIT"), json.Value<string>("RECEIVEUNIT"),
                json.Value<string>("TRADE"), json.Value<string>("EXEMPTING"), json.Value<string>("REMARK"), startdate, enddate, json.Value<string>("ISMODEL"), stopman, json.Value<string>("ENABLED"),receiveunitname,json.Value<string>("ID"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public DataTable Before_Change(JObject json)
        {
            string sql = @"select * from sys_recordinfo where id = '{0}'";
            sql = String.Format(sql,json.Value<string>("ID"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        public int saveChanges_recordinfo(JObject json,string content)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"insert into base_alterrecord(id,
                            tabid,tabkind,alterman,
                            reason,contentes,alterdate) values(base_alterrecord_id.nextval,
                            '{0}','{1}','{2}',
                            '{3}','{4}',sysdate)";
            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_RecordInfo, createman,
                json.Value<string>("REASON"), content
            );
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public DataTable export_recordinfo(string strwhere)
        {
            string sql = @"select t1.*,
                       t2.name as createmanname,
                       t3.name as stopmanname,
                       T4.NAME AS TRADENAME,
                       T5.NAME AS EXEMPTINGNAME,
                       t6.name as busiunitname
                      from SYS_RECORDINFO t1
                      LEFT JOIN sys_user t2
                        on t1.createman = t2.id
                      LEFT JOIN sys_user t3
                        on t1.stopman = t3.id
                      LEFT JOIN BASE_DECLTRADEWAY t4
                        ON t1.TRADE = t4.CODE
                       AND t4.enabled = 1
                      LEFT JOIN BASE_EXEMPTINGNATURE t5
                        ON t1.EXEMPTING = t5.CODE
                       AND t5.enabled = 1
                      left join base_company t6
                        on t1.busiunit = t6.code
                        where 1 = 1 {0}";
            sql = String.Format(sql,strwhere);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }


        public DataTable LaoDataDetails(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"SELECT t1.*,
                           t2.name AS createmanname,
                           t3.name AS stopmanname,
                           t4.name AS unitname,
                           t5.code as recordinfocode,
                           t6.id   as hsid
                      FROM SYS_RECORDINFO_DETAIL t1
                      LEFT JOIN SYS_USER t2
                        ON t1.createman = t2.id
                      LEFT JOIN SYS_USER t3
                        ON t1.stopman = t3.id
                      LEFT JOIN BASE_DECLPRODUCTUNIT t4
                        ON t1.unit = t4.code
                       AND t4.enabled = 1
                      LEFT JOIN SYS_RECORDINFO t5
                        ON t1.recordinfoid = t5.id
                      LEFT JOIN (select id,
                                        hscode,
                                        extracode,
                                        enabled,
                                        ROW_NUMBER() OVER(PARTITION BY bc.hscode, bc.extracode ORDER BY bc.enabled desc, bc.id) as rownumber
                                   from BASE_COMMODITYHS bc) t6
                        ON t1.hscode = t6.hscode
                       AND t6.enabled = 1
                       AND t1.additionalno = t6.extracode
                       AND t6.rownumber = 1 {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.itemno", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }


        public int insert_record_details(string recordinfoid ,JObject json)
        {
            try
            {
                bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
                string sql = @"insert into sys_recordinfo_detail(id,recordinfoid,itemno,hscode,additionalno,itemnoattribute,commodityname,specificationsmodel,unit,version,enabled,remark,createman,stopman,createdate,startdate,enddate,abnormal,partno)
            values(sys_recordinfo_detail_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',sysdate,to_date('{13}','yyyy/mm/dd hh24:mi:ss'),
            to_date('{14}','yyyy/mm/dd hh24:mi:ss'),'{15}','{16}')";
                sql = String.Format(sql, recordinfoid, json.Value<string>("ITEMNO"), json.Value<string>("HSCODE"), json.Value<string>("ADDITIONALNO"), json.Value<string>("ITEMNOATTRIBUTE"), json.Value<string>("COMMODITYNAME").Replace("'", "'||chr(39)||'"),
                    json.Value<string>("SPECIFICATIONSMODEL").Replace("'", "'||chr(39)||'"), json.Value<string>("UNIT"), json.Value<string>("VERSION"), json.Value<string>("ENABLED"), json.Value<string>("REMARK"), createman, stopman, startdate, enddate, 0, json.Value<string>("PARTNO"));
                int i = DBMgrBase.ExecuteNonQuery(sql);
                return i;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        //判断备案详情是否存在
        public DataTable check_repeat_record_details(JObject json,string recordinfoid)
        {
            string strWhere = String.Empty;
            string sql = @"select * from sys_recordinfo_detail where 1 = 1 ";
            if (!string.IsNullOrEmpty(json.Value<string>("ITEMNO").ToString()))
            {
                strWhere +=  " and itemno = '" + json.Value<string>("ITEMNO").ToString() + "'";
            }
            if (!string.IsNullOrEmpty(recordinfoid))
            {
                strWhere += " and recordinfoid = '"+recordinfoid+"'";
            }
            if (!string.IsNullOrEmpty(json.Value<string>("ITEMNOATTRIBUTE").ToString()))
            {
                strWhere += " and itemnoattribute = '" + json.Value<string>("ITEMNOATTRIBUTE").ToString() + "'";
            }
            sql = sql + strWhere;
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }


        public DataTable check_update_repeat_record_details(JObject json, string recordinfoid)
        {
            string strWhere = String.Empty;
            string sql = @"select * from sys_recordinfo_detail where 1 = 1 and id not in ('"+json.Value<string>("ID")+"') ";
            if (!string.IsNullOrEmpty(json.Value<string>("ITEMNO").ToString()))
            {
                strWhere += " and itemno = '" + json.Value<string>("ITEMNO").ToString() + "'";
            }
            if (!string.IsNullOrEmpty(recordinfoid))
            {
                strWhere += " and recordinfoid = '" + recordinfoid + "'";
            }
            if (!string.IsNullOrEmpty(json.Value<string>("ITEMNOATTRIBUTE").ToString()))
            {
                strWhere += " and itemnoattribute = '" + json.Value<string>("ITEMNOATTRIBUTE").ToString() + "'";
            }
            sql = sql + strWhere;
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        public int update_record_details(JObject json,string recordinfoid)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"update sys_recordinfo_detail set recordinfoid='{0}',itemno='{1}',hscode='{2}',additionalno='{3}',itemnoattribute='{4}',commodityname='{5}',specificationsmodel='{6}',unit='{7}',version='{8}',enabled='{9}',remark='{10}', stopman='{11}',
            startdate=to_date('{12}','yyyy/mm/dd hh24:mi:ss'), enddate=to_date('{13}','yyyy/mm/dd hh24:mi:ss'),abnormal='{14}',partno='{15}' where id='{16}'";
            sql = String.Format(sql, recordinfoid, json.Value<string>("ITEMNO"), json.Value<string>("HSCODE"), json.Value<string>("ADDITIONALNO"), json.Value<string>("ITEMNOATTRIBUTE"), json.Value<string>("COMMODITYNAME").Replace("'", "'||chr(39)||'"),
                json.Value<string>("SPECIFICATIONSMODEL").Replace("'", "'||chr(39)||'"), json.Value<string>("UNIT"), json.Value<string>("VERSION"), json.Value<string>("ENABLED"), json.Value<string>("REMARK"), stopman,
                startdate,enddate,0,json.Value<string>("PARTNO"),json.Value<string>("ID"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        //插入修改记录
        public int insert_alert_record_details(JObject json,string contents)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"insert into base_alterrecord(id,
                            tabid,tabkind,alterman,
                            reason,contentes,alterdate) values(base_alterrecord_id.nextval,
                            '{0}','{1}','{2}',
                            '{3}','{4}',sysdate)";
            sql = String.Format(sql,json.Value<string>("ID"),(int)Base_YearKindEnum.Busi_RecordInfoDetail,createman,json.Value<string>("REASON"),contents);
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        //根据id来查找未修改之前的数据记录
        public DataTable get_before_change_record_details(JObject json)
        {
            string sql = @"select * from sys_recordinfo_detail where id = '" + json.Value<string>("ID") + "'";
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }
        //导出
        public DataTable export_record_details(string strwhere, string recordinfoid)
        {
            string sql = @"SELECT t1.*,
                           t2.name AS createmanname,
                           t3.name AS stopmanname,
                           t4.name AS unitname,
                           t5.code as recordinfocode,
                           t6.id   as hsid
                      FROM SYS_RECORDINFO_DETAIL t1
                      LEFT JOIN SYS_USER t2
                        ON t1.createman = t2.id
                      LEFT JOIN SYS_USER t3
                        ON t1.stopman = t3.id
                      LEFT JOIN BASE_DECLPRODUCTUNIT t4
                        ON t1.unit = t4.code
                       AND t4.enabled = 1
                      LEFT JOIN SYS_RECORDINFO t5
                        ON t1.recordinfoid = t5.id
                      LEFT JOIN (select id,
                                        hscode,
                                        extracode,
                                        enabled,
                                        ROW_NUMBER() OVER(PARTITION BY bc.hscode, bc.extracode ORDER BY bc.enabled desc, bc.id) as rownumber
                                   from BASE_COMMODITYHS bc) t6
                        ON t1.hscode = t6.hscode
                       AND t6.enabled = 1
                       AND t1.additionalno = t6.extracode
                       AND t6.rownumber = 1 where t1.recordinfoid = '{0}' {1}";
            sql = String.Format(sql,recordinfoid,strwhere);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }
    }
}