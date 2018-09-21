using ch.appl.psoft.db;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Subscription.
    /// </summary>
    public class Subscription : DBObject {
        public Subscription(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return _db.execute("delete from subscription where ID=" + ID);
        }

		public bool deleteSubscriptions(string triggerName, long triggerId) 
		{
			bool retVal = false;

			_db.execute("delete from SUBSCRIPTION where TRIGGERNAME = '"+ triggerName +"' and TRIGGER_ID = " + triggerId.ToString());

			return retVal;
		}

        /// <summary>
        /// Get subscription query string
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="triggerName"></param>
        /// <param name="triggerId"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public string subscriptionQuery (long userId, string triggerName, long triggerId, bool all) {
            string sql = "select * from SUBSCRIPTION where TYP = 0";

            if (userId > 0) {
                sql += " and PERSON_ID="+userId;
            }
            if (triggerId > 0) {
                sql += " and TRIGGER_ID="+triggerId;
            }
            if (triggerName != "") {
                sql += " and TRIGGERNAME='"+triggerName+"'";
            }
            if (!all) {
                sql += " and ACTIVE = 1 and ISNULL(VALID_FROM,GETDATE()) <= GETDATE() and ISNULL(VALID_TO,GETDATE()) >= GETDATE()";
            }

            return sql;
        }
        
    }
}
