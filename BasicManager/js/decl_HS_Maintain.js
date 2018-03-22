var data2 = [];
var formadata1 = [];
var util = {


    //维护界面的大小
    addMaintain_Win: function (ID, formdata) {
        formadata1 = formdata;
        console.log(formadata1);
        console.log(formadata1["ID"]);
        Ext.Ajax.request({
            url: 'decl_HS.aspx',
            params: { action: 'getUnit' },
            type: 'Post',
            success: function (response, option) {
                
                var commondata = Ext.decode(response.responseText);
                data2 = commondata;
                //console.log(data2);

                util.Maintain_init_search(ID);
                util.Maintain_gridbind(ID);
                var win = Ext.create("Ext.window.Window",
                    {
                        id: "win_d",
                        title: 'HS代码维护',
                        width: 1500,
                        height: 700,
                        modal: true,
                        items: [Ext.getCmp('formpanel_search2'), Ext.getCmp('gridpanel2')]
                    });
                win.show();
            }
        });


        
    },
    Maintain_init_search: function (ID) {
        var HsCodeSearch = Ext.create('Ext.form.field.Text', { id: 'HsCodeSearch', name: 'HsCodeSearch', fieldLabel: 'HS编码' });

        var HsNameSearch = Ext.create('Ext.form.field.Text',
            { id: 'HsNameSearch', name: 'HsNameSearch', fieldLabel: '商品名称' });

        var store_ENABLED_S2 = Ext.create('Ext.data.JsonStore',
            {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
            });

        var combo_ENABLED_S2 = Ext.create('Ext.form.field.ComboBox',
            {
                id: 'combo_ENABLED_S2',
                name: 'ENABLED_S2',
                store: store_ENABLED_S2,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '是否启用',
                displayField: 'NAME',
                valueField: 'CODE',

            });

        var toolbar2 = Ext.create('Ext.toolbar.Toolbar',
            {
                items: [
                    {
                        text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增',
                        handler: function () { util.addCustomer_Win(ID, "", ""); }
                    },
                    {
                        text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改',
                        width: 80,
                        handler: function () { util.editCustomer(ID); }
                    },
                    //{
                    //    text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除',
                    //    width: 80,
                    //    handler: function() { del(); }
                    //},
                    {
                        text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入',
                        width: 80,
                        handler: function () { util.importfile('add', ID); }
                    },
                    {
                        text: '<span class="icon iconfont">&#xe625;</span>&nbsp;导 出',
                        handler: function () { util.exportdata(ID); }
                    }, '->',
                    {
                        text: '<span class="icon iconfont">&#xe60b;</span>&nbsp;查 询',
                        width: 80,
                        handler: function () { Ext.getCmp("pgbar2").moveFirst(); }
                    },
                    {
                        text: '<span class="icon iconfont">&#xe633;</span>&nbsp;重 置',
                        width: 80,
                        handler: function () { util.Maintain_reset(); }
                    }
                ]
            });

        var formpanel_search2 = Ext.create('Ext.form.Panel',
            {
                id: 'formpanel_search2',
                region: 'north',
                border: 0,
                bbar: toolbar2,
                fieldDefaults: {
                    margin: '5',
                    columnWidth: 0.25,
                    labelWidth: 70
                },
                items: [
                    { layout: 'column', border: 0, items: [HsCodeSearch, HsNameSearch, combo_ENABLED_S2] }
                ]
            });

    },

    //维护界面数据加载
    Maintain_gridbind: function (ID) {

        var CiqDataBase2 = Ext.create('Ext.data.JsonStore',
            {
                fields: [
                    'HSCODE', 'EXTRACODE', 'NAME', 'ELEMENTS','LEGALUNITNAME','LEGALUNIT','SECONDUNIT','SECONDUNITNAME','TYPENAME','CHAPTERNAME','CLASSCODE','CLASSNAME','CUSTOMREGULATORY','INSPECTIONREGULATORY','GENERALRATE','FAVORABLERATE','VATRATE','EXPORTREBATRATE','TOPPRICE','LOWPRICE','AGREETAXFILE','YEARNAME','SPECIALMARK',
                    'ENABLED', 'STARTDATE', 'CREATEMANNAME', 'CREATEDATE', 'STOPMANNAME', 'ENDDATE', 'REMARK', 'ID'
                ],
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: 'decl_HS.aspx?action=MaintainloadData&id=' + ID + '',
                    reader: {
                        root: 'rows',
                        type: 'json',
                        totalProperty: 'total'
                    }
                },
                autoLoad: true,
                listeners: {
                    beforeload: function (store, options) {
                        CiqDataBase2.getProxy().extraParams = Ext.getCmp('formpanel_search2').getForm().getValues();
                    }
                }
            });
        var pgbar2 = Ext.create('Ext.toolbar.Paging',
            {
                id: 'pgbar2',
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: CiqDataBase2,
                displayInfo: true
            });

        var gridpanel2 = Ext.create('Ext.grid.Panel',
            {
                id: 'gridpanel2',
                height: 590,
                region: 'center',
                store: CiqDataBase2,
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar2,
                columns: [
                    { xtype: 'rownumberer', width: 35 },
                    { header: 'HS编码', dataIndex: 'HSCODE', width: 150 },
                    { header: '附加码', dataIndex: 'EXTRACODE', width: 100},
                    { header: '商品名称', dataIndex: 'NAME', width: 150 },
                    { header: '申报要素', dataIndex: 'ELEMENTS', width: 100 },
                    { header: '法定单位', dataIndex: 'LEGALUNITNAME', width: 100 },
                    { header: '第二单位', dataIndex: 'SECONDUNITNAME', width: 100 },
                    { header: '所属类别', dataIndex: 'TYPENAME', width: 100 },
                    { header: '所属章节', dataIndex: 'CHAPTERNAME', width: 100 },
                    { header: '所属小类', dataIndex: 'CLASSNAME', width: 100 },
                    { header: '海关监管', dataIndex: 'CUSTOMREGULATORY', width: 100 },
                    { header: '商检监管', dataIndex: 'INSPECTIONREGULATORY', width: 100 },
                    { header: '一般税率', dataIndex: 'GENERALRATE', width: 100 },
                    { header: '最惠税率', dataIndex: 'FAVORABLERATE', width: 100 },
                    { header: '增值税率', dataIndex: 'VATRATE', width: 100 },
                    { header: '出口退税率', dataIndex: 'EXPORTREBATRATE', width: 100 },
                    { header: '最高价格', dataIndex: 'TOPPRICE', width: 100 },
                    { header: '最低价格', dataIndex: 'LOWPRICE', width: 100 },
                    { header: '协定税文件', dataIndex: 'AGREETAXFILE', width: 100 },
                    { header: '代码库', dataIndex: 'YEARNAME', width: 100 },
                    { header: '特殊标志', dataIndex: 'SPECIALMARK', width: 100 },

                    { header: '启用情况', dataIndex: 'ENABLED', width: 100, renderer: util.gridrender },
                    { header: '启用时间', dataIndex: 'STARTDATE', width: 100 },
                    { header: '维护人', dataIndex: 'CREATEMANNAME', width: 100 },
                    { header: '维护时间', dataIndex: 'CREATEDATE', width: 100 },
                    { header: '停用人', dataIndex: 'STOPMANNAME', width: 100 },
                    { header: '停用时间', dataIndex: 'ENDDATE', width: 100 },
                    { header: '备注', dataIndex: 'REMARK', width: 270 },

                    { header: 'ID', dataIndex: 'ID', width: 200, hidden: true }
                ],
                listeners:
                {
                    'itemdblclick': function (view, record, item, index, e) {
                        util.editCustomer();
                    }
                },
                viewConfig: {
                    enableTextSelection: true
                }
            });

    },

    //switch...case
    gridrender: function (value, cellmeta, record, rowIndex, columnIndex, stroe) {
        var dataindex = cellmeta.column.dataIndex;
        var str = "";
        switch (dataindex) {
            case "ENABLED":
                str = value == "1" ? '<span class="icon iconfont" style="font-size:12px;color:blue;">&#xe628;</span>' : '<span class="icon iconfont" style="font-size:12px;color:red;">&#xe634;</span>';
                break;
        }
        return str;
    },

    //HS代码重置方法
    Maintain_reset: function () {
        Ext.each(Ext.getCmp('formpanel_search2').getForm().getFields().items, function (field) {
            field.reset();
        });
    },

    form_ini_win: function (ID) {
        var field_ID = Ext.create('Ext.form.field.Hidden', {
            id: 'ID',
            name: 'ID'
        });
        var field_HS = Ext.create('Ext.form.field.Text', {
            id: 'HSCODE',
            name: 'HSCODE',
            fieldLabel: '商品编码', flex: .5,
            allowBlank: false,
            blankText: '商品编码不可为空!'
        });
        var field_EXTRACODE = Ext.create('Ext.form.field.Text', {
            id: 'EXTRACODE',
            name: 'EXTRACODE',
            fieldLabel: '附加码', flex: .5,
            allowBlank: false,
            blankText: '商品编码不可为空!'
        });

        var field_HSNAME = Ext.create('Ext.form.field.Text', {
            id: 'NAME',
            name: 'NAME',
            fieldLabel: '商品名称', flex: .5,
            allowBlank: false,
            blankText: '商品名称不可为空!'
        });

        //法定单位
        var LEGALUNITNAME_ENABLED = Ext.create('Ext.data.JsonStore', {
            fields: ['CODE', 'NAME'],
            data: data2
        });
        var LEGALUNITNAME_combo_ENABLED = Ext.create('Ext.form.field.ComboBox', {
            id: 'LEGALUNITNAME_combo_ENABLED',
            name: 'LEGALUNIT',
            store: LEGALUNITNAME_ENABLED,
            queryMode: 'local',
            anyMatch: true,
            fieldLabel: '法定单位', flex: .5,
            displayField: 'NAME',
            valueField: 'CODE',
            allowBlank: false,
            blankText: '法定单位不能为空!'
        });
        //第二单位
        var SECONDUNITNAME_ENABLED = Ext.create('Ext.data.JsonStore', {
            fields: ['CODE', 'NAME'],
            data: data2
        });
        var SECONDUNITNAME_combo_ENABLED = Ext.create('Ext.form.field.ComboBox', {
            id: 'SECONDUNITNAME_combo_ENABLED',
            name: 'SECONDUNIT',
            store: SECONDUNITNAME_ENABLED,
            queryMode: 'local',
            anyMatch: true,
            fieldLabel: '第二单位', flex: .5,
            displayField: 'NAME',
            valueField: 'CODE',
            allowBlank: false,
            blankText: '第二单位不能为空!'
        });



        var field_CUSTOMREGULATORY = Ext.create('Ext.form.field.Text', {
            id: 'CUSTOMREGULATORY',
            name: 'CUSTOMREGULATORY',
            fieldLabel: '海关监管', flex: .5
        });

        var field_INSPECTIONREGULATORY = Ext.create('Ext.form.field.Text', {
            id: 'INSPECTIONREGULATORY',
            name: 'INSPECTIONREGULATORY',
            fieldLabel: '国检监管', flex: .5
        });

        var field_GENERALRATE = Ext.create('Ext.form.field.Text', {
            id: 'GENERALRATE',
            name: 'GENERALRATE',
            fieldLabel: '一般税率', flex: .5
        });

        var field_FAVORABLERATE = Ext.create('Ext.form.field.Text', {
            id: 'FAVORABLERATE',
            name: 'FAVORABLERATE',
            fieldLabel: '最惠税率', flex: .5
        });

        var field_VATRATE = Ext.create('Ext.form.field.Text', {
            id: 'VATRATE',
            name: 'VATRATE',
            fieldLabel: '增值税率', flex: .5
        });

        var field_EXPORTREBATRATE = Ext.create('Ext.form.field.Text', {
            id: 'EXPORTREBATRATE',
            name: 'EXPORTREBATRATE',
            fieldLabel: '出口退税率', flex: .5
        });

        var field_TEMPRATE = Ext.create('Ext.form.field.Text', {
            id: 'TEMPRATE',
            name: 'TEMPRATE',
            fieldLabel: '暂定税率', flex: .5
        });

        var field_CONSUMERATE = Ext.create('Ext.form.field.Text', {
            id: 'CONSUMERATE',
            name: 'CONSUMERATE',
            fieldLabel: '消费税率', flex: .5
        });

        var field_EXPORTRATE = Ext.create('Ext.form.field.Text', {
            id: 'EXPORTRATE',
            name: 'EXPORTRATE',
            fieldLabel: '出口税率', flex: .5
        });

        var field_TOPPRICR = Ext.create('Ext.form.field.Text', {
            id: 'TOPPRICE',
            name: 'TOPPRICE',
            fieldLabel: '最高价格', flex: .5
        });

        var field_LOWPRICE = Ext.create('Ext.form.field.Text', {
            id: 'LOWPRICE',
            name: 'LOWPRICE',
            fieldLabel: '最低价格', flex: .5
        });

        var field_YEARNAME = Ext.create('Ext.form.field.Text', {
            id: 'YEARNAME',
            name: 'YEARNAME',
            fieldLabel: '代码库', flex: .5,
            value: formadata1["NAME"]
            
        });

        var field_CUSTOMAREA = Ext.create('Ext.form.field.Text', {
            id: 'CUSTOMAREA',
            name: 'CUSTOMAREA',
            fieldLabel: '关区', flex: .5,
            value: formadata1["CUSTOMAREANAME"]
        });

        var filed_SPECIALMARK = Ext.create('Ext.form.field.Text', {
            id: 'SPECIALMARK',
            name: 'SPECIALMARK',
            fieldLabel: '特殊标志', flex: .5
        });

        var filed_ELEMENTS = Ext.create('Ext.form.field.Text', {
            id: 'ELEMENTS',
            name: 'ELEMENTS',
            fieldLabel: '申报要素', flex: .5
        });



        var start_date = Ext.create('Ext.form.field.Date',
            {
                id: 'STARTDATE',
                name: 'STARTDATE',
                format: 'Y-m-d',
                fieldLabel: '启用日期',
                flex: .5

            });
        var end_date = Ext.create('Ext.form.field.Date',
            {
                id: 'ENDDATE',
                name: 'ENDDATE',
                format: 'Y-m-d',
                fieldLabel: '停用日期',
                flex: .5
            });

        var CreatemanName = Ext.create('Ext.form.field.Text', {
            id: 'CREATEMANNAME',
            name: 'CREATEMANNAME',
            fieldLabel: '维护人',
            readOnly: true
        });

        var field_REMARK = Ext.create('Ext.form.field.Text', {
            id: 'REMARK',
            name: 'REMARK',
            fieldLabel: '备注'
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
            fieldLabel: '是否启用', flex: .5,
            displayField: 'NAME',
            valueField: 'CODE',
            value: 1,
            allowBlank: false,
            blankText: '是否启用不能为空!'
        });

        //修改原因输入框
        var change_reason2 = Ext.create('Ext.form.field.Text', {
            id: 'REASON',
            name: 'REASON',
            fieldLabel: '修改原因',
            hidden: true
        });
        //新增布局
        var formpanel_Win = Ext.create('Ext.form.Panel', {
            id: 'formpanel_Win',
            minHeight: 170,
            border: 0,
            buttonAlign: 'center',
            fieldDefaults: {
                margin: '0 5 10 0',
                labelWidth: 80,
                columnWidth: 0.25,
                labelAlign: 'right',
                labelSeparator: '',
                msgTarget: 'under'
            },
            items: [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_HS, field_EXTRACODE, field_HSNAME, LEGALUNITNAME_combo_ENABLED] },
                //{ layout: 'column', height: 42, border: 0, items: [field_HSNAME, LEGALUNITNAME_combo_ENABLED] },
                { layout: 'column', height: 42, border: 0, items: [SECONDUNITNAME_combo_ENABLED, field_CUSTOMREGULATORY, field_INSPECTIONREGULATORY, field_GENERALRATE] },
                //{ layout: 'column', height: 42, border: 0, items: [field_INSPECTIONREGULATORY, field_GENERALRATE] },
                { layout: 'column', height: 42, border: 0, items: [field_FAVORABLERATE, field_VATRATE, field_EXPORTREBATRATE, field_TEMPRATE] },
                //{ layout: 'column', height: 42, border: 0, items: [field_EXPORTREBATRATE, field_TEMPRATE] },
                { layout: 'column', height: 42, border: 0, items: [field_CONSUMERATE, field_EXPORTRATE, field_TOPPRICR, field_LOWPRICE] },
                //{ layout: 'column', height: 42, border: 0, items: [field_TOPPRICR, field_LOWPRICE] },
                { layout: 'column', height: 42, border: 0, items: [field_YEARNAME, field_CUSTOMAREA, filed_SPECIALMARK, filed_ELEMENTS] },
                //{ layout: 'column', height: 42, border: 0, items: [filed_SPECIALMARK, filed_ELEMENTS] },
                { layout: 'column', height: 42, border: 0, items: [combo_ENABLED, start_date, end_date, CreatemanName] },
                //{ layout: 'column', height: 42, border: 0, items: [end_date, CreatemanName] },
                { layout: 'column', height: 42, border: 0, items: [field_REMARK, change_reason2] },
                field_ID
            ],
            buttons: [{

                text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {

                    if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                        return;
                    }

                    var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());

                    Ext.Ajax.request({
                        url: 'decl_HS.aspx?ID=' + ID + '',
                        type: 'Post',
                        params: { action: 'MaintainSave', formdata: formdata },
                        success: function (response, option) {
                            var data = Ext.decode(response.responseText);
                            if (data.success == "4") {
                                Ext.Msg.alert('提示',
                                    "保存失败:HS代码重复!",
                                    function () {
                                        Ext.getCmp("pgbar2").moveFirst();
                                        Ext.getCmp("win_d2").close();
                                    });
                            } else {
                                Ext.Msg.alert('提示', "保存成功", function () {
                                    Ext.getCmp("pgbar2").moveFirst(); Ext.getCmp("win_d2").close();
                                });
                            }


                        }
                    });
                }
            }]
        });


    },
    addCustomer_Win: function (ID, formdata, ID2) {
        util.form_ini_win(ID);
        Ext.getCmp('CREATEMANNAME').setValue(username);
        console.log(ID);
        if (ID2 != "") {

            Ext.getCmp('REASON').hidden = false;
            Ext.getCmp('REASON').allowBlank = false;
            Ext.getCmp('REASON').blankText = '修改原因不可为空!';
            //默认值的
            Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
        }

        var win = Ext.create("Ext.window.Window", {
            id: "win_d2",
            title: 'HS编码',
            width: 1200,
            height: 400,
            modal: true,
            items: [Ext.getCmp('formpanel_Win')]
        });
        win.show();
    },
    editCustomer: function (ID) {
        var recs = Ext.getCmp('gridpanel2').getSelectionModel().getSelection();

        if (recs.length == 0) {
            Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
            return;
        }

        util.addCustomer_Win(ID, recs[0].data, recs[0].get("ID"));
        console.log(recs[0].data);
    },

    exportdata: function (ID) {
        var HsCodeSearch = Ext.getCmp('HsCodeSearch').getValue();
        var HsNameSearch = Ext.getCmp('HsNameSearch').getValue();
        var combo_ENABLED_S2 = Ext.getCmp('combo_ENABLED_S2').getValue();


        var path = 'decl_HS.aspx?action=export&HsCodeSearch=' + HsCodeSearch + '&HsNameSearch=' + HsNameSearch + '&id=' + ID + '&combo_ENABLED_S2=' + combo_ENABLED_S2;
        $('#exportform').attr("action", path).submit();
    },

    importfile: function (action, ID) {
        if (action == "add") {
            util.importexcel(action, ID);
        }
    },
    importexcel: function (action, ID) {
        var radio_module = Ext.create('Ext.form.RadioGroup', {
            name: "RADIO_MODULE", id: "RADIO_MODULE", fieldLabel: '模板类型',
            items: [
                { boxLabel: "<a href='/FileUpload/base_hs.xls'><b>模板</b></a>", name: 'RADIO_MODULE', inputValue: '1', checked: true }
            ]
        });

        var uploadfile = Ext.create('Ext.form.field.File', {
            id: 'UPLOADFILE', name: 'UPLOADFILE', fieldLabel: '导入数据', labelAlign: 'right', msgTarget: 'under'
            , anchor: '90%', buttonText: '浏览文件', regex: /.*(.xls|.xlsx)$/, regexText: '只能上传xls,xlsx文件'
            , allowBlank: false, blankText: '文件不能为空!'
        });

        var start_date = Ext.create('Ext.form.field.Date',
            {
                id: 'STARTDATE',
                name: 'STARTDATE',
                format: 'Y-m-d',
                fieldLabel: '启用日期',
                flex: .5

            });

        var end_date = Ext.create('Ext.form.field.Date',
            {
                id: 'ENDDATE',
                name: 'ENDDATE',
                format: 'Y-m-d',
                fieldLabel: '停用日期',
                flex: .5


            });

        var CreatemanName = Ext.create('Ext.form.field.Text', {
            id: 'CREATEMANNAME',
            name: 'CREATEMANNAME',
            fieldLabel: '维护人',
            readOnly: true,
            flex: .5,
            margin: '0 5 10 122',
        });

        var formpanel_upload = Ext.create('Ext.form.Panel', {
            id: 'formpanel_upload', height: 180,
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
                { layout: 'column', height: 42, border: 0, items: [start_date, end_date] },
                { layout: 'column', height: 42, border: 0, items: [uploadfile] }
            ],
            buttons: [{
                text: '确认上传',
                handler: function () {
                    if (Ext.getCmp('formpanel_upload').getForm().isValid()) {

                        var formdata = Ext.encode(Ext.getCmp('formpanel_upload').getForm().getValues());

                        Ext.getCmp('formpanel_upload').getForm().submit({
                            type: 'Post',
                            url: 'decl_HS.aspx?id=' + ID + '',
                            params: { formdata: formdata, action: action },
                            waitMsg: '数据导入中...',
                            success: function (form, action) {
                                console.log(action.result);
                                var data = action.result.success;
                                var reg = /,$/gi;
                                idStr = data.replace(reg, "!");
                                Ext.Msg.alert('提示', idStr, function () {
                                    Ext.getCmp("pgbar2").moveFirst();
                                    Ext.getCmp('win_upload').close();
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

        var win_upload = Ext.create("Ext.window.Window", {
            id: "win_upload",
            title: 'HS导入',
            width: 600,
            height: 240,
            modal: true,
            items: [Ext.getCmp('formpanel_upload')]
        });
        Ext.getCmp('CREATEMANNAME').setValue(username);
        win_upload.show();

    }

}