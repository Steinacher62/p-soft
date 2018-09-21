using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Registry.
    /// </summary>
    public class Registry : DBObject {
        public const string TableName = "REGISTRY";

        public const string SelectRowWithId = "select * from " + TableName + " where id=";
        public const string DeleteRowWithId = "delete from " + TableName + " where id=";
        public const string SelectIdWithParentId = "select id from " + TableName + " where parent_id=";

        public Registry(DBData db, HttpSessionState session) : base(db, session) { }

        public long getRootID() 
        {
            return ch.psoft.Util.Validate.GetValid(_db.lookup("WERT", "PROPERTY", "GRUPPE='system' and TITLE='registryRootID'", false), -1L);
        }

        public string getTitle(long ID){
            return _db.lookup("TITLE", TableName, "ID=" + ID, false);
        }

        public long find(long root, long search) 
        {
            return _db.Tree(TableName).Find(root,search);
        }

        public override int delete(long id, bool rootEnable) {
            int retValue = 0;

            // delete sub-registries...
            string sql = SelectIdWithParentId + id;
            DataTable table = _db.getDataTable(sql);

            foreach (DataRow row in table.Rows) {
                retValue += delete(ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L), rootEnable);
            }

            if (retValue > 0)
                retValue += _db.execute(DeleteRowWithId + id);
        
            return retValue;
        }

        /// <summary>
        /// Copies all registry-entries from one object to another
        /// </summary>
        /// <param name="sourceTableName">Table-name of the source-object</param>
        /// <param name="sourceRowID">ID of the source-object</param>
        /// <param name="targetTableName">Table-name of the target-object</param>
        /// <param name="targetRowID">ID of the target-object</param>
        /// <returns>Nr of registry-entries copied.</returns>
        public int copyRegistryEntries(string sourceTableName, long sourceRowID, string targetTableName, long targetRowID){
            return copyRegistryEntries(_db.ID2UID(sourceRowID, sourceTableName), _db.ID2UID(targetRowID, targetTableName));
        }

        /// <summary>
        /// Copies all registry-entries from one object to another
        /// </summary>
        /// <param name="sourceUID">UID of source-object</param>
        /// <param name="targetUID">UID of target-object</param>
        /// <returns>Nr of registry-entries copied.</returns>
        public int copyRegistryEntries(long sourceUID, long targetUID){
            int retValue = 0;
            DataTable table = _db.getDataTable("select REGISTRY_ID from REGISTRY_ENTRY where OBJECT_UID=" + sourceUID);

            foreach (DataRow row in table.Rows) {
                try {
                    addRegistryEntry(DBColumn.GetValid(row[0], -1L), targetUID);
                    retValue ++;
                }
                catch (Exception e) {
                    Logger.Log(e, Logger.ERROR);
                }
            }
            return retValue;
        }

        /// <summary>
        /// Adds a registry-entry to the DB
        /// </summary>
        /// <param name="registryID">ID of the registry</param>
        /// <param name="tableName">Name of the table to registe</param>
        /// <param name="rowID">ID of the object to register</param>
        /// <returns>ID of the new registry-entry</returns>
        public long addRegistryEntry(long registryID, string tableName, long rowID) {
            return addRegistryEntry(registryID, _db.ID2UID(rowID, tableName));
        }

        /// <summary>
        /// Adds a registry-entry to the DB
        /// </summary>
        /// <param name="registryID">ID of the registry</param>
        /// <param name="objectUID">UID of the object to register</param>
        /// <returns>ID of the new registry-entry</returns>
        public long addRegistryEntry(long registryID, long objectUID){
            long entryID = -1L;
            if (objectUID > 0){
                entryID = _db.newId("REGISTRY_ENTRY");
                _db.execute("insert into REGISTRY_ENTRY (ID, REGISTRY_ID, OBJECT_UID) values (" + entryID + "," + registryID + "," + objectUID + ")");
            }
            return entryID;
        }

        /// <summary>
        /// Removes a registry-entry from the DB
        /// </summary>
        /// <param name="registryID">ID of the registry</param>
        /// <param name="tableName">Name of the registered table</param>
        /// <param name="rowID">ID of the registered object</param>
        /// <returns></returns>
        public bool removeRegistryEntry(long registryID, string tableName, long rowID)
        {
            return removeRegistryEntry(registryID, _db.ID2UID(rowID, tableName));
        }

        /// <summary>
        /// Removes a registry-entry from the DB
        /// </summary>
        /// <param name="registryID">ID of the registry</param>
        /// <param name="objectUID">UID of the registered object</param>
        /// <returns></returns>
        public bool removeRegistryEntry(long registryID, long objectUID){
            _db.execute("delete from REGISTRY_ENTRY where REGISTRY_ID=" + registryID + " and OBJECT_UID=" + objectUID);
            return true;
        }

        /// <summary>
        /// Updates several registry-entries in the DB
        /// </summary>
        /// <param name="registryIDs">IDs (','-separated) of the registries. A negativ ID represents a deregistration.</param>
        /// <param name="tableName">Name of the registered table</param>
        /// <param name="rowID">ID of the registered object</param>
        /// <returns></returns>
        public int updateRegistryEntries(string registryIDs, string tableName, long rowID)
        {
            return updateRegistryEntries(registryIDs, _db.ID2UID(rowID, tableName));
        }

        /// <summary>
        /// Updates several registry-entries in the DB
        /// </summary>
        /// <param name="registryIDs">IDs (','-separated) of the registries. A negativ ID represents a deregistration.</param>
        /// <param name="objectUID">UID of the registered object</param>
        /// <returns></returns>
        public int updateRegistryEntries(string registryIDs, long objectUID){
            int retValue = 0;

            foreach (string strRegistryID in registryIDs.Split(',')) {
                long registryID = ch.psoft.Util.Validate.GetValid(strRegistryID, 0L);
                if (registryID > 0) {
                    addRegistryEntry(registryID, objectUID);
                    retValue++;
                }
                else if (registryID < 0) {
                    removeRegistryEntry(-registryID, objectUID);
                    retValue++;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Ergänzt eine Liste von Registry-IDs durch die ID's aller untergeordneten
        /// Registries
        /// </summary>
        /// <param name="registryIDs">Liste von Registry-Ids, getrennt durch ','</param>
        /// <returns>ergänzte Liste, getrennt durch ','</returns>
        public string addAllSubRegistries(string registryIDs) {
            return _db.Tree(TableName).AddAllSubnodes(registryIDs);
        }

        /// <summary>
        /// Ergänzt eine Liste von Registry-IDs durch die ID's aller untergeordneten
        /// Registries in der Searchresult-tabelle
        /// </summary>
        /// <param name="registryIDs">Liste von Registry-IDs, getrennt durch ','</param>
        /// <param name="resultID">SearchResult-ID</param>
        /// <returns>#rows inserted</returns>
        public int addAllSubRegistries(string registryIDs, long resultID) {
            return _db.Tree(TableName).AddAllSubnodes(registryIDs, resultID);
        }

        /// <summary>
        /// Ergänzt eine Liste von Registry-Ids durch die Id's aller untergeordneten
        /// Registries in der Searchresult-tabelle
        /// </summary>
        /// <param name="resultID">Target SearchResult-ID</param>
        /// <returns>#rows inserted</returns>
        public int addAllSubRegistries(long resultID) {
            return _db.Tree(TableName).AddAllSubnodes(resultID);
        }

        /// <summary>
        /// Ergänzt eine Liste von Registry-IDs durch die IDs aller übergeordneten Registries.
        /// </summary>
        /// <param name="registryIDs">Liste von Registry-Ids, getrennt durch ','</param>
        /// <returns>ergänzte Liste, getrennt durch ','</returns>
        public string addAllParentRegistries(string registryIDs){
            ArrayList list = new ArrayList();
            string[] registryIDarr = registryIDs.Split(',');
            foreach (string registryID in registryIDarr){
                list.AddRange(getParentRegistryIDList(long.Parse(registryID), true));
            }
            string retValue = "";
            for (int i=0; i<list.Count; i++){
                if (i>0){
                    retValue += ",";
                }
                retValue += list[i].ToString();
            }
            return retValue;
        }

        /// <summary>
        /// Returns list of parent Registry-IDs (path) corresponding to a certain Registry
        /// </summary>
        /// <param name="registryID">ID of a Registry</param>
        /// <param name="inclRegistry">true: inclusive registryID (default false)</param>
        /// <returns></returns>
        public long[] getParentRegistryIDs(long registryID, params bool[] inclRegistry) {
            return (long[]) getParentRegistryIDList(registryID, inclRegistry).ToArray(typeof(long));
        }
        /// <summary>
        /// Returns list of parent Registry-IDs (path) corresponding to a certain Registry
        /// </summary>
        /// <param name="registryID">ID of a Registry</param>
        /// <param name="inclRegistry">true: inclusive registryID (default false)</param>
        /// <returns></returns>
        public ArrayList getParentRegistryIDList(long registryID, params bool[] inclRegistry) {
            bool incl = inclRegistry.Length > 0 ? inclRegistry[0] : false;

            return _db.Tree(TableName).GetPath(registryID,incl);
        }

        /// <summary>
        /// Für eine Liste von Registry-IDs wird eine ID-Liste der registrierten 
        /// (zugeordneten) Tabelleneinträge unter Berücksichtigung der Vererbung bestimmt
        /// </summary>
        /// <param name="registryIDs">Liste von Registry-Ids des Registraturbaumes, getrennt durch ','.</param>
        /// <param name="tableName">Name der zugeordneten Tabelle</param>
        /// <param name="defaultID">fak. Default ID falls liste leer ist.</param>
        /// <returns>Liste der zugeordneten Knoten Id's, getrennt durch ','.</returns>
        public string getRegisteredIDs(string registryIDs, string tableName, long defaultID) {
            return getRegisteredIDs(registryIDs, tableName, true, true, defaultID);
        }
        
        /// <summary>
        /// Für eine Liste von Registry-IDs wird eine ID-Liste der registrierten 
        /// (zugeordneten) Tabelleneinträge unter Berücksichtigung der Vererbung und Richtung bestimmt
        /// </summary>
        /// <param name="registryIDs">Liste von Registry-Ids des Registraturbaumes, getrennt durch ','.</param>
        /// <param name="tableName">Name der zugeordneten Tabelle</param>
        /// <param name="useInheritence">Soll die Vererbung berücksichtigt werden?</param>
        /// <param name="downwards">Soll die Vererbung die untergeordneten (true) oder die übergeordneten (false) Registraturmerkmale berücksichtigen</param>
        /// <param name="defaultID">fak. Default ID falls liste leer ist.</param>
        /// <returns>Liste der zugeordneten row-ID's, getrennt durch ','.</returns>
        public string getRegisteredIDs(string registryIDs, string tableName, bool useInheritence, bool downwards, long defaultID) 
        {
            if (registryIDs == "")
            {
                return defaultID.ToString();
            }

            string registeredIdList = "";
            string allRegistryIDs = registryIDs;
            if (useInheritence){
                if (downwards){
                    allRegistryIDs = _db.Registry.addAllSubRegistries(allRegistryIDs);
                }
                else{
                    allRegistryIDs = _db.Registry.addAllParentRegistries(allRegistryIDs);
                }
            }
            string sql= "select distinct ROW_ID from REGISTRY_ENTRY_V where TABLENAME='" + tableName + "' and REGISTRY_ID in (" + allRegistryIDs + ") and ROW_ID is not null";

            DataTable data = _db.getDataTable(sql,Logger.VERBOSE);
            
            foreach (DataRow row in data.Rows) 
            {
                registeredIdList += "," + row[0];
            }

            if (registeredIdList.Length > 0)
            {
                return registeredIdList.Substring(1);
            }

            return defaultID.ToString();
        }

        /// <summary>
        /// Gibt alle Registraturen eines oder mehrerer Tabelleneinträge
        /// </summary>
        /// <param name="IDs">Liste von IDs, getrennt durch ','</param>
        /// <param name="tableName">Name der zugeordneten Tabelle</param>
        /// <returns>Liste der zugeordneten Registraturen Id's, getrennt durch ','</returns>
        public string getRegistryIDs(string IDs, string tableName) {
            string result = "";

            if (IDs == "")
            {
                return "";
            }

            string sql = "select distinct REGISTRY_ID from REGISTRY_ENTRY_V where TABLENAME='" + tableName + "' and ROW_ID in (" + IDs + ")";
                
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
        /// Gibt alle Registraturen eines Objektes
        /// </summary>
        /// <param name="objectUID">UID des Objektes</param>
        /// <returns>Komma-separierte Liste der zugeordneten Registraturen-IDs</returns>
        public string getRegistryIDs(long objectUID){
            string result = "";

            string sql = "select distinct REGISTRY_ID from REGISTRY_ENTRY where OBJECT_UID=" + objectUID;
            DataTable data = _db.getDataTable(sql, Logger.VERBOSE);
            
            foreach (DataRow row in data.Rows) {
                result += "," + row[0];
            }

            if (result.Length > 0) {
                result = result.Substring(1);
            }

            return result;
        }

        /// <summary>
        /// Gibt den gesamten Pfad einer Registratur zurück.
        /// </summary>
        /// <param name="ID">ID der Registratur</param>
        /// <param name="separator">Zu verwendendes Trennzeichen</param></param>
        /// <returns>Pfad der Registratur</returns>
        public string getRegistryPath(long ID, string separator, bool includeRoot){
            string retValue = "";
            long[] regIDs = getParentRegistryIDs(ID, true);
            bool isFirst = true;
            foreach(long regID in regIDs){
                if (isFirst){
                    if (includeRoot){
                        isFirst = false;
                        retValue += getTitle(regID);
                    }
                    else{
                        includeRoot = true;
                    }
                }
                else{
                    retValue += separator;
                    retValue += getTitle(regID);
                }
            }
            return retValue;
        }
    }
}
