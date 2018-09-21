namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Summary description for ContactSearchList.
    /// </summary>
    public partial class ContactSearchList : PSOFTSearchListUserControl 
	{
		public const string PARAM_SQL = "PARAM_SQL";
		public const string PARAM_RELOAD = "PARAM_RELOAD";
		public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_SUCCESS_HINT = "PARAM_SUCCESS_HINT";


		public static string Path
		{
			get {return Global.Config.baseURL + "/Contact/Controls/ContactSearchList.ascx";}
		}

        public ContactSearchList() : base()
        {
            DeleteEnabled = false;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            CheckBoxEnabled = true;
            UseFirstLetterAsPageSelector = true;
            HeaderEnabled = true;
        }

		#region Properities
		public bool Reload
		{
			get {return GetBool(PARAM_RELOAD);}
			set {SetParam(PARAM_RELOAD, value);}
		}

		public string NextURL
		{
			get { return GetString(PARAM_NEXT_URL); }
			set {SetParam(PARAM_NEXT_URL, value);}
		}

		public string SearchSQL
		{
			get {return GetString(PARAM_SQL);}
			set {SetParam(PARAM_SQL, value);}
		}
		#endregion

		/// <summary>
		/// Load page
		/// </summary>
		/// <param name="orderColumn">Column for order by</param>
		/// <param name="reload">true: reload page</param>
		/// <param name="backURL">URL to go redirect afterwards (replaces %SearchResultID by current id)</param>
		/// <param name="backTarget">Target for backURL</param>
		/// </param>
		protected void Page_Load(object sender, System.EventArgs e) 
		{
			if (this.Visible)
			{
				Execute();
			}
		}

		protected override void DoExecute()
		{
			if ((SearchSQL == null) || (SearchSQL.Trim() == ""))
				return;

            base.DoExecute();

            listTab.Rows.Clear();

			DBData db = DBData.getDBData(Session);
			DataTable table;

			if(Reload)
				SearchSQL = ch.psoft.Util.Validate.GetValid(SearchSQL, "select * from CONTACTV where ID=-1");

			next.Text = _mapper.get("next");
			pageTitle.Text = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_PTSEARCHCONTACTLIST);

			try 
			{
				db.connect();
				table = db.getDataTableExt(SearchSQL + " order by " + OrderColumn + " " + OrderDir, "CONTACTV");

                table.Columns["CONTACT_TYPE_ID"].ExtendedProperties["In"] = ContactModule.getContactTypes(db);
                table.Columns["CONTACT_ROLE_ID"].ExtendedProperties["In"] =  ContactModule.getContactRoles(db);
                table.Columns["TABLENAME"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["FIRM_ID"].ExtendedProperties["In"] =  ContactModule.getContactFirms(db);
                
                IDColumn = "ID";
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

        /// <summary>
		/// Map all control event to its server side handler methode
		/// </summary>
		private void mapControls () 
		{
			this.next.Click += new System.EventHandler(this.next_Click);
		}

		/// <summary>
		/// Event handler for the 'next' button
		/// The selected item(s) database ID will be stored in the SEARCHRESULT table
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void next_Click(object sender, System.EventArgs e) 
		{
            long searchResultID = SaveInSearchResult(listTab, "PERSON");
            searchResultID = SaveInSearchResult(listTab, "FIRM", searchResultID);

            NextURL = NextURL.Replace("%25SearchResultID","%SearchResultID").Replace("%SearchResultID", searchResultID.ToString());

			_nextArgs.LoadUrl = NextURL;
			DoOnNextClick(next);
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
		}
		#endregion
	}
}