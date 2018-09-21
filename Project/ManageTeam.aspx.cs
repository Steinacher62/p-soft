using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;
using ch.psoft.Util;
using System;


namespace ch.appl.psoft.Project
{

    public partial class ManageTeam : PsoftContentPage
	{
        private const string PAGE_URL = "/Project/ManageTeam.aspx";

        static ManageTeam() {
            SetPageParams(PAGE_URL, "projectID", "orderColumn", "orderDir");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public ManageTeam() : base() {
            PageURL = PAGE_URL;
        }

        protected long _projectID = -1L;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            _projectID = GetQueryValue("projectID", _projectID);

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "PROJECT", _projectID, true, true)){
                    BreadcrumbCaption = PsoftPageLayout.PageTitle = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_PROJECT_TEAM).Replace("#1", db.lookup("TITLE", "PROJECT", "ID=" + _projectID, false));

                    // Team-list
                    ProjectTeamList pList = (ProjectTeamList) LoadPSOFTControl(ProjectTeamList.Path, "_pl");
                    pList.ProjectID = _projectID;
                    pList.OrderColumn = GetQueryValue("orderColumn", db.langAttrName("JOB_PERS_FUNC_V", "PERSON"));
                    pList.OrderDir = GetQueryValue("orderDir", "asc");
                    pList.SortURL = psoft.Project.ManageTeam.GetURL("projectID",_projectID);
                    SetPageLayoutContentControl(DGLContentLayout.DETAIL, pList);		

                    // Context-links
                    PsoftLinksControl links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                    links.LinkGroup1.Caption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CMT_PROJECTTEAM);
                    if (db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "PROJECT", _projectID, true, true)){
                        links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_ADD_TEAMMEMBER), psoft.Project.TeamMemberEdit.GetURL("projectID",_projectID, "mode","add", "nextURL",Request.RawUrl));
                    }
                    SetPageLayoutContentControl(DGLContentLayout.LINKS, links);
                }
                else{
                    BreadcrumbVisible = false;
                    Response.Redirect(NotFound.GetURL(), false);
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally{
                db.disconnect();
            }
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
