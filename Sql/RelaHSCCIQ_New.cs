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
    public class RelaHSCIQ_New
    {
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_new_hsciq t1 
                            left join sys_user t2 on t1.createman=t2.id left join sys_user t3 on t1.stopman=t3.id {0}";
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
            string HSNAME = "";
            if (!string.IsNullOrEmpty(json.Value<string>("HSNAME")))
            {
                if (json.Value<string>("HSNAME").Contains("|"))
                {
                    HSNAME = json.Value<string>("HSNAME").Substring(0, json.Value<string>("HSNAME").IndexOf("|"));
                }
                else
                {
                    HSNAME = json.Value<string>("HSNAME");
                }
            }
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql = @"insert into base_new_hsciq (id,hscode,hsname,ciqcode,ciqname,remark,enabled,createman,stopman,createdate,startdate,enddate,type)
                                   values (base_new_hsciq_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',sysdate,to_date('{8}','yyyy-mm-dd hh24:mi:ss'),
                                   to_date('{9}','yyyy-mm-dd hh24:mi:ss'),'{10}')";
            sql = string.Format(sql, json.Value<string>("HSCODE"), HSNAME.Replace("'", "'||chr(39)||'"), 
                json.Value<string>("CIQCODE"), json.Value<string>("CIQNAME").Replace("'", "'||chr(39)||'"), 
                json.Value<string>("REMARK"), json.Value<string>("ENABLED"),json_user.GetValue("ID"), stopman, 
                json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
                 json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
                 json.Value<string>("TYPE").Replace("'", "'||chr(39)||'"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public int insert_rela_hs_ciq_excel(string HSCODE, string CIQCODE,string hsname,string ciqname,string ENABLED,string TYPE,string REMARK,string sdate,string edate)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql = @"insert into base_new_hsciq (id,hscode,hsname,ciqcode,ciqname,remark,enabled,createman,createdate,type)
                                   values (base_new_hsciq_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}',sysdate,'{7}')";
            sql = string.Format(sql, HSCODE, hsname.Replace("'", "'||chr(39)||'"), CIQCODE, ciqname.Replace("'", "'||chr(39)||'"),
                REMARK.Replace("'", "'||chr(39)||'"), ENABLED, json_user.GetValue("ID"), TYPE.Replace("'", "'||chr(39)||'"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public int update_rela_hs_ciq(JObject json, string stopman)
        {
            int i = 0;
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            string HSNAME = "";
            if (!string.IsNullOrEmpty(json.Value<string>("HSNAME")))
            {
                if(json.Value<string>("HSNAME").Contains("|"))
                {
                    HSNAME = json.Value<string>("HSNAME").Substring(0, json.Value<string>("HSNAME").IndexOf("|"));
                }
                else
                {
                    HSNAME = json.Value<string>("HSNAME");
                }
            }
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql = @"update base_new_hsciq set hscode='{0}',hsname='{1}',ciqcode='{2}',ciqname='{3}',remark='{4}',enabled='{5}',stopman='{6}',
                            startdate= to_date('{7}','yyyy-mm-dd hh24:mi:ss'),enddate= to_date('{8}','yyyy-mm-dd hh24:mi:ss'),type='{9}' where id='{10}'";
            sql = string.Format(sql, json.Value<string>("HSCODE"), HSNAME, json.Value<string>("CIQCODE"), json.Value<string>("CIQNAME"),
                            json.Value<string>("REMARK"), json.Value<string>("ENABLED"), stopman,
                            json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
                            json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
                            json.Value<string>("TYPE"), json.Value<string>("ID"));
            i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public DataTable check_hscode_repeat(string hscode, string ciqcode, string strWhere)
        {
            string sql = "select * from base_new_hsciq t1 where t1.enabled=1 and  t1.hscode='{0}' and t1.ciqcode='{1}'";
            sql = string.Format(sql, hscode, ciqcode) + strWhere;
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
                                json.Value<string>("ID"), (int)Base_YearKindEnum.BASE_NEW_HSCIQ, json_user.GetValue("ID"),
                                json.Value<string>("REASON"), getChange(dt, json));
            int i = DBMgrBase.ExecuteNonQuery(sql);

            return i;

        }

        public DataTable LoadDataById(string id)
        {
            string sql = "select * from base_new_hsciq t1 where t1.id='{0}'";
            sql = string.Format(sql, id);
            return DBMgrBase.GetDataTable(sql);
        }

        public string getChange(DataTable dt, JObject json)
        {
            string str = "";

            if (dt.Rows[0]["hscode"].ToString2() != json.Value<string>("HSCODE"))
            {
                str += "hs代码：" + dt.Rows[0]["hscode"] + "——>" + json.Value<string>("HSCODE") + "。";
            }

            if (dt.Rows[0]["ciqcode"].ToString2() != json.Value<string>("CIQCODE"))
            {
                str += "ciq代码：" + dt.Rows[0]["ciqcode"] + "——>" + json.Value<string>("CIQCODE") + "。";
            }

            if (dt.Rows[0]["enabled"].ToString2() != json.Value<string>("ENABLED"))
            {
                str += "启用：" + dt.Rows[0]["enabled"] + "——>" + json.Value<string>("ENABLED") + "。";
            }

            if (dt.Rows[0]["remark"].ToString2() != json.Value<string>("REMARK"))
            {
                str += "备注：" + dt.Rows[0]["remark"] + "——>" + json.Value<string>("REMARK") + "。";
            }
            if (dt.Rows[0]["StartDate"].ToString2() != json.Value<string>("STARTDATE"))
            {
                str += "开始时间：" + dt.Rows[0]["StartDate"] + "——>" + json.Value<string>("STARTDATE") + "。";
            }
            if (dt.Rows[0]["EndDate"].ToString2() != json.Value<string>("ENDDATE"))
            {
                str += "停用时间：" + dt.Rows[0]["EndDate"] + "——>" + json.Value<string>("ENDDATE") + "。";
            }
            if (dt.Rows[0]["TYPE"].ToString2() != json.Value<string>("TYPE"))
            {
                str += "类型：" + dt.Rows[0]["TYPE"] + "——>" + json.Value<string>("TYPE") + "。";
            }
            return str;

        }


        public DataTable export_rela_hs_ciq(string strWhere)
        {
            string sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_new_hsciq t1 
left join sys_user t2 on t1.createman=t2.id left join sys_user t3 on t1.stopman=t3.id {0}";
            sql = string.Format(sql, strWhere);
            return DBMgrBase.GetDataTable(sql);
        }


    }
}