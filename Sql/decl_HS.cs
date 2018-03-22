using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Web_After.BasicManager;
using Web_After.BasicManager.BasicManager;
using Web_After.Common;

namespace Web_After.Sql
{
    public class decl_HS
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
                        where t1.kind = {0} {1}
                            ";
            sql = string.Format(sql, (int)Base_YearKindEnum.Decl_HS, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.name", "desc", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }
        //查询base_year是否有重复HS代码库（新增）
        public DataSet check_base_year(JObject json)
        {
            string sql = @"select * from base_year where name = '{0}' and kind = '{1}'";
            sql = string.Format(sql, json.Value<string>("NAME"), (int)Base_YearKindEnum.Decl_HS);
            return DBMgrBase.GetDataSet(sql);
        }

        //插入代码库
        public int insertTable(JObject json)
        {

            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"insert into base_year(id,name,startdate,enddate,createdate,enabled,remark,createman,kind,customarea,stopman) values(base_ciqyear_id.nextval,'{0}',to_date('{1}','yyyy/mm/dd hh24:mi:ss')
                        ,to_date('{2}','yyyy/mm/dd hh24:mi:ss'),sysdate,'{3}','{4}','{5}'，'{6}','{7}','{8}')";
            sql = String.Format(sql, json.Value<string>("NAME"), startdate, enddate, json.Value<string>("ENABLED"), json.Value<string>("REMARK"), createman, (int)Base_YearKindEnum.Decl_HS, json.Value<string>("CUSTOMAREA"), stopman);
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public int update_base_year(JObject json)
        {

            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"update base_year set name ='{0}',startdate=to_date('{1}','yyyy/mm/dd '),enddate=to_date('{2}','yyyy/mm/dd '),customarea='{3}',
                           remark='{4}',enabled='{6}',stopman='" + stopman + "'  where id='{5}'";
            sql = String.Format(sql, json.Value<string>("NAME"), startdate, enddate, json.Value<string>("CUSTOMAREA"), json.Value<string>("REMARK"), json.Value<string>("ID"), json.Value<string>("ENABLED"));
            return DBMgrBase.ExecuteNonQuery(sql);
        }
        //查询base_year是否有重复HS代码库根据id和name（修改）
        public DataSet check_base_year_by_idandname(JObject json)
        {
            string sql = @"select * from base_year where name = '{0}' and id not in ('{1}') and kind = '{2}'";
            sql = string.Format(sql, json.Value<string>("NAME"), json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_HS);
            return DBMgrBase.GetDataSet(sql);
        }

        //根据id来找base_year的数据
        public DataTable getBeforeChangData(JObject json)
        {
            string sql = @"select * from base_year where id = '{0}'";
            sql = string.Format(sql, json.Value<string>("ID"));
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
            string sql = @"select t8.name as yearname,
                               t7.name as createmanname,
                               t6.name as typename,
                               t5.name as chaptername,
                               t4.name as classname,
                               t3.name as secondunitname,
                               t2.name as legalunitname,
                               t9.name as stopmanname,
                               t1.*
                          from BASE_COMMODITYHS t1
                          LEFT JOIN BASE_DECLPRODUCTUNIT t2
                            on T1.LEGALUNIT = T2.code
                          LEFT JOIN BASE_DECLPRODUCTUNIT t3
                            on t1.secondunit = t3.code
                          LEFT JOIN BASE_DECLHSCLASS t4
                            on t1.classcode = t4.code
                          LEFT JOIN BASE_DECLHSCHAPTER t5
                            on t4.chaptercode = t5.code
                          LEFT JOIN BASE_DECLHSTYPE t6
                            on t5.typecode = t6.code
                          LEFT JOIN sys_user t7
                            on t1.createman = t7.id
                          LEFT JOIN BASE_YEAR t8
                            on t1.yearid = t8.id
                          left join sys_user t9
                           on   t1.stopman = t9.id
                         where 1 = 1
                           and t1.yearid = '{0}' {1}
                           ";
            sql = string.Format(sql, id, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.hscode", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }


        //检查HS编码是否重复
        public DataTable check_repeat_base_insphs(JObject json, string yearid)
        {
            string sql = @"select * from base_commodityhs where hscode = '{0}' and yearid = '{1}' and extracode = '{2}' ";
            sql = String.Format(sql, json.Value<string>("HSCODE"), yearid, json.Value<string>("EXTRACODE"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        public int insert_base_insphs(JObject json, string yearid)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"insert into base_commodityhs(id,name,hscode,extracode,legalunit,secondunit,customregulatory,inspectionregulatory,classcode,agreetaxfile,
            topprice,lowprice,generalrate,favorablerate,vatrate,enabled,createman,stopman,createdate,startdate,enddate,yearid,remark,exportrebatrate,temprate,consumerate,exportrate,specialmark，elements 
            ) values(base_commodity_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}',sysdate,
            to_date('{17}','yyyy/mm/dd hh24:mi:ss'),to_date('{18}','yyyy/mm/dd hh24:mi:ss'),'{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}')";
            sql = String.Format(sql,
                json.Value<string>("NAME"), json.Value<string>("HSCODE"), json.Value<string>("EXTRACODE"), json.Value<string>("LEGALUNIT"), json.Value<string>("SECONDUNIT"),
                json.Value<string>("CUSTOMREGULATORY"), json.Value<string>("INSPECTIONREGULATORY"), "", "", json.Value<string>("TOPPRICE"),
                json.Value<string>("LOWPRICE"), json.Value<string>("GENERALRATE"), json.Value<string>("FAVORABLERATE"), json.Value<string>("VATRATE"), json.Value<string>("ENABLED"),createman,
                stopman, startdate, enddate, yearid, json.Value<string>("REMARK"), json.Value<string>("EXPORTREBATRATE"), json.Value<string>("TEMPRATE"), json.Value<string>("CONSUMERATE"), json.Value<string>("EXPORTRATE"),
                json.Value<string>("SPECIALMARK"), json.Value<string>("ELEMENTS"));

            string unit1 = "", unit2 = "";
            if (judgeNumOrWeight(json.Value<string>("LEGALUNIT")))//是重量
            {
                unit1 = json.Value<string>("LEGALUNIT");
                unit2 = json.Value<string>("SECONDUNIT");
            }
            else//是数量
            {
                unit2 = json.Value<string>("LEGALUNIT");
                unit1 = json.Value<string>("SECONDUNIT");
            }


            string sql_sub = @"insert into base_insphs(id,hscode,hsname,weight,num,customregulatory,inspectionregulatory,enabled,createman,stopman,createdate,startdate,
enddate,yearid,remark,legalunit) values(base_insphs_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',sysdate,to_date('{9}','yyyy/mm/dd hh24:mi:ss'),
to_date('{10}','yyyy/mm/dd hh24:mi:ss'),'{11}','{12}','{13}')";
            sql_sub = String.Format(sql_sub, json.Value<string>("HSCODE"), json.Value<string>("NAME"), unit1, unit2, json.Value<string>("CUSTOMREGULATORY"), json.Value<string>("INSPECTIONREGULATORY"), json.Value<string>("ENABLED"),createman,stopman,
                startdate, enddate, yearid, json.Value<string>("REMARK"), json.Value<string>("LEGALUNIT"));
            List<string> sqls = new List<string>();
            sqls.Add(sql);
            sqls.Add(sql_sub);
            int i = DBMgrBase.ExecuteNonQuery(sqls);
            return i;
        }

        //更新时验证HS编码重复
        public DataTable check_repeat_base_insphs_update(JObject json, string yearid)
        {
            string sql = @"select * from base_commodityhs where hscode = '{0}' and id not in ('{1}') and yearid = '{2}' and extracode = '{3}'";
            sql = String.Format(sql, json.Value<string>("HSCODE"), json.Value<string>("ID"), yearid, json.Value<string>("EXTRACODE"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //更新HS编码
        public int update_base_insphs(JObject json, string yearid)
        {
            bcm.getCommonInformation(out stopman, out createman, out startdate, out enddate, json);
            string sql = @"update base_commodityhs set name='{0}',hscode='{1}',extracode='{2}',legalunit='{3}',secondunit='{4}',customregulatory='{5}',inspectionregulatory='{6}',
classcode='{7}',agreetaxfile='{8}',topprice='{9}',lowprice='{10}',generalrate='{11}',favorablerate='{12}',vatrate='{13}',startdate=to_date('{14}','yyyy/mm/dd hh24:mi:ss'),
enddate=to_date('{15}','yyyy/mm/dd hh24:mi:ss'),exportrebatrate='{16}',temprate='{17}',consumerate='{18}',exportrate='{19}',specialmark='{20}',elements='{21}',enabled = '{22}' where id='{23}'";
            sql = String.Format(sql,
                json.Value<string>("NAME"), json.Value<string>("HSCODE"), json.Value<string>("EXTRACODE"), json.Value<string>("LEGALUNIT"), json.Value<string>("SECONDUNIT"),
                json.Value<string>("CUSTOMREGULATORY"), json.Value<string>("INSPECTIONREGULATORY"), "", "", json.Value<string>("TOPPRICE"), json.Value<string>("LOWPRICE"),json.Value<string>("GENERALRATE"),json.Value<string>("FAVORABLERATE"), json.Value<string>("VATRATE"),
                startdate, enddate, json.Value<string>("EXPORTREBATRATE"), json.Value<string>("TEMPRATE"), json.Value<string>("CONSUMERATE"), json.Value<string>("EXPORTRATE"), json.Value<string>("SPECIALMARK"), json.Value<string>("ELEMENTS"), json.Value<string>("ENABLED"), json.Value<string>("ID")
            );

            string unit1 = "", unit2 = "";
            if (judgeNumOrWeight(json.Value<string>("LEGALUNIT")))//是重量
            {
                unit1 = json.Value<string>("LEGALUNIT");
                unit2 = json.Value<string>("SECONDUNIT");
            }
            else//是数量
            {
                unit1 = json.Value<string>("LEGALUNIT");
                unit2 = json.Value<string>("SECONDUNIT");
            }
            
            string sql_sub = @"update base_insphs set hsname='{0}',weight='{1}',num='{2}',customregulatory='{3}',inspectionregulatory='{4}',legalunit='{5}' ,remark='{6}' 
where hscode='{7}' and  extracode='{8}' and enabled=1 and yearid = '{9}'";
            sql_sub = String.Format(sql_sub, json.Value<string>("NAME"), unit1, unit2, json.Value<string>("CUSTOMREGULATORY"), json.Value<string>("INSPECTIONREGULATORY"), json.Value<string>("LEGALUNIT"), json.Value<string>("REMARK"),
                json.Value<string>("HSCODE") , json.Value<string>("EXTRACODE"),yearid);
            List<string> sqls = new List<string>();
            sqls.Add(sql);
            sqls.Add(sql_sub);
            int i = DBMgrBase.ExecuteNonQuery(sqls);
            return i;
        }

        //根据ID来找出修改之前的数据
        public DataTable Before_Change(JObject json)
        {
            string sql = @"select * from base_commodityhs where id = '{0}'";
            sql = String.Format(sql, json.Value<string>("ID"));
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

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
            sql = String.Format(sql, json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_HS, json_user.GetValue("ID"),
                json.Value<string>("REASON"), content
            );
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }


        //导出数据
        public DataTable export_table(string yearid, string strwhere)
        {
            string sql = @"select t8.name as yearname,
                           t7.name as createmanname,
                           t6.name as typename,
                           t5.name as chaptername,
                           t4.name as classname,
                           t3.name as secondunitname,
                           t2.name as legalunitname,
                           t9.name as stopmanname,
                           t1.*
                      from BASE_COMMODITYHS t1
                      LEFT JOIN BASE_DECLPRODUCTUNIT t2
                        on T1.LEGALUNIT = T2.code
                      LEFT JOIN BASE_DECLPRODUCTUNIT t3
                        on t1.secondunit = t3.code
                      LEFT JOIN BASE_DECLHSCLASS t4
                        on t1.classcode = t4.code
                      LEFT JOIN BASE_DECLHSCHAPTER t5
                        on t4.chaptercode = t5.code
                      LEFT JOIN BASE_DECLHSTYPE t6
                        on t5.typecode = t6.code
                      LEFT JOIN sys_user t7
                        on t1.createman = t7.id
                      LEFT JOIN BASE_YEAR t8
                        on t1.yearid = t8.id
                      left join sys_user t9
                       on   t1.stopman = t9.id
                     where 1 = 1
                           and t1.yearid = '{0}' {1}
                           ";
            sql = string.Format(sql, yearid, strwhere);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        private bool judgeNumOrWeight(string unit)
        {
            bool flag = false;
            switch (unit)
            {
                case "035":
                case "036":
                case "070":
                case "071":
                case "072":
                case "073":
                case "074":
                case "075":
                case "076":
                case "077":
                case "078":
                case "079":
                case "080":
                case "081":
                case "083":
                case "084":
                    flag = true;//重量
                    break;
            }
            return flag;
        }

    }
}