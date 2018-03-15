using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using Web_After.Common;
using Web_After.model;

namespace Web_After.BasicManager.BasicManager
{
    public class Switch_helper_Base_blend_web
    {
        public string get_base_sql(string type, string table, JObject json, string contents)
        {
            string sql = "";
            string startdate = "";
            string enddate = "";
            string stopman = "";
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            if (json != null)
            {
                startdate = json.Value<string>("STARTDATE") == ""
                    ? DateTime.MinValue.ToShortDateString()
                    : json.Value<string>("STARTDATE");
                enddate = json.Value<string>("ENDDATE") == ""
                    ? DateTime.MaxValue.ToShortDateString()
                    : json.Value<string>("ENDDATE");
                if (json.Value<string>("ENABLED") == "1")
                {
                    stopman = "";
                }
                else
                {
                    stopman = json_user.GetValue("ID").ToString();
                }
            }


            string createman = json_user.GetValue("ID").ToString();
            switch (type)
            {
                //数据界面加载
                case "load":
                    switch (table)
                    {
                        case "insp_portin":
                            sql = @"select t1.*,
                            t2.name as createmanname,
                            t3.name as stopmanname,
                            t4.name as countryname
                            from base_port t1
                                left join sys_user t2
                                on t1.createman = t2.id
                            left join sys_user t3
                            on t1.stopman = t3.id
                            left join base_inspcountry t4
                            on t1.country = t4.code
                            where in_out = '1'";
                            break;
                        case "insp_portout":
                            sql = @"select t1.*,
                            t2.name as createmanname,
                            t3.name as stopmanname,
                            t4.name as countryname
                            from base_port t1
                            left join sys_user t2
                               on t1.createman = t2.id
                            left join sys_user t3
                               on t1.stopman = t3.id
                            left join base_inspcountry t4
                               on t1.country = t4.code
                            where in_out = '2'";
                            break;
                        case "base_currency":
                            sql = @"select t1.*,
                            t2.name as createmanname,
                            t3.name as stopmanname,
                            t4.name as yearname
                            from base_currency t1
                                left join sys_user t2
                                on t1.createman = t2.id
                            left join sys_user t3
                            on t1.stopman = t3.id
                            left join base_year t4
                            on t1.yearid = t4.id where 1 = 1 ";
                            break;
                        case "base_inspcountry":
                            sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
                              from base_inspcountry t1
                              left join sys_user t2
                                on t1.createman = t2.id
                              left join sys_user t3
                                on t1.stopman = t3.id where 1 = 1 ";
                            break;
                        case "base_productunit":
                            sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
                                  from base_productunit t1
                                  left join sys_user t2
                                    on t1.createman = t2.id
                                  left join sys_user t3
                                    on t1.stopman = t3.id where 1 = 1 ";
                            break;
                        case "base_insplicense":
                            sql = @"select t1.*,
                                       t2.name as createmanname,
                                       t3.name as stopmanname,
                                       t4.name as yearname
                                  from base_insplicense t1
                                  left join sys_user t2
                                    on t1.createman = t2.id
                                  left join sys_user t3
                                    on t1.stopman = t3.id
                                  left join base_year t4
                                    on t1.yearid = t4.id where 1 = 1";
                            break;
                        case "base_inspinvoice":
                            sql = @"select t1.*,
                                   t2.name as createmanname,
                                   t3.name as stopmanname,
                                   t4.name as yearname
                              from Base_InspInvoice t1
                              left join sys_user t2
                                on t1.createman = t2.id
                              left join sys_user t3
                                on t1.stopman = t3.id
                              left join base_year t4
                                on t1.yearid = t4.id where 1 = 1";
                            break;
                        case "base_country":
                            sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
                                  from base_country t1
                                  left join sys_user t2
                                    on t1.createman = t2.id
                                  left join sys_user t3
                                    on t1.stopman = t3.id where 1 = 1";
                            break;
                        case "base_decltradeway":
                            sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
                                  from base_decltradeway t1
                                  left join sys_user t2
                                    on t1.createman = t2.id
                                  left join sys_user t3
                                    on t1.stopman = t3.id where 1 = 1 ";
                            break;
                        case "base_exemptingnature":
                            sql = @"select t1.*,
                               t2.name as createmanname,
                               t3.name as stopmanname,
                               t4.name as yearname
                              from base_exemptingnature t1
                              left join sys_user t2
                                on t1.createman = t2.id
                              left join sys_user t3
                                on t1.stopman = t3.id
                              left join base_year t4
                                on t1.yearid = t4.id where 1 = 1 ";
                            break;
                        case "base_exchangeway":
                            sql = @"select t1.*,
                            t2.name as createmanname,
                            t3.name as stopmanname,
                            t4.name as yearname
                            from base_exchangeway t1
                                left join sys_user t2
                                on t1.createman = t2.id
                            left join sys_user t3
                            on t1.stopman = t3.id
                            left join base_year t4
                            on t1.yearid = t4.id where 1 = 1";
                            break;
                        case "base_harbour":
                            sql = @"select t1.*,
                                   t2.name as countryname,
                                   t5.name as createmanname,
                                   t3.name as stopmanname,
                                    t4.name as yearname
                              from base_harbour t1
                              left join base_country t2
                                on t1.country = t2.code
                              left join sys_user t3
                                on t1.stopman = t3.id
                              left join base_year t4
                                on t1.yearid = t4.id
                              left join sys_user t5
                                on t1.createman = t5.id
                             where t1.enabled = 1";
                            break;
                        case "base_booksdata":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as tradename  from base_booksdata t1 left join
                            sys_user t2 on t1.createman=t2.id left join sys_user t3 on t1.stopman=t3.id left join  base_decltradeway t4 on t1.trade=t4.code where 1 = 1 ";
                            break;
                        case "sys_repway":
                            sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
                              from SYS_REPWAY t1
                              left join sys_user t2
                                on t1.createman = t2.id
                              left join sys_user t3
                                on t1.stopman = t3.id where 1 = 1";
                            break;
                        case "base_containersize":
                            sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
                              from base_containersize t1
                              left join sys_user t2
                                on t1.createman = t2.id
                              left join sys_user t3
                                on t1.stopman = t3.id where 1=1 ";
                            break;
                        case "base_containertype":
                            sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
                                  from base_containertype t1
                                  left join sys_user t2
                                    on t1.createman = t2.id
                                  left join sys_user t3
                                    on t1.stopman = t3.id where 1=1 ";
                            break;
                        case "sys_woodpacking":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from SYS_WOODPACKING t1 left join sys_user t2 on t1.createman=t2.id 
                            left join sys_user t3 on t1.stopman=t3.id where 1 = 1 ";
                            break;
                        case "sys_declarationcar":
                            sql = @"select t1.*,
                                    t1.license as code,
                                    t1.whitecard as name,
                                    t2.name as createmanname,
                                    t3.name as stopmanname,
                                    t4.name as motorcadename
                                  from sys_declarationcar t1
                                  left join sys_user t2
                                    on t1.createman = t2.id
                                  left join sys_user t3
                                    on t1.stopman = t3.id
                                  left join base_motorcade t4
                                    on t1.motorcade = t4.code where 1 = 1 ";
                            break;
                        case "sys_reportlibrary":
                            sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
                                  from sys_reportlibrary t1
                                  left join sys_user t2
                                    on t1.createman = t2.id
                                  left join sys_user t3
                                    on t1.stopman = t3.id where 1 = 1 ";
                            break;
                    }
                
                break;

                //新增时检查数据是否重复
                case "checkrepeat":
                    switch (table)
                    {
                        case "insp_portin":
                            //sql = @"select * from base_port where in_out = '1' and code = '{0}'";
                            sql = @"select * from base_port where  code = '{0}'";
                            sql = string.Format(sql,json.Value<string>("CODE"));
                            break;
                        case "insp_portout":
                            //sql = @"select * from base_port where in_out = '2' and code = '{0}'";
                            sql = @"select * from base_port where  code = '{0}'";
                            sql = string.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "base_currency":
                            sql = @"select * from base_currency where code = '{0}'";
                            sql = String.Format(sql,json.Value<string>("CODE"));
                            break;
                        case "base_inspcountry":
                            sql = @"select * from base_inspcountry where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "base_productunit":
                            sql = @"select * from base_productunit where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "base_insplicense":
                            sql = @"select * from base_insplicense where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "base_inspinvoice":
                            sql = @"select * from base_inspinvoice where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "base_country":
                            sql = @"select * from base_country where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));                            
                            break;
                        case "base_decltradeway":
                            sql = @"select * from base_decltradeway where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE")); 
                            break;
                        case "base_exemptingnature":
                            sql = @"select * from base_exemptingnature where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "base_exchangeway":
                            sql = @"select * from base_exchangeway where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "base_harbour":
                            sql = @"select * from base_harbour where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "base_booksdata":
                            sql = @"select * from base_booksdata where trade = '{0}' and isinportname = '{1}' and isproductname = '{2}'";
                            sql = String.Format(sql, json.Value<string>("TRADE"), json.Value<string>("ISINPORTNAME"), json.Value<string>("ISPRODUCTNAME"));
                            break;
                        case "sys_repway":
                            sql = @"select * from sys_repway where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "base_containersize":
                            sql = @"select * from base_containersize where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "base_containertype":
                            sql = @"select * from base_containertype where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "sys_woodpacking":
                            sql = @"select * from sys_woodpacking where code = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "sys_declarationcar":
                            sql = @"select * from sys_declarationcar where license = '{0}'";
                            sql = String.Format(sql, json.Value<string>("CODE"));
                            break;
                        case "sys_reportlibrary":
                            sql = @"select * from sys_reportlibrary where code = '{0}'";
                            break;
                    }
                break;
                
                //新增数据
                case "insert":
                    switch (table)
                    {
                        case "insp_portin":
                            sql = @"insert into base_port(id,code,name,englishname,
                                    createdate,startdate,enddate,createman,stopman,
                                    enabled,remark,in_out)
                                    values(base_port_id.nextval,'{0}','{1}','{2}',
                                    sysdate,to_date('{3}','yyyy/mm/dd hh24:mi:ss'),to_date('{4}','yyyy/mm/dd hh24:mi:ss'),
                                    '{5}','{6}','{7}','{8}','1')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("ENGLISHNAME"),
                                startdate,enddate,createman,stopman,json.Value<string>("ENABLED"),json.Value<string>("REMARK"));
                            break;
                        case "insp_portout":
                            sql = @"insert into base_port(id,code,name,englishname,
                                    createdate,startdate,enddate,createman,stopman,
                                    enabled,remark,in_out,country)
                                    values(base_port_id.nextval,'{0}','{1}','{2}',
                                    sysdate,to_date('{3}','yyyy/mm/dd hh24:mi:ss'),to_date('{4}','yyyy/mm/dd hh24:mi:ss'),
                                    '{5}','{6}','{7}','{8}','2','{9}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("ENGLISHNAME"),
                                startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"), json.Value<string>("COUNTRY"));
                            break;
                        case "base_currency":
                            sql = @"insert into base_currency(id,code,name,
                                    createdate,startdate,enddate,
                                    createman,stopman,enabled,remark,abbreviation)
                                    values(base_currency_id.nextval,'{0}','{1}',
                                    sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}','{8}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"),
                                    startdate,enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"),json.Value<string>("ABBREVIATION"));
                            break;
                        case "base_inspcountry":
                            sql = @"insert into base_inspcountry(id,code,name,englishname,airabbrev,oceanabbrev
                                    ,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_country_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate,to_date('{5}','yyyy/mm/dd hh24:mi:ss'),to_date('{6}','yyyy/mm/dd hh24:mi:ss'),
                                    '{7}','{8}','{9}','{10}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("ENGLISHNAME"), json.Value<string>("AIRABBREV"), json.Value<string>("OCEANABBREV"),
                                startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_productunit":
                            sql = @"insert into base_productunit(id,code,name,
                                    createdate,startdate,enddate,createman,
                                    stopman,enabled,remark,englishname)values(base_productunit_id.nextval,
                                    '{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}','{8}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"),
                                startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"), json.Value<string>("ENGLISHNAME"));
                            break;
                        case "base_insplicense":
                             sql = @"insert into base_insplicense(id,code,inname,outname,
                                    enabled,createman,stopman,createdate,startdate,enddate,remark) values(base_insplicense_id.nextval,
                                '{0}','{1}','{2}','{3}','{4}','{5}',sysdate,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}')";
                             sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("INNAME"), json.Value<string>("OUTNAME"),
                                 json.Value<string>("ENABLED"), createman, stopman, startdate, enddate, json.Value<string>("REMARK"));
                            break;
                        case "base_inspinvoice":
                            sql = @"insert into Base_InspInvoice(id,code,inname,outname,enabled,createman,stopman,createdate,startdate,enddate,remark) values(Base_InspInvoice_id.nextval,
                            '{0}','{1}','{2}','{3}','{4}','{5}',sysdate,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("INNAME"), json.Value<string>("OUTNAME"),
                                json.Value<string>("ENABLED"), createman, stopman, startdate, enddate, json.Value<string>("REMARK"));
                            break;
                        case "base_country":
                            sql = @"insert into base_country(id,code,name,englishname,rate,remark,enabled,createman,stopman,createdate,startdate,enddate,ezm) values(base_customdistrict_id.nextval,
                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',sysdate,to_date('{8}','yyyy/mm/dd hh24:mi:ss'),to_date('{9}','yyyy/mm/dd hh24:mi:ss'),'{10}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("ENGLISHNAME"), json.Value<string>("RATE"),json.Value<string>("REMARK")
                                , json.Value<string>("ENABLED"), createman, stopman, startdate, enddate, json.Value<string>("EZM"));
                            break;
                        case "base_decltradeway":
                            sql = @"insert into base_decltradeway(id,code,name,remark,enabled,createman,stopman,createdate,startdate,enddate,fullname) values(base_decltradeway_id.nextval,
                            '{0}','{1}','{2}','{3}','{4}','{5}',sysdate,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), json.Value<string>("ENABLED"), createman, stopman, startdate, enddate, json.Value<string>("FULLNAME"));
                            break;
                        case "base_exemptingnature":
                            sql = @"insert into base_ExemptingNature(id,code,name,remark,enabled,createman,stopman,createdate,startdate,enddate,fullname) values(base_ExemptingNature_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}',sysdate,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),
                            to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), json.Value<string>("ENABLED"), createman, stopman, startdate, enddate, json.Value<string>("FULLNAME"));
                            break;
                        case "base_exchangeway":
                            sql = @"insert into base_exchangeway(id,code,name,remark,enabled,createman,stopman,createdate,startdate,enddate,fullname) values(
                                    base_exchangeway_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}',sysdate,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),
                                    to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), json.Value<string>("ENABLED"), createman, stopman, startdate, enddate, json.Value<string>("FULLNAME"));
                            break;
                        case "base_harbour":
                            sql = @"insert into base_harbour(id,code,name,remark,enabled,createman,stopman,createdate,startdate,enddate,country,englishname) values(base_harbour_id.nextval,
                            '{0}','{1}','{2}','{3}','{4}','{5}',sysdate,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}','{9}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), json.Value<string>("ENABLED"), createman, stopman, startdate, enddate, json.Value<string>("COUNTRY"), json.Value<string>("ENGLISHNAME"));
                            break;
                        case "base_booksdata":
                            sql = @"insert into base_booksdata(id,trade,isinportname,isproductname,remark,enabled,createman,stopman,createdate,startdate,enddate) values
                            (base_booksdata_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}',sysdate,to_date('{7}','yyyy/mm/dd hh24:mi:ss'),
                            to_date('{8}','yyyy/mm/dd hh24:mi:ss'))";
                            sql = String.Format(sql, json.Value<string>("TRADE"), json.Value<string>("ISINPORTNAME"), json.Value<string>("ISPRODUCTNAME"), json.Value<string>("REMARK"), json.Value<string>("ENABLED"), createman, stopman, startdate, enddate);
                            break;
                        case "sys_repway":
                            sql = @"insert into sys_repway(id,code,name,remark,enabled,createman,stopman,createdate,startdate,enddate,busitype) values(sys_repway_id.nextval,
                            '{0}','{1}','{2}','{3}','{4}','{5}',sysdate,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), json.Value<string>("ENABLED"),createman,stopman,startdate,enddate,json.Value<string>("BUSITYPE"));
                            break;
                        case "base_containersize":
                            sql = @"insert into base_containersize(id,code,name,remark,enabled,createman,stopman,createdate,startdate,enddate,declsize) values(
                            base_containersize_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}',sysdate,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),
                            to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), json.Value<string>("ENABLED"), createman, stopman, startdate, enddate, json.Value<string>("DECLSIZE"));
                            break;
                        case "base_containertype":
                            sql = @"insert into base_containertype(id,code,name,remark,enabled,createman,stopman,createdate,startdate,enddate,containercode) values(
                            base_containertype_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}',sysdate,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),
                            to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), json.Value<string>("ENABLED"), createman, stopman, startdate, enddate, json.Value<string>("CONTAINERCODE"));
                            break;
                        case "sys_woodpacking":
                            sql = @"select hscode, hsname, inspectionregulatory, customregulatory
                                    from base_insphs  where enabled = 1 and hscode = '"+json.Value<string>("HSCODE")+"'";
                            DataTable dt = DBMgrBase.GetDataTable(sql);
                            sql = @"insert into SYS_WOODPACKING(id,code,name,hscode,inspection,declaration,remark,enabled,createman,stopman,createdate,startdate,enddate) values(SYS_WOODPACKING_id.nextval,
                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',sysdate,to_date('{9}','yyyy/mm/dd hh24:mi:ss'),to_date('{10}','yyyy/mm/dd hh24:mi:ss'))";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("HSCODE"), dt.Rows[0]["InspectionRegulatory"], dt.Rows[0]["CustomRegulatory"], json.Value<string>("REMARK"), json.Value<string>("ENABLED"), createman, stopman, startdate, enddate);
                            break;
                        case "sys_declarationcar":
                            sql = @"insert into sys_declarationcar(id,license,whitecard,models,motorcade,remark,enabled,createman,stopman,createdate,startdate,enddate) values(
                            sys_declarationcar_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',sysdate,to_date('{8}','yyyy/mm/dd hh24:mi:ss'),
                            to_date('{9}','yyyy/mm/dd hh24:mi:ss'))";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("MODELS"), json.Value<string>("MOTORCADE"), json.Value<string>("REMARK"), json.Value<string>("ENABLED"), createman, stopman, startdate, enddate);
                            break;
                        case "sys_reportlibrary":
                            sql = @"insert into sys_reportlibrary(id,code,name,remark,enabled,createman,stopman,createdate,startdate,enddate,declname,internaltype) 
                            values(sys_reportlibrary_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}',sysdate,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),
                            to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}','{9}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), json.Value<string>("ENABLED"), createman, stopman, startdate, enddate, json.Value<string>("DECLNAME"), json.Value<string>("INTERNALTYPE"));
                            break;


                    }
                break;

                //更新时验证代码是否重复
                case "updateCheckrepeat":
                    switch (table)
                    {
                        case "insp_portin":   case "insp_portout":
                            sql = @"select * from base_port where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_currency":
                            sql = @"select * from base_currency where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_inspcountry":
                            sql = @"select * from base_inspcountry where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_productunit":
                            sql = @"select * from base_productunit where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_insplicense":
                            sql = @"select * from base_insplicense where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_inspinvoice":
                            sql = @"select * from base_inspinvoice where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_country":
                            sql = @"select * from base_country where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_decltradeway":
                            sql = @"select * from base_decltradeway where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_exemptingnature":
                            sql = @"select * from base_exemptingnature where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_exchangeway":
                            sql = @"select * from base_exchangeway where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_harbour":
                            sql = @"select * from base_harbour where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_booksdata":
                            sql = @"select * from base_booksdata where trade = '{0}' and isinportname = '{1}' and isproductname = '{2}' and id not in '{3}'";
                            sql = String.Format(sql, json.Value<string>("TRADE"), json.Value<string>("ISINPORTNAME"), json.Value<string>("ISPRODUCTNAME"),json.Value<string>("ID"));
                            break;
                        case "sys_repway":
                            sql = @"select * from sys_repway where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_containersize":
                            sql = @"select * from base_containersize where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_containertype":
                            sql = @"select * from base_containertype where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "sys_woodpacking":
                            sql = @"select * from sys_woodpacking where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "sys_declarationcar":
                            sql = @"select * from sys_declarationcar where license = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "sys_reportlibrary":
                            sql = @"select * from sys_reportlibrary where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;

                    }
                break;

                //更新数据
                case "update":
                    switch (table)
                    {
                        case "insp_portin":
                            sql = @"update base_port set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}',englishname = '{7}' where id = '{8}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ENGLISHNAME"), json.Value<string>("ID"));
                            break;
                        case "insp_portout":
                            sql = @"update base_port set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}',englishname = '{7}',country = '{8}' where id = '{9}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ENGLISHNAME"), json.Value<string>("COUNTRY"), json.Value<string>("ID"));
                            break;
                        case "base_currency":
                            sql = @"update base_currency set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}',abbreviation = '{7}' where id = '{8}' ";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ABBREVIATION"), json.Value<string>("ID"));
                            break;
                        case "base_inspcountry":
                            sql = @"update base_inspcountry set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}',englishname = '{7}',airabbrev = '{8}',oceanabbrev = '{9}' where id = '{10}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ENGLISHNAME"), json.Value<string>("AIRABBREV"), json.Value<string>("OCEANABBREV"), json.Value<string>("ID"));
                            break;
                        case "base_productunit":
                            sql = @"update base_productunit set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}',englishname = '{7}' where id = '{8}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ENGLISHNAME"), json.Value<string>("ID"));
                            break;
                        case "base_insplicense":
                            sql = @"update base_insplicense set code = '{0}',inname = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}',outname = '{7}' where id = '{8}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("INNAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("OUTNAME"), json.Value<string>("ID"));
                            break;
                        case "base_inspinvoice":
                            sql = @"update base_inspinvoice set code = '{0}',inname = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}',outname = '{7}' where id = '{8}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("INNAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("OUTNAME"), json.Value<string>("ID"));
                            break;
                        case "base_country":
                            sql = @"update base_country set code='{0}', name='{1}', englishname='{2}', rate='{3}', remark='{4}',startdate=to_date('{5}','yyyy/mm/dd hh24:mi:ss'),
                                    enddate=to_date('{6}','yyyy/mm/dd hh24:mi:ss'),ezm='{7}',enabled = '{8}' where id= '{9}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("ENGLISHNAME"), json.Value<string>("RATE"),
                                json.Value<string>("REMARK"), startdate, enddate, json.Value<string>("EZM"), json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_decltradeway":
                            sql = @"update base_decltradeway set code='{0}', name='{1}', remark='{2}',startdate=to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    enddate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),fullname='{5}',enabled = '{6}' where id='{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), startdate, enddate, json.Value<string>("FULLNAME"), json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_exemptingnature":
                            sql = @"update base_exemptingnature set code='{0}', name='{1}', remark='{2}',startdate=to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    enddate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),fullname='{5}',enabled = '{6}' where id='{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), startdate, enddate, json.Value<string>("FULLNAME"), json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_exchangeway":
                            sql = @"update base_exchangeway set code='{0}', name='{1}', remark='{2}',startdate=to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    enddate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),fullname='{5}',enabled = '{6}'  where id='{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), startdate, enddate, json.Value<string>("FULLNAME"), json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_harbour":
                            sql = @"update base_harbour set code='{0}', name='{1}', remark='{2}',startdate=to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    enddate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),country='{5}',englishname='{6}',enabled = '{7}'  where id='{8}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), startdate, enddate, json.Value<string>("COUNTRY"), json.Value<string>("ENGLISHNAME"), json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_booksdata":
                            sql = @"update base_BooksData set trade='{0}', isinportname='{1}',isproductname='{2}', remark='{3}',startdate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),
                            enddate=to_date('{5}','yyyy/mm/dd hh24:mi:ss'),enabled = '{6}' where id='{7}'";
                            sql = String.Format(sql, json.Value<string>("TRADE"), json.Value<string>("ISINPORTNAME"), json.Value<string>("ISPRODUCTNAME"), json.Value<string>("REMARK"), startdate, enddate, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "sys_repway":
                            sql = @"update sys_repway set code='{0}',name='{1}',remark='{2}',startdate=to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                            enddate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),busitype='{5}',enabled = '{6}'  where id='{7}'";
                            sql = String.Format(sql,json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), startdate, enddate,json.Value<string>("BUSITYPE"),json.Value<string>("ENABLED"),json.Value<string>("ID"));
                            break;
                        case "base_containersize":
                            sql = @"update base_containersize set code='{0}', name='{1}', remark='{2}',startdate=to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                            enddate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),declsize='{5}',enabled = '{6}' where id='{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), startdate, enddate, json.Value<string>("DECLSIZE"), json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_containertype":
                            sql = @"update base_containertype set code='{0}', name='{1}', remark='{2}',startdate=to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                            enddate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),ContainerCode='{5}',enabled = '{6}' where id='{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), startdate, enddate, json.Value<string>("CONTAINERCODE"), json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "sys_woodpacking":
                            sql = @"select hscode, hsname, inspectionregulatory, customregulatory
                                    from base_insphs  where enabled = 1 and hscode = '"+json.Value<string>("HSCODE")+"'";
                            DataTable dt = DBMgrBase.GetDataTable(sql);
                            sql = @"update SYS_WOODPACKING set code='{0}', name='{1}', remark='{2}',startdate=to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                            enddate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),hscode='{5}',inspection='{6}',declaration='{7}',enabled = '{8}' where id='{9}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), startdate, enddate, json.Value<string>("HSCODE"), dt.Rows[0]["InspectionRegulatory"], dt.Rows[0]["CustomRegulatory"], json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "sys_declarationcar":
                            sql = @"update sys_declarationcar set license='{0}',whitecard='{1}', models='{2}', motorcade='{3}', remark='{4}',startdate=to_date('{5}','yyyy/mm/dd hh24:mi:ss'),
                            enddate=to_date('{6}','yyyy/mm/dd hh24:mi:ss'),enabled = '{7}' where id='{8}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("MODELS"), json.Value<string>("MOTORCADE"), json.Value<string>("REMARK"), startdate, enddate, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "sys_reportlibrary":
                            sql = @"update sys_reportlibrary set code='{0}', name='{1}', remark='{2}',startdate=to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                            enddate=to_date('{4}','yyyy/mm/dd hh24:mi:ss'),declname='{5}',internaltype='{6}',enabled='{7}' where id='{8}'";
                            sql = String.Format(sql,json.Value<string>("CODE"), json.Value<string>("NAME"),json.Value<string>("REMARK"),startdate,enddate,json.Value<string>("DECLNAME"),json.Value<string>("INTERNALTYPE"),json.Value<string>("ENABLED"),json.Value<string>("ID"));
                            break;
                    }
                    break;

                    //未修改之前的的数据
                case "BeforChange":
                    switch (table)
                    {
                        case "insp_portin":  case "insp_portout":
                            sql = @"select * from base_port where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_currency":
                            sql = @"select * from base_currency where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_inspcountry":
                            sql = @"select * from base_inspcountry where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_productunit":
                            sql = @"select * from base_productunit where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_insplicense":
                            sql = @"select * from base_insplicense where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_inspinvoice":
                            sql = @"select * from base_inspinvoice where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_country":
                            sql = @"select * from base_country where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_decltradeway":
                            sql = @"select * from base_decltradeway where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_exemptingnature":
                            sql = @"select * from base_exemptingnature where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_exchangeway":
                            sql = @"select * from base_exchangeway where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_harbour":
                            sql = @"select * from base_harbour where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_booksdata":
                            sql = @"select * from base_booksdata where id = '{0}'";
                            sql = String.Format(sql,json.Value<string>("ID"));
                            break;
                        case "sys_repway":
                            sql = @"select * from sys_repway where id = '{0}'";
                            sql = String.Format(sql,json.Value<string>("ID"));
                            break;
                        case "base_containersize":
                            sql = @"select * from base_containersize where id = '{0}'";
                            sql = String.Format(sql,json.Value<string>("ID"));
                            break;
                        case "base_containertype":
                            sql = @"select * from base_containertype where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "sys_woodpacking":
                            sql = @"select * from sys_woodpacking where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "sys_declarationcar":
                            sql = @"select * from sys_declarationcar where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "sys_reportlibrary":
                            sql = @"select * from sys_reportlibrary where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                    }
                    break;

                case "change":
                    switch (table)
                    {
                        case "insp_portin":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.PortIn, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "insp_portout":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.PortOut, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_currency":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Currency, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_inspcountry":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Country, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_productunit":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.ProductUnit, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_insplicense":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Insp_License, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_inspinvoice":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Insp_Invoice, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_country":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_Country, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_decltradeway":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_TradeWay, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_exemptingnature":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_ExemptingNature, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_exchangeway":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_ExchangeWay, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_harbour":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_Harbour, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_booksdata":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_BooksData, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_repway":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_ReportWay, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_containersize":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_ContainerSize, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_containertype":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_ContainerType, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_woodpacking":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_WoodPacking, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_declarationcar":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_DeclCar, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_reportlibrary":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_ReportLibrary, createman, json.Value<string>("REASON"), contents);
                            break;
                            
                    }
                    break;
                   
            }
            return sql;
        }

        //获取excel的名字
        public string get_excelname(string table)
        {
            string excelname = string.Empty;
            switch (table)
            {
                case "insp_portin":
                    excelname = "国内口岸";
                    break;
                case "insp_portout":
                    excelname = "国际口岸";
                    break;
                case "base_currency":
                    excelname = "币种";
                    break;
                case "base_inspcountry":
                    excelname = "国家代码";
                    break;
                case "base_productunit":
                    excelname = "计量单位";
                    break;
                case "base_insplicense":
                    excelname = "许可证代码";
                    break;
                case "base_inspinvoice":
                    excelname = "报检随附单据";
                    break;
                case "base_country":
                    excelname = "国别代码";
                    break;
                case "base_decltradeway":
                    excelname = "贸易方式";
                    break;
                case "base_exemptingnature":
                    excelname = "征免性质";
                    break;
                case "base_exchangeway":
                    excelname = "结汇性质";
                    break;
                case "base_harbour":
                    excelname = "港口代码";
                    break;
                case "base_booksdata":
                    excelname = "账册数据规则";
                    break;
                case "sys_repway":
                    excelname = "申报方式";
                    break;
                case "base_containersize":
                    excelname = "集装箱尺寸";
                    break;
                case "base_containertype":
                    excelname = "集装箱类型";
                    break;
                case "sys_woodpacking":
                    excelname = "木质包装";
                    break;
                case "sys_declarationcar":
                    excelname = "报告车信息";
                    break;
                case "sys_reportlibrary":
                    excelname = "申报库别";
                    break;

            }
            return excelname;
        }

        //创建excel
        public HSSFWorkbook createExcel(DataTable dt,string table)
        {
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet(get_excelname(table));
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);


            string[] publicrows = { "启用情况", "启用时间", "维护人", "维护时间", "停用人", "停用时间", "备注" };
            string[] rowStrings = new string[]{};
            switch (table)
            {
                case "insp_portin":
                    rowStrings = new string[]{ "口岸代码", "口岸名称", "英文名称" };                   
                    break;
                case "insp_portout":
                    rowStrings = new string[] { "口岸代码", "口岸名称", "英文名称", "所属国家" };
                    break;
                case "base_currency":
                    rowStrings = new string[] { "币制代码", "币制名称", "币制缩写"};
                    break;
                case "base_inspcountry":
                    rowStrings = new string[] { "国家代码", "中文名", "英文名称","空运缩写","海运缩写" };
                    break;
                case "base_productunit":
                    rowStrings = new string[] { "单位代码", "单位名称", "英文名称" };
                    break;
                case "base_insplicense":
                    rowStrings = new string[] { "许可证代码", "进口许可证名称", "出口许可证名称" };
                    break;
                case "base_inspinvoice":
                    rowStrings = new string[] { "随附单据代码", "进口随附单据名称", "出口随附单据名称" };
                    break;
                case "base_country":
                    rowStrings = new string[] { "国家代码", "二字码", "中文名称","英文名称" };
                    break;
                case "base_decltradeway":
                    rowStrings = new string[] { "贸易方式代码", "贸易方式简称", "贸易方式全称"};
                    break;
                case "base_exemptingnature":
                    rowStrings = new string[] { "征免性质代码", "征免性质简称", "征免性质全称" };
                    break;
                case "base_exchangeway":
                    rowStrings = new string[] { "结汇方式代码", "结汇方式简称", "结汇方式全称" };
                    break;
                case "base_harbour":
                    rowStrings = new string[] { "港口代码", "港口名称", "所属国家","英文名称" };
                    break;
                case "base_booksdata":
                    rowStrings = new string[]{"贸易方式代码","贸易方式名称","进口/出口","成品/料件"};
                    break;
                case "sys_repway":
                    rowStrings = new string[]{"申报方式代码","申报方式名称","业务类型"};
                    break;
                case "base_containersize":
                    rowStrings = new[] { "集装箱尺寸代码","集装箱尺寸","集装箱申报尺寸"};
                    break;
                case "base_containertype":
                    rowStrings = new string[]{"集装箱类型代码","集装箱类型","集装箱编码"};
                    break;
                case "sys_woodpacking":
                    rowStrings = new string[]{"木质包装代码","木质包装名称","木质包装HS编码","检验检疫类别","海关监管条件"};
                    break;
                case "sys_declarationcar":
                    rowStrings = new string[]{"车牌号","白卡号","车型","车队"};
                    break;
                case "sys_reportlibrary":
                    rowStrings = new string[]{"申报库别代码","申报库别名称","报关单名称","进出口类型"};
                    break;

            }
            string[] totalStrings = new string[publicrows.Length + rowStrings.Length];
            totalStrings = rowStrings.Concat(publicrows).ToArray();

            for (int i = 0; i < totalStrings.Length; i++)
            {
                row1.CreateCell(i).SetCellValue(totalStrings[i]);
            }

            string[] dtRows = new string[]{};
            string[] publicDt = { "ENABLED", "STARTDATE", "CREATEMANNAME",
                "CREATEDATE", "STOPMANNAME", "ENDDATE", "REMARK"};
            switch (table)
            {
                case "insp_portin":  case "base_productunit":
                    dtRows = new string[] { "CODE", "NAME", "ENGLISHNAME" };
                    break;
                case "insp_portout":
                    dtRows = new string[] { "CODE", "NAME", "ENGLISHNAME", "COUNTRYNAME" };
                    break;
                case "base_currency":
                    dtRows = new string[] { "CODE", "NAME", "ABBREVIATION" };
                    break;
                case "base_inspcountry":
                    dtRows = new string[] { "CODE", "NAME", "ENGLISHNAME", "AIRABBREV", "OCEANABBREV" };
                    break;
                case "base_inspinvoice":  case "base_insplicense":
                    dtRows = new string[] { "CODE", "INNAME", "OUTNAME" };
                    break;
                case "base_country":
                    dtRows = new string[] { "CODE", "EZM", "NAME","ENGLISHNAME" };
                    break;
                case "base_decltradeway": case "base_exemptingnature": case "base_exchangeway":
                    dtRows = new string[] { "CODE", "NAME", "FULLNAME" };
                    break;
                case "base_harbour":
                    dtRows = new string[] { "CODE", "NAME", "COUNTRYNAME", "ENGLISHNAME" };
                    break;
                case "base_booksdata":
                    dtRows = new string[] { "TRADE", "TRADENAME", "ISINPORTNAME", "ISPRODUCTNAME" };
                    break;
                case "sys_repway":
                    dtRows = new string[]{"CODE","NAME","BUSITYPE"};
                    break;
                case "base_containersize":
                    dtRows = new string[] { "CODE", "NAME", "DECLSIZE" };
                    break;
                case "base_containertype":
                    dtRows = new string[] { "CODE", "NAME", "CONTAINERCODE" };
                    break;
                case "sys_woodpacking":
                    dtRows = new string[]{"CODE","NAME","HSCODE","INSPECTION","DECLARATION"};
                    break;
                case "sys_declarationcar":
                    dtRows = new string[] { "CODE", "NAME", "MODELS", "MOTORCADE" };
                    break;
                case "sys_reportlibrary":
                    dtRows = new string[] { "CODE", "NAME", "DECLNAME", "INTERNALTYPE" };
                    break;
            }

            string[] totalStrings2 = dtRows.Concat(publicDt).ToArray();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                for (int j = 0; j < totalStrings2.Length; j++)
                {
                    rowtemp.CreateCell(j).SetCellValue(dt.Rows[i][totalStrings2[j]].ToString());
                }
                
            }
            return book;
        }


        //导入插入的值
        public string importValue(JObject json_formdata, string table, List<string> stringList)
        {
            string getValue = String.Empty;           
            //启用日期
            string startdate = json_formdata.Value<string>("STARTDATE");
            //停用日期
            string enddate = json_formdata.Value<string>("ENDDATE");
            switch (table)
            {
                case "insp_portin":   case "base_productunit":
                    string code = stringList[0]; string name = stringList[1];
                    string englishname = stringList[2];string enabled = stringList[3] == "是" ? "1" : "0";
                    string remark = stringList[4];
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"ENGLISHNAME\":\"" + englishname + "\"}";
                    break;
                case "insp_portout":
                    code = stringList[0];  name = stringList[1];
                    englishname = stringList[2]; string country = stringList[3];
                    enabled = stringList[4] == "是" ? "1" : "0";
                    remark = stringList[5];
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"ENGLISHNAME\":\"" + englishname + "\",\"COUNTRY\":\"" + country + "\"}";
                    break;
                case "base_currency":
                    code = stringList[0]; name = stringList[1];
                    string abbreviation  = stringList[2];enabled = stringList[3] == "是" ? "1" : "0";
                    remark = stringList[4];
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"ABBREVIATION\":\"" + abbreviation + "\"}";
                    break;
                case "base_inspcountry":
                    code = stringList[0]; name = stringList[1];
                    englishname = stringList[2]; string airabbrev  = stringList[3];
                    string oceanabbrev  = stringList[4];
                    enabled = stringList[5] == "是" ? "1" : "0";
                    remark = stringList[6];
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"ENGLISHNAME\":\"" + englishname + "\",\"AIRABBREV\":\"" + airabbrev + "\",\"OCEANABBREV\":\"" + oceanabbrev + "\"}";
                    break;
                case "base_insplicense":  case "base_inspinvoice":
                    code = stringList[0];string inname = stringList[1];
                    string outname = stringList[2];
                    enabled = stringList[3] == "是" ? "1" : "0";
                    remark = stringList[4];
                    getValue = "{\"CODE\":\"" + code + "\",\"INNAME\":\"" + inname + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"OUTNAME\":\"" + outname + "\"}";
                    break;
                case "base_country":
                    code = stringList[0];string ezm = stringList[1];
                    name = stringList[2];englishname = stringList[3];
                    enabled = stringList[4] == "是" ? "1" : "0";
                    remark = stringList[5];
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"ENGLISHNAME\":\"" + englishname + "\",\"EZM\":\"" + ezm + "\"}";
                    break;
                case "base_decltradeway":  case "base_exemptingnature": case "base_exchangeway":
                    code = stringList[0];name = stringList[1];
                    string fullname = stringList[2];
                    enabled = stringList[3] == "是" ? "1" : "0";
                    remark = stringList[4];
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"FULLNAME\":\"" + fullname + "\"}";
                    break;
                case "base_harbour":
                    code = stringList[0];name = stringList[1];
                    country = stringList[2];englishname = stringList[3];
                    enabled = stringList[4] == "是" ? "1" : "0";
                    remark = stringList[5];
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"COUNTRY\":\"" + country + "\",\"ENGLISHNAME\":\"" + englishname + "\"}";
                    break;
                case "base_booksdata":
                    string trade = stringList[0];string isinportname = stringList[1];
                    string isproductname = stringList[2];
                    enabled = stringList[3] == "是" ? "1" : "0";
                    remark = stringList[4];
                    getValue = "{\"TRADE\":\"" + trade + "\",\"ISINPORTNAME\":\"" + isinportname + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"ISPRODUCTNAME\":\"" + isproductname + "\"}";
                    break;
                case "sys_repway":
                    code = stringList[0];name = stringList[1];
                    string busitype = stringList[2];
                    enabled = stringList[3] == "是" ? "1" : "0";
                    remark = stringList[4];
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"BUSITYPE\":\"" + busitype + "\"}";
                    break;
                case "base_containersize":
                    code = stringList[0];name = stringList[1];
                    string declsize = stringList[2];
                    enabled = stringList[3] == "是" ? "1" : "0";
                    remark = stringList[4];
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"DECLSIZE\":\"" + declsize + "\"}";
                    break;
                case "base_containertype":
                    code = stringList[0];name = stringList[1];
                    string containercode = stringList[2];
                    enabled = stringList[3] == "是" ? "1" : "0";
                    remark = stringList[4];
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"CONTAINERCODE\":\"" + containercode + "\"}";
                    break;
                case "sys_woodpacking":
                    code = stringList[0];name = stringList[1];
                    string hsname = stringList[2];
                    enabled = stringList[3] == "是" ? "1" : "0";
                    remark = stringList[4];
                    string sql = @"select hscode, hsname, inspectionregulatory, customregulatory
                                    from base_insphs  where enabled = 1 and hsname = '"+hsname+"'";
                    DataTable dt = DBMgrBase.GetDataTable(sql);
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"HSCODE\":\"" + dt.Rows[0]["HSCODE"] + "\"}";
                    break;
                case "sys_declarationcar":
                    code = stringList[0];name = stringList[1];
                    string models = stringList[2]; string motorcardename = stringList[3];
                    enabled = stringList[3] == "是" ? "1" : "0";
                    remark = stringList[4];
                    sql = @"select * from base_motorcade where code = '" + motorcardename + "'";
                    dt = DBMgrBase.GetDataTable(sql);
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"MOTORCADE\":\"" + dt.Rows[0]["CODE"] + "\",\"MODELS\":\"" + models + "\"}";
                    break;
                case "sys_reportlibrary":
                    code = stringList[0];name = stringList[1];
                    string declname = stringList[2]; string internaltype = stringList[3];
                    enabled = stringList[3] == "是" ? "1" : "0";
                    remark = stringList[4];
                    getValue = "{\"CODE\":\"" + code + "\",\"NAME\":\"" + name + "\",\"ENABLED\":\"" + enabled + "\",\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"DECLNAME\":\"" + declname + "\",\"INTERNALTYPE\":\"" + internaltype + "\"}";
                    break;
            }
            return getValue;
        }
    }
}