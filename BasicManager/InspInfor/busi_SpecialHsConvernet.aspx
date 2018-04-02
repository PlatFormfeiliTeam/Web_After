<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="busi_SpecialHsConvernet.aspx.cs" Inherits="Web_After.BasicManager.InspInfor.busi_SpecialHsConvernet" %>

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
    <script type="text/javascript">
        var country = [];
        var username = '<%=Username()%>';
        Ext.onReady(function () {
            Ext.Ajax.request({
                url: 'busi_SpecialHsConvernet.aspx',
                params: { action: 'getCheckBoxData' },
                type: 'Post',
                success: function(response, option) {
                    var commondata = Ext.decode(response.responseText);
                    country = commondata.COUNTRY;


                }
            });
            init_search();
            grindbind();
            var panel = Ext.create('Ext.form.Panel', {
                title: '特殊商品单位换算',
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
            var CodeBase = Ext.create('Ext.form.field.Text', { id: 'CodeBase', name: 'CodeBase', fieldLabel: '商品HS编码' });
            var RecordBase = Ext.create('Ext.form.field.Text', { id: 'RecordBase', name: 'RecordBase', fieldLabel: '商品名称' });
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
                    , { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入', width: 80, handler: function () { importfile('add'); } }
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
                    fields: ['CODE', 'EXTRACODE', 'NAME', 'TYPE', 'COUNTRY', 'ENABLED', 'STARTDATE', 'CREATEMANNAME', 'CREATEDATE','STOPMANNAME', 'ENDDATE', 'REMARK', 'ID'],
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'busi_SpecialHsConvernet.aspx?action=loadData',
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
                        { header: '商品HS编码', dataIndex: 'CODE', width: 200 },
                        { header: '附加码', dataIndex: 'EXTRACODE', width: 150 },
                        { header: '商品名称', dataIndex: 'NAME', width: 150 },
                        { header: '类型', dataIndex: 'TYPE', width: 150 },
                        { header: '国家', dataIndex: 'COUNTRY', width: 150 },
                        { header: '是否启用', dataIndex: 'ENABLED', width: 150, renderer: gridrender },
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
                fieldLabel: '商品HS编码',
                flex: .5,
                allowBlank: false,
                blankText: '商品HS编码不可为空!'
            });
            var field_extracode = Ext.create('Ext.form.field.Text', {
                id: 'EXTRACODE',
                name: 'EXTRACODE',
                fieldLabel: '附加码',
                flex: .5,
                allowBlank: false,
                blankText: '附加码不可为空!'
            });
            var field_name = Ext.create('Ext.form.field.Text', {
                id: 'NAME',
                name: 'NAME',
                fieldLabel: '商品名称',
                flex: .5
            });
            //国家，下拉框的显示
            var store_country = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: country
            });

            var storesdf = Ext.create('Ext.form.field.ComboBox', {
                id:'COUNTRY',
                name: 'COUNTRY',
                fieldLabel: '国家',
                multiSelect: true,
                store: store_country,
                listConfig: {
                    itemTpl: Ext.create('Ext.XTemplate', '<input type=checkbox>{[values.NAME]}'),
                    onItemSelect: function (record) {
                        var node = this.getNode(record);
                        if (node) {
                            Ext.fly(node).addCls(this.selectedItemCls);
                            var checkboxs = node.getElementsByTagName("input");
                            if (checkboxs != null)
                                var checkbox = checkboxs[0];
                            checkbox.checked = true;
                        }
                    },
                    listeners: {
                        itemclick: function (view, record, item, index, e, eOpts) {
                            var isSelected = view.isSelected(item);
                            var checkboxs = item.getElementsByTagName("input");
                            if (checkboxs != null) {
                                var checkbox = checkboxs[0];
                                if (!isSelected) {
                                    checkbox.checked = true;
                                } else {
                                    checkbox.checked = false;
                                }
                            }
                        }
                    }
                },
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE'
            });
            //***********************
            var field_type = Ext.create('Ext.form.field.Text', {
                id: 'TYPE',
                name: 'TYPE',
                fieldLabel: '类型',
                flex: .5
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
                    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_code, field_extracode] },
                    { layout: 'column', height: 42, border: 0, items: [field_name, storesdf] },
                    { layout: 'column', height: 42, border: 0, items: [field_type, start_date] },
                    { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, end_date] },
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
                        var country = Ext.getCmp('COUNTRY').getValue();
                        Ext.Ajax.request({
                            url: 'busi_SpecialHsConvernet.aspx',
                            type: 'Post',
                            params: { action: 'save', formdata: formdata ,country:country},
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
                height: 350,
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

        //导出
        function exportdata() {
            var CodeBase = Ext.getCmp('CodeBase').getValue();
            var RecordBase = Ext.getCmp('RecordBase').getValue();
            var combo_ENABLED_S = Ext.getCmp('combo_ENABLED_S').getValue();
            var path = 'busi_SpecialHsConvernet.aspx?action=export&CodeBase=' + CodeBase + '&RecordBase=' + RecordBase + '&combo_ENABLED_S=' + combo_ENABLED_S;
            $('#exportform').attr("action", path).submit();
        }

        //导入
        function importfile(action) {
            if (action == "add") {
                importexcel(action);
            }

        }

        function importexcel(action) {

            var radio_module = Ext.create('Ext.form.RadioGroup', {
                name: "RADIO_MODULE", id: "RADIO_MODULE", fieldLabel: '模板类型',
                items: [
                    { boxLabel: "<a href='/FileUpload/busi_SpecialHsConvernet.xls'><b>模板</b></a>", name: 'RADIO_MODULE', inputValue: '1', checked: true }
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
                                url: 'busi_SpecialHsConvernet.aspx',
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
                title: '特殊商品单位换算导入',
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
<form id="exportform" name="form" enctype="multipart/form-data" method="post"> <%--style="display:inline-block"--%>
                   
</form> 
</body>
</html>
