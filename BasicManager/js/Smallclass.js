
function init_search_smallclass() {

    var txtCODE = Ext.create('Ext.form.field.Text', { id: 'CODE_S_smallclass', name: 'CODE_S_smallclass', fieldLabel: '所属小类代码' });
    var txtCNNAME = Ext.create('Ext.form.field.Text', { id: 'CNNAME_S_smallclass', name: 'CNNAME_S_smallclass', fieldLabel: '所属小类名称' });

    var toolbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [
            { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { addCustomer_Win2(""); } }
            , { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入', width: 80, handler: function () { importfile2('add_smallclass'); } }
            , '->'
            , { text: '<span class="icon iconfont">&#xe60b;</span>&nbsp;查 询', width: 80, handler: function () { Ext.getCmp("pgbar_smallclass").moveFirst(); } }
            , { text: '<span class="icon iconfont">&#xe633;</span>&nbsp;重 置', width: 80, handler: function () { reset2(); } }
        ]
    });

    formpanel_search_smallclass = Ext.create('Ext.form.Panel', {
        id: 'formpanel_search_smallclass',
        region: 'north',
        border: 0,
        bbar: toolbar,
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.25,
            labelWidth: 95
        },
        items: [
            { layout: 'column', border: 0, items: [txtCODE, txtCNNAME] }

        ]
    });
}

function gridbind_smallclass() {
    var store_customer = Ext.create('Ext.data.JsonStore',
        {
            fields: [
                'CODE',
                'NAME',
                'CREATEMANNAME',
                'CREATETIME',
                'ID',
                'CHAPTERCODE',
                'CHAPTERNAME',
                'TYPENAME'
            ],
            pageSize: 20,
            proxy: {
                type: 'ajax',
                url: 'Decl_HSClass.aspx?action=loaddatasmallclass',
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
                        Ext.getCmp('formpanel_search_smallclass').getForm().getValues();
                }
            }
        });
    var pgbar_smallclass = Ext.create('Ext.toolbar.Paging',
        {
            id: 'pgbar_smallclass',
            displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
            store: store_customer,
            displayInfo: true
        });
    gridpanel_smallclass = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel_smallclass',
        height: 750,
        region: 'center',
        store: store_customer,
        selModel: { selType: 'checkboxmodel' },
        bbar: pgbar_smallclass,
        columns: [
            { xtype: 'rownumberer', width: 35 },
            { header: '所属小类编码', dataIndex: 'CODE', width: 100 },
            { header: '所属小类名称', dataIndex: 'NAME', width: 600 },
            { header: '所属章节名称', dataIndex: 'CHAPTERNAME', width: 300 },
            { header: '所属类别名称', dataIndex: 'TYPENAME', width: 300 },
            { header: '维护人', dataIndex: 'CREATEMANNAME', width: 200 },
            { header: '维护时间', dataIndex: 'CREATETIME', width: 200 }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                editCustomer();
            }
        },
        viewConfig: {
            enableTextSelection: true
        }
    });
}

function reset2() {
    Ext.each(Ext.getCmp('formpanel_search_smallclass').getForm().getFields().items,
        function (field) {
            field.reset();
        });
}


function form_ini_win2() {
    var field_code = Ext.create('Ext.form.field.Text', {
        id: 'CODE',
        name: 'CODE',
        fieldLabel: '所属分类代码',
        flex: .5,
        allowBlank: false,
        blankText: '所属分类代码不可为空!',
        listeners: {
            blur: function () {
                
                Ext.Ajax.request({
                    url: 'Decl_HSClass.aspx?table=smallclass',
                    params: { action: 'hsclass',code:Ext.getCmp('CODE').getValue().substr(0,2) },
                    type: 'Post',
                    success: function (response, option) {

                        var commondata = Ext.decode(response.responseText);
                        if (commondata.length > 0) {
                            console.log(commondata[0]["CODEADDNAME"]);
                            Ext.getCmp('CHAPTERCODE').setValue(commondata[0]["CODEADDNAME"]);
                            Ext.getCmp('TYPECODE').setValue(commondata[0]["TYPECODEADDNAME"]);
                        } else {
                            Ext.MessageBox.alert("提示框","无法找到正确的章节");
                        }


                    }
                });
            }
        }
    });
    var createtime = Ext.create('Ext.form.field.Date',
        {
            id: 'CREATETIME',
            name: 'CREATETIME',
            format: 'Y-m-d',
            fieldLabel: '维护时间',
            flex: .5

        });
    var CreatemanName = Ext.create('Ext.form.field.Text', {
        id: 'CREATEMANNAME',
        name: 'CREATEMANNAME',
        fieldLabel: '维护人',
        readOnly: true,
        value: username
    });
    var field_name = Ext.create('Ext.form.field.Text', {
        id: 'NAME',
        name: 'NAME',
        fieldLabel: '所属分类名称',
        flex: .5,
        allowBlank: false,
        blankText: '所属分类名称不可为空!',
    });


    var chapter = Ext.create('Ext.form.field.Text', {
        id: 'CHAPTERCODE',
        name: 'CHAPTERCODE',
        fieldLabel: '所属章节名称',
        flex: .5
    });

    var type = Ext.create('Ext.form.field.Text', {
        id: 'TYPECODE',
        name: 'TYPECODE',
        fieldLabel: '所属类别名称',
        flex: .5
    });


    var formpanel_Win = Ext.create('Ext.form.Panel', {
        id: 'formpanel_Win',
        minHeight: 170,
        border: 0,
        buttonAlign: 'center',
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .5,
            labelAlign: 'right',
            labelSeparator: '',
            msgTarget: 'under'
        },
        items: [
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_code, field_name] },
            { layout: 'column', height: 42, border: 0, items: [createtime, CreatemanName] },
            { layout: 'column', height: 42, border: 0, items: [chapter,type] }
        ],
        buttons: [{

            text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {
                if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {

                    return;
                }

                var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                Ext.Ajax.request({
                    url: 'Decl_HSClass.aspx',
                    type: 'Post',
                    params: { action: 'save_smallclass', formdata: formdata },
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

                            Ext.Msg.alert('提示', "保存失败:章节代码重复！", function () {
                                Ext.getCmp("pgbar").moveFirst(); Ext.getCmp("win_d").close();
                            });
                        }
                    }
                });

            }
        }]
    });


}


function addCustomer_Win2(ID, formdata) {


    form_ini_win2();
    var win = Ext.create("Ext.window.Window", {
        id: "win_d",
        title: '商品HS类型',
        width: 1200,
        height: 250,
        modal: true,
        items: [Ext.getCmp('formpanel_Win')]
    });
    win.show();
}


function importfile2(action, param) {
    if (action == "add_smallclass") {
        importexcel2(action, "base_declhsclass");
    }

}
function importexcel2(action, param) {

    var radio_module = Ext.create('Ext.form.RadioGroup', {
        name: "RADIO_MODULE", id: "RADIO_MODULE", fieldLabel: '模板类型',
        items: [
            { boxLabel: "<a href='/FileUpload/base_declhsclass.xls'><b>模板</b></a>", name: 'RADIO_MODULE', inputValue: '1', checked: true }
        ]
    });


    var uploadfile = Ext.create('Ext.form.field.File', {
        id: 'UPLOADFILE', name: 'UPLOADFILE', fieldLabel: '导入数据', labelAlign: 'right', msgTarget: 'under'
        , anchor: '90%', buttonText: '浏览文件', regex: /.*(.xls|.xlsx)$/, regexText: '只能上传xls,xlsx文件'
        , allowBlank: false, blankText: '文件不能为空!'
    });


    var CreatemanName = Ext.create('Ext.form.field.Text', {
        id: 'CREATEMANNAME',
        name: 'CREATEMANNAME',
        fieldLabel: '维护人',
        readOnly: true,
        flex: .5,
        margin: '0 5 10 122',
        value: username
    });

    var formpanel_upload1 = Ext.create('Ext.form.Panel', {
        id: 'formpanel_upload1', height: 180,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            labelAlign: 'right',
            labelSeparator: '',
            msgTarget: 'under'
        },
        buttonAlign: 'center',

        items: [
            { layout: 'column', height: 42, border: 0, items: [radio_module, CreatemanName] },
            { layout: 'column', height: 42, border: 0, items: [uploadfile] }
        ],
        buttons: [{
            text: '确认上传',
            handler: function () {
                if (Ext.getCmp('formpanel_upload1').getForm().isValid()) {

                    var formdata = Ext.encode(Ext.getCmp('formpanel_upload1').getForm().getValues());

                    Ext.getCmp('formpanel_upload1').getForm().submit({
                        type: 'Post',
                        url: 'Decl_HSClass.aspx',
                        params: { formdata: formdata, action: action, table: param },
                        waitMsg: '数据导入中...',
                        success: function (form, action) {
                            console.log(action.result);
                            var data = action.result.success;
                            var reg = /,$/gi;
                            idStr = data.replace(reg, "!");
                            Ext.Msg.alert('提示', idStr, function () {
                                //pgbar.moveFirst();
                                Ext.getCmp('pgbar').moveFirst();
                                Ext.getCmp('win_upload1').close();
                            });
                        },
                        failure: function (form, action) {//失败要做的事情 
                            Ext.MessageBox.alert("提示", "保存失败", function () { });
                        }
                    });

                }
            }
        }]
    });

    var win_upload2 = Ext.create("Ext.window.Window", {
        id: "win_upload1",
        title: '导入',
        width: 600,
        height: 240,
        modal: true,
        items: [Ext.getCmp('formpanel_upload1')]
    });
    Ext.getCmp('CREATEMANNAME').setValue(username);
    win_upload2.show();
}

