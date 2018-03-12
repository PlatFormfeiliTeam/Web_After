<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeclStatusConfig.aspx.cs" Inherits="Web_After.BasicManager.DataRela.DeclStatusConfig" %>

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
        var title = "回执状态";
        var username = '<%=Username()%>';
        var common_data_declstatus = [];
        var store_declstatus;

        Ext.onReady(function () {
            Ext.Ajax.request({
                url: 'DeclStatusConfig.aspx',
                params: { action: 'Ini_Base_Data' },
                type: 'Post',
                success: function (response, option) {
                    var commondata = Ext.decode(response.responseText);
                    common_data_declstatus = commondata.DECLSTATUS;//

                    store_declstatus = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_declstatus });
       

                    init_search();
                    gridbind();

                    var panel = Ext.create('Ext.form.Panel', {
                        title: title,
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
            var searchCode = Ext.create('Ext.form.field.Text', { id: 'STATUSCODE', name: 'STATUSCODE', fieldLabel: '回执状态码' });
            var searchName = Ext.create('Ext.form.field.Text', { id: 'STATUSNAME', name: 'STATUSNAME', fieldLabel: '回执状态名称' });

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
                    //, { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入', width: 80, handler: function () { importfile('add'); } }
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
                    labelWidth: 100
                },
                items: [
                    { layout: 'column', border: 0, items: [searchCode,searchName, combo_ENABLED_S] }

                ]
            });
        }

        //数据绑定
        function gridbind() {
            var store_customer = Ext.create('Ext.data.JsonStore',
                {
                    fields: ['CODE', 'NAME', 'BUSISTATUSNAME', 'TYPE', 'DESCRIPTION','ORDERNO',
                         'ENABLED', 'STARTDATE', 'CREATEMANNAME', 'CREATEDATE', 'STOPMANNAME','ENDDATE','REMARK','BUSISTATUS','ID'],
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'DeclStatusConfig.aspx?action=loadData',
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
                    { header: '回执状态代码', dataIndex: 'CODE', width: 150 },
                    { header: '回执状态名称', dataIndex: 'NAME', width: 150 },
                    { header: '业务状态', dataIndex: 'BUSISTATUSNAME', width: 150 },
                    { header: '所属类型', dataIndex: 'TYPE',renderer:gridrender, width: 150 },
                     { header: '显示描述', dataIndex: 'DESCRIPTION', width: 100 },
                     { header: '序号', dataIndex: 'ORDERNO', width: 100 },  
                    { header: '启用', dataIndex: 'ENABLED', renderer: gridrender, width: 100 },
                    { header: '启用时间', dataIndex: 'STARTDATE', width: 100 },
                    { header: '维护人名称', dataIndex: 'CREATEMANNAME', width: 100 },
                    { header: '维护时间', dataIndex: 'CREATEDATE', width: 150 },
                    { header: '停用人', dataIndex: 'STOPMANNAME', width: 100 },
                    { header: '停用时间', dataIndex: 'ENDDATE', width: 100 },
                    { header: '备注', dataIndex: 'REMARK', width: 100 },
                    { header: 'BUSISTATUS', dataIndex: 'BUSISTATUS', width: 200, hidden: true },
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

        function gridrender(value, cellmeta, record, rowIndex, columnIndex, stroe) {
            var dataindex = cellmeta.column.dataIndex;
            var str = "";
            switch (dataindex) {
                case "ENABLED":
                    str = value == "1" ? '<span class="icon iconfont" style="font-size:12px;color:blue;">&#xe628;</span>' : '<span class="icon iconfont" style="font-size:12px;color:red;">&#xe634;</span>';
                    break;
                case "TYPE":
                    str = value == "1" ? '报关' : '报检';
            }
            return str;
        }

        //重置查询条件
        function reset() {
            Ext.each(Ext.getCmp('formpanel_search').getForm().getFields().items,
                function (field) {
                    field.reset();
                });
        }

        //新增
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
                title: '回执状态',
                width: 1200,
                height: 430,
                modal: true,
                items: [Ext.getCmp('formpanel_Win')]
            });
            win.show();
        }


        function form_ini_win() {
            var field_ID = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });
            var field_BusiStatus =Ext.create('Ext.form.field.Hidden', {
                id: 'BUSISTATUS',
                name: 'BUSISTATUS'
            });

            var store_for_typeclass = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 1, "NAME": "报关" }, { "CODE": 2, "NAME": "报检" }]
            });

            var store_for_status = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_declstatus
            });

            var field_Code = Ext.create('Ext.form.field.Text', {
                id: 'CODE',
                name: 'CODE',
                fieldLabel: '回执状态代码',
                flex: .5,
                allowBlank: false,
                blankText: '回执状态代码不可为空!',
            });

            var field_Name = Ext.create('Ext.form.field.Text', {
                id: 'NAME',
                name: 'NAME',
                fieldLabel: '回执状态名称',
                flex: .5,
                allowBlank: false,
                blankText: '回执状态名称不可为空!',
            });

            var field_BusiStatusName = Ext.create('Ext.form.field.ComboBox', {
                id: 'BUSISTATUSNAME',
                name: 'BUSISTATUSNAME',
                store: store_for_status,
                hideTrigger: true,
                minChars: 1,
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE',
                anyMatch: true,
                fieldLabel: '业务状态',
                flex: .5,
                allowBlank: true,
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    }
                }
            });

            var field_Type = Ext.create('Ext.form.field.ComboBox', {
                id: 'TYPE',
                name: 'TYPE',
                store: store_for_typeclass,
                minChars: 1,
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE',
                anyMatch: true,
                fieldLabel: '所属类型',
                flex: .5,
                allowBlank: false,
                blankText: '所属类型不可为空!',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    }
                }
            });


            var field_Description = Ext.create('Ext.form.field.Text', {
                id: 'DESCRIPTION',
                name: 'DESCRIPTION',
                fieldLabel: '显示描述',
                flex: .5,
                allowBlank: false,
                blankText: '显示描述不可为空!',
            });

            var field_OrderNo = Ext.create('Ext.form.field.Text', {
                id: 'ORDERNO',
                name: 'ORDERNO',
                fieldLabel: '序号',
                flex: .5,
                allowBlank: false,
                blankText: '序号不可为空!',
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
                    labelWidth: 100,
                    columnWidth: .5,
                    labelAlign: 'right',
                    labelSeparator: '',
                    msgTarget: 'under'
                },
                items: [
                    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_Code, field_Name] },
                    { layout: 'column', height: 42, border: 0, items: [field_BusiStatusName, field_Type] },
                    { layout: 'column', height: 42, border: 0, items: [field_Description, field_OrderNo] },
                    { layout: 'column', height: 42, border: 0, items: [combo_ENABLED] },
                    { layout: 'column', height: 42, border: 0, items: [start_date, end_date] },
                    { layout: 'column', height: 42, border: 0, items: [CreatemanName] },
                    { layout: 'column', height: 42, border: 0, items: [field_REMARK, change_reason] },
                    field_BusiStatus,
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
                            url: 'DeclStatusConfig.aspx',
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


        function editCustomer() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
                return;
            }
            addCustomer_Win(recs[0].get("ID"), recs[0].data);
        }


        function exportdata() {
            var STATUSCODE = Ext.getCmp('STATUSCODE').getValue();
            var STATUSNAME = Ext.getCmp('STATUSNAME').getValue();
            var combo_ENABLED_S = Ext.getCmp('combo_ENABLED_S').getValue();
            var path = 'DeclStatusConfig.aspx?action=export&STATUSCODE=' + STATUSCODE + '&STATUSNAME=' + STATUSNAME + '&combo_ENABLED_S=' + combo_ENABLED_S;
            $('#exportform').attr("action", path).submit();
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
