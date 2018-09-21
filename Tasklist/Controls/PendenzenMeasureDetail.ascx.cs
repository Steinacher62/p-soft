namespace ch.appl.psoft.Tasklist.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PendenzenMeasureDetail.
    /// </summary>
    public partial  class PendenzenMeasureDetail : PSOFTDetailViewUserControl {
        public const string PARAM_ID = "PARAM_ID";


        protected DBData _db = null;
        protected bool _template;
		private bool _hideTasklist;
        public static string Path {
            get {return Global.Config.baseURL + "/Tasklist/Controls/PendenzenMeasureDetail.ascx";}
        }

		#region Properities
        public long MeasureID {
            get {return GetLong(PARAM_ID);}
            set {SetParam(PARAM_ID, value);}
        }

        public bool Template 
        {
            get {return _template;}
            set {_template = value;}
        }

		public bool HideTasklist 
		{
			get {return _hideTasklist;}
			set {_hideTasklist = value;}
		}

        protected string deleteMessage {
            get { return _mapper.get("tasklist","deleteMeasureConfirm"); }
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();

            _db = DBData.getDBData(Session);

            _db.connect();
            try {

                // load details of measure
                detailTab.Rows.Clear();
                string detailSql = "select * from MEASURE where ID=" + MeasureID;

                DataTable _table = _db.getDataTableExt(detailSql,"MEASURE");

                DataTable personTable = _db.Person.getWholeNameMATable(true);
                DataTable tasklistTable = _db.getDataTable("select id,title from tasklist order by title");

                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = personTable;
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%RESPONSIBLE_PERSON_ID", "mode","oe");
                _table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["In"] = personTable;
                _table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%AUTHOR_PERSON_ID", "mode","oe");
				if (HideTasklist)
					_table.Columns["TASKLIST_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
				else 
					_table.Columns["TASKLIST_ID"].ExtendedProperties["In"] = tasklistTable;
                _table.Columns["CLIPBOARD_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                if (Template)
                {
                    _table.Columns["CREATIONDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["STARTDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["DUEDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["TRIGGER_UID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                }
                else
                {
                    _table.Columns["TRIGGER_UID"].ExtendedProperties["In"] = this;
                    _table.Columns["TRIGGER_UID"].ExtendedProperties["ContextLink"] = psoft.Goto.GetURL("uid","%TRIGGER_UID");
                }

                ArrayList states = new ArrayList(_mapper.getEnum("tasklist", "state", true));
                _table.Columns["STATE"].ExtendedProperties["In"] = states;

				_table.Columns["CRITICAL"].ExtendedProperties["InputControlType"] = typeof(CheckBox);
				_table.Columns["CRITICAL"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("boolean", true));

                base.CheckOrder = true;
                base.View = "MEASURE_DETAIL";
                base.LoadDetail(_db, _table, detailTab);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }
        public string lookup(DataColumn col, object id, bool http) {
            if (col.ColumnName == "TRIGGER_UID" && DBColumn.GetValid(id,0L) > 0) return _db.UID2NiceName((long)id,_mapper);
            return "";
        }

        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) {
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
		#endregion
    }
}