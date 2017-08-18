<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoticeList.aspx.cs" Inherits="Web_After.BeforeMaintain.NoticeList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />   
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/pan.js" type="text/javascript"></script>



     <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/bootstrap32/css/bootstrap.min.css" rel="stylesheet" />
    <link href="/css/common.css" rel="stylesheet" />
    <script type="text/javascript" src="/js/ueditor/ueditor.config.js"></script>
    <script type="text/javascript" src="/js/ueditor/ueditor.all.min.js"></script>
    <script type="text/javascript" src="/js/ueditor/lang/zh-cn/zh-cn.js"></script>

    <script src="/js/upload/plupload.full.min.js"></script>
    <script src="/js/My97DatePicker/WdatePicker.js"></script>

    <script type="text/javascript" src="/js/datetime/UX_TimePickerField.js"></script>

    <script type="text/javascript" src="/js/datetime/UX_DateTimePicker.js"></script>

    <script type="text/javascript" src="/js/datetime/UX_DateTimeMenu.js"></script>

    <script type="text/javascript" src="/js/datetime/UX_DateTimeField.js"></script>

    <script type="text/javascript">

        var pgbar;
        var gridpanel,treepanel;
        Ext.onReady(function () {
            west();
            east();

            var panel = Ext.create('Ext.form.Panel', {
                title: '资讯管理',
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
                useArrows: true,
                animate: true,
                rootVisible: false,
                region: 'west',
                store: treepanelstore,
                width:200,
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
                    url: 'NoticeList.aspx?action=load',
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
                                      // opencenterwin_no("NoticeEdit.aspx?option=add", 950, 800);
                                      
                                      open_Win_edit();
                                  }
                              }
                              , '-', {
                                  text: '<i class="icon iconfont">&#xe632;</i>&nbsp;修 改', handler: function () {

                                     
                                      //var recs = gridpanel.getSelectionModel().getSelection();
                                      //if (recs.length == 0) {
                                      //    Ext.Msg.alert("提示", "请选择修改记录!");
                                      //    return;
                                      //}
                                      //opencenterwin_no("NoticeEdit.aspx?action=load&option=update&ID=" + recs[0].get("ID"), 950, 800);
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
                                                  url: 'NoticeList.aspx?action=delete',
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
                    { header: '发布日期', dataIndex: 'PUBLISHDATE', width: 120 },
                    { header: '类别', dataIndex: 'TYPENAME', width: 120 },
                    { header: '更新时间', dataIndex: 'UPDATETIME', width: 150 }
                ],
                //添加双击事件
                listeners:
                {
                    'itemdblclick': function (view, record, item, index, e) {
                        opencenterwin_no("NoticeEdit.aspx?action=load&option=update&ID=" + record.data.ID, 950, 800);
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

        function open_Win_edit(ID) {
           
            var _editor, uploader;
            var j = "";//前缀符号，用于显示父子关系，这里可以使用其它符号
         
            function creatSelectTree(data, Array_data) {

                Ext.each(data, function (item) {
                    Array_data.push({ CODE: item.ID, NAME: j + item.NAME });
                    if (item.children.length)  {
                        j += "——";
                        creatSelectTree(item.children,Array_data);
                        j = j.slice(0, j.length - 2);
                    } 

                });
            }

            Ext.Ajax.request({
                url: "NewCategoryHandler.ashx",
                success: function (response, option) {
                    var data = Ext.decode(response.responseText);
                    var Array_data = [];
                    creatSelectTree(data, Array_data);

                    var store_cat = Ext.create('Ext.data.JsonStore', {
                        fields: ['CODE','NAME'],
                        data: Array_data
                    })
                       



                    /////////

                    var tf_rtbTitle = Ext.create('Ext.form.field.Text', {
                        id: 'rtbTitle',
                        name: 'rtbTitle',
                        fieldLabel: '标题',
                        margin: 5,
                        labelAlign: 'right',
                        labelWidth: 80,
                        anchor: '100%',
                        allowBlank: false,
                        blankText: '标题不能为空!'
                    });

    
                    var tf_rtbREFERENCESOURCE = Ext.create('Ext.form.field.Text', {
                        id: 'rtbREFERENCESOURCE',
                        name: 'rtbREFERENCESOURCE',
                        fieldLabel: '本文来源',
                        margin: 5,
                        labelAlign: 'right',
                        labelWidth: 80,
                        anchor: '100%'
                    });
                    var combo_cat = Ext.create('Ext.form.field.ComboBox', {
                        id: 'rcbType',
                        name: 'rcbType',
                        store: store_cat,
                        hideTrigger: false,
                        fieldLabel: '类别',
                        displayField: 'NAME',
                        valueField: 'CODE',
                        editable: false,
                        margin: 5,
                        labelAlign: 'right',
                        labelWidth: 80,
                        anchor: '100%',
                        queryMode: 'local'
                    })

                    //文件列表
                   file_store = Ext.create('Ext.data.JsonStore', {
                        fields: ['FILENAME', 'ORIGINALNAME', 'UPLOADTIME', 'SIZES']
                    })
                    var tmp = new Ext.XTemplate(
                         '<tpl for=".">',
                        '<div class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:240px">',
                        '<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><a href={[values.FILENAME]} target="_blank"><span class="glyphicon glyphicon-paperclip"></span></a></div></div>',
                        '<tpl>{[values.SIZES/1024 > 1024?Math.round(values.SIZES/(1024*1024))+"M":Math.round(values.SIZES/1024)+"K"]}</tpl>',
                        '|{[values.UPLOADTIME]}</div></div>',
                        '</tpl>'
                        )
                    var fileview = Ext.create('Ext.view.View', {
                        id: 'w_fileview',
                        store: file_store,
                        overflowY: 'scroll',
                        height: 100,
                        tpl: tmp,
                        itemSelector: 'div.thumb-wrap',
                        multiSelect: true
                    })
                    var panel = Ext.create('Ext.panel.Panel', {
                        border: 0,
                        
                        items: [fileview]
                    })



                    var formpanel_Win = Ext.create('Ext.form.Panel', {
                        id: 'formpanel_Win',
                        border: 0,
                        buttonAlign: 'center',
                        height: 750,
                        items: [
                            tf_rtbTitle, combo_cat,
                            {
                                xtype: 'datetimefield',
                                labelAlign: 'right',
                                labelWidth: 80,
                                margin: 5,
                                anchor: '100%',
                                fieldLabel: '开始时间',
                                format: 'Y-m-d H:i:s ',
                                name: 'rtbPublishDate',
                                id: 'rtbPublishDate'
                            },
                        {
                            xtype: 'fieldcontainer',
                            fieldLabel: '内容',
                            margin: 5,
                            labelAlign: 'right',
                            labelWidth: 80,
                            html: '<div id="ue" style="width:100%"></div>',
                            listeners: {
                                render: function () {
                                    _editor = UE.getEditor('ue');
                                    _editor.ready(function () {
                                        _editor.setHeight(290);
                                        _editor.setContent('test');
                                    });
                                }
                            }
                        }, tf_rtbREFERENCESOURCE,
                            {
                                xtype: 'fieldcontainer',
                                fieldLabel: '附件',
                                labelAlign: 'right',
                                labelWidth: 80,
                                anchor: '100%',
                                html: '<button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button><button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'

                            }, panel
                        ]
                        , buttons: [{
                            text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {
                                var formdata = Ext.encode(formpanel_Win.getForm().getValues());
                                var filedata = Ext.encode(Ext.pluck(file_store.data.items, 'data'));
                                Ext.Ajax.request({
                                    url: "NoticeEdit.aspx?action=save",
                                    params: { rtbID: ID, formdata: formdata, rchAttachment: filedata },
                                    success: function (response, option) {
                                        if (response.responseText) {
                                            //var data = Ext.decode(response.responseText);
                                            //if (data.success) {
                                            //    ordercode = data.ordercode;
                                            //    Ext.MessageBox.alert("提示", action == 'submit' ? "提交成功！" : "保存成功！", function () {
                                            //        loadform();
                                            //    });
                                            //}
                                            //else {
                                            //    Ext.MessageBox.alert("提示", action == 'submit' ? "提交失败！" : "保存失败！");
                                            //}
                                        }
                                    }
                                });
                            }
                        }]
                    });

                    var win = Ext.create("Ext.window.Window", {
                        id: "win_d",
                        title: '资讯管理-新增||修改',
                        width: 800,
                        height: 800,
                        modal: true,
                        items: [formpanel_Win],
                        listeners: { close: function () { _editor.destroy(); } }
                    });
                    win.show();

                    if (uploader == null) {
                        upload_ini();
                    }

                    
                }

            });

         
        }
      
    </script>
</head>
<body>

</body>
</html>
