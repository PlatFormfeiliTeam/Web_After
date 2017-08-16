<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AttachList.aspx.cs" Inherits="Web_After.OtherManager.AttachList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <script src="/js/pan.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />   
    <script type="text/javascript" >
        Ext.onReady(function () {
            init_search();
            gridbind();

            var panel = Ext.create('Ext.form.Panel', {
                title: '文件列表',
                region: 'center',
                layout: 'border',
                items: [Ext.getCmp('formpanel_search'), Ext.getCmp('gridpanel')]
            });
            var viewport = Ext.create('Ext.container.Viewport', {
                layout: 'border',
                items: [panel]
            })
        });

        function init_search() {
            var txtORDERCODE = Ext.create('Ext.form.field.Text', { id: 'ORDERCODE', name: 'ORDERCODE', fieldLabel: '订单编号' });

            var store_splitstatus = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": "0", "NAME": "未拆分" }, { "CODE": "1", "NAME": "已拆分" }]
            });
            var combo_splitstatus = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_splitstatus', name: 'SPLITSTATUS',
                store: store_splitstatus,
                fieldLabel: '拆分状态',
                displayField: 'NAME',
                queryMode: 'local',
                valueField: 'CODE', labelAlign: 'right'
            });
            var store_filetype = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": "44", "NAME": "订单文件" }, { "CODE": "61", "NAME": "报关单" }]
            });
            var combo_filetype = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_filetype', name: 'FILETYPE',
                store: store_filetype,
                fieldLabel: '文件类型',
                displayField: 'NAME',
                queryMode: 'local',
                valueField: 'CODE', labelAlign: 'right'
            });
            var start_date = Ext.create('Ext.form.field.Date', {
                id: 'start_date', name: 'START_DATE',
                format: 'Y-m-d',
                fieldLabel: '开始时间', labelAlign: 'right'
            })
            var end_date = Ext.create('Ext.form.field.Date', {
                id: 'end_date', name: 'END_DATE',
                format: 'Y-m-d',
                fieldLabel: '结束时间', labelAlign: 'right'
            });

            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<span class="icon iconfont">&#xe62f;</span>&nbsp;文件拆分', handler: function () { Views(); } }
                    , '->'
                    , { text: '<span class="icon iconfont">&#xe60b;</span>&nbsp;查 询', width: 80, handler: function () { Ext.getCmp("pgbar").moveFirst(); } }
                    , { text: '<span class="icon iconfont">&#xe625;</span>&nbsp;导 出', handler: function () { Export(); } }
                ]
            });

            var formpanel_search = Ext.create('Ext.form.Panel', {
                id: 'formpanel_search',
                region: 'north',
                border: 0,
                bbar:toolbar,
                fieldDefaults: {
                    margin: '5',
                    columnWidth: 0.20,
                    labelWidth: 70
                },
                items: [
                    { layout: 'column', border: 0, items: [txtORDERCODE, combo_splitstatus, combo_filetype, start_date, end_date] }
                ]
            });
        }

        function gridbind() {
            var store_attach = Ext.create('Ext.data.JsonStore', {
                fields: ['ID', 'FILENAME', 'ORIGINALNAME', 'UPLOADTIME', 'UPLOADUSERID', 'FILETYPE', 'CUSTOMERCODE'
                    , 'SIZES', 'ORDERCODE', 'FILESUFFIX', 'FILETYPENAME', 'SPLITSTATUS', 'IETYPE', 'PGINDEX'
                    , 'ORDERCOUNT', 'UPLOADUSERNAME', 'FILEPAGES'],
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: 'AttachList.aspx?action=loadattach',
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
                            ORDERCODE: Ext.getCmp("ORDERCODE").getValue(),
                            SPLITSTATUS: Ext.getCmp("combo_splitstatus").getValue(),
                            FILETYPE: Ext.getCmp("combo_filetype").getValue(),
                            START_DATE: Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s'),
                            END_DATE: Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s')
                        }
                        Ext.apply(store.proxy.extraParams, new_params);
                    }
                }
            })
            var pgbar = Ext.create('Ext.toolbar.Paging', {
                id: 'pgbar',
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: store_attach,
                displayInfo: true
            })
            var gridpanel = Ext.create('Ext.grid.Panel', {
                id: 'gridpanel',
                height: 560,
                region: 'center',
                store: store_attach,
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                columns: [
                    { xtype: 'rownumberer', width: 35 },
                    { header: 'ID', dataIndex: 'ID', width: 80, locked: true },
                    { header: '文件路径', dataIndex: 'FILENAME', width: 300, locked: true, tdCls: 'tdValign' },
                    { header: '原文件名', dataIndex: 'ORIGINALNAME', width: 250, locked: true },
                    { header: '订单编号', dataIndex: 'ORDERCODE', width: 120, locked: true },
                    { header: '是否拆分', dataIndex: 'SPLITSTATUS', width: 60, renderer: render, locked: true },
                    { header: '文件类型', dataIndex: 'FILETYPE', width: 60 },
                    { header: '文件类型名称', dataIndex: 'FILETYPENAME', width: 120 },
                    { header: '页数', dataIndex: 'FILEPAGES', width: 60 },
                    { header: '上传时间', dataIndex: 'UPLOADTIME', width: 150 },
                    { header: '文件大小', dataIndex: 'SIZES', width: 80 },
                    { header: '文件扩展名', dataIndex: 'FILESUFFIX', width: 130 },
                    { header: '上传人', dataIndex: 'UPLOADUSERID', width: 60 },
                    { header: '上传人姓名', dataIndex: 'UPLOADUSERNAME', width: 130 }
                ],
                listeners:
                {
                    'itemdblclick': function (view, record, item, index, e) {
                        opencenterwin("/PdfView.aspx?ordercode=" + record.data.ORDERCODE + "&userid=15", 1600, 900);
                    }
                },
                viewConfig: {
                    enableTextSelection: true
                }
            });
        }

        function Views() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
                return;
            }
            opencenterwin("/PdfView.aspx?ordercode=" + recs[0].get("ORDERCODE") + "&userid=15", 1600, 900);
        }
        function Export() {
            var ORDERCODE = Ext.getCmp("ORDERCODE").getValue();
            var FILETYPE = Ext.getCmp("combo_filetype").getValue(); FILETYPE = FILETYPE == null ? "" : FILETYPE;
            var START_DATE = Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s');
            var END_DATE = Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s');

            var path = 'AttachList.aspx?action=export&ORDERCODE=' + ORDERCODE + '&FILETYPE=' + FILETYPE + '&START_DATE=' + START_DATE + '&END_DATE=' + END_DATE;
            $('#exportform').attr("action", path).submit();
        }
    </script>
</head>
<body>
    <div>
        <form id="exportform" name="form" enctype="multipart/form-data" method="post"> <%--style="display:inline-block"--%>
                   
        </form>
    </div>
</body>
</html>
