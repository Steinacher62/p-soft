namespace ch.appl.psoft.Performance.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for ExpectationDetailView.
    /// </summary>
    public partial  class ExpectationDetailView : PSOFTDetailViewUserControl
	{
		public const string PARAM_EXPECTATION_ID = "PARAM_EXPECTATION_ID";


		protected DBData _db = null;


		#region Properities
		public long ExpectationID 
		{
			get {return GetLong(PARAM_EXPECTATION_ID);}
			set {SetParam(PARAM_EXPECTATION_ID, value);}
		}

		#endregion


		public static string Path 
		{
			get {return Global.Config.baseURL + "/Performance/Controls/ExpectationDetailView.ascx";}
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}


		protected override void DoExecute() 
		{
			base.DoExecute();

			_db = DBData.getDBData(Session);

			_db.connect();
			try 
			{
				if (!IsPostBack) 
				{
					detailTitle.Text = _mapper.get("performance","detailJobExpectation");
				}

				// load details of measure
				detailTab.Rows.Clear();
				DataTable _table = _db.Performance.getJobExpectationEditTable(ExpectationID);
				CheckOrder = true;
				base.LoadDetail(_db, _table, detailTab);
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
