using ch.appl.psoft.db;
using System;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for News.
    /// </summary>
    public class News : DBObject {
        public enum ACTION {
            DELETE = 1,
            EDIT = 2,
            NEW = 4
        }
        public enum CONTEXT {
            NEWS = 0,
            SUBSCRIPTION = 1
        }

        public News(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return _db.execute("delete from newsassign where ID=" + ID);
        }

        /// <summary>
        /// Get trigger name
        /// </summary>
        /// <param name="row">news/subscription row</param>
        /// <returns>Triggername</returns>
        public string getTriggerName(DataRow row) {
            string table = DBColumn.GetValid(row["TRIGGERNAME"],"");
            long id = DBColumn.GetValid(row["TRIGGER_ID"],0L);
            int typ = 0;

            switch (table) {
            case "NEWSPROXY":
                table = _db.lookup("TABLENAME","NEWSPROXY","ID="+id,false);
                typ = DBColumn.GetValid(_db.lookup("TYP","NEWSPROXY","ID="+id),0);
                break;
            case "FOLDER":
            case "DOCUMENT":
                typ = DBColumn.GetValid(_db.lookup("TYP",table,"ID="+id),0);
                break;
            }
            switch (table) {
            case "FOLDER":
                table = ((Document.FolderType) typ).ToString().ToUpper();
                break;
            case "DOCUMENT":
                table = ((Document.DocType) typ).ToString().ToUpper();
                break;
            default:
                break;
            }
            return table;
        }
        /// <summary>
        /// Get trigger value
        /// </summary>
        /// <param name="row">news/subscription row</param>
        /// <returns>Trigger value</returns>
        public string getTriggerValue(DataRow row) {
            string table = DBColumn.GetValid(row["TRIGGERNAME"],"");
            string attribut = DBColumn.GetValid(row["TRIGGERATTRIBUT"],"TITLE");
            long id = DBColumn.GetValid(row["TRIGGER_ID"],0L);
            string val = "";

            if (table == "NEWSPROXY") {
                object[] objs = _db.lookup(new string[] {"TABLENAME","ATTRIBUTNAME","ROWID"},"NEWSPROXY","ID="+id);
                table = DBColumn.GetValid(objs[0],"");
                attribut = DBColumn.GetValid(objs[1],"TITLE");
                long id1 = DBColumn.GetValid(objs[2],0L);
                objs = _db.lookup(new String[] {"ID",attribut},table,"id="+id1);
                if (DBColumn.IsNull(objs[0])) {
                    string langAttr = _db.langAttrName(table,attribut);
                    attribut = langAttr.Replace(attribut,"TEXT");
                    // hack
                    val = _db.lookup("P."+attribut,"NEWSPROXY P","P.ID="+id, false);
                }
                else val = DBColumn.GetValid(objs[1],"");
            }
            else val = _db.lookup(attribut,table,"id="+id, false);
            return val;
        }
        /// <summary>
        /// Get trigger id
        /// </summary>
        /// <param name="row">news/subscription row</param>
        /// <returns>Trigger Id</returns>
        public long getTriggerId(DataRow row) {
            string table = DBColumn.GetValid(row["TRIGGERNAME"],"");
            long id = DBColumn.GetValid(row["TRIGGER_ID"],0L);

            if (table == "NEWSPROXY") {
                object[] objs = _db.lookup(new string[] {"rowid","tablename"},table,"id="+id);
                id = DBColumn.GetValid(_db.lookup("id",objs[1].ToString(),"id="+objs[0]),0L);
            }
            return id;
        }
        /// <summary>
        /// Get trigger UID
        /// </summary>
        /// <param name="row">news/subscription row</param>
        /// <returns>Trigger UID</returns>
        public long getTriggerUID(DataRow row) {
            string table = DBColumn.GetValid(row["TRIGGERNAME"],"");
            long id = DBColumn.GetValid(row["TRIGGER_ID"],0L);
            long uid = 0;

            if (table == "NEWSPROXY") {
                object[] objs = _db.lookup(new string[] {"rowid","tablename"},table,"id="+id);
                id = DBColumn.GetValid(objs[0],0L);
                table = DBColumn.GetValid(objs[1],"");
            }
            uid = DBColumn.GetValid(_db.lookup("uid",table,"id="+id),0L);
            return uid;
        }
            
    }
}
