namespace ch.appl.psoft.Document.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for DokAblageCHeckIn.
    /// </summary>
    public partial class DokAblageCheckIn : PSOFTMapperUserControl {
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
            get {return Global.Config.baseURL + "/Document/Controls/DokAblageCheckIn.ascx";}
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

                _inputMask.load(_db,_table,documentTab,_mapper,view);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }
		
        private void addRow(DataRow row, DataColumn col, TableRow r) {
            if (col.ColumnName == "FILENAME")
            {
                switch ((Document.DocType)DBColumn.GetValid(row["TYP"], (int)Document.DocType.Document))
                {
                    case Document.DocType.Document:
                        TableCell cell = r.Cells[1];
                        r.Cells[0].RowSpan = 2;

                        cell.Controls.Clear();
                        cell.ColumnSpan = 3;
                        this.inputFileName.Style.Add("width", "430px");
                        this.inputFileName.ID = "fileSelector";
                        this.inputFileName.Attributes.Add("Class", "Inputmask_Value");
                        cell.Controls.Add(this.inputFileName);

                        if (!IsPostBack && !DBColumn.IsNull(_table.Rows[0]["XFILENAME"]))
                        {
                            targetXFileName.Text = ch.psoft.Util.Validate.GetValid((string)_table.Rows[0]["XFILENAME"]);
                            targetFileName.Text = ch.psoft.Util.Validate.GetValid((string)_table.Rows[0]["FILENAME"]);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                documentTab.Rows.Remove(r);
            }
        }

        private void mapControls() {
            apply.Click += new System.EventHandler(apply_Click);
        }

        private void apply_Click(object sender, System.EventArgs e) {
            string file = targetFileName.Text;
            string selFile = this.inputFileName.PostedFile != null ? ch.psoft.Util.Validate.GetValid(this.inputFileName.PostedFile.FileName) : "";
            string s;
            StringBuilder sql;
            string xDir = _config.documentTempDirectory;
            string xFile = targetXFileName.Text;
            bool clearFile = false;

            _db.connect();
            try {
                if (selFile != "") {   
                    // upload from selector                     
                    string tmpXFile;
                    
                    file = System.IO.Path.GetFileName(selFile);
                    xFile = _db.Document.EncodeXFileName(XID, file);
                    tmpXFile = xFile;
                    if (xDir != "") tmpXFile = xDir+"\\"+tmpXFile;
                    this.inputFileName.PostedFile.SaveAs(tmpXFile);
                    clearFile = false;
                    _db.Document.checkIn(XID);
                }

                if (BackURL == "") this.Response.Redirect(psoft.Document.Detail.GetURL("xID",XID, "clipboardID", ClipboardID, "selectedFolderID", FolderID, "table", "DOCUMENT"), false);
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
