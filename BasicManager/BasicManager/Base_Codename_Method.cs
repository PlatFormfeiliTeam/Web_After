using System.Data;
using Newtonsoft.Json.Linq;

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

        


    }
}