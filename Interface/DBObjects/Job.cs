using ch.appl.psoft.db;
using System;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class Job : DBObject {
        public const int TYP_EMPLOYEE = 0;
        public const int TYP_LEADER = 1;
        public const string TableName = "JOB";

        public Job(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long id, bool cascade) {
            int retValue = 0;
            if (_db.dbObjectExists("OBJECTIVE",'U')) {
                // deleting dependent objectives...
                DataTable table = _db.getDataTable("select ID from OBJECTIVE where JOB_ID=" + id);
                foreach (DataRow row in table.Rows) {
                    retValue += _db.Objective.delete(DBColumn.GetValid(row[0], -1L), cascade);
                }
            }
            if (_db.dbObjectExists("PERFORMANCERATING_ITEMS",'U')) {
                // deleting dependent performancerating items...
                DataTable table = _db.getDataTable("select P.ID from PERFORMANCERATING_ITEMS P inner join JOB_EXPECTATION E on P.EXPECTATION_REF = E.ID where E.JOB_REF = " + id);
                foreach (DataRow row in table.Rows) {
                    retValue += _db.Performance.deleteItem(DBColumn.GetValid(row[0], -1L));
                }
            }

            _db.execute("delete from " + TableName + " where ID=" + id);

            return retValue;
        }

        public long add(long oeID, long personID, long functionID, int jobTyp){
            long jobID = -1L;
            if (oeID>0 && personID>0 && functionID>0){
                jobID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "JOBPERSONV", "ORGENTITY_ID=" + oeID + " and PERSON_ID=" + personID, false), -1L);
                if (jobID == -1L){
                    long organisationID = ch.psoft.Util.Validate.GetValid(_db.lookup("ORGANISATION.ID", "ORGANISATION inner join ORGENTITY on ORGANISATION.ORGENTITY_ID = ORGENTITY.ROOT_ID", "ORGENTITY.ID=" + oeID, false), -1L);
                    long employmentID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "EMPLOYMENT", "ORGANISATION_ID=" + organisationID + " and PERSON_ID=" + personID, false), -1L);
                    if (employmentID == -1L){
                        employmentID = _db.newId("EMPLOYMENT");
                        _db.execute("insert into EMPLOYMENT (ID, PERSON_ID, ORGANISATION_ID) values (" + employmentID + ", " + personID + ", " + organisationID + ")");
                    }
                    jobID = _db.newId(TableName);
                    _db.execute("insert into JOB (ID, FUNKTION_ID, ORGENTITY_ID, EMPLOYMENT_ID, ENGAGEMENT, TYP) values (" + jobID + ", " + functionID + ", " + oeID + ", " + employmentID + ", 100, " + jobTyp + ")");
                }
            }
            return jobID;
        }

		public String getTitle(long jobId)
		{
			return _db.lookup(_db.langAttrName("JOB","TITLE"),"JOB","ID = " + jobId,"");
		}

		public String getFunctionTitle(long jobId)
		{
			long function_id = _db.lookup("FUNKTION_ID","JOB","ID = " + jobId,-1L);
			return _db.lookup(_db.langAttrName("FUNKTION","TITLE"),"FUNKTION","ID = " + function_id,"");
		}


        /// <summary>
        /// REturn the name of the department name if any. DEpartment is represented
        /// by the table ORGENTITY
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public String getDepartmentName(long jobId)
        {
            long orgentId = _db.lookup("ORGENTITY_ID", "JOB", "ID = " + jobId, -1L);
            if (orgentId == -1)
            {
                return "";
            }
            string depName = _db.lookup(_db.langAttrName("ORGENTITY", "TITLE"), "ORGENTITY", "ID = " + orgentId, "");
            return depName;
        }

    }
}
