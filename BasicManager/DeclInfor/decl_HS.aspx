﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="decl_HS.aspx.cs" Inherits="Web_After.BasicManager.DeclInfor.decl_HS" %>

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
    <script src="/BasicManager/js/decl_HS_Maintain.js" type="text/javascript"></script>
    <script type="text/javascript">
        var data1 = [];
        var username = '<%=Username()%>';
        Ext.onReady(function () {
            Ext.Ajax.request({
                url: 'decl_HS.aspx',
                params: { action: 'getYear' },
                type: 'Post',
                success: function (response, option) {
                    var commondata = Ext.decode(response.responseText);
                    data1 = commondata;

                    init_search();
                    grindbind();


                    var panel = Ext.create('Ext.form.Panel', {
                        title: 'HS编码',
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
        function init_search() {

            var ciqCodeBase = Ext.create('Ext.form.field.Text', { id: 'CiqCodeBase', name: 'CiqCodeBase', fieldLabel: 'HS代码库' });
            var start_date = Ext.create('Ext.form.field.Date',
                {
                    id: 'start_date',
                    name: 'start_date',
                    format: 'Y-m-d',
                    fieldLabel: '开始时间',
                    labelAlign: 'right'
                });


            var end_date = Ext.create('Ext.form.field.Date',
                {
                    id: 'end_date',
                    name: 'end_date',
                    format: 'Y-m-d',
                    fieldLabel: '结束时间',
                    labelAlign: 'right'
                });


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

            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { addCustomer_Win(""); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改', width: 80, handler: function () { editCustomer(); } }
                    , { text: '<span class="icon iconfont">&#xe617;</span>&nbsp;维 护', width: 80, handler: function () { Maintain(); } }
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
                    labelWidth: 70
                },
                items: [
                    { layout: 'column', border: 0, items: [ciqCodeBase, combo_ENABLED_S, start_date, end_date] }

                ]
            });
        }

        //页面数据加载
        function grindbind() {
            var CiqDataBase = Ext.create('Ext.data.JsonStore',
                {
                    fields: ['NAME', 'CUSTOMAREANAME', 'ENABLED', 'STARTDATE', 'ENDDATE', 'CREATEMANNAME', 'CREATEDATE', 'STOPMANNAME', 'REMARK', 'ID', 'CUSTOMAREA'],
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'decl_HS.aspx?action=loadData',
                        reader: {
                            root: 'rows',
                            type: 'json',
                            totalProperty: 'total'
                        }
                    },
                    autoLoad: true,
                    listeners: {
                        beforeload: function (store, options) {
                            CiqDataBase.getProxy().extraParams = Ext.getCmp('formpanel_search').getForm().getValues();
                        }
                    }
                });

            var pgbar = Ext.create('Ext.toolbar.Paging',
                {
                    id: 'pgbar',
                    displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                    store: CiqDataBase,
                    displayInfo: true
                });

            var gridpanel = Ext.create('Ext.grid.Panel',
                {
                    id: 'gridpanel',
                    height: 560,
                    region: 'center',
                    store: CiqDataBase,
                    selModel: { selType: 'checkboxmodel' },
                    bbar: pgbar,
                    columns: [
                        { xtype: 'rownumberer', width: 35 },
                        { header: '规则名称', dataIndex: 'NAME', width: 200 },
                        { header: '关区', dataIndex: 'CUSTOMAREANAME', width: 150 },
                        { header: '维护人', dataIndex: 'CREATEMANNAME', width: 150 },
                        { header: '停用人', dataIndex: 'STOPMANNAME', width: 150},
                        { header: '启用时间', dataIndex: 'STARTDATE', width: 250 },
                        { header: '停用时间', dataIndex: 'ENDDATE', width: 250 },
                        { header: '维护时间', dataIndex: 'CREATEDATE', width: 250},
                        { header: '启用情况', dataIndex: 'ENABLED', width: 100, renderer: gridrender},
                        { header: '备注', dataIndex: 'REMARK', width: 284},
                        { header: 'ID', dataIndex: 'ID', width: 200, hidden: true }
                    ],
                    listeners:
                    {
                        'itemdblclick': function (view, record, item, index, e) {
                            editCustomer();
                        }
                    },
                    viewConfig: {
                        enableTextSelection: true
                    }
                });
        }

        //switch.....case
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
            Ext.each(Ext.getCmp('formpanel_search').getForm().getFields().items, function (field) {
                field.reset();
            });
        }


        //新增按钮所在的界面
        function form_ini_win() {


            var field_ID = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });

            var field_CODE = Ext.create('Ext.form.field.Text', {
                id: 'NAME',
                name: 'NAME',
                fieldLabel: '规则名称',
                flex: .5,
                allowBlank: false,
                blankText: '规则名称不可为空!'
            });

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

            var year_ENABLED = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: data1
            });
            var year_combo_ENABLED = Ext.create('Ext.form.field.ComboBox', {
                id: 'year_combo_ENABLED',
                name: 'CUSTOMAREA',
                store: year_ENABLED,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '申报关区', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE',
                allowBlank: false,
                blankText: '申报关区不能为空!'
            });

            var start_date1 = Ext.create('Ext.form.field.Date',
                {
                    id: 'STARTDATE',
                    name: 'STARTDATE',
                    format: 'Y-m-d',
                    fieldLabel: '启用日期',
                    flex: .5

                });
            var end_date1 = Ext.create('Ext.form.field.Date',
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

            //修改原因输入框
            var change_reason = Ext.create('Ext.form.field.Text', {
                id: 'REASON',
                name: 'REASON',
                fieldLabel: '修改原因',
                hidden: true


            });

            //新增布局
            var formpanel_Win = Ext.create('Ext.form.Panel', {
                id: 'formpanel_Win',
                minHeight: 170,
                border: 0,
                buttonAlign: 'center',
                fieldDefaults: {
                    margin: '0 5 10 0',
                    labelWidth: 75,
                    columnWidth: .5,
                    labelAlign: 'right',
                    labelSeparator: '',
                    msgTarget: 'under'
                },
                items: [
                    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_CODE,year_combo_ENABLED ] },
                    { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, start_date1] },
                    { layout: 'column', height: 42, border: 0, items: [end_date1, CreatemanName] },
                    { layout: 'column', height: 42, border: 0, items: [field_REMARK, change_reason] },
                    
                    field_ID
                ],
                buttons: [{

                    text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {

                        if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                            return;
                        }

                        var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());

                        Ext.Ajax.request({
                            url: 'decl_HS.aspx',
                            type: 'Post',
                            params: { action: 'save', formdata: formdata },
                            success: function (response, option) {
                                var data = Ext.decode(response.responseText);
                                if (data.success == "4") {
                                    Ext.Msg.alert('提示',
                                        "保存失败:规则名称重复!",
                                        function () {
                                            Ext.getCmp("pgbar").moveFirst();
                                            Ext.getCmp("win_d").close();
                                        });
                                } else {
                                    if (data.success) {
                                        Ext.Msg.alert('提示', "保存成功", function () {
                                            Ext.getCmp("pgbar").moveFirst(); Ext.getCmp("win_d").close();
                                        });
                                    }
                                    else {
                                        Ext.Msg.alert('提示', "保存失败", function () {
                                            Ext.getCmp("pgbar").moveFirst(); Ext.getCmp("win_d").close();
                                        });
                                    }
                                }


                            }
                        });
                    }
                }]
            });
        }

        //弹出新增框/修改框
        function addCustomer_Win(ID, formdata) {
            form_ini_win();
            Ext.getCmp('CREATEMANNAME').setValue(username);
            if (ID != "") {

                Ext.getCmp('REASON').hidden = false;
                Ext.getCmp('REASON').allowBlank = false;
                Ext.getCmp('REASON').blankText = '修改原因不可为空!';

                //默认值的
                Ext.getCmp('formpanel_Win').getForm().setValues(formdata);

            }

            var win = Ext.create("Ext.window.Window", {
                id: "win_d",
                title: 'HS代码库',
                width: 1000,
                height: 250,
                modal: true,
                items: [Ext.getCmp('formpanel_Win')]
            });
            win.show();
        }

        function editCustomer() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
                return;
            }
            addCustomer_Win(recs[0].get("ID"), recs[0].data);
        }

        function Maintain() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
                return;
            }

            util.addMaintain_Win(recs[0].get("ID"), recs[0].data);

            //console.log(recs[0].data);
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
