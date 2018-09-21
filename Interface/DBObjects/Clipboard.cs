using ch.appl.psoft.db;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Clipboard.
    /// </summary>
    public class Clipboard  : DBObject {

        public const int TYPE_PUBLIC  = 0;
        public const int TYPE_PRIVATE = 1;

        public Clipboard(DBData db, HttpSessionState session) : base(db, session) { }
    
        /// <summary>
        /// Copies, recursively, the clipboard with all folders and documents to the target-clipboard
        /// </summary>
        /// <param name="ID">Clipboard to copy</param>
        /// <param name="cascade">Copies recursively, if true</param>
        /// <param name="accessorID">ID of the accessor, which receives all the rights (FULL_ACCESS) on the copied clipboard</param>
        /// <param name="triggerUID">UID of the SEEK-Object that should be the owner(trigger).</param>
        /// <param name="isTemplate">If true, the new clipboard will be marked as template</param>
        /// <param name="type">The type (public or private) of the new clipboard</param>
        /// <returns>The new ID of the copied clipboard</returns>
        public long copy(
			long ID,
			bool cascade,
			long accessorID,
			long triggerUID,
			bool isTemplate,
			int type,
			bool copyRootFolderRegistry
		)
		{
            long newID = 0;
            long folderId = 0;
            string colNames = "," + _db.getColumnNames("CLIPBOARD") + ",";
            string attrs = colNames;
            string sql;
            
            if (cascade) {
                folderId = DBColumn.GetValid(_db.lookup("folder_id","CLIPBOARD","id=" + ID),0L);
				if (folderId > 0)
				{
					folderId = _db.Folder.copy(
							folderId,
							0,
							triggerUID,
							true,
							copyRootFolderRegistry,
							true
						);
				}
            }
            if (folderId == 0) {
                attrs = attrs.Replace(",FOLDER_ID,", ",null,");
                newID = _db.newId("CLIPBOARD");
            }
            else {
                attrs = attrs.Replace(",FOLDER_ID,", ","+folderId+",");
                newID = folderId;
            }
            attrs = attrs.Replace(",ID,", "," + newID + ",");
            attrs = attrs.Replace(",EXTERNAL_REF,", ","); colNames = colNames.Replace(",EXTERNAL_REF,", ",");
            attrs = attrs.Replace(",UID,", ","); colNames = colNames.Replace(",UID,", ",");
            attrs = attrs.Replace(",TEMPLATE,", "," + (isTemplate? "1":"0") + ",");
            attrs = attrs.Replace(",TYP,", "," + type + ",");

            if (triggerUID > 0){
                attrs = attrs.Replace(",TRIGGER_UID,", "," + triggerUID + ",");
            }
            else{
                attrs = attrs.Replace(",TRIGGER_UID,", ",null,");
            }

            attrs = attrs.Substring(1,attrs.Length-2);
            colNames = colNames.Substring(1,colNames.Length-2);
            sql = "insert into CLIPBOARD (" + colNames + ") select " + attrs + " from CLIPBOARD where ID=" + ID;
               
            _db.execute(sql);
            
            _db.grantRowAuthorisation(DBData.AUTHORISATION.FULL_ACCESS, accessorID, "CLIPBOARD", newID);
            
            return newID;
        }
        public override int delete(long ID, bool cascade) {
            string sql = "delete from clipboard where id = "+ID;
            int rows = 0;

            if (cascade) {
                long folderId = DBColumn.GetValid(_db.lookup("folder_id","CLIPBOARD","id=" + ID),0L);
                rows = _db.execute(sql);
                if (folderId > 0) _db.Folder.delete(folderId,true);
            }
            else rows = _db.execute(sql);

            return rows;
        }
    }
}
 
