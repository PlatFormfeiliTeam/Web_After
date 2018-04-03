<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BusitypeDetailBaseConfig.aspx.cs" Inherits="Web_After.PageConfig.BusitypeDetailBaseConfig" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>业务细项基本维护</title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />
    <script src="/js/import/importExcel.js" type="text/javascript"></script>
    <script type="text/javascript">
        var store1,store2,store3;

        Ext.onReady(function () {
            Ext.Ajax.request({
                url: 'BusitypeDetailBaseConfig.aspx',
                params: { action: 'Ini_Base_Data' },
                type: 'Post',
                success: function (response, option) {
                    init_search();
                    gridbind();

                    var panel = Ext.create('Ext.form.Panel', {
                        title: "业务细项基本维护",
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
                    url: "BusitypeDetailBaseConfig.aspx?action=loadbasebusitype",
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
                    url: "BusitypeDetailBaseConfig.aspx?action=loadbasebusidetail",
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
                    //, { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;编辑', width: 80, handler: function () { edit_config(); } }
                    , { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { delete_config(); } }
                    , { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;启用', width: 80, handler: function () { enable_config(); } }
                    , { text: '<span class="icon iconfont">&#xe63c;</span>&nbsp;禁用', width: 80, handler: function () { disable_config(); } }
                    , { text: '<span class="icon iconfont">&#xe625;</span>&nbsp;导 出', handler: function () { } }
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
                    { layout: 'column', border: 0, items: [combo_search_busitype, combo_search_busidetail, combo_enabled_s]},
                ]
            });
        }

        //数据表格绑定
        function gridbind() {
            var store_customer = Ext.create('Ext.data.JsonStore',
                {
                    fields: ['BUSITYPECODE','BUSITYPENAME', 'BUSIITEMCODE', 'BUSIITEMNAME', 'CREATETIME', 'ENABLE',
                        'CREATEUSERID', 'CREATEUSERNAME', 'ID'],
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'BusitypeDetailBaseConfig.aspx?action=loadData',
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
                    { header: '业务类型编号', dataIndex: 'BUSITYPECODE', width: 150 },
                    { header: '业务类型名称', dataIndex: 'BUSITYPENAME', width: 150 },
                    { header: '业务细项编号', dataIndex: 'BUSIITEMCODE', width: 150 },
                    { header: '业务细项名称', dataIndex: 'BUSIITEMNAME', width: 150 },
                    { header: '创建时间', dataIndex: 'CREATETIME', width: 150 },
                    { header: '是否启用', dataIndex: 'ENABLE', renderer: gridrender, width: 150 },
                    { header: '用户ID', dataIndex: 'CREATEUSERID', width: 150 },
                    { header: '用户名称', dataIndex: 'CREATEUSERNAME', width: 150 },
                    { header: 'ID', dataIndex: 'ID', width: 150, hidden: true }
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
                case "ENABLE":
                    str = value == "1" ? '<span class="icon iconfont" style="font-size:12px;color:blue;">&#xe628;</span>' : '<span class="icon iconfont" style="font-size:12px;color:red;">&#xe634;</span>';
                    break;
            }
            return str;
        }

        //新增配置
        function add_config(ID, formdata)
        {
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
                width: 1000,
                height: 300,
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

            //业务类型
            var field_busitype = Ext.create('Ext.form.field.ComboBox', {
                id: 'BUSITYPECODE',
                name: 'BUSITYPECODE',
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

            var field_busidetailcode = Ext.create('Ext.form.field.Text', {
                id: 'BUSIITEMCODE',
                name: 'BUSIITEMCODE',
                fieldLabel: '业务细项代码'
            });

            var field_busidetailname = Ext.create('Ext.form.field.Text', {
                id: 'BUSIITEMNAME',
                name: 'BUSIITEMNAME',
                fieldLabel: '业务细项名称'
            });

            var store_enabled_s = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
            });
            //是否启用
            var field_enabled = Ext.create('Ext.form.field.ComboBox', {
                id: 'ENABLE',
                name: 'ENABLE',
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
                minHeight: 120,
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
                    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_busitype] },
                    { layout: 'column', height: 42, border: 0, items: [field_busidetailcode, field_busidetailname] },
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
                            url: 'BusitypeDetailBaseConfig.aspx',
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

        //删除
        function delete_config()
        {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要删除的详细记录！');
                return;
            }

            Ext.Ajax.request({
                url: 'BusitypeDetailBaseConfig.aspx',
                type: 'Post',
                params: { action: 'delete', deleteid: recs[0].get("ID") },
                success: function (response, option) {
                    var data = Ext.decode(response.responseText);
                    if (data.success == "5") {
                        Ext.Msg.alert('提示',
                            "删除成功",
                            function () {
                                Ext.getCmp("pgbar").moveFirst();
                                Ext.getCmp("win_d").close();
                            });
                    } else {
                        var errorMsg = data.success;
                        var reg = /,$/gi;
                        idStr = errorMsg.replace(reg, "!");
                        Ext.Msg.alert('提示', "删除失败:" + idStr, function () {
                            Ext.getCmp("pgbar").moveFirst(); Ext.getCmp("win_d").close();
                        });
                    }
                }
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
                url: "BusitypeDetailBaseConfig.aspx/EnableConfig",
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
                url: "BusitypeDetailBaseConfig.aspx/DisableConfig",
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

     </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
