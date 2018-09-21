namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for ContactList.
    /// </summary>
    public partial class ContactList : PSOFTListViewUserControl
    {
        private long _contactID = -1;
        private long _xID = -1;
        private string _mode = "";

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        public static string Path
        {
            get {return Global.Config.baseURL + "/Contact/Controls/ContactList.ascx";}
        }

        public ContactList() : base()
        {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
        }

		#region Properities
        public long contactID
        {
            get {return _contactID;}
            set {_contactID = value;}
        }

        public long xID
        {
            get {return _xID;}
            set {_xID = value;}
        }

        public string Mode
        {
            get {return _mode;}
            set {_mode = value;}
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
                string sql = "select * from CONTACTV";

                db.connect();
                switch (_mode.ToLower())
                {
                    case ContactDetail.MODE_CONTACTGROUP:
                        pageTitle.Text = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CTCONTACTGROUPLIST).Replace("#1", db.lookup("TITLE", "CONTACT_GROUP", "ID=" + _xID, false));
                        sql = "select * from CONTACT_GROUP_CONTACT_V where CONTACT_GROUP_ID=" + _xID;
                        View = "CONTACT_GROUP_CONTACT_V";
                        break;

                    case ContactDetail.MODE_SEARCHRESULT:
                        pageTitle.Text = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CTSEARCHCONTACTLIST);
                        sql = "select * from CONTACTV where ID IN (select ROW_ID from SEARCHRESULT where ID=" + _xID + ")";
                        View = "CONTACTV";
                        break;

                    case ContactDetail.MODE_FIRM:
                        pageTitle.Text = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CTFIRMCONTACTLIST).Replace("#1", db.lookup("TITLE", "FIRM", "ID=" + _xID, false));
                        sql = "select * from CONTACTV where FIRM_ID=" + _xID;
                        View = "CONTACTV";
                        break;
                }

                sql += " order by " + OrderColumn + " " + OrderDir;

                DataTable table = db.getDataTableExt(sql,View);
                IDColumn = "ID";
                if (_contactID > 0)
                    HighlightRecordID = _contactID;

                table.Columns["CONTACT_TYPE_ID"].ExtendedProperties["In"] = ContactModule.getContactTypes(db);
                table.Columns["CONTACT_ROLE_ID"].ExtendedProperties["In"] = ContactModule.getContactRoles(db);
                table.Columns["TABLENAME"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                if (_mode.ToLower() == ContactDetail.MODE_FIRM){
                    table.Columns["FIRM_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                }
                else{
                    table.Columns["FIRM_ID"].ExtendedProperties["In"] = ContactModule.getContactFirms(db);
                }

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

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (ListBuilder.IsInfoCell(cell)){
                string iconName = "kt_kontakt" + DBColumn.GetValid(row["TABLENAME"], "").ToLower() + ".gif";
                cell.Text = cell.Text.Replace("images/info.gif", "Contact/images/" + iconName);
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
