using System.Data;
using Web_After.BasicManager;
using Web_After.Common;

namespace Web_After.Sql
{
    public class Base_year
    {

        public DataTable LoaData(string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string sql = @"select t1.*,
                           t2.name as createmanname,
                           t3.name as customareaname,
                           s.name  as stopmanName
                           from base_year t1
                           left join sys_user t2
                           on t1.createman = t2.id
                           left join BASE_CUSTOMDISTRICT t3
                           on t1.customarea = t3.code
                           left join sys_user s
                           on t1.stopman = s.id
                           where t1.kind ={0} {1}
                            ";
            sql = string.Format(sql, (int)Base_YearKindEnum.HS, strWhere);
            sql = Extension.GetPageSql2(sql, "t1.name", "desc", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }


    }
}