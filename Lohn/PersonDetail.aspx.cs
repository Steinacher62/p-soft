using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Lohn.Controls;
using System;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// Anzeige der History
    /// </summary>
    public partial class PersonDetail : PsoftContentPage
    {
        private const string PAGE_URL = "/Lohn/PersonDetail.aspx";

        static PersonDetail()
        {
            SetPageParams(PAGE_URL, "salaryComponent", "orgId", "lohnId");
        }

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        public PersonDetail() : base()
        {
            PageURL = PAGE_URL;
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.BreadcrumbCaption = _mapper.get("lohn", "history");
            this.BreadcrumbName = "lohnPersonDetail";

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout
                = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("lohn", "history");

            // Setting content layout of page layout
            DGLContentLayout contentControl
                = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentControl;

            // Setting parameters
            PersonDetailControl detail = (PersonDetailControl)this.LoadPSOFTControl(
                    PersonDetailControl.getPath(LohnModule.KundenModuleName),
                    "_detail"
                );
            detail.SalaryComponent = GetQueryValue("salaryComponent", "");
            detail.OrgentityId = GetQueryValue("orgId", (long)-1);
            detail.LohnId = GetQueryValue("lohnId", (long)-1);
            detail.ReadOnly = true;
            detail.Kontext = "history";

            // Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, detail);	
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
		
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {    
            this.ID = "lohnPersonDetail";
        }
		#endregion
    }
}
