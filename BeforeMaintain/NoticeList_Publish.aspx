<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="NoticeList_Publish.aspx.cs" Inherits="Web_After.NoticeList_Publish" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/pan.js" type="text/javascript"></script>
    <link href="/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />   

    <script type="text/javascript">
        var pgbar;
        var gridpanel, treepanel;
        Ext.onReady(function () {
            west();
            east();

            var panel = Ext.create('Ext.form.Panel', {
                title: '资讯发布',
                region: 'center',
                layout: 'border',
                items: [treepanel, gridpanel]
            })

            var viewport = Ext.create('Ext.container.Viewport', {
                layout: 'border',
                region: 'center',
                items: [panel]

            });
        });

        function west() {
            var treepanelstore = Ext.create('Ext.data.TreeStore', {
                fields: ["ID", "NAME", "leaf", "PID"],
                proxy: {
                    type: 'ajax',
                    url: 'NewCategoryHandler.ashx',
                    reader: 'json',
                },
                root: {
                    expanded: true,
                    text: "my root"
                }
            });

              treepanel = Ext.create('Ext.tree.Panel', {
                id: 'treepanel',
                region: 'west',
                useArrows: true,
                animate: true,
                rootVisible: false,
                store: treepanelstore,
                width: 200,
                columns: [
                { text: 'ID', dataIndex: 'ID', width: 500, hidden: true },
                { text: 'leaf', dataIndex: 'leaf', width: 100, hidden: true },
                { header: '类别名称', xtype: 'treecolumn', text: 'NAME', dataIndex: 'NAME', flex: 1 },
                { text: 'PID', dataIndex: 'PID', width: 100, hidden: true }
                ],
                listeners: {
                    'checkchange': function (node, checked, eOpts) {
                        setChildChecked(node, checked);
                        pgbar.moveFirst();
                    }
                }
            });

        }

        function east() {
            var store_Notice = Ext.create('Ext.data.JsonStore', {
                fields: ['ID', 'TITLE', 'ISINVALID', 'TYPE', 'PUBLISHDATE', 'TYPENAME', 'UPDATETIME'],
                pageSize: 15,
                proxy: {
                    type: 'ajax',
                    url: 'NoticeList_Publish.aspx?action=load',
                    reader: {
                        root: 'rows',
                        type: 'json',
                        totalProperty: 'total'
                    }
                },
                autoLoad: true,
                listeners: {
                    beforeload: function (store, options) {
                        var nodeid = "";
                        var a = Ext.getCmp('treepanel').getChecked();
                        for (var i = 0; i < a.length; i++) {
                            nodeid = nodeid + a[i].data.ID;
                            if (i != a.length - 1) { nodeid = nodeid + ","; }
                        }
                        var new_params = {
                            TYPEID: nodeid,
                            TITLE: Ext.getCmp("TITLE").getValue()
                        }
                        Ext.apply(store.proxy.extraParams, new_params);
                    }
                }
            })

            var toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: [
                            {
                                xtype: 'textfield', fieldLabel: '标题', labelWidth: 60, labelAlign: 'right', id: 'TITLE', flex: 1
                            },
                              {
                                  xtype: 'button', text: '<i class="iconfont">&#xe60b;</i>&nbsp;查 询', handler: function () {
                                      pgbar.moveFirst();
                                  }
                              }, '-', {
                                  text: '<i class="iconfont">&#xe622;</i>&nbsp;添 加', handler: function () {
                                      opencenterwin_no("NoticeEdit_Publish.aspx?option=add", 950, 800);
                                  }
                              }
                              , '-', {
                                  text: '<i class="icon iconfont">&#xe632;</i>&nbsp;修 改', handler: function () {

                                      var recs = gridpanel.getSelectionModel().getSelection();
                                      if (recs.length == 0) {
                                          Ext.Msg.alert("提示", "请选择修改记录!");
                                          return;
                                      }
                                      opencenterwin_no("NoticeEdit_Publish.aspx?action=load&option=update&ID=" + recs[0].get("ID"), 950, 800);
                                  }
                              }
                              , '-', {
                                  text: '<i class="icon iconfont">&#xe6d3;</i>&nbsp;删 除', handler: function () {
                                      var recs = gridpanel.getSelectionModel().getSelection();
                                      if (recs.length == 0) {
                                          Ext.MessageBox.alert('提示', '请选择需要删除的记录！');
                                          return;
                                      }

                                      var formIds = "";
                                      Ext.each(recs, function (rec) {
                                          formIds += "'" + rec.get("ID") + "',";
                                      })
                                      if (formIds.length > 0) { formIds = formIds.substr(0, formIds.length - 1); }

                                      Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
                                          if (btn == 'yes') {
                                              Ext.Ajax.request({
                                                  url: 'NoticeList_Publish.aspx?action=delete',
                                                  params: { Id: formIds },
                                                  success: function (response, success, option) {
                                                      var res = Ext.decode(response.responseText);
                                                      if (res.success) {
                                                          Ext.MessageBox.alert('提示', '删除成功！');
                                                          pgbar.moveFirst();
                                                      }
                                                      else {
                                                          Ext.MessageBox.alert('提示', '删除失败！');
                                                      }
                                                  }
                                              });
                                          }
                                      });
                                  }
                              }
                ]
            })
            Ext.tip.QuickTipManager.init();
            pgbar = Ext.create('Ext.toolbar.Paging', {
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: store_Notice,
                displayInfo: true
            })

              gridpanel = Ext.create('Ext.grid.Panel', {
                store: store_Notice,
                height: 550,
                tbar: toolbar,
                region: 'center',
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                columns: [
                    { xtype: 'rownumberer', width: 35 },
                    { header: 'ID', dataIndex: 'ID', hidden: true },
                    { header: '标题', dataIndex: 'TITLE', flex: 1, renderer: ViewAll },
                    { header: '是否发布', dataIndex: 'ISINVALID', width: 80, renderer: render },
                    { header: '发布日期', dataIndex: 'PUBLISHDATE', width: 120 },
                    { header: '类别', dataIndex: 'TYPENAME', width: 120 },
                    { header: '更新时间', dataIndex: 'UPDATETIME', width: 150 }
                ],
                //添加双击事件
                listeners:
                {
                    'itemdblclick': function (view, record, item, index, e) {
                        opencenterwin_no("NoticeEdit_Publish.aspx?action=load&option=update&ID=" + record.data.ID, 950, 800);
                    }
                },
                viewConfig: {
                    enableTextSelection: true
                }
            });
        }

        function ViewAll(value, meta, record) {
            meta.tdAttr = 'data-qtip="' + value + '"';
            return value;
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
    </script>
    </head>
<body>

</body>
</html>
