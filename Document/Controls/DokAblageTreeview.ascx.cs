namespace ch.appl.psoft.Document.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for DokAblageTreeview.
    /// </summary>
    public partial class DokAblageTreeview : PSOFTListViewUserControl
	{
		public const string PARAM_CLIPBOARD_ID = "PARAM_CLIPBOARD_ID";
        public const string PARAM_OWNERTTABLE = "PARAM_OWNERTTABLE";
        public const string PARAM_DOCUMENT_ADD_ENABLE = "PARAM_DOCUMENT_ADD_ENABLE";
		public const string PARAM_REGISTRY_ENABLE = "PARAM_REGISTRY_ENABLE";
		public const string PARAM_SELECTED_FOLDER_ID = "PARAM_SELECTED_FOLDER_ID";
        public const string PARAM_ACTIVEX_ERROR_TOOLTIP = "PARAM_ACTIVEX_ERROR_TOOLTIP";




		protected Config _config = null;
        protected long _highlightId = 0;
        protected DBData _db = null;

        public const string PSOFTLOCKFILE = "<PSOFT:lockFile runat=\"server\" id=\"LockFile1\"/>";

		public static string Path
		{
			get {return Global.Config.baseURL + "/Document/Controls/DokAblageTreeview.ascx";}
		}

		#region Properities
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

        public long ClipboardID
        {
            get {return GetLong(PARAM_CLIPBOARD_ID);}
            set {SetParam(PARAM_CLIPBOARD_ID, value);}
        }

        public long SelectedFolderID
        {
            get {return GetLong(PARAM_SELECTED_FOLDER_ID);}
            set {
                SetParam(PARAM_SELECTED_FOLDER_ID, value);
                _highlightId = value;
            }
        }

        public string NewDocumentPath
		{
			get
			{
                return psoft.Document.Add.GetURL("table","DOCUMENT", "XID",SelectedFolderID, "clipboardID",ClipboardID, "registryEnable",RegistryEnable, "ownerTable",OwnerTable);
			}
		}

        public string NewDocumentLinkPath {
            get {
                return psoft.Document.DocumentSearch.GetURL("includeLinks","false", "nextURL",psoft.Document.AddDocumentLinks.GetURL("searchResultID","%SearchResultID", "folderID",SelectedFolderID, "nextURL",psoft.Document.Clipboard.GetURL("id",ClipboardID, "selectedFolderID",SelectedFolderID, "ownerTable",OwnerTable)));
            }
        }

        public string NewFolderPath
		{
			get
			{
                return psoft.Document.Add.GetURL("table","FOLDER", "XID",SelectedFolderID, "clipboardID",ClipboardID, "registryEnable",RegistryEnable, "ownerTable",OwnerTable);
            }
		}

		public string NewExchangeFolderPath
		{
			get
			{
                return psoft.Document.Add.GetURL("table", "EXCHANGE_FOLDER", "XID", SelectedFolderID, "clipboardID", ClipboardID, "registryEnable", RegistryEnable, "ownerTable", OwnerTable);
            }
		}

        public string EditFolderPath {
            get {
                return psoft.Document.Edit.GetURL("registryEnable",RegistryEnable, "xID",SelectedFolderID, "ClipboardID",ClipboardID, "Table","FOLDER", "selectedFolderID",SelectedFolderID);
            }
        }

        public string OwnerTable
        {
            get {return GetString(PARAM_OWNERTTABLE);}
            set {SetParam(PARAM_OWNERTTABLE, value);}
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
            _config = Global.Config;

			ShelfTreeTitle.Text = _mapper.get("person","shelfTreeTitle");
            
            _db = DBData.getDBData(Session);
            
            try
			{
                _db.connect();
				//newDocument.Visible = DocumentAddEnable;

				if (SelectedFolderID >= 0)
					Session["DocumentFolderId"] = SelectedFolderID;
				else
					SelectedFolderID = Session["DocumentFolderId"] == null ? 0 : (int) Session["DocumentFolderId"];

				if (!IsPostBack)
				{
					ShelfListTitle.Text = _mapper.get("person","shelfListTitle");

                    if (SelectedFolderID <= 0)
                        SelectedFolderID = ch.psoft.Util.Validate.GetValid(_db.lookup("FOLDER_ID", "CLIPBOARD", "ID=" + ClipboardID, false), 0);
                    
					if (SelectedFolderID > 0)
					{
						string _sql = "select * from FolderDocumentV where FOLDER_ID=" + SelectedFolderID;

						// make sure folders are displayed first...
						if (_sql.IndexOf("order") < 0)
                            _sql += " order by TYPE " + (OrderDir=="asc"? "desc" : "asc") + "," + OrderColumn + " " + OrderDir;

						DataTable table = _db.getDataTableExt(_sql,"FOLDERDOCUMENTV");
                        table.Columns["TRIGGER_UID"].ExtendedProperties["In"] = this;
                        table.Columns["TRIGGER_UID"].ExtendedProperties["ContextLink"] = psoft.Goto.GetURL("uid","%TRIGGER_UID");
                        DeleteEnabled = true;
						EditEnabled = true;
                        DetailEnabled = true;
						EditURL = psoft.Document.Edit.GetURL("registryEnable",RegistryEnable, "xID","%ID", "ClipboardID",ClipboardID, "Table","%TYPE", "selectedFolderID",SelectedFolderID);
						DetailURL = psoft.Document.Detail.GetURL("registryEnable",RegistryEnable, "xID","%ID", "ClipboardID",ClipboardID, "Table","%TYPE", "selectedFolderID",SelectedFolderID);
						SortURL = psoft.Document.Clipboard.GetURL("id",ClipboardID, "selectedFolderID",SelectedFolderID, "ownerTable",OwnerTable);
                        UseJavaScriptToSort = false;
						LoadList(_db, table, documentList);
					}
				}
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
                    case "TRIGGER_UID":
                        retValue = _db.UID2NiceName((long)id,_mapper);
                        break;
                }
            }
            return retValue;
        }

        public string buildTree
		{
			get
			{
				Common.Tree tree = new Common.Tree("FOLDER", Response, psoft.Document.Clipboard.GetURL("id",ClipboardID, "ownerTable",OwnerTable, "selectedFolderID","%ID", "documentAddEnable",DocumentAddEnable, "registryEnable",RegistryEnable));
                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                tree.BranchToolTipColum = "DESCRIPTION";
                DBData db = DBData.getDBData(Session);

				Response.Write("<script language=\"javascript\">\n");
				Session["DocumentTree"] = tree;
				try
				{
					db.connect();
                    string [] roots = {db.lookup("FOLDER_ID", "CLIPBOARD", "ID=" + ClipboardID, false)};
					long [] iRoots = new long[roots.Length];
					string [] rootNames = new string[roots.Length];
					int i;
					for (i=0; i<roots.Length; i++)
					{
						iRoots[i] = long.Parse(roots[i]);
						rootNames[i] = "documentTree" + i;
					}
					tree.build(db,iRoots,rootNames);
				}
				catch (Exception ex)
				{
                    DoOnException(ex);
                }
				finally
				{
					db.disconnect();
				}
				return "</script>";
			}
		}

		protected override void onAddHeaderCell (DataRow row, DataColumn col, TableRow r, TableCell cell)
		{
			if (col != null)
			{
				if (col.ColumnName.Equals("TITLE"))
					cell.Width=200;
				else if (col.ColumnName.Equals("FILENAME"))
					cell.Width=200;
			}
		}

		protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell c)
		{
            if (ListBuilder.IsInfoCell(c)){
				string image = " ondragstart=\"listDragStart()\" ondragend=\"listDragEnd()\"";
                
				if (c.Text.StartsWith("<img"))
				{
                    string ID = ListBuilder.getID(r);
                    string type = row["TYPE"].ToString();
                    if (type == "FOLDER")
                        ListBuilder.ReplaceInfoImage(c, "folder.gif");
                    else{
                        int typ = DBColumn.GetValid(row["TYP"],0);

                        switch (typ) {
                            case (int) Document.DocType.Document:
                                ListBuilder.ReplaceInfoImage(c,"document.gif");
                                break;
                            case (int) Document.DocType.Document_Link:
                                ListBuilder.ReplaceInfoImage(c,"doclink.gif");
                                break;
                            default:
                                break;
                        }
                    }
					image = image+" id=\"" + type + "_" + ID + "\">";
					c.Text = c.Text.Substring(0,c.Text.Length-1)+image;
				}
			}
			if (col != null)
			{
				switch (col.ColumnName)
				{
					case "FILENAME":
						if (ch.psoft.Util.Validate.GetValid(c.Text) != "")
						{
                            HyperLink link = new HyperLink();
							c.Controls.Clear(); 
							c.Controls.Add(link);
							link.CssClass = "List";
							link.Target = "_blank";
							link.NavigateUrl = psoft.Document.GetDocument.GetURL("documentID",row["ID"]);
							link.Text = c.Text;
						}
						break;
				}
			}
		}

		public string backURL
		{
			get { return ch.psoft.Util.Validate.GetValid((string) Session["DocumentBackURL"]); }
		}

		public string backText
		{
			get { return _mapper.get("back"); }
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
