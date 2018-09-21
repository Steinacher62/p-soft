using ch.appl.psoft.Admin.Skills.Controls;
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

namespace ch.appl.psoft.Admin.Skills
{
    public partial class SkillsCatalog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LMRORU_Layout.MainTitle = _map.get("skills", "name");
            LMRORU_Layout.pageViewL.Controls.Add((SkillsTreeCtrl)LoadControl("Controls/SkillsTreeCtrl.ascx"));
            LMRORU_Layout.pageViewRO.Controls.Add((SkillSearchCtrl)LoadControl("Controls/SkillSearchCtrl.ascx"));
            LMRORU_Layout.pageViewRU.Controls.Add((SkillsSearchListCtrl)LoadControl("Controls/SkillsSearchListCtrl.ascx"));
            LMRORU_Layout.pageViewM.Controls.Add((SkillDetailCtrl)LoadControl("Controls/SkillDetailCtrl.ascx"));
            if (!IsPostBack)
            {
                RadPageView GroupDetailPageView = new RadPageView();
                GroupDetailPageView.ID = "PageViewM1";
                GroupDetailPageView.EnableViewState = false;
                GroupDetailPageView.Controls.Add((SkillGroupDetailCtrl)LoadControl("Controls/SkillGroupDetailCtrl.ascx"));
                LMRORU_Layout.pageViewM.Parent.Controls.Add(GroupDetailPageView);
            }
        }
    }
}