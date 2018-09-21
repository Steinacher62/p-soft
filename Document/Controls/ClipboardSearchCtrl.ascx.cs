namespace ch.appl.psoft.Document.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web;
    using System.Web.UI.WebControls;

    public partial class ClipboardSearchCtrl : PSOFTSearchUserControl {
        public const string PARAM_INCLUDE_LINKS = "PARAM_INCLUDE_LINKS";
        public const string PARAM_INCLUDE_TEMPLATES = "PARAM_INCLUDE_TEMPLATES";

        private DataTable _table;

        protected System.Web.UI.WebControls.Table Table1;
        protected TextBox _tbContents = new TextBox();
        protected DBData _db = null;
        protected ArrayList _selectedRegistry = new ArrayList();
        protected BitsetCtrl _bsCategory = new BitsetMapCtrl();
        protected ArrayList _categoriesAll;
        protected Config _config = null;

        protected static int CATEGORY_COUNT = 7;

        public static string Path {
            get {return Global.Config.baseURL + "/Document/Controls/ClipboardSearchCtrl.ascx";}
        }

		#region Properities
        public bool IncludeLinks {
            get {return GetBool(PARAM_INCLUDE_LINKS);}
            set {SetParam(PARAM_INCLUDE_LINKS, value);}
        }

        public bool IncludeTemplates {
            get {return GetBool(PARAM_INCLUDE_TEMPLATES);}
            set {SetParam(PARAM_INCLUDE_TEMPLATES, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();
            _db = DBData.getDBData(Session);
            _config = Global.Config;

            try {
                if (!IsPostBack) {
                    apply.Text = _mapper.get("search");
                    apply.Attributes.Add("onclick", "readRegistryFlag(" + registryFlags.ClientID + ");if (top.showProgressBarDelayed) top.showProgressBarDelayed(1500);");
                    registryRelation.Items.Add(new ListItem(_mapper.get("or"),"or"));
                    registryRelation.Items.Add(new ListItem(_mapper.get("and"),"and"));
                    registryRelation.ToolTip = _mapper.get("registry","registryRelationTP");
                    registryRelation.SelectedIndex = 0;
                }

                if (ViewState["DocSearchSelectedRegistry"] != null) _selectedRegistry = ViewState["DocSearchSelectedRegistry"] as ArrayList;
                else ViewState.Add("DocSearchSelectedRegistry",_selectedRegistry);

                
                _db.connect();
                _table = _db.getDataTableExt("select * from CLIPBOARD where ID=-1", "CLIPBOARD");

                LoadInput(_db, _table, searchTab);

				/*
                // Field for content-search
                TableRow row = new TableRow();
                searchTab.Rows.Add(row);
                TableCell cell = new TableCell();
                row.Cells.Add(cell);
                cell.Text = _mapper.get("document", "contents");
                cell = new TableCell();
                row.Cells.Add(cell);
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(_tbContents);
                _tbContents.Columns = 30;
                _tbContents.ID = "tbContents";
                _tbContents.EnableViewState = true;

                // Field for category check boxes
                row = new TableRow();
                searchTab.Rows.Add(row);
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Text = _mapper.get("document", "category");
                cell.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;
                cell = new TableCell();
                row.Cells.Add(cell);
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(_bsCategory);
                _bsCategory.repeatDirection = RepeatDirection.Horizontal;
                _bsCategory.numberOfGroupItems = 1;
                _bsCategory.columnWidth = 110;
                _categoriesAll = new ArrayList(_mapper.getEnum("document","documentCategory",true));
                Hashtable categoriesHash = new Hashtable(CATEGORY_COUNT);
                categoriesHash.Add(Document.DocCategory.General,"enabled");
                categoriesHash.Add(Document.DocCategory.Organisation,"enabled");
                categoriesHash.Add(Document.DocCategory.Employee,"enabled");
                if (_config.isModuleEnabled("knowledge"))
                    categoriesHash.Add(Document.DocCategory.Knowledge,"enabled");
                if (_config.isModuleEnabled("tasklist"))
                    categoriesHash.Add(Document.DocCategory.Tasklist,"enabled");
                if (_config.isModuleEnabled("project"))
                    categoriesHash.Add(Document.DocCategory.Project,"enabled");
                if (_config.isModuleEnabled("contact"))
                    categoriesHash.Add(Document.DocCategory.Contact,"enabled");
                for (int i=0; _categoriesAll.Count > i; i++)
                {                   
                    object obj = _categoriesAll[i];
                    if (obj is string)
                    {
                        if (categoriesHash.ContainsKey((Document.DocCategory)i))
                            _bsCategory.setBit(i,obj.ToString(),false);
                        
                    }
                }
				*/
 

            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        private void mapControls () {
            apply.Click += new System.EventHandler(apply_Click);
        }

        private string sqlAppendWhere(string sql, string clause) {
            sql += ((sql.ToLower().IndexOf(" where ") > 0) ? " and " : " where ") + clause;
            return sql;
        }

        private string sqlAppendCategory(string sql, string clause)
        {
            sql += (sql == "") ? clause : " or " + clause;
            return sql;
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
		
        private void apply_Click(object sender, System.EventArgs e) {
            if (checkInputValue(_table, searchTab)){
                string sql = getSql(_table, searchTab);
                if (sql == ""){
                    sql = "select * from CLIPBOARD";
                }
                string[] regIds = registryFlags.Value.Split(',');
                ArrayList cbList = new ArrayList(_bsCategory.items);

                _selectedRegistry.Clear();

                if (registryFlags.Value != "") { // consider registry only if a checkbox is selected
                    _db.connect();
                    try {
                        if (registryRelation.SelectedItem.Value == "or" || regIds.Length <= 1) {
                            string registeredFolderIDs = _db.Folder.getRegisteredIDs(registryFlags.Value, -1);

                            registeredFolderIDs = _db.Folder.addAllSubFolders(registeredFolderIDs);
                            string registeredIDs = _db.Document.getRegisteredIDs(registryFlags.Value, -1);
                            sql = sqlAppendWhere(sql, "(ID in (" + registeredIDs + ") or FOLDER_ID in (" + registeredFolderIDs + "))");
                        }
                        else {
                            foreach (string regId in regIds) {
                                string registeredFolderIDs = _db.Folder.getRegisteredIDs(regId, -1);
                                registeredFolderIDs = _db.Folder.addAllSubFolders(registeredFolderIDs);
                                string registeredIDs = _db.Document.getRegisteredIDs(regId, -1);
                                sql = sqlAppendWhere(sql, "(ID in (" + registeredIDs + ") or FOLDER_ID in (" + registeredFolderIDs + "))");
                                if (registeredFolderIDs == "-1" && registeredIDs == "-1") break;
                            }
                        }
                    }
                    catch (Exception ex) {
                        DoOnException(ex);
                    }
                    finally {
                        _db.disconnect();
                    }
                    foreach (string regId in regIds){
                        _selectedRegistry.Add(regId);
                    }
                }
                if (!IncludeLinks){
                    sql = sqlAppendWhere(sql, "TYP<>" + (int)Document.DocType.Document_Link);
                }
                // categories
                string part = "";
                string table = "";
                string sqlCategory = "";
                for (int i=0; cbList.Count > i && CATEGORY_COUNT > i; i++)
                {
                    if (cbList[i] is CheckBox)
                    {
                        CheckBox cb = (CheckBox)cbList[i];
                        if (cb.Checked)
                        {
                            switch (i)
                            {
                                case (int) Document.DocCategory.General:
                                    part = "FOLDER_ID in (select ID from FOLDER where ROOT_ID in (select FOLDER_ID from CLIPBOARD where TYP=0))";
                                    break;
                                case (int)Document.DocCategory.Organisation:
                                    table = "ORGENTITY";
                                    part = "FOLDER_ID in (select ID from FOLDER where ROOT_ID in (select FOLDER_ID from CLIPBOARD CB inner join " + table + " O on CB.ID = O.CLIPBOARD_ID))";
                                    break;
                                case (int)Document.DocCategory.Employee:
                                    table = "PERSON";
                                    part = "FOLDER_ID in (select ID from FOLDER where ROOT_ID in (select FOLDER_ID from CLIPBOARD CB inner join " + table + " O on CB.ID = O.CLIPBOARD_ID and (O.TYP & 3)=1))";
                                    break;
                                case (int)Document.DocCategory.Knowledge:
                                    part = "FOLDER_ID is null"; //Eindeutiger wäre 'KNOWLEDGE_ID is not null', aber View enthält Spalte nicht.
                                    break;
                                case (int)Document.DocCategory.Tasklist:
                                    table = "MEASURE";
                                    part = "FOLDER_ID in (select ID from FOLDER where ROOT_ID in (select FOLDER_ID from CLIPBOARD CB inner join " + table + " O on CB.ID = O.CLIPBOARD_ID)";
                                    table = "TASKLIST";
                                    part += " or ROOT_ID in (select FOLDER_ID from CLIPBOARD CB inner join " + table + " O on CB.ID = O.CLIPBOARD_ID))";
                                    break;
                                case (int)Document.DocCategory.Project:
                                    table = "PROJECT";
                                    part = "FOLDER_ID in (select ID from FOLDER where ROOT_ID in (select FOLDER_ID from CLIPBOARD CB inner join " + table + " O on CB.ID = O.CLIPBOARD_ID))";
                                    break;
                                case (int)Document.DocCategory.Contact:
                                    table = "PERSON";
                                    part = "FOLDER_ID in (select ID from FOLDER where ROOT_ID in (select FOLDER_ID from CLIPBOARD"+
                                        " where ID in (select CLIPBOARD_ID from "+table+" where uid in (select uid from CONTACTV where TABLENAME = '"+table+"') and CLIPBOARD_ID is not null)";
                                    table = "FIRM";
                                    part += " or ID in (select CLIPBOARD_ID from "+table+" where uid in (select uid from CONTACTV where TABLENAME = '"+table+"') and CLIPBOARD_ID is not null)))";
                                    break;
                            }
                            sqlCategory = sqlAppendCategory(sqlCategory, part);
                        }
                    }
                }
                if (sqlCategory != "")
                {
                    sql = sqlAppendWhere(sql, "("+sqlCategory+")");
                }

                if (!IncludeTemplates){
                    sql = sqlAppendWhere(sql, "(FOLDER_ID is null or FOLDER_ID in (select ID from FOLDER where ROOT_ID in (select FOLDER_ID from CLIPBOARD where TEMPLATE=0)))");
                }

                // Setting search event args
                _searchArgs.ReloadList = true;
                _searchArgs.SearchSQL = sql;
                _searchArgs.CustomData = _tbContents.Text;

                DoOnSearchClick(apply);
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {    
            this.ID = "Search";

        }
		#endregion
    }
}