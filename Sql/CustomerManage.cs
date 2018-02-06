using System.Data;
using System.Web.Services.Description;
using Web_After.Common;
using Web_After.model;

namespace Web_After.Sql
{
    public class CustomerManage
    {
        //导入之前做一个客户代码重复验证
        public DataTable before_import_check(string code)
        {
            string sql = "select * from cusdoc.Sys_Customer where code = '" + code + "'";            
            DataTable dt = DBMgr.GetDataTable(sql);           
            return dt;
        }

        //导入数据库
        public int insert_import_sys_customer(CustomerEn cus)
        {
            string sql = @"insert into cusdoc.Sys_Customer(Id, Code, name, ChineseAbbreviation, HSCode, CIQCode, ChineseAddress, EnglishName, EnglishAddress, Enabled, Remark,
                ISCUSTOMER,ISSHIPPER,ISCOMPANY) values(cusdoc.Sys_Customer_Id.nextval, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', {8}, '{9}',{10},{11},{12})";
            sql = string.Format(sql, cus.Code, cus.name, cus.ChineseAbbreviation, cus.HSCode, cus.CIQCode, cus.ChineseAddress, cus.EnglishName, cus.EnglishAddress,
                cus.Enabled, cus.Remark, cus.ISCUSTOMER, cus.ISSHIPPER, cus.ISCOMPANY);
            int i = DBMgr.ExecuteNonQuery(sql);
            return i;
        }
    }
}