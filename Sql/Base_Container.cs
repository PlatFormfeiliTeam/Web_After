using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Web_After.Common;

namespace Web_After.Sql
{
    public class Base_Container
    {
        //信息加载
        public DataTable LoaData(string table, string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,
                                  t2.name as createmanname,
                                  t3.name as stopmanname
                                  from base_containerstandard t1 left join sys_user t2 on t1.createman=t2.id 
                                  left join sys_user t3 on t1.stopman=t3.id 
                                  where 1 = 1 {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.code", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }
    }
}