using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Web_After.model
{
    public class CustomerEn
    {
        public int Id { get; set; }

        [Description("编号")]
        public string Code { get; set; }

        [Description("中文名称")]
        public string name { get; set; }

        [Description("中文简称")]
        public string ChineseAbbreviation { get; set; }

        [Description("中文地址")]
        public string ChineseAddress { get; set; }

        [Description("英文名称")]
        public string EnglishName { get; set; }

        [Description("英文地址")]
        public string EnglishAddress { get; set; }

        [Description("海关编码")]
        public string HSCode { get; set; }

        [Description("国检编码")]
        public string CIQCode { get; set; }

        [Description("是否启用")]
        public int Enabled { get; set; }

        [Description("备注")]
        public string Remark { get; set; }

        [Description("客户")]
        public int ISCUSTOMER { get; set; }
        [Description("供应商")]
        public int ISSHIPPER { get; set; }
        [Description("生产型企业")]
        public int ISCOMPANY { get; set; }
        [Description("客户")]
        public string ISCUSTOMER1 { get; set; }
        [Description("供应商")]
        public string ISSHIPPER1 { get; set; }
        [Description("生产型企业")]
        public string ISCOMPANY1 { get; set; }
    }
}