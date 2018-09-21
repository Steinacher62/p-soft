using ch.appl.psoft.db;
using ch.psoft.Util;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Orgentity.
    /// </summary>
    public class Orgentity : DBObject {
        public const string TableName = "ORGENTITY";

        public const string SelectRowWithId = "select * from " + TableName + " where id=";
        public const string DeleteRowWithId = "delete from " + TableName + " where id=";
        public const string SelectIdWithParentId = "select id from " + TableName + " where parent_id=";

        public const string TABLE_TASKLIST_GROUP = "TASKLIST_GROUP_TASKLIST";
        public const string TABLE_CONTACT_GROUP_PERSON = "CONTACT_GROUP_PERSON";
        public const string TABLE_CONTACT_GROUP_FIRM = "CONTACT_GROUP_FIRM";
        public const string TABLE_PROJECT_GROUP = "PROJECT_GROUP_PROJECT";

        public const string ID_TASKLIST_GROUP = "TASKLIST_GROUP_ID";
        public const string ID_CONTACT_GROUP = "CONTACT_GROUP_ID";
        public const string ID_PROJECT_GROUP = "PROJECT_GROUP_ID";

        private bool _employmentOrgentity_idExists = false;

        /// <summary>
        /// Holds a cache-entry of all sub-OE IDs
        /// </summary>
        protected class SubOEIDsCacheEntry : DBCacheEntry{
            private string _oeIDs = "";
            private string _subOEIDs = "";

            public SubOEIDsCacheEntry(string oeIDs) : base(60){ // the cache-entry remains fresh for 1 minute.
                _oeIDs = oeIDs;
            }

            protected override void onRefreshing(DBData db){
                _subOEIDs = db.Tree(TableName).AddAllSubnodes(_oeIDs);
            }

            public string getSubOEIDs(DBData db){
                lock (this){
                    refreshIfStale(db);
                    return _subOEIDs;
                }
            }
        }

        public Orgentity(DBData db, HttpSessionState session) : base(db, session) {
            _employmentOrgentity_idExists = ch.psoft.Util.Validate.GetValid(_db.lookup("OBJID", "DESCRIBEOBJV", "TABLENAME='EMPLOYMENT' and COLNAME='ORGENTITY_ID'", false), -1L) != -1L;
        }

        private SubOEIDsCacheEntry getSubOEIDsCacheEntry(string oeIDs){
            SubOEIDsCacheEntry cacheEntry = null;
            lock (_db.CacheSyncRoot){
                string key = "SubOEIDs_" + oeIDs;
                cacheEntry = (SubOEIDsCacheEntry) _db.getCacheEntry(key);
                if (cacheEntry == null){
                    cacheEntry = new SubOEIDsCacheEntry(oeIDs);
                    _db.addCacheEntry(key, cacheEntry);
                }
            }
            return cacheEntry;
        }

        /// <summary>
        /// Adds to a comma-separated list of OrgEntity IDs all subordinate OrgEntity IDs.
        /// </summary>
        /// <param name="oeIDs">Comma-separated list of IDs</param>
        /// <returns>Comma-separated list of IDs</returns>
        public string addAllSubOEIDs(string oeIDs){
            return getSubOEIDsCacheEntry(oeIDs).getSubOEIDs(_db);
        }

        /// <summary>
        /// Get root of main-organisation
        /// </summary>
        /// <returns>root id</returns>
        public long getRootID() {
            return ch.psoft.Util.Validate.GetValid(_db.lookup("ORGENTITY_ID", "ORGANISATION", "MAINORGANISATION=1", false), -1L);
        }

        /// <summary>
        /// Get roor of orgentity
        /// </summary>
        /// <param name="oeId"></param>
        /// <returns>root id</returns>
        public long getRootID(long oeId) {
            return _db.lookup("ROOT_ID", "ORGENTITY", "ID="+oeId,-1L);
        }

        public long find(long root, long search) 
        {
            return _db.Tree(TableName).Find(root,search);
        }

        public override int delete(long id, bool cascade) {
            int retValue = 0;

            // delete sub-registries...
            string sql = SelectIdWithParentId + id;
            DataTable table = _db.getDataTable(sql);
            foreach (DataRow row in table.Rows) {
                retValue += delete(ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L), cascade);
            }

            // deleting dependent jobs...
            table = _db.getDataTable("select ID from JOB where ORGENTITY_ID=" + id);
            foreach (DataRow row in table.Rows) {
                retValue += _db.Job.delete(ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L), cascade);
            }
	                            
            // deleting dependent chartnodes...
            table = _db.getDataTable("select ID from CHARTNODE where ORGENTITY_ID=" + id);
            foreach (DataRow row in table.Rows) {
                retValue += _db.ChartNode.delete(ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L), cascade);
            }
	                            
            // update affected organisation...
            _db.execute("update ORGANISATION set ORGENTITY_ID=null where ORGENTITY_ID=" + id);

            // Zuständig für Lohn annullieren
            if (_employmentOrgentity_idExists) {
                _db.execute("update EMPLOYMENT set ORGENTITY_ID=null where ORGENTITY_ID=" + id);
            }

            if (_db.dbObjectExists("BUDGET",'U')) {
                // deleting dependent budgets...
                table = _db.getDataTable("select ID from BUDGET where ORGENTITY_ID=" + id);
                foreach (DataRow row in table.Rows) {
					retValue += _db.Lohn.disconnectLohnkomponenten(DBColumn.GetValid(row[0], -1L));
                    retValue += _db.Lohn.deleteBudget(DBColumn.GetValid(row[0], -1L));
                }
            }

            if (_db.dbObjectExists("OBJECTIVE",'U')) {
                // deleting dependent objectives...
                table = _db.getDataTable("select ID from OBJECTIVE where ORGENTITY_ID=" + id);
                foreach (DataRow row in table.Rows) {
                    retValue += _db.Objective.delete(DBColumn.GetValid(row[0], -1L), cascade);
                }
            }

            retValue += _db.execute(DeleteRowWithId + id);
        
            return retValue;
        }


        public long getOrgentityGroupID(string groupIdColumn,long orgentityID)
        {
            return DBColumn.GetValid(_db.lookup(groupIdColumn,TableName,"ID="+orgentityID),0L);
        }
        /// <summary>
        /// Adds an entry into an assignment table (DB)
        /// </summary>
        /// <param name="orgentityID">ID of the orgentity</param>
        /// <param name="tableName">Name of the assignment table</param>
        /// <param name="rowID">ID of the object assigning the group</param>
        /// <returns></returns>
        public long addOrgentityGroupAssignmentEntry(long orgentityID, string tableName, long rowID) // rowID eg. tasklist
        {
            long newID = -1;
            switch(tableName)
            {
                case TABLE_TASKLIST_GROUP:
                    newID = _db.newId(tableName);
                    _db.execute("insert into "+tableName+" (ID,TASKLIST_GROUP_ID,TASKLIST_ID) values ("+newID+","+getOrgentityGroupID(ID_TASKLIST_GROUP,orgentityID)+","+rowID+")");
                    break;
                case TABLE_CONTACT_GROUP_FIRM:
                    newID = _db.newId(tableName);
                    _db.execute("insert into "+tableName+" (ID,CONTACT_GROUP_ID,FIRM_ID) values ("+newID+","+getOrgentityGroupID(ID_CONTACT_GROUP,orgentityID)+","+rowID+")");
                    break;
                case TABLE_CONTACT_GROUP_PERSON:
                    newID = _db.newId(tableName);
                    _db.execute("insert into "+tableName+" (ID,CONTACT_GROUP_ID,PERSON_ID) values ("+newID+","+getOrgentityGroupID(ID_CONTACT_GROUP,orgentityID)+","+rowID+")");
                    break;
                case TABLE_PROJECT_GROUP:
                    newID = _db.newId(tableName);
                    _db.execute("insert into "+tableName+" (ID,PROJECT_GROUP_ID,PROJECT_ID) values ("+newID+","+getOrgentityGroupID(ID_PROJECT_GROUP,orgentityID)+","+rowID+")");
                    break;
            }
            return newID;
        }

        /// <summary>
        /// Removes an entry from an assignment table (DB)
        /// </summary>
        /// <param name="orgentityIDs">ID of the orgentity</param>
        /// <param name="tableName">Name of the assignment table</param>
        /// <param name="rowID">ID of the object assigning the group</param>
        /// <returns></returns>
        public bool removeOrgentityGroupAssignmentEntry(long orgentityID, string tableName, long rowID)
        {
            switch(tableName)
            {
                case TABLE_TASKLIST_GROUP:
                    _db.execute("delete from "+tableName+" where "+ID_TASKLIST_GROUP+"="+getOrgentityGroupID(ID_TASKLIST_GROUP,orgentityID)+" and TASKLIST_ID="+rowID);
                    break;
                case TABLE_CONTACT_GROUP_FIRM:
                    _db.execute("delete from "+tableName+" where "+ID_CONTACT_GROUP+"="+getOrgentityGroupID(ID_CONTACT_GROUP,orgentityID)+" and FIRM_ID="+rowID);
                    break;
                case TABLE_CONTACT_GROUP_PERSON:
                    _db.execute("delete from "+tableName+" where "+ID_CONTACT_GROUP+"="+getOrgentityGroupID(ID_CONTACT_GROUP,orgentityID)+" and PERSON_ID="+rowID);
                    break;
                case TABLE_PROJECT_GROUP:
                    _db.execute("delete from "+tableName+" where "+ID_PROJECT_GROUP+"="+getOrgentityGroupID(ID_PROJECT_GROUP,orgentityID)+" and PROJECT_ID="+rowID);
                    break;
            }
            return true;
        }

        /// <summary>
        /// Updates several assignment table entries in the DB
        /// </summary>
        /// <param name="orgentityIDs">IDs (','-separated) of the assignments. A negativ ID represents a deassignment.</param>
        /// <param name="tableName">Name of the assignment table</param>
        /// <param name="rowID">ID of the object assigning the group</param>
        /// <returns></returns>
        public int updateOrgentityGroupAssignmentEntries(string orgentityIDs, string tableName, long rowID)
        {
            int retValue = 0;
            foreach (string strOrgentityID in orgentityIDs.Split(',')) 
            {
                long orgentityID = ch.psoft.Util.Validate.GetValid(strOrgentityID, 0L);
                if (orgentityID > 0)
                {
                    addOrgentityGroupAssignmentEntry(orgentityID, tableName, rowID);
                    retValue++;
                }
                else if (orgentityID < 0)
                {
                    removeOrgentityGroupAssignmentEntry(-orgentityID, tableName, rowID);
                    retValue++;
                }
            }

            return retValue;
        }


        /// <summary>
        /// Gibt alle Registraturen eines oder mehrerer Tabelleneinträge
        /// </summary>
        /// <param name="IDs">Liste von IDs, getrennt durch ','</param>
        /// <param name="tableName">Name der zugeordneten Tabelle</param>
        /// <returns>Liste der zugeordneten Registraturen Id's, getrennt durch ','</returns>
        public string getOrgentityIDs(string IDs, string tableName) {
            string result = "";

            if (IDs == "")
            {
                return "";
            }
            string sql = "";
            switch(tableName)
            {
                case TABLE_TASKLIST_GROUP:
                    sql = "select distinct o.ID from "+TableName+" o inner join "+TABLE_TASKLIST_GROUP+" t on o."+ID_TASKLIST_GROUP+" = t."+ID_TASKLIST_GROUP+" and t.TASKLIST_ID in ("+IDs+")";
                    break;
                case TABLE_CONTACT_GROUP_FIRM:
                    sql = "select distinct o.ID from "+TableName+" o inner join "+TABLE_CONTACT_GROUP_FIRM+" t on o."+ID_CONTACT_GROUP+" = t."+ID_CONTACT_GROUP+" and t.FIRM_ID in ("+IDs+")";
                    break;
                case TABLE_CONTACT_GROUP_PERSON:
                    sql = "select distinct o.ID from "+TableName+" o inner join "+TABLE_CONTACT_GROUP_PERSON+" t on o."+ID_CONTACT_GROUP+" = t."+ID_CONTACT_GROUP+" and t.PERSON_ID in ("+IDs+")";
                    break;
                case TABLE_PROJECT_GROUP:
                    sql = "select distinct o.ID from "+TableName+" o inner join "+TABLE_PROJECT_GROUP+" t on o."+ID_PROJECT_GROUP+" = t."+ID_PROJECT_GROUP+" and t.PROJECT_ID in ("+IDs+")";
                    break;
            }
               
            DataTable data = _db.getDataTable(sql,Logger.VERBOSE);
            
            foreach (DataRow row in data.Rows) {
                result += ","+row[0];
            }

            if (result.Length > 0)
            {
                return result.Substring(1);
            }

            return "";
        }

        /// <summary>
        /// get employment
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>=-1: vakant, =0: MA, =1: leader</returns>
        public int getEmployment(long userId) {
            return DBColumn.GetValid(_db.lookup("job.typ","job inner join employment emp on job.employment_id = emp.id","emp.person_id="+userId),-1);
        }

        /// <summary>
        /// Get employment
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oeId"></param>
        /// <returns>=-1: undefined, =0: MA, =1: leader</returns>
        public int getEmployment(long userId, long oeId) {
            return DBColumn.GetValid(_db.lookup("job.typ","job inner join employment emp on job.employment_id = emp.id","job.orgentity_id = "+oeId+" and emp.person_id="+userId),-1);
        }

        /// <summary>
        /// Has oe's ?
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool hasOEs(long userId) {
            return DBColumn.GetValid(_db.lookup("oe.id","orgentity oe inner join oepersonv p on oe.parent_id = p.oe_id","p.id="+userId),0L) > 0L;
        }

        /// <summary>
        /// Has jobs ?
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool hasJobs(long userId) {
            return DBColumn.GetValid(_db.lookup("job.id","job inner join employment emp on job.employment_id = emp.id","emp.person_id = "+userId),0L) > 0L;
        }

        /// <summary>
        /// Has persons ?
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool hasPersons(long userId) {
            return DBColumn.GetValid(_db.lookup("emp.person_id","job inner join employment emp on job.employment_id = emp.id","emp.person_id = "+userId),0L) > 0L;
        }

        /// <summary>
        /// Has child oe's ?
        /// </summary>
        /// <param name="oeId"></param>
        /// <returns></returns>
        public bool hasChildOEs(long oeId) {
            return DBColumn.GetValid(_db.lookup("id","orgentity","parent_id="+oeId),0L) > 0L;
        }
        /// <summary>
        /// Has child jobs ?
        /// </summary>
        /// <param name="oeId"></param>
        /// <returns></returns>
        public bool hasChildJobs(long oeId) {
            long id = DBColumn.GetValid(_db.lookup("id","job","orgentity_id="+oeId),0L);
            if (id > 0) return true;
            return DBColumn.GetValid(_db.lookup("job.id","job inner join orgentity oe on job.orgentity_id = oe.id","oe.parent_id="+oeId),0L) > 0;
        }

        /// <summary>
        /// Get orgentities
        /// </summary>
        public DataTable orgentities {
            get { return _db.getDataTable("select id,"+_db.langAttrName("ORGENTITY","TITLE")+" from ORGENTITY order by "+_db.langAttrName("ORGENTITY","TITLE"));}
        }

        /// <summary>
        /// Get jobs
        /// </summary>
        public DataTable jobs {
            get { return _db.getDataTable("select id,"+_db.langAttrName("JOB","TITLE")+" from JOB order by "+_db.langAttrName("JOB","TITLE"));}
        }

        public long getLeaderGroupAccessorID(long oeID){
            return _db.getLeaderGroupAccessorID(TableName, oeID);
        }

        public long getMemberGroupAccessorID(long oeID){
            return _db.getMemberGroupAccessorID(TableName, oeID);
        }

        /// <summary>
        /// Summiert Werte im OE-Tree hierarchisch (über alle Knoten und den Startknoten)
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="nodeValue">Liefert den zu summierenden Wert (delegate)</param>
        /// <returns></returns>
        public static double GetHierarchicOrgentityTreeSum(
            long orgentityId,
            Tree.NodeValue nodeValue,
            params object[] parameterList
        )
        {
            DBData db = DBData.getDBData();
            Tree tree = db.Tree("ORGENTITY");
            return tree.getHierarchicTreeSum(orgentityId, nodeValue, parameterList);
        }

        /// <summary>
        /// Summiert Werte im OE-Tree
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="nodeValue">Liefert den zu summierenden Wert (delegate)</param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static double GetOrgentityTreeSum(
            long orgentityId,
            Tree.NodeValue nodeValue,
            Tree.TreeSumMode mode,
            params object[] parameterList
        )
        {
            DBData db = DBData.getDBData();
            Tree tree = db.Tree("ORGENTITY");
            return tree.getTreeSum(orgentityId, nodeValue, mode, parameterList);
        }
    }
}
