namespace ch.appl.psoft.Tasklist.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PendenzenTasklist.
    /// </summary>
    public partial class PendenzenTaskList : PSOFTSearchListUserControl {

        public const string PARAM_CONTEXT = "PARAM_CONTEXT";
        public const string PARAM_XID = "PARAM_XID";
        public const string PARAM_ROOT_ID = "PARAM_ROOT_ID";
        public const string PARAM_SELECTED_ID = "PARAM_SELECTED_ID";
        public const string PARAM_SHOW_CONTACTS = "PARAM_SHOW_CONTACTS";
        public const string PARAM_SORT_ENABLED = "PARAM_SORT_ENABLED";
        public const string PARAM_DELETE_URL = "PARAM_DELETE_URL";
        public const string PARAM_TEMPLATE = "PARAM_TEMPLATE";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";

        private const string _cName = "TasklistList";

        protected int _measureID = -1;
        protected string _tableName = "TASKLIST";
        protected string _redComment;
        protected string _orangeComment;
        protected string _greenComment;
        protected string _doneComment;
        protected string [] _states = null;

        protected DBData _db = null;
        protected DataTable _table;

        

        public static string Path {
            get {return Global.Config.baseURL + "/Tasklist/Controls/PendenzenTaskList.ascx";}
        }

        #region Properities
        public string Kontext {
            get {return GetString(PARAM_CONTEXT);}
            set {SetParam(PARAM_CONTEXT, value);}
        }

        public long XID {
            get {return GetID(PARAM_XID);}
            set {SetParam(PARAM_XID, value);}
        }

        public long RootID {
            get {return GetID(PARAM_ROOT_ID);}
            set {SetParam(PARAM_ROOT_ID, value);}
        }

        public long SelectedID {
            get {return GetID(PARAM_SELECTED_ID);}
            set {SetParam(PARAM_SELECTED_ID,value);}
        }

        public bool ShowContacts {
            get {return GetBool(PARAM_SHOW_CONTACTS);}
            set {SetParam(PARAM_SHOW_CONTACTS, value);}
        }

        public bool SortEnabled {
            get {return GetBool(PARAM_SORT_ENABLED);}
            set {SetParam(PARAM_SORT_ENABLED, value);}
        }

        public string DeleteURL {
            get {return GetString(PARAM_DELETE_URL);}
            set {SetParam(PARAM_DELETE_URL, value);}
        }

        public bool Template
        {
            get {return GetBool(PARAM_TEMPLATE);}
            set {SetParam(PARAM_TEMPLATE, value);}
        }

        public string NextURL {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            if (this.Visible) {
                loadList();
            }
        }

        protected override void DoExecute() {
            loadList();
        }

        private void loadList() {
            deleteMessage = _mapper.get("tasklist","deleteTasklistConfirm");
            _redComment = TaskListModule.getSemaphoreTasklistComment(Session, 0);
            _orangeComment = TaskListModule.getSemaphoreTasklistComment(Session, 1);
            _greenComment = TaskListModule.getSemaphoreTasklistComment(Session, 2);
            _doneComment = TaskListModule.getSemaphoreTasklistComment(Session, 3);

            CBShowDone.Text = _mapper.get("tasklist", "showDoneTasklists");
            CBShowDone.Checked = SessionData.showDoneTasklists(Session);
            next.Text = _mapper.get("next");
            _states = _mapper.getEnum("tasklist", "state", true);

            _db = DBData.getDBData(Session);
            try {
                tasklistList.Rows.Clear();
                string sql = "";
                
                DetailEnabled = true;
                DeleteEnabled = true;
                EditEnabled = true;
                InfoBoxEnabled = true;
                HeaderEnabled = true;
                UseFirstLetterAsPageSelector = true;

                _db.connect();

                switch (Kontext) {
                    case "tasklistgroup":
                        TasklistListTitle.Text = _mapper.get("tasklist", "tasklistgroup").Replace("#1", _db.lookup("TITLE", "TASKLIST_GROUP", "ID=" + XID, false));
                        sql = "select * from TASKLIST where ID in (select TASKLIST_ID from TASKLIST_GROUP_TASKLIST where TASKLIST_GROUP_ID=" + XID + ") and TEMPLATE=0";
                        break;

                    case "search":
                        if (Template)
                        {
						    TasklistListTitle.Text = _mapper.get("tasklist","foundTasklistTemplates");
                        }
                        else
                        {
                            TasklistListTitle.Text = _mapper.get("tasklist","foundTasklists");
                            CheckBoxEnabled = true;
                            next.Visible = true;
                        }

                        sql = Session["TasklistSQLSearch"] as string;
                        CBShowDone.Visible = false;
                        break;
                    case "searchassign":
                        SingleResultRecord = true;
                        CheckBoxEnabled = true;
                        DetailEnabled = false;
                        EditEnabled = false;
                        DeleteEnabled = false;
                        TasklistListTitle.Text = _mapper.get("tasklist","foundTasklists");
                        sql = Session["TasklistSQLSearch"] as string;
                        next.Visible = true;
                        CBShowDone.Visible = false;
                        break;

                    case "selection":
                        CBShowDone.Visible = false;
                        TasklistListTitle.Text = _mapper.get("tasklist","selectedTasklists");
                        if(XID > 0) sql = "select * from TASKLIST where ID in (select ROW_ID from SEARCHRESULT where TABLENAME='TASKLIST' and ID=" + XID + ")";
                        else sql = Session["TasklistSQLSearch"] as string;
                        break;

                    case "measure": // 23.06.06 pvs: keine Verwendung im Code gefunden!
                        TasklistListTitle.Text = _mapper.get("tasklist", "tasklists");
                        sql = "select TASKLIST.* from TASKLIST inner join MEASURE on TASKLIST.ID = MEASURE.TASKLIST_ID where TASKLIST.template=0 and MEASURE.ID="+XID;
                        break;

                    default:
                        TasklistListTitle.Text = _mapper.get("tasklist", "tasklist");
                        break;
                }

                if (sql == "" || sql == null) 
                    sql = "select * from TASKLIST where template=0";
                
                if (!SessionData.showDoneTasklists(Session) && !Template)
                {
                    sql += " and ("
                        +" exists (select 1 from MEASURE where TASKLIST_ID=TASKLIST.ID and MEASURE.STATE=0)"
                        +" or not exists (select 1 from MEASURE where TASKLIST_ID=TASKLIST.ID)"
                        +" or (exists"
	                    +"     (select 1 from TASKLIST SUBT where SUBT.ROOT_ID=TASKLIST.ID"
		                +"         and ("
		                +"         exists (select 1 from MEASURE SUBM where SUBM.TASKLIST_ID=SUBT.ID and SUBM.STATE=0)"
		                +"         or not exists(select 1 from MEASURE SUBM where SUBM.TASKLIST_ID=SUBT.ID)"
		                +"         or SUBT.KEEP_OPEN=1"
		                +"         )"
	                    +"     ))"
                        +" or (exists"
	                    +"     (select 1 from TASKLIST SUBTASS join TASKLIST_ASSIGNMENT TASS on SUBTASS.ID = TASS.ASSIGNED_TASKLIST_ID and TASS.PARENT_TASKLIST_ID=TASKLIST.ID"
		                +"         and ("
		                +"         exists (select 1 from MEASURE SUBMTASS where SUBMTASS.TASKLIST_ID=SUBTASS.ID and SUBMTASS.STATE=0)"
		                +"         or not exists(select 1 from MEASURE SUBMTASS where SUBMTASS.TASKLIST_ID=SUBTASS.ID)"
		                +"         or SUBTASS.KEEP_OPEN=1"
		                +"         )"
	                    +"     ))"
                        +" or KEEP_OPEN=1"
                        +")";
                }

                sql += " order by " + OrderColumn + " " + OrderDir;
                if (sql.IndexOf("distinct") < 0) sql = sql.Replace("select","select distinct");
                if (sql.IndexOf("distinct 1 from") > 0) sql = sql.Replace("distinct 1 from","1 from");

                _table = _db.getDataTableExt(sql,"TASKLIST");
                _table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                _table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%AUTHOR_PERSON_ID", "mode","oe");
				_table.Columns["TEMPLATE_TASKLIST_ID"].ExtendedProperties["In"] = _db.Tasklist.getTemplatesTable();
				
				if (Template)
				{
					_table.Columns["CREATIONDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
					_table.Columns["STARTDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
					_table.Columns["DUEDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
				}

				EditURL = psoft.Tasklist.EditTasklist.GetURL(
						"ID", "%ID",
						"backURL", Request.RawUrl
					);

				if (DetailURL == "")
				{
					DetailURL = psoft.Tasklist.TaskDetail.GetURL("ID", "%ID");
                }

                IDColumn = "ID";
                View = "TASKLIST_DETAIL";

				if (SelectedID > 0) {
                    HighlightRecordID = SelectedID;
                }

                int numRec = LoadList(_db, _table, tasklistList);

            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected override void onAfterAddCells(DataRow row, TableRow r) 
		{
			if (!Template)
			{
				long ID = DBColumn.GetValid(row["ID"],0L);

				TableCell cState = new TableCell();
				cState.CssClass = "List";
				r.Cells.Add(cState);
				cState.Text = _states[0] + "(" + _db.Tasklist.getOpenMeasureCount(ID,true) + ")";

				TableCell cSemaphore = new TableCell();
				cSemaphore.CssClass = "List";
				r.Cells.Add(cSemaphore);
				System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
				switch (_db.Tasklist.getSemaphore(ID, true)) {
				case 0:
					image.ImageUrl = "../../images/ampelRot.gif";
					image.ToolTip = _redComment;
					break;
				case 1:
					image.ImageUrl = "../../images/ampelOrange.gif";
					image.ToolTip = _orangeComment;
					break;
				case 2:
					image.ImageUrl = "../../images/ampelGruen.gif";
					image.ToolTip = _greenComment;
					break;
				case 3:
					image.ImageUrl = "../../images/ampelGrau.gif";
					image.ToolTip = _doneComment;
					cState.Text = _states[1];
					break;
				}

				cSemaphore.Controls.Add(image);
			}
        }
           
        /// <summary>
        /// Event handler for the 'next' button
        /// The selected item(s) database ID will be stored in the SEARCHRESULT table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void next_Click(object sender, System.EventArgs e) {
            long searchResultID = SaveInSearchResult(tasklistList, "TASKLIST");

            if (Kontext == "searchassign") {
                _nextArgs.LoadUrl = psoft.Tasklist.AssignTasklist.GetURL(
                    "parentID", XID,
                    "searchResultID", searchResultID,
                    "rootID", RootID
                    );
            }
            else if (NextURL != ""){
                _nextArgs.LoadUrl = NextURL.Replace("%25SearchResultID","%SearchResultID").Replace("%SearchResultID", searchResultID.ToString());
            }
            else
			{
                _nextArgs.LoadUrl = psoft.Tasklist.TaskDetail.GetURL(
						"context", "selection",
						"xID", searchResultID > 0 ? searchResultID.ToString(): "0"
					);
            }

            DoOnNextClick(next);
        }

        private void MapButtonMethods() {
            next.Click += new System.EventHandler(next_Click);
            this.CBShowDone.CheckedChanged += new System.EventHandler(this.CBShowDone_CheckedChanged);
        }

        private void CBShowDone_CheckedChanged(object sender, System.EventArgs e) {
            SessionData.setShowDoneTasklists(Session, CBShowDone.Checked);
            loadList();
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            MapButtonMethods();
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

