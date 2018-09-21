namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Text;

    /// <summary>
    ///		Summary description for ContactAddCtrl.
    /// </summary>
    public partial class ContactAddCtrl : PSOFTMapperUserControl
	{
        public const string PARAM_FIRM_ID = "PARAM_FIRM_ID";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_TYPE = "PARAM_TYPE";

        private InputMaskBuilder _inputMaskContact;
        private InputMaskBuilder _inputMaskAddress;
        private DataTable _tableContact;
        private DataTable _tableAddress;
        private DBData _db;


		public static string Path
		{
			get {return Global.Config.baseURL + "/Contact/Controls/ContactAddCtrl.ascx";}
		}

		#region Properities
        public string NextURL
        {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public long FirmID
        {
            get {return GetLong(PARAM_FIRM_ID);}
            set {SetParam(PARAM_FIRM_ID, value);}
        }

        public string Type
        {
            get {return GetString(PARAM_TYPE);}
            set {SetParam(PARAM_TYPE, value);}
        }
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}

		protected override void DoExecute()
		{
			base.DoExecute ();
            _inputMaskContact = new InputMaskBuilder(InputMaskBuilder.InputType.Add, Session);
            _inputMaskAddress = new InputMaskBuilder(InputMaskBuilder.InputType.Add, Session);
            
            if (!IsPostBack)
            {
                apply.Text = _mapper.get("apply");
                contactTabTitle.Text = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, Type == ContactModule.TYPE_PERSON ? ContactModule.LANG_MNEMO_ADDEDITCONTACTCONTACT : ContactModule.LANG_MNEMO_ADDEDITCONTACTFIRM);
                addressTabTitle.Text = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_ADDEDITCONTACTADDRESS);
            }

            _db = DBData.getDBData(Session);
			try 
			{
                string tableName = Type == ContactModule.TYPE_PERSON ? "PERSON" : "FIRM";
				_db.connect();

                //contact....
				_tableContact = _db.getDataTableExt("select * from " + tableName + " where ID=-1", tableName);
                
                if (Type == ContactModule.TYPE_PERSON)
                {
                    if (FirmID > 0)
                    {
                        _tableContact.Columns["FIRM_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    }
                    else
                    {
                        _tableContact.Columns["FIRM_ID"].ExtendedProperties["In"] =  ContactModule.getContactFirms(_db);
                        _tableContact.Columns["FIRM_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    }
                    _tableContact.Columns["CONTACT_ROLE_ID"].ExtendedProperties["In"] =  ContactModule.getContactRoles(_db);
                    _tableContact.Columns["CONTACT_ROLE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                }

                _tableContact.Columns["CONTACT_TYPE_ID"].ExtendedProperties["In"] =  ContactModule.getContactTypes(_db);
                _tableContact.Columns["CONTACT_TYPE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                
                _inputMaskContact.load(_db, _tableContact, contactTab, _mapper, Type == ContactModule.TYPE_PERSON ? "CONTACT_PERSON" : "CONTACT_FIRM");

                //address...
                _tableAddress = _db.getDataTableExt("select * from ADDRESS where ID=-1", "ADDRESS");
                _tableAddress.Columns["COUNTRY"].ExtendedProperties["In"] = _mapper.getEnum("address", "country");
                _tableAddress.Columns["COUNTRY"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                
                _inputMaskAddress.load(_db, _tableAddress, addressTab, _mapper, "ADDRESS");
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
            if (_inputMaskContact.checkInputValue(_tableContact, contactTab, _mapper) && _inputMaskAddress.checkInputValue(_tableAddress, addressTab, _mapper))
            {

                _db.connect();
                try 
                {
                    _db.beginTransaction();
                    //save contact...
                    StringBuilder sbSQL = _inputMaskContact.getSql(_tableContact, contactTab, true);
                    long id = _db.newId(_tableContact.TableName);
                
                    _inputMaskContact.extendSql(sbSQL, _tableContact, "ID", id);
                    _inputMaskContact.extendSql(sbSQL, _tableContact, "TYP", Type == ContactModule.TYPE_PERSON ? 8 : 2);
                    if (Type == ContactModule.TYPE_PERSON && FirmID > 0)
                        _inputMaskContact.extendSql(sbSQL, _tableContact, "FIRM_ID", FirmID);

                    string sql = _inputMaskContact.endExtendSql(sbSQL);
                    if (sql.Length > 0)
                    {
                        _db.execute(sql);

                        //save address...
                        sbSQL = _inputMaskAddress.getSql(_tableAddress, addressTab, true);
                        _inputMaskAddress.extendSql(sbSQL, _tableAddress, Type == ContactModule.TYPE_PERSON ? "PERSON_ID" : "FIRM_ID", id);
                        sql = _inputMaskAddress.endExtendSql(sbSQL);
                        if (sql.Length > 0)
                            _db.execute(sql);
                    }

                    NextURL = NextURL.Replace("%25ID","%ID").Replace("%ID", id.ToString());
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
