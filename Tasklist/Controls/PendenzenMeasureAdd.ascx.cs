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
    using System.Text;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PendenzenMeasureAdd.
    /// </summary>
    public partial class PendenzenMeasureAdd : PSOFTInputViewUserControl {

        public const string PARAM_BACK_URL = "PARAM_BACK_URL";
        public const string PARAM_TASK_LIST_ID = "PARAM_TASK_LIST_ID";
        public const string PARAM_ASSIGN_PERSON = "PARAM_ASSIGN_PERSON";
        public const string PARAM_TRIGGER_UID = "PARAM_TRIGGER_UID";
        public const string PARAM_TEMPLATE = "PARAM_TEMPLATE";
		public const string PARAM_HIDETASKLIST = "PARAM_HIDETASKLIST";

        protected DataTable _table = null;
        protected DBData _db = null;

        protected System.Web.UI.HtmlControls.HtmlForm Edit;

        public static string Path {
            get {return Global.Config.baseURL + "/Tasklist/Controls/PendenzenMeasureAdd.ascx";}
        }

		#region Properities
        public string BackUrl {
            get {return GetString(PARAM_BACK_URL);}
            set {SetParam(PARAM_BACK_URL, value);}
        }

        public long TaskListId {
            get {return GetID(PARAM_TASK_LIST_ID);}
            set {SetParam(PARAM_TASK_LIST_ID, value);}
        }

        public bool AssignPerson {
            get {return GetBool(PARAM_ASSIGN_PERSON);}
            set {SetParam(PARAM_ASSIGN_PERSON, value);}
        }
        public long TriggerUID {
            get {return GetID(PARAM_TRIGGER_UID);}
            set {SetParam(PARAM_TRIGGER_UID, value);}
        }
        public bool Template 
        {
            get {return GetBool(PARAM_TEMPLATE);}
            set {SetParam(PARAM_TEMPLATE, value);}
        }
		public bool HideTasklist
		{
			get {return GetBool(PARAM_HIDETASKLIST);}
			set {SetParam(PARAM_HIDETASKLIST, value);}
		}
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            _db = DBData.getDBData(Session);

            try {
                if (!IsPostBack) {
                    apply.Text = _mapper.get("apply");
                }
                _db.connect();

                _table = _db.getDataTableExt("select * from MEASURE where id = -1", "MEASURE");

				if (HideTasklist)
					_table.Columns["TASKLIST_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
				else
					_table.Columns["TASKLIST_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
               
                _table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                if (Template)
                {
                    _table.Columns["STARTDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["DUEDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                }
                else
                {
                    ArrayList states = new ArrayList(_mapper.getEnum("tasklist", "state", true));
                    _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    _table.Columns["STATE"].ExtendedProperties["In"] = states;
                }

                DataTable personTable = _db.Person.getWholeNameMATable(false);
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = personTable;

				_table.Columns["CRITICAL"].ExtendedProperties["InputControlType"] = typeof(CheckBox);

                View = "MEASURE";
                LoadInput(_db,_table,addTab);

                // Proposal of next higher measure-number
                int nextNumber = ch.psoft.Util.Validate.GetValid(_db.lookup("max(cast(NUMMER as integer))+1", "MEASURE", "isnumeric(NUMMER)=1 and TASKLIST_ID in (" + _db.Tasklist.addAllSubTasklistIDs(_db.Tasklist.addAllParentTasklistIDs(TaskListId.ToString())) + ")", false), 0);
                setInputValue(_table, addTab, "NUMMER", nextNumber);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
            
        }

        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) {
			if (col != null)
			{
				switch (col.ColumnName.ToUpper())
				{
					case "TASKLIST_ID":
						Label lbl = (Label) r.Cells[1].Controls[0];
						lbl.Text = _db.lookup("title","tasklist","id="+TaskListId,true);
						break;
					case "CRITICAL":
						CheckBox cb = (CheckBox) r.Cells[1].Controls[0];
						cb.Checked = true;
						break;
				}
			}
        }

        private void mapControls () {
            apply.Click += new System.EventHandler(apply_Click);
        }

        private void apply_Click(object sender, System.EventArgs e) {
            string sql = "";

            if (!base.checkInputValue(_table,addTab)) {
                return;
            }
            _db.connect();
            try {
                _db.beginTransaction();
                StringBuilder sb = base.getSql(_table, addTab, true);
                long id = _db.newId(_table.TableName);
                base.extendSql(sb, _table, "ID", id);
                base.extendSql(sb, _table, "AUTHOR_PERSON_ID", SessionData.getUserID(Session));
                base.extendSql(sb, _table, "TEMPLATE", Template ? "1" : "0");
                base.extendSql(sb, _table, "CREATIONDATE", "GetDate()");
                base.extendSql(sb, _table, "TASKLIST_ID", TaskListId);
                if (TriggerUID > 0) base.extendSql(sb, _table, "TRIGGER_UID", TriggerUID);
               
                sql = base.endExtendSql(sb);

                if (Template)
                {
                    _db.execute(sql);
                }
                else
                {
                    _db.executeProcedure("MODIFYTABLEROW",
                        new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                        new ParameterCtx("USERID",_db.userId),
                        new ParameterCtx("TABLENAME","MEASURE"),
                        new ParameterCtx("ROWID",id),
                        new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                        new ParameterCtx("INHERIT",1)
                    );
                }

                //grant base rights...
                _db.Measure.refreshAccessRights(id);

                _db.commit();
                if (BackUrl != "") Response.Redirect(BackUrl, false);
            }
            catch (Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();   
            }

        }

        private void back_Click(object sender, System.EventArgs e) {
            if (BackUrl != "")
                Response.Redirect(BackUrl, false);
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
