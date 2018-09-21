using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Lohn.Controls
{
    /// <summary>
    ///	Budgetvergleich unten links in SalaryAdjustment für context = ""
    /// </summary>
    public partial class BudgetControl : PSOFTListViewUserControl {
        public const string PARAM_BUDGETTYP_ID = "PARAM_BUDGETTYP_ID";
        public const string PARAM_ORGENTITY_ID = "PARAM_ORGENTITY_ID";
        public const string PARAM_SALARY_COMPONENT = "PARAM_SALARY_COMPONENT";


        string _kompSql = "";
        private int _freigabestatusMinimal = (int)LohnModule.ClearingState.Disabled;

        public static string Path {
            get {return Global.Config.baseURL + "/Lohn/Controls/BudgetControl.ascx";}
        }
        public static string getPath(string module) {
            if (Global.isModuleEnabled(module)) {
                return Global.Config.baseURL
                    + "/" + module.ToUpper()
                    + "/Controls/"
                    + module.ToUpper() + "BudgetControl.ascx";
            }
            else return Path;
        }

        #region Properities
        public long OrgentityId {
            get {return GetLong(PARAM_ORGENTITY_ID);}
            set {SetParam(PARAM_ORGENTITY_ID, value);}
        }

        public virtual string SalaryComponent { // Komponenten-Bezeichnung
            get {return GetString(PARAM_SALARY_COMPONENT);}
            set {SetParam(PARAM_SALARY_COMPONENT, value);}
        }

        public long BudgettypId {
            get {return GetLong(PARAM_BUDGETTYP_ID);}
            set {SetParam(PARAM_BUDGETTYP_ID, value);}
        }

		#endregion
        
        protected virtual void Page_Load(object sender, System.EventArgs e) {
            if (this.Visible) {
                Execute();
            }
        }

        protected override void DoExecute() {
            base.DoExecute();
            DBData db = DBData.getDBData(Session);
            db.connect();

            try {
                string kompDescr = DBColumn.toSql(SalaryComponent);
                kompDescr = kompDescr.Replace(",","','");
                _kompSql = "(KOMPONENTE K inner join VARIANTE V on K.VARIANTE_ID = V.ID"
                    +" and V.HAUPTVARIANTE = 1 and K.BEZEICHNUNG in ('" + kompDescr + "'))";

                object [] values = db.lookup(
                    new string [] {"min(B.BEARBEITUNGSSTATUS)", "min(B.FREIGABESTATUS)"},
                    "BUDGET B inner join "+_kompSql + " on B.KOMPONENTE_ID = K.ID",
                    "B.ORGENTITY_ID = " + OrgentityId
                    +(BudgettypId > 0 ? " and B.BUDGETTYP_ID = " + BudgettypId : "")
                    );
                _freigabestatusMinimal = DBColumn.GetValid(
                    values[1],
                    (int)LohnModule.ClearingState.Disabled
                    );

                if (!IsPostBack) {
                    pageTitle.Text = _mapper.get("lohn", "budgetComp");
                    done.Text = _mapper.get("lohn", "terminate");
                    int bearbeitungsstatusMinimal = DBColumn.GetValid(
                        values[0],
                        (int)LohnModule.EditingState.Pending
                        );
                    done.Enabled = (bearbeitungsstatusMinimal < (int)LohnModule.EditingState.Done);
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }

            loadList();
        }

        /// <summary>
        /// Konstruiert den Select 
        /// Erstellt die Liste auf Grund des Selects und der extended Attribute von
        /// BUDGET_V
        /// </summary>
        private void loadList() {
            DataTable table;
            DBData db = DBData.getDBData(Session);
            db.connect();

            try {
                string sql = "select K.BEZEICHNUNG KOMP_BEZEICHNUNG,"
                    + " B.ID,B.BUDGETTYP, B.BETRAG,"
                    + " isnull(sum(L.BETRAG), 0) AUFWAND,"
                    + " B.BETRAG - isnull(sum(L.BETRAG), 0) DIFFERENZ"
                    + " from (LOHNKOMPONENTE L right join BUDGET_V B"
                    + " on L.BUDGET_ID = B.ID and L.BETRAG is not null and L.KOMPONENTE_ID = B.KOMPONENTE_ID) inner join "
                    +_kompSql + " on B.KOMPONENTE_ID = K.ID"
                    + " where B.ORGENTITY_ID = " + OrgentityId
                    +(BudgettypId > 0 ? " and B.BUDGETTYP_ID = " + BudgettypId : "");

                if (_freigabestatusMinimal != (int)LohnModule.ClearingState.Enabled) {
                    sql += " and 0 = 1" // leerer select
                        + " group by K.BEZEICHNUNG, B.ID, B.BUDGETTYP, B.BETRAG";
                }
                else {
                    sql += " group by K.BEZEICHNUNG, B.ID, B.BUDGETTYP, B.BETRAG";

                    if (OrderColumn == "") {
                        sql += " order by K.BEZEICHNUNG desc,BUDGETTYP";
                    }
                    else if (OrderColumn.ToLower().IndexOf("budgettyp") == -1) {
                        sql += " order by " + OrderColumn + " " + OrderDir + ", BUDGETTYP";
                    }
                    else {
                        sql += " order by " + OrderColumn + " " + OrderDir;
                    }
                }

                table = db.getDataTableExt(sql, "BUDGET_V");

                DetailEnabled = false;
                DeleteEnabled = false;
                EditEnabled = false;
                InfoBoxEnabled = false;
                SortURL = "";
                RowsPerPage = SessionData.getRowsPerListPage(Session);
                
                // Budgettyp nur anzeigen, wenn mit mehreren Budgettypen gearbeitet wird
                int anzahlBudgettypen = DBColumn.GetValid(
                    db.lookup("count(*)", "BUDGETTYP", ""),
                    0
                    );

                if (anzahlBudgettypen < 2) {
                    table.Columns["BUDGETTYP"].ExtendedProperties["Visibility"]
                        = DBColumn.Visibility.INVISIBLE;
                }
                if (SalaryComponent.Split(',').Length > 1) {
                    table.Columns["KOMP_BEZEICHNUNG"].ExtendedProperties["Visibility"] = DBColumn.Visibility.LIST;
                }

                LoadList(db, table, listTab);
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
                case "DIFFERENZ":
                    double differenz = DBColumn.GetValid(row[col.ColumnName], (double)0);
                        
                    if (differenz < 0) {
                        cell.ForeColor = Color.Red;
                    }

                    break;
                default:
                    break;
                }
            }
        }

        /// <summary>
        /// Klicken auf Abschliessen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void done_Click(object sender, System.EventArgs e) {
            Response.Redirect(
                BudgetDoneCheck.GetURL(
                "salaryComponent", SalaryComponent,
                "orgId", OrgentityId,
                "backURL", Request.RawUrl,
                "budgettypId",BudgettypId
                ),
                false
                );
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
