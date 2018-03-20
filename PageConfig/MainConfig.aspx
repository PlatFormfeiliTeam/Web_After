<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainConfig.aspx.cs" Inherits="Web_After.PageConfig.MainConfig" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>配置管理</title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />
    <script src="/js/import/importExcel.js" type="text/javascript"></script>
    <script type="text/javascript">
        var common_data_customercode = [];
        var store_customer;
        var store1, store2, store3;

        Ext.onReady(function () {
            Ext.Ajax.request({
                url: 'MainConfig.aspx',
                params: { action: 'Ini_Base_Data' },
                type: 'Post',
                success: function (response, option) {
                    var commondata = Ext.decode(response.responseText);
                    common_data_customercode = commondata.customercode;//
                    store_customer = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_customercode });
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
                    url: "MainConfig.aspx?action=loadbasebusitype",
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
                    url: "MainConfig.aspx?action=loadbasebusidetail",
                    reader: {
                        type: "json",
                        root: "busidetail"
                    }
                }
            });
            store1.load();
            store2.load();

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
                valueField: 'CODE'
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
                listeners: {
                    change: function (f, n, o) {
                        store3.removeAll();
                        combo_detail = Ext.getCmp("SEARCH_BUSIDETAIL");
                        combo_detail.reset();
                        store2.each(function (record) {
                            if (record.get('BUSITYPE') == n) {
                                store3.add(record);
                            }
                        });
                    }
                }
            });

            //业务细项
            var combo_search_busidetail = Ext.create('Ext.form.field.ComboBox', {
                id: 'SEARCH_BUSIDETAIL',
                name: 'SEARCH_BUSIDETAIL',
                store: store3,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '业务细项',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            //名称
            var txt_search_name = Ext.create('Ext.form.field.Text', { id: 'SEARCH_NAME', name: 'SEARCH_NAME', fieldLabel: '名称' });
            //编号
            var txt_search_code = Ext.create('Ext.form.field.Text', { id: 'SEARCH_CODE', name: 'SEARCH_CODE', fieldLabel: '编号' });
            //适用客商
            var store_account = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_customercode
            });

            var txt_search_account = Ext.create('Ext.form.field.ComboBox', {
                id: 'SEARCH_ACCOUNT',
                name: 'SEARCH_ACCOUNT',
                store: store_account,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '适用客商',
                displayField: 'NAME',
                valueField: 'CODE'
            });
            //启禁用
            var store_enabled_s = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
            });
            var combo_enabled_s = Ext.create('Ext.form.field.ComboBox', {
                id: 'SEARCH_ENABLED',
                name: 'SEARCH_ENABLED',
                store: store_enabled_s,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '是否启用',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { add_config("", ""); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;编辑', width: 80, handler: function () { edit_config(); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;复制新增', width: 80, handler: function () { } }
                    , { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { delete_config();} }
                    , { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;启用', width: 80, handler: function () { enable_config(); } }
                    , { text: '<span class="icon iconfont">&#xe63c;</span>&nbsp;禁用', width: 80, handler: function () { disable_config(); } }
                    , { text: '<span class="icon iconfont">&#xe625;</span>&nbsp;导 出', handler: function () { } }
                    , { text: '<span class="icon iconfont">&#xe6e4;</span>&nbsp;配置', handler: function () { set_config();} }
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
                    labelWidth: 100
                },
                items: [
                    { layout: 'column', border: 0, items: [combo_search_page, combo_search_busitype, combo_search_busidetail] },
                    { layout: 'column', border: 0, items: [txt_search_name, txt_search_code, txt_search_account, combo_enabled_s] }
                ]
            });
        }

        //数据表格绑定
        function gridbind() {
            var store_customer = Ext.create('Ext.data.JsonStore',
                {
                    fields: ['CODE', 'NAME', 'PAGENAME', 'CONFIGCONTENT', 'CUSTOMERCODE', 'CREATETIME', 'ENABLED',
                        'USERID', 'USERNAME', 'ID', 'BUSITYPE', 'BUSIDETAIL'],
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'MainConfig.aspx?action=loadData',
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
                    { header: '编号', dataIndex: 'CODE', width: 150 },
                    { header: '名称', dataIndex: 'NAME', width: 150 },
                    { header: '页面名称', dataIndex: 'PAGENAME', width: 150 },
                    { header: '配置内容', dataIndex: 'CONFIGCONTENT', width: 250 },
                    { header: '适用客商', dataIndex: 'CUSTOMERCODE', width: 250 },
                    { header: '创建时间', dataIndex: 'CREATETIME', width: 150 },
                    { header: '是否启用', dataIndex: 'ENABLED', renderer: gridrender, width: 150 },
                    { header: '用户ID', dataIndex: 'USERID', width: 150 },
                    { header: '用户名称', dataIndex: 'USERNAME', width: 150 },
                    { header: 'ID', dataIndex: 'ID', width: 200, hidden: true },
                    { header: 'BUSITYPE', dataIndex: 'BUSITYPE', width: 150, hidden: true },
                    { header: 'BUSIDETAIL', dataIndex: 'BUSIDETAIL', width: 150, hidden: true }
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

        //新增配置主表
        function add_config(ID, formdata) {
            form_ini_win();

            if (ID != "") {
                Ext.getCmp('REASON').hidden = false;
                Ext.getCmp('REASON').allowBlank = false;
                Ext.getCmp('REASON').blankText = '修改原因不可为空!';
                //默认值的
                Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
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

        //初始化界面
        function form_ini_win() {
            var field_id = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });

            //代码
            var field_code = Ext.create('Ext.form.field.Text', {
                id: 'CODE',
                name: 'CODE',
                fieldLabel: '代码',
                allowBlank: false,
                blankText: '代码不可为空!',
            });

            //名称
            var field_name = Ext.create('Ext.form.field.Text', {
                id: 'NAME',
                name: 'NAME',
                fieldLabel: '名称',
                allowBlank: false,
                blankText: '名称不可为空!',
            });

            //适用页面
            var store_page = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": "关务维护页面", "NAME": "关务维护页面" }, { "CODE": "关务管理页面", "NAME": "关务管理页面" }]
            });

            var field_configpage = Ext.create('Ext.form.field.ComboBox', {
                id: 'PAGENAME',
                name: 'PAGENAME',
                store: store_page,
                minChars: 1,
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE',
                anyMatch: true,
                fieldLabel: '适用页面',
                flex: .5,
                allowBlank: false,
                blankText: '适用页面不可为空!',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    }
                }
            });



            //业务类型
            var field_busitype = Ext.create('Ext.form.field.ComboBox', {
                id: 'BUSITYPE',
                name: 'BUSITYPE',
                store: store1,
                minChars: 1,
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE',
                anyMatch: true,
                fieldLabel: '业务类型',
                flex: .5,
                allowBlank: false,
                blankText: '业务类型不可为空!',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    },
                    change: function (f, n, o) {
                        store3.removeAll();
                        combo_detail = Ext.getCmp("BUSIDETAIL");
                        combo_detail.reset();
                        store2.each(function (record) {
                            if (record.get('BUSITYPE') == n) {
                                store3.add(record);
                            }
                        });
                    }
                }
            });


            //业务细项

            var field_busidetail = Ext.create('Ext.form.field.ComboBox', {
                id: 'BUSIDETAIL',
                name: 'BUSIDETAIL',
                store: store3,
                minChars: 1,
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE',
                anyMatch: true,
                fieldLabel: '业务细项',
                flex: .5,
                allowBlank: false,
                blankText: '业务细项不可为空!',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    }
                }
            });

            //适用客商
            var store_account = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_customercode
            });

            var field_account = Ext.create('Ext.form.field.ComboBox', {
                id: 'CUSTOMERCODE',
                name: 'CUSTOMERCODE',
                store: store_account,
                minChars: 1,
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE',
                anyMatch: true,
                fieldLabel: '适用客商',
                flex: .5,
                allowBlank: false,
                blankText: '适用客商不可为空!',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    }
                }
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
                    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_code, field_name] },
                    { layout: 'column', height: 42, border: 0, items: [field_configpage, field_busitype] },
                    { layout: 'column', height: 42, border: 0, items: [field_busidetail, field_account] },
                    { layout: 'column', height: 42, border: 0, items: [field_enabled, change_reason] },
                    field_id
                ],
                buttons: [{

                    text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {
                        if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                            return;
                        }

                        var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                        Ext.Ajax.request({
                            url: 'MainConfig.aspx',
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

        //重置查询条件
        function reset() {
            Ext.each(Ext.getCmp('formpanel_search').getForm().getFields().items,
                function (field) {
                    field.reset();
                });
        }

        //启用
        function enable_config() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要启用的记录！');
                return;
            }
            var ids = '';
            for (var i = 0; i < recs.length; i++) {
                ids += recs[i].get("ID") + ';';
            }
            $.ajax({
                url: "MainConfig.aspx/EnableConfig",
                type: "POST",
                data: "{\"id\":\"" + ids + "\"}",
                contentType: "Application/json;charset=utf-8",
                success: function (data, textStatus, jqXHR) {
                    Ext.MessageBox.alert('启用结果', data.d, function (btn) {
                        Ext.getCmp("pgbar").moveFirst();
                    });
                }
            });
        }

        //禁用
        function disable_config() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要禁用的记录！');
                return;
            }
            var ids = '';
            for (var i = 0; i < recs.length; i++) {
                ids += recs[i].get("ID") + ';';
            }
            $.ajax({
                url: "MainConfig.aspx/DisableConfig",
                type: "POST",
                data: "{\"id\":\"" + ids + "\"}",
                contentType: "Application/json;charset=utf-8",
                success: function (data, textStatus, jqXHR) {
                    Ext.MessageBox.alert('禁用结果', data.d, function (btn) {
                        Ext.getCmp("pgbar").moveFirst();
                    });
                }
            });
        }

        //配置
        function set_config()
        {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要配置的记录！');
                return;
            }
            var id = recs[0].get("ID");
            var enabled = recs[0].get("ENABLED");
            if (enabled == '0') {
                alert("该记录已被禁用，不能配置");
                return;
            }
            window.open('ConfigDetail.aspx?parentid=' + id, '', '', '');
        }
        
        //删除
        function delete_config()
        {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要删除的记录！');
                return;
            }
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
