using ch.appl.psoft.Admin.Authorisation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Admin.Authorisation
{
    public partial class ApplicationPermissions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ApplicationAuthorisationWindow.FindControl("C").FindControl("AuthorisationSplitter").FindControl("AuthorisationPaneTop").Controls.Add((AuthorisationUserCtrl)LoadControl("Controls/AuthorisationUserCtrl.ascx"));
            ApplicationAuthorisationWindow.FindControl("C").FindControl("AuthorisationSplitter").FindControl("AuthorisationPaneBottom").Controls.Add((AuthorisationPermissionCtrl)LoadControl("Controls/AuthorisationPermissionCtrl.ascx"));
            AccessorWindow.FindControl("C").Controls.Add((AccessorCtrl)LoadControl("Controls/AccessorCtrl.ascx"));
        }
    }
}