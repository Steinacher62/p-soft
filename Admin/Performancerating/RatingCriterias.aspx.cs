using ch.appl.psoft.Admin.Performancerating.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin.Performancerating
{
    public partial class RatingCriterias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            
            LOLUMOMU_Layout.pageViewLO.Controls.Add((CriteriaDetailCtrl)LoadControl("Controls/CriteriaDetailCtrl.ascx"));
            LOLUMOMU_Layout.pageViewLU.Controls.Add((CriteriaFunctionratingLink)LoadControl("Controls/CriteriaFunctionratingLink.ascx"));
            LOLUMOMU_Layout.pageViewMO.Controls.Add((CriteriaSearchCtrl)LoadControl("Controls/CriteriaSearchCtrl.ascx"));
            LOLUMOMU_Layout.pageViewMU.Controls.Add((CriteriaSearchListCtrl)LoadControl("Controls/CriteriaSearchListCtrl.ascx"));
        }
    }
}