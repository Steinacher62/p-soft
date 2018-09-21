using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;
using System;

namespace ch.appl.psoft.Project
{
    public partial class BillingAdd : PsoftTreeViewPage
    {
        private const string PAGE_URL = "/Project/BillingAdd.aspx";



        static BillingAdd() {
            SetPageParams(PAGE_URL, "projectID");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public BillingAdd()
            : base()
        {
            PageURL = PAGE_URL;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            BreadcrumbCaption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_ADDBILLING);

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = BreadcrumbCaption;

            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting parameters
            BillingAddCtrl pAdd = (BillingAddCtrl)LoadPSOFTControl(BillingAddCtrl.Path, "_ad");
            pAdd.ProjectID = ch.psoft.Util.Validate.GetValid(Request.QueryString["projectID"], -1);

            // Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, pAdd);		

        }
    }
}
