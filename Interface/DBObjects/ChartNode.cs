using ch.appl.psoft.db;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class ChartNode : DBObject {
        public const string TableName = "CHARTNODE";

        public ChartNode(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long id, bool cascade) {
            int retValue = 0;

            _db.execute("delete from CHARTTEXT where CHARTNODE_ID=" + id);
            _db.execute("delete from " + TableName + " where ID=" + id);

            return retValue;
        }
    }
}
