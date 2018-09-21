using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;
using ch.psoft.Util;
using System;


namespace ch.appl.psoft.Project
{
    public partial class ProjectScoreCard : PsoftContentPage
	{
        private const string PAGE_URL = "/Project/ProjectScoreCard.aspx";

        static ProjectScoreCard() {
            SetPageParams(PAGE_URL, "projectID");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public ProjectScoreCard() : base() {
            PageURL = PAGE_URL;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
            // Setting main page layout
            PsoftPageLayout pageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
			PageLayoutControl = pageLayout;

			// Setting content layout of page layout
			PageLayoutControl.ContentLayoutControl = (SimpleContentLayout) LoadPSOFTControl(SimpleContentLayout.Path, "_sC");

			// Load score-card control
			ProjectScoreCardCtrl scoreCardCtrl = (ProjectScoreCardCtrl) LoadPSOFTControl(ProjectScoreCardCtrl.Path, "_ctrl");
			scoreCardCtrl.ProjectID = GetQueryValue("projectID", -1);
            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "PROJECT", scoreCardCtrl.ProjectID, true, true)){

                    // Setting breadcrumb menu for chart title
                    BreadcrumbCaption = pageLayout.PageTitle = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_SCORECARD);
                    pageLayout.PageTitle += " - " + db.lookup("TITLE", "PROJECT", "ID=" + scoreCardCtrl.ProjectID, "");

                    SetPageLayoutContentControl(SimpleContentLayout.CONTENT, scoreCardCtrl);
                    if (scoreCardCtrl.ProjectID > 0){
                        pageLayout.ButtonPrintVisible = true;
                        pageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('" + psoft.Project.CreateProjectReport.GetURL("projectID",scoreCardCtrl.ProjectID) + "','_blank')");
                    }
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
