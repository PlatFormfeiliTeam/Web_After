<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PdfEdit.aspx.cs" Inherits="Web_After.PdfEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" type="text/css" href="js/jquery-easyui-1.4.5/themes/default/easyui.css" />
    <link rel="stylesheet" type="text/css" href="js/jquery-easyui-1.4.5/themes/icon.css" />
    <script type="text/javascript" src="js/jquery-easyui-1.4.5/jquery.min.js"></script>
    <script type="text/javascript" src="js/jquery-easyui-1.4.5/jquery.easyui.min.js" ></script>
    <script src="js/pan.js"></script>
     <style>
         html, body, form {
             height: 99%;
         }
         .input{
            width: 70%;
            text-align: left;
            background-color: #e5f1f4;
            border-top-style: none;
            border-right-style: none;
            border-left-style: none;
            border-bottom: green 1px solid;
        }
         
        .datagrid-header td,
        .datagrid-body td,
        .datagrid-footer td {
          border-width: 0 1px 1px 0;
          border-style: solid;
          margin: 0;
          padding: 0;
        }
    </style>
    <script type="text/javascript">
        var ordercode = getQueryString("ordercode");
        var userid = getQueryString("userid");
        var filetype = 44;
        var fileid = "";
        var allow_sel;

        $(function () {
            iniform();
        });

        function iniform() {
            $.ajax({
                type: 'Post',
                url: "PdfEdit.aspx",
                dataType: "text",
                data: { ordercode: ordercode, action: 'loadform' },
                async: false,
                success: function (data) {
                    var obj = eval("(" + data + ")");//将字符串转为json
                    var formdata = obj.formdata;
                    $("#txt_Busitype").val(formdata["BUSITYPENAME"]);
                    $("#txt_busiunit").val(formdata["BUSIUNITNAME"]);
                    $("#txt_Splitstatus").val(formdata["FILESTATUSDESC"]);

                    if ($.trim($("#td_radio").text()) == "") {
                        $('<input />', {
                            type: "radio", name: "rdo", checked: "checked", val: formdata["CODE"],
                            change: function () {
                                ordercode = $(this).val();
                                iniform();
                            }
                        }).appendTo("#td_radio");
                        $("<span style='margin-right:50px;'>" + formdata["CODE"] + "</span>").appendTo("#td_radio");

                        if (formdata["ASSOCIATENO"] != "") {
                            $('<input />', {
                                type: "radio", name: "rdo", val: formdata["ASSOCIATENO"],
                                change: function () {
                                    ordercode = $(this).val();
                                    iniform();
                                }
                            }).appendTo("#td_radio");

                            $("<span style='margin-right:50px;'>" + formdata["ASSOCIATENO"] + "</span>").appendTo("#td_radio");
                        }
                    }

                    $("#td_cbl").text('');
                    for (var i = 0; i < obj.filedata.length; i++) {
                        $('<input />', {
                            type: "checkbox", name: "cbox", val: obj.filedata[i]["ID"],
                            change: function () {
                                fileid = $(this).val();
                                pdfview();
                            }
                        }).appendTo("#td_cbl");
                        $("<span style='margin-right:60px;'>订单文件" + (i + 1) + "</span>").appendTo("#td_cbl");
                    }

                    if (obj.filedata.length == 1) {
                        $("input[name='cbox']").get(0).click(); /*$("input[name='cbox']").get(0).checked = true;  $("input[name='cbox']").trigger("change");*/
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {//请求失败处理函数  
                    alert(XMLHttpRequest.status);
                    alert(XMLHttpRequest.readyState);
                    alert(textStatus);
                }
            });

        }

        function pdfview() {
            $.ajax({
                type: 'Post',
                url: "PdfEdit.aspx",
                dataType: "text",
                data: { ordercode: ordercode, fileid: fileid, action: 'loadpdf', timestamp: new Date().getTime() },
                async: false,
                success: function (data) {
                    var obj = eval("(" + data + ")");//将字符串转为json
                    var str = '<embed  id="pdf" width="100%" height="98%" src="/file/' + obj.src + '"></embed>';
                    $("#pdfdiv").html(str);
                    //按钮控制开始
                    var ordertatus = $("#txt_Splitstatus").val();
                    if (ordertatus == '已拆分') {//订单的拆分状态
                        $("#btn_merge").linkbutton('disable');
                        $("#btn_confirmsplit").linkbutton('disable');
                        allow_sel = false;
                        if (obj.filestatus == 0) {//文件的拆分状态
                            $("#btn_cancelsplit").linkbutton('disable');
                        }
                        else {
                            $("#btn_cancelsplit").linkbutton('enable');
                        }
                    }
                    else {
                        if ($("input[name='cbox']:checked").length > 1) {
                            $("#btn_merge").linkbutton('enable');
                        }
                        else {
                            $("#btn_merge").linkbutton('disable');
                        }
                        $("#btn_confirmsplit").linkbutton('enable');
                        allow_sel = true;
                        $("#btn_cancelsplit").linkbutton('disable');
                    }
                    //按钮控制结束 

                    var columnarray_frozen = new Array(); var columnarray = new Array();

                    for (var key in obj.rows[0]) {
                        switch (key) {
                            case "ID":
                                columnarray_frozen.push({
                                    title: '页码', field: key, width: 48, align: 'center', frozen: true, formatter: function (value, row, index) {
                                        return '第' + value + '页';
                                    }
                                });
                                columnarray_frozen.push({
                                    width: 48, title: '操作', field: '_operate', align: 'center', formatter: function (value, row, index) {
                                        return '<img alt="" src="/images/shared/arrow_up.gif" onclick="" /><img alt="" src="/images/shared/arrow_down.gif" onclick="" />';
                                    }
                                });
                                break;
                            default:
                                var start = key.indexOf("|");
                                var header = key.slice(start + 1);
                                columnarray.push({ title: header, field: key, width: 65, align: 'center', editor: 'text' });
                                break;
                        }
                    }
                    var cols_frozen = new Array(columnarray_frozen); var cols = new Array(columnarray);

                    $('#appConId').datagrid({
                        singleSelect: true,
                        frozenColumns: cols_frozen,
                        columns: cols,
                        data: obj.rows,
                        onClickRow: function (index, row) {
                            var PDF = document.getElementById("pdf");
                            PDF.setCurrentPage(row["ID"]);
                        },
                        onClickCell: function (index, fieldname, value) {
                            if (allow_sel) {
                                if (fieldname != "ID" && fieldname != "_operate") {

                                    /***改变HTML的值
                                    var td = $('.datagrid-body td[field="' + fieldname + '"]')[index];
                                    var div = $(td).find('div')[0];
                                    var newtext = $(div).text() == "√" ? "" : "√";
                                    $(div).text(newtext);
                                    */
                                    
                                    //改变val值，以便数据传后台
                                    $(this).datagrid('beginEdit', index);
                                    var ed = $(this).datagrid('getEditor', { index: index, field: fieldname });
                                    $(ed.target).val($(ed.target).val() == "√" ? "" : "√");
                                    $(this).datagrid('endEdit', index);

                                }
                            }
                        },
                        onLoadError: function (XMLHttpRequest, textStatus, errorThrown) {//请求失败处理函数
                            alert(XMLHttpRequest.status); alert(XMLHttpRequest.responseText); document.write(XMLHttpRequest.responseText);
                            alert(XMLHttpRequest.readyState);
                            alert(textStatus);
                        }
                    });


                    //清除追加的button按钮
                    $('#tb a').each(function (index, element) {
                        if (index >= 3) {
                            $(this).remove();
                        }
                    });

                    //拆分完成后添加拆分好文件类型的查看按钮    
                    for (var i = 0; i < obj.result.length; i++) {
                        var id = obj.result[i]["ID"]; var typeid = obj.result[i]["FILETYPEID"]; var typename = obj.result[i]["FILETYPENAME"];

                        $('<a id="' + typeid + "_" + id + '" href="#" onclick="viewfiledetail(this.id)">' + typename + '</a>').appendTo("#tb");
                        $('#' + typeid + "_" + id).linkbutton({
                            iconCls: 'icon-search', plain: 'true'
                        });
                    }

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {//请求失败处理函数  
                    alert(XMLHttpRequest.status);
                    alert(XMLHttpRequest.readyState);
                    alert(textStatus);
                }
            });
        }

        function viewfiledetail(id) {
            $('#appConId').datagrid('loadData', { total: 0, rows: [] });
            loadfile(id);
        }

        function loadfile(id) {
            var array1 = id.split('_');
            $.ajax({
                type: 'Post',
                url: "PdfEdit.aspx",
                dataType: "text",
                data: { fileid: array1[1], action: 'loadfile', timestamp: new Date().getTime() },
                async: false,
                success: function (data) {
                    var obj = eval("(" + data + ")");//将字符串转为json
                    if (obj.success) {
                        var str = '<embed  id="pdf" width="100%" height="98%" src="/file/' + obj.src + '"></embed>';
                        $("#pdfdiv").html(str);
                    }                   
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {//请求失败处理函数  
                    alert(XMLHttpRequest.status);
                    alert(XMLHttpRequest.readyState);
                    alert(textStatus);
                }
            });
        }

        function mergefile() {//文件合并
            $("input[name='cbox']").each(function () {
                if (this.checked) {
                    alert($(this).val());
                }
            });
        }

        function confirmsplit() {//确定拆分
            var allowsplit = false;

            var s = JSON.stringify($("#appConId").datagrid("getData"));
            alert(s);

            var gvdata = $("#appConId").datagrid("getData").rows; var td; var div;
            for (var i = 0; i < gvdata.length; i++) {
                for (var key in gvdata[i]) {
                    td = $('.datagrid-body td[field="' + key + '"]')[i];
                    div = $(td).find('div')[0];
                    if ($(div).text() == "√") {
                        allowsplit = true;
                    }
                }
            }
            if (!allowsplit) {
                $("#pdf").hide();
                $.messager.alert('提示', '请先勾选具体的拆分明细！', 'info', function () {
                    $("#pdf").show();
                });
                return;
            }

            $("#btn_confirmsplit").linkbutton('disable');

            //var pages = Ext.encode(Ext.pluck(gridpanel.store.data.items, 'data'));
            //Ext.Ajax.request({
            //    url: "PdfView.aspx",
            //    //?action=split&fileid=" + fileid + "&filetype=" + filetype + "&ordercode=" + ordercode
            //    params: { action: 'split', fileid: fileid, filetype: filetype, ordercode: ordercode, pages: pages, userid: userid },
            //    success: function (response) {
            //        panel.hide();
            //        var json = Ext.decode(response.responseText);
            //        if (json.success) {
            //            Ext.MessageBox.alert('提示', '拆分成功！', function () {
            //                panel.show();
            //                field_filestatus.setValue('已拆分');
            //            })
            //            Ext.getCmp('btn_cancelsplit').setDisabled(false);
            //            allow_sel = false;
            //            for (var i = 0; i < json.result.length; i++) {
            //                //拆分完成后添加拆分好文件类型的查看按钮  
            //                var btn = Ext.create('Ext.Button', {
            //                    id: json.result[i].FILETYPEID + "_" + json.result[i].ID,
            //                    text: '<i class="fa fa-file-pdf-o"></i>&nbsp;' + json.result[i].FILETYPENAME,
            //                    handler: function () {
            //                        gridpanel.getStore().removeAll();
            //                        loadfile(this.id);
            //                    }
            //                })
            //                toolbar.add(btn);
            //            }
            //        }
            //        else {
            //            Ext.MessageBox.alert('提示', '拆分失败，文件压缩中，请稍后再试！', function () {
            //                panel.show();
            //            })
            //        }
            //    }
            //});
        }

        function cancelsplit() {//撤销拆分     
            $("#pdf").hide();
            $.messager.confirm("提示", "确定要撤销拆分吗？", function (r) {
                if (r) {
                    $("#btn_cancelsplit").linkbutton('disable');
                    $.ajax({
                        type: 'Post',
                        url: "PdfEdit.aspx",
                        dataType: "text",
                        data: { ordercode: ordercode, fileid: fileid, userid: userid, action: 'cancelsplit', timestamp: new Date().getTime() },
                        async: false,
                        success: function (data) {
                             $("#pdf").hide(); 
                            $.messager.alert('提示', '撤销拆分成功！', 'info', function () {
                                $("#txt_Splitstatus").val("未拆分");
                                $("#pdf").show();
                            });

                            $("#btn_cancelsplit").linkbutton('enable');
                            allow_sel = true;
                            //清除追加的button按钮
                            $('#tb a').each(function (index, element) {
                                if (index >= 3) {
                                    $(this).remove();
                                }
                            });

                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {//请求失败处理函数  
                            alert(XMLHttpRequest.status);
                            alert(XMLHttpRequest.readyState);
                            alert(textStatus);
                        }
                    });
                } else {
                    $("#pdf").show();
                }               

            });

        }

    </script>

</head>
<body>
    <form id="form1" runat="server">  
        <div class="easyui-layout" style="height:100%;">
            <div region="north" style="height:120px;">
                <table style="width: 100%;height:100%;" cellpadding=" 0" cellspacing="0" >
                    <tr>
                        <td style="width:5%; text-align:right;">订单号：</td>
                        <td style="width:25%;" id="td_radio">
                        </td>
                        <td style="width:25%">业务类型：<input id="txt_Busitype" type="text" class="input" readonly /></td>
                        <td style="width:25%">经营单位：<input id="txt_busiunit" type="text" class="input" readonly /></td>
                        <td style="width:25%">拆分状态：<input id="txt_Splitstatus" type="text" class="input"  readonly /></td>
                    </tr>
                    <tr>
                        <td style="width:5%; text-align:right;">订单文件：</td>
                        <td colspan="4" style="width:95%" id="td_cbl">
                        </td>
                    </tr>
                </table>
            </div>           
            <div id="pdfdiv" region="center"></div>
            <div region="east" style="width:40%">
                 <div id="tb" style="padding:5px;height:auto"> 
                    <a id="btn_merge" href="#" class="easyui-linkbutton" iconcls="icon-add" plain="true" disabled="true" onclick="mergefile()">文件合并</a>
                    <a id="btn_confirmsplit" href="#" class="easyui-linkbutton" iconcls="icon-cut" plain="true" disabled="true" onclick="confirmsplit()">确定拆分</a>
                    <a id="btn_cancelsplit" href="#" class="easyui-linkbutton" iconcls="icon-undo" plain="true" disabled="true" onclick="cancelsplit()">撤销拆分</a>
                </div> 
                <table id="appConId" class="easyui-datagrid" toolbar="#tb" style="height:100%;width:100%;"></table>
            </div>
        </div>        
    </form>
</body>
</html>



                    <%--
                        


                        //var str = '<input type="radio" name="rdo" checked="checked" value="' + formdata["CODE"] + '" />' + formdata["CODE"];
                        //$("#td_radio").html(str);
                        
                        //if (obj.success) {
                    //    var json = eval(obj.rows);
                        //var strul = "", strdiv = "";
                        //$.each(json, function (idx, item) {
                           
                        //    if (idx == 0) {
                        //        strul += '<li class="current">';
                        //        strdiv += '<div';
                        //    } else {
                        //        strul += '<li>';
                        //        strdiv += '<div class="hide"';
                        //    }
                        //    var newid = typeid + "_" + id;
                        //    html1 += '<button type="button" class="btn  btn-primary btn-sm" onclick="loadfile(\'' + newid + '\')"><i class="fa fa-file-pdf-o"></i>&nbsp;' + json.rows[i].FILETYPENAME + "_" + type_index + '</button>';
                        //}
                        //html1 += '</div>';
                        //toolbar.add(html1);
                    //}--%>

    <%--        /*$('#appConId').datagrid({
                url: "PdfEdit.aspx",
                pagination: true,//显示分页
                pageSize: 20,//分页大小
                rownumbers: true,//行号
                striped: true,
                remoteSort: true,
                loadMsg: '数据加载中......',
                queryParams: {
                    'ordercode': ordercode, 'action': 'loadform'
                },
                columns: [],
                onClickRow: function (rowIndex, rowData) {
                    $('#appConId').datagrid('unselectAll');
                    $('#appConId').datagrid('selectRow', rowIndex);
                },
                onDblClickRow: function (rowIndex, rowData) {
                    opencenterwin("/AccountManagement/ChildEdit?ID=" + rowData.ID, 1000, 400);
                },
                onLoadSuccess: function (data) {
                    $('.pagination-page-list').hide();//隐藏PageList
                },
                onLoadError: function (XMLHttpRequest, textStatus, errorThrown) {//请求失败处理函数
                    alert(XMLHttpRequest.status); alert(XMLHttpRequest.responseText); document.write(XMLHttpRequest.responseText);
                    alert(XMLHttpRequest.readyState);
                    alert(textStatus);
                }
            });
            var pager = $('#appConId').datagrid('getPager');	// get the pager of datagrid*/--%>