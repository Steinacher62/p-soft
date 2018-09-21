using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Novis.Controls;
using System;

namespace ch.appl.psoft.Novis
{
    public partial class CreateNovisReport : PsoftTreeViewPage
    {
        // Fields
        private CreateNovisReportCtrl _report;
        public const string MODE_SLAVE = "slave";
        public const string ORDER_COLUMN = "TITLE";
        private const string PAGE_URL = "/Novis/CreateNovisReport.aspx";

        // Methods
       

        public static string GetURL(params object[] queryParams)
        {
            return PsoftPage.CreateURL("/Novis/CreateNovisReport.aspx", queryParams);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        private void InitializeComponent()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)base.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            base.PageLayoutControl = PsoftPageLayout;
            DGLContentLayout layout = (DGLContentLayout)base.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            base.PageLayoutControl.ContentLayoutControl = layout;

            base.BreadcrumbCaption = "Report erstellen"; //base._mapper.get("novis", "bcCreateNovisReport");
            this._report = (CreateNovisReportCtrl)base.LoadPSOFTControl(CreateNovisReportCtrl.Path, "_report");
            base.SetPageLayoutContentControl("DETAIL", this._report);
            (base.PageLayoutControl as PsoftPageLayout).PageTitle = base.BreadcrumbCaption; 
        }
    }
}