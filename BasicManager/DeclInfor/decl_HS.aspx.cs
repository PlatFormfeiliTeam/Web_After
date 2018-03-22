using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Web_After.BasicManager.BasicManager;
using Web_After.Common;

namespace Web_After.BasicManager.DeclInfor
{
    public partial class decl_HS : System.Web.UI.Page
    {
        IsoDateTimeConverter iso = new IsoDateTimeConverter();
        int totalProperty = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string action = Request["action"];
                iso.DateTimeFormat = "yyyy-MM-dd";
                switch (action)
                {
                    case "loadData":
                        loadDate();
                        break;
                    case "getYear":
                        getYear();
                        break;
                    case "save":
                        save(Request["formdata"]);
                        break;
                    case "MaintainloadData":
                        MaintainloadData();
                        break;
                    case "getUnit":
                        getUnit();
                        break;
                    case "MaintainSave":
                        MaintainSave(Request["formdata"]);
                        break;
                    case "export":
                        export();
                        break;
                    case "add":
                        ImportExcelData();
                        break;
                }
            }
        }

        //获取维护人的姓名
        public string Username()
        {
            string userName = "";
            FormsIdentity identity = User.Identity as FormsIdentity;
            if (identity == null)
            {
                return "";
            }
            return userName = identity.Name;
        }

        private void loadDate()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["CiqCodeBase"]))
            {
                strWhere = strWhere + " and t1.Name like '%" + Request["CiqCodeBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["start_date"]))
            {
                strWhere = strWhere + " and t1.startDate >= to_date('" + Request["start_date"] + "','yyyy-mm-dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(Request["end_date"]))
            {
                strWhere = strWhere + " and t1.endDate < to_date('" + Request["end_date"] + "','yyyy-mm-dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.decl_HS bc = new Sql.decl_HS();
            DataTable dt = bc.LoaData(strWhere, "ID", "desc", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }

        public void getYear()
        {
            string sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname,t4.name as yearname from BASE_CUSTOMDISTRICT t1 left join sys_user t2 on t1.createman=t2.id 
            left join sys_user t3 on t1.stopman=t3.id left join base_year t4 on t1.yearid=t4.id where 1=1 ";
            DataTable dt = DBMgrBase.GetDataTable(sql);
            string  json = JsonConvert.SerializeObject(dt, iso);
            Response.Write(json);
            Response.End();
        }

        public void getUnit()
        {
            string sql = @"select t1.*,
                           t2.name as createmanname,
                           t3.name as stopmanname,
                           t4.name as yearname
                        from base_declproductunit t1
                      left join sys_user t2
                        on t1.createman = t2.id
                      left join sys_user t3
                        on t1.stopman = t3.id
                      left join base_year t4
                        on t1.yearid = t4.id
                        where t1.enabled = '1' ";
            DataTable dt = DBMgrBase.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write(json);
            Response.End();
        }

        public void save(string formdata)
        {
            string response = String.Empty;
            Sql.decl_HS by = new Sql.decl_HS();
            Base_Codename_Method bcm = new Base_Codename_Method();
            //从前端获取值
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                //查询是否有重复值
                if (json.Value<string>("ENABLED") == "1")
                {
                    DataSet dt = by.check_base_year(json);
                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        //当数据有重复时success返回值为4
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        int i = by.insertTable(json);
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }

                }
                else
                {

                    int i = by.insertTable(json);
                    response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }
            }
            else
            {
                //查询是否有重复值
                if (json.Value<string>("ENABLED") == "1")
                {
                    DataSet dt = by.check_base_year_by_idandname(json);
                    if (dt.Tables[0].Rows.Count > 0)
                    {
                        //当数据有重复时success返回值为4
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        DataTable data = by.getBeforeChangData(json);
                        string content = bcm.getChangeBase_year2(data, json);
                        int i = by.update_base_year(json);
                        if (i > 0)
                        {
                            //插入修改记录
                            by.insert_base_alterrecord(json, content);
                        }
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }
                }
                else
                {
                    DataTable data = by.getBeforeChangData(json);
                    string content = bcm.getChangeBase_year2(data, json);
                    int i = by.update_base_year(json);
                    if (i > 0)
                    {
                        //插入修改记录
                        by.insert_base_alterrecord(json, content);
                    }
                    response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }
            }

            Response.Write(response);
            Response.End();
        }

        private void MaintainloadData()
        {
            string ID = Request["id"];
            string strWhere = string.Empty;
            Sql.decl_HS by = new Sql.decl_HS();

            if (!string.IsNullOrEmpty(Request["HsCodeSearch"]))
            {
                strWhere = strWhere + " and t1.hscode like '%" + Request["HsCodeSearch"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["HsNameSearch"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["HsNameSearch"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S2"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S2"] + "'";
            }
            DataTable dt = by.MaintainloadData(ID, strWhere, "", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }


        public void MaintainSave(string formdata)
        {
            Base_Codename_Method bcm = new Base_Codename_Method();

            string yearid = Request["ID"];
            //从前端获取值
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);

            string response = String.Empty;
            Sql.decl_HS by = new Sql.decl_HS();



            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                if (json.Value<string>("ENABLED") == "1")
                {
                    //查询是否有重复值
                    DataTable dt = by.check_repeat_base_insphs(json, yearid);
                    if (dt.Rows.Count > 0)
                    {
                        //当数据有重复时success返回值为4
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        int i = by.insert_base_insphs(json, yearid);
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }

                }
                else
                {
                    int i = by.insert_base_insphs(json, yearid);
                    response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }
            }
            else
            {
                if (json.Value<string>("ENABLED") == "1")
                {
                    DataTable dt = by.check_repeat_base_insphs_update(json, yearid);
                    //判断是否有重复值
                    if (dt.Rows.Count > 0)
                    {
                        //当数据有重复时success返回值为4
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {

                        //获取修改之前的记录
                        DataTable getChanges = by.Before_Change(json);
                        int i = by.update_base_insphs(json, yearid);
                        if (i > 0)
                        {
                            //获取修改的内容
                            string content = bcm.getChangeHsCode2(getChanges, json);
                            by.saveChangeBaseHsCode(json, content);
                            response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                        }

                    }

                }
                else
                {
                    //获取修改之前的记录
                    DataTable getChanges = by.Before_Change(json);
                    int i = by.update_base_insphs(json, yearid);
                    if (i > 0)
                    {
                        //获取修改的内容
                        string content = bcm.getChangeHsCode2(getChanges, json);
                        by.saveChangeBaseHsCode(json, content);
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }
                }
            }
            Response.Write(response);
            Response.End();
        }



        //导出
        public void export()
        {
            Sql.decl_HS by = new Sql.decl_HS();
            string strWhere = string.Empty;
            string yearid = Request["id"];
            string combo_ENABLED_S2 = Request["combo_ENABLED_S2"];
            if (combo_ENABLED_S2 == "null")
            {
                combo_ENABLED_S2 = String.Empty;
            }
            if (!string.IsNullOrEmpty(Request["HsCodeSearch"]))
            {
                strWhere = strWhere + " and t1.hscode like '%" + Request["HsCodeSearch"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["HsNameSearch"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["HsNameSearch"] + "%'";
            }
            if (!string.IsNullOrEmpty(combo_ENABLED_S2))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S2"] + "'";
            }
            DataTable dt = by.export_table(yearid, strWhere);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("HS代码");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("HS编码");
            row1.CreateCell(1).SetCellValue("附加码");
            row1.CreateCell(2).SetCellValue("商品名称");
            row1.CreateCell(3).SetCellValue("申报要素");
            row1.CreateCell(4).SetCellValue("法定单位");
            row1.CreateCell(5).SetCellValue("第二单位");
            row1.CreateCell(6).SetCellValue("所属类别");
            row1.CreateCell(7).SetCellValue("所属章节");
            row1.CreateCell(8).SetCellValue("所属小类");
            row1.CreateCell(9).SetCellValue("海关监管");
            row1.CreateCell(10).SetCellValue("商检监管");
            row1.CreateCell(11).SetCellValue("一般税率");
            row1.CreateCell(12).SetCellValue("最惠税率");
            row1.CreateCell(13).SetCellValue("增值税率");
            row1.CreateCell(14).SetCellValue("出口退税率");
            row1.CreateCell(15).SetCellValue("最高价格");
            row1.CreateCell(16).SetCellValue("最低价格");
            row1.CreateCell(17).SetCellValue("协定税文件");
            row1.CreateCell(18).SetCellValue("代码库");
            row1.CreateCell(19).SetCellValue("特殊标志");
            row1.CreateCell(20).SetCellValue("启用情况");
            row1.CreateCell(21).SetCellValue("启用时间");
            row1.CreateCell(22).SetCellValue("维护人");
            row1.CreateCell(23).SetCellValue("维护时间");
            row1.CreateCell(24).SetCellValue("停用人");
            row1.CreateCell(25).SetCellValue("停用时间");
            row1.CreateCell(26).SetCellValue("备注");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["HSCODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["EXTRACODE"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["NAME"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["ELEMENTS"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["LEGALUNITNAME"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["SECONDUNITNAME"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["TYPENAME"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["CHAPTERNAME"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["CLASSNAME"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["CUSTOMREGULATORY"].ToString());
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["INSPECTIONREGULATORY"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["GENERALRATE"].ToString());
                rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["FAVORABLERATE"].ToString());
                rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["VATRATE"].ToString());
                rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["EXPORTREBATRATE"].ToString());
                rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["TOPPRICE"].ToString());
                rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["LOWPRICE"].ToString());
                rowtemp.CreateCell(17).SetCellValue(dt.Rows[i]["AGREETAXFILE"].ToString());
                rowtemp.CreateCell(18).SetCellValue(dt.Rows[i]["YEARNAME"].ToString());
                rowtemp.CreateCell(19).SetCellValue(dt.Rows[i]["SPECIALMARK"].ToString());
                rowtemp.CreateCell(20).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(21).SetCellValue(dt.Rows[i]["STARTDATE"].ToString());
                rowtemp.CreateCell(22).SetCellValue(dt.Rows[i]["CREATEMANNAME"].ToString());
                rowtemp.CreateCell(23).SetCellValue(dt.Rows[i]["CREATEDATE"].ToString());
                rowtemp.CreateCell(24).SetCellValue(dt.Rows[i]["STOPMANNAME"].ToString());
                rowtemp.CreateCell(25).SetCellValue(dt.Rows[i]["ENDDATE"].ToString());
                rowtemp.CreateCell(26).SetCellValue(dt.Rows[i]["REMARK"].ToString());

            }
            try
            {
                // 输出Excel
                string filename = "商品HS编码.xls";
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

        //导入
        private void ImportExcelData()
        {
            //获取yearid
            string yearid = Request["id"];
            Base_Company_Method bcm = new Base_Company_Method();
            string formdata = Request["formdata"]; string action = Request["action"];
            JObject json_formdata = (JObject)JsonConvert.DeserializeObject(formdata);
            string reponseresult = "";
            HttpPostedFile postedFile = Request.Files["UPLOADFILE"];//获取上传信息对象  
            string fileName = Path.GetFileName(postedFile.FileName);
            if (!Directory.Exists("/FileUpload/PreData"))
            {
                Directory.CreateDirectory("/FileUpload/PreData");
            }
            string newfile = @"/FileUpload/PreData/" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + fileName;
            postedFile.SaveAs(Server.MapPath(newfile));
            Dictionary<int, List<int>> result = upload_base_company(newfile, fileName, action, json_formdata, yearid);

            List<int> succInts = result[1];
            List<int> errorInts = result[2];
            string errorStr = "";
            for (int i = 0; i < errorInts.Count; i++)
            {
                errorStr = errorStr + errorInts[i] + ",";
            }

            //返回失败信息
            string responseerrorlist = "";
            //返回成功信息
            string responsesuccesslist = "";


            if (errorInts.Count > 0)
            {
                responseerrorlist = "插入失败的行数为：" + errorStr;
            }

            if (succInts.Count > 0)
            {
                responsesuccesslist = "成功插入" + succInts[0] + "行!";
            }
            reponseresult = responsesuccesslist + responseerrorlist;

            string response = "{\"success\":\"" + reponseresult + "\"}";
            Response.Write(response);
            Response.End();
        }

        public Dictionary<int, List<int>> upload_base_company(string newfile, string fileName, string action, JObject json_formdata, string yearid)
        {
            Base_Company_Method bcm = new Base_Company_Method();
            DataTable dtExcel = bcm.GetExcelData_Table(Server.MapPath(newfile), 0);

            Sql.decl_HS by = new Sql.decl_HS();
            //判断是否有重复的ciq代码
            //excel中得到的数据是：CIQ代码，CIQ中文名，启用情况，备注

            List<string> insert_base_hscode = new List<string>();

            //记住成功插入的条数
            int countsuccess = 0;

            //记住失败的行数
            List<int> errorlines = new List<int>();

            //记住成功的个数
            List<int> successInts = new List<int>();

            //返回值
            Dictionary<int, List<int>> retundDictionary = new Dictionary<int, List<int>>();
            for (int i = 0; i < dtExcel.Rows.Count; i++)
            {

                for (int j = 0; j < dtExcel.Columns.Count; j++)
                {
                    insert_base_hscode.Add(dtExcel.Rows[i][j].ToString());
                }
                //hs编码                                   //附加码
                string HSCODE = insert_base_hscode[0]; string EXTRACODE = insert_base_hscode[1];
                //商品名称                                           //申报要素
                string NAME = insert_base_hscode[2]; string ELEMENTS = insert_base_hscode[3];
                //法定单位                                          //第二单位
                string LEGALUNIT = insert_base_hscode[4]; string SECONDUNIT = insert_base_hscode[5];
                //海关监管
                string CUSTOMREGULATORY = insert_base_hscode[6];
                //商检监管
                string INSPECTIONREGULATORY = insert_base_hscode[7];
                //一般税率
                string GENERALRATE = insert_base_hscode[8];
                //最惠税率
                string FAVORABLERATE = insert_base_hscode[9];
                //增值税率
                string VATRATE = insert_base_hscode[10];
                //出口退税率
                string EXPORTREBATRATE = insert_base_hscode[11];
                //暂定税率
                string TEMPRATE = insert_base_hscode[12];
                //消费税率
                string CONSUMERATE = insert_base_hscode[13];
                //出口税率
                string EXPORTRATE = insert_base_hscode[14];
                //最高价格
                string TOPPRICE = insert_base_hscode[15];
                //最低价格
                string LOWPRICE = insert_base_hscode[16];
                //特殊标志
                string SPECIALMARK = insert_base_hscode[17];                                
                //启用情况                                                     
                string ENABLED = insert_base_hscode[18] == "是" ? "1" : "0"; 
                //备注
                string REMARK = insert_base_hscode[19];
                //启用日期
                string startdate = json_formdata.Value<string>("STARTDATE");
                //停用日期
                string enddate = json_formdata.Value<string>("ENDDATE");

                string formdata = "{\"HSCODE\":\"" + HSCODE + "\",\"NAME\":\"" + NAME + "\",\"LEGALUNIT\":\"" + LEGALUNIT + "\",\"EXTRACODE\":\"" + EXTRACODE + "\",\"ELEMENTS\":\"" + ELEMENTS + "\",\"CUSTOMREGULATORY\":\"" + CUSTOMREGULATORY + "\",\"INSPECTIONREGULATORY\":\"" + INSPECTIONREGULATORY + "\",\"ENABLED\":\"" + ENABLED + "\"," +
                                  "\"REMARK\":\"" + REMARK + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\"," +
                                  "\"SECONDUNIT\":\"" + SECONDUNIT + "\",\"GENERALRATE\":\"" + GENERALRATE + "\",\"FAVORABLERATE\":\"" + FAVORABLERATE + "\"," +
                                  "\"VATRATE\":\"" + VATRATE + "\",\"EXPORTREBATRATE\":\"" + EXPORTREBATRATE + "\",\"TEMPRATE\":\"" + TEMPRATE + "\"," +
                                  "\"CONSUMERATE\":\"" + CONSUMERATE + "\",\"EXPORTRATE\":\"" + EXPORTRATE + "\",\"TOPPRICE\":\"" + TOPPRICE + "\"," +
                                  "\"LOWPRICE\":\"" + LOWPRICE + "\",\"SPECIALMARK\":\"" + SPECIALMARK + "\"}";

                JObject json = (JObject)JsonConvert.DeserializeObject(formdata);

                if (by.check_repeat_base_insphs(json, yearid).Rows.Count > 0 || string.IsNullOrEmpty(HSCODE) || string.IsNullOrEmpty(NAME))
                {
                    errorlines.Add(i + 2);
                }
                else
                {
                    by.insert_base_insphs(json, yearid);
                    countsuccess = countsuccess + 1;
                }

                insert_base_hscode.Clear();
            }

            successInts.Add(countsuccess);
            retundDictionary.Add(1, successInts);
            retundDictionary.Add(2, errorlines);
            return retundDictionary;

        }

    }
}