using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Handler wird nach dem Zufuegen einer Zelle gerufen
    /// </summary>
    /// <param name="row">Datenrow</param>
    /// <param name="col">DB-kolonne</param>
    /// <param name="r">Row der Web-tabelle</param>
    /// <param name="cell">Zelle der Web-tabelle</param>
    public delegate void AddCellHandler(DataRow row, DataColumn col, TableRow r, TableCell cell);
    /// <summary>
    /// Handler wird nach dem Zufuegen einer Zeile gerufen
    /// </summary>
    /// <param name="row">Datenrow</param>
    /// <param name="r">Row der Web-tabelle mit allen Zellen</param>
    public delegate void AddRowHandler(DataRow row, TableRow r);
    /// <summary>
    /// Handler wird nach dem Zufuegen einer Property (Kolonnenname Kolonnenwert)
    /// </summary>
    /// <param name="row">Datenrow</param>
    /// <param name="col">DB-kolonne</param>
    /// <param name="r">Row der Web-tabelle (besteht aus 2 Zellen)</param>
    public delegate void AddPropertyHandler(DataRow row, DataColumn column, TableRow r);
    /// <summary>
    /// Behandelt alle Aspekte einer List.
    /// </summary>
    public class ListBuilder : Access {
        private const string DELETE_CELL_ID = "LB_DELETE_CELL_";
        private const string EDIT_CELL_ID = "LB_EDIT_CELL_";
        private const string INFO_CELL_ID = "LB_INFO_CELL_";

        protected Table _listTable = null;
        private DataColumn _idColumn = null;
        private DataColumn _detailColumn = null;
        private DataColumn _orderColumn = null;
        private enum ORDER_DIR {
            ASC,
            DESC
        }
        private ORDER_DIR _orderDir = ORDER_DIR.ASC;
        private bool _headerEnable = true;
        private bool _infoBoxEnable = true;
        private string _infoImage = "icon_info.png"; //"info.gif";
        private bool _checkBoxEnable = false;
        private string _sortURL = "List.aspx";
        private bool _deleteEnable = false;
        private bool _editEnable = false;
        private bool _detailEnable = false;
        private string _detailURL = "Detail.aspx?id=%ID";
        private string _detailToolTip = "";
        private string _detailTARGET = "_self";
        private string _editURL = "Edit.aspx?id=%ID";
        private string _editTARGET = "_self";
        private string _rowIdScope = "R";
        private int _rowsPerPage = 0;
        private int _actualPage = 0;
        private bool _checked = false;
        private bool _radioButtons = false;
        private string _radioButtonsGroupName = "LIST_BUILDER";
        private bool _rightsEnable = false;
        private LanguageMapper _map;
        private DBData _db;
        private bool _checkOrder = false;
        private string _viewName = "";
        private bool _useFirstLetterAsPageSelector = false;
        private ArrayList _pageSelector = null;
        private long _highlightRecordID = -1;
		
        private bool _firstColumn = true;
        private TableRow _headerRow;
        private TableRow _currentRow;
        private TableCell _infoBoxCell = null;
        private StringBuilder _infoBoxParam = new StringBuilder();
        private int _rowAuthorisations;
        private int _rowNumber = -1;
                
        // Allow compability with version 2.0
		private bool _useJavaScriptToSort = false;
        /// <summary>
        /// Add cell event. Wird nach add cell gerufen
        /// </summary>
        public event AddCellHandler addCell = null;
        /// <summary>
        /// Add cell event. Wird nach add header cell gerufen
        /// </summary>
        public event AddCellHandler addHeaderCell = null;
        /// <summary>
        /// After add cells event. Wird nach alle Datenzellen gerufen.
        /// </summary>
        public event AddRowHandler afterAddCells = null;
        /// <summary>
        /// Add row event. Wird nach add row gerufen
        /// </summary>
        public event AddRowHandler addRow = null;
        /// <summary>
        /// Set check columnorder in select
        /// </summary>
        public bool checkOrder {
            get { return _checkOrder; }
            set { _checkOrder = value; }
        }
        /// <summary>
        /// Get/set info image
        /// </summary>
        public string infoImage {
            get { return _infoImage; }
            set { _infoImage = value; }
        }
        /// <summary>
        /// Bildet DB-table als Web-liste ab. Row.ID = R+dbid
        /// </summary>
        /// <param name="db">DB context</param>
        /// <param name="table">DB Tabelle</param>
        /// <param name="listTab">Web tabelle</param>
        /// <param name="map">Sprach mapper</param>
        /// <param name="view">View</param>
        /// <returns>#Zeilen in der Liste</returns>
        public int load(DBData db, DataTable table, Table listTab, LanguageMapper map, params string[] param) {
            string view = param.Length > 0 ? param[0] : "";
            int rows = 0;
            
            if (_orderColumn != null && DBColumn.GetBaseType(_orderColumn) == DBColumn.InputDataType.Date){
                // don't use first letters as page-selector for Dates.
                _useFirstLetterAsPageSelector = false;
            }
            _pageSelector = new ArrayList();
            _listTable = listTab;
            _map = map;
            _db = db;
            _viewName = view == "" ? table.TableName : view;
            _firstColumn = true;
            _rowNumber = -1;
            _actualPage = 0;

            if (_headerEnable) {
                _headerRow = new TableRow();               
                _headerRow.CssClass = "ListHeader";
                listTab.Rows.Add(_headerRow);
                _db.scanColumn += new ScanActionColumnHandler(this.scanColumn);
                _db.scanTableColumn(table,_viewName,(int) SQLColumn.Visibility.LIST,DBData.AUTHORISATION.READ,_checkOrder);
                _db.scanColumn -= new ScanActionColumnHandler(this.scanColumn);
            }
            _db.scanRow += new ScanActionRowHandler(this.scanRow);
            _db.scanCell += new ScanActionCellHandler(this.scanCell);
            rows = _db.scanTableData(table,_viewName,(int) SQLColumn.Visibility.LIST+(int)SQLColumn.Visibility.INFO,DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON,_checkOrder, _editEnable || _deleteEnable || _rightsEnable);
            _db.scanRow -= new ScanActionRowHandler(this.scanRow);
            _db.scanCell -= new ScanActionCellHandler(this.scanCell);

            // make only the actual page visible
            if (_rowNumber > 0) {
                for (int i=_actualPage*_rowsPerPage; i<(_actualPage+1)*_rowsPerPage; i++) {


                    listTab.Rows[Math.Min(i+(_headerEnable?1:0),listTab.Rows.Count-1)].Style.Remove("display");
                }
            }
            else{
                listTab.Rows.Clear();
                TableRow r = new TableRow(); 
                TableCell c = new TableCell(); 
                listTab.Rows.Add(r);
                r.Cells.Add(c);
                c.CssClass = "List";
                c.Text = map.get("noEntriesFound");
            }

            // page selector
            if (_rowsPerPage > 0 && rows > _rowsPerPage) 
            {
				string _lTabId = listTab.ClientID;
				
				TableRow r = new TableRow();
                listTab.Rows.Add(r);
                TableCell c = new TableCell();
                r.Cells.Add(c);
                c.ColumnSpan = listTab.Rows[listTab.Rows.Count - 2].Cells.Count;
                HyperLink l = new HyperLink();
                l.Text = "< ";
                l.ID = "actualPagePrev";
                l.NavigateUrl = "javascript: showPreviousPage(" + _lTabId + "," + _rowsPerPage + "," + (_headerEnable ? "true" : "false") + ");";
                if (_actualPage == 0)
                    l.Attributes.Add("disabled", "true");
                c.Controls.Add(l);
                for (int i=0; i<=(rows-1)/_rowsPerPage; i++) 
                {
                    l = new HyperLink();
                    l.Text = " " + (_useFirstLetterAsPageSelector && _pageSelector.Count>i? _pageSelector[i] : ((int)i+1).ToString()) + " ";
                    l.ID = "actualPage" + i + "_";
                    l.NavigateUrl = "javascript: showActualPage(" + _lTabId + "," + i + "," + _rowsPerPage + "," + (_headerEnable ? "true" : "false") + ");";
                    l.Attributes.Add("hrefOrig", l.NavigateUrl);
                    if (_actualPage == i) 
                    {
                        l.CssClass = "selected";
                        l.NavigateUrl = "";
                    }
                    c.Controls.Add(l);
                }
                l = new HyperLink();
                l.Text = " >";
                l.ID = "actualPageNext";
                l.NavigateUrl = "javascript: showNextPage(" + _lTabId + "," + _rowsPerPage + "," + (_headerEnable ? "true" : "false") + ");";
                if (_actualPage == (rows-1)/_rowsPerPage)
                    l.Attributes.Add("disabled", "true");
                c.Controls.Add(l);
            }

            return rows;
        }        
        
        private bool scanRow(DataTable table, DataRow row, int rowNumber, int rowAuthorisations) {
            if (!rowAccess(table,row,true,DBData.AUTHORISATION.READ)) return false;
            _currentRow = new TableRow();
            _rowNumber = rowNumber;
            _listTable.Rows.Add(_currentRow);
            _currentRow.CssClass = _listTable.Rows.Count%2 == 1 ? "ListEven" : "ListOdd";
            _currentRow.Style.Add("display", "none");
            _currentRow.ID = this._rowIdScope + rowNumber + "_" + (_rowsPerPage > 0? (rowNumber-1)/_rowsPerPage : 0) + "_";
            if (_idColumn != null)
            {
                long recordID = (long)row[_idColumn];
                _currentRow.ID += recordID;
                if (_highlightRecordID > 0 && recordID == _highlightRecordID)
                {
                    _currentRow.CssClass += "_selected";
                    _actualPage = (rowNumber-1) / _rowsPerPage;
                }
            }
            _infoBoxCell = null;
            _infoBoxParam.Length = 0;
            _firstColumn = true;
            _rowAuthorisations = rowAuthorisations;
            return true;
        }
        private void scanColumn(DataTable table, DataColumn column, int columnNumber) {
            TableCell c;
            HyperLink url;
            int w,h;
            
            if (columnNumber > 0) {
                if (_firstColumn) {
                    if (_checkBoxEnable) {
                        c = new TableHeaderCell();
                        _headerRow.Cells.Add(c);
                        this.onAddHeaderCell(null,null,_headerRow,c);
                    }
                    if (_infoBoxEnable) {
                        c = new TableHeaderCell();
                        _headerRow.Cells.Add(c);
                        this.onAddHeaderCell(null,null,_headerRow,c);
                    }
                }
                c = new TableHeaderCell();
                c.CssClass = "ListHeader";
                url = new HyperLink();
                c.Controls.Add(url);
                _map.getSize(_viewName,column.ColumnName,out w, out h);
                //if (w >= 0) c.Width = w;
                _headerRow.Cells.Add(c);
                url.CssClass = "ListHeader";
                url.Text = _map.get(_viewName,column.ColumnName);
				if (_useJavaScriptToSort)
				{
					url.NavigateUrl = "javascript: sortTable('" + _listTable.ClientID + "', " + Convert.ToString(_headerRow.Cells.Count - 1) + ", " + _rowsPerPage + ")";
				}
				else
				{
					if (_sortURL != "") 
					{
						url.NavigateUrl = _sortURL;
						if (_sortURL.IndexOf('?') >= 0)
							url.NavigateUrl += "&";
						else
							url.NavigateUrl += "?";
						url.NavigateUrl += "orderColumn=" + column.ColumnName + "&orderDir=";
						if (column == _orderColumn) 
						{
							if (_orderDir == ORDER_DIR.ASC) 
							{
								url.NavigateUrl += "desc";
								url.Text = (char) 0x25b2 + url.Text;
							}
							else 
							{
								url.NavigateUrl += "asc";
								url.Text = (char) 0x25bc + url.Text;
							}
						}
						else
							url.NavigateUrl += "asc";
					}
				}
                _firstColumn = false;
                this.onAddHeaderCell(null,column,_headerRow,c);
            }
        }
        private void scanCell(DataTable table, DataRow row, int rowNumber, DataColumn column, int columnNumber) {
            TableCell c;
            HyperLink url;
            int w,h;
            int visi = (int) column.ExtendedProperties["Visibility"];
            
            if (columnNumber > 0) {

                if ((visi & (int) SQLColumn.Visibility.INFO) > 0) {
                    _infoBoxParam.Append(_map.get(_viewName,column.ColumnName)).Append(":=");
                    _infoBoxParam.Append(HttpUtility.UrlEncode((column.ExtendedProperties["In"] == null) ? _db.GetDisplayValue(column,row[column],false) : DBColumn.LookupIn(column, row[column], false)));
                    _infoBoxParam.Append(HttpUtility.UrlEncode((column.ExtendedProperties["Unit"] == null) ? "" : " " + column.ExtendedProperties["Unit"]));
                    _infoBoxParam.Append("&");
                }                       

                if ((visi & (int) SQLColumn.Visibility.LIST) > 0) { 
                    string cellText = (column.ExtendedProperties["In"] == null) ? _db.GetDisplayValue(column,row[column],true) : DBColumn.LookupIn(column, row[column], true);
                    
                    c = new TableCell();
                    _currentRow.Cells.Add(c);
                    if (_firstColumn) {
                        if (_checkBoxEnable) {
                            if (_radioButtons) {
                                RadioButton rb = new RadioButton();
                    
                                rb.GroupName = _radioButtonsGroupName;
                                if (idColumn != null) rb.ID = _radioButtonsGroupName + row[idColumn];
                    
                                if (rowNumber == 1)  rb.Checked = _checked;
                                c.Controls.Add(rb);
                            }
                            else {
                                CheckBox cb = new CheckBox();
                                if (idColumn != null) cb.ID = rowNumber.ToString() + "_" + row[idColumn];
                    
                                cb.Checked = _checked;
                                c.Controls.Add(cb);
                            }
                            // add CSS class for checkbox / 02.02.10 / mkr
                            c.CssClass = "CheckBox";
                            c = new TableCell();
                            _currentRow.Cells.Add(c); 
                        }
                        _infoBoxCell = c;
                        if (_infoBoxEnable) {
                            c = new TableCell();
                            _currentRow.Cells.Add(c); 
                        }   
                        c.Text = cellText;
                    }
                    else {
                        c.Text = cellText;
                    }
                    if (!_headerEnable && _listTable.Rows.Count == 1) 
                    {
                        _map.getSize(_viewName,column.ColumnName,out w,out h);
                        if (w >= 0) c.Width = w;
                    }
                    // now link if element documnet 2013 04 02 MSr
                    //if (_detailEnable && ( (_firstColumn && _detailColumn == null) || (column == _detailColumn) ) && !row[0].ToString().Equals("DOCUMENT") )
                        if (_detailEnable && ((_firstColumn && _detailColumn == null) || (column == _detailColumn))) 
                    {
                        string tmpStr = _detailURL;
                    
                        url = new HyperLink();                               
                        c.Controls.Add(url);
                        tmpStr = replaceURLPlaceholders(tmpStr, row);
						tmpStr = HttpUtility.UrlEncode(tmpStr);

                        url.NavigateUrl = "javascript: highlightElement('"+_detailTARGET+"','"+tmpStr+"','"+_currentRow.ClientID+"')";
                        url.CssClass = _currentRow.CssClass;
                        url.Text = c.Text;
                        if (this._detailToolTip != "") url.ToolTip = this._detailToolTip;
                    }
                    else {
                        Type controlType = (Type) column.ExtendedProperties["ListControlType"];
                        if (controlType != null) {
                            string text = c.Text;
                            WebControl ctrl = (WebControl) controlType.GetConstructor(Type.EmptyTypes).Invoke(null);

                            c.Text = null;
                            if (ctrl is Label) (ctrl as Label).Text = text;
                            else if (ctrl is EMailLink) {
                                (ctrl as EMailLink).Text = text;
                                (ctrl as EMailLink).NavigateUrl = text;
                            }
                            else if (ctrl is HyperLink) {
                                (ctrl as HyperLink).Text = text;
                                (ctrl as HyperLink).NavigateUrl = text;
                            }
                            else ctrl = null;
                            if (ctrl != null) c.Controls.Add(ctrl);
                            else c.Text = text;
                        }
                        else{
                            string contextLink = column.ExtendedProperties["ContextLink"] as string;
                            if (contextLink != null && contextLink != ""){
                                contextLink = replaceURLPlaceholders(contextLink, row);
								contextLink = HttpUtility.UrlEncode(contextLink);
								url = new HyperLink();                               
                                c.Controls.Add(url);

                                url.NavigateUrl = "javascript: highlightElement('"+column.ExtendedProperties["ContextLinkTarget"]+"','"+contextLink+"','"+_currentRow.ClientID+"')";
                                url.CssClass = _currentRow.CssClass;
                                url.Text = c.Text;
                            }
                        }
                    }
                    if (_useFirstLetterAsPageSelector && column == _orderColumn && rowNumber % _rowsPerPage == 1)
                    {
                        string decCellText = HttpUtility.HtmlDecode(cellText);
                        _pageSelector.Add(decCellText.Length > 0 ? decCellText.Substring(0,Math.Min(decCellText.Length, 2)) : "-");
                    }
                    c.CssClass = _currentRow.CssClass;
                    _firstColumn = false;
                    this.onAddCell(row,column,_currentRow,c);
                }
            } // columnNumber > 0
            bool edit = _editEnable;
            bool delete = _deleteEnable;
            if (Math.Abs(columnNumber) == table.Columns.Count && !_firstColumn) {
                onAfterAddCells(row,_currentRow);

                // edit
                if (_editEnable)
                {
                    // HACK: always allow edit on objectives
                    if (rowAccess(table,row,
                        _db.hasAuthorisation(DBData.AUTHORISATION.UPDATE, _rowAuthorisations),
                        DBData.AUTHORISATION.UPDATE) || table.TableName == "OBJECTIVEV") 
                    {
                        string tmpStr = _editURL;
                        c = new TableCell();
                        _currentRow.Cells.Add(c);
                        url = new HyperLink();
                        url.CssClass = _currentRow.CssClass;
                        c.Controls.Add(url);
                        c.ID = EDIT_CELL_ID+rowNumber;
                        c.EnableViewState = false;
                        tmpStr = replaceURLPlaceholders(tmpStr, row);
						tmpStr = HttpUtility.UrlEncode(tmpStr);
                        url.NavigateUrl = "javascript: highlightElement('"+_editTARGET+"','"+tmpStr+"','"+_currentRow.ClientID+"')";
                        url.Text = "E";
                        url.ToolTip = _map.get("edit");
                        this.onAddCell(row,null,_currentRow,c);
                    }
                    else {
                        _currentRow.Cells.Add(new TableCell());
                        edit = false;
                    }
                }
                // delete
                if (_deleteEnable)
                {
                    // HACK: always allow delete on objectives
                    if (rowAccess(table,row,
                        _db.hasAuthorisation(DBData.AUTHORISATION.DELETE, _rowAuthorisations),
                        DBData.AUTHORISATION.DELETE) || table.TableName == "OBJECTIVEV")
                    {
                        c = new TableCell();
                        _currentRow.Cells.Add(c);
                        if (edit) c.Text = "|";
                        c = new TableCell();
                        url = new HyperLink();
                        url.CssClass = _currentRow.CssClass;
                        c.Controls.Add(url);
                        c.EnableViewState = false;
                        c.ID = DELETE_CELL_ID+rowNumber;
                        _currentRow.Cells.Add(c);
                        url.NavigateUrl = "javascript: listDeleteRowConfirm('"+_currentRow.ClientID+"','"+row[_idColumn]+"','" + table.TableName + "')";
                        url.Text = "D";
                        url.ToolTip = _map.get("delete");
                        this.onAddCell(row,null,_currentRow,c);
                    }
                    else 
                    {
                        _currentRow.Cells.Add(new TableCell());
                        delete = false;
                    }
                }

                // access-rights
                if (_rightsEnable) {
                    if (rowAccess(table,row,
                        _db.hasAuthorisation(DBData.AUTHORISATION.ADMIN, _rowAuthorisations),
                        DBData.AUTHORISATION.ADMIN))
                    {
                        if (edit || delete){
                            c = new TableCell();
                            c.Text = "|";
                            _currentRow.Cells.Add(c);
                        }
                        c = new TableCell();
                        url = new HyperLink();
                        url.CssClass = _currentRow.CssClass;
                        c.Controls.Add(url);
                        c.EnableViewState = false;
                        _currentRow.Cells.Add(c);
                        url.NavigateUrl = "javascript: highlightElement('_self','javascript: openPopupWindow(\"" + Global.Config.baseURL + "/Common/Authorisations.aspx?tableName=" + table.TableName + "&rowID=" + row[_idColumn] + "\",\"400\",\"420\");','"+_currentRow.ClientID+"')";
                        url.Text = "R";
                        url.ToolTip = _map.get("authorisation","authorisationsButton");
                        this.onAddCell(row,null,_currentRow,c);
                    }
                    else{
                        _currentRow.Cells.Add(new TableCell());
                    }
                }

                if (_infoBoxParam.Length > 0 && _infoBoxEnable) {
                    /*
                    StringBuilder link = new StringBuilder();
                    
                    link.Append("<img src=\"" + Global.Config.baseURL + "/images/"+_infoImage+"\" ");
                    link.Append("onmouseout=\"erasePropertyBox()\" onmouseover=\"drawPropertyBox('");
                    // double conversion to javascript because the parameter is used twice...
                    link.Append(PSOFTConvert.ToJavascript(PSOFTConvert.ToJavascript(_infoBoxParam.ToString(0,_infoBoxParam.Length-1)))).Append("')\">");
                    _infoBoxCell.Text = link.ToString();
                    */
                    _infoBoxCell.HorizontalAlign = HorizontalAlign.Center;
                    _infoBoxCell.CssClass = "InfoBox";
                    Image image = new Image();
                    image.ImageUrl = Global.Config.baseURL + "/images/"+_infoImage;
                    PropertyBox.addPropertyHandler(image,PSOFTConvert.ToJavascript(_infoBoxParam.ToString(0,_infoBoxParam.Length-1)));
                    _infoBoxCell.Controls.Add(image);

                    _infoBoxCell.ID = INFO_CELL_ID+rowNumber;
                    this.onAddCell(row,null,_currentRow,_infoBoxCell);
                }
                onAddRow(row,_currentRow);
            }
        }

        public static bool IsEditCell(TableCell cell) {
            if (cell == null){
                return false;
            }
            string cellID = cell.ID;
            if (cellID == null){
                return false;
            }
            return cellID.StartsWith(EDIT_CELL_ID);
        }

        public static bool IsDeleteCell(TableCell cell) {
            if (cell == null){
                return false;
            }
            string cellID = cell.ID;
            if (cellID == null){
                return false;
            }
            return cellID.StartsWith(DELETE_CELL_ID);
        }

        public static bool IsInfoCell(TableCell cell) {
            if (cell == null || cell.ID == null) return false;
            return cell.ID.StartsWith(INFO_CELL_ID);
        }

        public static void ReplaceInfoImage(TableCell cell,string newImage) {
            cell.Text = cell.Text.Replace("info.gif",newImage);
        }

        public static long IdColumnValue(TableRow row) {
            int idx = row.ID.LastIndexOf("_")+1;
            long id = 0;
            if (idx > 0 && row.ID.Length-idx > 0) id = Validate.GetValid(row.ID.Substring(idx,row.ID.Length-idx),0L);
            return id;
        }



        private string replaceURLPlaceholders(string url, DataRow row)
        {
            foreach (DataColumn col in row.Table.Columns)
            {
                url = url.Replace("%25"+col.ColumnName, "%"+col.ColumnName).Replace("%"+col.ColumnName, row[col].ToString());
            }

            return url;
        }
        /// <summary>
        /// set/get Kolonne der ID. Zwingend falls detail, edit, delete enable
        /// </summary>
        public DataColumn idColumn { 
            get { return _idColumn ; } 
            set { _idColumn = value; }
        }
        /// <summary>
        /// set/get Kolonne des Detaillinks. Optional.
        /// </summary>
        public DataColumn detailColumn { 
            get { return _detailColumn; } 
            set { _detailColumn = value; }
        }
        /// <summary>
        /// set/get Kolonne nach welcher die Daten sortiert sind. Optional.
        /// </summary>
        public DataColumn orderColumn { 
            get { return _orderColumn; } 
            set { _orderColumn = value; }
        }
        /// <summary>
        /// set/get die Richtung der Sortierung der Daten.
        /// "asc"  : aufsteigend
        /// "desc" : absteigend
        /// </summary>
        public string orderDir {
            get { return _orderDir == ORDER_DIR.ASC? "asc" : "desc"; }
            set { _orderDir = value == "asc"? ORDER_DIR.ASC : ORDER_DIR.DESC; }
        }
        /// <summary>
        /// set/get mit Tabellenkopf (default: true)
        /// </summary>
        public bool headerEnable { 
            get { return  _headerEnable; } 
            set { _headerEnable = value; }
        }
        /// <summary>
        /// set/get mit Infobox (default: true)
        /// </summary>
        public bool infoBoxEnable { 
            get { return  _infoBoxEnable; }
            set {_infoBoxEnable  = value; }
        }
        /// <summary>
        /// set/get mit Checkbox (default: false)
        /// </summary>
        public bool checkBoxEnable { 
            get { return  _checkBoxEnable; }
            set {_checkBoxEnable  = value; }
        }
        /// <summary>
        /// set/get für checked CheckBox (default: false)
        /// </summary>
        public bool checkBoxChecked { 
            get { return  _checked; }
            set {_checked  = value; }
        }
        /// <summary>
        /// set/get URL fuer sortierung (default: List.aspx)
        /// </summary>
        public string sortURL {
            get { return _sortURL ; }
            set {_sortURL  = value; }
        }
        /// <summary>
        /// set/get mit loeschen (default: false)
        /// </summary>
        public bool deleteEnable { 
            get { return _deleteEnable ; }
            set { _deleteEnable = value; }
        }
        /// <summary>
        /// set/get mit editieren (default: false)
        /// </summary>
        public bool editEnable {
            get { return _editEnable ; } 
            set { _editEnable = value; }
        }
        /// <summary>
        /// set/get mit detaillink
        /// </summary>
        public bool detailEnable {
            get { return _detailEnable ; } 
            set { _detailEnable = value; }
        }
        /// <summary>
        /// set/get detail URL (default: Detail.aspx)
        /// </summary>
        public string detailURL {
            get { return _detailURL ; } 
            set { _detailURL = value; }
        }

        /// <summary>
        /// set/get detail URL TARGET (default: _self)
        /// </summary>
        public string detailTARGET {
            get { return _detailTARGET ; } 
            set { _detailTARGET = value; }
        }

        /// <summary>
        /// set/get edit URL (default: Edit.aspx)
        /// </summary>
        public string editURL {
            get { return _editURL ; } 
            set { _editURL = value; }
        }

        /// <summary>
        /// set/get edit URL TARGET (default: _self)
        /// </summary>
        public string editTARGET {
            get { return _editTARGET ; } 
            set { _editTARGET = value; }
        }
        /// <summary>
        /// get/set scope for rowId
        /// </summary>
        public string rowIdScope {
            get { return _rowIdScope ; } 
            set { _rowIdScope = value; }
        }
        /// <summary>
        /// get/set tooltip for detaillink
        /// </summary>
        public string detailToolTip {
            get { return _detailToolTip ; } 
            set { _detailToolTip = value; }
        }
        
        /// <summary>
        /// Number of rows to display per page.
        /// If 0, only one page will be displayed. 
        /// </summary>
        public int rowsPerPage {
            get { return _rowsPerPage; }
            set { _rowsPerPage = value; }
        }

        /// <summary>
        /// Property access methode for Radio Button ja/nein
        /// false: Checkbox (Default)
        /// true:  Radio-Buttons
        /// </summary>
        public bool radioButtons {
            get{ return _radioButtons; }
            set {_radioButtons = value;}
        }
        
        /// <summary>
        /// Property access methode for Gruppenname der Radio Buttons
        /// Default: "LIST_BUILDER"
        /// </summary>
        public string radioButtonsGroupName {
            get{ return _radioButtonsGroupName; }
            set {_radioButtonsGroupName = value;}
        }
        
        /// <summary>
        /// set/get to enable access-rights link
        /// false: (default) no access-rights link
        /// true:  access-rights link visible
        /// </summary>
        public bool rightsEnable {
            get{ return _rightsEnable; }
            set {_rightsEnable = value;}
        }
        
        /// <summary>
        /// set/get to define the format of the page-selector link
        /// false: page-number will be used
        /// true:  first letter of the sorted column will be used
        /// </summary>
        public bool useFirstLetterAsPageSelector 
        {
            get { return _useFirstLetterAsPageSelector; }
            set { _useFirstLetterAsPageSelector = value; }
        }

		/// <summary>
		/// set/get to define what kind of sort alghoritm will be
		/// use to sort data table.
		/// false : functionality from version 2.0 - sort by URL,
		/// true  : sort by JavaScript methods, 3.0 new feature.
		/// By default is set to true.
		/// </summary>
		public bool UseJavaScriptToSort
		{
			get {return _useJavaScriptToSort;}
			set {_useJavaScriptToSort = value;}
		}

        /// <summary>
        /// set/get the row-ID (of ID-Column) of the record to highlight.
        /// </summary>
        public long highlightRecordID
        {
            get {return _highlightRecordID;}
            set {_highlightRecordID = value;}
        }

        /// <summary>
        /// set/get the number of the page to display.
        /// </summary>
        public int actualPage
        {
            get {return _actualPage;}
            set {_actualPage = value;}
        }

        private void onAddCell (DataRow row, DataColumn col, TableRow r, TableCell cell) 
        {
            if (addCell != null) {
                // Invokes the delegates. 
                addCell(row,col,r,cell);
            }
        }
        private void onAddHeaderCell (DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (addHeaderCell != null) {
                // Invokes the delegates. 
                addHeaderCell(row,col,r,cell);
            }
        }
        private void onAfterAddCells (DataRow row, TableRow r) {
            if (afterAddCells != null) {
                // Invokes the delegates. 
                afterAddCells(row,r);
            }
        }
        private void onAddRow (DataRow row, TableRow r) {
            if (addRow != null) {
                // Invokes the delegates. 
                addRow(row,r);
            }
        }
        public static string getID(TableRow row)
        {
            string retValue = "";
            if (row.ID != null)
            {
                string [] splits = row.ID.Split('_');
                if (splits.Length > 2)
                    retValue = splits[2];
            }
            return retValue;
        }
        public static int getPage(TableRow row)
        {
            int retValue = -1;
            if (row.ID != null)
            {
                string [] splits = row.ID.Split('_');
                if (splits.Length > 1)
                    retValue = int.Parse(splits[1]);
            }
            return retValue;
        }
        public static bool isChecked(TableRow row)
        {
            bool retValue = false;

            if (row.Cells.Count > 0 && row.Cells[0].Controls.Count > 0) 
            {
                CheckBox cb = row.Cells[0].Controls[0] as CheckBox;
                if (cb != null)
                    retValue = cb.Checked;
            }
            return retValue;
        }
        public static void setChecked(TableRow row, bool isChecked)
        {
            if (row.Cells.Count > 0 && row.Cells[0].Controls.Count > 0) 
            {
                CheckBox cb = row.Cells[0].Controls[0] as CheckBox;
                if (cb != null)
                    cb.Checked = isChecked;
            }
        }
    }
}
