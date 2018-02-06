using System;
using System.Collections.Generic;
using System.Data;
using Aspose.Cells;
using iTextSharp.text;
using Newtonsoft.Json.Linq;
using Web_After.Common;

namespace Web_After.BasicManager.DeclInfor
{
    public class Base_Company_Method
    {
        //判断内部编码,海关编码,社会信用代码是否有重复
        public List<int> CheckRepeat(string id,string incode, string code, string socialcreditno)
        {
            string strWhere = String.Empty;
            if (string.IsNullOrEmpty(id))
            {
                strWhere = "";
            }
            else
            {
                strWhere = "and id not in('"+id+"')";
            }
            List<int> addList = new List<int>();
            Sql.Base_Company bc = new Sql.Base_Company();
            //内部编码重复返回值为1
            if (bc.check_incode_repeat(incode,strWhere).Rows.Count > 0)
            {
                 addList.Add(1);
            }
            //海关编码重复返回值为2
            if (bc.check_code_repeat(code,strWhere).Rows.Count > 0)
            {                
                addList.Add(2);
            }
            //社会信用代码重复返回值为3
            if (!string.IsNullOrEmpty(socialcreditno))
            {
                if (bc.check_socialcreditno_repeat(socialcreditno, strWhere).Rows.Count > 0)
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
                    repeat = repeat + "内部编码重复,";
                }
                if (retunRepeat[i] == 2)
                {
                    repeat = repeat + "海关编码重复,";
                }
                if (retunRepeat[i] == 3)
                {
                    repeat = repeat + "社会信用代码重复,";
                }

            }
            return repeat;

        }


        //上传
        public DataTable GetExcelData_Table(string filePath, int sheetPoint)
        {
            Workbook book = new Workbook(filePath);
            //book.Open(filePath);
            Worksheet sheet = book.Worksheets[sheetPoint];
            Cells cells = sheet.Cells;
            DataTable dt_Import = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);//获取excel中的数据保存到一个datatable中
            return dt_Import;

        }

        //获取conten
        public string getChange(DataTable dt,JObject json)
        {
            string str = "";
            //if (info.InCode != oldInfo.InCode)
            //{
            //    str = "企业代码：" + oldInfo.InCode + "——>" + info.InCode + "。";
            //}

            if (dt.Rows[0]["name"] != json.Value<string>("NAME"))
            {
                str += "企业名称：" + dt.Rows[0]["name"] + "——>" + json.Value<string>("NAME") + "。";
            }
            if (dt.Rows[0]["englishname"] != json.Value<string>("ENGLISHNAME"))
            {
                str += "英文名称：" + dt.Rows[0]["englishname"] + "——>" + json.Value<string>("ENGLISHNAME") + "。";
            }
            if (dt.Rows[0]["declnature"] != json.Value<string>("DECLNATURENAME"))
            {
                str += "海关性质：" + dt.Rows[0]["declnature"] + "——>" + json.Value<string>("DECLNATURENAME") + "。";
            }
            if (dt.Rows[0]["inspnature"] != json.Value<string>("INSPNATURENAME"))
            {
                str += "商检性质：" + dt.Rows[0]["inspnature"] + "——>" + json.Value<string>("INSPNATURENAME") + "。";
            }
            if (dt.Rows[0]["GoodsLocal"] != json.Value<string>("GOODSLOCAL"))
            {
                str += "货物存放地：" + dt.Rows[0]["GoodsLocal"] + "——>" + json.Value<string>("GOODSLOCAL") + "。";
            }
            if (dt.Rows[0]["receivertype"] != json.Value<string>("RECEIVERTYPE"))
            {
                str += "收货人类型：" + dt.Rows[0]["receivertype"] + "——>" + json.Value<string>("RECEIVERTYPE") + "。";
            }
            if (dt.Rows[0]["code"] != json.Value<string>("CODE"))
            {
                str += "报关代码：" + dt.Rows[0]["code"] + "——>" + json.Value<string>("CODE") + "。";
            }
            if (dt.Rows[0]["inspcode"] != json.Value<string>("INSPCODE"))
            {
                str += "报检代码：" + dt.Rows[0]["inspcode"] + "——>" + json.Value<string>("INSPCODE") + "。";
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
            if (dt.Rows[0]["SOCIALCREDITNO"] != json.Value<string>("SOCIALCREDITNO"))
            {
                str += "社会信用代码：" + dt.Rows[0]["SOCIALCREDITNO"] + "——>" + json.Value<string>("SOCIALCREDITNO") + "。";
            }
            return str;

        }

        
    }
}