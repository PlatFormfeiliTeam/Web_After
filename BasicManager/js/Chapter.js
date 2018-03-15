
function init_search() {

        var txtCODE = Ext.create('Ext.form.field.Text', { id: 'CODE_S', name: 'CODE_S', fieldLabel: '内部编码' });
        var txtCNNAME = Ext.create('Ext.form.field.Text', { id: 'CNNAME_S', name: 'CNNAME_S', fieldLabel: '企业名称' });

        var txtSEACODE = Ext.create('Ext.form.field.Text', { id: 'CODE_Sea', name: 'CODE_Sea', fieldLabel: '海关编码' });
        var txtInspCODE = Ext.create('Ext.form.field.Text', { id: 'CODE_Insp', name: 'CODE_Insp', fieldLabel: '商检编码' });

        var store_ENABLED_S = Ext.create('Ext.data.JsonStore', {
            fields: ['CODE', 'NAME'],
            data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
        });
        var combo_ENABLED_S = Ext.create('Ext.form.field.ComboBox', {
            id: 'combo_ENABLED_S',
            name: 'ENABLED_S',
            store: store_ENABLED_S,
            queryMode: 'local',
            anyMatch: true,
            fieldLabel: '是否启用',
            displayField: 'NAME',
            valueField: 'CODE'
        });

        var toolbar = Ext.create('Ext.toolbar.Toolbar', {
            items: [
                { text: '<span class="icon iconfont">&#xe622;</span>&nbsp;新 增', handler: function () { addCustomer_Win(""); } }
                , { text: '<span class="icon iconfont">&#xe632;</span>&nbsp;修 改', width: 80, handler: function () { editCustomer(); } }
                //, { text: '<span class="icon iconfont">&#xe6d3;</span>&nbsp;删 除', width: 80, handler: function () { del(); } }
                , { text: '<span class="icon iconfont">&#xe670;</span>&nbsp;导 入', width: 80, handler: function () { importfile('add'); } }
                , { text: '<span class="icon iconfont">&#xe625;</span>&nbsp;导 出', handler: function () { exportdata(); } }
                , '->'
                , { text: '<span class="icon iconfont">&#xe60b;</span>&nbsp;查 询', width: 80, handler: function () { Ext.getCmp("pgbar").moveFirst(); } }
                , { text: '<span class="icon iconfont">&#xe633;</span>&nbsp;重 置', width: 80, handler: function () { reset(); } }
            ]
        });

        formpanel_search = Ext.create('Ext.form.Panel', {
            id: 'formpanel_search',
            region: 'north',
            border: 0,
            bbar: toolbar,
            fieldDefaults: {
                margin: '5',
                columnWidth: 0.25,
                labelWidth: 70
            },
            items: [
                { layout: 'column', border: 0, items: [txtCODE, txtCNNAME, txtSEACODE, txtInspCODE, combo_ENABLED_S] }

            ]
        });
    }
