using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;
using System;

namespace ch.appl.psoft.Project
{

    public partial class ProjectSummary : PsoftDetailPage {

 
        public const string ARGNAME_URL_ID = "xID";

        public const string ARGNAME_URL_CONTEXT = "context";

        public const string ARGNAME_URL_PRINT = "print";


  
        private const string PAGE_URL = "/Project/ProjectSummary.aspx";

        static ProjectSummary() {
            SetPageParams(PAGE_URL, ARGNAME_URL_CONTEXT, ARGNAME_URL_ID, ARGNAME_URL_PRINT);
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        protected void Page_Load(object sender, System.EventArgs e) 
        {
			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;
			PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_PROJECT_SUMMARY);

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)LoadPSOFTControl(DGLContentLayout.Path, "_cl");
			PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting control
            ProjectSummaryCtrl ps = (ProjectSummaryCtrl)LoadPSOFTControl(ProjectSummaryCtrl.Path, "_ps");
            ps.xID = ch.psoft.Util.Validate.GetValid(Request.QueryString[ARGNAME_URL_ID], -1);
            ps.Kontext = ch.psoft.Util.Validate.GetValid(Request.QueryString[ARGNAME_URL_CONTEXT], "");
            ps.IsPrintRequest = ch.psoft.Util.Validate.GetValid(Request.QueryString[ARGNAME_URL_PRINT], false);

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, ps);

            PsoftPageLayout.ButtonPrintVisible = true;
            PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('" +
                psoft.Project.ProjectSummary.GetURL(ARGNAME_URL_ID, ps.xID, ARGNAME_URL_CONTEXT, ps.Kontext, ARGNAME_URL_PRINT, "true") +
                "','_blank')");

        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
        private void InitializeComponent() {    
            this.ID = "Add";

        }
		#endregion
    }
}