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
    ///	Budgetkontrolle unten in BudgetCheck
    /// </summary>
    public partial class BudgetCheckListControl : PSOFTListViewUserControl
	{
        public const string PARAM_ORGENTITY_ID = "PARAM_ORGENTITY_ID";
        public const string PARAM_SALARY_COMPONENT = "PARAM_SALARY_COMPONENT";
        public const string PARAM_BUDGETTYP_ID = "PARAM_BUDGETTYP_ID";


        private DBData _db;
        private long _komponenteId;
        private long _varianteId;
        private string _titleColumn;
        private Hashtable _salaryComponentTable = LohnModule.SalaryComponentTable;
        private ArrayList _editingStates;
        private ArrayList _clearingStates;
        private double _differenzVerteilbar; // nur in onAddCell() verwendet!
        private double _differenz; // nur in onAddCell() verwendet!

        public static string Path
        {
            get {return Global.Config.baseURL + "/Lohn/Controls/BudgetCheckListControl.ascx";}
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

		#endregion
        
        protected void Page_Load(object sender, System.EventArgs e)
		{
            if (this.Visible) 
            {
                Execute();
            }
        }

        protected override void DoExecute() 
        {
            base.DoExecute();
            pageTitle.Text = _mapper.get("lohn", "subOE");
            _editingStates = new ArrayList(_mapper.getEnum("lohn", "editingState", true));
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
                _komponenteId = DBColumn.GetValid(values[0], (long)-1);
                _varianteId = DBColumn.GetValid(values[1], (long)-1);

                if (_komponenteId == -1)
                {
                    throw new Exception("Komponente not found");
                }
            }
            catch (Exception ex) 
            {
                DoOnException(ex);
            }
            finally 
            {
                _db.disconnect();
            }

            loadList();
        }

        private void loadList() 
        {
            DataTable table;
            _db = DBData.getDBData(Session);
            _db.connect();

            try
            {
                _titleColumn = _db.langAttrName("ORGENTITY", "TITLE");

                if (OrderColumn == "")
                {
                    OrderColumn = "O." + _titleColumn;
                }

                // Abfolge VERFUEGBAR, AUFWAND, DIFFERENZ ist fix wegen der Berechnung
                // der Differenz in onAddCell()
                string sql = "select O.ID, B.ID BUDGET_ID,"
                    + " O." + _titleColumn + ","
                    + " B.BETRAG_VERTEILBAR VERTEILBAR,"
                    + " cast(null as float) VERFUEGBAR,"
                    + " cast(null as float) AUFWAND,"
                    + " cast(null as float) VERTEILT," // f�r Differenz verteilbar
                    + " cast(null as float) DIFFERENZ,"
                    + " isnull(B.BEARBEITUNGSSTATUS, " + (int)LohnModule.EditingState.Pending + ") BEARBEITUNGSSTATUS,"
                    + " cast(null as integer) FREIGABESTATUS"
                    + " from BUDGET B right join ORGENTITY O"
                    + " on O.ID = B.ORGENTITY_ID"
                    + "  and B.KOMPONENTE_ID = " + _komponenteId
                    + (BudgettypId == -1 ? "" : (" and B.BUDGETTYP_ID = " + BudgettypId))
                    + " where O.PARENT_ID = " + OrgentityId
                    + " order by " + OrderColumn + " " + OrderDir;
                table = _db.getDataTableExt(sql, "DLA_ORGENTITY_V");

                DetailURL = BudgetCheck.GetURL(
                    "orgId", "%ID",
                    "salaryComponent", SalaryComponent,
                    "budgettypId", BudgettypId
                );
                DetailEnabled = true;
                DeleteEnabled = false;
                EditEnabled = true;
                EditURL = SalaryAdjustment.GetURL(
                    "orgId", "%ID",
                    "salaryComponent", SalaryComponent
                );
                InfoBoxEnabled = false;
                SortURL = BudgetCheck.GetURL(
                    "orgId", OrgentityId,
                    "salaryComponent", SalaryComponent,
                    "budgettypId", BudgettypId
                );
                RowsPerPage = SessionData.getRowsPerListPage(Session);

                LoadList(_db, table, listTab);
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

        /// <summary>
        /// Spezialbehandlung der Headerzellen
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="r"></param>
        /// <param name="cell"></param>
        protected override void onAddHeaderCell(DataRow row, DataColumn col, TableRow r, TableCell cell)
        {
            if (col != null)
            {
                HyperLink link = cell.Controls[0] as HyperLink;

                if (link != null)
                {
                    switch (col.ColumnName)
                    {
                        case "VERFUEGBAR":
                            link.NavigateUrl = "";
                            break;
                        case "AUFWAND":
                            link.NavigateUrl = "";
                            break;
                        case "VERTEILT": // f�r Differenz verteilbar
                            link.Text = _mapper.get("lohn", "differenzVerteilbar");
                            link.NavigateUrl = "";
                            break;
                        case "DIFFERENZ": // f�r Differenz verteilbar
                            link.Text = _mapper.get("lohn", "differenzVerfuegbar");
                            link.NavigateUrl = "";
                            break;
                       case "FREIGABESTATUS":
                            link.NavigateUrl = "";
                            break;
                    }
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
        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell)
        {
            if (col != null)
            {
                long subOrgentityId = DBColumn.GetValid(row["ID"], (long)-1);
                double betrag = 0;
                System.Web.UI.WebControls.Image image;

                switch (col.ColumnName)
                {
                    case "VERTEILBAR":
                        cell.HorizontalAlign = HorizontalAlign.Right;
                        break;
                    case "VERFUEGBAR":
                        if (subOrgentityId != -1)
                        {
                            betrag = LohnModule.GetBudgetVerfuegbar(
                                    subOrgentityId,
                                    _komponenteId,
                                    BudgettypId,
                                    true
                                );
                            cell.Text = _db.GetDisplayValue(col, betrag, true);
                            _differenz = betrag;
                        }

                        cell.HorizontalAlign = HorizontalAlign.Right;
                        break;
                    case "AUFWAND":
                        if (subOrgentityId != -1)
                        {
                            betrag = LohnModule.GetAufwand(
                                    subOrgentityId,
                                    _komponenteId,
                                    BudgettypId,
                                    true
                                );
                            cell.Text = _db.GetDisplayValue(col, betrag, true);
                            _differenzVerteilbar
                                = DBColumn.GetValid(row["VERTEILBAR"], (double)0) - betrag; 
                            _differenz -= betrag; 
                        }

                        cell.HorizontalAlign = HorizontalAlign.Right;
                        break;
                    case "VERTEILT": // f�r Differenz verteilbar
                        if (subOrgentityId != -1)
                        {
                            cell.Text = _db.GetDisplayValue(col, _differenzVerteilbar, true);
                        }

                        cell.HorizontalAlign = HorizontalAlign.Right;
                        break;
                    case "DIFFERENZ":
                        if (subOrgentityId != -1)
                        {
                            cell.Text = _db.GetDisplayValue(col, _differenz, true);
                        }

                        cell.HorizontalAlign = HorizontalAlign.Right;
                        break;
                    case "BEARBEITUNGSSTATUS":
                        image = new System.Web.UI.WebControls.Image();
                        int bearbeitungsstatus 
                            = DBColumn.GetValid(row[col], (int)LohnModule.EditingState.Pending);

                        switch (bearbeitungsstatus)
                        {
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

                        if (image.ImageUrl != "")
                        {
                            cell.Controls.Clear();
                            cell.Controls.Add(image);
                        }

                        cell.HorizontalAlign = HorizontalAlign.Center;
                        break;
                    case "FREIGABESTATUS":
                        image = new System.Web.UI.WebControls.Image();
                        int freigabestatus = DBColumn.GetValid(
                                _db.lookup(
                                    "min(FREIGABESTATUS)",
                                    "BUDGET",
                                    "ORGENTITY_ID = " + subOrgentityId
                                        + " and KOMPONENTE_ID = " + _komponenteId
                                ),
                                (int)LohnModule.ClearingState.Disabled
                            );

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

                        cell.HorizontalAlign = HorizontalAlign.Center;
                        break;
                    default:
                        // link nur, falls Unter-OEs vorhanden
                        if (col.ColumnName == _titleColumn)
                        {
                            long subSubOrgentityId = DBColumn.GetValid(
                                    _db.lookup(
                                        "ID",
                                        "ORGENTITY",
                                        "PARENT_ID = " + subOrgentityId
                                    ),
                                    (long)-1
                                );
                            
                            if (subSubOrgentityId == -1)
                            {
                                System.Web.UI.Control control = cell.Controls[0];

                                if (control is HyperLink)
                                {
                                    ((HyperLink)control).Enabled = false;
                                }
                            }
                        }

                        break;
                }
            }
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
