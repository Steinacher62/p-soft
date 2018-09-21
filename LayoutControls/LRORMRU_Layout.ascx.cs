using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.LayoutControls
{
    public partial class LRORMRU_Layout1 : System.Web.UI.UserControl
    {
        public RadPageView pageViewL { get { return this.PageViewL; } }
        public RadPageView pageViewRO { get { return this.PageViewRO; } }
        public RadPageView pageViewRM { get { return this.PageViewRM; } }
        public RadPageView pageViewRU { get { return this.PageViewRU; } }
        public string MainTitle { get { return this.MainContainerTitle.Text; } set { this.MainContainerTitle.Text = value; } }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Header.Controls.Add(new System.Web.UI.LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Style/admin.css\" />"));
        }
    }
}