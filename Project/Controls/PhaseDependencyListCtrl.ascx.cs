namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class PhaseDependencyListCtrl : PSOFTListViewUserControl
    {
        private long _projectID = -1;
        private long _phaseID = -1;
        private string _postDeleteURL;

        protected DBData _db = null;

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        protected string _redComment;
        protected string _orangeComment;
        protected string _greenComment;
        protected string _doneComment;
        protected ArrayList _states;
        protected int _criticalDays = 1;

        public static string Path
        {
            get {return Global.Config.baseURL + "/Project/Controls/PhaseDependencyListCtrl.ascx";}
        }

        public PhaseDependencyListCtrl() : base()
        {
            HeaderEnabled = true;
            DeleteEnabled = true;
            DetailEnabled = true;
            EditEnabled = true;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
        }

		#region Properities
        public long ProjectID {
            get {return _projectID;}
            set {_projectID = value;}
        }

        public long PhaseID {
            get {return _phaseID;}
            set {_phaseID = value;}
        }

        public string PostDeleteURL {
            get {return _postDeleteURL;}
            set {_postDeleteURL = value;}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();

            loadList();
        }

        protected void loadList(){
			string sql = "select * from PHASEDEPENDENCYV where MASTER_PHASE_ID=-1";
            _db = DBData.getDBData(Session);
            _states = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PHASE);
            listTab.Rows.Clear();
            try 
            {
                _db.connect();
                pageTitle.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_PHASEDEPENDENCY_LIST).Replace("#1", _db.lookup("TITLE", "PROJECT", "ID=" + ProjectID, false));
                _criticalDays = _db.Project.getCriticalDays(ProjectID);

                _redComment = ProjectModule.getSemaphorePhaseComment(Session, 0, _criticalDays);
                _orangeComment = ProjectModule.getSemaphorePhaseComment(Session, 1, _criticalDays);
                _greenComment = ProjectModule.getSemaphorePhaseComment(Session, 2, _criticalDays);
                _doneComment = ProjectModule.getSemaphorePhaseComment(Session, 3, _criticalDays);

				if(PhaseID > 0) sql = "select * from PHASEDEPENDENCYV where MASTER_PHASE_ID=" + PhaseID + " or SLAVE_PHASE_ID=" + PhaseID + " order by " + OrderColumn + " " + OrderDir;
				else if (ProjectID > 0) 
				{
					string projPhases = _db.Project.getAllProjectPhases(ProjectID,true);
					sql = "select * from PHASEDEPENDENCYV where MASTER_PHASE_ID in (" + ((projPhases == "") ? "-1" : projPhases) + ") or SLAVE_PHASE_ID in (" + ((projPhases == "") ? "-1" : projPhases) + ") order by " + OrderColumn + " " + OrderDir;
				}
				DataTable table = _db.getDataTableExt(sql, "PHASEDEPENDENCYV");
                IDColumn = "ID";
                if (_phaseID > 0)
                    HighlightRecordID = _phaseID;

                table.Columns["SLAVE_STATE"].ExtendedProperties["In"] = _states;
				table.Columns["MASTER_STATE"].ExtendedProperties["In"] = _states;
				table.Columns["SLAVE_STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
				table.Columns["MASTER_STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                LoadList(_db, table, listTab);
            }
            catch (Exception ex) 
            {
                DoOnException(ex);
            }
            finally 
            {
                _db.disconnect();
            }
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (col != null) {
                switch(col.ColumnName) {
                    case "MASTER_TITLE":
						if(PhaseID != DBColumn.GetValid(row["MASTER_PHASE_ID"],0L))
						{
							HyperLink linkMaster = new HyperLink();
							cell.Controls.Add(linkMaster);
							linkMaster.NavigateUrl = psoft.Project.PhaseDetail.GetURL("ID",DBColumn.GetValid(row["MASTER_PHASE_ID"],0L));
							linkMaster.Text = cell.Text;
						}
						else 
							cell.Font.Bold = true;
						break;
					case "SLAVE_TITLE":
						if(PhaseID != DBColumn.GetValid(row["SLAVE_PHASE_ID"],0L))
						{
							HyperLink linkSlave = new HyperLink();
							cell.Controls.Add(linkSlave);
							linkSlave.NavigateUrl = psoft.Project.PhaseDetail.GetURL("ID",DBColumn.GetValid(row["SLAVE_PHASE_ID"],0L));
							linkSlave.Text = cell.Text;
						}
						else 
							cell.Font.Bold = true;
						break;
                }
            }
        }

		protected override bool onRowAccess (DataTable table, DataRow row, bool isRowAccessPermitted, int requestedAuthorisation) 
		{
			bool retValue = false;
			long masterPhaseID = DBColumn.GetValid(row["MASTER_PHASE_ID"],0L);
			long slavePhaseID = DBColumn.GetValid(row["SLAVE_PHASE_ID"],0L);
			if (requestedAuthorisation == DBData.AUTHORISATION.READ)
			{
				retValue = _db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "PHASE", masterPhaseID, true, true) &&  _db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "PHASE", slavePhaseID, true, true);
			}
			else if (requestedAuthorisation == DBData.AUTHORISATION.UPDATE)
			{
				retValue = _db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "PHASE", masterPhaseID, true, true) || _db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "PHASE", slavePhaseID, true, true);
			}
			else if (requestedAuthorisation == DBData.AUTHORISATION.DELETE)
			{
				retValue = _db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "PHASE", masterPhaseID, true, true) || _db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "PHASE", slavePhaseID, true, true);
			}
			return retValue;
		}


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
		#endregion
    }
}
