using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.LayoutControls
{
    public partial class AdminContentLayout : System.Web.UI.UserControl
    {
        public RadPageView pageViewOrganisation { get { return this.PageViewOrganisation; } }
        public RadPageView pageViewPersonDetail { get { return this.PageviewPersonDetail; } }
        public RadPageView pageViewOrganisationDetail { get { return this.PageViewOrganisationDetail; } }
        public RadPageView pageViewJobDetail { get { return this.PageViewJobDetail; } }
        public RadPageView pageViewPersonTree { get { return this.PageViewPersonTree; } }

        public string MainTitle { get { return this.MainContainerTitle.Text; } set { this.MainContainerTitle.Text = value; } }


        protected void Page_Load(object sender, EventArgs e)
        {
            imageUrl.Value = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("Organisation.aspx")) + "images/";
            Page.Header.Controls.Add(new System.Web.UI.LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"../Style/admin.css\" />"));
        }


    }
}