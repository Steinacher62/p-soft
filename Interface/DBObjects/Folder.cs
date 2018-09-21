using ch.appl.psoft.db;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Folder.
    /// </summary>
    public class Folder : DBObject {
        private static bool CheckRelease = true;
        private const string _TABLENAME = "FOLDER";

        public Folder(DBData db, HttpSessionState session) : base(db, session) { 
            _dbModuleName = "base";
            _dbRelease = 2;
            _release = 2;
            CheckRelease = !checkRelease(CheckRelease);
        }

        /// <summary>
        /// Copies, recursively, the folder with all the documents to the target-folder
        /// </summary>
        /// <param name="ID">Folder to copy</param>
        /// <param name="targetParentFolderID">Target-parent-folder to copy to</param>
        /// <param name="triggerUID">UID of the SEEK-Object that should be the owner(trigger).</param>
        /// <param name="cascade">Copies recursively, if true</param>
		/// <param name="withRegistry">true: kopiert die Registry-Einträge für das aktuelle Folder</param>
		/// <param name="allChildsWithRegistry">true: kopiert die Registry-Einträge für alle untergeordneten
		/// Documente und Folder</param>
		/// <returns>The new ID of the copied folder</returns>
        public long copy(
				long ID, 
				long targetFolderID, 
				long triggerUID, 
				bool cascade, 
				bool withRegistry, 
				bool allChildsWithRegistry
			)
		{
            return copy(ID, targetFolderID, triggerUID, cascade, withRegistry, allChildsWithRegistry, true);
        }

        /// <summary>
        /// Copies, recursively, the folder with all the documents to the target-folder
        /// </summary>
        /// <param name="ID">Folder to copy</param>
        /// <param name="targetParentFolderID">Target-parent-folder to copy to</param>
        /// <param name="triggerUID">UID of the SEEK-Object that should be the owner(trigger).</param>
        /// <param name="cascade">Copies recursively, if true</param>
		/// <param name="withRegistry">true: kopiert die Registry-Einträge für das aktuelle Folder</param>
		/// <param name="allChildsWithRegistry">true: kopiert die Registry-Einträge für alle untergeordneten
		/// Documente und Folder</param>
		/// <param name="inherit">true: Erzeugt eine Abo-Message für das neue Folder</param>
		/// <returns>The new ID of the copied folder</returns>
        private long copy(
				long ID,
				long targetParentFolderID,
				long triggerUID,
				bool cascade,
				bool withRegistry,
				bool allChildsWithRegistry, 
				bool inherit
			)
		{
            long newID = _db.newId(_TABLENAME);
            string colNames = "," + _db.getColumnNames(_TABLENAME) + ",";
            string attrs = colNames;
            string sql;
            DataTable table;
            
            attrs = attrs.Replace(",ID,", "," + newID + ",");
            attrs = attrs.Replace(",EXTERNAL_REF,", ","); colNames = colNames.Replace(",EXTERNAL_REF,", ",");
            attrs = attrs.Replace(",UID,", ","); colNames = colNames.Replace(",UID,", ",");
            long rootId = newID;
            if (targetParentFolderID > 0){
                attrs = attrs.Replace(",PARENT_ID,", "," + targetParentFolderID + ",");
                rootId = DBColumn.GetValid(_db.lookup("ROOT_ID", _TABLENAME, "ID="+targetParentFolderID), 0L);
            }
            else{
                attrs = attrs.Replace(",PARENT_ID,", ",null,");
            }
            attrs = attrs.Replace(",ROOT_ID,", "," + rootId + ",");

            if (triggerUID > 0){
                attrs = attrs.Replace(",TRIGGER_UID,", "," + triggerUID + ",");
            }
            else{
                attrs = attrs.Replace(",TRIGGER_UID,", ",null,");
            }

            sql = "insert into " + _TABLENAME + " (" + colNames.Substring(1,colNames.Length-2) + ") select " + attrs.Substring(1,attrs.Length-2) + " from " + _TABLENAME + " where ID=" + ID;
               
            _db.executeProcedure("MODIFYTABLEROW",
                new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                new ParameterCtx("USERID",_db.userId),
                new ParameterCtx("TABLENAME",_TABLENAME),
                new ParameterCtx("ROWID",newID),
                new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                new ParameterCtx("INHERIT",inherit ? 1 : 0)
                );
            
            sql = "select ID from DOCUMENT where FOLDER_ID=" + ID;
            table = _db.getDataTable(sql);
            foreach (DataRow row in table.Rows) {
                _db.Document.copy((long)row[0], newID, triggerUID, allChildsWithRegistry, false);
            }

			if (withRegistry)
			{
                _db.Registry.copyRegistryEntries(_TABLENAME, ID, _TABLENAME, newID);
			}
			
			if (cascade) 
			{
                sql = "select id from " + _TABLENAME + " where PARENT_ID=" + ID;
                table = _db.getDataTable(sql);
                foreach (DataRow row in table.Rows) {
                    copy((long)row[0], newID, triggerUID, cascade, allChildsWithRegistry, allChildsWithRegistry, false);
                }
            }
            
            return newID;
        }

        public long find(long root, long search) {
            return _db.Tree(_TABLENAME).Find(root, search);
        }

        public override int delete(long ID, bool rootEnable) {
            return delete(ID,rootEnable,true);
        }
        private int delete(long ID, bool rootEnable, bool inherit) {
            int numDel = 0;
            
            DataTable table = _db.getDataTable("select ID from " + _TABLENAME + " where PARENT_ID=" + ID);
            foreach (DataRow row in table.Rows) {
                numDel += delete((long)row[0], true, false);
            }
            
            table = _db.getDataTable("select ID from DOCUMENT where FOLDER_ID=" + ID);
            foreach (DataRow row in table.Rows) {
                try {
                    _db.Document.delete(ch.psoft.Util.Validate.GetValid(row[0].ToString(),0), true, false);
                }
                catch (Exception e) {
                    Logger.Log(e,Logger.WARNING);
                }
            }
            
            string sql = "delete from " + _TABLENAME + " where ID=" + ID;
            if (!rootEnable) sql += " and PARENT_ID is not null";
            numDel++;
            ParameterCtx rows = new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int));
            _db.executeProcedure("MODIFYTABLEROW",
                rows,
                new ParameterCtx("USERID",_db.userId),
                new ParameterCtx("TABLENAME",_TABLENAME),
                new ParameterCtx("ROWID",ID),
                new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                new ParameterCtx("INHERIT",inherit ? 1 : 0)
                );

            return numDel + _db.parameterValue(rows,0);
        }

        /// <summary>
        /// Ergänzt eine Liste von Folder-IDs durch die IDs aller untergeordneten Folder
        /// </summary>
        /// <param name="folderIDs">Liste von Folder-Ids, getrennt durch ','</param>
        /// <returns>ergänzte Liste, getrennt durch ','</returns>
        public string addAllSubFolders(string folderIDs) {
            return _db.Tree(_TABLENAME).AddAllSubnodes(folderIDs);
        }

        /// <summary>
        /// Ergänzt eine Liste von Folder-IDs durch die IDs aller untergeordneten
        /// Folder in der Searchresult-tabelle
        /// </summary>
        /// <param name="folderIDs">Liste von Folder-IDs, getrennt durch ','</param>
        /// <param name="resultId">SearchResult-ID</param>
        /// <returns>#rows inserted</returns>
        public int addAllSubFolders(string folderIDs, long resultId) {
            return _db.Tree(_TABLENAME).AddAllSubnodes(folderIDs, resultId);
        }

        /// <summary>
        /// Ergänzt eine Liste von Folder-IDs durch die ISs aller untergeordneten
        /// Folder in der Searchresult-tabelle
        /// </summary>
        /// <param name="resultId">Target SearchResult-ID</param>
        /// <returns>#rows inserted</returns>
        public int addAllSubFolders(long resultId) {
            return _db.Tree(_TABLENAME).AddAllSubnodes(resultId);
        }

        /// <summary>
        /// Returns list of parent folder-IDs (path) corresponding to a certain folder
        /// </summary>
        /// <param name="folderID">ID of a folder</param>
        /// <param name="inclFolder">true: inclusive folderID (default false)</param>
        /// <returns></returns>
        public long[] getParentFolderIDs(long folderID, bool inclFolder) {
            ArrayList list = _db.Tree(_TABLENAME).GetPath(folderID, inclFolder);

            return (long[]) list.ToArray(typeof(long));
        }
        /// <summary>
        /// Returns list of parent folder-IDs (path) corresponding to a certain folder
        /// </summary>
        /// <param name="folderID">ID of a folder</param>
        /// <param name="inclFolder">true: inclusive folderID (default false)</param>
        /// <returns></returns>
        public ArrayList getParentFolderIDList(long folderID, bool inclFolder) {
            return _db.Tree(_TABLENAME).GetPath(folderID, inclFolder);
        }
    
        /// <summary>
        /// Für eine Liste von Registry-IDs wird eine ID-Liste der registrierten 
        /// (zugeordneten) Verzeichnise unter Berücksichtigung der Vererbung bestimmt
        /// </summary>
        /// <param name="registryIDs">Liste von Registry-Ids des Registraturbaumes, getrennt durch ','.</param>
        /// <param name="defaultId">fak. Default ID falls liste leer ist.</param>
        /// <returns>Liste der zugeordneten Verzeichnis Id's, getrennt durch ','.</returns>
        public string getRegisteredIDs(string registryIDs, long defaultID) {
            return _db.Registry.getRegisteredIDs(registryIDs, _TABLENAME, defaultID);
        }
        /// <summary>
        /// Get number of versions
        /// </summary>
        /// <param name="folderID"></param>
        /// <returns></returns>
        public int getNumOfVersions(long folderID) {
            object[] objs;
            int num = -2;

            while (true) {
                objs = _db.lookup(new String[] {"NUMOFDOCVERSIONS","PARENT_ID"},"FOLDER","ID = " + folderID);
                num = DBColumn.GetValid(objs[0],-2);
                if (num > -2) break;
                folderID = DBColumn.GetValid(objs[1],0L);
                if (folderID == 0) return Global.Config.numberOfDocumentVersions;
            }

            return num;
        }


    }
}
