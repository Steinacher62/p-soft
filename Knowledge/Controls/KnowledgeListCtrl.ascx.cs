namespace ch.appl.psoft.Knowledge.Controls
{
    using db;
    using Interface.DBObjects;
    using LayoutControls;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    public partial  class KnowledgeListCtrl : PSOFTSearchListUserControl {
        protected DBData _db = null;


        public static string Path {
            get {return Global.Config.baseURL + "/Knowledge/Controls/KnowledgeListCtrl.ascx";}
        }

        #region Properities
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public string NextURL {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public const string PARAM_QUERY = "PARAM_QUERY";
        public string Query {
            get {return GetString(PARAM_QUERY);}
            set {SetParam(PARAM_QUERY, value);}
        }

        public const string PARAM_KNOWLEDGE_ID = "PARAM_KNOWLEDGE_ID";
        public long KnowledgeID {
            get {return GetLong(PARAM_KNOWLEDGE_ID);}
            set {SetParam(PARAM_KNOWLEDGE_ID, value);}
        }

        public const string PARAM_KONTEXT = "PARAM_KONTEXT";
        public string Kontext {
            get {return GetString(PARAM_KONTEXT);}
            set {SetParam(PARAM_KONTEXT, value);}
        }

        public const string PARAM_X_ID = "PARAM_X_ID";
        public long XID {
            get {return GetLong(PARAM_X_ID);}
            set {SetParam(PARAM_X_ID, value);}
        }

        #endregion
    
        public KnowledgeListCtrl() : base() {
            HeaderEnabled = true;
            DeleteEnabled = false;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            CheckOrder = true;
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }
        protected override void DoExecute() {
            _db = DBData.getDBData(Session);
            if (Visible)
                loadList();
        }

        private void loadList() {
            _db.connect();
            try {
                string sql = "select * from " + Knowledge._VIEWNAME;
                pageTitle.Text = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_BC_SEARCH_KNOWLEDGE);

                switch(Kontext) {
                    case "search":
                        if (NextURL != ""){
                            next.Visible = true;
                            RowsPerPage = 10;
                            //_listBuilder.rowsPerPage = 10;
                            next.Text = _mapper.get("next"); 
                            CheckBoxEnabled = true;
                            SingleResultRecord = true;
                        }
                        if (Query != ""){
                            sql = Query;
                        }
                        break;

                    case "searchResult":
                        sql += " where ID in (select ROW_ID from SEARCHRESULT where TABLENAME='" + Knowledge._VIEWNAME + "' and ID=" + XID + ")";
                        break;
                }//switch

                sql += " order by " + OrderColumn + " " + OrderDir;

                DataTable table = _db.getDataTableExt(sql, Knowledge._VIEWNAME);
                foreach (DataRow row in table.Rows)
                {
                    if (!_db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "Knowledge", Convert.ToInt32(row["ID"]), true, true))
                    {
                        row.Delete();
                    }
                }

                table.AcceptChanges();

                DataTable personTab = _db.Person.getWholeNameMATable(true);
                table.Columns[_db.langAttrName(Knowledge._VIEWNAME, "CREATOR_PERSON_ID")].ExtendedProperties["In"] = personTab;
                table.Columns[_db.langAttrName(Knowledge._VIEWNAME, "CREATOR_PERSON_ID")].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%" + _db.langAttrName(Knowledge._VIEWNAME, "CREATOR_PERSON_ID"), "mode","oe");
                
				//TODO: remove it from db
				//Who wrote that and why should I do that? cst, 080123
                if (table.Columns.Contains("TRIGGER_UID")) {
                    table.Columns["TRIGGER_UID"].ExtendedProperties["Visibility"] =  DBColumn.Visibility.INVISIBLE;
                }//if
  
				if(Global.Config.isModuleEnabled("suggestion")) {
					table.Columns["TYPE"].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.LIST + (int) DBColumn.Visibility.INFO;
                    table.Columns["TYPE"].ExtendedProperties["In"] = new System.Collections.ArrayList(_mapper.getEnum("KNOWLEDGE", "types", true));
					table.Columns["ISACTIVE"].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.LIST + (int) DBColumn.Visibility.INFO;
					table.Columns["ISACTIVE"].ExtendedProperties["In"] = new System.Collections.ArrayList(_mapper.getEnum("KNOWLEDGE", "isActiveSuggestion", true));
                }//if

                IDColumn = "ID";
                if (KnowledgeID > 0) {
                    HighlightRecordID = KnowledgeID;
                }//if

				CheckOrder = true;
                listTable.Rows.Clear();
                LoadList(_db, table, listTable);
            } catch (Exception ex) {
                DoOnException(ex);
            } finally {
                _db.disconnect();
            }//try
        }

        public string lookup(DataColumn col, object id, bool http) {
            string retValue = "";

            if (col != null && !(id is System.DBNull)) {
                switch (col.ColumnName) {
                    case "TRIGGER_UID":
                        retValue = _db.UID2NiceName((long)id,_mapper);
                        break;
                }
            }
            return retValue;
        }

        /// <summary>
        /// Event handler for the 'next' button
        /// The selected item(s) database ID will be stored in the SEARCHRESULT table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void next_Click(object sender, System.EventArgs e) {
            long searchResultID = SaveInSearchResult(listTable, Knowledge._TABLENAME);

            NextURL = NextURL.Replace("%25SearchResultID","%SearchResultID").Replace("%SearchResultID", searchResultID.ToString());

            _nextArgs.LoadUrl = NextURL;
            DoOnNextClick(next);
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) 
        {
			//Nothing to do here			
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
