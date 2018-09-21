using ch.appl.psoft.Admin.Authorisation.Controls;
using ch.appl.psoft.Admin.Controls;
using ch.appl.psoft.Common;
using ch.appl.psoft.Contact;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Person.Controls;
using ch.appl.psoft.Common;
using ch.psoft.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.admin
{
    public partial class Organisation : PsoftDetailPage
    {
        private RadSplitter splitter;
        private RadMultiPage multiPageLeft;
        private RadPageView organisationPageView;
        protected void Page_Load(object sender, EventArgs e)
        {
            //splitter = (RadSplitter)AdminContentLayout.FindControl("Splitter");
            //splitter.Width = Unit.Percentage(98);
            //splitter.Height = Unit.Percentage(80);

            this.BreadcrumbCaption = _mapper.get("application", "administration");
            this.BreadcrumbLink = Global.Config.baseURL + "/admin/Organisation.aspx";

            multiPageLeft = (RadMultiPage)AdminContentLayout.FindControl("Splitter").FindControl("PaneLeft").FindControl("MultiPageLeft");
            organisationPageView = (RadPageView)multiPageLeft.FindControl("PageViewOrganisation");
            organisationPageView.Controls.Add((OrganisationTreeCtrl)LoadControl("Controls/OrganisationTreeCtrl.ascx"));

            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            AdminContentLayout.MainTitle = _map.get("ORGANISATION", "TITLE");

            AdminContentLayout.pageViewOrganisationDetail.Controls.Add((OrganisationDetailCtrl)LoadControl("Controls/OrganisationDetailCtrl.ascx"));
            AdminContentLayout.pageViewJobDetail.Controls.Add((JobDetailCtrl)LoadControl("Controls/JobDetailCtrl.ascx"));
            AdminContentLayout.pageViewPersonDetail.Controls.Add((PersonDetailCtrl)LoadControl("Controls/PersonDetailCtrl.ascx"));
            AdminContentLayout.FindControl("SplitterRight").FindControl("PaneRightTop").Controls.Add((PersonSearchCtrl)LoadControl("Controls/PersonSearchCtrl.ascx"));
            AdminContentLayout.pageViewPersonTree.Controls.Add((PersonTreeCtrl)LoadControl("Controls/PersonTreeCtrl.ascx"));
            AdminContentLayout.FindControl("WindowManager").FindControl("ClipboardWindow").FindControl("C").FindControl("ClipboardSplitter").FindControl("ClipboardPaneLeft").Controls.Add((ClipboardTreeCtrl)LoadControl("Controls/ClipboardTreeCtrl.ascx"));
            AdminContentLayout.FindControl("WindowManager").FindControl("ClipboardWindow").FindControl("C").FindControl("ClipboardSplitter").FindControl("ClipboardPaneRight").Controls.Add((ClipboardDetailCtrl)LoadControl("Controls/ClipboardDetailCtrl.ascx"));
            AdminContentLayout.FindControl("WindowManager").FindControl("EmploymentWindow").FindControl("C").Controls.Add((EmploymentDetailCtrl)LoadControl("Controls/EmploymentDetailCtrl.ascx"));
            AdminContentLayout.FindControl("WindowManager").FindControl("AuthorisationWindow").FindControl("C").FindControl("AuthorisationSplitter").FindControl("AuthorisationPaneTop").Controls.Add((AuthorisationUserCtrl)LoadControl("Authorisation/Controls/AuthorisationUserCtrl.ascx"));
            AdminContentLayout.FindControl("WindowManager").FindControl("AuthorisationWindow").FindControl("C").FindControl("AuthorisationSplitter").FindControl("AuthorisationPaneBottom").Controls.Add((AuthorisationPermissionCtrl)LoadControl("Authorisation/Controls/AuthorisationPermissionCtrl.ascx"));
            AdminContentLayout.FindControl("WindowManager").FindControl("AccessorWindow").FindControl("C").Controls.Add((AccessorCtrl)LoadControl("Authorisation/Controls/AccessorCtrl.ascx"));
            AdminContentLayout.FindControl("WindowManager").FindControl("AddressWindow").FindControl("C").Controls.Add((AddressDetailCtrl)LoadControl("Controls/AddressDetailCtrl.ascx"));

            if (!Page.IsPostBack)
            {
                RadTabStrip tabstripCenter = (RadTabStrip)AdminContentLayout.FindControl("Splitter").FindControl("PaneCenter").FindControl("TabStripCenter");
                RadTab organisationTab = new RadTab();
                organisationTab.PageViewID = "PageViewOrganisationDetail";
                organisationTab.Value = "orgentity";
                organisationTab.Text = "Abteilung";
                tabstripCenter.Tabs.Add(organisationTab);

                RadTab jobTab = new RadTab();
                jobTab.PageViewID = "PageViewJobDetail";
                jobTab.Value = "job";
                jobTab.Text = "Stelle";
                tabstripCenter.Tabs.Add(jobTab);

                RadTab personTab = new RadTab();
                personTab.PageViewID = "PageViewPersonDetail";
                personTab.Text = "Person";
                personTab.Value = "person";
                tabstripCenter.Tabs.Add(personTab);
            }
        }
    }
}