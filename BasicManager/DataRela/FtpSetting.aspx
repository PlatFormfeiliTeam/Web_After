<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FtpSetting.aspx.cs" Inherits="Web_After.BasicManager.DataRela.FtpSetting" %>

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
    <script type="text/javascript">
        var title = "通道FTP配置管理";

        Ext.onReady(function () {
            init_search();
            gridbind();
            var panel = Ext.create('Ext.form.Panel', {
                title: 'FTP设置',
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

        //查询栏
        function init_search() {
            var searchCode = Ext.create('Ext.form.field.Text', { id: 'PROFILENAMECODE', name: 'PROFILENAMECODE', fieldLabel: '配置方案' });

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
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { addCustomer_Win("", ""); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改', width: 80, handler: function () { editCustomer(); } }
                    //, { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { del(); } }
                    //, { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入', width: 80, handler: function () { importfile('add'); } }
                    , { text: '<span class="icon iconfont">&#xe625;</span>&nbsp;导 出', handler: function () { exportdata(); } }
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
                    labelWidth: 100
                },
                items: [
                    { layout: 'column', border: 0, items: [searchCode, combo_ENABLED_S] }

                ]
            });
        }

        //数据绑定
        function gridbind() {
            var store_customer = Ext.create('Ext.data.JsonStore',
                {
                    fields: ['PROFILENAME', 'URI', 'PORT', 'USERNAME', 'PASSWORD',
                         'ENABLED', 'CHANNELNAME', 'FILETYPE', 'CUSTOMDISTRICTCODE', 'ENTRUSTTYPE'],
                    pageSize: 20,
                    proxy: {
                        type: 'ajax',
                        url: 'FtpSetting.aspx?action=loadData',
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


                    { header: '配置方案名称', dataIndex: 'PROFILENAME', width: 150 },
                    { header: 'FTP服务器URI', dataIndex: 'URI', width: 150 },
                    { header: '端口号', dataIndex: 'PORT', width: 150 },
                    { header: 'FTP用户名', dataIndex: 'USERNAME', width: 150 },
                     { header: 'FTP密码', dataIndex: 'PASSWORD', width: 100 },
                    { header: '启用', dataIndex: 'ENABLED', renderer: gridrender, width: 100 },
                    { header: '通道名称', dataIndex: 'CHANNELNAME', width: 100 },
                    { header: '发送文件类型', dataIndex: 'FILETYPE', width: 100 },
                    { header: '适用关区', dataIndex: 'CUSTOMDISTRICTCODE', width: 150 },
                    { header: '申报类型', dataIndex: 'ENTRUSTTYPE', width: 100 },
                    { header: 'ID', dataIndex: 'ID', width: 200, hidden: true }
                ],
                listeners:
                {
                    'itemdblclick': function (view, record, item, index, e) {
                        //editCustomer();
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

        //重置查询条件
        function reset() {
            Ext.each(Ext.getCmp('formpanel_search').getForm().getFields().items,
                function (field) {
                    field.reset();
                });
        }

        //新增
        function addCustomer_Win(ID, formdata) {
            form_ini_win();

            if (ID != "") {

                Ext.getCmp('REASON').hidden = false;
                Ext.getCmp('REASON').allowBlank = false;
                Ext.getCmp('REASON').blankText = '修改原因不可为空!';

                //默认值的
                Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
            }

            var win = Ext.create("Ext.window.Window", {
                id: "win_d",
                title: 'FTP配置设定',
                width: 1200,
                height: 430,
                modal: true,
                items: [Ext.getCmp('formpanel_Win')]
            });
            win.show();
        }


        function form_ini_win() {
            var field_ID = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });

            var store_for_decl = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 01, "NAME": "报关" }, { "CODE": 02, "NAME": "报检" }]
            });



            var field_ProfileName = Ext.create('Ext.form.field.Text', {
                id: 'PROFILENAME',
                name: 'PROFILENAME',
                hideTrigger: true,
                minChars: 1,
                fieldLabel: '配置方案名称',
                flex: .5,
                allowBlank: false,
                blankText: '配置方案名称不可为空!',
            });

            var field_Uri = Ext.create('Ext.form.field.Text', {
                id: 'URI',
                name: 'URI',
                hideTrigger: true,
                fieldLabel: 'FTP服务器URI',
                flex: .5,
                allowBlank: false,
                blankText: 'FTP服务器URI不可为空!',
            });

            var field_Port = Ext.create('Ext.form.field.Text', {
                id: 'PORT',
                name: 'PORT',
                hideTrigger: true,
                fieldLabel: '端口号',
                flex: .5,
                allowBlank: false,
                blankText: '端口号不能为空!',
            });

            var field_UserName = Ext.create('Ext.form.field.Text', {
                id: 'USERNAME',
                name: 'USERNAME',
                hideTrigger: true,
                fieldLabel: 'FTP用户名',
                flex: .5,
                allowBlank: false,
                blankText: 'FTP用户名不能为空!',
            });

            var field_Password = Ext.create('Ext.form.field.Text', {
                id: 'PASSWORD',
                name: 'PASSWORD',
                hideTrigger: true,
                fieldLabel: 'FTP密码',
                flex: .5,
                allowBlank: false,
                blankText: 'FTP密码不能为空!',
            });

            var field_ChannelName = Ext.create('Ext.form.field.Text', {
                id: 'CHANNELNAME',
                name: 'CHANNELNAME',
                hideTrigger: true,
                fieldLabel: '通道名称',
                flex: .5,
                allowBlank: false,
                blankText: '通道名称不能为空!',
            });

            var field_FileType = Ext.create('Ext.form.field.Text', {
                id: 'FILETYPE',
                name: 'FILETYPE',
                hideTrigger: true,
                fieldLabel: '发送文件类型',
                flex: .5,
                allowBlank: false,
                blankText: '发送文件类型不能为空!',
            });

            var field_CustomDistrictCode = Ext.create('Ext.form.field.Text', {
                id: 'CUSTOMDISTRICTCODE',
                name: 'CUSTOMDISTRICTCODE',
                hideTrigger: true,
                fieldLabel: '适用关区',
                flex: .5,
                allowBlank: false,
                blankText: '适用关区不能为空!',
            });


            var field_EnTrustType = Ext.create('Ext.form.field.ComboBox', {
                id: 'ENTRUSTTYPE',
                name: 'ENTRUSTTYPE',
                store: store_for_decl,
                queryMode: 'local',
                displayField: 'NAME',
                valueField: 'CODE',
                anyMatch: true,
                fieldLabel: '申报类型',
                flex: .5,
                allowBlank: false,
                blankText: '申报类型不可为空!'
            });

            var store_ENABLED = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
            });
            var combo_ENABLED = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_ENABLED',
                name: 'ENABLED',
                store: store_ENABLED,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '是否启用',
                flex: .5,
                displayField: 'NAME',
                valueField: 'CODE',
                value: 1,
                allowBlank: false,
                blankText: '是否启用不能为空!'
            });


            //修改原因输入框
            var change_reason = Ext.create('Ext.form.field.Text', {
                id: 'REASON',
                name: 'REASON',
                fieldLabel: '修改原因',
                hidden: true


            });

            var formpanel_Win = Ext.create('Ext.form.Panel', {
                id: 'formpanel_Win',
                minHeight: 170,
                border: 0,
                buttonAlign: 'center',
                fieldDefaults: {
                    margin: '0 5 10 0',
                    labelWidth: 100,
                    columnWidth: .5,
                    labelAlign: 'right',
                    labelSeparator: '',
                    msgTarget: 'under'
                },
                items: [
                    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_ProfileName, field_Uri] },
                    { layout: 'column', height: 42, border: 0, items: [field_Port, combo_ENABLED] },
                    { layout: 'column', height: 42, border: 0, items: [field_UserName, field_Password] },
                    { layout: 'column', height: 42, border: 0, items: [field_ChannelName, field_FileType] },
                    { layout: 'column', height: 42, border: 0, items: [field_CustomDistrictCode, field_EnTrustType] },
                    { layout: 'column', height: 42, border: 0, items: [change_reason] },
                    field_ID
                ],
                buttons: [{

                    text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {
                        console.log('bbbb');
                        if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {

                            return;
                        }

                        var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                        console.log('aaaaa');
                        Ext.Ajax.request({
                            url: 'RelaRegion.aspx',
                            type: 'Post',
                            params: { action: 'save', formdata: formdata },
                            success: function (response, option) {

                                var data = Ext.decode(response.responseText);
                                if (data.success == "5") {
                                    Ext.Msg.alert('提示',
                                        "保存成功",
                                        function () {
                                            Ext.getCmp("pgbar").moveFirst();
                                            Ext.getCmp("win_d").close();
                                        });
                                } else {
                                    var errorMsg = data.success;
                                    var reg = /,$/gi;
                                    idStr = errorMsg.replace(reg, "!");
                                    Ext.Msg.alert('提示', "保存失败:" + idStr, function () {
                                        Ext.getCmp("pgbar").moveFirst(); Ext.getCmp("win_d").close();
                                    });
                                }


                            }
                        });

                    }
                }]
            });
        }

        //编辑
        function editCustomer() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
                return;
            }
            addCustomer_Win(recs[0].get("ID"), recs[0].data);
        }

        function exportdata() {
            var PROFILENAMECODE = Ext.getCmp('PROFILENAMECODE').getValue();
            var combo_ENABLED_S = Ext.getCmp('combo_ENABLED_S').getValue();
            var path = 'FtpSetting.aspx?action=export&PROFILENAMECODE=' + PROFILENAMECODE +'&combo_ENABLED_S=' + combo_ENABLED_S;
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
