﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Web_After.SysFrame" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js" type="text/javascript"></script>
    <link href="/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        a {
            cursor: pointer;
            text-decoration: none;
        }
    </style>
    <script type="text/javascript">
        Ext.onReady(function () {
            Ext.regModel("Model", { fields: ['id', 'name', 'leaf', 'url'] });
            var store = new Ext.data.TreeStore({
                model: 'Model',
                nodeParam: 'id',
                proxy: {
                    type: 'ajax',
                    url: 'Default.aspx',
                    reader: 'json'
                },
                root: {
                    //expanded: true,
                    name: '功能菜单',
                    id: '-1'
                }
            });
            var treepanel = Ext.create('Ext.tree.Panel', {
                title: '功能菜单',
                useArrows: true,
                region: 'west',
                collapsible: true,
                hideHeaders: true,
                animate: true,
                rootVisible: false,
                width: 200,
                store: store,
                columns: [
                { text: 'id', dataIndex: 'id', width: 100, hidden: true },
                { text: 'leaf', dataIndex: 'leaf', width: 100, hidden: true },
                { xtype: 'treecolumn', text: '名称', dataIndex: 'name', width: 170 }
                ],
                hrefTarget: 'mainContent',
                listeners: {
                    itemclick: function (view, rec, item, index, e) {
                        Ext.getDom("contentIframe").src = rec.get("url");
                    }
                }
            });
            var viewport = new Ext.container.Viewport({
                layout: 'border',
                items: [{
                    height: 80,
                    region: 'north',
                    html: '<table border="1" cellspacing="0"  style="width:100%; height:78px; background-image:url(images/head/lantp.png);border:none; position:absolute" ><tr ><td style="border:none;width:187px; height:78px "></td><td style=" margin-right:200px;line-height:78px;height:78px;vertical-align:middle; border:none;font-size:35px; text-align:center; color:rgb(255,255,255);font-weight:bold">江苏飞力达关务系统</td><td style="line-height:78px;height:78px;vertical-align:middle; border:none;font-size:20px; text-align:right; color:rgb(255,255,255);font-weight:bold"><%=userName%></td><td style="border:none;width:187px; height:78px"><a style="text-decoration:underline; font-size:17px;" id="logout"><img src="images/head/zx3.png"/></a></td></tr></table>'
                }, treepanel,
                {
                    region: 'center',
                    layout: 'fit',
                    id: 'mainContent',
                    collasible: true,
                    margin: '-1 0 0 0',
                    contentEl: 'contentIframe'
                }]
            })
            treepanel.expandAll();
            $("#logout").click(function () {
                $.post("Default.aspx?action=logout", {}, function (rtn) {
                    window.location.href = "Login.aspx";
                });
            });
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:LoginView ID="LoginView1" runat="server">
                <AnonymousTemplate>
                    <div style="margin: 0 auto; width: 1000px; margin-top: 100px;">
                        <font size="5">欢迎访问, 游客 !</font> <a href="Login.aspx"><font size="5" color="red">请登录！</font></a>
                    </div>
                </AnonymousTemplate>
                <LoggedInTemplate>
                    <iframe id="contentIframe" width="100%" height="100%" name="mainContent" frameborder="no" border="0" marginwidth="0" marginheight="0"></iframe>
                </LoggedInTemplate>
            </asp:LoginView>
            <br />
        </div>
    </form>
</body>
</html>
