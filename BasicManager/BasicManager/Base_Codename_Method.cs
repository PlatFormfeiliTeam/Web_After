using System;
using System.Data;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Web_After.Common;

namespace Web_After.BasicManager.BasicManager
{
    public class Base_Codename_Method
    {
        //获取修改记录
        public string getChange(DataTable dt,JObject json,string table)
        {
            string str = "";
            Switch_helper_Base_codename sc = new Switch_helper_Base_codename();
            if (dt.Rows[0][sc.getColum(table)[2]] != json.Value<string>(sc.getColum(table)[2]))
            {
                str += sc.getColum(table)[0] + "：" + dt.Rows[0][sc.getColum(table)[2]] + "——>" + json.Value<string>(sc.getColum(table)[2]) + "。";
            }

            if (dt.Rows[0][sc.getColum(table)[3]] != json.Value<string>(sc.getColum(table)[3]))
            {
                str += sc.getColum(table)[1] + "：" + dt.Rows[0][sc.getColum(table)[3]] + "——>" + json.Value<string>(sc.getColum(table)[3]) + "。";
            }

            if (dt.Rows[0]["StartDate"] != json.Value<string>("STARTDATE"))
            {
                str += "开始时间：" + dt.Rows[0]["StartDate"] + "——>" + json.Value<string>("STARTDATE") + "。";
            }
            if (dt.Rows[0]["EndDate"] != json.Value<string>("ENDDATE"))
            {
                str += "停用时间：" + dt.Rows[0]["EndDate"] + "——>" + json.Value<string>("ENDDATE") + "。";
            }
            if (dt.Rows[0]["remark"] != json.Value<string>("REMARK"))
            {
                str += "备注：" + dt.Rows[0]["remark"] + "——>" + json.Value<string>("REMARK") + "。";
            }
            return str;
        }

        
        //获取当前用户，停用人，启用时间/停用时间的判断
        public void getCommonInformation(out string stopman, out string createman, out string startdate, out string enddate, JObject json)
        {
             startdate = "";
             enddate = "";
             stopman = "";
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


             createman = json_user.GetValue("ID").ToString();

        }
        //修改
        public string getChangeBase_year(DataTable dt,JObject json)
        {
            string str = "";
            if (dt.Rows[0]["NAME"] != json.Value<string>("NAME"))
            {
                str += "规则名称：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
            }

            if (dt.Rows[0]["ENABLED"] != json.Value<string>("ENABLED"))
            {
                str += "是否启用：" + dt.Rows[0]["ENABLED"] + "——>" + json.Value<string>("ENABLED") + "。";
            }

            if (dt.Rows[0]["StartDate"] != json.Value<string>("STARTDATE"))
            {
                str += "开始时间：" + dt.Rows[0]["StartDate"] + "——>" + json.Value<string>("STARTDATE") + "。";
            }
            if (dt.Rows[0]["EndDate"] != json.Value<string>("ENDDATE"))
            {
                str += "停用时间：" + dt.Rows[0]["EndDate"] + "——>" + json.Value<string>("ENDDATE") + "。";
            }
            if (dt.Rows[0]["remark"] != json.Value<string>("REMARK"))
            {
                str += "备注：" + dt.Rows[0]["remark"] + "——>" + json.Value<string>("REMARK") + "。";
            }
            return str;
        }

    }
}