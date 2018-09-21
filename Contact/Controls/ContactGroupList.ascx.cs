namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for ContactGroupList.
    /// </summary>
    public partial class ContactGroupList : PSOFTListViewUserControl
    {
        private long _contactGroupID = -1;
        private long _personID = -1;

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        public static string Path
        {
            get {return Global.Config.baseURL + "/Contact/Controls/ContactGroupList.ascx";}
        }

        public ContactGroupList() : base()
        {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
        }

		#region Properities
        public long ContactGroupID
        {
            get {return _contactGroupID;}
            set {_contactGroupID = value;}
        }

        public long PersonID
        {
            get {return _personID;}
            set {_personID = value;}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();

            DBData db = DBData.getDBData(Session);
            try 
            {
                db.connect();
                pageTitle.Text = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CTPERSONCONTACTGROUPS);
                string sql = "select * from CONTACT_GROUP where PERSON_ID=" + _personID;

                sql += " order by " + OrderColumn + " " + OrderDir;

                DataTable table = db.getDataTableExt(sql, "CONTACT_GROUP");
                IDColumn = "ID";
                if (_contactGroupID > 0)
                    HighlightRecordID = _contactGroupID;

                LoadList(db, table, listTab);
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

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
		#endregion
    }
}
