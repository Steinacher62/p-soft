namespace ch.appl.psoft.Tasklist.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Web;

    /// <summary>
    ///		Summary description for TasklistTreeCtrl.
    /// </summary>
    public partial class TasklistTreeCtrl : PSOFTDetailViewUserControl {

        private static string[] Ampeln = {"ampelRot.gif","ampelOrange.gif","ampelGruen.gif" ,"ampelGrau.gif"};
        private static string[] Pfeile = {"pfeilRot.gif","pfeilOrange.gif","pfeilGruen.gif" ,"pfeilGrau.gif"};

        protected long _selectedID = 0;
        protected bool _showRoot = true;
        protected long _rootID = -1;
		protected bool _template = false;

        private DBData _db = null;

        public static string Path {
            get {return Global.Config.baseURL + "/Tasklist/Controls/TasklistTreeCtrl.ascx";}
        }

		#region Properities
        public bool ShowRoot {
            get {return _showRoot;}
            set {_showRoot = value;}
        }

        public long SelectedID {
            get {return _selectedID;}
            set {_selectedID = value;}
        }

        public long RootID {
            get {return _rootID;}
            set {_rootID= value;}
        }

		public bool Template
		{
            get {return _template;}
            set {_template = value;}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected string buildTree {
            get {
                long rootId = ShowRoot ? (_rootID > 0 ? _rootID : Validate.GetValid(_db.lookup("root_id","tasklist","id="+_selectedID, true),0)) : _selectedID;
                string url = "";

				if (Template)
				{
					url = psoft.Tasklist.TaskDetail.GetURL(
							"ID", "%ID",
							"rootID", rootId,
							"context", "tasklisttemplate"
						);
				}
				else
				{
					url = psoft.Tasklist.TaskDetail.GetURL("ID", "%ID", "rootID", rootId);
				}

				Common.Tree tree = new Common.Tree("TASKLISTTREEV", Response, url);
                tree.extendNode += new ExtendNodeHandler(this.extendNode);
                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                tree.BranchToolTipColum = "DESCRIPTION";
                Response.Write("<script language=\"javascript\">\n");

                tree.build(_db, rootId, "tasklistTree");
                
                return "</script>";
            }
        }

        private bool extendNode(HttpResponse response, string nodeName,  DataRow row, int level) {
            long id = DBColumn.GetValid(row["ID"],0L);
            string image = "";
            int ampel = _db.Tasklist.getSemaphore(id,true);

            if (DBColumn.GetValid(row["ISASSIGNED"], 0) == 1){
                image = Pfeile[ampel];
            }
            else{
                image = Ampeln[ampel];
            }
            response.Write(nodeName+".prependHTML=\"<img align='absmiddle' src='../images/" + image + "'>&nbsp;&nbsp;\";\n");
            return true;
        }

        protected override void DoExecute() {
            base.DoExecute ();

            _db = DBData.getDBData(Session);
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
