namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Text;

    public partial class JournalAddCtrl : PSOFTMapperUserControl
	{
        public const string PARAM_XID = "PARAM_XID";
        public const string PARAM_CONTACTID = "PARAM_CONTACTID";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_MODE = "PARAM_MODE";

        private InputMaskBuilder _inputMask;
        private DataTable _table;
        private DBData _db;


		public static string Path
		{
			get {return Global.Config.baseURL + "/Contact/Controls/JournalAddCtrl.ascx";}
		}

		#region Properities
        public string NextURL
        {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public long xID
        {
            get {return GetLong(PARAM_XID);}
            set {SetParam(PARAM_XID, value);}
        }

        public long ContactID
        {
            get {return GetLong(PARAM_CONTACTID);}
            set {SetParam(PARAM_CONTACTID, value);}
        }

        public string Mode
        {
            get {return GetString(PARAM_MODE);}
            set {SetParam(PARAM_MODE, value);}
        }
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}

		protected override void DoExecute()
		{
			base.DoExecute ();
            _inputMask = new InputMaskBuilder(InputMaskBuilder.InputType.Add, Session);
            
            if (!IsPostBack)
            {
                apply.Text = _mapper.get("apply");
            }

            _db = DBData.getDBData(Session);
			try 
			{
				_db.connect();

				_table = _db.getDataTableExt("select * from JOURNAL where ID=-1", "JOURNAL");
                
                _table.Columns["JOURNAL_TYPE_ID"].ExtendedProperties["In"] =  ContactModule.getJournaltTypes(_db);
                _table.Columns["JOURNAL_TYPE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                
                _inputMask.load(_db, _table, journalTab, _mapper);
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
            if (_inputMask.checkInputValue(_table, journalTab, _mapper))
            {

                _db.connect();
                try 
                {
                    //save journal...
                    StringBuilder sbSQL = _inputMask.getSql(_table, journalTab, true);
                    long id = _db.newId(_table.TableName);
                
                    _inputMask.extendSql(sbSQL, _table, "ID", id);
                    _inputMask.extendSql(sbSQL, _table, "CREATOR_PERSON_ID", _db.userId);

                    string sql = _inputMask.endExtendSql(sbSQL);
                    if (sql.Length > 0)
                    {
                        _db.beginTransaction();
                        _db.execute(sql);

                        // assign the journal to the selected contacts...
                        DataTable table = null;
                        switch (Mode)
                        {
                            case ContactDetail.MODE_CONTACTGROUP:
                                table = _db.getDataTable("select TABLENAME, ID from CONTACT_GROUP_CONTACT_V where CONTACT_GROUP_ID=" + xID);
                                break;

                            case ContactDetail.MODE_SEARCHRESULT:
                                table = _db.getDataTable("select TABLENAME, ROW_ID from SEARCHRESULT where ID=" + xID);
                                break;

                            case ContactDetail.MODE_FIRM:
                                table = _db.getDataTable("select TABLENAME, ID from CONTACTV where FIRM_ID=" + xID);
                                break;

                            default:
                                table = _db.getDataTable("select TABLENAME, ID from CONTACTV where ID=" + ContactID);
                                break;
                        }

                        if (table != null)
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                string tablename = row[0].ToString();
                                _db.execute("insert into JOURNAL_" + tablename + " (JOURNAL_ID, " + tablename + "_ID) values (" + id + ", " + row[1].ToString() + ")");
                            }
                            _db.commit();
                            NextURL = NextURL.Replace("%25ID","%ID").Replace("%ID", id.ToString());
                        }
                        else
                        {
                            _db.rollback();
                            NextURL = "";
                        }
                    }
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
                if (NextURL != "")
                    Response.Redirect(NextURL);
                else
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
