<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Decl_HSClass.aspx.cs" Inherits="Web_After.BasicManager.DeclInfor.Decl_HSClass" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/pan.js" type="text/javascript"></script>
    <script src="/BasicManager/js/Chapter.js"></script>
    <script type="text/javascript">
        init_search();
        Ext.onReady(function () {
            var tabpanel = new Ext.TabPanel({
                deferredRender: false,// 默认情况下tab不会被渲染,但是tab间有关联的情况下需要关闭延迟渲染
                //tabPosition: "top", //默认情况下是top
                region: 'center',
                layout: 'border',                
                activeTab: 1, //默认打开第二个
                //Tabpnale里面的选项是一个Panel
                items: [
                    { title: "面板2", items: [formpanel_search] },
                    //{ title: "News", autoLoad: "", closable: true }
                    { title: "News", autoLoad: "" }
                ]
            });

            var viewport = Ext.create('Ext.container.Viewport',
                {
                    layout: 'border',
                    items: [tabpanel]
                });


        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
