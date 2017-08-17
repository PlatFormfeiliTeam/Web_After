<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CollectInforCate.aspx.cs" Inherits="Web_After.BeforeMaintain.CollectInforCate" %>

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
        .bgcolor {
            background: #FFFFFF !important;
        }
    </style>

    <script type="text/javascript" >
        Ext.onReady(function () {
            gridbind();

            var panel = Ext.create('Ext.form.Panel', {
                title: '常用查询',
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
           
            var store_CollectInforCate = Ext.create('Ext.data.JsonStore', {
                fields: ["ID", "NAME", "ICON", "DESCRIPTION", "SORTINDEX"],
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: "CollectInforCate.aspx?action=select",
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
                store: store_CollectInforCate,
                displayInfo: true
            })
            var gridpanel = Ext.create('Ext.grid.Panel', {
                id: 'gridpanel',
                height: 560,
                region: 'center',
                store: store_CollectInforCate,
                columnLines: true,
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                columns: [
                    { xtype: 'rownumberer', width: 35, tdCls: 'tdValign' },
                    { header: 'ID', dataIndex: 'ID', width: 200, sortable: true, hidden: true },
                    { header: '常用类别', dataIndex: 'NAME', width: 200, sortable: true, tdCls: 'tdValign' },
                    { header: '描述', dataIndex: 'DESCRIPTION', flex: 1, sortable: true, tdCls: 'tdValign' },
                    {
                        header: '图标', dataIndex: 'ICON', width: 100, renderer: function (value) {
                            return "<i class=\"icon iconfont\" style=\"font-size: 23px;\">&#x" + value + ";</i>";
                        }
                    },
                    { header: '排列顺序', dataIndex: 'SORTINDEX', width: 100, sortable: true },
                    {
                        header: '操作', dataIndex: 'ID', width: 70, renderer: function render(value, cellmeta, record, rowIndex, columnIndex, store) {
                            return "<a style='cursor: pointer;' onclick='editcate(\"" + record.get("ID") + "\",\"" + record.get("NAME") + "\",\"" + record.get("DESCRIPTION") + "\",\"" + record.get("SORTINDEX") + "\")'><i class=\"icon iconfont\">&#xe632;</i></a>";
                        }
                    }
                ],
                plugins: [{
                    ptype: 'rowexpander',
                    rowBodyTpl: ['<div id="div_{ID}"></div>']
                }],
                viewConfig: {
                    enableTextSelection: true
                }
            });

            gridpanel.view.on('expandBody', function (rowNode, record, expandRow, eOpts) {
                displayInnerGrid(record.get('ID'));
            });
            gridpanel.view.on('collapsebody', function (rowNode, record, expandRow, eOpts) {
                destroyInnerGrid(record.get("ID"));
            });
        }

        function editcate(id, name, description, sortindex) {
            var field_id = Ext.create('Ext.form.field.Hidden', { name: 'ID', value: id });

            var field_NAME = Ext.create('Ext.form.field.Text', {
                name: 'NAME', fieldLabel: '类别', allowBlank: false, labelAlign: 'right', msgTarget: 'under', margin: '5', anchor: '90%', blankText: '名称不能为空!', value: name
            });
            var field_DESCRIPTION = Ext.create('Ext.form.field.Text', {
                name: 'DESCRIPTION', fieldLabel: '描述', allowBlank: false, labelAlign: 'right', msgTarget: 'under', margin: '5', anchor: '90%', blankText: '描述不能为空!', value: description
            });
            var field_SORTINDEX = Ext.create('Ext.form.Number', {
                name: 'SORTINDEX', fieldLabel: '排列顺序', allowBlank: false, labelAlign: 'right', anchor: '90%', margin: '5', msgTarget: 'under', blankText: '排列顺序不能为空!', value: sortindex
            });
            var formpanel = Ext.create('Ext.form.Panel', {
                id: 'f_formpanel',
                region: 'center',
                height: 250,
                buttonAlign: 'center',
                items: [field_id, field_NAME, field_DESCRIPTION, field_SORTINDEX],
                buttons: [{
                    text: '保 存',
                    handler: function () {
                        if (formpanel.getForm().isValid()) {
                            var formdata = Ext.encode(Ext.getCmp('f_formpanel').getForm().getValues());

                            Ext.getCmp('f_formpanel').getForm().submit({
                                url: 'CollectInforCate.aspx',
                                params: { formdata: formdata, action: 'save' },
                                waitMsg: '保存中...', //提示等待信息  
                                success: function (form, action) {
                                    Ext.Msg.alert('提示', '保存成功', function () {
                                        Ext.getCmp("pgbar").moveFirst();
                                        Ext.getCmp("win_d").close();
                                    });
                                },
                                failure: function (form, action) {//失败要做的事情 
                                    Ext.MessageBox.alert("提示", "保存失败!", function () { Ext.getCmp("win_d").close(); });
                                }
                            });

                        }
                    }
                }]
            });
            var win = Ext.create("Ext.window.Window", {
                id: "win_d",
                title: '常用查询',
                width: 500,
                height: 300,
                modal: true,
                items: [Ext.getCmp('f_formpanel')]
            });
            win.show();
        }

        function displayInnerGrid(div) {
            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                    { text: '<i class="iconfont">&#xe622;</i>&nbsp;添加', handler: function () { add_win(Ext.decode("{RID_TYPE:" + div + ",ISINVALID:0}"), store_inner); } }
                    , { text: '<i class="icon iconfont">&#xe632;</i>&nbsp;修改', handler: function () { edit_inner(grid_inner, store_inner); } }
                    , { text: '<i class="icon iconfont">&#xe6d3;</i>&nbsp;删除', handler: function () { del_inner(grid_inner, store_inner); } }
                ]
            });
            var store_inner = Ext.create('Ext.data.JsonStore', {
                fields: ['ID', 'NAME', 'URL', 'RID_TYPE', 'ICON', 'ISINVALID', 'CREATETIME'],
                proxy: {
                    url: 'CollectInforCate.aspx?action=loaddetail&id=' + div,
                    type: 'ajax',
                    reader: {
                        type: 'json',
                        root: 'innerrows'
                    }
                },
                autoLoad: true
            })
            var grid_inner = Ext.create('Ext.grid.Panel', {
                store: store_inner, tbar: toolbar,
                margin: '0 0 0 70',
                selModel: { selType: 'checkboxmodel' },
                columns: [
                    { xtype: 'rownumberer', width: 25 },
                    { header: 'ID', dataIndex: 'ID', hidden: true },
                    { header: '名称', dataIndex: 'NAME', width: 200 },
                    { header: '链接地址', dataIndex: 'URL', flex: 1 },
                    {
                        header: '图标', dataIndex: 'ICON', width: 100, renderer: function (value) {
                            return "<img style='width:46px;height:60px' src='" + value + "'/>";
                        }
                    },
                    {
                        header: '状态', dataIndex: 'ISINVALID', width: 120, renderer: function (value) {
                            return value == "0" ? "启用" : "停用";
                        }
                    },
                    { header: '创建时间', dataIndex: 'CREATETIME', width: 150 }
                ],
                renderTo: 'div_' + div
            })

            grid_inner.getEl().swallowEvent([
                'mousedown', 'mouseup', 'click',
                'contextmenu', 'mouseover', 'mouseout',
                'dblclick', 'mousemove'
            ]);

        }

        function destroyInnerGrid(div) {
            var parent = document.getElementById('div_' + div);
            var child = parent.firstChild;
            while (child) {
                child.parentNode.removeChild(child);
                child = child.nextSibling;
            }
        }
       
        function form_ini_win(store_inner) {
            var field_ID = Ext.create('Ext.form.field.Hidden', {
                id: 'ID',
                name: 'ID'
            });
            var field_RID_TYPE = Ext.create('Ext.form.field.Hidden', {
                id: 'RID_TYPE',
                name: 'RID_TYPE'
            });

            var label_baseinfo = {
                xtype: 'label', margin: '100',
                html: '<span style="color:blue; font-size:12px;">说明：修改时，图标不变，则不需要重新上传</span>'
            }

            var icon = Ext.create('Ext.form.field.File', {
                id: 'ICON', name: 'ICON', fieldLabel: '图标', labelAlign: 'right', msgTarget: 'under', margin: '10', anchor: '90%'
               , buttonText: '上传图片', regex: /.*(.jpg|.png|.gif)$/, regexText: '只能上传JPG,png,gif'
            });
            var name = Ext.create('Ext.form.field.Text', {
                name: 'NAME', fieldLabel: '名称', labelAlign: 'right', msgTarget: 'under', margin: '10', anchor: '90%'
            })

            var url = Ext.create('Ext.form.field.Text', {
                name: 'URL', fieldLabel: '链接地址', labelAlign: 'right', msgTarget: 'under', margin: '10', anchor: '90%'
            });

            var isinvalid = Ext.create('Ext.form.RadioGroup', {
                name: "ISINVALID", id: "ISINVALID", fieldLabel: '状态', labelAlign: 'right', anchor: '40%', margin: '10',
                items: [
                    { boxLabel: '启用', name: 'ISINVALID', inputValue: 0, checked: true },
                    { boxLabel: '停用', name: 'ISINVALID', inputValue: 1 }
                ]
            });

            var icon_old = Ext.create('Ext.form.field.Hidden', { name: 'ICON_OLD' });

            var formpanel_Win = Ext.create('Ext.form.Panel', {
                id: 'formpanel_Win',
                minHeight: 170,
                border: 0,
                buttonAlign: 'center',
                items: [icon, label_baseinfo, name, url, isinvalid, icon_old, field_ID, field_RID_TYPE],
                buttons: [{

                    text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {

                        if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                            return;
                        }

                        var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                        formpanel_Win.getForm().submit({
                            url: 'CollectInforCate.aspx',
                            params: { action: 'saveinner', formdata: formdata },
                            waitMsg: '保存中...', //提示等待信息  
                            success: function (form, action) {
                                Ext.Msg.alert('提示', '保存成功', function () {
                                    store_inner.reload();
                                    Ext.getCmp("win_d").close();
                                });
                            },
                            failure: function (form, action) {//失败要做的事情 
                                Ext.MessageBox.alert("提示", "图片尺寸必须为46*60!", function () {
                                    store_inner.reload();
                                    Ext.getCmp("win_d").close();
                                });
                            }
                        });

                    }
                }]
            });
        }

        function add_win(formdata, store_inner) {
            form_ini_win(store_inner);
            Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
            Ext.getCmp("ISINVALID").setValue({ ISINVALID: formdata.ISINVALID });
           

            var win = Ext.create("Ext.window.Window", {
                id: "win_d",
                title: '常用查询维护',
                width: 800,
                height: 300,
                modal: true,
                items: [Ext.getCmp('formpanel_Win')]
            });
            win.show();
        }

        function edit_inner(grid_inner, store_inner) {
            var recs = grid_inner.getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要修改的记录！');
                return;
            }
            add_win(recs[0].data, store_inner);
        }

        function del_inner(grid_inner, store_inner) {
            var recs = grid_inner.getSelectionModel().getSelection();
            if (recs.length == 0) {
                Ext.MessageBox.alert('提示', '请选择需要删除的记录！');
                return;
            }
            Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
                if (btn == 'yes') {
                    Ext.Ajax.request({
                        url: 'CollectInforCate.aspx?action=delete',
                        params: { id: recs[0].get("ID"), icon: recs[0].get("ICON") },
                        callback: function () {
                            Ext.MessageBox.alert('提示', '删除成功！', function () {
                                store_inner.reload();
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
