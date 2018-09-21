using ch.appl.psoft.Admin.Menus.Controls;
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



namespace ch.appl.psoft.Admin.Menus
{
    public partial class OrganisationMenu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LMRORU_Layout.MainTitle = _map.get("orgMenu", "orgMenu");
            LMRORU_Layout.pageViewL.Controls.Add((OrganisationMenuTreeCtrl)LoadControl("Controls/OrganisationMenuTreeCtrl.ascx"));
            LMRORU_Layout.pageViewRO.Controls.Add((SearchMenuItemCtrl)LoadControl("Controls/SearchMenuItemCtrl.ascx"));
            LMRORU_Layout.pageViewRU.Controls.Add((SearchListItemsCtrl)LoadControl("Controls/SearchListItemsCtrl.ascx"));
            LMRORU_Layout.pageViewM.Controls.Add((MenuGroupDetailCtrl)LoadControl("Controls/MenuGroupDetailCtrl.ascx"));
            if (!IsPostBack)
            {
                RadPageView GroupDetailPageView = new RadPageView();
                GroupDetailPageView.ID = "PageViewM1";
                GroupDetailPageView.EnableViewState = false;
                GroupDetailPageView.Controls.Add((MenuItemDetailCtrl)LoadControl("Controls/MenuItemDetailCtrl.ascx"));
                LMRORU_Layout.pageViewM.Parent.Controls.Add(GroupDetailPageView);
            }
           
        }
    }
}