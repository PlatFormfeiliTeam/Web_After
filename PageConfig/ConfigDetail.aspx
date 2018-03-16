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
        var store1, store2, store3;
        var store_table ,store_field, store_field_filter;
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
            Ext.define("comboboxtablename", {
                extend: "Ext.data.Model",
                fields: [
                     "CODE", "NAME"
                ]
            });

            store_table = Ext.create("Ext.data.Store", {
                storeId: "store_table",
                model: "comboboxtablename",
                proxy: {
                    type: "ajax",
                    url: "ConfigDetail.aspx?action=gettablename",
                    reader: {
                        type: "json",
                        root: "tablename"
                    }
                }
            });

            Ext.define("comboboxfieldname", {
                extend: "Ext.data.Model",
                fields: [
                     "CODE", "NAME","TABLENAME"
                ]
            });

            store_field = Ext.create("Ext.data.Store", {
                storeId: "store_field",
                model: "comboboxfieldname",
                proxy: {
                    type: "ajax",
                    url: "ConfigDetail.aspx?action=getfieldname",
                    reader: {
                        type: "json",
                        root: "fieldname"
                    }
                }
            });


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
            store_table.load();
            store_field.load();

            store3 = Ext.create("Ext.data.Store", {
                fields: [
                     "CODE", "NAME", "BUSITYPE"
                ]
            });
            store_field_filter = Ext.create("Ext.data.Store", {
                fields: [
                     "CODE", "NAME", "TABLENAME"
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
                        'FIELDCODE', 'TABLENAME', 'FIELDNAME', 'CREATETIME', 'USERID', 'USERNAME', 'ENABLED', 'ID', 'PARENTID'],
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

        //新增
        function add_config(ID,formdata)
        {
            form_ini_win();

            if (ID != "") {
                Ext.getCmp('REASON').hidden = false;
                Ext.getCmp('REASON').allowBlank = false;
                Ext.getCmp('REASON').blankText = '修改原因不可为空!';
                //默认值的
                Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
            }
            else
            {
                Ext.Ajax.request({
                    url: 'ConfigDetail.aspx?parentid=' + parentid,
                    params: { action: 'getorderno' },
                    type: 'Post',
                    success: function (response, option) {
                        var commondata = Ext.decode(response.responseText);
                        var order = commondata.orderno;//
                        Ext.getCmp('ORDERNO').setValue(order[0].orderno);
                    }
                });
            }

            var win = Ext.create("Ext.window.Window", {
                id: "win_d",
                title: '配置',
                width: 1200,
                height: 430,
                modal: true,
                items: [Ext.getCmp('formpanel_Win')]
            });
            win.show();
        }


        function form_ini_win() {
            var field_id = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });

            var field_parentid = Ext.create('Ext.form.field.Hidden', {
                id: 'PARENTID',
                name: 'PARENTID'
            });

            //序号
            var field_orderno = Ext.create('Ext.form.field.Text', {
                id: 'ORDERNO',
                name: 'ORDERNO',
                fieldLabel: '序号',
                allowBlank: false,
                blankText: '代码不可为空!',
                readOnly: true
            });
            //显示名称
            var field_name = Ext.create('Ext.form.field.Text', {
                id: 'NAME',
                name: 'NAME',
                fieldLabel: '显示名称',
                allowBlank: false,
                blankText: '显示名称不可为空!',
            });

            //控件类型
            var store_controltype = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": "文本", "NAME": "文本" }, { "CODE": "数字", "NAME": "数字" }, { "CODE": "下拉框", "NAME": "下拉框" }, { "CODE": "日期", "NAME": "日期" }]
            });

            var field_controltype = Ext.create('Ext.form.field.ComboBox', {
                id: 'CONTROLTYPE',
                name: 'CONTROLTYPE',
                store: store_controltype,
                minChars: 1,
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE',
                anyMatch: true,
                fieldLabel: '控件类型',
                flex: .5,
                allowBlank: false,
                blankText: '控件类型不可为空!',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    },
                    change: function (f, n, o) {
                        if (n == '下拉框') {
                            Ext.getCmp('SELECTCONTENT').show();
                            Ext.getCmp('SELECTCONTENT').allowBlank = false;
                        } else {
                            Ext.getCmp('SELECTCONTENT').hide();
                            Ext.getCmp('SELECTCONTENT').allowBlank = true;
                        }
                    }
                }
            });

            //下拉内容
            var field_selectcontent = Ext.create('Ext.form.field.Text', {
                id: 'SELECTCONTENT',
                name: 'SELECTCONTENT',
                fieldLabel: '下拉内容',
                blankText: '下拉内容不可为空!',
                hidden:true
            });

            //表名代码
            var field_tablecode = Ext.create('Ext.form.field.ComboBox', {
                id: 'TABLECODE',
                name: 'TABLECODE',
                store: store_table,
                minChars: 1,
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE',
                anyMatch: true,
                fieldLabel: '表名',
                flex: .5,
                allowBlank: false,
                blankText: '表名不可为空!',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    },
                    change: function (f, n, o) {
                        store_field_filter.removeAll();
                        combo_detail = Ext.getCmp("FIELDCODE");
                        combo_detail.reset();
                        store_field.each(function (record) {
                            if (record.get('TABLENAME') == n) {
                                store_field_filter.add(record);
                            }
                        });
                    }
                }
            });


            //字段名
            var field_fieldcode = Ext.create('Ext.form.field.ComboBox', {
                id: 'FIELDCODE',
                name: 'FIELDCODE',
                store: store_field_filter,
                minChars: 1,
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE',
                anyMatch: true,
                fieldLabel: '字段名',
                flex: .5,
                allowBlank: false,
                blankText: '字段名不可为空!',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    }
                }
            });

            var store_configtype = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": "界面维护", "NAME": "界面维护" }, { "CODE": "高级查询", "NAME": "高级查询" }, { "CODE": "列表显示", "NAME": "列表显示" }]
            });
            //配置类型
            var field_configtype = Ext.create('Ext.form.field.ComboBox', {
                id: 'CONFIGTYPE',
                name: 'CONFIGTYPE',
                store: store_configtype,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '配置类型',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            var store_enabled_s = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
            });
            //是否启用
            var field_enabled = Ext.create('Ext.form.field.ComboBox', {
                id: 'ENABLED',
                name: 'ENABLED',
                store: store_enabled_s,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '是否启用',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            //修改原因输入框
            var change_reason = Ext.create('Ext.form.field.Text', {
                id: 'REASON',
                name: 'REASON',
                fieldLabel: '修改原因',
                hidden: true
            });

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
                items: [
                    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_orderno, field_name] },
                    { layout: 'column', height: 42, border: 0, items: [field_controltype, field_selectcontent] },
                    { layout: 'column', height: 42, border: 0, items: [field_tablecode, field_fieldcode] },
                    { layout: 'column', height: 42, border: 0, items: [field_configtype] },
                    { layout: 'column', height: 42, border: 0, items: [field_enabled, change_reason] },
                    field_parentid,
                    field_id
                ],
                buttons: [{

                    text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {
                        if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                            return;
                        }

                        var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                        Ext.Ajax.request({
                            url: 'ConfigDetail.aspx?parentid='+parentid,
                            type: 'Post',
                            params: { action: 'save', formdata: formdata },
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
                                    var errorMsg = data.success;
                                    var reg = /,$/gi;
                                    idStr = errorMsg.replace(reg, "!");
                                    Ext.Msg.alert('提示', "保存失败:" + idStr, function () {
                                        Ext.getCmp("pgbar").moveFirst(); Ext.getCmp("win_d").close();
                                    });
                                }
                            }
                        });
                    }
                }]
            });
        }


        //编辑
        function edit_config() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
                return;
            }
            add_config(recs[0].get("ID"), recs[0].data);
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
