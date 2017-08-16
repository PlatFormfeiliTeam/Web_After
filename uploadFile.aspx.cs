using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.XSSF.UserModel;
using Web_After.model;
using Web_After.Common;

namespace Web_After
{
    public partial class uploadFlie : System.Web.UI.Page
    {
        public List<ExportErrorInfoEn> ErrorList = new List<ExportErrorInfoEn>();//错误信息集合
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"];
            string name = Request["tbl"];
            var sd = Request["upfile"];
            HttpFileCollection files = Request.Files;
            string json = "";
            if (files.Count < 0)
            {
                //json = "{success:false}";
            }
            try
            {
                DataTable dw = new DataTable();
                for (int iFile = 0; iFile < files.Count; iFile++)
                {
                    //检查文件扩展名
                    HttpPostedFile postFile = files[iFile];
                    String fileName = String.Empty;
                    String fileExtension = String.Empty;
                    String oldName = String.Empty;
                    fileName = System.IO.Path.GetFileName(postFile.FileName);
                    int pos = fileName.LastIndexOf('.');
                    if (pos > 0)
                    {
                        fileExtension = fileName.Substring(pos + 1);
                        oldName = fileName.Substring(0, pos);
                    }
                    if (fileName != "")
                    {
                        string savePath = HttpContext.Current.Request.MapPath(@"FileUpload\importExcel");
                        if (Directory.Exists(savePath) == false)
                            throw new Exception(savePath + "不存在，请创建");
                        string postPath = savePath + "\\" + fileName;
                        postFile.SaveAs(postPath);
                        importData(postPath, name);
                        //string virtualPath = postPath.Substring(postPath.IndexOf("\\Ext"), postPath.Length - postPath.IndexOf("\\Ext"));
                        //dw = GetExcelDataTable(virtualPath, "Sheet1");
                        //DataTable dw2 = GetTableColumn(dw, "test", columns);
                        //json = DataTableToJson(dw2);
                    }
                }
                Response.Write("{\"success\":true}");
                Response.End();
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write(ex.Message);
            }
        }

        private bool importData(string path,string name)
        {
            ISheet sheet;
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                IWorkbook hssfworkbook = new XSSFWorkbook(stream);
               
                sheet = hssfworkbook.GetSheetAt(0);
            }
            switch(name)
            {
                case "customer":
                    importCustomer(sheet);
                    break;
            }
            
            return true;
        }

        private void importCustomer(ISheet sheet)
        {
            CustomerEn cus = new CustomerEn();
            List<string> sqlList = new List<string>();
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                XSSFRow row = sheet.GetRow(i) as XSSFRow;
                cus.Code = row.GetCell(0).ToString2().Trim();
                if (string.IsNullOrEmpty(cus.Code))
                {
                    setErrorInfo("客商代码", "代码为空", i);
                    continue;
                }
                cus.HSCode = row.GetCell(1).ToString2().Trim();
                cus.CIQCode = row.GetCell(2).ToString2().Trim();
                cus.ChineseAbbreviation = row.GetCell(3).ToString2().Trim();
                cus.name = row.GetCell(4).ToString2().Trim();
                if (string.IsNullOrEmpty(cus.name))
                {
                    setErrorInfo("客商名称", "名称为空", i);
                    continue;
                }
                cus.ChineseAddress = row.GetCell(5).ToString2().Trim();
                cus.EnglishName = row.GetCell(6).ToString2().Trim();
                cus.EnglishAddress = row.GetCell(7).ToString2().Trim();
                cus.Enabled = row.GetCell(8).ToString2().Trim2() == "是" ? 1 : 0;
                cus.Remark = row.GetCell(8).ToString2().Trim();
                string sql = @"insert into cusdoc.Sys_Customer(Id, Code, name, ChineseAbbreviation, HSCode, CIQCode, ChineseAddress, EnglishName, EnglishAddress, Enabled, Remark,
                ISCUSTOMER,ISSHIPPER,ISCOMPANY) values(cusdoc.Sys_Customer_Id.nextval, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', {8}, '{9}',{10},{11},{12})";
                sql = string.Format(sql, cus.Code, cus.name, cus.ChineseAbbreviation, cus.HSCode, cus.CIQCode, cus.ChineseAddress, cus.EnglishName, cus.EnglishAddress, 
                    cus.Enabled, cus.Remark, cus.ISCUSTOMER, cus.ISSHIPPER, cus.ISCOMPANY);
                sqlList.Add(sql);
            }
            int count= DBMgr.ExecuteNonQuery(sqlList);
        }
        public void setErrorInfo(string fieldName, string reason, int errorRow)
        {
            ExportErrorInfoEn error = new ExportErrorInfoEn();
            error.FieldName = fieldName;
            error.Reason = reason;
            error.ErrorRow = (errorRow + 1);
            ErrorList.Add(error);
        }
    }
}