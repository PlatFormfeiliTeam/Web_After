var util = {
    //维护界面的大小
    addMaintain_Win: function (ID, formdata) {
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
    },
    //维护界面search条件
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
                        handler: function () { util.editCustomer(); }
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
                    'HSCODE', 'EXTRACODE', 'HSNAME', 'LEGALUNITNAME', 'NUMNAME', 'WEIGHT', 'CUSTOMREGULATORY', 'INSPECTIONREGULATORY', 'YEARNAME',
                    'ENABLED', 'STARTDATE', 'CREATEMANNAME', 'CREATEDATE', 'STOPMANNAME', 'ENDDATE', 'REMARK','ID'
                ],
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: 'Base_year.aspx?action=MaintainloadData&id=' + ID + '',
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
                    { header: 'HS编码', dataIndex: 'HSCODE', width: 150, locked: true },
                    { header: '附加码', dataIndex: 'EXTRACODE', width: 100, tdCls: 'tdValign', locked: true },
                    { header: '商品名称', dataIndex: 'HSNAME', width: 150, locked: true },
                    { header: '法定单位', dataIndex: 'LEGALUNITNAME', width: 100, tdCls: 'tdValign', locked: true },
                    { header: '数量', dataIndex: 'NUMNAME', width: 100, locked: true },
                    { header: '重量', dataIndex: 'WEIGHT', width: 100, locked: true },
                    { header: '海关监管', dataIndex: 'CUSTOMREGULATORY', width: 100, locked: true },
                    { header: '检验检疫', dataIndex: 'INSPECTIONREGULATORY', width: 100, locked: true },
                    { header: '代码库', dataIndex: 'YEARNAME', width: 100, locked: true },
                    { header: '启用情况', dataIndex: 'ENABLED', width: 100, renderer: util.gridrender, locked: true },
                    { header: '启用时间', dataIndex: 'STARTDATE', width: 100, locked: true },
                    { header: '维护人', dataIndex: 'CREATEMANNAME', locked: true, width: 100 },
                    { header: '维护时间', dataIndex: 'CREATEDATE', width: 100, locked: true },
                    { header: '停用人', dataIndex: 'STOPMANNAME', locked: true, width: 100 },
                    { header: '停用时间', dataIndex: 'ENDDATE', locked: true, width: 100 },                   
                    { header: '备注', dataIndex: 'REMARK', width: 270, locked: true },

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

    //CIQ代码重置方法
    Maintain_reset: function () {
        Ext.each(Ext.getCmp('formpanel_search2').getForm().getFields().items, function (field) {
            field.reset();
        });
    },
}