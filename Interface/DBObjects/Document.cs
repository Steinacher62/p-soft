using ch.appl.psoft.db;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.IO;
using System.Web.SessionState;


namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Provides methods for recurring tasks on documents.
    /// Encapsulates application-logic on documents.
    /// </summary>
    public class Document : DBObject {
        private static bool CheckRelease = true;

        public enum FolderType {
            Folder,
        }

        public enum DocType {
            Document,       // a normal document
            Document_Link   // a link to another document (also in an another clipboard)
        }

        public enum DocCategory 
        {
            General,
            Organisation,
            Employee,
            Knowledge,
            Tasklist,
            Project,
            Contact
        }
        
        public Document(DBData db, HttpSessionState session) : base(db, session) { 
            _dbModuleName = "base";
            _dbRelease = 2;
            _release = 2;
            CheckRelease = !checkRelease(CheckRelease);
        }

        /// <summary>
        /// Marks the document as checked out and copies latest version from save to temp directory.
        /// </summary>
        /// <param name="documentID">ID of document to check out</param>
        /// <returns>true, if successful, otherwise false</returns>
        public bool checkOut(long documentID) {
            bool retValue = false;
            
            string sql = "select XFILENAME, CHECKOUT_STATE from DOCUMENT where ID=" + documentID;
            DataTable table = _db.getDataTable(sql);
            if (table.Rows.Count > 0 && table.Rows[0]["CHECKOUT_STATE"].Equals(0)) {
                string xFilename = ch.psoft.Util.Validate.GetValid(table.Rows[0]["XFILENAME"].ToString(), "");
                if (xFilename != "") {
                    try {
                        string saveFilename = Global.Config.documentSaveDirectory+"\\"+xFilename;
                        string tempFilename = Global.Config.documentTempDirectory+"\\"+xFilename;
                        File.Copy(saveFilename,tempFilename,true);

                        sql = "update DOCUMENT set CHECKOUT_STATE=1, CHECKOUT_DATE=GetDate(), CHECKOUT_PERSON_ID=" + SessionData.getUserID(_session) + " where ID=" + documentID;
                        _db.execute(sql);
                        retValue = true;
                    }
                    catch (Exception e) {
                        Logger.Log(e,Logger.ERROR);
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// Marks the document as checked in,
        /// moves the current version to the history directory and moves the new document to the save directory.
        /// </summary>
        /// <param name="documentID">ID of document to check in</param>
        /// <returns>true, of successful, otherwise false</returns>
        public bool checkIn(long documentID) {
            bool retValue = false;
            
            try {
                _db.beginTransaction();
                string sql = "select XFILENAME, CHECKOUT_STATE, CHECKOUT_PERSON_ID from DOCUMENT where ID=" + documentID;
                DataTable table = _db.getDataTable(sql);
                long checkOutPersonID = ch.psoft.Util.Validate.GetValid(table.Rows[0]["CHECKOUT_PERSON_ID"].ToString(),0);

                if (table.Rows.Count > 0 && table.Rows[0]["CHECKOUT_STATE"].ToString() == "1" && checkOutPersonID == SessionData.getUserID(_session)) {
                    string xFilename = ch.psoft.Util.Validate.GetValid(table.Rows[0]["XFILENAME"].ToString(), "");
                    int version = -1;
                    if (xFilename != "") {
                        try {
                            //15.04.14 MSR Anpassung tmpxFilename XID durch documentId ersetzt
                            string tmpxFilename = documentID.ToString() + xFilename.Substring(xFilename.IndexOf("_"), xFilename.Length - xFilename.IndexOf("_"));
                            string tempFilename = Global.Config.documentTempDirectory + "\\" + tmpxFilename;
                            string saveFilename = Global.Config.documentSaveDirectory + "\\" + xFilename;
                            version = moveToHistory(documentID);
                            if(File.Exists(saveFilename))
                                File.Delete(saveFilename);
                            File.Move(tempFilename, saveFilename);
                        }
                        catch (Exception e) {
                            Logger.Log(e,Logger.WARNING);
                        }
                    }
                    sql = "update DOCUMENT set CHECKOUT_STATE=0, CHECKOUT_DATE=null, CHECKOUT_PERSON_ID=null, CHECKIN_DATE=GetDate(), CHECKIN_PERSON_ID=" + checkOutPersonID + (version > 0? ", VERSION=" + (version+1) : "") + " where ID=" + documentID;
                    _db.executeProcedure("MODIFYTABLEROW",
                        new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                        new ParameterCtx("USERID",_db.userId),
                        new ParameterCtx("TABLENAME","DOCUMENT"),
                        new ParameterCtx("ROWID",documentID),
                        new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                        new ParameterCtx("INHERIT",1)
                        );
                    _db.commit();
                    retValue = true;
                }
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
                _db.rollback();
            }

            return retValue;
        }

        /// <summary>
        /// Marks the document as checked in without creating a history-file.
        /// </summary>
        /// <param name="documentID"></param>
        /// <returns></returns>
        public bool checkOutUndo(long documentID) {
            bool retValue = false;
            
            try {
                _db.beginTransaction();
                string sql = "select XFILENAME, CHECKOUT_STATE, CHECKOUT_PERSON_ID from DOCUMENT where ID=" + documentID;
                DataTable table = _db.getDataTable(sql);
                long checkOutPersonID = ch.psoft.Util.Validate.GetValid(table.Rows[0]["CHECKOUT_PERSON_ID"].ToString(),0);

                if (table.Rows.Count > 0 && table.Rows[0]["CHECKOUT_STATE"].ToString() == "1" && checkOutPersonID == SessionData.getUserID(_session)) {
                    string xFilename = ch.psoft.Util.Validate.GetValid(table.Rows[0]["XFILENAME"].ToString(), "");
                    if (xFilename != "") {
                        try {
                            File.Delete(Global.Config.documentTempDirectory+"\\"+xFilename);
                        }
                        catch (Exception e) {
                            Logger.Log(e,Logger.WARNING);
                        }
                    }

                    sql = "update DOCUMENT set CHECKOUT_STATE=0, CHECKOUT_DATE=null, CHECKOUT_PERSON_ID=null where ID=" + documentID;
                    _db.execute(sql);
                    _db.commit();
                    retValue = true;
                }
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
                _db.rollback();
            }

            return retValue;
        }

        /// <summary>
        /// Moves a document to the history and returns the new version
        /// </summary>
        /// <param name="documentID">ID of the document to move to the history</param>
        /// <returns></returns>
        public int moveToHistory(long documentID) {
            int version = -1;
            int maxVersions = getNumOfVersions(documentID);
            int numVersions = _db.DocumentHistory.getNumOfVersions(documentID);

            if (maxVersions != 0) {
                string xFilename = ch.psoft.Util.Validate.GetValid(_db.lookup("XFILENAME", "DOCUMENT", "XFILENAME is not null and ID=" + documentID, false), "");

                if (xFilename != "") {
                    version = _db.DocumentHistory.getNextVersion(documentID);
                    string saveFilename = Global.Config.documentSaveDirectory + "\\" + xFilename;
                    string hFileName = _db.DocumentHistory.EncodeHFileName(IdFromXFile(xFilename), version, FileFromXFile(xFilename));
                    string historyFilename = Global.Config.documentHistoryDirectory + "\\" + hFileName;
                    try {
                        File.Move(saveFilename, historyFilename);
                    }
                    catch (Exception e) {
                        Logger.Log(e,Logger.WARNING);
                    }
                    _db.execute("insert into DOCUMENT_HISTORY (DOCUMENT_ID, VERSION, HFILENAME, PERSON_ID, VDATE, FILENAME) select ID, " + version + ",'" + hFileName + "', CHECKIN_PERSON_ID, CHECKIN_DATE, FILENAME from DOCUMENT where ID=" + documentID);

                    if (Global.isModuleEnabled("spz"))
                    {
                        DataTable tbldocs = _db.getDataTableExt("SELECT * FROM DOCUMENT WHERE (XFILENAME ='" + xFilename + "') AND (ID <> " + documentID +")", new object[0]);

                        foreach (DataRow row in tbldocs.Rows)
                        {
                            _db.execute("insert into DOCUMENT_HISTORY (DOCUMENT_ID, VERSION, HFILENAME, PERSON_ID, VDATE, FILENAME) select ID, " + version + ",'" + hFileName + "', CHECKIN_PERSON_ID, CHECKIN_DATE, FILENAME from DOCUMENT where ID=" + row[0]);
                        }
                    }
                
                }
            }

            if (maxVersions >= 0) _db.DocumentHistory.delete(documentID, maxVersions);
            return version;
        }

        /// <summary>
        /// Get number of versions
        /// </summary>
        /// <param name="documentID"></param>
        /// <returns></returns>
        public int getNumOfVersions(long documentID) {
            object[] objs  = _db.lookup(new String[] {"NUMOFDOCVERSIONS","FOLDER_ID"},"DOCUMENT","ID=" + documentID);
            int num = DBColumn.GetValid(objs[0],-2);

            if (num == -2) num = _db.Folder.getNumOfVersions(DBColumn.GetValid(objs[1],0L));
            return num;
        }


		/// <summary>
		/// Copies a document to a target folder
		/// </summary>
		/// <param name="id">Document to copy</param>
		/// <param name="targetFolderId"></param>
		/// <param name="triggerUID"></param>
		/// <param name="withRegistry">true: Registry-Einträge werden auch kopiert</param>
		/// <returns>The new ID of the copied document</returns>
        public long copy(long id, long targetFolderId, long triggerUID, bool withRegistry) {
            return copy(id, targetFolderId, triggerUID, withRegistry, true);
        }

        /// <summary>
        /// Copies a document to a target-table
        /// </summary>
        /// <param name="id">Document to copy</param>
        /// <param name="targetTablename">Name of the target-table</param>
        /// <param name="targetID">ID of the target-table record</param>
        /// <param name="triggerUID"></param>
        /// <param name="withRegistry">true: Registry-Einträge werden auch kopiert</param>
        /// <returns>The new ID of the copied document</returns>
        public long copy(long id, string targetTablename, long targetID, long triggerUID, bool withRegistry) {
            return copy(id, targetTablename, targetID, triggerUID, withRegistry, true);
        }
        
        /// <summary>
		///  Copies a document to a target folder
		/// </summary>
		/// <param name="id">Document to copy</param>
		/// <param name="targetFolderId"></param>
		/// <param name="triggerUID"></param>
		/// <param name="withRegistry">true: Erzeugt eine Abo-Message für das neue Dokument</param>
		/// <param name="inherit">true: Erzeugt eine Abo-Message für das neue Dokument</param>
		/// <returns>The new ID of the copied document</returns>
        public long copy(long id, long targetFolderId, long triggerUID, bool withRegistry, bool inherit) {
            return copy(id, "FOLDER", targetFolderId, triggerUID, withRegistry, inherit);
        }

        /// <summary>
        ///  Copies a document to a target-table
        /// </summary>
        /// <param name="id">Document to copy</param>
        /// <param name="targetTablename">Name of the target-table</param>
        /// <param name="targetID">ID of the target-table record</param>
        /// <param name="triggerUID"></param>
        /// <param name="withRegistry">true: Erzeugt eine Abo-Message für das neue Dokument</param>
        /// <param name="inherit">true: Erzeugt eine Abo-Message für das neue Dokument</param>
        /// <returns>The new ID of the copied document</returns>
        public long copy(long id, string targetTablename, long targetID, long triggerUID, bool withRegistry, bool inherit) {
            long newId = _db.newId("DOCUMENT");
            string colNames = "," + _db.getColumnNames("DOCUMENT") + ",";
            string attrs = colNames;
            string sql;
            DataTable table;
            
            attrs = attrs.Replace(",ID,",","+newId+",");
            attrs = attrs.Replace(",VERSION,",",1,");
            attrs = attrs.Replace(",EXTERNAL_REF,", ","); colNames = colNames.Replace(",EXTERNAL_REF,", ",");
            attrs = attrs.Replace(",UID,", ","); colNames = colNames.Replace(",UID,", ",");
            if (targetID > 0){
                attrs = attrs.Replace("," + targetTablename + "_ID,", "," + targetID + ",");
            }
            else{
                attrs = attrs.Replace("," + targetTablename + "_ID,", ",null,");
            }

            if (triggerUID > 0){
                attrs = attrs.Replace(",TRIGGER_UID,", "," + triggerUID + ",");
				attrs = attrs.Replace(",KNOWLEDGE_ID,", "," + triggerUID + ",");
            }
            else{
                attrs = attrs.Replace(",TRIGGER_UID,", ",null,");
            }

            sql = "insert into DOCUMENT ("+colNames.Substring(1,colNames.Length-2)+") select "+attrs.Substring(1,attrs.Length-2)+" from DOCUMENT where ID="+id;
               
            _db.executeProcedure("MODIFYTABLEROW",
                new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                new ParameterCtx("USERID",_db.userId),
                new ParameterCtx("TABLENAME","DOCUMENT"),
                new ParameterCtx("ROWID",newId),
                new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                new ParameterCtx("INHERIT",inherit ? 1 : 0)
                );
            
            sql = "select xfilename from DOCUMENT where XFILENAME is not null and ID=" + newId;
            table = _db.getDataTable(sql,"DOCUMENT");
            if (table.Rows.Count > 0) {
                string xFile = table.Rows[0][0].ToString();
                string newXFile = EncodeXFileName(newId,FileFromXFile(xFile));
                sql = "update document set XFILENAME='"+newXFile+"' where ID= "+newId;
                _db.execute(sql);
                try {
                    string documentDir = Global.Config.documentSaveDirectory;
                    if (documentDir != "") {
                        xFile = documentDir+"\\"+xFile;
                        newXFile = documentDir+"\\"+newXFile;
                    }
                    File.Copy(xFile,newXFile,true);
                }
                catch (Exception e) {
                    Logger.Log(e,Logger.WARNING);
                }
            }

			if (withRegistry)
			{
                _db.Registry.copyRegistryEntries("DOCUMENT", id, "DOCUMENT", newId);
			}

			return newId;
        }

        public override int delete(long documentID, bool cascade) {
            return delete(documentID,cascade,true);
        }
        public int delete(long documentID, bool cascade, bool inherit) {
            DataTable tbldocs = _db.getDataTableExt("SELECT * FROM DOCUMENT WHERE (XFILENAME =(SELECT TOP 1 XFILENAME FROM DOCUMENT WHERE ID =" + documentID + ") AND (ID <> " + documentID + "))", new object[0]);
            moveToHistory(documentID);
            ParameterCtx rows = new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int));
            _db.executeProcedure("MODIFYTABLEROW",
                rows,
                new ParameterCtx("USERID",_db.userId),
                new ParameterCtx("TABLENAME","DOCUMENT"),
                new ParameterCtx("ROWID",documentID),
                new ParameterCtx("MODIFY","delete from DOCUMENT where ID=" + documentID,ParameterDirection.Input,typeof(string),true),
                new ParameterCtx("INHERIT",inherit ? 1 : 0)
                );

            if (Global.isModuleEnabled("spz"))
            {
                foreach (DataRow row in tbldocs.Rows)
                {
                    ParameterCtx rows1 = new ParameterCtx("ROWS", null, ParameterDirection.Output, typeof(int));
                    _db.executeProcedure("MODIFYTABLEROW",
                        rows1,
                        new ParameterCtx("USERID", _db.userId),
                        new ParameterCtx("TABLENAME", "DOCUMENT"),
                        new ParameterCtx("ROWID", documentID),
                        new ParameterCtx("MODIFY", "delete from DOCUMENT where ID=" + row[0], ParameterDirection.Input, typeof(string), true),
                        new ParameterCtx("INHERIT", inherit ? 1 : 0)
                        );
                }
            }

            return _db.parameterValue(rows,0);
        }

        public bool canCheckOut(long documentID) {
            bool retValue = false;
            if (_db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "DOCUMENT", documentID, true, true)) {
                DataTable table = _db.getDataTableExt("select * from DOCUMENT where ID=" + documentID);
                retValue = table.Rows.Count > 0 && table.Rows[0]["FILENAME"].ToString() != "" && table.Rows[0]["XFILENAME"].ToString() != "" && table.Rows[0]["CHECKOUT_STATE"].ToString() == "0";
            }
            return retValue;
        }

        public bool canCheckIn(long documentID) {
            DataTable table = _db.getDataTableExt("select * from DOCUMENT where ID=" + documentID);
            return table.Rows.Count > 0 && table.Rows[0]["CHECKOUT_STATE"].ToString() == "1" && table.Rows[0]["CHECKOUT_PERSON_ID"].ToString() == _db.userId.ToString();
        }

        public bool hasValidFile(long documentID) {
            DataTable table = _db.getDataTableExt("select * from DOCUMENT where ID=" + documentID);
            return table.Rows.Count > 0 && table.Rows[0]["FILENAME"].ToString() != "" && table.Rows[0]["XFILENAME"].ToString() != "";
        }

        /// <summary>
        /// Konvertiert in internen Filenamen
        /// </summary>
        /// <param name="id">DB id</param>
        /// <param name="fileName">file namen</param>
        /// <returns></returns>
        public string EncodeXFileName(long id, string fileName) {
            return id+"_"+fileName;
        }

        /// <summary>
        /// Gibt DB id aus internem filenamen
        /// </summary>
        /// <param name="xFileName">interner filenamen</param>
        /// <returns>ID</returns>
        public long IdFromXFile(string xFileName) {
            int idx = xFileName.IndexOf("_");
            return Validate.GetValid(xFileName.Substring(0, idx), 0);
        }
        
        /// <summary>
        /// Gibt externer filanmen aus internem filenamen
        /// </summary>
        /// <param name="xFileName">interner filenamen</param>
        /// <returns>filenamen</returns>
        public string FileFromXFile(string xFileName) {
            int idx = xFileName.IndexOf("_");
            return xFileName.Substring(idx+1);
        }

        /// <summary>
        /// Für eine Liste von Registry-IDs wird eine ID-Liste der registrierten 
        /// (zugeordneten) Dokumente unter Berücksichtigung der Vererbung bestimmt
        /// </summary>
        /// <param name="registryIDs">Liste von Registry-Ids des Registraturbaumes, getrennt durch ','.</param>
        /// <param name="defaultId">fak. Default ID falls liste leer ist.</param>
        /// <returns>Liste der zugeordneten Dokument Id's, getrennt durch ','.</returns>
        public string getRegisteredIDs(string registryIDs, long defaultID) {
            return _db.Registry.getRegisteredIDs(registryIDs, "DOCUMENT", defaultID);
        }

        /// <summary>
        /// Gibt den typ des Dokuments zurück.
        /// </summary>
        /// <param name="id">ID des Dokuments</param>
        /// <returns>Typ des Dokuments</returns>
        public DocType getDocType(long id){
            return (DocType) ch.psoft.Util.Validate.GetValid(_db.lookup("TYP", "DOCUMENT", "ID=" + id, false), (int)DocType.Document);
        }
    }
}
