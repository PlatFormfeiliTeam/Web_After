<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebAuthList_After.aspx.cs" Inherits="Web_After.SysManager.AfterManager.WebAuthList_After" %>

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
         var gridUser, store_user, treeModel, treeModelstore, gridStation, store_Station;
         var userid = '';
         Ext.onReady(function () {
             grid_tree_panel();

             var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                 items: [{ text: '<i class="icon iconfont" style="font-size:12px;">&#xe7d2;</i>&nbsp;分 配', handler: function () { SaveAuthorization(); } }]
             });
             
             var panel = Ext.create('Ext.panel.Panel', {
                 title: '<font size=2>后台权限管理</font>', tbar: toolbar,
                 layout: 'border',
                 region: 'center',
                 items: [gridUser, treeModel]
             });
             var viewport = Ext.create('Ext.container.Viewport', {
                 layout: 'border',
                 items: [panel]
             });
         });

         function grid_tree_panel() {
             Ext.regModel('User', { fields: ['ID', 'NAME', 'REALNAME'] })
             store_user = Ext.create('Ext.data.JsonStore', {
                 model: 'User',
                 proxy: {
                     type: 'ajax',
                     url: 'WebAuthList_After.aspx?action=loaduser',
                     reader: {
                         root: 'rows',
                         type: 'json'
                     }
                 },
                 autoLoad: true
             })

             gridUser = Ext.create('Ext.grid.Panel', {
                 width: '35%',
                 region: 'west',
                 store: store_user,
                 columns: [
                     { xtype: 'rownumberer', width: 35 },
                     { header: 'ID', dataIndex: 'ID', hidden: true },
                     { header: '登录名', dataIndex: 'NAME', width: 180 },
                     { header: '姓名', dataIndex: 'REALNAME', width: 250 }
                 ],
                 listeners: {
                     itemclick: function (value, record, item, index, e, eOpts) {
                         treeModelstore.setProxy({
                             type: 'ajax',
                             url: 'WebAuthList_After.aspx?action=loadauthority',
                             reader: 'json'
                         });
                         userid = record.get("ID");
                         var proxys = treeModelstore.proxy;
                         proxys.extraParams.userid = userid;
                         treeModelstore.load();
                     }
                 }
             })

             var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据加载中，请稍等..." });

             //系统模块
             Ext.regModel("SysModelAuth", { fields: ["id", "name", "leaf", "url", "ParentID"] });
             treeModelstore = new Ext.data.TreeStore({
                 model: 'SysModelAuth',
                 nodeParam: 'id',
                 proxy: {
                     type: 'ajax',
                     url: 'WebAuthList_After.aspx?action=loadauthority',
                     reader: 'json'
                 },
                 root: {
                     expanded: true,
                     name: '后端模块',
                     id: '91a0657f-1939-4528-80aa-91b202a593ac'
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
                 rootVisible: false,
                 store: treeModelstore,
                 region: 'center',
                 columns: [
                 { text: 'id', dataIndex: 'id', width: 100, hidden: true },
                 { text: 'leaf', dataIndex: 'leaf', width: 100, hidden: true },
                 { header: '模块名称', xtype: 'treecolumn', text: 'name', dataIndex: 'name', width: 500 },
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

         function SaveAuthorization() {
             if (userid) {
                 var moduleids = "";
                 var recs = treeModel.getChecked();
                 for (var i = 0; i < recs.length; i++) {
                     moduleids += recs[i].data.id + ',';
                 }
                 var mask = new Ext.LoadMask(Ext.getBody(), { msg: "保存当前账户数据并同步更新子账号数据中，请稍等..." });
                 mask.show();
                 Ext.Ajax.request({
                     timeout: 1000000000,
                     url: 'WebAuthList_After.aspx?action=AuthorizationSave',
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
             }
             else {
                 Ext.MessageBox.alert('提示', '请先选择需要授权的账号！');
             }
         }

    </script>

</head>
<body>
    
</body>
</html>
