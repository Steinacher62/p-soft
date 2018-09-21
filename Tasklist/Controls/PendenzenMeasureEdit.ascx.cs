namespace ch.appl.psoft.Tasklist.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using Common;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PendenzenMeasureEdit.
    /// </summary>
    public partial class PendenzenMeasureEdit : PSOFTInputViewUserControl {
        public const string PARAM_BACK_URL = "PARAM_BACK_URL";
        public const string PARAM_ID = "PARAM_ID";
        public const string PARAM_TEMPLATE = "PARAM_TEMPLATE";
		public const string PARAM_HIDETASKLIST = "PARAM_HIDETASKLIST";

        protected DataTable _table = null;
        protected DBData _db = null;

        protected System.Web.UI.HtmlControls.HtmlForm Edit;

        public static string Path {
            get {return Global.Config.baseURL + "/Tasklist/Controls/PendenzenMeasureEdit.ascx";}
        }

		#region Properities
        public string BackUrl {
            get {return GetString(PARAM_BACK_URL);}
            set {SetParam(PARAM_BACK_URL, value);}
        }

        public long MeasureID {
            get {return GetLong(PARAM_ID);}
            set {SetParam(PARAM_ID, value);}
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
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _db = DBData.getDBData(Session);
            string sql = "select * from MEASURE where ID=" + MeasureID;

            base.InputType = InputMaskBuilder.InputType.Edit;
            try {
                if (!IsPostBack) {
                    apply.Text = _mapper.get("apply");
                }
                _db.connect();

                _table = _db.getDataTableExt(sql, "MEASURE");

                ArrayList states = new ArrayList(_mapper.getEnum("tasklist", "state", true));
				if (HideTasklist)
					_table.Columns["TASKLIST_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
				else
					_table.Columns["TASKLIST_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                _table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["CLIPBOARD_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                if (Template)
                {
                    _table.Columns["CREATIONDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["STARTDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["DUEDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                }
                else
                {
                    _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    _table.Columns["STATE"].ExtendedProperties["In"] = states;
                }

                DataTable personTable = _db.Person.getWholeNameMATable(false);
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = personTable;

				_table.Columns["CRITICAL"].ExtendedProperties["InputControlType"] = typeof(CheckBox);

                base.View = "MEASURE";
                base.LoadInput(_db, _table, editTab);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }            
        }
        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) {
            if (col != null && col.ColumnName == "TASKLIST_ID") {
                Label lbl = (Label) r.Cells[1].Controls[0];
                if (lbl != null) lbl.Text = _db.lookup("title","tasklist","template=0 and id="+DBColumn.GetValid(row["TASKLIST_ID"],0L),true);
            }
        }

        private void mapControls () {
            apply.Click += new System.EventHandler(apply_Click);
        }

        private void apply_Click(object sender, System.EventArgs e) {
            if (!base.checkInputValue(_table, editTab))
                return;

            _db.connect();
            try {
                StringBuilder sb = base.getSql(_table, editTab, true);

                string sql = base.endExtendSql(sb);

                if (sql != "") {
                    _db.executeProcedure("MODIFYTABLEROW",
                        new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                        new ParameterCtx("USERID",_db.userId),
                        new ParameterCtx("TABLENAME","MEASURE"),
                        new ParameterCtx("ROWID",MeasureID),
                        new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                        new ParameterCtx("INHERIT",1)
                        );
                }

                if (BackUrl != ""){
                    Response.Redirect(BackUrl, false);
                }
            }
            catch (Exception ex) {
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
