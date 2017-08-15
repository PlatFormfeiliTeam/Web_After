<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerManage.aspx.cs" Inherits="Web_After.CustomerMaintain.CustomerManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />
    <script src="/js/pan.js" type="text/javascript"></script>
    <script src="/js/import/importExcel.js" type="text/javascript"></script>
    <script type="text/javascript" >
        Ext.onReady(function () {
            init_search();
            gridbind();

            var panel= Ext.create('Ext.form.Panel', {
                title: '客商管理',
                region: 'center',
                layout: 'border',
                items: [Ext.getCmp('formpanel_search'), Ext.getCmp('gridpanel')]
            });
            var viewport = Ext.create('Ext.container.Viewport', {
                layout: 'border',
                items: [panel]
            })
        });

        function init_search() {
            var txtCODE = Ext.create('Ext.form.field.Text', { id: 'CODE_S', name: 'CODE_S', fieldLabel: '客户代码' });
            var txtCNNAME = Ext.create('Ext.form.field.Text', { id: 'CNNAME_S', name: 'CNNAME_S', fieldLabel: '中文名称' });
            var txtENGLISHNAME = Ext.create('Ext.form.field.Text', { id: 'ENGLISHNAME_S', name: 'ENGLISHNAME_S', fieldLabel: '英文名称' });
            var txtHSCODE = Ext.create('Ext.form.field.Text', { id: 'HSCODE_S', name: 'HSCODE_S', fieldLabel: '海关编码' });
            var txtCIQCODE = Ext.create('Ext.form.field.Text', { id: 'CIQCODE_S', name: 'CIQCODE_S', fieldLabel: '国检代码' });

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
                    , { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { del(); } }
                    , { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入', width: 80, handler: function () { onItemUpload('customer'); } }
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
                { layout: 'column', border: 0, items: [txtCODE, txtCNNAME, txtHSCODE, txtCIQCODE, txtENGLISHNAME, combo_ENABLED_S] },
                { layout: 'column', border: 0, items: [txtENGLISHNAME, combo_ENABLED_S] }
                ]
            });
        }

        function gridbind() {
            var store_customer = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'HSCODE', 'CIQCODE', 'NAME', 'CHINESEABBREVIATION', 'CHINESEADDRESS', 'ISCUSTOMER'
                    , 'ISSHIPPER', 'ISCOMPANY', 'DOCSERVICECOMPANY', 'ENABLED', 'LOGICAUDITFLAG', 'SOCIALCREDITNO', 'TOOLVERSION'
                    , 'ENGLISHNAME', 'ENGLISHADDRESS', 'ID'],
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: 'CustomerManage.aspx?action=loadData',
                    reader: {
                        root: 'rows',
                        type: 'json',
                        totalProperty: 'total'
                    }
                },
                autoLoad: true,
                listeners: {
                    beforeload: function (store, options) {
                        store_customer.getProxy().extraParams = Ext.getCmp('formpanel_search').getForm().getValues();
                    }
                }
            })
            var pgbar = Ext.create('Ext.toolbar.Paging', {
                id: 'pgbar',
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: store_customer,
                displayInfo: true
            })
            var gridpanel = Ext.create('Ext.grid.Panel', {
                id: 'gridpanel',
                height: 560,
                region: 'center',
                store: store_customer,
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                columns: [
                    { xtype: 'rownumberer', width: 35 },
                    { header: '客户代码', dataIndex: 'CODE', width: 120, locked: true },
                    { header: '海关编码', dataIndex: 'HSCODE', width: 100, locked: true },
                    { header: '国检编码', dataIndex: 'CIQCODE', width: 100, locked: true },
                    { header: '中文简称', dataIndex: 'CHINESEABBREVIATION', locked: true, width: 180 },
                    { header: '中文名称', dataIndex: 'NAME', width: 250, tdCls: 'tdValign' },
                    { header: '中文地址', dataIndex: 'CHINESEADDRESS', width: 250 },
                    { header: '客户', dataIndex: 'ISCUSTOMER', renderer: gridrender, width: 60 },
                    { header: '供应商', dataIndex: 'ISSHIPPER', renderer: gridrender, width: 60 },
                    { header: '生产型企业', dataIndex: 'ISCOMPANY', renderer: gridrender, width: 80 },
                    { header: '单证服务单位', dataIndex: 'DOCSERVICECOMPANY', renderer: gridrender, width: 100 },
                    { header: '是否启用', dataIndex: 'ENABLED', renderer: gridrender, width: 70 },
                    { header: '逻辑审核强制通过', dataIndex: 'LOGICAUDITFLAG', renderer: gridrender, width: 120 },
                    { header: '核销比对', dataIndex: 'SOCIALCREDITNO', width: 100, renderer: gridrender },
                    { header: '工具版本', dataIndex: 'TOOLVERSION', width: 100, renderer: gridrender },
                    { header: '英文名称', dataIndex: 'ENGLISHNAME', width: 200 },
                    { header: '英文地址', dataIndex: 'ENGLISHADDRESS', width: 200 },
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
                case "ISCUSTOMER":
                case "ISSHIPPER":
                case "ISCOMPANY":
                case "DOCSERVICECOMPANY":
                case "ENABLED":
                case "LOGICAUDITFLAG":
                    str = value == "0" ? "否" : "是";
                    break;
                case "SOCIALCREDITNO":
                    str = value == "N" ? "未开通" : "已开通";
                    break;
                case "TOOLVERSION":
                    if (value == "0") { str = "简化版"; }
                    if (value == "1") { str = "标准版"; }
                    if (value == "2") { str = "高级版"; }
                    break;
            }
            return str;
        }

        function form_ini_win() {
            var field_ID = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });

            var field_CODE = Ext.create('Ext.form.field.Text', {
                id: 'CODE',
                name: 'CODE',
                fieldLabel: '客户代码', flex: .5,
                allowBlank: false,
                blankText: '客户代码不可为空!'
            });

            var field_CHINESEABBREVIATION = Ext.create('Ext.form.field.Text', {
                id: 'CHINESEABBREVIATION',
                name: 'CHINESEABBREVIATION',
                fieldLabel: '中文简称', flex: .5,
            });

            var con_CODECHIN = {
                xtype: 'fieldcontainer',
                layout: 'hbox', margin: 0,
                items: [field_CODE, field_CHINESEABBREVIATION]
            }

            var field_HSCODE = Ext.create('Ext.form.field.Text', {
                id: 'HSCODE',
                name: 'HSCODE',
                fieldLabel: '海关编码', flex: .5
            });
            var field_CIQCODE = Ext.create('Ext.form.field.Text', {
                id: 'CIQCODE',
                name: 'CIQCODE',
                fieldLabel: '国检代码', flex: .5
            });

            var con_HSCIQ = {
                xtype: 'fieldcontainer',
                layout: 'hbox', margin: 0,
                items: [field_HSCODE, field_CIQCODE]
            }

            var field_NAME = Ext.create('Ext.form.field.Text', {
                id: 'NAME',
                name: 'NAME',
                fieldLabel: '中文名称',
                allowBlank: false,
                blankText: '中文名称不可为空!'
            });

            var field_CHINESEADDRESS = Ext.create('Ext.form.field.Text', {
                id: 'CHINESEADDRESS',
                name: 'CHINESEADDRESS',
                fieldLabel: '中文地址'
            });

            var field_ENGLISHNAME = Ext.create('Ext.form.field.Text', {
                id: 'ENGLISHNAME',
                name: 'ENGLISHNAME',
                fieldLabel: '英文名称'
            });
            var field_ENGLISHADDRESS = Ext.create('Ext.form.field.Text', {
                id: 'ENGLISHADDRESS',
                name: 'ENGLISHADDRESS',
                fieldLabel: '英文地址'
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


            var store_SOCIALCREDITNO = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": "N", "NAME": "未开通" }, { "CODE": "Y", "NAME": "已开通" }]
            });
            var combo_SOCIALCREDITNO = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_SOCIALCREDITNO',
                name: 'SOCIALCREDITNO',
                store: store_SOCIALCREDITNO,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '核销比对', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE',
                value: "N",
                allowBlank: false,
                blankText: '核销比对不能为空!'
            });

            var con_COMBOBOX = {
                xtype: 'fieldcontainer',
                layout: 'hbox', margin: 0,
                items: [combo_ENABLED, combo_SOCIALCREDITNO]
            }

            var rgb_TOOLVERSION = Ext.create('Ext.form.RadioGroup', {
                id: 'rgb_TOOLVERSION',
                name: "TOOLVERSION",
                fieldLabel: '工具版本',
                items: [
                    { boxLabel: '简化版', name: 'TOOLVERSION', inputValue: 0, checked: true },
                    { boxLabel: '标准版', name: 'TOOLVERSION', inputValue: 1 },
                    { boxLabel: '高级版', name: 'TOOLVERSION', inputValue: 2 }
                ]
            });

            var chk_ISCUSTOMER = Ext.create("Ext.form.field.Checkbox", {
                id: 'ISCUSTOMER',
                name: 'ISCUSTOMER',
                fieldLabel: '客户', flex: .2
            });
            var chk_ISSHIPPER = Ext.create("Ext.form.field.Checkbox", {
                id: 'ISSHIPPER',
                name: 'ISSHIPPER',
                fieldLabel: '供应商', flex: .2
            });

            var chk_ISCOMPANY = Ext.create("Ext.form.field.Checkbox", {
                id: 'ISCOMPANY',
                name: 'ISCOMPANY',
                fieldLabel: '生产型企业', flex: .2
            });
            var chk_DOCSERVICECOMPANY = Ext.create("Ext.form.field.Checkbox", {
                id: 'DOCSERVICECOMPANY',
                name: 'DOCSERVICECOMPANY',
                fieldLabel: '单证服务单位', labelWidth: 80, flex: .2
            });
            var chk_LOGICAUDITFLAG = Ext.create("Ext.form.field.Checkbox", {
                id: 'LOGICAUDITFLAG',
                name: 'LOGICAUDITFLAG',
                fieldLabel: '逻辑审核强制通过', labelWidth: 110, flex: .2
            });

            var con_CHK = {
                columnWidth: 1,
                xtype: 'fieldcontainer',
                layout: 'hbox', margin: 0,
                items: [chk_ISCUSTOMER, chk_ISSHIPPER, chk_ISCOMPANY, chk_DOCSERVICECOMPANY, chk_LOGICAUDITFLAG]
            }
            var field_REMARK = Ext.create('Ext.form.field.Text', {
                id: 'REMARK',
                name: 'REMARK',
                fieldLabel: '备注'
            });
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
                        { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [con_CODECHIN, con_HSCIQ] },
                        { layout: 'column', height: 42, border: 0, items: [field_NAME, field_CHINESEADDRESS] },
                        { layout: 'column', height: 42, border: 0, items: [field_ENGLISHNAME, field_ENGLISHADDRESS] },
                        { layout: 'column', height: 42, border: 0, items: [con_COMBOBOX, rgb_TOOLVERSION] },
                        { layout: 'column', height: 42, border: 0, items: [con_CHK] },
                        { layout: 'column', height: 42, border: 0, items: [field_REMARK] },
                        field_ID
                ],
                buttons: [{

                    text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {

                        if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                            return;
                        }

                        var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                        Ext.Ajax.request({
                            url: 'CustomerManage.aspx',
                            type: 'Post',
                            params: { action: 'save', formdata: formdata },
                            success: function (response, option) {
                                var data = Ext.decode(response.responseText);
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
                        });
                    }
                }]
            });
        }

        function addCustomer_Win(ID, formdata) {
            form_ini_win();
            if (ID != "") {
                Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
                //rgb需要单独赋值
                Ext.getCmp('rgb_TOOLVERSION').setValue({ TOOLVERSION: formdata.TOOLVERSION });
            }

            var win = Ext.create("Ext.window.Window", {
                id: "win_d",
                title: '客商信息维护',
                width: 1000,
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

        function del() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择要删除的记录！');
                return;
            }
            Ext.Ajax.request({
                url: 'CustomerManage.aspx',
                params: { action: 'delete', ID: recs[0].get("ID") },
                type: 'Post',
                success: function (response, option) {
                    var data = Ext.decode(response.responseText);
                    var msg = "";
                    if (data.success) { msg = "删除成功"; }
                    else { msg = "删除失败"; }
                    Ext.MessageBox.alert('提示', msg, function () {
                        Ext.getCmp("pgbar").moveFirst();
                    });
                }
            });
        }

        function reset() {
            Ext.each(Ext.getCmp('formpanel_search').getForm().getFields().items, function (field) {
                field.reset();
            });
        }

        function exportdata() {
            var path = 'CustomerManage.aspx?action=export';
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
