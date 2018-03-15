var util = {
    //获取url中所传的参数
    getUrlParam: function (param) {
        var params = Ext.urlDecode(location.search.substring(1));
        return param ? params[param] : params;
    },
    //获取ext_onready中的panel：title
    getPanelTitle: function (param) {
        var title = "";
        switch (param) {
            case "insp_portin":
                title = "国内口岸维护";
                break;
            case "insp_portout":
                title = "国际口岸维护";
                break;
            case "base_currency":
                title = "币种代码维护";
                break;
            case "base_inspcountry":
                title = "国家代码维护";
                break;
            case "base_productunit":
                title = "计量单位维护";
                break;
            case "base_insplicense":
                title = "许可证代码";
                break;
            case "base_inspinvoice":
                title = "报检随附单据";
                break;
            case "base_country":
                title = "国别代码";
                break;
            case "base_decltradeway":
                title = "贸易方式";
                break;
            case "base_exemptingnature":
                title = "征免性质";
                break;
            case "base_exchangeway":
                title = "结汇方式";
                break;
            case "base_harbour":
                title = "港口代码";
                break;
            case "base_booksdata":
                title = "账册数据规则";
                break;
            case "sys_repway":
                title = "申报方式";
                break;
            case "base_containersize":
                title = "集装箱尺寸";
                break;
            case "base_containertype":
                title = "集装箱类型";
                break;
            case "sys_woodpacking":
                title = "木质包装";
                break;
            case "sys_declarationcar":
                title = "报告车维护";
                break;
            case "sys_reportlibrary":
                title = "申报库别";
                break;
                

        }
        return title;
    },
    //获取init_search中的searchCode，searchName  //获取grindpanel中前两行
    getInitSearchSearchCode: function(param) {
        var title = "";
        switch (param) {
        case "insp_portin":
            title = "国内口岸代码";
            break;
        case "insp_portout":
            title = "国际口岸代码";
            break;
        case "base_currency":
            title = "币制代码";
            break;
        case "base_inspcountry":
            title = "国别代码";
            break;
        case "base_productunit":
            title = "计量单位代码";
            break;
        case "base_insplicense":
            title = "许可证代码";
            break;
        case "base_inspinvoice":
            title = "随附单据代码";
            break;
        case "base_country":
            title = "国别代码";
            break;
        case "base_decltradeway":
            title = "贸易方式代码";
            break;
        case "base_exemptingnature":
            title = "征免性质代码";
            break;
        case "base_exchangeway":
            title = "结汇方式代码";
            break;
        case "base_harbour":
            title = "港口代码";
            break;
        case "base_booksdata":
            title = "贸易方式代码";
            break;
        case "sys_repway":
            title = "申报方式代码";
            break;
        case "base_containersize":
            title = "集装箱尺寸代码";
            break;
        case "base_containertype":
            title = "集装箱类型代码";
            break;
        case "sys_woodpacking":
            title = "木质包装代码";
            break;
        case "sys_declarationcar":
            title = "车牌号";
            break;
        case "sys_reportlibrary":
            title = "申报库别代码";
            break;
        }
        return title;
    },
    getInitSearchSearchName: function(param) {
        var title = "";
        switch (param) {
            case "insp_portin":
                title = "国内口岸名称";
                break;
            case "insp_portout":
                title = "国际口岸名称";
                break;
            case "base_currency":
                title = "币制名称";
                break;
            case "base_inspcountry":
                title = "国别名称";
                break;
            case "base_productunit":
                title = "计量单位名称";
                break;
            case "base_insplicense":
                title = "进口许可证名称";
                break;
            case "base_inspinvoice":
                title = "进口单据名称";
                break;
            case "base_country":
                title = "国别名称";
                break;
            case "base_decltradeway":
                title = "贸易方式名称";
                break;
            case "base_exemptingnature":
                title = "征免性质名称";
                break;
            case "base_exchangeway":
                title = "结汇方式名称";
                break;
            case "base_harbour":
                title = "港口名称";
                break;
            case "base_booksdata":
                title = "贸易方式名称";
                break;
            case "sys_repway":
                title = "申报方式名称";
                break;
            case "base_containersize":
                title = "集装箱尺寸名称";
                break;
            case "base_containertype":
                title = "集装箱类型名称";
                break;
            case "sys_woodpacking":
                title = "木质包装名称";
                break;
            case "sys_declarationcar":
                title = "白卡号";
                break;
            case "sys_reportlibrary":
                title = "申报库别名称";
                break;
                
        }
        return title;
    },
    //数据绑定获取fields
    getFields: function(param) {
        var arr1 = [
            'ENABLED', 'STARTDATE', 'CREATEMANNAME',
            'CREATEDATE', 'STOPMANNAME', 'ENDDATE', 'REMARK', 'ID'
        ];
        
        switch (param) {
            case "insp_portin": case "base_productunit":
                var arr3 = ['CODE', 'NAME', 'ENGLISHNAME'];
                arr1 = arr1.concat(arr3);
            break;
            case "insp_portout":
                var arr4 = ['CODE', 'NAME', 'ENGLISHNAME', 'COUNTRYNAME', 'COUNTRY'];
                arr1 = arr1.concat(arr4);
                break;
            case "base_currency":
                var arr5 = ['CODE', 'NAME', 'ABBREVIATION'];
                arr1 = arr1.concat(arr5);
                break;
            case "base_inspcountry":
                var arr6 = ['CODE', 'NAME', 'ENGLISHNAME','AIRABBREV','OCEANABBREV'];
                arr1 = arr1.concat(arr6);
                break;
            case "base_insplicense": case "base_inspinvoice":
                var arr7 = ['CODE', 'INNAME', 'OUTNAME'];
                arr1 = arr1.concat(arr7);
                break;
            case "base_country":
                var arr8 = ['CODE', 'EZM', 'NAME', 'ENGLISHNAME','RATE'];
                arr1 = arr1.concat(arr8);
                break;
            case "base_decltradeway": case "base_exemptingnature": case "base_exchangeway":
                var arr9 = ['CODE', 'NAME', 'FULLNAME'];
                arr1 = arr1.concat(arr9);
                break;
            case "base_harbour":
                var arr10 = ['CODE', 'NAME', 'COUNTRYNAME', 'ENGLISHNAME','COUNTRY'];
                arr1 = arr1.concat(arr10);
                break;
            case "base_booksdata":
                var arr11 = ['TRADE','TRADENAME','ISINPORTNAME','ISPRODUCTNAME'];
                arr1 = arr1.concat(arr11);
                break;
            case "sys_repway":
                var arr12 = ['CODE', 'NAME', 'BUSITYPE'];
                arr1 = arr1.concat(arr12);
                break;
            case "base_containersize":
                var arr13 = ['CODE', 'NAME', 'DECLSIZE'];
                arr1 = arr1.concat(arr13);
                break;
            case "base_containertype":
                var arr14 = ['CODE', 'NAME', 'CONTAINERCODE'];
                arr1 = arr1.concat(arr14);
                break;
            case "sys_woodpacking":
                var arr15 = ['CODE','NAME','HSCODE','INSPECTION','DECLARATION'];
                arr1 = arr1.concat(arr15);
                break;
            case "sys_declarationcar":
                var arr16 = ['CODE', 'NAME', 'MODELS', 'MOTORCADENAME', 'MOTORCADE'];
                arr1 = arr1.concat(arr16);
                break;
            case "sys_reportlibrary":
                var arr17 = ['CODE', 'NAME', 'DECLNAME', 'INTERNALTYPE'];
                arr1 = arr1.concat(arr17);
                break;
            
        }
        
        return arr1;
    },
    //获取columns
    getGrindPanelColumn: function(param) {
        var arr1 = [
            { header: '启用情况', dataIndex: 'ENABLED', renderer: gridrender, width: 100 },
            { header: '启用时间', dataIndex: 'STARTDATE', width: 100 },
            { header: '维护人', dataIndex: 'CREATEMANNAME', width: 100 },
            { header: '维护时间', dataIndex: 'CREATEDATE', width: 100 },
            { header: '停用人', dataIndex: 'STOPMANNAME', width: 100 },
            { header: '停用时间', dataIndex: 'ENDDATE', width: 100 },
            { header: '备注', dataIndex: 'REMARK', width: 200 },
            { header: 'ID', dataIndex: 'ID', width: 200, hidden: true }
        ];
        switch (param) {
            case "insp_portin":
                var arr3 = [{ xtype: 'rownumberer', width: 35 }, { header: '口岸代码', dataIndex: 'CODE', width: 100 },
                    { header: '口岸名称', dataIndex: 'NAME', width: 100 }, { header: '英文名称', dataIndex: 'ENGLISHNAME', width: 100 }];
                arr1 = arr3.concat(arr1);
                break;
            case "insp_portout":
                var arr4 = [{ xtype: 'rownumberer', width: 35 }, { header: '口岸代码', dataIndex: 'CODE', width: 100 },
                    { header: '口岸名称', dataIndex: 'NAME', width: 100 }, { header: '英文名称', dataIndex: 'ENGLISHNAME', width: 100 },
                    { header: '所属国家', dataIndex: 'COUNTRYNAME', width: 100 }];
                arr1 = arr4.concat(arr1);
                break;
            case "base_currency":
                var arr5 = [{ xtype: 'rownumberer', width: 35 }, { header: '币制代码', dataIndex: 'CODE', width: 100 },
                    { header: '币制名称', dataIndex: 'NAME', width: 100 }, { header: '币制缩写', dataIndex: 'ABBREVIATION', width: 100 }];
                arr1 = arr5.concat(arr1);
                break;
            case "base_inspcountry":
                var arr6 = [{ xtype: 'rownumberer', width: 35 }, { header: '国家代码', dataIndex: 'CODE', width: 100 },
                    { header: '中文名', dataIndex: 'NAME', width: 100 }, { header: '英文名称', dataIndex: 'ENGLISHNAME', width: 100 },
                    { header: '空运缩写', dataIndex: 'AIRABBREV', width: 100 }, { header: '海运缩写', dataIndex: 'OCEANABBREV', width: 100 }
                ];
                arr1 = arr6.concat(arr1);
                break;
            case "base_productunit":
                var arr7 = [{ xtype: 'rownumberer', width: 35 }, { header: '单位代码', dataIndex: 'CODE', width: 100 },
                    { header: '单位名称', dataIndex: 'NAME', width: 100 }, { header: '英文名称', dataIndex: 'ENGLISHNAME', width: 100 }];
                arr1 = arr7.concat(arr1);
                break;
            case "base_insplicense":
                var arr8 = [{ xtype: 'rownumberer', width: 35 }, { header: '许可证代码', dataIndex: 'CODE', width: 100 },
                    { header: '进口许可证名称', dataIndex: 'INNAME', width: 100 }, { header: '出口许可证名称', dataIndex: 'OUTNAME', width: 100 }];
                arr1 = arr8.concat(arr1);
                break;
            case "base_inspinvoice":
                var arr9 = [{ xtype: 'rownumberer', width: 35 }, { header: '随附单据代码', dataIndex: 'CODE', width: 100 },
                    { header: '进口随附单据名称', dataIndex: 'INNAME', width: 150 }, { header: '出口随附单据名称', dataIndex: 'OUTNAME', width: 150 }];
                arr1 = arr9.concat(arr1);
                break;
            case "base_country":
                var arr10 = [{ xtype: 'rownumberer', width: 35 }, { header: '国家代码', dataIndex: 'CODE', width: 100 },
                    { header: '二字码', dataIndex: 'EZM', width: 150 }, { header: '中文名称', dataIndex: 'NAME', width: 150 }, { header: '英文名称', dataIndex: 'ENGLISHNAME', width: 150 }];
                arr1 = arr10.concat(arr1);
                break;
            case "base_decltradeway":
                var arr11 = [{ xtype: 'rownumberer', width: 35 }, { header: '贸易方式代码', dataIndex: 'CODE', width: 100 },
                    { header: '贸易方式简称', dataIndex: 'NAME', width: 100 }, { header: '贸易方式全称', dataIndex: 'FULLNAME', width: 100 }];
                arr1 = arr11.concat(arr1);
                break;
            case "base_exemptingnature":
                var arr12 = [{ xtype: 'rownumberer', width: 35 }, { header: '征免性质代码', dataIndex: 'CODE', width: 100 },
                    { header: '征免性质简称', dataIndex: 'NAME', width: 100 }, { header: '征免性质全称', dataIndex: 'FULLNAME', width: 100 }];
                arr1 = arr12.concat(arr1);
                break;
            case "base_exchangeway":
                var arr13 = [{ xtype: 'rownumberer', width: 35 }, { header: '结汇方式代码', dataIndex: 'CODE', width: 100 },
                    { header: '结汇方式简称', dataIndex: 'NAME', width: 100 }, { header: '结汇方式全称', dataIndex: 'FULLNAME', width: 100 }];
                arr1 = arr13.concat(arr1);
                break;
            case "base_harbour":
                var arr14 = [{ xtype: 'rownumberer', width: 35 }, { header: '港口代码', dataIndex: 'CODE', width: 100 },
                    { header: '港口名称', dataIndex: 'NAME', width: 100 }, { header: '所属国家', dataIndex: 'COUNTRYNAME', width: 100 }, { header: '英文名称', dataIndex: 'ENGLISHNAME', width: 100 }];
                arr1 = arr14.concat(arr1);
                break;
            case "base_booksdata":
                var arr15 = [{ xtype: 'rownumberer', width: 35 }, { header: '贸易方式代码', dataIndex: 'TRADE', width: 100 },
                    { header: '贸易方式名称', dataIndex: 'TRADENAME', width: 100 }, { header: '进口/出口', dataIndex: 'ISINPORTNAME', width: 100 }, { header: '成品/料件', dataIndex: 'ISPRODUCTNAME', width: 100 }];
                arr1 = arr15.concat(arr1);
                break;
            case "sys_repway":
                var arr16 = [{ xtype: 'rownumberer', width: 35 }, { header: '申报方式代码', dataIndex: 'CODE', width: 100 },
                    { header: '申报方式名称', dataIndex: 'NAME', width: 100 }, { header: '业务类型', dataIndex: 'BUSITYPE', width: 100 }];
                arr1 = arr16.concat(arr1);
                break;
            case "base_containersize":
                var arr17 = [{ xtype: 'rownumberer', width: 35 }, { header: '集装箱尺寸代码', dataIndex: 'CODE', width: 100 },
                    { header: '集装箱尺寸', dataIndex: 'NAME', width: 100 }, { header: '集装箱申报尺寸', dataIndex: 'DECLSIZE', width: 100 }];
                arr1 = arr17.concat(arr1);
                break;
            case "base_containertype":
                var arr18 = [{ xtype: 'rownumberer', width: 35 }, { header: '集装箱类型代码', dataIndex: 'CODE', width: 100 },
                    { header: '集装箱类型', dataIndex: 'NAME', width: 100 }, { header: '集装箱编码', dataIndex: 'CONTAINERCODE', width: 100 }];
                arr1 = arr18.concat(arr1);
                break;
            case "sys_woodpacking":
                var arr19 = [{ xtype: 'rownumberer', width: 35 }, { header: '木质包装代码', dataIndex: 'CODE', width: 100 },
                    { header: '木质包装名称', dataIndex: 'NAME', width: 100 }, { header: '木质包装HS编码', dataIndex: 'HSCODE', width: 100 },
                    { header: '检验检疫类别', dataIndex: 'INSPECTION', width: 100 }, { header: '海关监管条件', dataIndex: 'DECLARATION', width: 100 }];
                arr1 = arr19.concat(arr1);
                break;
            case "sys_declarationcar":
                var arr20 = [{ xtype: 'rownumberer', width: 35 }, { header: '车牌号', dataIndex: 'CODE', width: 100 },
                    { header: '白卡号', dataIndex: 'NAME', width: 100 }, { header: '车型', dataIndex: 'MODELS', width: 100 }, { header: '车队', dataIndex: 'MOTORCADENAME', width: 100 }];
                arr1 = arr20.concat(arr1);
                break;
            case "sys_reportlibrary":
                var arr21 = [{ xtype: 'rownumberer', width: 35 }, { header: '申报库别代码', dataIndex: 'CODE', width: 100 },
                    { header: '申报库别名称', dataIndex: 'NAME', width: 100 }, { header: '报关单名称', dataIndex: 'DECLNAME', width: 100 }, { header: '进出口类型', dataIndex: 'INTERNALTYPE', width: 100 }];
                arr1 = arr21.concat(arr1);
                break;
                

                

        }
        return arr1;
    },

    //判断字符串是否在数组中
    IfContains: function(arr, obj) {
        var i = arr.length;
        while (i--) {
            if (arr[i] === obj) {
                return true;
            }
        }
        return false;
    }

}