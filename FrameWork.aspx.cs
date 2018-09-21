using ch.psoft.Util;
using System;

namespace ch.appl.psoft
{
    /// <summary>
    /// Summary description for FrameWork.
    /// </summary>
    public partial class FrameWork : System.Web.UI.Page
    {
        protected string _contentFrameURL = psoft.Basics.contentInit.GetURL();
        protected string _noFramesURL = "";

        protected void Page_Load(object sender, System.EventArgs e)
        {
            _contentFrameURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["contentFrameURL"], _contentFrameURL);
            _noFramesURL = PSOFTConvert.ToJavascript(ch.psoft.Util.Validate.GetValid(Request.QueryString["noFramesURL"], _noFramesURL));

            // redirect to contentInit.aspx if logged in / 27.09.10
            bool _isLogged = Request.IsAuthenticated && !bool.Parse(ch.psoft.Util.Validate.GetValid(Request.QueryString["logout"], "false"));

            if (_isLogged)
            {
                Response.Redirect(Global.Config.baseURL + "/Basics/contentInit.aspx");
            }
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
        private void InitializeComponent() {    
        }
		#endregion
    }
}
