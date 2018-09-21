using ch.appl.psoft.db;
using ch.psoft.db;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    public class ProjectBilling : DBObject
    {
        public const string TABLE_NAME = "PROJECT_BILLING";

        public ProjectBilling(DBData db, HttpSessionState session) : base(db, session) { }

  
        override public int delete(long id, bool cascade)
        {
            //cascade argument ignored
            if (_db.lookup("ID", TABLE_NAME, "ID = " + id) == null)
            {
                return 0; // no rows deleted
            }
            long pid = _db.lookup("PROJECT_ID", TABLE_NAME, "ID = " + id, -1L);

            _db.execute("delete from " + TABLE_NAME + " where ID = " + id);

            //update project
            if (pid != -1)
            {
                //for every deleted bill entry the project costs are updated
                updateExternalProjectCosts(pid);
            }

            return 1;
        }

  
        /// <summary>
        /// not implemented: check if the value has been "manually" changed by the user.
        /// </summary>
        /// <param name="projectId"></param>
        public void updateExternalProjectCosts(long projectId)
        {
            //retrieve current billing saldo
            double totCredit   = _db.ProjectBilling.sigma(projectId, "CREDIT_VALUE");
            double totCreditor = _db.ProjectBilling.sigma(projectId, "CREDITOR_VALUE");
            double totDebitor  = _db.ProjectBilling.sigma(projectId, "DEBITOR_VALUE");
            //insert new values

            //soll Kosten
            string soll = _db.dbColumn.numberToSql(SQLColumn.InputDataType.Double, totCredit);
            _db.execute("update PROJECT set COST_EXTERNAL_NOMINAL=" + soll + " where ID=" + projectId);

            //ist Kosten
            string ist = _db.dbColumn.numberToSql(SQLColumn.InputDataType.Double, totCreditor - totDebitor);
            _db.execute("update PROJECT set COST_EXTERNAL_ACTUAL=" + ist + " where ID=" + projectId);
            
        }



        /// <summary>
        /// TODO: kann man grantRowAuthorisation mehrmals aufrufen?
        /// </summary>
        /// <param name="id"></param>
        /// <param name="leadersId"></param>
        public void setDefaultRights(long id, long[] leadersId)
        {
            if (id < 1)
                return;

            // set rights for leader
            foreach (long leaderId in leadersId)
            {
                if (leaderId > 0)
                {
                    long leaderAccessorID = _db.getAccessorID(leaderId);
                    _db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, leaderAccessorID, TABLE_NAME, id);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public double sigma(long projectId, string column)
        {
      
                double ret = 0;
                string sql = "select " + column + " from project_billing where project_id = " + projectId;
                DataTable data = _db.getDataTable(sql);
                if (data.Rows.Count == 0)
                {
                    return ret;
                }
                foreach (DataRow elm in data.Rows)
                {
                    if (!DBColumn.IsNull(elm[0]))
                        ret += (double)elm[0];
                }
                return ret;
        }

    }
}
