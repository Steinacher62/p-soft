namespace ch.appl.psoft.Performance.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for ExpectationEditView.
    /// </summary>
    public partial  class ExpectationEditView : PSOFTInputViewUserControl
	{
		public const string PARAM_BACK_URL = "PARAM_BACK_URL";
		public const string PARAM_EXPECTATION_ID = "PARAM_EXPECTATION_ID";


		private DBData _db = null;
		private DataTable _table;


		#region Properities
		public string BackUrl 
		{
			get {return GetString(PARAM_BACK_URL);}
			set {SetParam(PARAM_BACK_URL, value);}
		}

		public long ExpectationID 
		{
			get {return GetLong(PARAM_EXPECTATION_ID);}
			set {SetParam(PARAM_EXPECTATION_ID, value);}
		}
		#endregion


		public static string Path 
		{
			get {return Global.Config.baseURL + "/Performance/Controls/ExpectationEditView.ascx";}
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();

            if (Global.Config.getModuleParam("performance", "copyJobexpectation", "0") == "1")
            {
                DBData _db = null;
                _db = DBData.getDBData(Session);
                _db.connect();
                string jobId = ch.psoft.Util.Validate.GetValid(_db.lookup("JOB_REF", "JOB_EXPECTATION", "ID = " + ExpectationID).ToString(), "-1");
                
                //lists persons with same functions
                string confirmText = _mapper.get("expectation", "ConfirmCopy");
                lblConfirmText.Value = confirmText + "\n\n";

                DataTable jobTable = PerformanceModule.GetPersonJobs(jobId, _db);

                //get persons with this jobs
                foreach (DataRow job in jobTable.Rows)
                {
                    //get person id
                    string person_Id = ch.psoft.Util.Validate.GetValid(_db.lookup("PERSON_ID", "EMPLOYMENT", "ID = " + job["EMPLOYMENT_ID"].ToString()).ToString(), "-1");

                    //get person name
                    string name = ch.psoft.Util.Validate.GetValid(_db.lookup("FIRSTNAME + ' ' + PNAME AS NAME", "PERSON", "ID = " + person_Id).ToString(), "");
                    lblConfirmText.Value += name + "\n";
                }

                //add confirmation to button (only if more than one job affected)
                if (jobTable.Rows.Count > 1)
                {
                    apply.Attributes.Add("onclick", "javascript: return confirmCopy('" + lblConfirmText.ClientID + "')");
                }

                _db.disconnect();
            }
		}

		protected override void DoExecute() 
		{
			base.DoExecute ();
			_db = DBData.getDBData(Session);
			base.InputType = InputMaskBuilder.InputType.Edit;
			
			string sql = "select * from JOB_EXPECTATION where ID="+ExpectationID;
            CheckOrder = true;
			try 
			{
				if (!IsPostBack) 
				{
					apply.Text = _mapper.get("apply");
					TITLE_VALUE.Text = _mapper.get("performance","editExpectation");
				}
				_db.connect();
				_table = _db.getDataTableExt(sql,"JOB_EXPECTATION");

				_table.Columns["CRITERIA_REF"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
				_table.Columns["CRITERIA_REF"].ExtendedProperties["In"] = _db.Performance.getJobCriteriaTable(ch.psoft.Util.Validate.GetValid(_table.Rows[0]["JOB_REF"].ToString(),-1L) );

				base.LoadInput(_db, _table, editTab);
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
			if (!base.checkInputValue(_table, editTab))
				return;

			_db.connect();
			try
            {
                string sql = base.getSql(_table, editTab);

                if (sql != "")
                {  
                    _db.execute(sql);
                }

                if (Global.Config.getModuleParam("performance", "copyJobexpectation", "0")=="1") 
                {
                    if (Global.isModuleEnabled("spz"))
                    {
                        PerformanceModule.CopyJobExpectationJobName(_db, _table);
                    }
                    else
                    {
                        PerformanceModule.CopyJobExpectationFunktion(_db, _table);
                    }
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

			if (BackUrl != "")
				Response.Redirect(BackUrl, false);
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
