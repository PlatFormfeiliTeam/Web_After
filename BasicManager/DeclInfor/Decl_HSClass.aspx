<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Decl_HSClass.aspx.cs" Inherits="Web_After.BasicManager.DeclInfor.Decl_HSClass" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="/Extjs42/resources/css/ext-all-neptune.css" rel="stylesheet" type="text/css" />
    <script src="/Extjs42/bootstrap.js" type="text/javascript"></script>
    <script src="/js/jquery-1.8.2.min.js"></script>
    <link href="/css/iconfont/iconfont.css" rel="stylesheet" />
    <script type="text/javascript">
        var username = '<%=Username()%>';
    </script>   
    <script src="/BasicManager/js/Chapter.js"></script>
    <script src="/BasicManager/js/Category.js"></script>
    <script src="/BasicManager/js/Smallclass.js"></script>
    <script type="text/javascript">
        init_search();
        gridbind();
        init_search_category();
        gridbind_category();
        init_search_smallclass();
        gridbind_smallclass();
        Ext.onReady(function () {
            runajax();
            var tabpanel = new Ext.TabPanel({
                deferredRender: false,// 默认情况下tab不会被渲染,但是tab间有关联的情况下需要关闭延迟渲染
                region: 'center',
                layout: 'border',                
                activeTab: 0, //默认打开第二个
                //Tabpnale里面的选项是一个Panel
                items: [
                    { title: "商检HS所属类别", items: [formpanel_search_category, gridpanel_category] },
                    //{ title: "News", autoLoad: "", closable: true }
                    { title: "商检HS所属章节", items: [formpanel_search, gridpanel] },
                    { title: "商检HS所属小类", items: [formpanel_search_smallclass, gridpanel_smallclass] }
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
