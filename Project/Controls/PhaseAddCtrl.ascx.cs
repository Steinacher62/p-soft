namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;


    public partial class PhaseAddCtrl : PSOFTInputViewUserControl {
        public const string PARAM_PROJECT_ID = "PARAM_PROJECT_ID";

        protected DataTable _table;
        protected DBData _db = null;
        private ArrayList _parentRegistryList = new ArrayList();

        protected System.Web.UI.HtmlControls.HtmlInputFile inputFileName = new System.Web.UI.HtmlControls.HtmlInputFile();

        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/PhaseAddCtrl.ascx";}
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
            base.DoExecute ();
            _db = DBData.getDBData(Session);
            string sql = "select * from PHASE where ID=-1";

            _db.connect();
            try {
                if (!IsPostBack) {
                    apply.Text = _mapper.get("apply");
                }
                _table = _db.getDataTableExt(sql,"PHASE");

                _table.Columns["PROJECT_ID"].ExtendedProperties["InputControlType"] = typeof(Label);

                _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["STATE"].ExtendedProperties["In"] = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PHASE);
                
                DataTable personTable = _db.Person.getWholeNameMATable(false);
                _table.Columns["LEADER_PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["LEADER_PERSON_ID"].ExtendedProperties["In"] = personTable;
                _table.Columns["HAS_MILESTONE"].ExtendedProperties["InputControlType"] = typeof(CheckBox);

                LoadInput(_db,_table,addTab);
                setInputValue(_table, addTab, "LEADER_PERSON_ID", _db.userId.ToString());

                string path = SQLDB.BuildSQLArray(_db.Project.getParentProjectIDList(ProjectID, true).ToArray());
                _parentRegistryList = new ArrayList(_db.Registry.getRegistryIDs(path, "PROJECT").Split(','));
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) {
            if (col != null && col.ColumnName == "PROJECT_ID") {
                Label lbl = (Label) r.Cells[1].Controls[0];
                lbl.Text = _db.lookup("TITLE", "PROJECT", "ID=" + ProjectID, false);
            }
        }

        protected string buildTree {
            get {
                Common.Tree tree = new Common.Tree("REGISTRY", Response, "");
                tree.extendNode += new ExtendNodeHandler(this.extendNode);
                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                tree.BranchToolTipColum = "DESCRIPTION";
                Response.Write("<script language=\"javascript\">\n");
            
                tree.build(_db, _db.Registry.getRootID(), "registryTree");
            
                return "</script>";
            }
        }

        private bool extendNode(HttpResponse response, string nodeName, DataRow row, int level) {
            if (_parentRegistryList.Contains(row["ID"].ToString())) {
                response.Write(nodeName+".prependHTML = \"<input type='checkbox' checked disabled ID='RegFlag-"+row["ID"]+"'>\";\n");
                response.Write(nodeName+".setInitialOpen(1);\n");
            }
            else response.Write(nodeName+".prependHTML = \"<input type='checkbox' ID='RegFlag"+row["ID"]+"'>\";\n");
            return true;
        }

        private void mapControls () {
            apply.Click += new System.EventHandler(apply_Click);
            apply.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";
        }

        private void apply_Click(object sender, System.EventArgs e) {
            if (!base.checkInputValue(_table,addTab)){
                return;
            }
            
            long newID = -1;

            _db.connect();
            try {
                _db.beginTransaction();
                StringBuilder sb = getSql(_table, addTab, true);
                newID = _db.newId(_table.TableName);

                extendSql(sb, _table, "ID", newID);
                extendSql(sb, _table, "TEMPLATE", "0");

                extendSql(sb, _table, "PROJECT_ID", ProjectID);

                string sql = endExtendSql(sb);

                if (sql != "") {
                    _db.execute(sql);

                    // registry
                    if (registryFlags.Value != "") {
                        _db.Registry.updateRegistryEntries(registryFlags.Value, _table.TableName, newID);
                    }

                    //grant base rights...
                    _db.Phase.setDefaultRights(newID);

					_db.Project.createMessage(ProjectID, (int)News.ACTION.EDIT, true);

                    _db.commit();

                    Response.Redirect(psoft.Project.ProjectDetail.GetURL("ID",ProjectID), false);
                }
                else
                    _db.rollback();
            }
            catch (Exception ex) {
                _db.rollback();
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
