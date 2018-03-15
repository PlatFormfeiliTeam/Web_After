using System;
using System.Data;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Web_After.BasicManager;
using Web_After.BasicManager.BasicManager;
using Web_After.Common;

namespace Web_After.Sql
{
    public class Base_year
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
                           t3.name as customareaname,
                           s.name  as stopmanName
                           from base_year t1
                           left join sys_user t2
                           on t1.createman = t2.id
                           left join BASE_CUSTOMDISTRICT t3
                           on t1.customarea = t3.code
                           left join sys_user s
                           on t1.stopman = s.id
                           where t1.kind ={0} {1}
                            ";
            sql = string.Format(sql, (int)Base_YearKindEnum.HS, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.name", "desc", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }

        //插入代码库
        public int insertTable(JObject json)
        {
            
            bcm.getCommonInformation(out stopman,out createman,out startdate,out enddate,json);
            string sql = @"insert into base_year(id,name,startdate,enddate,createdate,enabled,remark,createman,kind,customarea,stopman) values(base_ciqyear_id.nextval,'{0}',to_date('{1}','yyyy/mm/dd hh24:mi:ss')
                        ,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),sysdate,'{3}','{4}','{5}'，'{6}','{7}','{8}')";
            sql = String.Format(sql, json.Value<string>("NAME"),startdate,enddate,json.Value<string>("ENABLED"), json.Value<string>("REMARK"),createman,(int)Base_YearKindEnum.HS, "",stopman);
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        //查询base_year是否有重复HS代码库（新增）
        public DataSet check_base_year(JObject json)
        {
            string sql = @"select * from base_year where name = '{0}' and kind = '{1}'";
            sql = string.Format(sql, json.Value<string>("NAME"), (int)Base_YearKindEnum.HS);
            return DBMgrBase.GetDataSet(sql);
        }
       //更新base_year中的HS代码库
        public int update_base_year(JObject json)
        {
            
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"update base_year set name ='{0}',startdate=to_date('{1}','yyyy/mm/dd '),enddate=to_date('{2}','yyyy/mm/dd '),customarea='{3}',
                           remark='{4}',enabled='{6}',stopman='" + stopman + "'  where id='{5}'";
            sql = String.Format(sql, json.Value<string>("NAME"), startdate, enddate, "", json.Value<string>("REMARK"), json.Value<string>("ID"), json.Value<string>("ENABLED"));
            return DBMgrBase.ExecuteNonQuery(sql);
        }

        //查询base_year是否有重复HS代码库根据id和name（修改）
        public DataSet check_base_year_by_idandname(JObject json)
        {
            string sql = @"select * from base_year where name = '{0}' and id not in ('{1}') and kind = '{2}'";
            sql = string.Format(sql, json.Value<string>("NAME"), json.Value<string>("ID"), (int)Base_YearKindEnum.HS);
            return DBMgrBase.GetDataSet(sql);
        }
        //根据id来找base_year的数据
        public DataTable getBeforeChangData(JObject json)
        {
            string sql = @"select * from base_year where id = '{0}'";
            sql = string.Format(sql,json.Value<string>("ID"));
            return DBMgrBase.GetDataTable(sql);
        }
        //增添修改记录
        public int insert_base_alterrecord(JObject json, string content)
        {
            //Tabid
            string Tabid = json.Value<string>("ID");
            //TabKind
            int TabKind = (int)Base_YearKindEnum.Base_Year;
            //AlterMan(id)
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string AlterMan = json_user.GetValue("ID").ToString();
            //AlterDate数据库中的时间
            //Reason
            string Reason = json.Value<string>("REASON");
            //content
            string sql =
                @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                sysdate)";
            sql = string.Format(sql, Tabid, TabKind, AlterMan, Reason, content);
            return DBMgrBase.ExecuteNonQuery(sql);
        }

        //获取HS代码的数据
        public DataTable MaintainloadData(string id, string strWhere, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,
                           t2.name as createmanname,
                           t3.name as stopmanname,
                           t4.name as yearname,
                           t5.name as weightname,
                           t6.name as numname,
                           t7.name as legalunitname
                              from base_insphs t1
                              left join sys_user t2
                                on t1.createman = t2.id
                              left join sys_user t3
                                on t1.stopman = t3.id
                              left join base_year t4
                                on t1.yearid = t4.id
                              left join BASE_PRODUCTUNIT t5
                                on t1.weight = t5.code
                              left join BASE_PRODUCTUNIT t6
                                on t6.code = t1.num
                              left join base_productunit t7
                                on t1.legalunit = t7.code
                           where 1 = 1
                           and t1.yearid = '{0}' {1}
                           ";
            sql = string.Format(sql, id, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.hscode", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }

        //新增HS编码
        public int insert_base_insphs(JObject json,string yearid)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"insert into base_insphs(id,
                           hscode,hsname,weight,customregulatory,
                           yearid,num,createdate,startdate,
                           enddate,createman,stopman,enabled,
                           remark,inspectionregulatory,legalunit) values(base_insphs_id.nextval,
                           '{0}','{1}','{2}','{3}',
                           '{4}','{5}',sysdate,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),
                           to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}','{9}','{10}',
                           '{11}','{12}','{13}')";
            sql = String.Format(sql,
                            json.Value<string>("HSCODE"), json.Value<string>("HSNAME"), json.Value<string>("WEIGHT"), json.Value<string>("CUSTOMREGULATORY"),
                            yearid, json.Value<string>("NUMNAME"), startdate, 
                            enddate, createman, stopman, json.Value<string>("ENABLED"),
                            json.Value<string>("REMARK"), json.Value<string>("INSPECTIONREGULATORY"), json.Value<string>("LEGALUNITNAME"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        //检查HS编码是否重复
        public DataTable check_repeat_base_insphs(JObject json,string yearid)
        {
            string sql = @"select * from base_insphs where hscode = '{0}' and yearid = '{1}'";
            sql = String.Format(sql,json.Value<string>("HSCODE"),yearid);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //更新HS编码
        public int update_base_insphs(JObject json,string yearid)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"update base_insphs set 
                            hscode='{0}',hsname='{1}',weight='{2}',
                            customregulatory='{3}',inspectionregulatory='{4}',
                            num='{5}',startdate=to_date('{6}','yyyy/mm/dd hh24:mi:ss'),enddate=to_date('{7}','yyyy/mm/dd hh24:mi:ss'),
                            remark='{8}',legalunit='{9}',stopman = '{10}' where id='{11}'";
            sql = String.Format(sql,
                            json.Value<string>("HSCODE"), json.Value<string>("HSNAME"), json.Value<string>("WEIGHT"),
                            json.Value<string>("CUSTOMREGULATORY"), json.Value<string>("INSPECTIONREGULATORY"),
                            json.Value<string>("NUMNAME"), startdate, enddate, json.Value<string>("REMARK"), json.Value<string>("LEGALUNITNAME"), stopman,json.Value<string>("ID")
                            );
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        //更新时验证HS编码重复
        public DataTable check_repeat_base_insphs_update(JObject json,string yearid)
        {
            string sql = @"select * from base_insphs where hscode = '{0}' and id not in ('{1}') and yearid = '{2}'";
            sql = String.Format(sql, json.Value<string>("HSCODE"), json.Value<string>("ID"),yearid);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }
        //导出数据
        public DataTable export_table(string yearid,string strwhere)
        {
            string sql = @"select t1.*,
                           t2.name as createmanname,
                           t3.name as stopmanname,
                           t4.name as yearname,
                           t5.name as weightname,
                           t6.name as numname,
                           t7.name as legalunitname
                              from base_insphs t1
                              left join sys_user t2
                                on t1.createman = t2.id
                              left join sys_user t3
                                on t1.stopman = t3.id
                              left join base_year t4
                                on t1.yearid = t4.id
                              left join BASE_PRODUCTUNIT t5
                                on t1.weight = t5.code
                              left join BASE_PRODUCTUNIT t6
                                on t6.code = t1.num
                              left join base_productunit t7
                                on t1.legalunit = t7.code
                           where 1 = 1
                           and t1.yearid = '{0}' {1}
                           ";
            sql = string.Format(sql, yearid, strwhere);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }
        //根据ID来找出修改之前的数据
        public DataTable Before_Change(JObject json)
        {
            string sql = @"select * from base_insphs where id = '{0}'";
            sql = String.Format(sql,json.Value<string>("ID"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }
        //保存修改记录
        public int saveChangeBaseHsCode(JObject json, string content)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);

            string sql = @"insert into base_alterrecord(id,
                            tabid,tabkind,alterman,
                            reason,contentes,alterdate) values(base_alterrecord_id.nextval,
                            '{0}','{1}','{2}',
                            '{3}','{4}',sysdate)";
            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.HS, json_user.GetValue("ID"),
                json.Value<string>("REASON"), content
            );
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

    }
}