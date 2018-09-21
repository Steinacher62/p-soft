namespace ch.appl.psoft.Performance.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for EmploymentRatingHistoryListView.
    /// </summary>
    public partial  class EmploymentRatingHistoryListView : PSOFTListViewUserControl
	{
		protected long _employmentID = -1;
		protected bool _selfRating = false;
		protected DBData db = null;

		protected string _redComment;
		protected string _greenComment;
		protected string _doneComment;



		public static string Path
		{
			get {return Global.Config.baseURL + "/Performance/Controls/EmploymentRatingHistoryListView.ascx";}
		}


		public EmploymentRatingHistoryListView() : base()
		{
			InfoBoxEnabled = false;
			HeaderEnabled = true;
			DetailEnabled = true;
			DeleteEnabled = true;
			EditEnabled = false;
			UseJavaScriptToSort = false;
			UseFirstLetterAsPageSelector = true;
			OrderColumn = "RATING_DATE";
			OrderDir = "asc";
		}

		public string RatingType
		{
			get {return _selfRating ? "self" : "leader";}
		}


		#region Properities
		public long EmploymentID
		{
			get {return _employmentID;}
			set {_employmentID = value;}
		}

		private long _jobID = -1;
		public long JobID
		{
			get {return _jobID;}
			set {_jobID = value;}
		}

		public bool SelfRating
		{
			get {return _selfRating;}
			set {_selfRating = value;}
		}
		#endregion


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
                string orgunit = "";
                if (JobID == -1)
                {
                    orgunit = "(" + _mapper.get("performance", "employmentRatingIsHistory") + ")";
                }
                else
                {
                    orgunit = db.lookup(db.langAttrName("ORGENTITY", "TITLE"), "ORGENTITY", "ID in (select distinct ORGENTITY_ID from JOB where ID = " + JobID + ")", "");
                    //orgunit = db.lookup(db.langAttrName("ORGENTITY", "TITLE"), "ORGENTITY", "ID in (select distinct ORGENTITY_ID from JOB)", "");
                    if (orgunit != "")
                    {
                        orgunit = "(" + _mapper.get("performance", "department") + orgunit + ")";
                    }
                }
				pageTitle.Text = _mapper.get("performance", "employmentRatingHistoryList") + " " + orgunit;

				IDColumn = "ID";
				CheckOrder = true;
				DetailURL = "EmploymentRating.aspx?performanceRatingID=%ID&employmentID="+EmploymentID.ToString()+"&mode=edit&type="+ (SelfRating ? "self" : "leader");
				View = "";
				ArrayList boolList = new ArrayList(_mapper.getEnum("boolean", true));
				//DataTable historyTable = db.Performance.getEmploymentRatingHistoryTable(_employmentID, JobID, _selfRating, OrderColumn, OrderDir);
                DataTable historyTable = db.Performance.getEmploymentRatingHistoryTable(_employmentID, -2, _selfRating, OrderColumn, OrderDir);
				historyTable.Columns["IS_SELFRATING"].ExtendedProperties["In"] = boolList;
                historyTable.Columns["RATING_PERSON_REF"].ExtendedProperties["In"] = db.Person.getWholeNameMATable(true);
                historyTable.Columns["RATING_PERSON_REF"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%RATING_PERSON_REF", "mode","oe");
                LoadList(db, historyTable , listTab);
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

        protected override void onAfterAddCells(DataRow row, TableRow r) {
            long ID = DBColumn.GetValid(row["ID"],0L);
            TableCell cSemaphore = new TableCell();
            cSemaphore.CssClass = "List";
            r.Cells.Add(cSemaphore);
            System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
            switch (db.Performance.getEmploymentRatingState(ID)) {
                case 0: //nothing done
                    image.ImageUrl = "../../images/ampelRot.gif";
                    image.ToolTip = _redComment;
                    break;
                case 1: //some done
                    image.ImageUrl = "../../images/ampelOrange.gif";
                    image.ToolTip = _doneComment;
                    break;
                case 2: //all done
                    image.ImageUrl = "../../images/ampelGruen.gif";
                    image.ToolTip = _greenComment;						
                    break;
                default:
                    break;
            }

            cSemaphore.Controls.Add(image);
        }
        
        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) 
		{
//            if (col != null && row != null && cell != null)
//            {
//                switch(col.ColumnName)
//                {
//                    case "RATING_PERSON_REF":
//                        cell.Text = db.Person.getWholeName(row[col].ToString());
//                        break;
//                    default:
//                        break;
//                }
//            }
            if (row != null && cell != null)
            {
                if (cell.ClientID.IndexOf("LB_DELETE_CELL") > 0)
                {
                    if (ch.psoft.Util.Validate.GetValid(row["LOCK"].ToString(),0) > 0)
                    {
                        cell.Visible = false;
                    }
                }
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
