namespace ch.appl.psoft.Training.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;


    public partial class TrainingCatalogTreeCtrl : PSOFTMapperUserControl {
        public const string PARAM_ADVANCEMENT_ID = "PARAM_ADVANCEMENT_ID";
        public const string PARAM_TRAINING_ID = "PARAM_TRAINING_ID";
        public const string PARAM_LEAF_NODE_URL = "PARAM_LEAF_NODE_URL";
        public const string PARAM_BRANCH_NODE_URL = "PARAM_BRANCH_NODE_URL";
        public const string PARAM_PERSERVESTATE = "PARAM_PERSERVESTATE";

        private Interface.DBObjects.Tree _dbTree = null;


        public static string Path {
            get {return Global.Config.baseURL + "/Training/Controls/TrainingCatalogTreeCtrl.ascx";}
        }

		#region Properities
        public long AdvancementID {
            get {return GetLong(PARAM_ADVANCEMENT_ID);}
            set {SetParam(PARAM_ADVANCEMENT_ID, value);}
        }
        public long TrainingID 
        {
            get {return GetLong(PARAM_TRAINING_ID);}
            set {SetParam(PARAM_TRAINING_ID, value);}
        }
        public String LeafNodeUrl { 
            get {return GetString(PARAM_LEAF_NODE_URL);}
            set {SetParam(PARAM_LEAF_NODE_URL, value);}
        }
        public String BranchNodeUrl { 
            get {return GetString(PARAM_BRANCH_NODE_URL);}
            set {SetParam(PARAM_BRANCH_NODE_URL, value);}
        }
        public string HighLightNode 
        {
            get 
            {
                string retValue = "";
                long trainingID = -1;
                if (TrainingID > 0)
                {
                    trainingID = TrainingID;
                }
                else if (AdvancementID > 0)
                {
                    trainingID = DBData.getDBData(Session).Training.getTrainingID(AdvancementID);
                }
                if (trainingID > 0) 
                {
                    if (_dbTree == null)
                    {
                        loadDBTree();
                    }
                    if (BranchNodeUrl != "" || _dbTree.getRoot(trainingID) != trainingID)
                    {
                        retValue = "highlightNode(" + trainingID + ");";
                    }
                }
                return retValue;
            }
        }

        public int PerserveState 
        {
            get {return GetInt(PARAM_PERSERVESTATE);}
            set {SetParam(PARAM_PERSERVESTATE, value);}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected string buildTree {
            get {
                Common.Tree tree = new Common.Tree("TRAININGGROUP", "TRAINING", "TRAININGGROUP_ID", Response, BranchNodeUrl,LeafNodeUrl);
                tree.extendedLeafRestriction = " ((VALID_FROM<=GetDate() and (VALID_TO>=GetDate() or VALID_TO is null)) or ID=" + TrainingID + ")";
                tree.BranchOrderColumn = "ORDNUMBER, ID";
                tree.LeafOrderColumn = "ORDNUMBER, ID";
                tree.BranchToolTipColum = "DESCRIPTION";
                tree.LeafToolTipColum = "DESCRIPTION";
                DBData db = DBData.getDBData(Session);

                Response.Write("<script language=\"javascript\">\n");
                try 
                {
                    db.connect();
                    string [] roots = {db.lookup("ID", "TRAININGGROUP", "PARENT_ID is null", false)};
                    long [] iRoots = new long[roots.Length];
                    string [] rootNames = new string[roots.Length];
                    int i;
                    for (i=0; i<roots.Length; i++) 
                    {
                        iRoots[i] = long.Parse(roots[i]);
                        rootNames[i] = "trainingGroupTree" + i;
                    }
                    tree.build(db,iRoots,rootNames);
                }
                catch (Exception ex) 
                {
                    DoOnException(ex);
                }
                finally 
                {
                    db.disconnect();
                }
                return "</script>";
            }
        }


        protected override void DoExecute() {
            base.DoExecute ();

            if (!IsPostBack) 
            {
                TrainingGroupTreeTitle.Text = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_CT_TRAININGGROUP_TREE);
                loadDBTree();
            }
         }

        private void loadDBTree()
        {
            _dbTree = DBData.getDBData(Session).Tree("TRAININGGROUP");
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
