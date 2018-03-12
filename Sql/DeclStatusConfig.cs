using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Web_After.Common;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using Web_After.BasicManager;

namespace Web_After.Sql
{
    public class DeclStatusConfig
    {
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as busistatusname from base_statusConfig t1 left join sys_user t2 on t1.createman=t2.id 
left join sys_user t3 on t1.stopman=t3.id left join base_declstatus t4 on t1.busistatus=t4.code  {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.code", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }

        public List<int> CheckRepeat(string id, string declcountry)
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
            Sql.Base_Company bc = new Sql.Base_Company();
            //对应关系重复返回值为1
            if (check_status_repeat(declcountry, strWhere).Rows.Count > 0)
            {
                addList.Add(1);
            }
            return addList;
        }

        public DataTable check_status_repeat(string declcountry, string strWhere)
        {
            string sql = @"select * from base_statusConfig where code='{0}' " + strWhere;
            sql = string.Format(sql, declcountry);
            return DBMgrBase.GetDataTable(sql);
        }

        public DataTable LoadDataById(string id)
        {
            string sql = @"select * from base_statusConfig t1 where t1.id='{0}'";
            sql = string.Format(sql, id);
            return DBMgrBase.GetDataTable(sql);
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
                                json.Value<string>("ID"), (int)Base_YearKindEnum.Busi_StatusConfig, json_user.GetValue("ID"),
                                json.Value<string>("REASON"), getChange(dt, json));
            int i = DBMgrBase.ExecuteNonQuery(sql);

            return i;

        }

        public string getChange(DataTable dt, JObject json)
        {
            string str = "";

            if (dt.Rows[0]["code"] != json.Value<string>("CODE"))
            {
                str += "回执状态码：" + dt.Rows[0]["code"] + "——>" + json.Value<string>("CODE") + "。";
            }

            if (dt.Rows[0]["name"] != json.Value<string>("NAME"))
            {
                str += "回执状态名称：" + dt.Rows[0]["name"] + "——>" + json.Value<string>("NAME") + "。";
            }
            if (dt.Rows[0]["busistatus"] != json.Value<string>("BUSISTATUS"))
            {
                str += "业务状态：" + dt.Rows[0]["busistatus"] + "——>" + json.Value<string>("BUSISTATUS") + "。";
            }
            if (dt.Rows[0]["type"] != json.Value<string>("TYPE"))
            {
                str += "所属类型：" + dt.Rows[0]["type"] + "——>" + json.Value<string>("TYPE") + "。";
            }
            if (dt.Rows[0]["description"] != json.Value<string>("DESCRIPTION"))
            {
                str += "显示描述：" + dt.Rows[0]["description"] + "——>" + json.Value<string>("DESCRIPTION") + "。";
            }
            if (dt.Rows[0]["orderno"] != json.Value<string>("ORDERNO"))
            {
                str += "序号：" + dt.Rows[0]["orderno"] + "——>" + json.Value<string>("ORDERNO") + "。";
            }
            if (dt.Rows[0]["enabled"] != json.Value<string>("ENABLED"))
            {
                str += "启用：" + dt.Rows[0]["enabled"] + "——>" + json.Value<string>("ENABLED") + "。";
            }

            if (dt.Rows[0]["remark"] != json.Value<string>("REMARK"))
            {
                str += "备注：" + dt.Rows[0]["remark"] + "——>" + json.Value<string>("REMARK") + "。";
            }
            if (dt.Rows[0]["StartDate"] != json.Value<string>("STARTDATE"))
            {
                str += "开始时间：" + dt.Rows[0]["StartDate"] + "——>" + json.Value<string>("STARTDATE") + "。";
            }
            if (dt.Rows[0]["EndDate"] != json.Value<string>("ENDDATE"))
            {
                str += "停用时间：" + dt.Rows[0]["EndDate"] + "——>" + json.Value<string>("ENDDATE") + "。";
            }
            return str;

        }


        public int insert_statusconfig(JObject json, string stopman)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);

            string sql = @"insert into base_statusconfig (id,code,name,busistatus,remark,enabled,createman,stopman,createdate,startdate,enddate,type,description,orderno)
values (base_statusconfig_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}',sysdate,to_date('{7}','yyyy-mm-dd hh24:mi:ss'),to_date('{8}','yyyy-mm-dd hh24:mi:ss'),'{9}',
'{10}','{11}')";
            sql = string.Format(sql,json.Value<string>("CODE"),json.Value<string>("NAME"),
                json.Value<string>("BUSISTATUS"),json.Value<string>("REMARK"),
                 json.Value<string>("ENABLED"),json_user.GetValue("ID"),stopman,
                 json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
                 json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
                 json.Value<string>("TYPE"), json.Value<string>("DESCRIPTION"),
                 json.Value<string>("ORDERNO"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public int update_statusconfig(JObject json, string stopman)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql = @"update base_statusconfig set code='{0}',name='{1}',busistatus='{2}',remark='{3}',
                                    enabled='{4}',createman='{5}',stopman='{6}',startdate=to_date('{7}','yyyy-mm-dd hh24:mi:ss'),
                                    enddate=to_date('{8}','yyyy-mm-dd hh24:mi:ss'),type='{9}',description='{10}',orderno='{11}'
                                    where id='{12}'";
            sql = string.Format(sql, json.Value<string>("CODE"),json.Value<string>("NAME"),
                json.Value<string>("BUSISTATUS"),json.Value<string>("REMARK"),
                 json.Value<string>("ENABLED"),json_user.GetValue("ID"),stopman,
                 json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
                 json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
                 json.Value<string>("TYPE"), json.Value<string>("DESCRIPTION"),
                 json.Value<string>("ORDERNO"), json.Value<string>("ID"));
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public DataTable export_rela_declstatus(string strWhere)
        {
            string sql = @"select t1.*,decode(t1.type,'1','报关','2','报检') as typeclass,t2.name as createmanname,t3.name as stopmanname,t4.name as busistatusname from base_statusConfig t1 left join sys_user t2 on t1.createman=t2.id 
left join sys_user t3 on t1.stopman=t3.id left join base_declstatus t4 on t1.busistatus=t4.code  {0}";
            sql = string.Format(sql, strWhere);
            return DBMgrBase.GetDataTable(sql);
        }
    }
}