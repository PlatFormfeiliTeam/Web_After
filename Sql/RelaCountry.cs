using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Web_After.Common;

namespace Web_After.Sql
{
    public class RelaCountry
    {
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,t2.name as declcountryname,t3.name as inspcountryname,t4.name as createmanname,t5.name as stopmanname from rela_country t1 left join 
                                  base_country t2 on t1.declcountry=t2.code left join base_inspcountry t3 on t1.inspcountry=t3.code  left join sys_user t4 on t1.createman=t4.id left join sys_user t5 on t1.stopman=t5.id {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.declcountry", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }
    }
}