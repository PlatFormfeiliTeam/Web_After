using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_After.model
{
    public class RecordDetailEn
    {

        public string ITEMNO { get; set; }
        //HS编号                                        
        public string HSCODE { get; set; }
        //HS附加码
        public string ADDITIONALNO { get; set; }
        //项号属性
        public string ITEMNOATTRIBUTE { get; set; }
        //商品名称
        public string COMMODITYNAME { get; set; }
        //料号
        public string PARTNO { get; set; }
        //规格型号
        public string SPECIFICATIONSMODEL { get; set; }
        //成交单位名称
        public string UNIT { get; set; }
        //版本号
        public string VERSION { get; set; }
        //是否启用
        public string ENABLED { get; set; }
        //备注
        public string REMARK { get; set; }

        //启用日期
        public string startdate { get; set; }
        //停用日期
        public string enddate { get; set; }
        public RecordDetailEn(List<string> stringList, JObject json_formdata)
        {
            ITEMNO = stringList[0];
            //HS编号                                        
            HSCODE = stringList[1];
            //HS附加码
            ADDITIONALNO = stringList[2];
            //项号属性
            ITEMNOATTRIBUTE = stringList[3];
            //商品名称
            COMMODITYNAME = stringList[4];
            //料号
            PARTNO = stringList[5];
            //规格型号
            SPECIFICATIONSMODEL = stringList[6];
            //成交单位名称
            UNIT = stringList[7];
            //版本号
            VERSION = stringList[8];
            //是否启用
            if (stringList.Count > 9) ENABLED = stringList[9] == "是" ? "1" : "0";
            //备注
            if (stringList.Count > 10) REMARK = stringList[10];

            //启用日期
            startdate = json_formdata.Value<string>("STARTDATE");
            //停用日期
            enddate = json_formdata.Value<string>("ENDDATE");
        }
    }

}