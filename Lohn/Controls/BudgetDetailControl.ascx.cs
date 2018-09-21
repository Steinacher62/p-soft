using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Lohn.Controls
{
    /// <summary>
    ///		Summary description for PendenzenQuelleDetail.
    /// </summary>
    public partial class BudgetDetailControl : PSOFTInputViewUserControl
    {
        public const string PARAM_ORGENTITY_ID = "PARAM_ORGENTITY_ID";
        public const string PARAM_SALARY_COMPONENT = "PARAM_SALARY_COMPONENT";
        public const string PARAM_BUDGETTYP_ID = "PARAM_BUDGETTYP_ID";

        private DBData _db = null;
        private DataTable _table;
        private long _varianteId;
        private double _verteilt;
        private ArrayList _clearingStates;

        protected string _onloadString = "";

        public static string Path
        {
            get {return Global.Config.baseURL + "/Lohn/Controls/BudgetDetailControl.ascx";}
        }

		#region Properities
        public long OrgentityId 
        {
            get {return GetLong(PARAM_ORGENTITY_ID);}
            set {SetParam(PARAM_ORGENTITY_ID, value);}
        }

        public string SalaryComponent // Komponenten-Bezeichnung
        {
            get {return GetString(PARAM_SALARY_COMPONENT);}
            set {SetParam(PARAM_SALARY_COMPONENT, value);}
        }

        public long BudgettypId 
        {
            get {return GetLong(PARAM_BUDGETTYP_ID);}
            set {SetParam(PARAM_BUDGETTYP_ID, value);}
        }

        /// <summary>
        /// Eingabewerte für apply in BudgetListControl ("" bedeutete keine Eingabe):
        /// 0: VERTEILBAR
        /// 1: VERFUEGBAR
        /// </summary>
        public string [] InputList
        {
            get
            {
                string [] returnValue = new string [] {"", ""};

                foreach (TableRow row in editTab.Rows)
                {
                    foreach (TableCell cell in row.Cells)
                    {
                        foreach (WebControl control in cell.Controls)
                        {
                            if (control is TwoValuesTextBox)
                            {
                                if (control.ID.EndsWith("-VERTEILBAR"))
                                {
                                    returnValue[0] = ((TwoValuesTextBox)control).ValidText == ""
                                        ? "null"
                                        : ((TwoValuesTextBox)control).ValidText;
                                }
                            }

                            if (control is TwoValuesTextBox)
                            {
                                if (control.ID.EndsWith("-VERFUEGBAR"))
                                {
                                    returnValue[1] = ((TwoValuesTextBox)control).ValidText == ""
                                        ? "null"
                                        : ((TwoValuesTextBox)control).ValidText;
                                }
                            }
                        }
                    }
                }
                return returnValue;
            }
        }

        /// <summary>
        /// Für apply in BudgetListControl
        /// </summary>
        public long BudgetId
        {
            get {return _table.Rows.Count > 0 ? DBColumn.GetValid(_table.Rows[0]["ID"], -1L) : -1L;}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute ();
            _clearingStates = new ArrayList(_mapper.getEnum("lohn", "clearingState", true));
            _db = DBData.getDBData(Session);
            _db.connect();

            try
            {
                object [] values = _db.lookup(
                            new string [] {"K.ID", "K.VARIANTE_ID"},
                            "KOMPONENTE K inner join VARIANTE V on K.VARIANTE_ID = V.ID",
                            "V.HAUPTVARIANTE = 1 and K.BEZEICHNUNG = '" + SalaryComponent + "'"
                        );
                long komponenteId = DBColumn.GetValid(values[0], (long)-1);
                _varianteId = DBColumn.GetValid(values[1], (long)-1);
 
                if (komponenteId == -1)
                {
                    throw new Exception("Komponente not found");
                }

                _verteilt = LohnModule.GetBudgetVerteilt(OrgentityId, komponenteId, BudgettypId);
                string titleAttribute = _db.langAttrName("ORGENTITY", "TITLE");
                values = _db.lookup(
                        new string [] {titleAttribute, "PARENT_ID"},
                        "ORGENTITY",
                        "ID = " + OrgentityId
                    );
                bool isRootOE = DBColumn.IsNull(values[1]);

                // Hat Root ein Budget? Gegebenenfalls erzeugen!
                if (isRootOE
                    && (LohnModule.BudgetModusConfig == (int)LohnModule.BudgetModus.budgetOhneImport
                        || LohnModule.BudgetModusConfig == (int)LohnModule.BudgetModus.budgetAusBedarf
                    )
                )
                {
                    long budgetId = DBColumn.GetValid(
                            _db.lookup(
                                "ID",
                                "BUDGET",
                                "KOMPONENTE_ID = " + komponenteId
                                    + " and B.ORGENTITY_ID = " + OrgentityId
                                    + (BudgettypId == -1
                                            ? ""
                                            : (" and B.BUDGETTYP_ID = " + BudgettypId)
                                        )
                            ),
                            (long)-1
                        );

                    if (budgetId == -1)
                    {
                        try
                        {
                            _db.beginTransaction();
                            budgetId = _db.newId("BUDGET");
                            _db.execute(
                                "insert into BUDGET ("
                                    + " ID, ORGENTITY_ID, KOMPONENTE_ID, BUDGETTYP_ID"
                                    + ") values ("
                                    + budgetId + ","
                                    + OrgentityId + ","
                                    + komponenteId + ","
                                    + (BudgettypId == -1 ? "null" : ("" + BudgettypId))
                                    + ")"
                            );
                            _db.execute(
                                "update LOHNKOMPONENTE set BUDGET_ID = " + budgetId
                                    + " where LOHN_ID in ("
                                    + "  select L.ID"
                                    + "  from LOHN L, EMPLOYMENT E"
                                    + "  where E.ORGENTITY_ID = " + OrgentityId
                                    + (BudgettypId == -1
                                            ? ""
                                            : (" and E.BUDGETTYP_ID = " + BudgettypId)
                                        )
                                    + "   and L.EMPLOYMENT_ID = E.ID"
                                    + ") and KOMPONENTE_ID = " + komponenteId
                            );
                            _db.commit();
                        }
                        catch
                        {
                            _db.rollback();
                        }
                    }
                }

                string oeTitle = DBColumn.GetValid(values[0], "");
                pageTitle.Text = oeTitle;

                editTab.Rows.Clear();
                string sql = "select B.ID,"
                    + " B.BETRAG_VERTEILBAR VERTEILBAR,"
                    + " B.BETRAG VERFUEGBAR,"
                    + " B.FREIGABESTATUS,"
                    + " cast(null as float) ABWEICHUNG,"
                    + " cast(null as float) VERTEILT,"
                    + " cast(null as float) DIFFERENZ"
                    + " from BUDGET B"
                    + " where B.KOMPONENTE_ID = " + komponenteId
                    + " and B.ORGENTITY_ID = " + OrgentityId
                    + (BudgettypId == -1 ? "" : (" and B.BUDGETTYP_ID = " + BudgettypId));
                _table = _db.getDataTableExt(sql, "DLA_ORGENTITY_V");

                int freigabestatus = _table.Rows.Count > 0 ? 
                    DBColumn.GetValid(_table.Rows[0]["FREIGABESTATUS"],(int)LohnModule.ClearingState.Disabled)
                    : (int)LohnModule.ClearingState.Disabled;

                if (!isRootOE || freigabestatus == (int)LohnModule.ClearingState.Enabled) 
                {
                    _table.Columns["VERTEILBAR"].ExtendedProperties["InputControlType"] = typeof(Label);
                }

                if (freigabestatus == (int)LohnModule.ClearingState.Enabled)
                {
                    _table.Columns["VERFUEGBAR"].ExtendedProperties["InputControlType"] = typeof(Label);
                }

                _table.Columns["ABWEICHUNG"].ExtendedProperties["InputControlType"] = typeof(Label);
                _table.Columns["VERTEILT"].ExtendedProperties["InputControlType"] = typeof(Label);
                _table.Columns["DIFFERENZ"].ExtendedProperties["InputControlType"] = typeof(Label);

                InputType = InputMaskBuilder.InputType.Edit;
                LoadInput(_db, _table, editTab);
            }
            catch (Exception ex)
            {
                DoOnException(ex);
            }
            finally
            {
                _db.disconnect();
            }
        }
        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r)
        {
            if (col != null)
            {
                System.Web.UI.Control control;
                TableCell cell;
                System.Web.UI.WebControls.Image image;

                switch (col.ColumnName)
                {
                    case "FREIGABESTATUS":
                        cell = r.Cells[1];
                        image = new System.Web.UI.WebControls.Image();
                        int freigabestatus 
                            = DBColumn.GetValid(row[col], (int)LohnModule.EditingState.Pending);

                        switch (freigabestatus)
                        {
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

                        if (image.ImageUrl != "")
                        {
                            cell.Controls.Clear();
                            cell.Controls.Add(image);
                        }

                        break;
                    case "ABWEICHUNG":
                        if (LohnModule.SalaryComponentTable.Contains(SalaryComponent))
                        {
                            control = r.Cells[1].Controls[0];

                            if (control is Label)
                            {
                                double abweichung = LohnModule.GetPositiveAbweichung(
                                            OrgentityId,
                                            _varianteId,
                                            BudgettypId,
                                            false
                                        );
                                ((Label)control).Text = _db.GetDisplayValue(col, abweichung, true);
                            }
                        }

                        break;
                    case "VERTEILT":
                        control = r.Cells[1].Controls[0];

                        if (control is Label)
                        {
                            ((Label)control).Text = _db.GetDisplayValue(col, _verteilt, true);
                        }

                        break;
                    case "DIFFERENZ":
                        control = r.Cells[1].Controls[0];

                        if (control is Label)
                        {
                            double differenz = DBColumn.GetValid(row["VERTEILBAR"], (double)0)
                                - _verteilt;
                            ((Label)control).Text = _db.GetDisplayValue(col, differenz, true);

                            if (differenz < 0)
                            {
                                ((Label)control).ForeColor = Color.Red;
                            }
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
		
        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
		#endregion
    }
}
