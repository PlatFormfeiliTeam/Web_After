<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PdfTab.aspx.cs" Inherits="Web_After.PdfTab" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="css/bootstrap32/css/bootstrap.min.css" rel="stylesheet" />
    <script src="js/jquery-1.8.2.min.js"></script>
    <script src="css/bootstrap32/js/bootstrap.min.js"></script>
    <script src="js/pan.js"></script>
    <script type="text/javascript">
        var ordercode = getQueryString("ordercode");
        $(function () {
            $.ajax({
                type: 'Post',
                url: "PdfTab.aspx",
                dataType: 'text',
                data: { action: "load", ordercode: ordercode },
                async: false,
                success: function (data) {
                    var obj = eval("(" + data + ")");//将字符串转为json
                    if (obj.success) {
                        var json = eval(obj.rows);
                        var strul = "";
                        $.each(json, function (idx, item) {
                            if (idx == 0) {
                                var content = '<embed width="100%" height="100%" src="/file/' + item.FILENAME + '" />';
                                $('#pdfdiv').html(content);
                                $('embed').height($(document).height() - $(".nav").height() - 60);
                            } 
                            strul += '<button class="btn btn-primary" id="' + item.FILENAME + '">' + item.FILETYPENAME + '</button>';
                        });
                        $('.btn-group').html(strul)
                        $("button").bind('click', function () {
                            $('#pdfdiv').html('');
                            var content = '<embed width="100%" height="100%" src="/file/' + this.id + '" />';
                            $('#pdfdiv').html(content);
                        });
                    }
                }
            });
        });
    </script>
</head>
<body>
    <div class="btn-group">
    </div>
    <div id="pdfdiv">
    </div>
</body>
</html>
