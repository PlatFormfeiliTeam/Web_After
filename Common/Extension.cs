using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Web_After.Common
{
    public static class Extension
    {
        public static string ToSHA1(this string value)
        {
            string result = string.Empty;
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] array = sha1.ComputeHash(Encoding.Unicode.GetBytes(value));
            for (int i = 0; i < array.Length; i++)
            {
                result += array[i].ToString("x2");
            }
            return result;
        }

        //获得用户信息
        public static JObject Get_UserInfo(string username)
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";


            string sql = @"select * from sys_user where name = '{0}'";
            sql = string.Format(sql, username);

            string jsonstr = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso).Replace("[", "").Replace("]", "");
            return (JObject)JsonConvert.DeserializeObject(jsonstr);
        }

        public static bool IsValidUserST(string userName, out string password)
        {
            string sql = "select password from SYS_USER where name = '" + userName + "'";
            DataTable ents = DBMgr.GetDataTable(sql);
            password = "";
            if (ents.Rows.Count > 0)
            {
                password = ents.Rows[0][0].ToString();
                return true;
            }
            return false;
        }

        public static string GetPageSql(string tempsql, string order, string asc, ref int totalProperty, int start, int limit)
        {
            //int start = Convert.ToInt32(Request["start"]);
            //int limit = Convert.ToInt32(Request["limit"]);
            string sql = "select count(1) from ( " + tempsql + " )";
            totalProperty = Convert.ToInt32(DBMgr.GetDataTable(sql).Rows[0][0]); 
            string pageSql = @"SELECT * FROM ( SELECT tt.*, ROWNUM AS rowno FROM ({0} ORDER BY {1} {2}) tt WHERE ROWNUM <= {4}) table_alias WHERE table_alias.rowno >= {3}";
            pageSql = string.Format(pageSql, tempsql, order, asc, start + 1, limit + start);
            return pageSql;
        }

        /// <summary>
        /// 将DataTable指定列转换为list
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnName">需要转换的列名</param>
        /// <returns></returns>
        public static List<string> getColumnFromDatatable(DataTable dt, string columnName)
        {
            List<string> columnList = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                columnList.Add(dr[columnName].ToString());
            }
            return columnList;
        }




        public static bool ToBool(this int value)
        {
            bool result = true;
            if (value > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string Trim2(this string value)
        {
            string result = value;
            if (!value.IsNullOrEmpty())
            {
                result = value.Trim();
            }
            return result;
        }
        /// <summary>
        /// 将日期转化为字符串类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultTime"></param>
        /// <returns></returns>
        public static string DateToString(this DateTime value, string defaultTime)
        {
            string result = defaultTime;
            if (value != null && value.IsDaylightSavingTime())
            {
                result = value.ToString();
            }
            return result;
        }
        /// <summary>
        /// 判断时间是否有效
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultTime"></param>
        /// <returns></returns>
        public static DateTime ToDate(this DateTime value, DateTime defaultTime)
        {
            DateTime result = value;
            string a = value.ToString();
            if (a == "0001/1/1 0:00:00")//表示时间为空
            {
                result = defaultTime;
            }
            return result;
        }
        public static string ToString2(this object value)
        {
            string result = "";
            if (value != null)
            {
                result = value.ToString();
            }
            return result;
        }
        public static string ToString2(this object value, string defaultValue)
        {
            string result = defaultValue;
            if (value != null)
            {
                result = value.ToString();
            }
            return result;
        }
        public static int ToInt(this string value)
        {
            return ToInt(value, 0);
        }

        public static int ToInt(this string value, int defaultValue)
        {
            int result = 0;
            bool success = Int32.TryParse(value, out result);
            if (!success)
            {
                result = defaultValue;
            }
            return result;
        }
        public static double ToDouble(this string value)
        {
            return ToDoule(value, 0.0);
        }

        public static double ToDoule(this string value, double defaultValue)
        {
            double result = 0;
            bool success = Double.TryParse(value, out result);
            if (!success)
            {
                result = defaultValue;
            }
            return result;
        }

        public static bool ToBool(this string value)
        {
            bool result = false;
            bool.TryParse(value, out result);
            return result;
        }

        public static DateTime ToDateTime(this string value)
        {
            return ToDateTime(value, DateTime.Now);
        }

        public static DateTime ToDateTime(this string value, DateTime defaultValue)
        {
            DateTime result;
            DateTime.TryParse(value, out result);
            return result == DateTime.MinValue ? defaultValue : result;
        }

       

    }
}
