namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for ContactGroupEditCtrl.
    /// </summary>
    public partial class ContactGroupEditCtrl : PSOFTInputViewUserControl
	{
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_GROUP_ID = "PARAM_GROUP_ID";

        private DataTable _tableContactGroup;
        private DBData _db;


		public static string Path
		{
			get {return Global.Config.baseURL + "/Contact/Controls/ContactGroupEditCtrl.ascx";}
		}

		#region Properities
        public string NextURL
        {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public long GroupID
        {
            get {return GetLong(PARAM_GROUP_ID);}
            set {SetParam(PARAM_GROUP_ID, value);}
        }
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}

		protected override void DoExecute()
		{
			base.DoExecute ();
            InputType = InputMaskBuilder.InputType.Edit;
            
            if (!IsPostBack)
            {
                apply.Text = _mapper.get("apply");
            }

            _db = DBData.getDBData(Session);
			try 
			{
				_db.connect();

                //edit mask contactgroup....
				_tableContactGroup = _db.getDataTableExt("select * from CONTACT_GROUP where ID=" + GroupID, "CONTACT_GROUP");
                LoadInput(_db, _tableContactGroup, contactGroupTab);
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
            if (checkInputValue(_tableContactGroup, contactGroupTab))
            {
                _db.connect();
                try 
                {
                    //save contactgroup...
                    string sql = getSql(_tableContactGroup, contactGroupTab);

                    if (sql.Length > 0)
                    {
                        _db.execute(sql);
                    }

                    NextURL = NextURL.Replace("%25ID","%ID").Replace("%ID", GroupID.ToString());
                }
                catch (Exception ex) 
                {
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
