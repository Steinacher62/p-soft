using ch.appl.psoft.db;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class Dimension : DBObject {
        public const string _TABLENAME = "DIMENSION";

        public Dimension(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return 0;
        }

        public string getTitle(long ID){
            return _db.lookup("TITLE", _TABLENAME, "ID=" + ID, false);
        }

		public object[] getTitles(long ID)
		{
			string[] columns = {"TITLE","TITLE2","TITLE3"};
			return _db.lookup(columns, _TABLENAME, "ID=" + ID);			
		}
    
        public string getDescription(long ID){
            return _db.lookup("DESCRIPTION", _TABLENAME, "ID=" + ID, false);
        }

		public bool existsAttachedDetailMatrix(long ID)
		{
			return _db.exists("CHARACTERISTIC", "DIMENSION_ID = " + ID + " AND DETAIL_MATRIX_ID is not null");
		}
    }
}
