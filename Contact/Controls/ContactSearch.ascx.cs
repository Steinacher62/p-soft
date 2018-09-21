namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;

    /// <summary>
    /// Summary description for ContactSearch.
    /// </summary>
    public partial class ContactSearch : PSOFTSearchUserControl 
	{

        private InputMaskBuilder _searchContact;		
        private InputMaskBuilder _searchJournal;		
        private DataTable _contactTable;
        private DataTable _journalTable;

		protected System.Web.UI.WebControls.Table Table1;
       
		public static string Path
		{
			get {return Global.Config.baseURL + "/Contact/Controls/ContactSearch.ascx";}
		}

		protected void Page_Load(object sender, System.EventArgs e) 
		{
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();
            DBData db = DBData.getDBData(Session);
            _searchContact = new InputMaskBuilder(InputMaskBuilder.InputType.Search, Session);
            _searchJournal = new InputMaskBuilder(InputMaskBuilder.InputType.Search, Session);

			try 
			{
				if (!IsPostBack) 
				{
					apply.Text = _mapper.get("search");
                    contactTitle.Text = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_SEARCHCONTACTCONTACT);
                    journalTitle.Text = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_SEARCHCONTACTJOURNAL);
                }

				db.connect();
                // contact mask
				_contactTable = db.getDataTableExt("select * from CONTACTV where ID=-1", "CONTACTV");

                _contactTable.Columns["CONTACT_TYPE_ID"].ExtendedProperties["In"] =  ContactModule.getContactTypes(db);
                _contactTable.Columns["CONTACT_TYPE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                _contactTable.Columns["CONTACT_ROLE_ID"].ExtendedProperties["In"] =  ContactModule.getContactRoles(db);
                _contactTable.Columns["CONTACT_ROLE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                _contactTable.Columns["TABLENAME"].ExtendedProperties["In"] =  ContactModule.getContactKinds(db, _mapper);
                _contactTable.Columns["TABLENAME"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                _contactTable.Columns["COUNTRY"].ExtendedProperties["In"] = _mapper.getEnum("address", "country");
                _contactTable.Columns["COUNTRY"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                _searchContact.load(db, _contactTable, searchTabContact, _mapper, "CONTACTV");


                // journal mask
                _journalTable = db.getDataTableExt("select * from JOURNAL where ID=-1", "JOURNAL");

                _journalTable.Columns["JOURNAL_TYPE_ID"].ExtendedProperties["In"] = ContactModule.getJournaltTypes(db);
                _journalTable.Columns["JOURNAL_TYPE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                _journalTable.Columns["CREATOR_PERSON_ID"].ExtendedProperties["In"] = db.Person.getWholeNameMATable(false);
                _journalTable.Columns["CREATOR_PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                _searchJournal.load(db, _journalTable, searchTabJournal, _mapper);
			}
			catch (Exception ex) 
            {
                DoOnException(ex);
            }
			finally 
			{
				db.disconnect();
			}
            
		}
		private void mapControls () 
		{
			apply.Click += new System.EventHandler(apply_Click);
		}

        private string sqlAppendWhere(string sql, string clause)
        {
            sql += ((sql.ToLower().IndexOf(" where ") > 0) ? " and " : " where ") + clause;
            return sql;
        }

		private void apply_Click(object sender, System.EventArgs e)
		{
            if (_searchContact.checkInputValue(_contactTable, searchTabContact, _mapper) &&  _searchJournal.checkInputValue(_journalTable, searchTabJournal, _mapper))
            {

                string sql = _searchContact.getSql(_contactTable, searchTabContact);

                if (sql == "")
                    sql = "select distinct * from " + _contactTable.TableName;

                string sqlJournal = ch.psoft.Util.Validate.GetValid(_searchJournal.getSql(_journalTable, searchTabJournal), "");

                if(sqlJournal != "")
                {
                    sql = sqlAppendWhere(sql, "ID in (select PERSON_ID from JOURNAL_PERSON where JOURNAL_ID in (" + sqlJournal.Replace("*", "ID") + ") union select FIRM_ID from JOURNAL_FIRM where JOURNAL_ID in (" + sqlJournal.Replace("*", "ID") + "))");
                }

                // Setting search event args
                _searchArgs.ReloadList = true;
                _searchArgs.SearchSQL = sql;

                DoOnSearchClick(apply);
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{    
            this.ID = "Search";

        }
		#endregion
	}
}