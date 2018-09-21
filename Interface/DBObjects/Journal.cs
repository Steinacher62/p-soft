using ch.appl.psoft.db;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Journal.
    /// </summary>
    public class Journal : DBObject {

        public Journal(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return _db.execute("delete from JOURNAL where ID=" + ID);
        }

        public long create(string title, string journalTypeMnemo){
            long journalID = -1;
            long journalTypeID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "JOURNAL_TYPE", "MNEMO='" + journalTypeMnemo + "'", false), -1L);
            if (journalTypeID > 0){
                journalID = _db.newId("JOURNAL");
                _db.execute("insert into JOURNAL (ID, JOURNAL_TYPE_ID, " + _db.langAttrName("JOURNAL", "TITLE") + ", CREATOR_PERSON_ID) values (" + journalID + ", " + journalTypeID + ", '" + DBColumn.toSql(title) + "', " + _db.userId + ")");
            }

            return journalID;
        }

        public void assignToPerson(long journalID, long personID){
            _db.execute("insert into JOURNAL_PERSON (PERSON_ID, JOURNAL_ID) values (" + personID + ", " + journalID + ")");
        }

        public void assignToFirm(long journalID, long firmID){
            _db.execute("insert into JOURNAL_FIRM (FIRM_ID, JOURNAL_ID) values (" + firmID + ", " + journalID + ")");
        }

        public void assignToContact(long journalID, long contactID){
            long personID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "PERSON", "ID=" + contactID, false), -1L);
            if (personID > 0){
                assignToPerson(journalID, contactID);
            }
            else{
                assignToFirm(journalID, contactID);
            }
        }
    
        public string getJournalTypeIcon(long journalTypeID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("ICON", "JOURNAL_TYPE", "ID=" + journalTypeID, false), "");
        }
    }
}
 
