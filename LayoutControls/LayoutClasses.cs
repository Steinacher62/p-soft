using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.LayoutControls
{
    public delegate void ExceptionEventHandler(object sender, Exception exception);

    /// <summary>
    /// This is a base class for all user controls.
    /// </summary>
    public class PSOFTUserControl : System.Web.UI.UserControl {
        public const string PARAM_VIEW_STATE_STORE = "PARAM_VIEW_STATE_STORE";
        public const string PARAM_EXECUTE_ACTION = "PARAM_EXECUTE_ACTION";


        public event ExceptionEventHandler OnException;

        protected Hashtable _parameters = new Hashtable();

        public PSOFTUserControl() : base() {}

        public void Execute() {
            try {
                DoExecute();
            }
            catch (Exception except) {
                DoOnException(except);
            }
        }

        protected virtual void DoExecute() {
        }

        protected void DoOnException(Exception except) {
            Logger.Log(except, Logger.ERROR);
            if (OnException != null)
                OnException(this, except);
            else
                throw new Exception("DoOnException. Unhandled exception occured in " + this.GetType().Name + ". " + except.Message, except);
        }

        public PSOFTUserControl LoadPSOFTControl(string path, string id) {
            return (PSOFTUserControl) LoadControl(path);
        }

        #region Protected method overrided from paren class
        protected override void LoadViewState(object savedState) {
            if (savedState != null) {
                base.LoadViewState(savedState);
                if (ViewState[PARAM_VIEW_STATE_STORE] != null)
                    _parameters = (Hashtable)ViewState[PARAM_VIEW_STATE_STORE];
            }
        }

        protected override object SaveViewState() {
            ViewState[PARAM_VIEW_STATE_STORE] = _parameters;
            return base.SaveViewState();
        }
		#endregion

		#region Public methods to param maintance
        public object GetParam(string paramName) {
            if (_parameters.ContainsKey(paramName))
                return _parameters[paramName];
            return null;
        }

        public void SetParam(string paramName, object value) {
            if (_parameters.ContainsKey(paramName))
                _parameters[paramName] = value;
            else
                _parameters.Add(paramName, value);
        }

        public void SetParams(Hashtable aParams) {
            foreach (string _key in aParams.Keys)
                SetParam(_key, aParams[_key]);
        }

        public string GetString(string paramName) {
            return Convert.ToString(GetParam(paramName));
        }

        public bool GetBool(string paramName) {
            return Convert.ToBoolean(GetParam(paramName));
        }

        public int GetInt(string paramName) {
            return Convert.ToInt32(GetParam(paramName));
        }

        public long GetLong(string paramName) {
            return Convert.ToInt64(GetParam(paramName));
        }

        public long GetID(string paramName) {
            try {
                return Convert.ToInt64(GetParam(paramName));
            }
            catch {}
            return 0;
        }

        public double GetDouble(string paramName) {
            paramName.Replace(",", ".");
            return Convert.ToDouble(GetParam(paramName));
        }

        public System.DateTime GetDataTime(string paramName) {
            return Convert.ToDateTime(GetParam(paramName));
        }
		#endregion
    }

    /// <summary>
    /// This is a user controls with LanguageMapper inside
    /// </summary>
    public class PSOFTMapperUserControl : PSOFTUserControl {
        protected LanguageMapper _mapper = null;

        public PSOFTMapperUserControl() : base() {}

        protected override void OnLoad(System.EventArgs e) {
            _mapper = LanguageMapper.getLanguageMapper(this.Session);
            base.OnLoad(e);
        }
    }

    public class SearchEventArgs {
        private string _searchSQL = "";
        private bool _reloadList = false;
        private string _customData = "";

        public SearchEventArgs(){}

        public string SearchSQL {
            get {return _searchSQL;}
            set {_searchSQL = value;}
        }

        public bool ReloadList {
            get {return _reloadList;}
            set {_reloadList = value;}
        }

        public string CustomData {
            get {return _customData;}
            set {_customData = value;}
        }
    }

    public delegate void SearchClickHandler(object Sender , SearchEventArgs e);

    public class PSOFTSearchUserControl : PSOFTInputViewUserControl {
        public const string PARAM_SHOWRELATION      = "PARAM_SHOWRELATION";
        public event SearchClickHandler OnSearchClick;

        protected SearchEventArgs _searchArgs = new SearchEventArgs();

        public PSOFTSearchUserControl() : base() {
            InputType = InputMaskBuilder.InputType.Search;
            ShowRelation = false;
        }


        public bool ShowRelation {
            get {return GetBool(PARAM_SHOWRELATION);}
            set {SetParam(PARAM_SHOWRELATION, value);}
        }

        protected void DoOnSearchClick(object Sender) {
            DoOnSearchClick(Sender, _searchArgs);
        }
	
        protected void DoOnSearchClick(object Sender, SearchEventArgs e) {
            if (OnSearchClick != null)
                OnSearchClick(Sender, e);
        }
        protected override void LoadInput(DBData db, DataTable dataTable, Table inputTable) {

            _inputBuilder.ShowRelation = ShowRelation;
            base.LoadInput(db,dataTable,inputTable);
        }

    }

    public class ListEventArgs {
        private Table _list = null;
        private bool _startLoad = false;
        private bool _endLoad = false;

        public ListEventArgs() {}
        public ListEventArgs(Table list, bool start, bool end) {
            _list = list;
            _startLoad = start;
            _endLoad = end;
        }

        public Table List {
            get {return _list;}
            set {_list = value;}
        }

        public bool StartLoad {
            get {return _startLoad;}
            set {_startLoad = value;}
        }

        public bool EndLoad {
            get {return _endLoad;}
            set {_endLoad = value;}
        }
    }
    public delegate void LoadListHandler(object Sender , ListEventArgs e);

    /// <summary>
    /// This is a base user control class for all user controls with list view.
    /// This class has two properties:
    ///   OrderColumn,
    ///   OrderDir
    /// Other needed properties should be also added.
    /// </summary>
    public class PSOFTListViewUserControl : PSOFTMapperUserControl {
        public const string PARAM_ORDER_COLUMN      = "PARAM_ORDER_COLUMN";
        public const string PARAM_ORDER_DIR         = "PARAM_ORDER_DIR";
        public const string PARAM_SORT_URL          = "PARAM_SORT_URL";
        public const string PARAM_EDIT_URL          = "PARAM_EDIT_URL";
        public const string PARAM_DETAIL_URL        = "PARAM_DETAIL_URL";
        public const string PARAM_HEADER_ENABLED    = "PARAM_HEADER_ENABLED";
        public const string PARAM_DELETE_ENABLED    = "PARAM_DELETE_ENABLED";
        public const string PARAM_EDIT_ENABLED      = "PARAM_EDIT_ENABLED";
        public const string PARAM_DETAIL_ENABLED    = "PARAM_DETAIL_ENABLED";
        public const string PARAM_RIGHTS_ENABLED    = "PARAM_RIGHTS_ENABLED";
        public const string PARAM_INFOBOX_ENABLED   = "PARAM_INFOBOX_ENABLED";
        public const string PARAM_ID_COLUMN         = "PARAM_ID_COLUMN";
        public const string PARAM_HIGLIGHTRECORD_ID = "PARAM_HIGLIGHTRECORD_ID";
        public const string PARAM_USE_FIRST_LETTER  = "PARAM_USE_FIRST_LETTER";
        public const string PARAM_USE_SCRIPT_SORT   = "PARAM_USE_SCRIPT_SORT";
        public const string PARAM_VIEW              = "PARAM_VIEW";
        public const string PARAM_CSS_CLASS         = "PARAM_CSS_CLASS";
        public const string PARAM_CELL_PADDING      = "PARAM_CELL_PADDING";
        public const string PARAM_CELL_SPACING      = "PARAM_CELL_SPACING";
        public const string PARAM_CHECK_ORDER       = "PARAM_CHECK_ORDER";
        public const string PARAM_DELETE_MESSAGE    = "PARAM_DELETE_MESSAGE";
        public const string PARAM_ROWS_PER_PAGE     = "PARAM_ROWS_PER_PAGE";

        protected ListBuilder _listBuilder = null;
        public event LoadListHandler OnLoadList;

        public PSOFTListViewUserControl() : base() {
            _listBuilder = new ListBuilder();
            OrderDir = _listBuilder.orderDir;
            HeaderEnabled = _listBuilder.headerEnable;
            DeleteEnabled = _listBuilder.deleteEnable;
            DetailEnabled = _listBuilder.detailEnable;
            RightsEnabled = _listBuilder.rightsEnable;
            EditEnabled = _listBuilder.editEnable;
            InfoBoxEnabled = _listBuilder.infoBoxEnable;
            HighlightRecordID = _listBuilder.highlightRecordID;
            UseFirstLetterAsPageSelector = _listBuilder.useFirstLetterAsPageSelector;
            UseJavaScriptToSort = _listBuilder.UseJavaScriptToSort; 
            CheckOrder = _listBuilder.checkOrder;
            CellPadding = 0;
            CellSpacing = 2;
            CssClass = "List";
            IDColumn = "ID";
        }

        protected override void OnLoad(System.EventArgs e) {
            base.OnLoad(e);
            if (deleteMessage == ""){
                deleteMessage = PSOFTConvert.ToJavascript(_mapper.get("MESSAGES", "deleteConfirm"));
            }
        }
		#region Properities
        /// <summary>
        /// Confirmation message displayed before deletion
        /// </summary>
        public virtual string deleteMessage {
            get {return GetString(PARAM_DELETE_MESSAGE);}
            set {SetParam(PARAM_DELETE_MESSAGE, value);}
        }

        /// <summary>
        /// get/set name of ordered column 
        /// </summary>
        public string OrderColumn {
            get {return GetString(PARAM_ORDER_COLUMN);}
            set {SetParam(PARAM_ORDER_COLUMN, value);}
        }

        /// <summary>
        /// get/set direction of order (asc, desc)
        /// </summary>
        public string OrderDir {
            get {return GetString(PARAM_ORDER_DIR);}
            set {SetParam(PARAM_ORDER_DIR, value);}
        }

        /// <summary>
        /// get/set true if 
        /// </summary>
        public bool HeaderEnabled {
            get {return GetBool(PARAM_HEADER_ENABLED);}
            set {SetParam(PARAM_HEADER_ENABLED, value);}
        }

        public bool DeleteEnabled {
            get {return GetBool(PARAM_DELETE_ENABLED);}
            set {SetParam(PARAM_DELETE_ENABLED, value);}
        }

        public bool EditEnabled {
            get {return GetBool(PARAM_EDIT_ENABLED);}
            set {SetParam(PARAM_EDIT_ENABLED, value);}
        }

        public bool DetailEnabled {
            get {return GetBool(PARAM_DETAIL_ENABLED);}
            set {SetParam(PARAM_DETAIL_ENABLED, value);}
        }

        public bool RightsEnabled {
            get {return GetBool(PARAM_RIGHTS_ENABLED);}
            set {SetParam(PARAM_RIGHTS_ENABLED, value);}
        }

        public bool InfoBoxEnabled {
            get {return GetBool(PARAM_INFOBOX_ENABLED);}
            set {SetParam(PARAM_INFOBOX_ENABLED, value);}
        }

        public string SortURL {
            get {return GetString(PARAM_SORT_URL);}
            set {SetParam(PARAM_SORT_URL, value);}
        }

        public string EditURL {
            get {return GetString(PARAM_EDIT_URL);}
            set {SetParam(PARAM_EDIT_URL, value);}
        }

        public string DetailURL {
            get {return GetString(PARAM_DETAIL_URL);}
            set {SetParam(PARAM_DETAIL_URL, value);}
        }

        public string IDColumn {
            get {return GetString(PARAM_ID_COLUMN);}
            set {SetParam(PARAM_ID_COLUMN, value);}
        }

        public long HighlightRecordID {
            get {return GetLong(PARAM_HIGLIGHTRECORD_ID);}
            set {SetParam(PARAM_HIGLIGHTRECORD_ID, value);}
        }

        public bool UseFirstLetterAsPageSelector {
            get {return GetBool(PARAM_USE_FIRST_LETTER);}
            set {SetParam(PARAM_USE_FIRST_LETTER, value);}
        }

        public bool UseJavaScriptToSort {
            get {return GetBool(PARAM_USE_SCRIPT_SORT);}
            set {SetParam(PARAM_USE_SCRIPT_SORT, value);}
        }

        public bool CheckOrder {
            get {return GetBool(PARAM_CHECK_ORDER);}
            set {SetParam(PARAM_CHECK_ORDER, value);}
        }

        public string View {
            get {return GetString(PARAM_VIEW);}
            set {SetParam(PARAM_VIEW, value);}
        }

        public string CssClass {
            get {return GetString(PARAM_CSS_CLASS);}
            set {SetParam(PARAM_CSS_CLASS, value);}
        }

        public int CellPadding {
            get {return GetInt(PARAM_CELL_PADDING);}
            set {SetParam(PARAM_CELL_PADDING, value);}
        }

        public int CellSpacing {
            get {return GetInt(PARAM_CELL_SPACING);}
            set {SetParam(PARAM_CELL_SPACING, value);}
        }

        public int RowsPerPage {
            get {return GetInt(PARAM_ROWS_PER_PAGE);}
            set {SetParam(PARAM_ROWS_PER_PAGE, value);}
        }
        #endregion

        protected virtual int LoadList(DBData db, DataTable dataTable, Table listTable) {
            _listBuilder.orderColumn = dataTable.Columns[db.langAttrName(dataTable.TableName, OrderColumn)];
            _listBuilder.orderDir = OrderDir;
            _listBuilder.headerEnable = HeaderEnabled;
            _listBuilder.deleteEnable = DeleteEnabled;
            _listBuilder.detailEnable = DetailEnabled;
            _listBuilder.rightsEnable = RightsEnabled;
            _listBuilder.editEnable = EditEnabled;
            _listBuilder.infoBoxEnable = InfoBoxEnabled;
            _listBuilder.sortURL = SortURL;
            _listBuilder.editURL = EditURL;
            _listBuilder.detailURL = DetailURL;
            _listBuilder.idColumn = dataTable.Columns[db.langAttrName(dataTable.TableName, IDColumn)];
            _listBuilder.rowsPerPage = RowsPerPage > 0? RowsPerPage : SessionData.getRowsPerListPage(Session);
            _listBuilder.highlightRecordID = HighlightRecordID;
            _listBuilder.useFirstLetterAsPageSelector = UseFirstLetterAsPageSelector;
            _listBuilder.UseJavaScriptToSort = UseJavaScriptToSort;
            _listBuilder.checkOrder = CheckOrder;
            
            listTable.CellPadding = CellPadding;
            listTable.CellSpacing = CellSpacing;
            listTable.CssClass = CssClass;

            AddCellHandler addCellHandler = new AddCellHandler(onAddCell); _listBuilder.addCell += addCellHandler;
            AddCellHandler addHeaderCellHandler = new AddCellHandler(onAddHeaderCell); _listBuilder.addHeaderCell += addHeaderCellHandler;
            AddRowHandler afterAddCellsHandler = new AddRowHandler(onAfterAddCells); _listBuilder.afterAddCells += afterAddCellsHandler;
            AddRowHandler addRowHandler = new AddRowHandler(onAddRow); _listBuilder.addRow += addRowHandler;
            RowAccessHandler rowAccessHandler = new RowAccessHandler(onRowAccess); _listBuilder.onRowAccess += rowAccessHandler;
            
            DoOnLoadList (this,new ListEventArgs(listTable,true,false));
            int retValue = _listBuilder.load(db, dataTable, listTable, _mapper, View);
            DoOnLoadList (this,new ListEventArgs(listTable,false,true));
                        
            _listBuilder.addRow -= addRowHandler;
            _listBuilder.afterAddCells -= afterAddCellsHandler;
            _listBuilder.addHeaderCell -= addHeaderCellHandler;
            _listBuilder.addCell -= addCellHandler;
            _listBuilder.onRowAccess -= rowAccessHandler;
            
            return retValue;
        }
        protected void DoOnLoadList(object Sender, ListEventArgs e) {
            if (OnLoadList != null) OnLoadList(Sender, e);
        }

        protected virtual void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
        }

        protected virtual void onAddHeaderCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
        }

        protected virtual void onAfterAddCells (DataRow row, TableRow r) {
        }

        protected virtual void onAddRow (DataRow row, TableRow r) {
        }

        protected virtual bool onRowAccess (DataTable table, DataRow row, bool isRowAccessPermitted, int requestedAuthorisation) {
            return isRowAccessPermitted;
        }
    }

    /// <summary>
    /// This is a base user control class for all user controls with detailk view.
    /// </summary>
    public class PSOFTDetailViewUserControl : PSOFTMapperUserControl {
        public const string PARAM_VIEW              = "PARAM_VIEW";
        public const string PARAM_CSS_CLASS         = "PARAM_CSS_CLASS";
        public const string PARAM_CELL_PADDING      = "PARAM_CELL_PADDING";
        public const string PARAM_CELL_SPACING      = "PARAM_CELL_SPACING";
        public const string PARAM_CHECK_ORDER       = "PARAM_CHECK_ORDER";
        public const string PARAM_DISPLAY_NULL      = "PARAM_DISPLAY_NULL";

        protected DetailBuilder _detailBuilder = null;

        public PSOFTDetailViewUserControl() : base() {
            _detailBuilder = new DetailBuilder();
            CellPadding = 0;
            CellSpacing = 2;
            CssClass = "Detail";
            CheckOrder = false;
            DisplayNull = true;
        }

		#region Properities
        public bool CheckOrder {
            get {return GetBool(PARAM_CHECK_ORDER);}
            set {SetParam(PARAM_CHECK_ORDER, value);}
        }

        public string View {
            get {return GetString(PARAM_VIEW);}
            set {SetParam(PARAM_VIEW, value);}
        }

        public string CssClass {
            get {return GetString(PARAM_CSS_CLASS);}
            set {SetParam(PARAM_CSS_CLASS, value);}
        }

        public int CellPadding {
            get {return GetInt(PARAM_CELL_PADDING);}
            set {SetParam(PARAM_CELL_PADDING, value);}
        }
        public int CellSpacing {
            get {return GetInt(PARAM_CELL_SPACING);}
            set {SetParam(PARAM_CELL_SPACING, value);}
        }

        public bool DisplayNull {
            get {return GetBool(PARAM_DISPLAY_NULL);}
            set {SetParam(PARAM_DISPLAY_NULL, value);}
        }
        #endregion

        protected virtual void LoadDetail(DBData db, DataTable dataTable, Table detailTable) {
            _detailBuilder.checkOrder = CheckOrder;
            
            detailTable.CellPadding = CellPadding;
            detailTable.CellSpacing = CellSpacing;
            detailTable.CssClass = CssClass;

            AddPropertyHandler addRowHandler = new AddPropertyHandler(onAddProperty); 
            _detailBuilder.detailRow += addRowHandler;
            
            _detailBuilder.load(db, dataTable, detailTable, _mapper, View);
                        
            _detailBuilder.detailRow -= addRowHandler;
            
        }

        protected virtual void onAddProperty (DataRow row, DataColumn col, TableRow r) {
        }
    }

    /// <summary>
    /// This is a base user control class for all user controls with detail view.
    /// </summary>
    public class PSOFTInputViewUserControl : PSOFTMapperUserControl {
        public const string PARAM_VIEW              = "PARAM_VIEW";
        public const string PARAM_CSS_CLASS         = "PARAM_CSS_CLASS";
        public const string PARAM_CELL_PADDING      = "PARAM_CELL_PADDING";
        public const string PARAM_CELL_SPACING      = "PARAM_CELL_SPACING";
        public const string PARAM_CHECK_ORDER       = "PARAM_CHECK_ORDER";
        public const string PARAM_ID_COLUMN         = "PARAM_ID_COLUMN";
        public const string PARAM_IDPREFIX          = "PARAM_IDPREFIX";
        public const string PARAM_MODIFCHECK        = "PARAM_MODIFCHECK";
        public const string PARAM_INPUTTYP          = "PARAM_INPUTTYP";

        protected InputMaskBuilder _inputBuilder = null;

        public PSOFTInputViewUserControl() : base() {
            _inputBuilder = new InputMaskBuilder();
            CellPadding = 0;
            CellSpacing = 2;
            CssClass = "Detail";
            IDColumn = "ID";
            CheckOrder = false;
            ModifCheck = true;
            IDPrefix = "";
            InputType = InputMaskBuilder.InputType.Add;
        }

		#region Properities
        public bool CheckOrder {
            get {return GetBool(PARAM_CHECK_ORDER);}
            set {SetParam(PARAM_CHECK_ORDER, value);}
        }

        public string View {
            get {return GetString(PARAM_VIEW);}
            set {SetParam(PARAM_VIEW, value);}
        }

        public string CssClass {
            get {return GetString(PARAM_CSS_CLASS);}
            set {SetParam(PARAM_CSS_CLASS, value);}
        }

        public int CellPadding {
            get {return GetInt(PARAM_CELL_PADDING);}
            set {SetParam(PARAM_CELL_PADDING, value);}
        }
        public int CellSpacing {
            get {return GetInt(PARAM_CELL_SPACING);}
            set {SetParam(PARAM_CELL_SPACING, value);}
        }

        public string IDColumn {
            get {return GetString(PARAM_ID_COLUMN);}
            set {SetParam(PARAM_ID_COLUMN, value);}
        }
        public string IDPrefix {
            get {return GetString(PARAM_IDPREFIX);}
            set {SetParam(PARAM_IDPREFIX, value);}
        }
        public bool ModifCheck {
            get {return GetBool(PARAM_MODIFCHECK);}
            set {SetParam(PARAM_MODIFCHECK, value);}
        }
        public InputMaskBuilder.InputType InputType {
            get {return (InputMaskBuilder.InputType) GetInt(PARAM_INPUTTYP);}
            set {SetParam(PARAM_INPUTTYP, (int) value);}
        }
        #endregion

        protected virtual string getSql(DataTable table, Table inputTab) {
            BuildSQLHandler buildSQLHandler = new BuildSQLHandler(onBuildSQL); 

            _inputBuilder.buildSQL += buildSQLHandler;
            string result = _inputBuilder.getSql(table,inputTab);
            _inputBuilder.buildSQL -= buildSQLHandler;

            return result;
        }
        protected virtual StringBuilder getSql(DataTable table, Table inputTab, bool extend) {
            return getSql(table, inputTab, extend, true);
        }
        protected virtual StringBuilder getSql(DataTable table, Table inputTab, bool extend, bool useDistinct) {
            BuildSQLHandler buildSQLHandler = new BuildSQLHandler(onBuildSQL); 

            _inputBuilder.buildSQL += buildSQLHandler;
            StringBuilder result = _inputBuilder.getSql(table,inputTab,extend,useDistinct);
            _inputBuilder.buildSQL -= buildSQLHandler;

            return result;
        }
        public void extendSql(StringBuilder sql, DataTable table, string attrName, object attrValue) {
            _inputBuilder.extendSql(sql,table,attrName,attrValue);
        }
        public bool emptySql(StringBuilder sql) {
            return _inputBuilder.emptySql(sql);
        }
        public string endExtendSql(StringBuilder sql) {
            return _inputBuilder.endExtendSql(sql);
        }
        public bool checkInputValue(DataTable table, Table inputTab) {
            return _inputBuilder.checkInputValue(table, inputTab, _mapper);
        }
        public object getInputValue(DataTable table, Table inputTab, string columnName) {
            return _inputBuilder.getInputValue(table, inputTab, columnName);
        }
        public void setInputValue(DataTable table, Table inputTab, string columnName, object value) {
            _inputBuilder.setInputValue(table, inputTab, columnName, value);
        }

        protected virtual void LoadInput(DBData db, DataTable inputTable, Table detailTable) {
            _inputBuilder.Session = Session;
            _inputBuilder.inputType = InputType;
            _inputBuilder.dbColumn = (DBColumn) db.dbColumn;
            _inputBuilder.idColumn = inputTable.Columns[IDColumn];
            _inputBuilder.checkOrder = CheckOrder;
            _inputBuilder.IDPrefix = IDPrefix;
            _inputBuilder.modifCheck = ModifCheck;
            
            detailTable.CellPadding = CellPadding;
            detailTable.CellSpacing = CellSpacing;
            detailTable.CssClass = CssClass;

            AddPropertyHandler addPropertyHandler = new AddPropertyHandler(onAddProperty); 

            _inputBuilder.addRow += addPropertyHandler;
            
            _inputBuilder.load(db, inputTable, detailTable, _mapper, View);
                        
            _inputBuilder.addRow -= addPropertyHandler;
            
        }

        protected virtual void onAddProperty (DataRow row, DataColumn col, TableRow r) {
        }
        protected virtual void onBuildSQL (StringBuilder build, System.Web.UI.Control control, DataColumn col, object val) {
            _inputBuilder.dbColumn.AddToSql(build,col,val);
        }
    }


    /// <summary>
    /// This is a base user control class for all user controls with summary view.
    /// </summary>
    public class PSOFTSummaryViewUserControl : PSOFTMapperUserControl {
        protected SummaryBuilder _summaryBuilder = null;

        public PSOFTSummaryViewUserControl() : base() {
            _summaryBuilder = new SummaryBuilder();
            _summaryBuilder.addCell += new AddSummaryCellHandler(onAddCell);
        }

		#region Properities
        public int CellPadding{
            get {return _summaryBuilder.CellPadding;}
            set {_summaryBuilder.CellPadding =  value;}
        }

        public int CellSpacing{
            get {return _summaryBuilder.CellSpacing;}
            set {_summaryBuilder.CellSpacing =  value;}
        }

        public int ButtonWidth{
            get {return _summaryBuilder.ButtonWidth;}
            set {_summaryBuilder.ButtonWidth =  value;}
        }

        public int [] ColumnWidths{
            get {return _summaryBuilder.ColumnWidths;}
            set {_summaryBuilder.ColumnWidths =  value;}
        }

        public HorizontalAlign [] HeaderColumnAligns{
            get {return _summaryBuilder.HeaderColumnAligns;}
            set {_summaryBuilder.HeaderColumnAligns =  value;}
        }

        public HorizontalAlign [] ColumnAligns{
            get {return _summaryBuilder.ColumnAligns;}
            set {_summaryBuilder.ColumnAligns =  value;}
        }

        public int ColumnCount{
            get {return _summaryBuilder.ColumnCount;}
            set {_summaryBuilder.ColumnCount =  value;}
        }
        #endregion

        protected virtual void BuildHeader(Table parentTable, string [] values){
            _summaryBuilder.BuildHeader(parentTable, values);
        }
          
        protected virtual void BuildEntry(Table parentTable, string prefix, string recordID, int indentLevel, string [] values) {
            _summaryBuilder.BuildEntry(parentTable, prefix, recordID, indentLevel, values);
        }
  
        protected virtual Table BuildChildTable(Table rootTable, string prefix, string recordID, int indentLevel) {
            return _summaryBuilder.BuildChildTable(rootTable, prefix, recordID, indentLevel);
        }
        
        protected virtual void onAddCell(TableRow r, TableCell cell, int columnIndex, int indentLevel){
        }
    }

    public class NextEventArgs {
        private string _loadUrl = "";
        private long _searchResultID = -1L;

        public NextEventArgs() {}

        public string LoadUrl {
            get {return _loadUrl;}
            set {_loadUrl = value;}
        }

        public long SearchResultID {
            get {return _searchResultID;}
            set {_searchResultID = value;}
        }
    }

    public delegate void NextEventHandler(object Sender, NextEventArgs e);

    public class PSOFTSearchListUserControl : PSOFTListViewUserControl {
        public const string PARAM_SINGLE_RESULT_SET = "PARAM_SINGLE_RESULT_SET";
        public const string PARAM_CHECKBOX_ENABLED  = "PARAM_CHECKBOX_ENABLED";
        public const string PARAM_SEARCHRESULT_ID  = "PARAM_SEARCHRESULT_ID";

        protected NextEventArgs _nextArgs = new NextEventArgs();

        public PSOFTSearchListUserControl() : base() {}

        #region Properties
        public bool SingleResultRecord {
            get { return GetBool(PARAM_SINGLE_RESULT_SET); }
            set {SetParam(PARAM_SINGLE_RESULT_SET, value);}
        }

        public bool CheckBoxEnabled {
            get { return GetBool(PARAM_CHECKBOX_ENABLED); }
            set {SetParam(PARAM_CHECKBOX_ENABLED, value);}
        }
        
        /// <summary>
        /// A SearchResultID can be provided to merge the new searchresult with the existing searchresult identified by SearchResultID.
        /// </summary>
        public long SearchResultID {
            get { return GetLong(PARAM_SEARCHRESULT_ID); }
            set {SetParam(PARAM_SEARCHRESULT_ID, value);}
        }
        #endregion

        public event NextEventHandler OnNextClick;

        protected void DoOnNextClick(object Sender) {
            DoOnNextClick(Sender, _nextArgs);
        }

        protected void DoOnNextClick(object Sender, NextEventArgs e) {
            if (OnNextClick != null)
                OnNextClick(Sender, e);
        }

        protected override int LoadList(DBData db, DataTable dataTable, Table listTable) {
            _listBuilder.radioButtons = SingleResultRecord;
            _listBuilder.checkBoxEnable = CheckBoxEnabled;
            return base.LoadList(db, dataTable, listTable);
        }

        /// <summary>
        /// Saves the checked rows in a new searchresult table
        /// </summary>
        /// <param name="listTable">Table with the checked list</param>
        /// <param name="tableName">Name of the table to be saved with searchresult</param>
        /// <returns></returns>
        protected long SaveInSearchResult(Table listTable, string tableName) {
            long id = SaveInSearchResult(listTable, tableName, tableName, "ID", SearchResultID);
            SearchResultID = id;
            return id;
        }

        /// <summary>
        /// Saves the checked rows in a new searchresult table
        /// </summary>
        /// <param name="listTable">Table with the checked list</param>
        /// <param name="tableName">Name of the table to be saved with searchresult</param>
        /// <param name="searchResultID"></param>
        /// <returns></returns>
        protected long SaveInSearchResult(Table listTable, string tableName,long searchResultID) {
            return SaveInSearchResult(listTable, tableName, tableName, "ID", searchResultID);
        }
        /// <summary>
        /// Saves the checked rows in the searchresult table
        /// </summary>
        /// <param name="listTable">Table with the checked list</param>
        /// <param name="tableName">Name of the table to be saved with searchresult</param>
        /// <param name="view">Name of view of id column</param>
        /// <param name="idColumn">ID column name</param>
        /// <param name="searchResultID">ID of the searchresult table, if &lt;= 0 a new one will be created.</param>
        /// <returns>id of the search-result table</returns>
        protected long SaveInSearchResult(Table listTable, string tableName, string view, string idColumn, long searchResultID) {
            DBData db = DBData.getDBData(Session);
            bool selectAll = false;
            bool isOneChecked = false;

            Logger.Log("Saving in SEARCHRESULT " + listTable.Rows.Count + " Rows...", Logger.DEBUG);

            //save selected items in DB
            db.connect();
            try {
                string sql = "";

                while (!isOneChecked) {
                    foreach (TableRow r in listTable.Rows) {
                        if (ListBuilder.isChecked(r) || selectAll) {
                            isOneChecked = true;
                            string recordID = ListBuilder.getID(r);
                            if (recordID == "") continue;
                            if (searchResultID < 1) searchResultID = db.newId("SEARCHRESULT");
                            if (sql == "") {
                                sql = "insert into SEARCHRESULT (ID, TABLENAME, ROW_ID) select " + searchResultID + ",'" + tableName + "'," + idColumn + " from "+view;
                                sql += " where %ID not in (select ROW_ID from SEARCHRESULT where id = "+searchResultID+" and  TABLENAME='"+tableName+"') and "+idColumn+" = %ID";
                            }
                            db.execute(sql.Replace("%ID",recordID));
                        }
                    }

                    if (selectAll)
                        break;

                    if (!isOneChecked)
                        selectAll = true;
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }

            return searchResultID;
        }
    }

    /// <summary>
    /// This is a base class for all page layouts user controls.
    /// This class has one virtual method to override in child classes:
    ///   DoSetContentLayoutControl(ContentLayoutControl layout)
    /// This method should add given layout variable to a specific ControlCollection in a child classes,
    /// f.e.: tableCell.Controls.Add(layout);
    /// </summary>
    public class PageLayoutControl : PSOFTMapperUserControl {
        public const string PARAM_ERROR_MAESSAGE = "ERROR_MESSAGE";

        private ContentLayoutControl _contentLayoutControl = null;

        public PageLayoutControl() : base() {}

		#region Properties
        public ContentLayoutControl ContentLayoutControl {
            get {return _contentLayoutControl;}
            set {SetContentLayoutControl(value);}
        }

        public string ErrorMessage {
            get {return GetString(PARAM_ERROR_MAESSAGE);}
            set {
                SetParam(PARAM_ERROR_MAESSAGE, value);
                DoSetErrorMessage(value);
            }
        }
		#endregion

		#region Public methods
        public void SetContentLayoutControl(ContentLayoutControl layout) {
            _contentLayoutControl = layout;
            DoSetContentLayoutControl(layout);
        }

        public void SetContentControl(string contentPlaceName, PSOFTUserControl control) {
            if (_contentLayoutControl != null)
                _contentLayoutControl.SetContentControl(contentPlaceName, control);
        }
		#endregion

		#region Protected virtual methods to override in child classes
        protected virtual void DoSetContentLayoutControl(ContentLayoutControl layout) {
        }

        protected virtual void DoSetErrorMessage(string message) {
            throw new Exception("You should override this method in your layout class: DoSetErrorMessage. " + GetType().Name);
        }
		#endregion
    }

    /// <summary>
    /// This is a base class for all layout controls.
    /// Be sure to override all virtual methods from this class in your child classes.
    /// </summary>
    public class ContentLayoutControl : PSOFTUserControl {
        public ContentLayoutControl() : base()	{}

		#region Public methods
        /// <summary>
        /// This method is used to adds given user control to this layout on specified place
        /// </summary>
        /// <param name="contentPlaceName">Content place name</param>
        /// <param name="control">Control to add</param>
        public void SetContentControl(string contentPlaceName, PSOFTUserControl control) {
            DoSetContentControl(contentPlaceName, control);
        }

        /// <summary>
        /// This method enables to set given layout place visible
        /// </summary>
        /// <param name="contentPlaceName">Content place name</param>
        /// <param name="value">Enable or disable layout place</param>
        public void SetContentPlaceVisible(string contentPlaceName, bool value) {
            DoSetContentPlaceVisible(contentPlaceName, value);
        }

        /// <summary>
        /// This method should return a control that is identified with contentPlaceName
        /// in child class.
        /// </summary>
        /// <param name="contentPlaceName">Content place name supported by this layout</param>
        /// <returns>Control identified by contentPlaceName</returns>
        public Control GetContentPlace(string contentPlaceName) {
            return DoGetContentPlace(contentPlaceName);
        }
		#endregion

		#region Protected virtual methods to override in child controls
        /// <summary>
        /// Default this method adds given control to content place found by contentPlaceName.
        /// If you would like to make other functionality please to override this method.
        /// </summary>
        /// <param name="contentPlaceName"></param>
        /// <param name="control"></param>
        protected virtual void DoSetContentControl(string contentPlaceName, PSOFTUserControl control) {
            GetContentPlace(contentPlaceName).Controls.Add(control);
        }

        /// <summary>
        /// By default this method set visible property of a content place found using contentPlaceName
        /// variable.
        /// If you would like to make other functionality please to override this method.
        /// </summary>
        /// <param name="contentPlaceName"></param>
        /// <param name="value"></param>
        protected virtual void DoSetContentPlaceVisible(string contentPlaceName, bool value) {
            GetContentPlace(contentPlaceName).Visible = value;
        }

        /// <summary>
        /// This method must be overrided in your child class.
        /// </summary>
        /// <param name="contentPlaceName"></param>
        /// <returns></returns>
        protected virtual Control DoGetContentPlace(string contentPlaceName) {
            throw new Exception("Please to override DoGetContentPlace method in your layout class: " + this.GetType().Name);
        }
		#endregion
    }
}
