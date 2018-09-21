namespace ch.appl.psoft.Performance.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for EmploymentRatingEditView.
    /// </summary>
    public partial  class EmploymentRatingEditView : PSOFTInputViewUserControl
	{
		public const string PARAM_RATING_ID = "PARAM_RATING_ID";
		public const string PARAM_EMPLOYMENT_ID = "PARAM_EMPLOYMENT_ID";
		public const string PARAM_RATING_TYPE_SELF = "PARAM_RATING_TYPE_SELF";
        public const string PARAM_CRITERIA_ID = "PARAM_CRITERIA_ID";
		public const string PARAM_JOB_EXPACTATION_ID = "PARAM_JOB_EXPACTATION_ID";
        public const string PARAM_ORDERCOLUMN = "PARAM_ORDERCOLUMN";
        public const string PARAM_ORDERDIR = "PARAM_ORDERDIR";
		public const string PARAM_RATINGITEM_ID = "PARAM_RATINGITEM_ID";


		private DBData _db = null;
		private DataTable _ratingLevels = null;
		private DataTable _ratingItems = null;
        private DataTable _rating = null;
        private string totComment =" ";
		protected ArrayList _dropDownListList = new ArrayList();
		protected string _updateRatingSQL = "";
		protected string _updateArgumentsSQL = "";
        protected string _updateMeasureSQL = " ";


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

		public bool RatingTypeSelf 
		{
			get {return GetBool(PARAM_RATING_TYPE_SELF);}
			set {SetParam(PARAM_RATING_TYPE_SELF, value);}
		}

        public string OrderColumn {
            get {return GetString(PARAM_ORDERCOLUMN);}
            set {SetParam(PARAM_ORDERCOLUMN, value);}
        }

        public string OrderDir {
            get {return GetString(PARAM_ORDERDIR);}
            set {SetParam(PARAM_ORDERDIR, value);}
        }

		public long RatingItemId 
		{
			get {return GetLong(PARAM_RATINGITEM_ID);}
			set {SetParam(PARAM_RATINGITEM_ID, value);}
		}

		#endregion

        private long _performanceRatingItem = -1;

		public static string Path 
		{
			get {return Global.Config.baseURL + "/Performance/Controls/EmploymentRatingEditView.ascx";}
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
				apply.Text = _mapper.get("apply");
				TITLE_VALUE.Text = _mapper.get("");//_mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_PERFORMANCERATING);
			}

			_db = DBData.getDBData(Session);
			try 
			{
				_db.connect();

				_ratingLevels = _db.Performance.getPerformanceRatingLevels();
				_ratingItems = _db.Performance.getEmploymentRatingViewTable(EmploymentRatingID, CriteriaID,RatingItemId);
                _rating = _db.getDataTable("SELECT * FROM PERFORMANCERATING WHERE ID = " + EmploymentRatingID);
               
				
				if(_ratingItems.Rows.Count == 1)
				{
					_performanceRatingItem = DBColumn.GetValid(_ratingItems.Rows[0]["ID"],-1L);
				}

				
				InputType = InputMaskBuilder.InputType.Edit;
				//_ratingItems.Columns["LEVEL_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
				_ratingItems.Columns["LEVEL_REF"].ExtendedProperties["InputControlType"] = typeof(RadioButtonList);
				_ratingItems.Columns["LEVEL_REF"].ExtendedProperties["In"] = _ratingLevels;
				_ratingItems.Columns[_db.langAttrName("PERFORMANCERATING_ITEMS_V", "CRITERIA_TITLE")].ExtendedProperties["InputControlType"] = typeof(Label);
				_ratingItems.Columns[_db.langAttrName("PERFORMANCERATING_ITEMS_V", "EXPECTATION_DESCRIPTION")].ExtendedProperties["InputControlType"] = typeof(Label);
				_ratingItems.Columns[_db.langAttrName("PERFORMANCERATING_ITEMS_V", "DESCRIPTION")].ExtendedProperties["InputControlType"] = typeof(Label);

                if (!PerformanceModule.showMeasure)
                {
                    _ratingItems.Columns["MEASURE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                }

                if (PerformanceModule.showGlobalComment)
                {
                    DataColumn GLOBAL_COMMENT = new DataColumn();
                    foreach (DictionaryEntry prop in _ratingItems.Columns["MEASURE"].ExtendedProperties)
                    {
                        GLOBAL_COMMENT.ExtendedProperties.Add(prop.Key, prop.Value);
                    }
                   GLOBAL_COMMENT.ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                   GLOBAL_COMMENT.ExtendedProperties["OrdNum"] = 9;
                   GLOBAL_COMMENT.MaxLength = 2000;
                   GLOBAL_COMMENT.ColumnName = "GLOBAL_COMMENT";
                   GLOBAL_COMMENT.Caption = "GLOBAL_COMMENT";
                   
                   _ratingItems.Columns.Add(GLOBAL_COMMENT);
                   if (_ratingItems.Rows.Count >0)
                   {
                       _ratingItems.Rows[0]["GLOBAL_COMMENT"] = _rating.Rows[0]["GLOBAL_COMMENT"].ToString();
                   }

                }

				base.CheckOrder = true;
				LoadInput(_db, _ratingItems, listTab);

				apply.Visible = _ratingItems.Rows.Count > 0;//!isFirst;
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

		protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) 
		{
			if (col != null)
			{
				if(col.ColumnName.IndexOf("CRITERIA_TITLE_") >= 0)
				{
					r.Cells[1].CssClass = "Detail_Label";
				}
				switch (col.ColumnName.ToUpper())
				{						
					case "LEVEL_REF":
						RadioButtonList rb = (RadioButtonList) r.Cells[1].Controls[0];
						rb.RepeatDirection = System.Web.UI.WebControls.RepeatDirection.Vertical;
						break;
					default:
						break;
				}
			}
		}


		protected override void onBuildSQL (StringBuilder build, System.Web.UI.Control control, DataColumn col, object val) 
		{
			if (col.ColumnName == "LEVEL_REF") 
			{
				_updateRatingSQL = "update PERFORMANCERATING_ITEMS";
				if (val.Equals(string.Empty))
				{
					_updateRatingSQL += " set RELATIV_WEIGHT=-1, LEVEL_REF = null";
					_updateRatingSQL += "," + _db.langExpand("LEVEL_TITLE%LANG%='-'", "PERFORMANCERATING_ITEMS", "LEVEL_TITLE");
					_updateRatingSQL += "," + _db.langExpand("LEVEL_DESCRIPTION%LANG%='-'", "PERFORMANCERATING_ITEMS", "LEVEL_DESCRIPTION");
					_updateRatingSQL += " where";
				}
				else 
				{
					_updateRatingSQL += " set PERFORMANCERATING_ITEMS.RELATIV_WEIGHT=PERFORMANCE_LEVEL.RELATIV_WEIGHT, PERFORMANCERATING_ITEMS.LEVEL_REF="+val.ToString();
					_updateRatingSQL += "," + _db.langExpand("PERFORMANCERATING_ITEMS.LEVEL_TITLE%LANG%=PERFORMANCE_LEVEL.TITLE%LANG%", "PERFORMANCERATING_ITEMS", "LEVEL_TITLE");
					_updateRatingSQL += "," + _db.langExpand("PERFORMANCERATING_ITEMS.LEVEL_DESCRIPTION%LANG%=PERFORMANCE_LEVEL.DESCRIPTION%LANG%", "PERFORMANCERATING_ITEMS", "LEVEL_DESCRIPTION");
					_updateRatingSQL += " from PERFORMANCERATING_ITEMS, PERFORMANCE_LEVEL where PERFORMANCE_LEVEL.ID=" + val + " and";
				}		
				_updateRatingSQL += " PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF=" + EmploymentRatingID;
				_updateRatingSQL += " and PERFORMANCERATING_ITEMS.CRITERIA_REF=" + CriteriaID;
				if(RatingItemId > 0)
				{
					_updateRatingSQL += " and PERFORMANCERATING_ITEMS.ID =" + RatingItemId;
				}
				
			}
            else if (col.ColumnName == "ARGUMENTS")
			{
				//convert string to sql string / 24.08.10 / mkr
                string strVal = val.ToString();
                strVal = strVal.Replace("'", "''");
                
                _updateArgumentsSQL = "update PERFORMANCERATING_ARGUMENTS";
                _updateArgumentsSQL += " set ARGUMENTS='" + strVal + "'";
				_updateArgumentsSQL += " where PERFORMANCERATING_REF=" + EmploymentRatingID;
				_updateArgumentsSQL += " and PERFORMANCERATING_CRITERIA_REF=" + CriteriaID;
				
				if(RatingItemId > 0)
				{
					_updateArgumentsSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID=" + RatingItemId;
				}
				else
				{
					_updateArgumentsSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID is null";
				}
			}
            if (col.ColumnName == "MEASURE")
            {
                string strVal = val.ToString();
                strVal = strVal.Replace("'", "''");
                
                _updateMeasureSQL = "update PERFORMANCERATING_ARGUMENTS";
                _updateMeasureSQL += " set MEASURE='" + strVal + "'";
				_updateMeasureSQL += " where PERFORMANCERATING_REF=" + EmploymentRatingID;
				_updateMeasureSQL += " and PERFORMANCERATING_CRITERIA_REF=" + CriteriaID;
				
				if(RatingItemId > 0)
				{
					_updateMeasureSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID=" + RatingItemId;
				}
				else
				{
					_updateMeasureSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID is null";
				}
            }

            if (col.ColumnName == "GLOBAL_COMMENT")
            {
                totComment = val.ToString().Replace("'", "''"); ;
            }

		}


		protected void apply_Click(object sender, System.EventArgs e) 
		{
            bool inTransaction = false;
			_db.connect();
            try
            {
                // Begründung als Plichtfeld setzen, falls gewählte Bewertung nicht dem Soll (=50%) entspricht...
                double weight = _db.lookup("RELATIV_WEIGHT", "PERFORMANCE_LEVEL", "ID=" + ch.psoft.Util.Validate.GetValid(getInputValue(_ratingItems, listTab, "LEVEL_REF").ToString(), -1L), 0.0);
                if (Global.isModuleEnabled("habasit") || Global.isModuleEnabled("laufenburg"))
                {
                    _ratingItems.Columns["ARGUMENTS"].ExtendedProperties["Nullable"] = false;
                }
                else if (Global.isModuleEnabled("foampartner"))
                {
                    _ratingItems.Columns["ARGUMENTS"].ExtendedProperties["Nullable"] = true;
                }
                else
                {
                    _ratingItems.Columns["ARGUMENTS"].ExtendedProperties["Nullable"] = weight == 50.0;
                }

                if (checkInputValue(_ratingItems, listTab))
                {
                    _db.beginTransaction();
                    inTransaction = true;

                    string sql = "";
                    StringBuilder sb = base.getSql(_ratingItems, listTab, true);

                    sql = base.endExtendSql(sb);

                    if (_updateRatingSQL != "")
                        _db.execute(_updateRatingSQL);

                    if (_updateArgumentsSQL != "")
                        _db.execute(_updateArgumentsSQL);
                    if (_updateMeasureSQL != "")
                        _db.execute(_updateMeasureSQL);

                    _db.commit();
                    if (!totComment.Equals(" "))
                    {
                        _db.execute("UPDATE PERFORMANCERATING SET GLOBAL_COMMENT ='" + totComment + "' WHERE ID =" + EmploymentRatingID);
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
                if (checkInputValue(_ratingItems, listTab))
                {
                    Response.Redirect(Request.Url.AbsoluteUri);
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
