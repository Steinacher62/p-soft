namespace ch.appl.psoft.Performance.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;

    public partial class ExpectationsSearch : PSOFTSearchUserControl {
        protected DataTable _table;
        protected string _jobIDs = "";

        public static string Path {
            get {return Global.Config.baseURL + "/Performance/Controls/ExpectationsSearch.ascx";}
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            DBData db = DBData.getDBData(Session);
            string sql = "select * from JOB_EXPECTATION_V where id = -1";
            
            try {
                if (!IsPostBack) {
                    apply.Text = _mapper.get("search");
                    title.Text = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_CT_EXPECTATION_COPY);
                    ShowRelation = false;
                }
                db.connect();

                _table = db.getDataTableExt(sql, "JOB_EXPECTATION_V");

                _jobIDs = db.Performance.getRateableJobIDs();
                if (_jobIDs.Length > 0){
					string sqlJobs = "select JOB.ID, PERSON.PNAME + ' ' + isnull(PERSON.FIRSTNAME,'') + ' (' + RTRIM(isnull(PERSON.MNEMO,'')) + ')' + ', ' + JOB." + db.langAttrName("JOB", "TITLE") + " from JOB inner join EMPLOYMENT on JOB.EMPLOYMENT_ID=EMPLOYMENT.ID inner join PERSON on EMPLOYMENT.PERSON_ID=PERSON.ID and JOB.ID in (" + _jobIDs + ") order by PERSON.PNAME, PERSON.FIRSTNAME, JOB." + db.langAttrName("JOB", "TITLE");
                    DataTable jobsTable = db.getDataTable(sqlJobs);
                    _table.Columns["JOB_REF"].ExtendedProperties["In"] = jobsTable;
                    _table.Columns["JOB_REF"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                }

                CheckOrder = true;
                LoadInput(db, _table, searchTab);

            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        }

        private void mapControls () {
            apply.Click += new System.EventHandler(apply_Click);
        }

        private void apply_Click(object sender, System.EventArgs e) {
            if (!checkInputValue(_table, searchTab))
                return;

            _searchArgs.SearchSQL = getSql(_table, searchTab);
            _searchArgs.ReloadList = true;

            DoOnSearchClick(apply);
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
