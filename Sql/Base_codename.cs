using System;
using System.Data;
using Newtonsoft.Json.Linq;
using Web_After.BasicManager.BasicManager;
using Web_After.Common;

namespace Web_After.Sql
{
    public class Base_codename
    {
        Switch_helper_Base_codename sc = new Switch_helper_Base_codename();
        //信息加载
        public DataTable LoaData(string table,string strWhere, string order, string asc, ref int totalProperty, int start, int limit)
        {
            string load = "load";
            JObject json = null;            
            string sql = sc.get_base_sql(load, table,json,"") + strWhere;
            sql = Extension.GetPageSql2(sql, "t1.code", "", ref totalProperty, start, limit);
            DataTable loDataSet = DBMgrBase.GetDataTable(sql);
            return loDataSet;
        }

        //新增
        public int insertTable(string table,JObject json)
        {
            string insert = "insert";           
            string sql = sc.get_base_sql(insert,table,json,"");
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

        //检查code是否重复
        public int check_repeat(string table,JObject json)
        {
            string checkrepeat = "checkrepeat";
            string sql = sc.get_base_sql(checkrepeat,table,json,"");
            int count = DBMgrBase.GetDataTable(sql).Rows.Count;
            return count;
        }
        //修改
        public int updataTable(string table,JObject json)
        {
            string update = "update";
            string sql = sc.get_base_sql(update, table, json,"");
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;

        }

        //修改提交时检查是否重复
        public int update_check_repeat(string table,JObject json)
        {
            string updateCheckrepeat = "updateCheckrepeat";
            string sql = sc.get_base_sql(updateCheckrepeat, table, json,"");
            int count = DBMgrBase.GetDataTable(sql).Rows.Count;
            return count;

        }
        //导出
        public DataTable ExportTable(string table,string strWhere)
        {
            string export = "export";
            JObject json = null;
            string sql = sc.get_base_sql(export, table, json,"") + strWhere;
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;
        }

        //获取修改之前的数据
        public DataTable getBeforeChangeData(string table,JObject json)
        {
            string BeforChange = "BeforChange";
            string sql = sc.get_base_sql("BeforChange", table, json,"");
            DataTable dt = DBMgrBase.GetDataTable(sql);
            return dt;

        }

        //插入修改信息表
        public int insertAlterRecordTable(DataTable dt,string table,JObject json)
        {
            string change = "change";
            Base_Codename_Method bcm = new Base_Codename_Method();
            string contants = bcm.getChange(dt,json,table);
            string sql = sc.get_base_sql(change,table,json,contants);
            int i = DBMgrBase.ExecuteNonQuery(sql);
            return i;
        }

    }
}