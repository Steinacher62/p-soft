using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace ch.appl.psoft.LayoutControls
{
    public partial class L_Layout : System.Web.UI.UserControl
    {
        public RadPageView pageViewL { get { return this.PageViewL; } }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Header.Controls.Add(new System.Web.UI.LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Style/admin.css\" />"));
        }
    }
}