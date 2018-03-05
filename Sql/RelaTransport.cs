using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Web_After.Common;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using Web_After.BasicManager;

namespace Web_After.Sql
{
    public class RelaTransport
    {
        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,t2.name as decltransportname,t3.name as insptransportname,t4.name as createmanname,t5.name as stopmanname from rela_transport t1 left join base_transport t2 on 
t1.decltransport = t2.code left join base_inspconveyance t3 on t1.insptransport = t3.code  left join sys_user t4 on t1.createman=t4.id left join sys_user t5 on t1.stopman=t5.id    {0}";
            sql = string.Format(sql, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.decltransport", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }

    }
}