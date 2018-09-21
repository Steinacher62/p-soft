namespace ch.appl.psoft.Tasklist.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PendenzenMeasureList.
    /// </summary>
    public partial class PendenzenMeasureList : PSOFTSearchListUserControl {

        public const string PARAM_CONTEXT = "PARAM_CONTEXT";
        public const string PARAM_XID = "PARAM_XID";
        public const string PARAM_ROOT_ID = "PARAM_ROOT_ID";
        public const string PARAM_SELECTED_ID = "PARAM_SELECTED_ID";
        public const string PARAM_ASSIGN_PERSON = "PARAM_ASSIGN_PERSON";
        public const string PARAM_ASSIGN_MEASURE = "PARAM_ASSIGN_MEASURE";
        public const string PARAM_BACK_URL = "PARAM_BACK_URL";
        public const string PARAM_SUBMENU_ENALBE = "PARAM_SUBMENU_ENALBE";
        public const string PARAM_SORT_ENABLED = "PARAM_SORT_ENABLED";
        public const string PARAM_DELETE_URL = "PARAM_DELETE_URL";
        public const string PARAM_TEMPLATE = "PARAM_TEMPLATE";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
		public const string PARAM_HIDETASKLIST = "PARAM_HIDETASKLIST";
        
        protected string _onloadString2 = ";";
        protected string _tableName = "MEASURE";
        protected string _redComment;
        protected string _orangeComment;
        protected string _greenComment;
        protected string _doneComment;
        protected ArrayList _states;

        protected DBData _db = null;
        protected DataTable _table;


		private int _criticalDays = 0;
		private bool _isSetCriticalDays = false;

        public static string Path {
            get {return Global.Config.baseURL + "/Tasklist/Controls/PendenzenMeasureList.ascx";}
        }

		#region Properities
        public bool AssignMeasure {
            get {return GetBool(PARAM_ASSIGN_MEASURE);}
            set {SetParam(PARAM_ASSIGN_MEASURE, value);}
        }

        public bool SubMenuEnable {
            get {return GetBool(PARAM_SUBMENU_ENALBE);}
            set {SetParam(PARAM_SUBMENU_ENALBE, value);}
        }

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

        public string AssignPerson {
            get {return GetString(PARAM_ASSIGN_PERSON);}
            set {SetParam(PARAM_ASSIGN_PERSON, value);}
        }

        public long SelectedID {
            get {return GetID(PARAM_SELECTED_ID);}
            set {SetParam(PARAM_SELECTED_ID,value);}
        }

        public bool SortEnabled {
            get {return GetBool(PARAM_SORT_ENABLED);}
            set {SetParam(PARAM_SORT_ENABLED, value);}
        }

        public string DeleteURL {
            get {return GetString(PARAM_DELETE_URL);}
            set {SetParam(PARAM_DELETE_URL, value);}
        }

        public bool Template {
            get {return GetBool(PARAM_TEMPLATE);}
            set {SetParam(PARAM_TEMPLATE, value);}
        }

        public string NextURL {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

		public bool HideTasklist
		{
			get {return GetBool(PARAM_HIDETASKLIST);}
			set {SetParam(PARAM_HIDETASKLIST, value);}
		}
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            deleteMessage = _mapper.get("tasklist","deleteMeasureConfirm");
            _redComment = TaskListModule.getSemaphoreMeasureComment(Session, 0);
            _orangeComment = TaskListModule.getSemaphoreMeasureComment(Session, 1);
            _greenComment = TaskListModule.getSemaphoreMeasureComment(Session, 2);
            _doneComment = TaskListModule.getSemaphoreMeasureComment(Session, 3);

            if (!IsPostBack) {
                CBShowDone.Text = _mapper.get("tasklist", "showDoneMeasures");
                CBShowDone.Checked = SessionData.showDoneMeasures(Session);
				CBShowSubs.Text = _mapper.get("tasklist", "showSubMeasures");
				CBShowSubs.Checked = SessionData.showSubMeasures(Session);
				next.Text = _mapper.get("next");
				_isSetCriticalDays = false;
            }

            if (this.Visible) {
                Execute();
            }
        }

        protected override void DoExecute() {
            base.DoExecute();

            loadList();
        }

        private void loadList() {
			_db = DBData.getDBData(Session);
            _states = new ArrayList(_mapper.getEnum("tasklist", "state", true));

            try {
                measureList.Rows.Clear();
                string sql = "";
                string defaultSql = "select * from MEASURE where template=0";

                if (Kontext == "search"){
                    CheckBoxEnabled = true;
                }

                DeleteEnabled = true;
                EditEnabled = true;
                DetailEnabled = true;
                InfoBoxEnabled = true;
                HeaderEnabled = true;
                UseFirstLetterAsPageSelector = true;
                IDColumn = "ID";
                if (DetailURL == ""){
                    if (Kontext == "search")
					{
                        DetailURL = psoft.Tasklist.MeasureDetail.GetURL("ID", "%ID");
                    }
                    else
					{
                        DetailURL = psoft.Tasklist.MeasureDetail.GetURL(
								"context", Kontext,
								"xID", XID,
								"ID", "%ID",
								"rootID", RootID,
								"orderColumn", OrderColumn,
								"orderDir", OrderDir
							);
                    }
                }
	
				int showsubs = 0;
				if (CBShowSubs.Checked) showsubs = 1;

                DataTable personTable = _db.Person.getWholeNameMATable(true);
                switch (Kontext) {
                    case "search":
                        MeasureListTitle.Text = _mapper.get("tasklist","foundMeasures");
                        sql = Session["TasklistSQLSearch"] as string;
                        next.Visible = true;
                        CBShowDone.Visible = false;
						CBShowSubs.Visible = false;
                        CBShowSubsTR.Visible = false;
                        break;

                    case "author":
                    case "responsible":
                        MeasureListTitle.Text = _mapper.get("tasklist","personMeasuresResponsible").Replace("#1", _db.Person.getWholeName(XID.ToString(),false,false,false));
                        defaultSql += " and " + Kontext.ToUpper() + "_PERSON_ID=" + XID;
                        break;

                    case "selection":
                        MeasureListTitle.Text = _mapper.get("tasklist","selectedMeasures");
                        if(XID > 0)
                            sql = "select * from MEASURE where ID in (select ROW_ID from SEARCHRESULT where TABLENAME='MEASURE' and ID=" + XID + ")";
                        else
                            sql = this.Session["TasklistSQLSearch"] as string;
                        break;

					case psoft.Project.Controls.ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
					case "tasklist":
                        MeasureListTitle.Text = _mapper.get("tasklist","measures");

                        if (Template)
                        {
                            CBShowDone.Visible = false;
						    //load measure list
						    //defaultSql = "select * from MEASURE where MEASURE.template=1 and MEASURE.TASKLIST_ID=" + XID;
							defaultSql = "select * from MEASURE where MEASURE.template=1 and MEASURE.TASKLIST_ID in (select * from get_tasklists(" + XID + "," + showsubs + "))";
                        }
                        else
                        {
                            //load measure list
                            defaultSql = "select * from MEASURE where MEASURE.template=0 and MEASURE.TASKLIST_ID in (select * from get_tasklists(" + XID + "," + showsubs + "))";
                        }

                        break;
				}

                if (sql == "" || sql == null)
                    sql = defaultSql;

                if (!SessionData.showDoneMeasures(Session) && !Template)
                    sql += " and STATE=0";

                if ((OrderColumn != null) && (OrderColumn != ""))
                    sql += " order by " + OrderColumn + " " + OrderDir;

                Session.Add("measurelistsql",sql);

                _db.connect();
                _table = _db.getDataTableExt(sql,"MEASURE");

                if (SortEnabled && SortURL == "")
				{
                    SortURL = psoft.Tasklist.MeasureDetail.GetURL(
							"context", Kontext,
							"xID", XID,
							"ID", SelectedID
						);
                }

                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = personTable;
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%RESPONSIBLE_PERSON_ID", "mode","oe");
                _table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["In"] = personTable;
                _table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%AUTHOR_PERSON_ID", "mode","oe");
                _table.Columns["STATE"].ExtendedProperties["In"] = _states;
                _table.Columns["TRIGGER_UID"].ExtendedProperties["In"] = this;
                _table.Columns["TRIGGER_UID"].ExtendedProperties["ContextLink"] = psoft.Goto.GetURL("uid","%TRIGGER_UID");
				
				if (Template)
				{
					_table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
					_table.Columns["CREATIONDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
					_table.Columns["STARTDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
					_table.Columns["DUEDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["TRIGGER_UID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
				}
				
				EditURL = psoft.Tasklist.EditMeasure.GetURL(
						"assignPerson", AssignPerson,
						"id", "%ID",
						"backURL", Request.RawUrl,
						"HideTasklist", HideTasklist
					);
                
				if (SelectedID > 0){
                    HighlightRecordID = SelectedID;
                }
                
				base.View = "MEASURE_DETAIL";
                base.CheckOrder = true;
                int numRec = LoadList(_db, _table, measureList);

            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        public string lookup(DataColumn col, object id, bool http) {
            if (col.ColumnName == "TRIGGER_UID" && DBColumn.GetValid(id,0L) > 0) return _db.UID2NiceName((long)id, _mapper);
            return "";
        }
        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            long ID = DBColumn.GetValid(row["ID"],0L);

            if (col != null) {
                switch(col.ColumnName) {
                case "STATE":
                    // create dropdown
                    cell.Controls.Clear();
                    DropDownList dd = new DropDownCtrl();
                    cell.Controls.Add(dd);
                    dd.Items.Add(new ListItem(_states[0] as string, "0"));
                    dd.Items.Add(new ListItem(_states[1] as string, "1"));
                    dd.SelectedIndex = int.Parse(row[col].ToString());
                    dd.ID = "DDSTATE" + row["ID"].ToString();
                    dd.SelectedIndexChanged += new EventHandler(ddState_SelectedIndexChanged);
                    dd.AutoPostBack = true;
					
					if (!_isSetCriticalDays)
					{
						_isSetCriticalDays = true;
						switch (Kontext)
						{
							case psoft.Project.Controls.ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
								long xid = _db.Project.getProjectIDbyTasklist(XID);
								if (xid < 0)
								{ 
									xid = _db.Phase.getPhaseIDbyTasklist(XID);
									_criticalDays = _db.Project.getCriticalDays(DBColumn.GetValid(_db.lookup("PROJECT_ID", "PHASE", "ID=" + xid), -1L));
								}
								else
								{
									_criticalDays = _db.Project.getCriticalDays(xid);
								}								
								break;
							default:
								_criticalDays = _db.Tasklist.getCriticalDays(DBColumn.GetValid(row["tasklist_id"], 0L));
								break;
						}
						_redComment = TaskListModule.getSemaphoreMeasureComment(Session, 0, _criticalDays);
						_orangeComment = TaskListModule.getSemaphoreMeasureComment(Session, 1, _criticalDays);
						_greenComment = TaskListModule.getSemaphoreMeasureComment(Session, 2, _criticalDays);
						_doneComment = TaskListModule.getSemaphoreMeasureComment(Session, 3, _criticalDays);
					}

                    // append semaphore...
                    TableCell c = new TableCell();
                    r.Cells.Add(c);
                    System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                    DateTime dueDate = DBColumn.GetValid(row["DUEDATE"], DateTime.MaxValue);
                    if (row[col].ToString() == "1") {
                        image.ImageUrl = "../../images/ampelGrau.gif";
                        image.ToolTip = _doneComment;
                    }
                    else if (dueDate > (DateTime.Now.AddDays(_criticalDays))) {
                        image.ImageUrl = "../../images/ampelGruen.gif";
                        image.ToolTip = _greenComment;
                    }
                    else if (dueDate > DateTime.Now) {
                        image.ImageUrl = "../../images/ampelOrange.gif";
                        image.ToolTip = _orangeComment;
                    }
                    else {
                        image.ImageUrl = "../../images/ampelRot.gif";
                        image.ToolTip = _redComment;
                    }

                    c.Controls.Add(image);
                    break;
                }
            }
            else if (cell.Text.StartsWith("<img")) {
                string id = ListBuilder.getID(r);
                cell.Text = "<img id=\""+id+"\" ondragstart=\"listDragStart('Measure')\" ondragend=\"listDragEnd()\""+cell.Text.Substring(4);
            }
        }
            
        protected override void onAddHeaderCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (col != null && col.ColumnName == "STATE") {
                r.Cells.Add(new TableCell());
            }
        }
            
        /// <summary>
        /// Event handler for the 'next' button
        /// The selected item(s) database ID will be stored in the SEARCHRESULT table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void next_Click(object sender, System.EventArgs e) {
            long searchResultID = SaveInSearchResult(measureList, "MEASURE");

            if (NextURL != ""){
                _nextArgs.LoadUrl = NextURL.Replace("%25SearchResultID","%SearchResultID").Replace("%SearchResultID", searchResultID.ToString());
            }
            else{
                _nextArgs.LoadUrl = psoft.Tasklist.MeasureDetail.GetURL(
                    "context", "selection",
                    "xID", searchResultID > 0 ? searchResultID.ToString(): "0"
                    );
            }

            DoOnNextClick(next);
        }

        private void MapButtonMethods() {
            next.Click += new System.EventHandler(next_Click);
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

        protected void CBShowDone_CheckedChanged(object sender, System.EventArgs e) {
            SessionData.setShowDoneMeasures(Session, CBShowDone.Checked);
            //list-button
            changeButtonList();
            loadList();
        }

		protected void CBShowSubs_CheckedChanged(object sender, System.EventArgs e) 
		{
			SessionData.setShowSubMeasures(Session, CBShowSubs.Checked);
			//list-button
			changeButtonList();
			loadList();
		}

        private void ddState_SelectedIndexChanged(object sender, System.EventArgs e) {
            if (sender is DropDownList) {
                DropDownList dd = (sender as DropDownList);
                try {
                    int measureID = int.Parse(dd.ID.Substring(7));
                    _db.connect();
                    string sql = "update MEASURE set STATE=" + dd.SelectedItem.Value + " where ID=" + measureID;
                    _db.executeProcedure("MODIFYTABLEROW",
                        new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                        new ParameterCtx("USERID",_db.userId),
                        new ParameterCtx("TABLENAME","MEASURE"),
                        new ParameterCtx("ROWID",measureID),
                        new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                        new ParameterCtx("INHERIT",1)
                        );
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

		public void changeButtonList() {
			changeButtonList(_db,_mapper);
		}//changeButtonList

        public void changeButtonList(DBData db, LanguageMapper map) {
            if (XID > 0) {
                string alias = "";
                string param1 = "";
				string param2 = "";

                switch(Kontext) {
                    case "selection":
                        alias = "TaskListSearchResult";
                        param1 = (SessionData.showDoneMeasures(Session)? "&param1=1" : "&param1=0"); // STATE=$1
                        break;

                    case "responsible":
                        alias = "TaskListResponsiblePerson";
                        param1 = (SessionData.showDoneMeasures(Session)? "&param1=1" : "&param1=0"); // STATE=$1
                        break;

					case psoft.Project.Controls.ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
						alias = psoft.Project.Controls.ProjectTreeCtrl.REPORT_TASKLIST_MEASURE_PROJMOD;
						param1 = (SessionData.showDoneMeasures(Session)? "&param1=1" : "&param1=0"); // STATE=$1
						param2 = ch.psoft.Util.Validate.GetValid(db.lookup("TITLE", "PROJECT", "TASKLIST_ID=" + XID, true),"");

						if (param2 == "") {
							param2 = ch.psoft.Util.Validate.GetValid(db.lookup("TITLE", "PHASE", "TASKLIST_ID=" + XID, true),"");
							param2 = "&param2="+map.get(psoft.Project.ProjectModule.LANG_SCOPE_PROJECT,psoft.Project.ProjectModule.LANG_MNEMO_PHASE)+": " + param2;
						} else {
							param2 = "&param2="+map.get(psoft.Project.ProjectModule.LANG_SCOPE_PROJECT,psoft.Project.ProjectModule.LANG_MNEMO_PROJECT)+": " + param2;
						}//if

						break;

                    case "tasklist":
                        alias = "TaskListTaskListID";
                        param1 = (SessionData.showDoneMeasures(Session)? "&param1=1" : "&param1=0"); // STATE=$1
						param2 = (SessionData.showSubMeasures(Session)? "&param2=1" : "&param2=0"); // STATE=$1
                        break;

                }//switch

                PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)this.Page.FindControl("_pL");

                if (PsoftPageLayout != null && alias != "") {
                    PsoftPageLayout.ButtonListAttributes.Add("onClick", "javascript: window.open('" + psoft.Goto.GetURL("alias",alias) + "&param0=" + XID + param1 + param2 + "');");
                    PsoftPageLayout.ButtonListVisible = true;
                    alias += "EXCEL";
                    PsoftPageLayout.ButtonExcelAttributes.Add("onClick", "javascript: window.open('" + psoft.Goto.GetURL("alias",alias) + "&param0=" + XID + param1 + param2 + "');");
                    PsoftPageLayout.ButtonExcelVisible = true;
                }//if
            }//if
        }//changeButtonList
    }
}
