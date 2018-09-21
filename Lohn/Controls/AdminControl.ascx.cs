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
    ///	Administration der Stati
    /// </summary>
    public partial class AdminControl : PSOFTListViewUserControl
	{
        public const string PARAM_SALARY_COMPONENT = "PARAM_SALARY_COMPONENT";

        protected ArrayList _editingStates;
        protected ArrayList _clearingStates;
        protected string _onloadString;

        public static string Path
        {
            get {return Global.Config.baseURL + "/Lohn/Controls/AdminControl.ascx";}
        }

        #region Properities

        public string SalaryComponent // Komponenten-Bezeichnung
        {
            get {return GetString(PARAM_SALARY_COMPONENT);}
            set {SetParam(PARAM_SALARY_COMPONENT, value);}
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
            _onloadString = "";
            loadList();
        }

        /// <summary>
        /// Konstruiert den Select 
        /// Erstellt die Liste auf Grund des Selects und der extended Attribute von
        /// BUDGET_V
        /// </summary>
        private void loadList() 
        {
            listTab.Rows.Clear(); // Löschen der beim Post Back aufgebauten Tabelle

            _editingStates = new ArrayList(_mapper.getEnum("lohn", "editingState", true));
            _clearingStates = new ArrayList(_mapper.getEnum("lohn", "clearingState", true));
            DBData db = DBData.getDBData(Session);
            DataTable table;
            db.connect();

            try
            {
                long komponenteId = DBColumn.GetValid(
                        db.lookup(
                            "K.ID",
                            "KOMPONENTE K, VARIANTE V",
                            "V.HAUPTVARIANTE = 1"
                                + " and K.VARIANTE_ID = V.ID"
                                + " and K.BEZEICHNUNG = '" + SalaryComponent + "'"
                        ),
                        (long)-1
                    );
                string oeTitleAttribute = db.langAttrName("BUDGET_V", "TITLE");
                string sql = "select ID, " + oeTitleAttribute + ","
                    + " BUDGETTYP, BETRAG, BEARBEITUNGSSTATUS, FREIGABESTATUS"
                    + " from BUDGET_V where KOMPONENTE_ID = " + komponenteId;

                if (OrderColumn == "")
                {
                    sql += " order by " + oeTitleAttribute + ", BUDGETTYP";
                }
                else if (OrderColumn.ToLower().IndexOf("budgettyp") == -1)
                {
                    sql += " order by " + OrderColumn + " " + OrderDir + ", BUDGETTYP";
                }
                else
                {
                    sql += " order by " + OrderColumn + " " + OrderDir;
                }

                table = db.getDataTableExt(sql, "BUDGET_V");

                DetailEnabled = false;
                DeleteEnabled = false;
                EditEnabled = false;
                InfoBoxEnabled = false;
                SortURL = Admin.GetURL("salaryComponent", SalaryComponent);
                RowsPerPage = (int)(SessionData.getRowsPerListPage(Session) * 1.3);
                LoadList(db, table, listTab);
            }
            catch (Exception ex)
            {
                DoOnException(ex);
            }
            finally
            {
                db.disconnect();
            }
        }
 
        /// <summary>
        /// Spezialbehandlung für Datenzellen der Liste
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="r"></param>
        /// <param name="cell"></param>
        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell)
        {
            if (col != null)
            {
                switch(col.ColumnName)
                {
                    case "BETRAG":
                        cell.HorizontalAlign = HorizontalAlign.Right;
                        break;
                    case "BEARBEITUNGSSTATUS":
                        DropDownList ddEdit = new DropDownList();
                        cell.Controls.Add(ddEdit);
                        ddEdit.Items.Add(new ListItem(_editingStates[0] as string, "0"));
                        ddEdit.Items.Add(new ListItem(_editingStates[1] as string, "1"));
                        ddEdit.Items.Add(new ListItem(_editingStates[2] as string, "2"));
                        ddEdit.SelectedIndex = int.Parse(row[col].ToString());
                        ddEdit.ID = "ddEditingState" + row["ID"].ToString();
                        ddEdit.SelectedIndexChanged += new EventHandler(ddEditingState_SelectedIndexChanged);
                        ddEdit.AutoPostBack = true;

                        System.Web.UI.WebControls.Image im = new System.Web.UI.WebControls.Image();

                        switch(ddEdit.SelectedIndex)
                        {
                            case 0:
                                im.ImageUrl = "../../images/ampelRot.gif";
                                im.ToolTip = _editingStates[0].ToString();
                                break;
                            case 1:
                                im.ImageUrl = "../../images/ampelOrange.gif";
                                im.ToolTip = _editingStates[1].ToString();
                                break;
                            default:
                                im.ImageUrl = "../../images/ampelGruen.gif";
                                im.ToolTip = _editingStates[2].ToString();
                                break;
                        }

                        cell.Controls.Add(im);
                        break;
                    case "FREIGABESTATUS":
                        cell.Controls.Clear();
                        DropDownList ddClear = new DropDownList();
                        cell.Controls.Add(ddClear);
                        ddClear.Items.Add(new ListItem(_clearingStates[0] as string, "0"));
                        ddClear.Items.Add(new ListItem(_clearingStates[1] as string, "1"));
                        ddClear.Items.Add(new ListItem(_clearingStates[2] as string, "2"));
                        ddClear.SelectedIndex = int.Parse(row[col].ToString());
                        ddClear.ID = "ddClearingState" + row["ID"].ToString();
                        ddClear.SelectedIndexChanged += new EventHandler(ddClearingState_SelectedIndexChanged);
                        ddClear.AutoPostBack = true;

                        System.Web.UI.WebControls.Image ie = new System.Web.UI.WebControls.Image();

                        switch(ddClear.SelectedIndex)
                        {
                            case 0:
                                ie.ImageUrl = "../../images/ampelRot.gif";
                                ie.ToolTip = _clearingStates[0].ToString();
                                break;
                            case 1:
                                ie.ImageUrl = "../../images/ampelOrange.gif";
                                ie.ToolTip = _clearingStates[1].ToString();
                                break;
                            default:
                                ie.ImageUrl = "../../images/ampelGruen.gif";
                                ie.ToolTip = _clearingStates[2].ToString();
                                break;
                        }

                        cell.Controls.Add(ie);
                        break;
                }
            }
        }

        /// <summary>
        /// Sorgt für die Anzeige der richtigen Seite, falls sich die Liste über
        /// mehrere Seiten verteilt ist
        /// </summary>
        /// <param name="budgetID"></param>
        private void showBudgetOnLoad(long budgetID)
        {
            if (listTab.Rows.Count > RowsPerPage)
            {
                int actualPage = 0;

                foreach (TableRow row in listTab.Rows) 
                {
                    if (ListBuilder.getID(row) == budgetID.ToString()) 
                    {
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

        /// <summary>
        /// Update des Bearbeitungsstatus in der Db bei Wechsel in der DropDownListe
        /// Falls es mehrere Budget mit verschiedenen Budgettypen gibt, werden die Stati
        /// bei allen gleichzeitig gesetzt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddEditingState_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (sender is DropDownList)
            {
                DBData db = DBData.getDBData(Session);
                DropDownList dd = (sender as DropDownList);
                long budgetID = long.Parse(dd.ID.Substring(14));
                db.connect();

                try
                {
                    object [] values = db.lookup(
                            new string [] {"ORGENTITY_ID", "KOMPONENTE_ID"},
                            "BUDGET",
                            "ID = " + budgetID
                        );
                    int number = db.execute(
                            "update BUDGET set BEARBEITUNGSSTATUS = " + dd.SelectedItem.Value
                                + " where ORGENTITY_ID = " + values[0]
                                + " and KOMPONENTE_ID = " + values[1]
                        );
                    
                    if (number < 1)
                    {
                        db.execute(
                            "update BUDGET set BEARBEITUNGSSTATUS = " + dd.SelectedItem.Value
                                + " where ID = " + budgetID
                        );
                    }
                }
                catch (Exception error)
                {
                    DoOnException(error);
                }
                finally
                {
                    db.disconnect();
                }

                loadList();
                showBudgetOnLoad(budgetID);
            }
        }

        /// <summary>
        /// Update des Freigabestatus in der Db bei Wechsel in der DropDownListe
        /// Das Zurücksetzen wird pro Komponente und Budgettyp hierarchisch nach unten
        /// weitergegeben
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddClearingState_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (sender is DropDownList)
            {
                DBData db = DBData.getDBData(Session);
                DropDownList dd = (sender as DropDownList);
                long budgetID = long.Parse(dd.ID.Substring(15));
                db.connect();

                try
                {
                    db.beginTransaction();
                    db.execute(
                        "update BUDGET set FREIGABESTATUS = " + dd.SelectedItem.Value
                        + " where ID = " + budgetID
                    );
                    int state = Validate.GetValid(
                            dd.SelectedItem.Value,
                            (int)LohnModule.ClearingState.Enabled
                        );

                    if (state < (int)LohnModule.ClearingState.Enabled)
                    {
                        object [] values = db.lookup(
                                new string [] {"ORGENTITY_ID", "KOMPONENTE_ID", "BUDGETTYP_ID"},
                                "BUDGET",
                                "ID = " + budgetID
                            );
                        LohnModule.resetClearingState(
                            DBColumn.GetValid(values[0], (long)-1),
                            DBColumn.GetValid(values[1], (long)-1),
                            DBColumn.GetValid(values[2], (long)-1),
                            (int)LohnModule.ClearingState.Disabled,
                            true
                        );
                    }

                    db.commit();
                }
                catch (Exception error)
                {
                    db.rollback();
                    DoOnException(error);
                }
                finally
                {
                    db.disconnect();
                }

                loadList();
                showBudgetOnLoad(budgetID);
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
