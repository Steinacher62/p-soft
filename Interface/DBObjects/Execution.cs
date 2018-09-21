using ch.appl.psoft.db;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    public class Execution : DBObject {
        public Execution(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return _db.execute("delete from EXECUTION where ID=" + ID);
        }

        /// <summary>
        /// Create a new execution.
        /// </summary>
        /// <param name="surveyID">Survey-ID</param>
        /// <returns>ID of the new execution, or -1 if no execution may be started.</returns>
        public long create(long surveyID){
            long newID = _db.newId("EXECUTION");

            bool isAnonymous = ch.psoft.Util.Validate.GetValid(_db.lookup("ISANONYMOUS", "SURVEY", "ID=" + surveyID, false), -1) > 0;

            _db.execute("insert into EXECUTION (ID, SURVEY_ID, PERSON_ID) values (" + newID + "," + surveyID + "," + (isAnonymous? "null" : SessionData.getUserID(_session).ToString()) + ")");
            
            return newID;
        }

        /// <summary>
        /// Returns the ID of the currently executing step
        /// </summary>
        /// <param name="ID">Execution-ID</param>
        /// <returns>ID of the current step</returns>
        public long getCurrentStepID(long ID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("CURRENT_STEP_ID", "EXECUTION", "ID=" + ID, false), -1L);
        }

        /// <summary>
        /// Sets the current step-ID of an execution
        /// </summary>
        /// <param name="ID">Execution-ID</param>
        /// <param name="stepID">New current step-ID</param>
        public void setCurrentStepID(long ID, long stepID){
            _db.execute("update EXECUTION set CURRENT_STEP_ID=" + (stepID > 0L? stepID.ToString() : "null") + " where ID=" + ID);
        }

        /// <summary>
        /// Sets the finished-flag
        /// </summary>
        /// <param name="ID">Execution-ID</param>
        /// <param name="isFinished">Set to true, if finished</param>
        public void setFinished(long ID, bool isFinished){
            _db.execute("update EXECUTION set ISFINISHED=" + (isFinished? "1" : "0") + " where ID=" + ID);
        }

        
    }
}
