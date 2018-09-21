namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Web;


    public partial class ProjectTreeCtrl : PSOFTMapperUserControl {

		public const string CONTEXT_TASKLIST_MEASURE = "tasklistmeasureprojmod";
		public const string REPORT_TASKLIST_MEASURE_PROJMOD = "TaskListMeasureProjMod";

        public const string PARAM_PROJECT_ID = "PARAM_PROJECT_ID";
		public const string PARAM_PHASE_ID = "PARAM_PHASE_ID";

        private DBData _db = null;
        private static string[] Ampeln = {"ampelRot.gif", "ampelOrange.gif", "ampelGruen.gif", "ampelGrau.gif", "ampelBlau.gif"};

        protected System.Web.UI.WebControls.Table detailTab;

		private Interface.DBObjects.Tree _dbTree = null;

        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/ProjectTreeCtrl.ascx";}
        }

		#region Properities
        public long ProjectID {
            get {return GetLong(PARAM_PROJECT_ID);}
            set {SetParam(PARAM_PROJECT_ID, value);}
        }
		public long PhaseID 
		{
			get {return GetLong(PARAM_PHASE_ID);}
			set {SetParam(PARAM_PHASE_ID, value);}
		}
		public string HighLightNode 
		{
			get 
			{
				string retValue = "";
				if (PhaseID > 0) 
				{
					if (_dbTree == null)
					{
						loadDBTree();
					}
//					if (BranchNodeUrl != "" || _dbTree.getRoot(PhaseID) != PhaseID)
//					{
//						retValue = "highlightNode(" + PhaseID + ");";
//					}
					retValue = "highlightNode(" + PhaseID + ");";
				}
				return retValue;
			}
		}
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

//        protected string buildTree {
//            get {
//                Common.Tree tree = new Common.Tree("PROJECT", Response, Psoft.Project.ProjectDetail.GetURL("ID","%ID"));
//                tree.extendNode += new ExtendNodeHandler(this.extendNode);
//                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
//                tree.BranchToolTipColum = "DESCRIPTION";
//                Response.Write("<script language=\"javascript\">\n");
//                long rootId = Validate.GetValid(_db.lookup("ROOT_ID", "PROJECT", "ID=" + ProjectID, true), -1L);
//
//                tree.build(_db, rootId, "projectTree");
//                
//                return "</script>";
//            }
//        }

		protected string buildTree 
		{
			get 
			{
				Common.Tree tree = new Common.Tree("PROJECT", "PHASE", "PROJECT_ID", Response, psoft.Project.ProjectDetail.GetURL("ID","%ID"), psoft.Project.PhaseDetail.GetURL("ID","%ID"));
				tree.extendNode += new ExtendNodeHandler(this.extendNode);
				tree.extendLeafNode += new ExtendLeafNodeHandler(this.extendLeafNode);
				tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
				tree.BranchToolTipColum = "DESCRIPTION";
				tree.BranchOrderColumn = "DUEDATE,TITLE";
				tree.LeafToolTipColum = "DESCRIPTION";
				tree.LeafOrderColumn = "DUEDATE,TITLE";
                tree.LeafsBeforeBranches = true;

				Response.Write("<script language=\"javascript\">\n");
				long rootId = Validate.GetValid(_db.lookup("ROOT_ID", "PROJECT", "ID=" + ProjectID, true), -1L);
				tree.build(_db, rootId, "projectTree");
				return "</script>";
			}
		}

        private bool extendNode(HttpResponse response, string nodeName,  DataRow row, int level) {
            long id = DBColumn.GetValid(row["ID"], 0L);
            int ampel = _db.Project.getSemaphore(id, true);
            response.Write(nodeName+".prependHTML=\"<img align='absmiddle' src='../images/" + Ampeln[ampel] + "'>&nbsp;&nbsp;\";\n");

            int openPhases = _db.Project.getOpenPhasesCount(id, true);
            response.Write(nodeName + ".desc += \" (" + openPhases + ")\";\n");
            return true;
        }

		private bool extendLeafNode(HttpResponse response, string nodeName,  DataRow row) 
		{
			long id = DBColumn.GetValid(row["ID"], 0L);
			int ampel = _db.Phase.getSemaphore(id,_db.Project.getCriticalDays(DBColumn.GetValid(row["PROJECT_ID"], 0L)), false);
			response.Write(nodeName+".prependHTML=\"<img align='absmiddle' src='../images/" + Ampeln[ampel] + "'>&nbsp;&nbsp;\";\n");
			return true;
		}

        protected override void DoExecute() {
            base.DoExecute ();

            _db = DBData.getDBData(Session);
        }

		private void loadDBTree()
		{
			_dbTree = DBData.getDBData(Session).Tree("PROJECT");
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
