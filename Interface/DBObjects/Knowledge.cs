using ch.appl.psoft.db;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class Knowledge : DBObject {
        public const string _TABLENAME = "KNOWLEDGE";
        public const string _VIEWNAME = "KNOWLEDGE_V";

        public Knowledge(DBData db, HttpSessionState session) : base(db, session) { }

        /// <summary>
        /// Kopiert, rekursiv, den Wissenseintrag mit allen Dokumenten.
        /// </summary>
        /// <param name="ID">Zu kopierender Wissenseintrag</param>
        /// <param name="triggerUID">UID des SEEK-Objekts welches der Besitzer sein soll (Trigger).</param>
        /// <param name="withRegistry">true: kopiert die Registry-Einträge für den aktuellen Wissenseintrag</param>
        /// <param name="allChildsWithRegistry">true: kopiert die Registry-Einträge für alle untergeordneten Dokumente</param>
        /// <returns>ID des neuen Wissenseintrags</returns>
        public long copy(long ID, long triggerUID, bool withRegistry, bool allChildsWithRegistry) {
            return copy(ID, triggerUID, withRegistry, allChildsWithRegistry, true);
        }

        /// <summary>
        /// Kopiert, rekursiv, den Wissenseintrag mit allen Dokumenten.
        /// </summary>
        /// <param name="ID">Zu kopierender Wissenseintrag</param>
        /// <param name="triggerUID">UID des SEEK-Objekts welches der Besitzer sein soll (Trigger).</param>
        /// <param name="withRegistry">true: kopiert die Registry-Einträge für den aktuellen Wissenseintrag</param>
        /// <param name="allChildsWithRegistry">true: kopiert die Registry-Einträge für alle untergeordneten Dokumente</param>
        /// <param name="inherit">true: Erzeugt eine Abo-Message für übergeordnete Abos</param>
        /// <returns>ID des neuen Wissenseintrags</returns>
        private long copy(long ID, long triggerUID, bool withRegistry, bool allChildsWithRegistry, bool inherit) {
            // Themen...
            // TODO


            // Wissenselement...
            long newID = _db.newId(_TABLENAME);
            string colNames = "," + _db.getColumnNames(_TABLENAME) + ",";
            string attrs = colNames;
            
            attrs = attrs.Replace(",ID,", "," + newID + ",");
            attrs = attrs.Replace(",EXTERNAL_REF,", ","); colNames = colNames.Replace(",EXTERNAL_REF,", ",");
            attrs = attrs.Replace(",UID,", ","); colNames = colNames.Replace(",UID,", ",");

            if (triggerUID > 0){
                attrs = attrs.Replace(",TRIGGER_UID,", "," + triggerUID + ",");
            }
            else{
                attrs = attrs.Replace(",TRIGGER_UID,", ",null,");
            }

            string sql = "insert into " + _TABLENAME + " (" + colNames.Substring(1,colNames.Length-2) + ") select " + attrs.Substring(1,attrs.Length-2) + " from " + _TABLENAME + " where ID=" + ID;
               
            _db.executeProcedure("MODIFYTABLEROW",
                new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                new ParameterCtx("USERID",_db.userId),
                new ParameterCtx("TABLENAME",_TABLENAME),
                new ParameterCtx("ROWID",ID),
                new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                new ParameterCtx("INHERIT",inherit ? 1 : 0)
                );
            
            // Dokumente...
            sql = "select ID from DOCUMENT where " + _TABLENAME + "_ID=" + ID;
            DataTable table = _db.getDataTable(sql);
            foreach (DataRow row in table.Rows) {
                _db.Document.copy((long)row[0], newID, triggerUID, allChildsWithRegistry, false);
            }

            // Registratur
            if (withRegistry) {
                _db.Registry.copyRegistryEntries(_TABLENAME, ID, _TABLENAME, newID);
            }
			
            return newID;
        }

        public override int delete(long ID, bool cascade) {
            return delete(ID, cascade, true);
        }

        private int delete(long ID, bool cascade, bool inherit) {
            int numDel = 0;
						
            // Deletion of latest version deletes all version
			if(getLatestKnowledgeIdFromHistory(ID) == ID )
			{
				DataTable tableVersions = _db.getDataTable("select ID from "+ _TABLENAME + " where HISTORY_ROOT_ID=" + ID);
				foreach (DataRow row in tableVersions.Rows) 
				{
					try 
					{
						delete((long)row["ID"], true, false);
					}
					catch (Exception e) 
					{
						Logger.Log(e,Logger.WARNING);
					}
				}
			}

			DataTable table = _db.getDataTable("select ID from DOCUMENT where " + _TABLENAME + "_ID=" + ID);
			foreach (DataRow row in table.Rows) 
			{
				try 
				{
					numDel += _db.Document.delete(ch.psoft.Util.Validate.GetValid(row[0].ToString(),0), true, false);
				}
				catch (Exception e) 
				{
					Logger.Log(e,Logger.WARNING);
				}
			}

			string[] supportedLanguages = _db.getSupportedLanguages(_TABLENAME,"BASE_THEME_ID_DE");
            ArrayList baseThemeIDs = new ArrayList();
			foreach(string lang in supportedLanguages)
			{
				long baseThemeId = _db.lookup("BASE_THEME_ID_" + lang,_TABLENAME,"ID="+ ID,-1L);
				if(baseThemeId > 0)
				{ 
					baseThemeIDs.Add(baseThemeId);
				}
			}
			
			// Registry Entries
			long UID = _db.lookup("UID",_TABLENAME," ID = "+ ID,-1L);
			string sqlRegistry = "delete from REGISTRY_ENTRY where OBJECT_UID =" + UID;
			_db.execute(sqlRegistry);
            
            string sql = "delete from " + _TABLENAME + " where ID=" + ID;
            ParameterCtx rows = new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int));
            _db.executeProcedure("MODIFYTABLEROW",
                rows,
                new ParameterCtx("USERID",_db.userId),
                new ParameterCtx("TABLENAME",_VIEWNAME),
                new ParameterCtx("ROWID",ID),
                new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                new ParameterCtx("INHERIT",0)
                );
			            
			foreach(long idBase in baseThemeIDs)
			{
			     _db.Theme.delete(idBase,true);				
			}

            return numDel + _db.parameterValue(rows,0);
        }

        public long getBaseThemeID(long ID){
            return Validate.GetValid(_db.lookup("BASE_THEME_ID", _TABLENAME, "ID=" + ID, false), -1L);
        }

        public void setBaseThemeID(long ID, long baseThemeID){
            _db.execute("update " + _TABLENAME + " set " + _db.langAttrName(_TABLENAME, "BASE_THEME_ID") + "=" + baseThemeID + " where ID=" + ID);
        }

        public string getTitle(long ID){
            return _db.lookup(_db.langAttrName(_VIEWNAME, "TITLE"), _VIEWNAME, "ID=" + ID, false);
        }
    
        public string getDescription(long ID){
            return _db.lookup("DESCRIPTION", _VIEWNAME, "ID=" + ID, false);
        }

        public long getUIDByTitle(string title){
            return _db.NiceName2UID(title, _TABLENAME);
        }

        /// <summary>
        /// Für eine Liste von Registry-IDs wird eine ID-Liste der registrierten 
        /// (zugeordneten) Wissenseinträge unter Berücksichtigung der Vererbung bestimmt
        /// </summary>
        /// <param name="registryIDs">Liste von Registry-Ids des Registraturbaumes, getrennt durch ','.</param>
        /// <param name="defaultId">fak. Default ID falls liste leer ist.</param>
        /// <returns>Liste der zugeordneten Verzeichnis Id's, getrennt durch ','.</returns>
        public string getRegisteredIDs(string registryIDs, long defaultID) {
            return _db.Registry.getRegisteredIDs(registryIDs, _TABLENAME, defaultID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseThemeID"></param>
        /// <param name="reason"></param>
        /// <param name="type"></param>
        /// <param name="suggestionExecutionId"></param>
        /// <returns></returns>
        public long create(long baseThemeID,string reason, int type, long suggestionExecutionId){
            if(!Global.isModuleEnabled("suggestion")) 
            {
                //error: should only be called with suggestion mudule enabled.
                return -1;
            }
            long knowledgeID = _db.newId(_TABLENAME);
            string sql = "insert into " + _TABLENAME + " (ID, " + _db.langAttrName(_TABLENAME, "BASE_THEME_ID") + ",REASON, TYPE, SUGGESTION_EXECUTION_ID) values (" + knowledgeID + "," + baseThemeID + ",'"+SQLColumn.toSql(reason) +"'," + type + "," + suggestionExecutionId + ")";
            _db.execute(sql);
            return knowledgeID;
        }

        public long create(long baseThemeID,string reason)
        {
            long knowledgeID = _db.newId(_TABLENAME);
            string sql = "insert into " + _TABLENAME + " (ID, " + _db.langAttrName(_TABLENAME, "BASE_THEME_ID") + ",REASON) values (" + knowledgeID + "," + baseThemeID + ",'"+SQLColumn.toSql(reason) + "')";
            _db.execute(sql);
            return knowledgeID;
        }

		public long createHistoryEntry(long knowledgeId, string reason)
		{
			long oldVersion = _db.lookup("VERSION","KNOWLEDGE","ID=" + knowledgeId,-1L);			
			long newVersion = oldVersion + 1;
			long oldKnowledgeUID = _db.lookup("UID","KNOWLEDGE","ID=" + knowledgeId,-1L);
			long newKnowledgeUID = -1;

			long historyRootId = getBaseKnowledgeId(knowledgeId);
			Theme theme = new Theme(_db,_session);

			long baseThemeId    = getBaseThemeID(knowledgeId);            
            long newBaseThemeId = theme.copy(baseThemeId,-1,true,_db.userId);
			            
			// Wissenselement...
			long newID = _db.newId(_TABLENAME);
			string colNames = "," + _db.getColumnNames(_TABLENAME) + ",";
			string attrs = colNames;
            
			attrs = attrs.Replace(",ID,", "," + newID + ",");
												
			if (baseThemeId > 0)
			{
				attrs = attrs.Replace(",BASE_THEME_ID_DE,", "," + newBaseThemeId + ",");
			}
			else
			{
				attrs = attrs.Replace(",TRIGGER_UID,", ",null,");
			}

			if (baseThemeId > 0)
			{
				attrs = attrs.Replace(",BASE_THEME_ID_DE,", "," + newBaseThemeId + ",");
			}
			else
			{
				attrs = attrs.Replace(",TRIGGER_UID,", ",null,");
			}
			
			attrs = attrs.Replace(",VERSION,", "," + oldVersion + ",");
		    attrs = attrs.Replace(",HISTORY_ROOT_ID,", "," + historyRootId + ",");
 
			//Remove unnessary Columns, values will be created through triggers or have default values
			attrs = attrs.Replace(",EXTERNAL_REF,", ","); colNames = colNames.Replace(",EXTERNAL_REF,", ",");
			attrs = attrs.Replace(",UID,", ","); colNames = colNames.Replace(",UID,", ",");
			//attrs = attrs.Replace(",CREATED,", ","); colNames = colNames.Replace(",CREATED,", ",");
		    			                    
			string sql = "insert into " + _TABLENAME + " (" + colNames.Substring(1,colNames.Length-2) + ") select " + attrs.Substring(1,attrs.Length-2) + " from " + _TABLENAME + " where ID=" + knowledgeId;
            			        
			_db.executeProcedure("MODIFYTABLEROW",
				new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
				new ParameterCtx("USERID",_db.userId),
				new ParameterCtx("TABLENAME",_TABLENAME),
				new ParameterCtx("ROWID",knowledgeId),
				new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
				new ParameterCtx("INHERIT", 0),
				new ParameterCtx("MEMOATTR", "TITLE"),
				new ParameterCtx("ACTION", 2)
				); 

			newKnowledgeUID = _db.lookup("UID","KNOWLEDGE","ID=" + newID,-1L);
			_db.copyRowAuthorisations(_TABLENAME,knowledgeId,_TABLENAME,newID);

			// now we create "shared" references to existing images 			
			WikiImage wImage = new WikiImage(_db,_session);
			long oldUID = (long ) _db.lookup("UID" ,"KNOWLEDGE" ," ID = " + knowledgeId);
			long newUID = (long ) _db.lookup("UID" ,"KNOWLEDGE" ," ID = " + newID);
			DataTable imagesResult = _db.getDataTable("SELECT ID FROM WIKI_IMAGE WHERE OWNER_UID = " + oldUID , _TABLENAME);
			
			foreach(DataRow r in imagesResult.Rows)
			{
				wImage.addReferenceToImage((long)r["ID"],newUID);
			}

            // Registry kopieren
			_db.Registry.copyRegistryEntries(_TABLENAME, knowledgeId, _TABLENAME, newID);
						
			sql = "select ID from DOCUMENT where " + _TABLENAME + "_ID=" + knowledgeId;
			DataTable table = _db.getDataTable(sql);
			
			foreach (DataRow row in table.Rows) 
			{
				_db.Document.copy((long)row[0], "", -1, newID, false, false);
			}		
            
			copyAssignments(oldKnowledgeUID,newUID);	
            
			string date = _db.lookup("CREATED" ,"KNOWLEDGE" , " ID = " + newID,"");
			_db.execute("UPDATE KNOWLEDGE SET VERSION = K2.VERSION + 1, CREATED = GETDATE(), REASON = '" + SQLColumn.toSql(reason) + "' from KNOWLEDGE, KNOWLEDGE K2 WHERE KNOWLEDGE.ID =" + knowledgeId + " AND K2.ID = " + newID);
			return knowledgeId;
		}

		public long getBaseKnowledgeId(long knowledgeID)
		{
			long historyRootId = _db.lookup("HISTORY_ROOT_ID", _TABLENAME, "ID=" + knowledgeID, -1L);
			return  historyRootId > 0 ?  historyRootId : knowledgeID ;
		}

		public long getLatestKnowledgeIdFromHistory(long knowledgeID)
		{
			long latestId = _db.lookup("HISTORY_ROOT_ID", _TABLENAME, "ID =" + knowledgeID, -1L);			
			return latestId > 0 ? latestId : knowledgeID;
		}

		// Work in Progress
		private void copyAssignments(long oldUID, long newUID)
		{
			if(oldUID > 0 && newUID > 0)
			{
				string colNames = "," + _db.getColumnNames(_TABLENAME) + ",";
				string attrs = colNames;

                string sql = "select TO_UID from UID_ASSIGNMENT where FROM_UID = " + oldUID;
				DataTable table = _db.getDataTable(sql);
			
				foreach (DataRow row in table.Rows) 
				{					
					StringBuilder sqlCopy = new StringBuilder("insert into UID_ASSIGNMENT(FROM_UID,TO_UID,OWNER_PERSON_ID,TYP,UID_STRUCTURE_ID)");
                                  sqlCopy.Append(" select #1,TO_UID,OWNER_PERSON_ID,TYP,UID_STRUCTURE_ID FROM UID_ASSIGNMENT WHERE FROM_UID = "+ oldUID + " AND TO_UID = "+ (long)row["TO_UID"]);
					sqlCopy.Replace("#1","" + newUID);
				    
					_db.execute(sqlCopy.ToString());
				}		
				
			}
		}

    }
}
