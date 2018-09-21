using ch.appl.psoft.Common;
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
    /// Anzeigen der OE's für den Aufruf von SalaryAdjustment.aspx.
    /// Verwendung in Main.aspx und  SalaryAdjustment.aspx
    /// Wird im ersten Schritt (in Main.aspx) nur angezeigt, wenn es mehrere sind.
    /// Erst innerhalb von SalaryAdjustment.aspx wird die Liste auch mit nur einer OE
    /// angezeigt.
    /// </summary>
    public partial class ApprovementControl : PSOFTListViewUserControl {
        public const string PARAM_ORGENTITY_ID = "PARAM_ORGENTITY_ID";
        public const string PARAM_SALARY_COMPONENT = "PARAM_SALARY_COMPONENT";
        public const string PARAM_ACTION = "PARAM_ACTION";
        protected string _onloadString; 

        private long _komponenteId;
        private long _budgetId;
        private ArrayList _editingStates;
        private ArrayList _genehmigenValues;
        private ArrayList _resetValues;
        private DBData _db = null;
        private DataTable _listTable = null;
        private string _listQuery = "";
        private string _titleAttribute = "";

        public static string Path {
            get {return Global.Config.baseURL + "/Lohn/Controls/ApprovementControl.ascx";}
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

        public string Action {
            get {return GetString(PARAM_ACTION);}
            set {SetParam(PARAM_ACTION, value);}
        }
		#endregion
        
        protected void Page_Load(object sender, System.EventArgs e) {
            if (this.Visible) {
                Execute();
            }
        }

        protected override void DoExecute() {
            base.DoExecute();
            _editingStates = new ArrayList(_mapper.getEnum("lohn", "editingState", true));
            _genehmigenValues = new ArrayList(_mapper.getEnum("lohn", "genehmigenValue", true));
            _resetValues = new ArrayList(_mapper.getEnum("lohn", "resetValue", true));
            _budgetId = -1;
            _onloadString = "";

            this.listTab.Rows.Clear();
            this.oeList.Rows.Clear();

            if (!IsPostBack) {
                approveAll.Text = _mapper.get("lohn","approveAll");
                cancel.Value = _mapper.get("cancel");
                approve.Text = _mapper.get("lohn","approve");
                closeApprove.Text = _mapper.get("lohn","closeApprove");
            }
            _db = DBData.getDBData(Session);
            _db.connect();
            _db.beginTransaction();
            try {
                listQuery();
                if (Action != "") {
                    doAction();
                }
                loadList();
                _db.commit();
            }
            catch (Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        /// <summary>
        /// Setzt Budgetstati gemäss Property Action
        /// Action (Format: SSVVMIIIIII) enthält folgende Informationen:
        ///  SS: BS für Bearbeitungsstatus, FS für Freigabestatus
        ///  VV: Statuswert (Value) zB 01
        ///  M:  A: bei allen Budget der gleichen OE und gleichen Komponente, sonst nur ein Budget
        ///  II: Budget-Id (variable Länge) =0: alle Budget der gleichen Komponente
        /// </summary>
        private void doAction() {
            if (Action.Length > 5) {
                object [] values;
                long orgentityId = -1;
                long budgettypId = -1;
                string stateType = Action.Substring(0, 2);
                int state = Validate.GetValid(Action.Substring(2, 2), -1);
                string modus = Action.Substring(4, 1);
                _budgetId = Validate.GetValid(Action.Substring(5), (long)-1);

                if (state != -1 && _budgetId != -1) {
                    switch (stateType) {
                    case "BS":
                        if (Validate.IsDefinedInEnum(state, new LohnModule.EditingState())) {
                            values = _db.lookup(new string[] {"ORGENTITY_ID","BUDGETTYP_ID"}, "BUDGET", "ID = " + _budgetId);
                            orgentityId = DBColumn.GetValid(values[0],0L);
                            budgettypId = DBColumn.GetValid(values[1],0L);
                            if (state == (int)LohnModule.EditingState.Approved) {
                                if (orgentityId == 0) loadList();
                                loadOEList(orgentityId,budgettypId);
                                if (oeList.Rows.Count > 0) {
                                    oeList.Rows[0].Cells[0].Text = _budgetId.ToString();
                                    this.oeContainer.Style.Remove("DISPLAY");
                                }
                            }

                            if (oeList.Rows.Count == 0) {
                                if (modus == "A") {
                                    values = _db.lookup(
                                        new string [] {"ORGENTITY_ID", "KOMPONENTE_ID"},
                                        "BUDGET",
                                        "ID = " + _budgetId
                                        );
                                    _db.execute(
                                        "update BUDGET set BEARBEITUNGSSTATUS = " + state
                                        + " where ORGENTITY_ID = " + DBColumn.GetValid(values[0],0L)
                                        + " and KOMPONENTE_ID = " + DBColumn.GetValid(values[1],0L)
                                        );
                                }
                                else {
                                    _db.execute(
                                        "update BUDGET set BEARBEITUNGSSTATUS = " + state
                                        + " where ID = " + _budgetId
                                        );
                                }
                            
                                if (orgentityId != -1) {
                                    Transfer transfer = LohnModule.getNewTransfer(LohnModule.KundenModuleName,_db);
                                    transfer.storeAll(
                                        LohnModule.DefaultDBName,
                                        SalaryComponent,
                                        orgentityId
                                        );
                                }
                            }
                        }

                        break;
                    case "FS":
                        if (Validate.IsDefinedInEnum(state, new LohnModule.ClearingState())) {
                            if (modus == "A") {
                                values = _db.lookup(
                                    new string [] {"ORGENTITY_ID", "KOMPONENTE_ID"},
                                    "BUDGET",
                                    "ID = " + _budgetId
                                    );
                                _db.execute(
                                    "update BUDGET set FREIGABESTATUS = " + state
                                    + " where ORGENTITY_ID = " + values[0]
                                    + " and KOMPONENTE_ID = " + values[1]
                                    );
                            }
                            else {
                                _db.execute(
                                    "update BUDGET set FREIGABESTATUS = " + state
                                    + " where ID = " + _budgetId
                                    );
                            }
                        }

                        break;
                    }
                }
            }
        }

        private void loadOEList(long orgentityId,long budgettypId) {
            string sql = "";
            DataTable table = null;

            if (orgentityId > 0) {
                sql = _listQuery.Replace("1=1","O.PARENT_ID = "+orgentityId.ToString());
                if (budgettypId > 0) sql = sql.Replace("3=3","B.BUDGETTYP_ID = "+budgettypId);
                table = _db.getDataTableExt(sql, "DLA_ORGENTITY_V");
            }
            else {
                table = _listTable;
            }

            TableRow r = null;
            TableCell c = null;
            long oeId = 0;
            foreach (DataRow row in table.Rows) {
                oeId = DBColumn.GetValid(row["ID"],0L);
                int state = DBColumn.GetValid(row["BEARBEITUNGSSTATUS"],0);

                if (state != (int) LohnModule.EditingState.Approved) {
                    if (oeList.Rows.Count == 0) {
                        r = new TableRow();
                        r.CssClass = "ListHeader";
                        oeList.Rows.Add(r);

                        c = new TableHeaderCell();
                        c.Visible = false;
                        r.Cells.Add(c);

                        c = new TableHeaderCell();
                        c.CssClass = "ListHeader";
                        r.Cells.Add(c);
                        c.Text = _mapper.get("DLA_ORGENTITY_V",_titleAttribute);

                        c = new TableHeaderCell();
                        c.CssClass = "ListHeader";
                        r.Cells.Add(c);
                        c.Text = _mapper.get("DLA_ORGENTITY_V","BUDGETTYP");

                        c = new TableHeaderCell();
                        c.CssClass = "ListHeader";
                        r.Cells.Add(c);
                        c.Text = _mapper.get("DLA_ORGENTITY_V","VERFUEGBAR");

                        c = new TableHeaderCell();
                        c.CssClass = "ListHeader";
                        r.Cells.Add(c);
                        c.Text = _mapper.get("DLA_ORGENTITY_V","AUFWAND");

                        c = new TableHeaderCell();
                        c.CssClass = "ListHeader";
                        r.Cells.Add(c);
                        c.Text = _mapper.get("DLA_ORGENTITY_V","DIFFERENZ");

                        c = new TableHeaderCell();
                        c.CssClass = "ListHeader";
                        r.Cells.Add(c);
                        c.Text = _mapper.get("DLA_ORGENTITY_V","PNAME");

                        c = new TableHeaderCell();
                        c.CssClass = "ListHeader";
                        r.Cells.Add(c);
                        c.Text = _mapper.get("DLA_ORGENTITY_V","BEARBEITUNGSSTATUS");

                    }
                    r = new TableRow();
                    oeList.Rows.Add(r);
                    r.CssClass = oeList.Rows.Count % 2 == 1 ? "ListEven" : "ListOdd";

                    c = new TableCell();
                    c.Visible = false;
                    r.Cells.Add(c);
                    c.Text = row["BUDGET_ID"].ToString();

                    c = new TableCell();
                    r.Cells.Add(c);
                    c.Text = DBColumn.GetValid(row[_titleAttribute],"");

                    c = new TableCell();
                    r.Cells.Add(c);
                    c.Text = DBColumn.GetValid(row["BUDGETTYP"],"");

                    c = new TableCell();
                    r.Cells.Add(c);
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Text = DBColumn.GetValid(row["VERFUEGBAR"],0.0).ToString("#,##0.00");

                    c = new TableCell();
                    r.Cells.Add(c);
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Text = DBColumn.GetValid(row["AUFWAND"],0.0).ToString("##,##0.00");

                    c = new TableCell();
                    r.Cells.Add(c);
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Text = DBColumn.GetValid(row["DIFFERENZ"],0.0).ToString("#,##0.00");

                    c = new TableCell();
                    r.Cells.Add(c);
                    c.Text = DBColumn.GetValid(row["PNAME"],"");

                    c = new TableCell();
                    r.Cells.Add(c);
                    addAmpel(state,c);

                }
                loadOEList(oeId,DBColumn.GetValid(row["BUDGETTYP_ID"],0L));
            }
        }


        private void listQuery() {
            _komponenteId = DBColumn.GetValid(
                _db.lookup(
                "K.ID",
                "KOMPONENTE K inner join VARIANTE V on K.VARIANTE_ID = V.ID",
                "V.HAUPTVARIANTE = 1 and K.BEZEICHNUNG = '" + SalaryComponent + "'"
                ),
                (long)-1
                );

            if (_komponenteId == -1) {
                throw new Exception("Komponente not found");
            }

            _titleAttribute = _db.langAttrName("ORGENTITY", "TITLE");
            _listQuery = "select O.ID,"
                + " B.ID BUDGET_ID,"
                + " O." + _titleAttribute + ","
                + " B.BUDGETTYP,B.BUDGETTYP_ID,"
                + " B.BETRAG VERFUEGBAR,"
                + " isnull(sum(L.BETRAG), 0) AUFWAND,"
                + " B.BETRAG - isnull(sum(L.BETRAG), 0) DIFFERENZ,"
                + " B.BEARBEITUNGSSTATUS,"
                + " 'X' GENEHMIGEN,"
                + " 'X' RESET,"
                + " P.ID PID,P.PNAME+' '+P.FIRSTNAME PNAME"
                + " from (LOHNKOMPONENTE L right join "
                +   "(BUDGET_V B inner join ORGENTITY O on"
                +   " 1=1"
                +   " and B.ORGENTITY_ID = O.ID"
                +   " and B.KOMPONENTE_ID = " + _komponenteId
                +   ") on L.BUDGET_ID = B.ID and L.BETRAG is not null"
                + ") left join OEPERSONV P on O.ID = P.OE_ID and P.JOB_TYP = 1"
                + " where 3=3";

            _listQuery += " group by O.ID,"
                + "B.ID,"
                + "O." + _titleAttribute + ","
                + "B.BUDGETTYP,B.BUDGETTYP_ID,B.BETRAG,B.BEARBEITUNGSSTATUS,"
                + "P.ID,P.PNAME,P.FIRSTNAME";

            if (OrderColumn == "") {
                _listQuery += " order by O." + _titleAttribute + ", B.BUDGETTYP";
            }
            else {
                _listQuery += " order by " + OrderColumn + " " + OrderDir;
            }
        }
        /// <summary>
        /// Konstruiert den Select 
        /// Erstellt die Liste auf Grund des Selects und der extended Attribute von
        /// DLA_ORGENTITY_V
        /// </summary>
        private void loadList() {
            if (_listTable != null) return;
            try {
                long rootId = _db.Orgentity.getRootID(OrgentityId);
                string sql = _listQuery.Replace("1=1","(O.PARENT_ID = "+OrgentityId.ToString()+" and 1=1)");

                if (OrgentityId == rootId) sql = sql.Replace("and 1=1","or O.ID = "+OrgentityId.ToString());
                _listTable = _db.getDataTableExt(sql, "DLA_ORGENTITY_V");

                DataTable budgettypTable = _db.getDataTable(
                    "select ID, BEZEICHNUNG from BUDGETTYP order by BEZEICHNUNG"
                    );
                _listTable.Columns["GENEHMIGEN"].ExtendedProperties["Visibility"]
                    = DBColumn.Visibility.LIST;
                _listTable.Columns["RESET"].ExtendedProperties["Visibility"]
                    = DBColumn.Visibility.LIST;

                DetailEnabled = true;
                IDColumn = "BUDGET_ID";
                DetailURL = Approvement.GetURL(
                    "orgId", "%ID",
                    "salaryComponent", SalaryComponent
                    );
                DeleteEnabled = false;
                EditEnabled = true;
                EditURL = SalaryAdjustment.GetURL(
                    "orgId", "%ID",
                    "salaryComponent", SalaryComponent
                    );
                InfoBoxEnabled = false;
                SortURL = Approvement.GetURL(
                    "orgId", OrgentityId,
                    "salaryComponent", SalaryComponent
                    );
                RowsPerPage = (int)(SessionData.getRowsPerListPage(Session) * 1.7);

                LoadList(_db, _listTable, listTab);

                if (_budgetId != -1) {
                    showBudgetOnLoad();
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
        }

        /// <summary>
        /// Sorgt für die Anzeige der richtigen Seite, falls sich die Liste über
        /// mehrere Seiten verteilt ist
        /// </summary>
        /// <param name="budgetID"></param>
        private void showBudgetOnLoad() {
            if (listTab.Rows.Count > RowsPerPage) {
                int actualPage = 0;

                foreach (TableRow row in listTab.Rows) {
                    if (ListBuilder.getID(row) == _budgetId.ToString()) {
                        actualPage = ListBuilder.getPage(row);
                        break;
                    }
                }

                _onloadString = "showActualPage("
                    + listTab.ClientID + ","
                    + actualPage + ","
                    + RowsPerPage
                    + ",true);";
            }
        }

        protected override void onAddHeaderCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (col != null) {
                switch (col.ColumnName) {
                case "GENEHMIGEN":
                    cell.Text = _mapper.get("lohn", "genehmigenTitle");
                    break;
                case "RESET":
                    cell.Text = _mapper.get("lohn", "resetTitle");
                    break;
                }
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
                int bearbeitungsstatus;
                int newBearbeitungsstatus;
                string action;
                System.Web.UI.WebControls.HyperLink link;

                switch (col.ColumnName) {
                case "VERFUEGBAR":
                case "AUFWAND":
                case "DIFFERENZ":
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    break;
                case "BEARBEITUNGSSTATUS":
                    bearbeitungsstatus 
                        = DBColumn.GetValid(row[col], (int)LohnModule.EditingState.Pending);
                    addAmpel(bearbeitungsstatus,cell);
                    break;
                case "GENEHMIGEN":
                    link = new System.Web.UI.WebControls.HyperLink();
                    bearbeitungsstatus = DBColumn.GetValid(
                        row["BEARBEITUNGSSTATUS"],
                        (int)LohnModule.EditingState.Pending
                        );

                    switch (bearbeitungsstatus) {
                    case (int)LohnModule.EditingState.Pending:
                        cell.Text = _genehmigenValues[0].ToString();
                        break;
                    case (int)LohnModule.EditingState.Done:
                        link.Text = _genehmigenValues[1].ToString();
                        newBearbeitungsstatus = (int)LohnModule.EditingState.Approved;
                        action = "BS"
                            + newBearbeitungsstatus.ToString("00")
                            + "E" // nur ein Budget
                            + DBColumn.GetValid(row["BUDGET_ID"], (long)-1);
                        link.NavigateUrl = Approvement.GetURL(
                            "orgId", OrgentityId,
                            "salaryComponent", SalaryComponent,
                            "action", action 
                            );
                        cell.Controls.Clear();
                        cell.Controls.Add(link);
                        break;
                    case (int)LohnModule.EditingState.Approved:
                        link.Text = _genehmigenValues[2].ToString();
                        newBearbeitungsstatus = (int)LohnModule.EditingState.Done;
                        action = "BS"
                            + newBearbeitungsstatus.ToString("00")
                            + "E" // nur ein Budget
                            + DBColumn.GetValid(row["BUDGET_ID"], (long)-1);
                        link.NavigateUrl = Approvement.GetURL(
                            "orgId", OrgentityId,
                            "salaryComponent", SalaryComponent,
                            "action", action 
                            );
                        cell.Controls.Clear();
                        cell.Controls.Add(link);
                        break;
                    default:
                        break;
                    }

                    break;
                case "RESET":
                    link = new System.Web.UI.WebControls.HyperLink();
                    bearbeitungsstatus = DBColumn.GetValid(
                        row["BEARBEITUNGSSTATUS"],
                        (int)LohnModule.EditingState.Pending
                        );

                    switch (bearbeitungsstatus) {
                    case (int)LohnModule.EditingState.Pending:
                        cell.Text = _resetValues[0].ToString();
                        break;
                    case (int)LohnModule.EditingState.Done:
                    case (int)LohnModule.EditingState.Approved:
                        link.Text = _resetValues[1].ToString();
                        newBearbeitungsstatus = (int)LohnModule.EditingState.Pending;
                        action = "BS"
                            + newBearbeitungsstatus.ToString("00")
                            + "A" // blockweise
                            + DBColumn.GetValid(row["BUDGET_ID"], (long)-1);
                        link.NavigateUrl = Approvement.GetURL(
                            "orgId", OrgentityId,
                            "salaryComponent", SalaryComponent,
                            "action", action 
                            );
                        cell.Controls.Clear();
                        cell.Controls.Add(link);
                        break;
                    default:
                        break;
                    }

                    break;
                }
            }
        }

        private void addAmpel(int bearbeitungsstatus, TableCell cell) {
            System.Web.UI.WebControls.Image image
                = new System.Web.UI.WebControls.Image();

            switch (bearbeitungsstatus) {
            case (int)LohnModule.EditingState.Pending:
                image.ImageUrl = "../../images/ampelRot.gif";
                image.ToolTip = _editingStates[0].ToString();
                break;
            case (int)LohnModule.EditingState.Done:
                image.ImageUrl = "../../images/ampelOrange.gif";
                image.ToolTip = _editingStates[1].ToString();
                break;
            case (int)LohnModule.EditingState.Approved:
                image.ImageUrl = "../../images/ampelGruen.gif";
                image.ToolTip = _editingStates[2].ToString();
                break;
            default:
                image.ImageUrl = "";
                break;
            }

            if (image.ImageUrl != "") {
                cell.Controls.Clear();
                cell.Controls.Add(image);
                cell.HorizontalAlign = HorizontalAlign.Center;
            }
        }


        private void updateBudget(string states, int targetState) {
            Transfer transfer = LohnModule.getNewTransfer(LohnModule.KundenModuleName,_db);
            foreach (TableRow r in oeList.Rows) {
                string sql = "update BUDGET set BEARBEITUNGSSTATUS = " + targetState
                    + " where BEARBEITUNGSSTATUS in ("+states+") and ID = " + r.Cells[0].Text;
                if (_db.execute(sql) > 0) {
                    
                    transfer.storeAll(
                        LohnModule.DefaultDBName,
                        SalaryComponent,
                        _db.lookup("ORGENTITY_ID","BUDGET","ID="+r.Cells[0].Text,0L)
                        );
                        
                }
            }
        }

        private void doButtonAction(object sender, EventArgs arg) {
            string url = Approvement.GetURL("orgId",OrgentityId,"salaryComponent",SalaryComponent);

            _db.connect();
            _db.beginTransaction();
            try {
                if (sender == approve) {
                    updateBudget(((int)LohnModule.EditingState.Done).ToString(),
                        (int)LohnModule.EditingState.Approved);
                }
                else if (sender == closeApprove) {
                    updateBudget(((int)LohnModule.EditingState.Pending).ToString()
                        +","+((int)LohnModule.EditingState.Done).ToString(),
                        (int)LohnModule.EditingState.Approved);
                }
                _db.commit();
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
                url = "";
                _db.rollback();
                throw e;
            }
            finally {
                _db.disconnect();
            }
            if (url != "") Response.Redirect(url,false);
        }

        private void doApproveAll(object sender, EventArgs arg) {
            string action = "BS"
                + ((int)LohnModule.EditingState.Approved).ToString("00")
                + "A"
                + "0";
            string url = Approvement.GetURL(
                "orgId", OrgentityId,
                "salaryComponent", SalaryComponent,
                "action", action 
                );

            Response.Redirect(url);
        }

        private void mapControls() {
            closeApprove.Click += new System.EventHandler(this.doButtonAction);
            approve.Click += new System.EventHandler(this.doButtonAction);
            approveAll.Click += new System.EventHandler(this.doApproveAll);
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
