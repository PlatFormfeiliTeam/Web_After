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
using Web_After.Common;

namespace Web_After.BasicManager.InspInfor
{
    public partial class busi_UnitConvert : System.Web.UI.Page
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
                        loaddata();
                        break;
                    case "getCombox":
                        getCombox();
                        break;
                    case "save":
                        save();
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

        private void loaddata()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["CodeBase"]))
            {
                strWhere = strWhere + " and t1.unitcode1 like '%" + Request["CodeBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["RecordBase"]))
            {
                strWhere = strWhere + " and t4.name like '%" + Request["RecordBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }

            Sql.busi_UnitConvert bu = new Sql.busi_UnitConvert();
            DataTable dt =  bu.load(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
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

        public void getCombox()
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
                on t1.yearid = t4.id ";
            DataTable dt = DBMgrBase.GetDataTable(sql);
            string json_unit = JsonConvert.SerializeObject(dt,iso);
            Response.Write("{UNIT:" + json_unit + "}");
            Response.End();
        }

        public void save()
        {
            string formdata = Request["formdata"];
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            string response = String.Empty;
            Sql.busi_UnitConvert bu = new Sql.busi_UnitConvert();
            Base_Codename_Method bm = new Base_Codename_Method();
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                if (json.Value<string>("ENABLED") == "1")
                {
                    //需要判断
                    if (bu.check_repeat(json).Rows.Count > 0)
                    {
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        int i = bu.insert_table(json);
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }
                }
                else
                {
                    int i = bu.insert_table(json);
                    response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }
            }
            else
            {
                if (json.Value<string>("ENABLED") == "1")
                {
                    if (bu.check_update_repeat(json).Rows.Count > 0)
                    {
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        DataTable dt = bu.befor_change(json);
                        int i = bu.update_unitconvert(json);
                        if (i > 0)
                        {
                            string content = bm.getunitconvert(dt, json);
                            bu.insert_alert_data(json, content);
                            response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                        }
                    }
                }
                else
                {
                    DataTable dt = bu.befor_change(json);
                    int i = bu.update_unitconvert(json);
                    if (i > 0)
                    {
                        string content = bm.getunitconvert(dt, json);
                        bu.insert_alert_data(json, content);
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }
                }
            }

            Response.Write(response);
            Response.End();

        }


        private void export()
        {
            string strWhere = "";
            string combo_ENABLED_S2 = Request["combo_ENABLED_S"];
            if (combo_ENABLED_S2 == "null")
            {
                combo_ENABLED_S2 = String.Empty;
            }
            if (!string.IsNullOrEmpty(Request["CodeBase"]))
            {
                strWhere = strWhere + " and t1.unitcode1 like '%" + Request["CodeBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["RecordBase"]))
            {
                strWhere = strWhere + " and t4.name like '%" + Request["RecordBase"] + "%'";
            }

            if (!string.IsNullOrEmpty(combo_ENABLED_S2))
            {
                strWhere = strWhere + " and t1.enabled='" + combo_ENABLED_S2 + "'";
            }

            Sql.busi_UnitConvert bu = new Sql.busi_UnitConvert();
            DataTable dt = bu.export(strWhere);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("计算单位换算");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("计量单位代码1");
            row1.CreateCell(1).SetCellValue("计量单位名称1");
            row1.CreateCell(2).SetCellValue("转换率");
            row1.CreateCell(3).SetCellValue("计量单位代码2");
            row1.CreateCell(4).SetCellValue("计量单位名称2");
            row1.CreateCell(5).SetCellValue("启用/禁用");
            row1.CreateCell(6).SetCellValue("维护人");
            row1.CreateCell(7).SetCellValue("停用人");
            row1.CreateCell(8).SetCellValue("启用时间");
            row1.CreateCell(9).SetCellValue("停用时间");
            row1.CreateCell(10).SetCellValue("维护时间");
            row1.CreateCell(11).SetCellValue("备注");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["UNITCODE1"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["UNITNAME1"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CONVERTRATE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["UNITCODE2"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["UNITNAME2"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["CREATEMANNAME"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["STOPMANNAME"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["STARTDATE"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["ENDDATE"].ToString());
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CREATEDATE"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["REMARK"].ToString());
            }
            try
            {
                // 输出Excel
                string filename = "计量单位换算.xls";
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
            Base_Company_Method bcm = new Base_Company_Method();
            string formdata = Request["formdata"];
            string action = Request["action"];
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
            Dictionary<int, List<int>> result = upload_base_company(newfile, fileName, action, json_formdata);

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

        public Dictionary<int, List<int>> upload_base_company(string newfile, string fileName, string action,
            JObject json_formdata)
        {
            Base_Company_Method bcm = new Base_Company_Method();
            Switch_helper_Base_blend_web sw = new Switch_helper_Base_blend_web();
            DataTable dtExcel = bcm.GetExcelData_Table(Server.MapPath(newfile), 0);
            List<string> stringList = new List<string>();

            //记住发生错误的行数
            List<int> errorlines = new List<int>();

            //记住插入成功的个数
            int count = 0;

            //Sql.busi_RecordInfor bc = new Sql.busi_RecordInfor();
            //Sql.busi_SpecialHsConvernet bsh = new Sql.busi_SpecialHsConvernet();
            Sql.busi_UnitConvert bu = new Sql.busi_UnitConvert();
            //插入成功的个数(返回放入dictionary)
            List<int> successinsert = new List<int>();
            //返回值
            Dictionary<int, List<int>> returndic = new Dictionary<int, List<int>>();
            for (int i = 0; i < dtExcel.Rows.Count; i++)
            {

                for (int j = 0; j < dtExcel.Columns.Count; j++)
                {
                    stringList.Add(dtExcel.Rows[i][j].ToString());
                }
                //计量单位1                           
                string UNITNAME1 = stringList[0];
                string UNITCODE1 = bu.getUnitName(UNITNAME1).Rows[0]["CODE"].ToString();
                //计量单位2
                string UNITNAME2 = stringList[1];
                string UNITCODE2 = bu.getUnitName(UNITNAME2).Rows[0]["CODE"].ToString();
                //转换率
                string CONVERTRATE = stringList[2];
                //是否启用
                string ENABLED = stringList[3] == "是" ? "1" : "0";
                //备注
                string REMARK = stringList[4];
                //启用日期
                string startdate = json_formdata.Value<string>("STARTDATE");
                //停用日期
                string enddate = json_formdata.Value<string>("ENDDATE");

                string formdata = "{\"UNITCODE1\":\"" + UNITCODE1 + "\",\"UNITCODE2\":\"" + UNITCODE2 + "\",\"ENABLED\":\"" + ENABLED + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\"," +
                                  "\"CONVERTRATE\":\"" + CONVERTRATE + "\",\"REMARK\":\"" + REMARK + "\"}";
                JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
                if (bu.check_repeat(json).Rows.Count > 0 || string.IsNullOrEmpty(json.Value<string>("UNITCODE1")))
                {
                    errorlines.Add(i + 2);
                }
                else
                {
                    bu.insert_table(json);
                    count = count + 1;
                }
                stringList.Clear();
            }
            successinsert.Add(count);
            returndic.Add(1, successinsert);
            returndic.Add(2, errorlines);
            return returndic;
        }

    }
}