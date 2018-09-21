namespace ch.appl.psoft.Performance.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for ExpectationListView.
    /// </summary>
    public partial class ExpectationListView : PSOFTSearchListUserControl
	{
        public const string PARAM_SQL = "PARAM_SQL";
        public const string PARAM_RELOAD = "PARAM_RELOAD";
        public const string PARAM_BACK_URL = "PARAM_BACK_URL";

        private long _jobID = -1;
		private string _mode = "job";



		public static string Path
		{
			get {return Global.Config.baseURL + "/Performance/Controls/ExpectationListView.ascx";}
		}

		public ExpectationListView() : base()
		{
			InfoBoxEnabled = false;
			HeaderEnabled = true;
			DetailEnabled = true;
			DeleteEnabled = true;
			EditEnabled = true;
			UseJavaScriptToSort = false;
			UseFirstLetterAsPageSelector = true;
		}


		#region Properities
		public long JobID
		{
			get {return _jobID;}
			set {_jobID = value;}
		}

        public string Mode {
            get {return _mode;}
            set {_mode = value;}
        }

        public bool Reload {
            get {return GetBool(PARAM_RELOAD);}
            set {SetParam(PARAM_RELOAD, value);}
        }

        public string BackURL {
            get { return GetString(PARAM_BACK_URL); }
            set {SetParam(PARAM_BACK_URL, value);}
        }

        public string SearchSQL {
            get {return GetString(PARAM_SQL);}
            set {SetParam(PARAM_SQL, value);}
        }
		#endregion


		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (this.Visible) {
                Execute();
            }
        }


		protected override void DoExecute()
		{
            if (SearchSQL == "" && _mode.ToLower() == "search")
                return;

            base.DoExecute();

			DBData db = DBData.getDBData(Session);
			try 
			{
				db.connect();
                IDColumn = "ID";
                DataTable table = null;
                CheckOrder = true;
                switch (_mode.ToLower()){
                    case "job":
                        pageTitle.Text = _mapper.get("performance", "jobExpectationList");
                        //DeleteEnabled = (db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "JOB_EXPECTATION", _jobID, true, true) || db.Person.isLeaderOfJob(_jobID, true));
                        table = db.Performance.getJobExpectationTable(_jobID, OrderColumn, OrderDir);
                        ButtonRow.Visible = false;
                        break;

                    case "search":
                        listTab.Rows.Clear();
                        if(Reload)
                            SearchSQL = ch.psoft.Util.Validate.GetValid(SearchSQL, "select * from JOB_EXPECTATION_V where ID=-1");
                        pageTitle.Text = _mapper.get("performance", "jobExpectationSearchResultList");
                        table = db.getDataTableExt(SearchSQL + " order by " + OrderColumn + " " + OrderDir, "JOB_EXPECTATION_V");
                        next.Text = _mapper.get("next");
                        CheckBoxEnabled = true;
                        break;
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

        protected void next_Click(object sender, System.EventArgs e) {
            long searchResultID = SaveInSearchResult(listTab, "JOB_EXPECTATION");

            BackURL = BackURL.Replace("%25SearchResultID","%SearchResultID").Replace("%SearchResultID", searchResultID.ToString());

            _nextArgs.LoadUrl = BackURL;
            _nextArgs.SearchResultID = searchResultID;
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
