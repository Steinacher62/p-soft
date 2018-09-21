using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;
using System;

namespace ch.appl.psoft.Project
{

    public partial class ProjectEdit : PsoftTreeViewPage {
		public const string	CONTEXT_PROJECT_EDIT = "CONTEXT_PROJECT_EDIT";
		public const string	CONTEXT_SPEC_EDIT = "CONTEXT_SPEC_EDIT";

		private const string PAGE_URL = "/Project/ProjectEdit.aspx";

		static ProjectEdit() 
		{
			SetPageParams(PAGE_URL, "projectID", "context");
		}

		public static string GetURL(params object[] queryParams) 
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		public ProjectEdit() : base() 
		{
			PageURL = PAGE_URL;
		}

		protected string _context = CONTEXT_PROJECT_EDIT;
		protected long _projectID = -1L;

        protected void Page_Load(object sender, System.EventArgs e) 
        {
			_context = GetQueryValue("context", _context);
			_projectID = GetQueryValue("projectID", _projectID);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;
			PageLayoutControl = PsoftPageLayout;
			switch (_context)
			{
				case CONTEXT_PROJECT_EDIT:
					PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_EDITPROJECT);
					break;
				case CONTEXT_SPEC_EDIT:
					PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_SPEC);
					break;
			}

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)LoadPSOFTControl(DGLContentLayout.Path, "_cl");
			PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting control
            ProjectEditCtrl pe = (ProjectEditCtrl)LoadPSOFTControl(ProjectEditCtrl.Path, "_pe");
            pe.ProjectID = _projectID;
            pe.NextURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["nextURL"], "");
			pe.Kontext = _context;

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, pe);		
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