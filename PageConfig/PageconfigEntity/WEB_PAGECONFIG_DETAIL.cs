using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_After.PageConfig.PageconfigEntity
{
    public class WEB_PAGECONFIG_DETAIL
    {
        public Int32 ID { get; set; }
        public Int32 PARENTID { get; set; }
        public Int32 ORDERNO { get; set; }
        public string NAME { get; set; }
        public string CONTROLTYPE { get; set; }
        public string SELECTCONTENT { get; set; }
        public string CONFIGTYPE { get; set; }
        public string TABLECODE { get; set; }
        public string FIELDCODE { get; set; }
        public string TABLENAME { get; set; }
        public string FIELDNAME { get; set; }
        public DateTime? CREATETIME { get; set; }
        public Int32 USERID { get; set; }
        public string USERNAME { get; set; }
        public Int32 ENABLED { get; set; }   

    }
}