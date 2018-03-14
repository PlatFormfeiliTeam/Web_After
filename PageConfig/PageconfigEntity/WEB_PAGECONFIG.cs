using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_After.PageConfig.PageconfigEntity
{
    public class WEB_PAGECONFIG
    {
        public Int32 ID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string PAGENAME { get; set; }
        public string CONFIGCONTENT { get; set; }
        public string CUSTOMERCODE { get; set; }
        public DateTime? CREATETIME { get; set; }
        public Int32 ENABLED { get; set; }
        public Int32 USERID { get; set; }
        public string USERNAME { get; set; }
        //业务类型和业务细项是从CONFIGCONTENT拆出来的
        public string BUSITYPE { get; set; }
        public string BUSIDETAIL { get; set; }
        public string REASON { get; set; }
    }
}