using System;
using System.Data;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Web_After.BasicManager;
using Web_After.Common;

namespace Web_After.Sql
{
    public class Basic_ciqcode
    {
        //获取ICQ代码库的数据
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
            sql = string.Format(sql,(int)Base_YearKindEnum.CIQ,strWhere);
            sql = Extension.GetPageSql2(sql, "t1.name", "desc", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }
        //新增ICQ代码库
        public int insert_base_year(JObject json)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            
            string sql = @"insert into base_year(id,name,startdate,enddate,createdate,enabled,remark,createman,kind,customarea) values(base_ciqyear_id.nextval,'{0}',to_date('{1}','yyyy/mm/dd hh24:mi:ss')
                        ,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),sysdate,'{3}','{4}','{5}'，'{6}','{7}')";
            sql = string.Format(sql, json.Value<string>("NAME"), json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"), json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"), json.Value<string>("ENABLED"), json.Value<string>("REMARK"), json_user.GetValue("ID"), (int)Base_YearKindEnum.CIQ, "");
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        //查询base_year是否有重复ICQ代码库（新增）
        public DataSet check_base_year(JObject json)
        {
            string sql = @"select * from base_year where name = '{0}'";
            sql = string.Format(sql, json.Value<string>("NAME"));
            return DBMgrBase.GetDataSet(sql);
        }

        //修改按钮所需sql
        public int update_base_year(JObject json,string stopman)
        {
            string sql = @"update base_year set name ='{0}',startdate=to_date('{1}','yyyy/mm/dd '),enddate=to_date('{2}','yyyy/mm/dd '),customarea='{3}',
                           remark='{4}',enabled='{6}',stopman='"+stopman+"'  where id='{5}'";
            sql = String.Format(sql, json.Value<string>("NAME"), json.Value<string>("STARTDATE"), json.Value<string>("ENDDATE"), "", json.Value<string>("REMARK"), json.Value<string>("ID"), json.Value<string>("ENABLED"));
            return DBMgrBase.ExecuteNonQuery(sql);
        }

        //查询base_year是否有重复ICQ代码库根据id和name（修改）
        public DataSet check_base_year_by_idandname(JObject json)
        {
            string sql = @"select * from base_year where name = '{0}' and id not in ('{1}')";
            sql = string.Format(sql, json.Value<string>("NAME"), json.Value<string>("ID"));
            return DBMgrBase.GetDataSet(sql);
        }

        //增添修改记录
        public int insert_base_alterrecord(JObject json,string content)
        {
            //Tabid
            string Tabid = json.Value<string>("ID");
            //TabKind
            int TabKind = (int)Base_YearKindEnum.Base_Year;
            //AlterMan(id)
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string  AlterMan = json_user.GetValue("ID").ToString();
            //AlterDate数据库中的时间
            //Reason
            string Reason = json.Value<string>("REASON");
            //content
            string sql =
                @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                sysdate)";
            sql = string.Format(sql, Tabid, TabKind, AlterMan, Reason,content);
            return DBMgrBase.ExecuteNonQuery(sql);
        }

        //获取CIQ代码的数据
        public DataTable MaintainloadData(string id, string strWhere, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,
                           t2.name as createmanname,
                           t3.name as stopmanname,
                           t4.name as yearname
                           from base_ciqcode t1
                           left join sys_user t2
                           on t1.createman = t2.id
                           left join sys_user t3
                           on t1.stopman = t3.id
                           left join base_year t4
                           on t1.yearid = t4.id
                           where 1 = 1
                           and t1.yearid = '{0}' {1}
                           ";
            sql = string.Format(sql,id,strWhere);
            sql = Extension.GetPageSql2(sql, "t1.ciq", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }

        //重复CIQ代码验证
        public DataTable check_repeat_base_ciqcode(JObject json,string yearid)
        {
            string sql = "select * from base_ciqcode where ciq = '{0}' ";
            sql = String.Format(sql,json.Value<string>("CIQ"),yearid);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //插入表base_ciqcode
        public int insert_base_ciqcode(JObject json,string yerid)
        {
            //停用人
            string stopman = "";
           
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            if (json.Value<string>("ENABLED") == "1")
            {
                stopman = "";
            }
            else
            {
                stopman = json_user.GetValue("ID").ToString();

            }

            string sql = @"insert into base_ciqcode(
                            id,
                            ciq,ciqname,enabled,createman,
                            stopman,createdate,
                            startdate,enddate,
                            yearid,remark) values(
                            base_ciq_id.nextval,
                            '{0}','{1}','{2}','{3}',
                            '{4}',sysdate,
                            to_date('{5}','yyyy/mm/dd hh24:mi:ss'),to_date('{6}','yyyy/mm/dd hh24:mi:ss'),
                            '{7}','{8}')";
            sql = String.Format(sql,
                            json.Value<string>("CIQ"), json.Value<string>("CIQNAME"), json.Value<string>("ENABLED"), json_user.GetValue("ID"),
                            stopman,
                            json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"), json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
                            yerid, json.Value<string>("REMARK")
                            );
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        //更新时检查是否存在该CIQ代码
        public DataTable update_check_repeat_base_ciqcode(JObject json)
        {
            string sql = @"select * from base_ciqcode where ciq = '{0}' and id not in ('{1}')";
            sql = String.Format(sql,json.Value<string>("CIQ"),json.Value<string>("ID"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //更新表base_ciqcode
        public int update_base_ciqcode(JObject json)
        {
            //停用人
            string stopman = "";
           
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            if (json.Value<string>("ENABLED") == "1")
            {
                stopman = "";
            }
            else
            {
                stopman = json_user.GetValue("ID").ToString();

            }
            string sql = @"update base_ciqcode set ciq = '{0}', ciqname = '{1}' , 
                           enabled = '{2}' , createman = {3},stopman = '{4}' ,
                           startdate=to_date('{5}','yyyy/mm/dd '),enddate=to_date('{6}','yyyy/mm/dd '),
                           remark = '{7}' where id = '{8}'
                           ";
            sql = String.Format(sql, json.Value<string>("CIQ"), json.Value<string>("CIQNAME"),
                                json.Value<string>("ENABLED"), json_user.GetValue("ID"),stopman,
                                json.Value<string>("STARTDATE"), json.Value<string>("ENDDATE"),
                                json.Value<string>("REMARK"), json.Value<string>("ID")
                );
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }


        //根据id来获取修改之前的数据
        public DataTable GetChangeDataTable(JObject json)
        {
            string sql = "select * from base_ciqcode where id = '"+json.Value<string>("ID")+"'";
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }


        //保存修改记录
        public int saveChangeBaseCiqCode(JObject json,string content)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);

            string  sql = @"insert into base_alterrecord(id,
                            tabid,tabkind,alterman,
                            reason,contentes,alterdate) values(base_alterrecord_id.nextval,
                            '{0}','{1}','{2}',
                            '{3}','{4}',sysdate)";
            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.CIQ,json_user.GetValue("ID"),
                                json.Value<string>("REASON"),content
                                );
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        //导出
        public DataTable exoprt_base_ciqcode(string strWhere,string yearid)
        {
            string sql = @"select t1.*,
                           t2.name as createmanname,
                           t3.name as stopmanname,
                           t4.name as yearname
                           from base_ciqcode t1
                           left join sys_user t2
                           on t1.createman = t2.id
                           left join sys_user t3
                           on t1.stopman = t3.id
                           left join base_year t4
                           on t1.yearid = t4.id
                           where 1 = 1
                           and t1.yearid = '{0}' {1} order by t1.ciq asc
                           ";
            sql = String.Format(sql,yearid,strWhere);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //导入
        public int import_base_ciqcode(JObject json, string ciq, string ciqname, string ENABLED,string remark,string yearid)
        {
            //停用人
            string stopman = "";

            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            if (ENABLED == "1")
            {
                stopman = "";
            }
            else
            {
                stopman = json_user.GetValue("ID").ToString();

            }
            string sql = @"insert into base_ciqcode(
                            id,
                            ciq,ciqname,enabled,createman,
                            stopman,createdate,
                            startdate,enddate,
                            yearid,remark) values(
                            base_ciq_id.nextval,
                            '{0}','{1}','{2}','{3}',
                            '{4}',sysdate,
                            to_date('{5}','yyyy/mm/dd hh24:mi:ss'),to_date('{6}','yyyy/mm/dd hh24:mi:ss'),
                            '{7}','{8}')";
            sql = String.Format(sql,
                                ciq,ciqname,ENABLED,json_user.GetValue("ID"),
                                stopman,
                                json.Value<string>("STARTDATE"),json.Value<string>("ENDDATE"),
                                yearid,remark
                                );

            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        //导入查看是否有ciq代码重复

        public DataTable Before_import_check(string ciq)
        {
            string sql = "select * from base_ciqcode where ciq = '"+ciq+"'";
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;

        }
    }
}