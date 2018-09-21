using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Survey.Controls;
using ch.psoft.Util;
using System;
using System.Web.UI.HtmlControls;


namespace ch.appl.psoft.Survey
{

    public partial class Execute : PsoftDetailPage
	{
        protected long _executionID = -1L;
        protected long _surveyID = -1L;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            HtmlMeta meta = new HtmlMeta();
            meta.HttpEquiv = "X-UA-Compatible";
            meta.Content = "IE=7";
            this.Master.FindControl("head").Controls.Add(meta);

            
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout) LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                _surveyID = ch.psoft.Util.Validate.GetValid(Request.QueryString["surveyID"], _surveyID);
                _executionID = ch.psoft.Util.Validate.GetValid(Request.QueryString["executionID"], _executionID);

                if (_executionID <= 0L){
                    if (IsPostBack){
                        _executionID = (long) ViewState["ExecutionID"];
                    }
                    else{
                        // create new execution if possible
                        _executionID = db.Survey.createExecution(_surveyID);
                        ViewState.Add("ExecutionID", _executionID);
                    }
                }
                else{
                    db.Execution.setFinished(_executionID, false);
                }

                if (_surveyID <= 0L){
                    _surveyID = ch.psoft.Util.Validate.GetValid(db.lookup("SURVEY_ID", "EXECUTION", "ID=" + _executionID, false), -1L);
                }

                ExecutionCtrl execution = (ExecutionCtrl) LoadPSOFTControl(ExecutionCtrl.Path, "_ex");
                execution.SurveyID = _surveyID;
                execution.ExecutionID = _executionID;
                execution.StepID = ch.psoft.Util.Validate.GetValid(Request.QueryString["stepID"], -1L);
                SetPageLayoutContentControl(SimpleContentLayout.CONTENT, execution);

                BreadcrumbCaption = PsoftPageLayout.PageTitle = db.lookup("TITLE", "SURVEY", "ID=" + _surveyID, false);

                if (_executionID > 0){
                    int nrOfSteps = db.Survey.getNrOfSteps(_surveyID);
                    int currentStep = 1;
                    if (nrOfSteps > 0){
                        if (execution.StepID > 0L){
                            currentStep = db.Survey.getStepIndex(_surveyID, execution.StepID);
                        }
                        else{
                            execution.StepID = db.Execution.getCurrentStepID(_executionID);
                            if (execution.StepID > 0L){
                                currentStep = db.Survey.getStepIndex(_surveyID, execution.StepID);
                            }
                            else{
                                execution.StepID = db.Survey.getStepIDfromIndex(_surveyID, currentStep);
                            }
                        }
                    }
                    PsoftPageLayout.PageTitleRight = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_STEP) + " " + currentStep + "/" + nrOfSteps;
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
