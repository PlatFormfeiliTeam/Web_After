using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Web_After.model
{
    public class ExportErrorInfoEn
    {
        public int Id { get; set; }
        [Description("表名")]
        public string TableName { get; set; }
        [Description("字段名")]
        public string FieldName { get; set; }
        [Description("错误原因")]
        public string Reason { get; set; }
        [Description("错误所在行")]
        public int ErrorRow { get; set; }
    }
}