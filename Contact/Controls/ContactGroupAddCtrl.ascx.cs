namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Text;

    /// <summary>
    ///		Summary description for ContactGroupAddCtrl.
    /// </summary>
    public partial class ContactGroupAddCtrl : PSOFTMapperUserControl
	{
        public const string PARAM_SEARCHRESULT_ID = "PARAM_SEARCHRESULT_ID";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_PERSON_ID = "PARAM_PERSON_ID";
        public const string PARAM_OWNER_TABLE = "PARAM_OWNER_TABLE";
        public const string PARAM_OWNER_ID = "PARAM_OWNER_ID";

        private InputMaskBuilder _inputMaskContactGroup;
        private DataTable _tableContactGroup;
        private DBData _db;


		public static string Path
		{
			get {return Global.Config.baseURL + "/Contact/Controls/ContactGroupAddCtrl.ascx";}
		}

		#region Properities
        public string NextURL
        {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public long SearchresultID {
            get {return GetLong(PARAM_SEARCHRESULT_ID);}
            set {SetParam(PARAM_SEARCHRESULT_ID, value);}
        }

        public long PersonID {
            get {return GetLong(PARAM_PERSON_ID);}
            set {SetParam(PARAM_PERSON_ID, value);}
        }

        public string OwnerTable {
            get {return GetString(PARAM_OWNER_TABLE);}
            set {SetParam(PARAM_OWNER_TABLE, value);}
        }
        public long OwnerID {
            get {return GetLong(PARAM_OWNER_ID);}
            set {SetParam(PARAM_OWNER_ID, value);}
        }

        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}

		protected override void DoExecute()
		{
			base.DoExecute ();
            _inputMaskContactGroup = new InputMaskBuilder(InputMaskBuilder.InputType.Add, Session);
            
            if (!IsPostBack)
            {
                apply.Text = _mapper.get("apply");
            }

            _db = DBData.getDBData(Session);
			try 
			{
				_db.connect();

                //contact....
				_tableContactGroup = _db.getDataTableExt("select * from CONTACT_GROUP where ID=-1", "CONTACT_GROUP");
                _inputMaskContactGroup.load(_db, _tableContactGroup, contactGroupTab, _mapper);
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
            if (_inputMaskContactGroup.checkInputValue(_tableContactGroup, contactGroupTab, _mapper))
            {
                _db.connect();
                try 
                {
                    _db.beginTransaction();
                    //save contactgroup...
                    StringBuilder sbSQL = _inputMaskContactGroup.getSql(_tableContactGroup, contactGroupTab, true);
                    long id = _db.newId(_tableContactGroup.TableName);
                
                    _inputMaskContactGroup.extendSql(sbSQL, _tableContactGroup, "ID", id);
                    if (PersonID > 0){
                        _inputMaskContactGroup.extendSql(sbSQL, _tableContactGroup, "PERSON_ID", PersonID);
                    }

                    string sql = _inputMaskContactGroup.endExtendSql(sbSQL);
                    if (sql.Length > 0)
                    {
                        _db.execute(sql);

                        if (OwnerTable != "" && OwnerID > 0){
                            // assign the contact-group to its owner
                            _db.execute("update " + OwnerTable + " set CONTACT_GROUP_ID=" + id + " where ID=" + OwnerID);
                        }

                        if (SearchresultID > 0)
                        {
                            // assign the selected contacts to group
                            DataTable table = _db.getDataTable("select TABLENAME, ROW_ID from SEARCHRESULT where ID=" + SearchresultID);
                            foreach (DataRow row in table.Rows)
                            {
                                string tablename = row["TABLENAME"].ToString();
                                _db.execute("insert into CONTACT_GROUP_" + tablename + " (CONTACT_GROUP_ID, " + tablename + "_ID) values (" + id + ", " + row["ROW_ID"].ToString() + ")");
                            }
                        }
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
