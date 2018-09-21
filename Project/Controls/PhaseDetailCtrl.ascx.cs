namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class PhaseDetailCtrl : PSOFTDetailViewUserControl {
        public const string PARAM_PHASE_ID = "PARAM_PHASE_ID";

        private DBData _db = null;
        private DataTable _table;
        private string _postDeleteURL;


        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/PhaseDetailCtrl.ascx";}
        }

		#region Properities
        public long PhaseID {
            get {return GetLong(PARAM_PHASE_ID);}
            set {SetParam(PARAM_PHASE_ID, value);}
        }

        public string PostDeleteURL {
            get {return _postDeleteURL;}
            set {_postDeleteURL = value;}
        }
		protected string deleteMessage 
		{
			get { return _mapper.get("project","deletePhaseConfirm"); }
		}
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();

            _db = DBData.getDBData(Session);
            _db.connect();
            try {
                DataTable parentTable = _db.getDataTable("select ID, TITLE from PROJECT");

                detailTab.Rows.Clear();
                _table = _db.getDataTableExt("select * from PHASE where ID=" + PhaseID, "PHASE");
                _table.Columns["LEADER_PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);;
                _table.Columns["LEADER_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%LEADER_PERSON_ID", "mode","oe");
                _table.Columns["PROJECT_ID"].ExtendedProperties["In"] = parentTable;
                _table.Columns["STATE"].ExtendedProperties["In"] = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PHASE);

                _table.Columns["HAS_MILESTONE"].ExtendedProperties["In"] = _mapper.getEnum("phase", "hasMilestone");

                LoadDetail(_db, _table, detailTab);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r)
        {
            if (col != null && col.ColumnName == "MILESTONE_DESCRIPTION")
            {
                int hasMl = ch.psoft.Util.Validate.GetValid(_db.lookup("HAS_MILESTONE", "PHASE", "ID=" + this.PhaseID, false), 0);
                if (hasMl == 0)  // no description
                {
                    r.Cells[0].Visible = false;
                    r.Cells[1].Visible = false;
                }
            }
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
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
