using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Lohn.Controls
{
    /// <summary>
    ///	Budgetvergleich unten links in SalaryAdjustment für context = ""
    /// </summary>
    public partial class BudgetListControl : PSOFTListViewUserControl {
        public const string PARAM_ORGENTITY_ID = "PARAM_ORGENTITY_ID";
        public const string PARAM_SALARY_COMPONENT = "PARAM_SALARY_COMPONENT";
        public const string PARAM_BUDGETTYP_ID = "PARAM_BUDGETTYP_ID";


        private DBData _db;
        private long _komponenteId;
        private long _varianteId;
        private string _titleColumn;
        private BudgetDetailControl _detailControl;
        private Hashtable _salaryComponentTable = LohnModule.SalaryComponentTable;
        private ArrayList _clearingStates;

        public static string Path {
            get {return Global.Config.baseURL + "/Lohn/Controls/BudgetListControl.ascx";}
        }

        #region Properities
        public long OrgentityId {
            get {return GetLong(PARAM_ORGENTITY_ID);}
            set {SetParam(PARAM_ORGENTITY_ID, value);}
        }

        public string SalaryComponent { // Komponenten-Bezeichnung
            get {return GetString(PARAM_SALARY_COMPONENT);}
            set {SetParam(PARAM_SALARY_COMPONENT, value);}
        }

        public long BudgettypId {
            get {return GetLong(PARAM_BUDGETTYP_ID);}
            set {SetParam(PARAM_BUDGETTYP_ID, value);}
        }

        public BudgetDetailControl DetailControl {
            get {return _detailControl;}
            set {_detailControl = value;}
        }

		#endregion
        
        protected void Page_Load(object sender, System.EventArgs e) {
            if (this.Visible) {
                Execute();
            }
        }

        protected override void DoExecute() {
            base.DoExecute();
            _clearingStates = new ArrayList(_mapper.getEnum("lohn", "clearingState", true));
            _db = DBData.getDBData(Session);
            _db.connect();

            try {
                object [] values = _db.lookup(
                    new string [] {"K.ID", "K.VARIANTE_ID"},
                    "KOMPONENTE K inner join VARIANTE V on K.VARIANTE_ID = V.ID",
                    "V.HAUPTVARIANTE = 1 and K.BEZEICHNUNG = '" + SalaryComponent + "'"
                    );
                _komponenteId = DBColumn.GetValid(values[0], (long)-1);
                _varianteId = DBColumn.GetValid(values[1], (long)-1);

                if (_komponenteId == -1) {
                    throw new Exception("Komponente not found");
                }

                if (!IsPostBack) {
                    pageTitle.Text = _mapper.get("lohn", "subOE");
                    apply.Text = _mapper.get("save");
                    clear.Text = _mapper.get("lohn", "clear");
                    clearAllocationAll.Text = _mapper.get("lohn", "clearAllocationAll");
                    clearAll.Text = _mapper.get("lohn", "clearAll");

                    // Save- und Freigabe-Button verfügbar?
                    values = _db.lookup(
                        new string [] {"O.PARENT_ID", "B.FREIGABESTATUS"},
                        "ORGENTITY O, BUDGET B",
                        "B.ID = " + _detailControl.BudgetId
                        + " and O.ID = B.ORGENTITY_ID"
                        );
                    int freigabestatus = DBColumn.GetValid(
                        values[1],
                        (int)LohnModule.ClearingState.Disabled
                        );
                    clear.Enabled = (
                        freigabestatus == (int)LohnModule.ClearingState.allocationEnabled
                        || (DBColumn.IsNull(values[0])
                        && freigabestatus == (int)LohnModule.ClearingState.Disabled
                        )
                        );
                    apply.Enabled = (freigabestatus != (int)LohnModule.ClearingState.Enabled);
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }

            loadList();
        }

        private void loadList() {
            DataTable table;
            _db = DBData.getDBData(Session);
            _db.connect();

            try {
                _titleColumn = _db.langAttrName("ORGENTITY", "TITLE");
                OrderColumn = "O." + _titleColumn;
                string sql = "select O.ID, B.ID BUDGET_ID,"
                    + " O." + _titleColumn + ","
                    + " cast(null as float) ABWEICHUNG,"
                    + " isnull(B.FREIGABESTATUS, " + (int)LohnModule.ClearingState.Disabled + ") FREIGABESTATUS,"
                    + " B.BETRAG_VERTEILBAR VERTEILBAR"
                    + " from BUDGET B right join ORGENTITY O"
                    + " on O.ID = B.ORGENTITY_ID"
                    + "  and B.KOMPONENTE_ID = " + _komponenteId
                    + (BudgettypId == -1 ? "" : (" and B.BUDGETTYP_ID = " + BudgettypId))
                    + " where O.PARENT_ID = " + OrgentityId
                    + " order by " + OrderColumn + " " + OrderDir;
                table = _db.getDataTableExt(sql, "DLA_ORGENTITY_V");

                DetailURL = BudgetAllocation.GetURL(
                    "orgId", "%ID",
                    "salaryComponent", SalaryComponent,
                    "budgettypId", BudgettypId
                    );
                DetailEnabled = true;
                DeleteEnabled = false;
                EditEnabled = false;
                InfoBoxEnabled = false;
                SortURL = BudgetAllocation.GetURL(
                    "orgId", OrgentityId,
                    "salaryComponent", SalaryComponent,
                    "budgettypId", BudgettypId
                    );
                RowsPerPage = SessionData.getRowsPerListPage(Session);

                listTab.Rows.Clear();
                LoadList(_db, table, listTab);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        /// <summary>
        /// Spezialbehandlung der Datenzellen
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="r"></param>
        /// <param name="cell"></param>
        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (col != null) {
                long subOrgentityId = DBColumn.GetValid(row["ID"], (long)-1);
                long budgetId;
                long orgentityId;

                switch (col.ColumnName) {
                case "ABWEICHUNG":
                    if (subOrgentityId != -1 && _salaryComponentTable.Contains(SalaryComponent)) {
                        cell.Text = _db.GetDisplayValue(
                            col,
                            LohnModule.GetPositiveAbweichung(
                            subOrgentityId,
                            _varianteId,
                            BudgettypId,
                            true
                            ),
                            true
                            );
                    }

                    cell.HorizontalAlign = HorizontalAlign.Right;
                    break;
                case "FREIGABESTATUS":
                    System.Web.UI.WebControls.Image image
                        = new System.Web.UI.WebControls.Image();
                    int freigabestatus 
                        = DBColumn.GetValid(row[col], (int)LohnModule.ClearingState.Disabled);

                    cell.HorizontalAlign = HorizontalAlign.Center;
                    switch (freigabestatus) {
                    case (int)LohnModule.ClearingState.Disabled:
                        image.ImageUrl = "../../images/ampelRot.gif";
                        image.ToolTip = _clearingStates[0].ToString();
                        break;
                    case (int)LohnModule.ClearingState.allocationEnabled:
                        image.ImageUrl = "../../images/ampelOrange.gif";
                        image.ToolTip = _clearingStates[1].ToString();
                        break;
                    case (int)LohnModule.ClearingState.Enabled:
                        image.ImageUrl = "../../images/ampelGruen.gif";
                        image.ToolTip = _clearingStates[2].ToString();
                        break;
                    default:
                        image.ImageUrl = "";
                        break;
                    }

                    if (image.ImageUrl != "") {
                        cell.Controls.Clear();
                        cell.Controls.Add(image);
                    }

                    break;
                case "VERTEILBAR":
                    if (apply.Enabled) {
                        budgetId = DBColumn.GetValid(row["BUDGET_ID"], (long)-1);
                        orgentityId = DBColumn.GetValid(row["ID"], (long)-1);
                        TwoValuesTextBox box = new TwoValuesTextBox();
                        box.OrigText = cell.Text;
                        box.LongText = row[col].ToString() == ""
                            ? ""
                            : ((double)row[col]).ToString(_db.dbColumn.UserCulture);
                        box.ID = "VERTEILBAR_"
                            + (budgetId == -1 ? "N" + orgentityId : "" + budgetId);
                        cell.Controls.Add(box);
                    }
                    else {
                        cell.HorizontalAlign = HorizontalAlign.Right;
                    }

                    break;
                default:
                    // link nur, falls Unter-OEs vorhanden
                    if (col.ColumnName == _titleColumn) {
                        long subSubOrgentityId = DBColumn.GetValid(
                            _db.lookup(
                            "ID",
                            "ORGENTITY",
                            "PARENT_ID = " + subOrgentityId
                            ),
                            (long)-1
                            );
                            
                        if (subSubOrgentityId == -1) {
                            System.Web.UI.Control control = cell.Controls[0];

                            if (control is HyperLink) {
                                ((HyperLink)control).Enabled = false;
                            }
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Speichert die Eingaben und setzt gegebenenfalls die Stati
        /// Bei OEs, welche keine Sub-OEs mit Zuständigkeit für Lohn haben,
        /// wird der Status direkt auf Freigegeben gesetzt und das verfügbare
        /// Budget eingefüllt
        /// </summary>
        /// <param name="setStates"></param>
        private void saveOrClear(bool setStates) {
            string sql;

            // Detail-Teil
            string [] inputList = _detailControl.InputList;
            long budgetId = _detailControl.BudgetId;

            if (budgetId != -1 && (inputList[0] != "" || inputList[1] != "")) {
                sql = "update BUDGET set";
                string setPart = "";

                if (inputList[0] != "") {
                    setPart = " BETRAG_VERTEILBAR = " + inputList[0];
                }

                if (inputList[1] != "") {
                    if (setPart != "") {
                        setPart += ",";
                    }

                    setPart += " BETRAG = " + inputList[1];
                }

                if (setStates) {
                    if (setPart != "") {
                        setPart += ",";
                    }

                    setPart += " FREIGABESTATUS = " + (int)LohnModule.ClearingState.Enabled;
                }

                sql += setPart + " where id = " + budgetId;
                _db.execute(sql);
            }

            // Listeneingaben
            TwoValuesTextBox box;
            double verteilbar = 0;
            string clearPart;
            DataTable table;
            long orgentityId;
            long id;
            bool setAllocationEnabled = false;

            foreach (TableRow row in listTab.Rows) {
                foreach (TableCell cell in row.Cells) {
                    foreach (WebControl control in cell.Controls) {
                        if (control is TwoValuesTextBox
                            && control.ID.StartsWith("VERTEILBAR_")
                            ) {
                            box = (TwoValuesTextBox)control;
                            verteilbar = Validate.GetValid(box.ValidText, (double)-1);

                            if (verteilbar >= 0) {
                                if (box.ID.Substring(11, 1) == "N") { // fehlendes Budget
                                    orgentityId = Validate.GetValid(box.ID.Substring(12), (long)-1);
                                    budgetId = _db.newId("BUDGET");
                                    _db.execute(
                                        "insert into BUDGET ("
                                        + " ID, ORGENTITY_ID, KOMPONENTE_ID, BUDGETTYP_ID"
                                        + ") values ("
                                        + budgetId + ","
                                        + orgentityId + ","
                                        + _komponenteId + ","
                                        + (BudgettypId == -1 ? "null" : ("" + BudgettypId))
                                        + ")"
                                        );
                                    _db.execute(
                                        "update LOHNKOMPONENTE set BUDGET_ID = " + budgetId
                                        + " where LOHN_ID in ("
                                        + "  select L.ID"
                                        + "  from LOHN L, EMPLOYMENT E"
                                        + "  where E.ORGENTITY_ID = " + orgentityId
                                        + (BudgettypId == -1
                                        ? ""
                                        : (" and E.BUDGETTYP_ID = " + BudgettypId)
                                        )
                                        + "   and L.EMPLOYMENT_ID = E.ID"
                                        + ") and KOMPONENTE_ID = " + _komponenteId
                                        );
                                }
                                else {
                                    budgetId = Validate.GetValid(box.ID.Substring(11), (long)-1);
                                }

                                if (setStates) {
                                    clearPart = ", freigabestatus = ";
                                    sql = "select O.ID"
                                        + " from BUDGET B, ORGENTITY O"
                                        + " where O.PARENT_ID = B.ORGENTITY_ID"
                                        + " and B.ID = " + box.ID.Substring(11);
                                    table = _db.getDataTable(sql);
                                    setAllocationEnabled = false;

                                    foreach (DataRow subOERow in table.Rows) {
                                        id = DBColumn.GetValid(subOERow[0], (long)-1);

                                        if (LohnModule.GetNumberOfBerechtigte(id, _komponenteId) > 0) {
                                            setAllocationEnabled = true;
                                            break;
                                        }
                                    }
                                        
                                    if (setAllocationEnabled) {
                                        clearPart += (int)LohnModule.ClearingState.allocationEnabled;
                                    }
                                    else {
                                        clearPart += (int)LohnModule.ClearingState.Enabled
                                            + ", betrag = " + verteilbar;
                                    }
                                }
                                else {
                                    clearPart = "";
                                }

                                sql = "update BUDGET set BETRAG_VERTEILBAR = " + verteilbar
                                    + clearPart
                                    + " where ID = " + budgetId;
                                _db.execute(sql);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Klicken auf Speichern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void apply_Click(object sender, System.EventArgs e) {
            _db = DBData.getDBData(Session);
            _db.connect();

            try {
                _db.beginTransaction();
                saveOrClear(false);
                _db.commit();
                Response.Redirect(Request.RawUrl, false);
            }
            catch(Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        /// <summary>
        /// Klicken auf Freigeben
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void clear_Click(object sender, System.EventArgs e) {
            _db = DBData.getDBData(Session);
            _db.connect();

            try {
                _db.beginTransaction();
                saveOrClear(true);
                _db.commit();
                Response.Redirect(Request.RawUrl, false);
            }
            catch(Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected void clearAll_Click(object sender, System.EventArgs e) {
            _db = DBData.getDBData(Session);
            _db.connect();

            try {
                _db.beginTransaction();
                saveOrClear(true);
                doClearAll(OrgentityId,LohnModule.ClearingState.Enabled);
                _db.commit();
                Response.Redirect(Request.RawUrl, false);
            }
            catch(Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected void clearAllocationAll_Click(object sender, System.EventArgs e) {
            _db = DBData.getDBData(Session);
            _db.connect();

            try {
                _db.beginTransaction();
                saveOrClear(true);
                doClearAll(OrgentityId,LohnModule.ClearingState.allocationEnabled);
                _db.commit();
                Response.Redirect(Request.RawUrl, false);
            }
            catch(Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        private bool doClearAll(long parentId,LohnModule.ClearingState state) {
            string sql = "select O.ID OE_ID, B.ID BUDGET_ID, B.FREIGABESTATUS"
                + " from BUDGET B right join ORGENTITY O"
                + " on O.ID = B.ORGENTITY_ID"
                + "  and B.KOMPONENTE_ID = " + _komponenteId
                + (BudgettypId == -1 ? "" : (" and B.BUDGETTYP_ID = " + BudgettypId))
                + " where O.PARENT_ID = " + parentId;
            DataTable table = _db.getDataTable(sql);

            foreach (DataRow row in table.Rows) {
                bool leaf = doClearAll(DBColumn.GetValid(row["OE_ID"],0L),state);
                int s = leaf ? (int) LohnModule.ClearingState.Enabled : (int) state;

                if (!DBColumn.IsNull(row["BUDGET_ID"])) {
                    if (DBColumn.GetValid(row["FREIGABESTATUS"],0) < s) {
                        sql = "update budget set freigabestatus = "+s+" where id = "+DBColumn.GetValid(row["BUDGET_ID"],0L);
                        _db.execute(sql);
                    }
                }
            }
            return table.Rows.Count == 0;
        }

        private void mapControls() {
            apply.Click += new System.EventHandler(apply_Click);
            clear.Click += new System.EventHandler(clear_Click);
            clearAll.Click += new System.EventHandler(clearAll_Click);
            clearAllocationAll.Click += new System.EventHandler(clearAllocationAll_Click);
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
