
using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;


namespace ch.appl.psoft.Project
{

    /// <summary>
    /// Class for editing project billing.
    /// </summary>
    public partial class BillingDetail : PsoftTreeViewPage
    {
        private const string PAGE_URL = "/Project/BillingDetail.aspx";

        static BillingDetail()
        {
            SetPageParams(PAGE_URL, "ID", "projectID");
        }

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }


        public BillingDetail()
            : base()
        {
            PageURL = PAGE_URL;
        }


        protected void Page_Load(object sender, System.EventArgs e)
        {
            BreadcrumbCaption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_EDITBILLING);

            long billingID = GetQueryValue("ID", -1L);
            long projectID = GetQueryValue("projectID", -1L);


            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = BreadcrumbCaption;

            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting parameters
            BillingEditCtrl pEditControl = (BillingEditCtrl)LoadPSOFTControl(BillingEditCtrl.Path, "_ad");
            pEditControl.BillingID = billingID;
            pEditControl.ProjectID = projectID;

            // Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, pEditControl);		
        }

    }
}
