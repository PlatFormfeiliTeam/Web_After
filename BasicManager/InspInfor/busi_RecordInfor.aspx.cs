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
using Web_After.model;

namespace Web_After.BasicManager.InspInfor
{
    public partial class busi_RecordInfor : System.Web.UI.Page
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
                        loadData();
                        break;
                    case "getCheckBoxData":
                        getCheckBoxData();
                        break;
                    case "save":
                        save(Request["formdata"]);
                        break;
                    case "export":
                        export();
                        break;
                    case "add":
                        ImportExcelData();
                        break;
                    case "loaddatamaterials":
                        loaddatamaterials();
                        break;
                    case "loaddataproduct":
                        loaddataproduct();
                        break;
                    case "getRecordDetails":
                        getRecordDetails();
                        break;
                    case "judge":
                        getRecordDetails_Hscode();
                        break;
                    case "save_recorddetails":
                        save_recorddetails();
                        break;
                    case "export_recorddetails":
                        export_recorddetails();
                        break;
                    case "add_recorddetails":
                        ImportExcelData_RecordDeatils();
                        break;

                }
            }
        }

        private void loadData()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["CodeBase"]))
            {
                strWhere = strWhere + " and t1.busiunit like '%" + Request["CodeBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["RecordBase"]))
            {
                strWhere = strWhere + " and t1.code like '%" + Request["RecordBase"] + "%'";
            }

            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S"] + "'";
            }
            Sql.busi_RecordInfor bc = new Sql.busi_RecordInfor();
            DataTable dt = bc.LoaData(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }


        //获取经营单位等下拉数据
        private void getCheckBoxData()
        {
            string sql = String.Empty;
            string jsonEXEMPTING = String.Empty;
            string jsonTRADEWAY = String.Empty;
            string jsonCOMPANY = String.Empty;
            string json = String.Empty;
            //征免性质
            sql = @"select t1.*,
                   t2.name as createmanname,
                   t3.name as stopmanname,
                   t4.name as yearname
              from base_exemptingnature t1
              left join sys_user t2
                on t1.createman = t2.id
              left join sys_user t3
                on t1.stopman = t3.id
              left join base_year t4
                on t1.yearid = t4.id";
            DataTable EXEMPTING = DBMgrBase.GetDataTable(sql);
            jsonEXEMPTING = JsonConvert.SerializeObject(EXEMPTING, iso);
            //贸易方式
            sql = @"select t1.*, t2.name as createmanname, t3.name as stopmanname
              from base_decltradeway t1
              left join sys_user t2
                on t1.createman = t2.id
              left join sys_user t3
                on t1.stopman = t3.id";
            DataTable tradeway = DBMgrBase.GetDataTable(sql);
            jsonTRADEWAY = JsonConvert.SerializeObject(tradeway,iso);
            //经营单位，收发货单位
            sql = @"select t1.*,t2.name as createmanname,t3.name as stopmanname from base_company t1 left join sys_user t2 on t1.createman=t2.id 
            left join sys_user t3 on t1.stopman=t3.id ";
            DataTable company = DBMgrBase.GetDataTable(sql);
            jsonCOMPANY = JsonConvert.SerializeObject(company, iso);
            Response.Write("{EXEMPTING:" + jsonEXEMPTING + ",TRADEWAY:" + jsonTRADEWAY + ",COMPANY:" + jsonCOMPANY + "}");
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
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            Sql.busi_RecordInfor br = new Sql.busi_RecordInfor();
            string response = String.Empty;
            Base_Codename_Method bm = new Base_Codename_Method();
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                //查询是否有重复值
                if (json.Value<string>("ENABLED") == "1")
                {
                    if (br.check_sys_recordinfo(json).Tables[0].Rows.Count > 0)
                    {
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        int i = br.insert_sys_recordinfo(json);
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }
                }
                else
                {
                    int i = br.insert_sys_recordinfo(json);
                    response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }
            }
            else
            {
                //更新
                if (json.Value<string>("ENABLED") == "1")
                {
                    if (br.update_check_sys_recordinfo(json).Rows.Count > 0)
                    {
                        //当数据有重复时success返回值为4
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        DataTable dt = br.Before_Change(json);
                        int i = br.update_sys_recordinfo(json);
                        if (i > 0)
                        {
                            string content = bm.getrecordinfor(dt,json);
                            br.saveChanges_recordinfo(json, content);
                            response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                        }
                        
                    }
                }
                else
                {
                    DataTable dt = br.Before_Change(json);
                    int i = br.update_sys_recordinfo(json);
                    if (i > 0)
                    {
                        string content = bm.getrecordinfor(dt, json);
                        br.saveChanges_recordinfo(json, content);
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
                strWhere = strWhere + " and t1.code like '%" + Request["CodeBase"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["RecordBase"]))
            {
                strWhere = strWhere + " and t1.busiunit like '%" + Request["RecordBase"] + "%'";
            }

            if (!string.IsNullOrEmpty(combo_ENABLED_S2))
            {
                strWhere = strWhere + " and t1.enabled='" + combo_ENABLED_S2 + "'";
            }

            Sql.busi_RecordInfor br = new Sql.busi_RecordInfor();
            DataTable dt = br.export_recordinfo(strWhere);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("备案信息");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("备案号");
            row1.CreateCell(1).SetCellValue("账册属性");
            row1.CreateCell(2).SetCellValue("经营单位代码");
            row1.CreateCell(3).SetCellValue("经营单位名称");
            row1.CreateCell(4).SetCellValue("收发货单位代码");
            row1.CreateCell(5).SetCellValue("收发货单位名称");
            row1.CreateCell(6).SetCellValue("贸易方式");
            row1.CreateCell(7).SetCellValue("征免性质");
            row1.CreateCell(8).SetCellValue("规格启用/禁用");
            row1.CreateCell(9).SetCellValue("启用/禁用");
            row1.CreateCell(10).SetCellValue("维护人");
            row1.CreateCell(11).SetCellValue("停用人");
            row1.CreateCell(12).SetCellValue("启用时间");
            row1.CreateCell(13).SetCellValue("停用时间");
            row1.CreateCell(14).SetCellValue("维护时间");
            row1.CreateCell(15).SetCellValue("备注");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["BOOKATTRIBUTE"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["BUSIUNIT"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["RECEIVEUNIT"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["RECEIVEUNITNAME"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["TRADENAME"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["EXEMPTINGNAME"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["ISMODEL"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CREATEMANNAME"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["STOPMANNAME"].ToString());
                rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["STARTDATE"].ToString());
                rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["ENDDATE"].ToString());
                rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["CREATEDATE"].ToString());
                rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["REMARK"].ToString());
            }
            try
            {
                // 输出Excel
                string filename = "备案信息.xls";
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

            Sql.busi_RecordInfor bc = new Sql.busi_RecordInfor();
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
                //备案号                           //账册属性
                string code = stringList[0]; string BOOKATTRIBUTE = stringList[1];
                //规格型号启用                      
                string ISMODEL = stringList[6] == "是" ? "1" : "0";
                //是否启用
                string ENABLED = stringList[7] == "是" ? "1" : "0";
                //BUSIUNIT                                        
                string BUSIUNIT = stringList[2]; 
                //RECEIVEUNIT
                string RECEIVEUNIT = stringList[3];
                //TRADE
                string TRADE = stringList[4];
                //EXEMPTING
                string EXEMPTING = stringList[5];
                //remark
                string REMARK = stringList[8];

                //启用日期
                string startdate = json_formdata.Value<string>("STARTDATE");
                //停用日期
                string enddate = json_formdata.Value<string>("ENDDATE");

                string formdata = "{\"CODE\":\"" + code + "\",\"BOOKATTRIBUTE\":\"" + BOOKATTRIBUTE + "\",\"ENABLED\":\"" + ENABLED + "\",\"STARTDATE\":\"" + startdate + "\",\"ENDDATE\":\"" + enddate + "\",\"ISMODEL\":\"" + ISMODEL + "\"," +
                                  "\"BUSIUNIT\":\"" + BUSIUNIT + "\",\"RECEIVEUNIT\":\"" + RECEIVEUNIT + "\",\"TRADE\":\"" + TRADE + "\",\"EXEMPTING\":\"" + EXEMPTING + "\",\"REMARK\":\"" + REMARK + "\"}";
                //string formdata = sw.importValue(json_formdata, table, stringList);
                JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
                if (bc.check_sys_recordinfo(json).Tables[0].Rows.Count > 0 || string.IsNullOrEmpty(json.Value<string>("CODE")))
                {
                    errorlines.Add(i + 2);
                }
                else
                {
                    bc.insert_sys_recordinfo(json);
                    count = count + 1;
                }
                stringList.Clear();
            }
            successinsert.Add(count);
            returndic.Add(1, successinsert);
            returndic.Add(2, errorlines);
            return returndic;
        }



        private void loaddatamaterials()
        {
            string recordinfoid = Request["recordinfoid"];
            string strWhere = " where t1.recordinfoid = '" + recordinfoid + "' and t1.itemnoattribute = '料件' ";
            if (!string.IsNullOrEmpty(Request["HSNUMBER"]))
            {
                strWhere = strWhere + " and t1.itemno like '%" + Request["HSNUMBER"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["HSCODE1"]))
            {
                strWhere = strWhere + " and t1.hscode like '%" + Request["HSCODE1"] + "%'";
            }

            if (!string.IsNullOrEmpty(Request["ENABLED_S2"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S2"] + "'";
            }
            Sql.busi_RecordInfor bc = new Sql.busi_RecordInfor();
            DataTable dt = bc.LaoDataDetails(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();

        }


        private void loaddataproduct()
        {
            string recordinfoid = Request["recordinfoid"];
            string strWhere = " where t1.recordinfoid = '" + recordinfoid + "' and t1.itemnoattribute = '成品' ";
            if (!string.IsNullOrEmpty(Request["HSNUMBER"]))
            {
                strWhere = strWhere + " and t1.itemno like '%" + Request["HSNUMBER"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["HSCODE1"]))
            {
                strWhere = strWhere + " and t1.hscode like '%" + Request["HSCODE1"] + "%'";
            }

            if (!string.IsNullOrEmpty(Request["ENABLED_S2"]))
            {
                strWhere = strWhere + " and t1.enabled='" + Request["ENABLED_S2"] + "'";
            }
            Sql.busi_RecordInfor bc = new Sql.busi_RecordInfor();
            DataTable dt = bc.LaoDataDetails(strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }


        private void getRecordDetails()
        {
            
            string Jsonunit = String.Empty;
            
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
                    on t1.yearid = t4.id";

            
            
            DataTable unit = DBMgrBase.GetDataTable(sql);
            Jsonunit = JsonConvert.SerializeObject(unit, iso);
            
            Response.Write("{UNIT:" + Jsonunit + "}");
            Response.End();

        }

        private void getRecordDetails_Hscode()
        {
            string hscode = Request["hscode"];
            string additionalno = Request["additionalno"];
            string JsonHscode = string.Empty;
            string sql2 = @"select ID, HSCODE, EXTRACODE from BASE_COMMODITYHS where 1=1 and enabled = 1 ";
            if (!string.IsNullOrEmpty(hscode))
            {
                sql2 += " and HSCODE = '" + hscode + "'";
            }
            if (!string.IsNullOrEmpty(additionalno))
            {
                sql2 += " and ExtraCode = '" + additionalno + "'";
            }
            DataTable hs = DBMgrBase.GetDataTable(sql2);
            JsonHscode = JsonConvert.SerializeObject(hs,iso);
            Response.Write("{HSCODE:" + JsonHscode + "}");
            Response.End();

        }


        private void save_recorddetails()
        {
            string recordinfoid = Request["recordinfoid"];
            string formdata = Request["formdata"];
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            Sql.busi_RecordInfor br = new Sql.busi_RecordInfor();
            Base_Codename_Method bm = new Base_Codename_Method();
            string response = String.Empty;
            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                if (json.Value<string>("ENABLED")=="1")
                {
                    if (br.check_repeat_record_details(json,recordinfoid).Rows.Count>0)
                    {
                        //当数据有重复时success返回值为4
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        int i = br.insert_record_details(recordinfoid, json);
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }
                }
                else
                {
                    int i = br.insert_record_details(recordinfoid, json);
                    response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                }  
            }
            else
            {
                if (json.Value<string>("ENABLED") == "1")
                {
                    if (br.check_update_repeat_record_details(json, recordinfoid).Rows.Count > 0)
                    {
                        //当数据有重复时success返回值为4
                        response = "{\"success\":\"4\"}";
                    }
                    else
                    {
                        //更新
                        DataTable dt = br.get_before_change_record_details(json);
                        int i = br.update_record_details(json, recordinfoid);
                        if (i > 0)
                        {
                            string content = bm.getrecordinfodetails(dt, json);
                            br.insert_alert_record_details(json, content);
                            response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                        }
                    }
                }
                else
                {
                    //更新
                    DataTable dt = br.get_before_change_record_details(json);
                    int i = br.update_record_details(json, recordinfoid);
                    if (i>0)
                    {
                        string content = bm.getrecordinfodetails(dt, json);
                        br.insert_alert_record_details(json, content);
                        response = "{\"success\":" + (i > 0 ? "true" : "false") + "}";
                    }
                }
            }
            Response.Write(response);
            Response.End();
        }


        private void export_recorddetails()
        {
            string recordinfoid = Request["ID"];
            string strWhere = "";
            string combo_ENABLED_S2 = Request["ENABLED_S2"];
            if (combo_ENABLED_S2 == "null")
            {
                combo_ENABLED_S2 = String.Empty;
            }
            if (!string.IsNullOrEmpty(Request["HSNUMBER"]))
            {
                strWhere = strWhere + " and t1.itemno like '%" + Request["HSNUMBER"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["HSCODE1"]))
            {
                strWhere = strWhere + " and t1.hscode like '%" + Request["HSCODE1"] + "%'";
            }

            if (!string.IsNullOrEmpty(combo_ENABLED_S2))
            {
                strWhere = strWhere + " and t1.enabled='" + combo_ENABLED_S2 + "'";
            }
            Sql.busi_RecordInfor br = new Sql.busi_RecordInfor();
            DataTable dt = br.export_record_details(strWhere, recordinfoid);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("备案详情");
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("项号");
            row1.CreateCell(1).SetCellValue("HS编码");
            row1.CreateCell(2).SetCellValue("附加码");
            row1.CreateCell(3).SetCellValue("项号属性");
            row1.CreateCell(4).SetCellValue("商品名称");
            row1.CreateCell(5).SetCellValue("料号");
            row1.CreateCell(6).SetCellValue("规格型号");
            row1.CreateCell(7).SetCellValue("成交单位");
            row1.CreateCell(8).SetCellValue("版本号");
            row1.CreateCell(9).SetCellValue("启用/禁用");
            row1.CreateCell(10).SetCellValue("启用时间");
            row1.CreateCell(11).SetCellValue("维护人");
            row1.CreateCell(12).SetCellValue("维护时间");
            row1.CreateCell(13).SetCellValue("停用人");
            row1.CreateCell(14).SetCellValue("停用时间");
            row1.CreateCell(15).SetCellValue("备注");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["ITEMNO"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["HSCODE"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["ADDITIONALNO"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["ITEMNOATTRIBUTE"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["COMMODITYNAME"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["PARTNO"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["SPECIFICATIONSMODEL"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["UNIT"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["VERSION"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["ENABLED"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["STARTDATE"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["CREATEMANNAME"].ToString());
                rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["CREATEDATE"].ToString());
                rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["STOPMANNAME"].ToString());
                rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["ENDDATE"].ToString());
                rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["REMARK"].ToString());
            }
            try
            {
                // 输出Excel
                string filename = "备案详情.xls";
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


        private void ImportExcelData_RecordDeatils()
        {
            string recordinfoid = Request["ID"];
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
            Dictionary<int, List<int>> result = upload_base_recorddetails(newfile, fileName, action, json_formdata, recordinfoid);

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

        public Dictionary<int, List<int>> upload_base_recorddetails(string newfile, string fileName, string action,
            JObject json_formdata, string recordinfoid)
        {
            
                Base_Company_Method bcm = new Base_Company_Method();
                Switch_helper_Base_blend_web sw = new Switch_helper_Base_blend_web();
                DataTable dtExcel = bcm.GetExcelData_Table(Server.MapPath(newfile), 0);
                List<string> stringList = new List<string>();

                //记住发生错误的行数
                List<int> errorlines = new List<int>();

                //记住插入成功的个数
                int count = 0;

                Sql.busi_RecordInfor bc = new Sql.busi_RecordInfor();
                //插入成功的个数(返回放入dictionary)
                List<int> successinsert = new List<int>();
                //返回值
                Dictionary<int, List<int>> returndic = new Dictionary<int, List<int>>();
                try
                {
                    for (int i = 0; i < dtExcel.Rows.Count; i++)
                    {

                        for (int j = 0; j < dtExcel.Columns.Count; j++)
                        {
                            stringList.Add(dtExcel.Rows[i][j].ToString());
                        }

                        //项号
                        if (i == 8)
                        {

                        }
                        RecordDetailEn detail = new RecordDetailEn(stringList, json_formdata);
                        string jsonStr = JsonConvert.SerializeObject(detail);
                        JObject json = (JObject)JsonConvert.DeserializeObject(jsonStr);
                        
                        if (bc.check_repeat_record_details(json, recordinfoid).Rows.Count > 0)
                        {
                            errorlines.Add(i + 2);
                        }
                        else
                        {
                            int f = bc.insert_record_details(recordinfoid, json);
                            if (f > 0) count = count + 1;
                            else errorlines.Add(i + 2);
                        }
                        stringList.Clear();
                    }
                    successinsert.Add(count);
                    returndic.Add(1, successinsert);
                    returndic.Add(2, errorlines);
                }
                catch (Exception ex)
                {

                }
                return returndic;

        }

        
    }
}