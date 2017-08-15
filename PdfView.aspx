<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PdfView.aspx.cs" Inherits="Web_After.PdfView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-gray.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="js/pan.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />
    <style type="text/css">
        .x-grid-cell {
            border-bottom-color: black;
            border-right-color: black;
        }

        .cbg-font-color {
            color: blue;
        }
    </style>
    <script type="text/javascript">
        var ordercode = getQueryString("ordercode");
        var userid = getQueryString("userid");
        var filetype = 44;
        var fileid = "";
        var path = "";
        var allow_sel;
        var filedarray = [];
        var formpanel, gridpanel, toolbar; var win_mergeordelete, store;
        Ext.onReady(function () {
            var field_code = Ext.create('Ext.form.RadioGroup', {
                id: 'field_code',
                margin: '10',
                columnWidth: .25,
                labelAlign: "right",
                fieldLabel: '订单号',
                columns: 2,
                vertical: true,
                items: [],
                listeners: {
                    change: function (rb, newValue, oldValue, eOpts) {
                        ordercode = newValue.rbg;
                        fileid = "";
                        iniform();
                    }
                }
            });
            var common_data_busitype = [
            { "CODE": "10", "NAME": "空运出口" }, { "CODE": "11", "NAME": "空运进口" },
            { "CODE": "20", "NAME": "海运出口" }, { "CODE": "21", "NAME": "海运进口" },
            { "CODE": "30", "NAME": "陆运出口" }, { "CODE": "31", "NAME": "陆运进口" },
            { "CODE": "40", "NAME": "国内出口" }, { "CODE": "41", "NAME": "国内进口" },
            { "CODE": "50", "NAME": "特殊区域出口" }, { "CODE": "51", "NAME": "特殊区域进口" }];

            var store_busitype = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_busitype
            })
            var combo_busitype = Ext.create('Ext.form.field.ComboBox', {
                name: 'BUSITYPE',
                store: store_busitype,
                displayField: 'NAME',
                queryMode: 'local',
                valueField: 'CODE',
                margin: 10,
                columnWidth: .25,
                labelAlign: "right",
                fieldLabel: '业务类型',
                readOnly: true
            })

            var field_busiunit = Ext.create('Ext.form.field.Text', {
                name: 'BUSIUNITNAME',
                margin: 10,
                columnWidth: .25,
                labelAlign: "right",
                fieldLabel: '经营单位',
                readOnly: true
            });
            var field_filestatus = Ext.create('Ext.form.field.Text', {
                id: 'field_filestatus',
                margin: '10',
                columnWidth: .25,
                labelAlign: "right",
                fieldLabel: '拆分状态',
                name: 'FILESTATUS',
                readOnly: true
            });

            var field_file = Ext.create('Ext.form.RadioGroup', {
                id: 'field_file',
                margin: 10,
                columnWidth: 1,
                labelAlign: "right",
                fieldLabel: '订单文件',
                columns: 10,
                vertical: false,
                items: [],
                listeners: {
                    change: function (rb, newValue, oldValue, eOpts) {
                        fileid = newValue.rbgfile; 
                        pdfview();
                    }
                }
            });

            formpanel = Ext.create('Ext.form.Panel', {
                title: '文件拆分',
                region: 'north',
                height: 120,
                items: [{ layout: 'column', border: 0, items: [field_code, combo_busitype, field_busiunit, field_filestatus] },
                        { layout: 'column', border: 0, items: [field_file] }
                ]
            });
            var html = '<div id="pdfdiv" style="width:100%;height:100%"></div>';
            var panel = Ext.create('Ext.panel.Panel', {
                region: 'center',
                html: html,
                id: 'panel_file'
            });
            toolbar = Ext.create("Ext.toolbar.Toolbar", {
                items: [
                    {
                        text: '<i class="iconfont">&#xe66c;</i>合并||删除', id: 'btn_mergedelete', 
                        handler: function () {                            
                            document.getElementById('pdfdiv').innerHTML = '<embed  id="pdf" width="100%" height="100%" src=""></embed>';
                            panel.hide();
                            win_mergeordelete.show();
                        }
                    },
                    {
                        text: '<i class="iconfont">&#xe6c3;</i>&nbsp;确定拆分', id: 'btn_confirmsplit', disabled: true, handler: function () {
                            var allowsplit = false;
                            var store_tmp = gridpanel.getStore();
                            for (var i = 0; i < store_tmp.getCount() ; i++) {
                                var rec = store_tmp.getAt(i);
                                for (var j = 0; j < filedarray.length; j++) {
                                    if (rec.get(filedarray[j]) == "√") {
                                        allowsplit = true;
                                    }
                                }
                            }
                            if (!allowsplit) {
                                panel.hide();
                                Ext.MessageBox.alert('提示', '请先勾选具体的拆分明细！', function () {
                                    panel.show();
                                })
                                return;
                            } 
                            if (Ext.getCmp('field_file').getChecked().length == 0)
                            {
                                panel.hide();
                                Ext.MessageBox.alert('提示', '请选择需要拆分的文件！', function () {
                                    panel.show();
                                });
                                return;
                            }
                            Ext.getCmp("btn_confirmsplit").setDisabled(true);
                            var pages = Ext.encode(Ext.pluck(gridpanel.store.data.items, 'data'));
                            Ext.Ajax.request({
                                url: "PdfView.aspx",
                                //?action=split&fileid=" + fileid + "&filetype=" + filetype + "&ordercode=" + ordercode
                                params: { action: 'split', fileid: fileid, filetype: filetype, ordercode: ordercode, pages: pages, userid: userid },
                                success: function (response) {
                                    panel.hide();
                                    var json = Ext.decode(response.responseText);
                                    if (json.success) {
                                        Ext.MessageBox.alert('提示', '拆分成功！', function () {
                                            panel.show();
                                            field_filestatus.setValue('已拆分');
                                        })
                                        Ext.getCmp('btn_cancelsplit').setDisabled(false);
                                        allow_sel = false;
                                        for (var i = 0; i < json.result.length; i++) {
                                            //拆分完成后添加拆分好文件类型的查看按钮  
                                            var btn = Ext.create('Ext.Button', {
                                                id: json.result[i].FILETYPEID + "_" + json.result[i].ID,
                                                text: '<i class="fa fa-file-pdf-o"></i>&nbsp;' + json.result[i].FILETYPENAME,
                                                handler: function () {
                                                    gridpanel.getStore().removeAll();
                                                    loadfile(this.id);
                                                }
                                            })
                                            toolbar.add(btn);
                                        }
                                    }
                                    else {
                                        Ext.MessageBox.alert('提示', '拆分失败，文件压缩中，请稍后再试！', function () {
                                            panel.show();
                                        })
                                    }
                                }
                            });
                        }
                    }, {
                        text: '<i class="iconfont">&#xe633;</i>&nbsp;撤销拆分', id: 'btn_cancelsplit', handler: function () {
                            panel.hide();
                            Ext.MessageBox.confirm('提示', '确定要撤销拆分吗？', function (btn) {
                                if (btn == 'yes') {
                                    Ext.getCmp('btn_cancelsplit').setDisabled(true);
                                    Ext.Ajax.request({
                                        url: 'PdfView.aspx?action=cancelsplit&ordercode=' + ordercode + "&fileid=" + fileid + "&userid=" + userid,
                                        success: function (response, opts) {
                                            panel.hide();
                                            Ext.MessageBox.alert('提示', '撤销拆分成功！', function () {
                                                field_filestatus.setValue('未拆分');
                                                panel.show();
                                            })
                                            Ext.getCmp("btn_confirmsplit").setDisabled(false);
                                            allow_sel = true;
                                            var times = toolbar.items.length
                                            for (var i = 3; i < times; i++) {
                                                var btn = toolbar.getComponent(3);//移除了第4个元素后，后面的元素会自动填充到第4的位置
                                                if (btn) {
                                                    toolbar.remove(btn);
                                                }
                                            }
                                        }
                                    })
                                }
                                panel.show();
                            })
                        }
                    }
                ]
            });
            gridpanel = Ext.create('Ext.grid.Panel', {
                region: 'east',
                columnLines: true,
                rowLines: true,
                width: 750,
                tbar: toolbar,
                sortableColumns: false,
                enableColumnHide: false,
                columns: [],
                listeners: {
                    itemclick: function (grid, record, item, index, e, eOpts) {
                        var PDF = Ext.get("pdf").dom;
                        PDF.setCurrentPage(record.get("ID"));
                    },
                    cellclick: function (view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                        if (allow_sel) {
                            var header = view.getHeaderCt().getHeaderAtIndex(cellIndex);
                            if (header.dataIndex != "ID") {
                                record.set(header.dataIndex, record.get(header.dataIndex) == "√" ? "" : "√");
                            }
                        }
                    }
                }
            })
            var viewport = Ext.create('Ext.container.Viewport', {
                layout: 'border',
                items: [formpanel, gridpanel, panel]
            })
            //如果只有一个订单文件,直接加载进行拆分;如果有多个订单文件，则不予加载,勾选后
            //默认加载第一个订单文件 
            iniform();
            init_mergeordelete();
        });
        function iniform() {
            Ext.Ajax.request({
                url: "PdfView.aspx",
                params: { ordercode: ordercode, action: 'loadform' },
                success: function (response, option) {
                    var json = Ext.decode(response.responseText);
                    formpanel.getForm().setValues(json.formdata);
                    if (json.formdata.FILESTATUS == 1) {
                        Ext.getCmp('field_filestatus').setValue("已拆分");
                    }
                    else {
                        Ext.getCmp('field_filestatus').setValue("未拆分");
                    }
                    if (Ext.getCmp('field_code').items.length == 0) {
                        Ext.getCmp('field_code').insert({ boxLabel: json.formdata.CODE, name: 'rbg', inputValue: json.formdata.CODE, checked: true })
                        if (json.formdata.ASSOCIATENO) {//如果存在两单关联号
                            Ext.getCmp('field_code').insert({ boxLabel: json.formdata.ASSOCIATENO, name: 'rbg', inputValue: json.formdata.ASSOCIATENO });
                        }
                    }                   

                    Ext.getCmp('field_file').removeAll();
                    Ext.getCmp('field_file').reset();//若不重置，会导致再选打开窗体之前的文件，点了没反应
                    for (var i = 0; i < json.filedata.length; i++) {
                        Ext.getCmp('field_file').insert(Ext.getCmp('field_file').items.length, {
                            boxLabel: '订单文件' + (i + 1), name: 'rbgfile', inputValue: json.filedata[i].ID
                        });
                    }

                    if (Ext.getCmp('field_file').items.length == 1) {
                        Ext.getCmp('field_file').getComponent(0).setValue(true);
                    } else {
                        fileid = "";
                        document.getElementById('pdfdiv').innerHTML = "";
                        //清除追加的button按钮
                        var times = toolbar.items.length
                        for (var i = 3; i < times; i++) {
                            var btn = toolbar.getComponent(3);//移除了第4个元素后，后面的元素会自动填充到第4的位置
                            if (btn) {
                                toolbar.remove(btn);
                            }
                        }
                        gridpanel.getStore().removeAll();
                        gridpanel.reconfigure(store,[]);
                    }

                    //var obj = Ext.getCmp("field_file").items.items;
                    //for (var i in obj) {
                    //    if (obj[i].checked) {
                    //        Ext.Msg.alert("Tip", "您点击的radio名称是*****" + obj[i].inputValue);
                    //    }
                    //    obj[i].checked = false;
                    //}
                }
            });
        }
        function pdfview() {
            Ext.Ajax.request({
                url: "PdfView.aspx?action=loadpdf&ordercode=" + ordercode + "&fileid=" + fileid,
                success: function (response) {
                    var box = document.getElementById('pdfdiv');
                    if (response.responseText) {
                        var json = Ext.decode(response.responseText);
                        path = json.src;
                        var str = '<embed  id="pdf" width="100%" height="100%" src="' + json.src + '"></embed>';
                        box.innerHTML = str;
                        //按钮控制开始
                        var ordertatus = Ext.getCmp('field_filestatus').getValue();
                        if (ordertatus == '已拆分') {//订单的拆分状态
                            Ext.getCmp("btn_confirmsplit").setDisabled(true);
                            allow_sel = false;
                            if (json.filestatus == 0) {//文件的拆分状态
                                // Ext.getCmp('btn_cancelsplit').setDisabled(true);
                            }
                            else {
                                  Ext.getCmp('btn_cancelsplit').setDisabled(false);
                            }
                        }
                        else {
                            
                            Ext.getCmp("btn_confirmsplit").setDisabled(false);
                            allow_sel = true;
                            //  Ext.getCmp('btn_cancelsplit').setDisabled(true);
                        }
                        //按钮控制结束 
                        fileid = json.fileid;
                        Ext.regModel('Pager', { fields: [] });
                        var columnarray = [];
                        for (var key in json.rows[0]) {
                            filedarray.push(key);
                            switch (key) {
                                case "ID":
                                    columnarray.push({ header: '页码', dataIndex: key, width: 48, renderer: RowRender });
                                    columnarray.push({
                                        xtype: 'actioncolumn', width: 48, text: '操作'
                                        , items: [{
                                            icon: '/images/shared/arrow_up.gif',
                                            width: 30,
                                            handler: function (grid, rowIndex, colIndex) { }
                                        }, {
                                            icon: '/images/shared/arrow_down.gif',
                                            handler: function (grid, rowIndex, colIndex) { }
                                        }
                                        ]
                                    });
                                    break;
                                default:
                                    var start = key.indexOf("@");
                                    var header = key.slice(start + 1);
                                    columnarray.push({ header: header, dataIndex: key, width: 60 });
                                    break;
                            }
                        }
                        Pager.setFields(filedarray); //Model构建完毕
                        store = Ext.create('Ext.data.JsonStore',
                        {
                            model: 'Pager',
                            data: json.rows
                        })
                        gridpanel.reconfigure(store, columnarray);
                        //清除追加的button按钮
                        var times = toolbar.items.length
                        for (var i = 3; i < times; i++) {
                            var btn = toolbar.getComponent(3);//移除了第4个元素后，后面的元素会自动填充到第4的位置
                            if (btn) {
                                toolbar.remove(btn);
                            }
                        }
                        //拆分完成后添加拆分好文件类型的查看按钮    
                        for (var i = 0; i < json.result.length; i++) {
                            var id = json.result[i].ID;
                            var typeid = json.result[i].FILETYPEID;
                            var btn = Ext.create('Ext.Button', {
                                id: json.result[i].FILETYPEID + "_" + json.result[i].ID,
                                text: '<i class="iconfont">&#xe61d;</i>&nbsp;' + json.result[i].FILETYPENAME,
                                handler: function () {
                                    gridpanel.getStore().removeAll();
                                    loadfile(this.id);
                                }
                            })
                            toolbar.add(btn);
                        }
                    }
                }
            })
        }
        function RowRender(value, cellmeta, record, rowIndex, columnIndex, store) {
            return '第' + value + '页';
        }
        function loadfile(id) {
            var array1 = id.split('_');
            Ext.Ajax.request({
                url: "PdfView.aspx?action=loadfile&fileid=" + array1[1],
                success: function (response) {
                    var box = document.getElementById('pdfdiv');
                    if (response.responseText) {
                        var json = Ext.decode(response.responseText);
                        var str = '<embed id="pdf" width="100%" height="100%" src="' + json.src + '"></embed>';
                        box.innerHTML = str;
                    }
                }
            });
        }

        function init_mergeordelete() {
            var field_CODE_w = Ext.create('Ext.form.field.Text', {
                id: 'field_CODE_w',
                name: 'CODE_w',
                fieldLabel: '订单编号',
                labelAlign: "right",
                readOnly: true
            });
            var cbg_files = Ext.create('Ext.form.CheckboxGroup', {
                id: 'cbg_files',
                fieldLabel: '订单文件',
                labelAlign: "right",
                columnWidth: 1,
                columns: 8,
                margin: 10,
                items: []
            });
            
            var w_formpanel = Ext.create('Ext.form.Panel', {
                region: 'center',
                items: [
                    { layout: 'column',margin:5, border: 0, items: [field_CODE_w] },
                    { layout: 'column', height: 52, border: 0, items: [cbg_files ] }
                ]
            });
            
            win_mergeordelete = Ext.create("Ext.window.Window", {
                title: "文件合并&删除",
                width: 800,
                height: 250,
                closeAction: 'hide',
                layout: "border",
                modal: true,
                items: [w_formpanel],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '<i class="iconfont">&#xe66c;</i>文件合并', id: 'btn_merge_w', handler: function () {                            
                            if (Ext.getCmp('cbg_files').getChecked().length <= 1) {
                                Ext.MessageBox.alert('提示', '请选择需要合并的文件！');
                                return;
                            }
                            Ext.Ajax.request({
                                url: 'PdfView.aspx?action=merge&ordercode=' + ordercode + "&fileids=" + cbg_files.getValue().cbg + "&userid=" + userid,
                                success: function (response, opts) {
                                    var json = Ext.decode(response.responseText);
                                    if (json.success) {
                                        Ext.MessageBox.alert('提示', '文件合并成功！', function () {
                                            win_mergeordelete.close();
                                            reloadform();
                                        })
                                    }
                                }
                            });
                        }
                    },
                    {
                        text: '<i class="iconfont">&#xe6d3;</i>文件删除', id: 'btn_delete_w', handler: function () {                           
                            
                            if (Ext.getCmp('cbg_files').getChecked().length <= 0) {
                                Ext.MessageBox.alert('提示', '请选择删除的文件！');
                                return;
                            }

                            Ext.MessageBox.show({
                                title: '提示',
                                closable: false,
                                msg: '确定要删除吗？',
                                buttons: Ext.MessageBox.YESNO,
                                icon: Ext.MessageBox.QUESTION,
                                defaultFocus: 2,
                                fn: function (btn) {
                                    if (btn == 'yes') {
                                        Ext.Ajax.request({
                                            url: 'PdfView.aspx?action=delete&ordercode=' + ordercode + "&fileids=" + cbg_files.getValue().cbg + "&userid=" + userid,
                                            success: function (response, opts) {
                                                var json = Ext.decode(response.responseText);
                                                if (json.success) {
                                                    Ext.MessageBox.alert('提示', '删除成功！', function () {
                                                        win_mergeordelete.close();
                                                        reloadform();
                                                    })
                                                } else {
                                                    Ext.MessageBox.alert('提示', '删除失败，错误信息:' + json.error);
                                                }

                                            }
                                        })
                                    }
                                }
                            });

                            /*Ext.MessageBox.confirm('提示', '确定要删除吗？', function (btn) {
                                if (btn == 'yes') {
                                    Ext.Ajax.request({
                                        url: 'PdfView.aspx?action=delete&ordercode=' + ordercode + "&fileids=" + cbg_files.getValue().cbg + "&userid=" + userid,
                                        success: function (response, opts) {
                                            var json = Ext.decode(response.responseText);
                                            if (json.success) {
                                                Ext.MessageBox.alert('提示', '删除成功！', function () {
                                                    win_mergeordelete.close();
                                                    reloadform();
                                                })
                                            } else {
                                                Ext.MessageBox.alert('提示', '删除失败，错误信息:' + json.error);
                                            }
                                           
                                        }
                                    })
                                }
                            });*/
                        
                        }
                    }
                ],
                listeners: {
                    "show": function() {
                        loadattach();
                    },
                    "close": function () {
                        reloadform(); 
                    } 
                }
            });
        }

        function loadattach() {
            Ext.Ajax.request({
                url: "PdfView.aspx",
                params: { ordercode: ordercode, action: 'loadattach' },
                success: function (response, option) {
                    var json = Ext.decode(response.responseText);

                    Ext.getCmp('cbg_files').removeAll(); Ext.getCmp('cbg_files').reset();
                    for (var i = 0; i < json.filedata.length; i++) {
                        Ext.getCmp('cbg_files').insert(Ext.getCmp('cbg_files').items.length, {
                            boxLabel: '订单文件' + (i + 1), name: 'cbg', inputValue: json.filedata[i].ID, listeners: {
                                change: function (cb, newValue, oldValue, eOpts) {
                                    fileid = cb.inputValue;
                                    for (var j = 0; j < Ext.getCmp('cbg_files').items.length; j++) {
                                        Ext.getCmp('cbg_files').getComponent(j).removeCls('cbg-font-color');
                                    }
                                    cb.addCls('cbg-font-color');
                                }
                            }
                        });
                    }
                    if (Ext.getCmp('field_filestatus').getValue() == '已拆分') {
                        Ext.getCmp('btn_merge_w').setDisabled(true); 
                        Ext.getCmp('btn_delete_w').setDisabled(true);
                    } else {
                        Ext.getCmp('btn_merge_w').setDisabled(false);
                        Ext.getCmp('btn_delete_w').setDisabled(false);
                    }
                    Ext.getCmp("field_CODE_w").setValue(ordercode);
                }
            });
        }

        function reloadform() {
           if (Ext.getCmp('field_file').items.length > 1) {
                fileid = "";
                document.getElementById('pdfdiv').innerHTML = '<embed  id="pdf" width="100%" height="100%" src=""></embed>';
                //清除追加的button按钮
                var times = toolbar.items.length
                for (var i = 3; i < times; i++) {
                    var btn = toolbar.getComponent(3);//移除了第4个元素后，后面的元素会自动填充到第4的位置
                    if (btn) {
                        toolbar.remove(btn);
                    }
                }
                gridpanel.getStore().removeAll();
                gridpanel.reconfigure(store, []);
                
            }
            iniform(); pdfview();

            Ext.getCmp('panel_file').show();
        }

    </script>
</head>
<body>
</body>
</html>
