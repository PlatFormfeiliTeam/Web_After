<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Base_blend_web.aspx.cs" Inherits="Web_After.BasicManager.InspInfor.Base_blend_web" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />    
    <script src="/js/import/importExcel.js" type="text/javascript"></script>
    <script src="/BasicManager/js/Util_Base_blend_web.js"></script>
    <script type="text/javascript">
        var username = '<%=Username()%>';
        var param = util.getUrlParam('param');
        console.log(param);
        //获取title:
        var title = util.getPanelTitle(param);
        //获取searchCode,searchName: //获取grindpanel
        var getSearchCode = util.getInitSearchSearchCode(param);
        var getSearchName = util.getInitSearchSearchName(param);
        var CountryName_ENABLED;
        var data1 = [];
        


        Ext.onReady(function () {
            Ext.Ajax.request({
                url: 'Base_blend_web.aspx?table='+param+'',
                params: { action: 'countryname' },
                type: 'Post',
                success: function (response, option) {
                    var commondata = Ext.decode(response.responseText);                                      
                    data1 = commondata;
                    //console.log(data1);
                    init_search(param);
                    gridbind(param);

                    var panel = Ext.create('Ext.form.Panel', {
                        title: title,
                        region: 'center',
                        layout: 'border',
                        items: [Ext.getCmp('formpanel_search'), Ext.getCmp('gridpanel')]
                    });
                    var viewport = Ext.create('Ext.container.Viewport',
                        {
                            layout: 'border',
                            items: [panel]
                        });
                }

            });

            
        });

        function init_search(param) {

            var searchCode = Ext.create('Ext.form.field.Text', { id: 'CODE_S', name: 'CODE_S', fieldLabel: getSearchCode });
            var searchName = Ext.create('Ext.form.field.Text', { id: 'CNNAME_S', name: 'CNNAME_S', fieldLabel: getSearchName });
                        
            var store_ENABLED_S = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
            });
            var combo_ENABLED_S = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_ENABLED_S',
                name: 'ENABLED_S',
                store: store_ENABLED_S,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '是否启用',
                displayField: 'NAME',
                valueField: 'CODE'
            });
            var arr4 = [searchCode, searchName, combo_ENABLED_S];

            //*************************************添加搜索组件************************************************
            switch (param) {           
                case "base_insplicense":
                    var searchOutName = Ext.create('Ext.form.field.Text', { id: 'searchOutName', name: 'searchOutName', fieldLabel: '出口许可证名称' });
                    arr4 = [searchCode, searchName, searchOutName, combo_ENABLED_S];
                    break;
                case "base_inspinvoice":
                    var searchOutName = Ext.create('Ext.form.field.Text', { id: 'searchOutName', name: 'searchOutName', fieldLabel: '出口单据名称' });
                    arr4 = [searchCode, searchName, searchOutName, combo_ENABLED_S];
                    break;
            }

            //*********************************************************************************************
            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { addCustomer_Win("", "", param); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改', width: 80, handler: function () { editCustomer(param); } }
                    //, { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { del(); } }
                    , { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入', width: 80, handler: function () { importfile('add', param); } }
                    , { text: '<span class="icon iconfont">&#xe625;</span>&nbsp;导 出', handler: function () { exportdata(param); } }
                    , '->'
                    , { text: '<span class="icon iconfont">&#xe60b;</span>&nbsp;查 询', width: 80, handler: function () { Ext.getCmp("pgbar").moveFirst(); } }
                    , { text: '<span class="icon iconfont">&#xe633;</span>&nbsp;重 置', width: 80, handler: function () { reset(); } }
                ]
            });

            var formpanel_search = Ext.create('Ext.form.Panel', {
                id: 'formpanel_search',
                region: 'north',
                border: 0,
                bbar: toolbar,
                fieldDefaults: {
                    margin: '5',
                    columnWidth: 0.25,
                    labelWidth: 95
                },
                items: [
                    { layout: 'column', border: 0, items: arr4 }

                ]
            });
        }


        //数据绑定
        function gridbind(param) {
            var store_customer = Ext.create('Ext.data.JsonStore',
                {
                    fields: util.getFields(param),
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'Base_blend_web.aspx?action=loadData&table=' + param,
                        reader: {
                            root: 'rows',
                            type: 'json',
                            totalProperty: 'total'
                        }
                    },
                    autoLoad: true,
                    listeners: {
                        beforeload: function (store, options) {
                            store_customer.getProxy().extraParams =
                                Ext.getCmp('formpanel_search').getForm().getValues();
                        }
                    }
                });
            var pgbar = Ext.create('Ext.toolbar.Paging',
                {
                    id: 'pgbar',
                    displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                    store: store_customer,
                    displayInfo: true
                });
            var gridpanel = Ext.create('Ext.grid.Panel', {
                id: 'gridpanel',
                height: 560,
                region: 'center',
                store: store_customer,
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                columns:util.getGrindPanelColumn(param),
                listeners:
                {
                    'itemdblclick': function (view, record, item, index, e) {
                        editCustomer(param);
                    }
                },
                viewConfig: {
                    enableTextSelection: true
                }
            });
        }
        //是否按钮转勾
        function gridrender(value, cellmeta, record, rowIndex, columnIndex, stroe) {
            var dataindex = cellmeta.column.dataIndex;
            var str = "";
            switch (dataindex) {
            case "ENABLED":
                str = value == "1" ? '<span class="icon iconfont" style="font-size:12px;color:blue;">&#xe628;</span>' : '<span class="icon iconfont" style="font-size:12px;color:red;">&#xe634;</span>';
                break;
            }
            return str;
        }
        //重置按钮
        function reset() {
            Ext.each(Ext.getCmp('formpanel_search').getForm().getFields().items,
                function (field) {
                    field.reset();
                });
        }
        //新增界面组件
        function form_ini_win(param,ID) {
            var field_ID = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });

            if (param != "base_booksdata") {
                var field_Code = Ext.create('Ext.form.field.Text',
                    {
                        id: 'CODE',
                        name: 'CODE',
                        fieldLabel: getSearchCode,
                        flex: .5,
                        allowBlank: false,
                        blankText: getSearchCode + '不可为空!'
                    });
            } else {
                var TradeName_ENABLED_s = Ext.create('Ext.data.JsonStore', {
                    fields: ['CODE', 'NAME'],
                    data: data1
                });

                var field_TradeName = Ext.create('Ext.form.field.ComboBox', {
                    id: 'TradeName_ENABLED',
                    name: 'TRADE',
                    store: TradeName_ENABLED_s,
                    queryMode: 'local',
                    anyMatch: true,
                    fieldLabel: '贸易方式', flex: .5,
                    displayField: 'NAME',
                    valueField: 'CODE',
                    allowBlank: false,
                    blankText: '贸易方式不能为空!'
                });
            }


            //**********************************根据表来创建组件***********************************************
            

            var s = ['insp_portin', 'insp_portout', 'base_currency', 'base_inspcountry', 'base_productunit', 'base_country', 'base_decltradeway', 'base_exemptingnature', 'base_exchangeway', 'base_harbour', 'sys_repway', 'base_containersize', 'base_containertype', 'sys_woodpacking', 'sys_declarationcar', 'sys_reportlibrary'];
            
            if (util.IfContains(s,param)) {
                
                var field_Name = Ext.create('Ext.form.field.Text', {
                    id: 'NAME',
                    name: 'NAME',
                    fieldLabel: getSearchName,
                    flex: .5,
                    allowBlank: false,
                    blankText: getSearchName + '不可为空!'
                });
            }
            
            
            switch (param) {
                
                case "insp_portin": case "base_productunit":
                    var field_EnglishName = Ext.create('Ext.form.field.Text', {
                        id: 'ENGLISHNAME',
                        name: 'ENGLISHNAME',
                        fieldLabel: '英文名称',
                        flex: .5
                    });
                    
                    break;
                case "insp_portout":
                    var field_EnglishName = Ext.create('Ext.form.field.Text', {
                        id: 'ENGLISHNAME',
                        name: 'ENGLISHNAME',
                        fieldLabel: '英文名称',
                        flex: .5
                    });
                    var CountryName_ENABLED_s = Ext.create('Ext.data.JsonStore', {
                        fields: ['CODE', 'NAME'],
                        data: data1
                    });

                    var field_CountryName = Ext.create('Ext.form.field.ComboBox', {
                        id: 'CountryName_ENABLED',
                        name: 'COUNTRY',
                        store: CountryName_ENABLED_s,
                        queryMode: 'local',
                        anyMatch: true,
                        fieldLabel: '所属国家', flex: .5,
                        displayField: 'NAME',
                        valueField: 'CODE',                        
                        allowBlank: false,
                        blankText: '所属国家不能为空!'
                    });
                    
                    break;
                case "base_currency":
                    var field_EnglishName = Ext.create('Ext.form.field.Text', {
                        id: 'ABBREVIATION',
                        name: 'ABBREVIATION',
                        fieldLabel: '英文缩写',
                        flex: .5
                    });
                    
                    break;
                case "base_inspcountry": 
                    var field_EnglishName = Ext.create('Ext.form.field.Text', {
                        id: 'ENGLISHNAME',
                        name: 'ENGLISHNAME',
                        fieldLabel: '英文名称',
                        flex: .5
                    });
                    var field_Airabbrev = Ext.create('Ext.form.field.Text', {
                        id: 'AIRABBREV',
                        name: 'AIRABBREV',
                        fieldLabel: '空运缩写',
                        flex: .5
                    });
                    var field_Oceanabbrev = Ext.create('Ext.form.field.Text', {
                        id: 'OCEANABBREV',
                        name: 'OCEANABBREV',
                        fieldLabel: '海运缩写',
                        flex: .5
                    });
                    
                    break;
                case "base_insplicense":
                    var field_InName = Ext.create('Ext.form.field.Text', {
                        id: 'INNAME',
                        name: 'INNAME',
                        fieldLabel: '进口许可证名称',
                        flex: .5
                    });
                    var field_OutName = Ext.create('Ext.form.field.Text', {
                        id: 'OUTNAME',
                        name: 'OUTNAME',
                        fieldLabel: '出口许可证名称',
                        flex: .5
                    });
                    break;
                case "base_inspinvoice":
                     field_InName = Ext.create('Ext.form.field.Text', {
                        id: 'INNAME',
                        name: 'INNAME',
                        fieldLabel: '进口单据名称',
                        flex: .5
                    });
                     field_OutName = Ext.create('Ext.form.field.Text', {
                        id: 'OUTNAME',
                        name: 'OUTNAME',
                        fieldLabel: '出口单据名称',
                        flex: .5
                    });
                     break;
                case "base_country":
                    field_EnglishName = Ext.create('Ext.form.field.Text', {
                        id: 'ENGLISHNAME',
                        name: 'ENGLISHNAME',
                        fieldLabel: '英文名称',
                        flex: .5
                    });

                    var field_rate = Ext.create('Ext.form.field.Text', {
                        id: 'RATE',
                        name: 'RATE',
                        fieldLabel: '优/普税率',
                        flex: .5
                    });

                    var field_ezm = Ext.create('Ext.form.field.Text', {
                        id: 'EZM',
                        name: 'EZM',
                        fieldLabel: '二字码',
                        flex: .5
                    });
                    break;
                case "base_decltradeway":
                    var filed_fullName = Ext.create('Ext.form.field.Text', {
                        id: 'FULLNAME',
                        name: 'FULLNAME',
                        fieldLabel: '贸易方式全称',
                        flex: .5
                    });
                    break;
                case "base_exemptingnature":
                    filed_fullName = Ext.create('Ext.form.field.Text', {
                        id: 'FULLNAME',
                        name: 'FULLNAME',
                        fieldLabel: '征免性质全称',
                        flex: .5
                    });
                    break;
                case "base_exchangeway":
                    filed_fullName = Ext.create('Ext.form.field.Text', {
                        id: 'FULLNAME',
                        name: 'FULLNAME',
                        fieldLabel: '结汇方式全称',
                        flex: .5
                    });
                    break;
                case "base_harbour":
                     field_EnglishName = Ext.create('Ext.form.field.Text', {
                        id: 'ENGLISHNAME',
                        name: 'ENGLISHNAME',
                        fieldLabel: '英文名称',
                        flex: .5
                    });
                    var CountryName_ENABLED_s = Ext.create('Ext.data.JsonStore', {
                        fields: ['CODE', 'NAME'],
                        data: data1
                    });

                    var field_CountryName = Ext.create('Ext.form.field.ComboBox', {
                        id: 'CountryName_ENABLED',
                        name: 'COUNTRY',
                        store: CountryName_ENABLED_s,
                        queryMode: 'local',
                        anyMatch: true,
                        fieldLabel: '所属国家', flex: .5,
                        displayField: 'NAME',
                        valueField: 'CODE',
                        allowBlank: false,
                        blankText: '所属国家不能为空!'
                    });

                    break;
                case "base_booksdata":
                    var store_ISINPORTNAME = Ext.create('Ext.data.JsonStore', {
                        fields: ['CODE', 'NAME'],
                        data: [{ "CODE": 0, "NAME": "进口" }, { "CODE": 1, "NAME": "出口" }]
                    });
                    var combo_ISINPORTNAME = Ext.create('Ext.form.field.ComboBox', {
                        id: 'combo_ISINPORTNAME',
                        name: 'ISINPORTNAME',
                        store: store_ISINPORTNAME,
                        queryMode: 'local',
                        anyMatch: true,
                        fieldLabel: '进/出口', flex: .5,
                        displayField: 'NAME',
                        valueField: 'NAME',
                        allowBlank: false,
                        blankText: '进/出口不能为空!'
                    });

                    var store_ISPRODUCTNAME = Ext.create('Ext.data.JsonStore', {
                        fields: ['CODE', 'NAME'],
                        data: [{ "CODE": 0, "NAME": "成品" }, { "CODE": 1, "NAME": "料件" }]
                    });
                    var combo_ISPRODUCTNAME = Ext.create('Ext.form.field.ComboBox', {
                        id: 'combo_ISPRODUCTNAME',
                        name: 'ISPRODUCTNAME',
                        store: store_ISPRODUCTNAME,
                        queryMode: 'local',
                        anyMatch: true,
                        fieldLabel: '成品/料件', flex: .5,
                        displayField: 'NAME',
                        valueField: 'NAME',                        
                        allowBlank: false,
                        blankText: '成品/料件不能为空!'
                    });                
                    break;
                case "sys_repway":
                    var fileld_busitype = Ext.create('Ext.form.field.Text', {
                        id: 'BUSITYPE',
                        name: 'BUSITYPE',
                        fieldLabel: '业务类型',
                        flex: .5
                    });
                    break;
                case "base_containertype":
                    var fileld_containercode = Ext.create('Ext.form.field.Text', {
                        id: 'CONTAINERCODE',
                        name: 'CONTAINERCODE',
                        fieldLabel: '集装箱编码',
                        flex: .5
                    });
                    break;
                case "base_containersize":
                    var fileld_declsize = Ext.create('Ext.form.field.Text', {
                        id: 'DECLSIZE',
                        name: 'DECLSIZE',
                        fieldLabel: '集装箱编码',
                        flex: .5
                    });
                    break;
                case "sys_woodpacking":
                    var HSCODE_ENABLED_s = Ext.create('Ext.data.JsonStore', {
                        fields: ['HSCODE', 'HSNAME'],
                        data: data1
                    });

                    var field_HSCODEName = Ext.create('Ext.form.field.ComboBox', {
                        id: 'HSCODE_ENABLED',
                        name: 'HSCODE',
                        store: HSCODE_ENABLED_s,
                        queryMode: 'local',
                        anyMatch: true,
                        fieldLabel: 'HS编码', flex: .5,
                        displayField: 'HSNAME',
                        valueField: 'HSCODE',
                        allowBlank: false,
                        blankText: 'HS编码不能为空!'
                    });
                    break;
                case "sys_declarationcar":
                    var fileld_models = Ext.create('Ext.form.field.Text', {
                        id: 'MODELS',
                        name: 'MODELS',
                        fieldLabel: '车型',
                        flex: .5
                    });


                    var MOTORCAD_ENABLED_s = Ext.create('Ext.data.JsonStore', {
                        fields: ['CODE', 'NAME'],
                        data: data1
                    });

                    var field_MOTORCADName = Ext.create('Ext.form.field.ComboBox', {
                        id: 'HSCODE_ENABLED',
                        name: 'MOTORCADE',
                        store: MOTORCAD_ENABLED_s,
                        queryMode: 'local',
                        anyMatch: true,
                        fieldLabel: '车队', flex: .5,
                        displayField: 'NAME',
                        valueField: 'CODE',
                        allowBlank: false,
                        blankText: '车队不能为空!'
                    });
                    break;
                case "sys_reportlibrary":
                    var field_declname = Ext.create('Ext.form.field.Text', {
                        id: 'DECLNAME',
                        name: 'DECLNAME',
                        fieldLabel: '报关单名称',
                        flex: .5
                    });

                    var store_internaltype = Ext.create('Ext.data.JsonStore', {
                        fields: ['CODE', 'NAME'],
                        data: [{ "CODE": 0, "NAME": "进口" }, { "CODE": 1, "NAME": "出口" }]
                    });
                    var combo_internaltype = Ext.create('Ext.form.field.ComboBox', {
                        id: 'combo_ISINPORTNAME',
                        name: 'INTERNALTYPE',
                        store: store_internaltype,
                        queryMode: 'local',
                        anyMatch: true,
                        fieldLabel: '进/出口类型', flex: .5,
                        displayField: 'NAME',
                        valueField: 'NAME',
                        allowBlank: false,
                        blankText: '进/出口不能为空!'
                    });
                    
                    break;
                   

            }
            //***********************************************************************************************

            var store_ENABLED = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
            });
            var combo_ENABLED = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_ENABLED',
                name: 'ENABLED',
                store: store_ENABLED,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '是否启用', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE',
                value: 1,
                allowBlank: false,
                blankText: '是否启用不能为空!'
            });

            var start_date = Ext.create('Ext.form.field.Date',
                {
                    id: 'STARTDATE',
                    name: 'STARTDATE',
                    format: 'Y-m-d',
                    fieldLabel: '启用日期',
                    flex: .5

                });

            var end_date = Ext.create('Ext.form.field.Date',
                {
                    id: 'ENDDATE',
                    name: 'ENDDATE',
                    format: 'Y-m-d',
                    fieldLabel: '停用日期',
                    flex: .5
                });
            var CreatemanName = Ext.create('Ext.form.field.Text', {
                id: 'CREATEMANNAME',
                name: 'CREATEMANNAME',
                fieldLabel: '维护人',
                readOnly: true
            });



            var field_REMARK = Ext.create('Ext.form.field.Text', {
                id: 'REMARK',
                name: 'REMARK',
                fieldLabel: '备注'
            });

            
            //*********************************items**************************************************************************************
                var arr = [
                    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_Code, field_Name] }
                ];
            var arr1 = [];
            var remark = { layout: 'column', height: 42, border: 0, items: [field_REMARK] };
            switch (param) {
                case "insp_portin":  case "base_productunit":
                     arr1 = [{ layout: 'column', height: 42, border: 0, items: [field_EnglishName, start_date] },
                        { layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, field_REMARK] }
                                
                    ];
                    arr = arr.concat(arr1);                    
                    break;
                case "insp_portout": case "base_harbour":
                     arr1 = [{ layout: 'column', height: 42, border: 0, items: [field_EnglishName, field_CountryName] },
                                { layout: 'column', height: 42, border: 0, items: [start_date, end_date] },
                                { layout: 'column', height: 42, border: 0, items: [CreatemanName, combo_ENABLED] },
                                remark
                    ];
                    arr = arr.concat(arr1);
                    break;
                case "base_currency":
                    arr1 = [{ layout: 'column', height: 42, border: 0, items: [field_EnglishName, start_date] },
                        { layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, field_REMARK] }

                    ];
                    arr = arr.concat(arr1);
                    break;
                case "base_inspcountry":
                    arr1 = [
                        { layout: 'column', height: 42, border: 0, items: [field_EnglishName, field_Airabbrev] },
                        { layout: 'column', height: 42, border: 0, items: [field_Oceanabbrev, start_date] },
                        { layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, field_REMARK] }
                    ];
                    arr = arr.concat(arr1);
                    break;
                
                case "base_insplicense":  case "base_inspinvoice":
                    arr = [{ layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_Code, field_InName] }];
                    arr1 = [
                        { layout: 'column', height: 42, border: 0, items: [field_OutName, start_date] },
                        { layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, field_REMARK] }
                    ];
                    arr = arr.concat(arr1);
                    break;

                case "base_country":
                    arr1 = [
                        { layout: 'column', height: 42, border: 0, items: [field_EnglishName, field_rate] },
                        { layout: 'column', height: 42, border: 0, items: [field_ezm, start_date] },
                        { layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, field_REMARK] }
                    ];
                    arr = arr.concat(arr1);
                    break;
                case "base_decltradeway": case "base_exemptingnature": case "base_exchangeway":
                    arr1 = [
                        { layout: 'column', height: 42, border: 0, items: [filed_fullName, start_date] },
                        { layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, field_REMARK] }
                    ];
                    arr = arr.concat(arr1);
                    break;
                case "base_booksdata":
                    arr = [{ layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_TradeName, combo_ISINPORTNAME] },
                        { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_ISPRODUCTNAME, start_date] },
                        { layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, field_REMARK] }
                    ];
                    break;
                case "sys_repway":
                    arr1 = [
                        { layout: 'column', height: 42, border: 0, items: [fileld_busitype, start_date] },
                        { layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, field_REMARK] }
                    ];
                    arr = arr.concat(arr1);
                    break;
                case "base_containersize":
                    arr1 = [
                        { layout: 'column', height: 42, border: 0, items: [fileld_declsize, start_date] },
                        { layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, field_REMARK] }
                    ];
                    arr = arr.concat(arr1);
                    break;
                case "base_containertype":
                    arr1 = [
                        { layout: 'column', height: 42, border: 0, items: [fileld_containercode, start_date] },
                        { layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, field_REMARK] }
                    ];
                    arr = arr.concat(arr1);
                    break;
                case "sys_woodpacking":
                    arr1 = [
                        { layout: 'column', height: 42, border: 0, items: [field_HSCODEName, start_date] },
                        { layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, field_REMARK] }
                    ];
                    arr = arr.concat(arr1);
                    break;
                case "sys_declarationcar":
                    arr1 = [{ layout: 'column', height: 42, border: 0, items: [fileld_models, field_MOTORCADName] },
                        { layout: 'column', height: 42, border: 0, items: [start_date, end_date] },
                        { layout: 'column', height: 42, border: 0, items: [CreatemanName, combo_ENABLED] },
                        remark
                    ];
                    arr = arr.concat(arr1);
                    break;
                case "sys_reportlibrary":
                    arr1 = [{ layout: 'column', height: 42, border: 0, items: [field_declname, combo_internaltype] },
                        { layout: 'column', height: 42, border: 0, items: [start_date, end_date] },
                        { layout: 'column', height: 42, border: 0, items: [CreatemanName, combo_ENABLED] },
                        remark
                    ];
                    arr = arr.concat(arr1);
                    break;

            }
            
            if (ID !=  "") {
                //修改原因输入框
                var change_reason = Ext.create('Ext.form.field.Text', {
                    id: 'REASON',
                    name: 'REASON',
                    fieldLabel: '修改原因',
                    flex: .5,
                    allowBlank: false,
                    blankText : '修改原因不可为空!'
                });
                switch (param) {
                    case "insp_portin": case "base_productunit": case "base_insplicense": case "base_currency": case "base_inspcountry":
                    case "base_inspinvoice": case "base_country": case "base_decltradeway": case "base_exemptingnature": case "base_exchangeway": case "base_booksdata":
                    case "sys_repway": case "base_containersize": case "base_containertype": case "sys_woodpacking":
                        var insp_portin = [{ layout: 'column', height: 42, border: 0, items: [change_reason] }];
                        arr = arr.concat(insp_portin);
                        break;
                    case "insp_portout": case "base_harbour": case "sys_declarationcar": case "sys_reportlibrary":
                        remark = { layout: 'column', height: 42, border: 0, items: [field_REMARK,change_reason] };
                        arr = [{ layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_Code, field_Name] }];
                        arr1[arr1.length-1] = remark;
                        arr = arr.concat(arr1);
                        break;
                   
                        
                }
            }
            //**************************************************************
            var formpanel_Win = Ext.create('Ext.form.Panel', {
                id: 'formpanel_Win',
                minHeight: 170,
                border: 0,
                buttonAlign: 'center',
                fieldDefaults: {
                    margin: '0 5 10 0',
                    labelWidth: 100,
                    columnWidth: .5,
                    labelAlign: 'right',
                    labelSeparator: '',
                    msgTarget: 'under'
                },
                items: arr.concat(field_ID),
                buttons: [{

                    text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {
                        console.log('bbbb');
                        if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {

                            return;
                        }

                        var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());

                        Ext.Ajax.request({
                            url: 'Base_blend_web.aspx',
                            type: 'Post',
                            params: { action: 'save', formdata: formdata, table: param },
                            success: function (response, option) {

                                var data = Ext.decode(response.responseText);
                                if (data.success == "5") {
                                    Ext.Msg.alert('提示',
                                        "保存成功",
                                        function () {
                                            Ext.getCmp("pgbar").moveFirst();
                                            Ext.getCmp("win_d").close();
                                        });
                                } else {
                                    Ext.Msg.alert('提示', "保存失败,代码重复！", function () {
                                        Ext.getCmp("pgbar").moveFirst(); Ext.getCmp("win_d").close();
                                    });
                                }


                            }
                        });

                    }
                }]
            });
        }

        function addCustomer_Win(ID, formdata, param) {
            //*************************根据表来创建height******************************
            var height = 260;
            switch (param) {
                case "insp_portout": case "base_harbour": case "sys_declarationcar": case "sys_reportlibrary":
                    height = 300;
                    break;
                case "base_inspcountry": case "base_country":
                    height = 330;
                    break;
            }


            //***********************************************************************
            form_ini_win(param,ID);
            Ext.getCmp('CREATEMANNAME').setValue(username);

            //**************************************************************************
            if (ID != "") {
                height = 300;
                switch (param) {
                    case "base_inspcountry": case "base_country":
                        height = 340;
                        break;
                }

                //默认值的
                Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
            }
            //*********************************************************************************
            var win = Ext.create("Ext.window.Window", {
                id: "win_d",
                title: title,
                width: 1200,
                height: height,
                modal: true,
                items: [Ext.getCmp('formpanel_Win')]
            });
            win.show();
        }

        //修改按钮
        function editCustomer(param) {

            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
                return;
            }
            addCustomer_Win(recs[0].get("ID"), recs[0].data, param);

        }
        //导出
        function exportdata(param) {

            var CODE_S = Ext.getCmp('CODE_S').getValue();
            var CNNAME_S = Ext.getCmp('CNNAME_S').getValue();
            var combo_ENABLED_S = Ext.getCmp('combo_ENABLED_S').getValue();
            var path = 'Base_blend_web.aspx?action=export&CODE_S=' + CODE_S + '&CNNAME_S=' + CNNAME_S + '&combo_ENABLED_S=' + combo_ENABLED_S + '&table=' + param;

            switch (param) {
                case "base_insplicense": case "base_inspinvoice":
                    var searchOutName = Ext.getCmp('searchOutName').getValue();
                    path = path + '&searchOutName=' + searchOutName;
                    break;
            }
            $('#exportform').attr("action", path).submit();
        }

        //导入
        function importfile(action, param) {
            if (action == "add") {
                importexcel(action, param);
            }

        }
        function importexcel(action, param) {

            var radio_module = Ext.create('Ext.form.RadioGroup', {
                name: "RADIO_MODULE", id: "RADIO_MODULE", fieldLabel: '模板类型',
                items: [
                    { boxLabel: "<a href='/FileUpload/" + param + ".xls'><b>模板</b></a>", name: 'RADIO_MODULE', inputValue: '1', checked: true }
                ]
            });


            var uploadfile = Ext.create('Ext.form.field.File', {
                id: 'UPLOADFILE', name: 'UPLOADFILE', fieldLabel: '导入数据', labelAlign: 'right', msgTarget: 'under'
                , anchor: '90%', buttonText: '浏览文件', regex: /.*(.xls|.xlsx)$/, regexText: '只能上传xls,xlsx文件'
                , allowBlank: false, blankText: '文件不能为空!'
            });

            var start_date = Ext.create('Ext.form.field.Date',
                {
                    id: 'STARTDATE',
                    name: 'STARTDATE',
                    format: 'Y-m-d',
                    fieldLabel: '启用日期',
                    flex: .5

                });

            var end_date = Ext.create('Ext.form.field.Date',
                {
                    id: 'ENDDATE',
                    name: 'ENDDATE',
                    format: 'Y-m-d',
                    fieldLabel: '停用日期',
                    flex: .5


                });

            var CreatemanName = Ext.create('Ext.form.field.Text', {
                id: 'CREATEMANNAME',
                name: 'CREATEMANNAME',
                fieldLabel: '维护人',
                readOnly: true,
                flex: .5,
                margin: '0 5 10 122',
            });

            var formpanel_upload = Ext.create('Ext.form.Panel', {
                id: 'formpanel_upload', height: 180,
                fieldDefaults: {
                    margin: '0 5 10 0',
                    labelWidth: 80,
                    labelAlign: 'right',
                    labelSeparator: '',
                    msgTarget: 'under'
                },
                buttonAlign: 'center',

                items: [
                    { layout: 'column', height: 42, border: 0, items: [radio_module, CreatemanName] },
                    { layout: 'column', height: 42, border: 0, items: [start_date, end_date] },
                    { layout: 'column', height: 42, border: 0, items: [uploadfile] }
                ],
                buttons: [{
                    text: '确认上传',
                    handler: function () {
                        if (Ext.getCmp('formpanel_upload').getForm().isValid()) {

                            var formdata = Ext.encode(Ext.getCmp('formpanel_upload').getForm().getValues());

                            Ext.getCmp('formpanel_upload').getForm().submit({
                                type: 'Post',
                                url: 'Base_blend_web.aspx',
                                params: { formdata: formdata, action: action, table: param },
                                waitMsg: '数据导入中...',
                                success: function (form, action) {
                                    console.log(action.result);
                                    var data = action.result.success;
                                    var reg = /,$/gi;
                                    idStr = data.replace(reg, "!");
                                    Ext.Msg.alert('提示', idStr, function () {
                                        //pgbar.moveFirst();
                                        Ext.getCmp('pgbar').moveFirst();
                                        Ext.getCmp('win_upload').close();
                                    });
                                },
                                failure: function (form, action) {//失败要做的事情 
                                    Ext.MessageBox.alert("提示", "保存失败", function () { });
                                }
                            });

                        }
                    }
                }]
            });

            var win_upload = Ext.create("Ext.window.Window", {
                id: "win_upload",
                title: title,
                width: 600,
                height: 240,
                modal: true,
                items: [Ext.getCmp('formpanel_upload')]
            });
            Ext.getCmp('CREATEMANNAME').setValue(username);
            win_upload.show();
        }
    </script>
</head>
<body>
    <form id="exportform" name="form" enctype="multipart/form-data" method="post">
    <div>
    
    </div>
    </form>
</body>
</html>
