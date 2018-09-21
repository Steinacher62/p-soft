namespace ch.appl.psoft.Survey.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Collections;
    using System.Data;


    public partial class SurveyList : PSOFTSearchListUserControl {
        public const string MODE_SEARCHRESULT = "searchresult";

        public const string CONTEXT_PERSON = "person";
        public const string CONTEXT_SEARCHRESULT = "searchresult";

        public const string PARAM_SQL = "PARAM_SQL";
        public const string PARAM_BACK_URL = "PARAM_BACK_URL";
        public const string PARAM_RELOAD = "PARAM_RELOAD";
        public const string PARAM_SURVEY_ID = "PARAM_SURVEY_ID";
        public const string PARAM_X_ID = "PARAM_X_ID";
        public const string PARAM_MODE = "PARAM_MODE";
        public const string PARAM_KONTEXT = "PARAM_KONTEXT";


        protected DBData _db = null;

        public static string Path {
            get {return Global.Config.baseURL + "/Survey/Controls/SurveyList.ascx";}
        }

        public SurveyList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = false;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            Mode = "";
        }

		#region Properities
        public long SurveyID {
            get {return GetLong(PARAM_SURVEY_ID);}
            set {SetParam(PARAM_SURVEY_ID, value);}
        }

        public long xID {
            get {return GetLong(PARAM_X_ID);}
            set {SetParam(PARAM_X_ID, value);}
        }

        public string Mode {
            get {return GetString(PARAM_MODE);}
            set {SetParam(PARAM_MODE, value);}
        }

        public string BackURL {
            get { return GetString(PARAM_BACK_URL); }
            set {SetParam(PARAM_BACK_URL, value);}
        }

        public string Kontext {
            get {return GetString(PARAM_KONTEXT);}
            set {SetParam(PARAM_KONTEXT, value);}
        }

        public bool Reload {
            get {return GetBool(PARAM_RELOAD);}
            set {SetParam(PARAM_RELOAD, value);}
        }

        public string SearchSQL {
            get {return GetString(PARAM_SQL);}
            set {SetParam(PARAM_SQL, value);}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();

            loadList();
        }

        protected void loadList(){
            listTab.Rows.Clear();

            _db = DBData.getDBData(Session);
            try {
                bool load = true;
                _db.connect();
                string sql = "select * from SURVEY";

                switch (Mode){
                    case MODE_SEARCHRESULT:
                        sql = SearchSQL;
                        load = Reload;
                        pageTitle.Text = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_CT_SURVEY_SEARCHRESULT);
                        next.Text = _mapper.get("next");
                        ButtonRow.Visible = true;
                        CheckBoxEnabled = true;
                        break;
                        
                    default:
                        switch (Kontext){
                            case CONTEXT_PERSON:
                                string involvedSurveys = _db.Survey.getAccessableSurveys(xID);
                                if (involvedSurveys != ""){
                                    sql += " where ID in (" + involvedSurveys + ")";
                                }
                                else {
                                    sql += " where ID=-1";
                                }
                                pageTitle.Text = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_CT_EXECUTABLE_SURVEYS).Replace("#1", _db.Person.getWholeName(xID));
                                break;

                            case CONTEXT_SEARCHRESULT:
                                sql += " where ID in (select ROW_ID from SEARCHRESULT where ID=" + xID + ")";
                                pageTitle.Text = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_CT_SURVEY_SELECTION);
                                break;
                        }
                        break;
                }

                if (load){
                    sql += " order by " + _db.langAttrName("SURVEY", OrderColumn) + " " + OrderDir;

                    DataTable table = _db.getDataTableExt(sql, "SURVEY");
                    IDColumn = "ID";
                    if (SurveyID > 0)
                        HighlightRecordID = SurveyID;

                    table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                    table.Columns["PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%PERSON_ID", "mode","oe");
                    ArrayList booleans = SurveyModule.getBooleans(_mapper);
                    table.Columns["ISANONYMOUS"].ExtendedProperties["In"] = booleans;
                    table.Columns["ISCORRECTABLE"].ExtendedProperties["In"] = booleans;
                    table.Columns["ISPUBLIC"].ExtendedProperties["In"] = booleans;

                    LoadList(_db, table, listTab);
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        /// <summary>
        /// Event handler for the 'next' button
        /// The selected item(s) database ID will be stored in the SEARCHRESULT table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void next_Click(object sender, System.EventArgs e) {
            long searchResultID = SaveInSearchResult(listTab, "SURVEY");

            BackURL = BackURL.Replace("%25SearchResultID","%SearchResultID").Replace("%SearchResultID", searchResultID.ToString());

            _nextArgs.LoadUrl = this.BackURL;
            DoOnNextClick(next);
        }

        private void mapControls () {
            this.next.Click += new System.EventHandler(this.next_Click);
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            mapControls();
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}
