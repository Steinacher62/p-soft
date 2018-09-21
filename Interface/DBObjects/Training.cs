using ch.appl.psoft.db;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class Training : DBObject
	{
        /// <summary>
        /// Holds a cache-entry of the person-IDs the current user has the training right
        /// </summary>
        protected class TrainingPersonIDsCacheEntry : DBCacheEntry{
            private string _personIDs = "";
            private bool _getJustFirst = false;

            public TrainingPersonIDsCacheEntry(bool getJustFirst) : base(600){ // the cache-entry remains fresh for 10 minutes.
                _getJustFirst = getJustFirst;
            }

            protected override void onRefreshing(DBData db){
                _personIDs = "";
                string sql = "select distinct EMPLOYMENT.PERSON_ID from JOB inner join ORGENTITY on JOB.ORGENTITY_ID=ORGENTITY.ID inner join ORGANISATION on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID and ORGANISATION.MAINORGANISATION=1 inner join EMPLOYMENT on JOB.EMPLOYMENT_ID=EMPLOYMENT.ID";
                if (!db.hasTableAuthorisation(DBData.AUTHORISATION.READ, "JOB", DBData.APPLICATION_RIGHT.TRAINING, true)){
                    sql += db.getAccessRightsRowInnerJoinSQL("JOB", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.TRAINING, true, true);
                }
                DataTable allPersonTable = db.getDataTable(sql);
                bool isFirstPerson = true;
                foreach (DataRow row in allPersonTable.Rows){
                    string personID = DBColumn.GetValid(row[0], "");
                    if (personID.Length > 0){
                        if (isFirstPerson){
                            isFirstPerson = false;
                        }
                        else{
                            _personIDs += ",";
                        }
                        _personIDs += personID;
                    }
                    if (_getJustFirst){
                        break;
                    }
                }
            }

            public string getTrainingPersonIDs(DBData db){
                lock (this){
                    refreshIfStale(db);
                    return _personIDs;
                }
            }
        }

        public Training(DBData db, HttpSessionState session) : base(db, session) { }

        public override int delete(long ID, bool cascade) {
            return 0;
        }

        private TrainingPersonIDsCacheEntry getTrainingPersonIDsCacheEntry(string baseKey, bool getJustFirst){
            TrainingPersonIDsCacheEntry cacheEntry = null;
            lock (_db.CacheSyncRoot){
                string key = baseKey + _db.userId;
                cacheEntry = (TrainingPersonIDsCacheEntry) _db.getCacheEntry(key);
                if (cacheEntry == null){
                    cacheEntry = new TrainingPersonIDsCacheEntry(getJustFirst);
                    _db.addCacheEntry(key, cacheEntry);
                }
            }
            return cacheEntry;
        }

        /// <summary>
        /// Liefert eine kommaseparierte Liste aller Personen-IDs,
        /// für welche die eingeloggte Person Ausbildungsmassnahmen erfassen kann
        /// </summary>
        /// <returns></returns>
        public string getTrainingPersonIDs(){
            return getTrainingPersonIDsCacheEntry("TrainingPersonIDs_", false).getTrainingPersonIDs(_db);
        }

        /// <summary>
        /// true, wenn die eingeloggte Person Ausbildungsmassnahmen auf mindestens einer Person erfassen kann.
        /// </summary>
        /// <returns></returns>
        public bool hasTrainingPersons(){
            return getTrainingPersonIDsCacheEntry("HasTrainingPersons_", true).getTrainingPersonIDs(_db).Length > 0;
        }

        public void refreshCacheEntriesAsynchronous(){
            getTrainingPersonIDsCacheEntry("HasTrainingPersons_", true).refreshAsynchronous(DBData.getDBData(_db.session));
            getTrainingPersonIDsCacheEntry("TrainingPersonIDs_", false).refreshAsynchronous(DBData.getDBData(_db.session));
        }

        public long getTrainingID(long advancementID) {
            long retValue = 0;
            if (advancementID > 0) {
                retValue = ch.psoft.Util.Validate.GetValid(_db.lookup("TRAINING_ID", "TRAINING_ADVANCEMENT", "ID=" + advancementID, false),0);
            }
            return retValue;
        }

        public long getPersonID(long advancementID) {
            long retValue = 0;
            if (advancementID > 0) {
                retValue = ch.psoft.Util.Validate.GetValid(_db.lookup("PERSON_ID", "TRAINING_ADVANCEMENT", "ID=" + advancementID, false),0);
            }
            return retValue;
        }
    }
}