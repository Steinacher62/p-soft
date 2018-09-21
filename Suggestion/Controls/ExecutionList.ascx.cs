namespace ch.appl.psoft.Suggestion.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class ExecutionList : PSOFTListViewUserControl {
        public const string CONTEXT_SUGGESTION = "suggestion";
        public const string CONTEXT_SEARCHRESULT = "searchresult";
        public const string CONTEXT_ALLRESULT = "allresult";

        public const string PARAM_EXECUTION_ID = "PARAM_EXECUTION_ID";
        public const string PARAM_X_ID = "PARAM_X_ID";
        public const string PARAM_KONTEXT = "PARAM_KONTEXT";
        public const string PARAM_POST_DELETE_URL = "PARAM_POST_DELETE_URL";


		protected long _personID = -1;

        protected DBData _db = null;

        public static string Path {
            get {return Global.Config.baseURL + "/Suggestion/Controls/ExecutionList.ascx";}
        }

        public ExecutionList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = true;
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
            //listTab.Rows.Clear();

            _db = DBData.getDBData(Session);
            try {
                _db.connect();
                string sql = "select *, PERSON_ID AS USER_ID from SUGGESTION_EXECUTION";

                switch (Kontext){
                    case CONTEXT_ALLRESULT:
                    case CONTEXT_SUGGESTION:
                        sql += " where SUGGESTION_ID=" + xID;
                        pageTitle.Text = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_CT_SUGGESTION_EXECUTIONS).Replace("#1", _db.lookup("TITLE", "SUGGESTION", "ID=" + xID, false));
                        break;
                    case CONTEXT_SEARCHRESULT:
                        sql += " where ID in (select ROW_ID from SEARCHRESULT where ID=" + xID + ")";
                        pageTitle.Text = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_CT_EXECUTION_SELECTION);
                        break;
                }

                if(!Kontext.Equals(CONTEXT_ALLRESULT)) 
                {
                    sql += " and PERSON_ID=" + _db.userId;
                }
                sql += " order by " + OrderColumn + " " + OrderDir;

                DataTable table = _db.getDataTableExt(sql, "SUGGESTION_EXECUTION");
                IDColumn = "ID";

				if (ExecutionID > 0) 
				{
					HighlightRecordID = ExecutionID;
				} 
				else if(table.Rows.Count > 0) 
				{
					HighlightRecordID = (long)table.Rows[0]["ID"];
				}

                table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                table.Columns["PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%PERSON_ID", "mode","oe");
				table.Columns["ISFINISHED"].ExtendedProperties["In"] = new System.Collections.ArrayList(_mapper.getEnum("suggestion", "finishedEnum", true));


				//link zum Wissenselement
                //this.EditURL = ch.appl.psoft.Knowledge.KnowledgeDetail.GetURL("context","history","suggestionExecutionID","%ID");
                this.EditURL = Global.Config.baseURL + "/Suggestion/Execute.aspx?executionID=%ID&suggestionID=" + xID;

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
            if (col != null)
            {
                switch(col.ColumnName) {
                    case "ISFINISHED":
                        System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                        if (DBColumn.GetValid(row[col], -1) == 1) //finished
                        {
                            image.ImageUrl = "../../images/ampelOrange.gif";
							image.AlternateText = _mapper.get("suggestion_execution","stateSent");
                        }
                        else if (DBColumn.GetValid(row[col], -1) == 2) //sent
                        {
                            image.ImageUrl = "../../images/ampelGruen.gif";
							image.AlternateText = _mapper.get("suggestion_execution","stateComplete");
                        }
                        else
                        {
                            image.ImageUrl = "../../images/ampelRot.gif";
							image.AlternateText = _mapper.get("suggestion_execution","stateNew");
                        }
                        cell.Controls.Add(image);
                        cell.HorizontalAlign = HorizontalAlign.Center;
                        break;

                    case "TITLE":
                        if (cell.Controls.Count > 0){
                            HyperLink l = cell.Controls[0] as HyperLink;
                            if (l != null && l.Text == ""){
                                l.Text = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_NO_EXECUTION_TITLE);
                            }
                        }
                        break;
                }
            }            
            else if (cell != null && row != null) 
            {
                if(cell.Controls[0] is HyperLink) 
                {
                    HyperLink l = cell.Controls[0] as HyperLink;
                    if(l.Text == "E" && (DBColumn.GetValid(row["ISFINISHED"], -1) == 2 ||
						_db.userId != DBColumn.GetValid(row["USER_ID"], -1L))) 
                    {
                        //disable edit if already sent or it is not your suggestion
                        l.Enabled = false;
                        l.NavigateUrl = "";
                    }
					//Abgesendete Vorschläge können nur mehr von Administratoren gelöscht werden
                    if(l.Text == "D") 
                    {
                        long suggestionID = DBColumn.GetValid(row["SUGGESTION_ID"], -1L);
                        bool isAdministrable = _db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "SUGGESTION", suggestionID, true, true);
                        bool isModuleRights = _db.hasApplicationAuthorisation(DBData.AUTHORISATION.ADMIN, DBData.APPLICATION_RIGHT.MODULE_SUGGESTION,true);
                        bool displayDelete = (isAdministrable || isModuleRights);
                        
                        if((DBColumn.GetValid(row["ISFINISHED"], -1) == 2))
                        {
                            l.ToolTip = _mapper.get("suggestion","deleteSuggKnow");
                            if(!displayDelete) 
                            {
                                l.Enabled = false;
                                l.NavigateUrl = "";
                            }
                        } 
                    }
                    
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
