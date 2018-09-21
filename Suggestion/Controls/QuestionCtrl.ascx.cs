namespace ch.appl.psoft.Suggestion.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Collections;
    using System.Data;


    public partial class QuestionCtrl : PSOFTMapperUserControl {

        public const string PARAM_QUESTION_ID = "PARAM_QUESTION_ID";
        public const string PARAM_EXECUTION_ID = "PARAM_EXECUTION_ID";
        public const string PARAM_QUESTION_INDEX = "PARAM_QUESTION_INDEX";

        public const int TYP_FREE_TEXT              = 0;
        public const int TYP_NUMBER                 = 1;
        public const int TYP_DATE                   = 2;
        public const int TYP_BOOLEAN                = 3;
        public const int TYP_SINGLE_CHOICE_DROPDOWN = 4;
        public const int TYP_SINGLE_CHOICE_RADIO    = 5;
        public const int TYP_MULTIPLE_CHOICE        = 6;

        public static  int MIN_INPUT_CHARS =  0;
        public static  int MAX_INPUT_CHARS =  1600;

        protected DBData _db = null;


        protected ArrayList _questionElements = new ArrayList();
        
        protected bool _enabled = true;

        public bool Enabled 
        {
            set { _enabled = value; }
        }

        public string Question  
        {
            get { return question.Text; }
        }

        public string Hint  
        {
            get { return hint.Text; }
        }

        public string Answer  
        {
            get { 
                string answer = ""; 
                foreach (QuestionElement qe in _questionElements)
                {
                    if(qe.AnswerExists) 
                    {
                        answer += qe.AnswerText + "\n";
                    }
                }
                return answer;
            }
        }


        public static string Path {
            get {return Global.Config.baseURL + "/Suggestion/Controls/QuestionCtrl.ascx";}
        }

		#region Properities
        public long QuestionID {
            get {return GetLong(PARAM_QUESTION_ID);}
            set {SetParam(PARAM_QUESTION_ID, value);}
        }

        public long ExecutionID {
            get {return GetLong(PARAM_EXECUTION_ID);}
            set {SetParam(PARAM_EXECUTION_ID, value);}
        }

        public int QuestionIndex {
            get {return GetInt(PARAM_QUESTION_INDEX);}
            set {SetParam(PARAM_QUESTION_INDEX, value);}
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
                DataTable table = _db.getDataTable("select * from SUGGESTION_QUESTION where ID=" + QuestionID);
                if (table.Rows.Count > 0){
                    DataRow row = table.Rows[0];

                    if (!IsPostBack){
                        questionNr.Text = QuestionIndex.ToString() + ". ";
                        question.Text =  DBColumn.GetValid(row[_db.langAttrName("SUGGESTION_QUESTION", "QUESTION")], "");
                        hint.Text = _db.GetDisplayValue(table.Columns[_db.langAttrName("SUGGESTION_QUESTION", "HINT")], row[_db.langAttrName("SUGGESTION_QUESTION", "HINT")], true);
                    }
                    
                    //no questionelement in suggestions
                    //DataTable qeTable = _db.getDataTable("select ID, TYP from QUESTIONELEMENT where QUESTION_ID=" + QuestionID + " order by ORDNUMBER asc");
                    DataTable qeTable = _db.getDataTable("select ID, TYP from SUGGESTION_QUESTION where ID=" + QuestionID + " order by ORDNUMBER asc");
                    foreach (DataRow qeRow in qeTable.Rows){
                        int typ = DBColumn.GetValid(qeRow["TYP"], TYP_FREE_TEXT);
                        long qeID = DBColumn.GetValid(qeRow["ID"], -1L); //qeID is the same as the QuestionID in suggestions
                        QuestionElement questionElement = null;
                        switch (typ){
                            case TYP_FREE_TEXT:
                                questionElement = new QuestionElementFreeText(_mapper, qeID, ExecutionID, questionElementTab, IsPostBack);                                
                                break;

                            case TYP_NUMBER:
                                questionElement = new QuestionElementNumber(_mapper, qeID, ExecutionID, questionElementTab, IsPostBack);
                                break;

                            case TYP_DATE:
                                questionElement = new QuestionElementDate(_mapper, qeID, ExecutionID, questionElementTab, IsPostBack);
                                break;

                            case TYP_BOOLEAN:
                                questionElement = new QuestionElementBoolean(_mapper, qeID, ExecutionID, questionElementTab, IsPostBack);
                                break;

                            case TYP_SINGLE_CHOICE_DROPDOWN:
                                questionElement = new QuestionElementSingleChoiceDropDown(_mapper, qeID, ExecutionID, questionElementTab, IsPostBack);
                                break;
                            // radio, drop-down and choice no more used.
                        }

                        if (questionElement != null){
                            questionElement.Enabled = this._enabled;
                            questionElement.Load(_db);
                            _questionElements.Add(questionElement);
                        }
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

        public void saveAnswer(){
            _db.connect();
            try {
                _db.beginTransaction();

                foreach (QuestionElement qe in _questionElements){
                    qe.saveAnswer(_db);
                }

                _db.commit();
            }
            catch (Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
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
    }
}
