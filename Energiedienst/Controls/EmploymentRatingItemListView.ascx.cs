namespace ch.appl.psoft.Energiedienst.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for EmploymentRatingItemListView.
    /// </summary>
    public partial  class EmploymentRatingItemListView : PSOFTListViewUserControl
	{
		long _ratingID = -1;
        long _criteriaID = -1;
		//long _jobExpectationID = -1;

		long ratingItemId = -1;



		public static string Path
		{
			get {return Global.Config.baseURL + "/Energiedienst/Controls/EmploymentRatingItemListView.ascx";}
		}


		public EmploymentRatingItemListView() : base()
		{
			InfoBoxEnabled = false;
			HeaderEnabled = true;
			DetailEnabled = true;
			DeleteEnabled = false;
			EditEnabled = true;
			UseJavaScriptToSort = false;
			UseFirstLetterAsPageSelector = true;
            // disable ordering by title (ordering should be the same as on right list) / 02.02.10 / mkr
			//OrderColumn = "CRITERIA_TITLE";
            OrderColumn = "ID";
			OrderDir = "asc";
		}

		#region Properities
		public long RatingID
		{
			get {return _ratingID;}
			set {_ratingID = value;}
		}

        public long CriteriaID {
            get {return _criteriaID;}
            set {_criteriaID = value;}
        }

		/*
		public long JobExpectationID 
		{
			get {return _jobExpectationID;}
			set {_jobExpectationID = value;}
		}
		*/

		public long RatingItemId
		{
			get {return ratingItemId;}
			set {ratingItemId = value;}
		}

		#endregion


		private DBData db = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}


		protected override void DoExecute()
		{
			base.DoExecute();

			db = DBData.getDBData(Session);
			try 
			{
				db.connect();
                DataTable itemTable = db.Performance.getEmploymentRatingTable(_ratingID, db.langAttrName("PERFORMANCERATING_ITEMS", OrderColumn), OrderDir);
                pageTitle.Text = _mapper.get("performance", "employmentRatingList");
				IDColumn = "ID";
				CheckOrder = true;
				View = "";
				EditEnabled = false;
				itemTable.Columns["RELATIV_WEIGHT"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
				itemTable.Columns["EXPECTATION_VALID_DATE"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
                itemTable.Columns["EXPECTATION_DESCRIPTION_DE"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
                itemTable.Columns["EXPECTATION_DESCRIPTION_FR"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
                itemTable.Columns["EXPECTATION_DESCRIPTION_EN"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
                itemTable.Columns["EXPECTATION_DESCRIPTION_IT"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
				LoadList(db, itemTable, listTab);
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

		protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell c) 
		{					
			
            long id = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1);
			if(id != -1 && id == this.RatingItemId)
			{
				c.CssClass = "List_selected";
			}
			else if(id == -1)
			{
					id = ch.psoft.Util.Validate.GetValid(row["CRITERIA_REF"].ToString(), -1);
					if(CriteriaID == id) c.CssClass = "List_selected";
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
