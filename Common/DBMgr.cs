using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Data;

namespace Web_After.Common
{
    public class DBMgr
    { 
        private static readonly string ConnectionString = ConfigurationManager.AppSettings["strconn"];
        public static DataSet GetDataSet(string sql)
        {
            DataSet ds = new DataSet();
            OracleConnection orclCon = null;
            try
            {
                using (orclCon = new OracleConnection(ConnectionString))
                {
                    DbCommand oc = orclCon.CreateCommand();
                    oc.CommandText = sql;
                    if (orclCon.State.ToString().Equals("Open"))
                    {
                        orclCon.Close();
                    }
                    orclCon.Open();
                    DbDataAdapter adapter = new OracleDataAdapter();
                    adapter.SelectCommand = oc;
                    adapter.Fill(ds);
                }
            }
            catch (Exception e)
            {
                //log.Error(e.Message + e.StackTrace);
            }
            finally
            {
                orclCon.Close();
            }
            return ds;
        }

        public static DataTable GetDataTable(string sql)
        {
            DataSet ds = new DataSet();
            OracleConnection orclCon = null;
            try
            {
                using (orclCon = new OracleConnection(ConnectionString))
                {
                    DbCommand oc = orclCon.CreateCommand();
                    oc.CommandText = sql;
                    if (orclCon.State.ToString().Equals("Open"))
                    {
                        orclCon.Close();
                    }
                    orclCon.Open();
                    DbDataAdapter adapter = new OracleDataAdapter();
                    adapter.SelectCommand = oc;
                    adapter.Fill(ds);
                    return ds.Tables[0];
                }
            }
            catch (Exception e)
            {
                throw;
                //log.Error(e.Message + e.StackTrace);
            }
            finally
            {
                orclCon.Close();
            }
           
        }

        public static int ExecuteNonQuery(string sql)
        {
            int retcount = -1;
            OracleConnection orclCon = null;
            try
            {
                using (orclCon = new OracleConnection(ConnectionString))
                {
                    OracleCommand oc = new OracleCommand(sql, orclCon);
                    //oc.Parameters.AddRange(OraPara);, OracleParameter[] OraPara
                    if (orclCon.State.ToString().Equals("Open"))
                    {
                        orclCon.Close();
                    }
                    orclCon.Open();
                    retcount = oc.ExecuteNonQuery();
                    oc.Parameters.Clear();
                }
            }
            catch (Exception e)
            {
                //log.Error(e.Message + e.StackTrace);
            }
            finally
            {
                orclCon.Close();
            }
            return retcount;
        }

        public static int ExecuteNonQuery(List<string> sqls)
        {
            int retcount = 0;
            OracleConnection orclCon = null;
            try
            {
                using (orclCon = new OracleConnection(ConnectionString))
                {
                    if (orclCon.State.ToString().Equals("Open"))
                    {
                        orclCon.Close();
                    }
                    orclCon.Open();
                    OracleCommand oc = orclCon.CreateCommand();
                    foreach(string sql in sqls)
                    {
                        oc.CommandText = sql;
                        retcount += oc.ExecuteNonQuery();
                    }
                    return retcount;
                }
            }
            catch (Exception e)
            {
                //log.Error(e.Message + e.StackTrace);
            }
            finally
            {
                orclCon.Close();
            }
            return retcount;
        }

        public static int ExecuteNonQuery(string sql, OracleParameter[] OraPara)
        {
            int retcount = -1;
            OracleConnection orclCon = null;
            try
            {
                using (orclCon = new OracleConnection(ConnectionString))
                {
                    OracleCommand oc = new OracleCommand(sql, orclCon);
                    oc.Parameters.AddRange(OraPara);
                    if (orclCon.State.ToString().Equals("Open"))
                    {
                        orclCon.Close();
                    }
                    orclCon.Open();
                    retcount = oc.ExecuteNonQuery();
                    oc.Parameters.Clear();
                }
            }
            catch (Exception e)
            {
                //log.Error(e.Message + e.StackTrace);
            }
            finally
            {
                orclCon.Close();
            }
            return retcount;
        }

    }
}