namespace ch.appl.psoft.Document.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web;
    using System.Web.UI.WebControls;


    /// <summary>
    ///		Summary description for DokAblageDetail.
    /// </summary>
    public partial class DokAblageDetail : PSOFTDetailViewUserControl
	{
        public const string PARAM_TABLE_NAME = "PARAM_TABLE_NAME";
        public const string PARAM_REGISTRY_ENABLE = "PARAM_REGISTRY_ENABLE";
        public const string PARAM_XID = "PARAM_XID";
        public const string PARAM_FOLDER_ID = "PARAM_FOLDER_ID";
        public const string PARAM_CLIPBOARD_ID = "PARAM_CLIPBOARD_ID";
        public const string PARAM_SHOW_ERR6_WARNING = "PARAM_SHOW_ERR6_WARNING";
        public const string PARAM_ACTIVEX_ERROR_TOOLTIP = "PARAM_ACTIVEX_ERROR_TOOLTIP";

		protected string _localFileName = "";
		protected string _serverFileName = "";
		protected string _serverTempPath = "";
		protected string _serverSavePath = "";
		protected string _serverName = "";
        protected string _reloadURL = "";
		private DBData _db;
        private DataTable _table = null;
        private ArrayList _registryList = new ArrayList();
        private ArrayList _parentRegistryList = new ArrayList();

		protected System.Web.UI.HtmlControls.HtmlForm detailForm;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Document/Controls/DokAblageDetail.ascx";}
		}

		#region Properities
        public string TableName
        {
            get {return GetString(PARAM_TABLE_NAME);}
            set {SetParam(PARAM_TABLE_NAME, value);}
        }
		
        public bool RegistryEnable
        {
            get {return GetBool(PARAM_REGISTRY_ENABLE);}
            set {SetParam(PARAM_REGISTRY_ENABLE, value);}
        }

        public long XID
        {
            get {return GetLong(PARAM_XID);}
            set {SetParam(PARAM_XID, value);}
        }

        public long ClipboardID
        {
            get {return GetLong(PARAM_CLIPBOARD_ID);}
            set {SetParam(PARAM_CLIPBOARD_ID, value);}
        }

        public long FolderID
        {
            get {return GetLong(PARAM_FOLDER_ID);}
            set {SetParam(PARAM_FOLDER_ID, value);}
        }

        public string ReloadURL{
            get {return _reloadURL;}
            set {_reloadURL = value;}
        }

        public bool ShowErr6Warning {
            get {return GetBool(PARAM_SHOW_ERR6_WARNING);}
            set {SetParam(PARAM_SHOW_ERR6_WARNING, value);}
        }

        public string ActiveXErrorTooltip
        {
            get {return GetString(PARAM_ACTIVEX_ERROR_TOOLTIP);}
            set {SetParam(PARAM_ACTIVEX_ERROR_TOOLTIP, value);}
        }
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute ();
            _db = DBData.getDBData(Session);
			string sql = "";
            
			try 
			{
				_db.connect();
                string folderFK;
                if (TableName == "DOCUMENT")
				{
					Config config = Global.Config;
					_serverTempPath = config.ftpDocumentTempDirectory;
					_serverSavePath = config.ftpDocumentSaveDirectory;
					_serverName = config.ftpDocumentServer;
					sql = "select * from DOCUMENT where ID=" + XID;
                    _table = _db.getDataTableExt(sql,"DOCUMENT");
                    _table.Columns["CHECKOUT_PERSON_ID"].ExtendedProperties["In"] = this;
                    _table.Columns["CHECKOUT_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%CHECKOUT_PERSON_ID", "mode","oe");
                    _table.Columns["CHECKIN_PERSON_ID"].ExtendedProperties["In"] = this;
                    _table.Columns["CHECKIN_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%CHECKIN_PERSON_ID", "mode","oe");
                    _table.Columns["FILENAME"].ExtendedProperties["ContextLink"] = psoft.Document.GetDocument.GetURL("documentID","%ID");
                    _table.Columns["FILENAME"].ExtendedProperties["ContextLinkTarget"] = "_blank";
                    _table.Columns["TRIGGER_UID"].ExtendedProperties["In"] = this;
                    _table.Columns["TRIGGER_UID"].ExtendedProperties["ContextLink"] = psoft.Goto.GetURL("uid","%TRIGGER_UID");
                    if (DBData.getNumberOfRows(_table) > 0) {
                        _localFileName = _table.Rows[0]["FILENAME"].ToString();
                        _serverFileName = _table.Rows[0]["XFILENAME"].ToString();
                        View = ((Document.DocType) DBColumn.GetValid(_table.Rows[0]["TYP"],(int) Document.DocType.Document)).ToString().ToUpper();
                    }
                    folderFK = "FOLDER_ID";
                }
                else if (TableName == "FOLDER") {
					sql = "select * from FOLDER where ID=" + XID;
					_table = _db.getDataTableExt(sql,"FOLDER");
                    _table.Columns["TRIGGER_UID"].ExtendedProperties["In"] = this;
                    _table.Columns["TRIGGER_UID"].ExtendedProperties["ContextLink"] = psoft.Goto.GetURL("uid","%TRIGGER_UID");
                    folderFK = "PARENT_ID";
                }
				else return;

                _registryList = new ArrayList(_db.Registry.getRegistryIDs(XID.ToString(), TableName).Split(','));
                if (_table.Rows.Count > 0 && !SQLColumn.IsNull(_table.Rows[0][folderFK])) 
                {
                    long folderID = (long) _table.Rows[0][folderFK];
                    string path = SQLDB.BuildSQLArray(_db.Folder.getParentFolderIDList(folderID, true).ToArray());

                    _parentRegistryList = new ArrayList(_db.Registry.getRegistryIDs(path, "FOLDER").Split(','));
                }
                
                LoadDetail(_db, _table, documentTab);

			}
			catch (Exception ex) 
			{
                DoOnException(ex);
            }
			finally 
			{
				_db.disconnect();
			}
            
		}


        public string lookup(DataColumn col, object id, bool http) {
            string retValue = "";

            if (col != null && !(id is System.DBNull)) {
                switch (col.ColumnName) {
                    case "CHECKOUT_PERSON_ID":
                        retValue = _db.Person.getWholeName(id.ToString());
                        break;

                    case "CHECKIN_PERSON_ID":
                        retValue = _db.Person.getWholeName(id.ToString());
                        break;

                    case "TRIGGER_UID":
                        retValue = _db.UID2NiceName((long)id,_mapper);
                        break;
                }
            }
            return retValue;
        }

        private static HyperLink createHyperLink(string navigateURL, string text){
            HyperLink link = new HyperLink();
            link.CssClass = "List";
            link.Target = "contentFrame";
            link.NavigateUrl = navigateURL;
            link.Text = text;
            return link;
        }

        /// <summary>
        /// Fügt einer Zelle eine Liste mit Verzeichnis-Links hinzu.
        /// </summary>
        /// <param name="cell">Tabellen-Zelle</param>
        /// <param name="folderID">Verzeichnis ID</param>
        public static void createPathLinks(DBData db, TableCell cell, long folderID){
            bool isFirst = true;
            cell.Controls.Clear();
            if (folderID > 0){
                do {
                    string sql = "select TITLE, PARENT_ID from FOLDER where ID=" + folderID;
                    DataTable table = db.getDataTable(sql);
                    if (table.Rows.Count > 0) {
                        if (isFirst){
                            isFirst = false;
                        }
                        else{
                            Label label = new Label();
                            label.Text = "/";
                            cell.Controls.AddAt(0, label);
                        }
                        cell.Controls.AddAt(0, createHyperLink(psoft.Document.GetFolder.GetURL("id",folderID), DBColumn.GetValid(table.Rows[0]["TITLE"], "")));
                        folderID = DBColumn.GetValid(table.Rows[0]["PARENT_ID"], -1L);
                    }
                } while(folderID > 0);
            }
        }

        protected override void onAddProperty (DataRow row, DataColumn col, TableRow r) {
            if (col != null) {
                switch (col.ColumnName) {
                case "FOLDER_ID":
                    createPathLinks(_db, r.Cells[1], DBColumn.GetValid(row[col], -1L));
                    break;

                case "LINKED_DOCUMENT_ID":
                    long linkedDocumentID = DBColumn.GetValid(row[col], -1L);
                    long folderID = ch.psoft.Util.Validate.GetValid(_db.lookup("FOLDER_ID", "DOCUMENT", "ID=" + linkedDocumentID, false), -1L);
                    TableCell cell = r.Cells[1];
                    createPathLinks(_db, cell, folderID);
                    Label label = new Label();
                    label.Text = "/";
                    cell.Controls.Add(label);
                    cell.Controls.Add(createHyperLink(psoft.Document.Detail.GetURL("xID",linkedDocumentID, "table","DOCUMENT", "registryEnable",RegistryEnable), _db.lookup("TITLE", "DOCUMENT", "ID=" + linkedDocumentID, false)));
                    break;

                case "NUMOFDOCVERSIONS":
                    string t = r.Cells[1].Text;

                    r.Cells[1].Text = _mapper.getToken("folder","documentVersions",t);
                    if (t == "-2") {
                        int maxV = _db.Folder.getNumOfVersions(DBColumn.GetValid(_db.lookup(_table.TableName == "FOLDER" ? "PARENT_ID" : "ID","FOLDER","ID="+(_table.TableName == "FOLDER" ? XID : DBColumn.GetValid(_table.Rows[0]["FOLDER_ID"], -1L))),0L));

                        r.Cells[1].Text += ": "+(maxV >= 0 ? maxV.ToString() : _mapper.getToken("folder","documentVersions",maxV.ToString()));
                    }
                    break;
                }
            }
        }

        protected string buildTree 
        {
            get 
            {
                if (RegistryEnable)
                {
                    Common.Tree tree = new Common.Tree("REGISTRY", Response, "");
                    tree.extendNode += new ExtendNodeHandler(this.extendNode);
                    tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                    tree.BranchToolTipColum = "DESCRIPTION";
                    Response.Write("<script language=\"javascript\">\n");
                
                    tree.build(_db, _db.Registry.getRootID(), "registryTree");
                
                    return "</script>";
                }
                else
                    return "";
            }
        }

        private bool extendNode(HttpResponse response, string nodeName, DataRow row, int level) 
        {
            if (_registryList.Contains(row["ID"].ToString())) 
            {
                response.Write(nodeName+".prependHTML=\"<input type='checkbox' checked disabled ID='RegFlag-"+row["ID"]+"'>\";\n");
                response.Write(nodeName+".setInitialOpen(1);\n");
            }
            else if (_parentRegistryList.Contains(row["ID"].ToString())) 
            {
                response.Write(nodeName+".prependHTML=\"<input type='checkbox' checked disabled ID='RegFlag-"+row["ID"]+"'>\";\n");
                response.Write(nodeName+".setInitialOpen(1);\n");
            }
            else response.Write(nodeName+".prependHTML=\"<input type='checkbox' disabled ID='RegFlag-"+row["ID"]+"'>\";\n");
            return true;
        }
		
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
