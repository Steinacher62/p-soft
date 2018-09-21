using ch.appl.psoft.Admin.Authorisation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace ch.appl.psoft.admin
{
    public partial class UserGroups : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LOLUMOMU_Layout.pageViewMO.Controls.Add((AccessorSearchCtrl)LoadControl("Controls/AccessorSearchCtrl.ascx"));
            LOLUMOMU_Layout.pageViewMU.Controls.Add((AccessorListCtrl)LoadControl("Controls/AccessorListCtrl.ascx"));
            LOLUMOMU_Layout.pageViewLU.Controls.Add((AccessorGroupDetailCtrl)LoadControl("Controls/AccessorGroupDetailCtrl.ascx"));
            LOLUMOMU_Layout.pageViewLO.Controls.Add((AccessorDetail)LoadControl("Controls/AccessorDetail.ascx"));

        }
    }
}