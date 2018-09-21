namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for ContactGroupDetailCtrl.
    /// </summary>
    public partial class ContactGroupDetailCtrl : PSOFTMapperUserControl
	{
        public const string PARAM_XID = "PARAM_XID";

        private DetailBuilder _detailBuilder;
        private DataTable _table;
        private DBData _db;


		public static string Path
		{
			get {return Global.Config.baseURL + "/Contact/Controls/ContactGroupDetailCtrl.ascx";}
		}

		#region Properities
        public long xID
        {
            get {return GetLong(PARAM_XID);}
            set {SetParam(PARAM_XID, value);}
        }
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}

		protected override void DoExecute()
		{
			base.DoExecute ();
            _detailBuilder = new DetailBuilder();
            
            if (!IsPostBack)
            {
            }

            _db = DBData.getDBData(Session);
			try 
			{
				_db.connect();
				_table = _db.getDataTableExt("select * from CONTACT_GROUP where ID=" + xID, "CONTACT_GROUP");
                _detailBuilder.load(_db, _table, contactGroupTab, _mapper);
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

        public string getTitle()
        {
            _db = DBData.getDBData(Session);
            try 
            {
                _db.connect();
                return _db.lookup("TITLE", "CONTACT_GROUP", "ID=" + xID, false);
            }
            catch (Exception ex) 
            {
                DoOnException(ex);
            }
            finally 
            {
                _db.disconnect();
            }

            return "";
        }

		private void mapControls () 
		{
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
