using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;

namespace ch.appl.psoft.Lohn.Controls
{
    /// <summary>
    ///	Prompt fürs Abschliessen der Lohnmassnahme
    ///	Checkt, ob alle zugehörigen Massnahmen bearbeitet sind.
    ///	Falls ja, wird abgeschlossen (update BUDGET set BEARBEITUNGSSTATUS = 2 (genehmigt))
    ///	bei allen Budgets der OE zum aktuellen Jahr und Resultat in der OGSDatenbank
    ///	speichern.
    ///	Falls nicht -> Prompt ja/nein
    /// </summary>
    public partial class BudgetDoneCheckControl : PSOFTDetailViewUserControl {
        public const string PARAM_BACK_URL = "PARAM_BACK_URL";
        public const string PARAM_ORGENTITY_ID = "PARAM_ORGENTITY_ID";
        public const string PARAM_SALARY_COMPONENT = "PARAM_SALARY_COMPONENT";
        public const string PARAM_BUDGETTYP_ID = "PARAM_BUDGETTYP_ID";


        private string _komponenteIds = "";

        public static string Path {
            get {return Global.Config.baseURL + "/Lohn/Controls/BudgetDoneCheckControl.ascx";}
        }

        #region Properities
        public string BackURL {
            get {return GetString(PARAM_BACK_URL);}
            set {SetParam(PARAM_BACK_URL, value);}
        }

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

		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            if (!IsPostBack) {
                textLabel.Text = _mapper.get("lohn", "budgetConfirmation");
                yes.Text = _mapper.get("yes");
                no.Text = _mapper.get("no");
            }

            DBData db = DBData.getDBData(Session);
            db.connect();

            try {
                string sql = "select distinct K.ID from KOMPONENTE K inner join VARIANTE V on K.VARIANTE_ID = V.ID where V.HAUPTVARIANTE = 1 and K.BEZEICHNUNG in ('";
                sql += SalaryComponent.Replace(",","','");
                sql += "')";
                DataTable table = db.getDataTable(sql);

                foreach (DataRow r in table.Rows) {
                    if (_komponenteIds != "") _komponenteIds += ",";
                    _komponenteIds += r[0].ToString();
                }
                if (_komponenteIds == "") {
                    throw new Exception("Komponente not found");
                }

                sql = "select L.ID"
                    + " from BUDGET B, LOHNKOMPONENTE L"
                    + " where B.KOMPONENTE_ID in (" + _komponenteIds +")"
                    + " and B.ORGENTITY_ID = " + OrgentityId
                    + " and L.BUDGET_ID = B.ID"
                    + " and L.BEARBEITUNGSSTATUS = " + (int)LohnModule.EditingState.Pending
                    + (BudgettypId > 0 ? " and B.BUDGETTYP_ID = " + BudgettypId : "");
                table = db.getDataTable(sql);

                if (table.Rows.Count <= 0) {
                    // alle Lohnkomponenten sind bearbeitet
                    budgetDone();
                }
            }
            catch(Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        }

        /// <summary>
        /// Budgets abschliessen (genehmigt setzen und Daten in der OGS-Datenbank
        /// speichern)
        /// Anzeigen der Seite der Massnamen für die gleiche Komponente (in der Regel der
        /// gleiche wie vorher, aber erneuert)
        /// </summary>
        private void budgetDone() {
            DBData db = DBData.getDBData(Session);
            db.connect();

            try {
                int bearbeitungsstatus = (int)LohnModule.EditingState.Approved;

                if (LohnModule.MitGenehmigungsverfahren) {
                    bearbeitungsstatus = (int)LohnModule.EditingState.Done;
                }

                string sql = "update BUDGET"
                    + " set BEARBEITUNGSSTATUS = " + bearbeitungsstatus
                    + " where ORGENTITY_ID=" + OrgentityId
                    + " and KOMPONENTE_ID in (" + _komponenteIds + ")"
                    + (BudgettypId > 0 ? " and BUDGETTYP_ID = " + BudgettypId : "");
                db.execute(sql);

                if (bearbeitungsstatus == (int)LohnModule.EditingState.Approved) {
                    Transfer transfer = LohnModule.getNewTransfer(LohnModule.KundenModuleName);
                    transfer.storeAll(
                        LohnModule.DefaultDBName,
                        SalaryComponent,
                        OrgentityId
                        );
                }

                Response.Redirect(BackURL, false);
            }
            catch(Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        }

        /// <summary>
        /// Klicken von ja 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void yes_Click(object sender, System.EventArgs e) {
            budgetDone();
        }

        /// <summary>
        /// Klicken von nein
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void no_Click(object sender, System.EventArgs e) {
            Response.Redirect(BackURL, false);
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            yes.Click += new System.EventHandler(yes_Click);
            no.Click += new System.EventHandler(no_Click);
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
