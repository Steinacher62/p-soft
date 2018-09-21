namespace ch.appl.psoft.Performance.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Text;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for ExpectationAddView.
    /// </summary>
    public partial  class ExpectationAddView : PSOFTInputViewUserControl
	{
		public const string PARAM_BACK_URL = "PARAM_BACK_URL";
		public const string PARAM_JOB_ID = "PARAM_JOB_ID";

		protected DataTable _table;
		protected DropDownList _templateChooser = null;
		protected DBData _db = null;



		#region Properities
		public string BackUrl 
		{
			get {return GetString(PARAM_BACK_URL);}
			set {SetParam(PARAM_BACK_URL, value);}
		}

		public long JobID 
		{
			get {return GetLong(PARAM_JOB_ID);}
			set {SetParam(PARAM_JOB_ID, value);}
		}
		#endregion


		public static string Path 
		{
			get {return Global.Config.baseURL + "/Performance/Controls/ExpectationAddView.ascx";}
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();

            if (Global.Config.getModuleParam("performance", "copyJobexpectation", "0") == "1")
            {
                DBData _db = null;
                _db = DBData.getDBData(Session);
                _db.connect();
                //string jobId = ch.psoft.Util.Validate.GetValid(_db.lookup("JOB_REF", "JOB_EXPECTATION", "ID = " + JobID).ToString(), "-1");

                //lists persons with same functions
                string confirmText = _mapper.get("expectation", "ConfirmCopy");
                lblConfirmText.Value = confirmText + "\n\n";

                DataTable jobTable = PerformanceModule.GetPersonJobs(JobID.ToString(), _db);

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

			CheckOrder = true;
			_db.connect();
			try 
			{
				if (!IsPostBack) 
				{
					apply.Text = _mapper.get("apply");
					pageTitle.Text = _mapper.get("performance","newJobExpectation");
				}

				_table = _db.Performance.getJobExpectationInsertTable();

				_table.Columns["CRITERIA_REF"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
				_table.Columns["CRITERIA_REF"].ExtendedProperties["In"] = _db.Performance.getJobCriteriaTable(JobID);

				base.LoadInput(_db,_table,addTab);
                setInputValue(_table, addTab, "VALID_DATE", DateTime.Now.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern));
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
			this.apply.Click += new System.EventHandler(apply_Click);
		}


		private void apply_Click(object sender, System.EventArgs e) 
		{
			if (!base.checkInputValue(_table,addTab))
				return;
            
			long newID = -1;

			_db.connect();
			try 
			{
				_db.beginTransaction();
				StringBuilder sb = base.getSql(_table, addTab, true);
				newID = _db.newId(_table.TableName);

				base.extendSql(sb, _table, "ID", newID);
				base.extendSql(sb, _table, "JOB_REF", JobID);
				string sql = base.endExtendSql(sb);

				if (sql != "") 
				{
					_db.execute(sql);

					_db.commit();

                    int num = (int)_db.lookup("COUNT(ID) AS Anz", "JOB_EXPECTATION", "(CRITERIA_REF = (SELECT CRITERIA_REF FROM JOB_EXPECTATION AS JOB_EXPECTATION_1 WHERE ID = " + newID + ")) AND (JOB_REF = (SELECT JOB_REF FROM JOB_EXPECTATION AS JOB_EXPECTATION_2 WHERE ID = " + newID + ")) GROUP BY CRITERIA_REF");              //"SELECT COUNT(ID) AS Anz FROM JOB_EXPECTATION WHERE(CRITERIA_REF = (SELECT CRITERIA_REF FROM JOB_EXPECTATION AS JOB_EXPECTATION_1 WHERE ID = " + newID + ")) AND (JOB_REF = (SELECT JOB_REF FROM JOB_EXPECTATION AS JOB_EXPECTATION_2 WHERE ID = " + newID + ")) GROUP BY CRITERIA_REF";


                    sql = "UPDATE JOB_EXPECTATION SET ORDNUMBER = " + num + " WHERE ID = " + newID;
                    _db.execute(sql);

                    sql = "select * from job_expectation where id = " + newID.ToString();
                    if (Global.Config.getModuleParam("performance", "copyJobexpectation", "0") == "1")
                    {    
                        DataTable jobTable = _db.getDataTableExt(sql, "JOB_EXPECTATION");
                        if (Global.isModuleEnabled("spz"))
                        {
                            PerformanceModule.CopyJobExpectationJobName(_db, jobTable);
                        }
                        else
                        {
                            PerformanceModule.CopyJobExpectationFunktion(_db, jobTable);
                        }
                    }

				}
				else
					_db.rollback();
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
            if (BackUrl == "")
                Response.Redirect("JobExpectation.aspx?ID=" + JobID + "&expectationID=" + newID, false);
            else
                Response.Redirect(BackUrl);

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
