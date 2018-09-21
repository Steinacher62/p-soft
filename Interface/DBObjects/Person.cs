using ch.appl.psoft.db;
using ch.psoft.Util;
using System.Data;
using System.IO;
using System.Web.Security;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Provides methods for recurring tasks on persons.
    /// Encapsulates application-logic and helper methods on persons.
    /// </summary>
    public class Person : DBObject {
        /// <summary>
        /// Bitmap constants to define the person-type
        /// </summary>
        public class TYP {
            public const int UNDEFINED     = 0;
            public const int EMPLOYEE      = 1;
            public const int INTERNAL      = 2;
            public const int ADMINISTRATOR = 4;
            public const int CONTACT       = 8;
        }

        public const string ONLY_ACTIVE_SQL_RESTRICTION = "IsNull(LEAVING, GetDate()+1)>GetDate()";
        public const string INACTIVE_PERSON_MARKER_SQL = "case when " + ONLY_ACTIVE_SQL_RESTRICTION + " then '' else '*' end";

        public Person(DBData db, HttpSessionState session) : base(db, session) { }

        /// <summary>
        /// Check User Login
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="pwd">User Password. If null is passed, the password will not be checked (for authentication-mode windows)</param>
        /// <returns>number of users found in database</returns>
        public int login(string userName, string pwd) {
            SessionData.setUserID(_session, -1);
            SessionData.setUserAccessorID(_session, -1);

            DataTable personDataTable;
            int rowCount = 0;

            if (pwd == null)
                pwd = "";
            else if (pwd == "")
                pwd = " and PASSWORD is null";
            else
                pwd = " and PASSWORD ='" + pwd + "'";

            string sqlStatement = "SELECT * FROM PERSON WHERE LOGIN='" + userName + "'" + pwd + " and " + ONLY_ACTIVE_SQL_RESTRICTION;

            personDataTable = _db.getDataTable(sqlStatement, "PERSON");

            rowCount = personDataTable.Rows.Count;

            //assign out parameters 
            if(rowCount == 1) {
                long personID = ch.psoft.Util.Validate.GetValid(DBData.getValue(personDataTable, 0, "ID").ToString(),0);
                long userAccessorID = _db.getAccessorID(personID);
                SessionData.setUserID(_session, personID);
                _session["UserInitials"] = userName;
                SessionData.setUserAccessorID(_session, userAccessorID);
                SessionData.setRowsPerListPage(_session, ch.psoft.Util.Validate.GetValid(Global.Config.getStringProperty("User","RowsPerListPage",personID), ch.psoft.Util.Validate.GetValid(Global.Config.rowsPerListPage, 15)));
                SessionData.setMaxTreeCaptionLength(_session, ch.psoft.Util.Validate.GetValid(Global.Config.getStringProperty("User","MaxTreeCaptionLength",personID), ch.psoft.Util.Validate.GetValid(Global.Config.maxTreeCaptionLength, 30)));
                SessionData.setRegistryRootID(_session, ch.psoft.Util.Validate.GetValid(Global.Config.getStringProperty("System", "REGISTRY_ROOT", 0), -1));

                string languageCode = getLanguageCode(personID);
                string regionCode = getRegionCode(personID);
                if (languageCode != "" && regionCode != "") {
                    // reload LanguageMapper
                    Global.reloadLanguageMapper(LanguageMapper.getLanguageMapper(_session), languageCode);

                    // reload DBColumn
                    SessionData.getDBColumn(_session).UserCultureName = languageCode+"-"+regionCode;
                }

                FormsAuthentication.SetAuthCookie(userName, false);
                Logger.Log("User '" + userName + "' logged in.", Logger.MESSAGE);

                //let's notify the modules about the successful login...
                foreach (psoftModule module in Global.s_ModulesDict.Values) {
                    module.OnAfterLogin(_session);
                }
            }
            return rowCount;
        }

        public int windowsLogin(string username){
            return login(username, null);
        }

        public static string getLanguageCode(long personID){
            return Global.Config.getStringProperty("User", "LanguageCode", personID);
        }

        public static string getRegionCode(long personID){
            return Global.Config.getStringProperty("User", "RegionCode", personID);
        }

        public static void setLanguageCode(long personID, string languageCode){
            Global.Config.setProperty("User", "LanguageCode", personID, languageCode);
        }

        public static void setRegionCode(long personID, string regionCode){
            Global.Config.setProperty("User", "RegionCode", personID, regionCode);
        }

        public string getWholeName(long personID) {
            return getWholeName(personID.ToString());
        }

        public string getWholeName(string personID) {
            return getWholeName(personID, false);
        }

        public string getWholeName(string personID, bool infoFirst) {
            return getWholeName(personID, infoFirst, true, false);
        }

        public string getWholeName(string personID, bool infoFirst, bool showInitials, bool showPersonnelNumber) {
            string retValue = "";

            if (personID != "") {
                string sql = "select FIRSTNAME, PNAME, MNEMO, PERSONNELNUMBER, MARKER=" + INACTIVE_PERSON_MARKER_SQL + " from PERSON where ID=" + personID;
                DataTable personTable = _db.getDataTable(sql, "PERSON");
                if (personTable.Rows.Count > 0) {
                    DataRow row = personTable.Rows[0];
                    retValue = _db.GetDisplayValue(personTable.Columns["PNAME"], row["PNAME"], false) + " " + _db.GetDisplayValue(personTable.Columns["FIRSTNAME"], row["FIRSTNAME"], false) + DBColumn.GetValid(row["MARKER"], "");

                    if (showInitials || showPersonnelNumber) {
                        string initials = _db.GetDisplayValue(personTable.Columns["MNEMO"], row["MNEMO"], false);
                        string personnelNumber = _db.GetDisplayValue(personTable.Columns["PERSONNELNUMBER"], row["PERSONNELNUMBER"], false);

                        string info = "(";
                        bool filled = false;
                        if (showInitials && initials.Length>0) {
                            info += initials;
                            filled = true;
                        }
                        if (showPersonnelNumber && personnelNumber.Length>0) {
                            if (filled)
                                info += "/";
                            info += personnelNumber;
                        }
                        if (info != "(") {
                            info += ")";
                            if (infoFirst)
                                retValue = info + " " + retValue;
                            else
                                retValue += " " + info;
                        }
                    }
                }
            }
            return retValue;
        }

        public bool isInternal(long personID) {
            int type = ch.psoft.Util.Validate.GetValid(_db.lookup("TYP", "PERSON", "ID=" + personID, false), 0);
            return (type & TYP.INTERNAL) > 0;
        }

        public DataTable getWholeNameMATable(bool includeInactivePersons){
            return getWholeNameMATable(false, true, false, includeInactivePersons);
        }

        public static string getWholeNameSQL(bool infoFirst, bool showInitials, bool showPersonnelNumber){
            string infoSql = "'('";
            bool showInfo = false;
            if (showInitials){
                infoSql += " + RTRIM(isnull(PERSON.MNEMO,''))";
                showInfo = true;
            }
            if (showPersonnelNumber){
                if (showInitials){
                    infoSql += " + '/'";
                }
                infoSql += " + isnull(PERSON.PERSONNELNUMBER,'')";
                showInfo = true;
            }
            infoSql += " + ')'";
            return (showInfo && infoFirst? infoSql + " + ' ' + " : "") + "PERSON.PNAME + ' ' + isnull(PERSON.FIRSTNAME,'') + " + INACTIVE_PERSON_MARKER_SQL + (showInfo && !infoFirst? " + ' ' + " + infoSql : "");
        }

        /// <summary>
        /// Get DataTable of Persons with the whole name, initials and personnell-number
        /// </summary>
        /// <param name="infoFirst">if true, the additional infos are shown in front of the name.</param>
        /// <param name="showInitials">if true, initials are shown.</param>
        /// <param name="showPersonnelNumber">if true, the personnel-number is shown.</param>
        /// <param name="includeInactivePersons">if true, also inactive persons are included.</param>
        /// <returns></returns>
        public DataTable getWholeNameMATable(bool infoFirst, bool showInitials, bool showPersonnelNumber, bool includeInactivePersons) {
            string personSql = "select ID, " + getWholeNameSQL(infoFirst, showInitials, showPersonnelNumber) + "AS fullname from PERSON where (TYP & 3)=1" + (includeInactivePersons? "": " and " + ONLY_ACTIVE_SQL_RESTRICTION) + " order by PNAME, FIRSTNAME";
            return _db.getDataTable(personSql);
        }

        public override int delete(long ID, bool cascade) {
            return _db.execute("delete from PERSON where ID=" + ID);
        }

        public long getClipboardID(long ID) {
            return ch.psoft.Util.Validate.GetValid(_db.lookup("CLIPBOARD_ID", "PERSON", "ID=" + ID, false),-1);
        }

        /// <summary>
        /// Returns comma-separated list of all OrgEntity IDs which the person is leading
        /// </summary>
        /// <param name="personID">ID of person</param>
        /// <returns>Comma-separated list of IDs</returns>
        public string getLeadingOEIDs(long personID) {
            DataTable oeTable = _db.getDataTable("select OE_ID from PERSONOEV where ID=" + _db.userId + " and JOB_TYP=1 and MAINORGANISATION=1");
            string oeIDs = "";
            bool isFirst = true;
            foreach (DataRow row in oeTable.Rows){
                if (isFirst){
                    isFirst = false;
                }
                else{
                    oeIDs += ",";
                }
                oeIDs += row[0].ToString();
            }
            return oeIDs;
        }

        /// <summary>
        /// Get jobs for main organisation
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        public DataTable getMOJobs(long personID) {
            string sql = "select JOB.* from ORGANISATION O inner join (ORGENTITY ORGENTITY inner join (JOB inner join EMPLOYMENT E on JOB.EMPLOYMENT_ID=E.ID and E.PERSON_ID=" + personID +") on JOB.ORGENTITY_ID=ORGENTITY.ID) on O.ORGENTITY_ID=ORGENTITY.ROOT_ID where O.MAINORGANISATION=1";
            DataTable data = _db.getDataTable(sql);
            return data;
        }


        /// <summary>
        /// get leader of
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>leader id</returns>
        public long getLeader(long userId, long jobId, long oeId, bool recursive) {
            string where = "o.mainorganisation = 1 and j.person_id = "+userId;
            if (jobId > 0) where += " and j.id = "+jobId;
            if (oeId > 0) where += " and j.orgentity_id = "+oeId;
            object[] objs = _db.lookup(new string[] {"j.typ","j.id","j.orgentity_id","j.oe_parentid"},"jobemploymentv j inner join organisation o on j.oe_rootid = o.orgentity_id",where);
            int typ = DBColumn.GetValid(objs[0],-1);
            jobId = DBColumn.GetValid(objs[1],0L);
            oeId = DBColumn.GetValid(objs[2],0L);
            long parentOE = DBColumn.GetValid(objs[3],0L);
            long id = 0;

            if (typ == 0) {
                objs = _db.lookup(new string[] {"p2.person_id","p2.oe_parentid"},"jobemploymentv p1 inner join (jobemploymentv p2 inner join organisation o on p2.oe_rootid = o.orgentity_id) on p1.orgentity_id = p2.orgentity_id","o.mainorganisation = 1 and p1.id = "+jobId+" and p2.person_id <> "+userId+" and p2.typ = 1");
            }
            else if (typ == 1) {
                objs = _db.lookup(new string[] {"j.person_id","j.oe_parentid"},"jobemploymentv j inner join organisation o on j.oe_rootid = o.orgentity_id","o.mainorganisation = 1 and j.typ = 1 and j.orgentity_id = "+parentOE);
            }
            else return 0;

            id = DBColumn.GetValid(objs[0],0L);
            if (id > 0) return id;
            parentOE = DBColumn.GetValid(objs[1],0L);

            if (recursive) {
                while (parentOE > 0) {
                    id = DBColumn.GetValid(_db.lookup("person_id","jobemploymentv","typ = 1 and orgentity_id="+parentOE),0L);
                    if (id > 0) return id;
                    parentOE = DBColumn.GetValid(_db.lookup("parent_id","orgentity","id="+parentOE),0L);
                }
            }
            return 0;
        }

        /// <summary>
        /// Checks if the currently logged person is leader (based on the mainorganisation) of another person (through job-id).
        /// </summary>
        /// <param name="ID">ID of person to check</param>
        /// <param name="recursive">set to true to consider also sub-organisation-entities</param>
        /// <returns></returns>
        public bool isLeaderOfJob(long jobID, bool recursive){
            string oeIDs = getLeadingOEIDs(_db.userId);
            if (oeIDs == "") {
                return false;
            }
            else {
                string subOEIDs = oeIDs;
                if (recursive){
                    subOEIDs = _db.Orgentity.addAllSubOEIDs(subOEIDs);
                }
                return ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "PERSONOEV", "JOB_ID=" + jobID + " and OE_ID in (" + subOEIDs + ") and not (JOB_TYP=1 and OE_ID in (" + oeIDs + ")) and not ID=" + _db.userId, false), -1L) > 0L;
            }
        }

        /// <summary>
        /// Checks if the currently logged person is leader (based on the mainorganisation) of another person (through person-id).
        /// </summary>
        /// <param name="ID">ID of person to check</param>
        /// <param name="recursive">set to true to consider also sub-organisation-entities</param>
        /// <returns></returns>
        public bool isLeaderOfPerson(long personID, bool recursive){
            return isLeaderOfPerson(_db.userId,personID,recursive);
        }

        /// <summary>
        /// Checks if a person is leader (based on the mainorganisation) of another person (through person-id).
        /// </summary>
        /// <param name="leaderId"></param>
        /// <param name="personID"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public bool isLeaderOfPerson(long leaderId, long personID, bool recursive){
            string oeIDs = getLeadingOEIDs(leaderId);
            if (oeIDs == "") {
                return false;
            }
            else {
                string subOEIDs = oeIDs;
                if (recursive){
                    subOEIDs = _db.Orgentity.addAllSubOEIDs(subOEIDs);
                }
                return ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "PERSONOEV", "ID=" + personID + " and OE_ID in (" + subOEIDs + ") and not (JOB_TYP=1 and OE_ID in (" + oeIDs + ")) and not ID=" + leaderId, false), -1L) > 0L;
            }
        }

        //        /// <summary>
        //        /// Returns comma-separated list of all Person IDs which are members of OrgEntity's (and subOrgEntity's) the person is leading 
        //        /// </summary>
        //        /// <param name="personID">ID of person</param>
        //        /// <param name="recursive">set to true to consider also sub-organisation-entities</param>
        //        /// <returns>Comma-separated list of IDs</returns>
        //        public string getMembersOfLeadingOEIDs(long personID, bool recursive) {
        //            DataTable table = _db.getDataTable("select OE_ID, OE_PARENTID from PERSONOEV where OE_PARENTID is not null");
        //            string leadingOEIDs = getLeadingOEIDs(personID);
        //
        //            if (recursive){
        //                leadingOEIDs = addAllSubOEIDs(leadingOEIDs);
        //            }
        //            
        //            string pIDs = "";
        //
        //            if (leadingOEIDs != ""){
        //                DataTable oeTable = _db.getDataTable("select distinct ID from PERSONOEV where OE_ID in (" + leadingOEIDs + ")");
        //                bool isFirst = true;
        //                foreach (DataRow row in oeTable.Rows) {
        //                    if (isFirst) {
        //                        isFirst = false;
        //                    }
        //                    else {
        //                        pIDs += ",";
        //                    }
        //                    pIDs += row[0].ToString();
        //                }
        //            }
        //            return pIDs;
        //        }
        //        
        //        /// <summary>
        //        /// Returns DataTable of all Person WholeNames which are members of OrgEntity's (and subOrgEntity's) the person is leading
        //        /// </summary>
        //        /// <param name="personID">ID of person</param>
        //        /// <param name="recursive">set to true to consider also sub-organisation-entities</param>
        //        /// <returns>Comma-separated list of IDs</returns>
        //        public DataTable getWholeNameMembersOfLeadingOEIDsTable(long personID, bool recursive)
        //        {
        //            string personSql = "select ID, " + getWholeNameSQL(false, true, false) + " from PERSON where (TYP & 1)>0 and (TYP & 2)=0 and id in (" + getMembersOfLeadingOEIDs(personID, recursive)  + ") order by PNAME, FIRSTNAME";
        //            return _db.getDataTable(personSql);
        //        }

        /// <summary>
        /// Get short phone number
        /// (the numer of characters can be set in the config.xml file)
        /// </summary>
        /// <param name="fullNumber">full phone number string</param>
        /// <returns>short phone number string</returns>
        public static string getShortPhoneNumber(string fullNumber) {
            int charNumber = ch.psoft.Util.Validate.GetValid(Global.Config.shortPhoneNumber, 0);
            int index = 0;

            for(index = fullNumber.Length; index > 0 && charNumber > 0; index--) {
                if(!fullNumber.Substring(index-1, 1).Equals(" "))
                    charNumber--;
            }

            return fullNumber.Substring(index, fullNumber.Length - index);
        }

        /// <summary>
        /// Update new photo information on server
        /// </summary>
        /// <param name="personID">person database record ID</param>
        /// <param name="filename">photo file name including extension</param>
        /// <param name="config">config object (application scope)</param>
        /// <returns>TRUE: successful FALSE:not successful</returns>
        public bool updatePhotoInformation(long personID, string filename, string path) {
            FileInfo photoFile = new FileInfo(path + ch.psoft.Util.Validate.GetValid(_db.lookup("PHOTO", "PERSON", "ID=" + personID, false), ""));

            if(photoFile.Exists){
                photoFile.Delete();
            }
			
            return _db.execute("UPDATE PERSON SET PHOTO='" + filename + "' WHERE ID=" + personID) == 1;
        }

        /// <summary>
        /// Checks if the person can change his password.
        /// </summary>
        /// <param name="personID">person database record ID</param>
        /// <returns>true, if he is allowed to change his password. Otherwise false.</returns>
        public bool canChangePassword(long personID){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("PWD_CHANGEABLE", "PERSON", "ID=" + personID, false), 0) == 1;
        }
    }
}
