using ch.appl.psoft.Interface;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.db
{
    /// <summary>
    /// Scan Zeilen actor
    /// </summary>
    /// <param name="table">Scan-Tabelle</param>
    /// <param name="row">Zeile</param>
    /// <param name="rowNumber">Zeilennummer > 0</param>
    /// <param name="rowAuthorisations">Zugrifferlaubnis auf Zeile</param>
    /// <return>false: skip row</return>
    public delegate bool ScanActionRowHandler(DataTable table, DataRow row, int rowNumber, int rowAuthorisations);
        
    /// <summary>
    /// Scan Zellen actor
    /// </summary>
    /// <param name="table">Scan-Tabelle</param>
    /// <param name="row">Zeile</param>
    /// <param name="rowNumber">Zeilennummer > 0</param>
    /// <param name="col">Kolonne</param>
    /// <param name="colNumber">Kolonnennummer &gt;0: sichtbar, &lt;0: unsichtbar</param>
    public delegate void ScanActionCellHandler(DataTable table, DataRow row,  int rowNumber, DataColumn col, int colNumber);
        
    /// <summary>
    /// Scan Kolonnen actor
    /// </summary>
    /// <param name="table">Scan-Tabelle</param>
    /// <param name="col">Kolonne</param>
    /// <param name="colNumber">Kolonnennummer &gt;0: sichtbar, &lt;0: unsichtbar</param>
    public delegate void ScanActionColumnHandler(DataTable table, DataColumn col, int colNumber);

    /// <summary>
    /// Summary description for DBData.
    /// </summary>
    public class DBData : SQLDB {
        /// <summary>
        /// READ: read right. Valid on table, row, column and cell.
        /// INSERT: insert right. Valid on table, row, column and cell.
        /// UPDATE: update right. Valid on table, row, column and cell.
        /// DELETE: delete right. Valid on table, row, column and cell.
        /// ADMIN: admin right. Valid on table and row.
        /// EXECUTE: execute right. Valid on application-rights.
        /// </summary>
        public class AUTHORISATION {
            public const int READ = 2;
            public const int INSERT = 4;
            public const int UPDATE = 8;
            public const int DELETE = 16;
            public const int ADMIN = 32;
            public const int EXECUTE = 64;
            // Composed rights
            public const int RU = READ + UPDATE;
            public const int RUD = RU + DELETE;
            public const int RUDI = RUD + INSERT;
            public const int RAUDI = RUDI + ADMIN;
            public const int AUDI = ADMIN + UPDATE + DELETE + INSERT;
            public const int FULL_ACCESS = RAUDI + EXECUTE;
        }

        // Accessor Groups
        public class ACCESSOR{
            public const int ALL            = 1;
            public const int ADMINISTRATORS = 2;
            public const int HR             = 3;
        }

        // All application-rights of all modules should be listed here...
        public class APPLICATION_RIGHT{
            //  default
            public const int COMMON                  = 0;
            public const int ADMIN_AUTHORISATIONS    = 1;
    
            //  from module performance
            public const int MODULE_PERFORMANCE      = 10;
            public const int PERFORMANCE_RATING      = 11;
            public const int JOB_EXPECTATION         = 12;

            //  from module fbs
            public const int MODULE_FBS              = 20;
            public const int JOB_DESCRIPTION         = 21;
    
            //  from module skills
            public const int MODULE_SKILLS           = 30;
            public const int SKILLS                  = 31;

            //  from module training
            public const int MODULE_TRAINING         = 40;
            public const int TRAINING                = 41;

            //  from module fbw
            public const int MODULE_FBW              = 50;
            public const int FBW                     = 51;

            //  from module mbo
            public const int MODULE_MBO              = 60;
            public const int MBO                     = 61;

            //  from module project
            public const int MODULE_PROJECT          = 70;

            //  from module contact
            public const int MODULE_CONTACT          = 80;

            //  from module knowledge
            public const int MODULE_KNOWLEDGE        = 90;

            //  from module survey
            public const int MODULE_SURVEY           = 100;

            //  from module lohn
            public const int MODULE_LOHN             = 110;
    
            //  from module news
            public const int MODULE_NEWS	         = 120;

			//  from module suggestion
			public const int MODULE_SUGGESTION       = 140; 
        }

        // UID Assignment type
        public class ASSIGNMENT{
            public const int ANY            = -1;
            public const int UNDEFINED      = 0;
            public const int CAUSALITY      = 1;
            public const int CHRONOLOGY     = 2;
            public const int GENERALIZATION = 3;
        }

        /// <summary>
        /// Internal Helperclass for DataColumns
        /// </summary>
        protected class DataColumnCtx : IComparable {
            public DataColumn column;
            public bool noAccess = false;
            public int colNumber = 0;
            private int ordNum = 9999;
        
            public DataColumnCtx(DataColumn col,bool noAccess,int colNumber) {
                this.column = col;
                this.noAccess = noAccess;
                this.colNumber = colNumber;
                this.ordNum = column.ExtendedProperties["OrdNum"] != null ? (int) column.ExtendedProperties["OrdNum"] : 9999;
            }
        
            public int CompareTo(object b) {
                DataColumnCtx ctxB = (DataColumnCtx) b;
                int ordNumB = ctxB.ordNum;
            
                return ordNum == ordNumB ? 0 : (ordNum > ordNumB ? 1 : -1);
            }
        }

        /// <summary>
        /// Holds a cache-entry of various user's accessorIDs
        /// </summary>
        protected class AccessorIDsCacheEntry : DBCacheEntry{
            protected long _accessorID = -1L;
            protected ArrayList _accessorIDs = null;
            protected string _accessorIDsSQLInClause = "";

            public AccessorIDsCacheEntry(long accessorID) : base(60){ // after 60 seconds the cache-entry becomes stale!
                _accessorID = accessorID;
            }

            protected override void onRefreshing(DBData db){
                _accessorIDs = db.getParentAccessorIDs(_accessorID);
                _accessorIDs.Add(_accessorID);
                _accessorIDsSQLInClause = BuildSQLInClause(_accessorIDs);
            }

            /// <summary>
            /// Returns the list of Accessors (IDs) corresponding to the Accessor (recursive)
            /// </summary>
            /// <param name="db">A connected DBData</param>
            /// <returns></returns>
            public ArrayList getAccessorIDs(DBData db){
                lock (this){
                    refreshIfStale(db);
                    return _accessorIDs;
                }
            }

            public string getAccessorIDsSQLInClause(DBData db){
                lock (this){
                    refreshIfStale(db);
                    return _accessorIDsSQLInClause;
                }
            }
        }

        protected class ColumnAuthorisationsCacheEntry : DBCacheEntry{
            protected long _accessorID = -1L;
            protected string _tableName = "";
            protected string _columnName = "";
            protected bool _recursive = false;
            protected bool _hierarchic = false;
            protected int _columnAuthorisations = 0;

            public ColumnAuthorisationsCacheEntry(long accessorID, string tableName, string columnName, bool recursive, bool hierarchic) : base(60){ // after 60 seconds the cache-entry becomes stale!
                _accessorID = accessorID;
                _tableName = tableName;
                _columnName = columnName;
                _recursive = recursive;
                _hierarchic = hierarchic;
            }

            protected override void onRefreshing(DBData db){
                _columnAuthorisations = 0;
                string sql = "select distinct AUTHORISATION from ACCESS_RIGHT_COLUMN where TABLENAME = '" + _tableName + "' and COLUMNNAME='" + _columnName + "' and ACCESSOR_ID ";
                if (_recursive) 
                    sql += db.getAccessorIDsSQLInClause(_accessorID);
                else
                    sql += "= " + _accessorID;

                DataTable table = db.getDataTable(sql,Logger.VERBOSE);

                if (table.Rows.Count > 0)
                    _columnAuthorisations = db.buildAuthorisationArray(table);

                if (_hierarchic)
                    _columnAuthorisations |= db.getTableAuthorisations(_accessorID, _tableName, _recursive);
            }

            public int getColumnAuthorisations(DBData db){
                lock (this){
                    refreshIfStale(db);
                    return _columnAuthorisations;
                }
            }
        }

        // the ID of the user logged in
        protected long _userID = -1L;

        // the ID of the accessor assigned to the user
        protected long _userAccessorID = -1L;

        // the static cache to hold the various cache-entries
        protected static Hashtable _cache = new Hashtable();

        // it set to true an ongoing TableData-Scan gets interrupted.
        protected bool _breakScanTableData = false;

        protected HttpSessionState _Session = null;
        //
        protected Interface.DBObjects.Document _document = null;
        protected Interface.DBObjects.DocumentHistory _documentHistory = null;
        protected Interface.DBObjects.Folder _folder = null;
        protected Interface.DBObjects.Clipboard _clipboard = null;
        protected Interface.DBObjects.Person _person = null;
        protected Interface.DBObjects.ContactGroup _contactGroup = null;
        protected Interface.DBObjects.Tasklist _tasklist = null;
        protected Interface.DBObjects.TasklistGroup _tasklistGroup = null;
        protected Interface.DBObjects.Performance _performance = null;
        protected Interface.DBObjects.Training _training = null;
        protected Interface.DBObjects.Measure _measure = null;
        protected Interface.DBObjects.News _news = null;
        protected Interface.DBObjects.Subscription _subscription = null;
        protected Interface.DBObjects.Mailing _mailing = null;
        protected Interface.DBObjects.DispatchDocument _dispatchDocument = null;
        protected Interface.DBObjects.Registry _registry = null;
        protected Interface.DBObjects.Journal _journal = null;
        protected Interface.DBObjects.Project _project = null;
        protected Interface.DBObjects.ProjectBilling _projectBilling = null;
        protected Interface.DBObjects.Phase _phase = null;
        protected Interface.DBObjects.Objective _objective = null;
        protected Interface.DBObjects.Survey _survey = null;
        protected Interface.DBObjects.Suggestion _suggestion = null;
        protected Interface.DBObjects.Execution _execution = null;
        protected Interface.DBObjects.SuggestionExecution _suggestionExecution = null;
        protected Interface.DBObjects.Organisation _organisation = null;
        protected Interface.DBObjects.Orgentity _orgentity = null;
        protected Interface.DBObjects.Job _job = null;
        protected Interface.DBObjects.Employment _employment = null;
        protected Interface.DBObjects.ChartNode _chartNode = null;
        protected Interface.DBObjects.Lohn _lohn = null;
        protected Interface.DBObjects.Knowledge _knowledge = null;
        protected Interface.DBObjects.Theme _theme = null;
        protected Interface.DBObjects.WikiImage _wikiImage = null;
        protected Interface.DBObjects.Matrix _matrix = null;
        protected Interface.DBObjects.Dimension _dimension = null;
        protected Interface.DBObjects.Characteristic _characteristic = null;
        protected Interface.DBObjects.ReportLayout _reportLayout = null;

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="Session"></param>
        private DBData(Config config, HttpSessionState Session) : base(config.dbDriver, config.dbUser, config.dbPassword, config.dbServer, config.dbPort, config.dbName, config.dbLanguageCode, true) {
            _Session = Session;
            if (_Session != null) {
                userId = SessionData.getUserID(_Session);
                userAccessorID = SessionData.getUserAccessorID(_Session);
                base._multiLanguageEnable = config.dbMultiLanguageEnable;
                base.dbColumn = SessionData.getDBColumn(_Session);
            }
        }
        
        public DBData(string className, string user, string pwd, string source, string port, string db, string languageCode, bool pooling) : base(className, user, pwd, source, port, db, languageCode, pooling) {
        }
        
        /// <summary>
        /// Actor event for row-scan
        /// </summary>
        public event ScanActionRowHandler scanRow = null;
        
        /// <summary>
        /// Actor event for cell-scan
        /// </summary>
        public event ScanActionCellHandler scanCell = null;

        /// <summary>
        /// Actor event for column-scan
        /// </summary>
        public event ScanActionColumnHandler scanColumn = null;

        public HttpSessionState session {
            get { return _Session; }
        }

        /// <summary>
        /// Creates an instance of DBData
        /// </summary>
        /// <returns>new DBData</returns>
        public static DBData getDBData() {
            return new DBData(Global.Config, null);
        }

        /// <summary>
        /// Creates an instance of DBData
        /// </summary
        /// <param name="Session">HttpSessionState</param>
        /// <returns></returns>
        public static DBData getDBData(HttpSessionState Session) {
            return new DBData(Global.Config, Session);
        }

        /// <summary>
        /// Creates an instance of DBData
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <returns></returns>
        public static DBData getDBData(Config config) {
            return new DBData(config,null);
        }

        public Interface.DBObjects.Document Document {
            get {
                if (_document == null)
                    _document = new Interface.DBObjects.Document(this, _Session);

                return _document;
            }
        }

        public Interface.DBObjects.DocumentHistory DocumentHistory {
            get {
                if (_documentHistory == null)
                    _documentHistory = new Interface.DBObjects.DocumentHistory(this, _Session);

                return _documentHistory;
            }
        }

        public Interface.DBObjects.Registry Registry {
            get {
                if (_registry == null)
                    _registry = new Interface.DBObjects.Registry(this, _Session);

                return _registry;
            }
        }

        public Interface.DBObjects.Journal Journal {
            get {
                if (_journal == null)
                    _journal = new Interface.DBObjects.Journal(this, _Session);

                return _journal;
            }
        }

        public Interface.DBObjects.Project Project {
            get {
                if (_project == null)
                    _project = new Interface.DBObjects.Project(this, _Session);

                return _project;
            }
        }

        public Interface.DBObjects.ProjectBilling ProjectBilling
        {
            get
            {
                if (_projectBilling == null)
                    _projectBilling = new Interface.DBObjects.ProjectBilling(this, _Session);

                return _projectBilling;
            }
        }

        public Interface.DBObjects.Phase Phase {
            get {
                if (_phase == null)
                    _phase = new Interface.DBObjects.Phase(this, _Session);

                return _phase;
            }
        }

        public Interface.DBObjects.Folder Folder {
            get {
                if (_folder == null)
                    _folder = new Interface.DBObjects.Folder(this, _Session);

                return _folder;
            }
        }

        public Interface.DBObjects.Clipboard Clipboard {
            get {
                if (_clipboard == null)
                    _clipboard = new Interface.DBObjects.Clipboard(this, _Session);

                return _clipboard;
            }
        }

        public Interface.DBObjects.Person Person {
            get {
                if (_person == null)
                    _person = new Interface.DBObjects.Person(this, _Session);

                return _person;
            }
        }

        public Interface.DBObjects.ContactGroup ContactGroup {
            get {
                if (_contactGroup == null)
                    _contactGroup = new Interface.DBObjects.ContactGroup(this, _Session);

                return _contactGroup;
            }
        }

        public Interface.DBObjects.Tasklist Tasklist {
            get {
                if (_tasklist == null)
                    _tasklist = new Interface.DBObjects.Tasklist(this, _Session);

                return _tasklist;
            }
        }

        public Interface.DBObjects.TasklistGroup TasklistGroup {
            get {
                if (_tasklistGroup == null)
                    _tasklistGroup = new Interface.DBObjects.TasklistGroup(this, _Session);

                return _tasklistGroup;
            }
        }

        public Interface.DBObjects.Performance Performance {
            get {
                if (_performance == null)
                    _performance = new Interface.DBObjects.Performance(this, _Session);

                return _performance;
            }
        }

        public Interface.DBObjects.Training Training {
            get {
                if (_training == null)
                    _training = new Interface.DBObjects.Training(this, _Session);

                return _training;
            }
        }

        public Interface.DBObjects.Measure Measure {
            get {
                if (_measure == null)
                    _measure = new Interface.DBObjects.Measure(this, _Session);

                return _measure;
            }
        }

        public Interface.DBObjects.News News {
            get {
                if (_news == null)
                    _news = new Interface.DBObjects.News(this, _Session);

                return _news;
            }
        }
        public Interface.DBObjects.Subscription Subscription {
            get {
                if (_subscription == null)
                    _subscription = new Interface.DBObjects.Subscription(this, _Session);

                return _subscription;
            }
        }

        public Interface.DBObjects.Mailing Mailing {
            get {
                if (_mailing == null)
                    _mailing = new Interface.DBObjects.Mailing(this, _Session);

                return _mailing;
            }
        }

        public Interface.DBObjects.DispatchDocument DispatchDocument {
            get {
                if (_dispatchDocument == null)
                    _dispatchDocument = new Interface.DBObjects.DispatchDocument(this, _Session);

                return _dispatchDocument;
            }
        }

        public Interface.DBObjects.Objective Objective {
            get {
                if (_objective == null)
                    _objective = new Interface.DBObjects.Objective(this, _Session);

                return _objective;
            }
        }

        public Interface.DBObjects.Survey Survey {
            get {
                if (_survey == null)
                    _survey = new Interface.DBObjects.Survey(this, _Session);

                return _survey;
            }
        }
        
        public Interface.DBObjects.Suggestion Suggestion 
        {
            get 
            {
                if (_suggestion == null)
                    _suggestion = new Interface.DBObjects.Suggestion(this, _Session);

                return _suggestion;
            }
        }
        
        public Interface.DBObjects.Execution Execution {
            get {
                if (_execution == null)
                    _execution = new Interface.DBObjects.Execution(this, _Session);

                return _execution;
            }
        }

        public Interface.DBObjects.SuggestionExecution SuggestionExecution 
        {
            get 
            {
                if (_suggestionExecution == null)
                    _suggestionExecution = new Interface.DBObjects.SuggestionExecution(this, _Session);

                return _suggestionExecution;
            }
        }

        public Interface.DBObjects.Knowledge Knowledge {
            get {
                if (_knowledge == null)
                    _knowledge = new Interface.DBObjects.Knowledge(this, _Session);

                return _knowledge;
            }
        }

        public Interface.DBObjects.Theme Theme {
            get {
                if (_theme == null)
                    _theme = new Interface.DBObjects.Theme(this, _Session);

                return _theme;
            }
        }

        public Interface.DBObjects.WikiImage WikiImage{
            get {
                if (_wikiImage == null)
                    _wikiImage = new Interface.DBObjects.WikiImage(this, _Session);

                return _wikiImage;
            }
        }

        public Interface.DBObjects.Matrix Matrix{
            get {
                if (_matrix == null)
                    _matrix = new Interface.DBObjects.Matrix(this, _Session);

                return _matrix;
            }
        }

        public Interface.DBObjects.Dimension Dimension{
            get {
                if (_dimension == null)
                    _dimension = new Interface.DBObjects.Dimension(this, _Session);

                return _dimension;
            }
        }

        public Interface.DBObjects.Characteristic Characteristic{
            get {
                if (_characteristic == null)
                    _characteristic = new Interface.DBObjects.Characteristic(this, _Session);

                return _characteristic;
            }
        }

        public Interface.DBObjects.ReportLayout ReportLayout{
            get {
                if (_reportLayout == null)
                    _reportLayout = new Interface.DBObjects.ReportLayout(this, _Session);

                return _reportLayout;
            }
        }

        public Interface.DBObjects.Tree Tree(string tableName) {
            return new Interface.DBObjects.Tree(tableName, this, _Session);
        }

        public Interface.DBObjects.Organisation Organisation {
            get {
                if (_organisation == null)
                    _organisation = new Interface.DBObjects.Organisation(this, _Session);

                return _organisation;
            }
        }

        public Interface.DBObjects.Orgentity Orgentity {
            get {
                if (_orgentity == null)
                    _orgentity = new Interface.DBObjects.Orgentity(this, _Session);

                return _orgentity;
            }
        }

        public Interface.DBObjects.Job Job{
            get {
                if (_job == null)
                    _job = new Interface.DBObjects.Job(this, _Session);

                return _job;
            }
        }

        public Interface.DBObjects.Employment Employment{
            get {
                if (_employment == null)
                    _employment = new Interface.DBObjects.Employment(this, _Session);

                return _employment;
            }
        }

        public Interface.DBObjects.ChartNode ChartNode{
            get {
                if (_chartNode == null)
                    _chartNode = new Interface.DBObjects.ChartNode(this, _Session);

                return _chartNode;
            }
        }

        public Interface.DBObjects.Lohn Lohn {
            get {
                if (_lohn == null) _lohn = new Interface.DBObjects.Lohn(this, _Session);
                return _lohn;
            }
        }

        /// <summary>
        /// Get/Set default user ID
        /// </summary>
        public long userId {
            get {return this._userID;}
            set {this._userID = value;}
        }

        /// <summary>
        /// Get/Set default userAccessor ID
        /// </summary>
        public long userAccessorID {
            get {return _userAccessorID;}
            set {
                _userAccessorID = value;
                getAccessorIDsCacheEntry(_userAccessorID); 
            }
        }

        /// <summary>
        /// Returns the SQL-In clause containing all the user's accessor-IDs
        /// </summary>
        protected string userAccessorIDsSQLInClause{
            get{
                return getAccessorIDsCacheEntry(_userAccessorID).getAccessorIDsSQLInClause(this);
            }
        }

        /// <summary>
        /// If set to true an ongoing TableData-Scan gets interrupted.
        /// </summary>
        public bool BreakScanTableData{
            set {_breakScanTableData = value; }
        }

        public DBCacheEntry getCacheEntry(object key){
            return _cache[key] as DBCacheEntry;
        }

        public void addCacheEntry(object key, DBCacheEntry cacheEntry){
            _cache.Add(key, cacheEntry);
        }

        public object CacheSyncRoot{
            get {return _cache.SyncRoot;}
        }

        protected AccessorIDsCacheEntry getAccessorIDsCacheEntry(long accessorID){
            lock (CacheSyncRoot){
                string key = "AccessorIDs_" + accessorID;
                AccessorIDsCacheEntry cacheEntry = (AccessorIDsCacheEntry) getCacheEntry(key);
                if (cacheEntry == null){
                    cacheEntry = new AccessorIDsCacheEntry(accessorID);
                    addCacheEntry(key, cacheEntry);
                }
                return cacheEntry;
            }
        }

        public void refreshAccessorIDsCacheEntry(long accessorID){
            getAccessorIDsCacheEntry(accessorID).setStale();
        }

        protected ColumnAuthorisationsCacheEntry getColumnAuthorisationsCacheEntry(long accessorID, string tableName, string columnName, bool recursive, bool hierarchic){
            lock (CacheSyncRoot){
                string key = "ColumnAuthorisations_" + accessorID + "_" + tableName + "_" + columnName + "_" + recursive + "_" + hierarchic;
                ColumnAuthorisationsCacheEntry cacheEntry = (ColumnAuthorisationsCacheEntry) getCacheEntry(key);
                if (cacheEntry == null){
                    cacheEntry = new ColumnAuthorisationsCacheEntry(accessorID, tableName, columnName, recursive, hierarchic);
                    addCacheEntry(key, cacheEntry);
                }
                return cacheEntry;
            }
        }

        public long getLeaderGroupAccessorID(string tableName, long ID) 
        {
            long retValue = -1L;

            string sql = "select LEADER_ACCESSOR_ID from " + tableName + " where ID=" + ID;
            DataTable data = getDataTable(sql,Logger.VERBOSE);
            if (data.Rows.Count > 0)
                retValue = Validate.GetValid(data.Rows[0][0].ToString(),-1L);

            return retValue;
        }

        public long getMemberGroupAccessorID(string tableName, long ID) 
        {
            long retValue = -1L;

            string sql = "select ACCESSOR_ID from " + tableName + " where ID=" + ID;
            DataTable data = getDataTable(sql,Logger.VERBOSE);
            if (data.Rows.Count > 0)
                retValue = Validate.GetValid(data.Rows[0][0].ToString(),-1L);

            return retValue;
        }

        /// <summary>
        /// Returns list of Accessors (IDs) having authorisations on a certain table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public long[] getTableAccessorIDs(string tableName) 
        {
            long [] retValue = new long[0];
            string sql = "select ID from ACCESSORV where ID in (select distinct ACCESSOR_ID from ACCESS_RIGHT_RT where TABLENAME='" + tableName + "' and ROW_ID=0) order by " + langAttrName("ACCESSORV", "TITLE");

            DataTable table = getDataTable(sql,Logger.VERBOSE);
            
            if (table.Rows.Count > 0) 
            {
                retValue = new long[table.Rows.Count];
                for (int i = 0; i < table.Rows.Count; i++) 
                {
                    retValue[i] = long.Parse(table.Rows[i][0].ToString());
                }
            }

            return retValue;
        }

        /// <summary>
        /// Returns list of Accessors (IDs) having authorisations on a certain table-column
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public long[] getColumnAccessorIDs(string tableName, string columnName) 
        {
            long [] retValue = new long[0];
            string sql = "select ID from ACCESSORV where ID in (select distinct ACCESSOR_ID from ACCESS_RIGHT_COLUMN where TABLENAME='" + tableName + "' and COLUMNNAME='" + columnName + "') order by " + langAttrName("ACCESSORV", "TITLE");

            DataTable table = getDataTable(sql,Logger.VERBOSE);
        
            if (table.Rows.Count > 0) 
            {
                retValue = new long[table.Rows.Count];
                for (int i = 0; i < table.Rows.Count; i++)
                    retValue[i] = long.Parse(table.Rows[i][0].ToString());
            }

            return retValue;
        }

        /// <summary>
        /// Returns list of Accessors (IDs) having authorisations on a certain table-entry
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        public long[] getRowAccessorIDs(string tableName, long rowID) 
        {
            long [] retValue = new long[0];
            string sql = "select ID from ACCESSORV where ID in (select distinct ACCESSOR_ID from ACCESS_RIGHT_RT where TABLENAME='" + tableName + "' and (ROW_ID=" + rowID + " or ROW_ID=0)) order by " + langAttrName("ACCESSORV", "TITLE");

            DataTable table = getDataTable(sql,Logger.VERBOSE);
        
            if (table.Rows.Count > 0) 
            {
                retValue = new long[table.Rows.Count];
                for (int i = 0; i < table.Rows.Count; i++) 
                {
                    retValue[i] = long.Parse(table.Rows[i][0].ToString());
                }
            }

            return retValue;
        }

        /// <summary>
        /// Returns array of accessors (IDs) having a certain authorisation on a certain table-entry
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="rowID"></param>
        /// <param name="authorisation"></param>
        /// <param name="applicationRight"></param>
        /// <returns></returns>
        public long[] getRowAccessorIDs(string tableName, long rowID, int authorisation, int applicationRight){
            ArrayList accessorIDs = new ArrayList();
            string sql = "select ID, TABLENAME from ACCESSOR where ID in (select distinct ACCESSOR_ID from ACCESS_RIGHT_RT where TABLENAME='" + tableName + "' and (ROW_ID=" + rowID + " or ROW_ID=0) and (AUTHORISATION&" + authorisation + ")=" + authorisation + " and APPLICATION_RIGHT=" + applicationRight + ")";
            DataTable table = getDataTable(sql,Logger.VERBOSE);
        
            for (int i=0; i<table.Rows.Count; i++) {
                string tablename = table.Rows[i][1].ToString();
                long accessorID = long.Parse(table.Rows[i][0].ToString());
                if (tablename == "PERSON"){
                    if (!accessorIDs.Contains(accessorID))
                        accessorIDs.Add(accessorID);
                }
                else{
                    long[] accessorGroupMembers = getAccessorGroupMembers(accessorID, true, true);
                    foreach (long memberAccessorID in accessorGroupMembers){
                        if (!accessorIDs.Contains(memberAccessorID))
                            accessorIDs.Add(memberAccessorID);
                    }
                }
            }

            long [] retValue = new long[accessorIDs.Count];
            accessorIDs.CopyTo(retValue);

            return retValue;
        }

        /// <summary>
        /// Returns cached list of Accessors (IDs) corresponding to a certain Accessor ID (recursive)
        /// </summary>
        /// <param name="personID">ID of an accessor</param>
        /// <returns></returns>
        public ArrayList getAccessorIDs(long accessorID) {
            return getAccessorIDsCacheEntry(accessorID).getAccessorIDs(this);
        }

        /// <summary>
        /// Returns cached SQL IN-clause "in (1,2,3,4)" for a certain Accessor ID (recursive)
        /// </summary>
        /// <param name="accessorID"></param>
        /// <returns></returns>
        public string getAccessorIDsSQLInClause(long accessorID) {
            return getAccessorIDsCacheEntry(accessorID).getAccessorIDsSQLInClause(this);
        }

        /// <summary>
        /// Returns the corresponding Accessor ID to a certain PersonID
        /// </summary>
        /// <param name="personID">ID of a user in table PERSON</param>
        /// <returns></returns>
        public long getAccessorID(long personID) 
        {
            long retValue = -1;

            if (personID != _userID) 
            {
                string sql = "select ID from ACCESSOR where TABLENAME='PERSON' and ROW_ID = " + personID;
                DataTable table = getDataTable(sql,Logger.VERBOSE);
                if (table.Rows.Count > 0)
                    retValue = long.Parse(table.Rows[0][0].ToString());
            }
            else
                retValue = _userAccessorID;

            return retValue;
        }
        
        /// <summary>
        /// Returns list of Parent-Accessors (IDs) corresponding to a certain AccessorID
        /// </summary>
        /// <param name="accessorID">ID of an Accessor</param>
        /// <returns></returns>
        public ArrayList getParentAccessorIDs(long accessorID) 
        {
            ArrayList retValue = new ArrayList(80);
            string sql = "select ACCESSOR_GROUP_ID from ACCESSOR_GROUP_ASSIGNMENT where ACCESSOR_MEMBER_ID = " + accessorID;

            DataTable table = getDataTable(sql,Logger.VERBOSE);

            if (table.Rows.Count > 0) 
            {
                long accID;
                foreach (DataRow row in table.Rows) 
                {
                    if (SQLColumn.IsNull(row[0])) continue;
                    accID = (long) row[0];
                    if (accID == accessorID) continue;
                    retValue.AddRange(getParentAccessorIDs(accID));
                    retValue.Add(accID);
                }
            }

            return retValue;
        }


        /// <summary>
        /// creates a new accessor-group
        /// </summary>
        /// <param name="title">title of the group</param>
        /// <param name="visible">true: set as visible, false: not visible</param>
        /// <returns>ID of accessor-group, -1 if fails</returns>
        public long createAccessorGroup(string title, bool visible) 
        {
            long retValue = -1;

            string sql = "insert into ACCESSOR (TITLE, VISIBLE) values ('" + title + "'," + (visible? "1":"0") + ")";
            execute(sql);
            retValue = getLastInsertedId("ACCESSOR");

            return retValue;
        }

        public void changeAccessorGroup(long accessorID, string title, bool visible) 
        {
            string sql = "update ACCESSOR set TITLE='" + title + "', VISIBLE=" + (visible? "1":"0") + "where ID=" + accessorID;
            execute(sql);
        }

        /// <summary>
        /// adds an accessor to a accessor-group
        /// </summary>
        /// <param name="memberAccessorID">the ID of the accessor to be added</param>
        /// <param name="groupAccessorID">the ID of the group</param>
        public void addAccessorGroupMember(long memberAccessorID, long groupAccessorID) 
        {
            if (memberAccessorID < 0 || groupAccessorID < 0)
                return;

            // let's check if the adding of the member would create a circular reference...
            // if one of the group's parents is the same as the member then we would have a circular reference  
            ArrayList forbiddenAccessors = getAccessorIDs(groupAccessorID); // all parent-groups (recursive) of the group including itself
            forbiddenAccessors.AddRange(getAccessorGroupMembers(groupAccessorID, false, false)); // all direct descendants of the group
        
            if (!forbiddenAccessors.Contains(memberAccessorID)){
                execute("insert into ACCESSOR_GROUP_ASSIGNMENT (ACCESSOR_MEMBER_ID, ACCESSOR_GROUP_ID) values (" + memberAccessorID + "," + groupAccessorID + ")");
            }
        }

        /// <summary>
        /// removes an accessor from an accessor-group
        /// </summary>
        /// <param name="memberAccessorID">the ID of the accessor to be removed</param>
        /// <param name="groupAccessorID">the ID of the group</param>
        public void removeAccessorGroupMember(long memberAccessorID, long groupAccessorID) 
        {
            if (memberAccessorID < 0 || groupAccessorID < 0)
                return;

            if (isAccessorGroupMember(memberAccessorID, groupAccessorID, false)) 
            {
                string sql = "delete from ACCESSOR_GROUP_ASSIGNMENT where ACCESSOR_MEMBER_ID=" + memberAccessorID + " and ACCESSOR_GROUP_ID=" + groupAccessorID;
                execute(sql);
            }
        }

        public long [] getAccessorGroupMembers(long groupAccessorID, bool recursive, bool onlyPersons) 
        {
            string sql = "select ACCESSOR_MEMBER_ID, TABLENAME from ACCESSOR_GROUP_ASSIGNMENT, ACCESSOR where ACCESSOR_MEMBER_ID=ACCESSOR.ID and ACCESSOR_GROUP_ID=" + groupAccessorID;

            DataTable data = getDataTable(sql,Logger.VERBOSE);
            System.Collections.ArrayList idArray = new System.Collections.ArrayList();
            System.Collections.ArrayList allIDArray = new System.Collections.ArrayList();
            for (int i=0; i<data.Rows.Count; i++) 
            {
                long memberID = long.Parse(data.Rows[i][0].ToString());
                bool isPerson = data.Rows[i][1].ToString().Equals("PERSON");
                if (!allIDArray.Contains(memberID)) 
                {
                    allIDArray.Add(memberID);

                    if (!onlyPersons || (onlyPersons && isPerson))
                        idArray.Add(memberID);

                    if (recursive) 
                    {
                        long [] ids = getAccessorGroupMembers(memberID, recursive, onlyPersons);

                        foreach (long id in ids) 
                        {
                            if (!allIDArray.Contains(id))
                                allIDArray.Add(id);
                            if (!idArray.Contains(id))
                                idArray.Add(id);
                        }
                    }
                }
            }

            long [] retValue = new long[idArray.Count];
            idArray.CopyTo(retValue);

            return retValue;
        }

        /// <summary>
        /// checks if an accessor is member of an accessor-group
        /// </summary>
        /// <param name="memberAccessorID"></param>
        /// <param name="groupAccessorID"></param>
        /// <param name="recursive">true : considers also groups where the accessor belongs to, false: considers only the accessor's rights</param>
        /// <returns></returns>
        public bool isAccessorGroupMember(long memberAccessorID, long groupAccessorID, bool recursive) 
        {
            if (memberAccessorID < 0 || groupAccessorID < 0)
                return false;

            bool retValue = false;

            string memberAccessorClause;
            if (recursive) 
                memberAccessorClause = getAccessorIDsSQLInClause(memberAccessorID);
            else
                memberAccessorClause = "=" + memberAccessorID.ToString();
            string sql = "select top 1 ACCESSOR_MEMBER_ID from ACCESSOR_GROUP_ASSIGNMENT where ACCESSOR_MEMBER_ID " + memberAccessorClause + " and ACCESSOR_GROUP_ID=" + groupAccessorID;

            DataTable data = getDataTable(sql,Logger.VERBOSE);
            if (data.Rows.Count > 0)
                retValue = true;

            return retValue;
        }

        /// <summary>
        /// gets the value of the INHERIT column of a certain table
        /// </summary>
        /// <param name="tableName">name of the table</param>
        /// <param name="ID">ID of the record</param>
        /// <param name="inheritFlag">out bool: true if flag is set, false if flag is not set</param>
        /// <returns>true if inherit flag exists on table and record exists, false otherwise</returns>
        public bool getInheritFlag(string tableName, long ID, out bool inheritFlag) 
        {
            bool retValue = false;
            inheritFlag = false;

            try 
            {
                string sql = "select INHERIT from " + tableName + " where ID=" + ID;
                DataTable data = getDataTable(sql,Logger.VERBOSE);
                if (data.Rows.Count > 0) 
                {
                    string inheritString = data.Rows[0][0].ToString();
                    if (inheritString.Equals("1"))
                        inheritFlag = true;
                    retValue = true;
                }
            }
            catch (Exception) 
            {
                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// sets inherit flag on a certain record
        /// </summary>
        /// <param name="tableName">name of the table</param>
        /// <param name="ID">record ID</param>
        /// <param name="inheritFlag">flag</param>
        public void setInheritFlag(string tableName, long ID, bool inheritFlag) 
        {
            string sql = "update " + tableName + " set INHERIT=" + (inheritFlag? "1":"0") + " where ID=" + ID;
            execute(sql);
        }
        
        /// <summary>
        /// returns true if the accessor is visible
        /// </summary>
        /// <param name="accessorID">ID of the accessor to check</param>
        /// <returns>true: accessor is visible
        ///          false: accessor is not visible</returns>
        public bool isAccessorVisible(long accessorID) 
        {
            bool retValue = false;

            string sql = "select VISIBLE from ACCESSOR where ID=" + accessorID + " and VISIBLE=1";

            DataTable data = getDataTable(sql,Logger.VERBOSE);
            if (data.Rows.Count > 0)
                retValue = true;

            return retValue;
        }

        /// <summary>
        /// build an array of authorisations out of a DataTable containing the list of
        /// authorisations from one of the access-tables.
        /// </summary>
        /// <param name="authorisations">DataTable containing the authorisations</param>
        /// <returns>bitmap of authorisations</returns>
        private int buildAuthorisationArray(DataTable authorisations) 
        {
            int authorisationMap = 0;

            foreach (DataRow row in authorisations.Rows) 
            {
                authorisationMap |= int.Parse(row[0].ToString());
            }

            return authorisationMap;
        }

        /// <summary>
        /// Bestimmt die Access Authorisations des Users auf eine Tabelle
        /// </summary>
        /// <param name="data">Tabelle</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        public int getTableAuthorisations(DataTable data, bool recursive) {
            return getTableAuthorisations(data, APPLICATION_RIGHT.COMMON, recursive);
        }

        /// <summary>
        /// Bestimmt die Access Authorisations des Users auf eine Tabelle
        /// </summary>
        /// <param name="data">Tabelle</param>
        /// <param name="applicationRight">Application-right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        public int getTableAuthorisations(DataTable data, int applicationRight, bool recursive) 
        {
            return getTableAuthorisations(data.TableName, applicationRight, recursive);
        }

        /// <summary>
        /// returns access-authorisations on a certain table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <returns></returns>
        public int getTableAuthorisations(string tableName, bool recursive) {
            return getTableAuthorisations(tableName, APPLICATION_RIGHT.COMMON, recursive);
        }

        /// <summary>
        /// returns access-authorisations on a certain table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="applicationRight">Application-right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <returns></returns>
        public int getTableAuthorisations(string tableName, int applicationRight, bool recursive) 
        {
            return getTableAuthorisations(_userAccessorID, tableName, applicationRight, recursive);
        }

        /// <summary>
        /// returns access-authorisations of a certain accessor on a certain table
        /// </summary>
        /// <param name="accessorID"></param>
        /// <param name="tableName"></param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <returns></returns>
        public int getTableAuthorisations(long accessorID, string tableName, bool recursive) {
            return getTableAuthorisations(accessorID, tableName, APPLICATION_RIGHT.COMMON, recursive);
        }

        /// <summary>
        /// returns access-authorisations of a certain accessor on a certain table
        /// </summary>
        /// <param name="accessorID"></param>
        /// <param name="tableName"></param>
        /// <param name="applicationRight">Application-right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <returns></returns>
        public int getTableAuthorisations(long accessorID, string tableName, int applicationRight, bool recursive) 
        {
            int authorisationList = 0;
            string sql = "select distinct AUTHORISATION from ACCESS_RIGHT_RT where TABLENAME='" + tableName + "' and ROW_ID=0 and APPLICATION_RIGHT=" + applicationRight + " and ACCESSOR_ID ";

            if (recursive) 
                sql += getAccessorIDsSQLInClause(accessorID);
            else
                sql += "= " + accessorID;

            DataTable table = getDataTable(sql,Logger.VERBOSE);

            if (table.Rows.Count > 0)
                authorisationList = buildAuthorisationArray(table);

            return authorisationList;
        }

        /// <summary>
        /// Bestimmt, ob eine Access AUTHORISATION dem User auf eine Tabelle zugeteilt ist.
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="data">Tabelle</param>
        /// <param name="applicationRight">Application-Right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        public bool hasTableAuthorisation(int authorisation, DataTable data, int applicationRight, bool recursive) 
        {
            return (authorisation & getTableAuthorisations(data, applicationRight, recursive)) == authorisation;
        }

        /// <summary>
        /// returns true if the according user owns the specified authorisation.
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="tableName"></param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <returns></returns>
        public bool hasTableAuthorisation(int authorisation, string tableName, bool recursive) {
            return hasTableAuthorisation(authorisation, tableName, APPLICATION_RIGHT.COMMON, recursive); 
        }

        /// <summary>
        /// returns true if the according user owns the specified authorisation.
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="tableName"></param>
        /// <param name="applicationRight">Application-Right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <returns></returns>
        public bool hasTableAuthorisation(int authorisation, string tableName, int applicationRight, bool recursive) 
        {
            return (authorisation & getTableAuthorisations(tableName, applicationRight, recursive)) == authorisation;
        }

        /// <summary>
        /// Bestimmt die Access Authorisations des Users auf eine Zeile in der Tabelle
        /// </summary>
        /// <param name="data">Tabelle</param>
        /// <param name="rowIndex">Zeilennummer (erste Zeilennummer = 0)</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        public int getRowAuthorisations(DataTable data, int rowIndex, bool recursive, bool hierarchic) {
            return getRowAuthorisations(data, data.Rows[rowIndex], APPLICATION_RIGHT.COMMON, recursive, hierarchic);
        }

        /// <summary>
        /// Bestimmt die Access Authorisations des Users auf eine Zeile in der Tabelle
        /// </summary>
        /// <param name="data">Tabelle</param>
        /// <param name="rowIndex">Zeilennummer (erste Zeilennummer = 0)</param>
        /// <param name="applicationRight">Application-Right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        public int getRowAuthorisations(DataTable data, int rowIndex, int applicationRight, bool recursive, bool hierarchic) 
        {
            return getRowAuthorisations(data, data.Rows[rowIndex], applicationRight, recursive, hierarchic);
        }

        /// <summary>
        /// Bestimmt die Access Authorisations des Users auf eine Zeile in der Tabelle
        /// </summary>
        /// <param name="data">Tabelle</param>
        /// <param name="row">Row</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public int getRowAuthorisations(DataTable data, DataRow row, bool recursive, bool hierarchic) {
            return getRowAuthorisations(data, row, APPLICATION_RIGHT.COMMON, recursive, hierarchic);
        }

        /// <summary>
        /// Bestimmt die Access Authorisations des Users auf eine Zeile in der Tabelle
        /// </summary>
        /// <param name="data">Tabelle</param>
        /// <param name="row">Row</param>
        /// <param name="applicationRight">Application-Right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public int getRowAuthorisations(DataTable data, DataRow row, int applicationRight, bool recursive, bool hierarchic) 
        {
            return getRowAuthorisations(data.TableName, ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1), applicationRight, recursive, hierarchic);
        }
        
        /// <summary>
        /// Returns all authorisations of the logged user on a certain table-row taking into account the table-authorisations.
        /// </summary>
        /// <param name="data">Tabelle</param>
        /// <param name="row">Row</param>
        /// <param name="applicationRight">Application-Right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="tableAuthorisations">Array of authorisations to consider as table-authorisation for the hierarchic inheritence</param>
        /// <returns></returns>
        public int getRowAuthorisations(DataTable data, DataRow row, int applicationRight, bool recursive, int tableAuthorisations) 
        {
            return getRowAuthorisations(data.TableName, (long) row["ID"], applicationRight, recursive, false) | tableAuthorisations;
        }

        /// <summary>
        /// returns access-authorisations on a entry in a table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="rowID"></param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public int getRowAuthorisations(string tableName, long rowID, bool recursive, bool hierarchic) {
            return getRowAuthorisations(tableName, rowID, APPLICATION_RIGHT.COMMON, recursive, hierarchic);
        }
        
        /// <summary>
        /// returns access-authorisations on a entry in a table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="rowID"></param>
        /// <param name="applicationRight">Application-Right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public int getRowAuthorisations(string tableName, long rowID, int applicationRight, bool recursive, bool hierarchic) 
        {
            return getRowAuthorisations(_userAccessorID, tableName, rowID, applicationRight, recursive, hierarchic);
        }

        /// <summary>
        /// returns access-authorisations of an accessor on a entry in a table
        /// </summary>
        /// <param name="accessorID"></param>
        /// <param name="tableName"></param>
        /// <param name="rowID"></param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public int getRowAuthorisations(long accessorID, string tableName, long rowID, bool recursive, bool hierarchic) {
            return getRowAuthorisations(accessorID, tableName, rowID, APPLICATION_RIGHT.COMMON, recursive, hierarchic);
        }

        /// <summary>
        /// returns access-authorisations of an accessor on a entry in a table
        /// </summary>
        /// <param name="accessorID"></param>
        /// <param name="tableName"></param>
        /// <param name="rowID"></param>
        /// <param name="applicationRight">Application-right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public int getRowAuthorisations(long accessorID, string tableName, long rowID, int applicationRight, bool recursive, bool hierarchic) 
        {
            int authorisationList = 0;
            string sql = "select distinct AUTHORISATION from ACCESS_RIGHT_RT where TABLENAME = '" + tableName + "' and ROW_ID = " + rowID + " and APPLICATION_RIGHT=" + applicationRight + " and ACCESSOR_ID ";

            if (recursive) 
                sql += getAccessorIDsSQLInClause(accessorID);
            else
                sql += "= " + accessorID;

            DataTable table = getDataTable(sql,Logger.VERBOSE);

            if (table.Rows.Count > 0)
                authorisationList = buildAuthorisationArray(table);
            if (hierarchic)
                authorisationList |= getTableAuthorisations(accessorID, tableName, applicationRight, recursive);

            return authorisationList;
        }

        /// <summary>
        /// checks if an authorisation exists in an authorisation-array
        /// </summary>
        /// <param name="authorisation">authorisation looking for</param>
        /// <param name="authorisations">bitmap of authorisations</param>
        /// <returns>true if authorisation exists, false otherwise</returns>
        public bool hasAuthorisation(int authorisation, int authorisations) 
        {
            return (authorisation & authorisations) == authorisation;
        }

        /// <summary>
        /// checks if an authorisation exists in an authorisation-array considering also
        /// the parent-authorisations (i.e. row-authorisations and table-authorisations)
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="authorisations"></param>
        /// <param name="parentAuthorisations"></param>
        /// <returns></returns>
        public bool hasAuthorisation(int authorisation, int authorisations, bool hasParentAuthorisation) 
        {
            if (hasAuthorisation(authorisation, authorisations))
                return true;
            else
                return hasParentAuthorisation;
        }

        /// <summary>
        /// Bestimmt, ob eine Access AUTHORISATION dem User auf eine Zeile in der
        /// Tabelle zugeteilt ist.
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="data">Tabelle</param>
        /// <param name="rowIndex">Zeilennummer (erste Zeilennummer = 0)</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        public bool hasRowAuthorisation(int authorisation, DataTable data, int rowIndex, bool recursive, bool hierarchic) {
            return hasRowAuthorisation(authorisation, data, rowIndex, APPLICATION_RIGHT.COMMON, recursive, hierarchic);
        }
        
        /// <summary>
        /// Bestimmt, ob eine Access AUTHORISATION dem User auf eine Zeile in der
        /// Tabelle zugeteilt ist.
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="data">Tabelle</param>
        /// <param name="rowIndex">Zeilennummer (erste Zeilennummer = 0)</param>
        /// <param name="applicationRight">Application-right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        public bool hasRowAuthorisation(int authorisation, DataTable data, int rowIndex, int applicationRight, bool recursive, bool hierarchic) 
        {
            return hasRowAuthorisation(authorisation, data, data.Rows[rowIndex], applicationRight, recursive, hierarchic);
        }

        /// <summary>
        /// Bestimmt, ob eine Access AUTHORISATION dem User auf eine Zeile in der
        /// Tabelle zugeteilt ist.
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="data">Tabelle</param>
        /// <param name="row">Row</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public bool hasRowAuthorisation(int authorisation, DataTable data, DataRow row, bool recursive, bool hierarchic) {
            return hasRowAuthorisation(authorisation, data, row, APPLICATION_RIGHT.COMMON, recursive, hierarchic);
        }

        /// <summary>
        /// Bestimmt, ob eine Access AUTHORISATION dem User auf eine Zeile in der
        /// Tabelle zugeteilt ist.
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="data">Tabelle</param>
        /// <param name="row">Row</param>
        /// <param name="applicationRight">Application-right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public bool hasRowAuthorisation(int authorisation, DataTable data, DataRow row, int applicationRight, bool recursive, bool hierarchic) 
        {
            return hasAuthorisation(authorisation, getRowAuthorisations(data, row, applicationRight, recursive, hierarchic));
        }

        /// <summary>
        /// returns true if the according user owns the specified authorisation.
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="tableName"></param>
        /// <param name="rowID"></param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public bool hasRowAuthorisation(int authorisation, string tableName, long rowID, bool recursive, bool hierarchic) {
            return hasAuthorisation(authorisation, getRowAuthorisations(tableName, rowID, recursive, hierarchic));
        }

        /// <summary>
        /// returns true if the according user owns the specified authorisation.
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="tableName"></param>
        /// <param name="rowID"></param>
        /// <param name="applicationRight">Application-right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public bool hasRowAuthorisation(int authorisation, string tableName, long rowID, int applicationRight, bool recursive, bool hierarchic) 
        {
            return hasAuthorisation(authorisation, getRowAuthorisations(tableName, rowID, applicationRight, recursive, hierarchic));
        }

        /// <summary>
        /// returns true if the according user owns the specified authorisation on object identified through its UID
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="UID">UID of object to check</param>
        /// <param name="applicationRight">Application-right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public bool hasUIDAuthorisation(int authorisation, long UID, int applicationRight, bool recursive, bool hierarchic){
            return hasAuthorisation(authorisation, getRowAuthorisations(UID2Tablename(UID), UID2ID(UID), recursive, hierarchic));
        }

        /// <summary>
        /// Bestimmt die Access Authorisations des Users auf eine Spalte in der Tabelle
        /// In der Tabelle muss der Spalten-Alias genau mit columnName übereinstimmen. Dh
        /// in der folgenden Form
        ///     Tabellenname + "_" + Tabellenspalte
        /// Ausnahme: einfacher Table-Select
        /// </summary>
        /// <param name="data">Tabelle</param>
        /// <param name="columnName">Spalte in der Tabelle</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        public int getColumnAuthorisations(DataTable data, string columnName, bool recursive, bool hierarchic) {
            return getColumnAuthorisations(data.TableName, columnName, recursive, hierarchic);
        }

        /// <summary>
        /// returns access-authorisation on a certain column of a table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public int getColumnAuthorisations(string tableName, string columnName, bool recursive, bool hierarchic) 
        {
            return getColumnAuthorisations(_userAccessorID, tableName, columnName, recursive, hierarchic);
        }

        /// <summary>
        /// returns access-authorisation of an accessor on a certain column of a table
        /// </summary>
        /// <param name="accessorID"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public int getColumnAuthorisations(long accessorID, string tableName, string columnName, bool recursive, bool hierarchic) 
        {
            return getColumnAuthorisationsCacheEntry(accessorID, tableName, columnName, recursive, hierarchic).getColumnAuthorisations(this);
        }

        /// <summary>
        /// Bestimmt, ob eine Access AUTHORISATION dem Users auf eine Spalte in der
        /// Tabelle zugeteilt ist.
        /// In der Tabelle muss der Spalten-Alias genau mit columnName übereinstimmen. Dh
        /// in der folgenden Form
        ///     Tabellenname + "_" + Tabellenspalte
        /// Ausnahme: einfacher Table-Select
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="data">Tabelle</param>
        /// <param name="columnName">Spalte in der Tabelle</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        public bool hasColumnAuthorisation(int authorisation, DataTable data, string columnName, bool recursive, bool hierarchic) 
        {
            return hasAuthorisation(authorisation, getColumnAuthorisations(data, columnName, recursive, hierarchic));
        }

        /// <summary>
        /// returns true if the according user owns the specified authorisation.
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public bool hasColumnAuthorisation(int authorisation, string tableName, string columnName, bool recursive, bool hierarchic) 
        {
            return hasAuthorisation(authorisation, getColumnAuthorisations(tableName, columnName, recursive, hierarchic));
        }

        /// <summary>
        /// returns array of authorisations the logged user holds on a specific application-right
        /// </summary>
        /// <param name="applicationRight"></param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <returns></returns>
        public int getApplicationAuthorisations(int applicationRight, bool recursive) 
        {
            return getApplicationAuthorisations(_userAccessorID, applicationRight, recursive);
        }
        
        /// <summary>
        /// returns array of authorisations the for the accessor on a specific application-right
        /// </summary>
        /// <param name="accessorID"></param>
        /// <param name="applicationRight"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public int getApplicationAuthorisations(long accessorID, int applicationRight, bool recursive) 
        {
            int authorisationList = 0;
            string sql = "select distinct AUTHORISATION from ACCESS_RIGHT_RT where APPLICATION_RIGHT=" + applicationRight + " and TABLENAME='RELEASE' and ROW_ID=0 and ACCESSOR_ID ";
            if (recursive) 
                sql += getAccessorIDsSQLInClause(accessorID);
            else
                sql += "= " + accessorID;

            DataTable table = getDataTable(sql,Logger.VERBOSE);

            if (table.Rows.Count > 0)
                authorisationList = buildAuthorisationArray(table);

            return authorisationList;
        }

        /// <summary>
        /// checks is the logged user has the specified authorisation on application-level
        /// </summary>
        /// <param name="authorisation"></param>
        /// <param name="applicationRight">identifies the application-managed right</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <returns></returns>
        public bool hasApplicationAuthorisation(int authorisation, int applicationRight, bool recursive) 
        {
            return hasAuthorisation(authorisation, getApplicationAuthorisations(applicationRight, recursive));
        }

        public void inheritRowAuthorisation(string parentTableName, string parentRowID, string tableName, string rowID) 
        {
            string sql="insert into ACCESS_RIGHT_RT (ACCESSOR_ID, AUTHORISATION, TABLENAME, ROW_ID, APPLICATION_RIGHT) select ACCESSOR_ID, AUTHORISATION, '" + tableName + "', " + rowID + ", APPLICATION_RIGHT from ACCESS_RIGHT_RT where TABLENAME = '" + parentTableName + "' and ROW_ID = '" + parentRowID + "'";
            execute(sql);
        }

        public void grantTableAuthorisation(int authorisation, long accessorID, string tableName) 
        {
            if (accessorID > 0 && tableName!="") 
            {
                _driver.executeProcedure("SET_ACCESS_RIGHT_TABLE",Logger.DEBUG,
                    new ParameterCtx("ACCESSOR_ID",accessorID),
                    new ParameterCtx("AUTHORISATION",authorisation),
                    new ParameterCtx("TABLENAME",tableName));
            }
        }

        /// <summary>
        /// Grand permission to a column
        /// </summary>
        /// <param name="authorisation">persmission</param>
        /// <param name="accessorID">accessor</param>
        /// <param name="tableName">table</param>
        /// <param name="columnName">column or *: all columns</param>
        public void grantColumnAuthorisation(int authorisation, long accessorID, string tableName, string columnName) 
        {
            if (accessorID > 0 && tableName != "" && columnName != "")
            {
                _driver.executeProcedure("SET_ACCESS_RIGHT_COLUMN",Logger.DEBUG,
                    new ParameterCtx("ACCESSOR_ID",accessorID),
                    new ParameterCtx("AUTHORISATION",authorisation),
                    new ParameterCtx("TABLENAME",tableName),
                    new ParameterCtx("COLUMNNAME",columnName));
            }
        }

        public void grantRowAuthorisation(int authorisation, long accessorID, string tableName, long rowID) {
            grantRowAuthorisation(authorisation, accessorID, tableName, rowID, APPLICATION_RIGHT.COMMON);
        }

        public void grantRowAuthorisation(int authorisation, long accessorID, string tableName, long rowID, int applicationRight) 
        {
            if (accessorID > 0 && tableName!="" && rowID > 0) 
            {
                _driver.executeProcedure("SET_ACCESS_RIGHT_ROW",Logger.DEBUG,
                    new ParameterCtx("ACCESSOR_ID",accessorID),
                    new ParameterCtx("AUTHORISATION",authorisation),
                    new ParameterCtx("TABLENAME",tableName),
                    new ParameterCtx("ROW_ID",rowID),
                    new ParameterCtx("APPLICATIONRIGHT", applicationRight)
                    );
            }
        }

        /// <summary>
        /// copies row-authorisations from one row to another
        /// </summary>
        /// <param name="fromTableName">source table</param>
        /// <param name="fromRowID">source row-ID</param>
        /// <param name="toTableName">target table</param>
        /// <param name="toRowID">target row-ID</param>
        public void copyRowAuthorisations(string fromTableName, long fromRowID, string toTableName, long toRowID){
            if (fromTableName != "" && fromRowID > 0 && toTableName != "" && toRowID > 0){
                _driver.executeProcedure("COPY_ACCESS_RIGHT_ROW", Logger.DEBUG,
                    new ParameterCtx("FROMTABLENAME", fromTableName),
                    new ParameterCtx("FROMROWID", fromRowID),
                    new ParameterCtx("TOTABLENAME", toTableName),
                    new ParameterCtx("TOROWID", toRowID));
            }
        }

        public void grantApplicationAuthorisation(int authorisation, long accessorID, int applicationRight) 
        {
            if (accessorID > 0) 
            {
                _driver.executeProcedure("SET_ACCESS_RIGHT_APPLICATION",Logger.DEBUG,
                    new ParameterCtx("ACCESSOR_ID",accessorID),
                    new ParameterCtx("AUTHORISATION",authorisation),
                    new ParameterCtx("APPLICATIONRIGHT",applicationRight));
            }
        }

        public void revokeTableAuthorisation(int authorisation, long accessorID, string tableName) 
        {
            if (accessorID > 0 && tableName!="") 
            {
                _driver.executeProcedure("DROP_ACCESS_RIGHT_TABLE",Logger.DEBUG,
                    new ParameterCtx("ACCESSOR_ID",accessorID),
                    new ParameterCtx("AUTHORISATION",authorisation),
                    new ParameterCtx("TABLENAME",tableName));
            }
        }

        public void revokeColumnAuthorisation(int authorisation, long accessorID, string tableName, string columnName) 
        {
            if (accessorID > 0 && tableName != "" && columnName != "")
            {
                _driver.executeProcedure("DROP_ACCESS_RIGHT_COLUMN",Logger.DEBUG,
                    new ParameterCtx("ACCESSOR_ID",accessorID),
                    new ParameterCtx("AUTHORISATION",authorisation),
                    new ParameterCtx("TABLENAME",tableName),
                    new ParameterCtx("COLUMNNAME",columnName));
            }
        }

        public void revokeRowAuthorisation(int authorisation, long accessorID, string tableName, long rowID) {
            revokeRowAuthorisation(authorisation, accessorID, tableName, rowID, APPLICATION_RIGHT.COMMON);
        }
        
        public void revokeRowAuthorisation(int authorisation, long accessorID, string tableName, long rowID, int applicationRight) 
        {
            _driver.executeProcedure("DROP_ACCESS_RIGHT_ROW",Logger.DEBUG,
                new ParameterCtx("ACCESSOR_ID",accessorID),
                new ParameterCtx("AUTHORISATION",authorisation),
                new ParameterCtx("TABLENAME",tableName),
                new ParameterCtx("ROW_ID",rowID),
                new ParameterCtx("APPLICATIONRIGHT", applicationRight)
                );
        }

        public void revokeApplicationAuthorisation(int authorisation, long accessorID, int applicationRight) 
        {
            if (accessorID > 0) 
            {
                _driver.executeProcedure("DROP_ACCESS_RIGHT_APPLICATION",Logger.DEBUG,
                    new ParameterCtx("ACCESSOR_ID",accessorID),
                    new ParameterCtx("AUTHORISATION",authorisation),
                    new ParameterCtx("APPLICATIONRIGHT",applicationRight));
            }
        }

        /// <summary>
        /// Returns for all columns the authorisation in a bitmap.
        /// </summary>
        /// <param name="authorisation">AUTHORISATION-flag to check on</param>
        /// <param name="data">Table</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (cell inherits from row, row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns></returns>
        public ulong getColumnsAuthorisation(int authorisation, DataTable data, bool recursive, bool hierarchic, bool hasParentAuthorisation) 
        {
            ulong authorisationMap = 0;
            ulong mask = 1;

            foreach (DataColumn col in data.Columns) 
            {
                if (hasParentAuthorisation && hierarchic){
                    authorisationMap = authorisationMap | mask;
                }
                else{
                    int columnAuthorisations = getColumnAuthorisations(data, col.ColumnName, recursive, false);
                    if (hierarchic) {
                        if (hasAuthorisation(authorisation, columnAuthorisations, hasParentAuthorisation))
                            authorisationMap = authorisationMap | mask;
                    }
                    else if (hasAuthorisation(authorisation, columnAuthorisations))
                        authorisationMap = authorisationMap | mask;
                }
                mask *= 2;
            }

            return authorisationMap;
        }
        
        public ulong getColumnsAuthorisation(int authorisation, DataTable data, bool recursive, bool hierarchic) 
        {
            ulong authorisationMap = 0;
            ulong mask = 1;

            foreach (DataColumn col in data.Columns) 
            {
                if (hasColumnAuthorisation(authorisation,data,col.ColumnName, recursive, hierarchic)) authorisationMap = authorisationMap | mask;
                mask *= 2;
            }

            return authorisationMap;
        }

        /// <summary>
        /// Scan all columns of a table
        /// </summary>
        /// <param name="table">Table</param>
        /// <param name="view">Tableview</param>
        /// <param name="visibilities">visibilities(set of SQLColumn.Visibility)</param>
        /// <param name="authorisation">authorisation needed to access the column</param>
        /// <param name="checkOrder">true: sort the columns by user sortorder</param>
        public void scanTableColumn(DataTable table, string view, int visibilities, int authorisation, bool checkOrder) 
        {
            ulong listVisi = getColumnsVisibility(visibilities,table,view);
            ulong colAuth = getColumnsAuthorisation(authorisation, table, true, true, hasTableAuthorisation(authorisation, table, APPLICATION_RIGHT.COMMON, true));
            
            if (checkOrder) 
            {
                int colNumber = 0;
                ulong columnMask = 1;
                ArrayList ordColumns = new ArrayList(table.Columns.Count);
                
                foreach (DataColumn col in table.Columns) 
                {
                    bool noAccess
                        = hasNoAccess(listVisi, colAuth, columnMask, table.TableName, col.ColumnName);
                    
                    colNumber++;    
                    ordColumns.Add(new DataColumnCtx(col,noAccess,colNumber));
                    columnMask *= 2;
                }  
                ordColumns.Sort();    
                colNumber = 0;          
                foreach (DataColumnCtx col in ordColumns) 
                {                    
                    colNumber++;    
                    onScanColumn (table, col.column, (col.noAccess ? -colNumber: colNumber));
                } 
            }
            else 
            {
                int colNumber = 0;
                ulong columnMask = 1;
                
                foreach (DataColumn col in table.Columns) 
                {
                    bool noAccess
                        = hasNoAccess(listVisi, colAuth, columnMask, table.TableName, col.ColumnName);
                    
                    colNumber++;    
                    onScanColumn (table, col, (noAccess ? -colNumber: colNumber));
                    columnMask *= 2;
                } 
            }               
        }
        
        /// <summary>
        /// Scan all rows of a table
        /// </summary>
        /// <param name="table">table</param>
        /// <param name="view">view</param>
        /// <param name="visibilities">visibilities (set of SQLColumn.Visibility)</param>
        /// <param name="authorisation">authorisation needed to access the data</param>
        /// <param name="applicationRight">Application-right</param>
        /// <param name="checkOrder">true: sort columns by user sortorder</param>
        /// <param name="returnRowAuthorisations">true: returns the row-Authorisations, which might be slower</param>
        /// <returns>number of rows</returns>
        public int scanTableData(DataTable table, string view, int visibilities, int authorisation, int applicationRight, bool checkOrder, bool returnRowAuthorisations) 
        {
            _breakScanTableData = false;
            int rowNumber = 0;
            ulong listVisi = getColumnsVisibility(visibilities,table,view);
            int tableAuthorisations = getTableAuthorisations(table, applicationRight, true);
            ulong colAuth = getColumnsAuthorisation(authorisation, table, true, true, hasAuthorisation(authorisation, tableAuthorisations));
            ArrayList ordColumns = null;
            
            if (checkOrder && scanCell != null) 
            {
                int colNumber = 0;
                ulong columnMask = 1;
                
                ordColumns = new ArrayList(table.Columns.Count);
                foreach (DataColumn col in table.Columns) 
                {
                    bool noAccess
                        = hasNoAccess(listVisi, colAuth, columnMask, table.TableName, col.ColumnName);
                    
                    colNumber++;    
                    ordColumns.Add(new DataColumnCtx(col,noAccess,colNumber));
                    columnMask *= 2;
                }  
                ordColumns.Sort();              
            }

            foreach (DataRow row in table.Rows) 
            {
                if (_breakScanTableData){
                    break;
                }
                int colNumber = 0;
                ulong columnMask = 1;
                int rowAuthorisations = 0;
                if (returnRowAuthorisations || !hasAuthorisation(authorisation, tableAuthorisations)){
                    rowAuthorisations = getRowAuthorisations(table, row, applicationRight, true, tableAuthorisations);
                    // HACK: always allow edit of objectives
                    if (!hasAuthorisation(authorisation, rowAuthorisations) && table.TableName != "OBJECTIVE") continue;
                }

                if (!onScanRow(table,row,rowNumber+1,rowAuthorisations)) continue;
                rowNumber++;
                
                if (scanCell != null) {
                    if (checkOrder) {
                        foreach (DataColumnCtx col in ordColumns) {
                            colNumber++;    
                            onScanCell (table, row, rowNumber, col.column, (col.noAccess ? -colNumber : colNumber));
                        }      
                    }
                    else {
                        foreach (DataColumn col in table.Columns) {
                            bool noAccess
                                = hasNoAccess(listVisi, colAuth, columnMask, table.TableName, col.ColumnName);
                    
                            colNumber++;    
                            onScanCell (table, row, rowNumber, col, (noAccess ? -colNumber : colNumber));
                            columnMask *= 2;
                        }      
                    }          
                }
            }
            return rowNumber;
        }
        
        private bool onScanRow (DataTable table, DataRow row, int rowNumber, int rowAuthorisations) 
        {
            if (scanRow != null) 
            {
                // Invokes the delegates. 
                return scanRow(table, row, rowNumber, rowAuthorisations);
            }
            return true;
        }
        
        private void onScanCell (DataTable table, DataRow row, int rowNumber, DataColumn col, int colNumber) 
        {
            if (scanCell != null) 
            {
                // Invokes the delegates. 
                scanCell(table, row,  rowNumber, col, colNumber);
            }
        }

        private void onScanColumn (DataTable table, DataColumn col, int colNumber) 
        {
            if (scanColumn != null) 
            {
                // Invokes the delegates. 
                scanColumn(table, col, colNumber);
            }
        }

        public long alias2UID(string alias, long defaultUID)
        {
            return ch.psoft.Util.Validate.GetValid(lookup("UID", "UID_ALIAS", "ALIAS='" + alias + "'", false), defaultUID);
        }

        public string UID2Tablename(long UID)
        {
            return ch.psoft.Util.Validate.GetValid(lookup("TABLENAME", "UID_KEY", "UID=" + UID, false), "");
        }

        public string UID2DetailURL(long UID)
        {
            string detailURL = ch.psoft.Util.Validate.GetValid(lookup("DETAIL_URL", "TABLEEXT", "TABLENAME='" + UID2Tablename(UID) + "'", false), "");
            return detailURL.Replace("%25ID","%ID").Replace("%ID", UID2ID(UID).ToString());
        }

        public long UID2ID(long UID)
        {
            return ch.psoft.Util.Validate.GetValid(lookup("ROW_ID", "UID_KEY", "UID=" + UID, false), -1);
        }

        public long ID2UID(long ID, string tablename){
            return ch.psoft.Util.Validate.GetValid(lookup("UID", tablename, "ID=" + ID, false), -1L);
        }

        /// <summary>
        /// Converts a UID into a human-readable string consisting of the title.
        /// </summary>
        /// <param name="UID">UID to convert</param>
        /// <returns></returns>
        public string UID2NiceName(long UID){
            return UID2NiceName(UID, null, false);
        }

        /// <summary>
        /// Converts a UID into a human-readable string consisting of the title and the type of the object.
        /// </summary>
        /// <param name="UID">UID to convert</param>
        /// <param name="map">Language-mapper</param>
        /// <returns></returns>
        public string UID2NiceName(long UID, LanguageMapper map){
            return UID2NiceName(UID, map, true);
        }

        /// <summary>
        /// Converts a UID into a human-readable string consisting of the title and the type of the object.
        /// </summary>
        /// <param name="UID">UID to convert</param>
        /// <param name="map">Language-mapper</param>
        /// <param name="showObjectType">If true, appends the type of the objects in brackets.</param>
        /// <returns></returns>
        public string UID2NiceName(long UID, LanguageMapper map, bool showObjectType){
            string niceName = "";
            string tablename = UID2Tablename(UID);
            switch (tablename.ToUpper()){
                case "PERSON":
                    niceName = ch.psoft.Util.Validate.GetValid(Person.getWholeName(UID2ID(UID)), UID.ToString());
                    break;

                default:
                    niceName = ch.psoft.Util.Validate.GetValid(lookup("NICENAME", "UID_KEY", "UID=" + UID, false), UID.ToString());
                    break;
            }
            if (showObjectType){
                string name = map.get("tableName", tablename);

                if (name != "") niceName += " (" + name + ")";
            }
            return niceName;
        }

        /// <summary>
        /// Converts the human-readable string of an object into its UID.
        /// </summary>
        /// <param name="niceName">Human-readable string</param>
        /// <param name="tableName">Tablename of the object</param>
        /// <returns>UID</returns>
        public long NiceName2UID(string niceName, string tableName){
            return ch.psoft.Util.Validate.GetValid(lookup("UID", "UID_KEY", langAttrName("UID_KEY", "NICENAME") + "='" + DBColumn.toSql(niceName) + "' and TABLENAME='" + tableName + "'", false), -1L);
        }

        /// <summary>
        /// Adds a generic assignment between two p-soft-Objects
        /// </summary>
        /// <param name="fromTablename">Tablename of source-object</param>
        /// <param name="fromID">ID of source-object</param>
        /// <param name="toTablename">Tablename of target-object</param>
        /// <param name="toID">ID of target-object</param>
        /// <param name="typ">Type of assignment</param>
        /// <param name="ownerPersonID">ID of owner-person, or -1 if its a global assignment</param>
        /// <param name="structureID">ID of UID-structure to which the new assignment belongs, or -1 if its a general assignment</param>
        /// <returns>ID of new UID-assignment</returns>
        public long addUIDAssignment(string fromTablename, long fromID, string toTablename, long toID, int typ, long ownerPersonID, long structureID){
            return addUIDAssignment(ID2UID(fromID, fromTablename), ID2UID(toID, toTablename), typ, ownerPersonID, structureID);
        }

        /// <summary>
        /// Adds a generic assignment between two p-soft-Objects
        /// </summary>
        /// <param name="fromUID">UID of source-object</param>
        /// <param name="toUID">UID of target-object</param>
        /// <param name="typ">Type of assignment</param>
        /// <param name="ownerPersonID">ID of owner-person, or -1 if its a global assignment</param>
        /// <param name="structureID">ID of UID-structure to which the new assignment belongs, or -1 if its a general assignment</param>
        /// <returns>ID of new UID-assignment or -1 if it already exists</returns>
        public long addUIDAssignment(long fromUID, long toUID, int typ, long ownerPersonID, long structureID){
            long newID = -1L;
            if (lookup("ID", "UID_ASSIGNMENT", "FROM_UID=" + fromUID + " and TO_UID=" + toUID + " and TYP=" + typ + " and OWNER_PERSON_ID" + (ownerPersonID > 0? "=" + ownerPersonID.ToString():" is null") + " and UID_STRUCTURE_ID" + (structureID > 0? "=" + structureID.ToString():" is null"), false) == ""){
                newID = newId("UID_ASSIGNMENT");
                execute("insert into UID_ASSIGNMENT (FROM_UID, TO_UID, TYP, OWNER_PERSON_ID, UID_STRUCTURE_ID) values (" + fromUID + ", " + toUID + ", " + typ + ", " + (ownerPersonID > 0? ownerPersonID.ToString():"null") + ", " + (structureID > 0? structureID.ToString():"null") + ")");
            }
            return newID;
        }

        /// <summary>
        /// Removes a generic assignment between two p-soft-Objects
        /// </summary>
        /// <param name="fromTablename">Tablename of source-object</param>
        /// <param name="fromID">ID of source-object</param>
        /// <param name="toTablename">Tablename of target-object</param>
        /// <param name="toID">ID of target-object</param>
        /// <param name="typ">Type of assignment or -1 to remove any type</param>
        /// <param name="ownerPersonID">ID of owner-person, or -1 if its a global assignment</param>
        /// <param name="structureID">ID of UID-structure to which the new assignment belongs, or -1 if its a general assignment</param>
        /// <returns>ID of new UID-assignment</returns>
        public void removeUIDAssignment(string fromTablename, long fromID, string toTablename, long toID, int typ, long ownerPersonID, long structureID){
            removeUIDAssignment(ID2UID(fromID, fromTablename), ID2UID(toID, toTablename), typ, ownerPersonID, structureID);
        }

        /// <summary>
        /// Returns an array of UIDs of all assigned p-soft-Objects
        /// </summary>
        /// <param name="fromTablename">Tablename of source-object</param>
        /// <param name="fromID">ID of source-object</param>
        /// <param name="toTablenames">Array of target-tablenames to consider, or null if all should be considered</param>
        /// <param name="excludedToTablenames">Array of target-tablenames to exclude, or null if none should be excluded</param>
        /// <param name="typ">Type of assignment or -1 for any type</param>
        /// <param name="ownerPersonID">ID of owner-person, or -1 for global assignments</param>
        /// <param name="structureID">ID of UID-structure to which the new assignment belongs, or -1 if its a general assignment</param>
        /// <returns>Array of target-UIDs</returns>
        public long[] getUIDAssignments(string fromTablename, long fromID, string[] toTablenames, string[] excludedToTablenames, int typ, long ownerPersonID, long structureID){
            string sql = "select TO_UID from UID_ASSIGNMENT_V where FROM_TABLENAME='" + fromTablename + "' and FROM_ROW_ID=" + fromID + (typ == ASSIGNMENT.ANY? "" : " and TYP=" + typ) + " and OWNER_PERSON_ID" + (ownerPersonID>0? "=" + ownerPersonID : " is null") + " and UID_STRUCTURE_ID" + (structureID > 0? "=" + structureID: " is null");
            if (toTablenames != null){
                sql += " and TO_TABLENAME in (";
                bool isFirst = true;
                foreach(string toTablename in toTablenames){
                    if (isFirst){
                        isFirst = false;
                    }
                    else{
                        sql += ",";
                    }
                    sql += "'" + toTablename + "'";
                }
                sql += ")";
            }
            if (excludedToTablenames != null){
                sql += " and TO_TABLENAME not in (";
                bool isFirst = true;
                foreach(string excludedToTablename in excludedToTablenames){
                    if (isFirst){
                        isFirst = false;
                    }
                    else{
                        sql += ",";
                    }
                    sql += "'" + excludedToTablename + "'";
                }
                sql += ")";
            }
            DataTable table = getDataTable(sql, "UID_ASSIGNMENT_V");
            long[] retValue = new long[table.Rows.Count];
            for (int i=0; i<table.Rows.Count; i++){
                retValue[i] = DBColumn.GetValid(table.Rows[i][0], -1L);
            }
            return retValue;
        }

        /// <summary>
        /// Removes a generic assignment between two p-soft-Objects
        /// </summary>
        /// <param name="fromUID">UID of source-object</param>
        /// <param name="toUID">UID of target-object</param>
        /// <param name="typ">Type of assignment or -1 to remove any type</param>
        /// <param name="ownerPersonID">ID of owner-person, or -1 if its a global assignment</param>
        /// <param name="structureID">ID of UID-structure to which the new assignment belongs, or -1 if its a general assignment</param>
        public void removeUIDAssignment(long fromUID, long toUID, int typ, long ownerPersonID, long structureID){
            execute("delete from UID_ASSIGNMENT where FROM_UID=" + fromUID + " and TO_UID=" + toUID + (typ == ASSIGNMENT.ANY? "" : " and TYP=" + typ) + " and OWNER_PERSON_ID" + (ownerPersonID > 0? "=" + ownerPersonID : " is null") + " and UID_STRUCTURE_ID" + (structureID > 0? "=" + structureID: " is null"));
        }

        /// <summary>
		/// Lookup using LOOKUP table
		/// </summary>
		/// <param name="lookupMnemoName">Mnemonic name of lookup definition from LOOKUP table (LEITERABTEILUNG)</param>
		/// <param name="lookupParamName">Param to replace in lookup definition by lookupParamValue (%ID)</param>
		/// <param name="lookupParamValue">Param value to replace in lookup definition (122)</param>
		/// <param name="lookupId">Append to result fields for lookup definition (last field)</param>
		/// <returns>see SQLDB.lookup</returns>
		public ArrayList lookupMnemo(string lookupMnemoName, string lookupParamName, string lookupParamValue, string lookupId) 
		{
			//restrive lookup string for mnemonic name
			String _lookupString = getLookupText(lookupMnemoName);

			if ((_lookupString == null) || (_lookupString == "")) 
			{
				return null;
			}

			//parsing lookup params
			//{lookup param1=FIELD param2=TABLE_NAME param3=CONDITION}
			string _field = "";
			string _tableName = "";
			string _condition = "";

			string _lookupStr = "{lookup";
			if (_lookupString.StartsWith(_lookupStr))
			{
				int _pos = _lookupString.IndexOf(" param1=") + " param1=".Length;
				int _len = _lookupString.IndexOf(" param2=") - _pos;
				_field = _lookupString.Substring(_pos, _len); 

				_pos = _lookupString.IndexOf(" param2=") + " param2=".Length;
				_len = _lookupString.IndexOf(" param3=") - _pos;
				_tableName = _lookupString.Substring(_pos, _len); 
			
				_pos = _lookupString.IndexOf(" param3=") + " param3=".Length;
				_len = _lookupString.IndexOf("}") - _pos;
				_condition = _lookupString.Substring(_pos, _len);

				if ((lookupParamName != null) && (lookupParamName != "")) 
				{
					_condition = _condition.Replace(lookupParamName, lookupParamValue);
				}

				if ((lookupId != null) && (lookupId != "")) 
				{
					return lookup(new string[] {_field, lookupId}, _tableName, _condition, "", false);
				}
				else 
				{
					return lookup(new string[] {_field}, _tableName, _condition, "", false);
				}
			}

			return null;
		}

		public string getLookupText(string lookupMnemoName) 
		{
			return (String)base.lookup("TEXT", "LOOKUP", "MNEMO='" + lookupMnemoName + "'");
		}

		protected override void loadExtensions(DataTable table, DataColumn col, bool firstColumn) 
		{
			base.loadExtensions(table, col, firstColumn);

			if (!col.ExtendedProperties["Format"].Equals(""))
			{
				SQLColumn.InputDataType dataType = SQLColumn.GetBaseType(col);

				if (dataType == SQLColumn.InputDataType.Double
					|| dataType == SQLColumn.InputDataType.Integer
					|| dataType == SQLColumn.InputDataType.Long
					)
				{
					col.ExtendedProperties["InputControlType"] = typeof(TwoValuesTextBox);
				}
			}
		}

        /// <summary>
        /// Return SQL join-clause to directly filter the recordset by access-rights.
        /// Attention: make sure to use "distinct columnname" (and not "distinct *") in the select-clause to avoid doubles.
        /// </summary>
        /// <param name="tablename">Table-name to join</param>
        /// <param name="authorisation">Authorisation to filter</param>
        /// <param name="applicationRight">Application-right to filter</param>
        /// <param name="recursive">true : considers also groups where the user belongs to, false: considers only the user's rights</param>
        /// <param name="hierarchic">true : hierarchic inheritence of table-rights (row inherits from table), false: no hierarchic inheritence of table-rights</param>
        /// <returns>the SQL join-clause</returns>
        public string getAccessRightsRowInnerJoinSQL(string tablename, int authorisation, int applicationRight, bool recursive, bool hierarchic){
            return " inner join ACCESS_RIGHT_RT on ACCESS_RIGHT_RT.TABLENAME='" + tablename + "' and (ACCESS_RIGHT_RT.ROW_ID=" + tablename + ".ID" + (hierarchic? " or ACCESS_RIGHT_RT.ROW_ID=0" : "") + ") and ACCESS_RIGHT_RT.APPLICATION_RIGHT=" + applicationRight + " and (ACCESS_RIGHT_RT.AUTHORISATION&" + authorisation + ")=" + authorisation + " and ACCESS_RIGHT_RT.ACCESSOR_ID " + (recursive? userAccessorIDsSQLInClause: "=" + _userAccessorID);
        }

        /// <summary>
        /// Column wird nicht angezeigt
        /// </summary>
        /// <param name="visibilities">Sichtbarkeiten aller Columns als Bitmap</param>
        /// <param name="authorisations">Berechtigungen aller Columns als Bitmap</param>
        /// <param name="columnMask">Column-Maske</param>
        /// <param name="tableName">nur für Log-Eintrag</param>
        /// <param name="columnName">nur für Log-Eintrag</param>
        /// <returns></returns>
        private bool hasNoAccess(
            ulong visibilities, 
            ulong authorisations, 
            ulong columnMask, 
            string tableName, 
            string columnName
        )
        {
            bool notVisible = (visibilities & columnMask) == 0;

            if (notVisible)
            {
                Logger.Log(tableName + "." + columnName + " is not Visible.", Logger.VERBOSE);
            }

            bool noAuthorisation = (authorisations & columnMask) == 0;

            if (noAuthorisation)
            {
                Logger.Log("The user has no authorisation for " + tableName + "." + columnName + ".", Logger.VERBOSE);
            }

            return notVisible || noAuthorisation;
        }
	}
}
