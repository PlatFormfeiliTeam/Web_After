using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Web_After.Common;

namespace Web_After.BasicManager.BasicManager
{
    public class Switch_helper_Base_codename
    {
        public string get_base_sql(string type,string table,JObject json,string contents)
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
                //界面数据
                case "load":
                    switch (table)
                    {
                        case "sys_reguareatype":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_reguareatype t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_tradeway":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_tradeway t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_insppackage":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_insppackage t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_inspectionagency":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_inspectionagency t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_inspconveyance":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_inspconveyance t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "sys_InspType":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_InspType t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_withinregion":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_withinregion t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_needdocument":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_needdocument t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_wastegoods":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_wastegoods t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_inspuse":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_inspuse t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_inspcompanynature":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_inspcompanynature t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_reportregion":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_reportregion t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_inspectflag":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_inspectflag t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_transport":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_transport t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;

                        case "base_shipping_destination":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_shipping_destination t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_transaction":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_transaction t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_declcurrency":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_declcurrency t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_declfee":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_declfee t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_packing":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_packing t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_declproductunit":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_declproductunit t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_exemptingway":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_exemptingway t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "sys_companynature":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_companynature t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_invoice":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_invoice t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";                      
                            break;
                        case "base_decluse":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_decluse t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_customdistrict":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_customdistrict t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "sys_status":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_status t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "sys_busitype":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_busitype t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_motorcade":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_motorcade t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "sys_declway":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_declway t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "sys_inspLibrary":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from sys_inspLibrary t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "sys_NoticeType":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_NoticeType t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "sys_goodstype":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_goodstype t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_consigneetype":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_consigneetype t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_listtype":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_listtype t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_assistkind":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_assistkind t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_orderstatus":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_orderstatus t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_declstatus":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_declstatus t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;

                    }
                 break;
                //新增
                case "insert":
                    switch (table)
                    {   
                        case "sys_reguareatype":
                            sql = @"insert into sys_reguareatype(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_inspectionagency_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql,json.Value<string>("CODE"),json.Value<string>("NAME"),startdate,enddate,createman,stopman,json.Value<string>("ENABLED"),json.Value<string>("REMARK"));
                            break;
                        case "base_tradeway":
                            sql = @"insert into base_tradeway(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_tradeway_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_insppackage":
                            sql = @"insert into base_insppackage(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_insppackage_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_inspectionagency":
                            sql = @"insert into base_inspectionagency(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_inspectionagency_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_inspconveyance":
                            sql = @"insert into base_inspconveyance(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_inspconveyance_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "sys_InspType":
                            sql = @"insert into sys_InspType(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(sys_InspType_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_withinregion":
                            sql = @"insert into base_withinregion(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_withinregion_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_needdocument":
                            sql = @"insert into base_needdocument(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_needdocument_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_wastegoods":
                            sql = @"insert into base_wastegoods(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_wastegoods_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_inspuse":
                            sql = @"insert into base_inspuse(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_inspuse_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_inspcompanynature":
                            sql = @"insert into base_inspcompanynature(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_inspcompanynature_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_reportregion":
                            sql = @"insert into base_reportregion(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_reportregion_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_inspectflag":
                            sql = @"insert into base_inspectflag(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_inspectflag_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_transport":
                            sql = @"insert into base_transport(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_transport_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_shipping_destination":
                            sql = @"insert into base_shipping_destination(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_shipping_destination_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_transaction":
                            sql = @"insert into base_transaction(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_transaction_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_declcurrency":
                            sql = @"insert into base_declcurrency(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_declcurrency_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_declfee":
                            sql = @"insert into base_declfee(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_declfee_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_packing":
                            sql = @"insert into base_packing(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_packing_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_declproductunit":
                            sql = @"insert into base_declproductunit(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_declproductunit_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_exemptingway":
                            sql = @"insert into base_exemptingway(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_exemptingway_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "sys_companynature":
                            sql = @"insert into sys_companynature(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(sys_companynature_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_invoice":
                            sql = @"insert into base_invoice(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_invoice_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_decluse":
                            sql = @"insert into base_decluse(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_decluse_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_customdistrict":
                            sql = @"insert into base_customdistrict(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_customdistrict_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "sys_status":
                            sql = @"insert into sys_status(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(sys_status_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "sys_busitype":
                            sql = @"insert into sys_busitype(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(sys_busitype_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_motorcade":
                            sql = @"insert into base_motorcade(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_motorcade_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "sys_declway":
                            sql = @"insert into sys_declway(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(sys_declway_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "sys_inspLibrary":
                            sql = @"insert into sys_inspLibrary(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(sys_inspLibrary_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "sys_NoticeType":
                            sql = @"insert into sys_NoticeType(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(sys_NoticeType_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "sys_goodstype":
                            sql = @"insert into sys_goodstype(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(sys_goodstype_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_consigneetype":
                            sql = @"insert into base_consigneetype(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_consigneetype_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_listtype":
                            sql = @"insert into base_listtype(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_listtype_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_assistkind":
                            sql = @"insert into base_assistkind(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_assistkind_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_orderstatus":
                            sql = @"insert into base_orderstatus(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_orderstatus_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                        case "base_declstatus":
                            sql = @"insert into base_declstatus(id,code,name,createdate,startdate,enddate,createman,stopman,enabled,remark)
                                    values(base_declstatus_id.nextval,'{0}','{1}',sysdate,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    '{4}','{5}','{6}','{7}')";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, createman, stopman, json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
                            break;
                    }

                break;

                //检查code是否重复
                case "checkrepeat":
                    sql = @"select * from {0} where enabled=1 and code = '{1}'";
                    sql = String.Format(sql, table, json.Value<string>("CODE"));
                break;
                
                //更新
                case "update":
                    switch (table)
                    {
                        case "sys_reguareatype":
                            sql = @"update sys_reguareatype set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                        break;
                        
                        case "base_tradeway":
                            sql = @"update base_tradeway set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                        break;

                        case "base_insppackage":
                        sql = @"update base_insppackage set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                        break;
                        case "base_inspectionagency":
                        sql = @"update base_inspectionagency set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                        break;
                        case "base_inspconveyance":
                        sql = @"update base_inspconveyance set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                        break;
                        case "sys_InspType":
                        sql = @"update sys_InspType set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_withinregion":
                            sql = @"update base_withinregion set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_needdocument":
                            sql = @"update base_needdocument set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_wastegoods":
                            sql = @"update base_wastegoods set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_inspuse":
                            sql = @"update base_inspuse set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_inspcompanynature":
                            sql = @"update base_inspcompanynature set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_reportregion":
                            sql = @"update base_reportregion set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_inspectflag":
                            sql = @"update base_inspectflag set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_transport":
                            sql = @"update base_transport set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_shipping_destination":
                            sql = @"update base_shipping_destination set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_transaction":
                            sql = @"update base_transaction set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_declcurrency":
                            sql = @"update base_declcurrency set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_declfee":
                            sql = @"update base_declfee set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_packing":
                            sql = @"update base_packing set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_declproductunit":
                            sql = @"update base_declproductunit set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_exemptingway":
                            sql = @"update base_exemptingway set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "sys_companynature":
                            sql = @"update sys_companynature set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_invoice":
                            sql = @"update base_invoice set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_decluse":
                            sql = @"update base_decluse set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_customdistrict":
                            sql = @"update base_customdistrict set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "sys_status":
                            sql = @"update sys_status set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "sys_busitype":
                            sql = @"update sys_busitype set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_motorcade":
                            sql = @"update base_motorcade set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "sys_declway":
                            sql = @"update sys_declway set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "sys_inspLibrary":
                            sql = @"update sys_inspLibrary set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "sys_NoticeType":
                            sql = @"update sys_NoticeType set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "sys_goodstype":
                            sql = @"update sys_goodstype set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_consigneetype":
                            sql = @"update base_consigneetype set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_listtype":
                            sql = @"update base_listtype set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_assistkind":
                            sql = @"update base_assistkind set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_orderstatus":
                            sql = @"update base_orderstatus set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                        case "base_declstatus":
                            sql = @"update base_declstatus set code = '{0}',name = '{1}',startdate = to_date('{2}','yyyy/mm/dd hh24:mi:ss'),enddate = to_date('{3}','yyyy/mm/dd hh24:mi:ss'),
                                    remark = '{4}' ,stopman = '{5}',enabled = '{6}' where id = '{7}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), startdate, enddate, json.Value<string>("REMARK"), stopman, json.Value<string>("ENABLED"), json.Value<string>("ID"));
                            break;
                    }

                break;

                //更新时检查是否有重复值
                case "updateCheckrepeat":
                    switch (table)
                    {
                        case "sys_reguareatype":
                            sql = @"select * from sys_reguareatype where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                        break;
                        case "base_tradeway":
                        sql = @"select * from base_tradeway where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                        break;
                        case "base_insppackage":
                        sql = @"select * from base_insppackage where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                        break;
                        case "base_inspectionagency":
                        sql = @"select * from base_inspectionagency where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                        break;
                        case "base_inspconveyance":
                        sql = @"select * from base_inspconveyance where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                        break;
                        case "sys_InspType":
                        sql = @"select * from sys_InspType where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                        break;
                        case "base_withinregion":
                            sql = @"select * from base_withinregion where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                        break;
                        case "base_needdocument":
                            sql = @"select * from base_needdocument where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                        break;
                        case "base_wastegoods":
                        sql = @"select * from base_wastegoods where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_inspuse":
                            sql = @"select * from base_inspuse where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_inspcompanynature":
                            sql = @"select * from base_inspcompanynature where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_reportregion":
                            sql = @"select * from base_reportregion where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_inspectflag":
                            sql = @"select * from base_inspectflag where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_transport":
                            sql = @"select * from base_transport where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_shipping_destination":
                            sql = @"select * from base_shipping_destination where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_transaction":
                            sql = @"select * from base_transaction where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_declcurrency":
                            sql = @"select * from base_declcurrency where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_declfee":
                            sql = @"select * from base_declfee where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_packing":
                            sql = @"select * from base_packing where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_declproductunit":
                            sql = @"select * from base_declproductunit where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_exemptingway":
                            sql = @"select * from base_exemptingway where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "sys_companynature":
                            sql = @"select * from sys_companynature where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_invoice":
                            sql = @"select * from base_invoice where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_decluse":
                            sql = @"select * from base_decluse where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_customdistrict":
                            sql = @"select * from base_customdistrict where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "sys_status":
                            sql = @"select * from sys_status where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "sys_busitype":
                            sql = @"select * from sys_busitype where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_motorcade":
                            sql = @"select * from base_motorcade where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "sys_declway":
                            sql = @"select * from sys_declway where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "sys_inspLibrary":
                            sql = @"select * from sys_inspLibrary where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "sys_NoticeType":
                            sql = @"select * from sys_NoticeType where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "sys_goodstype":
                            sql = @"select * from sys_goodstype where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_consigneetype":
                            sql = @"select * from base_consigneetype where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_listtype":
                            sql = @"select * from base_listtype where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_assistkind":
                            sql = @"select * from base_assistkind where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_orderstatus":
                            sql = @"select * from base_orderstatus where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                        case "base_declstatus":
                            sql = @"select * from base_declstatus where code = '{0}' and id not in '{1}'";
                            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("ID"));
                            break;
                    }
                break;

                //导出
                case "export":
                    switch (table)
                    {
                        case "sys_reguareatype":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_reguareatype t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_tradeway":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_tradeway t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_insppackage":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_insppackage t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_inspectionagency":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_inspectionagency t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_inspconveyance":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_inspconveyance t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "sys_InspType":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_InspType t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_withinregion":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_withinregion t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_needdocument":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_needdocument t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_wastegoods":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_wastegoods t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_inspuse":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_inspuse t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_inspcompanynature":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_inspcompanynature t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_reportregion":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_reportregion t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_inspectflag":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_inspectflag t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_transport":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_transport t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_shipping_destination":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_shipping_destination t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_transaction":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_transaction t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_declcurrency":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_declcurrency t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_declfee":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_declfee t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_packing":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_packing t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_declproductunit":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_declproductunit t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_exemptingway":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_exemptingway t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "sys_companynature":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_companynature t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_invoice":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_invoice t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_decluse":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_decluse t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "base_customdistrict":
                            sql =
                                @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from base_customdistrict t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "sys_status":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_status t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "sys_busitype":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_busitype t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_motorcade":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_motorcade t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "sys_declway":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_declway t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "sys_inspLibrary":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from sys_inspLibrary t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id  where 1=1";
                            break;
                        case "sys_NoticeType":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_NoticeType t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "sys_goodstype":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from sys_goodstype t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_consigneetype":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_consigneetype t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_listtype":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_listtype t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_assistkind":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_assistkind t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_orderstatus":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_orderstatus t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;
                        case "base_declstatus":
                            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_declstatus t1 left join sys_user t2 on t1.createman=t2.id 
                          left join sys_user t3 on t1.stopman=t3.id where 1 = 1";
                            break;

                    }
                break;


                case "BeforChange":
                    switch (table)
                    {
                        case "sys_reguareatype":
                            sql = @"select * from sys_reguareatype where id = '{0}'";
                            sql = String.Format(sql,json.Value<string>("ID"));
                        break;
                        case "base_tradeway":
                        sql = @"select * from base_tradeway where id = '{0}'";
                            sql = String.Format(sql,json.Value<string>("ID"));
                        break;
                        case "base_insppackage":
                        sql = @"select * from base_insppackage where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                        break;
                        case "base_inspectionagency":
                            sql = @"select * from base_inspectionagency where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                        break;
                        case "base_inspconveyance":
                        sql = @"select * from base_inspconveyance where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                        break;
                        case "sys_InspType":
                        sql = @"select * from sys_InspType where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_withinregion":
                            sql = @"select * from base_withinregion where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_needdocument":
                            sql = @"select * from base_needdocument where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_wastegoods":
                            sql = @"select * from base_wastegoods where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_inspuse":
                            sql = @"select * from base_inspuse where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_inspcompanynature":
                            sql = @"select * from base_inspcompanynature where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_reportregion":
                            sql = @"select * from base_reportregion where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_inspectflag":
                            sql = @"select * from base_inspectflag where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_transport":
                            sql = @"select * from base_transport where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_shipping_destination":
                            sql = @"select * from base_shipping_destination where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_transaction":
                            sql = @"select * from base_transaction where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_declcurrency":
                            sql = @"select * from base_declcurrency where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_declfee":
                            sql = @"select * from base_declfee where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_packing":
                            sql = @"select * from base_packing where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_declproductunit":
                            sql = @"select * from base_declproductunit where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_exemptingway":
                            sql = @"select * from base_exemptingway where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "sys_companynature":
                            sql = @"select * from sys_companynature where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_invoice":
                            sql = @"select * from base_invoice where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_decluse":
                            sql = @"select * from base_decluse where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_customdistrict":
                            sql = @"select * from base_customdistrict where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "sys_status":
                            sql = @"select * from sys_status where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "sys_busitype":
                            sql = @"select * from sys_busitype where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_motorcade":
                            sql = @"select * from base_motorcade where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "sys_declway":
                            sql = @"select * from sys_declway where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "sys_inspLibrary":
                            sql = @"select * from sys_inspLibrary where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "sys_NoticeType":
                            sql = @"select * from sys_NoticeType where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "sys_goodstype":
                            sql = @"select * from sys_goodstype where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_consigneetype":
                            sql = @"select * from base_consigneetype where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_listtype":
                            sql = @"select * from base_listtype where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_assistkind":
                            sql = @"select * from base_assistkind where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_orderstatus":
                            sql = @"select * from base_orderstatus where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;
                        case "base_declstatus":
                            sql = @"select * from base_declstatus where id = '{0}'";
                            sql = String.Format(sql, json.Value<string>("ID"));
                            break;

                    }
                break;
                //修改记录 //特殊监管区代码：Base_YearKindEnum.Busi_ReguAreaType

                case "change":
                    switch (table)
                    {
                        case "sys_reguareatype":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_ReguAreaType,createman,json.Value<string>("REASON"),contents);
                            
                        break;
                        case "base_tradeway":
                        sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                        sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_TradeWay, createman,json.Value<string>("REASON"), contents);
                        break;
                        case "base_insppackage":
                        sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                        sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Package, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_inspectionagency":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.InspectionAgency, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_inspconveyance":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Conveyance, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_InspType":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_InspType, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_withinregion":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Insp_WithinRegion, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_needdocument":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Insp_NeedDocument, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_wastegoods":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Insp_WasteGoods, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_inspuse":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Insp_Use, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_inspcompanynature":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Insp_CompanyNature, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_reportregion":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Insp_ReportRegion, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_inspectflag":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_InspectFlag, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_transport":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_Transport, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_shipping_destination":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_ShipDest, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_transaction":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_Transaction, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_declcurrency":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Rela_Currency, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_declfee":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_Fee, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_packing":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_Package, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_declproductunit":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_ProductUnit, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_exemptingway":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_ExemptingWay, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_companynature":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_CompanyNature, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_invoice":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_Invoice, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_decluse":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_Use, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_customdistrict":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_CustomArea, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_status":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_Status, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_busitype":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_BusiType, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_motorcade":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_Motorcade, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_declway":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_DeclWay, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_inspLibrary":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_InspLibrary, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_NoticeType":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_NotcieType, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "sys_goodstype":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_GoodsType, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_consigneetype":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_ConsigneeType, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_listtype":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_ListType, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_assistkind":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_AssistKind, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_orderstatus":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_OrderStatus, createman, json.Value<string>("REASON"), contents);
                            break;
                        case "base_declstatus":
                            sql = @"insert into base_alterrecord(id,tabid,tabkind,alterman,reason,contentes,alterdate) values(base_alterrecord_id.nextval,'{0}','{1}','{2}','{3}','{4}',
                                    sysdate)";
                            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_DeclStatus, createman, json.Value<string>("REASON"), contents);
                            break;



                    }
                break;


            }
            
            return sql;
        }

        //获取列名
        public List<string> getColum(string table)
        {
            List<string> ColumName = new List<string>();
            switch (table)
            {
                case "sys_reguareatype":
                    ColumName.Add("特殊监管区代码");
                    ColumName.Add("特殊监管区名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                break;

                case "base_tradeway":
                    ColumName.Add("贸易方式代码");
                    ColumName.Add("贸易方式名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                break;

                case "base_insppackage":
                    ColumName.Add("包装类别代码");
                    ColumName.Add("包装类别名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                break;
                case "base_inspectionagency":
                    ColumName.Add("检验检疫机构代码");
                    ColumName.Add("检验检疫机构名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                break;
                
                case "base_inspconveyance":
                    ColumName.Add("运输工具代码");
                    ColumName.Add("运输工具名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                break;

                case "sys_InspType":
                    ColumName.Add("报检类别代码");
                    ColumName.Add("报检类别名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;

                case "base_withinregion":
                    ColumName.Add("境内地区代码");
                    ColumName.Add("境内地区名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;


                case "base_needdocument":
                    ColumName.Add("所需单据代码");
                    ColumName.Add("所需单据名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;

                case "base_wastegoods":
                    ColumName.Add("废旧物品代码");
                    ColumName.Add("废旧物品名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;

                case "base_inspuse":
                    ColumName.Add("商检用途代码");
                    ColumName.Add("商检用途名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;

                case "base_inspcompanynature":
                    ColumName.Add("企业性质代码");
                    ColumName.Add("企业性质名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;

                case "base_reportregion":
                    ColumName.Add("申报区域代码");
                    ColumName.Add("申报区域名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;

                case "base_inspectflag":
                    ColumName.Add("法检标志代码");
                    ColumName.Add("法检标志名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_transport":
                    ColumName.Add("运输方式代码");
                    ColumName.Add("运输方式名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_shipping_destination":
                    ColumName.Add("境内区域代码");
                    ColumName.Add("境内区域名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_transaction":
                    ColumName.Add("成交方式代码");
                    ColumName.Add("成交方式名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_declcurrency":
                    ColumName.Add("币制代码");
                    ColumName.Add("币制代码名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_declfee":
                    ColumName.Add("费用形式代码");
                    ColumName.Add("费用形式名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_packing":
                    ColumName.Add("包装种类代码");
                    ColumName.Add("包装种类名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;

                case "base_declproductunit":
                    ColumName.Add("计量单位代码");
                    ColumName.Add("计量单位名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_exemptingway":
                    ColumName.Add("征免方式代码");
                    ColumName.Add("征免方式名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "sys_companynature":
                    ColumName.Add("企业性质代码");
                    ColumName.Add("企业性质名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_invoice":
                    ColumName.Add("随附单据代码");
                    ColumName.Add("随附单据名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_decluse":
                    ColumName.Add("用途代码");
                    ColumName.Add("用途代码名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_customdistrict":
                    ColumName.Add("关区代码");
                    ColumName.Add("关区代码名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "sys_status":
                    ColumName.Add("显示状态代码");
                    ColumName.Add("显示状态名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "sys_busitype":
                    ColumName.Add("业务类型代码");
                    ColumName.Add("业务类型名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_motorcade":
                    ColumName.Add("车队信息代码");
                    ColumName.Add("车队信息名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "sys_declway":
                    ColumName.Add("报关方式代码");
                    ColumName.Add("报关方式名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "sys_inspLibrary":
                    ColumName.Add("报检库别代码");
                    ColumName.Add("报检库别名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "sys_NoticeType":
                    ColumName.Add("通知类别代码");
                    ColumName.Add("通知类别名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "sys_goodstype":
                    ColumName.Add("货物类别代码");
                    ColumName.Add("货物类别名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_consigneetype":
                    ColumName.Add("收货人类型代码");
                    ColumName.Add("收货人类型名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_listtype":
                    ColumName.Add("清单类型代码");
                    ColumName.Add("清单类型名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_assistkind":
                    ColumName.Add("辅助选项代码");
                    ColumName.Add("辅助选项名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_orderstatus":
                    ColumName.Add("业务状态代码");
                    ColumName.Add("业务状态名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;
                case "base_declstatus":
                    ColumName.Add("报关状态代码");
                    ColumName.Add("报关状态名称");
                    ColumName.Add("CODE");
                    ColumName.Add("NAME");
                    break;




            }
            return ColumName;
        }

        //获取名称
        public string getExcelName(string table)
        {
            string ExcelName = String.Empty;
            switch (table)
            {
                case "sys_reguareatype":
                    ExcelName = "特殊监管";
                break;

                case "base_tradeway":
                    ExcelName = "贸易方式";
                break;

                case "base_insppackage":
                    ExcelName = "包装类别";
                break;
                case "base_inspectionagency":
                    ExcelName = "检验检疫机构";
                break;
                case "base_inspconveyance":
                    ExcelName = "运输工具";
                    break;
                case "sys_InspType":
                    ExcelName = "报检类别";
                    break;
                case "base_withinregion":
                    ExcelName = "境内地区";
                    break;
                case "base_needdocument":
                    ExcelName = "所需单据";
                    break;
                case "base_wastegoods":
                    ExcelName = "废旧物品";
                    break;
                case "base_inspuse":
                    ExcelName = "商检用途";
                    break;
                case "base_inspcompanynature":
                    ExcelName = "企业性质";
                    break;
                case "base_reportregion":
                    ExcelName = "申报区域";
                    break;
                case "base_inspectflag":
                    ExcelName = "法检标志";
                    break;
                case "base_transport":
                    ExcelName = "运输方式";
                    break;
                case "base_shipping_destination":
                    ExcelName = "境内区域";
                    break;
                case "base_transaction":
                    ExcelName = "成交方式";
                    break;
                case "base_declcurrency":
                    ExcelName = "币制代码";
                    break;
                case "base_declfee":
                    ExcelName = "费用形式";
                    break;
                case "base_packing":
                    ExcelName = "包装种类";
                    break;
                case "base_declproductunit":
                    ExcelName = "计量单位";
                    break;
                case "base_exemptingway":
                    ExcelName = "征免方式";
                    break;
                case "sys_companynature":
                    ExcelName = "企业性质";
                    break;
                case "base_invoice":
                    ExcelName = "随附单据";
                    break;
                case "base_decluse":
                    ExcelName = "用途代码";
                    break;
                case "base_customdistrict":
                    ExcelName = "关区代码";
                    break;
                case "sys_status":
                    ExcelName = "显示状态";
                    break;
                case "sys_busitype":
                    ExcelName = "业务类型";
                    break;
                case "base_motorcade":
                    ExcelName = "车队信息";
                    break;
                case "sys_declway":
                    ExcelName = "报关方式";
                    break;
                case "sys_inspLibrary":
                    ExcelName = "报检库别";
                    break;
                case "sys_NoticeType":
                    ExcelName = "通知类别";
                    break;
                case "sys_goodstype":
                    ExcelName = "货物类别";
                    break;
                case "base_consigneetype":
                    ExcelName = "收货人类型";
                    break;
                case "base_listtype":
                    ExcelName = "清单类型";
                    break;
                case "base_assistkind":
                    ExcelName = "辅助选项";
                    break;
                case "base_orderstatus":
                    ExcelName = "业务状态";
                    break;
                case "base_declstatus":
                    ExcelName = "报关状态";
                    break;
               

            }
            return ExcelName;
        }

    }
}