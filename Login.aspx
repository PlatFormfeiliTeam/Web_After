<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Web_After.SignIn" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="css/skin/default/style.css" rel="stylesheet" />
    <style type="text/css">
        .juzhong {
            margin: 0 auto;
            width: 1000px;
            margin-top: 100px;
        }
    </style>

</head>
<body class="loginbody">
    <form id="form1" runat="server">
        <div style="width: 100%; height: 100%; min-width: 300px; min-height: 260px;"></div>
        <div class="login-wrap">

            <asp:LoginView ID="LoginView1" runat="server">
                <LoggedInTemplate>
                    <asp:LoginName ID="LoginName1" runat="server" />
                    ，你已经登录了^_^
                    <br />
                    <a href="Default.aspx"><font size="5" color="red">转到后台！</font></a> 
                    <br />
                </LoggedInTemplate>
                <AnonymousTemplate>
                    <div class="login-form">
                        <div class="col">
                            <asp:TextBox ID="txtUserName" runat="server" CssClass="login-input" placeholder="请输入账号" title="登录账号"></asp:TextBox>
                            <label class="icon user" for="txtUserName"></label>
                        </div>
                        <div class="col">
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="login-input" TextMode="Password" placeholder="请输入密码" title="登录密码"></asp:TextBox>
                            <label class="icon pwd" for="txtPassword"></label>
                        </div>
                        <div class="col">
                            <asp:Button ID="btnLogin" runat="server" Text="登 录" CssClass="login-btn" OnClick="btnLogin_Click" />
                        </div>
                    </div>
                    <div class="login-tips">
                        <i></i>
                        <asp:Label ID="lbMessage" runat="server" ForeColor="Red" Text="请输入用户名和密码"></asp:Label>
                    </div>
                </AnonymousTemplate>
            </asp:LoginView>


        </div>

    </form>
</body>
</html>
