using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;
using System;

namespace ch.appl.psoft.Project
{
    public partial class TeamMemberEdit : PsoftEditPage
	{
        private const string PAGE_URL = "/Project/TeamMemberEdit.aspx";

        static TeamMemberEdit() {
            SetPageParams(PAGE_URL, "projectID", "jobID", "mode", "nextURL");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public TeamMemberEdit() : base() {
            PageURL = PAGE_URL;
        }

        protected void Page_Load(object sender, System.EventArgs e)
		{
			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
			PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = BreadcrumbCaption;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			TeamMemberEditCtrl pEdit = (TeamMemberEditCtrl) LoadPSOFTControl(TeamMemberEditCtrl.Path, "_ed");
            pEdit.ProjectID = GetQueryValue("projectID", -1L);
            pEdit.JobID = GetQueryValue("jobID", -1L);
            pEdit.NextURL = GetQueryValue("nextURL", "");
            switch(GetQueryValue("mode", "edit")){
                case "edit":
                    (PageLayoutControl as PsoftPageLayout).PageTitle = BreadcrumbCaption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_EDIT_TEAMMEMBER);
                    pEdit.InputType = InputMaskBuilder.InputType.Edit;
                    break;

                case "add":
                    (PageLayoutControl as PsoftPageLayout).PageTitle = BreadcrumbCaption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_ADD_TEAMMEMBER);
                    pEdit.InputType = InputMaskBuilder.InputType.Add;
                    break;
            }

			// Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, pEdit);		
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
