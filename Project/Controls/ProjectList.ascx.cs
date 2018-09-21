namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class ProjectList : PSOFTSearchListUserControl {
        public const string MODE_SEARCHRESULT = "searchresult";

        public const string CONTEXT_PERSON = "person";
        public const string CONTEXT_SEARCHRESULT = "searchresult";
        public const string CONTEXT_PROJECTGROUP = "projectgroup";
        public const string CONTEXT_EXPORT_PROJECT_OVERVIEW = "exportProjectOverview";


        public const string PARAM_SQL = "PARAM_SQL";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_RELOAD = "PARAM_RELOAD";
        public const string PARAM_PROJECT_ID = "PARAM_PROJECT_ID";
        public const string PARAM_X_ID = "PARAM_X_ID";
        public const string PARAM_MODE = "PARAM_MODE";
        public const string PARAM_KONTEXT = "PARAM_KONTEXT";

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        protected DBData _db = null;
        protected ArrayList _states;
        private string _postDeleteURL;

        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/ProjectList.ascx";}
        }

        public ProjectList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            Mode = "";
        }

		#region Properities
        public long ProjectID {
            get {return GetLong(PARAM_PROJECT_ID);}
            set {SetParam(PARAM_PROJECT_ID, value);}
        }

        public long xID {
            get {return GetLong(PARAM_X_ID);}
            set {SetParam(PARAM_X_ID, value);}
        }

        public string Mode {
            get {return GetString(PARAM_MODE);}
            set {SetParam(PARAM_MODE, value);}
        }

        public string NextURL {
            get { return GetString(PARAM_NEXT_URL); }
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public string Kontext {
            get {return GetString(PARAM_KONTEXT);}
            set {SetParam(PARAM_KONTEXT, value);}
        }

        public bool Reload {
            get {return GetBool(PARAM_RELOAD);}
            set {SetParam(PARAM_RELOAD, value);}
        }

        public string SearchSQL {
            get {return GetString(PARAM_SQL);}
            set {SetParam(PARAM_SQL, value);}
        }

        public string PostDeleteURL {
            get {return _postDeleteURL;}
            set {_postDeleteURL = value;}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();

            loadList();
        }

        protected void loadList(){
			CBShowInactive.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_SHOWINACTIVEPROJS);
			CBShowInactive.Checked = SessionData.showInactiveProjects(Session);

            _states = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PROJECT);

            listTab.Rows.Clear();

            _db = DBData.getDBData(Session);
            _db.connect();
            try {
                bool load = true;
                string sql = "select * from PROJECT";

                switch (Mode){
                    case MODE_SEARCHRESULT:
                        sql = SearchSQL;
                        load = Reload;
                        pageTitle.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CT_PROJECT_SEARCHRESULT);
                        next.Text = _mapper.get("next");
                        ButtonRow.Visible = true;
                        CheckBoxEnabled = true;
						CBShowInactive.Visible = false;
                        break;
                        
                    default:
                        switch (Kontext){
                            case CONTEXT_PERSON:
                                string involvedProjects = _db.Project.getInvolvedProjects(xID);
                                if (involvedProjects != ""){
                                    sql += " where ID in (" + involvedProjects + ")";
                                }
                                else {
                                    sql += " where ID=-1";
                                }
                                pageTitle.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CT_INVOLVED_PROJECTS).Replace("#1", _db.Person.getWholeName(xID));
                                break;

                            case CONTEXT_PROJECTGROUP:
                                sql += " where ID in (select PROJECT_ID from PROJECT_GROUP_PROJECT where PROJECT_GROUP_ID=" + xID + ")";
                                pageTitle.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CT_PROJECT_GROUP).Replace("#1", _db.lookup("TITLE", "PROJECT_GROUP", "ID=" + xID, false));
                                break;

                            case CONTEXT_SEARCHRESULT:
                                sql += " where ID in (select ROW_ID from SEARCHRESULT where ID=" + xID + ")";
                                pageTitle.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CT_PROJECT_SELECTION);
                                break;
                        }
						CBShowInactive.Visible = true;
                        break;
                }

                if (load){

					if (!SessionData.showInactiveProjects(Session))
					{
						if (sql.ToLower().IndexOf("where",0) > 0)
							sql += " and";
						else
							sql += " where";
						sql += " state in (0,1,3)";
					}

                    sql += " order by " + OrderColumn + " " + OrderDir;

                    DataTable table = _db.getDataTableExt(sql, "PROJECT");
					
                    IDColumn = "ID";
                    if (ProjectID > 0)
                        HighlightRecordID = ProjectID;

					table.Columns["PROJECT_TYPE_ID"].ExtendedProperties["In"] = _db.Project.getProjectTypes();
                    table.Columns["STATE"].ExtendedProperties["In"] = _states;

                    CheckOrder = true;
                    LoadList(_db, table, listTab);
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (col != null){
                switch(col.ColumnName) {
                    case "STATE":
                        if (Mode != MODE_SEARCHRESULT){
                            // create dropdown
                            cell.Controls.Clear();
                            DropDownList dd = new DropDownCtrl();
                            cell.Controls.Add(dd);
							for (int i=0; i < _states.Count; i++)
							{
								dd.Items.Add(new ListItem(_states[i] as string, i.ToString()));
							}
                            dd.SelectedIndex = int.Parse(row[col].ToString());
                            dd.ID = "DDSTATE" + row["ID"].ToString();
                            dd.SelectedIndexChanged += new EventHandler(ddState_SelectedIndexChanged);
                            dd.AutoPostBack = true;
                        }

                        // append open-counter
                        TableCell c = new TableCell();
                        c.CssClass = "List";
                        r.Cells.Add(c);
                        long ID = DBColumn.GetValid(row["ID"],0L);
                        c.Text = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PHASE)[0] + "(" + _db.Project.getOpenPhasesCount(ID, true) + ")";

                        // append semaphore...
                        c = new TableCell();
                        r.Cells.Add(c);
                        System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                        int semaphore = _db.Project.getSemaphore(ID, true);
                        switch (semaphore) {
                            case 0:
                                image.ImageUrl = "../../images/ampelRot.gif";
                                break;
                            case 1:
                                image.ImageUrl = "../../images/ampelOrange.gif";
                                break;
                            case 2:
                                image.ImageUrl = "../../images/ampelGruen.gif";
                                break;
                            case 3:
                                image.ImageUrl = "../../images/ampelGrau.gif";
                                break;
							case 4:
								image.ImageUrl = "../../images/ampelBlau.gif";
								break;
                        }
                        int criticalDays = DBColumn.GetValid(row["CRITICALDAYS"], 1);
                        image.ToolTip = ProjectModule.getSemaphoreProjectComment(Session, semaphore, criticalDays);

                        c.Controls.Add(image);
                        break;
                }
            }
        }

        private void ddState_SelectedIndexChanged(object sender, System.EventArgs e) {
            if (sender is DropDownList) {
                DropDownList dd = (sender as DropDownList);
                try {
                    int projectID = int.Parse(dd.ID.Substring(7));
                    _db.connect();
                    _db.execute("update PROJECT set STATE=" + dd.SelectedItem.Value + " where ID=" + projectID);
                }
                catch (Exception ex) {
                    DoOnException(ex);
                }
                finally {
                    _db.disconnect();
                }
                loadList();
            }
        }

        /// <summary>
        /// Event handler for the 'next' button
        /// The selected item(s) database ID will be stored in the SEARCHRESULT table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void next_Click(object sender, System.EventArgs e) {
            long searchResultID = SaveInSearchResult(listTab, "PROJECT");

            NextURL = NextURL.Replace("%25SearchResultID","%SearchResultID").Replace("%SearchResultID", searchResultID.ToString());

            _nextArgs.LoadUrl = NextURL;
            DoOnNextClick(next);
        }

        private void mapControls () {
            this.next.Click += new System.EventHandler(this.next_Click);
			this.CBShowInactive.CheckedChanged += new System.EventHandler(this.CBShowInactive_CheckedChanged);
        }

		private void CBShowInactive_CheckedChanged(object sender, System.EventArgs e) 
		{
			SessionData.setShowInactiveProjects(Session, CBShowInactive.Checked);
			loadList();
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
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}
