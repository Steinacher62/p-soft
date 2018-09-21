using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Base class for Sub-Navigation menu implementation.
    /// </summary>
    public class SubNavMenuPage : System.Web.UI.Page
	{
        protected string _colorItem = "";
        protected string _eleID = "";
        protected int _id = -1;
       
        public SubNavMenuPage()
        {
            PreRender += new EventHandler(addOnClickHighlight);
            Load += new System.EventHandler(preLoad);
        }

        protected void preLoad(object sender, System.EventArgs e)
        {
            _colorItem = ch.psoft.Util.Validate.GetValid(Request.QueryString["colorItem"],"").ToLower();
            _eleID = ch.psoft.Util.Validate.GetValid(Request.QueryString["eleID"],"0");
            _id = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"],-1);
            Page_Load(sender, e);
        }

        protected virtual void Page_Load(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// Adds an onClick eventhandler to all enabled hyperlinks on page not currently having an 'onClick' attribute
        /// to highlight the clicked menu-item.
        /// </summary>
        protected void addOnClickHighlight(object sender, System.EventArgs e)
        {
            foreach (Control c in Controls)
            {
                addOnClickHighlight(c);
            }
        }

        protected void addOnClickHighlight(Control control)
        {
            if (control.GetType() == typeof(HyperLink))
            {
                HyperLink h = (HyperLink) control;
                if (h.Enabled && h.NavigateUrl != "")
                {
                    if (h.Attributes["onClick"] == null)
                        h.Attributes.Add("onClick","top.highlightSubNavigation('" + h.ID + "')");
                }
            }
            else
            {
                foreach (Control c in control.Controls)
                {
                    addOnClickHighlight(c);
                }
            }
        }

    }
}
