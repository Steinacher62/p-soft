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
    ///		Summary description for DokAblageAdd.
    /// </summary>
    public partial class DokAblageAdd : PSOFTMapperUserControl
	{
        public const string PARAM_TABLE_NAME = "PARAM_TABLE_NAME";
        public const string PARAM_OWNERTTABLE = "PARAM_OWNERTTABLE";
        public const string PARAM_DOCUMENT_ADD_ENABLE = "PARAM_DOCUMENT_ADD_ENABLE";
		public const string PARAM_REGISTRY_ENABLE = "PARAM_REGISTRY_ENABLE";
        public const string PARAM_KONTEXT = "PARAM_KONTEXT";
        public const string PARAM_XID = "PARAM_XID";
        public const string PARAM_TRIGGER_UID = "PARAM_TRIGGER_UID";
        public const string PARAM_CLIPBOARD_ID = "PARAM_CLIPBOARD_ID";
        public const string PARAM_BACKURL = "PARAM_BACKURL";
        public const string PARAM_DOCUMENT_TYPE = "PARAM_DOCUMENT_TYPE";

		private Config _config = null;
		private InputMaskBuilder _inputMask;
		private DataTable _table;
		private DBData _db;
		private ArrayList _parentRegistryList = new ArrayList();

		protected System.Web.UI.HtmlControls.HtmlInputFile inputFileName = new System.Web.UI.HtmlControls.HtmlInputFile();


		public static string Path
		{
			get {return Global.Config.baseURL + "/Document/Controls/DokAblageAdd.ascx";}
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

        /// <summary>
        /// Get/Set documenttype: Document.DocType
        /// </summary>
        public Document.DocType DocumentType {
            get {return (Document.DocType) GetInt(PARAM_DOCUMENT_TYPE);}
            set {SetParam(PARAM_DOCUMENT_TYPE, (int) value);}
        }
		
		public bool DocumentAddEnable
		{
			get {return GetBool(PARAM_DOCUMENT_ADD_ENABLE);}
			set {SetParam(PARAM_DOCUMENT_ADD_ENABLE, value);}
		}

		public bool RegistryEnable
		{
			get {return GetBool(PARAM_REGISTRY_ENABLE);}
			set {SetParam(PARAM_REGISTRY_ENABLE, value);}
		}

        public string Kontext {
            get {return GetString(PARAM_KONTEXT);}
            set {SetParam(PARAM_KONTEXT, value);}
        }

        public long XID {
            get {return GetLong(PARAM_XID);}
            set {SetParam(PARAM_XID, value);}
        }

        public long TriggerUID {
            get {return GetLong(PARAM_TRIGGER_UID);}
            set {SetParam(PARAM_TRIGGER_UID, value);}
        }

        public long ClipboardID
        {
            get {return GetLong(PARAM_CLIPBOARD_ID);}
            set {SetParam(PARAM_CLIPBOARD_ID, value);}
        }

        public string TargetFileState
        {
            get{ return targetFileState.ClientID;}
        }

        public string TargetFileName
        {
            get{ return targetFileName.ClientID;}
        }

        public string OwnerTable
        {
            get {return GetString(PARAM_OWNERTTABLE);}
            set {SetParam(PARAM_OWNERTTABLE, value);}
        }
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}

		protected override void DoExecute()
		{
			base.DoExecute ();
			string sql = "";
            string view = "";
			_inputMask = new InputMaskBuilder(InputMaskBuilder.InputType.Add, Session);
            
			_db = DBData.getDBData(Session);
			_config = Global.Config;

			registryCell.Visible = RegistryEnable;

			try 
			{
				apply.Text = _mapper.get("apply");                   

				_db.connect();
                if (TableName == "DOCUMENT") {
                    sql = "select * from DOCUMENT where id = -1";
                    _table = _db.getDataTableExt(sql, "DOCUMENT");
                    view = DocumentType.ToString().ToUpper();
                }
                else if (TableName == "FOLDER") {
                    sql = "select * from FOLDER where id = -1";
                    _table = _db.getDataTableExt(sql,"FOLDER");
                }
                else return;

                _inputMask.addRow += new AddPropertyHandler(addRow);
                _table.Columns["NUMOFDOCVERSIONS"].ExtendedProperties["In"] = _mapper.getEnum("folder","documentVersions");
                _table.Columns["NUMOFDOCVERSIONS"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                if (Kontext == "clipboard") {
                    string path = SQLDB.BuildSQLArray(_db.Folder.getParentFolderIDList(XID, true).ToArray());
                    _parentRegistryList = new ArrayList(_db.Registry.getRegistryIDs(path, "FOLDER").Split(','));
                }

				_inputMask.load(_db,_table,documentTab,_mapper,view);

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
			if (_parentRegistryList.Contains(row["ID"].ToString())) 
			{
				response.Write(nodeName+".prependHTML = \"<input type='checkbox' checked disabled ID='RegFlag-"+row["ID"]+"'>\";\n");
				response.Write(nodeName+".setInitialOpen(1);\n");
			}
			else response.Write(nodeName+".prependHTML = \"<input type='checkbox' ID='RegFlag"+row["ID"]+"'>\";\n");
			return true;
		}

        private void addRow(DataRow row, DataColumn col, TableRow r) {

            switch (col.ColumnName)
            {
                case "FILENAME":
                    TableCell cell = r.Cells[1];
                    cell.Controls.Clear();
                    this.inputFileName.Style.Add("width","430px");
                    this.inputFileName.ID = "fileSelector";
                    this.inputFileName.Attributes.Add("Class", "InputMask_Value");
                    this.inputFileName.EnableViewState = false;
                    cell.Controls.Add(this.inputFileName);
                    break;
                case "NUMOFDOCVERSIONS":
                    DropDownCtrl dd = (DropDownCtrl) r.Cells[1].Controls[0];
                    int maxV = _db.Folder.getNumOfVersions(DBColumn.GetValid(_db.lookup(_table.TableName == "FOLDER" ? "PARENT_ID" : "ID","FOLDER","ID="+XID),0L));
                    dd.Items[0].Text += ": "+(maxV >= 0 ? maxV.ToString() : _mapper.getToken("folder","documentVersions",maxV.ToString()));
                    break;
            }
        }

		private void mapControls () 
		{
			apply.Click += new System.EventHandler(apply_Click);
			apply.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";
		}

		private void apply_Click(object sender, System.EventArgs e) 
		{
            string selFile = this.inputFileName.PostedFile != null ? ch.psoft.Util.Validate.GetValid(this.inputFileName.PostedFile.FileName) : "";
            bool err = false;
            
            if (selFile == "")
            {
                this.inputFileName.Attributes.Clear();
                this.inputFileName.Attributes.Add("Class", "InputMask_Error");
                err = true;
                if (this._table.TableName == "FOLDER")
                {
                    err = false;
                }
            }
            
            if (!_inputMask.checkInputValue(_table, documentTab, _mapper) || err){
                return;
            }

			string file = targetFileName.Text;
            string s;
			StringBuilder sql = null;
			long id;
			string xDir = _config.documentSaveDirectory;
			string xFile = "";
           
           

			_db.connect();
			try 
			{
                Document doc = _db.Document;
				_db.beginTransaction();
				sql = _inputMask.getSql(_table, documentTab,true);
				id = _db.newId(_table.TableName);
                
				_inputMask.extendSql(sql,_table,"ID",id);
                if (TriggerUID > 0){
                    _inputMask.extendSql(sql,_table,"TRIGGER_UID", TriggerUID);
                }
                switch(_table.TableName){
                    case "FOLDER":
                        _inputMask.extendSql(sql,_table,"PARENT_ID", XID);
                        long rootID = ch.psoft.Util.Validate.GetValid(_db.lookup("ROOT_ID", "FOLDER", "ID=" + XID, false), -1L);
                        if (rootID > 0L){
                            _inputMask.extendSql(sql,_table,"ROOT_ID", rootID);
                        }
                        break;

                    case "DOCUMENT":
                        switch(Kontext){
                            case "clipboard":
                                _inputMask.extendSql(sql,_table,"FOLDER_ID", XID);
                                break;

                            case "knowledge":
                                _inputMask.extendSql(sql,_table,"KNOWLEDGE_ID", XID);
                                break;
                        }
                        break;
                }

                if (targetFileState.Text == "StartStore") 
				{
					throw new System.Runtime.InteropServices.ExternalException("Error in upload !");
				}
                
				if (file != "") 
				{
					xFile = doc.EncodeXFileName(id,file);
					string dir = _config.documentTempDirectory;
					string tmpFile = file,tmpXFile = xFile;
                    
					if (dir != "") tmpFile = dir+"\\"+file;
					if (xDir != "") tmpXFile = xDir+"\\"+xFile;
                    
					Logger.Log("Copy '"+tmpFile+"' to '"+tmpXFile+"'",Logger.DEBUG);
					if (File.Exists(tmpFile))
						File.Copy(tmpFile,tmpXFile);
					else
						throw new FileNotFoundException("File '"+tmpFile+"' not found!");
                    
					try 
					{
						File.Delete(tmpFile);
					}
					catch (Exception ex) 
					{
						Logger.Log(ex,Logger.WARNING);
					}
				}
				else if (selFile != "") 
				{
					string tmpXFile;
                        
					file = System.IO.Path.GetFileName(selFile);
					xFile = _db.Document.EncodeXFileName(id,file);
					tmpXFile = xFile;
					if (xDir != "") tmpXFile = xDir+"\\"+tmpXFile;
					this.inputFileName.PostedFile.SaveAs(tmpXFile);
				}
				if (file != "") 
				{
					_inputMask.extendSql(sql,_table,"FILENAME",file);
                    _inputMask.extendSql(sql,_table,"XFILENAME",xFile);
                    _inputMask.extendSql(sql,_table,"CHECKIN_PERSON_ID",SessionData.getUserID(Session));
                }
                _inputMask.extendSql(sql,_table,"TYP",(int)DocumentType);
                s = _inputMask.endExtendSql(sql);
                if (s.Length > 0) {
                    _db.executeProcedure("MODIFYTABLEROW",
                        new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                        new ParameterCtx("USERID",_db.userId),
                        new ParameterCtx("TABLENAME",TableName),
                        new ParameterCtx("ROWID",id),
                        new ParameterCtx("MODIFY",s,ParameterDirection.Input,typeof(string),true),
                        new ParameterCtx("INHERIT",1)
                        );
                }

				// registry
				if (registryFlags.Value != "") 
				{
                    _db.Registry.updateRegistryEntries(registryFlags.Value, TableName, id);
				}

                _db.grantRowAuthorisation(DBData.AUTHORISATION.RUDI, _db.userAccessorID, _table.TableName, id);

				_db.commit();
                if (BackURL == "") this.Response.Redirect(psoft.Document.Clipboard.GetURL("id",ClipboardID, "selectedFolderID",XID, "ownerTable",OwnerTable), false);
                else this.Response.Redirect(BackURL, false);
            }
			catch (Exception ex) 
			{
				_db.rollback();
                DoOnException(ex);
            }
			finally 
			{
				_db.disconnect();   
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
