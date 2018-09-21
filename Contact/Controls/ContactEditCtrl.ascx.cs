namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for ContactEditCtrl.
    /// </summary>
    public partial class ContactEditCtrl : PSOFTMapperUserControl
	{
        public const string PARAM_XID = "PARAM_XID";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_TYPE = "PARAM_TYPE";

        private InputMaskBuilder _inputMaskContact;
        private InputMaskBuilder _inputMaskAddress;
        private DataTable _tableContact;
        private DataTable _tableAddress;
        private DBData _db;


		public static string Path
		{
			get {return Global.Config.baseURL + "/Contact/Controls/ContactEditCtrl.ascx";}
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
            _inputMaskContact = new InputMaskBuilder(InputMaskBuilder.InputType.Edit, Session);
            _inputMaskAddress = new InputMaskBuilder(InputMaskBuilder.InputType.Edit, Session);
            
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
				_tableContact = _db.getDataTableExt("select * from " + tableName + " where ID=" + xID, tableName);
                
                if (Type == ContactModule.TYPE_PERSON)
                {
                    _tableContact.Columns["FIRM_ID"].ExtendedProperties["In"] =  ContactModule.getContactFirms(_db);
                    _tableContact.Columns["FIRM_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    _tableContact.Columns["CONTACT_ROLE_ID"].ExtendedProperties["In"] =  ContactModule.getContactRoles(_db);
                    _tableContact.Columns["CONTACT_ROLE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                }

                _tableContact.Columns["CONTACT_TYPE_ID"].ExtendedProperties["In"] =  ContactModule.getContactTypes(_db);
                _tableContact.Columns["CONTACT_TYPE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                
                _inputMaskContact.load(_db, _tableContact, contactTab, _mapper, Type == ContactModule.TYPE_PERSON ? "CONTACT_PERSON" : "CONTACT_FIRM");

                //address...
                string addressID = _db.lookup("ID", "ADDRESS", tableName + "_ID=" + xID, false);
                if (addressID != "")
                {
                    _tableAddress = _db.getDataTableExt("select * from ADDRESS where ID=" + addressID, "ADDRESS");
                    _tableAddress.Columns["COUNTRY"].ExtendedProperties["In"] = _mapper.getEnum("address", "country");
                    _tableAddress.Columns["COUNTRY"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    _inputMaskAddress.load(_db, _tableAddress, addressTab, _mapper, "ADDRESS");
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

		private void mapControls () 
		{
			apply.Click += new System.EventHandler(apply_Click);
		}

		private void apply_Click(object sender, System.EventArgs e) 
		{
            if (_inputMaskContact.checkInputValue(_tableContact, contactTab, _mapper) && (_tableAddress == null || _inputMaskAddress.checkInputValue(_tableAddress, addressTab, _mapper)))
            {
                _db.connect();
                try 
                {
                    _db.beginTransaction();
                    //save contact...
                    string sql = _inputMaskContact.getSql(_tableContact, contactTab);
                    if (sql.Length > 0)
                        _db.execute(sql);

                    //save address...
                    if (_tableAddress != null)
                    {
                        sql = _inputMaskAddress.getSql(_tableAddress, addressTab);
                        if (sql.Length > 0)
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
                Response.Redirect(NextURL);
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
