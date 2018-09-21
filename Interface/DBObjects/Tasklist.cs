using ch.appl.psoft.db;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Tasklist.
    /// </summary>
    public class Tasklist : DBObject {
        public const int TYPE_PUBLIC  = 0;
        public const int TYPE_PRIVATE = 1;

        public Tasklist(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return delete(ID, cascade, true);
        }

		/// <summary>
		/// Löschen einer Pendenzenliste
		/// Bemerkungen:
		/// Nicht so fein parametrisierbar wie copy(). Tendenziell ist es rigoroser.
		/// dh cascade bezieht sich nur auf Subpendenzenlisten und Pendenzen
		/// Zugehörige Kontaktgruppe wird von einem Db-Trigger gelöscht
		/// Bei Vorlage keine Benachrichtigung!
		/// </summary>
		/// <param name="ID"></param>
		/// <param name="cascade"></param>
		/// <param name="inherit"></param>
		/// <returns>Anzahl gelöschter Tasklist-Records</returns>
        private int delete(long ID, bool cascade, bool inherit)
		{
            int numDel = 0;

            if (cascade)
			{
                // bottum up
                DataTable table = _db.getDataTable("select ID from TASKLIST where PARENT_ID=" + ID);

                foreach (DataRow row in table.Rows) {
                    numDel += delete(DBColumn.GetValid(row[0],0L),true,false);
                }

                table = _db.getDataTable("select ID from MEASURE where TASKLIST_ID=" + ID);

                foreach (DataRow row in table.Rows) {
                    _db.Measure.delete(DBColumn.GetValid(row[0],0L), true, false);
                }
            }

            // Annullieren von Referenzen
			string sql = "update TASKLIST set TEMPLATE_TASKLIST_ID = null"
					+ " where TEMPLATE_TASKLIST_ID=" + ID;
			_db.execute(sql);

            if (Global.isModuleEnabled("contact"))
            {
			    sql = "update PERSON set TASKLIST_ID = null"
					    + " where TASKLIST_ID=" + ID;
			    _db.execute(sql);
                sql = "update FIRM set TASKLIST_ID = null"
                    + " where TASKLIST_ID=" + ID;
                _db.execute(sql);
            }

            if (Global.isModuleEnabled("project"))
            {
			    sql = "update PHASE set TASKLIST_ID = null"
					    + " where TASKLIST_ID=" + ID;
			    _db.execute(sql);
            }

            // Links zu anderer Pendenzenlisten
			sql = "delete from TASKLIST_ASSIGNMENT"
					+ " where PARENT_TASKLIST_ID=" + ID + " or ASSIGNED_TASKLIST_ID=" + ID;
			_db.execute(sql);

			object [] values = _db.lookup(
					new string [] {"CLIPBOARD_ID", "TEMPLATE"},
					"TASKLIST",
					"id=" + ID
				);
			long clipboardId  = DBColumn.GetValid(values[0], (long)0);
			string template = DBColumn.GetValid(values[1], "0");
			sql = "delete from TASKLIST where ID=" + ID;

			if (template == "1") // bei Vorlage keine Benachrichtigung
			{
				numDel += _db.execute(sql);
			}
			else
			{
				ParameterCtx rows = new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int));
				_db.executeProcedure("MODIFYTABLEROW",
					rows,
					new ParameterCtx("USERID",_db.userId),
					new ParameterCtx("TABLENAME","TASKLIST"),
					new ParameterCtx("ROWID",ID),
					new ParameterCtx("MODIFY", sql, ParameterDirection.Input, typeof(string),true),
					new ParameterCtx("INHERIT",inherit ? 1 : 0)
					);
				numDel += _db.parameterValue(rows,0);
			}

			// Ablage löschen
			if (clipboardId > 0)
			{
				_db.Clipboard.delete(clipboardId, true);
			}


            return numDel;
        }

		/// <summary>
		/// Erstellt eine Kopie einer Pendenzenliste als Vorlage
		/// </summary>
		/// <param name="tasklistId">Original</param>
		/// <returns>Id der Kopie</returns>
		public long copyAsTemplate(
			long tasklistId
		)
		{
			return copy (
				tasklistId,
				-1,
				-1,
				-1,
				true,
				true,
				true,
				TYPE_PUBLIC,
				true,
				false,
				false, // bei Vorlagen keine Message
				true,
				true,
				true,
				new string [] {
					"TASKLIST.TEMPLATE_TASKLIST_ID",
					"TASKLIST.STARTDATE",
					"TASKLIST.DUEDATE",
					"MEASURE.STATE",
					"MEASURE.STARTDATE",
					"MEASURE.DUEDATE"
				}
			);
		}

		/// <summary>
        /// Copy Tasklist
        /// </summary>
        /// <param name="sourceTasklistID">tasklist id</param>
        /// <param name="targetTasklistID">id of the already existing tasklist</param>
        /// <param name="targetParentTasklistID">id of the parent-tasklist where the tasklist should be copied to</param>
        /// <param name="triggerUID">UID of the SEEK-Object that should be the owner(trigger).</param>
        /// <param name="cascade">true: copy recursive</param>
        /// <param name="copyMeasure">true: copy measure also</param>
        /// <param name="template">true: flag copy as template</param>
        /// <param name="typ">The type of the copied tasklist</param>
        /// <param name="assumeAuthor">true: assumes author of currently logged person</param>
        /// <returns>new Tasklist id</returns>
        public long copy (long sourceTasklistID, long targetTasklistID, long targetParentTasklistID, long triggerUID, bool cascade, bool copyMeasure, bool template, int typ, bool assumeAuthor) {
            return copy (
					sourceTasklistID,
					targetTasklistID,
					targetParentTasklistID,
					triggerUID,
					cascade,
					copyMeasure,
					template,
					typ,
					assumeAuthor,
					true,
				    true,
					false, // nur Vorlagen sind persönlich
					cascade, // bei Kaskadieren auch Kontaktgruppen kopieren
					cascade // bei Kaskadieren auch Zugehörigkeit zu Tasklistgruppen kopieren
				);
        }

        /// <summary>
        /// Kopiert Pendenzenliste
        /// Falls targetTasklistID > 0, wird der Tasklistrecord nicht kopiert, sondern der Ziel-
        /// Tasklist die Kopien der zum Original gehörenden Daten zugefügt.
        /// </summary>
        /// <param name="sourceTasklistID">Original</param>
        /// <param name="targetTasklistID">Ziel-Tasklist</param>
        /// <param name="targetParentTasklistID">Falls > 0, übergeordnete Pendenzenliste</param>
        /// <param name="triggerUID"></param>
        /// <param name="cascade">
        ///		Untergeordnetes (untergeordnete Pendenzenlisten, Links zu anderer Pendenzenlisten,
        ///		Ablagen) auch kopieren
        /// </param>
        /// <param name="copyMeasure">Pendenzen kopieren</param>
        /// <param name="template">Vorlage</param>
        /// <param name="typ"></param>
        /// <param name="assumeAuthor">Der kopierende Benutzer wird als Autor eingetragen</param>
        /// <param name="inherit">Benachrichtigung beim rekursiven Kopieren</param>
        /// <param name="sendMessage">Soll eine Benachrichtigung stattfinden? (false für Vorlagen)</param>
        /// <param name="assumeOwner">Der kopierende Benutzer wird als Eigentümer eingetragen</param>
        /// <param name="copyContactGroup">Kontaktgruppen dupplizieren</param>
        /// <param name="copyTasklistGroupAssignments">Zuordnungen zu Tasklistgruppen kopieren</param>
		/// <param name="attributesToAnnull">
		///		Liste der Attribute deren Wert nicht übernommen wird.
		///		(Format: Bsp "MEASURE.STARTDATE" in Uppercase!)
		/// </param>
		/// <returns>Id der Kopie</returns>
		private long copy (
			long sourceTasklistID,
			long targetTasklistID,
			long targetParentTasklistID,
			long triggerUID,
			bool cascade,
			bool copyMeasure,
			bool template,
			int typ,
			bool assumeAuthor,
			bool inherit,
			bool sendMessage,
			bool assumeOwner,
			bool copyContactGroup,
			bool copyTasklistGroupAssignments,
			params string [] attributesToAnnull
		)
		{
			// Kopieren der zugehörigen Kontaktgruppe
			long contactGroupId = 0;

			if (copyContactGroup && Global.isModuleEnabled("contact"))
			{
				long sourceContactGroupId = DBColumn.GetValid(
						_db.lookup("contact_group_id", "tasklist", "id=" + sourceTasklistID),
						(long)0
					);

				if (sourceContactGroupId > 0)
				{
					if (targetTasklistID > 0)
					{
						contactGroupId = DBColumn.GetValid(
								_db.lookup("contact_group_id", "tasklist", "id=" + targetTasklistID),
								(long)0
							);
					}

					contactGroupId = _db.ContactGroup.copy(sourceContactGroupId, contactGroupId);
				}
			}
			
            // copy tasklist record
            if (targetTasklistID > 0)
            {
                if (contactGroupId > 0)
                {
                    _db.execute(
                        "update TASKLIST set CONTACT_GROUP_ID=" + contactGroupId
                            + " where id=" + targetTasklistID
                    );
                }
            }
            else
			{
                targetTasklistID = _db.newId("TASKLIST");

				string colNames = "," + _db.getColumnNames("TASKLIST") + ",";
                string attrs = colNames;
                attrs = attrs.Replace(",ID,", ","+targetTasklistID+",");
                attrs = attrs.Replace(",EXTERNAL_REF,", ","); 
				colNames = colNames.Replace(",EXTERNAL_REF,", ",");
                attrs = attrs.Replace(",UID,", ",");
				colNames = colNames.Replace(",UID,", ",");
                long rootId = -1;

                if (targetParentTasklistID > 0)
				{
                    rootId = DBColumn.GetValid(
							_db.lookup("root_id","tasklist","id=" + targetParentTasklistID),
							(long)0
						);
                    attrs = attrs.Replace(",PARENT_ID,", "," + targetParentTasklistID + ",");
                }
                else
				{
                    rootId = targetTasklistID;
					attrs = attrs.Replace(",PARENT_ID,", ",null,");
                }

                attrs = attrs.Replace(",ROOT_ID,", "," + rootId + ",");
                attrs = attrs.Replace(",TEMPLATE,", "," + (template ? "1" : "0") + ",");
                attrs = attrs.Replace(",TYP,", "," + typ + ",");
				attrs = attrs.Replace(",CLIPBOARD_ID,", ",null,");

				if (triggerUID > 0)
				{
                    attrs = attrs.Replace(",TRIGGER_UID,", "," + triggerUID + ",");
                }
                else
				{
                    attrs = attrs.Replace(",TRIGGER_UID,", ",null,");
                }

				if (assumeAuthor)
				{
					attrs = attrs.Replace(",AUTHOR_PERSON_ID,", ","+SessionData.getUserID(_session)+",");
				}

				if (assumeOwner)
				{
					attrs = attrs.Replace(
							",OWNER_PERSON_ID,",
							"," + SessionData.getUserID(_session) + ","
						);
				}
				else
				{
					attrs = attrs.Replace(",OWNER_PERSON_ID,", ",null,");
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

					if (attribute.Length > 1 && attribute[0] == "TASKLIST")
					{
						attrs = attrs.Replace("," + attribute[1] + ",", ",null,"); 
					}
				}

                attrs = attrs.Replace(",CREATIONDATE,", ",GetDate(),");

                attrs = attrs.Substring(1, attrs.Length - 2);
                colNames = colNames.Substring(1, colNames.Length - 2);
            
                string sql = "insert into TASKLIST ("+colNames+") select "+attrs+" from TASKLIST where ID="+sourceTasklistID;

				if (sendMessage)
				{
					_db.executeProcedure(
						"MODIFYTABLEROW",
						new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
						new ParameterCtx("USERID",_db.userId),
						new ParameterCtx("TABLENAME","TASKLIST"),
						new ParameterCtx("ROWID",targetTasklistID),
						new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
						new ParameterCtx("INHERIT",inherit ? 1 : 0)
					);
				}
				else
				{
					_db.execute(sql);
				}

                refreshAccessRights(targetTasklistID);
            }

            // Kopieren der zugehörigen Pendenzen
			if (copyMeasure)
			{
                DataTable table = _db.getDataTable("select ID from MEASURE where TASKLIST_ID=" + sourceTasklistID);

                foreach (DataRow row in table.Rows) {
                    long measureID = DBColumn.GetValid(row[0],0L);
                    _db.Measure.copy(
						measureID,
						targetTasklistID,
						triggerUID,
						cascade,
						template,
						assumeAuthor,
						false,
						sendMessage,
						copyContactGroup,
						attributesToAnnull
					);
                }
            }

            // Kopieren der Ablage, Child-Pendenzenlisten und der Tasklist-Assignments
			if (cascade)
			{
                // copy shelf...
                long clipboardId = DBColumn.GetValid(_db.lookup("CLIPBOARD_ID", "TASKLIST", "ID=" + sourceTasklistID), 0L);
                if (clipboardId > 0) {
                    long tasklistUID = DBColumn.GetValid(_db.lookup("UID", "TASKLIST", "ID="+targetTasklistID), 0L);
                    clipboardId = _db.Clipboard.copy(
							clipboardId,
							true,
							_db.userAccessorID,
							tasklistUID,
							template,
							Clipboard.TYPE_PRIVATE,
							true
						);
                    _db.execute("update TASKLIST set clipboard_id=" + clipboardId + " where id=" + targetTasklistID);
                }

                // copy subordinate tasklists...
                DataTable table = _db.getDataTable("select ID from TASKLIST where PARENT_ID=" + sourceTasklistID);
                foreach (DataRow row in table.Rows) {
                    long taskID = DBColumn.GetValid(row[0],0L);
                    copy(
						taskID,
						-1,
						targetTasklistID,
						triggerUID,
						true,
						copyMeasure,
						template,
						typ,
						assumeAuthor,
						false,
						sendMessage,
						assumeOwner,
						copyContactGroup,
						copyTasklistGroupAssignments,
						attributesToAnnull
					);
                }

                table = _db.getDataTable("select distinct ASSIGNED_TASKLIST_ID from TASKLIST_ASSIGNMENT where PARENT_TASKLIST_ID=" + sourceTasklistID);
                foreach (DataRow row in table.Rows) 
				{
                    long assTaskID = DBColumn.GetValid(row[0], 0L);
                    _db.execute("insert into TASKLIST_ASSIGNMENT (PARENT_TASKLIST_ID, ASSIGNED_TASKLIST_ID) values (" + targetTasklistID + "," + assTaskID + ")");
                }
            }

			if (copyTasklistGroupAssignments)
			{
                long taslistGroupId = 0;
				DataTable table = _db.getDataTable(
						"select distinct TASKLIST_GROUP_ID from TASKLIST_GROUP_TASKLIST"
							+ " where TASKLIST_ID=" + sourceTasklistID
					);

                foreach (DataRow row in table.Rows) 
				{
                    taslistGroupId = DBColumn.GetValid(row[0], (long)0);
                    _db.TasklistGroup.addTasklist_group_tasklist(taslistGroupId, targetTasklistID);
                }
			}

            return targetTasklistID;
        }

        /// <summary>
        /// Copies, the all the measures of the source-tasklist to the target-tasklist, but doesn't copy the tasklist itself
        /// </summary>
        /// <param name="sourceTasklistID">ID of the source-tasklist where the structure will be copied from</param>
        /// <param name="targetTasklistID">ID of the target-tasklist where the structure will be copied to</param>
        /// <param name="template">true: the new measures will be marked as template</param>
        /// <returns>true, if the copying was successful, otherwise false</returns>
        public bool copyMeasures(long sourceTasklistID, long targetTasklistID, bool cascade, bool template) {
            bool retValue = true;

            string sql = "select ID from MEASURE where TASKLIST_ID=" + sourceTasklistID;
            DataTable table = _db.getDataTable(sql);

            foreach (DataRow row in table.Rows) {
                long measureID = long.Parse(row[0].ToString());
                _db.Measure.copy(measureID, targetTasklistID, -1, cascade, template, true);
            }

            return retValue;
        }

        public void refreshAccessRights(long tasklistID) {
            if (tasklistID < 1)
                return;

            // set rights for author
            long authorID = ch.psoft.Util.Validate.GetValid(_db.lookup("AUTHOR_PERSON_ID", "TASKLIST", "ID=" + tasklistID, false), -1L);
            if (authorID > 0) {
                _db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, _db.getAccessorID(authorID), "TASKLIST", tasklistID);
            }
        }

        /// <summary>
        /// Returns the minimum semaphore state based on the states of the assigned measures
        /// </summary>
        /// <param name="ID">Identifier of task list</param>
        /// <param name="cascade">cascade</param>
        /// <returns>0: red, 1: orange, 2: green, 3: done</returns>
        public int getSemaphore(long ID, bool cascade) {
            return getSemaphore(ID,0,cascade);
        }
		/// <summary>
		/// Returns the minimum semaphore state based on the states of the assigned measures
		/// </summary>
		/// <param name="ID">Identifier of task list</param>
		/// <param name="cascade">cascade</param>
		/// <param name="useCritical">check for critical flag</param>
		/// <returns>0: red, 1: orange, 2: green, 3: done</returns>
		public int getSemaphore(long ID, bool cascade, bool useCritical) 
		{
			return getSemaphore(ID,0,cascade,useCritical);
		}
		/// <summary>
		/// Returns the minimum semaphore state based on the states of the assigned measures
		/// </summary>
		/// <param name="ID">Identifier of task list</param>
		/// <param name="criticalDays">criticalDays</param>
		/// <param name="cascade">cascade</param>
		/// <returns>0: red, 1: orange, 2: green, 3: done</returns>
		public int getSemaphore(long ID, int criticalDays, bool cascade)
		{
			return getSemaphore(ID,criticalDays,cascade,false);
		}
		/// <summary>
		/// Returns the minimum semaphore state based on the states of the assigned measures
		/// </summary>
		/// <param name="ID">Identifier of task list</param>
		/// <param name="criticalDays">criticalDays</param>
		/// <param name="cascade">cascade</param>
		/// <param name="useCritical">check for critical flag</param>
		/// <returns>0: red, 1: orange, 2: green, 3: done</returns>
		public int getSemaphore(long ID, int criticalDays, bool cascade, bool useCritical)
		{
			return getSemaphore(ID,false,criticalDays,cascade,false);
		}
		/// <summary>
		/// Returns the minimum semaphore state based on the states of the assigned measures
		/// </summary>
		/// <param name="ID">Identifier of task list</param>
		/// <param name="overLoadCriticalDays">overload the tasklist's criticaldays</param>
		/// <param name="criticalDays">criticalDays</param>
		/// <param name="cascade">cascade</param>
		/// <param name="useCritical">check for critical flag</param>
		/// <returns>0: red, 1: orange, 2: green, 3: done</returns>
        public int getSemaphore(long ID, bool overLoadCriticalDays, int criticalDays, bool cascade, bool useCritical)
        {
            string sql = "";
            DataTable table;
            int retVal = 3; // grau
			bool oneIsCritical = false;

            object [] values = _db.lookup(
                    new string [] {"template", "criticaldays"},
                    "tasklist",
                    "id=" + ID
                );

            // Vorlagen immer grau
            if (DBColumn.GetValid(values[0], "0") == "0") 
            {
				if (!overLoadCriticalDays)
				{
					int cd = DBColumn.GetValid(values[1], 0);
					criticalDays = cd > 0 ? cd : criticalDays;
				}
                sql = "select DUEDATE,CRITICAL from MEASURE where TASKLIST_ID=" + ID + " and STATE=0";
                table = _db.getDataTable(sql);

                if (table.Rows.Count > 0) 
                {
                    DateTime date = DateTime.Now;
                    DateTime dueDate;
					int returnRed = -1;
					int returnOrange = -1;
					
                    foreach (DataRow row in table.Rows) 
                    {
						bool actIsCritical = false;
						if (useCritical && int.Parse(Validate.GetValid(row[1].ToString(),"0")) == 1) 
						{
							oneIsCritical = true;
							actIsCritical = true;
						}
                        if (!DBColumn.IsNull(row[0])) 
                        {
                            dueDate = DBColumn.GetValid(row[0], DateTime.MaxValue);
							if (date > dueDate) 
							{
								returnRed = ((returnRed != 10) ? ((actIsCritical)? 10 : 0) : returnRed);
							}
							dueDate = dueDate.AddDays(-criticalDays);
							if (date > dueDate) 
							{
								returnOrange = ((returnOrange != 11) ? ((actIsCritical)? 11 : 1) : returnOrange);
							}
                        }
                    }

					if (oneIsCritical)
					{
						if (returnOrange == 11) retVal = 1; // Orange
						else retVal = 2; // Green
						if (returnRed == 10) retVal = 0; // Red
					}
					else 
					{
						if (returnOrange == 1) retVal = 1; // Orange
						else retVal = 2; // Green
						if (returnRed == 0) retVal = 0; // Red
					}
                }
                else 
                {
                    long id = DBColumn.GetValid(_db.lookup("ID","MEASURE","TASKLIST_ID=" + ID),0L);
                    if (id > 0) retVal = 3; // Done
                    else retVal = 2; // Green
                }

                if (cascade) 
                {
                    sql = "select id from TASKLIST where PARENT_ID=" + ID + " union select ASSIGNED_TASKLIST_ID from TASKLIST_ASSIGNMENT where PARENT_TASKLIST_ID=" + ID;
                    table = _db.getDataTable(sql);
                    foreach (DataRow row in table.Rows) 
                    {
                        retVal = Math.Min(retVal,getSemaphore((long) row[0],criticalDays,true,useCritical));
                        if (retVal == 0 || retVal == 10) return retVal; //redder than red is not possible!
                    }
                }
                if (retVal == 3)
                {
                    if (DBColumn.GetValid(_db.lookup("KEEP_OPEN","TASKLIST","ID=" + ID),0) > 0)
                    {
                        retVal = 2;
                    }
                }
				if (oneIsCritical)
					retVal = retVal + 10;
            }

            return retVal;
        }

        /// <summary>
        /// Get query for tasklists by semaphore value
        /// </summary>
        /// <param name="semaphore">0: red, 1: orange, 2: green, 3: done</param>
        /// <param name="onlyRoots">set to true if interested only in root</param>
        /// <returns>tasklist-id's</returns>
        public string GetTasklistBySemaphore(int semaphore, bool onlyRoots) {
            string retValue = "";
            bool isFirst = true;
            DataTable table = _db.getDataTable("select ID from TASKLIST where TEMPLATE=0" + (onlyRoots? " and PARENT_ID is null" :""));
            foreach (DataRow row in table.Rows){
                long ID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1L);
                if (getSemaphore(ID, true) == semaphore){
                    if (isFirst){
                        isFirst = false;
                    }
                    else{
                        retValue += ",";
                    }

                    retValue += ID.ToString();
                }
            }

            return retValue;
        }

        public int getCriticalDays(long ID) {
            return _db.lookup("CRITICALDAYS", "TASKLIST", "ID="+ID, 1);
        }

        /// <summary>
        /// Returns the number of assigned measures still in 'open' state
        /// </summary>
        /// <param name="ID">Identifier of task list</param>
        /// <returns></returns>
        public int getOpenMeasureCount(long ID, bool cascade) {
            int cnt = 0;

            if (cascade) {
                DataTable table = _db.getDataTable("select id from TASKLIST where PARENT_ID=" + ID + " union select ASSIGNED_TASKLIST_ID from TASKLIST_ASSIGNMENT where PARENT_TASKLIST_ID=" + ID);
                foreach (DataRow row in table.Rows) {
                    cnt += getOpenMeasureCount(DBColumn.GetValid(row[0],0L),true);
                }
            }
            return cnt + DBColumn.GetValid(_db.lookup("count(ID)", "MEASURE", "TASKLIST_ID=" + ID + " and STATE=0"),0);
        }

        /// <summary>
        /// Returns table (ID, TITLE) of all tasklist templates
        /// </summary>
        /// <returns>DataTable with tasklist templates</returns>
        public DataTable getTemplatesTable() {
            long owner_person_id = SessionData.getUserID(_session);
            string templateSql = "select ID, TITLE + ' (' + isNull(TYPE,'') + ')' as TITLE from TASKLIST"
                    + " where isnull(PARENT_ID,0) = 0 and TEMPLATE=1 and TYP=" + TYPE_PUBLIC
                    + " and (OWNER_PERSON_ID is null or OWNER_PERSON_ID=" + owner_person_id + ")";
            return _db.getDataTable(templateSql);
        }

        /// <summary>
        /// Returns comma-separated list of all subordinate tasklist-IDs
        /// </summary>
        /// <param name="tasklistIDs">Comma-separated list of (root-)tasklist-IDs</param>
        /// <returns>Comma-separated list of subordinate tasklist-IDs</returns>
        public string addAllSubTasklistIDs(string tasklistIDs){
            return _db.Tree("TASKLISTTREEV").AddAllSubnodes(tasklistIDs);
        }

        /// <summary>
        /// Returns comma-separated list of all parent tasklist-IDs (recursive)
        /// </summary>
        /// <param name="tasklistIDs">Comma-separated list of tasklist-IDs</param>
        /// <returns>Comma-separated list of all parent tasklist-IDs</returns>
        public string addAllParentTasklistIDs(string tasklistIDs){
            string retValue = tasklistIDs;
            string parentIDs = tasklistIDs;
            
            do{
                parentIDs = getParentTasklistIDs(parentIDs);
                if (parentIDs != ""){
                    retValue += "," + parentIDs;
                }
            } while (parentIDs != "");

            return retValue;
        }

        /// <summary>
        /// Returns comma-separated list of all direct parent tasklist-IDs
        /// </summary>
        /// <param name="tasklistIDs">Comma-separated list of tasklist-IDs</param>
        /// <returns>Comma-separated list of direct parent tasklist-IDs</returns>
        public string getParentTasklistIDs(string tasklistIDs){
            string retValue = "";
            bool isFirst = true;
            DataTable table = _db.getDataTable("select PARENT_ID from TASKLISTTREEV where ID in (" + tasklistIDs + ")");
            
            foreach (DataRow row in table.Rows){
                long parentID = DBColumn.GetValid(row[0], -1L);
                if (parentID > 0){
                    if (isFirst){
                        isFirst = false;
                    }
                    else{
                        retValue += ",";
                    }
                    retValue += parentID.ToString();
                }
            }
            
            return retValue;
        }

        /// <summary>
        /// Checks if a tasklist is assigned to a root-tasklist
        /// </summary>
        /// <param name="tasklistID">ID of tasklist to check</param>
        /// <param name="rootTasklistID">ID of root-tasklist</param>
        /// <returns>true, if tasklist is assigned to root-tasklist</returns>
        public bool isAssignedTasklist(long tasklistID, long rootTasklistID){
            return getAssignedParentTasklist(tasklistID, rootTasklistID) > 0;
        }

        /// <summary>
        /// Returns assigned parent tasklist-ID of a tasklist within tree
        /// </summary>
        /// <param name="tasklistID">ID of tasklist to analyse</param>
        /// <param name="rootTasklistID">ID of root-tasklist (identifies the tasklist-tree)</param>
        /// <returns>ID of assigned parent tasklist-ID if assigned, otherwise -1</returns>
        public long getAssignedParentTasklist(long tasklistID, long rootTasklistID){
            string parentTasklistIDs = rootTasklistID.ToString();
            do{
                DataTable table = _db.getDataTable("select ID, ISASSIGNED, PARENT_ID from TASKLISTTREEV where PARENT_ID in (" + parentTasklistIDs + ")");
                parentTasklistIDs = "";
                bool isFirst = true;
                foreach (DataRow row in table.Rows){
                    long subTasklistID = DBColumn.GetValid(row[0], -1L);
                    if (tasklistID == subTasklistID && DBColumn.GetValid(row[1], 0) == 1){
                        return DBColumn.GetValid(row[2], -1L);
                    }
                    if (isFirst){
                        isFirst = false;
                    }
                    else{
                        parentTasklistIDs += ",";
                    }
                    parentTasklistIDs += subTasklistID.ToString();
                }
            } while(parentTasklistIDs != "");
            return -1;
        }

        public long getUIDByTitle(string title){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("UID", "TASKLIST", "TITLE='" + DBColumn.toSql(title) + "' and TEMPLATE=0 and TYP=0", false), -1L);
        }

        public string getTitle(string id)
        {
            return ch.psoft.Util.Validate.GetValid(_db.lookup("TITLE", "TASKLIST", "ID=" + DBColumn.toSql(id) + " and TEMPLATE=0 and TYP=0", false), "");
        }

    }
}
