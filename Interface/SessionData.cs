using ch.appl.psoft.db;
using System;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface
{
    /// <summary>
    /// Summary description for SessionData.
    /// </summary>
    public class SessionData
	{
        protected const string _userID = "UserID";
        protected const string _userAccessorID = "UserAccessorID";
        protected const string _isAdmin = "IsAdmin";
        protected const string _rowsPerListPage = "RowsPerListPage";
        protected const string _maxTreeCaptionLength = "MaxTreeCaptionLength";
        protected const string _registryRootID = "RegistryRootID";
        protected const string _showDoneTasklists = "ShowDoneTasklists";
        protected const string _showDoneMeasures = "ShowDoneMeasures";
		protected const string _showSubMeasures = "ShowSubMeasures";
        protected const string _dbColum = "DBColumn";
        protected const string _showDoneAdvancements = "ShowDoneAdvancements";
        protected const string _sessionID = "SessionID";
        protected const string _showValidDutyCompOnly = "ShowValidDutyCompOnly";
        protected const string _showValidSkillLevelOnly = "ShowValidSkillLevelOnly";
		protected const string _showInactiveProjects = "ShowInactiveProjects";
        
        public SessionData()
		{
		}

        public static int getIntValue(HttpSessionState Session, string key)
        {
            Object retValue = Session[key];
            if (retValue == null)
                return -1;
            else
                return (int) retValue;
        }

        public static long getLongValue(HttpSessionState Session, string key)
        {
            Object retValue = Session[key];
            if (retValue == null)
                return -1;
            else
                return (long) retValue;
        }

        public static bool getBoolValue(HttpSessionState Session, string key)
        {
            Object retValue = Session[key];
            if (retValue == null)
                return false;
            else
                return (bool) retValue;
        }
        public static string getStringValue(HttpSessionState Session, string key) {
            Object retValue = Session[key];
            if (retValue == null)
                return "";
            else
                return (string) retValue;
        }


        public static void setUserID(HttpSessionState Session, long userID)
        {
            Session[_userID] = userID;
        }
        public static long getUserID(HttpSessionState Session)
        {
            return getLongValue(Session, _userID);
        }

        
        public static void setUserAccessorID(HttpSessionState Session, long userAccessorID)
        {
            Session[_userAccessorID] = userAccessorID;
        }
        public static long getUserAccessorID(HttpSessionState Session)
        {
            return getLongValue(Session, _userAccessorID);
        }

        
        public static void setRowsPerListPage(HttpSessionState Session, int rowsPerListPage) {
            Session[_rowsPerListPage] = rowsPerListPage;
        }
        public static int getRowsPerListPage(HttpSessionState Session) {
            return getIntValue(Session, _rowsPerListPage);
        }


        public static void setMaxTreeCaptionLength(HttpSessionState Session, int maxTreeCaptionLength) {
            Session[_maxTreeCaptionLength] = maxTreeCaptionLength;
        }
        public static int getMaxTreeCaptionLength(HttpSessionState Session) {
            return getIntValue(Session, _maxTreeCaptionLength);
        }

        
        public static void setRegistryRootID(HttpSessionState Session, long registryRootID)
        {
            Session[_registryRootID] = registryRootID;
        }
        public static long getRegistryRootID(HttpSessionState Session)
        {
            return getLongValue(Session, _registryRootID);
        }

        
        public static void setShowDoneTasklists(HttpSessionState Session, bool showDoneTasklists)
        {
            Session[_showDoneTasklists] = showDoneTasklists;
        }
        public static bool showDoneTasklists(HttpSessionState Session)
        {
            return getBoolValue(Session, _showDoneTasklists);
        }

        
        public static void setShowDoneMeasures(HttpSessionState Session, bool showDoneMeasures)
        {
            Session[_showDoneMeasures] = showDoneMeasures;
        }
        public static bool showDoneMeasures(HttpSessionState Session)
        {
            return getBoolValue(Session, _showDoneMeasures);
        }

		public static void setShowSubMeasures(HttpSessionState Session, bool showSubMeasures)
		{
			Session[_showSubMeasures] = showSubMeasures;
		}
		public static bool showSubMeasures(HttpSessionState Session)
		{
			return getBoolValue(Session, _showSubMeasures);
		}

        public static void setDBColumn(HttpSessionState Session, DBColumn dbColumn) {
            Session[_dbColum] = dbColumn;
        }

        public static DBColumn getDBColumn(HttpSessionState Session) {
            return (DBColumn) Session[_dbColum];
        }

        public static void setShowDoneAdvancements(HttpSessionState Session, bool showDoneAdvancements)
        {
            Session[_showDoneAdvancements] = showDoneAdvancements;
        }
        public static bool showDoneAdvancements(HttpSessionState Session)
        {
            return getBoolValue(Session, _showDoneAdvancements);
        }

        public static void setSessionID(HttpSessionState Session, int sessionID) {
            Session[_sessionID] = sessionID;
        }
        public static int getSessionID(HttpSessionState Session) {
            return getIntValue(Session, _sessionID);
        }

        public static void setShowValidDutyCompOnly(HttpSessionState Session, bool showValidDutyCompOnly)
        {
            Session[_showValidDutyCompOnly] = showValidDutyCompOnly;
        }
        public static bool showValidDutyCompOnly(HttpSessionState Session)
        {
            if (Session[_showValidDutyCompOnly] == null) 
                setShowValidDutyCompOnly(Session, true);
            return getBoolValue(Session, _showValidDutyCompOnly);
        }

        public static void setShowValidSkillLevelOnly(HttpSessionState Session, bool showValidSkillLevelOnly)
        {
            Session[_showValidSkillLevelOnly] = showValidSkillLevelOnly;
        }
        public static bool showValidSkillLevelOnly(HttpSessionState Session)
        {
            if (Session[_showValidSkillLevelOnly] == null) 
                setShowValidSkillLevelOnly(Session, true);
            return getBoolValue(Session, _showValidSkillLevelOnly);
        }

		public static void setShowInactiveProjects(HttpSessionState Session, bool showInactiveProjects)
		{
			Session[_showInactiveProjects] = showInactiveProjects;
		}
		public static bool showInactiveProjects(HttpSessionState Session)
		{
			return getBoolValue(Session, _showInactiveProjects);
		}
    }
}
