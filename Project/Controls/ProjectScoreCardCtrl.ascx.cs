///deprecated

namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;
    using System.Text;

    public partial  class ProjectScoreCardCtrl : PSOFTDetailViewUserControl {
        public const string PARAM_PROJECT_ID = "PARAM_PROJECT_ID";
        public const string PARAM_TARGET_FRAME = "PARAM_TARGET_FRAME";

        private static int MapId = 0;
        private ScoreCard _scoreCard = null;



        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/ProjectScoreCardCtrl.ascx";}
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        public long ProjectID {
            get {return GetLong(PARAM_PROJECT_ID);}
            set {SetParam(PARAM_PROJECT_ID, value);}
        }

        public string TargetFrame {
            get {return GetString(PARAM_TARGET_FRAME);}
            set {SetParam(PARAM_TARGET_FRAME, value);}
        }

        protected override void DoExecute() {
            base.DoExecute();

            if (!IsPostBack) {
                MapId++;
            }

            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                // Project details
                DataTable table = db.getDataTableExt("select ID, TITLE, IS_MAIN_OBJECTIVE, STARTDATE, DUEDATE from PROJECT where ID=" + ProjectID, "PROJECT");
                if (Global.Config.getModuleParam("project", "enableMainObjectiveField", "0").Equals("1"))
                {
                    table.Columns["IS_MAIN_OBJECTIVE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
                    table.Columns["IS_MAIN_OBJECTIVE"].ExtendedProperties["In"] = _mapper.getEnum("project", "isMainObjective");
                }
                LoadDetail(db, table, detailTab);

                long[] personIDs = db.Project.getLeaderPersonIDs(ProjectID);
                if (personIDs.Length > 0){
                    detailTab.Rows.Add(ProjectDetailCtrl.buildPersonlistRow(db, _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_LEADERS), personIDs));
                }

                // Let's create the score-card...
                _scoreCard = new ScoreCard(db, _mapper, ProjectID, TargetFrame);
                if (_scoreCard != null) {
                    System.Drawing.Image _img = _scoreCard.GetImage();
                    Session["ScoreCardImage"] = _img;
                    Session["ScoreCardImageWidth"] = _img.Width;
                    Session["ScoreCardImageHeight"] = _img.Height;

                    navigationImage.Width = _img.Width;
                    navigationImage.Height = _img.Height;
                    navigationImage.Src = Global.Config.baseURL + "/Project/ScoreCardImage.aspx?projectID=" + ProjectID + "&map=" + MapId; // hack for correct image load
                }
                else {
                    navigationImage.Visible = false;
                }
            }
            catch (Exception e) {
                DoOnException(e);
            }
            finally {
                db.disconnect();
            }
        }

        public string BuildImageMap() {
            if (_scoreCard == null)
                return "";

            StringBuilder _map = new StringBuilder(1024);
            _map.Append("<map name=\"TreeMap\">\n");
            _scoreCard.AppendImageMapInfo(_map);
            _map.Append("</map>");
            return _map.ToString();
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
