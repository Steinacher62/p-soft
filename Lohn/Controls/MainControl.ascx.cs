using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
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
    public partial class MainControl : PSOFTListViewUserControl {
        public const string PARAM_PARENT_ID = "PARAM_PARENT_ID";
        public const string PARAM_CONTEXT = "PARAM_CONTEXT";
        public const string PARAM_SALARY_COMPONENT = "PARAM_SALARY_COMPONENT";
        public const string PARAM_BUDGETTYP_ID = "PARAM_BUDGETTYP_ID";
        public const string PARAM_ROW_COUNT = "PARAM_ROW_COUNT";
        public const string WITH_BUDGET_ONLY = "WITH_BUDGET_ONLY";


        private ArrayList _editingStates;
        protected ArrayList _clearingStates;

        public static string Path {
            get {return Global.Config.baseURL + "/Lohn/Controls/MainControl.ascx";}
        }

        #region Properities
        public long ParentId {
            get {return GetLong(PARAM_PARENT_ID);}
            set {SetParam(PARAM_PARENT_ID, value);}
        }

        public string Kontext {
            get {return GetString(PARAM_CONTEXT);}
            set {SetParam(PARAM_CONTEXT, value);}
        }

        public string SalaryComponent { // Komponenten-Bezeichnung
            get {return GetString(PARAM_SALARY_COMPONENT);}
            set {SetParam(PARAM_SALARY_COMPONENT, value);}
        }

        public long BudgettypId {
            get {return GetLong(PARAM_BUDGETTYP_ID);}
            set {SetParam(PARAM_BUDGETTYP_ID, value);}
        }

        public int RowCount {
            get {return GetInt(PARAM_ROW_COUNT);}
            set {SetParam(PARAM_ROW_COUNT, value);}
        }
        public bool WithBudgetOnly {
            get {return GetBool(WITH_BUDGET_ONLY);}
            set {SetParam(WITH_BUDGET_ONLY, value);}
        }
		#endregion
        
        protected void Page_Load(object sender, System.EventArgs e) {
            if (this.Visible) {
                Execute();
            }
        }

        protected override void DoExecute() {
            base.DoExecute();
            loadList();
        }

        /// <summary>
        /// Konstruiert den Select 
        /// Erstellt die Liste auf Grund des Selects und der extended Attribute von
        /// DLA_ORGENTITY_V
        /// </summary>
        private void loadList() {
            _editingStates = new ArrayList(_mapper.getEnum("lohn", "editingState", true));
            _clearingStates = new ArrayList(_mapper.getEnum("lohn", "clearingState", true));
            long komponenteId = -1;
            DBData db = DBData.getDBData(Session);
            DataTable table;
            db.connect();

            try {
                komponenteId = DBColumn.GetValid(
                    db.lookup(
                    "K.ID",
                    "KOMPONENTE K, VARIANTE V",
                    "V.HAUPTVARIANTE = 1"
                    + " and K.VARIANTE_ID = V.ID"
                    + " and K.BEZEICHNUNG = '" + SalaryComponent + "'"
                    ),
                    (long)-1
                    );
                OrderColumn = "O." + db.langAttrName("ORGENTITY", "TITLE");
                string sql = "select O.ID,"
                    + " O." + db.langAttrName("ORGENTITY", "TITLE") + ","
                    + " min(F." + db.langAttrName("FUNKTION", "TITLE") + ") FUNKTION_TITLE,"
                    + " isnull(min(B.BEARBEITUNGSSTATUS), 0) BEARBEITUNGSSTATUS,"
                    + " isnull(min(B.FREIGABESTATUS), 0) FREIGABESTATUS"
                    + " from "
                    + " (FUNKTION F right join JOB J on F.ID = J.FUNKTION_ID)"
                    + " right join (BUDGET B right join "
                    +               "ORGENTITY O on B.ORGENTITY_ID = O.ID and B.KOMPONENTE_ID = " + komponenteId
                    +              ")"
                    + " on J.ORGENTITY_ID = O.ID and J.TYP = 1";

                if (ParentId > 0) sql += " where O.PARENT_ID  = " + ParentId;
                else sql += ",EMPLOYMENT E where J.EMPLOYMENT_ID = E.ID and E.PERSON_ID  = " + SessionData.getUserID(Session);
                if (BudgettypId > 0) sql += " and B.BUDGETTYP_ID = " + BudgettypId;
                if (WithBudgetOnly) {
                    sql += " and (case isnull(O.HAT_BUDGET,0) when 0 then DBO.HAT_SUBBUDGET(O.ID) else 1 end) <> 0";
                }

                // Bei Budgetverteilung müssen OEs Unter-OEs haben
                if (Kontext == "budget") {
                    sql += " and exists (select 1 from ORGENTITY SO where SO.PARENT_ID = O.ID)";
                }

                sql += " group by O.ID,"
                    + " O." + db.langAttrName("ORGENTITY", "TITLE")
                    + " order by " + OrderColumn + " " + OrderDir;

                table = db.getDataTableExt(sql, "DLA_ORGENTITY_V");
                RowCount = table.Rows.Count;

                if (RowCount == 1 && ParentId < 0) { // bei Einstieg vom Menü
                    string orgId = DBColumn.GetValid(table.Rows[0]["ID"], "");
                    string URL = "";

                    switch (Kontext) {
                    case "budget":
                        URL = BudgetAllocation.GetURL(
                            "orgId", orgId,
                            "salaryComponent", SalaryComponent,
                            "budgettypId", BudgettypId
                            );
                        break;
                    case "approvement":
                        URL = Approvement.GetURL(
                            "orgId", orgId,
                            "salaryComponent", SalaryComponent
                            );
                        break;
                    case "budgetcheck":
                        URL = BudgetCheck.GetURL(
                            "orgId", orgId,
                            "salaryComponent", SalaryComponent,
                            "budgettypId", BudgettypId
                            );
                        break;
                    default:
                        URL = SalaryAdjustment.GetURL(
                            "orgId", orgId,
                            "salaryComponent", SalaryComponent,
                            "context", Kontext,
                            "budgettypId", BudgettypId
                            );
                        break;
                    }

                    Response.Redirect(URL, false);
                }
                else {
                    DetailEnabled = true;
                    double factor = 1;

                    switch (Kontext) {
                    case "budget":
                        DetailURL = BudgetAllocation.GetURL(
                            "orgId", "%ID",
                            "salaryComponent", SalaryComponent,
                            "budgettypId", BudgettypId
                            );
                        break;
                    case "approvement":
                        DetailURL = Approvement.GetURL(
                            "orgId", "%ID",
                            "salaryComponent", SalaryComponent
                            );
                        break;
                    case "budgetcheck":
                        DetailURL = BudgetCheck.GetURL(
                            "orgId", "%ID",
                            "salaryComponent", SalaryComponent,
                            "budgettypId", BudgettypId
                            );
                        break;
                    case "adjustment":
                        factor = 0.35;
                        goto default;
                    default:
                        DetailURL = SalaryAdjustment.GetURL(
                            "orgId", "%ID",
                            "salaryComponent", SalaryComponent,
                            "context", Kontext,
                            "budgettypId", BudgettypId
                            );
                        break;
                    }

                    DeleteEnabled = false;
                    EditEnabled = false;
                    InfoBoxEnabled = false;
                    SortURL = "";
                    RowsPerPage = (int)(SessionData.getRowsPerListPage(Session) * factor);

                    LoadList(db, table, listTab);
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
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
                switch (col.ColumnName) {
                case "BEARBEITUNGSSTATUS":
                    if (cell.Text != "") {
                        System.Web.UI.WebControls.Image im
                            = new System.Web.UI.WebControls.Image();

                        switch(cell.Text) {
                        case "0":
                            im.ImageUrl = "../../images/ampelRot.gif";
                            im.ToolTip = _editingStates[0].ToString();
                            break;
                        case "1":
                            im.ImageUrl = "../../images/ampelOrange.gif";
                            im.ToolTip = _editingStates[1].ToString();
                            break;
                        case "2":
                            im.ImageUrl = "../../images/ampelGruen.gif";
                            im.ToolTip = _editingStates[2].ToString();
                            break;
                        }

                        cell.Text = "";
                        cell.Controls.Add(im);
                        cell.HorizontalAlign = HorizontalAlign.Center;
                    }

                    break;
                case "FREIGABESTATUS":
                    if (cell.Text != "") {
                        System.Web.UI.WebControls.Image im
                            = new System.Web.UI.WebControls.Image();

                        switch(cell.Text) {
                        case "0":
                            im.ImageUrl = "../../images/ampelRot.gif";
                            im.ToolTip = _clearingStates[0].ToString();
                            break;
                        case "1":
                            im.ImageUrl = "../../images/ampelOrange.gif";
                            im.ToolTip = _clearingStates[1].ToString();
                            break;
                        case "2":
                            im.ImageUrl = "../../images/ampelGruen.gif";
                            im.ToolTip = _clearingStates[2].ToString();
                            break;
                        }

                        cell.Text = "";
                        cell.Controls.Add(im);
                        cell.HorizontalAlign = HorizontalAlign.Center;
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
