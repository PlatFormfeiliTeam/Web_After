using System;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using Web_After.Common;
using Web_After.model;

namespace Web_After
{
    public partial class SignIn : System.Web.UI.Page
    {
        private enum LoginResult
        {
            Success,
            UserNotExist,
            PasswordWrong
        }

        // 用户登录
        private LoginResult Login(string userName, string password)
        {
            string validPassword;  // 包含正确的密码

            // 判断用户名是否正确
            if (Extension.IsValidUserST(userName, out validPassword))
            {
                // 判断密码是否正确
                if (password.ToSHA1().Equals(validPassword))
                    return LoginResult.Success;
                else
                    return LoginResult.PasswordWrong;
            }

            // 用户名不存在
            return LoginResult.UserNotExist;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {

            TextBox txtUserName = LoginView1.FindControl("txtUserName") as TextBox;
            TextBox txtPassword = LoginView1.FindControl("txtPassword") as TextBox;
            Label lbMessage = LoginView1.FindControl("lbMessage") as Label;

            string userName = txtUserName.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(userName))
            {
                lbMessage.Text = "账号不能为空！";
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                lbMessage.Text = "密码不能为空！";
                return;
            }

            LoginResult result = Login(userName, password);

            string userData = "登录时间" + DateTime.Now.ToString();

            if (result == LoginResult.Success)
            {
                UserEn user = new UserEn();
                user.NAME = userName;
                Session["WebManageUserInfo"] = user;
                SetUserDataAndRedirect(userName, userData);
            }
            else if (result == LoginResult.UserNotExist)
            {
                lbMessage.Text = "用户名不存在！";
            }
            else
            {
                lbMessage.Text = "密码有误！";
            }

        }

        // 添加自定义的值，然后导航到来到此页面之前的位置
        private void SetUserDataAndRedirect(string userName, string userData)
        {
            // 获得Cookie
            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(userName, true);

            // 得到ticket凭据
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

            // 根据之前的ticket凭据创建新ticket凭据，然后加入自定义信息
            FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(
                ticket.Version, ticket.Name, ticket.IssueDate,
                ticket.Expiration, ticket.IsPersistent, userData);

            // 将新的Ticke转变为Cookie值，然后添加到Cookies集合中
            authCookie.Value = FormsAuthentication.Encrypt(newTicket);
            HttpContext.Current.Response.Cookies.Add(authCookie);

            // 获得 来到登录页之前的页面，即url中return参数的值
            string url = FormsAuthentication.GetRedirectUrl(userName, true);

            if (url=="/")
            {
                url = "/default.aspx";
            }
            Response.Redirect(url);
        }

    }
}