function opencenterwin(url, width, height) {
    var iWidth = width ? width : "1000", iHeight = height ? height : "600";
    var iTop = (window.screen.availHeight - 30 - iHeight) / 2; //获得窗口的垂直位置;
    var iLeft = (window.screen.availWidth - 10 - iWidth) / 2; //获得窗口的水平位置; 
    window.open(url, '', 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',toolbar=no,menubar=no,scrollbars=yes,resizable=yes');
}
function opencenterwin_no(url, width, height) {
    var iWidth = width ? width : "1000", iHeight = height ? height : "600";
    var iTop = (window.screen.availHeight - 30 - iHeight) / 2; //获得窗口的垂直位置;
    var iLeft = (window.screen.availWidth - 10 - iWidth) / 2; //获得窗口的水平位置; 
    window.open(url, '', 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',toolbar=no,menubar=no,scrollbars=yes,resizable=no');
}
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
//列渲染 by panhuaguo 2016-09-07
function render(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    var dataindex = cellmeta.column.dataIndex;
    switch (dataindex) {
        case "TYPE":
            rtn = value != 4 ? '外部账户' : '内部账户';
            break;
        case "ENABLED": 
            rtn = value ? '启用' : '停用';
            break;
        case "ID":
            rtn = "<span onclick='inipsd(\"" + record.get("ID") + "\",\"" + record.get("NAME") + "\")'><i class=\"fa fa-key fa-fw\"></i></span>";
            break;
        case "SPLITSTATUS":
            rtn = value == "1" ? "是" : "否";
            break;
        case "ISINVALID": 
            rtn = value == "0" ? '<span class="icon iconfont" style="font-size:12px;color:blue;">&#xe628;</span>' : '<span class="icon iconfont" style="font-size:12px;color:red;">&#xe634;</span>';
            break;
        case "ISCUSTOMER": case "ISSHIPPER": case "ISCOMPANY":
            rtn = value == "1" ? '<span class="icon iconfont" style="font-size:12px;color:blue;">&#xe628;</span>' : '<span class="icon iconfont" style="font-size:12px;color:red;">&#xe634;</span>';
            break;
    }
    return rtn;
}
//系统模块编辑窗口 by panhuaguo 2016-08-30
function module_edit_win(parentNode, action) {
    var formpanel_module = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        region: 'center',
        // title: '系统模块',
        defaults: { labelAlign: 'right', xtype: 'textfield', msgTarget: 'under', margin: '10' },
        items: [
        { name: 'NAME', anchor: '95%', fieldLabel: '模块名称', allowBlank: false, blankText: '模块名称不能为空', emptyText: '请输入模块名称' },
        { name: 'URL', anchor: '95%', fieldLabel: '链接地址', allowBlank: true, emptyText: '请输入链接地址' },
        { name: 'ICON', anchor: '95%', fieldLabel: '图标' },
        { name: 'SORTINDEX', anchor: '95%', fieldLabel: '显示顺序', xtype: 'numberfield' },
        { name: 'MODULEID', xtype: 'hidden' },
        { name: 'PARENTID', xtype: 'hidden', id: 'field_parentid' }
        ],
        buttons: [{
            text: '保 存', handler: function () {
                var baseForm = formpanel_module.getForm();
                if (baseForm.isValid()) {
                    Ext.Ajax.request({
                        url: "ModuleList.aspx",
                        params: { action: action, json: Ext.encode(baseForm.getValues()) },
                        callback: function (option, success, response) {
                            var result = Ext.decode(response.responseText);
                            if (result.success) {
                                Ext.Msg.alert("提示", "保存成功!", function () {
                                    if (action == "create") { //如果是新增 
                                        if (parentNode.data.leaf) {//如果是叶子
                                            parentNode.set("leaf", false);
                                            parentNode.expand();
                                        }
                                        else {
                                            if (parentNode.isExpanded()) {//如果已经展开了
                                                var childNode = parentNode.createNode({ MODULEID: result.data.MODULEID, NAME: result.data.NAME, leaf: true, URL: result.data.URL,ICON:result.data.ICON});
                                                parentNode.appendChild(childNode);
                                            }
                                            else {//如果未展开
                                                parentNode.expand();
                                            }
                                        }
                                    }
                                    else { //如果是修改
                                        parentNode.set("NAME", result.data.NAME);
                                        parentNode.set("URL", result.data.URL);
                                    }
                                    win_sysmodule.close();
                                });
                            }
                        }
                    });
                }
            }
        }],
        buttonAlign: 'center'
    })
    var win_sysmodule = Ext.create("Ext.window.Window", {
        title: '系统模块',
        width: 700,
        height: 570,
        modal: true,
        items: [formpanel_module],
        layout: 'border',
        buttonAlign: 'center'
    });
    win_sysmodule.show();
    if (action == "update") {//如果是修改 
        formpanel_module.getForm().setValues(parentNode.data);
    }
    if (action == "create" && parentNode.data.MODULEID) {
        Ext.getCmp("field_parentid").setValue(parentNode.get('MODULEID'));
    }
}

function upload_ini() {
    uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,html4',
        browse_button: 'pickfiles', // you can pass an id...
        url: 'NoticeEdit.aspx?action=uploadfile',
        flash_swf_url: '/js/upload/Moxie.swf',
        silverlight_xap_url: '/js/upload/Moxie.xap',
        unique_names: true,
        filters: {
            max_file_size: '10000mb',
            mime_types: [
                { title: "Image files", extensions: "*" },
                { title: "Zip files", extensions: "zip,rar" }
            ]
        }
    });
    uploader.init();
    uploader.bind('FilesAdded', function (up, files) {
        uploader.start();
    });
    uploader.bind('FileUploaded', function (up, file) {

        var timestamp = Ext.Date.now();  //1351666679575  这个方法只是获取的时间戳
        var date = new Date(timestamp);

        file_store.insert(file_store.data.length,
       { FILENAME: '/FileUpload/file/' + file.target_name, ORIGINALNAME: file.name, SIZES: file.size, UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s') });
    });
}

function panel_file_ini() {
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
        tpl: tmp,
        itemSelector: 'div.thumb-wrap',
        multiSelect: true
    })
    var panel = Ext.create('Ext.panel.Panel', {
        renderTo: "div_panel",
        border: 0,
        items: [fileview]
    })
}
//删除文件
function removeFile() {
    var records = Ext.getCmp('w_fileview').getSelectionModel().getSelection();
    if (records.length == 0) {
        Ext.MessageBox.alert("提示", "请选择要删除的记录！");
        return
    }
    Ext.MessageBox.confirm('提示', '确定要删除选择的记录吗？', function (btn) {
        if (btn == 'yes') {
            Ext.getCmp('w_fileview').store.remove(records);
        }
    })
}


