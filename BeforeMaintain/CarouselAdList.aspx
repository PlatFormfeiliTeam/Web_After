<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CarouselAdList.aspx.cs" Inherits="Web_After.BeforeMaintain.CarouselAdList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />   
     <style type="text/css">
        .tdValign {
            vertical-align: middle;
        }
    </style>
    <script type="text/javascript" >
         Ext.onReady(function () {
             gridbind();

             var panel = Ext.create('Ext.form.Panel', {
                 title: '轮播广告',
                 region: 'center',
                 layout: 'border',
                 items: [Ext.getCmp('gridpanel')]
             });
             var viewport = Ext.create('Ext.container.Viewport', {
                 layout: 'border',
                 items: [panel]
             })
         });

         function gridbind() {
             var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                 items: [
                     { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { add_win(""); } }
                     , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改', handler: function () { edit(); } }
                     , { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', handler: function () { del(); } }
                     , '<font color="blue">上传图片尺寸必须为1920*460!</font>'
                 ]
             });
             var store_CarouselAd = Ext.create('Ext.data.JsonStore', {
                 fields: ["ID", "IMGURL", "LINKURL", "DESCRIPTION", "STATUS", "FILENAME", "SORTINDEX"],
                 pageSize: 20,
                 proxy: {
                     type: 'ajax',
                     url: "CarouselAdList.aspx?action=select",
                     reader: {
                         root: 'rows',
                         type: 'json',
                         totalProperty: 'total'
                     }
                 },
                 autoLoad: true
             })
             var pgbar = Ext.create('Ext.toolbar.Paging', {
                 id: 'pgbar',
                 displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                 store: store_CarouselAd,
                 displayInfo: true
             })
             var gridpanel = Ext.create('Ext.grid.Panel', {
                 id: 'gridpanel',
                 height: 560,
                 tbar:toolbar,
                 region: 'center',
                 store: store_CarouselAd,
                 columnLines: true,
                 selModel: { selType: 'checkboxmodel' },
                 bbar: pgbar,
                 columns: [
                    { xtype: 'rownumberer', width: 35, tdCls: 'tdValign' },
                    { header: 'ID', dataIndex: 'ID', width: 200, sortable: true, hidden: true },
                    { header: '图片名称', dataIndex: 'FILENAME', width: 600, sortable: true, tdCls: 'tdValign' },
                    { header: '描述', dataIndex: 'DESCRIPTION', flex: 1, sortable: true, tdCls: 'tdValign' },
                    {
                        header: '缩略图', dataIndex: 'IMGURL', width: 100, renderer: function (value) {
                            return "<img style='width:80px;height:40px' src='" + value + "'/>";
                        }
                    },
                    {
                        header: '状态', dataIndex: 'STATUS', width: 100, sortable: true, renderer: function (value) {
                            if (value == 'true') {
                                return '启用';
                            } else {
                                return '停用';
                            };
                        }
                    },
                    { header: '排列顺序', dataIndex: 'SORTINDEX', width: 100, sortable: true }
                 ],
                 viewConfig: {
                     enableTextSelection: true
                 }
             });
         }

         function form_ini_win() {
             var field_ID = Ext.create('Ext.form.field.Hidden', {
                 id: 'ID',
                 name: 'ID'
             });

             var imgUrl = Ext.create('Ext.form.field.File', {
                 id: 'IMGURL', name: 'IMGURL', fieldLabel: '图片地址', labelAlign: 'right', msgTarget: 'under', margin: '10', anchor: '90%', buttonText: '上传图片', regex: /.*(.jpg|.png|.gif)$/, regexText: '只能上传JPG'
             });

             var description = Ext.create('Ext.form.field.TextArea', {
                 name: 'DESCRIPTION', fieldLabel: '描述', labelAlign: 'right', anchor: '90%', margin: '10', height: 60
             });

             var status = Ext.create('Ext.form.RadioGroup', {
                 name: "STATUS", id: "STATUS", fieldLabel: '状态', labelAlign: 'right', anchor: '40%', margin: '10',
                 items: [
                     { boxLabel: '启用', name: 'STATUS', inputValue: 'true', checked: true },
                     { boxLabel: '停用', name: 'STATUS', inputValue: 'false' }
                 ]
             });

             var SORTINDEX = Ext.create('Ext.form.Number', {
                 name: 'SORTINDEX', fieldLabel: '排列顺序', allowBlank: false, labelAlign: 'right', anchor: '90%', margin: '10', msgTarget: 'under', blankText: '排列顺序不能为空!'
             });

             var formpanel_Win = Ext.create('Ext.form.Panel', {
                 id: 'formpanel_Win',
                 minHeight: 170,
                 border: 0,
                 buttonAlign: 'center',
                 items: [imgUrl, SORTINDEX, status, description
                     , {
                         xtype: 'label',
                         html: '<font color="blue">提示：上传图片尺寸必须为1920*460!</font>',
                         margin: '0 0 0 80'
                     }
                    , field_ID
                 ],
                 buttons: [{

                     text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {

                         if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                             return;
                         }

                         var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());                        
                         formpanel_Win.getForm().submit({
                             url: 'CarouselAdList.aspx',
                             params: { action: 'save', formdata: formdata },
                             waitMsg: '保存中...', //提示等待信息  
                             success: function (form, action) {
                                 Ext.Msg.alert('提示', '保存成功', function () {
                                     Ext.getCmp("pgbar").moveFirst(); Ext.getCmp("win_d").close();
                                 });
                             },
                             failure: function (form, action) {//失败要做的事情 
                                 Ext.MessageBox.alert("提示", "图片尺寸必须为1920*460!", function () {
                                     Ext.getCmp("pgbar").moveFirst(); Ext.getCmp("win_d").close();
                                 });
                             }
                         });

                     }
                 }]
             });
         }

         function add_win(ID, formdata) {
             form_ini_win();
             if (ID != "") {
                 Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
                 //rgb需要单独赋值
                 Ext.getCmp("STATUS").setValue({ STATUS: formdata.STATUS });
                 Ext.getCmp("IMGURL").disable(true);
             }

             var win = Ext.create("Ext.window.Window", {
                 id: "win_d",
                 title: '轮播广告维护',
                 width: 800,
                 height: 300,
                 modal: true,
                 items: [Ext.getCmp('formpanel_Win')]
             });
             win.show();
         }

         function edit() {
             var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
             if (recs.length == 0) {
                 Ext.MessageBox.alert('提示', '请选择需要修改的记录！');
                 return;
             }
             add_win(recs[0].get("ID"), recs[0].data);
         }

         function del() {
             var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
             if (recs.length == 0) {
                 Ext.MessageBox.alert('提示', '请选择需要删除的记录！');
                 return;
             }
             Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
                 if (btn == 'yes') {
                     Ext.Ajax.request({
                         url: 'CarouselAdList.aspx?action=delete',
                         params: { id: recs[0].get("ID") },
                         callback: function () {
                             Ext.Msg.alert('提示', "删除成功", function () {
                                 Ext.getCmp("pgbar").moveFirst();
                             });
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
