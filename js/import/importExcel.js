var excelName;
function onItemUpload(name) {
    excelName = name;
    //alert(excelName);
    var FileRname = new Ext.form.TextField({
        name: 'FileRname',
        fieldLabel: '文件名',
        allowBlank: false,
        emptyText: '发布用于显示的文件名',
        anchor: '95%'
    });
    var AddfileForm = new Ext.FormPanel(
    {
        name: 'AddfileForm',
        frame: true,
        labelWidth: 90,
        url: '/uploadFile.aspx',
        fileUpload: true,
        width: 420,
        height:340,
        autoDestroy: true,
        bodyStyle: 'padding:0px 10px 0;',
        items: [{
            xtype: 'filefield',
            emptyText: '选择上传文件',
            fieldLabel: '文件',
            name: 'upfile',
            buttonText: '...',
            anchor: '95%',
            buttonCfg: {
                iconCls: 'icon_upfile'
            },
            listeners: {
                'fileselected': function (fb, v) {
                    var temp = v.replace(
                            /^.*(\.[^\.\?]*)\??.*$/, '$1');
                    var temp1 = temp.toLocaleLowerCase();
                    if (allowfiletype.indexOf(temp1) == -1) {
                        Ext.Msg.alert("错误", "不允许选择该类型文件，请重新选择！");
                        fb.setValue("");
                        FileRname.setValue("");
                    } else {
                        FileRname.setValue(v.replace(/^.+?\\([^\\]+?)(\.[^\.\\]*?)?$/gi, "$1"));
                    }
                }
            }
        }]
    });
    var AddfileWin = new Ext.Window(
    {
        name: 'AddfileWin',
        width: '450',
        layout: 'fit',
        //layout: {
        //    type: 'vbox',
        //    align: 'center'
        //},
        closeAction: 'close',
        title: '上传文件',
        buttonAlign: 'center',
        //resizable: false,
        modal: true,
        autoDestroy: true,
        items: AddfileForm,
        buttons: [{
            text: '保存',
            handler: function () {
                if (AddfileForm.getForm().isValid()) {
                    Ext.MessageBox.show({
                        title: '请稍等...',
                        msg: '文件上传中...',
                        progressText: '',
                        width: 300,
                        progress: true,
                        closable: false,
                        animEl: 'loding'
                    });
                    AddfileForm.getForm().submit(
                        {
                            params: {
                                tbl: excelName
                            },
                            success: function(form, action) {
                                var Result = action.result.flag;
                                if (Result != 0) {

                                    Ext.MessageBox.alert("提示", action.result.message);
                                    AddfileWin.close();
                                    var tabPanel = Ext.getCmp('centerTab');
                                    //SJSTgrid.getStore().reload();
                                    //tabPanel.setActiveTab(1);
                                } else if (Result == 0) {
                                    Ext.MessageBox.alert("提示", action.result.message);
                                    ds.load({
                                        params: {
                                            start: start,
                                            limit: limit
                                        }
                                    });
                                    AddfileForm.getForm().reset();
                                }
                            },
                            failure: function(form, action) {
                                Ext.MessageBox.alert("提示",
                                    "服务器请求错误,请稍后再试！");
                            }
                        });
                }
            }
        }, {
            text: '重置',
            handler: function () {
                AddfileForm.getForm().reset();
            }
        }, {
            text: '关闭',
            handler: function () {
                AddfileWin.close();
            }
        }]
    });
    AddfileWin.show();
}


