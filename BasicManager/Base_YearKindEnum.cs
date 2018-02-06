namespace Web_After.BasicManager
{
    public enum Base_YearKindEnum
    {
        /// <summary>
        /// 报检——CIQ 
        /// </summary>
        CIQ = 1,
        /// <summary>
        /// 报检——HS编码
        /// </summary>
        HS,
        /// <summary>
        /// 报检——国别
        /// </summary>
        Country,
        /// <summary>
        /// 报检——包装种类
        /// </summary>
        Package,
        /// <summary>
        /// 报检——国内口岸
        /// </summary>
        PortIn,
        /// <summary>
        /// 报检——国际口岸
        /// </summary>
        PortOut,
        /// <summary>
        /// 报检——贸易方式
        /// </summary>
        TradeWay,
        /// <summary>
        /// 报检——检验检疫机构
        /// </summary>
        InspectionAgency,
        /// <summary>
        /// 报检——币制代码
        /// </summary>
        Currency,
        /// <summary>
        /// 报检——计量单位
        /// </summary>
        ProductUnit,
        /// <summary>
        /// 报关——HS编码
        /// </summary>
        Decl_HS,
        /// <summary>
        /// 报关——关区
        /// </summary>
        Decl_CustomArea,
        /// <summary>
        /// 报关——国别
        /// </summary>
        Decl_Country,
        /// <summary>
        /// 报关——运输方式
        /// </summary>
        Decl_Transport,
        /// <summary>
        /// 报关——贸易方式
        /// </summary>
        Decl_TradeWay,
        /// <summary>
        /// 报关——征免性质
        /// </summary>
        Decl_ExemptingNature,
        /// <summary>
        /// 报关——征免方式
        /// </summary>
        Decl_ExemptingWay,
        /// <summary>
        /// 报关——结汇方式
        /// </summary>
        Decl_ExchangeWay,
        /// <summary>
        /// 报关——口岸
        /// </summary>
        Decl_Harbour,
        /// <summary>
        /// 报关——境内目的地
        /// </summary>
        Decl_ShipDest,
        /// <summary>
        /// 报关——成交方式
        /// </summary>
        Decl_Transaction,
        /// <summary>
        /// 报关——币制
        /// </summary>
        Decl_Currency,
        /// <summary>
        /// 报关——费用
        /// </summary>
        Decl_Fee,
        /// <summary>
        /// 报关——包装种类
        /// </summary>
        Decl_Package,
        /// <summary>
        /// 报关——计量单位
        /// </summary>
        Decl_ProductUnit,
        /// <summary>
        /// 报关——账册数据规则
        /// </summary>
        Decl_BooksData,
        /// <summary>
        /// 业务——申报方式
        /// </summary>
        Busi_ReportWay,
        /// <summary>
        /// 业务——木质包装
        /// </summary>
        Busi_WoodPacking,
        /// <summary>
        /// 业务——状态
        /// </summary>
        Busi_Status,
        /// <summary>
        /// 业务——业务类型
        /// </summary>
        Busi_BusiType,
        /// <summary>
        /// 业务——委托类型
        /// </summary>
        Busi_EntrustType,
        /// <summary>
        /// 业务——报关方式
        /// </summary>
        Busi_DeclWay,
        /// <summary>
        /// 业务——车辆信息
        /// </summary>
        Busi_DeclCar,
        /// <summary>
        /// 业务——备案信息表头
        /// </summary>
        Busi_RecordInfo,
        /// <summary>
        /// 业务——备案信息表体
        /// </summary>
        Busi_RecordInfoDetail,
        /// <summary>
        /// 报关——企业性质
        /// </summary>
        Busi_CompanyNature,
        /// <summary>
        /// 业务——申报库别
        /// </summary>
        Busi_ReportLibrary,
        /// <summary>
        /// 业务——报检类别
        /// </summary>
        Busi_InspType,
        /// <summary>
        /// 业务——通知类别
        /// </summary>
        Busi_NotcieType,
        /// <summary>
        /// 业务——特殊监管区类别
        /// </summary>
        Busi_ReguAreaType,
        /// <summary>
        /// 对应关系——hs和ciq对应
        /// </summary>
        Rela_HSCIQ,
        /// <summary>
        /// 对应关系——国别对应
        /// </summary>
        Rela_Country,
        /// <summary>
        /// 对应关系——包装种类对应
        /// </summary>
        Rela_Package,
        /// <summary>
        /// 对应关系——贸易方式对应
        /// </summary>
        Rela_Trade,
        /// <summary>
        /// 对应关系——币制对应
        /// </summary>
        Rela_Currency,
        /// <summary>
        /// 对应关系——计量单位对应
        /// </summary>
        Rela_ProductUnit,
        /// <summary>
        /// 运输工具
        /// </summary>
        Conveyance,
        /// <summary>
        /// 业务——货物类型
        /// </summary>
        Busi_GoodsType,
        /// <summary>
        /// 业务——随附单据
        /// </summary>
        Busi_Invoice,
        /// <summary>
        /// 报检——境内地区代码
        /// </summary>
        Insp_WithinRegion,
        /// <summary>
        /// 报检——随附单据
        /// </summary>
        Insp_NeedDocument,
        /// <summary>
        /// 报检——木质包装
        /// </summary>
        Insp_WasteGoods,
        /// <summary>
        /// 报检——用途
        /// </summary>
        Insp_Use,
        /// <summary>
        /// 报检——许可证
        /// </summary>
        Insp_License,
        /// <summary>
        /// 报关——用途
        /// </summary>
        Decl_Use,
        /// <summary>
        /// 业务——申报库别
        /// </summary>
        Busi_InspLibrary,
        /// <summary>
        /// 报关——企业信息
        /// </summary>
        Decl_Company,
        /// <summary>
        /// 业务——业务种类
        /// </summary>
        Busi_BusiKind,
        /// <summary>
        /// 业务——计量单位转化
        /// </summary>
        Busi_UnitConvert,
        /// <summary>
        /// 业务——车队信息
        /// </summary>
        Decl_Motorcade,
        /// <summary>
        /// 报检——所需单证
        /// </summary>
        Insp_Invoice,
        /// <summary>
        /// 代码库
        /// </summary>
        Base_Year,
        /// <summary>
        /// 对应关系——运输方式
        /// </summary>
        Rela_Transport,
        /// <summary>
        /// 报检——企业性质
        /// </summary>
        Insp_CompanyNature,
        /// <summary>
        /// 对应关系——企业性质
        /// </summary>
        Rela_CompanyNature,
        /// <summary>
        /// 报检——集装箱规格
        /// </summary>
        Insp_ContainerStandard,
        /// <summary>
        /// 业务——集装箱尺寸
        /// </summary>
        Busi_ContainerSize,
        /// <summary>
        /// 业务——集装箱类型
        /// </summary>
        Busi_ContainerType,
        /// <summary>
        /// 对应关系——集装箱
        /// </summary>
        Rela_Container,
        /// <summary>
        /// 业务——收货人类型
        /// </summary>
        Busi_ConsigneeType,
        /// <summary>
        /// 业务——特殊商品单位换算
        /// </summary>
        Busi_SpecialHSConvert,
        /// <summary>
        /// 业务——清单类型
        /// </summary>
        Busi_ListType,
        /// <summary>
        /// 业务——辅助选项
        /// </summary>
        Busi_AssistKind,
        /// <summary>
        /// 对应关系——港口
        /// </summary>
        Rela_Harbour,
        /// <summary>
        /// 对应关系——口岸
        /// </summary>
        Rela_Port,
        /// <summary>
        /// 对应关系——境内地区
        /// </summary>
        Rela_WithinRegion,

        /// <summary>
        /// 财务管理-配置管理-费目单位
        /// </summary>
        Finance_CostUnit,

        /// <summary>
        /// 财务管理-配置管理-收支项目
        /// </summary> 
        Finance_Expenditure,

        /// <summary>
        /// 财务管理-配置管理-预算类型 
        /// </summary> 
        Finance_BudgetType,
        /// <summary>
        /// 财务管理-配置管理-成本计算规则主档
        /// </summary> 
        Finance_CostSettlement,

        /// <summary>
        /// 财务管理-配置管理-成本计算规则明细档
        /// </summary> 
        finance_SettlementRules,
        /// <summary>
        /// 财务管理——报价标准规则主档
        /// </summary>
        Finance_ReceivableRules,  //ReceivableRulesCost
        /// <summary>
        /// 财务管理——报价客户规则主档
        /// </summary>
        Finance_ReceivableRulesCus,
        /// <summary>
        /// 申报区域
        /// </summary>
        Insp_ReportRegion,
        /// <summary>
        /// 法检类型
        /// </summary>
        Busi_InspectFlag,
        /// <summary>
        /// 订单状态
        /// </summary>
        Busi_OrderStatus,
        /// <summary>
        /// 回执状态配置
        /// </summary>
        Busi_StatusConfig,
        /// <summary>
        /// 报关状态
        /// </summary>
        Busi_DeclStatus,
        //=====================jiang 20160621 添加报价规则明细档
        /// <summary>
        /// 财务管理——报价标准规则明细档
        /// </summary>
        Finance_ReceivableRulesDetail,
        /// <summary>
        /// 财务管理——报价客户规则明细档
        /// </summary>
        Finance_ReceivableRulesCusDetail,
        //================================================================================
    }
}