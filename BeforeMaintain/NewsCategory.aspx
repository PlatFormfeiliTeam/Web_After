<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewsCategory.aspx.cs" Inherits="Web_After.BeforeMaintain.NewsCategory" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />   

    <script type="text/javascript">
        var data_category, category_Id, category_Pid;
        var json_data;
        var treepanelstore;
        Ext.define('category', {
            extend: 'Ext.data.Model',
            fields: ["ID", "NAME", "leaf", "PID", "REMARK", "SORTINDEX"]
        });
        Ext.onReady(function () {
            treepanel();

            var panel = Ext.create('Ext.form.Panel', {
                title: '类别管理',
                region: 'center',
                layout: 'border',
                items: [Ext.getCmp('treepanel')]
            });
            var viewport = Ext.create('Ext.container.Viewport', {
                layout: 'border',
                items: [panel]
            })
        });

        function treepanel() {
            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<i class="iconfont">&#xe622;</i>&nbsp;添加', handler: function () { add_cat(); } }
                    , { text: '<i class="icon iconfont">&#xe632;</i>&nbsp;修改', handler: function () { modify_cat(); } }
                    , { text: '<i class="icon iconfont">&#xe6d3;</i>&nbsp;删除', handler: function () { delete_cat(); } }
                ]
            });

            data_category = getTreeStore();
            treepanelstore = Ext.create('Ext.data.TreeStore', {
                model: 'category',
                proxy: {
                    type: 'memory',
                    data: data_category,
                    reader: {
                        type: 'json'
                    }
                },
                root: {
                    expanded: true,
                    text: "my root"
                }

            });
            var treepanel = Ext.create('Ext.tree.Panel', {
                id: 'treepanel',
                tbar: toolbar,
                useArrows: true,
                animate: true,
                region: 'center',
                rootVisible: true,
                store: treepanelstore,
                columns: [
                { dataIndex: 'ID', text: 'ID', width: 120, hidden: true },
                { dataIndex: 'leaf', width: 100, hidden: true },
                { header: '类别名称', xtype: 'treecolumn', dataIndex: 'NAME', text: 'NAME', width: 300 },
                { header: '备注', dataIndex: 'REMARK', width: 300 },
                { header: '显示顺序', dataIndex: 'SORTINDEX', width: 300 },
                { dataIndex: 'PID', text: 'PID', hidden: true }]
                ,
                listeners: {
                    beforeitemexpand: function (curnode, options) {
                        data_category = getTreeStore(curnode.data.ID);
                        var proxy = treepanel.store.getProxy();
                        proxy.data = data_category;

                    },
                    itemclick: function (thisTree, record, item, index, e, eOpts) {
                        category_Id = record.data.ID;
                        category_Pid = record.data.PID;
                        json_data = { "ID": record.data.ID, "NAME": record.data.NAME, "PID": record.data.PID, "leaf": record.data.leaf, "REMARK": record.data.REMARK, "SORTINDEX": record.data.SORTINDEX };

                    }
                }
            });
        }

        function getTreeStore(id) {
            if (typeof id == 'undefined') {
                id = "";
            }

            var data_temp;
            Ext.Ajax.request({
                method: 'POST',
                headers: { 'Content-Type': 'application/json;utf-8' },
                url: 'NewsCategory.aspx/getCate',
                params: "{ 'id': '" + id + "'}",
                async: false,
                success: function (reps, option) {
                    var json = Ext.decode(reps.responseText);
                    data_temp = Ext.decode(json.d);
                }

            });
            return data_temp;
        }

        function add_cat() {
            cat_edit_win(category_Id, "create");
        }

        function modify_cat() {

            cat_edit_win(category_Id, "update");


        }

        function delete_cat() {
            Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
                if (btn == "yes") {
                    Ext.Ajax.request({
                        method: 'POST',
                        url: "NewsCategory.aspx/CateManager",
                        headers: { 'Content-Type': 'application/json;utf-8' },
                        params: "{ 'action': 'delete','json': '{PID:" + category_Pid + ",ID:" + category_Id + "}'}",
                        callback: function (option, success, response) {
                            var result_tmp = Ext.decode(response.responseText);
                            var result = Ext.decode(result_tmp.d);
                            if (result.haveNews) {
                                Ext.Msg.alert("提示", "此类别下有新闻，请先移除再删除!");
                                return;
                            }
                            if (result.haveChild) {
                                Ext.Msg.alert("提示", "此类别下有子类别，请先移除再删除!");
                                return;
                            }
                            if (result.success) {
                                Ext.Msg.alert("提示", "操作成功!", function () {
                                    data_category = getTreeStore();
                                    var proxy = treepanelstore.getProxy();
                                    proxy.data = data_category;
                                    treepanelstore.load();
                                });
                            }
                        }
                    });
                }

            });

        }

        function cat_edit_win(id, action) {
            var formpanel_cat = Ext.create('Ext.form.Panel', {
                layout: 'anchor',
                region: 'center',
                defaults: { labelAlign: 'right', xtype: 'textfield', msgTarget: 'under', margin: '15 0 15 0' },
                items: [
                    { name: 'NAME', anchor: '95%', fieldLabel: '类别名称', allowBlank: false, blankText: '类别名称不能为空', emptyText: '请输入类别名称' },
                    { name: 'REMARK', anchor: '95%', fieldLabel: '备注', allowBlank: true, emptyText: '请输入备注信息' },
                    { name: 'SORTINDEX', anchor: '95%', fieldLabel: '显示顺序', allowBlank: true, emptyText: '请输显示顺序', xtype: 'numberfield' },
                    { name: 'ID', xtype: 'hidden' },
                    { name: 'PID', xtype: 'hidden', id: 'field_parentid' }
                ],
                buttons: [{
                    text: '保 存', handler: function () {
                        var baseForm = formpanel_cat.getForm();
                        if (baseForm.isValid()) {
                            Ext.Ajax.request({
                                method: 'POST',
                                url: "NewsCategory.aspx/CateManager",
                                headers: { 'Content-Type': 'application/json;utf-8' },
                                params: "{ 'action': '" + action + "','json': '" + Ext.encode(baseForm.getValues()) + "'}",
                                callback: function (option, success, response) {
                                    var result_tmp = Ext.decode(response.responseText);
                                    var result = Ext.decode(result_tmp.d);

                                    if (result.success) {
                                        Ext.Msg.alert("提示", "操作成功!", function () {
                                            data_category = getTreeStore();
                                            var proxy = treepanelstore.getProxy();
                                            proxy.data = data_category;
                                            treepanelstore.load();
                                            win_cat.close();
                                        });
                                    }
                                }
                            });
                        }
                    }
                }],
                buttonAlign: 'center'
            })
            var win_cat = Ext.create("Ext.window.Window", {
                title: '新闻类别维护',
                width: 500,
                height: 250,
                modal: true,
                items: [formpanel_cat],
                layout: 'border',
                buttonAlign: 'center'
            });
            win_cat.show();
            if (action == "update" && json_data != undefined) {//如果是修改 
                formpanel_cat.getForm().setValues(json_data);
            }
            if (action == "create" && id) {
                Ext.getCmp("field_parentid").setValue(id);
            }
        }
    </script>

</head>
<body>
    <div id="renderto"></div>
</body>
</html>
