<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GWYButtonManage.aspx.cs" Inherits="Web_After.GwyManager.GWYButtonManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />   

    <script>
        var treeModelstore, treeModel;
        var roleid = '';
        Ext.onReady(function () {
            grid_tree_panel();
           
            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [{ text: '<i class="icon iconfont" style="font-size:12px;">&#xe7d2;</i>&nbsp;分 配', handler: function () { SaveRoleAuthor(); } }]
            });

            var panel = Ext.create('Ext.panel.Panel', {
                title: "<font size=2>客商按钮配置</font>",
                region: "center",
                tbar: toolbar,
                layout: 'border',
                items: [Ext.getCmp("gridpanel_role"), treeModel]
            });
            var viewport = Ext.create('Ext.container.Viewport', {
                layout: 'border',
                items: [panel]
            });
        });

        function grid_tree_panel() {
            var store_role = Ext.create('Ext.data.JsonStore', {
                fields: ['ID', 'CODE', 'NAME'],
                proxy: {
                    type: 'ajax',
                    reader: {
                        root: 'rows',
                        type: 'json',
                    },
                    url: 'GWYButtonManage.aspx?action=loadManager',
                },
                autoLoad: true
            });

            var gridpanel_role = Ext.create('Ext.grid.Panel', {
                id: 'gridpanel_role',
                border: 1, width: '35%',region:'west',
                store: store_role,
                enableColumnHide: false,
                columns: [
                { xtype: 'rownumberer', width: 35 },
                { header: '客商代码', dataIndex: 'CODE', width: 120 },
                { header: '客商名称', dataIndex: 'NAME', width: 120 }
                ],
                listeners: {
                    itemclick: function (value, record, item, index, e, eOpts) {
                        //treeModelstore.setProxy({
                        //    type: 'ajax',
                        //    url: 'GWYButtonManage.aspx?action=loadmenu',
                        //    reader: 'json'
                        //});
                        roleid = record.data.ID;
                        treeModelstore.proxy.extraParams.roleid = roleid;
                        treeModelstore.load();
                    }
                },
                viewConfig: {
                    enableTextSelection: true
                },
                forceFit: true
            });

            var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据加载中，请稍等..." });

            //系统模块
            Ext.regModel("SysModelAuth", { fields: ["name", "leaf", "ParentID", "code"] });
            treeModelstore = new Ext.data.TreeStore({
                model: 'SysModelAuth',
                nodeParam: 'id',
                proxy: {
                    type: 'ajax',
                    url: 'GWYButtonManage.aspx?action=loadmenu',
                    reader: 'json'
                },
                root: {
                    expanded: true,
                    name: '按钮配置',
                    id: '-1'
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
                columns: [
                { text: 'leaf', dataIndex: 'leaf', width: 100, hidden: true },
                { header: '模块名称', xtype: 'treecolumn', text: 'name', dataIndex: 'name', width: 400 },
                { text: 'code', dataIndex: 'code', width: 100, hidden: true },
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

        function SaveRoleAuthor() {
            if (roleid) {
                var moduleids = "";
                var recs = treeModel.getChecked();
                for (var i = 0; i < recs.length; i++) {
                    moduleids += recs[i].data.ParentID + "|" + recs[i].data.code + ",";
                }
                moduleids = moduleids = "" ? moduleids : moduleids.substr(0, moduleids.length - 1);

                var mask = new Ext.LoadMask(Ext.getBody(), { msg: "保存当前角色数据中，请稍等..." });
                mask.show();
                Ext.Ajax.request({
                    timeout: 1000000000,
                    url: 'GWYButtonManage.aspx?action=saveButtonConfig',
                    params: { moduleids: moduleids, roleid: roleid },
                    success: function (option, success, response) {
                        var data = Ext.decode(option.responseText);
                        if (data.success) {
                            treeModelstore.load();
                            Ext.MessageBox.alert('提示', '保存成功！');
                        } else {
                            Ext.MessageBox.alert('提示', '保存失败！');
                        }
                        mask.hide();
                    }
                });
            }
            else {
                Ext.MessageBox.alert('提示', '请先选择需要授权的角色！');
            }
        }
    </script>

</head>
<body>
    
</body>
</html>
