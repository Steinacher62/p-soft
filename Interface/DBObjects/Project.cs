using ch.appl.psoft.db;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    public class Project : DBObject {

        public const string TableName = "PROJECT";

        public Project(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return delete(ID, cascade, true);
        }

        public int delete(long ID, bool cascade, bool createMessage) {
            int numDel = 0;
            string sql = "";
            long clipboardID = -1;
            long organisationID = -1;

            if (cascade) {
                // delete sub-projects...
                sql = "select ID from PROJECT where PARENT_ID=" + ID;
                DataTable table = _db.getDataTable(sql);

                foreach (DataRow row in table.Rows) {
                    delete(DBColumn.GetValid(row[0],0L),true,createMessage);
                }
                
                // delete phases...
                sql = "select ID from PHASE where PROJECT_ID=" + ID;
                table = _db.getDataTable(sql);
                foreach (DataRow row in table.Rows) {
                    numDel += _db.Phase.delete(DBColumn.GetValid(row[0],0L), true, false);
                }

                // delete billing...
                sql = "select ID from PROJECT_BILLING where PROJECT_ID=" + ID;
                table = _db.getDataTable(sql);
                foreach (DataRow row in table.Rows)
                {
                    numDel += _db.ProjectBilling.delete(DBColumn.GetValid(row[0], 0L), true);
                }

                clipboardID = getClipboardID(ID);
                organisationID = getProjectOrganisationID(ID);
            }

            sql = "delete from PROJECT where ID=" + ID;
            ParameterCtx rows = new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int));
            _db.executeProcedure("MODIFYTABLEROW",
                rows,
                new ParameterCtx("USERID",_db.userId),
                new ParameterCtx("TABLENAME","PROJECT"),
                new ParameterCtx("ROWID",ID),
                new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                new ParameterCtx("INHERIT",1)
                );

            numDel += _db.parameterValue(rows,0);

            // delete clipboard
            if (clipboardID > 0){
                _db.Clipboard.delete(clipboardID,true);
            }

            // delete organisation/organigrams
            if (organisationID > 0){
                _db.Organisation.delete(organisationID, true);
            }

			//delete subscriptions
			_db.Subscription.deleteSubscriptions("PROJECT",ID);

            return numDel;
        }

        /// <summary>
        /// Copy Project
        /// </summary>
        /// <param name="sourceProjectID">project id</param>
        /// <param name="targetParentProjectID">id of the parent-project where the project should be copied to</param>
        /// <param name="targetProjectID">id of the already existing project</param>
        /// <param name="cascade">true: copy recursive</param>
        /// <param name="copyPhase">true: copy phase also</param>
        /// <param name="template">true: flag copy as template</param>
        /// <param name="assumeLeader">true: the current user will be the project-leader</param>
        /// <param name="withRegistry">true: kopiert die Registry-Einträge für das aktuelle Projekt</param>
        /// <param name="allChildsWithRegistry">true: kopiert die Registry-Einträge für alle untergeordneten Projekte und Phasen
        /// <returns>new project ID</returns>
        public long copy (long sourceProjectID, long targetProjectID, long targetParentProjectID, bool cascade, bool copyPhase, bool template, bool assumeLeader, bool withRegistry, bool allChildsWithRegistry) {
            if (targetProjectID <= 0){
                targetProjectID = _db.newId("PROJECT");
                string colNames = "," + _db.getColumnNames("PROJECT") + ",";
                string attrs = colNames;
                attrs = attrs.Replace(",ID,", "," + targetProjectID + ",");
                attrs = attrs.Replace(",EXTERNAL_REF,", ","); colNames = colNames.Replace(",EXTERNAL_REF,", ",");
                attrs = attrs.Replace(",UID,", ","); colNames = colNames.Replace(",UID,", ",");
                long rootId = targetProjectID;
                if (targetParentProjectID > 0) {
                    rootId = DBColumn.GetValid(_db.lookup("ROOT_ID", "PROJECT", "ID=" + targetParentProjectID), 0L);
                    attrs = attrs.Replace(",PARENT_ID,", "," + targetParentProjectID + ",");
                }
                else{
                    attrs = attrs.Replace(",PARENT_ID,", ",null,");
                }
                attrs = attrs.Replace(",ROOT_ID,", "," + rootId + ",");
                attrs = attrs.Replace(",CREATIONDATE,", ",GetDate(),");
                attrs = attrs.Replace(",CLIPBOARD_ID,", ",null,");
                attrs = attrs.Replace(",TEMPLATE,", "," + (template? "1":"0") + ",");

                attrs = attrs.Replace(",ORGANISATION_ID,", ",null,");
                attrs = attrs.Replace(",CHART_ID,", ",null,");
                attrs = attrs.Replace(",COMMITEE_ORGENTITY_ID,", ",null,");
                attrs = attrs.Replace(",LEADER_ORGENTITY_ID,", ",null,");

                attrs = attrs.Substring(1, attrs.Length-2);
                colNames = colNames.Substring(1, colNames.Length-2);
            
                string sql = "insert into PROJECT (" + colNames + ") select " + attrs + " from PROJECT where ID=" + sourceProjectID;
                _db.executeProcedure("MODIFYTABLEROW",
                    new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                    new ParameterCtx("USERID",_db.userId),
                    new ParameterCtx("TABLENAME","PROJECT"),
                    new ParameterCtx("ROWID",targetProjectID),
                    new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                    new ParameterCtx("INHERIT",1)
                    );

                //create default organisation structure / organigram...
                createProjectOrganisation(targetProjectID, assumeLeader? _db.userId : -1L);

                //Projektteam übernehmen...
                copyProjectTeam(sourceProjectID, targetProjectID);
            }

            if (copyPhase) {
                copyPhases(sourceProjectID, targetProjectID, cascade, template, assumeLeader, allChildsWithRegistry);
            }

            if (cascade) {
                // copy clipboard...
                long clipboardID = getClipboardID(sourceProjectID);
                if (clipboardID > 0) {
                    long projectUID = DBColumn.GetValid(_db.lookup("UID", "PROJECT", "ID="+targetProjectID), 0L);
                    clipboardID = _db.Clipboard.copy(
							clipboardID,
							true,
							_db.userAccessorID,
							projectUID,
							template,
							Clipboard.TYPE_PRIVATE,
							allChildsWithRegistry
						);
                    _db.execute("update PROJECT set CLIPBOARD_ID=" + clipboardID + " where id = " + targetProjectID);
                }

                // copy tasklist...
                long tasklistID = DBColumn.GetValid(_db.lookup("TASKLIST_ID", "PROJECT", "ID="+sourceProjectID), 0L);
                if (tasklistID > 0) {
                    tasklistID = _db.Tasklist.copy(tasklistID, -1, -1, _db.ID2UID(targetProjectID, "PROJECT"), true, true, template, Tasklist.TYPE_PRIVATE, assumeLeader);
                    _db.execute("update PROJECT set TASKLIST_ID=" + tasklistID + " where id = " + targetProjectID);
                }

                DataTable table = _db.getDataTable("select ID from PROJECT where PARENT_ID=" + sourceProjectID);
                foreach (DataRow row in table.Rows) {
                    long projectID = DBColumn.GetValid(row[0],0L);
                    copy(projectID, -1, targetProjectID, cascade, copyPhase, template, assumeLeader, allChildsWithRegistry, allChildsWithRegistry);
                }
            }

            if (withRegistry) {
                _db.Registry.copyRegistryEntries("PROJECT", sourceProjectID, "PROJECT", targetProjectID);
            }

            return targetProjectID;
        }

        /// <summary>
        /// Copies, the all the phases of the source-project to the target-project, but doesn't copy the project itself
        /// </summary>
        /// <param name="sourceProjectID">ID of the source-project where the structure will be copied from</param>
        /// <param name="targetProjectID">ID of the target-project where the structure will be copied to</param>
        /// <param name="template">true: the new measures will be marked as template</param>
        /// <param name="assumeLeader"></param>
        /// <param name="withRegistry">true: Registry-Einträge werden auch kopiert</param>
        /// <returns>true, if the copying was successful, otherwise false</returns>
        public bool copyPhases(long sourceProjectID, long targetProjectID, bool cascade, bool template, bool assumeLeader, bool withRegistry) {
            bool retValue = true;

            string sql = "select ID from PHASE where PROJECT_ID=" + sourceProjectID;
            DataTable table = _db.getDataTable(sql);

            foreach (DataRow row in table.Rows) {
                long phaseID = long.Parse(row[0].ToString());
                _db.Phase.copy(phaseID, targetProjectID, cascade, template, assumeLeader, withRegistry);
            }

            return retValue;
        }

        /// <summary>
        /// Returns the minimum semaphore state based on the states of the assigned phases
        /// </summary>
        /// <param name="ID">Identifier of project</param>
        /// <param name="cascade">cascade</param>
        /// <returns>0: red, 1: orange, 2: green, 3: done</returns>
        public int getSemaphore(long ID, bool cascade) {
            return getSemaphore(ID,0,cascade);
        }
		/// <summary>
		/// Returns the minimum semaphore state based on the states of the assigned phases
		/// </summary>
		/// <param name="ID">Identifier of project</param>
		/// <param name="criticalDays">criticalDays</param>
		/// <param name="cascade">cascade</param>
		/// <returns>0: red, 1: orange, 2: green, 3: done</returns>
        private int getSemaphore(long ID, int criticalDays, bool cascade) 
		{
            int retVal = 3;
			int cd = getCriticalDays(ID);
			criticalDays = cd > 0 ? cd : criticalDays;

            DataTable table = _db.getDataTable("select STATE, DUEDATE, TASKLIST_ID from PROJECT where ID=" + ID);
            
            if (table.Rows.Count <= 0)
                return -1;

            DataRow row = table.Rows[0];

			string state = row["STATE"].ToString();
			if (state == "2" || state == "3" || state == "4" || state == "5")
			{
				return 3; // grau
			}
			else if (state == "0")
			{
				return 4; // blau
			}
			else if (state == "1")
			{
                DateTime dueDate = DBColumn.GetValid(row["DUEDATE"], DateTime.MaxValue);
                if (dueDate > (DateTime.Now.AddDays(criticalDays))) 
				{
					retVal = 2; // green
				}
				else if (dueDate > DateTime.Now)
				{
					retVal = 1; // orange
				}
				else
				{
					retVal = 0; // red
				}
			}

            long taskListId = int.Parse(DBColumn.GetValid(row["TASKLIST_ID"].ToString(),"0"));
            int taskListSemaphore = _db.Tasklist.getSemaphore(taskListId,true,criticalDays,true,true);
            if (taskListSemaphore > 9) {
                retVal = Math.Min(taskListSemaphore - 10, retVal);
            }

			table = _db.getDataTable("select DUEDATE,ID from PHASE where PROJECT_ID=" + ID + " and STATE=0");

            if (table.Rows.Count > 0) {
                DateTime date = DateTime.Now;
                DateTime dueDate;
				long phaseId;
				int phaseSemaphore = -1;
				int phaseSemaphoreLast = -1;

                bool isOrange = false;
				bool isCritical = false;
//                int cd = getCriticalDays(ID);

                foreach (DataRow r in table.Rows) {
//                    criticalDays = cd > 0 ? cd : criticalDays;
                    if (!DBColumn.IsNull(r[0])) {
                        dueDate = DBColumn.GetValid(r[0], DateTime.MaxValue);
                        if (date > dueDate){
                            return 0;
                        }
                        dueDate = dueDate.AddDays(-criticalDays);
                        if (date > dueDate){
                            isOrange = true;
                        }

						phaseId = int.Parse(Validate.GetValid(r[1].ToString(),"0"));
						phaseSemaphore = _db.Phase.getSemaphore(phaseId,criticalDays,true);
						if (phaseSemaphore > 9) 
						{
							isCritical = true;
							phaseSemaphore = phaseSemaphore - 10;
						}
						if (phaseSemaphoreLast > -1)
							phaseSemaphore = Math.Min(phaseSemaphore,phaseSemaphoreLast);
						phaseSemaphoreLast = phaseSemaphore;
                    }
                }
                if (isOrange){
                    retVal = Math.Min(1, retVal);
                }
                else{
                    retVal = Math.Min(2, retVal);
                }
				if (isCritical)
				{
					retVal = Math.Min(phaseSemaphore, retVal);
				}
            }
            else {
                long id = DBColumn.GetValid(_db.lookup("ID", "PHASE","PROJECT_ID=" + ID),0L);
                if (id > 0){
                    retVal = Math.Min(3, retVal);
                }
                else{
                    retVal = Math.Min(2, retVal);
                }
            }

            if (cascade) {
                table = _db.getDataTable("select id from PROJECT where PARENT_ID = "+ID);
                foreach (DataRow r in table.Rows) {
                    retVal = Math.Min(retVal, getSemaphore((long) r[0],criticalDays, true));
                }
                if (retVal == 0){
                    return 0;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Get query for root-project by semaphore value
        /// </summary>
        /// <param name="semaphore">0: red, 1: orange, 2: green, 3: done</param>
        /// <returns>project-id's</returns>
        public string getRootProjectsBySemaphore(int semaphore) {
            string retValue = "";
            bool isFirst = true;
            DataTable table = _db.getDataTable("select ID from PROJECT where PARENT_ID is null and TEMPLATE=0");
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

        /// <summary>
        /// Returns the number of assigned phases still in 'open' state
        /// </summary>
        /// <param name="ID">Identifier of project</param>
        /// <returns></returns>
        public int getOpenPhasesCount(long ID, bool cascade) {
            int cnt = 0;

            if (cascade) {
                DataTable table = _db.getDataTable("select ID from PROJECT where PARENT_ID = "+ID);
                foreach (DataRow row in table.Rows) {
                    cnt += getOpenPhasesCount(DBColumn.GetValid(row[0],0L),true);
                }
            }
            return cnt + DBColumn.GetValid(_db.lookup("count(ID)", "PHASE", "PROJECT_ID=" + ID + " and STATE=0"),0);
        }

        /// <summary>
        /// Returns table (ID, TITLE) of all project templates
        /// </summary>
        /// <returns>DataTable with project templates</returns>
        public DataTable getTemplatesTable() {
            string templateSql = "select ID, TITLE + ' (' + isNull(NUMBER,'') + ')' as TITLE from PROJECT where PARENT_ID is null and TEMPLATE=1";
            return _db.getDataTable(templateSql);
        }

        /// <summary>
        /// Returns comma-separated string of all project-IDs where the specified person is involved in.
        /// </summary>
        /// <param name="personID">ID of a person</param>
        /// <returns>Comma-separated string of project-IDs</returns>
		public string getInvolvedProjects(long personID)
		{
			string retValue = "";
			bool isFirst = true;
			long[] involvedOrgentityIDs = getInvolvedOrgentityIDs(personID);
			string involvedOrgentityIDList = "0";
			for (int i=0; i < involvedOrgentityIDs.Length; i++)
			{
				if (isFirst)
				{
					isFirst = false;
					involvedOrgentityIDList = "";
				}
				else 
				{
					involvedOrgentityIDList += ",";
				}
				involvedOrgentityIDList += involvedOrgentityIDs[i].ToString();
			}
			DataTable table = _db.getDataTable("select distinct ROOT_ID from PROJECT where TEMPLATE=0 and (COMMITEE_ORGENTITY_ID in ("+involvedOrgentityIDList+") or LEADER_ORGENTITY_ID in ("+involvedOrgentityIDList+")" +  " or ID in(select distinct PROJECT_ID from PHASE where LEADER_PERSON_ID=" + personID + "))");
			isFirst = true;
			foreach (DataRow row in table.Rows)
			{
				if (isFirst)
				{
					isFirst = false;
				}
				else
				{
					retValue += ",";
				}
				retValue += row[0].ToString();
			}

			return retValue;
		}

		public int getCriticalDays(long ID)
		{
			return _db.lookup("CRITICALDAYS", "PROJECT", "ID="+ID, 1);
		}

        public long getUIDByTitle(string title){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("UID", "PROJECT", "TITLE='" + DBColumn.toSql(title) + "' and TEMPLATE=0", false), -1L);
        }

        public long getClipboardID(long projectID){
            return DBColumn.GetValid(_db.lookup("CLIPBOARD_ID", "PROJECT", "ID="+projectID), -1L);
        }

        public long getProjectOrganisationID(long projectID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("ORGANISATION_ID", "PROJECT", "ID=" + projectID, false), -1L);
        }

        public long createProjectOrganisation(long projectID, long leaderPersonID){
            long organisationID = getProjectOrganisationID(projectID);
            if (organisationID <= 0){
                _db.executeProcedure("CREATE_PROJECT_ORGANISATION", new ParameterCtx("ProjectID", projectID), new ParameterCtx("LeaderPersonID", leaderPersonID));
                organisationID = getProjectOrganisationID(projectID);
            }
            return organisationID;
        }

        public long getDefaultChartID(long projectID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("CHART_ID", "PROJECT", "ID=" + projectID, false), -1L);
        }

        public long[] getCustomChartIDs(long projectID){
            return buildIDs(_db.getDataTable("select distinct CHART.ID, CHART." + _db.langAttrName("CHART", "TITLE") + " from CHART inner join ORGANISATION on CHART.ORGANISATION_ID=ORGANISATION.ID inner join PROJECT on ORGANISATION.ID=PROJECT.ORGANISATION_ID where PROJECT.ID=" + projectID + " and CHART.ID not in (select CHART_ID from PROJECT where ID=" + projectID + ") order by CHART." + _db.langAttrName("CHART", "TITLE")));
        }

        public long getCommiteeOrgentityID(long projectID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("COMMITEE_ORGENTITY_ID", "PROJECT", "ID=" + projectID, false), -1L);
        }

        public long getLeaderOrgentityID(long projectID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("LEADER_ORGENTITY_ID", "PROJECT", "ID=" + projectID, false), -1L);
        }

        public long[] getCommiteePersonIDs(long projectID){
            return buildIDs(_db.getDataTable("select distinct JOB_PERS_FUNC_V.PERSON_ID, " + _db.langAttrName("JOB_PERS_FUNC_V", "PERSON") + " from JOB_PERS_FUNC_V where ORGENTITY_ID=" + getCommiteeOrgentityID(projectID) + " and JOB_TYP=1 order by " + _db.langAttrName("JOB_PERS_FUNC_V", "PERSON")));
        }

        public long[] getLeaderPersonIDs(long projectID){
            return buildIDs(_db.getDataTable("select distinct JOB_PERS_FUNC_V.PERSON_ID, " + _db.langAttrName("JOB_PERS_FUNC_V", "PERSON") + " from JOB_PERS_FUNC_V where ORGENTITY_ID=" + getLeaderOrgentityID(projectID) + " and JOB_TYP=1 order by " + _db.langAttrName("JOB_PERS_FUNC_V", "PERSON")));
        }

        public long[] getMemberPersonIDs(long projectID){
            return buildIDs(_db.getDataTable("select distinct JOB_PERS_FUNC_V.PERSON_ID, " + _db.langAttrName("JOB_PERS_FUNC_V", "PERSON") + " from JOB_PERS_FUNC_V where ORGENTITY_ID=" + getLeaderOrgentityID(projectID) + " and JOB_TYP=0 order by " + _db.langAttrName("JOB_PERS_FUNC_V", "PERSON")));
        }

		public long[] getInvolvedOrgentityIDs(long personID)
		{
			return buildIDs(_db.getDataTable("select distinct JOB_PERS_FUNC_V.ORGENTITY_ID from JOB_PERS_FUNC_V where PERSON_ID=" + personID));
		}

        protected long[] buildIDs(DataTable table){
            long[] retValue = new long[table.Rows.Count];
            for(int i=0; i<table.Rows.Count; i++){
                retValue[i] = DBColumn.GetValid(table.Rows[i][0], -1L);
            }
            return retValue;
        }

        public long addCommiteePerson(long projectID, long personID, long functionID){
            long retVal = _db.Job.add(getCommiteeOrgentityID(projectID), personID, functionID, 1);
            createMessage(projectID, (int)News.ACTION.EDIT , true);
            return retVal;
        }

        public long addLeaderPerson(long projectID, long personID, long functionID){
            long retVal = _db.Job.add(getLeaderOrgentityID(projectID), personID, functionID, 1);
            createMessage(projectID, (int)News.ACTION.EDIT , true);
            return retVal;
        }

        public long addMemberPerson(long projectID, long personID, long functionID){
            long retVal = _db.Job.add(getLeaderOrgentityID(projectID), personID, functionID, 0);
            createMessage(projectID, (int)News.ACTION.EDIT , true);
            return retVal;
        }

        public void removeCommiteePerson(long projectID, long personID){
            removePerson(getCommiteeOrgentityID(projectID), personID, 1);
            createMessage(projectID, (int)News.ACTION.EDIT , true);
        }

        public void removeLeaderPerson(long projectID, long personID){
            removePerson(getLeaderOrgentityID(projectID), personID, 1);
            createMessage(projectID, (int)News.ACTION.EDIT , true);
        }

        public void removeMemberPerson(long projectID, long personID){
            removePerson(getLeaderOrgentityID(projectID), personID, 0);
            createMessage(projectID, (int)News.ACTION.EDIT , true);
        }

        protected void removePerson(long oeID, long personID, int jobTyp){
            long jobID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "JOB_PERS_FUNC_V", "ORGENTITY_ID=" + oeID + " and PERSON_ID=" + personID + " and JOB_TYP=" + jobTyp, false), -1L);
            _db.Job.delete(jobID, true);
        }

        public void copyProjectTeam(long sourceProjectID, long targetProjectID){
            long commiteeOrgentityID = getCommiteeOrgentityID(sourceProjectID);
            long leaderOrgentityID = getLeaderOrgentityID(sourceProjectID);
            DataTable commiteeTable = _db.getDataTable("select distinct PERSON_ID, FUNKTION_ID, JOB_TYP, ORGENTITY_ID from JOB_PERS_FUNC_V where ORGENTITY_ID in (" + commiteeOrgentityID + ", " + leaderOrgentityID + ")");
            foreach (DataRow row in commiteeTable.Rows){
                long personID = DBColumn.GetValid(row["PERSON_ID"], -1L);
                long functionID = DBColumn.GetValid(row["FUNKTION_ID"], -1L);
                int jobTyp = DBColumn.GetValid(row["JOB_TYP"], 0);
                long orgentityID = DBColumn.GetValid(row["ORGENTITY_ID"], -1L);
                if (orgentityID == commiteeOrgentityID){
                    addCommiteePerson(targetProjectID, personID, functionID);
                }
                else if (orgentityID == leaderOrgentityID){
                    if (jobTyp == Job.TYP_LEADER){
                        addLeaderPerson(targetProjectID, personID, functionID);
                    }
                    else{
                        addMemberPerson(targetProjectID, personID, functionID);
                    }
                }
            }
        }

        public bool isRootProject(long projectID)
		{
			return (ch.psoft.Util.Validate.GetValid(_db.lookup("ROOT_ID", "PROJECT", "ID=" + projectID, false), -1L)) == projectID ? true : false ;
		}

		public DataTable getProjectTypes()
		{
			return _db.getDataTable("select ID, "+_db.langAttrName("PROJECT_TYPE","TITLE")+" from PROJECT_TYPE");
		}

        public DataTable getLeaderPersons() {
            return _db.getDataTable("select distinct JOB_PERS_FUNC_V.PERSON_ID AS ID, " + _db.langAttrName("JOB_PERS_FUNC_V", "PERSON") + " from PROJECT join JOB_PERS_FUNC_V on PROJECT.LEADER_ORGENTITY_ID = JOB_PERS_FUNC_V.ORGENTITY_ID where PROJECT.PARENT_ID is null and JOB_PERS_FUNC_V.JOB_TYP=1 order by " + _db.langAttrName("JOB_PERS_FUNC_V", "PERSON"));
        }
        
        public long getRootProjectID(long projectID) {
            return ch.psoft.Util.Validate.GetValid(_db.lookup("ROOT_ID", "PROJECT", "ID=" + projectID, false), -1L);
        }


        public string getAllProjectPhases(long projectID, bool cascaded) {
            string retVal = "";

            DataTable table = _db.getDataTable("select ID from PHASE where PROJECT_ID=" + projectID);
            foreach (DataRow row in table.Rows) {
                retVal += retVal.Equals("") ? "" : ",";
                retVal += DBColumn.GetValid(row[0],0L);
            }

            if(cascaded) {
                table = _db.getDataTable("select ID from PROJECT where PARENT_ID=" + projectID);
                foreach (DataRow row in table.Rows) {
                    retVal += retVal.Equals("") ? "" : ",";
                    retVal += getAllProjectPhases(DBColumn.GetValid(row[0], -1L),cascaded);
                }
            }

            return retVal;
        }

        public bool createMessage(long projectID, int action, bool inherit)
		{
			bool retVal = false;
			_db.executeProcedure("CREATEMESSAGE",
				new ParameterCtx("ID",System.DBNull.Value),
				new ParameterCtx("USERID",_db.userId),
				new ParameterCtx("TABLENAME","PROJECT"),
				new ParameterCtx("ROWID",projectID),
				new ParameterCtx("TRIGGERNAME","PROJECT"),
				new ParameterCtx("TRIGGERID",projectID),
				new ParameterCtx("TRIGGERATTRIBUT","TITLE"),
				new ParameterCtx("TRIGGERACTION",action),
				new ParameterCtx("ACTION",action),
				new ParameterCtx("INHERIT",inherit ? 1 : 0));

			retVal = true;

			return retVal;
		}

        /// <summary>
        /// Returns list of parent project-IDs (path) corresponding to a certain project
        /// </summary>
        /// <param name="projectID">ID of a folder</param>
        /// <param name="inclProject">true: inclusive projectID (default false)</param>
        /// <returns></returns>
        public ArrayList getParentProjectIDList(long projectID, bool inclProject) {
            return _db.Tree("PROJECT").GetPath(projectID, inclProject);
        }

		public long getProjectIDbyTasklist(long tasklistID)
		{
			return ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "PROJECT", "TASKLIST_ID=" + tasklistID, false), -1L);
		}
    }
}
