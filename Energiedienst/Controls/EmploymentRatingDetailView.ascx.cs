namespace ch.appl.psoft.Energiedienst.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for EmploymentRatingDetailView.
    /// </summary>
    public partial  class EmploymentRatingDetailView : PSOFTDetailViewUserControl
	{
		public const string PARAM_RATING_ID = "PARAM_RATING_ID";
		public const string PARAM_EMPLOYMENT_ID = "PARAM_EMPLOYMENT_ID";
		public const string PARAM_RATING_TYPE_SELF = "PARAM_RATING_TYPE_SELF";
		public const string PARAM_CRITERIA_ID = "PARAM_CRITERIA_ID";
		public const string PARAM_JOB_EXPACTATION_ID = "PARAM_JOB_EXPACTATION_ID";
		public const string PARAM_RATINGITEM_ID = "PARAM_RATINGITEM_ID";


		private DBData _db = null;
		private DataTable _ratingLevels = null;
		private DataTable _ratingItems = null;
		private DataTable _argumentTable = null;
		protected ArrayList _dropDownListList = new ArrayList();


		#region Properities
		public long EmploymentRatingID 
		{
			get {return GetLong(PARAM_RATING_ID);}
			set {SetParam(PARAM_RATING_ID, value);}
		}

		public long CriteriaID 
		{
			get {return GetLong(PARAM_CRITERIA_ID);}
			set {SetParam(PARAM_CRITERIA_ID, value);}
		}

		public long EmploymentID 
		{
			get {return GetLong(PARAM_EMPLOYMENT_ID);}
			set {SetParam(PARAM_EMPLOYMENT_ID, value);}
		}
		

		public long RatingItemId 
		{
			get {return GetLong(PARAM_RATINGITEM_ID);}
			set {SetParam(PARAM_RATINGITEM_ID, value);}
		}

		public bool RatingTypeSelf 
		{
			get {return GetBool(PARAM_RATING_TYPE_SELF);}
			set {SetParam(PARAM_RATING_TYPE_SELF, value);}
		}
		#endregion



		public static string Path 
		{
			get {return Global.Config.baseURL + "/Energiedienst/Controls/EmploymentRatingDetailView.ascx";}
		}



		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}


		protected override void DoExecute() 
		{
			base.DoExecute ();

			if (!IsPostBack) 
			{
				TITLE_VALUE.Text = _mapper.get("");//_mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_PERFORMANCERATING);
			}

			_db = DBData.getDBData(Session);
			try 
			{
				_db.connect();

				_ratingLevels = _db.Performance.getPerformanceRatingLevels();
				_ratingItems = _db.Performance.getEmploymentRatingTable(EmploymentRatingID, CriteriaID,RatingItemId);

				bool isFirst = true;
				foreach (DataRow row in _ratingItems.Rows) 
				{
					if (isFirst)
					{
						isFirst = false;
					}
					else
					{
						TableRow r = new TableRow();
						TableCell c = new TableCell();
						r.Cells.Add(c);
						c.ColumnSpan = 2;
						c.Height = 10;
						editTab.Rows.Add(r);
					}

					addEmploymentRating(row);
				}

				if(!isFirst)
				{
					_argumentTable = _db.Performance.getCriteriaArgumentTable(EmploymentRatingID, CriteriaID, RatingItemId);
					base.LoadDetail(_db, _argumentTable, argumentTab);
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

		}


		protected void addEmploymentRating(DataRow row) 
		{
			long employmentRatingItemID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1);
			TableRow r = new TableRow();
			editTab.Rows.Add(r);
			r.VerticalAlign = VerticalAlign.Top;

			// 
			TableCell c = new TableCell();
			r.Cells.Add(c);
			c.CssClass = "Detail_special";
			c.Text = row[_db.langAttrName(row.Table.TableName, "CRITERIA_TITLE")].ToString();

			// Rating-level...
			c = new TableCell();
			r.Cells.Add(c);
			c.CssClass = "Detail";
            c.Text = row[_db.langAttrName(row.Table.TableName, "LEVEL_TITLE")].ToString();


			// Description
            
			r = new TableRow();
			editTab.Rows.Add(r);
			r.VerticalAlign = VerticalAlign.Top;

            c = new TableCell();
            r.Cells.Add(c);
            c.CssClass = "Detail_special";
            c.Text = _mapper.get(row.Table.TableName,_db.langAttrName(row.Table.TableName, "EXPECTATION_DESCRIPTION"));

			c = new TableCell();
			r.Cells.Add(c);
			c.CssClass = "Detail";
			c.Text = row[_db.langAttrName(row.Table.TableName, "EXPECTATION_DESCRIPTION")].ToString();

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
