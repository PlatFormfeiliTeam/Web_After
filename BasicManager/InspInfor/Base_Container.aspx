<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Base_Container.aspx.cs" Inherits="Web_After.BasicManager.InspInfor.Base_Container" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />    
    <script src="/js/import/importExcel.js" type="text/javascript"></script>
    <script src="Util_Base_Container.js"></script>
    <script type="text/javascript">
        var username = '<%=Username()%>';

        //集装箱规格：base_containerstandard
        var param = util.getUrlParam('param');
        console.log(param);
        //获取title:集装箱规格
        var title = util.getPanelTitle(param);
        //获取searchCode,searchName: //获取grindpanel中前两行
        var getSearchCode = util.getInitSearchSearchCode(param);
        var getSearchName = util.getInitSearchSearchName(param);
        var getHSCode = util.getGridNameCol3(param);
        var getHSName = util.getGridNameCol4(param);
        var getInspection = util.getGridNameCol5(param);
        var getDeclaration = util.getGridNameCol6(param);

        //获取gridpanel中的colums
        var arr = util.getGrindPanelColumsOne(param);

        Ext.onReady(function () {
            init_search(param);
            gridbind(param);

            var panel = Ext.create('Ext.form.Panel', {
                title: title,
                region: 'center',
                layout: 'border',
                items: [Ext.getCmp('formpanel_search'), Ext.getCmp('gridpanel')]
            });
            var viewport = Ext.create('Ext.container.Viewport',
                {
                    layout: 'border',
                    items: [panel]
                });
        });

        function init_search(param) {

            var searchCode = Ext.create('Ext.form.field.Text', { id: 'CODE_S', name: 'CODE_S', fieldLabel: getSearchCode });
            var searchName = Ext.create('Ext.form.field.Text', { id: 'CNNAME_S', name: 'CNNAME_S', fieldLabel: getSearchName });



            var store_ENABLED_S = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
            });
            var combo_ENABLED_S = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_ENABLED_S',
                name: 'ENABLED_S',
                store: store_ENABLED_S,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '是否启用',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { addCustomer_Win("", "", param); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改', width: 80, handler: function () { editCustomer(param); } }
                    //, { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { del(); } }
                    , { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入', width: 80, handler: function () { importfile('add', param); } }
                    , { text: '<span class="icon iconfont">&#xe625;</span>&nbsp;导 出', handler: function () { exportdata(param); } }
                    , '->'
                    , { text: '<span class="icon iconfont">&#xe60b;</span>&nbsp;查 询', width: 80, handler: function () { Ext.getCmp("pgbar").moveFirst(); } }
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
                    labelWidth: 95
                },
                items: [
                    { layout: 'column', border: 0, items: [searchCode, searchName, combo_ENABLED_S] }

                ]
            });
        }

        //数据绑定
        function gridbind(param) {
            var store_customer = Ext.create('Ext.data.JsonStore',
                {
                    fields: [arr[0], arr[1], 'ENABLED', 'STARTDATE', 'CREATEMANNAME',
                        'CREATEDATE', 'STOPMANNAME', 'ENDDATE', 'REMARK', 'ID'],
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'Base_Container.aspx?action=loadData&table=' + param,
                        reader: {
                            root: 'rows',
                            type: 'json',
                            totalProperty: 'total'
                        }
                    },
                    autoLoad: true,
                    listeners: {
                        beforeload: function (store, options) {
                            store_customer.getProxy().extraParams =
                                Ext.getCmp('formpanel_search').getForm().getValues();
                        }
                    }
                });
            var pgbar = Ext.create('Ext.toolbar.Paging',
                {
                    id: 'pgbar',
                    displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                    store: store_customer,
                    displayInfo: true
                });
            var gridpanel = Ext.create('Ext.grid.Panel', {
                id: 'gridpanel',
                height: 560,
                region: 'center',
                store: store_customer,
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                columns: [
                    { xtype: 'rownumberer', width: 35 },


                    { header: getSearchCode, dataIndex: arr[0], width: 150 },
                    { header: getSearchName, dataIndex: arr[1], width: 150 },
                    { header: getHSCode, dataIndex: arr[2], width: 150 },
                    { header: getHSName, dataIndex: arr[3], width: 150 },
                    { header: getInspection, dataIndex: arr[4], width: 150 },
                    { header: getDeclaration, dataIndex: arr[5], width: 150 },

                    { header: '启用情况', dataIndex: 'ENABLED', renderer: gridrender, width: 100 },
                    { header: '启用时间', dataIndex: 'STARTDATE', width: 100 },

                    { header: '维护人', dataIndex: 'CREATEMANNAME', width: 100 },
                    { header: '维护时间', dataIndex: 'CREATEDATE', width: 100 },
                    { header: '停用人', dataIndex: 'STOPMANNAME', width: 100 },
                    { header: '停用时间', dataIndex: 'ENDDATE', width: 100 },
                    { header: '备注', dataIndex: 'REMARK', width: 200 },
                    { header: 'ID', dataIndex: 'ID', width: 200, hidden: true }
                ],
                listeners:
                {
                    'itemdblclick': function (view, record, item, index, e) {
                        //editCustomer(param);
                    }
                },
                viewConfig: {
                    enableTextSelection: true
                }
            });
        }

        function gridrender(value, cellmeta, record, rowIndex, columnIndex, stroe) {
            var dataindex = cellmeta.column.dataIndex;
            var str = "";
            switch (dataindex) {
                case "ENABLED":
                    str = value == "1" ? '<span class="icon iconfont" style="font-size:12px;color:blue;">&#xe628;</span>' : '<span class="icon iconfont" style="font-size:12px;color:red;">&#xe634;</span>';
                    break;
            }
            return str;
        }


    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
