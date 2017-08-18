using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.SysManager.AfterManager
{
    public partial class WebAuthListByPosition : System.Web.UI.Page
    {
        string action = string.Empty; string username = string.Empty; string userid = string.Empty; string type = string.Empty; string positionid = string.Empty; string ParentID = string.Empty;
        string sql = string.Empty; string sql_table = string.Empty;
        string result = ""; DataTable ents; string result_1 = ""; string result_2 = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            action = Request["action"]; userid = Request["userid"]; type = Request["type"]; positionid = Request["positionid"]; ParentID = Request["ParentID"];

            sql_table = "";
            if (positionid == "1") { sql_table = " POSITIONID_BEFORE=1"; }
            if (positionid == "2") { sql_table = " POSITIONID_AFTER=1"; }

            switch (action)
            {
                case "loaduser":
                    username = HttpContext.Current.User.Identity.Name;
                    JObject json_user = Extension.Get_UserInfo(username);

                    if (json_user.Value<string>("TYPE") == "0")//管理员
                    {
                        result_1 = getTree("1"); result_2 = getTree("2");
                        result = "[{id:'',name:'前台管理',ParentID:'',leaf:false,children:" + result_1 + "},{id:'',name:'后台管理',ParentID:'',leaf:false,children:" + result_2 + "}]";
                    }
                    else
                    {
                        sql = "select * from sys_user where name='" + username + "'";
                        DataTable dt = DBMgr.GetDataTable(sql);
                        result = getchildrenTree(dt.Rows[0]["ID"].ToString(), dt.Rows[0]["POSITIONID"].ToString());
                    }
                    Response.Write(result);
                    Response.End();
                    break;
                case "SaveAuthorByPosition":
                    string moduleids = Request["moduleids"];
                    sql = @"DELETE FROM SYS_MODULEUSER_back WHERE USERID = '{0}'";
                    sql = string.Format(sql, userid);
                    DBMgr.ExecuteNonQuery(sql);
                    string[] ids = moduleids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string moduleid in ids)
                    {
                        sql = @"insert into SYS_MODULEUSER_back (USERID,MODULEID) values ('{0}','{1}')";
                        sql = string.Format(sql, userid, moduleid);
                        DBMgr.ExecuteNonQuery(sql);
                    }

                    updateChildrenAuthority();

                    Response.Write("{success:true}");
                    Response.End();
                    break;
                case "loadauthority":
                    if (!string.IsNullOrEmpty(userid))
                    {
                        if (sql_table != "")
                        {
                            if (type == "1")//主账号 拉出 前或者后 的有权限菜单
                            {
                                sql_table = "select * from sysmodule where " + sql_table;

                                sql = @"select t.*,u.MODULEID AUTHORITY from ({2}) t  left join (select * from sys_moduleuser_back where userid='{0}') u on t.MODULEID=u.MODULEID
                                            where  t.ParentId='{1}' order by t.SortIndex";
                                sql = string.Format(sql, userid, Request["id"], sql_table);
                            }
                            if (type == "2")//子账号 拉出 主账号的有权限菜单
                            {
                                sql_table = "select b.* from sys_moduleuser_back a inner join sysmodule b on a.MODULEID=b.MODULEID  where userid='" + ParentID + "'";

                                sql = @"select t.*,u.MODULEID AUTHORITY from ({2}) t  left join (select * from sys_moduleuser_back where userid='{0}') u on t.MODULEID=u.MODULEID
                                            where  t.ParentId='{1}' order by t.SortIndex";
                                sql = string.Format(sql, userid, Request["id"], sql_table);
                            }
                        }
                    }
                    result = "[";
                    if (!string.IsNullOrEmpty(sql))
                    {
                        DataTable dt = DBMgr.GetDataTable(sql);
                        int i = 0;
                        string children = string.Empty;
                        foreach (DataRow dr in dt.Rows)
                        {
                            children = getchildren(dr["MODULEID"].ToString(), userid, sql_table);
                            if (i != dt.Rows.Count - 1)
                            {
                                result += "{id:'" + dr["MODULEID"] + "',name:'" + dr["NAME"] + "',ParentID:'" + dr["PARENTID"] + "',leaf:'" + dr["ISLEAF"] + "',checked:" + (string.IsNullOrEmpty(dr["AUTHORITY"] + "") ? "false" : "true") + ",children:" + children + "},";
                            }
                            else
                            {
                                result += "{id:'" + dr["MODULEID"] + "',name:'" + dr["NAME"] + "',ParentID:'" + dr["PARENTID"] + "',leaf:'" + dr["ISLEAF"] + "',checked:" + (string.IsNullOrEmpty(dr["AUTHORITY"] + "") ? "false" : "true") + ",children:" + children + "}";
                            }
                            i++;
                        }
                    }
                    result += "]";
                    Response.Write(result);
                    Response.End();
                    break;
            }
        }

        public string getTree(string positionid)
        {
            string returnstr = "[";
            string sql = "select * from sys_user where enabled=1 and parentid is null and type=1 and positionid={0}";
            sql = string.Format(sql, positionid);

            DataTable dt = DBMgr.GetDataTable(sql);
            int i = 0;
            string children = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                children = getchildrenTree(dr["ID"].ToString(), positionid);
                returnstr += "{id:'" + dr["ID"] + "',name:'" + dr["NAME"] + "',ParentID:'" + dr["PARENTID"] + "',leaf:" + (dr["TYPE"] + "" == "2" ? true : false).ToString().ToLower()
                    + ",positionid:'" + positionid + "',REALNAME:'" + dr["REALNAME"] + "',type:'1',children:" + children + "}";
                if (i != dt.Rows.Count - 1) { result += ","; }
                i++;
            }
            returnstr += "]";
            return returnstr;
        }

        //查出一级子账号
        private string getchildrenTree(string id, string positionid)
        {
            string children = "[";
            sql = "select * from sys_user where enabled=1 and parentid ='{0}'";
            sql = string.Format(sql, id);
            DataTable dt = DBMgr.GetDataTable(sql);
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                children += "{id:'" + dr["ID"] + "',name:'" + dr["NAME"] + "',ParentID:'" + dr["PARENTID"] + "',leaf:true,positionid:'" + positionid + "',REALNAME:'" + dr["REALNAME"] + "',type:'2'}";
                if (i != dt.Rows.Count - 1) { children += ","; }
                i++;
            }
            children += "]";
            return children;
        }


        private void updateChildrenAuthority()
        {
            try
            {
                //主账号的ModuleId集合
                DataTable moduleIdDt = DBMgr.GetDataTable("select MODULEID from SYS_MODULEUSER_back where userid = " + userid);
                List<string> mIdList = Extension.getColumnFromDatatable(moduleIdDt, "MODULEID");

                //子账号ID集合
                DataTable cdIds = DBMgr.GetDataTable("select ID from SYS_USER where parentid = " + userid);

                //遍历子账号
                foreach (DataRow drId in cdIds.Rows)
                {
                    //查询该子账号的moduleIds
                    DataTable mIdsDt = DBMgr.GetDataTable("select MODULEID from SYS_MODULEUSER_back where userid = " + drId["ID"].ToString());

                    //删除该子账号原有的moduleIds
                    DBMgr.ExecuteNonQuery("DELETE FROM SYS_MODULEUSER_back WHERE USERID = " + drId["ID"].ToString());

                    //插入子账号moduleIds
                    foreach (DataRow mId in mIdsDt.Rows)
                    {
                        //过滤掉子账号中主账号没有的moduleIds
                        if (mIdList.Contains(mId["MODULEID"].ToString()))
                        {
                            DBMgr.ExecuteNonQuery("insert into SYS_MODULEUSER_back (USERID,MODULEID) values ('" + drId["ID"].ToString() + "','" + mId["MODULEID"].ToString() + "')");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //log.Error(e.Message + e.StackTrace);
            }

            Response.Write("{success:true}");
            Response.End();

        }

        private string getchildren(string moduleid, string userid, string sql_table)
        {
            string children = "[";
            sql = @"select t.*,u.MODULEID AUTHORITY from ({2}) t left join (select * from sys_moduleuser_back where userid='{0}') u on t.MODULEID=u.MODULEID
                where  t.ParentId ='{1}' order by t.SortIndex";
            sql = string.Format(sql, userid, moduleid, sql_table);
            DataTable dt = DBMgr.GetDataTable(sql);
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string tmp_children = getchildren(dr["MODULEID"].ToString(), userid, sql_table);
                if (i != dt.Rows.Count - 1)
                {
                    children += "{id:'" + dr["MODULEID"] + "',name:'" + dr["NAME"] + "',ParentID:'" + dr["PARENTID"] + "',leaf:'" + dr["ISLEAF"] + "',checked:" + (string.IsNullOrEmpty(dr["AUTHORITY"] + "") ? "false" : "true") + ",children:" + tmp_children + "},";
                }
                else
                {
                    children += "{id:'" + dr["MODULEID"] + "',name:'" + dr["NAME"] + "',ParentID:'" + dr["PARENTID"] + "',leaf:'" + dr["ISLEAF"] + "',checked:" + (string.IsNullOrEmpty(dr["AUTHORITY"] + "") ? "false" : "true") + ",children:" + tmp_children + "}";
                }
                i++;
            }
            children += "]";
            return children;
        }

    }
}