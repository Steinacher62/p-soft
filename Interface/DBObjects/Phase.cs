using ch.appl.psoft.db;
using ch.psoft.db;
using System;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    public class Phase : DBObject
	{
        public Phase(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade)
        {
			return delete(ID, cascade, true);
        }

        public int delete(long ID, bool cascade, bool doCreateMessage)
        {
            int numDel = 0;

            long tasklistID = -1;
            if (cascade) {
                tasklistID = DBColumn.GetValid(_db.lookup("TASKLIST_ID", "PHASE", "ID=" + ID), -1L);
            }

            // delete phase-dependencies...
            _db.execute("delete from PHASE_DEPENDENCY where MASTER_PHASE_ID=" + ID + " or SLAVE_PHASE_ID=" + ID);

            // delete phase...
            string sql = "delete from PHASE where ID=" + ID;

            ParameterCtx rows = new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int));
            _db.executeProcedure("MODIFYTABLEROW",
                rows,
                new ParameterCtx("USERID",_db.userId),
                new ParameterCtx("TABLENAME","PHASE"),
                new ParameterCtx("ROWID",ID),
                new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                new ParameterCtx("INHERIT",1)
                );

            numDel = _db.parameterValue(rows,0);

            // delete tasklist...
            if (tasklistID > 0){
                _db.Tasklist.delete(tasklistID, true);
            }

			//delete subscriptions
			_db.Subscription.deleteSubscriptions("PHASE",ID);

            return numDel;
        }

        /// <summary>
        /// Copies a phase
        /// </summary>
        /// <param name="ID">Identifier of the Phase to copy</param>
        /// <param name="targetProjectID">Identifier of the target project</param>
        /// <param name="template">True: The new Phase is flagged as template</param>
        /// <param name="assumeAuthor">True: The new Phase will assume the leader of the logged user</param>
        /// <param name="withRegistry">true: Registry-Einträge werden auch kopiert</param>
        /// <returns></returns>
        public long copy(long ID, long targetProjectID, bool cascade, bool template, bool assumeLeader, bool withRegistry) 
        {
            long newPhaseID = _db.newId("PHASE");
            string colNames = "," + _db.getColumnNames("PHASE") + ",";
            string attrs = colNames;
            
            attrs = attrs.Replace(",ID,", ","+newPhaseID+",");
            attrs = attrs.Replace(",CREATIONDATE,", ",GetDate(),");
            attrs = attrs.Replace(",TASKLIST_ID,", ",null,");
            attrs = attrs.Replace(",TEMPLATE,", "," + (template? "1":"0") + ",");
            attrs = attrs.Replace(",EXTERNAL_REF,", ","); colNames = colNames.Replace(",EXTERNAL_REF,", ",");
            attrs = attrs.Replace(",UID,", ","); colNames = colNames.Replace(",UID,", ",");
            if (targetProjectID > 0){
                attrs = attrs.Replace(",PROJECT_ID,", ","+targetProjectID+",");
            }
            if (assumeLeader)
                attrs = attrs.Replace(",LEADER_PERSON_ID,", ","+SessionData.getUserID(_session)+",");

            // shift the start/due-dates accordingly...
            DateTime templateCreation = _db.lookup("CREATIONDATE", "PHASE", "ID=" + ID, DateTime.Now);
            TimeSpan timeShift = DateTime.Now - templateCreation;
            attrs = attrs.Replace(",STARTDATE,", ",STARTDATE+" + timeShift.Days + ",");
            attrs = attrs.Replace(",DUEDATE,", ",DUEDATE+" + timeShift.Days + ",");

            attrs = attrs.Substring(1,attrs.Length-2);
            colNames = colNames.Substring(1,colNames.Length-2);
               
            string sql = "insert into PHASE ("+colNames+") select "+attrs+" from PHASE where ID="+ID;

            _db.executeProcedure("MODIFYTABLEROW",
                new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                new ParameterCtx("USERID",_db.userId),
                new ParameterCtx("TABLENAME","PROJECT"),
                new ParameterCtx("ROWID",targetProjectID),
                new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                new ParameterCtx("INHERIT",1)
                );
            
            setDefaultRights(newPhaseID);

            if (cascade) {
                // copy tasklist...
                long tasklistID = DBColumn.GetValid(_db.lookup("TASKLIST_ID", "PHASE", "ID="+ID), 0L);
                if (tasklistID > 0) {
                    tasklistID = _db.Tasklist.copy(tasklistID, -1, -1, _db.ID2UID(newPhaseID, "PHASE"), true, true, template, Tasklist.TYPE_PRIVATE, assumeLeader);
                    _db.execute("update PHASE set TASKLIST_ID=" + tasklistID + " where id = " + newPhaseID);
                }
            }

            if (withRegistry) {
                _db.Registry.copyRegistryEntries("PHASE", ID, "PHASE", newPhaseID);
            }

            return newPhaseID;
        }


        /// <summary>
        /// Returns the semaphore state based on the due-date and the number of critical days
        /// </summary>
        /// <param name="ID">Identifier of measure</param>
        /// <param name="useCriticalInvolve">tasklist inheritance bottom-up</param>
        /// <returns>0: red, 1: orange, 2: green, 3: done</returns>
		public int getSemaphore(long ID, bool useCriticalInvolve) 
		{
			return getSemaphore(ID,0,useCriticalInvolve);
		}
		/// <summary>
		/// Returns the semaphore state based on the due-date and the number of critical days
		/// </summary>
		/// <param name="ID">Identifier of measure</param>
		/// <param name="criticalDays">criticalDays</param>
		/// <returns>0: red, 1: orange, 2: green, 3: done</returns>
		public int getSemaphore(long ID, int criticalDays) 
		{
			return getSemaphore(ID,criticalDays,false);
		}
		/// <summary>
		/// Returns the semaphore state based on the due-date and the number of critical days
		/// </summary>
		/// <param name="ID">Identifier of measure</param>
		/// <param name="criticalDays">criticalDays</param>
		/// <param name="useCriticalInvolve">tasklist inheritance bottom-up</param>
		/// <returns>0: red, 1: orange, 2: green, 3: done</returns>
        public int getSemaphore(long ID, int criticalDays, bool useCriticalInvolve)
        {
			int retVal = 3;

            DataTable table = _db.getDataTable("select STATE,DUEDATE,TASKLIST_ID from PHASE where ID=" + ID);
            
            if (table.Rows.Count <= 0)
                return -1;
     
            DataRow row = table.Rows[0];
            DateTime dueDate = DBColumn.GetValid(row["DUEDATE"], DateTime.MaxValue);

            if (row["STATE"].ToString() == "1")
                return 3;
            else if (dueDate > (DateTime.Now.AddDays(criticalDays)))
                retVal = 2;
            else if (dueDate > DateTime.Now)
                retVal = 1;
            else
                retVal = 0;

			long taskListId = int.Parse(DBColumn.GetValid(row["TASKLIST_ID"].ToString(),"0"));
			int taskListSemaphore = _db.Tasklist.getSemaphore(taskListId,true,criticalDays,true,true);
			if (taskListSemaphore > 9) 
			{
				retVal = Math.Min(taskListSemaphore - 10, retVal) + ((useCriticalInvolve)? 10 : 0);
			}

			return retVal;
        }

        public long getLeaderID(long phaseID) {
            return ch.psoft.Util.Validate.GetValid(_db.lookup("LEADER_PERSON_ID", "PHASE", "ID=" + phaseID, false), -1L);
        }

        /// <summary>
        /// Sets/changes the leader person
        /// </summary>
        /// <param name="ID">Identifier of phase</param>
        /// <param name="leaderID">Identifier of leader person</param>
        public void setLeader(long ID, long leaderID)
        {
            // revoke the old leader's rights and set the rights for the new responsibe...
            long oldLeaderID = getLeaderID(ID);
            if (oldLeaderID > 0){
                _db.revokeRowAuthorisation(DBData.AUTHORISATION.RAUDI, _db.getAccessorID(oldLeaderID), "PHASE", ID);
            }

            if (leaderID > 0){
                _db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, _db.getAccessorID(leaderID), "PHASE", ID);
            }

            // change the leader person
            string sql = "update PHASE set LEADER_PERSON_ID=" + leaderID + " where ID=" + ID;
            _db.executeProcedure("MODIFYTABLEROW",
                new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                new ParameterCtx("USERID",_db.userId),
                new ParameterCtx("TABLENAME","PHASE"),
                new ParameterCtx("ROWID",ID),
                new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                new ParameterCtx("INHERIT",1)
                );
        }

        public void setDefaultRights(long phaseID)
        {
            if (phaseID < 1)
                return;

            // set rights for leader
            long leaderID = getLeaderID(phaseID);
            if (leaderID > 0){
                long leaderAccessorID = _db.getAccessorID(leaderID);
                _db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, leaderAccessorID, "PHASE", phaseID);
            }
        }

		public bool createMessage(long phaseID, int action, bool inherit)
		{
			bool retVal = false;
			_db.executeProcedure("CREATEMESSAGE",
				new ParameterCtx("ID",System.DBNull.Value),
				new ParameterCtx("USERID",_db.userId),
				new ParameterCtx("TABLENAME","PHASE"),
				new ParameterCtx("ROWID",phaseID),
				new ParameterCtx("TRIGGERNAME","PHASE"),
				new ParameterCtx("TRIGGERID",phaseID),
				new ParameterCtx("TRIGGERATTRIBUT","TITLE"),
				new ParameterCtx("TRIGGERACTION",action),
				new ParameterCtx("ACTION",action),
				new ParameterCtx("INHERIT",inherit ? 1 : 0));

			retVal = true;

			return retVal;
		}

		public long getPhaseIDbyTasklist(long tasklistID)
		{
			return ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "PHASE", "TASKLIST_ID=" + tasklistID, false), -1L);
		}
    }
}
