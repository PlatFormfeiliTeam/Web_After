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
    public class FtpSetting
    {
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.id
                                        ,t1.profilename
                                        ,t1.uri
                                        ,t1.port
                                        ,t1.username
                                        ,t1.password
                                        ,t1.enabled
                                        ,t1.channelname
                                        ,t1.filetype
                                        ,t1.customdistrictcode
                                        ,decode(t1.entrusttype,'01','报关','02','报检') as entrusttype
                                   from sys_ftpsettings t1 {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.profilename", "", ref totalProperty, start, limit);
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
            if (check_hscode_repeat(declcountry, strWhere).Rows.Count > 0)
            {
                addList.Add(1);
            }
            return addList;
        }

        public DataTable check_hscode_repeat(string declcountry, string strWhere)
        {
            string sql = @"select * from sys_ftpsettings where profilename='{0}'  '" + strWhere;
            sql = string.Format(sql, declcountry);
            return DBMgrBase.GetDataTable(sql);
        }

        public int insert_Ftpsetting(JObject json)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);

//            string sql = @"insert into rela_withinregion (id,declregion,inspregion,createman,stopman,createdate,startdate,enddate,enabled,remark)
//                                  values(rela_withinregion_id.nextval,'{0}','{1}','{2}','{3}',sysdate,to_date('{4}','yyyy-mm-dd hh24:mi:ss'),
//                                  to_date('{5}','yyyy-mm-dd hh24:mi:ss'),'{6}','{7}')";
//            sql = string.Format(sql, json.Value<string>("DECLREGION"), json.Value<string>("INSPREGION"), json_user.GetValue("ID"), stopman,
//                json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
//                 json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
//                 json.Value<string>("ENABLED"), json.Value<string>("REMARK"));
            string sql = @"insert into sys_ftpsettings (ID,profilename,uri,port,username,password,enabled,channelname,filetype,customdistrictcode,entrusttype)
values (sys_ftpsettings_id.nextval,'0','1','2','3','4','5','6','7','8','9')";
            sql = string.Format(sql, json.Value<string>("PROFILENAME"),
                json.Value<string>("URI"),
                json.Value<string>("PORT"),
                json.Value<string>("USERNAME"),
                json.Value<string>("PASSWORD"),
                json.Value<string>("ENABLED"),
                json.Value<string>("CHANNELNAME"),
                json.Value<string>("FILETYPE"),
                json.Value<string>("CUSTOMDISTRICTCODE"),
                json.Value<string>("ENTRUSTTYPE")
                );
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        public DataTable LoadDataById(string id)
        {
            string sql = @"select * from sys_ftpsettings t1 where t1.id='{0}'";
            sql = string.Format(sql, id);
            return DBMgrBase.GetDataTable(sql);
        }

        public int update_Ftpsetting(JObject json)
        {
            FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
            string userName = identity.Name;
            JObject json_user = Extension.Get_UserInfo(userName);
//            string sql = @"update rela_withinregion set declregion='{0}',inspregion='{1}',createman='{2}',stopman='{3}',createdate=sysdate,
//                                 startdate =to_date('{4}','yyyy-mm-dd hh24:mi:ss'),enddate=to_date('{5}','yyyy-mm-dd hh24:mi:ss'),enabled='{6}',remark='{7}'
//                                 where id='{8}'";
//            sql = string.Format(sql, json.Value<string>("DECLREGION"), json.Value<string>("INSPREGION"), json_user.GetValue("ID"), stopman,
//                 json.Value<string>("STARTDATE") == "" ? DateTime.MinValue.ToShortDateString() : json.Value<string>("STARTDATE"),
//                 json.Value<string>("ENDDATE") == "" ? DateTime.MaxValue.ToShortDateString() : json.Value<string>("ENDDATE"),
//                 json.Value<string>("ENABLED"), json.Value<string>("REMARK"), json.Value<string>("ID"));
            string sql = @"update sys_ftpsettings set profilename='{0}',uri='{1}',port='{2}',username='{3}',password='{4}',enabled='{5}',channelname='{6}',filetype='{7}',customdistrictcode='{8}',
entrusttype='{9}' where id='{10}'";
            sql = string.Format(sql, json.Value<string>("PROFILENAME"),
                json.Value<string>("URI"),
                json.Value<string>("PORT"),
                json.Value<string>("USERNAME"),
                json.Value<string>("PASSWORD"),
                json.Value<string>("ENABLED"),
                json.Value<string>("CHANNELNAME"),
                json.Value<string>("FILETYPE"),
                json.Value<string>("CUSTOMDISTRICTCODE"),
                json.Value<string>("ENTRUSTTYPE"),
                json.Value<string>("ID")
                );
            int i = DBMgrBase.ExecuteNonQuery(sql);
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

            if (dt.Rows[0]["profilename"] != json.Value<string>("PROFILENAME"))
            {
                str += "配置方案名称：" + dt.Rows[0]["profilename"] + "——>" + json.Value<string>("PROFILENAME") + "。";
            }

            if (dt.Rows[0]["uri"] != json.Value<string>("URI"))
            {
                str += "FTP服务器URI：" + dt.Rows[0]["uri"] + "——>" + json.Value<string>("URI") + "。";
            }
            if (dt.Rows[0]["port"] != json.Value<string>("PORT"))
            {
                str += "FTP服务器PORT：" + dt.Rows[0]["port"] + "——>" + json.Value<string>("PORT") + "。";
            }
            if (dt.Rows[0]["username"] != json.Value<string>("USERNAME"))
            {
                str += "FTP用户名：" + dt.Rows[0]["username"] + "——>" + json.Value<string>("USERNAME") + "。";
            }
            if (dt.Rows[0]["password"] != json.Value<string>("PASSWORD"))
            {
                str += "FTP密码：" + dt.Rows[0]["password"] + "——>" + json.Value<string>("PASSWORD") + "。";
            }

            if (dt.Rows[0]["enabled"] != json.Value<string>("ENABLED"))
            {
                str += "启用：" + dt.Rows[0]["enabled"] + "——>" + json.Value<string>("ENABLED") + "。";
            }
            if (dt.Rows[0]["channelname"] != json.Value<string>("CHANNELNAME"))
            {
                str += "通道名称：" + dt.Rows[0]["channelname"] + "——>" + json.Value<string>("CHANNELNAME") + "。";
            }
            if (dt.Rows[0]["filetype"] != json.Value<string>("FILETYPE"))
            {
                str += "发送文件类型：" + dt.Rows[0]["filetype"] + "——>" + json.Value<string>("FILETYPE") + "。";
            }
            if (dt.Rows[0]["customdistrictcode"] != json.Value<string>("CUSTOMDISTRICTCODE"))
            {
                str += "适用关区：" + dt.Rows[0]["customdistrictcode"] + "——>" + json.Value<string>("CUSTOMDISTRICTCODE") + "。";
            }
            if (dt.Rows[0]["entrusttype"] != json.Value<string>("ENTRUSTTYPE"))
            {
                str += "申报类型：" + dt.Rows[0]["entrusttype"] + "——>" + json.Value<string>("ENTRUSTTYPE") + "。";
            }
            return str;

        }


        public DataTable export_rela_ftpsetting(string strWhere)
        {
            string sql = @"select t1.id
                                        ,t1.profilename
                                        ,t1.uri
                                        ,t1.port
                                        ,t1.username
                                        ,t1.password
                                        ,t1.enabled
                                        ,t1.channelname
                                        ,t1.filetype
                                        ,t1.customdistrictcode
                                        ,decode(t1.entrusttype,'01','报关','02','报检') as entrusttype
                                   from sys_ftpsettings t1  {0}";
            sql = string.Format(sql, strWhere);
            return DBMgrBase.GetDataTable(sql);
        }
    }
}