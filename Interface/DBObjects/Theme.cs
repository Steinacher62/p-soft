using ch.appl.psoft.db;
using ch.appl.psoft.Wiki;
using ch.psoft.db;
using ch.psoft.Util;
using System.Collections;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class Theme : DBObject {
        public const string _TABLENAME = "THEME";
        private Wiki2HTML _wiki2html = new Wiki2HTML();

        public Theme(DBData db, HttpSessionState session) : base(db, session) { }

        public long getRootID(long ID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("ROOT_ID", _TABLENAME, "ID=" + ID, false), -1L);
        }
    
        /// <summary>
        /// Kopiert, rekursiv, das Thema mit allen Bildern.
        /// </summary>
        /// <param name="ID">Zu kopierendes Thema</param>
        /// <param name="parentID">ID des übergeordneten Themas, oder -1 für Wurzel</param>
        /// <param name="inherit">true: Erzeugt eine Abo-Message für übergeordnete Abos</param>
        /// <returns>ID des neuen Themas</returns>
        public long copy(long ID, long parentID, bool inherit, long creatorPersonId) {
            long newID = _db.newId(_TABLENAME);
            string colNames = "," + _db.getColumnNames(_TABLENAME) + ",";
            string attrs = colNames;
            
            attrs = attrs.Replace(",ID,", "," + newID + ",");
            attrs = attrs.Replace(",EXTERNAL_REF,", ","); colNames = colNames.Replace(",EXTERNAL_REF,", ",");
            attrs = attrs.Replace(",UID,", ","); colNames = colNames.Replace(",UID,", ",");

            if (parentID > 0){
                attrs = attrs.Replace(",PARENT_ID,", "," + parentID + ",");
                attrs = attrs.Replace(",ROOT_ID,", "," + getRootID(parentID) + ",");
            }
            else{
                attrs = attrs.Replace(",PARENT_ID,", ",null,");
                attrs = attrs.Replace(",ROOT_ID,", ","+newID+",");
            }
			
			if(creatorPersonId > 0)
			{
				attrs = attrs.Replace(",CREATOR_PERSON_ID,", "," + creatorPersonId + ",");
			}

            string sql = "insert into " + _TABLENAME + " (" + colNames.Substring(1,colNames.Length-2) + ") select " + attrs.Substring(1,attrs.Length-2) + " from " + _TABLENAME + " where ID=" + ID;
               
            _db.executeProcedure("MODIFYTABLEROW",
                new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                new ParameterCtx("USERID",_db.userId),
                new ParameterCtx("TABLENAME",_TABLENAME),
                new ParameterCtx("ROWID",newID),
                new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                new ParameterCtx("INHERIT",0)
                );
            
            DataTable childThemesResult = _db.getDataTable("SELECT ID FROM "+ _TABLENAME +" WHERE PARENT_ID = " + ID , _TABLENAME);
			foreach(DataRow r in childThemesResult.Rows)
			{
              copy((long)r["ID"],newID,inherit,creatorPersonId);
			}

            return newID;
        }

        public override int delete(long ID, bool cascade) {
            return delete(ID, cascade, true);
        }

        private int delete(long ID, bool cascade, bool inherit) {
            int numDel = 0;
			
			if(cascade)
			{
				string sqlCascade = "select ID from " + _TABLENAME + " where PARENT_ID=" + ID;
				DataTable table = _db.getDataTable(sqlCascade);
				foreach (DataRow row in table.Rows) 
				{
					delete((long)row["ID"],true);
				}
			}

            string sql = "delete from " + _TABLENAME + " where ID=" + ID;
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
        /// Für eine Liste von Registry-IDs wird eine ID-Liste der registrierten 
        /// (zugeordneten) Wissenseinträge unter Berücksichtigung der Vererbung bestimmt
        /// </summary>
        /// <param name="registryIDs">Liste von Registry-Ids des Registraturbaumes, getrennt durch ','.</param>
        /// <param name="defaultId">fak. Default ID falls liste leer ist.</param>
        /// <returns>Liste der zugeordneten Verzeichnis Id's, getrennt durch ','.</returns>
        public string getRegisteredIDs(string registryIDs, long defaultID) {
            return _db.Registry.getRegisteredIDs(registryIDs, _TABLENAME, defaultID);
        }

        public int nextOrdnumber(long parentThemeID) {
            return ch.psoft.Util.Validate.GetValid(_db.lookup("MAX(ORDNUMBER)", _TABLENAME, "PARENT_ID=" + parentThemeID, false), -1) + 1;
        }

        public string text2HTML(string text, DBData db, long ownerUID, ref AutoNumbering autoNumbering, int indentLevel, ref ArrayList contentsEntries){
            return _wiki2html.Translate(text, db, ownerUID, ref autoNumbering, indentLevel, ref contentsEntries);
//            return text.Replace("\n", "<br>");
        }

		public long getKnowledgeID(long ID)
		{
			return Validate.GetValid(_db.lookup("ID", Knowledge._TABLENAME, _db.langAttrName(Knowledge._TABLENAME, "BASE_THEME_ID") + "=" + getRootID(ID), false), -1L);
		}

		public long getParentID(long ID)
		{
			return ch.psoft.Util.Validate.GetValid(_db.lookup("PARENT_ID", _TABLENAME, "ID=" + ID, false), -1L);
		}
    }
}
