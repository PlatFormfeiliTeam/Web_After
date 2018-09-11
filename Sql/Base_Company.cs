using System;
using System.Data;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Web_After.BasicManager;
using Web_After.BasicManager.DeclInfor;
using Web_After.Common;

namespace Web_After.Sql
{
    public class Base_Company
    {
        //企业所有信息的加载
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,
                           t2.name as createmanname,
                           t3.name as stopmanname,
                           t4.name as declnaturename,
                           t5.name as inspnaturename,
                           t6.name as receivertypename
                           from base_company t1
                           left join sys_user t2
                           on t1.createman = t2.id
                           left join sys_user t3
                           on t1.stopman = t3.id
                           left join SYS_COMPANYNATURE t4
                           on t1.declnature = t4.code
                           left join BASE_INSPCOMPANYNATURE t5
                           on t1.inspnature = t5.code
                           left join Base_consigneetype t6
                           on t1.receivertype = t6.code
                           where 1 = 1 {0}
                           ";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.code", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }
        //判断内部编码是否重复
        public DataTable check_incode_repeat(string incode,string strWhere)
        {
            string sql = "select * from base_company where enabled = '1' and lower(incode) = lower('{0}') {1}";
            sql = string.Format(sql,incode,strWhere);
            DataTable check_table = DBMgrBase.GetDataTable(sql);
            return check_table;

        }
        //判断海关代码是否重复
        public DataTable check_code_repeat(string code,string strwhere)
        {
            string sql = "select * from base_company where enabled = '1' and lower(code) = lower('{0}') {1}";
            sql = string.Format(sql, code,strwhere);
            DataTable check_table = DBMgrBase.GetDataTable(sql);
            return check_table;
        }
        //判断社会信用代码是否重复
        public DataTable check_socialcreditno_repeat(string socialcreditno,string strWhere)
        {
            string sql = "select * from base_company where enabled = '1' and lower(SOCIALCREDITNO)  = lower('{0}') {1}";
            sql = string.Format(sql, socialcreditno,strWhere);
            DataTable check_table = DBMgrBase.GetDataTable(sql);
            return check_table;
        }
        //向base_company表中插入数据
        public int insert_base_company(JObject json,string stopman)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            string a = json.Value<string>("DECLNATURENAME");
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql = @"insert into base_company(id
                                    ,code,name,remark,enabled,createman,stopman,createdate
                                    ,startdate,enddate,englishname,declnature,incode
                                    ,inspcode,inspnature,goodslocal,receivertype,SOCIALCREDITNO
                            ) values(base_company_id.nextval
                                    ,'{0}','{1}','{2}','{3}','{4}','{5}',sysdate
                                    ,to_date('{6}','yyyy/mm/dd hh24:mi:ss'),to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}','{9}','{10}'
                                    ,'{11}','{12}','{13}','{14}','{15}'
                            )";
            sql = String.Format(sql
                , json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"), json.Value<string>("ENABLED") , json_user.GetValue("ID"), stopman
                , json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE")
                    ,json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE")
                    , json.Value<string>("ENGLISHNAME"),json.Value<string>("DECLNATURENAME"), json.Value<string>("INCODE")
                , json.Value<string>("INSPCODE"), json.Value<string>("INSPNATURENAME"), json.Value<string>("GOODSLOCAL"), json.Value<string>("RECEIVERTYPE"), json.Value<string>("SOCIALCREDITNO")
                );
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        //点击修改
        public int update_base_company(JObject json,string stopman)
        {
            int i = 0;
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            string a = json.Value<string>("DECLNATURENAME");
            JObject json_user = Extension.Get_UserInfo(userName);
            //修改经营单位代码
            string sql = @"update base_company set 
                            code='{0}', name='{1}', remark='{2}',
                            startdate=to_date('{3}','yyyy/mm/dd'),
                            enddate=to_date('{4}','yyyy/mm/dd'),
                            englishname='{5}',declnature='{6}',inspcode='{7}',
                            inspnature='{8}',goodslocal='{9}',receivertype='{10}',
                            SOCIALCREDITNO='{11}',enabled='{12}',stopman='{13}',createdate = sysdate where id='{14}'";
            sql = string.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("REMARK"),
                json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
                json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
                json.Value<string>("ENGLISHNAME"), json.Value<string>("DECLNATURENAME"), json.Value<string>("INSPCODE"),
                json.Value<string>("INSPNATURENAME"), json.Value<string>("GOODSLOCAL"), json.Value<string>("RECEIVERTYPE"),
                json.Value<string>("SOCIALCREDITNO"), json.Value<string>("ENABLED"),stopman,json.Value<string>("ID"));
            i =  DBMgrBase.ExecuteNonQuery(sql);

            //修改客商信息代码
            //info.Code, info.InspCode, info.InCode
            sql = @"update sys_customer set hscode='{0}',ciqcode='{1}' where code='{2}'";
            sql = String.Format(sql, json.Value<string>("CODE"), json.Value<string>("INSPCODE"), json.Value<string>("INCODE"));
            DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        
        //导出信息
        public DataTable export_base_company(string strWhere)
        {
            string sql = @"select t1.*,
                           t2.name as createmanname,
                           t3.name as stopmanname,
                           t4.name as declnaturename,
                           t5.name as inspnaturename,
                           t6.name as receivertypename
                           from base_company t1
                           left join sys_user t2
                           on t1.createman = t2.id
                           left join sys_user t3
                           on t1.stopman = t3.id
                           left join SYS_COMPANYNATURE t4
                           on t1.declnature = t4.code
                           left join BASE_INSPCOMPANYNATURE t5
                           on t1.inspnature = t5.code
                           left join Base_consigneetype t6
                           on t1.receivertype = t6.code
                           where 1 = 1 {0} order by t1.code asc
                           ";
            sql = string.Format(sql,strWhere);
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //导入sql语句
        public int export_insert_base_company(string incode, string CODE, string INSPCODE, string NAME, string ENGLSHNAME, string GOODSLOCAL, string RECEIVERTYPE, string ENABLED, string REMARK, string STARTDATE, string ENDDATE, string DeclNature, string InspNature, string stopman, string SOCIALCREDITNO)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql = @"insert into base_company(id,
                                                    code,name,remark,enabled,createman,stopman,createdate,
                                                    startdate,enddate,englishname,declnature,incode,
                                                    inspcode,inspnature,goodslocal,receivertype,SOCIALCREDITNO
                                                    ) values(base_company_id.nextval,
                                                    '{0}','{1}','{2}','{3}','{4}','{5}',sysdate,
                                                    to_date('{6}','yyyy/mm/dd hh24:mi:ss'),to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}','{9}','{10}'
                                                    ,'{11}','{12}','{13}','{14}','{15}'
                                                    )";
            sql = String.Format(sql, 
                CODE, NAME, REMARK, ENABLED, json_user.GetValue("ID"), stopman, 
                STARTDATE, ENDDATE,ENGLSHNAME,DeclNature,incode
                , INSPCODE, InspNature, GOODSLOCAL, RECEIVERTYPE, SOCIALCREDITNO);
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }
        //更新

        //查询修改记录之前的字段(根据id)
        public DataTable LoadDataById(string id)
        {
            string sql = "select * from base_company where id = '"+id+"'";
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //插入修改信息表base_alterrecord
        public int insert_base_alterrecord(JObject json,DataTable dt)
        {
            Base_Company_Method bcm = new Base_Company_Method();

            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql  = @"insert into base_alterrecord(id,
                                tabid,tabkind,alterman,
                                reason,contentes,alterdate) 
                                values(base_alterrecord_id.nextval,
                                '{0}','{1}','{2}',
                                '{3}','{4}',sysdate)";
            sql = String.Format(sql,
                                json.Value<string>("ID"), (int)Base_YearKindEnum.Decl_Company, json_user.GetValue("ID"),
                                json.Value<string>("REASON"),bcm.getChange(dt,json));
            int i = DBMgrBase.ExecuteNonQuery(sql);

            return i;

        }



    }
}