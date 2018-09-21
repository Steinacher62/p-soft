namespace ch.appl.psoft.Document.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web.UI.WebControls;

    public partial class MailingListAddCtrl : PSOFTInputViewUserControl
	{   
		public const string PARAM_XID = "PARAM_XID";
		
		private DataTable _table;
        private DBData _db;
		private ArrayList _exchangeFolder;

		protected System.Web.UI.WebControls.Table journalTab;

		#region Properties
		
		public long XID 
		{
			get {return GetLong(PARAM_XID);}
			set {SetParam(PARAM_XID, value);}
		}

		#endregion

		public static string Path
		{
			get {return Global.Config.baseURL + "/Document/Controls/MailingListAddCtrl.ascx";}
		}

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

				_table = _db.getDataTableExt("select * from EXCHANGE_FOLDER where ID=-1", "EXCHANGE_FOLDER");

				_exchangeFolder =  ExchangeHelper.getAllFolders(Session);
				_table.Columns["PATH"].ExtendedProperties["In"] = _exchangeFolder;
				_table.Columns["PATH"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                base.LoadInput(_db, _table, exchangeFolderTable);
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

		/*
		protected override void onAddProperty (DataRow row, DataColumn col, TableRow r) 
		{
			if(col.ColumnName == "PATH") 
			{
				
			}//if
		}//onAddProperty
		*/
		
		private void mapControls () 
		{
			apply.Click += new System.EventHandler(apply_Click);
		}

		private void apply_Click(object sender, System.EventArgs e) 
		{
			
            if (checkInputValue(_table, exchangeFolderTable))
            {

                _db.connect();
                try 
                {
					ListControl lc = (ListControl) exchangeFolderTable.FindControl("Input-EXCHANGE_FOLDER-PATH");
					int selectedvalue =  Int32.Parse(lc.SelectedItem.Value);
					ExchangeFolder exfolder = (ExchangeFolder) this._exchangeFolder[selectedvalue];

                    //setInputValue(_table, exchangeFolderTable, "PATH", exfolder.Title);
					lc.SelectedItem.Value = exfolder.Title;

                    StringBuilder sbSQL = getSql(_table, exchangeFolderTable, true);
                    long id = _db.newId(_table.TableName);
			
                    extendSql(sbSQL, _table, "ID", id);
                    extendSql(sbSQL, _table, "EXCHANGE_ID", exfolder.Exchangeid);
					extendSql(sbSQL, _table, "CREATOR_PERSON_ID", _db.userId);
					extendSql(sbSQL, _table, "FOLDER_ID", XID);
                    
					string sql = endExtendSql(sbSQL);

                    if (sql.Length > 0)
                    {
                        _db.beginTransaction();
                        _db.execute(sql);
						_db.commit();
                    }//if
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
            }//if
			
        }//apply_Click

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
