using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_After.model
{
    public class Inspection
    {
        public string APPROVALCODE { get; set; }
        public string INSPECTIONCODE { get; set; }
        public string TRADEWAY { get; set; }
        public string CLEARANCECODE { get; set; }
        public string SHEETNUM { get; set; }
        public string COMMODITYNUM { get; set; }
        public string INSPSTATUS { get; set; }
        public string MODIFYFLAG { get; set; }
        public string PREINSPCODE { get; set; }
        public string CUSNO { get; set; }
        public string OLDINSPECTIONCODE { get; set; }
        public string ISDEL { get; set; }
        public string ISNEEDCLEARANCE { get; set; }
        public string LAWFLAG { get; set; }
        public string DIVIDEREDISKEY { get; set; }
    }

}