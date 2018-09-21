namespace ch.appl.psoft.Document.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for DokAblageCreate.
    /// </summary>
    public partial class DokAblageCreate : PSOFTInputViewUserControl
	{
        public const string PARAM_OWNERTTABLE = "PARAM_OWNERTTABLE";
        public const string PARAM_REGISTRY_ENABLE = "PARAM_REGISTRY_ENABLE";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_OWNER_ID = "PARAM_OWNER_ID";
        public const string PARAM_CLIPBOARD_TYPE = "PARAM_CLIPBOARD_TYPE";
        public const string PARAM_ACCESSOR_ID = "PARAM_ACCESSOR_ID";
        public const string PARAM_TRIGGER_UID = "PARAM_TRIGGER_UID";

        protected Config _config = null;
		protected DBData _db = null;
		protected DataTable _table;
		private ArrayList _registryList = new ArrayList();


        protected DropDownList _templateChooser = null;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Document/Controls/DokAblageCreate.ascx";}
		}

		#region Properities
        public string NextUrl 
        {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }
		public bool RegistryEnable
		{
			get {return GetBool(PARAM_REGISTRY_ENABLE);}
			set {SetParam(PARAM_REGISTRY_ENABLE, value);}
		}

        public string OwnerTable
        {
            get {return GetString(PARAM_OWNERTTABLE);}
            set {SetParam(PARAM_OWNERTTABLE, value);}
        }
        public long OwnerID
        {
            get {return GetLong(PARAM_OWNER_ID);}
            set {SetParam(PARAM_OWNER_ID, value);}
        }

        public int ClipboardType
        {
            get {return GetInt(PARAM_CLIPBOARD_TYPE);}
            set {SetParam(PARAM_CLIPBOARD_TYPE, value);}
        }

        public long AccessorID
        {
            get {return GetLong(PARAM_ACCESSOR_ID);}
            set {SetParam(PARAM_ACCESSOR_ID, value);}
        }

        public long TriggerUID{
            get {return GetLong(PARAM_TRIGGER_UID);}
            set {SetParam(PARAM_TRIGGER_UID, value);}
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

            _config = Global.Config;
            _db = DBData.getDBData(Session);

            try 
            {
                if (!IsPostBack) 
                {
                    apply.Text = _mapper.get("apply");                   
                }

                _db.connect();


                if (OwnerTable != "" && OwnerID > 0)
                {
                    sql = "select * from CLIPBOARDTEMPLATE_FOLDERV where ID=-1";
                    _table = _db.getDataTableExt(sql, "CLIPBOARDTEMPLATE_FOLDERV");
                    DataTable templateTable = _db.getDataTableExt(
							"select ID, TITLE from CLIPBOARD where template=1",
							"CLIPBOARD"
						);
                    _table.Columns["TEMPLATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    _table.Columns["TEMPLATE"].ExtendedProperties["In"] = templateTable;
                    _table.Columns["TEMPLATE"].ExtendedProperties["Nullable"] = true;
                }
                else return;

				base.LoadInput(_db,_table,addTab);
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
			if (_registryList.Contains(row["ID"].ToString())) 
			{
				// kein "-" bei RegFlag!
				response.Write(
					nodeName + ".prependHTML=\"<input type='checkbox' checked ID='RegFlag"
					+ row["ID"] + "'>\";\n"
				);
				response.Write(nodeName+".setInitialOpen(1);\n");
			}
			else response.Write(nodeName+".prependHTML = \"<input type='checkbox' ID='RegFlag"+row["ID"]+"'>\";\n");
			return true;
		}
		

        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) 
        {
            if (col != null && col.ColumnName == "TEMPLATE") 
            {
                Control ctrl = r.Cells[1].Controls[0];

                if (ctrl is DropDownList) 
                {
                    _templateChooser = ctrl as DropDownList;
					_templateChooser.EnableViewState = true;
                    _templateChooser.AutoPostBack = true;
                    _templateChooser.SelectedIndexChanged += new System.EventHandler(this.templateSelectionChanged);
                }
            }
        }

        private void mapControls() {
            apply.Click += new System.EventHandler(apply_Click);
            apply.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";
		}

        private void templateSelectionChanged(object sender, System.EventArgs e) 
        {
            string id = Validate.GetValid(_templateChooser.SelectedItem.Value,"0");

            _db.connect();

            try 
            {
                string sql = "select f.* from CLIPBOARD c inner join FOLDER f on c.FOLDER_ID = f.ID and c.ID=" + id;
                DataTable table = _db.getDataTable(sql);

				if (table.Rows.Count == 0)
				{
					foreach (TableRow r in addTab.Rows) 
					{
						TableCell c = r.Cells[1];
						Control ctrl = c.Controls[0];

						if (ctrl is TextBox) 
						{
							((TextBox) ctrl).Text = "";
						}
					}

					_registryList = new ArrayList();
				}
				else
				{
					foreach (TableRow r in addTab.Rows) 
					{
						try 
						{
							TableCell c = r.Cells[1];
							Control ctrl = c.Controls[0];

							if (ctrl is TextBox) 
							{
								string name = ctrl.ID;
								int idx = name.IndexOf("-",6);
								if (idx >= 0) 
								{
									name = name.Substring(idx+1);
									string textValue = _db.dbColumn.GetDisplayValue(table.Columns[name],table.Rows[0][name],false);
									((TextBox) ctrl).Text = textValue;
								}
							}
						}
						catch {}
					}

					_registryList = new ArrayList(
							_db.Registry.getRegistryIDs(table.Rows[0]["ID"].ToString(), "FOLDER").Split(',')
						);
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

        private void apply_Click(object sender, System.EventArgs e) 
        {
            if (!base.checkInputValue(_table,addTab))
                return;
            
            long newID = -1;
			long templateId = Validate.GetValid(_templateChooser.SelectedItem.Value, (long)0);
            _db.connect();

            try 
            {
                _db.beginTransaction();
                StringBuilder sb = base.getSql(_table, addTab, true);

                string titleIn = SQLColumn.toSql(base.getInputValue(_table,addTab,"TITLE").ToString());
                string descrIn = SQLColumn.toSql(base.getInputValue(_table,addTab,"DESCRIPTION").ToString());
                                    
                string folderName = titleIn == "" ? _mapper.get("clipboard") : titleIn;
                string clipboardName = _mapper.get("clipboard") + " " + OwnerTable + "_" + OwnerID;   
                    
                if (templateId > 0) 
                {
                    newID = _db.Clipboard.copy(
							templateId,
							true,
							AccessorID,
							TriggerUID,
							false,
							ClipboardType,
							false // Registry-Einträge aus Eingabe
						); // id = clipboard = folder
                    // update clipboard and folder
                    _db.execute("update CLIPBOARD set TITLE='"+clipboardName+"' where ID="+newID);
                    _db.execute("update FOLDER set TITLE='"+folderName+"'"+ (descrIn == "" ? "" : ", DESCRIPTION='"+descrIn+"'")+" where ID="+newID);

                }
                else
                {
                    newID = _db.newId("FOLDER");
                    _db.execute("insert into FOLDER (ID, ROOT_ID, TRIGGER_UID, TITLE"+ (descrIn == "" ? "" : ", DESCRIPTION") +") values(" + newID + ", " + newID + ", " + (TriggerUID>0? TriggerUID.ToString() : "null") + ", '" + folderName + "'" + (descrIn == "" ? "" : ", '" + descrIn + "'") + ")");
                    _db.execute("insert into CLIPBOARD (ID, FOLDER_ID, TRIGGER_UID, TITLE, TYP) values(" + newID + ", " + newID + ", " + (TriggerUID>0? TriggerUID.ToString() : "null") + ", '" + clipboardName + "', " + ClipboardType + ")");
                    // Setting the access-rights on the clipboard. Default: the currently logged user.
                    if (AccessorID > 0)
                    {
                        _db.grantRowAuthorisation(DBData.AUTHORISATION.FULL_ACCESS, AccessorID, "CLIPBOARD", newID);
                    }

                }

                //assign owner...
                if (OwnerTable != "" && OwnerID > 0)
                {
                    _db.execute("update " + OwnerTable + " set CLIPBOARD_ID=" + newID + " where ID=" + OwnerID);
                    //copy rights from owner...
                    _db.copyRowAuthorisations(OwnerTable, OwnerID, "CLIPBOARD", newID);
                }
                
                // registry
                if (registryFlags.Value != "") 
                {                    
                    _db.Registry.updateRegistryEntries(registryFlags.Value, "FOLDER", newID);
                }  

                _db.commit();

                
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

            if (NextUrl == "") 
            {
                Response.Redirect(ch.appl.psoft.Document.Clipboard.GetURL("ID",newID, "ownerTable",OwnerTable));
            }
            else
            {
                Response.Redirect(NextUrl);
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
