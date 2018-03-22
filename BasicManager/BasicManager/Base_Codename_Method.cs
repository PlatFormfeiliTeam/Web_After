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


        public string getChangeBase_year2(DataTable dt, JObject json)
        {
            string str = "";
            if (dt.Rows[0]["NAME"] != json.Value<string>("NAME"))
            {
                str += "规则名称：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
            }
            if (dt.Rows[0]["CUSTOMAREA"].ToString() != json.Value<string>("CUSTOMAREA"))
            {
                str += "申报关区：" + dt.Rows[0]["CUSTOMAREA"] + "——>" + json.Value<string>("CUSTOMAREA") + "。";
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

        public string getChangeHsCode(DataTable dt,JObject json)
        {
            string str = "";
            if (dt.Rows[0]["HSCODE"].ToString() != json.Value<string>("HSCODE").ToString())
            {
                str += "HS编码：" + dt.Rows[0]["HSCODE"] + "——>" + json.Value<string>("HSCODE") + "。";
            }
            if (dt.Rows[0]["HSNAME"].ToString() != json.Value<string>("HSNAME").ToString())
            {
                str += "名称：" + dt.Rows[0]["HSNAME"] + "——>" + json.Value<string>("HSNAME") + "。";
            }
            if (dt.Rows[0]["LEGALUNIT"].ToString() != json.Value<string>("LEGALUNITNAME").ToString())
            {
                str += "法定单位：" + dt.Rows[0]["LEGALUNIT"] + "——>" + json.Value<string>("LEGALUNITNAME") + "。";
            }
            if (dt.Rows[0]["NUM"].ToString() != json.Value<string>("NUMNAME").ToString())
            {
                str += "数量：" + dt.Rows[0]["NUMNAME"] + "——>" + json.Value<string>("NUMNAME") + "。";
            }
            if (dt.Rows[0]["WEIGHT"].ToString() != json.Value<string>("WEIGHT").ToString())
            {
                str += "重量：" + dt.Rows[0]["WEIGHT"] + "——>" + json.Value<string>("WEIGHT") + "。";
            }
            if (dt.Rows[0]["CUSTOMREGULATORY"].ToString() != json.Value<string>("CUSTOMREGULATORY").ToString())
            {
                str += "海关监管：" + dt.Rows[0]["CUSTOMREGULATORY"] + "——>" + json.Value<string>("CUSTOMREGULATORY") + "。";
            }
            if (dt.Rows[0]["INSPECTIONREGULATORY"].ToString() != json.Value<string>("INSPECTIONREGULATORY").ToString())
            {
                str += "检验检疫：" + dt.Rows[0]["INSPECTIONREGULATORY"] + "——>" + json.Value<string>("INSPECTIONREGULATORY") + "。";
            }
            
            if (dt.Rows[0]["ENABLED"].ToString() != json.Value<string>("ENABLED").ToString())
            {
                str += "启用情况：" + dt.Rows[0]["ENABLED"] + "——>" + json.Value<string>("ENABLED") + "。";
            }
            if (dt.Rows[0]["StartDate"].ToString() != json.Value<string>("STARTDATE"))
            {
                str += "开始时间：" + dt.Rows[0]["StartDate"] + "——>" + json.Value<string>("STARTDATE") + "。";
            }
            if (dt.Rows[0]["EndDate"].ToString() != json.Value<string>("ENDDATE"))
            {
                str += "停用时间：" + dt.Rows[0]["EndDate"] + "——>" + json.Value<string>("ENDDATE") + "。";
            }
            if (dt.Rows[0]["remark"].ToString() != json.Value<string>("REMARK"))
            {
                str += "备注：" + dt.Rows[0]["remark"] + "——>" + json.Value<string>("REMARK") + "。";
            }
            return str;
        }

        public string getChangeHsCode2(DataTable dt, JObject json)
        {
            string str = "";
            if (dt.Rows[0]["HSCODE"].ToString() != json.Value<string>("HSCODE").ToString())
            {
                str += "HS编码：" + dt.Rows[0]["HSCODE"] + "——>" + json.Value<string>("HSCODE") + "。";
            }
            if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME").ToString())
            {
                str += "名称：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
            }
            if (dt.Rows[0]["LEGALUNIT"].ToString() != json.Value<string>("LEGALUNIT").ToString())
            {
                str += "法定单位：" + dt.Rows[0]["LEGALUNIT"] + "——>" + json.Value<string>("LEGALUNIT") + "。";
            }
            if (dt.Rows[0]["SECONDUNIT"].ToString() != json.Value<string>("SECONDUNIT").ToString())
            {
                str += "第二单位单位：" + dt.Rows[0]["SECONDUNIT"] + "——>" + json.Value<string>("SECONDUNIT") + "。";
            }
            if (dt.Rows[0]["EXTRACODE"].ToString() != json.Value<string>("EXTRACODE").ToString())
            {
                str += "附加码：" + dt.Rows[0]["EXTRACODE"] + "——>" + json.Value<string>("EXTRACODE") + "。";
            }
            if (dt.Rows[0]["EXTRACODE"].ToString() != json.Value<string>("EXTRACODE").ToString())
            {
                str += "附加码：" + dt.Rows[0]["EXTRACODE"] + "——>" + json.Value<string>("EXTRACODE") + "。";
            }
            if (dt.Rows[0]["CUSTOMREGULATORY"].ToString() != json.Value<string>("CUSTOMREGULATORY").ToString())
            {
                str += "海关监管：" + dt.Rows[0]["CUSTOMREGULATORY"] + "——>" + json.Value<string>("CUSTOMREGULATORY") + "。";
            }
            if (dt.Rows[0]["INSPECTIONREGULATORY"].ToString() != json.Value<string>("INSPECTIONREGULATORY").ToString())
            {
                str += "国检监管：" + dt.Rows[0]["INSPECTIONREGULATORY"] + "——>" + json.Value<string>("INSPECTIONREGULATORY") + "。";
            }
            if (dt.Rows[0]["GENERALRATE"].ToString() != json.Value<string>("GENERALRATE").ToString())
            {
                str += "一般税率：" + dt.Rows[0]["GENERALRATE"] + "——>" + json.Value<string>("GENERALRATE") + "。";
            }
            if (dt.Rows[0]["FAVORABLERATE"].ToString() != json.Value<string>("FAVORABLERATE").ToString())
            {
                str += "最惠税率：" + dt.Rows[0]["FAVORABLERATE"] + "——>" + json.Value<string>("FAVORABLERATE") + "。";
            }
            if (dt.Rows[0]["VATRATE"].ToString() != json.Value<string>("VATRATE").ToString())
            {
                str += "增值税率：" + dt.Rows[0]["VATRATE"] + "——>" + json.Value<string>("VATRATE") + "。";
            }
            if (dt.Rows[0]["EXPORTREBATRATE"].ToString() != json.Value<string>("EXPORTREBATRATE").ToString())
            {
                str += "出口退税率：" + dt.Rows[0]["EXPORTREBATRATE"] + "——>" + json.Value<string>("EXPORTREBATRATE") + "。";
            }
            if (dt.Rows[0]["TEMPRATE"].ToString() != json.Value<string>("TEMPRATE").ToString())
            {
                str += "暂定税率：" + dt.Rows[0]["TEMPRATE"] + "——>" + json.Value<string>("TEMPRATE") + "。";
            }
            if (dt.Rows[0]["CONSUMERATE"].ToString() != json.Value<string>("CONSUMERATE").ToString())
            {
                str += "消费税率：" + dt.Rows[0]["CONSUMERATE"] + "——>" + json.Value<string>("CONSUMERATE") + "。";
            }
            if (dt.Rows[0]["EXPORTRATE"].ToString() != json.Value<string>("EXPORTRATE").ToString())
            {
                str += "出口税率：" + dt.Rows[0]["EXPORTRATE"] + "——>" + json.Value<string>("EXPORTRATE") + "。";
            }
            if (dt.Rows[0]["TOPPRICE"].ToString() != json.Value<string>("TOPPRICE").ToString())
            {
                str += "最高价格：" + dt.Rows[0]["TOPPRICE"] + "——>" + json.Value<string>("TOPPRICE") + "。";
            }
            if (dt.Rows[0]["LOWPRICE"].ToString() != json.Value<string>("LOWPRICE").ToString())
            {
                str += "最低价格：" + dt.Rows[0]["LOWPRICE"] + "——>" + json.Value<string>("LOWPRICE") + "。";
            }
            if (dt.Rows[0]["SPECIALMARK"].ToString() != json.Value<string>("SPECIALMARK").ToString())
            {
                str += "特殊标志：" + dt.Rows[0]["SPECIALMARK"] + "——>" + json.Value<string>("SPECIALMARK") + "。";
            }
            if (dt.Rows[0]["ELEMENTS"].ToString() != json.Value<string>("ELEMENTS").ToString())
            {
                str += "申报要素：" + dt.Rows[0]["ELEMENTS"] + "——>" + json.Value<string>("ELEMENTS") + "。";
            }
            if (dt.Rows[0]["ENABLED"].ToString() != json.Value<string>("ENABLED").ToString())
            {
                str += "启用情况：" + dt.Rows[0]["ENABLED"] + "——>" + json.Value<string>("ENABLED") + "。";
            }
            if (dt.Rows[0]["StartDate"].ToString() != json.Value<string>("STARTDATE"))
            {
                str += "开始时间：" + dt.Rows[0]["StartDate"] + "——>" + json.Value<string>("STARTDATE") + "。";
            }
            if (dt.Rows[0]["EndDate"].ToString() != json.Value<string>("ENDDATE"))
            {
                str += "停用时间：" + dt.Rows[0]["EndDate"] + "——>" + json.Value<string>("ENDDATE") + "。";
            }
            if (dt.Rows[0]["remark"].ToString() != json.Value<string>("REMARK"))
            {
                str += "备注：" + dt.Rows[0]["remark"] + "——>" + json.Value<string>("REMARK") + "。";
            }
            return str;
        }

        //获取修改记录
        public string getBlendChange(DataTable dt, JObject json, string table)
        {
            string str = "";
            switch (table)
            {
                case "insp_portin":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "国内口岸代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "国内口岸名称：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
                    }
                    if (dt.Rows[0]["ENGLISHNAME"].ToString() != json.Value<string>("ENGLISHNAME"))
                    {
                        str += "英文名称：" + dt.Rows[0]["ENGLISHNAME"] + "——>" + json.Value<string>("ENGLISHNAME") + "。";
                    }
                    break;
                case "insp_portout":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "国际口岸代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "国际口岸名称：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
                    }
                    if (dt.Rows[0]["ENGLISHNAME"].ToString() != json.Value<string>("ENGLISHNAME"))
                    {
                        str += "英文名称：" + dt.Rows[0]["ENGLISHNAME"] + "——>" + json.Value<string>("ENGLISHNAME") + "。";
                    }
                    if (dt.Rows[0]["country"].ToString() != json.Value<string>("COUNTRYNAME"))
                    {
                        str += "所属国家：" + dt.Rows[0]["country"] + "——>" + json.Value<string>("COUNTRYNAME") + "。";
                    }
                    break;
                case "base_currency":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "币制代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "币制名称：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
                    }
                    if (dt.Rows[0]["abbreviation"].ToString() != json.Value<string>("ABBREVIATION"))
                    {
                        str += "币制缩写：" + dt.Rows[0]["ABBREVIATION"] + "——>" + json.Value<string>("ABBREVIATION") + "。";
                    }
                    break;
                case "base_inspcountry":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "国家代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "中文名：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
                    }
                    if (dt.Rows[0]["ENGLISHNAME"].ToString() != json.Value<string>("ENGLISHNAME"))
                    {
                        str += "英文名：" + dt.Rows[0]["ENGLISHNAME"] + "——>" + json.Value<string>("ENGLISHNAME") + "。";
                    }
                    if (dt.Rows[0]["AIRABBREV"].ToString() != json.Value<string>("AIRABBREV"))
                    {
                        str += "空运缩写：" + dt.Rows[0]["AIRABBREV"] + "——>" + json.Value<string>("AIRABBREV") + "。";
                    }
                    if (dt.Rows[0]["OCEANABBREV"].ToString() != json.Value<string>("OCEANABBREV"))
                    {
                        str += "海运缩写：" + dt.Rows[0]["OCEANABBREV"] + "——>" + json.Value<string>("OCEANABBREV") + "。";
                    }
                    break;
                case "base_productunit":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "计量单位代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "计量单位名称：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
                    }
                    if (dt.Rows[0]["ENGLISHNAME"].ToString() != json.Value<string>("ENGLISHNAME"))
                    {
                        str += "英文名称：" + dt.Rows[0]["ENGLISHNAME"] + "——>" + json.Value<string>("ENGLISHNAME") + "。";
                    }
                    break;
                case "base_insplicense":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "许可证代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["INNAME"].ToString() != json.Value<string>("INNAME"))
                    {
                        str += "进口许可证名称：" + dt.Rows[0]["INNAME"] + "——>" + json.Value<string>("INNAME") + "。";
                    }
                    if (dt.Rows[0]["OUTNAME"].ToString() != json.Value<string>("OUTNAME"))
                    {
                        str += "出口许可证名称：" + dt.Rows[0]["OUTNAME"] + "——>" + json.Value<string>("OUTNAME") + "。";
                    }
                    break;
                case "base_inspinvoice":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "随附单据代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["INNAME"].ToString() != json.Value<string>("INNAME"))
                    {
                        str += "进口随附单据名称：" + dt.Rows[0]["INNAME"] + "——>" + json.Value<string>("INNAME") + "。";
                    }
                    if (dt.Rows[0]["OUTNAME"].ToString() != json.Value<string>("OUTNAME"))
                    {
                        str += "出口随附单据名称：" + dt.Rows[0]["OUTNAME"] + "——>" + json.Value<string>("OUTNAME") + "。";
                    }
                    break;
                case "base_country":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "国家代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["EZM"].ToString() != json.Value<string>("EZM"))
                    {
                        str += "二字码：" + dt.Rows[0]["EZM"] + "——>" + json.Value<string>("EZM") + "。";
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "中文名：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
                    }
                    if (dt.Rows[0]["ENGLISHNAME"].ToString() != json.Value<string>("ENGLISHNAME"))
                    {
                        str += "英文名：" + dt.Rows[0]["ENGLISHNAME"] + "——>" + json.Value<string>("ENGLISHNAME") + "。";
                    }
                    if (dt.Rows[0]["RATE"].ToString() != json.Value<string>("RATE"))
                    {
                        str += "优/普税率：" + dt.Rows[0]["RATE"] + "——>" + json.Value<string>("RATE") + "。";
                    }
                    break;
                case "base_decltradeway":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "贸易方式代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "贸易方式简称：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
                    }
                    if (dt.Rows[0]["FULLNAME"].ToString() != json.Value<string>("FULLNAME"))
                    {
                        str += "贸易方式全称：" + dt.Rows[0]["FULLNAME"] + "——>" + json.Value<string>("FULLNAME") + "。";
                    }
                    break;
                case "base_exemptingnature":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "征免性质代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "征免性质简称：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
                    }
                    if (dt.Rows[0]["FULLNAME"].ToString() != json.Value<string>("FULLNAME"))
                    {
                        str += "征免性质全称：" + dt.Rows[0]["FULLNAME"] + "——>" + json.Value<string>("FULLNAME") + "。";
                    }
                    break;
                case "base_exchangeway":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "结汇方式代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "结汇方式简称：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
                    }
                    if (dt.Rows[0]["FULLNAME"].ToString() != json.Value<string>("FULLNAME"))
                    {
                        str += "结汇方式全称：" + dt.Rows[0]["FULLNAME"] + "——>" + json.Value<string>("FULLNAME") + "。";
                    }
                    break;
                case "base_harbour":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "港口代码：" + dt.Rows[0]["CODE"] + "——>" + json.Value<string>("CODE") + "。";
                    }
                    if (dt.Rows[0]["COUNTRY"].ToString() != json.Value<string>("COUNTRY"))
                    {
                        str += "所属国家：" + dt.Rows[0]["COUNTRY"] + "——>" + json.Value<string>("COUNTRY") + "。";
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "港口名称：" + dt.Rows[0]["NAME"] + "——>" + json.Value<string>("NAME") + "。";
                    }
                    if (dt.Rows[0]["ENGLISHNAME"].ToString() != json.Value<string>("ENGLISHNAME"))
                    {
                        str += "英文名称：" + dt.Rows[0]["ENGLISHNAME"] + "——>" + json.Value<string>("ENGLISHNAME") + "。";
                    }
                    break;
                case "base_booksdata":
                    if (dt.Rows[0]["TRADE"].ToString() != json.Value<string>("TRADE"))
                    {
                        str += "贸易方式代码:" + dt.Rows[0]["TRADE"].ToString() + "——>" + json.Value<string>("TRADE");
                    }
                    if (dt.Rows[0]["ISINPORTNAME"].ToString() != json.Value<string>("ISINPORTNAME"))
                    {
                        str += "贸易方式代码:" + dt.Rows[0]["ISINPORTNAME"].ToString() + "——>" + json.Value<string>("ISINPORTNAME");
                    }
                    if (dt.Rows[0]["ISPRODUCTNAME"].ToString() != json.Value<string>("ISPRODUCTNAME"))
                    {
                        str += "贸易方式代码:" + dt.Rows[0]["ISPRODUCTNAME"].ToString() + "——>" + json.Value<string>("ISPRODUCTNAME");
                    }

                    break;
                case "sys_repway":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "申报方式代码:" + dt.Rows[0]["CODE"].ToString() + "——>" + json.Value<string>("CODE");
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "申报方式名称:" + dt.Rows[0]["NAME"].ToString() + "——>" + json.Value<string>("NAME");
                    }
                    if (dt.Rows[0]["BUSITYPE"].ToString() != json.Value<string>("BUSITYPE"))
                    {
                        str += "申报方式名称:" + dt.Rows[0]["BUSITYPE"].ToString() + "——>" + json.Value<string>("BUSITYPE");
                    }
                    break;
                case "base_containersize":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "集装箱尺寸代码:" + dt.Rows[0]["CODE"].ToString() + "——>" + json.Value<string>("CODE");
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "集装箱尺寸:" + dt.Rows[0]["NAME"].ToString() + "——>" + json.Value<string>("NAME");
                    }
                    if (dt.Rows[0]["DECLSIZE"].ToString() != json.Value<string>("DECLSIZE"))
                    {
                        str += "集装箱申报尺寸:" + dt.Rows[0]["DECLSIZE"].ToString() + "——>" + json.Value<string>("DECLSIZE");
                    }
                    break;
                case "base_containertype":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "集装箱类型代码:" + dt.Rows[0]["CODE"].ToString() + "——>" + json.Value<string>("CODE");
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "集装箱类型:" + dt.Rows[0]["NAME"].ToString() + "——>" + json.Value<string>("NAME");
                    }
                    if (dt.Rows[0]["CONTAINERCODE"].ToString() != json.Value<string>("CONTAINERCODE"))
                    {
                        str += "集装箱编码:" + dt.Rows[0]["CONTAINERCODE"].ToString() + "——>" + json.Value<string>("CONTAINERCODE");
                    }
                    break;
                case "sys_woodpacking":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "木质包装代码:" + dt.Rows[0]["CODE"].ToString() + "——>" + json.Value<string>("CODE");
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "木质包装名称:" + dt.Rows[0]["NAME"].ToString() + "——>" + json.Value<string>("NAME");
                    }
                    if (dt.Rows[0]["HSCODE"].ToString() != json.Value<string>("HSCODE"))
                    {
                        str += "木质包装HS编码:" + dt.Rows[0]["HSCODE"].ToString() + "——>" + json.Value<string>("HSCODE");
                    }
                    break;
                case "sys_declarationcar":
                    if (dt.Rows[0]["LICENSE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "车牌号:" + dt.Rows[0]["LICENSE"].ToString() + "——>" + json.Value<string>("CODE");
                    }
                    if (dt.Rows[0]["WHITECARD"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "白卡号:" + dt.Rows[0]["WHITECARD"].ToString() + "——>" + json.Value<string>("NAME");
                    }
                    if (dt.Rows[0]["MODELS"].ToString() != json.Value<string>("MODELS"))
                    {
                        str += "车型:" + dt.Rows[0]["MODELS"].ToString() + "——>" + json.Value<string>("MODELS");
                    }
                    if (dt.Rows[0]["MOTORCADE"].ToString() != json.Value<string>("MOTORCADE"))
                    {
                        str += "车型:" + dt.Rows[0]["MOTORCADE"].ToString() + "——>" + json.Value<string>("MOTORCADE");
                    }
                    break;
                case "sys_reportlibrary":
                    if (dt.Rows[0]["CODE"].ToString() != json.Value<string>("CODE"))
                    {
                        str += "申报库别代码:" + dt.Rows[0]["CODE"].ToString() + "——>" + json.Value<string>("CODE");
                    }
                    if (dt.Rows[0]["NAME"].ToString() != json.Value<string>("NAME"))
                    {
                        str += "申报库别名称:" + dt.Rows[0]["NAME"].ToString() + "——>" + json.Value<string>("NAME");
                    }
                    if (dt.Rows[0]["DECLNAME"].ToString() != json.Value<string>("DECLNAME"))
                    {
                        str += "报关单名称:" + dt.Rows[0]["DECLNAME"].ToString() + "——>" + json.Value<string>("DECLNAME");
                    }
                    if (dt.Rows[0]["INTERNALTYPE"].ToString() != json.Value<string>("INTERNALTYPE"))
                    {
                        str += "进出口类型:" + dt.Rows[0]["INTERNALTYPE"].ToString() + "——>" + json.Value<string>("INTERNALTYPE");
                    }
                    break;

            }



            if (dt.Rows[0]["StartDate"].ToString() != json.Value<string>("STARTDATE"))
            {
                str += "开始时间：" + dt.Rows[0]["StartDate"] + "——>" + json.Value<string>("STARTDATE") + "。";
            }
            if (dt.Rows[0]["EndDate"].ToString() != json.Value<string>("ENDDATE"))
            {
                str += "停用时间：" + dt.Rows[0]["EndDate"] + "——>" + json.Value<string>("ENDDATE") + "。";
            }
            if (dt.Rows[0]["remark"].ToString() != json.Value<string>("REMARK"))
            {
                str += "备注：" + dt.Rows[0]["remark"] + "——>" + json.Value<string>("REMARK") + "。";
            }
            return str;
        }

    }
}