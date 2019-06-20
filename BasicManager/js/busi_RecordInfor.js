//成交单位
var unit;
var util = {
    //维护界面的大小
    
    addMaintain_Win: function (ID, formdata) {
        
        Ext.Ajax.request({
            url: 'busi_RecordInfor.aspx',
            params: { action: 'getRecordDetails' },
            type: 'Post',
            success: function(response, option) {
                var commondata = Ext.decode(response.responseText);
                //成交单位
                unit = commondata.UNIT;
            }
        });

        util.Maintain_init_search(ID);
        util.Maintain_Materials(ID);
        util.Maintain_product(ID);
        var tabpanel = new Ext.TabPanel({
            deferredRender: false,
            region: 'center',
            layout: 'border',
            activeTab: 0,           
            items: [
                { title: "料件", items: [gridpanel_category] },
                { title: "成品", items: [gridpanel_product] }
            ]
        });

        var win = Ext.create("Ext.window.Window",
            {
                id: "win_d",
                title: '备案信息维护',
                width: 1500,
                height: 800,
                modal: true,
                items: [Ext.getCmp('formpanel_search2'), tabpanel]
            });
        win.show();
    },
    Maintain_init_search: function(ID) {
        var HSNUMBER = Ext.create('Ext.form.field.Text', { id: 'HSNUMBER', name: 'HSNUMBER', fieldLabel: '项号' });
        var HSCODE = Ext.create('Ext.form.field.Text', { id: 'HSCODE1', name: 'HSCODE1', fieldLabel: 'HS代码' });
        var store_ENABLED_S2 = Ext.create('Ext.data.JsonStore',
            {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
            });

        var combo_ENABLED_S2 = Ext.create('Ext.form.field.ComboBox',
            {
                id: 'ENABLED_S2',
                name: 'ENABLED_S2',
                store: store_ENABLED_S2,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '是否启用',
                displayField: 'NAME',
                valueField: 'CODE'

            });

        var toolbar2 = Ext.create('Ext.toolbar.Toolbar',
            {
                items: [
                    {
                        text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增',
                        handler: function () { util.Maintain_add_recorddetails(ID, "", ""); }
                    },
                    {
                        text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改',
                        width: 80,
                        handler: function () { util.Maintain_edit_recorddetails(ID); }
                    },
                    {
                        text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入',
                        width: 80,
                        handler: function () { util.Maintain_importfile('add_recorddetails', ID); }
                    },
                    {
                        text: '<span class="icon iconfont">&#xe625;</span>&nbsp;导 出',
                        handler: function () { util.Maintain_exportdata_recorddetails(ID); }
                    }, '->',
                    {
                        text: '<span class="icon iconfont">&#xe60b;</span>&nbsp;查 询',
                        width: 80,
                        handler: function () { Ext.getCmp("pgbar_category").moveFirst(); Ext.getCmp("pgbar_product").moveFirst(); }
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
                    { layout: 'column', border: 0, items: [HSNUMBER, HSCODE, combo_ENABLED_S2] }
                ]
            });

    },

    Maintain_Materials: function(ID) {
        var store_customer = Ext.create('Ext.data.JsonStore',
            {
                fields: [
                    'ITEMNO',
                    'HSCODE',
                    'ADDITIONALNO',
                    'ITEMNOATTRIBUTE',
                    'COMMODITYNAME',
                    'PARTNO',
                    'SPECIFICATIONSMODEL',
                    'UNITNAME',
                    'UNIT',
                    'VERSION',
                    'ENABLED',
                    'STARTDATE',
                    'CREATEMANNAME',
                    'CREATEDATE',
                    'STOPMANNAME',
                    'ENDDATE',
                    'REMARK',
                    'ID'
                ],
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: 'busi_RecordInfor.aspx?action=loaddatamaterials&recordinfoid='+ID+'',
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
                            Ext.getCmp('formpanel_search2').getForm().getValues();
                    }
                }
            });
        var pgbar_category = Ext.create('Ext.toolbar.Paging',
            {
                id: 'pgbar_category',
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: store_customer,
                displayInfo: true
                
            });
        gridpanel_category = Ext.create('Ext.grid.Panel', {
            id: 'gridpanel_category',
            height: 650,
            region: 'center',
            store: store_customer,
            selModel: { selType: 'checkboxmodel' },
            bbar: pgbar_category,
            
            columns: [
                { xtype: 'rownumberer', width: 35 },
                { header: '项号', dataIndex: 'ITEMNO', width: 100 },
                { header: 'HS编码', dataIndex: 'HSCODE', width: 200 },
                { header: '附加码', dataIndex: 'ADDITIONALNO', width: 100 },
                { header: '项号属性', dataIndex: 'ITEMNOATTRIBUTE', width: 100 },
                { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 100 },
                { header: '料号', dataIndex: 'PARTNO', width: 100 },
                { header: '规格型号', dataIndex: 'SPECIFICATIONSMODEL', width: 100 },
                { header: '成交单位', dataIndex: 'UNIT', width: 100 },
                { header: '版本号', dataIndex: 'VERSION', width: 100 },
                { header: '启用/禁用', dataIndex: 'ENABLED', width: 100, renderer: util.Maintain_gridrender },
                { header: '启用时间', dataIndex: 'STARTDATE', width: 100 },
                { header: '维护人', dataIndex: 'CREATEMANNAME', width: 100 },
                { header: '维护时间', dataIndex: 'CREATEDATE', width: 100 },
                { header: '停用人', dataIndex: 'STOPMANNAME', width: 100 },
                { header: '停用时间', dataIndex: 'ENDDATE', width: 100 },
                { header: '备注', dataIndex: 'REMARK', width: 100 }
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
    },
    Maintain_reset: function () {
        Ext.each(Ext.getCmp('formpanel_search2').getForm().getFields().items,
            function (field) {
                field.reset();
            });
    },

    Maintain_product: function(ID) {
        var store_customer = Ext.create('Ext.data.JsonStore',
            {
                fields: [
                    'ITEMNO',
                    'HSCODE',
                    'ADDITIONALNO',
                    'ITEMNOATTRIBUTE',
                    'COMMODITYNAME',
                    'SPECIFICATIONSMODEL',
                    'UNITNAME',
                    'UNIT',
                    'VERSION',
                    'ENABLED',
                    'STARTDATE',
                    'CREATEMANNAME',
                    'CREATEDATE',
                    'STOPMANNAME',
                    'ENDDATE',
                    'REMARK',
                    'ID'
                ],
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: 'busi_RecordInfor.aspx?action=loaddataproduct&recordinfoid=' + ID + '',
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
                            Ext.getCmp('formpanel_search2').getForm().getValues();
                    }
                }
            });
        var pgbar_product = Ext.create('Ext.toolbar.Paging',
            {
                id: 'pgbar_product',
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: store_customer,
                displayInfo: true

            });
        gridpanel_product = Ext.create('Ext.grid.Panel', {
            id: 'gridpanel_product',
            height: 650,
            region: 'center',
            store: store_customer,
            selModel: { selType: 'checkboxmodel' },
            bbar: pgbar_product,

            columns: [
                { xtype: 'rownumberer', width: 35 },
                { header: '项号', dataIndex: 'ITEMNO', width: 100 },
                { header: 'HS编码', dataIndex: 'HSCODE', width: 200 },
                { header: '附加码', dataIndex: 'ADDITIONALNO', width: 100 },
                { header: '项号属性', dataIndex: 'ITEMNOATTRIBUTE', width: 100 },
                { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 100 },
                { header: '规格型号', dataIndex: 'SPECIFICATIONSMODEL', width: 100 },
                { header: '成交单位', dataIndex: 'UNIT', width: 100 },
                { header: '版本号', dataIndex: 'VERSION', width: 100 },
                { header: '启用/禁用', dataIndex: 'ENABLED', width: 100, renderer: util.Maintain_gridrender },
                { header: '启用时间', dataIndex: 'STARTDATE', width: 100 },
                { header: '维护人', dataIndex: 'CREATEMANNAME', width: 100 },
                { header: '维护时间', dataIndex: 'CREATEDATE', width: 100 },
                { header: '停用人', dataIndex: 'STOPMANNAME', width: 100 },
                { header: '停用时间', dataIndex: 'ENDDATE', width: 100 },
                { header: '备注', dataIndex: 'REMARK', width: 100 }
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
    },

    Maintain_recorddetails: function (ID) {

        var field_ID = Ext.create('Ext.form.field.Hidden', {
            id: 'ID',
            name: 'ID'
        });

        var field_itemno = Ext.create('Ext.form.field.Number', {
            id: 'ITEMNO',
            name:'ITEMNO',
            fieldLabel: '项号',
            allowDecimals: false,//不允许输入小数
            nanText: '请输入正确的数据类型',
            allowNegative: false,
            flex: .5,
            minValue: 0,
            allowBlank: false,
            blankText:'项号不能为空'
        });

        var field_hscode = Ext.create('Ext.form.field.Number',{
            id: 'HSCODE',
            name:'HSCODE',
            fieldLabel: 'HS编码',
            allowDecimals: false,//不允许输入小数
            nanText: '请输入正确的数据类型',
            allowNegative: false,
            flex: .5,
            minValue: 0,
            allowBlank: false,
            blankText: 'HS编码不能为空',
            listeners: {
                blur: function() {
                    Ext.Ajax.request({
                        url: 'busi_RecordInfor.aspx',
                        type: 'Post',
                        params: { action: 'judge', hscode: Ext.getCmp('HSCODE').getValue(), additionalno: Ext.getCmp('ADDITIONALNO').getValue() },
                        success: function(response, option) {
                            var commondata = Ext.decode(response.responseText);
                            var hscode = commondata.HSCODE;
                            console.log(commondata);
                            if (hscode.length == 0) {
                                Ext.getCmp('COMMODITYNAME').setDisabled(true);
                                Ext.getCmp('SPECIFICATIONSMODEL').setDisabled(true);
                                Ext.Msg.alert("提示","HS编码不存在，非法");
                            }
                        }

                    });
                }
            }
        });

        var field_additionalno = Ext.create('Ext.form.field.Text', {
            id: 'ADDITIONALNO',
            name: 'ADDITIONALNO',
            fieldLabel: 'HS附加码',
            flex: .5,
            allowBlank: false,
            blankText: 'HS附加码不可为空!'
        });

        //账册属性
        var store_itemnoattribute = Ext.create('Ext.data.JsonStore', {
            fields: ['CODE', 'NAME'],
            data: [{ "CODE": "料件", "NAME": "料件" }, { "CODE": "成品", "NAME": "成品" }]
        });
        var combo_itemnoattribute = Ext.create('Ext.form.field.ComboBox', {
            id: 'combo_itemnoattribute',
            name: 'ITEMNOATTRIBUTE',
            store: store_itemnoattribute,
            queryMode: 'local',
            anyMatch: true,
            fieldLabel: '项号属性', flex: .5,
            displayField: 'NAME',
            valueField: 'CODE',
            allowBlank: false,
            blankText: '项号属性不能为空!'
        });

        //商品名称
        var field_commodityname = Ext.create('Ext.form.field.Text', {
            id: 'COMMODITYNAME',
            name: 'COMMODITYNAME',
            fieldLabel: '商品名称',
            flex: .5,
            allowBlank: false,
            blankText: '商品名称不可为空!',
            readOnly:false
        });
        //规格型号
        var field_specificationsmodel = Ext.create('Ext.form.field.Text', {
            id: 'SPECIFICATIONSMODEL',
            name: 'SPECIFICATIONSMODEL',
            fieldLabel: '规格型号',
            flex: .5,
            readOnly: false
        });
        
        //成交单位
        var store_unit = Ext.create('Ext.data.JsonStore', {
            fields: ['CODE', 'NAME'],
            data: unit
        });
        var combo_unit = Ext.create('Ext.form.field.ComboBox', {
            id: 'UNIT',
            name: 'UNIT',
            store: store_unit,
            queryMode: 'local',
            anyMatch: true,
            fieldLabel: '成交单位', flex: .5,
            displayField: 'NAME',
            valueField: 'CODE',
            allowBlank: false,
            blankText: '成交单位不能为空!'
        });
        //版本号
        var field_version = Ext.create('Ext.form.field.Text', {
            id: 'VERSION',
            name: 'VERSION',
            fieldLabel: '版本号',
            flex: .5,
        });
        //是否启用
        var store_recorddetail = Ext.create('Ext.data.JsonStore',
            {
                fields: ['CODE', 'NAME'],
                data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
            });

        var combo_recorddetail = Ext.create('Ext.form.field.ComboBox',
            {
                id:'ENABLED',
                name: 'ENABLED',
                store: store_recorddetail,
                queryMode: 'local',
                anyMatch: true,
                fieldLabel: '是否启用', flex: .5,
                displayField: 'NAME',
                valueField: 'CODE',
                value: 1,
                allowBlank: false,
                blankText: '是否启用不能为空!'

            });

        //启用时间
        var start_date_recorddetail = Ext.create('Ext.form.field.Date',
            {
                id: 'STARTDATE',
                name: 'STARTDATE',
                format: 'Y-m-d',
                fieldLabel: '启用日期',
                flex: .5

            });
        //停用时间
        var end_date_recorddetail = Ext.create('Ext.form.field.Date',
            {
                id: 'ENDDATE',
                name: 'ENDDATE',
                format: 'Y-m-d',
                fieldLabel: '停用日期',
                flex: .5
            });
        //维护人
        var CreatemanName_recorddetail = Ext.create('Ext.form.field.Text', {
            id: 'CREATEMANNAME',
            name: 'CREATEMANNAME',
            fieldLabel: '维护人',
            readOnly: true
        });
        //备注
        var field_remark = Ext.create('Ext.form.field.Text', {
            id: 'REMARK',
            name: 'REMARK',
            fieldLabel: '备注'
        });
        //料号
        var field_partno = Ext.create('Ext.form.field.Text', {
            id: 'PARTNO',
            name: 'PARTNO',
            fieldLabel: '料号'
        });
        //修改原因输入框
        var change_reason_recorddetail = Ext.create('Ext.form.field.Text', {
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
                labelWidth: 80,
                columnWidth: .5,
                labelAlign: 'right',
                labelSeparator: '',
                msgTarget: 'under'
            },
            items: [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_itemno, field_hscode] },
                { layout: 'column', height: 42, border: 0, items: [field_additionalno, combo_itemnoattribute] },
                { layout: 'column', height: 42, border: 0, items: [field_commodityname, field_specificationsmodel] },
                { layout: 'column', height: 42, border: 0, items: [combo_unit, field_version] },
                { layout: 'column', height: 42, border: 0, items: [field_partno, combo_recorddetail] },
                { layout: 'column', height: 42, border: 0, items: [start_date_recorddetail, end_date_recorddetail ] },
                { layout: 'column', height: 42, border: 0, items: [CreatemanName_recorddetail,field_remark] },
                { layout: 'column', height: 42, border: 0, items: [change_reason_recorddetail] },
                field_ID
            ],
            buttons: [{

                text: '<span class="icon iconfont" style="font-size:12px;">&#xe60c;</span>&nbsp;保存', handler: function () {
                    if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                        return;
                    }

                    var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                    Ext.Ajax.request({
                        url: 'busi_RecordInfor.aspx',
                        type: 'Post',
                        params: { action: 'save_recorddetails', formdata: formdata,recordinfoid:ID },
                        success: function (response, option) {

                            var data = Ext.decode(response.responseText);
                            if (data.success == "4") {
                                Ext.Msg.alert('提示',
                                    "保存失败:HS编码重复!",
                                    function () {
                                        Ext.getCmp("pgbar_product").moveFirst();
                                        Ext.getCmp("pgbar_category").moveFirst();
                                        Ext.getCmp("win_recorddeatils").close();
                                    });
                            } else {
                                if (data.success) {
                                    Ext.Msg.alert('提示', "保存成功", function () {
                                        Ext.getCmp("pgbar_product").moveFirst(); Ext.getCmp("win_recorddeatils").close();
                                        Ext.getCmp("pgbar_category").moveFirst();
                                    });
                                }
                                else {
                                    Ext.Msg.alert('提示', "保存失败", function () {
                                        Ext.getCmp("pgbar_product").moveFirst(); Ext.getCmp("win_recorddeatils").close();
                                        Ext.getCmp("pgbar_category").moveFirst();
                                    });
                                }
                            }


                        }
                    });

                }
            }]
        });


    },

    Maintain_gridrender:function(value, cellmeta, record, rowIndex, columnIndex, stroe) {
        var dataindex = cellmeta.column.dataIndex;
        var str = "";
        switch (dataindex) {
        case "ENABLED": case "ISMODEL":
            str = value == "1" ? '<span class="icon iconfont" style="font-size:12px;color:blue;">&#xe628;</span>' : '<span class="icon iconfont" style="font-size:12px;color:red;">&#xe634;</span>';
            break;
        }
        return str;
    },
    Maintain_add_recorddetails: function(ID,formdata,ID2) {
        util.Maintain_recorddetails(ID);
        Ext.getCmp('CREATEMANNAME').setValue(username);
        if (ID2 != "") {
            Ext.getCmp('REASON').hidden = false;
            Ext.getCmp('REASON').allowBlank = false;
            Ext.getCmp('REASON').blankText = '修改原因不可为空!';
            //默认值的
            Ext.getCmp('formpanel_Win').getForm().setValues(formdata);
        }
        var win = Ext.create("Ext.window.Window", {
            id: "win_recorddeatils",
            title: '备案信息',
            width: 1200,
            height: 430,
            modal: true,
            items: [Ext.getCmp('formpanel_Win')]
        });
        win.show();
    },
    Maintain_edit_recorddetails: function (ID) {

        var recs = Ext.getCmp('gridpanel_category').getSelectionModel().getSelection();
        var recs2 = Ext.getCmp('gridpanel_product').getSelectionModel().getSelection();
        if (recs.length + recs2.length == 0) {
            Ext.MessageBox.alert('提示', '请选择需要查看详细的记录！');
            return;
        }
        if (recs.length != 0) {
            util.Maintain_add_recorddetails(ID, recs[0].data, recs[0].get("ID"));
        } else {
            util.Maintain_add_recorddetails(ID, recs2[0].data, recs2[0].get("ID"));
        }
        

    },
    Maintain_exportdata_recorddetails: function(ID) {
        var HSNUMBER = Ext.getCmp('HSNUMBER').getValue();
        var HSCODE1 = Ext.getCmp('HSCODE1').getValue();
        var ENABLED_S2 = Ext.getCmp('ENABLED_S2').getValue();
        var path = 'busi_RecordInfor.aspx?action=export_recorddetails&HSNUMBER=' + HSNUMBER + '&HSCODE1=' + HSCODE1 + '&ENABLED_S2=' + ENABLED_S2 + '&id=' + ID;
        $('#exportform').attr("action", path).submit();
    },
    Maintain_importfile: function (action,ID) {
        if (action == "add_recorddetails") {
            util.Maintain_importexcel(action,ID);
        }
    },
    Maintain_importexcel: function(action,ID) {
        var radio_module = Ext.create('Ext.form.RadioGroup', {
            name: "RADIO_MODULE", id: "RADIO_MODULE", fieldLabel: '模板类型',
            items: [
                { boxLabel: "<a href='../FileUpload/record_details.xls'><b>模板</b></a>", name: 'RADIO_MODULE', inputValue: '1', checked: true }
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
                            url: 'busi_RecordInfor.aspx',
                            params: { formdata: formdata, action: action,id:ID },
                            waitMsg: '数据导入中...',
                            success: function (form, action) {
                                console.log(action.result);
                                var data = action.result.success;
                                var reg = /,$/gi;
                                idStr = data.replace(reg, "!");
                                Ext.Msg.alert('提示', idStr, function () {
                                    Ext.getCmp('pgbar_category').moveFirst();
                                    Ext.getCmp('pgbar_product').moveFirst();
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
            title: '备案信息',
            width: 600,
            height: 240,
            modal: true,
            items: [Ext.getCmp('formpanel_upload')]
        });
        Ext.getCmp('CREATEMANNAME').setValue(username);
        win_upload.show();
    }

}