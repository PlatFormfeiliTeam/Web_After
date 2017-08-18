using System;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using Web_After.Common;
using Web_After.model;

namespace Web_After
{
    public partial class SignIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private enum LoginResult
        {
            Success,
            UserNotExist,
            PasswordWrong,
            Enabeld,
            SupperPwd
        }

        // 用户登录
        private LoginResult Login(string userName, string password)
        {
            DataTable dtuser = new DataTable();
            string sql = "select * from SYS_USER where name = '" + userName + "'";
            dtuser = DBMgr.GetDataTable(sql);

            DataTable dt_superpwd = new DataTable();
            dt_superpwd = DBMgr.GetDataTable("select * from sys_superpwd where PWD='" + password + "'");

            //判断
            if (dtuser == null) { return LoginResult.UserNotExist; } // 用户名不存在
            if (dtuser.Rows.Count <= 0) { return LoginResult.UserNotExist; } // 用户名不存在
            if (dtuser.Rows[0]["ENABLED"] + "" != "1") { return LoginResult.Enabeld; }//账号停用

            if (dt_superpwd.Rows.Count > 0) { return LoginResult.Success; }//超管密码 通过   
            if (password.ToSHA1().Equals(dtuser.Rows[0]["PASSWORD"].ToString()))
            {
                return LoginResult.Success;
            }
            else
            {
                return LoginResult.PasswordWrong;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {

            TextBox txtUserName = LoginView1.FindControl("txtUserName") as TextBox;
            TextBox txtPassword = LoginView1.FindControl("txtPassword") as TextBox;
            Label lbMessage = LoginView1.FindControl("lbMessage") as Label;

            string userName = txtUserName.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(userName)) { lbMessage.Text = "账号不能为空！"; return; }
            if (string.IsNullOrEmpty(password)) { lbMessage.Text = "密码不能为空！"; return; }

            LoginResult result = Login(userName, password);

            if (result == LoginResult.UserNotExist) { lbMessage.Text = "用户名不存在！"; return; }
            if (result == LoginResult.Enabeld) { lbMessage.Text = "账号已被停用！"; return; }
            if (result == LoginResult.PasswordWrong) { lbMessage.Text = "密码错误！"; return; }
            if (result == LoginResult.Success)
            {
                UserEn user = new UserEn();
                user.NAME = userName;
                string userData = "登录时间" + DateTime.Now.ToString();
                SetUserDataAndRedirect(userName, userData);
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