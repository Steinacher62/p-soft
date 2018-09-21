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
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;
    using Telerik.Web.UI;


    public partial class KnowledgeEditCtrl : PSOFTInputViewUserControl
	{
        private DataTable _table;
        private DBData _db;


        private ArrayList _registryList = new ArrayList();

		public static string Path
		{
			get {return Global.Config.baseURL + "/Knowledge/Controls/KnowledgeEditCtrl.ascx";}
		}

		#region Properties

        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public string NextURL
        {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public const string PARAM_KNOWLEDGE_ID = "PARAM_KNOWLEDGE_ID";
        public long KnowledgeID
        {
            get {return GetLong(PARAM_KNOWLEDGE_ID);}
            set {SetParam(PARAM_KNOWLEDGE_ID, value);}
        }

        public const string PARAM_LINKNG_KNOWLEDGE_ID = "PARAM_LINKNG_KNOWLEDGE_ID";
        public long LinkingKnowledgeID{
            get {return GetLong(PARAM_LINKNG_KNOWLEDGE_ID);}
            set {SetParam(PARAM_LINKNG_KNOWLEDGE_ID, value);}
        }

        public const string PARAM_TITLE = "PARAM_TITLE";
        public string Title{
            get {return GetString(PARAM_TITLE);}
            set {SetParam(PARAM_TITLE, value);}
        }
        #endregion

        protected long knowledgeUID = -1;

        public KnowledgeEditCtrl() : base(){
            InputType = InputMaskBuilder.InputType.Edit;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}

		protected override void DoExecute()
		{
			base.DoExecute ();
            
            if (!IsPostBack)
            {
                apply.Text = _mapper.get("knowledge", "saveAndQuit");
                apply.ToolTip = _mapper.get("knowledge", "saveAndQuitTooltip"); 
                save.Text = _mapper.get("knowledge", "saveAndContinue");
                save.ToolTip = _mapper.get("knowledge", "saveAndContinueTooltip");
            }

            _db = DBData.getDBData(Session);
			try 
			{
				_db.connect();
                long baseThemeID = _db.Knowledge.getBaseThemeID(KnowledgeID);
                if (baseThemeID <= 0){
                    InputType = InputMaskBuilder.InputType.Add;
                    apply.Visible = false;
                }

                knowledgeUID = _db.ID2UID(KnowledgeID, "KNOWLEDGE");

				_table = _db.getDataTableExt("select * from THEME where ID=" + baseThemeID, "THEME");
                _table.Columns["ORDNUMBER"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
				_table.Columns["THEMETYPE_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["DESCRIPTION"].ExtendedProperties["InputControlType"] = typeof(RadEditor);

                if (baseThemeID <= 0){
                    _table.Columns["DESCRIPTION"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;              
                    InputType = InputMaskBuilder.InputType.Add;
                }


                LoadInput(_db, _table, editTab);
                if (!IsPostBack) {
                    if (InputType == InputMaskBuilder.InputType.Add){
                        setInputValue(_table, editTab, "TITLE", Title);
                    }
                }

                _registryList = new ArrayList(_db.Registry.getRegistryIDs(KnowledgeID.ToString(), Knowledge._TABLENAME).Split(','));
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

        bool convertWikiSyntax = false;
        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r)
        {
            if (col != null && col.ColumnName == "DESCRIPTION")
            {
                r.Cells[0].Text = "";
            }

            if (InputType == InputMaskBuilder.InputType.Edit)
            {
                if (col != null && col.ColumnName == "TITLE")
                {
                    string text = row[col].ToString();
                    if (text[0] == '@')
                    {
                        convertWikiSyntax = true;
                        TextBox e = r.Cells[1].Controls[0] as TextBox;
                        e.Text = text.Substring(1);
                    }
                }

                if (col != null && col.ColumnName == "DESCRIPTION")
                {
                    if (convertWikiSyntax)
                    {
                        string text = row[col].ToString();
                        AutoNumbering autoNumbering = new AutoNumbering();
                        ArrayList entries = new ArrayList();
                        RadEditor e = r.Cells[1].Controls[0] as RadEditor;
                        e.Text = _db.Theme.text2HTML(DBColumn.GetValid(row[col], ""), _db, knowledgeUID, ref autoNumbering, 1, ref entries);
                    }
                }
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
            else response.Write(nodeName+".prependHTML = \"<input type='checkbox' ID='RegFlag"+row["ID"]+"'>\";\n");
            return true;
        }

        private void mapControls () 
		{
			apply.Click += new System.EventHandler(apply_Click);
            apply.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";

            save.Click += new System.EventHandler(apply_Click);
            save.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";           
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
                    long themeID = -1;
                    if (InputType == InputMaskBuilder.InputType.Add){
                        themeID = _db.newId(Theme._TABLENAME);
                        extendSql(sb, _table, "ID", themeID);
                        extendSql(sb, _table, "ROOT_ID", themeID);			
                        extendSql(sb, _table, "CREATOR_PERSON_ID", _db.userId);
                    }
					
                    string sql = base.endExtendSql(sb);
                    if (sql.Length > 0){
                        _db.execute(sql);
                    }
                    if (InputType == InputMaskBuilder.InputType.Add){
                        if (KnowledgeID <= 0){
                            KnowledgeID = _db.Knowledge.create(themeID, "Initialversion");
                        }
                        else{
                            _db.Knowledge.setBaseThemeID(KnowledgeID, themeID);
                        }
                        if (LinkingKnowledgeID > 0){
                            _db.addUIDAssignment(Knowledge._TABLENAME, LinkingKnowledgeID, Knowledge._TABLENAME, KnowledgeID, DBData.ASSIGNMENT.UNDEFINED, -1L, -1L);
                        }

                        // new knowledge-entries are open for administrator and creator
                        _db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, _db.userAccessorID, Knowledge._TABLENAME, KnowledgeID);
                    }

                    // registry
                    if (registryFlags.Value != "") {                    
                        _db.Registry.updateRegistryEntries(registryFlags.Value, Knowledge._TABLENAME, KnowledgeID);
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

                if (sender == save)
                {
                    if (InputType == InputMaskBuilder.InputType.Add)
                    {
                        NextURL = psoft.Knowledge.KnowledgeDetail.GetURL("knowledgeID", KnowledgeID);
                    }
                    string url = psoft.Knowledge.EditKnowledge.GetURL("mode", "edit", "knowledgeID", KnowledgeID,"backURL",NextURL);
                    Response.Redirect(url);
                }
                else
                {                   
                    Response.Redirect(NextURL);
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
