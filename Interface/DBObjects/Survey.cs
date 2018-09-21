using ch.appl.psoft.db;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    public class Survey : DBObject {
        public Survey(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return 0;   // administered in central part
        }

        /// <summary>
        /// Returns comma-separated string of all Survey-IDs which the specified person may access.
        /// </summary>
        /// <param name="personID">ID of a person</param>
        /// <returns>Comma-separated string of Survey-IDs</returns>
        public string getAccessableSurveys(long personID){
            string retValue = "";
            bool isFirst = true;

            DataTable table = _db.getDataTable("select ID, " + _db.langAttrName("SURVEY", "TITLE") + " from SURVEY where ISPUBLIC=1 order by " + _db.langAttrName("SURVEY", "TITLE"));
            foreach (DataRow row in table.Rows){
                long surveyID = DBColumn.GetValid(row[0], -1L);
                if (_db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "SURVEY", surveyID, true, true)){
                    if (isFirst){
                        isFirst = false;
                    }
                    else{
                        retValue += ",";
                    }
                    retValue += surveyID;
                }            
            }

            return retValue;
        }

        /// <summary>
        /// Checks if the survey is currently valid (VALID_FROM less than TODAY less than VALID_TO)
        /// </summary>
        /// <param name="ID">Survey-ID</param>
        /// <returns></returns>
        public bool isValid(long ID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)", "SURVEY", "(IsNull(VALID_FROM, GetDate() - 1) < GetDate()) and (IsNull(VALID_TO, GetDate() + 1) > GetDate()) and ID=" + ID, false), 0) > 0;
        }

        /// <summary>
        /// Checks if the currently logged person is eligible for execution of the specified survey.
        /// </summary>
        /// <param name="ID">Survey-ID</param>
        /// <returns>true, if the logged person is eligible for execution</returns>
        public bool isExecutable(long ID){
            bool retValue = false;

            if (isValid(ID)){
                if (_db.hasRowAuthorisation(DBData.AUTHORISATION.EXECUTE, "SURVEY", ID, true, true)){
                    int maxExecutions = ch.psoft.Util.Validate.GetValid(_db.lookup("MAXEXECUTIONS", "SURVEY", "ISANONYMOUS=0 and ID=" + ID, false), 0);

                    if (maxExecutions == 0){
                        retValue = true;
                    }
                    else{
                        int nrOfExecutions = ch.psoft.Util.Validate.GetValid(_db.lookup("COUNT(ID)", "EXECUTION", "SURVEY_ID=" + ID + " and PERSON_ID=" + SessionData.getUserID(_session), false), 0);
                        retValue = maxExecutions > nrOfExecutions;
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// Starts a new execution, if the logged person is eligible for execution of the specified survey.
        /// </summary>
        /// <param name="ID">Survey-ID</param>
        /// <returns>ID of the new execution, or -1 if no execution may be started.</returns>
        public long createExecution(long ID){
            long retValue = -1L;

            if (isExecutable(ID)){
                retValue = _db.Execution.create(ID);
            }

            return retValue;
        }

        /// <summary>
        /// Returns the number of assigned steps
        /// </summary>
        /// <param name="ID">Survey-ID</param>
        /// <returns>Number of steps</returns>
        public int getNrOfSteps(long ID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)", "STEP", "SURVEY_ID=" + ID, false), -1);
        }

        /// <summary>
        /// Returns the index of a certain step within all the steps of the survey.
        /// </summary>
        /// <param name="ID">Survey-ID</param>
        /// <param name="stepID">Step-ID</param>
        /// <returns>Index (position) of step</returns>
        public int getStepIndex(long ID, long stepID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)", "STEP", "SURVEY_ID=" + ID + " and ORDNUMBER<=(select ORDNUMBER from STEP where ID=" + stepID + ")", false), -1);
        }

        /// <summary>
        /// Returns the step-ID of the step with a certain index.
        /// </summary>
        /// <param name="ID">Survey-ID</param>
        /// <param name="stepIndex">Index of the step (1 .. getNrOfSteps())</param>
        /// <returns>Step-ID</returns>
        public long getStepIDfromIndex(long ID, int stepIndex){
            long retValue = -1L;

            DataTable table = _db.getDataTable("select top " + stepIndex + " ID from STEP where SURVEY_ID=" + ID + " order by ORDNUMBER asc");
            if (table.Rows.Count == stepIndex){
                retValue = DBColumn.GetValid(table.Rows[stepIndex-1][0], -1L);
            }

            return retValue;
        }
    }
}
