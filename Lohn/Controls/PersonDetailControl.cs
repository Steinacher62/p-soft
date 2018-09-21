using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Collections;

namespace ch.appl.psoft.Lohn.Controls
{
    /// <summary>
    ///	Anzeige der Lohndaten und Eingabe der Massnahmen und Kommentare
    ///	Control für PersonDetail (History) und SalaryAdjustment (Lohnmassnahmen)
    /// </summary>
    public class PersonDetailControl : PSOFTDetailViewUserControl {
        public const string PARAM_LOHN_ID = "PARAM_LOHN_ID";
        public const string PARAM_BUDGETTYP_ID = "PARAM_BUDGETTYP_ID";
        public const string PARAM_READ_ONLY = "PARAM_READ_ONLY";
        public const string PARAM_ORGENTITY_ID = "PARAM_ORGENTITY_ID";
        public const string PARAM_SALARY_COMPONENT = "PARAM_SALARY_COMPONENT";
        public const string PARAM_CONTEXT = "PARAM_CONTEXT";

        protected System.Web.UI.WebControls.Table detailTable;
        protected System.Web.UI.WebControls.Label Title;
        protected System.Web.UI.WebControls.Button apply;
        protected System.Web.UI.WebControls.Button clear;
        protected System.Web.UI.WebControls.Button print;
        protected System.Web.UI.WebControls.Button previous;
        protected System.Web.UI.WebControls.Button history;
        protected System.Web.UI.WebControls.Button next;
        protected string _onloadString;

        protected long _komponenteId;
        protected long _varianteId;
        protected long _personId;
        protected long _employmentId;
        protected long _previousLohnId;
        protected long _nextLohnId;
        protected int _salaryYear;
        protected int _activeSalaryYear;
        protected double _salaryDivisor = 1;
        protected int _bearbeitungsstatus = -1;
        protected double _currentYearSalary = 0;
        protected double _proposedYearSalary = 0;
        protected ArrayList _salaryType;

        public static string Path {
            get {return "";}
        }

        public static string getPath(string kundenModuleName) {
            return Global.Config.baseURL
                + "/" + kundenModuleName.ToUpper()
                + "/Controls/"
                + kundenModuleName.ToUpper() + "PersonDetailControl.ascx";
        }

        #region Properities
        public long LohnId {
            get {return GetLong(PARAM_LOHN_ID);}
            set {SetParam(PARAM_LOHN_ID, value);}
        }

        public long BudgettypId {
            get {return GetLong(PARAM_BUDGETTYP_ID);}
            set {SetParam(PARAM_BUDGETTYP_ID, value);}
        }

        public bool ReadOnly {
            get {return GetBool(PARAM_READ_ONLY);}
            set {SetParam(PARAM_READ_ONLY, value);}
        }

        public long OrgentityId {
            get {return GetLong(PARAM_ORGENTITY_ID);}
            set {SetParam(PARAM_ORGENTITY_ID, value);}
        }

        public string SalaryComponent { // Komponenten-Bezeichnung
            get {return GetString(PARAM_SALARY_COMPONENT);}
            set {SetParam(PARAM_SALARY_COMPONENT, value);}
        }

        public string Kontext { // "history" oder ""
            get {return GetString(PARAM_CONTEXT);}
            set {SetParam(PARAM_CONTEXT, value);}
        }
		#endregion

        protected override void DoExecute() {
            base.DoExecute();

            _salaryType = new ArrayList(_mapper.getEnum("lohn", "salaryType", true));
            DBData db = DBData.getDBData(Session);
            db.connect();

            try {
                object [] values = db.lookup(
                    new string [] {"E.ID", "E.PERSON_ID", "L.VARIANTE_ID"},
                    "LOHN L inner join EMPLOYMENT E on L.EMPLOYMENT_ID = E.ID",
                    "L.ID = " + LohnId
                    );
                _employmentId = DBColumn.GetValid(values[0], (long)-1);
                _personId = DBColumn.GetValid(values[1], (long)-1);
                _varianteId = DBColumn.GetValid(values[2], (long)-1);
                values = db.lookup(
                    new string [] {"K.ID", "V.LR_GUELTIG_AB"},
                    "KOMPONENTE K inner join VARIANTE_LOHNRUNDE_V V on K.VARIANTE_ID = V.ID",
                    "V.ID = " + _varianteId + " and K.BEZEICHNUNG = '" + SalaryComponent + "'"
                    );
                _komponenteId = DBColumn.GetValid(values[0], (long)-1);
                _salaryYear = DBColumn.GetValid(values[1], DateTime.Now.AddYears(1)).Year;
                _activeSalaryYear = DBColumn.GetValid(db.lookup("V.LR_GUELTIG_AB",
                    "KOMPONENTE K inner join VARIANTE_LOHNRUNDE_V V on K.VARIANTE_ID = V.ID",
                    "V.HAUPTVARIANTE = 1 and K.ID = " + _komponenteId),
                    DateTime.Now.AddYears(1)).Year;

                if (_komponenteId == -1) {
                    throw new Exception("Komponente not found");
                }

                _previousLohnId = DBColumn.GetValid(
                    db.lookup(
                    "L2.ID",
                    "LOHN L1 inner join VARIANTE_LOHNRUNDE_V V1 on V1.ID = L1.VARIANTE_ID"
                    + " inner join VARIANTE_LOHNRUNDE_V V2 on 1 = 1"
                    + " inner join LOHN L2 on L2.VARIANTE_ID = V2.ID",
                    "L1.ID = " + LohnId
                    + " and L2.employment_id = " + _employmentId
                    + " and V2.LR_GUELTIG_AB < V1.LR_GUELTIG_AB"
                    + " order by V2.LR_GUELTIG_AB desc"
                    ),
                    (long)-1
                    );

                _nextLohnId = DBColumn.GetValid(
                    db.lookup(
                    "L2.ID",
                    "LOHN L1 inner join VARIANTE_LOHNRUNDE_V V1 on V1.ID = L1.VARIANTE_ID"
                    + " inner join VARIANTE_LOHNRUNDE_V V2 on 1 = 1"
                    + " inner join LOHN L2 on L2.VARIANTE_ID = V2.ID",
                    "L1.ID = " + LohnId
                    + " and L2.employment_id = " + _employmentId
                    + " and V2.LR_GUELTIG_AB > V1.LR_GUELTIG_AB"
                    + " order by V2.LR_GUELTIG_AB"
                    ),
                    (long)-1
                    );

                if (!IsPostBack) {
                    Title.Text = db.Person.getWholeName(_personId.ToString(), false);
                    apply.Text = _mapper.get("apply");
                    clear.Text = _mapper.get("lohn", "clear");
                    print.Text = _mapper.get("lohn", "print");
                    previous.Text = _mapper.get("lohn", "previous");
                    history.Text = _mapper.get("lohn", "history");
                    next.Text = _mapper.get("lohn", "next");
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        
            // print und history Button
            if (_previousLohnId == -1) {
                history.Enabled = false;
            }
            else {
                history.Attributes.Add(
                    "onclick",
                    "openPopupWindow('PersonDetail.aspx?readOnly=true&orgId=" + OrgentityId + "&lohnId=" + _previousLohnId + "&salaryComponent=" + SalaryComponent + "', 450, 700);"
                    );
            }

            string url = "../" + LohnModule.KundenModuleName.ToUpper() + "/CreatePersonDetailReport.aspx?lohnId=" + LohnId + "&salaryComponent=" + SalaryComponent;
            print.Attributes.Add(
                "onclick",
                "window.open('" + url + "');"
                );
        }

        protected double getPartSalary(double salary) {
            return salary / _salaryDivisor;
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
            this.apply.Click += new System.EventHandler(this.apply_Click);
            this.clear.Click += new System.EventHandler(this.clear_Click);
            this.previous.Click += new System.EventHandler(this.previous_Click);
            this.next.Click += new System.EventHandler(this.next_Click);
            this.print.Click += new System.EventHandler(this.refreshAfterClick);
            this.history.Click += new System.EventHandler(this.refreshAfterClick);
        }
		#endregion

        private void refreshAfterClick(object sender, System.EventArgs e) {
            Response.Redirect(Request.RawUrl, false);
        }

        /// <summary>
        /// Updaten der Massnahme in der Datenbank und neu Anzeigen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void apply_Click(object sender, System.EventArgs e) {
            Response.Redirect(Request.RawUrl, false);
        }

        /// <summary>
        /// Freigeben (Update in der Datenbank) und neu Anzeigen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void clear_Click(object sender, System.EventArgs e) {
            DBData db = DBData.getDBData(Session);
            db.connect();

            try {
                doClear(db,_komponenteId);
                Response.Redirect(Request.RawUrl, false);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        }

        protected void doClear(DBData db, long komponenteId) {
            int bearbeitungsstatus = (int)LohnModule.EditingState.Pending;
            long lohnkomponenteId = db.lookup(
                "ID",
                "LOHNKOMPONENTE",
                "LOHN_ID = " + LohnId + " and KOMPONENTE_ID = " + komponenteId,-1L);

            // bei fehlender Berechtigung eine Komponente zufügen
            if (lohnkomponenteId == -1) {
                long budgetId = db.lookup(
                    "B.ID",
                    "BUDGET B, EMPLOYMENT E, LOHN L",
                    "L.ID = " + LohnId
                    + " and E.ID = L.EMPLOYMENT_ID"
                    + " and B.ORGENTITY_ID = E.ORGENTITY_ID"
                    + " and B.KOMPONENTE_ID = " + komponenteId
                    + " and isnull(B.BUDGETTYP_ID, -1) = isnull(E.BUDGETTYP_ID, -1)",-1L);


                db.execute(
                    "insert into LOHNKOMPONENTE ("
                    + "LOHN_ID, KOMPONENTE_ID, BUDGET_ID, BETRAG, BEARBEITUNGSSTATUS"
                    + ") values ("
                    + LohnId + "," + komponenteId
                    + (budgetId == -1 ? ",null" : ","+budgetId)
                    + ",0," + bearbeitungsstatus
                    + ")"
                    );
            }
            else {
                db.execute(
                    "update LOHNKOMPONENTE set BEARBEITUNGSSTATUS=" + bearbeitungsstatus
                    + " where LOHN_ID = " + LohnId + " and KOMPONENTE_ID = " + komponenteId
                    + " and BEARBEITUNGSSTATUS <> " + bearbeitungsstatus
                    );
            }
        }

        /// <summary>
        /// Bei Anzeige der History: Anzeigen der Daten des vorherigen Jahres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void previous_Click(object sender, System.EventArgs e) {
            if (_previousLohnId != -1) {
                Response.Redirect(
                    PersonDetail.GetURL(
                    "orgId", OrgentityId,
                    "salaryComponent", SalaryComponent,
                    "lohnId", _previousLohnId
                    ),
                    false
                    );
            }
        }

        /// <summary>
        /// Bei Anzeige der History: Anzeigen der Daten des nächsten Jahres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void next_Click(object sender, System.EventArgs e) {
            if (_nextLohnId != -1) {
                Response.Redirect(
                    PersonDetail.GetURL(
                    "orgId", OrgentityId,
                    "salaryComponent", SalaryComponent,
                    "lohnId", _nextLohnId
                    ),
                    false
                    );
            }
        }
    }
}
