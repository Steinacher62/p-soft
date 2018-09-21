using ch.appl.psoft.db;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class Organisation : DBObject {
        public const string TableName = "ORGANISATION";

        public Organisation(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long id, bool cascade) {
            int retValue = 0;

            // deleting dependent employments...
            DataTable table = _db.getDataTable("select ID from EMPLOYMENT where ORGANISATION_ID=" + id);
            foreach (DataRow row in table.Rows) {
                retValue += _db.Employment.delete(DBColumn.GetValid(row[0],0L), true);
            }
            if (_db.dbObjectExists("VARIANTE",'U')) {
                // deleting dependent varianten...
                table = _db.getDataTable("select ID from VARIANTE where ORGANISATION_ID = " + id);
                foreach (DataRow row in table.Rows) {
                    retValue += _db.Lohn.deleteVariante(DBColumn.GetValid(row[0], -1L),true);
                }
            }

            // deleting orgentities...
            _db.Orgentity.delete(getRootOrgentityID(id), true);

            _db.execute("delete from " + TableName + " where ID=" + id);

            return retValue;
        }

        public long getRootOrgentityID(long id){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("ORGENTITY_ID", TableName, "ID=" + id, false), -1L);
        }
    }
}
