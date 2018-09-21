namespace ch.appl.psoft.Document.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class ClipboardList : PSOFTSearchListUserControl {
        public const string MODE_SEARCHRESULT = "searchresult";

        public const string CONTEXT_FOLDER = "folder";
        public const string CONTEXT_SEARCHRESULT = "searchresult";

        public const string PARAM_SQL = "PARAM_SQL";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_RELOAD = "PARAM_RELOAD";
        public const string PARAM_DOCUMENT_ID = "PARAM_DOCUMENT_ID";
        public const string PARAM_X_ID = "PARAM_X_ID";
        public const string PARAM_MODE = "PARAM_MODE";
        public const string PARAM_KONTEXT = "PARAM_KONTEXT";
        public const string PARAM_CONTENTS = "PARAM_CONTENTS";

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        protected DBData _db = null;
        private string _postDeleteURL;

        public static string Path {
            get {return Global.Config.baseURL + "/Document/Controls/ClipboardList.ascx";}
        }

        public ClipboardList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = true;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            CheckOrder = true;
            Mode = "";
        }

		#region Properities
        public long DocumentID {
            get {return GetLong(PARAM_DOCUMENT_ID);}
            set {SetParam(PARAM_DOCUMENT_ID, value);}
        }

        public long xID {
            get {return GetLong(PARAM_X_ID);}
            set {SetParam(PARAM_X_ID, value);}
        }

        public string Mode {
            get {return GetString(PARAM_MODE);}
            set {SetParam(PARAM_MODE, value);}
        }

        public string NextURL {
            get { return GetString(PARAM_NEXT_URL); }
            set {SetParam(PARAM_NEXT_URL, value);}
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

        public string PostDeleteURL {
            get {return _postDeleteURL;}
            set {_postDeleteURL = value;}
        }

        public string Contents {
            get {return GetString(PARAM_CONTENTS);}
            set {SetParam(PARAM_CONTENTS, value);}
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
                string sql = "select * from CLIPBOARD ";

                switch (Mode){
                    case MODE_SEARCHRESULT:
                        sql = SearchSQL;
                        load = Reload;
                        pageTitle.Text = _mapper.get("clipboard", "searchResult");
                        next.Text = _mapper.get("next");
                        next.Attributes.Add("onclick", "if (top.showProgressBarDelayed) top.showProgressBarDelayed(1500);");
                        ButtonRow.Visible = true;
                        CheckBoxEnabled = true;
                        break;
                        
                    default:
						/*
                        switch (Kontext){
                            case CONTEXT_FOLDER:
                                sql += " where FOLDER_ID=" + xID;
                                pageTitle.Text = _mapper.get("document", "contextFolder").Replace("#1", _db.lookup("TITLE", "FOLDER", "ID=" + xID, false));
                                break;

                            case CONTEXT_SEARCHRESULT:
                                sql += " where ID in (select ROW_ID from SEARCHRESULT where ID=" + xID + ")";
                                pageTitle.Text = _mapper.get("document", "selection");
                                break;
                        }*/
                        break;
                }

                if (load){
					sql += " order by " + OrderColumn + " " + OrderDir;
                    DataTable table = _db.getDataTableExt(sql, "CLIPBOARD", "FOLDER"); 
															                    
					IDColumn = "ID";
                    if (DocumentID > 0)
                        HighlightRecordID = DocumentID;

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

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (ListBuilder.IsInfoCell(cell)) {     
           
                ListBuilder.ReplaceInfoImage(cell,"folder.gif");
            }
            if (col != null){
                switch(col.ColumnName) {
                    case "FOLDER_ID":
                        DokAblageDetail.createPathLinks(_db, cell, ch.psoft.Util.Validate.GetValid(cell.Text, -1L));
                        break;
                }
            }
        }

        /// <summary>
        /// Event handler for the 'next' button
        /// The selected item(s) database ID will be stored in the SEARCHRESULT table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void next_Click(object sender, System.EventArgs e) {
            long searchResultID = SaveInSearchResult(listTab, "CLIPBOARD");

            NextURL = NextURL.Replace("%25SearchResultID","%SearchResultID").Replace("%SearchResultID", searchResultID.ToString());

            _nextArgs.LoadUrl = NextURL;
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
