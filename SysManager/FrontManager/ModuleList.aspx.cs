using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.SysManager.FrontManager
{
    public partial class ModuleList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"];
            string moduleid = Request["MODULEID"];
            string sql = "";
            DataTable dt;
            switch (action)
            {
                case "select":
                    if (string.IsNullOrEmpty(moduleid))
                    {
                        sql = @"select * from sysmodule where ParentId='91a0657f-1939-4528-80aa-91b202a593ab' order by SortIndex";
                    }
                    else
                    {
                        sql = @"select * from sysmodule where ParentId ='" + moduleid + "' order by SortIndex";
                    }
                    dt = DBMgr.GetDataTable(sql);
                    string result = "[";
                    int i = 0;
                    foreach (DataRow smEnt in dt.Rows)
                    {
                        string children = string.Empty;
                        if (smEnt["ISLEAF"] + "" == "1")
                        {
                            result += "{children:[],";
                        }
                        else
                        {
                            result += "{";
                        }
                        if (i != dt.Rows.Count - 1)
                        {
                            result += "MODULEID:'" + smEnt["MODULEID"] + "',NAME:'" + smEnt["NAME"] + "',SORTINDEX:'" + smEnt["SORTINDEX"] + "',PARENTID:'" + smEnt["PARENTID"] + "',leaf:'" + smEnt["ISLEAF"] + "',URL:'" + smEnt["URL"] + "',ICON:'" + smEnt["ICON"] + "'},";
                        }
                        else
                        {
                            result += "MODULEID:'" + smEnt["MODULEID"] + "',NAME:'" + smEnt["NAME"] + "',SORTINDEX:'" + smEnt["SORTINDEX"] + "',PARENTID:'" + smEnt["PARENTID"] + "',leaf:'" + smEnt["ISLEAF"] + "',URL:'" + smEnt["URL"] + "',ICON:'" + smEnt["ICON"] + "'}";
                        }
                        i++;
                    }
                    result += "]";
                    Response.Write(result);
                    Response.End();
                    break;
                case "loadform":
                    sql = @"select ModuleID,Name,Url,ParentID,SORTINDEX,ICON from sysmodule where ModuleID = '" + moduleid + "'";
                    dt = DBMgr.GetDataTable(sql);
                    Response.Write("{success:true,data:" + JsonConvert.SerializeObject(dt).TrimStart('[').TrimEnd(']') + "}");
                    Response.End();
                    break;
                case "create":
                    string json = Request["json"];
                    JObject joc = (JObject)JsonConvert.DeserializeObject(json);
                    string newid = Guid.NewGuid().ToString();
                    string PARENTID = joc.Value<string>("PARENTID");
                    if (string.IsNullOrEmpty(PARENTID)) { PARENTID = "91a0657f-1939-4528-80aa-91b202a593ab"; }

                    sql = @"insert into sysmodule (MODULEID,NAME,ISLEAF,URL,PARENTID,SORTINDEX,ICON) 
                          values ('" + Guid.NewGuid().ToString() + "','" + joc.Value<string>("NAME") + "','1','" + joc.Value<string>("URL")
                                     + "','" + PARENTID + "','" + joc.Value<string>("SORTINDEX") + "','"
                        //+ "','" + joc.Value<string>("PARENTID") + "','" + joc.Value<string>("SORTINDEX") + "','" 
                                     + joc.Value<string>("ICON") + "')";
                    DBMgr.ExecuteNonQuery(sql);
                    joc.Remove("MODULEID");
                    joc.Add("MODULEID", newid);
                    joc.Add("leaf", 1);
                    sql = "select * from sysmodule where MODULEID='" + joc.Value<string>("PARENTID") + "'";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["ISLEAF"] + "" == "1")//如果父节点是叶子,需要改写父节点
                        {
                            sql = "update sysmodule set ISLEAF=NULL where MODULEID='" + joc.Value<string>("PARENTID") + "'";
                            DBMgr.ExecuteNonQuery(sql);
                        }
                    }
                    Response.Write("{success:true,data:" + joc + "}");
                    Response.End();
                    break;
                case "update":
                    json = Request["json"];
                    JObject jou = (JObject)JsonConvert.DeserializeObject(json);
                    sql = @"update sysmodule set NAME = '" + jou.Value<string>("NAME") + "' ,url = '" + jou.Value<string>("URL") + "',SORTINDEX = '" + jou.Value<string>("SORTINDEX") + "',ICON='" + jou.Value<string>("ICON") + "' where MODULEID = '" + jou.Value<string>("MODULEID") + "'";
                    DBMgr.ExecuteNonQuery(sql);
                    Response.Write("{success:true,data:" + jou + "}");
                    Response.End();
                    break;
                case "delete":
                    try
                    {
                        JObject jo = (JObject)JsonConvert.DeserializeObject(Request["json"]);
                        sql = "delete from sysmodule where MODULEID='" + jo.Value<string>("MODULEID") + "'";
                        DBMgr.ExecuteNonQuery(sql);
                        sql = "select * from sysmodule where PARENTID='" + jo.Value<string>("PARENTID") + "'";
                        dt = DBMgr.GetDataTable(sql);
                        if (dt.Rows.Count == 0)
                        {
                            sql = "update sysmodule set isleaf=1 where MODULEID='" + jo.Value<string>("PARENTID") + "'";
                            DBMgr.ExecuteNonQuery(sql);
                        }
                        Response.Write("{success:true}");
                    }
                    catch
                    {
                        Response.Write("{success:false}");
                    }
                    Response.End();
                    break;
            }
        }
    }
}