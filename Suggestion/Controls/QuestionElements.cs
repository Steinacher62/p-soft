using ch.appl.psoft.Common;
using ch.appl.psoft.Common.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Suggestion.Controls
{

    /// <summary>
    /// Abstract base-class for all types of question-elements
    /// </summary>
    public abstract class QuestionElement {
        


        /// <summary>
        /// Represents an answer-option (for single-choice and multiple-choice questions).
        /// </summary>
        protected class AnswerOption{
            public long _ID = -1L;
            public string _label;
            public string _value;
            public bool _isAnswer;
            public WebControl _control = null;

            public AnswerOption(long ID, string label, string value){
                _ID = ID;
                _label = label;
                _value = value;
            }

            public AnswerOption(long ID, string label, string value, bool isAnswer)
            {
                _ID = ID;
                _label = label;
                _value = value;
                _isAnswer = isAnswer;
            }

            public bool isControlSelected(){
                bool retValue = false;
                
                CheckBox cb = _control as CheckBox;
                if (cb != null){
                    retValue = cb.Checked;
                }

                return retValue;
            }
        }//AnswerOption


        public const int ALIGN_HORIZONTAL = 0;
        public const int ALIGN_VERTICAL   = 1;
        public const int ALIGN_TABLE      = 2;

        protected const string SUGGESTION_FLAG            = "suggestionFlag";
        protected const string SUGGESTION_FLAG_TABLEALIGN = "tableAlign";


        protected bool _isPostBack = false;
        protected long _ID = -1L;
        protected long _executionID = -1L;
        protected long _answerID = -1L;
        protected bool _answerRequired = false;
        protected bool _asExecutionTitle = false;
        protected int _alignment = ALIGN_HORIZONTAL;
        protected bool _focusControlSet = false;
        protected string _origAnswerText = "";

        protected Table _table = null;
        protected TableRow _currentRow = null;
        protected TableCell _currentCell = null;
        protected TableCell _validatorCell = null;
        
        protected DBData _db = null;
        protected DataRow _qeDataRow = null;
        protected DataRow _answerDataRow = null;

        protected LanguageMapper _mapper = null;
        protected ArrayList _answerOptions = null;

        protected CustomValidator _customRequiredValidator = null;
        protected CustomValidator _customRangeValidator = null;

        protected bool _enabled = true;

        public bool Enabled 
        {
            set { _enabled = value; }
            get { return _enabled;  }
        }

        public QuestionElement(LanguageMapper mapper, long ID, long executionID, Table table, bool isPostBack) {
            _mapper = mapper;
            _table = table;
            _ID = ID;
            _executionID = executionID;
            _isPostBack = isPostBack;
        }

        protected TableRow addRow(){
            return addRow("");
        }

        protected TableRow addRow(string flag){
            _currentRow = new TableRow();
            if (flag != ""){
                setSurveyFlag(_currentRow, flag);
            }
            _table.Rows.Add(_currentRow);
            _validatorCell = new TableCell();
            _validatorCell.ColumnSpan = 20;
            _validatorCell.EnableViewState = false;
            _currentRow.Cells.Add(_validatorCell);
            return _currentRow;
        }

        protected TableCell addCell(){
            _currentCell = new TableCell();
             _currentRow.Cells.AddAt(_currentRow.Cells.GetCellIndex(_validatorCell), _currentCell);
            _currentCell.Enabled = this._enabled;
            return _currentCell;
        }

        protected void setSurveyFlag(TableRow row, string flag){
            row.Attributes.Add(SUGGESTION_FLAG, flag);
        }

        protected string getSurveyFlag(TableRow row){
            return ch.psoft.Util.Validate.GetValid(row.Attributes[SUGGESTION_FLAG], "");
        }

        protected bool IsOpenTableAlign{
            get{
                bool retValue = false;
                int index = _table.Rows.GetRowIndex(_currentRow);
                if (index > 0){
                    retValue = getSurveyFlag(_table.Rows[index-1]) == SUGGESTION_FLAG_TABLEALIGN;
                }
                return retValue;
            }
        }

        protected void createTableAlignHeader(){
            addCell();
            foreach (AnswerOption answerOption in _answerOptions){
                addCell().Text = answerOption._label;
                _currentCell.HorizontalAlign = HorizontalAlign.Center;
            }
            addRow();
        }

        protected void setTableAlignRow(){
            if (!IsOpenTableAlign){
                createTableAlignHeader();
            }
            setSurveyFlag(_currentRow, SUGGESTION_FLAG_TABLEALIGN);
        }
        /// <summary>
        /// Returns true, if the user answered the question.
        /// </summary>
        public virtual bool AnswerExists{
            get{return false;}
        }

        /// <summary>
        /// Returns the answer as a string. Used to fill the execution-title if the flag is set.
        /// </summary>
        public virtual string AnswerText{
            get{return "";}
        }

        /// <summary>
        /// Returns the already in the database existing answer as a string. Used to fill the execution-title if the flag is set.
        /// </summary>
        public virtual string OrigAnswerText{
            get{return _origAnswerText;}
        }

        /// <summary>
        /// Sets the focus-control ID on the Question-control if it exists.
        /// </summary>
        public void setFocusControlID(bool setFocus){
            PsoftContentPage.SetFocusControl(InputControl, setFocus);
        }

        /// <summary>
        /// Returns the client-ID of the generated control, if it needs the focus.
        /// </summary>
        protected virtual WebControl InputControl{
            get{return null;}
        }

        /// <summary>
        /// Add the description whether the answer must be a text, a number, a date, ...
        /// </summary>
        protected void addLabelCell()
        {
            TableCell retValue = addCell(); // returns _currentCell (retValue is _currentCell)
            int typ = DBColumn.GetValid(_qeDataRow[_db.langAttrName("SUGGESTION_QUESTION", "TYP")], -1);
            bool isCompulsory = (DBColumn.GetValid(_qeDataRow["ANSWERREQUIRED"], 0) == 1);
            if(isCompulsory) 
            {
                retValue.Font.Bold = true; 
            }

            retValue.Text = createInfoLabels(typ)[0];
            retValue.ToolTip = createInfoLabels(typ)[1];

            retValue.CssClass =  "suggestionAnswerLabelTableCell"; // width to be set in Psoft.css
        }


        /// <summary>
        /// Add range cell 
        /// </summary>
        protected TableCell addRangeLabelCell()
        {
            TableCell retValue = addCell();
            int typ = DBColumn.GetValid(_qeDataRow[_db.langAttrName("SUGGESTION_QUESTION", "TYP")], -1);
            
            retValue.Text = createInfoLabels(typ)[1];
            
            retValue.CssClass =  "suggestionRangeLabelTableCell"; // width to be set in Psoft.css
            return retValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns>an array with two elements the first element contains 
        /// the label text, the second one the range if any</returns>
        private string[] createInfoLabels(int type) 
        {
            string[] ret = new string[2];
            ret[0] = ret[1] = "";
            switch (type)
            {
                case QuestionCtrl.TYP_FREE_TEXT:
                    ret[0] = this._mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, "labelFreeText");

                    //int charsMin = DBColumn.GetValid(_qeDataRow["CHARS_MIN"], QuestionCtrl.MIN_INPUT_CHARS);
                    //int charsMax = DBColumn.GetValid(_qeDataRow["CHARS_MAX"], QuestionCtrl.MAX_INPUT_CHARS);
                    //if( charsMin > QuestionCtrl.MIN_INPUT_CHARS || charsMax < QuestionCtrl.MAX_INPUT_CHARS) 
                    //{
                    //    retValue.Text = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_INFO_RANGE_TEXT_MIN_MAX).Replace("#1", charsMin.ToString()).Replace("#2", charsMax.ToString());
                    //}
                    break;
                case QuestionCtrl.TYP_NUMBER:
                    ret[0] = this._mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, "labelNumber");

                    double numMin = DBColumn.GetValid(_qeDataRow["NUMBER_MIN"], double.MinValue);
                    double numMax = DBColumn.GetValid(_qeDataRow["NUMBER_MAX"], double.MaxValue);
                    if( (numMin>double.MinValue) && (numMax < double.MaxValue) )
                    {
                        ret[1] = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_INFO_RANGE_NUMBER_MIN_MAX).Replace("#1", numMin.ToString()).Replace("#2", numMax.ToString());
                    } 
                    else if(numMin > double.MinValue) 
                    {
                        ret[1] = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_INFO_RANGE_NUMBER_MIN).Replace("#1", numMin.ToString());
                    }
                    else if(numMax < double.MaxValue)
                    {
                        ret[1] = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_INFO_RANGE_NUMBER_MAX).Replace("#1", numMax.ToString());
                    }
                    break;
                case QuestionCtrl.TYP_DATE:
                    ret[0] = this._mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, "labelDate");

                    DateTime dateMin = DBColumn.GetValid(_qeDataRow["DATE_MIN"], DateTime.MinValue);
                    DateTime dateMax = DBColumn.GetValid(_qeDataRow["DATE_MAX"], DateTime.MaxValue);
                    if( (dateMin > DateTime.MinValue) && (dateMax < DateTime.MaxValue) ) 
                    {
                        ret[1] = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_INFO_RANGE_DATE_MIN_MAX).Replace("#1", dateMin.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo)).Replace("#2", dateMax.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo) ).Replace("#2", dateMin.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo)).Replace("#2", dateMax.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo));
                    }
                    else if(dateMin > DateTime.MinValue)
                    {
                        ret[1] = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_INFO_RANGE_DATE_MIN).Replace("#1", dateMin.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo)).Replace("#2", dateMax.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo) ).Replace("#2", dateMin.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo));
                    }
                    else if(dateMax < DateTime.MaxValue)
                    {
                        ret[1] = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_INFO_RANGE_DATE_MAX).Replace("#1", dateMax.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo)).Replace("#2", dateMax.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo) ).Replace("#2", dateMin.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo));
                    }
                    break;
                case QuestionCtrl.TYP_BOOLEAN:
                    ret[0] = this._mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, "labelYesOrNo");
                    ret[1] = "";
                    break;
                case QuestionCtrl.TYP_SINGLE_CHOICE_DROPDOWN:
                    ret[0] = this._mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, "labelSingleChoiceDropDown");
                    ret[1] = "";
                    break;
                default:
                    ret[0] = this._mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, "labelFreeText");
                    ret[1] = "";
                    break;
            }
            return ret;
        }

        /// <summary>
        /// The method where all the controls must be loaded.
        /// </summary>
        public virtual void Load(DBData db){
            _db = db;
            addRow();

            //no questionelement in suggestions
            //DataTable table = _db.getDataTable("select * from QUESTIONELEMENT where ID=" + _ID);
            DataTable table = _db.getDataTable("select * from SUGGESTION_QUESTION where ID=" + _ID);
            if (table.Rows.Count > 0){
                _qeDataRow = table.Rows[0];
                _answerRequired = DBColumn.GetValid(_qeDataRow["ANSWERREQUIRED"], 0) > 0;
                _asExecutionTitle = false;//DBColumn.GetValid(_qeDataRow["ASEXECUTIONTITLE"], 0) > 0;
                _alignment = 2;//DBColumn.GetValid(_qeDataRow["ALIGNMENT"], 0);
            }

            table = _db.getDataTable("select * from SUGGESTION_ANSWER where SUGGESTION_QUESTION_ID=" + _ID + " and SUGGESTION_EXECUTION_ID=" + _executionID);
            if (table.Rows.Count > 0) {
                _answerDataRow = table.Rows[0];
                _answerID = DBColumn.GetValid(_answerDataRow["ID"], -1L);
            }

            if (_answerRequired){
                //Add a custom required-validator. Used to check if the user answered a required question-element.
                _customRequiredValidator = new CustomValidator();
                _customRequiredValidator.ErrorMessage = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_ANSWER_REQUIRED);
                _customRequiredValidator.Display = ValidatorDisplay.Dynamic;
                _customRequiredValidator.EnableClientScript = false;
                _customRequiredValidator.ServerValidate += new ServerValidateEventHandler(CustomRequiredValidate);
                _validatorCell.Controls.Add(_customRequiredValidator);
            }
        }

        /// <summary>
        /// Loads all answer-options assigned to the question-element.
        /// </summary>
        protected void loadAnswerOptions(){
            _answerOptions = new ArrayList();
            //TODO add deleted
            string sql = "select q.*, a.ID as AID from SUGGESTION_QUESTIONOPTIONS q left join SUGGESTION_ANSWER a on q.ID = a.SUGGESTION_QUESTIONOPTIONS_ID  and a.SUGGESTION_EXECUTION_ID=" + _executionID + "where q.SUGGESTION_QUESTION_ID=" + _ID + " order by q.ORDNUMBER asc";
            DataTable table = _db.getDataTable(sql);
            string labelColumn = _db.langAttrName("SUGGESTION_QUESTIONOPTIONS", "TITLE");
            string valueColumn = _db.langAttrName("SUGGESTION_QUESTIONOPTIONS", "TITLE");  //TODO
            bool isFirst = true;
            foreach (DataRow row in table.Rows){
                AnswerOption answerOption = new AnswerOption(DBColumn.GetValid(row["ID"], -1L), DBColumn.GetValid(row[labelColumn], ""), DBColumn.GetValid(row[valueColumn], ""), DBColumn.GetValid(row["AID"], -1L) > 0);
                _answerOptions.Add(answerOption);
                /*if (answerOption._isAnswer){
                    if (isFirst){
                        isFirst = false;
                    }
                    else{
                        _origAnswerText += ";";
                    }
                    _origAnswerText += answerOption._label;
                }*/
            }
        }

        private void CustomRequiredValidate(object source, ServerValidateEventArgs args){
            CustomRequiredValidation(source, args);
            if (!args.IsValid){
                setFocusControlID(true);
            }
        }

        /// <summary>
        /// Ovverridable delegete-method to check if the user answered a required question-element. Set args.IsValid = true if answer exists.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected virtual void CustomRequiredValidation(object source, ServerValidateEventArgs args){
        }

        /// <summary>
        /// Saves the answer of this question-element, but only if an answer exists.
        /// </summary>
        /// <param name="db"></param>
        public void saveAnswer(DBData db){
            _db = db;

            //remove previous answer if it exists
            _db.execute("delete from SUGGESTION_ANSWER where SUGGESTION_QUESTION_ID=" + _ID + " and SUGGESTION_EXECUTION_ID=" + _executionID);

            //create new answer
            if (AnswerExists){
                _answerID = _db.newId("SUGGESTION_ANSWER");
                string attributes = "insert into SUGGESTION_ANSWER (ID, SUGGESTION_QUESTION_ID, SUGGESTION_EXECUTION_ID";
                string values = ") values (" + _answerID + "," + _ID + "," + _executionID;
                appendSaveAttributes(ref attributes, ref values);
                _db.execute(attributes + values + ")");

                if (_asExecutionTitle){
                    string answerText = AnswerText;
                    string origAnswerText = ", " + OrigAnswerText + ",";
                    if (answerText != "" || origAnswerText != ""){
                        string executionTitle = ", " + ch.psoft.Util.Validate.GetValid(_db.lookup("TITLE", "SUGGESTION_EXECUTION", "ID=" + _executionID, false), "") + ",";
                        if (origAnswerText != ", ," && executionTitle.IndexOf(origAnswerText) >= 0){
                            executionTitle = executionTitle.Replace(origAnswerText, ", " + answerText + ",");
                            executionTitle = executionTitle.Substring(2, executionTitle.Length-3);
                        }
                        else{
                            executionTitle = executionTitle.Substring(2, executionTitle.Length-3);
                            if (executionTitle != ""){
                                executionTitle += ", ";
                            }
                            executionTitle += answerText;
                        }
                        _db.execute("update SUGGESTION_EXECUTION set TITLE='" + executionTitle + "' where ID=" + _executionID);
                    }
                }
            }

            /// no selected answers in suggestions.
            //assignSelectedAnswers();
        }

        /// <summary>
        /// Called by saveAnswer() to append additional question-type-specific attributes to the sql-statement.
        /// </summary>
        /// <param name="attributes">Attribute-string: append additional columns separated through commas.</param>
        /// <param name="values">Value-string: append additional valules separated through commas.</param>
        protected virtual void appendSaveAttributes(ref String attributes, ref String values){
        }

                /// no selected answers in suggestions.
//        /// <summary>
//        /// Called by saveAnswer() to assign the selected answer-options as answer-elements.
//        /// </summary>
//        protected virtual void assignSelectedAnswers(){
//        }
    }

    /// <summary>
    /// Abstract class for question-elements containing only a text-box (free-text, number, date)
    /// </summary>
    public abstract class QuestionElementTextBox : QuestionElement {
        protected TextBox _tbAnswer = null;
        protected TableCell _rangeLabelCell = null;

        public QuestionElementTextBox(LanguageMapper mapper, long ID, long executionID, Table table, bool isPostBack) : base(mapper, ID, executionID, table, isPostBack){
        }

        public override bool AnswerExists{
            get{return _tbAnswer.Text.Length > 0;}
        }

        protected override WebControl InputControl{
            get{ return _tbAnswer;}
        }

        public override void Load(DBData db){
            base.Load(db);
            addLabelCell();
            addCell();
            _tbAnswer = new TextBox();
            _tbAnswer.Enabled = this.Enabled;
            _tbAnswer.ID = "SUGGESTION_ANSWER" + _ID;
            _currentCell.Controls.Add(_tbAnswer);
            _customRangeValidator = new CustomValidator();
            _customRangeValidator.ErrorMessage = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_OUT_OF_RANGE);
            _customRangeValidator.Display = ValidatorDisplay.Dynamic;
            _customRangeValidator.EnableClientScript = false;
            _customRangeValidator.ServerValidate += new ServerValidateEventHandler(CustomRangeValidate);
            _validatorCell.Controls.Add(_customRangeValidator);
            setFocusControlID(!_isPostBack);
            _rangeLabelCell = addRangeLabelCell(); //this is now the current cell, if the _currentCell is used further call addRangeLabelCell() once again.
          
        }

        protected override void CustomRequiredValidation(object source, ServerValidateEventArgs args){
            args.IsValid = _tbAnswer.Text.Length > 0;
        }

        private void CustomRangeValidate(object source, ServerValidateEventArgs args){
            if (!_answerRequired || (_customRequiredValidator != null && _customRequiredValidator.IsValid)){
                CustomRangeValidation(source, args);
                if (!args.IsValid)
                {
                    setFocusControlID(true); 
                    _rangeLabelCell.Visible = false;
                } 
                else 
                {
                    _rangeLabelCell.Visible = true;
                }
            }
            else{
               args.IsValid = true;
               _rangeLabelCell.Visible = true;
            }
        }

        protected virtual void CustomRangeValidation(object source, ServerValidateEventArgs args){
        }
    }

    /// <summary>
    /// Abstract class for question-elements containing a drop-down list (boolean, single-choice dropdown).
    /// </summary>
    public abstract class QuestionElementDropDown : QuestionElement {
        protected const string NOANSWER_ITEMVALUE = "_-noANSWer-_";
        protected DropDownList _ddAnswer = null;

        public QuestionElementDropDown(LanguageMapper mapper, long ID, long executionID, Table table, bool isPostBack) : base(mapper, ID, executionID, table, isPostBack){
        }

        public override bool AnswerExists{
            get{return _ddAnswer.SelectedItem.Value != NOANSWER_ITEMVALUE;}
        }

        public override string AnswerText{
            get{return _ddAnswer.SelectedItem.Text;}
        }

        protected override WebControl InputControl{
            get{
                return _ddAnswer;
            }
        }

        public override void Load(DBData db){
            base.Load(db);
            addLabelCell();
            addCell();
            _ddAnswer = new DropDownCtrl();
            _ddAnswer.ID = "SUGGESTION_QUESTIONOPTIONS" + _ID;
            _currentCell.Controls.Add(_ddAnswer);
            setFocusControlID(!_isPostBack);
        }

        protected override void CustomRequiredValidation(object source, ServerValidateEventArgs args){
            args.IsValid = _ddAnswer.SelectedItem.Value != NOANSWER_ITEMVALUE;
        }
    }

    /// <summary>
    /// Question-element with textbox to enter a free-text.
    /// </summary>
    public class QuestionElementFreeText : QuestionElementTextBox {

        const int FREETEXT_CELL_COL_WIDTH = 80;

        protected int _charsMin = QuestionCtrl.MIN_INPUT_CHARS;
        protected int _charsMax = QuestionCtrl.MAX_INPUT_CHARS;

        protected string _answerText = "";

        public QuestionElementFreeText(LanguageMapper mapper, long ID, long executionID, Table table, bool isPostBack) : base(mapper, ID, executionID, table, isPostBack){
        }

        public override string AnswerText{
            get{return _answerText;}
        }

        public override void Load(DBData db){
            base.Load(db);
            _charsMin = DBColumn.GetValid(_qeDataRow["CHARS_MIN"], _charsMin);
            _charsMax = DBColumn.GetValid(_qeDataRow["CHARS_MAX"], _charsMax);
            //should not be too long
            if(_charsMax>(QuestionCtrl.MAX_INPUT_CHARS)) 
            {
                _charsMax = QuestionCtrl.MAX_INPUT_CHARS;
            }
            _tbAnswer.Columns = FREETEXT_CELL_COL_WIDTH;
            _tbAnswer.CssClass = "suggestionAnswerFreeTextCell";
            _tbAnswer.Rows = _charsMax / 200 + 1;
            _tbAnswer.MaxLength = _charsMax;
            if (_tbAnswer.Rows > 1){
                _tbAnswer.TextMode = TextBoxMode.MultiLine;
            }
            if (_answerDataRow != null){
                _origAnswerText = DBColumn.GetValid(_answerDataRow[_db.langAttrName("SUGGESTION_ANSWER", "TEXT")], "");
                if (!_isPostBack){
                    _tbAnswer.Text = _origAnswerText;
                }
            }

            if (_charsMin > 0){
                if (_charsMax < QuestionCtrl.MAX_INPUT_CHARS){
                    _customRangeValidator.ErrorMessage = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_RANGE_TEXT_MIN_MAX).Replace("#1", _charsMin.ToString()).Replace("#2", _charsMax.ToString());
                }
                else{
                    _customRangeValidator.ErrorMessage = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_RANGE_TEXT_MIN).Replace("#1", _charsMin.ToString());
                }
            }
            else if (_charsMax < QuestionCtrl.MAX_INPUT_CHARS){
                _customRangeValidator.ErrorMessage = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_RANGE_TEXT_MAX).Replace("#1", _charsMax.ToString());
            }
        }

        protected override void CustomRangeValidation(object source, ServerValidateEventArgs args){
            _answerText = _tbAnswer.Text;
            args.IsValid = (!_answerRequired && !AnswerExists) || (_answerText.Length >= _charsMin && _answerText.Length <= _charsMax);
        }

        protected override void appendSaveAttributes(ref String attributes, ref String values){
            attributes += "," + _db.langAttrName("SUGGESTION_ANSWER", "TEXT");
            values += ",'" + DBColumn.toSql(_answerText) + "'";
        }
    }

    /// <summary>
    /// Question-element with textbox to enter a number.
    /// </summary>
    public class QuestionElementNumber : QuestionElementTextBox {

        const int NUMBER_CELL_COL_WIDTH = 30;

        protected double _numberMin = double.MinValue;
        protected double _numberMax = double.MaxValue;
        protected double _answerNumber = double.MinValue;

        public QuestionElementNumber(LanguageMapper mapper, long ID, long executionID, Table table, bool isPostBack) : base(mapper, ID, executionID, table, isPostBack){
        }

        public override string AnswerText{
            get{return _answerNumber.ToString();}
        }

        public override void Load(DBData db){
            base.Load(db);
            _numberMin = DBColumn.GetValid(_qeDataRow["NUMBER_MIN"], _numberMin);
            _numberMax = DBColumn.GetValid(_qeDataRow["NUMBER_MAX"], _numberMax);
            _tbAnswer.Columns = NUMBER_CELL_COL_WIDTH;
            _tbAnswer.CssClass = "suggestionAnswerNumberCell";
            _tbAnswer.TextMode = TextBoxMode.SingleLine;
            if (_answerDataRow != null){
                _origAnswerText = DBColumn.GetValid(_answerDataRow["NUMBER"], "");
                if (!_isPostBack){
                    _tbAnswer.Text = _origAnswerText;
                }
            }

            if (_numberMin > double.MinValue){
                if (_numberMax < double.MaxValue){
                    _customRangeValidator.ErrorMessage = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_RANGE_NUMBER_MIN_MAX).Replace("#1", _numberMin.ToString()).Replace("#2", _numberMax.ToString());
                }
                else{
                    _customRangeValidator.ErrorMessage = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_RANGE_NUMBER_MIN).Replace("#1", _numberMin.ToString());
                }
            }
            else if (_numberMax < double.MaxValue){
                _customRangeValidator.ErrorMessage = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_RANGE_NUMBER_MAX).Replace("#1", _numberMax.ToString());
            }
        }

        protected override void CustomRangeValidation(object source, ServerValidateEventArgs args){
            _answerNumber = ch.psoft.Util.Validate.GetValid(_tbAnswer.Text, double.NaN);
            args.IsValid = (!_answerRequired && !AnswerExists) || (_answerNumber >= _numberMin && _answerNumber <= _numberMax);
        }

        protected override void appendSaveAttributes(ref String attributes, ref String values){
            attributes += ",NUMBER";
            values += "," + AnswerText;
        }
    }

    /// <summary>
    /// Question-element with textbox to enter a date.
    /// </summary>
    public class QuestionElementDate : QuestionElementTextBox {

        const int DATE_CELL_COL_WIDTH = 30;

        protected DateTime _dateMin = DateTime.MinValue;
        protected DateTime _dateMax = DateTime.MaxValue;
        protected DateTime _answerDate = DateTime.MinValue;

        public QuestionElementDate(LanguageMapper mapper, long ID, long executionID, Table table, bool isPostBack) : base(mapper, ID, executionID, table, isPostBack){
        }

        public override string AnswerText{
            get{return _answerDate.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo);}
        }

        public override void Load(DBData db){
            base.Load(db);
            _dateMin = DBColumn.GetValid(_qeDataRow["DATE_MIN"], DateTime.MinValue);
            _dateMax = DBColumn.GetValid(_qeDataRow["DATE_MAX"], DateTime.MaxValue);
            _tbAnswer.Columns = DATE_CELL_COL_WIDTH;
            _tbAnswer.CssClass = "suggestionAnswerDateCell";
            _tbAnswer.TextMode = TextBoxMode.SingleLine;
            if (_answerDataRow != null && !DBColumn.IsNull(_answerDataRow["DATE"])){
                _origAnswerText = ((DateTime) _answerDataRow["DATE"]).ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo);
                if (!_isPostBack){
                    _tbAnswer.Text = _origAnswerText;
                }
            }

            Label l = new Label();
            l.Text = " ";
            _currentCell.Controls.Add(l);
            //_currentCell.Controls.Add(DateSelector.getDateSelectorButton(_tbAnswer, ""));
            //_currentCell.Controls.Add(CalendarSelector.getDateSelectorButton(_tbAnswer, ""));
            CalendarSelector calsel = new CalendarSelector(_mapper, _tbAnswer, "");
            _currentCell.Controls.Add(calsel.imgButton); 
            _currentCell.Controls.Add(calsel.cal);
            
            _rangeLabelCell = addRangeLabelCell(); 

            if (_dateMin > DateTime.MinValue){
                if (_dateMax < DateTime.MaxValue){
                    _customRangeValidator.ErrorMessage = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_RANGE_DATE_MIN_MAX).Replace("#1", _dateMin.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo)).Replace("#2", _dateMax.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo));
                }
                else{
                    _customRangeValidator.ErrorMessage = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_RANGE_DATE_MIN).Replace("#1", _dateMin.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo));
                }
            }
            else if (_dateMax < DateTime.MaxValue){
                _customRangeValidator.ErrorMessage = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_VAL_RANGE_DATE_MAX).Replace("#1", _dateMax.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, DateTimeFormatInfo.InvariantInfo));
            }
        }

        protected override void CustomRangeValidation(object source, ServerValidateEventArgs args){
            if (AnswerExists){
                try{
                    _answerDate = DateTime.Parse(_tbAnswer.Text, _db.dbColumn.UserCulture);
                    args.IsValid = _answerDate >= _dateMin && _answerDate <= _dateMax;
                }
                catch(Exception){
                    args.IsValid = false;
                }
            }
            else{
                args.IsValid = !_answerRequired;
            }
        }

        protected override void appendSaveAttributes(ref String attributes, ref String values){
            attributes += ",DATE";
            values += ",'" + _answerDate.ToString(DBColumn.DBCulture) + "'";
        }
    }

    /// <summary>
    /// Question-element with a boolean type of answer.
    /// </summary>
    public class QuestionElementBoolean : QuestionElementDropDown {

        public QuestionElementBoolean(LanguageMapper mapper, long ID, long executionID, Table table, bool isPostBack) : base(mapper, ID, executionID, table, isPostBack){
        }

        public override void Load(DBData db){
            base.Load(db);
            int origIndex = -1;
            if (_answerDataRow != null){
                switch (DBColumn.GetValid(_answerDataRow["BOOLEAN"], NOANSWER_ITEMVALUE)){
                    case NOANSWER_ITEMVALUE:
                        origIndex = 0;
                        break;
                                        
                    case "0":
                        origIndex = 1;
                        break;

                    case "1":
                        origIndex = 2;
                        break;
                }
            }
            if (!_isPostBack){
                _ddAnswer.Items.Add(new ListItem("", NOANSWER_ITEMVALUE));
                _ddAnswer.Items.Add(new ListItem(_mapper.get("no"), "0"));
                _ddAnswer.Items.Add(new ListItem(_mapper.get("yes"), "1"));
                if (origIndex >= 0){
                    _ddAnswer.SelectedIndex = origIndex;
                }
            }
            if (origIndex >= 0){
                _origAnswerText = _ddAnswer.Items[origIndex].Text;
            }
        }
    
        protected override void appendSaveAttributes(ref String attributes, ref String values){
            attributes += ",BOOLEAN";
            values += "," + _ddAnswer.SelectedItem.Value;
        }
    }

    /// <summary>
    /// Question-element representing a single-choice question with radio-buttons.
    /// </summary>
    public class QuestionElementSingleChoiceRadio : QuestionElement {

        public QuestionElementSingleChoiceRadio(LanguageMapper mapper, long ID, long executionID, Table table, bool isPostBack) : base(mapper, ID, executionID, table, isPostBack){
        }

        public override bool AnswerExists{
            get{
                foreach (AnswerOption answerOption in _answerOptions){
                    if (answerOption.isControlSelected()){
                        return true;
                    }
                }
                return false;
            }
        }

        public override string AnswerText{
            get{
                foreach (AnswerOption answerOption in _answerOptions){
                    if (answerOption.isControlSelected()){
                       return answerOption._label;
                    }
                }
                return "";
            }
        }

        protected override WebControl InputControl{
            get{
                if (_answerOptions.Count > 0){
                    return ((AnswerOption) _answerOptions[0])._control;
                }
                return null;
            }
        }

        public override void Load(DBData db){
            base.Load(db);
            loadAnswerOptions();
            if (_alignment == ALIGN_TABLE){
                setTableAlignRow();
            }
            addLabelCell();
            foreach (AnswerOption answerOption in _answerOptions){
                if (_alignment == ALIGN_VERTICAL){
                    addRow();
                    addCell();
                }
                RadioButton rb = new RadioButton();
                answerOption._control = rb;
                rb.GroupName = _ID.ToString();
                //rb.Checked = answerOption._isAnswer;
                if (_alignment != ALIGN_TABLE){
                    rb.Text = answerOption._label;
                }
                addCell().Controls.Add(rb);
                if (_alignment == ALIGN_TABLE){
                    _currentCell.HorizontalAlign = HorizontalAlign.Center;
                }
            }
            setFocusControlID(!_isPostBack);
        }

        protected override void CustomRequiredValidation(object source, ServerValidateEventArgs args){
            args.IsValid = false;

            foreach (AnswerOption answerOption in _answerOptions){
                if (answerOption.isControlSelected()){
                    args.IsValid = true;
                    break;
                }
            }
        }
    
    }

    /// <summary>
    /// Question-element representing a single-choice question with drop-down list.
    /// </summary>
    public class QuestionElementSingleChoiceDropDown : QuestionElementDropDown {

        public QuestionElementSingleChoiceDropDown(LanguageMapper mapper, long ID, long executionID, Table table, bool isPostBack) : base(mapper, ID, executionID, table, isPostBack){
        }

        public override void Load(DBData db){
            base.Load(db);
            loadAnswerOptions();
            if (!_isPostBack){
                _ddAnswer.Items.Add(new ListItem("", NOANSWER_ITEMVALUE));
                foreach (AnswerOption answerOption in _answerOptions){
                    ListItem item = new ListItem(answerOption._label, answerOption._ID.ToString());
                    _ddAnswer.Items.Add(item);
                    item.Selected = answerOption._isAnswer;
                }
            }
        }

        protected override void appendSaveAttributes(ref String attributes, ref String values)
        {
            attributes += "," + _db.langAttrName("SUGGESTION_ANSWER", "TEXT");
            attributes += ",SUGGESTION_QUESTIONOPTIONS_ID";
            values += ",'" + _ddAnswer.SelectedItem.Text + "'";
            values += "," + _ddAnswer.SelectedItem.Value;
        }
    }

    /// <summary>
    /// Question-element representing a multiple-choice question with check-boxes.
    /// </summary>
    public class QuestionElementMultipleChoice : QuestionElement {

        public QuestionElementMultipleChoice(LanguageMapper mapper, long ID, long executionID, Table table, bool isPostBack) : base(mapper, ID, executionID, table, isPostBack){
        }

        public override bool AnswerExists{
            get{return true;}
        }

        protected override WebControl InputControl{
            get{
                if (_answerOptions.Count > 0){
                    return ((AnswerOption) _answerOptions[0])._control;
                }
                return null;
            }
        }

        public override string AnswerText{
            get{
                string retValue = "";
                bool isFirst = true;
                foreach (AnswerOption answerOption in _answerOptions){
                    if (answerOption.isControlSelected()){
                        if (isFirst){
                            isFirst = false;
                        }
                        else{
                            retValue += ";";
                        }
                        retValue += answerOption._label;
                    }
                }
                return retValue;
            }
        }

        public override void Load(DBData db){
            base.Load(db);
            loadAnswerOptions();
            if (_alignment == ALIGN_TABLE){
                setTableAlignRow();
            }
            addLabelCell();
            foreach (AnswerOption answerOption in _answerOptions){
                if (_alignment == ALIGN_VERTICAL){
                    addRow();
                    addCell();
                }
                CheckBox cb = new CheckBox();
                answerOption._control = cb;
                //cb.Checked = answerOption._isAnswer;
                if (_alignment != ALIGN_TABLE){
                    cb.Text = answerOption._label;
                }
                addCell().Controls.Add(cb);
                if (_alignment == ALIGN_TABLE){
                    _currentCell.HorizontalAlign = HorizontalAlign.Center;
                }
            }
            setFocusControlID(!_isPostBack);
        }

        protected override void CustomRequiredValidation(object source, ServerValidateEventArgs args){
            args.IsValid = true;
        }

        /// no selected answers in suggestions.
//        protected override void assignSelectedAnswers(){
//            foreach (AnswerOption answerOption in _answerOptions){
//                if (answerOption.isControlSelected()){
//                    _db.execute("insert into SUGGESTION_SELECTEDANSWER (ANSWER_ID, ANSWEROPTION_ID) values (" + _answerID + "," + answerOption._ID + ")");
//                }
//            }
//        }
    }
}
