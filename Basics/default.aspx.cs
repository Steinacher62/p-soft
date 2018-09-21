using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace ch.appl.psoft
{
    /// <summary>
    /// Summary description for _default.
    /// </summary>
    public partial class _default : System.Web.UI.Page
	{
        protected string _returnURL = "";
        protected string _logosURL = "logos.htm";
        protected bool _logout = false;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            _returnURL = HttpUtility.UrlEncode(ch.psoft.Util.Validate.GetValid(Request.QueryString["ReturnURL"], _returnURL));
            _logout = bool.Parse(ch.psoft.Util.Validate.GetValid(Request.QueryString["logout"], "false"));
            if (!Global.Config.showPartnerLogos){
                _logosURL = "noLogos.htm";
            }

            if (_logout)
            {
                if (!Session.IsNewSession)
                    Session.Abandon();

                FormsAuthentication.SignOut();
            }
            else
            {
                if (Request.IsAuthenticated) {
                    Response.Redirect(HttpUtility.UrlDecode(_returnURL));
                }
                else if (Global.Config.authenticationMode == Config.AUTHENTICATION_MODE.ANONYMOUS || Global.Config.authenticationMode == Config.AUTHENTICATION_MODE.WINDOWS){
                    DBData db = DBData.getDBData(Session);
                    db.connect();
                    try{
                        string username = "";
                        string password = "";
                        switch (Global.Config.authenticationMode){
                            case Config.AUTHENTICATION_MODE.ANONYMOUS:
                                username = Global.Config.anonymousAccount;
                                password = Global.Config.anonymousPassword;
                                break;

                            case Config.AUTHENTICATION_MODE.WINDOWS:
                                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                                username = identity.Name;
                                password = null;
                                break;

                            default:
                                break;
                        }
                        if (db.Person.login(username, password) == 1)
                        {
                            Response.Redirect(Request.RawUrl);  // redirect to original page to ensure the request is authenticated.
                        }

                    }
                    catch (Exception ex) {
                        Logger.Log(ex,Logger.ERROR);
                    }
                    finally {
                        db.disconnect();
                    }
                }
                

            }
            Response.Redirect("login.aspx");
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
