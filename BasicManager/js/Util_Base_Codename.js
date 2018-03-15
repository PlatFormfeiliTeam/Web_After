var util = {
    //获取url中所传的参数
    getUrlParam: function(param) {
        var params = Ext.urlDecode(location.search.substring(1));
        return param ? params[param] : params;
    },
    //获取ext_onready中的panel：title
    getPanelTitle: function (param) {
        var title = "";
        switch (param) {
            case "sys_reguareatype":
                title = "特殊监管区";
                break;
            case "base_tradeway": 
                title = "贸易方式";
                break;
            case "base_insppackage":
                title = "包装类别";
                break;
            case "base_inspectionagency":
                title = "检验检疫机构";
                break;
            case "base_inspconveyance":
                title = "运输工具";
                break;
            case "sys_InspType":
                title = "报检类别";
                break;
            case "base_withinregion":
                title = "境内地区";
                break;
            case "base_needdocument":
                title = "所需单据";
                break;
            case "base_wastegoods":
                title = "废旧物品";
                break;
            case "base_inspuse":
                title = "商检用途";
                break;
            case "base_inspcompanynature":
                title = "企业性质";
                break;
            case "base_reportregion":
                title = "申报区域";
                break;
            case "base_inspectflag":
                title = "法检标志";
                break;
            case "base_transport":
                title = "运输方式";
                break;
            case "base_shipping_destination":
                title = "境内区域";
                break;
            case "base_transaction":
                title = "成交方式";
                break;
            case "base_declcurrency":
                title = "币制代码";
                break;
            case "base_declfee":
                title = "费用形式";
                break;
            case "base_packing":
                title = "包装种类";
                break;
            case "base_declproductunit":
                title = "计量单位";
                break;
            case "base_exemptingway":
                title = "征免方式";
                break;
            case "sys_companynature":
                title = "企业性质";
                break;
            case "base_invoice":
                title = "随附单据";
                break;
            case "base_decluse":
                title = "用途代码";
                break;
            case "base_customdistrict":
                title = "关区代码";
                break;
            case "sys_status":
                title = "显示状态";
                break;
            case "sys_busitype":
                title = "业务类型";
                break;
            case "base_motorcade":
                title = "车队信息";
                break;
            case "sys_declway":
                title = "报关方式";
                break;
            case "sys_inspLibrary":
                title = "报检库别";
                break;
            case "sys_NoticeType":
                title = "通知类别";
                break;
            case "sys_goodstype":
                title = "货物类别";
                break;
            case "base_consigneetype":
                title = "收货人类型";
                break;
            case "base_listtype":
                title = "清单类型";
                break;
            case "base_assistkind":
                title = "辅助选项";
                break;
            case "base_orderstatus":
                title = "业务状态";
                break;
            case "base_declstatus":
                title = "报关状态";
                break;


        }
        return title;
    },
    //获取init_search中的searchCode，searchName  //获取grindpanel中前两行
    getInitSearchSearchCode: function(param) {
        var title = "";
        switch (param) {
        case "sys_reguareatype":
            title = "特殊监管区代码";
            break;
        case "base_tradeway":
            title = "贸易方式代码";
            break;
        case "base_insppackage":
            title = "包装类别代码";
            break;
        case "base_inspectionagency":
            title = "机构代码";
            break;
            case "base_inspconveyance":
            title = "运输工具代码";
            break;
        case "sys_InspType":
            title = "报检类别代码";
            break;
        case "base_withinregion":
            title = "境内地区代码";
            break;
        case "base_needdocument":
            title = "所需单据代码";
            break;
        case "base_wastegoods":
            title = "废旧物品代码";
            break;
        case "base_inspuse":
            title = "商检用途代码";
            break;
        case "base_inspcompanynature":
            title = "企业性质代码";
            break;
        case "base_reportregion":
            title = "申报区域代码";
            break;
        case "base_inspectflag":
            title = "法检标志代码";
            break;
        case "base_transport":
            title = "运输方式代码";
            break;
        case "base_shipping_destination":
            title = "境内区域代码";
            break;
        case "base_transaction":
            title = "成交方式代码";
            break;
        case "base_declcurrency":
            title = "币制代码";
            break;
        case "base_declfee":
            title = "费用形式代码";
            break;
        case "base_packing":
            title = "包装种类代码";
            break;
        case "base_declproductunit":
            title = "计量单位代码";
            break;
        case "base_exemptingway":
            title = "征免方式代码";
            break;
        case "sys_companynature":
            title = "企业性质代码";
            break;
        case "base_invoice":
            title = "随附单据代码";
            break;
        case "base_decluse":
            title = "用途代码";
            break;
        case "base_customdistrict":
            title = "关区代码";
            break;
        case "sys_status":
            title = "显示状态代码";
            break;
        case "sys_busitype":
            title = "业务类型代码";
            break;
        case "base_motorcade":
            title = "车队信息代码";
            break;
        case "sys_declway":
            title = "报关方式代码";
            break;
        case "sys_inspLibrary":
            title = "报检库别代码";
            break;
        case "sys_NoticeType":
            title = "通知类别代码";
            break;
        case "sys_goodstype":
            title = "货物类别代码";
            break;
        case "base_consigneetype":
            title = "收货人类型代码";
            break;
        case "base_listtype":
            title = "清单类型代码";
            break;
        case "base_assistkind":
            title = "辅助选项代码";
            break;
        case "base_orderstatus":
            title = "业务状态代码";
            break;
        case "base_declstatus":
            title = "报关状态代码";
            break;
        }
        return title;
    },
    getInitSearchSearchName: function (param) {
        var title = "";
        switch (param) {
        case "sys_reguareatype":
            title = "特殊监管区名称";
            break;
        case "base_tradeway":
            title = "贸易方式名称";
            break;
        case "base_insppackage":
            title = "包装类别名称";
            break;
        case "base_inspectionagency":
            title = "机构名称";
            break;
        case "base_inspconveyance":
            title = "运输工具名称";
            break;
        case "sys_InspType":
            title = "报检类别名称";
            break;
        case "base_withinregion":
            title = "境内地区名称";
            break;
        case "base_needdocument":
            title = "所需单据名称";
            break;
        case "base_wastegoods":
            title = "废旧物品名称";
            break;
        case "base_inspuse":
            title = "商检用途名称";
            break;
        case "base_inspcompanynature":
            title = "企业性质名称";
            break;
        case "base_reportregion":
            title = "申报区域名称";
            break;
        case "base_inspectflag":
            title = "法检标志名称";
            break;
        case "base_transport":
            title = "运输方式名称";
            break;
        case "base_shipping_destination":
            title = "境内区域名称";
            break;
        case "base_transaction":
            title = "成交方式名称";
            break;
        case "base_declcurrency":
            title = "币制代码名称";
            break;
        case "base_declfee":
            title = "费用形式名称";
            break;
        case "base_packing":
            title = "包装种类名称";
            break;
        case "base_declproductunit":
            title = "计量单位名称";
            break;
        case "base_exemptingway":
            title = "征免方式名称";
            break;
        case "sys_companynature":
            title = "企业性质名称";
            break;
        case "base_invoice":
            title = "随附单据名称";
            break;
        case "base_decluse":
            title = "用途代码名称";
            break;
        case "base_customdistrict":
            title = "关区代码名称";
            break;
        case "sys_status":
            title = "显示状态名称";
            break;
        case "sys_busitype":
            title = "业务类型名称";
            break;
        case "base_motorcade":
            title = "车队信息名称";
            break;
        case "sys_declway":
            title = "报关方式名称";
            break;
        case "sys_inspLibrary":
            title = "报检库别名称";
            break;
        case "sys_NoticeType":
            title = "通知类别名称";
            break;
        case "sys_goodstype":
            title = "货物类别名称";
            break;
        case "base_consigneetype":
            title = "收货人类型名称";
            break;
        case "base_listtype":
            title = "清单类型名称";
            break;
        case "base_assistkind":
            title = "辅助选项名称";
            break;
        case "base_orderstatus":
            title = "业务状态名称";
            break;
        case "base_declstatus":
            title = "报关状态名称";
            break;
        }
        return title;
    },
    //获取gridpanel中的colums
    getGrindPanelColumsOne: function(param) {
        var arr = new Array();
        switch (param) {
            case "sys_reguareatype": case "base_tradeway":
            case "base_insppackage": case "base_inspectionagency":
            case "base_inspconveyance": case "sys_InspType":
            case "base_withinregion": case "base_needdocument":
            case "base_wastegoods": case "base_inspuse":
            case "base_inspcompanynature": case "base_reportregion":
            case "base_inspectflag": case "base_transport":
            case "base_shipping_destination": case "base_transaction":
            case "base_declcurrency":  case "base_declfee":
            case "base_packing": case "base_declproductunit":
            case "base_exemptingway": case "sys_companynature":
            case "base_invoice": case "base_decluse":
            case "base_customdistrict": case "sys_status":
            case "sys_busitype": case "base_motorcade":
            case "sys_declway": case "sys_inspLibrary":
            case "sys_NoticeType": case "sys_goodstype":
            case "base_consigneetype": case "base_listtype":
            case "base_assistkind": case "base_orderstatus":
            case "base_declstatus":
            arr[0] = "CODE";
            arr[1] = "NAME";
            break;
                   
        }
        return arr;
    },

    


}