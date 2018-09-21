using ch.appl.psoft.db;
using ch.psoft.Util;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    public class Suggestion : DBObject {
        public Suggestion(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return 0;   // administered in central part
        }

        /// <summary>
        /// Comma-separated string of active Suggestion-IDs, there must be only one active suggestion. 
        /// </summary>
        /// <param name="personID">ID of a person</param>
        /// <returns>Comma-separated string of active Suggestion-IDs</returns>
        public string getActiveSuggestion()
        {
            string retValue = "-1";

            //no ispublic field in suggestion  
            DataTable table = _db.getDataTable("select ID, " + _db.langAttrName("SUGGESTION", "TITLE") + " from SUGGESTION where ISACTIVE = 1 order by " + _db.langAttrName("SUGGESTION", "TITLE"));
            if(table.Rows.Count != 1) 
            {
                 Logger.Log("more than one active suggestion found",Logger.DEBUG);
            }
            if(table.Rows.Count > 0) 
            {
                long id = DBColumn.GetValid(table.Rows[0][0], -1L);
                retValue = id.ToString();              
            }

            return retValue;
        }

        /// <summary>
        /// Checks if the survey is currently valid (VALID_FROM less than TODAY less than VALID_TO)
        /// </summary>
        /// <param name="ID">Suggestion-ID</param>
        /// <returns></returns>
        public bool isValid(long ID){
            return true; //ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)", "SUGGESTION", "(IsNull(VALID_FROM, GetDate() - 1) < GetDate()) and (IsNull(VALID_TO, GetDate() + 1) > GetDate()) and ID=" + ID, false), 0) > 0;
        }

        /// <summary>
        /// Checks if the currently logged person is eligible for SuggestionExecution of the specified survey.
        /// </summary>
        /// <param name="ID">Suggestion-ID</param>
        /// <returns>true, if the logged person is eligible for SuggestionExecution</returns>
        public bool isExecutable(long ID){
            bool ret = false;
            if (isValid(ID))
            {
                ret = (_db.hasRowAuthorisation(DBData.AUTHORISATION.EXECUTE, "SUGGESTION", ID, true, true) && isActive(ID));
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool isActive(long ID) 
        {
            bool ret = false;
            if (isValid(ID))
            {
                int isactive = _db.lookup("ISACTIVE", "SUGGESTION","ID=" + ID,0); 
                ret = (isactive == 1);
            }
            return ret;
        }

        /// <summary>
        /// Starts a new SuggestionExecution, if the logged person is eligible for SuggestionExecution of the specified survey.
        /// </summary>
        /// <param name="ID">Suggestion-ID</param>
        /// <returns>ID of the new SuggestionExecution, or -1 if no SuggestionExecution may be started.</returns>
        public long createExecution(long ID)
        {
            LanguageMapper mapper = LanguageMapper.getLanguageMapper(this._session);
            string state = mapper.get("suggestion_execution", "stateNew");
            return createExecution(ID, state);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private long createExecution(long ID, string title){
            long retValue = -1L;

            if (isExecutable(ID))
            {
                retValue = _db.SuggestionExecution.create(ID, title);
            }

            return retValue;
        }

        /// <summary>
        /// Returns the number of assigned steps
        /// </summary>
        /// <param name="ID">Suggestion-ID</param>
        /// <returns>Number of steps</returns>
        public int getNrOfSteps(long ID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)", "SUGGESTION_STEP", "SUGGESTION_ID=" + ID, false), -1);
        }

        /// <summary>
        /// Returns the index of a certain step within all the steps of the survey.
        /// </summary>
        /// <param name="ID">Suggestion-ID</param>
        /// <param name="stepID">Step-ID</param>
        /// <returns>Index (position) of step</returns>
        public int getStepIndex(long ID, long stepID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)", "SUGGESTION_STEP", "SUGGESTION_ID=" + ID + " and ORDNUMBER<=(select ORDNUMBER from SUGGESTION_STEP where ID=" + stepID + ")", false), -1);
        }

        /// <summary>
        /// Returns the step-ID of the step with a certain index.
        /// </summary>
        /// <param name="ID">Suggestion-ID</param>
        /// <param name="stepIndex">Index of the step (1 .. getNrOfSteps())</param>
        /// <returns>Step-ID</returns>
        public long getStepIDfromIndex(long ID, int stepIndex){
            long retValue = -1L;

            DataTable table = _db.getDataTable("select top " + stepIndex + " ID from SUGGESTION_STEP where SUGGESTION_ID=" + ID + " order by ORDNUMBER asc");
            if (table.Rows.Count == stepIndex){
                retValue = DBColumn.GetValid(table.Rows[stepIndex-1][0], -1L);
            }

            return retValue;
        }
    }
}
