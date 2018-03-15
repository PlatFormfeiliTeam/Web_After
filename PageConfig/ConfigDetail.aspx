<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigDetail.aspx.cs" Inherits="Web_After.PageConfig.ConfigDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>详细配置</title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />
    <script src="/js/import/importExcel.js" type="text/javascript"></script>
    <script src="/js/pan.js" type="text/javascript"></script>
    <script type="text/javascript">
        var parentid = getQueryString("parentid");
        if (parentid == undefined || parentid == "") {
            parentid = -1;
        }
        var store1, store2, store3;
        var store_customercode;
        var store_parentinfo;
        var commondata;
        var common_data_parentinfo = [];

        Ext.onReady(function () {
            Ext.Ajax.request({
                url: 'ConfigDetail.aspx?parentid=' + parentid,
                params: { action: 'loadparentinfo' },
                type: 'Post',
                success: function (response, option) {
                    var commondata = Ext.decode(response.responseText);
                    common_data_parentinfo = commondata.parentinfo;//
                    store_parentinfo = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME', 'PAGENAME', 'BUSITYPE', 'BUSIDETAIL', 'CUSTOMERCODE'], data: common_data_parentinfo });
                    init_search();
                    gridbind();

                    var panel = Ext.create('Ext.form.Panel', {
                        title: "配置管理",
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

        //查询栏
        function init_search() {
            Ext.define("comboboxcustomercode", {
                extend: "Ext.data.Model",
                fields: [
                     "CODE", "NAME"
                ]
            });

            store_customercode = Ext.create("Ext.data.Store", {
                storeId: "store_customercode",
                model: "comboboxcustomercode",
                proxy: {
                    type: "ajax",
                    url: "ConfigDetail.aspx?action=loadbasecustomercode",
                    reader: {
                        type: "json",
                        root: "customercode"
                    }
                }
            });

            Ext.define("comboboxbusitype", {
                extend: "Ext.data.Model",
                fields: [
                     "CODE", "NAME"
                ]
            });

            store1 = Ext.create("Ext.data.Store", {
                storeId: "store1",
                model: "comboboxbusitype",
                proxy: {
                    type: "ajax",
                    url: "ConfigDetail.aspx?action=loadbasebusitype",
                    reader: {
                        type: "json",
                        root: "busitype"
                    }
                }
            });

            Ext.define("comboboxbusidetail", {
                extend: "Ext.data.Model",
                fields: [
                     "CODE", "NAME", "BUSITYPE"
                ]
            });

            store2 = Ext.create("Ext.data.Store", {
                storeId: "store2",
                model: "comboboxbusidetail",
                proxy: {
                    type: "ajax",
                    url: "ConfigDetail.aspx?action=loadbasebusidetail",
                    reader: {
                        type: "json",
                        root: "busidetail"
                    }
                }
            });
            store1.load();
            store2.load();
            store_customercode.load();

            store3 = Ext.create("Ext.data.Store", {
                fields: [
                     "CODE", "NAME", "BUSITYPE"
                ]
            });
            //适用页面
            var store_page = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": "关务维护页面", "NAME": "关务维护页面" }, { "CODE": "关务管理页面", "NAME": "关务管理页面" }]
            });

            var combo_search_page = Ext.create('Ext.form.field.ComboBox', {
                id: 'SEARCH_PAGE',
                name: 'SEARCH_PAGE',
                store: store_page,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '适用页面',
                displayField: 'NAME',
                valueField: 'CODE',
                //hiddenTrriger: true,
                readOnly:true
            });

            //业务类型
            var combo_search_busitype = Ext.create('Ext.form.field.ComboBox', {
                id: 'SEARCH_BUSITYPE',
                name: 'SEARCH_BUSITYPE',
                store: store1,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '业务类型',
                displayField: 'NAME',
                valueField: 'CODE',
                readOnly:true,
                listeners: {
                    change: function (f, n, o) {
                        //store3.removeAll();
                        //combo_detail = Ext.getCmp("SEARCH_BUSIDETAIL");
                        //combo_detail.reset();
                        //store2.each(function (record) {
                        //    if (record.get('BUSITYPE') == n) {
                        //        store3.add(record);
                        //    }
                        //});
                    }
                }
            });

            //业务细项
            var combo_search_busidetail = Ext.create('Ext.form.field.ComboBox', {
                id: 'SEARCH_BUSIDETAIL',
                name: 'SEARCH_BUSIDETAIL',
                store: store2,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '业务细项',
                displayField: 'NAME',
                valueField: 'CODE',
                readOnly: true
            });

            //名称
            var txt_search_name = Ext.create('Ext.form.field.Text', { id: 'SEARCH_NAME', name: 'SEARCH_NAME', fieldLabel: '名称', readOnly: true });
            //编号
            var txt_search_code = Ext.create('Ext.form.field.Text', { id: 'SEARCH_CODE', name: 'SEARCH_CODE', fieldLabel: '编号', readOnly: true });
            //适用客商
            var txt_search_account = Ext.create('Ext.form.field.ComboBox', {
                id: 'SEARCH_ACCOUNT',
                name: 'SEARCH_ACCOUNT',
                store: store_customercode,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '适用客商',
                displayField: 'NAME',
                valueField: 'CODE',
                readOnly: true
            });

            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { add_config("", ""); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;编辑', width: 80, handler: function () { edit_config(); } }
                    , { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { } }
                    , '->'
                    , { text: '<span class="icon iconfont">&#xe60b;</span>&nbsp;查 询', width: 80, handler: function () { Ext.getCmp("pgbar").moveFirst(); } }
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
                    labelWidth: 100
                },
                items: [
                    { layout: 'column', border: 0, items: [combo_search_page, combo_search_busitype, combo_search_busidetail] },
                    { layout: 'column', border: 0, items: [txt_search_name, txt_search_code, txt_search_account] }
                ]
            });
  
            Ext.getCmp('SEARCH_PAGE').setValue(store_parentinfo.getAt(0).get('PAGENAME'));
            Ext.getCmp('SEARCH_CODE').setValue(store_parentinfo.getAt(0).get('CODE'));
            Ext.getCmp('SEARCH_NAME').setValue(store_parentinfo.getAt(0).get('NAME'));
            Ext.getCmp('SEARCH_BUSITYPE').setValue(store_parentinfo.getAt(0).get('BUSITYPE'));
            Ext.getCmp('SEARCH_BUSIDETAIL').setValue(store_parentinfo.getAt(0).get('BUSIDETAIL'));
            Ext.getCmp('SEARCH_ACCOUNT').setValue(store_parentinfo.getAt(0).get('CUSTOMERCODE'));
        }


        //数据表格绑定
        function gridbind() {
            var store_customer = Ext.create('Ext.data.JsonStore',
                {
                    fields: ['ORDERNO', 'NAME', 'CONTROLTYPE', 'SELECTCONTENT', 'CONFIGTYPE', 'TABLECODE',
                        'FIELDCODE', 'TABLENAME', 'FIELDNAME', 'CREATETIME', 'USERID', 'USERNAME', 'ENABLED', 'ID', 'PARENTID', ],
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'ConfigDetail.aspx?action=loadData&parentid=' + parentid,
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
                columns: [
                    { xtype: 'rownumberer', width: 35 },
                    { header: '序号', dataIndex: 'ORDERNO', width: 50 },
                    { header: '显示名称', dataIndex: 'NAME', width: 150 },
                    { header: '控件类型', dataIndex: 'CONTROLTYPE', width: 150 },
                    { header: '下拉内容', dataIndex: 'SELECTCONTENT', width: 250 },
                    { header: '配置类型', dataIndex: 'CONFIGTYPE', width: 250 },
                    { header: '表名代码', dataIndex: 'TABLECODE', width: 150 },
                    { header: '表名称', dataIndex: 'TABLENAME', width: 150 },
                    { header: '字段代码', dataIndex: 'FIELDCODE', width: 150 },
                    { header: '字段名称', dataIndex: 'FIELDNAME', width: 150 },
                    { header: '创建时间', dataIndex: 'CREATETIME', width: 100 },
                     { header: '用户ID', dataIndex: 'USERID', width: 100 },
                     { header: '用户名', dataIndex: 'USERNAME', width: 100 },
                      { header: '是否启用', dataIndex: 'ENABLED', renderer: gridrender, width: 100 },
                    { header: 'ID', dataIndex: 'ID', width: 200, hidden: true },
                    { header: 'PARENTID', dataIndex: 'PARENTID', width: 150, hidden: true },
                ],
                listeners:
                {
                    'itemdblclick': function (view, record, item, index, e) {
                        //editCustomer();
                    }
                },
                viewConfig: {
                    enableTextSelection: true
                }
            });
        }

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
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
