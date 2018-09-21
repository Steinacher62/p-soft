namespace ch.appl.psoft.Survey.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class ExecutionList : PSOFTSearchListUserControl {
        public const string CONTEXT_SURVEY = "survey";
        public const string CONTEXT_SEARCHRESULT = "searchresult";

        public const string PARAM_EXECUTION_ID = "PARAM_EXECUTION_ID";
        public const string PARAM_X_ID = "PARAM_X_ID";
        public const string PARAM_KONTEXT = "PARAM_KONTEXT";
        public const string PARAM_POST_DELETE_URL = "PARAM_POST_DELETE_URL";


        protected DBData _db = null;

        public static string Path {
            get {return Global.Config.baseURL + "/Survey/Controls/ExecutionList.ascx";}
        }

        public ExecutionList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
        }

		#region Properities
        public long ExecutionID {
            get {return GetLong(PARAM_EXECUTION_ID);}
            set {SetParam(PARAM_EXECUTION_ID, value);}
        }

        public long xID {
            get {return GetLong(PARAM_X_ID);}
            set {SetParam(PARAM_X_ID, value);}
        }

        public string Kontext {
            get {return GetString(PARAM_KONTEXT);}
            set {SetParam(PARAM_KONTEXT, value);}
        }

        public string PostDeleteURL {
            get {return GetString(PARAM_POST_DELETE_URL);}
            set {SetParam(PARAM_POST_DELETE_URL, value);}
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
                _db.connect();
                string sql = "select * from EXECUTION";

                switch (Kontext){
                    case CONTEXT_SURVEY:
                        sql += " where SURVEY_ID=" + xID;
                        pageTitle.Text = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_CT_SURVEY_EXECUTIONS).Replace("#1", _db.lookup("TITLE", "SURVEY", "ID=" + xID, false));
                        break;

                    case CONTEXT_SEARCHRESULT:
                        sql += " where ID in (select ROW_ID from SEARCHRESULT where ID=" + xID + ")";
                        pageTitle.Text = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_CT_EXECUTION_SELECTION);
                        break;
                }

                sql += " and PERSON_ID=" + _db.userId;
                sql += " order by " + OrderColumn + " " + OrderDir;

                DataTable table = _db.getDataTableExt(sql, "EXECUTION");
                IDColumn = "ID";
                if (ExecutionID > 0)
                    HighlightRecordID = ExecutionID;

                table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                table.Columns["PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%PERSON_ID", "mode","oe");

                LoadList(_db, table, listTab);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (col != null){
                switch(col.ColumnName) {
                    case "ISFINISHED":
                        System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                        if (DBColumn.GetValid(row[col], -1) > 0){
                            image.ImageUrl = "../../images/ampelGruen.gif";
                        }
                        else{
                            image.ImageUrl = "../../images/ampelRot.gif";
                        }
                        cell.Controls.Add(image);
                        cell.HorizontalAlign = HorizontalAlign.Center;
                        break;

                    case "TITLE":
                        if (cell.Controls.Count > 0){
                            HyperLink l = cell.Controls[0] as HyperLink;
                            if (l != null && l.Text == ""){
                                l.Text = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_NO_EXECUTION_TITLE);
                            }
                        }
                        break;
                }
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
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}
