using ch.appl.psoft.db;
using System.Data;
using System.Drawing;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class Characteristic : DBObject {
        public const string _TABLENAME = "CHARACTERISTIC";

        public Characteristic(DBData db, HttpSessionState session) : base(db, session) { }

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

        public long getKnowledgeID(long ID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup(Knowledge._TABLENAME + "_ID", _TABLENAME, "ID=" + ID, false), -1L);
        }

        public long getDetailMatrixID(long ID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("DETAIL_" + Matrix._TABLENAME + "_ID", _TABLENAME, "ID=" + ID, false), -1L);
        }
        
        public Color getColor(long ID)
        {
            Color returnValue = Color.White;
            DataTable table = _db.getDataTable(
                    "select C.COLOR"
                    + " from COLORATION C"
                    + " where C.ID = " + ID
                );
            
            if (table.Rows.Count > 0)
            {
                returnValue = Color.FromArgb((int)table.Rows[0][0]);
            }

            return returnValue;
        }


		public string getColorationTitle(long ID)
		{
			return _db.lookup("TITLE", "COLORATION", "ID=" + ID, false);
		}

		public Color getSlaveColor(long slaveId, long charId)
		{
			Color returnValue = Color.White;
			
			long slaveCharacteristicID = DBColumn.GetValid(_db.lookup("ID","SLAVE_CHARACTERISTIC"," SLAVE_ID = " + slaveId +" and CHARACTERISTIC_ID = " +charId),-1L);
			long colorationID = DBColumn.GetValid(_db.lookup("COLORATION_ID","COLORATION_HISTORY"," SLAVE_CHARACTERISTIC_ID = " + slaveCharacteristicID + " ORDER BY GUELTIG_AB DESC"),-1L);
			DataTable table = _db.getDataTable(
				"select C.COLOR"
				+ " from COLORATION C"
				+ " where C.ID = " + colorationID
				);
            
			if (table.Rows.Count > 0)
			{
				returnValue = Color.FromArgb((int)table.Rows[0][0]);
			}

			return returnValue;
		}





    }
}
