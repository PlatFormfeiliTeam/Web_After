<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserList.aspx.cs" Inherits="Web_After.SysManager.UserList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />  
    <script type="text/javascript" >
        var common_data_ksdh = [];

        Ext.onReady(function () {
            Ext.Ajax.request({
                url: 'UserList.aspx',
                params: { action: 'Ini_Base_Data' },
                type: 'Post',
                success: function (response, option) {
                    var commondata = Ext.decode(response.responseText)
                    common_data_ksdh = commondata.ksdh;

                    init_search();
                    gridbind();

                    var panel = Ext.create('Ext.form.Panel', {
                        title: '主账号管理',
                        region: 'center',
                        layout: 'border',
                        items: [Ext.getCmp('formpanel_search'), Ext.getCmp('gridpanel')]
                    });
                    var viewport = Ext.create('Ext.container.Viewport', {
                        layout: 'border',
                        items: [panel]
                    });
                }
            });
        });

        function init_search() {
            var txtNAME = Ext.create('Ext.form.field.Text', { id: 'NAME_S', name: 'NAME_S', fieldLabel: '账号' });
            var txtREALNAME = Ext.create('Ext.form.field.Text', { id: 'REALNAME_S', name: 'REALNAME_S', fieldLabel: '名称' });
            var store_POSITIONID_S = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "无" }, { "CODE": 1, "NAME": "前台管理" }, { "CODE": 2, "NAME": "后台管理" }]
            });
            var combo_POSITIONID_S = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_POSITIONID_S',
                name: 'POSITIONID_S',
                store: store_POSITIONID_S,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '管理权限',
                displayField: 'NAME',
                valueField: 'CODE'
            });            

            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { addUser_Win(""); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改', width: 80, handler: function () { editUser(); } }
                    , { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { del(); } }
                    , { text: '<span class="icon iconfont" style="font-size:12px;">&#xe628;</span>&nbsp;启 用', width: 80, handler: function () { enabled(1); } }
                    , { text: '<span class="icon iconfont" style="font-size:12px;">&#xe634;</span>&nbsp;禁 用', handler: function () { enabled(0); } }
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
                { layout: 'column', border: 0, items: [txtNAME, txtREALNAME, combo_POSITIONID_S] }
                ]
            });
        }

        function gridbind() {
            var store_user = Ext.create('Ext.data.JsonStore', {
                fields: ['ID', 'NAME', 'REALNAME', 'EMAIL', 'TELEPHONE', 'MOBILEPHONE', 'POSITIONID',
                    'CUSTOMERID', 'ENABLED', 'CREATETIME', 'TYPE'],
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: 'UserList.aspx?action=loaduser',
                    reader: {
                        root: 'rows',
                        type: 'json',
                        totalProperty: 'total'
                    }
                },
                autoLoad: true,
                listeners: {
                    beforeload: function (store, options) {
                        store_user.getProxy().extraParams = Ext.getCmp('formpanel_search').getForm().getValues();
                    }
                }
            })
            var pgbar = Ext.create('Ext.toolbar.Paging', {
                id: 'pgbar',
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: store_user,
                displayInfo: true
            })
            var gridpanel = Ext.create('Ext.grid.Panel', {
                id: 'gridpanel',
                height: 560,
                region: 'center',
                store: store_user,
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                columns: [
                    { xtype: 'rownumberer', width: 35 },
                    { header: 'ID', dataIndex: 'ID', hidden: true },
                    { header: '登录账户', dataIndex: 'NAME', width: 100 },
                    { header: '名称', dataIndex: 'REALNAME', width: 250 },
                    { header: '邮箱', dataIndex: 'EMAIL', width: 160 },
                    { header: '电话', dataIndex: 'TELEPHONE', width: 110 },
                    { header: '管理权限', dataIndex: 'POSITIONID', renderer: gridrender, width: 100 },
                    { header: '是否启用', dataIndex: 'ENABLED', renderer: gridrender, width: 100 },
                    { header: '创建时间', dataIndex: 'CREATETIME', width: 180 },
                    { header: '初始化密码', dataIndex: 'ID', width: 100, renderer: gridrender }
                ],
                plugins: [{
                    ptype: 'rowexpander',
                    rowBodyTpl: ['<div id="div_{ID}"></div>']
                }],
                viewConfig: {
                    enableTextSelection: true
                }
            });

            gridpanel.view.on('expandBody', function (rowNode, record, expandRow, eOpts) {
                displayInnerGrid(record.get('ID'));
            });
            gridpanel.view.on('collapsebody', function (rowNode, record, expandRow, eOpts) {
                destroyInnerGrid(record.get("ID"));
            });
            
        }

        function displayInnerGrid(div) {
            var store_inner = Ext.create('Ext.data.JsonStore', {
                fields: ['ID', 'NAME', 'REALNAME', 'EMAIL', 'TELEPHONE', 'MOBILEPHONE', 'POSITIONID', 'SEX', 'ENABLED', 'CREATETIME'],
                proxy: {
                    url: 'UserList.aspx?action=loadchildaccount&id=' + div,
                    type: 'ajax',
                    reader: {
                        type: 'json',
                        root: 'innerrows'
                    }
                },
                autoLoad: true
            })
            var grid_inner = Ext.create('Ext.grid.Panel', {
                store: store_inner,
                margin: '0 0 0 70',
                selModel: { selType: 'checkboxmodel' },
                columns: [
                    { xtype: 'rownumberer', width: 25 },
                    { header: 'ID', dataIndex: 'ID', hidden: true },
                    { header: '子账号名', dataIndex: 'NAME', width: 120 },
                    { header: '姓名', dataIndex: 'REALNAME', width: 120 },
                    { header: '邮箱', dataIndex: 'EMAIL', width: 180 },
                    { header: '电话', dataIndex: 'TELEPHONE', width: 120 },
                    { header: '状态', dataIndex: 'ENABLED', width: 80, renderer: gridrender },
                    { header: '创建时间', dataIndex: 'CREATETIME', width: 180 },
                    { header: '操作', dataIndex: 'ID', width: 130, renderer: gridrender }
                ],
                renderTo: 'div_' + div
            })

            grid_inner.getEl().swallowEvent([
                'mousedown', 'mouseup', 'click',
                'contextmenu', 'mouseover', 'mouseout',
                'dblclick', 'mousemove'
            ]);

        }

        function destroyInnerGrid(div) {
            var parent = document.getElementById('div_' + div);
            var child = parent.firstChild;
            while (child) {
                child.parentNode.removeChild(child);
                child = child.nextSibling;
            }
        }

        function gridrender(value, cellmeta, record, rowIndex, columnIndex, stroe) {
            var dataindex = cellmeta.column.dataIndex;
            var str = "";
            switch (dataindex) {
                case "ENABLED":
                    str = value == "1" ? '启用' : '停用';
                    break;
                case "ID":
                    str = "<span class=\"icon iconfont\" onclick='inipsd(\"" + record.get("ID") + "\",\"" + record.get("NAME") + "\")'>&#xe6d4;</span>";
                    break;
                case "POSITIONID":
                    if (value == 1) { str = "前台管理"; }                        
                    else if (value == 2) { str = "后台管理"; }
                    else { str = "无"; }                        
                    break;
            }
            return str;
        }

        function inipsd(id, name) {
            Ext.MessageBox.confirm('提示', '初始化密码，确定要执行该操作吗？', function (btn) {
                if (btn == 'yes') {
                    Ext.Ajax.request({
                        url: 'UserList.aspx?action=inipsd&id=' + id + "&name=" + name,
                        success: function (response) {
                            if (response.responseText) {
                                Ext.MessageBox.alert('提示', '初始化密码成功！');
                            }
                        }
                    });
                }
            })

        }

        function enabled(flag) {
            var recs = gridpanel.getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择记录！');
                return;
            }
            Ext.Ajax.request({
                url: 'UserList.aspx',
                params: { action: 'enabled', FLAG: flag, ID: recs[0].get("ID") },
                type: 'Post',
                success: function (response, option) {
                    var data = Ext.decode(response.responseText);
                    if (data.success) {
                        store_user.load();
                    }
                }
            });
        }

        function del() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择要删除的记录！');
                return;
            }
            Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
                if (btn == 'yes') {
                    Ext.Ajax.request({
                        url: 'UserList.aspx',
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
            });
        }

        function form_ini_win() {           

            var field_ID = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });

            var field_NAME = Ext.create('Ext.form.field.Text', {
                id: 'NAME',
                name: 'NAME',
                fieldLabel: '账号',
                allowBlank: false,
                blankText: '账号不可为空!'
            });

            var field_REALNAME = Ext.create('Ext.form.field.Text', {
                id: 'REALNAME',
                name: 'REALNAME',
                fieldLabel: '名称',
                allowBlank: false,
                blankText: '名称不可为空!'
            });
            var field_MOBILEPHONE = Ext.create('Ext.form.field.Text', {
                id: 'MOBILEPHONE',
                name: 'MOBILEPHONE',
                fieldLabel: '手机'
            });

            var field_TELEPHONE = Ext.create('Ext.form.field.Text', {
                id: 'TELEPHONE',
                name: 'TELEPHONE',
                fieldLabel: '电话'
            });

            var field_EMAIL = Ext.create('Ext.form.field.Text', {
                id: 'EMAIL',
                name: 'EMAIL',
                fieldLabel: '邮箱'
            });

            var store_CUSTOMERID = Ext.create('Ext.data.Store', {
                fields: ["CODE", "NAME"],
                data: common_data_ksdh
            });

            var combo_CUSTOMERID = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_CUSTOMERID',
                name: 'CUSTOMERID',
                store: store_CUSTOMERID,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '客商', 
                displayField: 'NAME',
                valueField: 'CODE',
                allowBlank: false,
                blankText: '客商不能为空!'
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
                fieldLabel: '是否启用',
                displayField: 'NAME',
                valueField: 'CODE',
                value: 1,
                allowBlank: false,
                blankText: '是否启用不能为空!'
            });

            var store_POSITIONID = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "无" }, { "CODE": 1, "NAME": "前台管理" }, { "CODE": 2, "NAME": "后台管理" }]
            });
            var combo_POSITIONID = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_POSITIONID',
                name: 'POSITIONID',
                store: store_POSITIONID,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '权限',
                displayField: 'NAME',
                valueField: 'CODE',
                value:0,
                allowBlank: false,
                blankText: '权限不能为空!'
            });

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
                        { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_NAME, field_REALNAME ] },
                        { layout: 'column', height: 42, border: 0, items: [field_MOBILEPHONE, field_TELEPHONE] },
                        { layout: 'column', height: 42, border: 0, items: [field_EMAIL, combo_CUSTOMERID] },
                        { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, combo_POSITIONID] },
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
                            url: 'UserList.aspx',
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

        function addUser_Win(ID, formdata) {
            form_ini_win();
            if (ID != "") {
                Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
            }

            var win = Ext.create("Ext.window.Window", {
                id: "win_d",
                title: '主账号维护',
                width: 800,
                height: 300,
                modal: true,
                items: [Ext.getCmp('formpanel_Win')]
            });
            win.show();
        }

        function editUser(){
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
                return;
            }
            addUser_Win(recs[0].get("ID"), recs[0].data);
        }
       

    </script>
</head>
<body>
   
</body>
</html>
