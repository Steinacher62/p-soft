using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;
using System;

namespace ch.appl.psoft.Project
{
    public partial class ProjectAdd : PsoftTreeViewPage
	{
        private const string PAGE_URL = "/Project/ProjectAdd.aspx";

        static ProjectAdd() {
            SetPageParams(PAGE_URL, "parentID");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public ProjectAdd() : base() {
            PageURL = PAGE_URL;
        }

        protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_ADDPROJECT);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
			PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = BreadcrumbCaption;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			ProjectAddCtrl pAdd = (ProjectAddCtrl) LoadPSOFTControl(ProjectAddCtrl.Path, "_ad");
            pAdd.ParentID = GetQueryValue("parentID", -1);

			// Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, pAdd);		
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
			this.ID = "Add";

		}
		#endregion

	}
}
