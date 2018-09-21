using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Survey.Controls;
using ch.psoft.Util;
using System;


namespace ch.appl.psoft.Survey
{

    public partial class SurveyDetail : PsoftDetailPage
	{
        protected string _context = "";
        protected long _xID = -1;

        #region Protected overridden methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();

            // Setting navigation-menu
            SubNavMenuUrl = "/Survey/SubNavMenu.aspx";
        }

		#endregion
        
        protected void Page_Load(object sender, System.EventArgs e)
		{
            _context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"], _context);
            _xID = ch.psoft.Util.Validate.GetValid(Request.QueryString["xID"], _xID);

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            
            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                PsoftLinksControl links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                links.LinkGroup1.Caption = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_CMT_SELECTED_SURVEY);

                SurveyDetailCtrl detail = (SurveyDetailCtrl) LoadPSOFTControl(SurveyDetailCtrl.Path, "_det");
                detail.SurveyID = ch.psoft.Util.Validate.GetValid(Request.QueryString["ID"],-1);

                switch (_context){
                    case "person":
                        if (_xID <= 0){
                            _xID = db.userId;
                        }
                        string SurveyIDList = db.Survey.getAccessableSurveys(_xID);
                        if (detail.SurveyID <= 0){
                            string[] SurveyIDs = SurveyIDList.Split(',');
                            if (SurveyIDs.Length > 0){
                                detail.SurveyID = ch.psoft.Util.Validate.GetValid(SurveyIDs[0], -1L);
                            }
                        }
                        break;

                    case "searchresult":
                        if (detail.SurveyID <= 0){
                            detail.SurveyID = ch.psoft.Util.Validate.GetValid(db.lookup("row_id", "searchresult", "ID=" + _xID, false), -1);
                        }
                        break;
                }

                bool isExecutable = db.Survey.isExecutable(detail.SurveyID);
                if (isExecutable){
                    links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_CM_EXECUTE_SURVEY), "/Survey/Execute.aspx?surveyID=" + detail.SurveyID);
                }

                if (_context == "person" || _context == "searchresult"){
                    if (detail.SurveyID > 0){
                        links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_CM_SHOW_EXECUTIONS), "/Survey/SurveyDetail.aspx?id=" + detail.SurveyID);
                    }
//                    links.LinkGroup2.Caption = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_CMT_LISTED_SURVEYS);
                    SurveyList surveyList = (SurveyList) LoadPSOFTControl(SurveyList.Path, "_list");
                    surveyList.xID = _xID;
                    surveyList.Kontext = _context;
                    surveyList.SurveyID = detail.SurveyID;
                    surveyList.OrderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], "TITLE");
                    surveyList.OrderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], "asc");
                    surveyList.DetailURL = Global.Config.baseURL + "/Survey/SurveyDetail.aspx?context=" + _context + "&id=%ID&xID=" + _xID;
                    surveyList.DetailEnabled = true;
                    surveyList.DeleteEnabled = false;
                    surveyList.EditEnabled = false;
                    SetPageLayoutContentControl(DGLContentLayout.GROUP, surveyList);
                }
                else{
                    ExecutionList executionList = (ExecutionList) LoadPSOFTControl(ExecutionList.Path, "_list");
                    executionList.xID = detail.SurveyID;
                    executionList.Kontext = ExecutionList.CONTEXT_SURVEY;
                    executionList.OrderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], "CREATED");
                    executionList.OrderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], "asc");
                    executionList.DetailURL = Global.Config.baseURL + "/Survey/Execute.aspx?executionID=%ID&surveyID=" + detail.SurveyID;
                    executionList.DetailEnabled = true;
                    executionList.PostDeleteURL = Request.RawUrl;
                    SetPageLayoutContentControl(DGLContentLayout.GROUP, executionList);
                }

                // Setting content layout user controls
                SetPageLayoutContentControl(DGLContentLayout.DETAIL, detail);
                SetPageLayoutContentControl(DGLContentLayout.LINKS, links);

                BreadcrumbCaption = PsoftPageLayout.PageTitle = db.lookup("TITLE", "SURVEY", "ID=" + detail.SurveyID, false);
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
