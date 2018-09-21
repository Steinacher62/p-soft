using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Xml;

namespace ch.appl.psoft.Interface
{
    /// <summary>
    /// Psoft Konfigurations-parameter
    /// </summary>
    public class Config {
		public class AUTHENTICATION_MODE{
			public const string LOGIN = "login";
			public const string ANONYMOUS = "anonymous";
			public const string WINDOWS = "windows";
			public const string WINDOWS_CONFIRM = "windowsConfirm";
		}

		private string _directory = "";
		private string _file = "";
		private XmlNode _lang = null;
		private XmlNode _db = null;
		private XmlNode _ftp = null;
		private XmlNode _www = null;
		private XmlNode _organisation = null;
		private XmlNode _cms = null;
		private XmlNode _display = null;
		private XmlNode _authentication = null;
		private XmlNode _commonsettings = null;
		private XmlNodeList _modules = null;
		private bool _loaded = false;
		private XmlDocument _document = null;

		private const string _DB = "db";
		private const string _FTP = "ftp";
		private const string _WWW = "www";
		private const string _LANGUAGE = "language";
		private const string _ORGANISATION = "organisation";
		private const string _CMS = "cms";
		private const string _DISPLAY = "display";
		private const string _AUTHENTICATION = "authentication";
		private const string _COMMONSETTINGS = "commonsettings";
		private const string _MODULE = "module";

		/// <summary>
		/// Set/get Directory
		/// </summary>
		public string directory {			
			get { return _directory; }
			set {
				_directory = value; 
				if (!_directory.EndsWith("\\")) _directory = _directory+"\\";
			}
		}

		/// <summary>
		/// Set/get XML file name
		/// </summary>
		public string file {			
			get { return _file; }
			set {_file = value; }
		}
		/// <summary>
		/// Load Psoft configuration xml-file
		/// </summary>
		/// <returns>true: ok</returns>
		public bool load () {
			bool ok = true;
			string file = _directory+_file;
			_document = new XmlDocument();
			try {         
				Logger.Log("Load config: " + file,Logger.DEBUG);
				_document.Load(file);
				_lang = _document.GetElementsByTagName(_LANGUAGE)[0];   
				_db = _document.GetElementsByTagName(_DB)[0];   
				_ftp = _document.GetElementsByTagName(_FTP)[0];   
				_www = _document.GetElementsByTagName(_WWW)[0];   
				_organisation = _document.GetElementsByTagName(_ORGANISATION)[0];   
				_cms = _document.GetElementsByTagName(_CMS)[0];   
				_display = _document.GetElementsByTagName(_DISPLAY)[0]; 
				_authentication = _document.GetElementsByTagName(_AUTHENTICATION)[0];
				_commonsettings = _document.GetElementsByTagName(_COMMONSETTINGS)[0];
				_modules = _document.GetElementsByTagName(_MODULE);   
				_loaded = true;
			}
			catch (Exception e) {
				Logger.Log(e.Message,Logger.ERROR);
				ok = false;
			}
			return ok;
		}

		/// <summary>
		/// Writes Psoft configuration xml-file
		/// </summary>
		public void write() {
			string file = _directory + _file;
			_document.Save(file);
		}

		private string getParameterAttribute(string xmlNode, string attributeName) {
			return getParameterAttribute(xmlNode,attributeName, ""); 
		}

		private string getParameterAttribute(string xmlNode, string attributeName, string defaultValue) {
			if (!_loaded)
				return defaultValue;

			switch (xmlNode) {
			case _LANGUAGE:
				return _lang.Attributes[attributeName].Value; 
			case _DB:
				return _db.Attributes[attributeName].Value; 
			case _FTP:
				return _ftp.Attributes[attributeName].Value; 
			case _WWW:
				return _www.Attributes[attributeName].Value; 
			case _ORGANISATION:
				return _organisation.Attributes[attributeName].Value; 
			case _CMS:
				return _cms.Attributes[attributeName].Value; 
			case _DISPLAY:
				return _display.Attributes[attributeName].Value; 
			case _AUTHENTICATION:
				return _authentication.Attributes[attributeName].Value; 
			default:
				return "";
			}
		}

		private void setParameterAttribute(string xmlNode, string attributeName, string attributeValue) {
			if (_loaded) {
				switch (xmlNode) {
				case _LANGUAGE:
					_lang.Attributes[attributeName].Value = attributeValue;
					break;
				case _DB:
					_db.Attributes[attributeName].Value = attributeValue; 
					break;
				case _FTP:
					_ftp.Attributes[attributeName].Value = attributeValue; 
					break;
				case _WWW:
					_www.Attributes[attributeName].Value = attributeValue; 
					break;
				case _ORGANISATION:
					_organisation.Attributes[attributeName].Value = attributeValue; 
					break;
				case _CMS:
					_cms.Attributes[attributeName].Value = attributeValue; 
					break;
				case _DISPLAY:
					_display.Attributes[attributeName].Value = attributeValue; 
					break;
				case _AUTHENTICATION:
					_authentication.Attributes[attributeName].Value = attributeValue; 
					break;
				default:
					break;
				}
			}
		}

		/// <summary>
		/// Get language locale
		/// </summary>
		public string languageCode {
			get { return getParameterAttribute(_LANGUAGE,"LanguageCode"); }
			set { setParameterAttribute(_LANGUAGE,"LanguageCode",value); }
		}
		/// <summary>
		/// Get region locale
		/// </summary>
		public string regionCode {
			get { return getParameterAttribute(_LANGUAGE,"RegionCode"); }
			set { setParameterAttribute(_LANGUAGE,"RegionCode",value); }
		}
		/// <summary>
		/// Get DB-Server ip
		/// </summary>
		public string dbServer {
			get { return getParameterAttribute(_DB,"Server"); }
			set { setParameterAttribute(_DB,"Server",value); }
		}
		/// <summary>
		/// Get DB-Server ip
		/// </summary>
		public string dbPort {
			get { return getParameterAttribute(_DB,"Port"); }
			set { setParameterAttribute(_DB,"Port",value); }
		}
		/// <summary>
		/// Get DB name
		/// </summary>
		public string dbName {
			get { return getParameterAttribute(_DB,"Name"); }
			set { setParameterAttribute(_DB,"Name",value); }
		}
		/// <summary>
		/// Get DB user
		/// </summary>
		public string dbUser {
			get { return getParameterAttribute(_DB,"User"); }
			set { setParameterAttribute(_DB,"User",value); }
		}
		/// <summary>
		/// Get DB userpassword
		/// </summary>
		public string dbPassword {
			get { return getParameterAttribute(_DB,"Password"); }
			set { setParameterAttribute(_DB,"Password",value); }
		}
		/// <summary>
		/// Get DB-Driver classname
		/// </summary>
		public string dbDriver {
			get { return getParameterAttribute(_DB,"DriverClass"); }
			set { setParameterAttribute(_DB,"DriverClass",value); }
		}

		/// <summary>
		/// Get DB language code
		/// </summary>
		public string dbLanguageCode {
			get { return getParameterAttribute(_DB,"LanguageCode"); }
			set { setParameterAttribute(_DB,"LanguageCode",value); }
		}
		/// <summary>
		/// Get DB region code
		/// </summary>
		public string dbRegionCode {
			get { return getParameterAttribute(_DB,"RegionCode"); }
			set { setParameterAttribute(_DB,"RegionCode",value); }
		}

		/// <summary>
		/// Get/Set DB MultiLanguage
		/// </summary>
		public bool dbMultiLanguageEnable {
			get { return getParameterAttribute(_DB,"MultiLanguage") == "enable"; }
			set { setParameterAttribute(_DB,"MultiLanguage",value ? "enable" : "disable"); }
		}
		/// <summary>
		/// Get number of short phone number representation
		/// </summary>
		public string shortPhoneNumber {
			get { return getParameterAttribute(_ORGANISATION,"ShortPhoneNumber", "4"); }
			set { setParameterAttribute(_ORGANISATION,"ShortPhoneNumber",value); }
		}
		
		/// <summary>
		/// Should only main job be displayed?
		/// </summary>
		public bool showMainJobOnly
		{
			get { return getParameterAttribute(_ORGANISATION, "showMainJobOnly", "0") == "1"; }
			set { setParameterAttribute(_ORGANISATION, "showMainJobOnly", value ? "1" : "0"); }
		}

		/// <summary>
		/// Should Report "Stammdaten" be displayed?
		/// </summary>
		public bool showReportPersonData
		{
			get { return getParameterAttribute(_ORGANISATION, "showReportPersonData", "0") == "1"; }
			set { setParameterAttribute(_ORGANISATION, "showReportPersonData", value ? "1" : "0"); }
		}

		/// <summary>
		/// Should charts be exportable to Visio?
		/// </summary>
		public bool enableVisio
		{
			get { return getParameterAttribute(_ORGANISATION, "enableVisio", "0") == "1"; }
			set { setParameterAttribute(_ORGANISATION, "enableVisio", value ? "1" : "0"); }
		}

		/// <summary>
		/// Should display vage field?
		/// </summary>
		public bool showVageField
		{
			get { return getParameterAttribute(_ORGANISATION, "showVageField", "0") == "1"; }
			set { setParameterAttribute(_ORGANISATION, "showVageField", value ? "1" : "0"); }
		}



		/// <summary>
		/// Should display jounal for Creator only
		/// </summary>
		public bool showJournalOnlyCreator
		{
			get { return getParameterAttribute(_ORGANISATION, "showJournalOnlyCreator", "0") == "1"; }
			set { setParameterAttribute(_ORGANISATION, "showJournalOnlyCreator", value ? "1" : "0"); }
		}

		/// <summary>
		/// Shop active
		/// </summary>
		public bool shopActive
		{
			get { return getParameterAttribute(_ORGANISATION, "shopActive", "0") == "1"; }
			set { setParameterAttribute(_ORGANISATION, "shopActive", value ? "1" : "0"); }
		}

		/// <summary>
		/// Get Paypal Client ID
		/// </summary>
		public string clientId
		{
			get { return getParameterAttribute(_ORGANISATION, "clientId",""); }
		}

		/// <summary>
		/// Get Paypal Client Secret
		/// </summary>
		public string clientSecret
		{
			get { return getParameterAttribute(_ORGANISATION, "clientSecret", ""); }
		}

		/// <summary>
		/// Should the engagement be displayed?
		/// </summary>
		public bool showEngagement
		{
			get { return getParameterAttribute(_ORGANISATION, "showEngagement", "0") == "1"; }
			set { setParameterAttribute(_ORGANISATION, "showEngagement", value ? "1" : "0"); }
		}
		
		/// <summary>
		/// Should the Organisation module be visible?
		/// </summary>
		public bool organisationModuleVisible {
			get { return getParameterAttribute(_ORGANISATION, "ModuleVisible", "1") == "1"; }
			set { setParameterAttribute(_ORGANISATION, "ModuleVisible", value? "1" : "0"); }
		}
		/// <summary>
		/// Get directory where the person photos are stored
		/// </summary>
		public string personPhotoDirectory {
			get { return getParameterAttribute(_ORGANISATION,"PersonPhotoDirectory", ""); }
			set { setParameterAttribute(_ORGANISATION,"PersonPhotoDirectory",value); }
		}
		/// <summary>
		/// Get directory where the organisation images are stored
		/// </summary>
		public string organisationImageDirectory {
			get { return getParameterAttribute(_ORGANISATION,"OrganisationImageDirectory", ""); }
			set { setParameterAttribute(_ORGANISATION,"OrganisationImageDirectory",value); }
		}
		/// <summary>
		/// Get directory where the logos are stored
		/// </summary>
		public string logoImageDirectory
		{
			get { return getParameterAttribute(_ORGANISATION, "LogoImageDirectory", ""); }
			set { setParameterAttribute(_ORGANISATION, "LogoImageDirectory", value); }
		}
		/// <summary>
		/// Get number of rows to display per list-page
		/// </summary>
		public string rowsPerListPage {
			get { return getParameterAttribute(_DISPLAY,"RowsPerListPage", "15"); }
			set { setParameterAttribute(_DISPLAY,"RowsPerListPage",value); }
		}
		/// <summary>
		/// Get maximal length of tree-captions
		/// </summary>
		public string maxTreeCaptionLength {
			get { return getParameterAttribute(_DISPLAY,"MaxTreeCaptionLength", "30"); }
			set { setParameterAttribute(_DISPLAY,"MaxTreeCaptionLength",value); }
		}

		/// <summary>
		/// Get maximal number of MaxBreadcrumb Items
		/// </summary>
		public int MaxBreadcrumbItems 
		{
			get { return System.Int32.Parse(getParameterAttribute(_DISPLAY,"MaxBreadcrumbItems", "30")); }
			set { setParameterAttribute(_DISPLAY,"MaxBreadcrumbItems",value.ToString()); }
		}

		/// <summary>
		/// Gets/sets the startup URL (URL loaded right after login)
		/// </summary>
		public string startUpURL {
			get { return getParameterAttribute(_DISPLAY, "StartUpURL", ""); }
			set { setParameterAttribute(_DISPLAY, "StartUpURL", value); }
		}
		/// <summary>
		/// Should the partner-logos on login-page be showed?
		/// </summary>
		public bool showPartnerLogos {
			get { return getParameterAttribute(_DISPLAY, "ShowPartnerLogos", "1") == "1"; }
			set { setParameterAttribute(_DISPLAY, "ShowPartnerLogos", value? "1" : "0"); }
		}
		/// <summary>
		/// Gets/sets the header logo (upper right corner)
		/// </summary>
		public string headerLogo {
			get { return getParameterAttribute(_DISPLAY, "HeaderLogo", ""); }
			set { setParameterAttribute(_DISPLAY, "HeaderLogo", value); }
		}
		/// <summary>
		/// Gets/sets the header logo's link when clicked
		/// </summary>
		public string headerLogoURL {
			get { return getParameterAttribute(_DISPLAY, "HeaderLogoURL", ""); }
			set { setParameterAttribute(_DISPLAY, "HeaderLogoURL", value); }
		}
		/// <summary>
		/// Should the global document-search be showed?
		/// </summary>
		public bool showDocumentSearch {
			get { return getParameterAttribute(_DISPLAY, "ShowDocumentSearch", "1") == "1"; }
			set { setParameterAttribute(_DISPLAY, "ShowDocumentSearch", value? "1" : "0"); }
		}
		/// <summary>
		/// Gets the highlight color
		/// </summary>
		public string highlightColor {
			get { return getParameterAttribute(_DISPLAY, "HighlightColor", ""); }
			set { setParameterAttribute(_DISPLAY, "HighlightColor", value); }
		}
		/// <summary>
		/// Should the language-selector be showed?
		/// </summary>
		public bool showLanguageSelector {
			get { return getParameterAttribute(_DISPLAY, "ShowLanguageSelector", "1") == "1"; }
			set { setParameterAttribute(_DISPLAY, "ShowLanguageSelector", value? "1" : "0"); }
		}
		/// <summary>
		/// Should the news menu and news be showed?
		/// </summary>
		public bool ShowNews {
			get { return getParameterAttribute(_DISPLAY, "ShowNews", "1") == "1"; }
			set { setParameterAttribute(_DISPLAY, "ShowNews", value? "1" : "0"); }
		}



		/// <summary>
		/// Defines a commaseparated list of languages
		/// </summary>
		public string availableLanguages 
		{
			get { return getParameterAttribute(_DISPLAY, "AvailableLanguages", "de");}
			set { setParameterAttribute(_DISPLAY, "AvailableLanguages", value); }
		}

		/// <summary>
		/// Should display helpfile
		/// </summary>
		public bool ShowHelpFile
		{
			get { return getParameterAttribute(_DISPLAY, "ShowHelpFile", "0") == "1"; }
			set { setParameterAttribute(_DISPLAY, "ShowHelpFile", value ? "1" : "0"); }
		}

		/// <summary>
		/// Should display FontSizeSelector
		/// </summary>
		public bool SelectFontSize
		{
			get { return getParameterAttribute(_DISPLAY, "SelectFontSize", "0") == "1"; }
			set { setParameterAttribute(_DISPLAY, "SelectFontSize", value ? "1" : "0"); }
		}

		/// <summary>
		/// Application p-soft / Sokrates
		/// </summary>
		public string ApplicationName
		{
			get { return getParameterAttribute(_DISPLAY, "ApplicationName", "p-soft"); }
			set { setParameterAttribute(_DISPLAY, "ApplicationName",value); }
		}

		/// <summary>
		/// Should display Password Recovery
		/// </summary>
		public bool ShowPasswordrecovery
		{
			get { return getParameterAttribute(_DISPLAY, "ShowPasswordrecovery", "0") == "1"; }
			set { setParameterAttribute(_DISPLAY, "ShowPasswordrecovery", value ? "1" : "0"); }
		}

		/// <summary>
		/// Should display Password Change
		/// </summary>
		public bool ShowPasswordChange
		{
			get { return getParameterAttribute(_DISPLAY, "ShowPasswordChange", "0") == "1"; }
			set { setParameterAttribute(_DISPLAY, "ShowPasswordChange", value ? "1" : "0"); }
		}

		/// <summary>
		/// Should display new Account
		/// </summary>
		public bool ShowNewAccount
		{
			get { return getParameterAttribute(_DISPLAY, "ShowNewAccount", "0") == "1"; }
			set { setParameterAttribute(_DISPLAY, "ShowNewAccount", value ? "1" : "0"); }
		}

		/// <summary>
		/// Background Pic Startpage
		/// </summary>
		public string BackgroundStartpage
		{
			get { return getParameterAttribute(_DISPLAY, "BackgroundStartpage", ""); }
			set { setParameterAttribute(_DISPLAY, "BackgroundStartpage", value); }
		}

		public bool ShowAdmin
		{
			get { return getParameterAttribute(_DISPLAY, "ShowAdmin", "0") == "1"; }
			set { setParameterAttribute(_DISPLAY, "ShowAdmin", value ? "1" : "0"); }
		}

		/// <summary>
		/// Get the authentication-mode
		/// </summary>
		public string authenticationMode {
			get { return getParameterAttribute(_AUTHENTICATION, "mode", "login"); }
			set { setParameterAttribute(_AUTHENTICATION, "mofr", value); }
		}
		/// <summary>
		/// Get the initials of the anonymous account
		/// </summary>
		public string anonymousAccount {
			get { return getParameterAttribute(_AUTHENTICATION, "anonymousAccount", "anony"); }
			set { setParameterAttribute(_AUTHENTICATION, "anonymousAccount", value); }
		}
		/// <summary>
		/// Get the initials of the anonymous password
		/// </summary>
		public string anonymousPassword {
			get { return getParameterAttribute(_AUTHENTICATION, "anonymousPassword", ""); }
			set { setParameterAttribute(_AUTHENTICATION, "anonymousPassword", value); }
		}
		/// <summary>
		/// Get IP for ftp document server
		/// </summary>
		public string ftpDocumentServer {
			get {
				string ftpServer = getParameterAttribute(_FTP,"FtpDocumentServer", "localhost");
				if (ftpServer.ToLower() == "localhost")
					ftpServer = System.Net.Dns.GetHostName(); // get hostname, otherwise remote clients cannot upload documents via ftp
				return ftpServer; 
			}
			set { setParameterAttribute(_FTP,"FtpDocumentServer",value); }
		}
		/// <summary>
		/// Get name for ftp document save directory
		/// </summary>
		public string ftpDocumentSaveDirectory {
			get { return getParameterAttribute(_FTP,"FtpDocumentSaveDirectory"); }
			set { setParameterAttribute(_FTP,"FtpDocumentSaveDirectory",value); }
		}
		/// <summary>
		/// Get name for ftp document temporary directory
		/// </summary>
		public string ftpDocumentTempDirectory {
			get { return getParameterAttribute(_FTP,"FtpDocumentTempDirectory"); }
			set { setParameterAttribute(_FTP,"FtpDocumentTempDirectory",value); }
		}
		/// <summary>
		/// Get name for ftp document history directory
		/// </summary>
		public string ftpDocumentHistoryDirectory {
			get { return getParameterAttribute(_FTP,"FtpDocumentHistoryDirectory"); }
			set { setParameterAttribute(_FTP,"FtpDocumentHistoryDirectory",value); }
		}
		public string ftpUser 
		{
			get { return getParameterAttribute(_FTP,"FtpUser",""); }
			set { setParameterAttribute(_FTP,"FtpUser",value); }
		}
		public string ftpPassword
		{
			get { return getParameterAttribute(_FTP,"FtpPassword",""); }
			set { setParameterAttribute(_FTP,"FtpPassword",value); }
		}
		/// <summary>
		/// Get name for document save directory
		/// </summary>
		public string documentSaveDirectory {
			get { return getParameterAttribute(_FTP,"DocumentSaveDirectory"); }
			set { setParameterAttribute(_FTP,"DocumentSaveDirectory",value); }
		}
		/// <summary>
		/// Get name for document temporary save directory
		/// </summary>
		public string documentTempDirectory {
			get { return getParameterAttribute(_FTP,"DocumentTempDirectory"); }
			set { setParameterAttribute(_FTP,"DocumentTempDirectory",value); }
		}
		/// <summary>
		/// Get name for document history save directory
		/// </summary>
		public string documentHistoryDirectory {
			get { return getParameterAttribute(_FTP,"DocumentHistoryDirectory"); }
			set { setParameterAttribute(_FTP,"DocumentHistoryDirectory",value); }
		}
		/// <summary>
		/// Get URL for document save directory
		/// </summary>
		public string documentSaveURL {
			get { return getParameterAttribute(_FTP,"DocumentSaveURL"); }
			set { setParameterAttribute(_FTP,"DocumentSaveURL",value); }
		}
		/// <summary>
		/// Get URL for document history directory
		/// </summary>
		public string documentHistoryURL {
			get { return getParameterAttribute(_FTP,"DocumentHistoryURL"); }
			set { setParameterAttribute(_FTP,"DocumentHistoryURL",value); }
		}
		/// <summary>
		/// Get name for indexing service catalog
		/// </summary>
		public string indexingCatalogName {
			get { return getParameterAttribute(_FTP,"IndexingCatalogName"); }
			set { setParameterAttribute(_FTP,"IndexingCatalogName",value); }
		}

		/// <summary>
		/// Get/set number of document versions
		/// </summary>
		public int numberOfDocumentVersions  {
			get { return ch.psoft.Util.Validate.GetValid(getParameterAttribute(_FTP,"NumberOfDocumentVersions"),-1); }
			set { setParameterAttribute(_FTP,"NumberOfDocumentVersions",value.ToString()); }
		}
		/// <summary>
		/// Get base URL for web-application
		/// Example: for Application on "http://server.com/Psoft" the base-URL is "/Psoft"
		/// </summary>
		public string baseURL {
			get { return getParameterAttribute(_WWW,"BaseURL",DefaultValues.BaseURL); }
			set { setParameterAttribute(_WWW,"BaseURL",value); }
		}
		/// <summary>
		/// Get/set base email
		/// </summary>
		public string email {
			get { return getParameterAttribute(_WWW,"EMail"); }
			set { setParameterAttribute(_WWW,"EMail",value); }
		}
		/// <summary>
		/// Get/set domain
		/// </summary>
		public string domain {
			get { return getParameterAttribute(_WWW,"Domain"); }
			set { setParameterAttribute(_WWW,"Domain",value); }
		}
		/// <summary>
		/// Get root URL for CMS Documents
		/// Example: for '/Psoft/CMSRoot' the root URL will be "http://server.com/Psoft/CMSRoot"
		/// </summary>
		public string cmsRootURL {
			get { return getParameterAttribute(_CMS,"CMSRootURL"); }
			set { setParameterAttribute(_CMS,"CMSRootURL",value); }
		}

		/// <summary>
		/// Returns true, if module is enabled
		/// </summary>
		/// <param name="moduleName"></param>
		/// <returns></returns>
		public bool isModuleEnabled(string moduleName) {
			return _loaded ? getModuleAttribute(moduleName, "enabled") == "1" : false; 
		}

		/// <summary>
		/// Sets the enabled-state of the specified module.
		/// </summary>
		/// <param name="moduleName">Name of the module</param>
		/// <param name="isEnabled">true if module is enabled, otherwise false</param>
		public void setModuleEnabled(string moduleName, bool isEnabled) {
			if (isEnabled)
				setModuleAttribute(moduleName, "enabled", "1");
			else
				removeModule(moduleName);
		}

		/// <summary>
		/// Removes module entry from the xml, used during installation to avoid exposure of existing modules.
		/// </summary>
		/// <param name="moduleName"></param>
		public void removeModule(string moduleName) {
			foreach (XmlNode n in _modules) {
				if (n.Attributes["name"].Value == moduleName) {
					n.ParentNode.RemoveChild(n);
					break;
				}
			}
		}

		/// <summary>
		/// Returns value of specific module-attribute
		/// </summary>
		/// <param name="moduleName">module-name</param>
		/// <param name="attributeName">attribute name</param>
		/// <returns>attribute-value</returns>
		public string getModuleAttribute(string moduleName, string attributeName) {

			foreach (XmlNode n in _modules) {
				if (n.Attributes["name"].Value == moduleName) {
					XmlAttribute a = n.Attributes[attributeName];
					if (a != null) return a.Value;
					break;
				}
			}

			return "";
		}

		public string getModuleParam(string moduleName, string paramName, string dflt) {
			foreach (XmlNode n in _modules) {
				if (n.Attributes["name"].Value == moduleName) {
					foreach (XmlNode p in ((XmlElement)n).GetElementsByTagName("param")) {
						if (p.Attributes["name"].Value == paramName) return p.InnerText;
					}
				}
			}

			return dflt;
		}

		public string getCommonSetting(string paramName, string dflt)
		{
			foreach (XmlNode p in ((XmlElement) _commonsettings).GetElementsByTagName("param"))
			{
				if (p.Attributes["name"].Value == paramName) return p.InnerText;
			}

			return dflt;
		}

		/// <summary>
		/// Set the value of a specific module-attribute
		/// </summary>
		/// <param name="moduleName">module-name</param>
		/// <param name="attributeName">attribute name</param>
		/// <param name="attributeValue">the new value</param>
		public void setModuleAttribute(string moduleName, string attributeName, string attributeValue) {
			foreach (XmlNode n in _modules) {
				if (n.Attributes["name"].Value == moduleName) {
					XmlAttribute a = n.Attributes[attributeName];
					if (a != null) a.Value = attributeValue;
					break;
				}
			}
		}

		/// <summary>
		/// returns an array of modulenames
		/// </summary>
		/// <returns></returns>
		public string [] getModuleNames() {
			ArrayList arr = new ArrayList();

			foreach (XmlNode n in _modules) {
				arr.Add(n.Attributes["name"].Value);
			}

			return (string[]) arr.ToArray(typeof(string));
		}
		/// <summary>
		/// Get property from DB
		/// </summary>
		/// <param name="group">Groupname</param>
		/// <param name="name">Propertyname</param>
		/// <param name="persinId">&gt;0: owner ID</param>
		/// <returns>Propertyvalue</returns>
		public int getIntProperty (string group, string name, long persId) {
			return getIntProperty(group, name, persId, 0);
		}

		/// <summary>
		/// Get property from DB with default-value
		/// </summary>
		/// <param name="group">Groupname</param>
		/// <param name="name">Propertyname</param>
		/// <param name="persId">&gt;0: owner ID</param>
		/// <param name="defaultValue">default value</param>
		/// <returns>Propertyvalue or default-value, if not found</returns>
		public int getIntProperty (string group, string name, long persId, int defaultValue) {
			DBData db = DBData.getDBData(this);
			string sql = "select wert from property where gruppe='"+group+"' and title='"+name+"'";
			
			if (persId > 0)
				sql = sql + " and owner=" + persId;
			else
				sql = sql + " and owner is null"; // Optimizer verwendet Index
			
			db.connect();
			try {
				DataTable table = db.getDataTable(sql);
				if (table.Rows.Count > 0)
					return ch.psoft.Util.Validate.GetValid(table.Rows[0]["WERT"].ToString(), defaultValue);
			}
			catch (Exception e) {
				Logger.Log(e,Logger.ERROR);
			}
			finally {
				db.disconnect();
			}
			return defaultValue;
		}

		/// <summary>
		/// Get property from DB
		/// </summary>
		/// <param name="group">Groupname</param>
		/// <param name="name">Propertyname</param>
		/// <param name="persinId">&gt;0: owner ID</param>
		/// <returns>Propertyvalue</returns>
		public string getStringProperty (string group, string name, long persId) {
			return getStringProperty(group, name, persId, "");
		}

		/// <summary>
		/// Get property from DB with default-value
		/// </summary>
		/// <param name="group">Groupname</param>
		/// <param name="name">Propertyname</param>
		/// <param name="persId">&gt;0: owner ID</param>
		/// <param name="defaultValue">default value</param>
		/// <returns>Propertyvalue or default-value, if not found</returns>
		public string getStringProperty (string group, string name, long persId, string defaultValue) {
			DBData db = DBData.getDBData(this);
			string sql = "select wert from property where gruppe='"+group+"' and title='"+name+"'";
			
			if (persId > 0)
				sql = sql + " and owner=" + persId;
			else
				sql = sql + " and owner is null"; // Optimizer verwendet Index
			
			db.connect();
			try {
				DataTable table = db.getDataTable(sql);
				if (table.Rows.Count > 0)
					return ch.psoft.Util.Validate.GetValid(table.Rows[0]["WERT"].ToString(), defaultValue);
			}
			catch (Exception e) {
				Logger.Log(e,Logger.ERROR);
			}
			finally {
				db.disconnect();
			}
			return defaultValue;
		}


		public void setProperty(string group, string name, long persId, string wert) {
			DBData db = DBData.getDBData(this);
			string sql = "";
			if (getStringProperty(group, name, persId).Equals("")) {
				sql = "insert into property (owner, gruppe, title, wert) values (" + ((persId > 0)? persId.ToString() : "null") + ",'" + group + "','" + name + "','" + wert + "')";
			}
			else {
				sql = "update property set wert='" + wert + "' where gruppe='"+group+"' and title='"+name+"'";
				if (persId > 0)
					sql += " and owner=" + persId;
				else
					sql += " and owner is null";
			}
			
			db.connect();
			try {
				db.execute(sql);
			}
			catch (Exception e) {
				Logger.Log(e,Logger.ERROR);
			}
			finally {
				db.disconnect();
			}
		}
		private XmlNode childNode(XmlNode node, string name) {
			foreach (XmlNode n in node.ChildNodes) {
				if (n.Name == name) return n;
			}
			return null;
		}
	}
}
