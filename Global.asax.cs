using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Organisation;
using ch.appl.psoft.Report;
using ch.appl.psoft.Subscription;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;

namespace ch.appl.psoft
{
    /// <summary>
    /// Application Scope objects:
    /// 1. LogUser:		number of 
    /// 2. Config:		Configuration objects
    /// 3. LangMap:		Language Mapper Object
    /// 
    /// Session Scope objects:
    /// 1. UserID:		Person database record ID after successful log in
    /// </summary>
    public class Global : System.Web.HttpApplication {
        private static string _HighlightColorRGB = "#f2b400";
        private static Color _HighlightColor = Color.FromArgb(0x78f2b400);
        private static Config _config = new Config();
        private static Global _CurrentInstance = null;
        private static int _threadID = 1;
        private static int _sessionID = 1;

        public static ListDictionary s_ModulesDict = new ListDictionary();
 
        public static psoftModule Module(string moduleName) {
            return (psoftModule) s_ModulesDict[moduleName];
        }

        public static bool isModuleEnabled(string moduleName) {
            return s_ModulesDict[moduleName] != null;
        }

        /// <summary>
        /// Property access methode für die Highlight-Farbe
        /// </summary>
        public static Color HighlightColor {
            get{ return _HighlightColor; }
        }
        /// <summary>
        /// Get highlight color rgb-value
        /// </summary>
        public static string HighlightColorRGB {
            get{ return _HighlightColorRGB; }
        }
 
        public static Config Config {
            get{ return _config; }
            set{ _config = value; }
        }

        public static Global CurrentInstance {
            get{ return _CurrentInstance; }
        }

        public Global() {
            _CurrentInstance = this;
            InitializeComponent();
        }	
		
        public static bool reloadLanguageMapper(LanguageMapper map, string languageCode)
        {
            bool retValue = true;
            
            string fileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "XML/data/language_" + languageCode + ".xml";
            map.load(fileName, languageCode, true);

            foreach (psoftModule module in s_ModulesDict.Values) 
            {
                module.LoadLanguageFile(map, languageCode);
            }

            //load delta language mapper, i.e. the customer specific language mapper which only defines a subset of values.
            if (!Global.Config.getCommonSetting("deltaLanguageDir", "").Equals(""))
            {
                if (!Global.Config.getCommonSetting("deltaLanguageName", "").Equals(""))
                {
                    string deltaLanguageFileName = AppDomain.CurrentDomain.BaseDirectory +
                                                   Global.Config.getCommonSetting("deltaLanguageDir", "XML/data/") +
                                                   Global.Config.getCommonSetting("deltaLanguageName", "") +
                                                   "_" +
                                                   languageCode + ".xml";
                    map.load(deltaLanguageFileName, languageCode, false, true);
                }
                else
                {
                    //error

                }
            }

            return retValue;
        }

        protected void Application_Start(Object sender, EventArgs e) {

            string JQueryVer = "3.1.0";
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition
            {
                Path = "~/JavaScript/jquery-" + JQueryVer + ".min.js",
                DebugPath = "~/JavaScript/jquery-" + JQueryVer + ".js",
                //CdnPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-" + JQueryVer + ".min.js",
                //CdnDebugPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-" + JQueryVer + ".js",
                CdnSupportsSecureConnection = true,
                LoadSuccessExpression = "window.jQuery"
            });



            bool connected = false;

            Application["LogUser"] = 0;

            // Initialize Authentication
            FormsAuthentication.Initialize();

            string temp = Server.MapPath("~/XML/data");

            // Logger setup
            Logger.Directory = Server.MapPath("~/log");
            Logger.Open();

            // Load Config
            _config.directory = temp;
            _config.file = "config.xml";
            _config.load();

            // Load Modules
            s_ModulesDict.Clear();
            s_ModulesDict.Add("organisation", new OrganisationModule()); // organisation-module is part of base
            s_ModulesDict.Add("news", new NewsModule()); // news-module is part of base
            string [] modules = _config.getModuleNames();
            foreach (string module in modules) {
                if (_config.isModuleEnabled(module)) {
                    string assemblyName = _config.getModuleAttribute(module, "assemblyName");
                    string className = _config.getModuleAttribute(module, "className");
                    if (className != "") {
                        if (className.IndexOf('.') < 0)
                            className = "ch.appl.psoft.Interface." + className;
                        Assembly ass = null;
                        if (assemblyName != "") {
                            if (File.Exists(assemblyName))
                            {
                                ass = Assembly.Load(assemblyName);
                            }
                            else
                            {
                                Logger.Log("Assembly '" + assemblyName + "' of module '" + module + "' could not be loaded! Trying to find it locally...", Logger.WARNING);
                            }
                        }

                        if (ass == null)
                            ass = Assembly.GetExecutingAssembly();

                        if (ass != null) {
                            object o = ass.CreateInstance(className, true);
                            if (o != null) {
                                s_ModulesDict.Add(module, o);
                                Logger.Log("Module '" + module + "' loaded.", Logger.MESSAGE);
                            }
                            else
                                Logger.Log("Instance '" + className + "' of module '" + module + "' could not be created!", Logger.ERROR);
                        }
                    }
                    else
                        Logger.Log("Module '" + module + "' must contain a 'className' attribute!", Logger.ERROR);
                }
            }

            // Initialize DBColumn
            DBColumn.DBLanguageCode = _config.dbLanguageCode+"-"+_config.dbRegionCode;

            // Load LanguageMapper
            LanguageMapper map = new LanguageMapper();
            if (reloadLanguageMapper(map, _config.languageCode))
                LanguageMapper.setLanguageMapper(Application, map);

            // Set default colors...
            if (_config.highlightColor != ""){
                _HighlightColorRGB = _config.highlightColor;
                int red = int.Parse(_HighlightColorRGB.Substring(1,2), System.Globalization.NumberStyles.AllowHexSpecifier);
                int green = int.Parse(_HighlightColorRGB.Substring(3,2), System.Globalization.NumberStyles.AllowHexSpecifier);
                int blue = int.Parse(_HighlightColorRGB.Substring(5,2), System.Globalization.NumberStyles.AllowHexSpecifier);
                _HighlightColor = Color.FromArgb(red, green, blue);
            }

            // Clean-up temporary-directories
            string sql = "select filename from document_delete";
            // pooling set to false to be able to dispose the connection. Otherwise detaching in administration page does not work.
            SQLDB db = new SQLDB(_config.dbDriver, _config.dbUser, _config.dbPassword, _config.dbServer, _config.dbPort, _config.dbName, _config.dbLanguageCode, false);

            foreach (string file in Directory.GetFiles(_config.documentTempDirectory != "" ? _config.documentTempDirectory : Directory.GetCurrentDirectory())) {
                try {
                    File.Delete(file);
                }
                catch (Exception ex) {
                    Logger.Log(ex,Logger.WARNING);
                }
            }
            try {
                db.connect();
                connected = true;
                DataTable table = db.getDataTable(sql);
                foreach (DataRow row in table.Rows) {
                    try {
                        string file = row[0].ToString();
                        if (_config.documentSaveDirectory != "")
                            file = _config.documentSaveDirectory+"\\"+file;
                        File.Delete(file);
                    }
                    catch (Exception ex) {
                        Logger.Log(ex,Logger.WARNING);
                    }
                }
                sql = "delete from document_delete";
                db.execute(sql);
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                try {
                    if (connected) {
                        db.disconnect();
                        db.connection.Dispose();
                    }
                }
                catch (Exception ex) {
                    Logger.Log(ex,Logger.ERROR);
                }
            }
            
            // Clean-up reports
            ReportModule.CleanupReportsDirectory();

            foreach (psoftModule module in s_ModulesDict.Values) {
                module.Application_Start(Application);
            }

            //SokratesMaps checkout table
            DataTable lockedSokratesMaps = new DataTable() ;
            lockedSokratesMaps.Columns.Add("mapId", typeof(int));
            lockedSokratesMaps.Columns.Add("userName", typeof(string));

            Application.Add("lockedSokratesMaps", lockedSokratesMaps);



        }
 
        protected void Session_Start(Object sender, EventArgs e) {
            // Increment counter of logged users
            lock(Application) {
                int num = (int) Application["LogUser"];
                SessionData.setUserID(Session, -1);
                num++;
                Application["LogUser"] = num;
                Logger.Log("Starting new session - open sessions: " + num, Logger.MESSAGE);
            }

            // Load LanguageMapper
            LanguageMapper map = new LanguageMapper();
            if (reloadLanguageMapper(map, _config.languageCode)) 
                LanguageMapper.setLanguageMapper(Session, map);

            // Load DBColumn
            DBColumn dbColumn = new DBColumn();
            dbColumn.UserCultureName = _config.languageCode+"-"+_config.regionCode;
            SessionData.setDBColumn(Session, dbColumn);
            SessionData.setSessionID(Session, _sessionID++);

            // Make sure that the user goes through the login-process
            FormsAuthentication.SignOut();

            foreach (psoftModule module in s_ModulesDict.Values) {
                module.Session_Start(Session);
            }
        }

        protected void Application_PreRequestHandlerExecute(Object sender, EventArgs e) {
        /*    
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            try{
                DBColumn dbColumn = SessionData.getDBColumn(Session);
                string languageCode = ch.psoft.Util.Validate.GetValid(Request.QueryString["languageCode"], "");
                string regionCode = ch.psoft.Util.Validate.GetValid(Request.QueryString["regionCode"], "");
                if (languageCode != "" || regionCode != "") {
                    LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
                    if (languageCode == ""){
                        string [] splits = dbColumn.UserCultureName.Split('-');
                        if (splits.Length > 1) {
                            languageCode = splits[0];
                        }
                    }
                    if (regionCode == ""){
                        string [] splits = dbColumn.UserCultureName.Split('-');
                        if (splits.Length > 1) {
                            regionCode = splits[1];
                        }
                    }
                    if (regionCode != ""){
                        dbColumn.UserCultureName = languageCode + "-" + regionCode;
                    }            
                    if (map.LanguageCode != languageCode){
                        reloadLanguageMapper(LanguageMapper.getLanguageMapper(Session), languageCode);
                    }
                }
                // Let's set the culture-info of the thread to ensure a culturally correct user interface
                Thread.CurrentThread.CurrentCulture = dbColumn.UserCulture;
            }
            catch(HttpException){
                // we can't get the SessionState in certain context (i.e. ../WebService/PsoftService.asmx?WSDL)
            }
         */
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (ch.psoft.Util.Validate.GetValid(Thread.CurrentThread.Name, "") == "")
                Thread.CurrentThread.Name = _threadID++.ToString();

            Logger.Log("URL: " + Request.RawUrl, Logger.DEBUG);
        }

        protected void Application_EndRequest(Object sender, EventArgs e) {
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
        }

        protected void Application_Error(Object sender, EventArgs e) {

        }

        protected void Session_End(Object sender, EventArgs e) {

			//Clean tmp-tables in DB
			DBData db = DBData.getDBData(Session);
			try 
			{
				db.connect();
                //TODO exchange enable

				//db.execute("DELETE FROM EXCHANGE_MESSAGES_TMP WHERE SESSIONID='" + Session.SessionID + "'");
				//db.execute("DELETE FROM EXCHANGE_ATTACHEMENTS_TMP WHERE SESSIONID='" + Session.SessionID + "'");
			} 
			catch (Exception) 
			{
				//ignore
			} 
			finally 
			{
				db.disconnect();
			}


            if (SessionData.getUserID(Session) > 0){
                Logger.Log("User '" + Session["UserInitials"] + "' logged out.", Logger.MESSAGE);
            }

            lock(Application) {
                int num = (int) Application["LogUser"];
                
                num--;
                Application["LogUser"] = num;
                Logger.Log("Ending session - open sessions: " + num, Logger.MESSAGE);
            }        

            foreach (psoftModule module in s_ModulesDict.Values) {
                module.Session_End(Session);
            }
        }

        protected void Application_End(Object sender, EventArgs e) {
            DBData db = DBData.getDBData();

            db.connect();
            try {

                //delete any temporary records in data base
                string sqlStatement = "delete from SEARCHRESULT where TEMPORARY=1";
                db.execute(sqlStatement);

                // Delete all temporary mailings and documents
                db.Mailing.deleteAllTemporary();
                db.DispatchDocument.deleteAllTemporary();

                //delete viewstate files
                this.CleanupViewStateDirectory();
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                db.disconnect();
            }

            foreach (psoftModule module in s_ModulesDict.Values) {
                module.Application_End(Application);
            }

            Logger.Log("exiting Application",Logger.DEBUG);
            Logger.Close();
        }

        protected void CleanupViewStateDirectory()
        {
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory.ToString() + "tmp");
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, Logger.ERROR);
                }
            }
        }

        // methods to encode and decode strings to/from Base64 (used for parameters in reports) / 27.07.10 / mkr
        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.Unicode.GetBytes(toEncode);

            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }

        public static string DecodeFrom64(string toDecode)
        {
            string returnValue = "";
            
            try
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(toDecode);
                returnValue = System.Text.ASCIIEncoding.Unicode.GetString(encodedDataAsBytes);
            }
            catch (FormatException e)
            {
                //invalid data!
                returnValue = "no_data";
            }

            return returnValue;
        }
			
		#region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {    
        }
		#endregion
    }
}

