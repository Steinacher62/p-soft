namespace ch.appl.psoft.FBS.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;


    public partial class DutyCatalogTreeCtrl : PSOFTMapperUserControl {
        public const string PARAM_ADVANCEMENT_ID = "PARAM_ADVANCEMENT_ID";
        public const string PARAM_DUTYVALIDITY_ID = "PARAM_DUTYVALIDITY_ID";
        public const string PARAM_LEAF_NODE_URL = "PARAM_LEAF_NODE_URL";
        public const string PARAM_BRANCH_NODE_URL = "PARAM_BRANCH_NODE_URL";
        public const string PARAM_PERSERVESTATE = "PARAM_PERSERVESTATE";

        private Interface.DBObjects.Tree _dbTree = null;


        public static string Path {
            get {return Global.Config.baseURL + "/FBS/Controls/DutyCatalogTreeCtrl.ascx";}
        }

		#region Properities
        public long DutyValidityID {
            get {return GetLong(PARAM_DUTYVALIDITY_ID);}
            set {SetParam(PARAM_DUTYVALIDITY_ID, value);}
        }

        public String LeafNodeUrl { 
            get {return GetString(PARAM_LEAF_NODE_URL);}
            set {SetParam(PARAM_LEAF_NODE_URL, value);}
        }

        public String BranchNodeUrl { 
            get {return GetString(PARAM_BRANCH_NODE_URL);}
            set {SetParam(PARAM_BRANCH_NODE_URL, value);}
        }

        public string HighLightNode {
            get {
                string retValue = "";
                if (DutyValidityID > 0) {
                    if (_dbTree == null) {
                        loadDBTree();
                    }
                    if (BranchNodeUrl != "" || _dbTree.getRoot(DutyValidityID) != DutyValidityID) {
                        retValue = "highlightNode(" + DutyValidityID + ");";
                    }
                }
                return retValue;
            }
        }

        protected string deleteMessage {
            get { return _mapper.get("MESSAGES", "deleteConfirm"); }
        }

        public int PerserveState {
            get {return GetInt(PARAM_PERSERVESTATE);}
            set {SetParam(PARAM_PERSERVESTATE, value);}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected string buildTree {
            get {
                Common.Tree tree = new Common.Tree("DUTYGROUP", "DUTY_VALIDITY_V", "DUTYGROUP_ID", Response, BranchNodeUrl, LeafNodeUrl);
                tree.extendedLeafRestriction = " (VALID_FROM<=GetDate() and (VALID_TO>=GetDate() or VALID_TO is null))";
                tree.BranchOrderColumn = "ORDNUMBER, ID";
                tree.LeafOrderColumn = "ORDNUMBER, ID";
                tree.BranchToolTipColum = "DESCRIPTION";
                tree.LeafToolTipColum = "DESCRIPTION";
                DBData db = DBData.getDBData(Session);

                Response.Write("<script language=\"javascript\">\n");
                try {
                    db.connect();
                    string [] roots = {db.lookup("ID", "DUTYGROUP", "PARENT_ID is null", false)};
                    long [] iRoots = new long[roots.Length];
                    string [] rootNames = new string[roots.Length];
                    int i;
                    for (i=0; i<roots.Length; i++) {
                        iRoots[i] = long.Parse(roots[i]);
                        rootNames[i] = "dutyGroupTree" + i;
                    }
                    tree.build(db,iRoots,rootNames);
                }
                catch (Exception ex) {
                    DoOnException(ex);
                }
                finally {
                    db.disconnect();
                }
                return "</script>";
            }
        }


        protected override void DoExecute() {
            base.DoExecute ();

            if (!IsPostBack) {
                DutyGroupTreeTitle.Text = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CT_DUTYGROUP_TREE);
                loadDBTree();
            }
        }

        private void loadDBTree() {
            _dbTree = DBData.getDBData(Session).Tree("DUTYGROUP");
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
