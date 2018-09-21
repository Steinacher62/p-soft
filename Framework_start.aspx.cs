using System;


namespace ch.appl.psoft
{
    public partial class Framework_start : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // redirect to contentInit.aspx if logged in / 27.09.10
            bool _isLogged = Request.IsAuthenticated && !bool.Parse(ch.psoft.Util.Validate.GetValid(Request.QueryString["logout"], "false"));
           
            if (_isLogged)
            {
               
                    Response.Redirect(Global.Config.baseURL + "/Basics/contentInit.aspx");
                
            }
        }
    }
}
