using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;



namespace ch.appl.psoft.Basics
{
    /// <summary>
    /// Summary description for login.
    /// </summary>
    public partial class login : System.Web.UI.Page
    {

        protected String _loginError = "";
        //protected long _userID = -1;
        //protected long _userAccessorID = -1;
        protected LanguageMapper _map = null;


        [DllImport("advapi32.dll")]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword,
            int dwLogonType, int dwLogonProvider, out int phToken);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!Global.Config.ShowPasswordChange)
            {
                ((HyperLink)Login1.FindControl("changePasswordLink")).Visible = false;
            }
            if (!Global.Config.ShowPasswordrecovery)
            {
                ((HyperLink)Login1.FindControl("forgotPasswordLink")).Visible = false;
            }
            if (!Global.Config.ShowNewAccount)
            {
                ((HyperLink)Login1.FindControl("newAccountLink")).Visible = false;
            }
            _map = LanguageMapper.getLanguageMapper(Session);
            if (_map == null)
                _map = LanguageMapper.getLanguageMapper(Application);

            ((Label)Login1.FindControl("TitleLabelLogin")).Text = _map.get("titleLogin");
            ((Label)Login1.FindControl("UserNameLabel")).Text = _map.get("loginUsername");
            ((Label)Login1.FindControl("PasswordLabel")).Text = _map.get("loginPassword");
            ((Button)Login1.FindControl("LoginButton")).Text = _map.get("loginButton");
            if (!(Global.Config.authenticationMode == Config.AUTHENTICATION_MODE.WINDOWS_CONFIRM) || !(Global.Config.authenticationMode == Config.AUTHENTICATION_MODE.WINDOWS))
            {
                ((HyperLink)Login1.FindControl("changePasswordLink")).Text = _map.get("changePassword");
                ((HyperLink)Login1.FindControl("forgotPasswordLink")).Text = _map.get("forgotPassword");
                if (Global.Config.ShowNewAccount)
                {
                    ((HyperLink)Login1.FindControl("newAccountLink")).Text = _map.get("newAccount");
                }
            }

            ((RequiredFieldValidator)Login1.FindControl("PasswordRequired")).Text = _map.get("passwordRequired");
            ((RequiredFieldValidator)Login1.FindControl("UserNameRequired")).Text = _map.get("userNameRequired");

            if (!IsPostBack && (Global.Config.authenticationMode == Config.AUTHENTICATION_MODE.WINDOWS_CONFIRM || Global.Config.authenticationMode == Config.AUTHENTICATION_MODE.WINDOWS))
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                if (!Global.isModuleEnabled("habasit"))
                {
                    Login1.UserName = identity.Name;
                }

                string test = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
        }




        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            //InitializeComponent();
            //base.OnInit(e);
            //        // mapControls();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        protected void LoginButton_Click1(object sender, EventArgs e)
        {

        }
        protected void Login1_Authenticate(object sender, System.Web.UI.WebControls.AuthenticateEventArgs e)
        {
            DBData db = DBData.getDBData(Session);  

            bool isWindowsLogin = Global.Config.authenticationMode == Config.AUTHENTICATION_MODE.WINDOWS_CONFIRM || Global.Config.authenticationMode == Config.AUTHENTICATION_MODE.WINDOWS;
            bool isLoggedOn = false;

            // disable windows login if username doesn't contain "\" / 18.10.10 / mkr
            if (!Login1.UserName.Contains("\\"))
            {
                isWindowsLogin = false;
            }

            if (isWindowsLogin)
            {
                // The Windows NT user token.
                int token1;

                int pos = Login1.UserName.IndexOf('\\');
                string domain = Login1.UserName.Substring(0, pos);
                string username = Login1.UserName.Substring(pos + 1);

                // Get the user token for the specified user, machine, and password using the unmanaged LogonUser method.
                isLoggedOn = LogonUser(
                    username,
                    domain,
                    Login1.Password,

                    // Logon type = LOGON32_LOGON_NETWORK_CLEARTEXT.
                    3,

                    // Logon provider = LOGON32_PROVIDER_DEFAULT.
                    0,

                    // The user token for the specified user is returned here.
                    out token1);
            }

            db.connect();
            try
            {
                int logReturn = 0;

                // always fail login if no password submitted / 18.10.10 / mkr
                if (Login1.Password == "")
                {
                    logReturn = 2;
                }
                else
                {
                    if (isWindowsLogin)
                    {
                        if (isLoggedOn)
                        {
                            logReturn = db.Person.windowsLogin(Login1.UserName);
                        }
                    }
                    else
                    {
                        logReturn = db.Person.login(Login1.UserName, Login1.Password);
                    }
                }

                switch (logReturn)
                {
                    case 0:
                        _loginError = _map.get("loginFailed");
                        break;
                    case 1:
                        string originalURL = FormsAuthentication.GetRedirectUrl(Login1.UserName, false);
                        //string redirectURL = "../FrameWork_master.aspx";
                        string redirectURL = "contentInit.aspx";
                        if (originalURL.IndexOf(psoft.Goto.PAGE_URL) >= 0 || originalURL.IndexOf(psoft.GotoCMSPage.PAGE_URL) >= 0)
                        {
                            if (originalURL.ToLower().IndexOf("noframes=true") > 0)
                            {
                                redirectURL += "?noFramesURL=" + HttpUtility.UrlEncode(originalURL);
                            }
                            else
                            {
                                redirectURL += "?contentFrameURL=" + HttpUtility.UrlEncode(originalURL);
                            }
                        }
                        else if (Global.Config.startUpURL != "")
                        {
                            //redirectURL += "?contentFrameURL=" + HttpUtility.UrlEncode(Global.Config.baseURL + Global.Config.startUpURL);
                            if (Global.Config.startUpURL != "myPage")
                                redirectURL = Global.Config.baseURL + Global.Config.startUpURL;
                            else
                                redirectURL = Global.Config.baseURL + "/Person/DetailFrame.aspx?ID=" + db.lookup("ID", "PERSON", "LOGIN = '" + Login1.UserName + "'") + "&mode=oe";
                        }
                        Response.Redirect(redirectURL, false);
                        break;
                    case 2:
                        _loginError = _map.get("loginNoPassword");
                        break;
                    default:
                        _loginError = _map.get("loginRedundant");
                        break;
                }
                Login1.FailureText = _loginError;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
            finally
            {
                db.disconnect();
            }
        }
    }
}

