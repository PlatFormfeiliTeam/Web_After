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
    <script src="/BasicManager/js/busi_RecordInfor.js" type="text/javascript"></script>
    <script type="text/javascript">
        var EXEMPTING = [];
        var tradeway = [];
        var company = [];
        var username = '<%=Username()%>';
        Ext.onReady(function() {
            Ext.Ajax.request({
                url: 'busi_RecordInfor.aspx',
                params: { action: 'getCheckBoxData' },
                type: 'Post',
                success: function (response, option) {
                    var commondata = Ext.decode(response.responseText);
                    
                    //征免方式
                    EXEMPTING = commondata.EXEMPTING;
                    //贸易方式
                    tradeway = commondata.TRADEWAY;
                    //经营单位，收发货单位
                    company = commondata.COMPANY;
                    console.log(company);
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

                }
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
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { addCustomer_Win("", ""); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改', width: 80, handler: function () { editCustomer(); } }
                    //, { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { del(); } }
                    , { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入', width: 80, handler: function () { importfile('add'); } }
                    , { text: '<span class="icon iconfont">&#xe625;</span>&nbsp;导 出', handler: function () { exportdata(); } }
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
                    fields: ['CODE', 'BOOKATTRIBUTE', 'BUSIUNIT', 'BUSIUNITNAME', 'RECEIVEUNIT', 'RECEIVEUNITNAME', 'TRADENAME', 'EXEMPTINGNAME', 'ISMODEL', 'ENABLED', 'CREATEMANNAME', 'STOPMANNAME', 'STARTDATE', 'ENDDATE', 'CREATEDATE', 'REMARK','TRADE','EXEMPTING','ID'],
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
            case "ENABLED": case "ISMODEL":
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


            //账册属性
            var store_BOOKATTRIBUTE = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": "料件账册", "NAME": "料件账册" }, { "CODE": "设备账册", "NAME": "设备账册" }, { "CODE": "金二物流账册", "NAME": "金二物流账册" }, { "CODE": "金二加工贸易账册", "NAME": "金二加工贸易账册" }]
            });
            var combo_BOOKATTRIBUTE = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_BOOKATTRIBUTE',
                name: 'BOOKATTRIBUTE',
                store: store_BOOKATTRIBUTE,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '账册属性', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE',
                allowBlank: false,
                blankText: '账册属性不能为空!'
            });

            //规格型号是否启用
            var store_ISMODEL = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
            });
            var combo_ISMODEL = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_ISMODEL',
                name: 'ISMODEL',
                store: store_ISMODEL,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '规格型号启用', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE',
                value: 1,
                allowBlank: false,
                blankText: '规格型号启用不能为空!'
            });
            //备案信息是否启用
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
            //经营单位

            var store_BUSIUNIT = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data:company
            });

            var combo_BUSIUNIT = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_BUSIUNIT',
                name: 'BUSIUNIT',
                store: store_BUSIUNIT,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '经营单位', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE',
                allowBlank: false,
                blankText: '经营单位不能为空!'
            });
            //收发货单位
            var store_RECEIVEUNIT = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: company
            });

            var combo_RECEIVEUNIT = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_RECEIVEUNIT',
                name: 'RECEIVEUNIT',
                store: store_RECEIVEUNIT,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '收发货单位', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE',
                allowBlank: false,
                blankText: '收发货单位不能为空!'
            });

            //贸易方式
            var store_TRADE = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: tradeway
            });

            var combo_TRADE = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_TRADE',
                name: 'TRADE',
                store: store_TRADE,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '贸易方式', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE',
                allowBlank: false,
                blankText: '贸易方式不能为空!'
            });
            //征免性质
            var store_EXEMPTING = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: EXEMPTING
            });

            var combo_EXEMPTING = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_EXEMPTING',
                name: 'EXEMPTING',
                store: store_EXEMPTING,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '征免性质', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE',
                allowBlank: false,
                blankText: '征免性质不能为空!'
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
                    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_code, combo_BOOKATTRIBUTE] },
                    { layout: 'column', height: 42, border: 0, items: [combo_ISMODEL, combo_ENABLED] },
                    { layout: 'column', height: 42, border: 0, items: [combo_BUSIUNIT, combo_RECEIVEUNIT] },
                    { layout: 'column', height: 42, border: 0, items: [combo_TRADE, combo_EXEMPTING] },
                    { layout: 'column', height: 42, border: 0, items: [start_date, end_date] },
                    { layout: 'column', height: 42, border: 0, items: [CreatemanName, field_REMARK] },
                    { layout: 'column', height: 42, border: 0, items: [change_reason] },


                    field_ID
                ],
                buttons: [{

                    text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {
                        if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                            return;
                        }

                        var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                        Ext.Ajax.request({
                            url: 'busi_RecordInfor.aspx',
                            type: 'Post',
                            params: { action: 'save', formdata: formdata },
                            success: function (response, option) {

                                var data = Ext.decode(response.responseText);
                                if (data.success == "4") {
                                    Ext.Msg.alert('提示',
                                        "保存失败:备案号重复!",
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
                title: '备案信息',
                width: 1200,
                height: 430,
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

        function exportdata() {
            var CodeBase = Ext.getCmp('CodeBase').getValue();
            var RecordBase = Ext.getCmp('RecordBase').getValue();
            var combo_ENABLED_S = Ext.getCmp('combo_ENABLED_S').getValue();
            var path = 'busi_RecordInfor.aspx?action=export&CodeBase=' + CodeBase + '&RecordBase=' + RecordBase + '&combo_ENABLED_S=' + combo_ENABLED_S;
            $('#exportform').attr("action", path).submit();
        }

        function importfile(action) {
            if (action == "add") {
                importexcel(action);
            }

        }


        function importexcel(action) {

            var radio_module = Ext.create('Ext.form.RadioGroup', {
                name: "RADIO_MODULE", id: "RADIO_MODULE", fieldLabel: '模板类型',
                items: [
                    { boxLabel: "<a href='/FileUpload/busi_RecordInfo.xls'><b>模板</b></a>", name: 'RADIO_MODULE', inputValue: '1', checked: true }
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
                                url: 'busi_RecordInfor.aspx',
                                params: { formdata: formdata, action: action },
                                waitMsg: '数据导入中...',
                                success: function (form, action) {
                                    console.log(action.result);
                                    var data = action.result.success;
                                    var reg = /,$/gi;
                                    idStr = data.replace(reg, "!");
                                    Ext.Msg.alert('提示', idStr, function () {
                                        pgbar.moveFirst();
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
                title: '国家代码对应关系',
                width: 600,
                height: 240,
                modal: true,
                items: [Ext.getCmp('formpanel_upload')]
            });
            Ext.getCmp('CREATEMANNAME').setValue(username);
            win_upload.show();
        }


        function Maintain() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
                return;
            }

            util.addMaintain_Win(recs[0].get("ID"), recs[0].data);
        }

    </script>
</head>
<body>
<form id="exportform" name="form" enctype="multipart/form-data" method="post"> <%--style="display:inline-block"--%>
                   
</form> 
</body>
</html>
