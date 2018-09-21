namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;

    public partial class ProjectSearchCtrl : PSOFTSearchUserControl {

        private DBData _db = null;
        private DataTable _table;
        private ArrayList _selectedRegistry = new ArrayList();

        protected System.Web.UI.WebControls.Table Table1;
       

        protected DropDownCtrl _ddPersons;

        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/ProjectSearchCtrl.ascx";}
        }

		#region Properities
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected string buildTree {
            get {
                Common.Tree tree = new Common.Tree("REGISTRY", Response, "");
                Response.Write("<script language=\"javascript\">\n");
                tree.extendNode += new ExtendNodeHandler(this.extendNode);
                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                tree.BranchToolTipColum = "DESCRIPTION";
                tree.build(_db,  _db.Registry.getRootID(), "registryTree");
                return "</script>";
            }
        }
        
        private bool extendNode(HttpResponse response, string nodeName, DataRow row, int level) {
            string ID = row["ID"].ToString();
            if (nodeName != "registryTree") {
                if (_selectedRegistry.Contains(ID)) {
                    response.Write(nodeName+".prependHTML=\"<input type='checkbox' checked ID='RegFlag" + ID + "'>\";\n");
                    response.Write(nodeName+".setInitialOpen(1);\n");
                }
                else{
                    response.Write(nodeName+".prependHTML = \"<input type='checkbox' ID='RegFlag" + ID + "'>\";\n");
                }
            }
            return true;
        }

        protected override void DoExecute() {
            base.DoExecute();
            _db = DBData.getDBData(Session);
            _db.connect();
            try {
                if (!IsPostBack) {
                    Session["ProjectSQLSearch"] = null;
                    apply.Text = _mapper.get("search");
                    semaphore.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_SEMAPHORE);
                    rbAlle.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_SEMAPHORE_ALL);
                    rbAlle.Checked = true;
                    imRed.ToolTip = ProjectModule.getSemaphoreProjectComment(Session, 0, -1);
                    imOrange.ToolTip = ProjectModule.getSemaphoreProjectComment(Session, 1, -1);
                    imGreen.ToolTip = ProjectModule.getSemaphoreProjectComment(Session, 2, -1);
                    imDone.ToolTip = ProjectModule.getSemaphoreProjectComment(Session, 3, -1);
					imBlue.ToolTip = ProjectModule.getSemaphoreProjectComment(Session, 4, -1);
                    tdDone.Attributes.Add("onclick",rbDone.ClientID+".checked=true");
                    tdGreen.Attributes.Add("onclick",rbGreen.ClientID+".checked=true");
                    tdOrange.Attributes.Add("onclick",rbOrange.ClientID+".checked=true");
                    tdRed.Attributes.Add("onclick",rbRed.ClientID+".checked=true");
					tdBlue.Attributes.Add("onclick",rbBlue.ClientID+".checked=true");
                    CBShowInactive.Checked = SessionData.showInactiveProjects(Session);
                    CBShowInactive.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_SHOWINACTIVEPROJS);
                    registryRelation.Items.Add(new ListItem(_mapper.get("or"),"or"));
                    registryRelation.Items.Add(new ListItem(_mapper.get("and"),"and"));
                    registryRelation.ToolTip = _mapper.get("registry","registryRelationTP");
                    registryRelation.SelectedIndex = 0;
                }
                if (ViewState["ProjectSearchSelectedRegistry"] != null){
                    _selectedRegistry =  ViewState["ProjectSearchSelectedRegistry"] as ArrayList;
                }
                else{
                    ViewState.Add("ProjectSearchSelectedRegistry", _selectedRegistry);
                }

                _table = _db.getDataTableExt("select * from PROJECT where ID=-1", "PROJECT");

                _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["STATE"].ExtendedProperties["In"] = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PROJECT);
				_table.Columns["PROJECT_TYPE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
				_table.Columns["PROJECT_TYPE_ID"].ExtendedProperties["In"] = _db.Project.getProjectTypes();
				
                CheckOrder = true;
                LoadInput(_db, _table, searchTab);

                // DropDown for leaders...
                TableRow r = new TableRow();
                searchTab.Rows.Add(r);
                TableCell c = new TableCell();
                c.Text = _mapper.get("project", "projectLeaders");
                c.CssClass = "InputMask_Label";
                r.Cells.Add(c);
                r.Cells.Add(new TableCell());
                c = new TableCell();
                r.Cells.Add(c);
                _ddPersons = new DropDownCtrl();
                _ddPersons.ID = "PERSON_ID";
                c.Controls.Add(_ddPersons);
                if (!IsPostBack) {
                    _ddPersons.Items.Add("");
                    DataTable leaderPersons = _db.Project.getLeaderPersons();
                    foreach (DataRow row in leaderPersons.Rows) {
                        _ddPersons.Items.Add(new ListItem(row[1].ToString(),row[0].ToString()));
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

        private void mapControls () {
            apply.Click += new System.EventHandler(apply_Click);
            apply.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";
            CBShowInactive.CheckedChanged += new System.EventHandler(this.CBShowInactive_CheckedChanged);
        }

        private string sqlAppendWhere(string sql, string clause) {
            sql += ((sql.ToLower().IndexOf(" where ") > 0) ? " and " : " where ") + clause;
            return sql;
        }

        private void apply_Click(object sender, System.EventArgs e) {
            if (checkInputValue(_table, searchTab)){
                StringBuilder sqlEx = getSql(_table, searchTab, true);
                extendSql(sqlEx, _table, "TEMPLATE", 0);
                string sql = endExtendSql(sqlEx);
                sql += " and PARENT_ID is null";
                
                DBData db = DBData.getDBData(Session);
                db.connect();
                try{
                    string inStr = "-";
                    if (rbRed.Checked) {
                        inStr = db.Project.getRootProjectsBySemaphore(0);
                    }
                    else if (rbOrange.Checked) {
                        inStr = db.Project.getRootProjectsBySemaphore(1);
                    }
                    else if (rbGreen.Checked) {
                        inStr = db.Project.getRootProjectsBySemaphore(2);
                    }
                    else if (rbDone.Checked) {
                        inStr = db.Project.getRootProjectsBySemaphore(3);
                    }
					else if (rbBlue.Checked) 
					{
						inStr = db.Project.getRootProjectsBySemaphore(4);
					}

                    if (inStr != "-") {
                        if (inStr == ""){
                            inStr = "null";
                        }
                        sql += " and id in (" + inStr + ")";
                    }

                    //leader...
                    long leaderPersonID = ch.psoft.Util.Validate.GetValid(_ddPersons.SelectedItem.Value, -1L);
                    if (leaderPersonID > 0){
                        sql += " and LEADER_ORGENTITY_ID in (select ORGENTITY_ID from JOB_PERS_FUNC_V where JOB_TYP=1 and PERSON_ID=" + leaderPersonID + ")";
                    }

                    //registry...
                    string[] regIds = registryFlags.Value.Split(',');
                    _selectedRegistry.Clear();
                    if (registryFlags.Value != "") { // nur falls Checkboxen angeklickt wurden wird die Registratur berücksichtigt.
                        // falls über die Registratur nichts gefunden wird, soll die Liste leer bleiben
                        if (registryRelation.SelectedItem.Value == "or" || regIds.Length <= 1) {
                            sql += " and ID in (" + _db.Registry.getRegisteredIDs(registryFlags.Value, "PROJECT", -1) + ")";
                        }
                        else {
                            foreach (string regId in regIds) {
                                string registered = _db.Registry.getRegisteredIDs(regId, "PROJECT", -1);
                                sql += " and ID in (" + registered + ")";
                                if (registered == "-1"){
                                    break;
                                }
                            }
                        }

                        foreach (string regId in regIds){
                            _selectedRegistry.Add(regId);
                        }
                    }
                }
                catch (Exception ex) {
                    DoOnException(ex);
                }
                finally {
                    db.disconnect();
                }

                Session["ProjectSQLSearch"] = sql;

                // Setting search event args
                _searchArgs.ReloadList = true;
                _searchArgs.SearchSQL = sql;

                DoOnSearchClick(apply);
            }
        }

        private void CBShowInactive_CheckedChanged(object sender, System.EventArgs e) {
            SessionData.setShowInactiveProjects(Session, CBShowInactive.Checked);

            if (!CBShowInactive.Checked && rbDone.Checked) {
                rbDone.Checked = false;
                rbAlle.Checked = true;
            }

            if (Session["ProjectSQLSearch"] != null) {
                _searchArgs.ReloadList = true;
                _searchArgs.SearchSQL = Session["ProjectSQLSearch"] as string;
                DoOnSearchClick(apply);
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {    
            ID = "Search";

        }
		#endregion
    }
}