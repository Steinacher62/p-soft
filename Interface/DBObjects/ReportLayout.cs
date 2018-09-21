using ch.appl.psoft.db;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class ReportLayout : DBObject {
        public const string _TABLENAME = "REPORTLAYOUT";

        public ReportLayout(DBData db, HttpSessionState session) : base(db, session) { }

        public long getIDbyTitleMnemo(string titleMnemo){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("ID", _TABLENAME, "TITLE_MNEMO='" + titleMnemo + "'", false), -1L);
        }

        public override int delete(long ID, bool cascade) {
            return 0;
        }

    }
}
