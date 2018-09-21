using ch.appl.psoft.db;
using ch.psoft.db;
using System;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Measure.
    /// </summary>
    public class Measure : DBObject {
        public Measure(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return delete(ID,cascade,true);
        }
        
		/// <summary>
		/// Löschen einer Pendenz
		/// Bemerkungen:
		/// Ablagen werden von cascade unabhängig, immer gelöscht
		/// Zugehörige Kontaktgruppe wird von einem Db-Trigger gelöscht
		/// Bei Vorlage keine Benachrichtigung!
		/// </summary>
		/// <param name="ID"></param>
		/// <param name="cascade"></param>
		/// <param name="inherit"></param>
		/// <returns>Anzahl gelöschter Measure-Records</returns>
		public int delete(long ID, bool cascade, bool inherit)
		{
			int numDel = 0;
			object [] values = _db.lookup(
					new string [] {"CLIPBOARD_ID", "TEMPLATE"},
					"MEASURE",
					"id=" + ID
				);
			long clipboardId  = DBColumn.GetValid(values[0], (long)0);
			string template = DBColumn.GetValid(values[1], "0");
			string sql = "delete from MEASURE where ID=" + ID;

			if (template == "1") // bei Vorlage keine Benachrichtigung
			{
				numDel = _db.execute(sql);
			}
			else
			{
				ParameterCtx rows = new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int));
				_db.executeProcedure("MODIFYTABLEROW",
					rows,
					new ParameterCtx("USERID",_db.userId),
					new ParameterCtx("TABLENAME","MEASURE"),
					new ParameterCtx("ROWID",ID),
					new ParameterCtx("MODIFY", sql, ParameterDirection.Input ,typeof(string),true),
					new ParameterCtx("INHERIT",inherit ? 1 : 0)
				);
				numDel = _db.parameterValue(rows,0);
			}

			if (clipboardId > 0)
			{
				_db.Clipboard.delete(clipboardId,true);
			}

            return numDel;
        }
        /// <summary>
        /// Delete all measures from a specific person
        /// </summary>
        /// <param name="personId">responsibe person Id</param>
        /// <param name="tasklistId">Fak. Tasklist Id</param>
        /// <returns>#deleted records</returns>
        public int deleteByPerson(long personId, params long[] tasklistId) {
            string sql = "select id from measure where responsible_person_id="+personId;
            int num = 0;

            if (tasklistId.Length > 0) sql += " and tasklist_id = "+tasklistId[0];
            DataTable table = _db.getDataTable(sql);

            foreach (DataRow row in table.Rows) {
                num += delete((long) row["id"],false,true);
            }
            return num;
        }

        /// <summary>
        /// Copies a Measure
        /// </summary>
        /// <param name="ID">Identifier of the Measure to copy</param>
        /// <param name="targetTasklistID">Identifier of the target Tasklist</param>
        /// <param name="triggerUID">UID of the SEEK-Object that should be the owner(trigger).</param>
        /// <param name="cascade">true: copy recursive</param>
        /// <param name="template">True: The new Measure is flagged as template</param>
        /// <param name="assumeAuthor">True: The new Measure will assume the author of the logged user</param>
        /// <returns></returns>
        public long copy(long ID, long targetTasklistID, long triggerUID, bool cascade, bool template, bool assumeAuthor) {
            return copy(
					ID, 
					targetTasklistID, 
					triggerUID, 
					cascade, 
					template, 
					assumeAuthor, 
					true,
					true,
					cascade // bei Kaskadieren auch Kontaktgruppen kopieren
				);
        }
        
		/// <summary>
		///	Kopiert Pendenz
		/// </summary>
		/// <param name="ID">Original</param>
		/// <param name="targetTasklistID">Zieltaskliste</param>
		/// <param name="triggerUID"></param>
		/// <param name="cascade">Untergeordnetes (hier Ablagen) auch kopieren</param>
		/// <param name="template">Vorlage</param>
		/// <param name="assumeAuthor">Der kopierende Benutzer wird als Autor eingetragen</param>
		/// <param name="inherit">Benachrichtigung beim rekursiven Kopieren</param>
		/// <param name="sendMessage">Soll eine Benachrichtigung stattfinden? (false für Vorlagen)</param>
		/// <param name="copyContactGroup">Kontaktgruppen dupplizieren</param>
		/// <param name="attributesToAnnull">
		///		Liste der Attribute deren Wert nicht übernommen wird.
		///		(Format: Bsp "MEASURE.STARTDATE" in Uppercase!)
		/// </param>
		/// <returns>Id der Kopie</returns>
		public long copy(
			long ID, 
			long targetTasklistID, 
			long triggerUID, 
			bool cascade, 
			bool template, 
			bool assumeAuthor, 
			bool inherit,
			bool sendMessage,
			bool copyContactGroup,
			params string [] attributesToAnnull
		) 
		{
			// Kopieren der zugehörigen Kontaktgruppe
			long contactGroupId = 0;

			if (copyContactGroup && Global.isModuleEnabled("contact"))
			{
				long sourceContactGroupId = DBColumn.GetValid(
						_db.lookup("contact_group_id", "measure", "id=" + ID),
						(long)0
					);

				if (sourceContactGroupId > 0)
				{
					contactGroupId = _db.ContactGroup.copy(sourceContactGroupId, contactGroupId);
				}
			}
			
			// Kopieren der Pendenz
			long newId = _db.newId("MEASURE");

			string colNames = "," + _db.getColumnNames("MEASURE") + ",";
            string attrs = colNames;
            string sql;
            
            attrs = attrs.Replace(",ID,", "," + newId + ",");
            attrs = attrs.Replace(",EXTERNAL_REF,", ",");
			colNames = colNames.Replace(",EXTERNAL_REF,", ",");
            attrs = attrs.Replace(",UID,", ",");
			colNames = colNames.Replace(",UID,", ",");
            attrs = attrs.Replace(",CLIPBOARD_ID,", ",null,");
            attrs = attrs.Replace(",TEMPLATE,", "," + (template ? "1" : "0") + ",");

			if (targetTasklistID > 0)
			{
				attrs = attrs.Replace(",TASKLIST_ID,", "," + targetTasklistID + ",");
			}

			if (assumeAuthor)
			{
                attrs = attrs.Replace(",AUTHOR_PERSON_ID,", "," + SessionData.getUserID(_session) + ",");
			}

            if (triggerUID > 0)
			{
                attrs = attrs.Replace(",TRIGGER_UID,", "," + triggerUID + ",");
            }
            else
			{
                attrs = attrs.Replace(",TRIGGER_UID,", ",null,");
            }

			if (copyContactGroup && contactGroupId > 0)
			{
				attrs = attrs.Replace(",CONTACT_GROUP_ID,",	"," + contactGroupId + ",");
			}
			else
			{
				attrs = attrs.Replace(",CONTACT_GROUP_ID,", ",null,");
			}

			string [] attribute = null;

			// Attribute, deren Werte nicht übernommen werden sollen
			for (int counter = 0; counter < attributesToAnnull.Length; counter++)
			{
				attribute = attributesToAnnull[counter].Split(new Char[] {'.'});

				if (attribute.Length > 1 && attribute[0] == "MEASURE")
				{
					attrs = attrs.Replace("," + attribute[1] + ",", ",null,"); 
				}
			}
			
            attrs = attrs.Replace(",CREATIONDATE,", ",GetDate(),");
            attrs = attrs.Replace(",STATE,", ",isnull(STATE,0),");

            attrs = attrs.Substring(1, attrs.Length - 2);
            colNames = colNames.Substring(1, colNames.Length - 2);
            sql = "insert into MEASURE (" + colNames + ")"
				+ " select " + attrs + " from MEASURE where ID=" + ID;
               
			if (sendMessage)
			{
				_db.executeProcedure("MODIFYTABLEROW",
					new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
					new ParameterCtx("USERID",_db.userId),
					new ParameterCtx("TABLENAME","MEASURE"),
					new ParameterCtx("ROWID",newId),
					new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
					new ParameterCtx("INHERIT",inherit ? 1 : 0)
                );
			}
			else
			{
				_db.execute(sql);
			}
            
            if (cascade) {
                // copy shelf...
                long clipboardId = DBColumn.GetValid(_db.lookup("CLIPBOARD_ID", "MEASURE", "ID="+ID), 0L);
                if (clipboardId > 0) {
                    long measureUID = DBColumn.GetValid(_db.lookup("UID", "MEASURE", "ID="+newId), 0L);
                    clipboardId = _db.Clipboard.copy(
							clipboardId,
							true,
							_db.userAccessorID,
							measureUID,
							template,
							Clipboard.TYPE_PRIVATE,
							true
						);
                    sql = "update measure set clipboard_id="+clipboardId+" where id = "+newId;
                    _db.execute(sql);
                }
            }

            refreshAccessRights(newId);
            
            return newId;
        }


        /// <summary>
        /// Returns the semaphore state based on the due-date and the number of critical days
        /// </summary>
        /// <param name="ID">Identifier of measure</param>
        /// <returns>0: red, 1: orange, 2: green, 3: done</returns>
        public int getSemaphore(long ID, int criticalDays) {
            DataTable table = _db.getDataTable("select STATE, DUEDATE from MEASURE where ID=" + ID);
            
            if (table.Rows.Count <= 0)
                return -1;
     
            DataRow row = table.Rows[0];
            DateTime dueDate = DBColumn.GetValid(row["DUEDATE"], DateTime.MaxValue);

            if (row["STATE"].ToString() == "1")
                return 3;
            else if (dueDate > (DateTime.Now.AddDays(criticalDays)))
                return 2;
            else if (dueDate > DateTime.Now)
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// Sets/changes the responsible person
        /// </summary>
        /// <param name="ID">Identifier of measure</param>
        /// <param name="responsibleID">Identifier of responsible person</param>
        public void setResponsible(long ID, long responsibleID) {
            // revoke the old responsible's rights and set the rights for the new responsibe...
            long authorID = ch.psoft.Util.Validate.GetValid(_db.lookup("AUTHOR_PERSON_ID", "MEASURE", "ID=" + ID, false), -1L);
            long oldResponsibleID = ch.psoft.Util.Validate.GetValid(_db.lookup("RESPONSIBLE_PERSON_ID", "MEASURE", "ID=" + ID, false), -1L);
            if (oldResponsibleID > 0 && oldResponsibleID != authorID) {
                _db.revokeRowAuthorisation(DBData.AUTHORISATION.RUDI, _db.getAccessorID(oldResponsibleID), "MEASURE", ID);
            }

            if (responsibleID != authorID) {
                _db.grantRowAuthorisation(DBData.AUTHORISATION.RUDI, _db.getAccessorID(responsibleID), "MEASURE", ID);
            }

            // change the responsible person
            _db.execute("update MEASURE set RESPONSIBLE_PERSON_ID=" + responsibleID + " where ID=" + ID);
        }

        public void refreshAccessRights(long measureID) {
            if (measureID < 1)
                return;

            long authorID = ch.psoft.Util.Validate.GetValid(_db.lookup("AUTHOR_PERSON_ID", "MEASURE", "ID=" + measureID, false), -1L);
            long responsibleID = ch.psoft.Util.Validate.GetValid(_db.lookup("RESPONSIBLE_PERSON_ID", "MEASURE", "ID=" + measureID, false), -1L);

            // set rights for author
            if (authorID > 0){
                _db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, _db.getAccessorID(authorID), "MEASURE", measureID);
            }

            // set rights for responsible
            if (responsibleID > 0 && responsibleID != authorID){
                _db.grantRowAuthorisation(DBData.AUTHORISATION.RUDI, _db.getAccessorID(responsibleID), "MEASURE", measureID);
            }
        }
    }
}
