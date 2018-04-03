<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigItem.aspx.cs" Inherits="Web_After.PageConfig.ConfigItem" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>label显示值配置</title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />
    <script src="/js/import/importExcel.js" type="text/javascript"></script>
    <script type="text/javascript">
        var store1, store2, store3;

        Ext.onReady(function () {
            Ext.Ajax.request({
                url: 'ConfigItem.aspx',
                params: { action: '' },
                type: 'Post',
                success: function (response, option) {
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
                    url: "ConfigItem.aspx?action=loadbasebusitype",
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
                    url: "ConfigItem.aspx?action=loadbasebusidetail",
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

            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { add_config("", ""); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;编辑', width: 80, handler: function () { edit_config(); } }
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
                    { layout: 'column', border: 0, items: [combo_search_busitype, combo_search_busidetail] }
                ]
            });
        }


        //数据表格绑定
        function gridbind() {
            var store_customer = Ext.create('Ext.data.JsonStore',
                {
                    fields: ['BUSITYPECODE', 'BUSITYPENAME', 'BUSIITEMCODE', 'BUSIITEMNAME', 'ORIGINNAME', 'CONFIGNAME', 'CREATEUSERID',
                        'CREATEUSERID', 'CREATEUSERNAME', 'ID'],
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'ConfigItem.aspx?action=loadData',
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
                    { header: '业务细项名称', dataIndex: 'BUSIITEMNAME', width: 250 },
                    { header: '文本名称', dataIndex: 'ORIGINNAME', width: 250 },
                    { header: '配置名称', dataIndex: 'CONFIGNAME', width: 150 },
                    { header: '用户ID', dataIndex: 'CREATEUSERID', width: 150 },
                    { header: '用户名称', dataIndex: 'CREATEUSERNAME', width: 150 },
                    { header: 'ID', dataIndex: 'ID', width: 200, hidden: true }
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

        function add_config(ID, formdata)
        {
            form_ini_win();

            if (ID != "") {
                //Ext.getCmp('REASON').hidden = false;
                //Ext.getCmp('REASON').allowBlank = false;
                //Ext.getCmp('REASON').blankText = '修改原因不可为空!';
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

            //适用页面
            var store_label = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": "文本1", "NAME": "文本1" }, { "CODE": "文本2", "NAME": "文本2" }
                , { "CODE": "数字1", "NAME": "数字1" }, { "CODE": "数字2", "NAME": "数字2" }
                , { "CODE": "日期1", "NAME": "日期1" }, { "CODE": "日期2", "NAME": "日期2" }
                , { "CODE": "人员1", "NAME": "人员1" }, { "CODE": "人员2", "NAME": "人员2" }]
            });

            var field_originname = Ext.create('Ext.form.field.ComboBox', {
                id: 'ORIGINNAME',
                name: 'ORIGINNAME',
                store: store_label,
                minChars: 1,
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE',
                anyMatch: true,
                fieldLabel: '文本名称',
                flex: .5,
                allowBlank: false,
                blankText: '文本名称不可为空!',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    }
                }
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
                id: 'BUSIITEMCODE',
                name: 'BUSIITEMCODE',
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

            //代码
            var field_configname = Ext.create('Ext.form.field.Text', {
                id: 'CONFIGNAME',
                name: 'CONFIGNAME',
                fieldLabel: '配置名称',
                allowBlank: false,
                blankText: '配置名称不可为空!',
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
                    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_busitype, field_busidetail] },
                    { layout: 'column', height: 42, border: 0, items: [field_originname, field_configname] },
                    { layout: 'column', height: 42, border: 0, items: [change_reason] },
                    field_id
                ],
                buttons: [{

                    text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {
                        if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                            return;
                        }

                        var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                        Ext.Ajax.request({
                            url: 'ConfigItem.aspx',
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
