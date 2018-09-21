namespace ch.appl.psoft.FBS.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;


    public partial class FunctionSearchCtrl : PSOFTSearchUserControl {

        private DBData _db = null;
        private DataTable _table = null;


        public static string Path {
            get {return Global.Config.baseURL + "/FBS/Controls/FunctionSearchCtrl.ascx";}
        }

		#region Properities

		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }
        protected override void DoExecute() {
            base.DoExecute ();

            if (!IsPostBack) {
                apply.Text = _mapper.get("search");
            }
            if (Visible) loadDetail();
        }
        private void loadDetail() {
            _db = DBData.getDBData(Session);
            _db.connect();
            try {
                // load details of tasklist
                detailTab.Rows.Clear();
                _table = _db.getDataTableExt("select * from FUNCGROUPV where FID=-1","FUNCGROUPV");
                if (_table.Columns.Contains("FBW_REVISION") && !Global.isModuleEnabled("fbw")) _table.Columns["FBW_REVISION"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                base.LoadInput(_db, _table, detailTab);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }
        private void mapControls() { 
            apply.Click += new System.EventHandler(apply_Click);
        }

        private void apply_Click(object sender, System.EventArgs e) {
            if (!base.checkInputValue(_table,detailTab))
                return;

            string sql = base.getSql(_table, detailTab);
           
            // Setting search event args
            _searchArgs.ReloadList = true;
            _searchArgs.SearchSQL = sql;

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
