namespace ch.appl.psoft.Suggestion.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text.RegularExpressions;
    using System.Web.UI.WebControls;


    public partial class ExecutionCtrl : PSOFTMapperUserControl {
        public const string PARAM_SUGGESTION_ID = "PARAM_SUGGESTION_ID";
        public const string PARAM_EXECUTION_ID = "PARAM_EXECUTION_ID";
        public const string PARAM_STEP_ID = "PARAM_STEP_ID";

        private DBData _db = null;
        protected int _currentStepIndex = 0;
        protected int _nrOfSteps = 0;
        protected string _focusControlID = "";
        protected ArrayList _questions = new ArrayList();
		protected string _context = "";

        ///button used to save the current survey as knowledge element
        ///the survey can be sent only once. 
        protected System.Web.UI.WebControls.Button sendToKnowledge;  


        public static string Path {
            get {return Global.Config.baseURL + "/Suggestion/Controls/ExecutionCtrl.ascx";}
        }

		#region Properities
		public string ContextStr {
			get {return _context;}
			set {_context = value;}
		}

        public long SuggestionID {
            get {return GetLong(PARAM_SUGGESTION_ID);}
            set {SetParam(PARAM_SUGGESTION_ID, value);}
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

                _currentStepIndex = _db.Suggestion.getStepIndex(SuggestionID, StepID);
				if(_currentStepIndex == 1) 
				{
					//show suggestion description in the first step only.
					string description = ch.psoft.Util.Validate.GetValid(_db.lookup(_db.langAttrName("SUGGESTION", "DESCRIPTION"),"SUGGESTION", "ID=" + SuggestionID, false), "");
					Regex rexNewLine = new Regex("\n");
					description = rexNewLine.Replace(description,"<br>");
				}
                _nrOfSteps = _db.Suggestion.getNrOfSteps(SuggestionID);

                bool enabledfields = true;
                if(_db.SuggestionExecution.isSent(ExecutionID)) 
                {
                    enabledfields = false;
                }
                if (_currentStepIndex == _nrOfSteps && _db.SuggestionExecution.isSent(ExecutionID) )
                {
                    this.sendToKnowledge.Enabled = false;
                    this.next.Enabled = false;
                }
                else 
                {
                    this.sendToKnowledge.Enabled = true;
                    this.next.Enabled = true;
                }

                if (!IsPostBack){
                    stepTitle.Text = _db.lookup("TITLE", "SUGGESTION_STEP", "ID=" + StepID, false);
                    stepDescription.Text = _db.lookup("DESCRIPTION", "SUGGESTION_STEP", "ID=" + StepID, true);
                    back.Value = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_PREVIOUS_STEP);
                    if (_currentStepIndex == _nrOfSteps){
                        next.Text = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_FINISH);
                        this.sendToKnowledge.Visible = true;
                        this.sendToKnowledge.Text = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_SENDTOKNOWLEDGE);
                    }
                    else{
                        next.Text = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_NEXT_STEP);
                        this.sendToKnowledge.Visible = false;
                    }
                    back.Disabled = _currentStepIndex <= 1;
                    //next.Enabled = true;
                    _db.SuggestionExecution.setCurrentStepID(ExecutionID, StepID);
                }

                //display questions...
                DataTable table = _db.getDataTable("select ID from SUGGESTION_QUESTION where SUGGESTION_STEP_ID=" + StepID + " order by ORDNUMBER asc");
                int questionIndex = ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)", "SUGGESTION_QUESTION", "SUGGESTION_STEP_ID in (select ID from SUGGESTION_STEP where SUGGESTION_ID=" + SuggestionID + " and ORDNUMBER<(select ORDNUMBER from SUGGESTION_STEP where ID=" + StepID + "))", false), 0);
                foreach (DataRow row in table.Rows){
                    long questionID = DBColumn.GetValid(row[0], -1L);
                    if (questionID > 0L){
                        QuestionCtrl question = (QuestionCtrl) LoadPSOFTControl(QuestionCtrl.Path, "_q" + questionID);
                        question.QuestionID = questionID;
                        question.ExecutionID = ExecutionID;
                        question.Enabled = enabledfields;
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
                long prevStepID = _db.Suggestion.getStepIDfromIndex(SuggestionID, _currentStepIndex - 1);
                Response.Redirect(Global.Config.baseURL + "/Suggestion/Execute.aspx?suggestionID=" + SuggestionID + "&executionID=" + ExecutionID + "&stepID=" + prevStepID, false);
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
                        long suggestionQuestionId = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "SUGGESTION_QUESTION", "suggestion_step_id in (select id from suggestion_step where ordnumber = 1 and suggestion_id = " + this.SuggestionID + ") and ordnumber = 1", false), -1L);
                        string title_ = ch.psoft.Util.Validate.GetValid(_db.lookup(_db.langAttrName("SUGGESTION_ANSWER", "TEXT"), "SUGGESTION_ANSWER", "SUGGESTION_EXECUTION_ID=" + this.ExecutionID + " and SUGGESTION_QUESTION_ID= " + suggestionQuestionId, false), "");
                        _db.SuggestionExecution.setFinished(ExecutionID, true, title_, false);
                        Response.Redirect(Global.Config.baseURL + "/Suggestion/SuggestionDetail.aspx?ID=" + SuggestionID, false);
                    }
                    else{
                        long nextStepID = _db.Suggestion.getStepIDfromIndex(SuggestionID, _currentStepIndex + 1);
                        Response.Redirect(Global.Config.baseURL + "/Suggestion/Execute.aspx?suggestionID=" + SuggestionID + "&executionID=" + ExecutionID + "&stepID=" + nextStepID, false);
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

        protected void sendToKnowledge_Click(object sender, System.EventArgs e)
        {
            if (!Page.IsValid) 
            {
                return;
            }
            saveAnswers();

            _db.connect();
            try
            {
                if ( _currentStepIndex == _nrOfSteps && !_db.SuggestionExecution.isSent(ExecutionID) )
                {
                    //set the survey as complete and frozen so that it cannot be sent anymore 
                    //there will be only one suggestion_execution entry pro person_id and suggestion_id
                    long suggestionQuestionId = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "SUGGESTION_QUESTION", "suggestion_step_id in (select id from suggestion_step where ordnumber = 1 and suggestion_id = " + this.SuggestionID + ") and ordnumber = 1", false), -1L);
                    string title_ = ch.psoft.Util.Validate.GetValid(_db.lookup(_db.langAttrName("SUGGESTION_ANSWER", "TEXT"), "SUGGESTION_ANSWER", "SUGGESTION_EXECUTION_ID=" + this.ExecutionID + " and SUGGESTION_QUESTION_ID= " + suggestionQuestionId, false), "");
                    _db.SuggestionExecution.setSent(ExecutionID, true, title_, false); // the send state
                    Response.Redirect(Global.Config.baseURL + "/Suggestion/SuggestionDetail.aspx?ID=" + SuggestionID + "&context=" + _context, false);
                }
                else
                {
                    //TODO: handle the error
                    return;
                }
  
                //store the data as a Wissenelement
                //information needed: suggestion id, execution id, person id
				string title = "";
				//title should be Title of Suggestion

                //title += _db.lookup(_db.langAttrName("SUGGESTION", "TITLE"),       "SUGGESTION", "ID=" + SuggestionID, "");
                string description = "";//_db.lookup(_db.langAttrName("SUGGESTION", "DESCRIPTION"), "SUGGESTION", "ID=" + SuggestionID, "");

                // check if the suggestion_execution is really available
                long currentPersonId = _db.userId;
                long personId = _db.lookup("PERSON_ID", "SUGGESTION_EXECUTION", "ID=" + ExecutionID, -1L);
                int isFinished = _db.lookup("ISFINISHED", "SUGGESTION_EXECUTION", "ID=" + ExecutionID, -1);
                if(personId != currentPersonId && isFinished <= 0) 
                {
                    //TODO: handle the error
                    return;
                }

                string personFName = _db.lookup("FIRSTNAME", "PERSON", "ID=" + personId, "");
                string personPName = _db.lookup("PNAME", "PERSON", "ID=" + personId, "");
                string personInfo = personFName + " " + personPName;
                DateTime created = _db.lookup("CREATED", "SUGGESTION_EXECUTION", "ID=" + ExecutionID, new DateTime());

                long suggestionQuestionId_ = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "SUGGESTION_QUESTION", "suggestion_step_id in (select id from suggestion_step where DELETABLE = 1 and suggestion_id = " + this.SuggestionID + ") and DELETABLE = 1", false), -1L);
                string firstQuestion = "";
                if(suggestionQuestionId_ > 0) 
                {
                    firstQuestion = ch.psoft.Util.Validate.GetValid(_db.lookup(_db.langAttrName("SUGGESTION_ANSWER", "TEXT"), "SUGGESTION_ANSWER", "SUGGESTION_EXECUTION_ID=" + this.ExecutionID + " and SUGGESTION_QUESTION_ID= " + suggestionQuestionId_, false), "");
                }
                title += firstQuestion + " - " + created.ToString("dd.MM.yyyy");
                description = _mapper.get("suggestion", "textCreatorPerson") + " " + personInfo + "<br/>";

                long rootThemeId = this.storeAsWe(title,description);

                //store questions and answers as Themen
                // data must be read from the suggestion_answer table.
                DataTable answerTable = _db.getDataTable("select * from SUGGESTION_ANSWER where SUGGESTION_EXECUTION_ID=" + ExecutionID + " order by created asc");

				int position = 0;
				long oldStepID = -1;				
				string themeText = "";
				string stepTitle = "";

                foreach (DataRow answer in answerTable.Rows)
                {
                    string text = _db.langAttrName("SUGGESTION_ANSWER", "TEXT");
                    string answer_ = "";
                    if(answer[text] is System.String) 
                    {
                        answer_ = answer[text].ToString();
                    }
                    else if(!(answer["NUMBER"] is System.DBNull))   //TODO correction + date ecc
                    {
                        answer_ = answer["NUMBER"].ToString();
                    }
                    else if(!(answer["DATE"] is System.DBNull) && (answer["DATE"] is DateTime)) 
                    {
                        DateTime dt = (DateTime)answer["DATE"];
                        answer_ = dt.ToLongDateString();
                    }
                    else if(!(answer["BOOLEAN"] is System.DBNull)) 
                    {
                        int yesorno = DBColumn.GetValid(answer["BOOLEAN"], 0);
                        if( yesorno == 0 ) //no
                        {
                            answer_ = _mapper.get("no");
                        }
                        else 
                        {
                            answer_ = _mapper.get("yes");
                        }              
                    }
                    else 
                    {
                        //no entry
                    }
                    long suggestionQuestionId = (long)answer["SUGGESTION_QUESTION_ID"];
					long newStepID = (long)_db.lookup(_db.langAttrName("SUGGESTION_QUESTION", "SUGGESTION_STEP_ID"),"SUGGESTION_QUESTION","ID=" + suggestionQuestionId, -1L);

					//store Theme
					if(oldStepID != newStepID && position > 0)
					{
						storeThemen(rootThemeId, stepTitle, themeText, position);
						themeText = "";
					}

					string question_ = (string)_db.lookup(_db.langAttrName("SUGGESTION_QUESTION", "QUESTION"),"SUGGESTION_QUESTION","ID=" + suggestionQuestionId, "");
					stepTitle = (string)_db.lookup(_db.langAttrName("SUGGESTION_STEP", "TITLE"),"SUGGESTION_STEP","ID=" + newStepID, "");					
					
					themeText += "<br/><b>" + question_ + "</b><br/>";
                    themeText += answer_ + "<br/>";
					oldStepID = newStepID;
					
					position++;
                }//foreach
				storeThemen(rootThemeId, stepTitle, themeText, position);
            }
            catch (Exception ex) 
            {
                DoOnException(ex);
            }
            finally 
            {
                _db.disconnect();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="personinfo"></param>
        /// <param name="description"></param>
        /// <returns>The ID of the root theme</returns>
        long storeAsWe(string title, string description)
        {
            long themeID = -1;
            try 
            {
                _db.beginTransaction();
                themeID = _db.newId(Theme._TABLENAME);
                string sql = "insert into THEME (TITLE,DESCRIPTION,ID,ROOT_ID,CREATOR_PERSON_ID) values('" 
                    + title + "','" 
                    + description + "'," 
                    + themeID + ", " 
                    + themeID + ", " 
                    + _db.userId + ")";

                _db.execute(sql);
        
                long knowledgeID = _db.Knowledge.create(themeID, "Initialversion", 1, ExecutionID);
                
                _db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, DBData.ACCESSOR.ALL, Knowledge._TABLENAME, knowledgeID);
                _db.commit();
            }
            catch (Exception ex) 
            {
                _db.rollback();
                throw (ex);
            }
            return themeID;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootThemeId">The id of the root Thema, i.e., the one directly connected with Knowledge</param>
        /// <param name="themenTitle"></param>
        /// <param name="ThemenText"></param>
        /// <param name="position"></param>
        void storeThemen(long rootThemeId, string themenTitle, string ThemenText, int position)
        {  
            try 
            {
                _db.beginTransaction();

                long id = _db.newId("THEME"); // The id of the current Thema.
                long parentId = rootThemeId;  // The id of the parent Thema (it is NULL for root, but root is already available).
                long creatorPersonId = _db.userId;

                string sql = "insert into THEME (TITLE,DESCRIPTION,ORDNUMBER,ID,PARENT_ID,ROOT_ID,CREATOR_PERSON_ID) values('"
                    + themenTitle + "','" + ThemenText + "'," + position + "," + id + "," + parentId + "," + rootThemeId + "," + creatorPersonId +")";

                _db.execute(sql);
                _db.commit();
            }
            catch (Exception ex) 
            {
                _db.rollback();
                throw (ex);
            }
        }
    }
}
