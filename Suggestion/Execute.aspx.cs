using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Suggestion.Controls;
using ch.psoft.Util;
using System;


namespace ch.appl.psoft.Suggestion
{

    public partial class Execute : PsoftDetailPage
	{
        protected long _executionID = -1L;
        protected long _suggestionID = -1L;
		protected string _context = "";

        public const string PAGE_URL = "/Suggestion/Execute.aspx";

		public static string GetURL(params object[] queryParams)
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			_context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"], _context);
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout) LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                _suggestionID = ch.psoft.Util.Validate.GetValid(Request.QueryString["suggestionID"], _suggestionID);
                _executionID = ch.psoft.Util.Validate.GetValid(Request.QueryString["executionID"], _executionID);

                if (_executionID <= 0L){
                    if (IsPostBack)
                    {
                        _executionID = (long) ViewState["ExecutionID"];
                    }
                    else
                    {
                        //always create a new execution
                        _executionID = db.Suggestion.createExecution(_suggestionID);
                        ViewState.Add("ExecutionID", _executionID);
                    }
                }
                else{
                    if(!db.SuggestionExecution.isSent(_executionID)) 
                    {
                        long suggestionQuestionId = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SUGGESTION_QUESTION", "suggestion_step_id in (select id from suggestion_step where ordnumber = 1 and suggestion_id = " + this._suggestionID + ") and ordnumber = 1", false), -1L);
                        string title_ = ch.psoft.Util.Validate.GetValid(db.lookup(db.langAttrName("SUGGESTION_ANSWER", "TEXT"), "SUGGESTION_ANSWER", "SUGGESTION_EXECUTION_ID=" + _executionID + " and SUGGESTION_QUESTION_ID= " + suggestionQuestionId, false), "");
                        db.SuggestionExecution.setFinished(_executionID, false, title_, false);
                    }
                }

                if (_suggestionID <= 0L){
                    _suggestionID = ch.psoft.Util.Validate.GetValid(db.lookup("SUGGESTION_ID", "SUGGESTION_EXECUTION", "ID=" + _executionID, false), -1L);
                }

                ExecutionCtrl execution = (ExecutionCtrl) LoadPSOFTControl(ExecutionCtrl.Path, "_ex");
                execution.SuggestionID = _suggestionID;
                execution.ExecutionID = _executionID;
                execution.StepID = ch.psoft.Util.Validate.GetValid(Request.QueryString["stepID"], -1L);
				execution.ContextStr = _context;
                SetPageLayoutContentControl(SimpleContentLayout.CONTENT, execution);

                BreadcrumbCaption = PsoftPageLayout.PageTitle = db.lookup("TITLE", "SUGGESTION", "ID=" + _suggestionID, false);

                if (_executionID > 0){
                    int nrOfSteps = db.Suggestion.getNrOfSteps(_suggestionID);
                    int currentStep = 1;
                    if (nrOfSteps > 0){
                        if (execution.StepID > 0L){
                            currentStep = db.Suggestion.getStepIndex(_suggestionID, execution.StepID);
                        }
                        else{
                            execution.StepID = db.SuggestionExecution.getCurrentStepID(_executionID);
                            if (execution.StepID > 0L){
                                currentStep = db.Suggestion.getStepIndex(_suggestionID, execution.StepID);
                            }
                            else{
                                execution.StepID = db.Suggestion.getStepIDfromIndex(_suggestionID, currentStep);
                            }
                        }
                    }
                    PsoftPageLayout.PageTitleRight = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_STEP) + " " + currentStep + "/" + nrOfSteps;
                    BreadcrumbCaption += " (" + currentStep + "/" + nrOfSteps + ")";
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
