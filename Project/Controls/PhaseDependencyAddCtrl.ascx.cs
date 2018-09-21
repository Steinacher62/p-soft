namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Text;


    public partial class PhaseDependencyAddCtrl : PSOFTInputViewUserControl {
        public const string PARAM_PHASE_ID = "PARAM_PHASE_ID";

        protected DataTable _tableMaster;
		protected DataTable _tableSlave;
		protected DataTable _phaseTable;
        protected DBData _db = null;


        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/PhaseDependencyAddCtrl.ascx";}
        }

		#region Properities
        public long PhaseID {
            get {return GetLong(PARAM_PHASE_ID);}
            set {SetParam(PARAM_PHASE_ID, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _db = DBData.getDBData(Session);
			long projectID = _db.lookup("PROJECT_ID", "PHASE", "ID=" + PhaseID, -1L);
            string sqlPhase = "select distinct ID, PROJECT_TITLE + ': ' + PHASE_TITLE as PROJECT_TITLE from PHASEV"
						+ _db.getAccessRightsRowInnerJoinSQL("PHASEV", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true)
						+ " where ID in ("+ _db.Project.getAllProjectPhases(_db.Project.getRootProjectID(projectID),true) +") and ID not in ("+ PhaseID +") order by PROJECT_TITLE asc";
			string sql = "select * from PHASE_DEPENDENCY where ID=-1";

			_db.connect();
            try {
                if (!IsPostBack) {
                    apply.Text = _mapper.get("apply");
                }
				_phaseTable = _db.getDataTableExt(sqlPhase,"PHASEV");
                _tableMaster = _db.getDataTableExt(sql,"PHASE_DEPENDENCY");
				_tableSlave = _db.getDataTableExt(sql,"PHASE_DEPENDENCY");

                _tableMaster.Columns["MASTER_PHASE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _tableMaster.Columns["MASTER_PHASE_ID"].ExtendedProperties["In"] = _phaseTable;
				_tableMaster.Columns["MASTER_PHASE_ID"].ExtendedProperties["Nullable"] = true;
				_tableMaster.Columns["SLAVE_PHASE_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
				                
                _tableSlave.Columns["SLAVE_PHASE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _tableSlave.Columns["SLAVE_PHASE_ID"].ExtendedProperties["In"] = _phaseTable;
				_tableSlave.Columns["SLAVE_PHASE_ID"].ExtendedProperties["Nullable"] = true;
				_tableSlave.Columns["MASTER_PHASE_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                LoadInput(_db,_tableMaster,addMasterTab);
				LoadInput(_db,_tableSlave,addSlaveTab);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }


        private void mapControls () {
            this.apply.Click += new System.EventHandler(apply_Click);
        }

        private void apply_Click(object sender, System.EventArgs e) {
            if (!base.checkInputValue(_tableMaster,addMasterTab) && !base.checkInputValue(_tableSlave,addSlaveTab)){
                return;
            }
            
            long newID = -1;

            _db.connect();
            try {
				//master phase
                _db.beginTransaction();
                StringBuilder sb = getSql(_tableMaster, addMasterTab, true);
                newID = _db.newId(_tableMaster.TableName);

                extendSql(sb, _tableMaster, "ID", newID);
				extendSql(sb, _tableMaster, "SLAVE_PHASE_ID", PhaseID);
                string sql = endExtendSql(sb);

                if (sql.IndexOf("MASTER_PHASE_ID") > 0) {
                    _db.execute(sql);
                    _db.commit();
                }
                else
                    _db.rollback();

				//slave phase
				_db.beginTransaction();
				sb = getSql(_tableSlave, addSlaveTab, true);
				newID = _db.newId(_tableSlave.TableName);

				extendSql(sb, _tableSlave, "ID", newID);
				extendSql(sb, _tableSlave, "MASTER_PHASE_ID", PhaseID);
				sql = endExtendSql(sb);

				if (sql.IndexOf("SLAVE_PHASE_ID") > 0) 
				{
					_db.execute(sql);
					_db.commit();
				}
				else
					_db.rollback();
            }
            catch (Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect(); 
				Response.Redirect(psoft.Project.PhaseDetail.GetURL("ID",PhaseID), false);
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
