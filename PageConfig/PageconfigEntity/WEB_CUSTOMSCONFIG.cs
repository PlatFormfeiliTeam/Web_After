using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_After.PageConfig.PageconfigEntity
{
    public class WEB_CUSTOMSCONFIG
    {
        public Int32 ID { get; set; }
        public string BUSITYPECODE { get; set; }
        public string BUSITYPENAME { get; set; }
        public string BUSIITEMCODE { get; set; }
        public string BUSIITEMNAME { get; set; }
        public Int32 CREATEUSERID { get; set; }
        public string CREATEUSERNAME { get; set; }
        public Int32 ENABLE { get; set; }
        public DateTime? STARTTIME { get; set; }
        public string REASON { get; set; }
    }
}