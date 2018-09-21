using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace ch.appl.psoft.LayoutControls
{
    public partial class LOLUMOMURORU_Layout : System.Web.UI.UserControl
    {
        public RadPageView pageViewLO { get { return this.PageViewLO; } }
        public RadPageView pageViewLU { get { return this.PageViewLU; } }
        public RadPageView pageViewMO { get { return this.PageViewMO; } }
        public RadPageView pageViewMU { get { return this.PageViewMU; } }
        public RadPageView pageViewRO { get { return this.PageViewRO; } }
        public RadPageView pageViewRU { get { return this.PageViewRU; } }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Header.Controls.Add(new System.Web.UI.LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"../../Style/admin.css\" />"));
        }
    }
}