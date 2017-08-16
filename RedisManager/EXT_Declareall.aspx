<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EXT_Declareall.aspx.cs" Inherits="Web_After.RedisManager.EXT_Declareall" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/pan.js" type="text/javascript"></script>
        <script type="text/javascript" >
            Ext.onReady(function () {
                
     

                //总
                var store_attach = Ext.create('Ext.data.JsonStore', {
                    fields: ['ID', 'DECLARATIONCODE', 'TRADECODE', 'TRANSNAME', 'GOODSNUM', 'GOODSGW', 'SHEETNUM', 'COMMODITYNUM', 'CUSTOMSSTATUS',
                             'MODIFYFLAG', 'PREDECLCODE', 'CUSNO', 'OLDDECLARATIONCODE', 'ISDEL', 'DIVIDEREDISKEY', 'SUPPLYCHAINCODE', 'DATES'],
                    pageSize: 10,
                    proxy: {
                        type: 'ajax',
                        url: 'EXT_Declareall.aspx?action=loadattach',
                        reader: {
                            root: 'rows',
                            type: 'json',
                            totalProperty: 'total'
                        }
                    },
                    autoLoad: true,
                    listeners: {
                        beforeload: function (store, options) {
                            var new_params = {
                                CUSNO: Ext.getCmp("CUSNO").getValue(), DECLARATIONCODE: Ext.getCmp("DECLARATIONCODE").getValue(), FENKEY: Ext.getCmp("FENKEY").getValue()
                            }
                            Ext.apply(store.proxy.extraParams, new_params);
                        }
                    }
                });
                var pgbar = Ext.create('Ext.toolbar.Paging', {
                    displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                    store: store_attach,
                    displayInfo: true
                });

                var gridpanel = Ext.create('Ext.grid.Panel', {
                    title: '报关单--总KEY',
                    region: 'center',
                    //height: 500,
                    store: store_attach,
                    selModel: { selType: 'checkboxmodel' },
                    bbar: pgbar,
                    columns: [
                         { xtype: 'rownumberer', width: 35 },
                        { header: 'ID', dataIndex: 'ID', width: 100, locked: true },
                        { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 150, locked: true },
                        { header: '贸易方式', dataIndex: 'TRADECODE', width: 100, locked: true },
                        { header: '运输工具', dataIndex: 'TRANSNAME', width: 150, locked: true },
                        { header: '件数', dataIndex: 'GOODSNUM', width: 80, locked: true },
                        { header: '毛重', dataIndex: 'GOODSGW', width: 80 },
                        { header: '报关单张数', dataIndex: 'SHEETNUM', width: 80 },
                        { header: '商品项数', dataIndex: 'COMMODITYNUM', width: 80 },
                        { header: '海关状态', dataIndex: 'CUSTOMSSTATUS', width: 80 },
                        { header: '删改单标志', dataIndex: 'MODIFYFLAG', width: 80 },
                        { header: '预制单编号', dataIndex: 'PREDECLCODE', width: 150 },
                        { header: '企业编号', dataIndex: 'CUSNO', width: 150 },
                        { header: '旧报关单号', dataIndex: 'OLDDECLARATIONCODE', width: 150 },
                        { header: '是否删除', dataIndex: 'ISDEL', width: 80 },
                        { header: '供应链代码', dataIndex: 'SUPPLYCHAINCODE', width: 100 },
                        { header: '分KEY', dataIndex: 'DIVIDEREDISKEY', width: 200 },
                        { header: '时间', dataIndex: 'DATES', width: 180 }
                    ],
                    viewConfig: {
                        enableTextSelection: true
                    }
                });
                //分
                var store_attach_fenkey = Ext.create('Ext.data.JsonStore', {
                    fields: ['ID', 'DECLARATIONCODE', 'TRADECODE', 'TRANSNAME', 'GOODSNUM', 'GOODSGW', 'SHEETNUM', 'COMMODITYNUM', 'CUSTOMSSTATUS',
                             'MODIFYFLAG', 'PREDECLCODE', 'CUSNO', 'OLDDECLARATIONCODE', 'ISDEL', 'SUPPLYCHAINCODE', 'CREATETIME'],
                    pageSize: 10,
                    proxy: {
                        type: 'ajax',
                        url: 'EXT_Declareall.aspx?action=loadattach1',
                        reader: {
                            root: 'rows',
                            type: 'json',
                            totalProperty: 'total'
                        }
                    },
                    autoLoad: true,
                    listeners: {
                        beforeload: function (store, options) {
                            var new_params = {
                                CUSNO: Ext.getCmp("CUSNO").getValue(), DECLARATIONCODE: Ext.getCmp("DECLARATIONCODE").getValue(), FENKEY: Ext.getCmp("FENKEY").getValue()
                            }
                            Ext.apply(store.proxy.extraParams, new_params);
                        }
                    }
                })

                var pgbar_fenkey = Ext.create('Ext.toolbar.Paging', {
                    displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                    store: store_attach_fenkey,
                    displayInfo: true
                })

                var gridpanel_fenkey = Ext.create('Ext.grid.Panel', {
                    title: '报关单--分KEY',
                    region: 'south',
                    store: store_attach_fenkey,
                    selModel: { selType: 'checkboxmodel' },
                    bbar: pgbar_fenkey,
                    columns: [
                        { xtype: 'rownumberer', width: 35 },
                        { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 150, locked: true },
                        { header: '贸易方式', dataIndex: 'TRADECODE', width: 100, locked: true },
                        { header: '运输工具', dataIndex: 'TRANSNAME', width: 150, locked: true },
                        { header: '件数', dataIndex: 'GOODSNUM', width: 80, locked: true },
                        { header: '毛重', dataIndex: 'GOODSGW', width: 80 },
                        { header: '报关单张数', dataIndex: 'SHEETNUM', width: 80 },
                        { header: '商品项数', dataIndex: 'COMMODITYNUM', width: 80 },
                        { header: '海关状态', dataIndex: 'CUSTOMSSTATUS', width: 80 },
                        { header: '删改单标志', dataIndex: 'MODIFYFLAG', width: 80 },
                        { header: '预制单编号', dataIndex: 'PREDECLCODE', width: 150 },
                        { header: '企业编号', dataIndex: 'CUSNO', width: 150 },
                        { header: '旧报关单号', dataIndex: 'OLDDECLARATIONCODE', width: 150 },
                        { header: '是否删除', dataIndex: 'ISDEL', width: 80 },
                        { header: '供应链代码', dataIndex: 'SUPPLYCHAINCODE', width: 100 },
                        { header: '创建时间', dataIndex: 'CREATETIME', width: 120 }
                    ],
                    viewConfig: {
                        enableTextSelection: true
                    }
                })

                //
                var formpanel_search = Ext.create('Ext.form.Panel', {
                    id: 'formpanel_search',
                    region: 'north',
                    border: 0,
                    fieldDefaults: {
                        margin: '5',
                        columnWidth: 0.25
                    },
                    items: {
                        layout: 'column', border: 0, items: [
                        {
                            xtype: 'textfield', fieldLabel: '客户编号', labelWidth: 80, labelAlign: 'right', id: 'CUSNO', flex: .90
                        },
                        {
                            xtype: 'textfield', fieldLabel: '报关单号', labelWidth: 80, labelAlign: 'right', id: 'DECLARATIONCODE', flex: .90
                        },
                        {
                            xtype: 'textfield', fieldLabel: '分KEY', labelWidth: 80, labelAlign: 'right', id: 'FENKEY', flex: .90
                        },
                        {
                            xtype: 'button', text: '<i class="iconfont">&#xe60b;</i>&nbsp;查询', handler: function () {
                                pgbar.moveFirst();
                                pgbar_fenkey.moveFirst();

                            }
                        }
                        ]
                    }
                });


                var panel_c = Ext.create('Ext.form.Panel', {
                    region: 'center',
                    layout: 'border',
                    items: [gridpanel, gridpanel_fenkey],
                    listeners: {
                        resize: function (container, width, height, oldWidth, oldHeight, obj) {
                            gridpanel_fenkey.setHeight(height/2);
                        }
                    }
                });

                var panel= Ext.create('Ext.form.Panel', {
                    title: '报关单',
                    region: 'center',
                    layout: 'border',
                    items: [formpanel_search, panel_c]
                })

         
                var viewport = Ext.create('Ext.container.Viewport', {
                    layout: 'border',
                    items: [panel]
                
                })
            });
            </script>
</head>
<body>
</body>
</html>
