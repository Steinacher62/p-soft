using ch.appl.psoft.db;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    public class SuggestionExecution : DBObject {
        public SuggestionExecution(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return _db.execute("delete from SUGGESTION_EXECUTION where ID=" + ID);
        }

        /// <summary>
        /// Create a new execution.
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>ID of the new execution, or -1 if no execution may be started.</returns>
        public long create(long id, string title)
        {
            long newID = _db.newId("SUGGESTION_EXECUTION");
            _db.execute("insert into SUGGESTION_EXECUTION (ID, SUGGESTION_ID, PERSON_ID, TITLE) values (" + newID + "," + id + "," + SessionData.getUserID(_session).ToString() + ", '" + title + "')");
            return newID;
        }

        /// <summary>
        /// Returns the ID of the currently executing step
        /// </summary>
        /// <param name="ID">SuggestionExecution-ID</param>
        /// <returns>ID of the current step</returns>
        public long getCurrentStepID(long ID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("CURRENT_SUGGESTION_STEP_ID", "SUGGESTION_EXECUTION", "ID=" + ID, false), -1L);
        }

        /// <summary>
        /// Sets the current step-ID of an execution
        /// </summary>
        /// <param name="ID">SuggestionExecution-ID</param>
        /// <param name="stepID">New current step-ID</param>
        public void setCurrentStepID(long ID, long stepID){
            _db.execute("update SUGGESTION_EXECUTION set CURRENT_SUGGESTION_STEP_ID=" + (stepID > 0L? stepID.ToString() : "null") + " where ID=" + ID);
        }

        /// <summary>
        /// Sets the finished-flag
        /// </summary>
        /// <param name="ID">SuggestionExecution-ID</param>
        /// <param name="isFinished">Set to true, if finished</param>
        public void setFinished(long ID, bool isFinished, string title, bool stateOn){
            LanguageMapper mapper = LanguageMapper.getLanguageMapper(this._session);
            if(stateOn) 
            {
                string state = (isFinished? mapper.get("suggestion_execution", "stateComplete"): mapper.get("suggestion_execution", "stateIncomplete"));
                title = title + "(" + state + ")";
            }

            _db.execute("update SUGGESTION_EXECUTION set ISFINISHED=" + (isFinished? "1" : "0") + ",TITLE = '" + title + "' where ID=" + ID);
        }

        /// <summary>
        /// If finisched returns true, also returns true if the report has already been sent
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool isFinished(long ID) 
        {
            int res = _db.lookup("ISFINISHED","SUGGESTION_EXECUTION","ID = " + ID, 0);
            return (res > 0);
        }

        /// <summary>
        /// Sets the sent-flag
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="isSent"></param>
        public void setSent(long ID, bool isSent, string title, bool stateOn)
        { 
            LanguageMapper mapper = LanguageMapper.getLanguageMapper(this._session);
            if(stateOn) 
            {
                string state = (isSent? mapper.get("suggestion_execution", "stateSent"): mapper.get("suggestion_execution", "stateComplete"));
                title = title + "(" + state + ")";
            }

            _db.execute("update SUGGESTION_EXECUTION set ISFINISHED=" + (isSent? "2" : "1") + ",TITLE= '" + title + "' where ID=" + ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool isSent(long ID) 
        {
            int res = _db.lookup("ISFINISHED","SUGGESTION_EXECUTION","ID = " + ID, 0);
            return (res == 2);

        }
    }
}
