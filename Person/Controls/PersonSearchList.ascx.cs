namespace ch.appl.psoft.Person.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;

    /// <summary>
    /// Summary description for List.
    /// </summary>
    public partial class PersonSearchList : PSOFTSearchListUserControl 
	{
		public const string PARAM_SQL = "PARAM_SQL";
		public const string PARAM_RELOAD = "PARAM_RELOAD";
		public const string PARAM_BACK_URL = "PARAM_BACK_URL";
		public const string PARAM_BACK_TARGET = "PARAM_BACK_TARGET";
        public const string PARAM_SUCCESS_HINT = "PARAM_SUCCESS_HINT";


		public static string Path
		{
			get {return Global.Config.baseURL + "/Person/Controls/PersonSearchList.ascx";}
		}

        public PersonSearchList() : base()
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

		public string BackURL 
		{
			get { return GetString(PARAM_BACK_URL); }
			set {SetParam(PARAM_BACK_URL, value);}
		}

		public string BackTarget 
		{
			get { return GetString(PARAM_BACK_TARGET); }
			set {SetParam(PARAM_BACK_TARGET, value);}
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
		/// <param name="singleResultRecord">true: radio-buttons / false: check-boxes</param>
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
				SearchSQL = ch.psoft.Util.Validate.GetValid(SearchSQL, "select * from person where ID=-1");

			next.Text = _mapper.get("next");
			pageTitle.Text = _mapper.get("person","pageTitleSearchListPerson");

			try 
			{
				db.connect();
                long groupAccessorId = DBColumn.GetValid(
                    db.lookup("ID", "ACCESSOR", "TITLE = 'HR'"),
                    (long)-1);
                // Gruppe HR bei SPZ zusätzliche Felder anzeigen
                if (Global.isModuleEnabled("spz") & (db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true)))
                {
                    //SearchSQL.Replace("
                    table = db.getDataTableExt(SearchSQL + " order by " + OrderColumn + " " + OrderDir, "PERSONOEV");
                    table.Columns["JOB_TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    table.Columns["MNEMO"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                }
                else
                {
                    table = db.getDataTableExt(SearchSQL + " order by " + OrderColumn + " " + OrderDir, "PERSON");
                }
				
                
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

            BackURL = BackURL.Replace("%25SearchResultID","%SearchResultID").Replace("%SearchResultID", searchResultID.ToString());

			_nextArgs.LoadUrl = this.BackURL;
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