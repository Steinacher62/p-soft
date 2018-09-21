namespace ch.appl.psoft.Document.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for DokAblageEdit.
    /// </summary>
    public partial class DokAblageEdit : PSOFTMapperUserControl {
        public const string PARAM_TABLE_NAME = "PARAM_TABLE_NAME";
        public const string PARAM_OWNERTTABLE = "PARAM_OWNERTTABLE";
        public const string PARAM_REGISTRY_ENABLE = "PARAM_REGISTRY_ENABLE";
        public const string PARAM_XID = "PARAM_XID";
        public const string PARAM_FOLDER_ID = "PARAM_FOLDER_ID";
        public const string PARAM_CLIPBOARD_ID = "PARAM_CLIPBOARD_ID";
        public const string PARAM_BACKURL = "PARAM_BACKURL";

        protected Config _config = null;
        protected InputMaskBuilder _inputMask;
        protected DBData _db = null;
        protected DataTable _table;
        private ArrayList _registryList = new ArrayList();
        private ArrayList _parentRegistryList = new ArrayList();

        protected System.Web.UI.HtmlControls.HtmlInputFile inputFileName = new System.Web.UI.HtmlControls.HtmlInputFile();

        public static string Path {
            get {return Global.Config.baseURL + "/Document/Controls/DokAblageEdit.ascx";}
        }

		#region Properities
        public string TableName {
            get {return GetString(PARAM_TABLE_NAME);}
            set {SetParam(PARAM_TABLE_NAME, value);}
        }
        public string BackURL {
            get {return GetString(PARAM_BACKURL);}
            set {SetParam(PARAM_BACKURL, value);}
        }
		
        public bool RegistryEnable {
            get {return GetBool(PARAM_REGISTRY_ENABLE);}
            set {SetParam(PARAM_REGISTRY_ENABLE, value);}
        }

        public long XID {
            get {return GetLong(PARAM_XID);}
            set {SetParam(PARAM_XID, value);}
        }

        public long ClipboardID {
            get {return GetLong(PARAM_CLIPBOARD_ID);}
            set {SetParam(PARAM_CLIPBOARD_ID, value);}
        }

        public long FolderID {
            get {return GetLong(PARAM_FOLDER_ID);}
            set {SetParam(PARAM_FOLDER_ID, value);}
        }

        public string TargetFileState {
            get{ return targetFileState.ClientID;}
        }

        public string TargetFileName {
            get{ return targetFileName.ClientID;}
        }

        public string OwnerTable {
            get {return GetString(PARAM_OWNERTTABLE);}
            set {SetParam(PARAM_OWNERTTABLE, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            string sql = "";
            string view = "";

            _inputMask = new InputMaskBuilder(InputMaskBuilder.InputType.Edit, Session);
            _config = Global.Config;
            _db = DBData.getDBData(Session);

            try {
                if (!IsPostBack) {
                    apply.Text = _mapper.get("apply");                   
                }

                _db.connect();

                string folderFK;
                if (TableName == "DOCUMENT") {
                    sql = "select * from DOCUMENT where ID=" + XID;
                    _table = _db.getDataTableExt(sql, "DOCUMENT");
                    folderFK = "FOLDER_ID";
                    if (DBData.getNumberOfRows(_table) > 0) {
                        view = ((Document.DocType) DBColumn.GetValid(_table.Rows[0]["TYP"],(int) Document.DocType.Document)).ToString().ToUpper();
                    }
                }
                else if (TableName == "FOLDER") {
                    sql = "select * from FOLDER where ID=" + XID;
                    _table = _db.getDataTableExt(sql, "FOLDER");
                    folderFK = "PARENT_ID";
                }
                else return;

                _inputMask.addRow += new AddPropertyHandler(addRow);
                _table.Columns["NUMOFDOCVERSIONS"].ExtendedProperties["In"] = _mapper.getEnum("folder","documentVersions");
                _table.Columns["NUMOFDOCVERSIONS"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                _registryList = new ArrayList(_db.Registry.getRegistryIDs(XID.ToString(), TableName).Split(','));
                if (_table.Rows.Count > 0 && !SQLColumn.IsNull(_table.Rows[0][folderFK])) {
                    long folderID = (long) _table.Rows[0][folderFK];
                    string path = SQLDB.BuildSQLArray(_db.Folder.getParentFolderIDList(folderID, true).ToArray());

                    _parentRegistryList = new ArrayList(_db.Registry.getRegistryIDs(path, "FOLDER").Split(','));
                }
                _inputMask.load(_db,_table,documentTab,_mapper,view);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected string buildTree {
            get {
                if (RegistryEnable) {
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

        private bool extendNode(HttpResponse response, string nodeName, DataRow row, int level) {
            if (_registryList.Contains(row["ID"].ToString())) {
                response.Write(nodeName+".prependHTML=\"<input type='checkbox' checked ID='RegFlag-"+row["ID"]+"'>\";\n");
                response.Write(nodeName+".setInitialOpen(1);\n");
            }
            else if (_parentRegistryList.Contains(row["ID"].ToString())) {
                response.Write(nodeName+".prependHTML = \"<input type='checkbox' checked disabled ID='RegFlag-"+row["ID"]+"'>\";\n");
                response.Write(nodeName+".setInitialOpen(1);\n");
            }
            else response.Write(nodeName+".prependHTML = \"<input type='checkbox' ID='RegFlag"+row["ID"]+"'>\";\n");
            return true;
        }
		
        private void addRow(DataRow row, DataColumn col, TableRow r) {
            if (col.ColumnName == "FILENAME") {
                switch ((Document.DocType) DBColumn.GetValid(row["TYP"],(int) Document.DocType.Document)) {
                case Document.DocType.Document:
                    TableCell cell = r.Cells[1];
                    r.Cells[0].RowSpan = 2;
                
                    cell.Controls.Clear();
                    cell.ColumnSpan = 3;
                    this.inputFileName.Style.Add("width","430px");
                    this.inputFileName.Attributes.Add("onclick","clearInput()");
                    this.inputFileName.ID = "fileSelector";
                    this.inputFileName.Attributes.Add("Class", "Inputmask_Value");
                    cell.Controls.Add(this.inputFileName);
                
                    r = new TableRow();
                    this.documentTab.Rows.Add(r);
                    cell = new TableCell();
                    cell.Text = "<input id=\"clearFile\" class=\"button\" type=\"button\" align=\"top\" onclick=\"clearInput()\" value=\""+_mapper.get("deleteInput")+"\">";
                    cell.VerticalAlign = VerticalAlign.Top;
                    cell.HorizontalAlign = HorizontalAlign.Left;
                    r.Cells.Add(cell);

                    if (!IsPostBack && !DBColumn.IsNull(_table.Rows[0]["XFILENAME"])) {
                        targetXFileName.Text = ch.psoft.Util.Validate.GetValid((string)_table.Rows[0]["XFILENAME"]);
                        targetFileName.Text = ch.psoft.Util.Validate.GetValid((string)_table.Rows[0]["FILENAME"]);
                    }
                    break;
                default:
                    break;
                }
            }
            else if (col.ColumnName == "NUMOFDOCVERSIONS") {
                DropDownCtrl dd = (DropDownCtrl) r.Cells[1].Controls[0];
                int maxV = _db.Folder.getNumOfVersions(DBColumn.GetValid(_db.lookup(_table.TableName == "FOLDER" ? "PARENT_ID" : "FOLDER_ID",_table.TableName,"ID="+XID),0L));

                dd.Items[0].Text += ": "+(maxV >= 0 ? maxV.ToString() : _mapper.getToken("folder","documentVersions",maxV.ToString()));
            }
        }

        private void mapControls() {
            apply.Click += new System.EventHandler(apply_Click);
            apply.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";
        }

        private void apply_Click(object sender, System.EventArgs e) {
            string file = targetFileName.Text;
            string selFile = this.inputFileName.PostedFile != null ? ch.psoft.Util.Validate.GetValid(this.inputFileName.PostedFile.FileName) : "";
            string s;
            StringBuilder sql;
            string xDir = _config.documentSaveDirectory;
            string xFile = targetXFileName.Text;
            bool clearFile = false;
            
            if (!_inputMask.checkInputValue(_table,documentTab,_mapper))
                return;

            _db.connect();
            try {
                _db.beginTransaction();
                sql = _inputMask.getSql(_table,documentTab,true);
                if (targetFileState.Text == "StartStore") {
                    throw new System.Runtime.InteropServices.ExternalException("Error in upload !");
                }
                if (targetFileState.Text == "ClearFile" ||
                    (targetFileState.Text == "EndStore" && file != "") ||
                    selFile != "") {   
                    // delete old   
                    string tmpXFile = xFile;   
                               
                    _db.Document.moveToHistory(XID);
                    if (xDir != "") tmpXFile = xDir+"\\"+xFile;
                    try {
                        File.Delete(tmpXFile);
                    }
                    catch (Exception ex) {
                        Logger.Log(ex,Logger.WARNING);
                    }
                    clearFile = true;
                }
                if (targetFileState.Text == "EndStore" && file != "") {
                    // upload from drag&drop
                    string tmpFile = file;
                    string tmpXFile;
                    string dir = _config.documentTempDirectory;
                               
                    xFile = _db.Document.EncodeXFileName(XID, file);
                    tmpXFile = xFile;
                    if (dir != "") tmpFile = dir+"\\"+file;
                    if (xDir != "") tmpXFile = xDir+"\\"+tmpXFile;
                    // delete temp
                    if (File.Exists(tmpFile))
                        File.Copy(tmpFile,tmpXFile,true);
                    else
                        throw new FileNotFoundException("File '"+tmpFile+"' not found!");

                    try {
                        File.Delete(tmpFile);
                    }
                    catch (Exception ex) {
                        Logger.Log(ex,Logger.WARNING);
                    }
                    clearFile = false;
                }
                else if (selFile != "") {   
                    // upload from selector                     
                    string tmpXFile;
                    
                    file = System.IO.Path.GetFileName(selFile);
                    xFile = _db.Document.EncodeXFileName(XID, file);
                    tmpXFile = xFile;
                    if (xDir != "") tmpXFile = xDir+"\\"+tmpXFile;
                    this.inputFileName.PostedFile.SaveAs(tmpXFile);
                    clearFile = false;
                }
                if (clearFile) {
                    _inputMask.extendSql(sql,_table,"FILENAME",null);
                    _inputMask.extendSql(sql,_table,"XFILENAME",null);
                }
                else if (file != "") {
                    _inputMask.extendSql(sql,_table,"FILENAME",file);
                    _inputMask.extendSql(sql,_table,"XFILENAME",xFile);
                }
                s = _inputMask.endExtendSql(sql);
                if (s.Length > 0) {
                    _db.executeProcedure("MODIFYTABLEROW",
                        new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                        new ParameterCtx("USERID",_db.userId),
                        new ParameterCtx("TABLENAME",TableName),
                        new ParameterCtx("ROWID",XID),
                        new ParameterCtx("MODIFY",s,ParameterDirection.Input,typeof(string),true),
                        new ParameterCtx("INHERIT",1)
                        );

                }
                // registry
                if (registryFlags.Value != "") {                    
                    _db.Registry.updateRegistryEntries(registryFlags.Value, TableName, XID);
                }     
                _db.commit();
                if (BackURL == "") this.Response.Redirect(psoft.Document.Clipboard.GetURL("ID",ClipboardID, "selectedFolderID",FolderID, "ownerTable",OwnerTable), false);
                else this.Response.Redirect(BackURL, false);
            }
            catch (Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();   
            }
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            mapControls();
        }
		
        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
		#endregion
    }
}
