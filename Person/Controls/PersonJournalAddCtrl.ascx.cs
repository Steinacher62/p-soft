namespace ch.appl.psoft.Person.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Text;

    public partial class PersonJournalAddCtrl : PSOFTInputViewUserControl
	{   
        private DataTable _table;
        private DBData _db;


		public static string Path
		{
			get {return Global.Config.baseURL + "/Person/Controls/PersonJournalAddCtrl.ascx";}
		}

		#region Properties
        public long contextID
        {
            get {return GetLong("contextID");}
            set {SetParam("contextID", value);}
        }
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}

		protected override void DoExecute()
		{
			base.DoExecute ();
			InputType = InputMaskBuilder.InputType.Add;
            
            if (!IsPostBack)
            {
                apply.Text = _mapper.get("apply");
            }

            _db = DBData.getDBData(Session);
			try 
			{
				_db.connect();

				_table = _db.getDataTableExt("select * from PERSON_JOURNAL where ID=-1", "PERSON_JOURNAL");

                DataTable journalTypes = _db.getDataTable("select ID," + _db.langAttrName("PERSON_JOURNAL_TYPE", "TITLE") + " from PERSON_JOURNAL_TYPE order by " + _db.langAttrName("PERSON_JOURNAL_TYPE", "TITLE"));
                _table.Columns["PERSON_JOURNAL_TYPE_ID"].ExtendedProperties["In"] =  journalTypes;
                _table.Columns["PERSON_JOURNAL_TYPE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                
                base.LoadInput(_db, _table, journalTab);
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
			
            if (checkInputValue(_table, journalTab))
            {

                _db.connect();
                try 
                {
                    //save journal...
                    StringBuilder sbSQL = getSql(_table, journalTab, true);
                    long id = _db.newId(_table.TableName);
                
                    extendSql(sbSQL, _table, "ID", id);
                    extendSql(sbSQL, _table, "CREATOR_PERSON_ID", _db.userId);
					extendSql(sbSQL, _table, "PERSON_ID", contextID);

                    string sql = endExtendSql(sbSQL);

                    if (sql.Length > 0)
                    {
                        _db.beginTransaction();
                        _db.execute(sql);
						_db.commit();
                    }
                }
                catch (Exception ex) 
                {
					DoOnException(ex);
                    _db.rollback();
                }
                finally 
                {
                    _db.disconnect();   
                }

                ((PsoftContentPage) Page).RemoveBreadcrumbItem();
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
