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
    public partial class FunctionDescription : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LMRORU_Layout.MainTitle = _map.get("functionDescription", "name");
            LMRORU_Layout.pageViewL.Controls.Add((FunctionDescriptionTreeCtrl)LoadControl("Controls/FunctionDescriptionTreeCtrl.ascx"));
            LMRORU_Layout.pageViewM.Controls.Add((FunctionDescriptionDetailCtrl)LoadControl("Controls/FunctionDescriptionDetailCtrl.ascx"));
            LMRORU_Layout.pageViewRO.Controls.Add((DutySearchCtrl)LoadControl("Controls/DutySearchCtrl.ascx"));
            LMRORU_Layout.pageViewRU.Controls.Add((FunctionDescriptionDutyTreeCtrl)LoadControl("Controls/FunctionDescriptionDutyTreeCtrl.ascx"));
            if (!IsPostBack)
            {
                RadPageView GroupDetailPageView = new RadPageView();
                GroupDetailPageView.ID = "PageViewM1";
                GroupDetailPageView.EnableViewState = false;
                GroupDetailPageView.Controls.Add((FunctionDescriptionDutyDetailCtrl)LoadControl("Controls/FunctionDescriptionDutyDetailCtrl.ascx"));
                LMRORU_Layout.pageViewM.Parent.Controls.Add(GroupDetailPageView);
            }
        }
    }
}