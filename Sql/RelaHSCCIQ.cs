using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using Web_After.BasicManager;
using Web_After.Common;

namespace Web_After.Sql
{
    public class RelaHSCCIQ
    {
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,t2.hsname as hsname,t3.ciqname,t4.name as createmanname,t5.name as stopmanname from rela_hsciq t1 left join base_insphs t2 on 
                                   t1.hscode=(t2.hscode||t2.extracode) left join base_ciqcode t3 on t1.ciqcode=t3.ciq  left join sys_user t4 on t1.createman=t4.id left join sys_user t5 on t1.stopman=t5.id {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.hscode", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }

    }
}