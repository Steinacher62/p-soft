namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;


    public partial class PhaseEditCtrl : PSOFTInputViewUserControl
	{
        public const string PARAM_PHASE_ID = "PARAM_PHASE_ID";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";

        private DataTable _table;
        private DBData _db;
        private ArrayList _registryList = new ArrayList();
        private ArrayList _parentRegistryList = new ArrayList();


		public static string Path
		{
			get {return Global.Config.baseURL + "/Project/Controls/PhaseEditCtrl.ascx";}
		}

		#region Properities
        public string NextURL
        {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public long PhaseID
        {
            get {return GetLong(PARAM_PHASE_ID);}
            set {SetParam(PARAM_PHASE_ID, value);}
        }
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}

		protected override void DoExecute()
		{
			base.DoExecute ();
            InputType = InputMaskBuilder.InputType.Edit;

            if (!IsPostBack)
            {
                apply.Text = _mapper.get("apply");
            }

            _db = DBData.getDBData(Session);
			try 
			{
				_db.connect();

				_table = _db.getDataTableExt("select * from PHASE where ID=" + PhaseID, "PHASE");

                _table.Columns["PROJECT_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                _table.Columns["PROJECT_ID"].ExtendedProperties["In"] = _db.getDataTable("select ID, TITLE from PROJECT");
                _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["STATE"].ExtendedProperties["In"] = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PHASE);

                DataTable personTable = _db.Person.getWholeNameMATable(false);
                _table.Columns["LEADER_PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["LEADER_PERSON_ID"].ExtendedProperties["In"] = personTable;

                _table.Columns["HAS_MILESTONE"].ExtendedProperties["InputControlType"] = typeof(CheckBox);

                LoadInput(_db, _table, editTab);

                _registryList = new ArrayList(_db.Registry.getRegistryIDs(PhaseID.ToString(), "PHASE").Split(','));
                if (_table.Rows.Count > 0) {
                    long projectID = DBColumn.GetValid(_table.Rows[0]["PROJECT_ID"], -1L);
                    if (projectID > 0){
                        string path = SQLDB.BuildSQLArray(_db.Project.getParentProjectIDList(projectID, true).ToArray());
                        _parentRegistryList = new ArrayList(_db.Registry.getRegistryIDs(path, "PROJECT").Split(','));
                    }
                }
            }
			catch (Exception ex) 
			{
				DoOnException(ex);
			}
			finally 
			{
				_db.disconnect();
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
            if (_registryList.Contains(row["ID"].ToString())) {
                response.Write(nodeName+".prependHTML=\"<input type='checkbox' checked ID='RegFlag-"+row["ID"]+"'>\";\n");
                response.Write(nodeName+".setInitialOpen(1);\n");
            }
            else if (_parentRegistryList.Contains(row["ID"].ToString())) {
                response.Write(nodeName+".prependHTML = \"<input type='checkbox' checked disabled ID='RegFlag-"+row["ID"]+"'>\";\n");
                response.Write(nodeName+".setInitialOpen(1);\n");
            }
            else response.Write(nodeName+".prependHTML = \"<input type='checkbox' ID='RegFlag"+row["ID"]+"'>\";\n");
            return true;
        }

        private void mapControls () 
		{
			apply.Click += new System.EventHandler(apply_Click);
            apply.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";
        }

		private void apply_Click(object sender, System.EventArgs e) 
		{
            if (checkInputValue(_table, editTab))
            {
                _db.connect();
                try 
                {
                    _db.beginTransaction();
                    StringBuilder sb = getSql(_table, editTab, true);

                    string sql = base.endExtendSql(sb);
                    if (sql.Length > 0){
                        _db.execute(sql);
                        _db.executeProcedure("MODIFYTABLEROW",
                            new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                            new ParameterCtx("USERID",_db.userId),
                            new ParameterCtx("TABLENAME","PHASE"),
                            new ParameterCtx("ROWID",PhaseID),
                            new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                            new ParameterCtx("INHERIT",1)
                            );
                    }

                    // registry
                    if (registryFlags.Value != "") {                    
                        _db.Registry.updateRegistryEntries(registryFlags.Value, "PHASE", PhaseID);
                    }     

                    _db.commit();
                }
                catch (Exception ex) 
                {
                    _db.rollback();
                    DoOnException(ex);
                }
                finally 
                {
                    _db.disconnect();   
                }

                ((PsoftContentPage) Page).RemoveBreadcrumbItem();
                if (NextURL != ""){
                    Response.Redirect(NextURL);
                }
                else{
                    ((PsoftContentPage) Page).RedirectToPreviousPage();
                }
            }		
        }

        #region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{

        }
		#endregion
	}
}
