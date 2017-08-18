<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GWYMenuManage.aspx.cs" Inherits="Web_After.GwyManager.GWYMenuManage" %>

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
        var treeModelstore; var lpanel; var treepanel;

        Ext.onReady(function () {
            tree();
            formpanel();

            var t_bar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { add(); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改', handler: function () { edit(); } }
                    , { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', handler: function () { del(); } }
                ]
            });

            var panel = Ext.create('Ext.form.Panel', {
                title: '菜单管理',
                tbar: t_bar,
                layout: 'border',
                region: 'center',
                items: [treepanel, lpanel]
            });
            var viewport = Ext.create('Ext.container.Viewport', {
                layout: 'border',
                items: [panel]
            });
        });

        function tree() {
            var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据加载中，请稍等..." });
            Ext.regModel("SysModelAuth", { fields: ["id", "name", "leaf", "ParentID", "frmname", "assemblyname", "remark", "args"] });
            treeModelstore = new Ext.data.TreeStore({
                model: 'SysModelAuth',
                nodeParam: 'id',
                proxy: {
                    type: 'ajax',
                    url: 'GWYMenuManage.aspx?action=loadMenu',
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
            treepanel = new Ext.tree.TreePanel(
                {
                    id: 'treepanel',
                    useArrows: true,
                    animate: true,
                    rootVisible: false,
                    region: 'west',
                    width: '40%',
                    store: treeModelstore,
                    columns: [
                    { text: 'id', dataIndex: 'id', width: 500, hidden: true },
                    { text: 'leaf', dataIndex: 'leaf', width: 100, hidden: true },
                    { text: 'ParentID', dataIndex: 'ParentID', width: 100, hidden: true },
                    { text: 'frmname', dataIndex: 'frmname', width: 100, hidden: true },
                    { text: 'assemblyname', dataIndex: 'assemblyname', width: 100, hidden: true },
                    { text: 'remark', dataIndex: 'remark', width: 100, hidden: true },
                    { text: 'args', dataIndex: 'args', width: 100, hidden: true },
                    { text: 'name', dataIndex: 'name', header: '模块名称', xtype: 'treecolumn', width: 400 }
                    ],
                    listeners: {
                        itemclick: function (node, e) {
                            var recs = Ext.getCmp('treepanel').getSelectionModel().getSelection();
                            if (recs.length > 0) {
                                lpanel.getForm().setValues(recs[0].data);
                            }
                        }
                    }
                }
            );
        }

        function formpanel() {
            lpanel = Ext.create('Ext.form.Panel', {
                name: 'lpanel',
                region: 'center',
                border: 0,
                defaults: { xtype: 'textfield', labelAlign: 'right', labelWidth: 80, margin: '10,0,10,0' },
                items: [
                    { name: 'name', anchor: '95%', fieldLabel: '菜 单' },
                    { name: 'assemblyname', anchor: '95%', fieldLabel: '所属DLL' },
                    { name: 'frmname', anchor: '95%', fieldLabel: 'FORM窗口' },
                    { name: 'args', anchor: '95%', fieldLabel: '窗口参数' },
                    { name: 'remark', anchor: '95%', fieldLabel: '备 注' }
                ]
            });
        }        

        function cat_edit_win(item, action) {
            var formpanel_cat = Ext.create('Ext.form.Panel', {
                layout: 'anchor',
                region: 'center',
                defaults: { labelWidth: 60, labelAlign: 'right', xtype: 'textfield', msgTarget: 'under', margin: '8,0,0,0,0' },
                items: [
                { name: 'name', anchor: '95%', fieldLabel: '菜 单' },
                { name: 'assemblyname', anchor: '95%', fieldLabel: '所属DLL' },
                { name: 'frmname', anchor: '95%', fieldLabel: 'FORM窗口' },
                { name: 'args', anchor: '95%', fieldLabel: '窗口参数' },
                { name: 'remark', anchor: '95%', fieldLabel: '备 注' },
                ],
                buttons: [{
                    text: '保 存', handler: function () {
                        var baseForm = formpanel_cat.getForm();
                        var data = Ext.encode(formpanel_cat.getForm().getValues());
                        alert(data);
                        if (baseForm.isValid()) {
                            Ext.Ajax.request({
                                method: 'POST',
                                url: "GWYMenuManage.aspx",
                                params: { action: action, id: item.id, json: data },
                                success: function (response, option) {
                                    var data = Ext.decode(response.responseText);
                                    if (data.success) {
                                        treeModelstore.load();//重新加载数据
                                        var record = treepanel.getRootNode();//默认选中根节点
                                        treepanel.getSelectionModel().select(record);
                                        Ext.each(lpanel.getForm().getFields().items, function (field) {
                                            field.reset();
                                        });//置空form
                                        win_cat.close();//关闭维护窗口
                                        Ext.MessageBox.alert("保存成功");
                                    }
                                    else {
                                        Ext.MessageBox.alert("保存失败");
                                    }
                                }
                            });
                        }
                    }
                }],
                buttonAlign: 'center'
            })

            var win_cat = Ext.create("Ext.window.Window", {
                title: '菜单维护',
                width: 350,
                height: 250,
                modal: true,
                items: [formpanel_cat],
                layout: 'border',
                buttonAlign: 'center'
            });
            win_cat.show();
            if (action == "update" && item != undefined) {//如果是修改 
                formpanel_cat.getForm().setValues(item);
            }

        }

        function add() {
            var recs = Ext.getCmp('treepanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择要新增的父节点！');
                return;
            }
            cat_edit_win(recs[0].data, "add");
        }

        function edit() {
            var recs = Ext.getCmp('treepanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择要修改的节点！');
                return;
            }
            cat_edit_win(recs[0].data, "update");
        }

        function del() {
            var recs = Ext.getCmp('treepanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择要删除的节点！');
                return;
            }
            Ext.MessageBox.confirm("请确认", "确定删除该节点及其子节点吗？", function (button, text) {
                if (button == 'yes') {
                    Ext.Ajax.request({
                        url: 'GWYMenuManage.aspx',
                        params: { action: 'delete', ID: recs[0].data.id },//id区分大小写
                        type: 'Post',
                        success: function (response, option) {
                            var data = Ext.decode(response.responseText);
                            if (data.success) {
                                //重新加载数据
                                treeModelstore.load();
                                //默认选中根节点
                                var record = treepanel.getRootNode();
                                treepanel.getSelectionModel().select(record);
                                //置空form
                                Ext.each(lpanel.getForm().getFields().items, function (field) {
                                    field.reset();
                                });
                                Ext.MessageBox.alert('提示', '删除成功');
                            }
                            else
                                Ext.MessageBox.alert('提示', '删除失败');
                        }
                    })
                }
            });
        }

</script>

</head>
<body>
    
</body>
</html>
