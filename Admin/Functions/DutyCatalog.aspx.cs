using ch.appl.psoft.Admin.Functions.Controls;
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

namespace ch.appl.psoft.Admin.Functions
{
    public partial class DutyCatalog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LMRORU_Layout.MainTitle = _map.get("functionDescription", "dutyCatalog");
            LMRORU_Layout.pageViewL.Controls.Add((DutyTreeCtrl)LoadControl("Controls/DutyTreeCtrl.ascx"));
            LMRORU_Layout.pageViewM.Controls.Add((DutyDetailCtrl)LoadControl("Controls/DutyDetailCtrl.ascx"));
            LMRORU_Layout.pageViewRO.Controls.Add((DutySearchCtrl)LoadControl("Controls/DutySearchCtrl.ascx"));
            LMRORU_Layout.pageViewRU.Controls.Add((DutySearchListCtrl)LoadControl("Controls/DutySearchListCtrl.ascx"));
            if (!IsPostBack)
            {
                RadPageView GroupDetailPageView = new RadPageView();
                GroupDetailPageView.ID = "PageViewM1";
                GroupDetailPageView.EnableViewState = false;
                GroupDetailPageView.Controls.Add((DutyGroupDetailCtrl)LoadControl("Controls/DutyGroupDetailCtrl.ascx"));
                LMRORU_Layout.pageViewM.Parent.Controls.Add(GroupDetailPageView);
            }

        }
    }
}