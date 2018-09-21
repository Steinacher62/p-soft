namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class ProjectTeamList : PSOFTSearchListUserControl {
        public const string PARAM_PROJECT_ID = "PARAM_PROJECT_ID";


        protected DBData _db = null;

        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/ProjectTeamList.ascx";}
        }

        public ProjectTeamList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = false;
            EditEnabled = false;
            DetailEnabled = true;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
        }

		#region Properities
        public long ProjectID {
            get {return GetLong(PARAM_PROJECT_ID);}
            set {SetParam(PARAM_PROJECT_ID, value);}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();

            loadList();
        }

        protected void loadList(){
            commiteeListTab.Rows.Clear();
            leaderListTab.Rows.Clear();
            memberListTab.Rows.Clear();

            _db = DBData.getDBData(Session);
            try {
                _db.connect();
                IDColumn = "ID";
                if (_db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "PROJECT", ProjectID, true, true)){
                    DeleteEnabled = true;
                    EditEnabled = true;
                    EditURL = psoft.Project.TeamMemberEdit.GetURL("projectID",ProjectID, "jobID","%ID", "mode","edit", "nextURL",Request.RawUrl);
                }
                commiteeLabel.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_COMMITEE);
                leaderLabel.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_LEADERS);
                memberLabel.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_MEMBERS);
                loadOEList(commiteeListTab, _db.Project.getCommiteeOrgentityID(ProjectID), 1);
                loadOEList(leaderListTab, _db.Project.getLeaderOrgentityID(ProjectID), 1);
                loadOEList(memberListTab, _db.Project.getLeaderOrgentityID(ProjectID), 0);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected void loadOEList(Table listTable, long oeID, int jobTyp){
            string sql = "select * from JOB_PERS_FUNC_V where ORGENTITY_ID=" + oeID + " and JOB_TYP=" + jobTyp;
            sql += " order by " + OrderColumn + " " + OrderDir;
            DataTable table = _db.getDataTableExt(sql, "JOB_PERS_FUNC_V");

            DetailURL = psoft.Person.DetailFrame.GetURL("ID","%PERSON_ID", "mode","oe", "xID",oeID);
            LoadList(_db, table, listTable);
        }

        private void mapControls () {
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
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}
