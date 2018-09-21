namespace ch.appl.psoft.Project.Controls
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
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;


    public partial class ProjectAddCtrl : PSOFTInputViewUserControl {
        public const string PARAM_PARENT_ID = "PARAM_PARENT_ID";

        protected DataTable _table;
        protected DropDownList _templateChooser = null;
        protected DBData _db = null;
        private ArrayList _parentRegistryList = new ArrayList();
        private ArrayList _registryList = new ArrayList();

        protected System.Web.UI.HtmlControls.HtmlInputFile inputFileName = new System.Web.UI.HtmlControls.HtmlInputFile();

        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/ProjectAddCtrl.ascx";}
        }

		#region Properities
        public long ParentID {
            get {return GetLong(PARAM_PARENT_ID);}
            set {SetParam(PARAM_PARENT_ID, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _db = DBData.getDBData(Session);
            string sql = "select * from PROJECT where ID=-1";

            _db.connect();
            try {
                if (!IsPostBack) {
                    apply.Text = _mapper.get("apply");
                }
                _table = _db.getDataTableExt(sql,"PROJECT");

                DataTable templateTable = _db.Project.getTemplatesTable();
                _table.Columns["TEMPLATE_PROJECT_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["TEMPLATE_PROJECT_ID"].ExtendedProperties["In"] = templateTable;
				_table.Columns["PROJECT_TYPE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
				_table.Columns["PROJECT_TYPE_ID"].ExtendedProperties["In"] = _db.Project.getProjectTypes();

                if (ParentID <= 0) {
                    _table.Columns["PARENT_ID"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
                }
                else {
                    _table.Columns["PARENT_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                }

                _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["STATE"].ExtendedProperties["In"] = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PROJECT);

                if (Global.Config.getModuleParam("project", "enableMainObjectiveField", "0").Equals("1"))
                {
                    //enabled
                    _table.Columns["IS_MAIN_OBJECTIVE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.ADD;
                    _table.Columns["IS_MAIN_OBJECTIVE"].ExtendedProperties["InputControlType"] = typeof(CheckBox);
                }


                LoadInput(_db,_table,addTab);

                string path = SQLDB.BuildSQLArray(_db.Project.getParentProjectIDList(ParentID, true).ToArray());
                _parentRegistryList = new ArrayList(_db.Registry.getRegistryIDs(path, "PROJECT").Split(','));
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) {
            if (col != null){
				Control ctrl = null;
                switch(col.ColumnName){
                    case "TEMPLATE_PROJECT_ID":
                        ctrl = r.Cells[1].Controls[0];

                        if (ctrl is DropDownList) {
                            _templateChooser = ctrl as DropDownList;
							_templateChooser.EnableViewState = true;
                            _templateChooser.AutoPostBack = true;
                            _templateChooser.SelectedIndexChanged += new System.EventHandler(templateSelectionChanged);
                        }
                        break;

                    case "PARENT_ID":
                        Label lbl = (Label) r.Cells[1].Controls[0];
                        if (lbl != null){
                            lbl.Text = _db.lookup("TITLE", "PROJECT", "ID=" + ParentID, true);
                        }
                        break;

                    case "CRITICALDAYS":
                        TextBox tx = (TextBox) r.Cells[1].Controls[0];
                        tx.Text = Global.Config.getModuleParam("project","criticalDays","5");
                        break;

					case "STATE":
						ctrl = r.Cells[1].Controls[0];
						
						if (ctrl is DropDownList) 
						{
							((DropDownList) ctrl).SelectedIndex = ProjectModule.INDEX_PROJECT_STATE_START;
							((DropDownList) ctrl).Enabled = false;
						}
						break;
                }
            }
        }

        protected string buildTree {
            get {
                Common.Tree tree = new Common.Tree("REGISTRY", Response, "");
                tree.extendNode += new ExtendNodeHandler(this.extendNode);
                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                tree.BranchToolTipColum = "DESCRIPTION";
                Response.Write("<script language=\"javascript\">\n");
            
                tree.build(_db, _db.Registry.getRootID(), "registryTree");
            
                return "</script>";
            }
        }

        private bool extendNode(HttpResponse response, string nodeName, DataRow row, int level) {
            if (_parentRegistryList.Contains(row["ID"].ToString())) {
                response.Write(nodeName+".prependHTML = \"<input type='checkbox' checked disabled ID='RegFlag-"+row["ID"]+"'>\";\n");
                response.Write(nodeName+".setInitialOpen(1);\n");
            }
            else if (_registryList.Contains(row["ID"].ToString())) {
                // kein "-" bei RegFlag!
                response.Write(nodeName + ".prependHTML=\"<input type='checkbox' checked ID='RegFlag"+ row["ID"] + "'>\";\n");
                response.Write(nodeName+".setInitialOpen(1);\n");
            }
            else response.Write(nodeName+".prependHTML = \"<input type='checkbox' ID='RegFlag"+row["ID"]+"'>\";\n");
            return true;
        }

        private void mapControls () {
            apply.Click += new System.EventHandler(apply_Click);
            apply.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";
        }


        private void templateSelectionChanged(object sender, System.EventArgs e) {
            string id = Validate.GetValid(_templateChooser.SelectedItem.Value,"0");

            _db.connect();

            try {
                string sql = "select * from PROJECT where ID=" + id;
                DataTable table = _db.getDataTableExt(sql, "PROJECT");

				if (table.Rows.Count == 0)
				{
                    _registryList = new ArrayList();
                    
                    foreach (TableRow r in addTab.Rows) 
					{
						TableCell c = r.Cells[1];
						Control ctrl = c.Controls[0];

						if (ctrl is TextBox) 
						{
							string name = ctrl.ID;
							int idx = name.IndexOf("-", 6);

							if (idx >= 0  && name.Substring(idx + 1) == "CRITICALDAYS") 
							{
								((TextBox) ctrl).Text 
									= Global.Config.getModuleParam("project", "criticalDays", "5");
							}
							else
							{
								((TextBox) ctrl).Text = "";
							}
						}
						else if (ctrl is DropDownList)
						{
							string name = ctrl.ID;
							int idx = name.IndexOf("-", 6);
							if (idx >= 0) 
							{
								name = name.Substring(idx+1);
								if (name == "STATE")
								{
									if (((DropDownList) ctrl).Items.Count > ProjectModule.INDEX_PROJECT_STATE_START - 1)
										((DropDownList) ctrl).SelectedIndex = ProjectModule.INDEX_PROJECT_STATE_START;
								}
								else 
								{
									((DropDownList) ctrl).SelectedIndex = 0;
								}
							}
							
						}
					}
				}
				else
				{
                    _registryList = new ArrayList(_db.Registry.getRegistryIDs(id.ToString(), "PROJECT").Split(','));
                    foreach (TableRow r in addTab.Rows) {
						try {
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
							else if (ctrl is DropDownList)
							{							
								string name = ctrl.ID;
								int idx = name.IndexOf("-",6);
								if (idx >= 0) 
								{
									name = name.Substring(idx+1);
									if (name == "PROJECT_TYPE_ID")
									{
										string itemId = _db.dbColumn.GetDisplayValue(table.Columns[name],table.Rows[0][name],false);
										DataTable itemTable = _db.Project.getProjectTypes();
										int itemIdx = 0;
										int itemCnt = 0;
										foreach (DataRow row in itemTable.Rows) 
										{
											itemCnt++;
											if (row["ID"].ToString() == itemId)
												itemIdx = itemCnt;
										}
										((DropDownList) ctrl).SelectedIndex = itemIdx;
									}
									else if (name == "STATE")
									{
										if (((DropDownList) ctrl).Items.Count > ProjectModule.INDEX_PROJECT_STATE_START - 1)
											((DropDownList) ctrl).SelectedIndex = ProjectModule.INDEX_PROJECT_STATE_START;
									}
									else if (name != "TEMPLATE_PROJECT_ID")
									{
										((DropDownList) ctrl).SelectedIndex = 0;
									}
								}
							}
                            else if (ctrl is CheckBox)
                            {
                                string name = ctrl.ID;
                                int idx = name.IndexOf("-", 6);
                                if (idx >= 0)
                                {
                                    name = name.Substring(idx + 1);
                                    if (name == "IS_MAIN_OBJECTIVE")
                                    {
                                        string itemId = _db.dbColumn.GetDisplayValue(table.Columns[name], table.Rows[0][name], false);
                                        ((CheckBox)ctrl).Checked = (itemId == "1");
                                    }
                                }
                            }
						}
						catch {}
					}
				}                
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();   
            }
        }

        private void apply_Click(object sender, System.EventArgs e) {
            if (!base.checkInputValue(_table,addTab)){
                return;
            }
            
            long newID = -1;
            long templateId = ch.psoft.Util.Validate.GetValid(_templateChooser.SelectedItem.Value,0L);

            _db.connect();
            try {
                _db.beginTransaction();
                StringBuilder sb = getSql(_table, addTab, true);
                newID = _db.newId(_table.TableName);

                extendSql(sb, _table, "ID", newID);
                extendSql(sb, _table, "TEMPLATE", "0");

                if (ParentID <= 0){
                    extendSql(sb, _table, "ROOT_ID", newID);
                }
                else {
                    extendSql(sb, _table, "PARENT_ID", ParentID);
                    extendSql(sb, _table, "ROOT_ID", _db.lookup("ROOT_ID","PROJECT","ID=" + ParentID));
                }

                string sql = endExtendSql(sb);

                if (sql != "") {
                    _db.execute(sql);

                    if (templateId > 0){
                        newID = _db.Project.copy(templateId, newID, ParentID, true, true, false, true, false, true);
                    }

                    //create default organisation structure / access-rights / organigram...
                    _db.Project.createProjectOrganisation(newID, _db.userId);

                    // registry
                    if (registryFlags.Value != "") {
                        _db.Registry.updateRegistryEntries(registryFlags.Value, _table.TableName, newID);
                    }
                    
                    _db.Project.createMessage(ParentID, (int)News.ACTION.EDIT, true);

                    _db.commit();

                    //we need to refresh the AccessorIDsCacheEntry in _db, otherwise the following redirect can not be correctly authorised!!!
                    _db.refreshAccessorIDsCacheEntry(_db.userAccessorID);

                    Response.Redirect(psoft.Project.ProjectDetail.GetURL("ID",newID), false);
                }
                else
                    _db.rollback();
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
