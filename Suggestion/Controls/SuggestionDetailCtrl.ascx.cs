namespace ch.appl.psoft.Suggestion.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class SuggestionDetailCtrl : PSOFTDetailViewUserControl {
		public const string CONTEXT_SUGGESTION = "suggestion";
		public const string CONTEXT_SEARCHRESULT = "searchresult";
		public const string CONTEXT_ALLRESULT = "allresult";

        public const string CSS_CLASS_ID_INFO = "suggestionInfo";

        public const string PARAM_SUGGESTION_ID = "PARAM_SUGGESTION_ID";
		public const string PARAM_EXECUTION_ID = "PARAM_EXECUTION_ID";
		public const string PARAM_CONTEXT = "PARAM_CONTEXT";
		public const string PARAM_ORDERCOLUMN = "PARAM_ORDERCOLUMN";
		public const string PARAM_ORDERDIR = "PARAM_ORDERDIR";

        private DBData _db = null;
        private DataTable _table;


		protected PsoftLinksControl _links = null;

        public static string Path {
            get {return Global.Config.baseURL + "/Suggestion/Controls/SuggestionDetailCtrl.ascx";}
        }

		#region Properities
        public long SuggestionID {
            get {return GetLong(PARAM_SUGGESTION_ID);}
            set {SetParam(PARAM_SUGGESTION_ID, value);}
        }

		public long ExecutionID 
		{
			get {return GetLong(PARAM_EXECUTION_ID);}
			set {SetParam(PARAM_EXECUTION_ID, value);}
		}

		public string Kontext 
		{
			get {return GetString(PARAM_CONTEXT);}
			set {SetParam(PARAM_CONTEXT, value);}
		}

		public PsoftLinksControl Links 
		{
			get {return _links;}
			set {_links = value;}
		}

		public LanguageMapper Mapper 
		{
			get {return _mapper;}
			set {_mapper = value;}
		}

		public string OrderColumn 
		{
			get {return GetString(PARAM_ORDERCOLUMN);}
			set {SetParam(PARAM_ORDERCOLUMN, value);}
		}

		public string OrderDir 
		{
			get {return GetString(PARAM_ORDERDIR);}
			set {SetParam(PARAM_ORDERDIR, value);}
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
                detailTab.Rows.Clear();

                if(SuggestionID <= 0) 
                {
                    TableRow r = new TableRow();
                    TableCell c = new TableCell();
                    r.Cells.Add(c);
                    detailTab.Rows.Add(r);

                    Label l = new System.Web.UI.WebControls.Label();
                    l.Text = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_NO_ACTIVE_SUGGESTION_FOUND);
                    l.CssClass = CSS_CLASS_ID_INFO;
                    c.Controls.Add(l);

                    return; 
                }

                if( ExecutionID < 0)
                {
                    //First call of detail Suggestions Filled
                    string sql = "select * from SUGGESTION_EXECUTION";

                    switch (Kontext)
                    {
                        case CONTEXT_ALLRESULT:
                        case CONTEXT_SUGGESTION:
                            sql += " where SUGGESTION_ID=" + SuggestionID;
                            break;
                        case CONTEXT_SEARCHRESULT:
                            sql += " where ID in (select ROW_ID from SEARCHRESULT where ID=" + SuggestionID + ")";
                            break;
                    }

                    if(!Kontext.Equals(CONTEXT_ALLRESULT)) 
                    {
                        sql += " and PERSON_ID=" + _db.userId;
                    }

                    if(OrderColumn != "") 
                    {
                        sql += " order by " + OrderColumn + " " + OrderDir;
                    }

                    DataTable table = _db.getDataTableExt(sql, "SUGGESTION_EXECUTION");
					
                    if(table.Rows.Count > 0) 
                    {
                        ExecutionID = (long)table.Rows[0]["ID"];
                    }
                } 
                
				if(ExecutionID > 0) 
				{
					//A special Suggestion
					_table = _db.getDataTableExt("select * from SUGGESTION_DETAIL_V where SUGGESTION_ID= " + SuggestionID + " AND ID=" + ExecutionID, "SUGGESTION_DETAIL_V");

					_table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
					_table.Columns["PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%PERSON_ID", "mode","oe");
					_table.Columns["ISFINISHED"].ExtendedProperties["In"] = new System.Collections.ArrayList(_mapper.getEnum("suggestion", "finishedEnum", true));
                    _table.Columns["ISFINISHED"].ExtendedProperties["In"] = new System.Collections.ArrayList(_mapper.getEnum("suggestion", "finishedEnum", true));
				
					int isfinished = ch.psoft.Util.Validate.GetValid(_db.lookup("ISFINISHED", "SUGGESTION_DETAIL_V", "ID=" + ExecutionID, false),-1);
					if(isfinished == 2) 
					{
						_links.LinkGroup1.AddLink(_mapper.get("importantLinks"), _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_CM_LINK_KNOWLEDGE), ch.appl.psoft.Knowledge.KnowledgeDetail.GetURL("context","history","suggestionExecutionID",ExecutionID));
					}
					else if(_db.userId == ch.psoft.Util.Validate.GetValid(_db.lookup("PERSON_ID", "SUGGESTION_DETAIL_V", "ID=" + ExecutionID, false),-1))
					{
						_links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_CM_EDIT_SUGGESTION), Global.Config.baseURL + "/Suggestion/Execute.aspx?executionID=" + ExecutionID + "&suggestionID=" + SuggestionID);
					}
                    LoadDetail(_db, _table, detailTab);
				} 
				else
				{
                    DataTable table = _db.getDataTableExt("select * from SUGGESTION where ID= " + SuggestionID, "SUGGESTION");
                    LoadDetail(_db, table, detailTab);
                }
            }
            catch (Exception ex) {
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
