using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Project
{
    public partial class SubNavMenu : PsoftMenuPage
	{
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Setting main page layout
            MenuPageLayout pageLayout = (MenuPageLayout) LoadPSOFTControl(MenuPageLayout.Path, "_pl");;
            PageLayoutControl = pageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout) LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting detail parameters
            MenuControl ctrl = (MenuControl) LoadPSOFTControl(MenuControl.Path, "_ctrl");
            ctrl.Title = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_SUBNAV_TITLE);
            ctrl.addMenuItem(null, "searchProject", _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_SUBNAV_SEARCHPROJECT), psoft.Project.ProjectSearch.GetURL());
            ctrl.addMenuItem(null, "newProject", _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_SUBNAV_NEWPROJECT), psoft.Project.ProjectAdd.GetURL("parentID",-1));
            if (Global.Config.getModuleParam("project", "enableExportProjectOverview", "0").Equals("1"))
            {
                ctrl.addMenuItem(null,
                          "exportProjectOverview",
                          _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_EXPORTPROJECTOVERVIEW),
                          psoft.Project.ProjectSummary.GetURL(psoft.Project.ProjectSummary.ARGNAME_URL_ID, -1,
                                                                                           psoft.Project.ProjectSummary.ARGNAME_URL_CONTEXT,
                                                                                           psoft.Project.Controls.ProjectList.CONTEXT_EXPORT_PROJECT_OVERVIEW,
                                                                                           psoft.Project.ProjectSummary.ARGNAME_URL_PRINT, "true"),
                           psoft.Project.ProjectSummary.GetURL(psoft.Project.ProjectSummary.ARGNAME_URL_ID, -1,
                                                                                           psoft.Project.ProjectSummary.ARGNAME_URL_CONTEXT,
                                                                                           psoft.Project.Controls.ProjectList.CONTEXT_EXPORT_PROJECT_OVERVIEW,
                                                                                           psoft.Project.ProjectSummary.ARGNAME_URL_PRINT, "true")
                                                                                           );
            }

            ctrl.StartPageLink = psoft.Project.ProjectSearch.GetURL();

            //Setting content layout user controls
            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, ctrl);
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
        }
		#endregion
	}
}
