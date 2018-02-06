using System.Data;
using Newtonsoft.Json.Linq;

namespace Web_After.BasicManager.BasicManager
{
    public class Base_Code_Method
    {
        //获取修改记录
        public string getChange(DataTable dt,JObject json,string table)
        {
            string str = "";
            Switch_helper_Base_codename sc = new Switch_helper_Base_codename();

            if (dt.Rows[0][sc.getColum(table)] != json.Value<string>("NAME"))
            {
                str = "企业代码：" + oldInfo.InCode + "——>" + info.InCode + "。";
            }
        }
    }
}