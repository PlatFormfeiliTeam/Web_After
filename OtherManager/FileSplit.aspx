<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileSplit.aspx.cs" Inherits="Web_After.OtherManager.FileSplit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />  
    <script src="/js/commondata.js"></script>
    <script type="text/javascript" >
        var common_data_jydw = [], common_data_wtdw = [], common_data_wjlx = [], common_data_myfs = [];
        var store_wjlx, store_busitype, store_tradeway;

        Ext.onReady(function () {
            Ext.Ajax.request({
                url: 'FileSplit.aspx',
                params: { action: 'Ini_Base_Data' },
                type: 'Post',
                success: function (response, option) {
                    var commondata = Ext.decode(response.responseText)
                    common_data_jydw = commondata.jydw;//经营单位、申报单位
                    common_data_wtdw = commondata.wtdw;//委托单位
                    common_data_wjlx = commondata.wjlx;//文件类型
                    common_data_myfs = commondata.myfs;//贸易方式

                    store_wjlx = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_wjlx });
                    store_busitype = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_busitype });
                    store_tradeway = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_myfs });

                    init_search();
                    gridbind();

                    var panel=Ext.create('Ext.form.Panel', {
                        title: '不可拆分配置',
                        region: 'center',
                        layout: 'border',
                        items: [Ext.getCmp('formpanel_search'), Ext.getCmp('gridpanel')]
                    });
                    var viewport = Ext.create('Ext.container.Viewport', {
                        layout: 'border',
                        items: [panel]
                    });
                }
            });

        });

        function init_search() {

            //经营单位
            var store_BUSIUNITCODE_S = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_jydw
            });
            var combo_BUSIUNITCODE_S = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_BUSIUNITCODE_S',
                name: 'BUSIUNITCODE_S',
                store: store_BUSIUNITCODE_S,
                hideTrigger: true,
                minChars: 4,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '经营单位',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            //委托单位
            var store_CUSTOMERCODE_S = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_wtdw
            });
            var combo_CUSTOMERCODE_S = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_CUSTOMERCODE_S',
                name: 'CUSTOMERCODE_S',
                store: store_CUSTOMERCODE_S,
                hideTrigger: true,
                minChars: 4,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '委托单位',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            //申报单位
            var store_REPUNITCODE_S = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_jydw
            });
            var combo_REPUNITCODE_S = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_REPUNITCODE_S',
                name: 'REPUNITCODE_S',
                store: store_REPUNITCODE_S,
                hideTrigger: true,
                minChars: 4,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '申报单位',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            //业务类型
            var store_BUSITYPE_S = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_busitype
            });
            var combo_BUSITYPE_S = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_BUSITYPE_S',
                name: 'BUSITYPE_S',
                store: store_BUSITYPE_S,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '业务类型',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            //贸易方式
            var store_TRADEWAY_S = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_myfs
            });
            var combo_TRADEWAY_S = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_TRADEWAY_S',
                name: 'TRADEWAY_S',
                store: store_TRADEWAY_S,
                queryMode: 'local',
                anyMatch: true,
                hideTrigger: true,
                fieldLabel: '贸易方式',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            //文件类型
            var store_FILETYPE_S = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_wjlx
            });
            var combo_FILETYPE_S = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_FILETYPE_S',
                name: 'FILETYPE_S',
                store: store_FILETYPE_S,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '文件类型',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { addfilesplit_Win("", "{}"); } }
                    , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改', width: 80, handler: function () { editfilesplit(); } }
                    , { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { del(); } }
                    , '->'
                    , { text: '<span class="icon iconfont">&#xe60b;</span>&nbsp;查 询', width: 80, handler: function () { Ext.getCmp("pgbar").moveFirst(); } }
                    , { text: '<span class="icon iconfont">&#xe633;</span>&nbsp;重 置', width: 80, handler: function () { reset(); } }
                ]
            });

            var formpanel_search = Ext.create('Ext.form.Panel', {
                id: 'formpanel_search',
                border: 0,
                region: 'north',
                bbar: toolbar,
                fieldDefaults: {
                    margin: '5',
                    columnWidth: 0.25,
                    labelWidth: 70
                },
                items: [
                { layout: 'column', border: 0, items: [combo_BUSIUNITCODE_S, combo_CUSTOMERCODE_S, combo_REPUNITCODE_S, combo_BUSITYPE_S] },
                { layout: 'column', border: 0, items: [combo_TRADEWAY_S, combo_FILETYPE_S] }
                ]
            });
        }

        function gridbind() {
            var store_filesplit = Ext.create('Ext.data.JsonStore', {
                fields: ['ID', 'BUSIUNITCODE', 'CUSTOMERCODE', 'REPUNITCODE', 'BUSITYPE', 'FILETYPE', 'CREATEUSERID'
                    , 'CREATEUSERNAME', 'CREATETIME', 'TRADEWAY', 'BUSIUNITNAME', 'CUSTOMERNAME', 'REPUNITNAME', 'PROMPT'],
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: 'FileSplit.aspx?action=loadData',
                    reader: {
                        root: 'rows',
                        type: 'json',
                        totalProperty: 'total'
                    }
                },
                autoLoad: true,
                listeners: {
                    beforeload: function (store, options) {
                        store_filesplit.getProxy().extraParams = Ext.getCmp('formpanel_search').getForm().getValues();
                    }
                }
            })
            Ext.tip.QuickTipManager.init();
            var pgbar = Ext.create('Ext.toolbar.Paging', {
                id: 'pgbar',
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: store_filesplit,
                displayInfo: true
            })
            var gridpanel = Ext.create('Ext.grid.Panel', {
                id: 'gridpanel',
                height: 560,
                region: 'center',
                store: store_filesplit,
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                columns: [
                    { xtype: 'rownumberer', width: 35 },
                    { header: '经营单位', dataIndex: 'BUSIUNITCODE', width: 90 },
                    { header: '经营单位', dataIndex: 'BUSIUNITNAME', width: 180 },
                    { header: '委托单位', dataIndex: 'CUSTOMERCODE', width: 120 },
                    { header: '委托单位', dataIndex: 'CUSTOMERNAME', width: 180 },
                    { header: '申报单位', dataIndex: 'REPUNITCODE', width: 90 },
                    { header: '申报单位', dataIndex: 'REPUNITNAME', width: 180 },
                    { header: '贸易方式', dataIndex: 'TRADEWAY', renderer: gridrender, width: 120 },
                    { header: '业务类型', dataIndex: 'BUSITYPE', renderer: gridrender, width: 100 },
                    { header: '文件类型', dataIndex: 'FILETYPE', renderer: gridrender, width: 80 },
                    { header: '提示内容', dataIndex: 'PROMPT', renderer: gridrender, width: 120 },
                    { header: '创建人', dataIndex: 'CREATEUSERNAME', width: 120 },
                    { header: '创建时间', dataIndex: 'CREATETIME', width: 140 },
                    { header: 'ID', dataIndex: 'ID', hidden: true }
                ],
                listeners:
                {
                    'itemdblclick': function (view, record, item, index, e) {
                        editfilesplit();
                    }
                },
                viewConfig: {
                    enableTextSelection: true
                }
            });
        }

        function gridrender(value, cellmeta, record, rowIndex, columnIndex, stroe) {
            var rtn = "";
            var dataindex = cellmeta.column.dataIndex;
            if (dataindex == "BUSITYPE" && value) {
                var rec = store_busitype.findRecord('CODE', value);
                if (rec) {
                    rtn = rec.get("NAME");
                }
            }
            if (dataindex == "FILETYPE" && value) {
                var rec = store_wjlx.findRecord('CODE', value);
                if (rec) {
                    rtn = rec.get("NAME");
                }
            }

            if (dataindex == "TRADEWAY" && value) {
                var rec = store_tradeway.findRecord('CODE', value);
                if (rec) {
                    rtn = rec.get("NAME");
                }
            }
            if (dataindex == "PROMPT" && value) {
                cellmeta.tdAttr = 'data-qtip="' + value + '"';
                rtn = value;
            }

            return rtn;
        }

        function form_ini_win() {
            var field_ID = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });

            //经营单位
            var store_BUSIUNITCODE = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_jydw
            });
            var combo_BUSIUNITCODE = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_BUSIUNITCODE',
                name: 'BUSIUNITCODE',
                store: store_BUSIUNITCODE,
                hideTrigger: true,
                minChars: 4,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '经营单位',
                displayField: 'NAME',
                valueField: 'CODE',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    }
                }
            });

            //委托单位
            var store_CUSTOMERCODE = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_wtdw
            });
            var combo_CUSTOMERCODE = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_CUSTOMERCODE',
                name: 'CUSTOMERCODE',
                store: store_CUSTOMERCODE,
                hideTrigger: true,
                minChars: 4,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '委托单位',
                displayField: 'NAME',
                valueField: 'CODE',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    }
                }
            });

            //申报单位
            var store_REPUNITCODE = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_jydw
            });
            var combo_REPUNITCODE = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_REPUNITCODE',
                name: 'REPUNITCODE',
                store: store_REPUNITCODE,
                hideTrigger: true,
                minChars: 4,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '申报单位',
                displayField: 'NAME',
                valueField: 'CODE',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    }
                }
            });

            //贸易方式
            var store_TRADEWAY = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_myfs
            });
            var combo_TRADEWAY = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_TRADEWAY',
                name: 'TRADEWAY',
                store: store_TRADEWAY,
                queryMode: 'local',
                anyMatch: true,
                hideTrigger: true,
                fieldLabel: '贸易方式',
                displayField: 'NAME',
                valueField: 'CODE',
                listeners: {
                    focus: function (cb) {
                        cb.clearInvalid();
                    }
                }
            });

            //业务类型
            var store_BUSITYPE = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_busitype
            });
            var combo_BUSITYPE = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_BUSITYPE',
                name: 'BUSITYPE', flex: .5,
                store: store_BUSITYPE,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '业务类型',
                displayField: 'NAME',
                valueField: 'CODE'
            });

            //文件类型
            var store_FILETYPE = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_wjlx
            });
            var combo_FILETYPE = Ext.create('Ext.form.field.ComboBox', {
                id: 'combo_FILETYPE',
                name: 'FILETYPE', 
                store: store_FILETYPE,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '文件类型',
                displayField: 'NAME',
                valueField: 'CODE',
                allowBlank: false,
                blankText: '文件类型不能为空!'
            });

            var con_CON = {
                xtype: 'fieldcontainer',
                layout: 'hbox', margin: 0,
                items: [combo_BUSITYPE, combo_FILETYPE]
            };

            var field_PROMPT = Ext.create('Ext.form.field.Text', {
                id: 'PROMPT',
                name: 'PROMPT',
                fieldLabel: '提示内容',
                allowBlank: false,
                blankText: '提示内容不可为空!'
            });

            var formpanel_Win = Ext.create('Ext.form.Panel', {
                id: 'formpanel_Win',
                minHeight: 200,
                border: 0,
                buttonAlign: 'center',
                fieldDefaults: {
                    margin: '0 5 10 0',
                    labelWidth: 75,
                    columnWidth: 1,
                    labelAlign: 'right',
                    labelSeparator: '',
                    msgTarget: 'under'
                },
                items: [
                        { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_BUSIUNITCODE] },
                        { layout: 'column', height: 42, border: 0, items: [combo_CUSTOMERCODE] },
                        { layout: 'column', height: 42, border: 0, items: [combo_REPUNITCODE] },
                        { layout: 'column', height: 42, border: 0, items: [combo_TRADEWAY] },
                        { layout: 'column', height: 42, border: 0, items: [con_CON] },
                        { layout: 'column', height: 42, border: 0, items: [field_PROMPT] },
                        field_ID
                ],
                buttons: [{

                    text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {

                        if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                            return;
                        }

                        var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                        Ext.Ajax.request({
                            url: 'FileSplit.aspx',
                            type: 'Post',
                            params: { action: 'save', formdata: formdata },
                            success: function (response, option) {
                                var data = Ext.decode(response.responseText);
                                if (data.success) {
                                    Ext.Msg.alert('提示', "保存成功", function () {
                                        Ext.getCmp("pgbar").moveFirst();
                                    });
                                }
                                else {
                                    Ext.Msg.alert('提示', "保存失败", function () {
                                        Ext.getCmp("pgbar").moveFirst();
                                    });
                                }

                            }
                        });
                    }
                }]
            });
        }

        function addfilesplit_Win(ID, formdata) {
            form_ini_win();
            if (ID != "") {
                Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
            }

            var win = Ext.create("Ext.window.Window", {
                id: "win_d",
                title: '不可拆分配置维护',
                width: 500,
                height: 350,
                modal: true,
                items: [Ext.getCmp('formpanel_Win')]
            });
            win.show();
        }

        function editfilesplit() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
                return;
            }
            addfilesplit_Win(recs[0].get("ID"), recs[0].data);
        }

        function reset() {
            Ext.getCmp("combo_BUSIUNITCODE_S").setValue("");
            Ext.getCmp("combo_CUSTOMERCODE_S").setValue("");
            Ext.getCmp("combo_REPUNITCODE_S").setValue("");
            Ext.getCmp("combo_BUSITYPE_S").setValue("");
            Ext.getCmp("combo_FILETYPE_S").setValue("");
        }

        function del() {
            var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择要删除的记录！');
                return;
            }

            var ids = ""; 
            Ext.each(recs, function (rec) {
                ids += rec.get("ID") + ",";
            });            
            ids = ids.substr(0, ids.length - 1);

            Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
                if (btn == 'yes') {
                    Ext.Ajax.request({
                        url: 'FileSplit.aspx',
                        params: { action: 'delete', ids: ids },
                        type: 'Post',
                        success: function (response, option) {
                            var data = Ext.decode(response.responseText);
                            var msg = "";
                            if (data.success) { msg = "删除成功"; }
                            else { msg = "删除失败"; }
                            Ext.MessageBox.alert('提示', msg, function () {
                                Ext.getCmp("pgbar").moveFirst();
                            });
                        }
                    });
                }
            });
        }

    </script>
</head>
<body>

</body>
</html>
