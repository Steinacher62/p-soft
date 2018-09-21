namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    public partial class JournalList : PSOFTListViewUserControl {
        private long _contactID = -1;
        private long _journalID = -1;
        private DBData _db = null;

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        public static string Path {
            get {return Global.Config.baseURL + "/Contact/Controls/JournalList.ascx";}
        }

        public JournalList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
        }

		#region Properities
        public long ContactID {
            get {return _contactID;}
            set {_contactID = value;}
        }

        public long JournalID {
            get {return _journalID;}
            set {_journalID = value;}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();

            _db = DBData.getDBData(Session);
            try {
                _db.connect();
                pageTitle.Text = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CT_CONTACTJOURNALLIST).Replace("#1", _db.lookup("NAME", "CONTACTV", "ID=" + _contactID, false).ToString());
                string sql = "select * from JOURNAL_CONTACT_V where CONTACT_ID=" + _contactID;

                sql += " order by " + OrderColumn + " " + OrderDir;

                DataTable table = _db.getDataTableExt(sql, "JOURNAL_CONTACT_V");
                table.Columns["JOURNAL_TYPE_ID"].ExtendedProperties["In"] =  ContactModule.getJournaltTypes(_db);
                table.Columns["CREATOR_PERSON_ID"].ExtendedProperties["In"] =  _db.Person.getWholeNameMATable(true);
                table.Columns["CREATOR_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%CREATOR_PERSON_ID", "mode","oe");
                IDColumn = "ID";
                if (_journalID > 0)
                    HighlightRecordID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "JOURNAL_CONTACT_V", "JOURNAL_ID=" + _journalID + " and CONTACT_ID=" + _contactID, false), -1);

                LoadList(_db, table, listTab);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell c) {
			// cst, 070913: added, set images right
			if (ListBuilder.IsInfoCell(c))
			{
				long journalTypeID = DBColumn.GetValid(row["JOURNAL_TYPE_ID"], -1L);

				if (journalTypeID > 0L)
				{
					string icon = _db.lookup("ICON", "JOURNAL_TYPE", "ID=" + journalTypeID, false);
					if (icon != ""  || icon != null) 
					{
						string imgURL = ((System.Web.UI.WebControls.Image) c.Controls[0]).ImageUrl;
						imgURL = imgURL.Replace("images/icon_info.png","images/journaltypes/" + icon);
						((System.Web.UI.WebControls.Image) c.Controls[0]).ImageUrl = imgURL;
					}
				}
			}
			/*
			//cst, 070913: commented out, images are not right set
			if (ListBuilder.IsInfoCell(c)){
                long journalTypeID = DBColumn.GetValid(row["JOURNAL_TYPE_ID"], -1L);
                if (journalTypeID > 0L){
                    string icon = _db.Journal.getJournalTypeIcon(journalTypeID);
                    if (icon != ""){
                        c.Text = c.Text.Replace("images/info.gif","Contact/images/" + icon);
                    }
                }
            }
			*/
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}
