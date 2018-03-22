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
using Web_After.Common;

namespace Web_After.BasicManager.DeclInfor
{
    public partial class Decl_HSClass : System.Web.UI.Page
    {
        IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
        int totalProperty = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"];
            string table = Request["table"];
            iso.DateTimeFormat = "yyyy-MM-dd";
            switch (action)
            {
                case "loaddatachapter":
                    loaddatachapter();
                    break;
                case "loaddatacategory":
                    loaddatacategory();
                    break;
                case "loaddatasmallclass":
                    loaddatasmallclass();
                    break;
                case "save_category":  case "save_chapter": case "save_smallclass":
                    save_category(Request["formdata"],action);
                    break;
                case "add_category":  case "add_chapter": case "add_smallclass":
                    ImportExcelData(table);
                    break;
                case "hsclass":
                    getclass();
                    break;
                
            }
        }

        public void loaddatachapter()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["CODE_S"]))
            {
                strWhere = strWhere + " and t1.code like '%" + Request["CODE_S"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["CNNAME_S"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["CNNAME_S"] + "%'";
            }
            string table = "chapter";
            Sql.Base_blend_web bw = new Sql.Base_blend_web();
            DataTable dt = bw.LoaData(table, strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }

        public void loaddatacategory()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["CODE_S_category"]))
            {
                strWhere = strWhere + " and t1.code like '%" + Request["CODE_S_category"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["CNNAME_S_category"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["CNNAME_S_category"] + "%'";
            }
            string table = "category";
            Sql.Base_blend_web bw = new Sql.Base_blend_web();
            DataTable dt = bw.LoaData(table, strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }

        private void loaddatasmallclass()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["CODE_S_smallclass"]))
            {
                strWhere = strWhere + " and t1.code like '%" + Request["CODE_S_smallclass"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["CNNAME_S_smallclass"]))
            {
                strWhere = strWhere + " and t1.name like '%" + Request["CNNAME_S_smallclass"] + "%'";
            }
            string table = "smallclass";
            Sql.Base_blend_web bw = new Sql.Base_blend_web();
            DataTable dt = bw.LoaData(table, strWhere, "", "", ref totalProperty, Convert.ToInt32(Request["start"]),
                Convert.ToInt32(Request["limit"]));
            string json = JsonConvert.SerializeObject(dt, iso);
            Response.Write("{rows:" + json + ",total:" + totalProperty + "}");
            Response.End();
        }

        private void save_category(string formdata,string action)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            Sql.Base_blend_web bc = new Sql.Base_blend_web();
            string response = "";
            string table = "";
            switch (action)
            {
                case "save_category":
                    table = "category";
                    break;
                case "save_chapter":
                    table = "base_declhschapter";
                    break;
                case "save_smallclass":
                    table = "base_declhsclass";
                    break;
            }
            
            
                if (bc.check_repeat(table, json) > 0)
                {
                    response = "{\"success\":\"4\"}";
                }
                else
                {
                    int i = bc.insertTable(table, json);
                    if (i > 0)
                    {
                        response = "{\"success\":\"5\"}";
                    }
                }
            
            Response.Write(response);
            Response.End();
        }


        private void ImportExcelData(string table)
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
            Dictionary<int, List<int>> result = upload_base_company(newfile, fileName, action, json_formdata, table);

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
           JObject json_formdata, string table)
        {
            Base_Company_Method bcm = new Base_Company_Method();
            Switch_helper_Base_blend_web sw = new Switch_helper_Base_blend_web();
            DataTable dtExcel = bcm.GetExcelData_Table(Server.MapPath(newfile), 0);
            List<string> stringList = new List<string>();

            //记住发生错误的行数
            List<int> errorlines = new List<int>();

            //记住插入成功的个数
            int count = 0;

            Sql.Base_blend_web bc = new Sql.Base_blend_web();
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
                string formdata = sw.importValue(json_formdata, table, stringList);
                JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
                if (bc.check_repeat(table, json) > 0 || string.IsNullOrEmpty(json.Value<string>("CODE")))
                {
                    errorlines.Add(i + 2);
                }
                else
                {
                    bc.insertTable(table, json);
                    count = count + 1;
                }
                stringList.Clear();
            }
            successinsert.Add(count);
            returndic.Add(1, successinsert);
            returndic.Add(2, errorlines);
            return returndic;
        }


        private void getclass()
        {
            string table = Request["table"];
            string sql = String.Empty;
            string json = String.Empty;
            switch (table)
            {
                case "chapter":
                    sql = @"select t1.*, t2.name as createmanname
                    from base_declhstype t1
                    left join sys_user t2
                    on t1.createman = t2.id ";
                    break;
                case "smallclass":
                    sql = @"select t1.*, t2.code as typecode, t2.name as typename,t1.code || '('|| t1.name ||')' as codeaddname,t2.code || '|' || t2.name  as typecodeaddname
                          from base_declhschapter t1
                          left join base_declhstype t2
                          on t1.typecode = t2.code where t1.code ='" +Request["code"]+"' ";
                    break;
            }
            if (!string.IsNullOrEmpty(sql))
            {
                DataTable dt = DBMgrBase.GetDataTable(sql);
                json = JsonConvert.SerializeObject(dt, iso);
            }
            else
            {
                json = "[]";
            }

            Response.Write(json);
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



    }
}