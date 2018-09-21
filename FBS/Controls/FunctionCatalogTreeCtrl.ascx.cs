namespace ch.appl.psoft.FBS.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;


    public partial class FunctionCatalogTreeCtrl : PSOFTDetailViewUserControl {

        protected long _highlightId = 0;


        public static string Path {
            get {return Global.Config.baseURL + "/FBS/Controls/FunctionCatalogTreeCtrl.ascx";}
        }

		#region Properities
        public long id {
            set {_highlightId = value;}
            get {return _highlightId;}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected string buildTree {
            get {
                string branchNodeUrl = "";
                string leafNodeUrl = Global.Config.baseURL + "/FBS/FunctionCatalogTree.aspx?id=%ID";
                Common.Tree tree = new Common.Tree("FUNKTION_GROUP","FUNKTION","FUNKTION_GROUP_ID", Response, branchNodeUrl,leafNodeUrl);
                DBData db = DBData.getDBData(Session);

                Response.Write("<script language=\"javascript\">\n");
                db.connect();
                try {
                    long root = DBColumn.GetValid(db.lookup("ID","FUNKTION_GROUP","PARENT_ID IS NULL"),0L);
                    tree.BranchOrderColumn = db.langAttrName("FUNKTION_GROUP","TITLE");
                    tree.LeafOrderColumn = db.langAttrName("FUNKTION","TITLE");
                    tree.BranchToolTipColum = "DESCRIPTION";
                    tree.LeafToolTipColum = "DESCRIPTION";
                    tree.build(db,root,"functionCatalog");
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
