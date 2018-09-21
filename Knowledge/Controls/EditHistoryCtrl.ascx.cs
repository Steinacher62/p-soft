namespace ch.appl.psoft.Knowledge.Controls
{
    using Common;
    using db;
    using Interface.DBObjects;
    using LayoutControls;
    using System;
    using System.Data;
    using System.Text;


    public partial class EditHistoryCtrl : PSOFTInputViewUserControl
	{
        private DataTable _table;
        
		private DBData _db;

		protected System.Web.UI.WebControls.Table colorationTab;
		

		public enum ControlMode
		{
			Edit,
			New
		}

		public static string Path
		{
			get {return Global.Config.baseURL + "/Knowledge/Controls/EditHistoryCtrl.ascx";}
		}

		#region Properities

		private long knowledgeId = -1L;
		public long KnowledgeId
		{
			get {return knowledgeId;}
			set {knowledgeId =  value; }
		}

		private ControlMode mode = ControlMode.Edit;
		public ControlMode Mode
		{
			get {return mode;}
			set {mode =  value; }
		}

		#endregion

        public EditHistoryCtrl() : base(){
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
                apply.Text = _mapper.get("apply");
            }

            _db = DBData.getDBData(Session);
			_db.connect();				
			try 
			{				
				if(Mode == ControlMode.Edit)
				{
					_table = _db.getDataTableExt("select ID,REASON from KNOWLEDGE where ID=" + KnowledgeId, "KNOWLEDGE");
				}
				else
				{
					_table = _db.getDataTableExt("select ID,REASON from KNOWLEDGE where ID=" + -1, "KNOWLEDGE");
				}

				View = "HISTORY";
				LoadInput(_db, _table, editTab);
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

		private void mapControls () 
		{
			apply.Click += new System.EventHandler(apply_Click);
        }

		private void apply_Click(object sender, System.EventArgs e) 
		{         
   			string nextURL = "";			                    					
			if (checkInputValue(_table, editTab))
			{				
				_db.connect();			
				long id = 0;
				
				try 
				{				
					_db.beginTransaction();
					
					if (Mode == ControlMode.New)
					{
						Knowledge knowledge = new Knowledge(_db,Session);
						id = knowledge.createHistoryEntry(KnowledgeId,(string)getInputValue(_table,editTab,"REASON"));						
					}
					else
					{
						StringBuilder sb = getSql(_table, editTab, true);										
						string sql = base.endExtendSql(sb);
					
						if (sql.Length > 0)
						{
							_db.execute(sql);						
						}			
					}
					if (Mode == ControlMode.New)
					{
						nextURL = KnowledgeDetail.GetURL("knowledgeID",id);					
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
			}
			
			((PsoftContentPage) Page).RemoveBreadcrumbItem();
			if (nextURL != "")
			{
				Response.Redirect(nextURL);
			}
			else
			{
				((PsoftContentPage) Page).RedirectToPreviousPage();
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
