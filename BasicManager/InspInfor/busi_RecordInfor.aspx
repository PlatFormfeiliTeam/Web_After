<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="busi_RecordInfor.aspx.cs" Inherits="Web_After.BasicManager.InspInfor.busi_RecordInfor" %>

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
        Ext.onReady(function() {
            init_search();
            grindbind();
            var panel = Ext.create('Ext.form.Panel', {
                title: '备案信息',
                region: 'center',
                layout: 'border',
                items: [Ext.getCmp('formpanel_search'), Ext.getCmp('gridpanel')]
            });

            var viewport = Ext.create('Ext.container.Viewport',
                {
                    layout: 'border',
                    items: [panel]
                });
        });
        function init_search() {
            var CodeBase = Ext.create('Ext.form.field.Text', { id: 'CodeBase', name: 'CodeBase', fieldLabel: '经营单位代码' });
            var RecordBase = Ext.create('Ext.form.field.Text', { id: 'RecordBase', name: 'RecordBase', fieldLabel: '备案号' });
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
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { addCustomer_Win("", "", param); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改', width: 80, handler: function () { editCustomer(param); } }
                    //, { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { del(); } }
                    , { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入', width: 80, handler: function () { importfile('add', param); } }
                    , { text: '<span class="icon iconfont">&#xe625;</span>&nbsp;导 出', handler: function () { exportdata(); } }
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
                    labelWidth: 90
                },
                items: [
                    { layout: 'column', border: 0, items: [CodeBase, RecordBase, combo_ENABLED_S] }

                ]
            });
        }

        //页面数据加载
        function grindbind() {
            var CiqDataBase = Ext.create('Ext.data.JsonStore',
                {
                    fields: ['CODE', 'BOOKATTRIBUTE', 'BUSIUNIT', 'BUSIUNITNAME', 'RECEIVEUNIT', 'RECEIVEUNITNAME', 'TRADENAME', 'EXEMPTINGNAME', 'ISMODEL', 'ENABLED', 'CREATEMANNAME', 'STOPMANNAME', 'STARTDATE', 'ENDDATE', 'CREATEDATE', 'REMARK','ID'],
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'busi_RecordInfor.aspx?action=loadData',
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
                        { header: '备案号', dataIndex: 'CODE', width: 200 },
                        { header: '账册属性', dataIndex: 'BOOKATTRIBUTE', width: 150 },
                        { header: '经营单位代码', dataIndex: 'BUSIUNIT', width: 150 },
                        { header: '经营单位名称', dataIndex: 'BUSIUNITNAME', width: 150 },
                        { header: '收发货单位代码', dataIndex: 'RECEIVEUNIT', width: 150 },
                        { header: '收发货单位名称', dataIndex: 'RECEIVEUNITNAME', width: 150 },
                        { header: '贸易方式', dataIndex: 'TRADENAME', width: 150 },
                        { header: '征免性质', dataIndex: 'EXEMPTINGNAME', width: 150 },
                        { header: '规格启用/禁用', dataIndex: 'ISMODEL', width: 150, renderer: gridrender },
                        { header: '启用/禁用', dataIndex: 'ENABLED', width: 150, renderer: gridrender },
                        { header: '维护人', dataIndex: 'CREATEMANNAME', width: 150 },
                        { header: '停用人', dataIndex: 'STOPMANNAME', width: 150 },
                        { header: '启用时间', dataIndex: 'STARTDATE', width: 250 },
                        { header: '停用时间', dataIndex: 'ENDDATE', width: 250 },
                        { header: '维护时间', dataIndex: 'CREATEDATE', width: 250 },
                        { header: '备注', dataIndex: 'REMARK', width: 284 },
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

        function reset() {
            Ext.each(Ext.getCmp('formpanel_search').getForm().getFields().items,
                function (field) {
                    field.reset();
                });
        }

        function form_ini_win() {
            var field_ID = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });

            var field_code = Ext.create('Ext.form.field.Text', {
                id: 'CODE',
                name: 'CODE',
                fieldLabel: '备案号',
                flex: .5,
                allowBlank: false,
                blankText: '备案号不可为空!'
            });

            var field_Name = Ext.create('Ext.form.field.Text', {
                id: 'NAME',
                name: 'NAME',
                fieldLabel: '企业名称',
                flex: .5,
                allowBlank: false,
                blankText: '企业名称不可为空!'
            });

            var field_EnglishName = Ext.create('Ext.form.field.Text', {
                id: 'ENGLISHNAME',
                name: 'ENGLISHNAME',
                fieldLabel: '企业英文名',
                flex: .5

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

            var field_Code = Ext.create('Ext.form.field.Text', {
                id: 'CODE',
                name: 'CODE',
                fieldLabel: '海关编码',
                flex: .5,
                allowBlank: false,
                blankText: '海关编码不可为空!',
                maxLength: 10

            });

            var field_Inspcode = Ext.create('Ext.form.field.Text', {
                id: 'INSPCODE',
                name: 'INSPCODE',
                fieldLabel: '商检编码',
                flex: .5,

            });

            var store_declnature = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 1, "NAME": "国有" }, { "CODE": 2, "NAME": "合作" }, { "CODE": 3, "NAME": "合资" }, { "CODE": 4, "NAME": "独资" }, { "CODE": 5, "NAME": "集体" },
                    { "CODE": 6, "NAME": "私营" }, { "CODE": 7, "NAME": "个体商户" }, { "CODE": 8, "NAME": "报关" }, { "CODE": 9, "NAME": "其它" }]
            });

            var combo_declnature = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_declnature',
                name: 'DECLNATURENAME',
                store: store_declnature,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '海关性质', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE'

            });

            var store_inspanture = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 5, "NAME": "集体企业" }, { "CODE": 3, "NAME": "中外合资企业" }, { "CODE": 1, "NAME": "国有企业" }, { "CODE": 6, "NAME": "私营企业" }, { "CODE": 4, "NAME": "外商独资企业" },
                    { "CODE": 2, "NAME": "中外合作企业" }, { "CODE": 9, "NAME": "其它" }]
            });

            var combo_inspanture = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_inspanture',
                name: 'INSPNATURENAME',
                store: store_inspanture,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '商检性质', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE'

            });

            var field_Goodslocal = Ext.create('Ext.form.field.Text', {
                id: 'GOODSLOCAL',
                name: 'GOODSLOCAL',
                fieldLabel: '货物存放地',
                flex: .5

            });

            var store_receiver = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 001, "NAME": "IUBSID" }]
            });

            var combo_receiver = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_receiver',
                name: 'RECEIVERTYPE',
                store: store_receiver,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '收货人类型', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE'

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

            var field_Socialcreditno = Ext.create('Ext.form.field.Text', {
                id: 'SOCIALCREDITNO',
                name: 'SOCIALCREDITNO',
                fieldLabel: '社会信用代码',
                flex: .5
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

            var formpanel_Win = Ext.create('Ext.form.Panel', {
                id: 'formpanel_Win',
                minHeight: 170,
                border: 0,
                buttonAlign: 'center',
                fieldDefaults: {
                    margin: '0 5 10 0',
                    labelWidth: 80,
                    columnWidth: .5,
                    labelAlign: 'right',
                    labelSeparator: '',
                    msgTarget: 'under'
                },
                items: [
                    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_Incode, field_Name] },
                    { layout: 'column', height: 42, border: 0, items: [field_EnglishName, combo_ENABLED] },
                    { layout: 'column', height: 42, border: 0, items: [field_Code, field_Inspcode] },
                    { layout: 'column', height: 42, border: 0, items: [combo_declnature, combo_inspanture] },
                    { layout: 'column', height: 42, border: 0, items: [field_Goodslocal, combo_receiver] },
                    { layout: 'column', height: 42, border: 0, items: [start_date, end_date] },
                    { layout: 'column', height: 42, border: 0, items: [CreatemanName, field_Socialcreditno] },
                    { layout: 'column', height: 42, border: 0, items: [field_REMARK, change_reason] },

                    field_ID
                ],
                buttons: [{

                    text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {
                        console.log('bbbb');
                        if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {

                            return;
                        }

                        var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                        console.log('aaaaa');
                        Ext.Ajax.request({
                            url: 'Base_Company.aspx',
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

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
