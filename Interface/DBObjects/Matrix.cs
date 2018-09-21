using ch.appl.psoft.db;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class Matrix : DBObject {
        public const string _TABLENAME = "MATRIX";
		public const string _TABLENAME_SLAVE = "SLAVE";
 

        public Matrix(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return 0;
        }

        public string getTitle(long ID){
            return _db.lookup("TITLE", _TABLENAME, "ID=" + ID, false);
        }

		public string getSlaveTitle(long ID)
		{
			return _db.lookup("TITLE", _TABLENAME_SLAVE, "ID=" + ID, false);
		}
    
        public string getDescription(long ID){
            return _db.lookup("DESCRIPTION", _TABLENAME, "ID=" + ID, false);
        }

		public string getSlaveDescription(long ID)
		{
			return _db.lookup("DESCRIPTION", _TABLENAME_SLAVE, "ID=" + ID, false);
		}
		
    }
}
