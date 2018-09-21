namespace ch.appl.psoft.Knowledge.Controls
{
    using ch.psoft.Util;
    using Common;
    using db;
    using LayoutControls;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web.UI.WebControls;
    using Telerik.Web.UI;

    public partial class ThemeEditCtrl : PSOFTInputViewUserControl
	{
        private DataTable _table;
		
		private DBData _db;

        protected long knowledgeID = 0;               
        protected long knowledgeUID = 0;       
        protected long themeUID = 0;		

		public static string Path
		{
			get {return Global.Config.baseURL + "/Knowledge/Controls/ThemeEditCtrl.ascx";}
		}

		#region Properties

        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public string NextURL
        {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public const string PARAM_THEME_ID = "PARAM_THEME_ID";
        public long ThemeID
        {
            get {return GetLong(PARAM_THEME_ID);}
            set {SetParam(PARAM_THEME_ID, value);}
        }

        public const string PARAM_PARENT_THEME_ID = "PARAM_PARENT_THEME_ID";			
        public long ParentThemeID{
            get {return GetLong(PARAM_PARENT_THEME_ID);}
            set {SetParam(PARAM_PARENT_THEME_ID, value);}
        }

		public const string SLAVE_ID = "SLAVE_ID";
		public long SlaveCharacteristicID
		{
			get {return GetLong(SLAVE_ID);}
			set {SetParam(SLAVE_ID, value);}
		}

        

        #endregion

        public ThemeEditCtrl() : base(){
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
   
                // needed informations for fck, exposed as javascript variables --> see ascx
                themeUID = _db.ID2UID(ThemeID, "THEME");
                if (ThemeID > 0)
                {
                    knowledgeID = _db.Theme.getKnowledgeID(ThemeID);
                    knowledgeUID = _db.ID2UID(knowledgeID, "KNOWLEDGE");
                }
                else
                {
                    knowledgeID = _db.Theme.getKnowledgeID(ParentThemeID);
                    knowledgeUID = _db.ID2UID(knowledgeID, "KNOWLEDGE");
                }

				if(SlaveCharacteristicID > 0)
				{
					long themeId = _db.lookup("THEME_ID","SLAVE_CHARACTERISTIC", "ID = "+ SlaveCharacteristicID,-1L);
					_table = _db.getDataTableExt("select * from THEME where ID=" + themeId, "THEME");
					_table.Columns["ORDNUMBER"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
				}
				else
				{
					_table = _db.getDataTableExt("select * from THEME where ID=" + ThemeID, "THEME");
				}

                _table.Columns["DESCRIPTION"].ExtendedProperties["InputControlType"] = typeof(RadEditor);					
				_table.Columns["THEMETYPE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
				_table.Columns["THEMETYPE_ID"].ExtendedProperties["In"] =  _db.getDataTable("SELECT ID,TITLE FROM THEMETYPE");

				LoadInput(_db, _table, editTab);
				if (InputType == InputMaskBuilder.InputType.Add && SlaveCharacteristicID <= 0)
				{
						setInputValue(_table, editTab, "ORDNUMBER", _db.Theme.nextOrdnumber(ParentThemeID));
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


		private void mapControls () 
		{
			apply.Click += new System.EventHandler(apply_Click);
            save.Click += new System.EventHandler(apply_Click);
        }

		private void apply_Click(object sender, System.EventArgs e) 
		{            						                    					
			if (checkInputValue(_table, editTab))
			{
				_db.connect();			
				long id = 0;
                long knowledgeId = -1;
				
				try 
				{

                    // check if any theme type was selected, if not set the default theme type which was declared in the java application
                    long defaultThemeTypeId = DBColumn.GetValid(_db.lookup("ID", "THEMETYPE", "STANDARD = 1"), -1L);
                    if (((String)getInputValue(_table, editTab, "THEMETYPE_ID")) == "")
                    {
                        setInputValue(_table, editTab, "THEMETYPE_ID",defaultThemeTypeId.ToString());
                    }

					_db.beginTransaction();
					StringBuilder sb = getSql(_table, editTab, true);
                    if (InputType == InputMaskBuilder.InputType.Add)
                    {

						
                        id = _db.newId("THEME");
                        extendSql(sb, _table, "ID", id);
                        extendSql(sb, _table, "PARENT_ID", ParentThemeID);
                        extendSql(sb, _table, "ROOT_ID", _db.Theme.getRootID(ParentThemeID));
                        extendSql(sb, _table, "CREATOR_PERSON_ID", _db.userId);
                    }
                    else 
                    {
                        long themeRootId = DBColumn.GetValid(_db.lookup("ROOT_ID","THEME", "ID = " + ThemeID ),-1L);
                        knowledgeId = DBColumn.GetValid(_db.lookup("ID","KNOWLEDGE", _db.langAttrName("KNOWLEDGE", "BASE_THEME_ID") + "="  + themeRootId),-1L);
                    }
										
					string sql = base.endExtendSql(sb);
					
					if (sql.Length > 0)
					{
						_db.execute(sql);						
					}			
		
					if(SlaveCharacteristicID > 0 && InputType == InputMaskBuilder.InputType.Add)
					{
						sql = "UPDATE SLAVE_CHARACTERISTIC SET THEME_ID = "+ id + " WHERE ID = " + SlaveCharacteristicID;
						_db.execute(sql);
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
                    string url = psoft.Knowledge.EditTheme.GetURL("mode", "edit", "themeID", ThemeID, "backURL", NextURL);
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
