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
    public class Base_Container
    {
        //信息加载
        public DataTable LoaData(string table, string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,
                                  t2.name as createmanname,
                                  t3.name as stopmanname
                                  from base_containerstandard t1 left join sys_user t2 on t1.createman=t2.id 
                                  left join sys_user t3 on t1.stopman=t3.id 
                                  where 1 = 1 {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.code", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }

        //判断内部编码,海关编码,社会信用代码是否有重复
        public List<int> CheckRepeat(string id, string code, string name, string hscode)
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
            //集装箱规格编号重复返回值为1
            if (check_code_repeat(code, strWhere).Rows.Count > 0)
            {
                addList.Add(1);
            }
            //集装箱规格名称重复返回值为2
            if (check_name_repeat(name, strWhere).Rows.Count > 0)
            {
                addList.Add(2);
            }
            //对应hscode重复返回值为3
            if (!string.IsNullOrEmpty(hscode))
            {
                if (check_hscode_repeat(hscode, strWhere).Rows.Count > 0)
                {
                    addList.Add(3);
                }
            }


            return addList;
        }

        //判断重复返回值
        public string Check_Repeat(List<int> retunRepeat)
        {
            string repeat = "";
            for (int i = 0; i < retunRepeat.Count; i++)
            {
                if (retunRepeat[i] == 1)
                {
                    repeat = repeat + "集装箱规格编号重复,";
                }
                if (retunRepeat[i] == 2)
                {
                    repeat = repeat + "集装箱规格名称重复,";
                }
                if (retunRepeat[i] == 3)
                {
                    repeat = repeat + "集装箱对应hscode重复,";
                }

            }
            return repeat;

        }

        //向base_container表中插入数据
        public int insert_base_container(JObject json, string stopman)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
//            string sql = @"insert into base_containerstandard (id,code,name,hscode,hsname,inspection,declaration,enabled,remark,createman,stopman,startdate,enddate,createdate)
//values(base_containerstandard_id.nextval,'{0-code}','{1-name}','{2-hscode}','{3-hsname}','{4-inspection}','{5-declaration}','{6-enabled}','{7-remark}','{8-createman}','{9-stopman}',to_date('{10-startdate}','yyyy/mm/dd hh24:mi:ss'),
//to_date('{11-enddate}','yyyy/mm/dd hh24:mi:ss'),sysdate)";
            string sql = @"insert into base_containerstandard (id,code,name,hscode,hsname,inspection,declaration,enabled,remark,createman,stopman,startdate,enddate,createdate)
                                   values(base_containerstandard_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',to_date('{10}','yyyy/mm/dd hh24:mi:ss'),
                                   to_date('{11}','yyyy/mm/dd hh24:mi:ss'),sysdate)";
            sql = String.Format(sql
                , json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("HSCODE"), "", "", "", json.Value<string>("ENABLED"), json.Value<string>("REMARK"), json_user.GetValue("ID"), stopman
                , json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE")
                , json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE")
                );
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        /// <summary>
        ///从数据库根据id抓取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable LoadDataById(string id)
        {
            string sql = "select * from base_containerstandard where id = '" + id + "'";
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="json"></param>
        /// <param name="stopman"></param>
        /// <returns></returns>
        public int update_base_container(JObject json,string stopman)
        {
            int i = 0;
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
            string sql = @"update base_containerstandard set code='{0}',name='{1}',hscode='{2}',enabled='{3}',
                                   createman='{4}',stopman='{5}',remark='{6}',startdate=to_date('{7}','yyyy/mm/dd'),enddate=to_date('{8}','yyyy/mm/dd')
                                   where id='{9}'";
            sql = string.Format(sql, json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("HSCODE"),json.Value<string>("ENABLED"),
                 json_user.GetValue("ID"), stopman,json.Value<string>("REMARK"), json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE")
                , json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),json.Value<string>("ID"));
             i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
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
                                json.Value<string>("ID"), (int)Base_YearKindEnum.Insp_ContainerStandard, json_user.GetValue("ID"),
                                json.Value<string>("REASON"), getChange(dt, json));
            int i = DBMgrBase.ExecuteNonQuery(sql);

            return i;

        }

        public string getChange(DataTable dt, JObject json)
        {
            string str = "";

            if (dt.Rows[0]["code"] != json.Value<string>("CODE"))
            {
                str += "集装箱规格代码：" + dt.Rows[0]["code"] + "——>" + json.Value<string>("CODE") + "。";
            }

            if (dt.Rows[0]["name"] != json.Value<string>("NAME"))
            {
                str += "集装箱规格名称：" + dt.Rows[0]["name"] + "——>" + json.Value<string>("NAME") + "。";
            }

            if (dt.Rows[0]["hscode"] != json.Value<string>("HSCODE"))
            {
                str += "集装箱规格名称：" + dt.Rows[0]["hscode"] + "——>" + json.Value<string>("HSCODE") + "。";
            }

            if (dt.Rows[0]["enabled"] != json.Value<string>("ENABLED"))
            {
                str += "启用：" + dt.Rows[0]["name"] + "——>" + json.Value<string>("NAME") + "。";
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

        private DataTable check_code_repeat(string code, string strWhere)
        {
            string sql = "select * from base_containerstandard where enabled = '1' and lower(code) = lower('{0}') {1}";
            sql = string.Format(sql, code, strWhere);
            DataTable check_table = DBMgrBase.GetDataTable(sql);
            return check_table;
        }

        private DataTable check_name_repeat(string name, string strWhere)
        {
            string sql = "select * from base_containerstandard where enabled = '1' and name = '{0}' {1}";
            sql = string.Format(sql, name, strWhere);
            DataTable check_table = DBMgrBase.GetDataTable(sql);
            return check_table;
        }

        private DataTable check_hscode_repeat(string hscode, string strWhere)
        {
            string sql = "select * from base_containerstandard where enabled = '1' and hscode = '{0}' {1}";
            sql = string.Format(sql, hscode, strWhere);
            DataTable check_table = DBMgrBase.GetDataTable(sql);
            return check_table;
        }

    }
}