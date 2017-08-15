using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.CustomerMaintain
{
    public partial class CustomerManage : System.Web.UI.Page
    {
        IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
        int totalProperty = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string action = Request["action"];
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                switch (action)
                {
                    case "loadData":
                        loadData();
                        break;
                    case "delete":
                        deleteData();
                        break;
                    case "export":
                        exportData();
                        break;
                    case "save":
                        save(Request["formdata"]);
                        break;
                }
            }

        }
        /// <summary>
        /// 加载数据
        /// </summary>
        private void loadData()
        {
            string strWhere = string.Empty;

            if (!string.IsNullOrEmpty(Request["CODE_S"]))
            {
                strWhere = " and instr(code,'" + Request["CODE_S"] + "')>0";
            }
            if (!string.IsNullOrEmpty(Request["CNNAME_S"]))
            {
                strWhere = " and (instr(name,'" + Request["CNNAME_S"] + "')>0 or instr(chineseabbreviation, '" + Request["CNNAME_S"] + "')>0)";
            }
            if (!string.IsNullOrEmpty(Request["ENGLISHNAME_S"]))
            {
                strWhere = " and instr(englishname,'" + Request["ENGLISHNAME_S"] + "')>0";
            }
            if (!string.IsNullOrEmpty(Request["HSCODE_S"]))
            {
                strWhere = " and hscode='" + Request["HSCODE_S"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["CIQCODE_S"]))
            {
                strWhere = " and ciqcode='" + Request["CIQCODE_S"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = " and enabled='" + Request["ENABLED_S"] + "'";
            }
            string sql = "select * from cusdoc.sys_customer where 1=1 " + strWhere;
            sql = Extension.GetPageSql(sql, "ID", "desc", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            DataTable dt = DBMgr.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        private void deleteData()
        {
            string id = Request["ID"];
            string sql = "delete from cusdoc.sys_customer where id='{0}'";
            string str = DBMgr.ExecuteNonQuery(string.Format(sql, id)) > 0 ? "true" : "false";
            Response.Write("{\"success\":" + str + "}");
            Response.End();
        }
        private void exportData()
        {
            string sql = @"SELECT * FROM cusdoc.sys_customer order by id desc";
            DataTable dt = DBMgr.GetDataTable(sql);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("客商信息");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("代码");
            row1.CreateCell(1).SetCellValue("海关代码");
            row1.CreateCell(2).SetCellValue("国检代码");
            row1.CreateCell(3).SetCellValue("中文名称");
            row1.CreateCell(4).SetCellValue("中文简称");
            row1.CreateCell(5).SetCellValue("中文地址");
            row1.CreateCell(6).SetCellValue("英文名称");
            row1.CreateCell(7).SetCellValue("英文地址");
            row1.CreateCell(8).SetCellValue("是否启用");
            row1.CreateCell(9).SetCellValue("备注");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["HSCODE"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CIQCODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["NAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CHINESEABBREVIATION"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CHINESEADDRESS"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["ENGLISHNAME"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["ENGLISHADDRESS"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["REMARK"].ToString());
            }
            try
            {
                // 输出Excel
                string filename = "客商信息.xls";
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", Server.UrlEncode(filename)));
                Response.Clear();

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                Response.BinaryWrite(ms.GetBuffer());
                Response.End();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private void save(string formdata)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);

            string sql = "";
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                sql = @"insert into cusdoc.sys_customer(id
                                    ,code,name,chineseabbreviation,chineseaddress,hscode,ciqcode
                                    ,englishname,englishaddress,iscustomer,isshipper,iscompany
                                    ,logicauditflag,docservicecompany,enabled,remark,SOCIALCREDITNO
                                    ,TOOLVERSION
                                ) values(cusdoc.sys_customer_id.nextval
                                    ,'{0}','{1}','{2}', '{3}','{4}','{5}'
                                    ,'{6}','{7}','{8}','{9}','{10}'
                                    ,'{11}','{12}','{13}','{14}','{15}'
                                    ,'{16}')";
                sql = string.Format(sql
                        , json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("CHINESEABBREVIATION"), json.Value<string>("CHINESEADDRESS"), json.Value<string>("HSCODE"), json.Value<string>("CIQCODE")
                        , json.Value<string>("ENGLISHNAME"), json.Value<string>("ENGLISHADDRESS"), GetChk(json.Value<string>("ISCUSTOMER")), GetChk(json.Value<string>("ISSHIPPER")), GetChk(json.Value<string>("ISCOMPANY"))
                        , GetChk(json.Value<string>("LOGICAUDITFLAG")), GetChk(json.Value<string>("DOCSERVICECOMPANY")), json.Value<string>("ENABLED"), json.Value<string>("REMARK"), json.Value<string>("SOCIALCREDITNO")
                        , json.Value<string>("TOOLVERSION")
                    );
            }
            else
            {
                sql = @"update cusdoc.sys_customer set code='{0}',name='{1}',chineseabbreviation='{2}',chineseaddress='{3}',hscode='{4}',ciqcode='{5}'
                                    ,englishname='{6}',englishaddress='{7}',iscustomer='{8}',isshipper='{9}',iscompany='{10}'
                                    ,logicauditflag='{11}',docservicecompany='{12}',enabled='{13}',remark='{14}',SOCIALCREDITNO='{15}' 
                                    ,TOOLVERSION='{16}' 
                                where id={17}";
                sql = string.Format(sql
                        , json.Value<string>("CODE"), json.Value<string>("NAME"), json.Value<string>("CHINESEABBREVIATION"), json.Value<string>("CHINESEADDRESS"), json.Value<string>("HSCODE"), json.Value<string>("CIQCODE")
                        , json.Value<string>("ENGLISHNAME"), json.Value<string>("ENGLISHADDRESS"), GetChk(json.Value<string>("ISCUSTOMER")), GetChk(json.Value<string>("ISSHIPPER")), GetChk(json.Value<string>("ISCOMPANY"))
                        , GetChk(json.Value<string>("LOGICAUDITFLAG")), GetChk(json.Value<string>("DOCSERVICECOMPANY")), json.Value<int>("ENABLED"), json.Value<string>("REMARK"), json.Value<string>("SOCIALCREDITNO")
                        , json.Value<string>("TOOLVERSION"), json.Value<string>("ID")
                   );
            }

            int i = DBMgr.ExecuteNonQuery(sql);

            string response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
            Response.Write(response);
            Response.End();
        }
        public string GetChk(string check_val)
        {
            return check_val == "on" ? "1" : "0";
        }
    }
}