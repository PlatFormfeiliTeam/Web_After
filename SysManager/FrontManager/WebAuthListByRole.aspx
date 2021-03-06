﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebAuthListByRole.aspx.cs" Inherits="Web_After.SysManager.FrontManager.WebAuthListByRole" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css?t=<%=System.Configuration.ConfigurationManager.AppSettings["Version"].ToString() %>" rel="stylesheet" />   
    
    <script src="/js/pan.js?t=<%=System.Configuration.ConfigurationManager.AppSettings["Version"].ToString() %>" type="text/javascript"></script>
    <script>
        var store_user; var treeModelstore, treeModel;
        var userid = "";
        Ext.onReady(function () {
            init_search();
            grid_tree_panel();

            //var toolbar = Ext.create('Ext.toolbar.Toolbar', {
            //    items: ['->',{ text: '<i class="icon iconfont" style="font-size:12px;">&#xe7d2;</i>&nbsp;分 配', handler: function () { SaveAuthorByRole(); } }]
            //})

            var panel = Ext.create('Ext.panel.Panel', {
                title: '<font size=2>主账号BY角色</font>',
                //tbar: toolbar,
                layout: 'border',
                region: 'center',
                minHeight: 100,
                items: [Ext.getCmp('formpanel_search'), Ext.getCmp("gridUser"), treeModel]
            });

            var viewport = Ext.create('Ext.container.Viewport', {
                layout: 'border',
                items: [panel]
            });
        });

        function init_search() {
            var txtNAME = Ext.create('Ext.form.field.Text', { id: 'NAME_S', name: 'NAME_S', fieldLabel: '账号' });
            var txtREALNAME = Ext.create('Ext.form.field.Text', { id: 'REALNAME_S', name: 'REALNAME_S', fieldLabel: '姓名' });

            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<i class="icon iconfont">&#xe7d2;</i>&nbsp;分 配', handler: function () { SaveAuthorByRole(); } }                   
                    , '->'
                    , { text: '<span class="icon iconfont">&#xe60b;</span>&nbsp;查 询', width: 80, handler: function () { Ext.getCmp("gridUser").store.load(); } }
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
                { layout: 'column', border: 0, items: [txtNAME, txtREALNAME] }
                ]
            });
        }

        function grid_tree_panel() {
            Ext.regModel('User', { fields: ['ID', 'CUSTOMERNAME', 'REALNAME', 'NAME', 'ISCUSTOMER', 'ISSHIPPER', 'ISCOMPANY','ISRECEIVER'] })
            store_user = Ext.create('Ext.data.JsonStore', {
                model: 'User',
                proxy: {
                    type: 'ajax',
                    url: 'WebAuthListByRole.aspx?action=loaduser',
                    reader: {
                        root: 'rows',
                        type: 'json'
                    }
                },
                autoLoad: true,
                listeners: {
                    beforeload: function (store, options) {
                        store_user.getProxy().extraParams = Ext.getCmp('formpanel_search').getForm().getValues();
                    }
                }
            })

            var gridUser = Ext.create('Ext.grid.Panel', {
                id: 'gridUser',
                border: 1,
                region: 'west',
                width:'70%',
                store: store_user,
                columns: [
                    { xtype: 'rownumberer', width: 35 },
                    { header: 'ID', dataIndex: 'ID', hidden: true },
                    { header: '账号', dataIndex: 'NAME', width: 120 },
                    { header: '姓名', dataIndex: 'REALNAME', width: 200 },
                    { header: '所属客户', dataIndex: 'CUSTOMERNAME', width: 300 },
                    { header: '接单单位', dataIndex: 'ISRECEIVER', width: 70, renderer: render },
                    { header: '委托单位', dataIndex: 'ISCUSTOMER', width: 70, renderer: render },
                    { header: '供应商', dataIndex: 'ISSHIPPER', width: 65, renderer: render },
                    { header: '生产型企业', dataIndex: 'ISCOMPANY', width: 85, renderer: render }
                ],
                listeners: {
                    beforeload: function (store, options) {
                        store_user.getProxy().extraParams = Ext.getCmp('formpanel_search').getForm().getValues();
                    },
                    itemclick: function (value, record, item, index, e, eOpts) {
                        treeModelstore.setProxy({
                            type: 'ajax',
                            url: 'WebAuthListByRole.aspx?action=loadauthority',
                            reader: 'json'
                        });
                        userid = record.get("ID");
                        var proxys = treeModelstore.proxy;
                        proxys.extraParams.userid = userid;
                        proxys.extraParams.ISRECEIVER = record.get("ISRECEIVER"); proxys.extraParams.ISCUSTOMER = record.get("ISCUSTOMER");
                        proxys.extraParams.ISSHIPPER = record.get("ISSHIPPER"); proxys.extraParams.ISCOMPANY = record.get("ISCOMPANY");
                        treeModelstore.load();
                    }
                },
                viewConfig: {
                    enableTextSelection: true
                }
            });

            var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据加载中，请稍等..." });

            //系统模块
            Ext.regModel("SysModelAuth", { fields: ["id", "name", "leaf", "url", "ParentID"] });
            treeModelstore = new Ext.data.TreeStore({
                model: 'SysModelAuth',
                nodeParam: 'id',
                proxy: {
                    type: 'ajax',
                    url: 'WebAuthListByRole.aspx?action=loadauthority',
                    reader: 'json'
                },
                root: {
                    expanded: true,
                    name: '前端模块',
                    id: '91a0657f-1939-4528-80aa-91b202a593ab'
                },
                listeners: {
                    beforeload: function () {
                        myMask.show();
                    },
                    load: function (st, rds, opts) {
                        if (myMask) { myMask.hide(); }
                    }
                }
            });
            treeModel = Ext.create('Ext.tree.Panel', {
                useArrows: true,
                animate: true,
                rootVisible: false, region: 'center',
                store: treeModelstore,
                height: 600,
                columns: [
                { text: 'id', dataIndex: 'id', width: 500, hidden: true },
                { text: 'leaf', dataIndex: 'leaf', width: 100, hidden: true },
                { header: '模块名称', xtype: 'treecolumn', text: 'name', dataIndex: 'name', flex: 1 },
                { text: 'ParentID', dataIndex: 'ParentID', width: 100, hidden: true }
                ],
                listeners: {
                    'checkchange': function (node, checked) {
                        setChildChecked(node, checked);
                        setParentChecked(node, checked);
                    }
                }
            });
        }

        function SaveAuthorByRole() {
            if (userid) {
                var moduleids = "";
                var recs = treeModel.getChecked();
                for (var i = 0; i < recs.length; i++) {
                    moduleids += recs[i].data.id + ',';
                }
                var mask = new Ext.LoadMask(Ext.getBody(), { msg: "保存当前账户权限数据并同步更新子账号数据中，请稍等..." });
                mask.show();
                Ext.Ajax.request({
                    timeout: 1000000000,
                    url: 'WebAuthListByRole.aspx?action=SaveAuthorByRole',
                    params: { moduleids: moduleids, userid: userid },
                    success: function (option, success, response) {
                        if (option.responseText == '{success:true}') {
                            Ext.MessageBox.alert('提示', '保存成功！');
                        } else {
                            Ext.MessageBox.alert('提示', '保存失败！');
                        }
                        mask.hide();
                    }
                })
            } else {
                Ext.MessageBox.alert('提示', '请先选择需要授权的账号！');
            }
        }

        //选择子节点
        function setChildChecked(node, checked) {
            node.expand();
            node.set('checked', checked);
            if (node.hasChildNodes()) {
                node.eachChild(function (child) {
                    setChildChecked(child, checked);
                });
            }
        }

        //选择父节点
        function setParentChecked(node, checked) {
            node.set({ checked: checked });
            var parentNode = node.parentNode;
            if (parentNode != null) {
                var flag = false;
                parentNode.eachChild(function (childnode) {
                    if (childnode.get('checked')) {
                        flag = true;
                    }
                });
                if (checked == false) {
                    if (!flag) {
                        setParentChecked(parentNode, checked);
                    }
                } else {
                    if (flag) {
                        setParentChecked(parentNode, checked);
                    }
                }
            }
        }

        function reset() {
            Ext.each(Ext.getCmp('formpanel_search').getForm().getFields().items, function (field) {
                field.reset();
            });
        }

    </script>
</head>
<body>
</body>
</html>
