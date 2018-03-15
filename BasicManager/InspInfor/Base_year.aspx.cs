using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Web_After.BasicManager.BasicManager;
using Web_After.BasicManager.DeclInfor;
using Web_After.Sql;

namespace Web_After.BasicManager.InspInfor
{
    public partial class Base_year : System.Web.UI.Page
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
                    case "save":
                        save(Request["formdata"]);
                        break;
                    case "MaintainloadData":
                        MaintainloadData();
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

        private void loadDate()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["HsCodeBase"]))
            {
                strWhere = strWhere + " and t1.Name like '%" + Request["HsCodeBase"] + "%'";
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
            Sql.Base_year by = new Sql.Base_year();
            DataTable dt = by.LoaData(strWhere, "ID", "desc", ref totalProperty, Convert.ToInt32(Request["start"]), Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();

        }

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

        private void save(string formdata)
        {
            string response = String.Empty;
            Sql.Base_year by = new Sql.Base_year();
            Base_Codename_Method bcm = new Base_Codename_Method();
            //从前端获取值
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                //查询是否有重复值
                if (json.Value<string>("ENABLED") == "1")
                {
                    DataSet  dt = by.check_base_year(json);
                    if (dt.Tables[0].Rows.Count>0)
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
                        string content = bcm.getChangeBase_year(data, json);
                        int i = by.update_base_year(json);
                        if (i>0)
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
                    string content = bcm.getChangeBase_year(data, json);
                    int i = by.update_base_year(json);
                    if (i>0)
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

        //HS代码维护界面数据加载
        private void MaintainloadData()
        {
            string ID = Request["id"];
            string strWhere = string.Empty;
            Sql.Base_year by = new Sql.Base_year();

            if (!string.IsNullOrEmpty(Request["HsCodeSearch"]))
            {
                strWhere = strWhere + " and t1.hscode like '%" + Request["HsCodeSearch"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["HsNameSearch"]))
            {
                strWhere = strWhere + " and t1.hsname like '%" + Request["HsNameSearch"] + "%'";
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

        //新增,修改HS代码
        public void MaintainSave(string formdata)
        {
            Base_Codename_Method bcm = new Base_Codename_Method();

            string yearid = Request["ID"];
            //从前端获取值
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);

            string response = String.Empty;
            Sql.Base_year by = new Sql.Base_year();

            

            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                if (json.Value<string>("ENABLED")=="1")
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
                            string content = bcm.getChangeHsCode(getChanges, json);
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
                    if (i>0)
                    {                       
                        //获取修改的内容
                        string content = bcm.getChangeHsCode(getChanges, json);
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
            Sql.Base_year by = new Sql.Base_year();
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
                strWhere = strWhere + " and t1.hsname like '%" + Request["HsNameSearch"] + "%'";
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
            row1.CreateCell(3).SetCellValue("法定单位");
            row1.CreateCell(4).SetCellValue("数量");
            row1.CreateCell(5).SetCellValue("重量");
            row1.CreateCell(6).SetCellValue("海关监管");
            row1.CreateCell(7).SetCellValue("检验检疫");
            row1.CreateCell(8).SetCellValue("代码库");
            row1.CreateCell(9).SetCellValue("维护人");
            row1.CreateCell(10).SetCellValue("停用人");
            row1.CreateCell(11).SetCellValue("启用时间");
            row1.CreateCell(12).SetCellValue("停用时间");
            row1.CreateCell(13).SetCellValue("维护时间");
            row1.CreateCell(14).SetCellValue("启用情况");
            row1.CreateCell(15).SetCellValue("备注");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["HSCODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["EXTRACODE"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["HSNAME"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["LEGALUNITNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["NUMNAME"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["WEIGHT"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["CUSTOMREGULATORY"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["INSPECTIONREGULATORY"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["YEARNAME"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["CREATEMANNAME"].ToString());
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["STOPMANNAME"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["STARTDATE"].ToString());
                rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["ENDDATE"].ToString());
                rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["CREATEDATE"].ToString());
                rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["REMARK"].ToString());

            }
            try
            {
                // 输出Excel
                string filename = "HS代码.xls";
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
            Dictionary<int, List<int>> result = upload_base_company(newfile, fileName, action, json_formdata,yearid);

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

            Sql.Base_year by = new Sql.Base_year();
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
                //hs编码                                   //商品名称
                string HSCODE = insert_base_hscode[0]; string HSNAME = insert_base_hscode[1];
                //法定单位                                           //数量
                string LEGALUNITNAME = insert_base_hscode[2]; string NUMNAME = insert_base_hscode[3];
                //重量                                          //海关监管
                string WEIGHT = insert_base_hscode[4]; string CUSTOMREGULATORY = insert_base_hscode[5];
                //检验检疫
                string INSPECTIONREGULATORY = insert_base_hscode[6];
                //启用情况                                                     //备注
                string ENABLED = insert_base_hscode[7] == "是" ? "1" : "0"; string remark = insert_base_hscode[8];

                //启用日期
                string startdate = json_formdata.Value<string>("STARTDATE");
                //停用日期
                string enddate = json_formdata.Value<string>("ENDDATE");

                string formdata = "{\"HSCODE\":\"" + HSCODE + "\",\"HSNAME\":\"" + HSNAME + "\",\"LEGALUNITNAME\":\"" + LEGALUNITNAME + "\",\"NUMNAME\":\"" + NUMNAME + "\",\"WEIGHT\":\"" + WEIGHT + "\",\"CUSTOMREGULATORY\":\"" + CUSTOMREGULATORY + "\",\"INSPECTIONREGULATORY\":\"" + INSPECTIONREGULATORY + "\",\"ENABLED\":\"" + ENABLED + "\"," +
                                  "\"REMARK\":\"" + remark + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\"}";

                JObject json = (JObject)JsonConvert.DeserializeObject(formdata);

                if (by.check_repeat_base_insphs(json,yearid).Rows.Count > 0 || string.IsNullOrEmpty(HSCODE) || string.IsNullOrEmpty(HSNAME))
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