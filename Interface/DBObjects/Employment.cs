using ch.appl.psoft.db;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class Employment : DBObject {
        public const string TableName = "EMPLOYMENT";

        public Employment(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long id, bool cascade) {
            int retValue = 0;

            _db.execute("update JOB set EMPLOYMENT_ID=null where EMPLOYMENT_ID=" + id);
            if (_db.dbObjectExists("PERFORMANCERATING",'U')) {
                // deleting dependent performanceratings...
                DataTable table = _db.getDataTable("select ID from PERFORMANCERATING where EMPLOYMENT_REF = " + id);
                foreach (DataRow row in table.Rows) {
                    retValue += _db.Performance.delete(DBColumn.GetValid(row[0], -1L),true);
                }
            }
            retValue += _db.execute("delete from " + TableName + " where ID=" + id);

            return retValue;
        }


        /// <summary>
        /// REturn the name of the department name if any. DEpartment is represented
        /// by the table ORGENTITY
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public string getDepartmentName(long employmentId)
        {
            try
            {
                long orgentId = _db.lookup("ORGENTITY_ID", "EMPLOYMENT", "ID = " + employmentId, -1L);
                if (orgentId == -1)
                {
                    return "";
                }
                string depName = _db.lookup(_db.langAttrName("ORGENTITY", "TITLE"), "ORGENTITY", "ID = " + orgentId, "");
                return depName;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employmentId"></param>
        /// <returns></returns>
        public string getPersonName(long employmentId)
        {
            try
            {
                long personId = _db.lookup("PERSON_ID", "EMPLOYMENT", "ID = " + employmentId, -1L);
                if (personId == -1)
                {
                    return "";
                }
                return (_db.lookup("PNAME", "PERSON", "ID = " + personId, "") + " " + _db.lookup("FIRSTNAME", "PERSON", "ID = " + personId, ""));
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="employmentId"></param>
        /// <returns></returns>
        public long getPersonId(long employmentId)
        {
            try
            {
                return _db.lookup("PERSON_ID", "EMPLOYMENT", "ID = " + employmentId, -1L);
            }
            catch 
            {
                return -1;
            }
        }

        
    }
}
