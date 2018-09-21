namespace ch.appl.psoft.Common.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    public partial  class UIDAssignmentsListCtrl : PSOFTSearchListUserControl {
        protected DBData _db = null;


        public static string Path {
            get {return Global.Config.baseURL + "/Common/Controls/UIDAssignmentsListCtrl.ascx";}
        }

        #region Properities
        public const string PARAM_FROM_UID = "PARAM_FROM_UID";
        public long FromUID {
            get {return GetLong(PARAM_FROM_UID);}
            set {SetParam(PARAM_FROM_UID, value);}
        }
        #endregion
    
        public UIDAssignmentsListCtrl() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
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
                if (_db.hasUIDAuthorisation(DBData.AUTHORISATION.READ, FromUID, DBData.APPLICATION_RIGHT.COMMON, true, true)){
                    pageTitle.Text = _mapper.get("uidAssignment", "ptList");
                    string sql = "select * from UID_ASSIGNMENT_V where FROM_UID=" + FromUID + " and (OWNER_PERSON_ID=" + _db.userId + " or OWNER_PERSON_ID is null)";
                    sql += " order by " + OrderColumn + " " + OrderDir;

                    DataTable table = _db.getDataTableExt(sql, "UID_ASSIGNMENT_V");
                    table.Columns["FROM_TABLENAME"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INFO;
                    table.Columns["FROM_TABLENAME"].ExtendedProperties["In"] = this;
                    table.Columns["TO_TABLENAME"].ExtendedProperties["In"] = this;
                    table.Columns[_db.langAttrName("UID_ASSIGNMENT_V", "FROM_NICENAME")].ExtendedProperties["Visibility"] = DBColumn.Visibility.INFO;
                    table.Columns["TYP"].ExtendedProperties["In"] = _mapper.getEnum("uidAssignment", "typesShort");
                    table.Columns[_db.langAttrName("UID_ASSIGNMENT_V", "TO_NICENAME")].ExtendedProperties["ContextLink"] = psoft.Goto.GetURL("uid","%TO_UID");
                    IDColumn = "ID";

                    listTable.Rows.Clear();
                    LoadList(_db, table, listTable);
                }
                else{
                    Response.Redirect(psoft.NotFound.GetURL(), false);
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
        /// Die Rechte fürs Löschen/Editieren sind eine UND-Kombination der Rechte auf der UID_STRUCTURE und dem Objekt am Verknüpfungsstart (FROM_).
        /// Das Leserecht ist abhängig vom Leserecht auf dem Objekt auf dem Verknüpfungsende (TO_). Das Leserecht auf dem Verknüpfungsstart wird bereits beim Laden der Seite geprüft.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="row"></param>
        /// <param name="isRowAccessPermitted"></param>
        /// <param name="requestedAuthorisation"></param>
        /// <returns></returns>
        protected override bool onRowAccess(DataTable table, DataRow row, bool isRowAccessPermitted, int requestedAuthorisation) {
            bool retValue = true;
            if (requestedAuthorisation == DBData.AUTHORISATION.READ){
                long toID = DBColumn.GetValid(row["TO_ROW_ID"], -1L);
                string toTablename = DBColumn.GetValid(row["TO_TABLENAME"], "");
                retValue = _db.hasRowAuthorisation(requestedAuthorisation, toTablename, toID, true, true);
            }
            else{
                long ownerPersonID = DBColumn.GetValid(row["OWNER_PERSON_ID"], -1L);
                if (ownerPersonID != _db.userId){
                    long UIDStructureID = DBColumn.GetValid(row["UID_STRUCTURE_ID"], -1L);
                    if (UIDStructureID > 0){
                        retValue = _db.hasRowAuthorisation(requestedAuthorisation, "UID_STRUCTURE", UIDStructureID, true, true);
                    }
                    if (retValue){
                        long fromID = DBColumn.GetValid(row["FROM_ROW_ID"], -1L);
                        string fromTablename = DBColumn.GetValid(row["FROM_TABLENAME"], "");
                        retValue = _db.hasRowAuthorisation(requestedAuthorisation, fromTablename, fromID, true, true);
                    }
                }
            }
            return retValue;
        }

        public string lookup(DataColumn col, object id, bool http) {
            string retValue = "";

            if (col != null && !(id is System.DBNull)) {
                switch (col.ColumnName) {
                    case "TO_TABLENAME":
                    case "FROM_TABLENAME":
                        string tablename = id.ToString();
                        retValue = _mapper.get("tableName", tablename);
                        break;
                }
            }
            return retValue;
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (ListBuilder.IsInfoCell(cell)) {
                string toTablename = DBColumn.GetValid(row["TO_TABLENAME"], "");

                switch (toTablename) {
                    case "DOCUMENT":
                        ListBuilder.ReplaceInfoImage(cell,"dokument.gif");
                        break;

                    case "PERSON":
                        ListBuilder.ReplaceInfoImage(cell,"stelle.gif");
                        break;

                    case "FIRM":
                        ListBuilder.ReplaceInfoImage(cell,"../Contact/images/kt_kontaktfirm.gif");
                        break;

                    case "TASKLIST":
                        ListBuilder.ReplaceInfoImage(cell,"pendenzliste.gif");
                        break;

                    case "MEASURE":
                        ListBuilder.ReplaceInfoImage(cell,"pendenz.gif");
                        break;

                    case "PROJECT":
                        ListBuilder.ReplaceInfoImage(cell,"projekt.gif");
                        break;

                    case "KNOWLEDGE":
                        ListBuilder.ReplaceInfoImage(cell,"../Contact/images/div_features.gif");
                        break;

                    default:
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
        protected void next_Click(object sender, System.EventArgs e) {
            long searchResultID = SaveInSearchResult(listTable, "UID_ASSIGNMENT");

//            NextURL = NextURL.Replace("%25SearchResultID","%SearchResultID").Replace("%SearchResultID", searchResultID.ToString());
//
//            _nextArgs.LoadUrl = NextURL;
            DoOnNextClick(next);
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
