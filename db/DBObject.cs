using System.Web.SessionState;

namespace ch.appl.psoft.db
{
    /// <summary>
    /// Summary description for DBObject.
    /// </summary>
    public abstract class DBObject
	{
        protected DBData _db;
        protected HttpSessionState _session;
        protected int _release = 1;  // release dezentral
        protected int _dbRelease = 1;// release zentral
        protected string _dbModuleName = "?"; // modulename zentral

        protected DBObject() {}

        protected DBObject(DBData db, HttpSessionState session) {
            _db = db;
            _session = session;
        }

        /// <summary>
        /// Check release
        /// </summary>
        /// <param name="checkRelease">true: need check </param>
        /// <returns>true: release checked or not neeed to check</returns>
        protected bool checkRelease(bool checkRelease) {
            if (checkRelease) {
                _db.connect();
                try {
                    string[] obj = _db.lookup(new string[] {"release","remote_release"},"release","module='"+_dbModuleName+"'",false);
                    if (obj[0] != "") {
                        int r = ch.psoft.Util.Validate.GetValid(obj[0],0);
                        if (r < _dbRelease) throw new System.InvalidOperationException("Wrong Server Release: "+r);
                        r = ch.psoft.Util.Validate.GetValid(obj[1],0);
                        if (r < _release) {
                            string sql = "update release set remote_release="+_release+",remote_release_date=getdate(),remote_enable=1 where module='"+_dbModuleName+"'";
                            _db.execute(sql);
                        }
                    }
                }
                finally {
                    _db.disconnect();
                }
            }
            return true;
        }

        public abstract int delete(long ID, bool cascade);
    }
}
