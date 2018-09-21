namespace ch.appl.psoft.FBW.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for CatalogCtrl.
    /// </summary>
    public partial class CatalogCtrl : PSOFTDetailViewUserControl {
        public const string PARAM_KRITERIUM_ID = "PARAM_KRITERIUM_ID";
        public const string PARAM_ARGUMENT_ID = "PARAM_ARGUMENT_ID";




        protected Config _config = null;
        protected DBData _db = null;


        public static string Path {
            get {return Global.Config.baseURL + "/FBW/Controls/CatalogCtrl.ascx";}
        }

		#region Properities
        public long KriteriumID {
            get {return GetLong(PARAM_KRITERIUM_ID);}
            set {SetParam(PARAM_KRITERIUM_ID, value);}
        }

        public long ArgumentID {
            get {return GetLong(PARAM_ARGUMENT_ID);}
            set {SetParam(PARAM_ARGUMENT_ID, value);}
        }

        public string HighLightNode {
            get {
                string retValue = "";
                if (ArgumentID > 0){
                    retValue = "highlightNode(" + ArgumentID + ");";
                }
                return retValue;
            }
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _config = Global.Config;

            treeTitle.Text = _mapper.get(FBWModule.LANG_SCOPE_FBW, FBWModule.LANG_MNEMO_CT_CRITERIA_TREE);
            
            _db = DBData.getDBData(Session);
            
            try {
                _db.connect();

                if (!IsPostBack) {
                    detailTitle.Text = _mapper.get(FBWModule.LANG_SCOPE_FBW, FBWModule.LANG_MNEMO_CT_ARGUMENT_DETAIL);
                }

                if (ArgumentID > 0){
                    DataTable table = _db.getDataTableExt("select ID,BEZEICHNUNG,BESCHREIBUNG"+
                        ",ERLAEUTERUNG,FBW_REGEL_ID"+
                        ",isnull(FBW_STUFE_VORSCHLAG, isnull(STUFE_BEZEICHNUNG,STUFE_KRITERIUMRANG)) as STUFE_BEZEICHNUNG from ARGUMENTDESCRIPTIONV where ID=" + ArgumentID, "ARGUMENTDESCRIPTIONV");

                    DataTable regelTable = _db.getDataTable("select ID, BEZEICHNUNG from FBW_REGEL");
                    table.Columns["FBW_REGEL_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
                    table.Columns["FBW_REGEL_ID"].ExtendedProperties["In"] = regelTable;

                    LoadDetail(_db, table, detailTab);
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }

        }

        public string buildTree {
            get {
                if (KriteriumID > 0){
                    DBData db = DBData.getDBData(Session);

                    db.connect();
                    try {
                        long catalogID = ch.psoft.Util.Validate.GetValid(db.lookup("FBW_ARGUMENT_KATALOG_ID", "FBW_KRITERIUM", "ID=" + KriteriumID, false), -1L);

                        Common.Tree tree = new Common.Tree("FBW_ARGUMENT_KATALOG", "FBW_ARGUMENT", "FBW_ARGUMENT_KATALOG_ID", Response, "", "FBWCatalog.aspx?KriteriumID=" + KriteriumID + "&ArgumentID=%ID");
                        tree.BranchLabelColumn = "BEZEICHNUNG";
                        tree.BranchOrderColumn = "ORDNUMBER";
                        tree.LeafLabelColumn = "BEZEICHNUNG";
                        tree.LeafOrderColumn = "ORDNUMBER";
                        tree.BranchToolTipColum = "BESCHREIBUNG";
                        tree.LeafToolTipColum = "BESCHREIBUNG";
                        tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);

                        Response.Write("<script language=\"javascript\">\n");

                        string [] roots = {db.lookup("ID", "FBW_ARGUMENT_KATALOG", "ID=" + catalogID, false)};
                        long [] iRoots = new long[roots.Length];
                        string [] rootNames = new string[roots.Length];
                        int i;
                        for (i=0; i<roots.Length; i++) {
                            iRoots[i] = long.Parse(roots[i]);
                            rootNames[i] = "catalogTree" + i;
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

                return "";
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
