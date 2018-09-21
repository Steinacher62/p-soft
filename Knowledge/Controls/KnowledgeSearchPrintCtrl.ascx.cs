namespace ch.appl.psoft.Knowledge.Controls
{
    using Common;
    using db;
    using Interface.DBObjects;
    using LayoutControls;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for SearchView.
    /// </summary>
    public partial  class KnowledgeSearchPrintCtrl : PSOFTSearchUserControl {
        protected long _registryRootId = 0;
        protected string _backURL = "";

        private DBData _db = null;
        private DataTable _table = null;
        private ArrayList _selectedRegistry = new ArrayList();
        private bool _union = true;


        //Used for dynamic drop-down
        protected DropDownList typeCtrl = null;
        protected DropDownList isactiveCtrl = null;
        protected TableRow isactiveCtrlRow = null;
        //

        public bool IsCheckBoxKnowledgeChecked 
        {
            get { return this.checkBoxKnowledge.Checked; }
        }


        public static string Path {
            get {return Global.Config.baseURL + "/Knowledge/Controls/KnowledgeSearchPrintCtrl.ascx";}
        }
        #region Properties
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

    
        protected override void DoExecute() {
            base.DoExecute ();
            this.Page.Form.DefaultButton = apply.UniqueID;
            if (!IsPostBack) {
                apply.Text = _mapper.get("search"); 
                this.checkBoxKnowledge.Checked = true;
                this.checkBoxKnowledge.Text = _mapper.get("knowledge", "cbKnowledgeOnly");
                // add "and" and "or"
                registryRelation.Items.Add(new ListItem(_mapper.get("or"),"or"));
                registryRelation.Items.Add(new ListItem(_mapper.get("and"),"and"));
                registryRelation.ToolTip = _mapper.get("knowledge","ttAndOder");
                registryRelation.SelectedIndex = 0; 
            }
            _db = DBData.getDBData(Session);
            _db.connect();
            try{
                loadDetail();
            }
            finally{
                _db.disconnect();
            }
        }

        private void loadDetail() {
            // load details
            detailTab.Rows.Clear();

            String tablename = Knowledge._VIEWNAME; 
            _table = _db.getDataTableExt("select * from KNOWLEDGE_V where ID=-1","KNOWLEDGE_V");

            _table.Columns[_db.langAttrName(tablename, "CREATED")].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            
            _table.Columns[_db.langAttrName(tablename, "CREATOR_PERSON_ID")].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.INVISIBLE;
            _table.Columns[_db.langAttrName(tablename, "DESCRIPTION")].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.INVISIBLE;
            
            DataTable themeTypeTab = _db.getDataTable("select ID, title from THEMETYPE ORDER by TITLE");

            _table.Columns[_db.langAttrName(tablename, "THEMETYPE_ID")].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.SEARCH + (int) DBColumn.Visibility.EDIT;
            _table.Columns[_db.langAttrName(tablename, "THEMETYPE_ID")].ExtendedProperties["In"] = themeTypeTab;
            _table.Columns[_db.langAttrName(tablename, "THEMETYPE_ID")].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

            if(Global.Config.isModuleEnabled("suggestion")) 
            {
                _table.Columns["TYPE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.SEARCH; 
                _table.Columns["TYPE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["TYPE"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("KNOWLEDGE", "types", true));

                _table.Columns["ISACTIVE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.SEARCH; 
                _table.Columns["ISACTIVE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["ISACTIVE"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("KNOWLEDGE", "isActiveSuggestion", true));
            }

            base.CheckOrder = true;
            base.LoadInput(_db, _table, detailTab);
        }

        protected override void onAddProperty (DataRow row, DataColumn col, TableRow r) 
        {
            if (col != null)
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
        }

       
        private void mapControls() { 
            apply.Click += new System.EventHandler(apply_Click);
        }

        /// <summary>
        /// 
        /// </summary>
        private void apply_Click(object sender, System.EventArgs e) 
        {
            if (!base.checkInputValue(_table,detailTab)) 
            {
                return;
            }

            if(Global.Config.isModuleEnabled("suggestion")) 
            {
                if( typeCtrl.SelectedIndex != 2 ) 
                {
                    isactiveCtrlRow.Style["display"] = "none";
                    isactiveCtrl.Enabled = false;
                    isactiveCtrl.SelectedIndex = 0;
                } 
                else 
                {
                    isactiveCtrlRow.Style["display"] = "block";
                    isactiveCtrl.Enabled = true;
                }//if
            }//if


            //change the table
            this._union = (this.registryRelation.SelectedIndex == 1);            


            if(this.checkBoxKnowledge.Checked)
            {
                //nur wissenelemente
                this.wissenElemente();

            }
            else 
            {
                //themen
                this.themen();

            }

            
            DoOnSearchClick(apply);       
        }

        /// <summary>
        /// Themen suchen.
        /// </summary>
        private void themen()
        {
            //Themenlist is the target list
            string SQL_FIRST_PART = "SELECT t.ID, t.UID, t.TITLE, t.CREATOR_PERSON_ID, k.ID AS K_ID, k.TYPE as TYPE, k.ISACTIVE as ISACTIVE, k." + 
                      _db.langAttrName("KNOWLEDGE_V", "TITLE") + " AS DESCRIPTION, k." + 
                      _db.langAttrName("KNOWLEDGE_V", "CREATOR_PERSON_ID");
            
            string title = (string)this.getInputValue(_table,detailTab,_db.langAttrName("KNOWLEDGE_V", "TITLE"));
            string themeid = (string)this.getInputValue(_table,detailTab,_db.langAttrName("KNOWLEDGE_V", "THEMETYPE_ID"));
            string sqltitleChunk = (title=="")?"":"k." + _db.langAttrName("KNOWLEDGE_V", "TITLE") + " like '%" + title  + "%' ";
            string sqlthemeidChunk = (themeid=="")?"":" t.THEMETYPE_ID = " + themeid;

            //are not considered with the and/or logic
            string weType = (string)this.getInputValue(_table,detailTab,_db.langAttrName("KNOWLEDGE_V", "TYPE")); // 0: Standard; 1: Vorschlag
            string weSurveyIsActive = (string)this.getInputValue(_table,detailTab,_db.langAttrName("KNOWLEDGE_V", "ISACTIVE")); // 0: Inactive; 1: Active

            string sql = "";
            string union = this._union?"AND":"OR";
           
            if(title!="" && themeid!="")
                sql = sqltitleChunk + union + sqlthemeidChunk;
            if(title=="" && themeid!="")
                sql = sqlthemeidChunk;
            if(title!="" && themeid=="")
                sql = sqltitleChunk;

            _selectedRegistry.Clear();

            string WissenElementThemePart =  " OR " + _db.langAttrName("KNOWLEDGE_V", "BASE_THEME_ID") + " = t.ID";

            if(title=="" && themeid=="")
            {
                sql = SQL_FIRST_PART + " FROM KNOWLEDGE_V k, THEME t WHERE (k." +
                    _db.langAttrName("KNOWLEDGE_V", "BASE_THEME_ID") + " = t.PARENT_ID" + WissenElementThemePart + ")" +
                    " and HISTORY_ROOT_ID is null ";
            }
            else 
            {
                sql = SQL_FIRST_PART + " FROM KNOWLEDGE_V k, THEME t WHERE ("+ sql + ") AND (k." +
                    _db.langAttrName("KNOWLEDGE_V", "BASE_THEME_ID") + " = t.PARENT_ID" + WissenElementThemePart + ")" +
                    " and HISTORY_ROOT_ID is null ";
            }

            if(weType != "") 
            {
                sql += " and k.TYPE = " + weType; 
            }

            if(weSurveyIsActive != "")
            {
                sql += " and k.ISACTIVE = " + weSurveyIsActive;
            }

            sql += " ORDER BY t.ROOT_ID, t.PARENT_ID";

            // Setting search event args
            _searchArgs.ReloadList = true;
            _searchArgs.SearchSQL = sql;  
            _searchArgs.CustomData = "Theme";

        }

        /// <summary>
        /// Wissenelemente suchen.
        /// </summary>
        private void wissenElemente() 
        {
            //Wissenelementelist is the target list
            string SQL_FIRST_PART = "SELECT t.TITLE, t.DESCRIPTION, t.CREATED, k.ID AS ID, k.TYPE as TYPE, k.ISACTIVE as ISACTIVE, k." + 
                    _db.langAttrName("KNOWLEDGE_V", "TITLE") + ", k." + 
                    _db.langAttrName("KNOWLEDGE_V", "CREATOR_PERSON_ID");

            string title = (string)this.getInputValue(_table,detailTab,_db.langAttrName("KNOWLEDGE_V", "TITLE"));
            string themeid = (string)this.getInputValue(_table,detailTab,_db.langAttrName("KNOWLEDGE_V", "THEMETYPE_ID"));

            //are not considered with the and/or logic
            string weType = (string)this.getInputValue(_table,detailTab,_db.langAttrName("KNOWLEDGE_V", "TYPE")); // 0: Standard; 1: Vorschlag
            string weSurveyIsActive = (string)this.getInputValue(_table,detailTab,_db.langAttrName("KNOWLEDGE_V", "ISACTIVE")); // 0: Inactive; 1: Active
            
            string sqltitleChunk = (title=="")?"":"k." + _db.langAttrName("KNOWLEDGE_V", "TITLE") + " like '%" + title  + "%' ";
            string sqlthemeidChunk = (themeid=="")?"":" t.THEMETYPE_ID = " + themeid;

            string sql = "";
            string union = this._union?"AND":"OR";
           
            if(title!="" && themeid!="")
                sql = sqltitleChunk + union + sqlthemeidChunk;
            if(title=="" && themeid!="")
                sql = sqlthemeidChunk;
            if(title!="" && themeid=="")
                sql = sqltitleChunk;

            _selectedRegistry.Clear();

            if(title=="" && themeid=="")
            {
                sql = SQL_FIRST_PART + " FROM KNOWLEDGE_V k, THEME t WHERE k." +
                    _db.langAttrName("KNOWLEDGE_V", "BASE_THEME_ID") + " = t.ID" + " and HISTORY_ROOT_ID is null";
            }
            else 
            {
                sql = SQL_FIRST_PART + 
                    " FROM KNOWLEDGE_V k, THEME t WHERE ("+ sql + ") AND k." +
                    _db.langAttrName("KNOWLEDGE_V", "BASE_THEME_ID") + " = t.ID" + " and HISTORY_ROOT_ID is null";
            }

            if(weType != "") 
            {
                sql += " and k.TYPE = " + weType; 
            }

            if(weSurveyIsActive != "")
            {
                sql += " and k.ISACTIVE = " + weSurveyIsActive;
            }

            // Setting search event args
            _searchArgs.ReloadList = true;
            _searchArgs.SearchSQL = sql;  
            _searchArgs.CustomData = "Wissen";

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
