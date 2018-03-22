using System.Data;
using Web_After.BasicManager;
using Web_After.BasicManager.BasicManager;
using Web_After.Common;

namespace Web_After.Sql
{
    public class busi_RecordInfor
    {
        Base_Codename_Method bcm = new Base_Codename_Method();
        string stopman = "";
        string createman = "";
        string startdate = "";
        string enddate = "";
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,
                       t2.name as createmanname,
                       t3.name as stopmanname,
                       T4.NAME AS TRADENAME,
                       T5.NAME AS EXEMPTINGNAME,
                       t6.name as busiunitname
                      from SYS_RECORDINFO t1
                      LEFT JOIN sys_user t2
                        on t1.createman = t2.id
                      LEFT JOIN sys_user t3
                        on t1.stopman = t3.id
                      LEFT JOIN BASE_DECLTRADEWAY t4
                        ON t1.TRADE = t4.CODE
                       AND t4.enabled = 1
                      LEFT JOIN BASE_EXEMPTINGNATURE t5
                        ON t1.EXEMPTING = t5.CODE
                       AND t5.enabled = 1
                      left join base_company t6
                        on t1.busiunit = t6.code
                        where 1 = 1 {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.code", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
            
        }
    }
}