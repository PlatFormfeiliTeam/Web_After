using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.RedisManager
{
    public partial class EXT_Inspectionall : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"];
            string cusno = Request["CUSNO"] + ""; string approvalcode = Request["APPROVALCODE"] + ""; string inspectioncode = Request["INSPECTIONCODE"] + "";
            string fenkey = Request["FENKEY"];
            // fenkey = "declareall";
            int totalProperty = 0;
            long totalProperty_fenkey = 0;
            string where = string.Empty;
            DataTable dt;
            string json = string.Empty;
            string json_fenkey = string.Empty;
            string sql = "select * from redis_inspectionall where 1=1";
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            switch (action)
            {
                case "loadattach":

                    if (!string.IsNullOrEmpty(cusno))
                    {
                        where += " and CUSNO like '%" + cusno + "%'";
                    }
                    if (!string.IsNullOrEmpty(approvalcode))
                    {
                        where += " and APPROVALCODE like '%" + approvalcode + "%'";
                    }
                    if (!string.IsNullOrEmpty(inspectioncode))
                    {
                        where += " and INSPECTIONCODE like '%" + inspectioncode + "%'";
                    }
                    if (!string.IsNullOrEmpty(fenkey))
                    {
                        where += " and DIVIDEREDISKEY like '%" + fenkey + "%'";
                    }
                    sql += where;

                    sql = Extension.GetPageSql(sql, "ID", "desc", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
                    dt = DBMgr.GetDataTable(sql);
                    json = JsonConvert.SerializeObject(dt, iso);
                    Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
                    Response.End();
                    break;
                case "loadattach1":


                    IDatabase db = SeRedis.redis.GetDatabase();
                    json_fenkey = "[]";
                    if (fenkey != string.Empty && db.KeyExists(fenkey))
                    {
                        json_fenkey = "";
                        long start = Convert.ToInt64(Request["start"]);
                        long end = Convert.ToInt64(Request["start"]) + Convert.ToInt64(Request["limit"]);

                        if (cusno == string.Empty && approvalcode == string.Empty && inspectioncode == string.Empty)
                        {
                            RedisValue[] jsonlist = db.ListRange(fenkey, start, end - 1);
                            totalProperty_fenkey = db.ListLength(fenkey);
                            for (long i = 0; i < jsonlist.Length; i++)
                            {
                                json_fenkey += jsonlist[i];
                                if (i < jsonlist.Length - 1) { json_fenkey += ","; }
                            }
                            json_fenkey = "[" + json_fenkey + "]";
                        }
                        else
                        {
                            long len = db.ListLength(fenkey);
                            long tempi = 200; long i = 0;

                            List<string> jsonlist_t = new List<string>();
                            for (; i < len; i = i + tempi)
                            {

                                if ((i + tempi) >= len) { tempi = (len - i); }

                                RedisValue[] StatusList = db.ListRange(fenkey, i, i + (tempi - 1));
                                StatusList.Where<RedisValue>(st =>
                                {
                                    if (st.ToString().Contains(cusno) && st.ToString().Contains(approvalcode) && st.ToString().Contains(inspectioncode))
                                    {
                                        jsonlist_t.Add(st.ToString());
                                        return true;
                                    }
                                    return false;
                                }).ToList<RedisValue>();
                                tempi = 200;
                            }

                            totalProperty_fenkey = (long)jsonlist_t.Count;
                            if (totalProperty_fenkey < end) { end = totalProperty_fenkey; }
                            for (long j = start; j < end; j++)
                            {
                                if (totalProperty_fenkey <= start) { break; }


                                if (jsonlist_t[(int)j] != "")
                                {
                                    json_fenkey += jsonlist_t[(int)j];
                                    if (j < (end - 1)) { json_fenkey += ","; }
                                }

                            }
                            json_fenkey = "[" + json_fenkey + "]";


                        }
                    }

                    Response.Write("{rows:" + json_fenkey + ",total:" + totalProperty_fenkey + "}");
                    Response.End();
                    break;
            }

        }
    }
}