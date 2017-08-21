<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoticeEdit_Publish.aspx.cs" Inherits="Web_After.NoticeEdit_Publish" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/bootstrap32/css/bootstrap.min.css" rel="stylesheet" />

    <script type="text/javascript" src="/js/ueditor/ueditor.config.js"></script>
    <script type="text/javascript" src="/js/ueditor/ueditor.all.min.js"></script>
    <script type="text/javascript" src="/js/ueditor/lang/zh-cn/zh-cn.js"></script>

    <script src="/js/upload/plupload.full.min.js"></script>
    <script src="/js/pan.js" type="text/javascript"></script>

     <link href="/Extjs42/resources/css/ext-all-gray.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <link href="/css/common.css" rel="stylesheet" />

    <script src="/js/My97DatePicker/WdatePicker.js"></script>

    <script type="text/javascript">
        var option = getQueryString("option"); var ID = getQueryString("ID");
        var uploader, file_store;

        $(document).ready(function () {

            initform(option);

            $("#btnCancel").click(function () {
                window.close();
            });

            $("#btnSubmit").click(function () {
                if ($("#rcbType").val() == "") {
                    Ext.MessageBox.alert("提示", "请输入类型！");
                    return;
                }

                var filedata = Ext.encode(Ext.pluck(file_store.data.items, 'data'));
                $("#rchAttachment").val(filedata);

                document.getElementById("form1").submit();
            });

        });

        function initform(option) {

            var strjoson = eval("<%=Bind_rcbType()%>");
            $("#rcbType").html(creatSelectTree(strjoson));

            panel_file_ini();//随附文件初始化
            if (uploader == null) {
                upload_ini();
            }

            if (option == "update") {
                var rcbType = "<%=rcbType %>";
                if (rcbType != null && rcbType.length > 0) {
                    $("#rcbType").find("option[value='" + rcbType + "']").attr("selected", true);
                }

                var rcbIsinvalid = "<%=rcbValid %>";
                if (rcbIsinvalid != null && rcbIsinvalid.length > 0) {
                    if (rcbIsinvalid == "0") {
                        $("#rd_y").attr("checked", true);
                    }
                    if (rcbIsinvalid == "1") {
                        $("#rd_n").attr("checked", true);
                    }
                }
                var rchAttachment = eval('<%=rchAttachment %>');
                file_store.loadData(rchAttachment);
            }

        }

        var _editor = UE.getEditor('reContent');
        _editor.ready(function () {
            _editor.setContent('<%=reContent %>');
        });

        //生成树下拉菜单
        var j = "";//前缀符号，用于显示父子关系，这里可以使用其它符号
        function creatSelectTree(d) {
            var option = "";
            for (var i = 0; i < d.length; i++) {
                if (d[i].children.length) {//如果有子集
                    option += "<option value='" + d[i].ID + "'>" + j + d[i].NAME + "</option>";
                    //j += "--";//前缀符号加一个符号
                    j += "--";//前缀符号加一个符号
                    option += creatSelectTree(d[i].children);//递归调用子集
                    j = j.slice(0, j.length - 2);//每次递归结束返回上级时，前缀符号需要减两个符号
                } else {//没有子集直接显示
                    option += "<option value='" + d[i].ID + "'>" + j + d[i].NAME + "</option>";
                }
            }
            return option;//返回最终html结果
        }

    </script>
</head>
<body>
    <form id="form1" action="?action=save" method="post" enctype="multipart/form-data">
        <input type="hidden" id="rtbID" name="rtbID" value="<%=rtbID %>" />
        <div class="panel panel-primary">
            <div class="panel-heading">资讯发布-新增||修改</div>
            <div class="panel-body">
                <table class="table table-bordered">
                    <tr>
                        <td style="width: 10%;" align="right">
                            标题<%--<label>标题</label>--%>
                        </td>
                        <td colspan="3">
                            <input type="text" style="width:100%;" id="rtbTitle" name="rtbTitle" value="<%=rtbTitle %>" class="input" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            类别
                        </td>
                        <td colspan="3">
                            <select id="rcbType" name="rcbType" style="width:30%;"></select>
                        </td>
                    </tr>
                    <tr>                        
                        <td style="width: 10%" align="right">
                            发布日期
                        </td>
                        <td>
                            <input type="text" style="width:80%;background-color: #e5f1f4; border-top-style: none; border-right-style: none;border-left-style: none;border-bottom: green 1px solid;" 
                                id="rtbPublishDate" name="rtbPublishDate" value="<%=rtbPublishDate %>"
                                class="Wdate" onclick="WdatePicker({ dateFmt: 'yyyy/MM/dd HH:mm', isShowClear: false, readOnly: true })" readonly="readonly"/>
                        </td>
                        <td style="width: 10%" align="right">
                            是否发布
                        </td>
                        <td>
                            <input id="rd_y" type="radio" name="rcbValid" value="0" />是
                            &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;
                            <input id="rd_n" type="radio" name="rcbValid" value="1" checked="checked" />否
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                           内容
                        </td>
                        <td colspan="3">
                            <textarea id="reContent"  name="reContent" style="width:100%;height:290px;"></textarea>
                        </td>
                    </tr>   
                    <tr>
                        <td style="width: 10%" align="right">
                            本文来源
                        </td>
                        <td colspan="3">
                            <input type="text" style="width:100%;" id="rtbREFERENCESOURCE" name="rtbREFERENCESOURCE" value="<%=rtbREFERENCESOURCE %>" class="input" />
                        </td>
                    </tr>                 
                    <tr>
                        <td align="right">
                            附件
                        </td>
                        <td colspan="3"> 
                            <div class="btn-group">
                                <button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button>
                                <button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>
                            </div>    
                            <div id="div_panel" style="width: 100%"></div>
                            <input id="rchAttachment" name="rchAttachment" type="text" value="<%=rchAttachment %>" hidden="hidden" />
                        </td>
                    </tr>
                </table>

            </div>
            <div class=" panel-footer" style="text-align: center">
                <div class="btn-block">
                    <a id="btnSubmit" class="btn btn-primary btn-sm"><span class="fa fa-floppy-o"></span><strong>保 存</strong></a> 
                    <a id="btnCancel" class="btn btn-primary btn-sm"><span class="fa fa-undo"></span><strong>关 闭</strong></a>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
