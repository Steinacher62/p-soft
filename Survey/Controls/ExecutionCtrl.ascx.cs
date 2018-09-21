namespace ch.appl.psoft.Survey.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class ExecutionCtrl : PSOFTMapperUserControl {
        public const string PARAM_SURVEY_ID = "PARAM_SURVEY_ID";
        public const string PARAM_EXECUTION_ID = "PARAM_EXECUTION_ID";
        public const string PARAM_STEP_ID = "PARAM_STEP_ID";

        private DBData _db = null;
        protected int _currentStepIndex = 0;
        protected int _nrOfSteps = 0;
        protected string _focusControlID = "";
        protected ArrayList _questions = new ArrayList();


        public static string Path {
            get {return Global.Config.baseURL + "/Survey/Controls/ExecutionCtrl.ascx";}
        }

		#region Properities
        public long SurveyID {
            get {return GetLong(PARAM_SURVEY_ID);}
            set {SetParam(PARAM_SURVEY_ID, value);}
        }

        public long ExecutionID {
            get {return GetLong(PARAM_EXECUTION_ID);}
            set {SetParam(PARAM_EXECUTION_ID, value);}
        }

        public long StepID {
            get {return GetLong(PARAM_STEP_ID);}
            set {SetParam(PARAM_STEP_ID, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();

            _db = DBData.getDBData(Session);
            _db.connect();
            try {
                _currentStepIndex = _db.Survey.getStepIndex(SurveyID, StepID);
                _nrOfSteps = _db.Survey.getNrOfSteps(SurveyID);

                if (!IsPostBack){
                    stepTitle.Text = _db.lookup("TITLE", "STEP", "ID=" + StepID, false);
                    stepDescription.Text = _db.lookup("DESCRIPTION", "STEP", "ID=" + StepID, true);
                    back.Value = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_PREVIOUS_STEP);
                    if (_currentStepIndex == _nrOfSteps){
                        next.Text = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_FINISH);
                    }
                    else{
                        next.Text = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_NEXT_STEP);
                    }
                    back.Disabled = _currentStepIndex <= 1;
                    next.Enabled = true;
                    _db.Execution.setCurrentStepID(ExecutionID, StepID);
                }

                //display questions...
                DataTable table = _db.getDataTable("select ID from QUESTION where STEP_ID=" + StepID + " order by ORDNUMBER asc");
                int questionIndex = ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)", "QUESTION", "STEP_ID in (select ID from STEP where SURVEY_ID=" + SurveyID + " and ORDNUMBER<(select ORDNUMBER from STEP where ID=" + StepID + "))", false), 0);
                foreach (DataRow row in table.Rows){
                    long questionID = DBColumn.GetValid(row[0], -1L);
                    if (questionID > 0L){
                        QuestionCtrl question = (QuestionCtrl) LoadPSOFTControl(QuestionCtrl.Path, "_q" + questionID);
                        question.QuestionID = questionID;
                        question.ExecutionID = ExecutionID;
                        question.QuestionIndex = ++questionIndex;
                        TableRow r = new TableRow();
                        questionTab.Rows.Add(r);
                        TableCell c = new TableCell();
                        r.Cells.Add(c);
                        c.Controls.Add(question);
                        _questions.Add(question);
                    }
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected void saveAnswers(){
            foreach (QuestionCtrl question in _questions){
                question.saveAnswer();
            }
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
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion

        protected void back_Click(object sender, System.EventArgs e) {
            saveAnswers();
            _db.connect();
            try{
                long prevStepID = _db.Survey.getStepIDfromIndex(SurveyID, _currentStepIndex - 1);
                Response.Redirect(Global.Config.baseURL + "/Survey/Execute.aspx?surveyID=" + SurveyID + "&executionID=" + ExecutionID + "&stepID=" + prevStepID, false);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected void next_Click(object sender, System.EventArgs e) {
            if (Page.IsValid){
                saveAnswers();
                _db.connect();
                try{
                    if (_currentStepIndex == _nrOfSteps){
                        _db.Execution.setFinished(ExecutionID, true);
                        Response.Redirect(Global.Config.baseURL + "/Survey/SurveyDetail.aspx?ID=" + SurveyID, false);
                    }
                    else{
                        long nextStepID = _db.Survey.getStepIDfromIndex(SurveyID, _currentStepIndex + 1);
                        Response.Redirect(Global.Config.baseURL + "/Survey/Execute.aspx?surveyID=" + SurveyID + "&executionID=" + ExecutionID + "&stepID=" + nextStepID, false);
                    }
                }
                catch (Exception ex) {
                    DoOnException(ex);
                }
                finally {
                    _db.disconnect();
                }
            }
        }
    }
}
