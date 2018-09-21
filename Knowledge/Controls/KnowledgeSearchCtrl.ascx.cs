namespace ch.appl.psoft.Knowledge.Controls
{
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using LayoutControls;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for SearchView.
    /// </summary>
    public partial  class KnowledgeSearchCtrl : PSOFTSearchUserControl {
        protected long _registryRootId = 0;
        protected string _backURL = "";

        private DBData _db = null;
        private DataTable _table = null;
        private ArrayList _selectedRegistry = new ArrayList();


		protected DropDownList typeCtrl = null;
		protected DropDownList isactiveCtrl = null;
		protected TableRow isactiveCtrlRow = null;

        public static string Path {
            get {return Global.Config.baseURL + "/Knowledge/Controls/KnowledgeSearchCtrl.ascx";}
        }
        #region Properities
        /// <summary>
        /// Get/Set back url
        /// </summary>
        public string backURL {
            get {return _backURL;}
            set {_backURL = value;}
        }

        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected string buildRegistryTree {
            get {
                Common.Tree tree = new Common.Tree("REGISTRY", Response, "");
                Response.Write("<script language=\"javascript\">\n");
                tree.extendNode += new ExtendNodeHandler(this.extendNode);
                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                tree.BranchToolTipColum = "DESCRIPTION";
                _registryRootId =  _db.Registry.getRootID();
                tree.build(_db, _registryRootId, "registryTree");
                return "</script>";
            }
        }
        
        private bool extendNode(HttpResponse response, string nodeName, DataRow row, int level) {
            string ID = row["ID"].ToString();
            if (nodeName != "registryTree") {
                if (_selectedRegistry.Contains(ID)) {
                    response.Write(nodeName+".prependHTML=\"<input type='checkbox' checked ID='RegFlag" + ID + "'>\";\n");
                    response.Write(nodeName+".setInitialOpen(1);\n");
                }
                else{
                    response.Write(nodeName+".prependHTML = \"<input type='checkbox' ID='RegFlag" + ID + "'>\";\n");
                }
            }
            return true;
        }
        
        protected override void DoExecute() {
            base.DoExecute ();
            this.Page.Form.DefaultButton = apply.UniqueID;
            if (!IsPostBack) {
                apply.Text = _mapper.get("search"); 
                registryRelation.Items.Add(new ListItem(_mapper.get("or"),"or"));
                registryRelation.Items.Add(new ListItem(_mapper.get("and"),"and"));
                registryRelation.ToolTip = _mapper.get("registry","registryRelationTP");
                registryRelation.SelectedIndex = 0;
            }//if
			
			if (ViewState["KnowledgeSearchSelectedRegistry"] != null) {
				_selectedRegistry =  ViewState["KnowledgeSearchSelectedRegistry"] as ArrayList;
			} else { 
				ViewState.Add("KnowledgeSearchSelectedRegistry",_selectedRegistry);
			}//if

            _db = DBData.getDBData(Session);
            _db.connect();
			
			try{
                loadDetail();
            } finally {
                _db.disconnect();
            }//try
        }//DoExecute

        private void loadDetail() {
            // load details
            detailTab.Rows.Clear();
            _table = _db.getDataTableExt("select * from KNOWLEDGE_V where ID=-1 and LOCAL = 0","KNOWLEDGE_V");
           
            DataTable personTab = _db.Person.getWholeNameMATable(false);
            
            _table.Columns[_db.langAttrName(Knowledge._VIEWNAME, "CREATOR_PERSON_ID")].ExtendedProperties["In"] = personTab;
            _table.Columns[_db.langAttrName(Knowledge._VIEWNAME, "CREATOR_PERSON_ID")].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
            
	
			if(Global.Config.isModuleEnabled("suggestion")) {
                _table.Columns["TYPE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.SEARCH;
                _table.Columns["TYPE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["TYPE"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("KNOWLEDGE", "types", true));

				_table.Columns["ISACTIVE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.SEARCH;
				_table.Columns["ISACTIVE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
				_table.Columns["ISACTIVE"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("KNOWLEDGE", "isActiveSuggestion", true));		   
            }//if
            
			base.CheckOrder = true; // the input bilder must care about the order of the fields.
            base.LoadInput(_db, _table, detailTab);
        }
       
        private void mapControls() { 
            apply.Click += new System.EventHandler(apply_Click);
            apply.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";
        }

        /// <summary>
        /// Suche ausführen. Werden im Registraturbaum Checkboxen eingefüllt,
        /// wird über die Registratur gesucht. Die eingegebenen Suchkriterien
        /// schränken das Resultat zusätzlich ein.
        /// Ohne Eingaben in den Checkboxen wird nur mit Hilfe der Suchkriterien
        /// gesucht.
        /// </summary>
        private void apply_Click(object sender, System.EventArgs e) {
			if (!base.checkInputValue(_table,detailTab)) {
				return;
			}//if

            string sql = getSql(_table, detailTab, false, false).ToString();
            if (sql == "")
            {
                sql = "select * from KNOWLEDGE_V where LOCAL = 0 ";
            }
            else
            {
                sql += " and LOCAL = 0";
            }

			if(Global.Config.isModuleEnabled("suggestion")) {
				if(sql.IndexOf("TYPE = 0") > 0) {
					isactiveCtrlRow.Style["display"] = "none";
					isactiveCtrl.Enabled = false;
					isactiveCtrl.SelectedIndex = 0;
				} else {
					isactiveCtrlRow.Style["display"] = "block";
					isactiveCtrl.Enabled = true;
				}//if
			}//if

            string[] regIds = registryFlags.Value.Split(',');
            bool hasWhere = false;
            
				_selectedRegistry.Clear();
            if (registryFlags.Value != "") { // nur falls Checkboxen angeklickt wurden wird die Registratur berücksichtigt.
                _db.connect();
                try {
                    hasWhere = sql.IndexOf(" where ") > 0;
                    // falls über die Registratur nichts gefunden wird, soll die Liste leer bleiben
                    if (registryRelation.SelectedItem.Value == "or" || regIds.Length <= 1) {
                        string registered = _db.Registry.getRegisteredIDs(registryFlags.Value, "KNOWLEDGE", -1);
                        if (!hasWhere){
                            sql += " where ";
                            hasWhere = true;
                        }
                        else{
                            sql += " and ";
                        }
                        sql += "ID in (" + registered + ")";
                    }
                    else {
                        foreach (string regId in regIds) {
                            string registered = _db.Registry.getRegisteredIDs(regId, "KNOWLEDGE", -1);
                            if (!hasWhere){
                                sql += " where ";
                                hasWhere = true;
                            }
                            else{
                                sql += " and ";
                            }
                            sql += "ID in (" + registered + ")";
                            if (registered == "-1") break;
                        }
                    }				
                }
                catch (Exception error) {
                    Logger.Log(error, Logger.ERROR);
                    DoOnException(error);
                }
                finally {
                    _db.disconnect();
                }
                foreach (string regId in regIds){
                    _selectedRegistry.Add(regId);
                }
            }
            hasWhere = sql.IndexOf(" where ") > 0;
			
			if (!hasWhere)
			{
				sql += " where ";
				hasWhere = true;
			}
			else
			{
				sql += " and ";
			}

			// nur die aktuellsten Versionen anzeigen
			sql += "HISTORY_ROOT_ID is null";
            
            // Setting search event args
            _searchArgs.ReloadList = true;
            _searchArgs.SearchSQL = sql;

            DoOnSearchClick(apply);
           
        }

        protected override void onAddProperty (DataRow row, DataColumn col, TableRow r) 
        {
            if (col != null)
            {
                switch(col.ColumnName)
                {
                    case "TYPE":
                    {
                        System.Web.UI.Control ctrl = r.Cells[2].Controls[0];
                        if (ctrl is DropDownList) 
                        {
							typeCtrl = (DropDownList)ctrl;
                        }
                    }
                        break;
                    case "ISACTIVE":
                    {
                        System.Web.UI.Control ctrl = r.Cells[2].Controls[0];
                        if (ctrl is DropDownList) 
                        {
							isactiveCtrl = (DropDownList)ctrl;
							isactiveCtrlRow = r;
                        }
                    }
                        break;                       
                }

				if(typeCtrl != null && isactiveCtrl != null) 
				{
					typeCtrl.SelectedIndex = 1;
					typeCtrl.Attributes["onchange"] = "viewIsActiveDropDown('" + typeCtrl.ClientID + "','" + isactiveCtrl.ClientID + "')";
					isactiveCtrl.SelectedIndex = 0;
					isactiveCtrl.Enabled = false;
					isactiveCtrlRow.Style["display"] = "none";
				}
            }//if
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
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

		}
		#endregion
    }
}
