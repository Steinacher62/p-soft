namespace ch.appl.psoft.Common.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web;


    public partial class RegistryEntriesCtrl : PSOFTMapperUserControl {
        public const string PARAM_UID = "PARAM_UID";
        public const string PARAM_PARENT_ENTRIES = "PARAM_PARENT_ENTRIES";

        private DBData _db;
        private ArrayList _registryList = new ArrayList();
        private ArrayList _parentRegistryList = new ArrayList();
        private bool _hasUpdateRight = false;
        protected string _onloadString = "";


        public static string Path {
            get {return Global.Config.baseURL + "/Common/Controls/RegistryEntriesCtrl.ascx";}
        }

		#region Properities
        public string ParentEntries {
            get {return GetString(PARAM_PARENT_ENTRIES);}
            set {SetParam(PARAM_PARENT_ENTRIES, value);}
        }

        public long UID {
            get {return GetLong(PARAM_UID);}
            set {SetParam(PARAM_UID, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();

            if (!IsPostBack) {
                apply.Text = _mapper.get("apply");
            }

            _db = DBData.getDBData(Session);
            _db.connect();
            try {
                _registryList = new ArrayList(_db.Registry.getRegistryIDs(UID).Split(','));
                _parentRegistryList = new ArrayList(ParentEntries.Split(','));
                apply.Enabled = _hasUpdateRight = _db.hasUIDAuthorisation(DBData.AUTHORISATION.UPDATE, UID, DBData.APPLICATION_RIGHT.COMMON, true, true);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected string buildTree {
            get {
                Common.Tree tree = new Common.Tree("REGISTRY", Response, "");
                tree.extendNode += new ExtendNodeHandler(this.extendNode);
                tree.BranchToolTipColum = "DESCRIPTION";
                Response.Write("<script language=\"javascript\">\n");
            
                tree.build(_db, _db.Registry.getRootID(), "registryTree");
            
                return "</script>";
            }
        }
        
        private bool extendNode(HttpResponse response, string nodeName, DataRow row, int level) {
            if (_registryList.Contains(row["ID"].ToString())) {
                response.Write(nodeName+".prependHTML=\"<input type='checkbox' checked " + (_hasUpdateRight? "" : "disabled ") + "ID='RegFlag-"+row["ID"]+"'>\";\n");
                response.Write(nodeName+".setInitialOpen(1);\n");
            }
            else if (_parentRegistryList.Contains(row["ID"].ToString())) {
                response.Write(nodeName+".prependHTML = \"<input type='checkbox' checked disabled ID='RegFlag-"+row["ID"]+"'>\";\n");
                response.Write(nodeName+".setInitialOpen(1);\n");
            }
            else response.Write(nodeName+".prependHTML = \"<input type='checkbox' " + (_hasUpdateRight? "" : "disabled ") + "ID='RegFlag"+row["ID"]+"'>\";\n");
            return true;
        }

        private void mapControls () {
            apply.Click += new System.EventHandler(apply_Click);
            apply.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";
        }

        private void apply_Click(object sender, System.EventArgs e) {
            _db.connect();
            try {
                _db.beginTransaction();

                if (registryFlags.Value != "") {                    
                    _db.Registry.updateRegistryEntries(registryFlags.Value, UID);
                }     

                _db.commit();
            }
            catch (Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();   
            }

            _onloadString = "window.close();";
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
