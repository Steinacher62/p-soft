namespace ch.appl.psoft.SPZ.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Web;

    /// <summary>
    ///		Summary description for DetailView.
    /// </summary>

    public partial class TreeView : PSOFTDetailViewUserControl {
        protected long _id = 0;
        protected long _rootId = 0;
        protected string _context = "";
        protected long _contextId = 0;
        protected long _highlightId = 0;
        protected bool _checkBoxEnable = false;
        protected bool _detailEnable = true;

        private DBData _db = null;
        protected System.Web.UI.WebControls.Table detailTab;
        protected System.Web.UI.HtmlControls.HtmlInputHidden checkFld;

        public static string Path {
            get {return Global.Config.baseURL + "/SPZ/Controls/TreeView.ascx";}
        }

		#region Properities

        /// <summary>
        /// Get/Set root id
        /// </summary>
        public long rootId {
            get {return _rootId;}
            set {_rootId = value; }
        }
        /// <summary>
        /// Get/Set current id
        /// </summary>
        public long id {
            get {return _id;}
            set {
                _id = value;
                _highlightId = value;
            }
        }

        /// <summary>
        /// Get/set context
        /// </summary>
        public string context {
            get {return _context;}
            set {_context = value; }
        }
        /// <summary>
        /// Get/set context id
        /// </summary>
        public long contextId {
            get {return _contextId;}
            set {_contextId = value; }
        }

        /// <summary>
        /// Set/Get checkbox enable
        /// </summary>
        public bool checkBoxEnable {
            get {return _checkBoxEnable;}
            set { _checkBoxEnable = value;}
        }

        /// <summary>
        /// Set/Get detail enable
        /// </summary>
        public bool detailEnable {
            get {return _detailEnable;}
            set { _detailEnable = value;}
        }

        /// <summary>
        /// Set/Get tree title
        /// </summary>
        public string treeTitle {
            get {return title.Text;}
            set { title.Text = value; title.Visible = true;}
        }

        /// <summary>
        /// get check flags
        /// </summary>
        public string checkFlags {
            get {return checkFld.Value;}
        }
 
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected string buildTree {
            get {
                Response.Write("<script language=\"javascript\">\n");
                Common.Tree tree = new Common.Tree("OBJECTIVE", Response, (_detailEnable ? "Detail.aspx?view=detail&context="+_context+"&contextId="+_contextId+"&id=%ID" : ""));
                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                tree.BranchOrderColumn = "NUMBER,TITLE";
                tree.LeafOrderColumn = "NUMBER,TITLE";
                tree.BranchToolTipColum = "DESCRIPTION";
                if (_checkBoxEnable) tree.extendNode += new ExtendNodeHandler(addCheckBox);
                title.Visible = tree.build(_db, _rootId, "objective");
                return "</script>";
            }
        }
        private bool addCheckBox(HttpResponse response, string nodeName, DataRow row, int level) {
            string prepend = nodeName+".prependHTML=\"<input type='checkbox' id='cb"+row["ID"]+"'>\";\n";
            response.Write(prepend);
            return true;
        }
        protected override void DoExecute() {
            base.DoExecute ();
            _db = DBData.getDBData(Session);

            _db.connect();
            try {
                if (_rootId <= 0) {
                    long rootId = DBColumn.GetValid(_db.lookup("root_id","objective","id="+_id),0L);
                     _rootId = _id;
                    if (rootId > 0) {
                        if (_db.Objective.hasAuthorisation(DBData.AUTHORISATION.READ)) _rootId = rootId;
                        else {
                            rootId = _id;
                            while (rootId > 0) {
                                rootId = DBColumn.GetValid(_db.lookup("parent_id","objective","id="+rootId),0L);
                                if (_db.hasRowAuthorisation(DBData.AUTHORISATION.READ,"OBJECTIVE",rootId,true,false)) _rootId = rootId;
                                else break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
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
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}
