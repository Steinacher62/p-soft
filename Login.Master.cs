using System;

namespace ch.appl.psoft
{
    public partial class Login : System.Web.UI.MasterPage
    {
        public string AppLogo;
        protected void Page_Load(object sender, EventArgs e)
        {
            string LogoPath = "Kundenspezifisch/" + Global.Config.ApplicationName +"/LoginLogo.png";

                AppLogo = "<img src=\"../images/"+ LogoPath + "\" style=\"display: block; margin-left: auto; margin-right:auto; padding-top:15px; padding-bottom:10px;\"/>";
           // RadSkinManager.Skin = "WebBlue";
        }
    }
}
