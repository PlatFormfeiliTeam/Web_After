<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModuleList.aspx.cs" Inherits="Web_After.SysManager.FrontManager.ModuleList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <script src="/js/pan.js" type="text/javascript"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />   

     <script type="text/javascript">
         var nodeid = getQueryString("nodeid");
         Ext.onReady(function () {
             treepanel();

             var panel = Ext.create('Ext.panel.Panel', {
                 title: '<font size=2>模块管理</font>',
                 region: 'center',
                 layout: 'border',
                 items: [Ext.getCmp("treepanel")]
             });
             var viewport = Ext.create('Ext.container.Viewport', {
                 layout: 'border',
                 items: [panel]
             });

         });

         function treepanel() {
             var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                 items: [
                    { text: '<i class="iconfont">&#xe622;</i>&nbsp;添加', handler: function () { add_module(); } }
                    , { text: '<i class="icon iconfont">&#xe632;</i>&nbsp;修改', handler: function () { modify_module(); } }
                    , { text: '<i class="icon iconfont">&#xe6d3;</i>&nbsp;删除', handler: function () { delete_module(); } }
                 ]
             })

             Ext.regModel("SysModule", { fields: ["MODULEID", "NAME", "leaf", "URL", "PARENTID", "SORTINDEX", "ICON"] });
             var treepanelstore = new Ext.data.TreeStore({
                 model: 'SysModule',
                 proxy: {
                     type: 'ajax',
                     url: 'ModuleList.aspx?action=select',
                     reader: 'json',
                     extraParams: {
                         MODULEID: ''
                     }
                 },
                 root: {
                     expanded: true,
                     text: '系统模块'
                 }
             });
             var treepanel = Ext.create('Ext.tree.Panel', {
                 id: 'treepanel', region: 'center',
                 useArrows: true,
                 animate: true,
                 tbar: toolbar,
                 selModel: { selType: 'checkboxmodel' },
                 rootVisible: false,
                 store: treepanelstore,
                 columns: [
                 { dataIndex: 'MODULEID', width: 120, hidden: true },
                 { dataIndex: 'leaf', width: 100, hidden: true },
                 { header: '模块名称', xtype: 'treecolumn', dataIndex: 'NAME', width: 300 },
                 { header: '链接地址', dataIndex: 'URL', width: 500 },
                 { header: '显示顺序', dataIndex: 'SORTINDEX', width: 100 },
                 { header: '图标', dataIndex: 'ICON', width: 100 },
                 { dataIndex: 'PARENTID', hidden: true }
                 ],
                 listeners: {
                     beforeitemexpand: function (curnode, options) {
                         var proxy = treepanel.store.getProxy();
                         proxy.extraParams.MODULEID = curnode.data.MODULEID;
                     }
                 }
             });
         }

         function add_module() {
             var recs = Ext.getCmp('treepanel').getSelectionModel().getSelection();
             //可以选择父节点，也可以不选择父节点，如果不选择的话默认父节点就是根节点 
             var parentNode;
             if (recs.length == 0) {
                 parentNode = Ext.getCmp('treepanel').store.getRootNode();
             }
             else {
                 parentNode = recs[0];
             }
             module_edit_win(parentNode, "create");
         }
         function modify_module() {
             var recs = Ext.getCmp('treepanel').getSelectionModel().getSelection();
             if (recs.length == 0) {
                 Ext.Msg.alert("提示", "请选择要修改的节点!");
                 return;
             }
             module_edit_win(recs[0], "update");
         }
         function delete_module() {
             var recs = Ext.getCmp('treepanel').getSelectionModel().getSelection();
             if (recs.length == 0) {
                 Ext.Msg.alert("提示", "请选择要删除的节点!");
                 return;
             }
             if (!recs[0].data.leaf) {//删除某个节点后有可能父节点不存在
                 Ext.Msg.alert("提示", "包含子节点的对象不允许删除!");
                 return;
             }
             Ext.MessageBox.confirm('提示', '确定要删除该模块吗？', function (btn) {
                 if (btn == 'yes') {
                     Ext.Ajax.request({
                         url: 'ModuleList.aspx?action=delete',
                         params: { json: Ext.encode(recs[0].data) },
                         callback: function (option, success, response) {
                             var result = Ext.decode(response.responseText);
                             if (result.success) {
                                 Ext.Msg.alert("提示", "删除成功!", function () {
                                     var pnode = recs[0].parentNode;
                                     pnode.removeChild(recs[0]);
                                     if (!pnode.hasChildNodes()) {//删除某个节点后有可能父节点不存在
                                         pnode.set("leaf", true);
                                     }
                                 });
                             }
                         }
                     })
                 }
             })
         }
    </script>

</head>
<body>
    
</body>
</html>
