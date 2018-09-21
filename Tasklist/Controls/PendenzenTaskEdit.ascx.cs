namespace ch.appl.psoft.Tasklist.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PendenzenTaskEdit.
    /// </summary>
    public partial class PendenzenTaskEdit : PSOFTInputViewUserControl {
        public const string PARAM_ID = "PARAM_ID";
        public const string PARAM_BACK_URL = "PARAM_BACK_URL";
		public const string PARAM_TEMPLATE = "PARAM_TEMPLATE";

        private DataTable _table;
        private DBData _db = null;

        protected System.Web.UI.HtmlControls.HtmlForm Edit;

        public static string Path {
            get {return Global.Config.baseURL + "/Tasklist/Controls/PendenzenTaskEdit.ascx";}
        }

		#region Properities
        public long id {
            get {return GetLong(PARAM_ID);}
            set {SetParam(PARAM_ID, value);}
        }

        public string BackUrl {
            get {return GetString(PARAM_BACK_URL);}
            set {SetParam(PARAM_BACK_URL, value);}
        }

		public bool Template 
		{
			get {return GetBool(PARAM_TEMPLATE);}
			set {SetParam(PARAM_TEMPLATE, value);}
		}
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _db = DBData.getDBData(Session);
            base.InputType = InputMaskBuilder.InputType.Edit;
			
            string sql = "select * from TASKLIST where ID="+id;

            try {
                if (!IsPostBack) {
                    apply.Text = _mapper.get("apply");
                }
                _db.connect();
                _table = _db.getDataTableExt(sql,"TASKLIST");
                _table.Columns["PARENT_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                _table.Columns["CLIPBOARD_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                if (Template)
                {
                    _table.Columns["CREATIONDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["STARTDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["DUEDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["TEMPLATE_TASKLIST_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["KEEP_OPEN"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                }
                else
                {
                    _table.Columns["TEMPLATE_TASKLIST_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                    _table.Columns["KEEP_OPEN"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.EDIT;
                    _table.Columns["KEEP_OPEN"].ExtendedProperties["InputControlType"] = typeof(CheckBox);
                }

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
            if (col != null && col.ColumnName == "PARENT_ID") {
                Label lbl = (Label) r.Cells[1].Controls[0];
                if (lbl != null) lbl.Text = _db.lookup("title","tasklist","template=0 and id="+SQLColumn.GetValid(row["PARENT_ID"],0L),true);
            }
            else if (col != null && col.ColumnName == "TEMPLATE_TASKLIST_ID") {
                Label lbl = (Label) r.Cells[1].Controls[0];
                if (lbl != null) lbl.Text = _db.lookup("title","tasklist","template=1 and id="+SQLColumn.GetValid(row["TEMPLATE_TASKLIST_ID"],0L),true);
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
                string sql = base.getSql(_table, editTab);

                if (sql != "") {
                    _db.executeProcedure("MODIFYTABLEROW",
                        new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                        new ParameterCtx("USERID",_db.userId),
                        new ParameterCtx("TABLENAME","TASKLIST"),
                        new ParameterCtx("ROWID",id),
                        new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                        new ParameterCtx("INHERIT",1)
                       );
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();   
            }

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