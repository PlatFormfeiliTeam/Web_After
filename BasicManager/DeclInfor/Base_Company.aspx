<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Base_Company.aspx.cs" Inherits="Web_After.BasicManager.DeclInfor.Base_Company" %>

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
        var username = '<%=Username()%>';
        Ext.onReady(function () {
            init_search();
            gridbind();

            var panel = Ext.create('Ext.form.Panel', {
                title: '企业信息',
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

            var txtCODE = Ext.create('Ext.form.field.Text', { id: 'CODE_S', name: 'CODE_S', fieldLabel: '内部编码' });
            var txtCNNAME = Ext.create('Ext.form.field.Text', { id: 'CNNAME_S', name: 'CNNAME_S', fieldLabel: '企业名称' });

            var txtSEACODE = Ext.create('Ext.form.field.Text', { id: 'CODE_Sea', name: 'CODE_Sea', fieldLabel: '海关编码' });
            var txtInspCODE = Ext.create('Ext.form.field.Text', { id: 'CODE_Insp', name: 'CODE_Insp', fieldLabel: '商检编码' });

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
                    //, { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { del(); } }
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
                    labelWidth: 70
                },
                items: [
                    { layout: 'column', border: 0, items: [txtCODE, txtCNNAME, txtSEACODE, txtInspCODE, combo_ENABLED_S] }
                
                ]
            });
        }

        function gridbind() {
            var store_customer = Ext.create('Ext.data.JsonStore',
                {
                    fields: [
                        'INCODE', 'CODE', 'INSPCODE', 'SOCIALCREDITNO',
                        'NAME', 'ENGLISHNAME', 'DECLNATURENAME', 'INSPNATURENAME',
                        'GOODSLOCAL', 'RECEIVERTYPE', 'ENABLED', 'STARTDATE', 'CREATEMANNAME',
                        'CREATEDATE', 'STOPMANNAME', 'ENDDATE', 'REMARK','ID'
                    ],
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'Base_Company.aspx?action=loadData',
                        reader: {
                            root: 'rows',
                            type: 'json',
                            totalProperty: 'total'
                        }
                    },
                    autoLoad: true,
                    listeners: {
                        beforeload: function(store, options) {
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
                    { header: '内部编码', dataIndex: 'INCODE', width: 145, locked: true },
                    { header: '海关编码', dataIndex: 'CODE', width: 100, locked: true },
                    { header: '商检编码', dataIndex: 'INSPCODE', width: 110, locked: true },
                    { header: '社会信用代码', dataIndex: 'SOCIALCREDITNO', locked: true, width: 144 },

                    { header: '企业名称', dataIndex: 'NAME', width: 211, tdCls: 'tdValign' },
                    { header: '英文名称', dataIndex: 'ENGLISHNAME', width: 140 },
                    { header: '海关性质', dataIndex: 'DECLNATURENAME', width: 70 },
                    { header: '商检性质', dataIndex: 'INSPNATURENAME', width: 93 },

                    { header: '货物存放地', dataIndex: 'GOODSLOCAL', width: 84 },
                    { header: '收货人类型', dataIndex: 'RECEIVERTYPE', width: 84 },
                    { header: '启用情况', dataIndex: 'ENABLED', renderer: gridrender, width: 73 },
                    { header: '启用时间', dataIndex: 'STARTDATE', width: 83 },

                    { header: '维护人', dataIndex: 'CREATEMANNAME',  width: 71 },
                    { header: '维护时间', dataIndex: 'CREATEDATE',  width: 83 },
                    { header: '停用人', dataIndex: 'STOPMANNAME', width: 71 },
                    { header: '停用时间', dataIndex: 'ENDDATE', width: 83 },
                    { header: '备注', dataIndex: 'REMARK', width: 200 },
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
                function(field) {
                    field.reset();
                });
        }

        function form_ini_win() {
            var field_ID = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });

            var field_Incode = Ext.create('Ext.form.field.Text', {
                id: 'INCODE',
                name: 'INCODE',
                fieldLabel: '内部编码',
                flex: .5,
                allowBlank: false,
                blankText: '内部编码不可为空!'
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
                //allowBlank: false,
                //blankText: '海关编码不可为空!',
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
                                        function() {
                                            Ext.getCmp("pgbar").moveFirst();
                                            Ext.getCmp("win_d").close();
                                        });
                                } else {
                                    var errorMsg = data.success;
                                    var reg = /,$/gi;
                                    idStr = errorMsg.replace(reg, "!");
                                    Ext.Msg.alert('提示', "保存失败:"+ idStr, function () {
                                        Ext.getCmp("pgbar").moveFirst(); Ext.getCmp("win_d").close();
                                    });
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


                Ext.getCmp('INCODE').readOnly = true;

                //默认值的
                Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
               


            }

            var win = Ext.create("Ext.window.Window", {
                id: "win_d",
                title: '企业',
                width: 1200,
                height:430,
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
            
            var CODE_S = Ext.getCmp('CODE_S').getValue();
            var CNNAME_S = Ext.getCmp('CNNAME_S').getValue();
            var CODE_Sea = Ext.getCmp('CODE_Sea').getValue();
            var CODE_Insp = Ext.getCmp('CODE_Insp').getValue();
            var combo_ENABLED_S = Ext.getCmp('combo_ENABLED_S').getValue();
            var path = 'Base_Company.aspx?action=export&CODE_S=' + CODE_S + '&CNNAME_S=' + CNNAME_S + '&CODE_Sea=' + CODE_Sea + '&CODE_Insp=' + CODE_Insp + '&combo_ENABLED_S=' + combo_ENABLED_S;
            $('#exportform').attr("action", path).submit();

        }


        function importfile(action)
        {
            if (action == "add") {
                importexcel(action);
            }

        }

        function importexcel(action) {

            var radio_module = Ext.create('Ext.form.RadioGroup', {
                name: "RADIO_MODULE", id: "RADIO_MODULE", fieldLabel: '模板类型',
                items: [
                    { boxLabel: "<a href='/FileUpload/Base_company.xls'><b>模板</b></a>", name: 'RADIO_MODULE', inputValue: '1', checked: true }
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
                                type:'Post',
                                url: 'Base_Company.aspx',
                                params: { formdata: formdata, action: action},
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
                title: '企业导入',
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
    
    <div>
        <form id="exportform" name="form" enctype="multipart/form-data" method="post"> <%--style="display:inline-block"--%>
                   
        </form>   
    </div>
    
</body>
</html>
