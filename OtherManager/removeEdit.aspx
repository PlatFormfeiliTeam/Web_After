<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="removeEdit.aspx.cs" Inherits="Web_After.OtherManager.removeEdit" %>

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
         Ext.onReady(function () {
             var removeItem = Ext.create('Ext.form.RadioGroup', {
                 name: "removeItem", id: "removeItem", fieldLabel: '解除项目选择', labelAlign: 'right', anchor: '100%', margin: '10', columns: 1,
                 items: [
                     { boxLabel: '解除客服编辑', name: 'item', inputValue: 'kf', checked: true },
                     { boxLabel: '解除制单编辑', name: 'item', inputValue: 'zd' },
                     { boxLabel: '解除审单编辑', name: 'item', inputValue: 'sd' }
                 ]
             });


             var store_bhtype = Ext.create('Ext.data.JsonStore', {
                 fields: ['CODE', 'NAME'],
                 data: [{ "CODE": "ddbh", "NAME": "订单编号" }, { "CODE": "qybh", "NAME": "企业编号" }]
             })
             var combo_bhxz = Ext.create('Ext.form.field.ComboBox', {
                 id: 'combo_bhxz',
                 name: 'combo_bhxz', margin: '0 5 10 10',
                 store: store_bhtype,
                 displayField: 'NAME',
                 valueField: 'CODE',
                 editable: false,
                 queryMode: 'local',
                 width: 90,
                 value: 'ddbh'
             });
             var field_bhsr = Ext.create('Ext.form.field.Text', {
                 id: 'field_bhsr',
                 name: 'itembh'
             });

             var container_bh = Ext.create('Ext.form.FieldContainer', {
                 id: 'container_bh',
                 layout: 'hbox',
                 items: [combo_bhxz, field_bhsr]
             });

             var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                 items: [
                            {
                                text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存',
                                handler: function () {
                                    var formdata = Ext.encode(formpanel.getForm().getValues());
                                    formpanel.getForm().submit({
                                        url: 'removeEdit.aspx?action=remove',
                                        params: formdata,
                                        waitMsg: '解除中...',
                                        success: function (form, action) {
                                            Ext.Msg.alert('提示', '解除成功！', function () {
                                                //formpanel.getForm().reset()
                                            });
                                        },
                                        failure: function (form, action) {//失败要做的事情 
                                            Ext.MessageBox.alert("提示", "解除失败！");
                                        }
                                    });
                                }
                            }
                 ]
             })
             var formpanel = Ext.create('Ext.form.Panel', {
                 id: 'formpanel',
                 title: '解除编辑',
                 region: 'center',
                 tbar:toolbar,
                 fieldDefaults: {
                     margin: '0 5 10 0',
                     labelWidth: 80,
                     columnWidth: 1,
                     labelAlign: 'right',
                     labelSeparator: '',
                     msgTarget: 'under'
                 },
                 items: [
                     { layout: 'column', border: 0, items: [removeItem] },
                     { layout: 'column', border: 0, items: [container_bh] }
                     ]
             });

             var viewport = Ext.create('Ext.container.Viewport', {
                 layout: 'border',
                 items: [formpanel]
             })
         });
     </script>
</head>
<body>
    
</body>
</html>
