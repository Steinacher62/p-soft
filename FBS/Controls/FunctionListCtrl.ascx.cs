namespace ch.appl.psoft.FBS.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for JobDescriptionCtrl.
    /// </summary>
    public partial class FunctionListCtrl : PSOFTSearchListUserControl {
        public const string PARAM_QUERY = "PARAM_QUERY";


        private long _id = 0;
        private DBData _db = null;
        private DataTable _table = null;
        private long _contextId = 0;
        private string _context = "";
        private const string JoinId = "(isnull(FUNCGROUPV.FID,0)*0x100000000+isnull(J.ID,0))";
        private const string Join = "FUNCGROUPV left join JOBEMPLOYMENTV J on FUNCGROUPV.FID = J.FUNKTION_ID";


        public static string Path {
            get {return Global.Config.baseURL + "/FBS/Controls/FunctionListCtrl.ascx";}
        }

		#region Properities
        /// <summary>
        /// Get/set context
        /// </summary>
        public string context {
            get {return _context;}
            set {_context = value;}
        }
        /// <summary>
        /// Get/Set context id
        /// </summary>
        public long contextId {
            get {return _contextId;}
            set {_contextId = value;}
        }
        /// <summary>
        /// Get/set current list element id
        /// </summary>
        public long id {
            get {return _id;}
            set {_id = value;}
        }
        /// <summary>
        /// Get/Set query for the list
        /// </summary>
        public string query {
            get {return GetString(PARAM_QUERY);}
            set {SetParam(PARAM_QUERY, value);}
        }

		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            if (!IsPostBack) {
            }
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();
            next.Text = _mapper.get("next");
            if (Visible) loadList();
        }
        public DataTable loadTable(int numOfRows) {
            string sql = "";
            DataTable table = null;
            string select = "select "+JoinId+" JOIN_ID,FUNCGROUPV.*,J.ID JOB_ID,J.ORGENTITY_ID,J.PERSON_ID from "+Join;

            _db = DBData.getDBData(Session);
            _db.connect();
            try {
                switch (_context) {
                case "SEARCH":
                    next.Visible = true;
                    CheckBoxEnabled = true;
                    if (query != "") {
                        int idx = query.IndexOf(" where");
                        sql = select + query.Substring(idx);
                    }
                    else sql = select;
                    break;

                case "SELECTION":
                    next.Visible = false;
                    if (_contextId > 0) sql = select + " where "+JoinId+" in (select ROW_ID from SEARCHRESULT where TABLENAME='FUNCGROUPV' and ID=" + _contextId + ")";
                    else if (query != "") {
                        int idx = query.IndexOf(" where");
                        sql = select + query.Substring(idx);
                    }
                    else sql = select;
                    break;

                default:
                    return table;
                }
                sql += " order by FUNCGROUPV."+_db.langAttrName("FUNCGROUPV","FTITLE");

                if (numOfRows > 0) sql = sql.Replace("select","select top "+numOfRows);
                table = _db.getDataTableExt(sql,"FUNCGROUPV"); 
            
            }
            finally {
                _db.disconnect();
            }
            return table;
        }

        private void loadList() {
            _db = DBData.getDBData(Session);
            DetailEnabled = true;
            DeleteEnabled = false;
            EditEnabled = false;
            InfoBoxEnabled = false;
            HeaderEnabled = true;
            base.DetailURL = "FunctionDetail.aspx?listId=%JOIN_ID&id=%FID&OEId=%ORGENTITY_ID&context="+_context+"&contextId=" + _contextId;

            _table = loadTable(0); 
            _table.Columns[_db.langAttrName("FUNCGROUPV","GTITLE")].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            _table.Columns[_db.langAttrName("FUNCGROUPV","FMNEMONIC")].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            _table.Columns["FDFLT"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            _table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.LIST;
            _table.Columns["PERSON_ID"].ExtendedProperties["ListControlType"] = typeof(HyperLink);
            if (_table.Columns.Contains("FUNKTION_TYP_ID")) _table.Columns["FUNKTION_TYP_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            if (_table.Columns.Contains("FBW_REVISION")) _table.Columns["FBW_REVISION"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            base.IDColumn = "JOIN_ID";
            if (_id > 0) HighlightRecordID = _id;
            listTab.Rows.Clear();
            base.LoadList(_db, _table, listTab);
        }
        protected override void onAddRow(DataRow row, TableRow r) {
        }
        protected override void onAddHeaderCell(DataRow row, DataColumn col, TableRow r, TableCell c) {
        }
        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell c) {
            if (col != null && col.ColumnName == "PERSON_ID") {
                long oeId = DBColumn.GetValid(row["ORGENTITY_ID"],0L);
                if (oeId > 0) {
                    HyperLink link = (HyperLink) c.Controls[0];
                    long persId = DBColumn.GetValid(row["PERSON_ID"],0L);
                    link.Text = persId > 0 ? _db.lookup(Person.getWholeNameSQL(false, true, false),"PERSON","ID="+persId,false) : "vakant";
                    link.NavigateUrl = Global.Config.baseURL + "/FBS/" + base.DetailURL.Replace("%25FID","%FID").Replace("%FID","0").Replace("%25JOIN_ID","%JOIN_ID").Replace("%JOIN_ID",row["JOIN_ID"].ToString()).Replace("%25ORGENTITY_ID","%ORGENTITY_ID").Replace("%ORGENTITY_ID","0");
                    if (!DBColumn.IsNull(row["JOB_ID"])) link.NavigateUrl += "&jobId="+row["JOB_ID"];
                }
            }
            else if (col != null && col.ColumnName == _db.langAttrName("FUNCGROUPV","FTITLE")) {
                HyperLink link = (HyperLink) c.Controls[0];
                Label lbl = null;
                long oeId = DBColumn.GetValid(row["ORGENTITY_ID"],0L);

                ArrayList path = _db.Tree("ORGENTITY").GetPath(oeId, true);
                lbl = new Label();
                lbl.Text = "";
                c.Controls.Clear();
                c.Controls.Add(lbl);
                foreach (long pathId in path) {
                    if (lbl.Text != "") lbl.Text += "/";
                    lbl.Text += _db.lookup("TITLE","ORGENTITY","ID="+pathId,true);
                }
                if (lbl.Text != "") lbl.Text += "/";
                c.Controls.Add(link);
            }
        }
        private void MapButtonMethods() {
            next.Click += new System.EventHandler(this.nextClick);
        }

        private void nextClick(object sender, System.EventArgs e) {
            long searchResultID = SaveInSearchResult(listTab, "FUNCGROUPV", Join, JoinId, 0L);

            _nextArgs.LoadUrl = "FunctionDetail.aspx?context=SELECTION&contextId=" + searchResultID;

            DoOnNextClick(next);
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            MapButtonMethods();
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
