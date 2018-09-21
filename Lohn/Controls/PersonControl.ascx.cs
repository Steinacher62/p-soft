using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Organisation;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Lohn.Controls
{
    /// <summary>
    ///	Übersicht der Lohndaten
    ///	Bei Kontext = "adjustment" mit Links für die Anzeige der Details. Anzeige wahlweise nur
    ///	                 jener mit Berechtigung oder aller.
    ///	Bei Kontext = "overview" mit Excel-Button. Anzeige immer alle.
    /// </summary>
    public partial class PersonControl : PSOFTListViewUserControl {
        public const string PARAM_ORGENTITY_ID = "PARAM_ORGENTITY_ID";
        public const string PARAM_SALARY_COMPONENT = "PARAM_SALARY_COMPONENT";
        public const string PARAM_CONTEXT = "PARAM_CONTEXT";
        public const string PARAM_BUDGETTYPID = "PARAM_BUDGETTYPID";
        private static int _fileVersion = 0;

        protected CSVWriter _excelExport = null;
        protected string _onloadString = "";

        private DBData _db;
        private ArrayList _editingStates;
        private long _komponenteId = -1;
        private long _varianteId = -1;
        private string _functionAttribute = "";
        private int _freigabestatusMinimal = (int)LohnModule.ClearingState.Disabled;
        private bool _readOnly = true;
        private MainControl _oeList;
        private string _sql = "";
        private int _compFlag = 0;

        public static string Path {
            get {return Global.Config.baseURL + "/Lohn/Controls/PersonControl.ascx";}
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

        public string Kontext {
            get {return GetString(PARAM_CONTEXT);}
            set {SetParam(PARAM_CONTEXT, value);}
        }
        
        public long BudgettypId {
            get {return GetLong(PARAM_BUDGETTYPID);}
            set {SetParam(PARAM_BUDGETTYPID, value);}
        }
        
        public string DBViewName {
            get {return LohnModule.KundenModuleName.ToUpper() + "_LISTE_V";}
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
            export.Text = _mapper.get("lohn", "exportExcel");
            showAllCb.Visible = true;

            _db = DBData.getDBData(Session);

            if (!IsPostBack) {
                export.Visible = Kontext == "overview";
                showAllLbl.Text = _mapper.get("lohn", "showAll"+Kontext);
                showAllCb.Checked = SessionData.getBoolValue(Session, "LOHN_SHOWALL_FILTER_"+Kontext);
                pnameLbl.Text = _mapper.get("PERSON","PNAME");
                pname.Text = SessionData.getStringValue(Session,"LOHN_PNAME_FILTER_"+Kontext);
                search.Text = _mapper.get("search"); 
                compRow.Visible = Kontext == "overview";
                compLbl.Text = _mapper.get("lohn","komponente");
                _compFlag = SessionData.getIntValue(Session,"LOHN_COMPLIST_FILTER_"+Kontext);
                if (_compFlag > 0) {
                    compList.Items[0].Selected = (_compFlag & 1) > 0;
                    compList.Items[1].Selected = (_compFlag & 2) > 0;
                }
            }

            if (!IsPostBack) loadQuery();
        }

        private void loadQuery() {
            _db.connect();

            try {
                object [] values = _db.lookup(
                    new string [] {"K.ID", "V.ID"},
                    "KOMPONENTE K inner join VARIANTE V on K.VARIANTE_ID = V.ID",
                    "V.HAUPTVARIANTE = 1 and K.BEZEICHNUNG = '" + SalaryComponent + "'"
                    );
                _komponenteId = DBColumn.GetValid(values[0], (long)-1);
                _varianteId = DBColumn.GetValid(values[1], (long)-1);

                if (_komponenteId == -1) {
                    throw new Exception("Komponente not found");
                }
                
                if (IsPostBack) {
                    _compFlag = 0;
                    if (compList.Items[0].Selected) _compFlag = 1;
                    if (compList.Items[1].Selected) _compFlag += 2;
                    Session["LOHN_COMPLIST_FILTER_"+Kontext] = _compFlag;
                    Session["LOHN_PNAME_FILTER_"+Kontext] = pname.Text;
                    Session["LOHN_SHOWALL_FILTER_"+Kontext] = showAllCb.Checked;
                }
                else {
                    string s = _db.lookup("BEZEICHNUNG","KOMPONENTE","ID="+_komponenteId,"");
                    compList.Items[0].Text = _mapper.get("lohn","with_"+s);
                    compList.Items[1].Text = _mapper.get("lohn","without_"+s);
                }

                _functionAttribute = _db.langAttrName(DBViewName, "TITLE");
                values = _db.lookup(
                    new string [] {"min(BEARBEITUNGSSTATUS)", "min(FREIGABESTATUS)"},
                    "BUDGET",
                    "KOMPONENTE_ID = " + _komponenteId + " and ORGENTITY_ID = " + OrgentityId
                    +(BudgettypId > 0 ? " and BUDGETTYP_ID="+BudgettypId : "")
                    );
                _freigabestatusMinimal = DBColumn.GetValid(
                    values[1],
                    (int)LohnModule.ClearingState.Disabled
                    );
                int bearbeitungsstatusMinimal = DBColumn.GetValid(
                    values[0],
                    (int)LohnModule.EditingState.Pending
                    );
                _readOnly = ((int)LohnModule.EditingState.Pending < bearbeitungsstatusMinimal);

                string columnList = "," + _db.getColumnNames(DBViewName) + ",";
                columnList = columnList.Replace(",", ",L.");
                columnList = columnList.Replace("L.BETRAG_PRO_STUNDE,", "");
                columnList = columnList.Replace("L.BETRAG_PRO_TAG,", "");
                columnList = columnList.Replace("L.BETRAG,", "LK.BETRAG,");
                columnList = columnList.Replace("L.BEARBEITUNGSSTATUS,", "LK.BEARBEITUNGSSTATUS,");

                _sql = "select " + columnList.Substring(1, columnList.Length - 3)
                    + " case isnull(L.ANZAHL_STUNDEN, 0) when 0 then 0"
                    + " else LK.BETRAG / L.ANZAHL_STUNDEN"
                    + " end BETRAG_PRO_STUNDE,"
                    + " case isnull(L.ANZAHL_TAGE, 0) when 0 then 0"
                    + " else LK.BETRAG / L.ANZAHL_TAGE"
                    + " end BETRAG_PRO_TAG,"
                    + " LK.ID LOHNKOMPONENTE_ID"
                    + " from LOHNKOMPONENTE LK"
                    + (_compFlag == 1 ? " inner join " : " right join ") + DBViewName + " L"
                    + " on LK.LOHN_ID = L.LOHN_ID and LK.KOMPONENTE_ID = " + _komponenteId
                    + " where VARIANTE_ID = " + _varianteId
                    + (BudgettypId > 0 ? " and L.BUDGETTYP_ID = "+BudgettypId : "")
                    + (_compFlag == 2 ? " and LK.ID is null " : "");

                switch (Kontext) {
                case "adjustment":
                    _sql += " and ORGENTITY_ID = " + OrgentityId
                        + (showAllCb.Checked ? "" : " and LK.ID is not null");

                    // leerer select, falls nicht alle Budget freigegeben sind
                    if (_freigabestatusMinimal != (int)LohnModule.ClearingState.Enabled) {
                        _sql += " and 0 = 1";
                    }

                    break;
                default:
                    _sql += " and ORGENTITY_ID ";

                    if (showAllCb.Checked) {
                        _sql += "in (" + _db.Orgentity.addAllSubOEIDs(OrgentityId.ToString()) + ")";
                    }
                    else {
                        _sql += "= " + OrgentityId;
                    }

                    break;
                }

                if (pname.Text != "") {
                    _sql += " and LOWER(L.PNAME) like '"+DBColumn.toSql(pname.Text.ToLower())+"%'";
                }
                if (OrderColumn == "") {
                    _sql += " order by L.PNAME, L.FIRSTNAME";
                }
                else {
                    _sql += " order by " + OrderColumn + " " + OrderDir;
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }


            loadList();

            _oeList = (MainControl)this.LoadPSOFTControl(MainControl.Path, "_oeList");
            _oeList.ParentId = OrgentityId;
            _oeList.Kontext = Kontext;
            _oeList.SalaryComponent = SalaryComponent;
            _oeList.BudgettypId = BudgettypId;
            _oeList.WithBudgetOnly = true;

            oeListTab.Controls.Add(_oeList);
        }

        /// <summary>
        /// Konstruiert den Select (_sql)
        /// Erstellt die Liste auf Grund des Selects, der extended Attribute von
        /// DBViewName und der View (Property)
        /// </summary>
        private void loadList() {
            listTab.Rows.Clear();

            _db.connect();

            try {
                DataTable table = _db.getDataTableExt(_sql, DBViewName);
                
                CheckOrder = true;
                IDColumn = "LOHN_ID";
                DetailEnabled = (Kontext == "adjustment");
                DetailURL = SalaryAdjustment.GetURL(
                    "orgId", OrgentityId,
                    "salaryComponent", SalaryComponent,
                    "lohnId", "%LOHN_ID",
                    "readOnly", _readOnly,
                    "orderColumn", OrderColumn,
                    "orderDir", OrderDir,
                    "budgettypId", BudgettypId
                    );
                EditEnabled = false;
                InfoBoxEnabled = false;
                SortURL = SalaryAdjustment.GetURL(
                    "orgId", OrgentityId,
                    "salaryComponent", SalaryComponent,
                    "context", Kontext
                    );
                RowsPerPage = SessionData.getRowsPerListPage(Session);
                View = SalaryComponent.ToUpper();

                if (Kontext == "adjustment") {
                    if (table.Columns.Contains("COSTCENTER_NUMBER")) table.Columns["COSTCENTER_NUMBER"].ExtendedProperties["OrdNum"] = 290;
                    RowsPerPage = (int)(RowsPerPage * 1.3);
                }
                else View = Kontext.ToUpper() + "_" + View;

                if (table.Columns.Contains("SEX")) {
                    table.Columns["SEX"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("person", "sex", true));
                }

                if (table.Columns.Contains("BUDGETTYP_ID")) {
                    DataTable budgettypTable = _db.getDataTable(
                        "select ID, BEZEICHNUNG from BUDGETTYP order by BEZEICHNUNG"
                        );
                    table.Columns["BUDGETTYP_ID"].ExtendedProperties["In"] = budgettypTable;
                }

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
                int lohnart = 0;

                switch (col.ColumnName) {
                case "BEARBEITUNGSSTATUS":
                    System.Web.UI.WebControls.Image im = new System.Web.UI.WebControls.Image();

                    switch(cell.Text) {
                    case "0":
                        im.ImageUrl = "../../images/ampelRot.gif";
                        im.ToolTip = _editingStates[0].ToString();
                        break;
                    default:
                        im.ImageUrl = "../../images/ampelGruen.gif";
                        im.ToolTip = _editingStates[1].ToString();
                        break;
                    }

                    cell.Text = "";
                    cell.Controls.Add(im);
                    cell.HorizontalAlign = HorizontalAlign.Center;
                    break;
                case "SEX":
                case "COSTCENTER_NUMBER":
                    cell.HorizontalAlign = HorizontalAlign.Center;
                    break;
                case "BETRAG":
                case "ISTLOHN_PROZENT":
                case "NEUER_LOHN":
                case "NEUER_LOHN_PRO_ISTLOHN":
                case "NEUER_LOHN_PROZENT":
                case "ZUNAHME_PROZENT":
                case "VERGLEICH":
                case "ABWEICHUNG":
                case "ABWEICHUNG_PROZENT":
                case "LOHNVORSCHLAG":
                case "LOHNVORSCHLAG_100":
                case "ISTLOHN":
                case "IST_TAGLOHN":
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    break;
                case "NEUER_STUNDENLOHN":
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    lohnart = DBColumn.GetValid(row["LOHNART"], 1); // default Jahreslohn

                    if (lohnart != 4) { // Stundenlohn
                        cell.Text = "";
                    }

                    break;
                case "NEUER_TAGLOHN":
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    lohnart = DBColumn.GetValid(row["LOHNART"], 1); // default Jahreslohn

                    if (lohnart != 3) { // Taglohn
                        cell.Text = "";
                    }

                    break;
                case "BETRAG_PRO_STUNDE":
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    lohnart = DBColumn.GetValid(row["LOHNART"], 1); // default Jahreslohn

                    if (lohnart != 4) { // Stundenlohn
                        cell.Text = "";
                    }

                    break;
                case "BETRAG_PRO_TAG":
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    lohnart = DBColumn.GetValid(row["LOHNART"], 1); // default Jahreslohn

                    if (lohnart != 3) { // Taglohn
                        cell.Text = "";
                    }

                    break;
                case "ENGAGEMENT":
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    lohnart = DBColumn.GetValid(row["LOHNART"], 1); // default Jahreslohn
                        
                    switch (lohnart) {
                    case 3: // Taglöhner
                    case 4: // Stundenlöhner
                        cell.Text = "" + DBColumn.GetValid(row["ANZAHL_TAGE"], 0)
                            + " " + _mapper.get("lohn", "shortDay");
                        break;
                    }

                    break;
                case "FUNCTION_EXTREF":
                    long employmentId = DBColumn.GetValid(row["ID"], (long)-1);
                    string extref  = OrganisationModule.getFunktionEXTREF(_db, employmentId, "");

                    cell.Text = extref;
                    break;

                default:
                    // Hauptfunktion anzeigen
                    if (col.ColumnName == _functionAttribute) {
                        employmentId = DBColumn.GetValid(row["ID"], (long)-1);
                        string alternative = LohnModule.KundenModuleName == "tpc" ? "JOB" : "";
                        string funktionTitle = OrganisationModule.getFunktionTitle(_db, employmentId, alternative);

                        if (funktionTitle != "") {
                            cell.Text = funktionTitle;
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Spezialbehandlung der Headerzellen
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="r"></param>
        /// <param name="cell"></param>
        protected override void onAddHeaderCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (col != null) {
                switch (col.ColumnName) {
                case "BETRAG":
                    if (SalaryComponent != "salaire") { // nur für Rsr und tpc (historisch!)
                        cell.Text = _mapper.get(LohnModule.KundenModuleName, SalaryComponent);
                    }

                    break;
                default:
                    break;
                }
            }
        }

        /// <summary>
        /// Grösse anpassen abhängig davon, ob untergeordnete OEs vorhanden sind
        /// oder nicht.
        /// </summary>
        /// <param name="ev"></param>
        protected override void OnPreRender(EventArgs ev) {
            base.OnPreRender(ev);

            if (_oeList != null) {
                if (_oeList.RowCount == 0) {
                    listTabRow.Height = Unit.Percentage(100);
                    oeListTabRow.Visible = false;
                }
                else {
                    listTabRow.Height = Unit.Percentage(67);
                    oeListTabRow.Height = Unit.Percentage(33);
                }
            }
        }

        /// <summary>
        /// Erstellen eines Excel-Reports (CSV) und Anzeigen in der Seite ShowReport
        /// Konzept:
        /// 1. neuer CSVWriter _excelExport (= new CSVWriter())
        /// 2. Aufruf von DBData.scanTableColumn() und DBData.scanTableData()
        ///    mit Handlern, welche in _excelExport die Zellen zufügt
        /// 3. _excelExport in File speichern
        /// 
        /// Bemerkung:
        /// _sql und das Anzeige-Konzept der Liste können verwendet werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void export_Click(object sender, System.EventArgs e) {
            loadQuery();
            _excelExport = new CSVWriter();
            DataTable table = _db.getDataTableExt(_sql, DBViewName);
            _db.scanColumn += new ScanActionColumnHandler(scanColumn);
            _db.scanTableColumn(table, View, (int) DBColumn.Visibility.LIST, DBData.AUTHORISATION.READ, true);
            _db.scanColumn -= new ScanActionColumnHandler(scanColumn);

            _db.scanRow += new ScanActionRowHandler(scanRow);
            _db.scanCell += new ScanActionCellHandler(scanCell);
            _db.scanTableData(
                table, 
                View, 
                (int)DBColumn.Visibility.LIST, 
                DBData.AUTHORISATION.READ,
                DBData.APPLICATION_RIGHT.COMMON, 
                true, 
                false
                );
            _db.scanRow -= new ScanActionRowHandler(scanRow);
            _db.scanCell -= new ScanActionCellHandler(scanCell);


            string param = SalaryComponent+"ExportTemplate";
            string template = Global.Config.getModuleParam(LohnModule.ModuleName,param,"");
            int version = (_fileVersion++ % 9)+1;
            string relativeFilename = ReportModule.REPORTS_DIRECTORY
                + "/Lohn_Export_" + SalaryComponent + "_" + SessionData.getUserID(Session) + "_"+ version;
            string url = "";

            _excelExport.save(Server.MapPath("~" + relativeFilename +".csv"));

            if (template != "") {
                template = Server.MapPath("~/Vorlagen/" + template);
                if (File.Exists(template)) {
                    File.Copy(template,Server.MapPath("~" + relativeFilename+".xls"),true);
                    url = ShowReport.GetURL("reportName", ".." + relativeFilename+".xls");
                }
            }
            if (url == "") url = ShowReport.GetURL("reportName", ".." + relativeFilename +".csv");
            _onloadString = "window.open('" + url + "');";
        }

        /// <summary>
        /// Handler für Headerzellen in _excelExport
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <param name="columnNumber"></param>
        private void scanColumn(DataTable table, DataColumn column, int columnNumber) {
            if (columnNumber > 0) {
                string cellText = _mapper.get(View, column.ColumnName);
                
                switch (column.ColumnName) {
                case "BETRAG":
                    if (SalaryComponent != "salaire") { // nur für Rsr und tpc (historisch!)
                        cellText = _mapper.get(LohnModule.KundenModuleName, SalaryComponent);
                    }

                    break;
                default:
                    break;
                }

                _excelExport.addColumn(column.ColumnName, cellText);
            }
        }

        /// <summary>
        /// Handler für Datenzellen in _excelExport
        /// </summary>
        /// <param name="table"></param>
        /// <param name="row"></param>
        /// <param name="rowNumber"></param>
        /// <param name="rowAuthorisations"></param>
        /// <returns></returns>
        private bool scanRow(DataTable table, DataRow row, int rowNumber, int rowAuthorisations) {
            _excelExport.addRow();
            return true;
        }

        private void scanCell(DataTable table, DataRow row, int rowNumber, DataColumn column, int columnNumber) {
            int visi = (int) column.ExtendedProperties["Visibility"];
            
            if (columnNumber > 0) {
                string cellText = (column.ExtendedProperties["In"] == null)? _db.GetDisplayValue(column, row[column], false) : DBColumn.LookupIn(column, row[column], false);
                
                switch (column.ColumnName) {
                case "NEUER_LOHN":
                case "BETRAG":
                case "NEUER_STUNDENLOHN":
                case "BETRAG_PRO_STUNDE":
                case "NEUER_TAGLOHN":
                case "BETRAG_PRO_TAG":
                case "VERGLEICH":
                    if ((column.ColumnName == "NEUER_STUNDENLOHN" || column.ColumnName == "BETRAG_PRO_STUNDE")
                        && row["LOHNART"].ToString() != "4" // nicht Stundenlohn
                        || (column.ColumnName == "NEUER_TAGLOHN" || column.ColumnName == "BETRAG_PRO_TAG")
                        && row["LOHNART"].ToString() != "3" // nicht Taglohn
                        ) {
                        cellText = "";
                    }
                    else {
                        cellText = LohnModule.toCurrency(ch.psoft.Util.Validate.GetValid(cellText, (double)0));
                    }

                    break;
                case "ENGAGEMENT":
                    int lohnart = DBColumn.GetValid(row["LOHNART"], 1); // default Jahreslohn
                        
                    switch (lohnart) {
                    case 3: // Taglöhner
                    case 4: // Stundenlöhner
                        cellText = "" + DBColumn.GetValid(row["ANZAHL_TAGE"], 0)
                            + " " + _mapper.get("lohn", "shortDay");
                        break;
                    }

                    break;

                case "FUNCTION_EXTREF":
                    long employmentId = DBColumn.GetValid(row["ID"], (long)-1);
                    cellText  = OrganisationModule.getFunktionEXTREF(_db, employmentId, "");
                    break;

                default:
                    // Hauptfunktion anzeigen
                    if (column.ColumnName == _functionAttribute) {
                        employmentId = DBColumn.GetValid(row["ID"], (long)-1);
                        string alternative = LohnModule.KundenModuleName == "tpc" ? "JOB" : "";
                        string funktionTitle = OrganisationModule.getFunktionTitle(_db, employmentId, alternative);

                        if (funktionTitle != "") {
                            cellText = funktionTitle;
                        }
                    }
                    break;
                }

                _excelExport.addRowData(column.ColumnName, cellText);
            }
        }

        private void search_Click(object sender, System.EventArgs e) {
            loadQuery();
            redirect();
        }

        private void redirect() {
            string url = Request.RawUrl;
            int idx = url.IndexOf("&lohnId=");
            if (idx > 0 && listTab.Rows.Count > 2) {
                int idx2 = url.IndexOf("&",idx+8);
                url = url.Substring(0,idx)+(idx2 > 0 ? url.Substring(idx2,url.Length-idx2) : "");
                Response.Redirect(url);
            }
        }

        private void mapControls() {
            export.Click += new System.EventHandler(export_Click);
            search.Click += new System.EventHandler(search_Click);
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
