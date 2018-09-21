namespace ch.appl.psoft.Performance.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PerformanceRatingSelectPeriod.
    /// </summary>
    public partial class PerformanceRatingSelectPeriod : PSOFTSearchUserControl
	{
        public const string PARAM_PERIOD = "PARAM_PERIOD";
        public const string PARAM_PERIODII = "PARAM_PERIODII";
        public const string PARAM_CONTEXT = "PARAM_CONTEXT";
        public const string PARAM_OEIDS = "PARAM_OEIDS";

        private const string _TABLE = "PERFORMANCERATING_PERIOD_V";
        protected DataTable _table;
        private DataTable _periodTable = null;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Performance/Controls/PerformanceRatingSelectPeriod.ascx";}
		}

		#region Properities
        public string Period 
        {
            get {return GetString(PARAM_PERIOD);}
            set {SetParam(PARAM_PERIOD, value);}
        }
        public string PeriodII
        {
            get {return GetString(PARAM_PERIODII);}
            set {SetParam(PARAM_PERIODII, value);}
        }
        public string ContextSearch
        {
            get {return GetString(PARAM_CONTEXT);}
            set {SetParam(PARAM_CONTEXT, value);}
        }
        public string OrgEntityIDs
        {
            get {return GetString(PARAM_OEIDS);}
            set {SetParam(PARAM_OEIDS, value);}
        }
 		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			DBData db = DBData.getDBData(Session);
            string sql = "select * from " + _TABLE + " where id = -1";

            
			try
			{
				if (!IsPostBack)
				{
					apply.Text = _mapper.get("search");
                    CBShowOpposite.Visible = false;
                    ShowRelation = false;
                }
				db.connect();

				_table = db.getDataTableExt(sql, _TABLE);

                sql = "select * from " + _TABLE +  " order by PERIOD asc";
                _periodTable = db.getDataTable(sql);
                                

                switch (ContextSearch) 
                {
                    case "subnavReportAverageOEPerformance":
                        _table.Columns["PERIOD"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                        _table.Columns["PERIOD"].ExtendedProperties["In"] = _periodTable;
                        _table.Columns["PERIODII"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
                        break;
                    case "subnavReportPerformanceChange":
                        _table.Columns["PERIOD"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                        _table.Columns["PERIOD"].ExtendedProperties["In"] = _periodTable;
                        _table.Columns["PERIODII"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                        _table.Columns["PERIODII"].ExtendedProperties["In"] = _periodTable;
                        break;
                }
             
				base.CheckOrder = true;
                base.View = _TABLE;
				base.LoadInput(db,_table,searchTab);

                DataTable OETable;
                switch (ContextSearch) 
                {
                    default:
                        // Dropdown for OrgEntity
                        TableRow r = new TableRow();
                        searchTab.Rows.Add(r);
                        TableCell c = new TableCell();
                        c.Text = _mapper.get("OEPERSONV", db.langAttrName("OEPERSONV","OE_TITLE"));
                        c.CssClass = "InputMask_Label";
                        r.Cells.Add(c);
                        r.Cells.Add(new TableCell());
                        c = new TableCell();
                        r.Cells.Add(c);
                        DropDownList d = new DropDownCtrl();
                        d.ID = "OE_TITLE";
                        c.Controls.Add(d);
                        if (!IsPostBack)
                        {
                            d.Items.Add("");
                            string OE_IDs = db.Orgentity.addAllSubOEIDs(db.lookup("ORGENTITY_ID", "ORGANISATION", "MAINORGANISATION=1", false));
                            if (OE_IDs != "")
                            {
                                OETable = db.getDataTable("select distinct "+db.langAttrName("ORGENTITY","TITLE")+" from ORGENTITY where ID in (" + OE_IDs + ") order by "+db.langAttrName("ORGENTITY","TITLE"));
                                foreach (DataRow row in OETable.Rows)
                                {
                                    d.Items.Add(row[0].ToString());
                                }
                            }
                        }

                        // Checkbox 'Orgentity recursive'
                        c = new TableCell();
                        c.CssClass = "InputMask_Label";
                        r.Cells.Add(c);
                        CheckBox cb = new CheckBox();
                        cb.Text = _mapper.get("person", "orgEntityRecursive");
                        cb.ID = "OE_RECURSIVE";
                        cb.Checked = true;
                        c.Controls.Add(cb);
                        break;
                }
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


		private void mapControls ()
		{
			apply.Click += new System.EventHandler(apply_Click);
		}

		private void apply_Click(object sender, System.EventArgs e)
		{
            if (checkInputValue(_table, searchTab)) {
                DropDownList d = (DropDownList) searchTab.FindControl("OE_TITLE");
                string selectedOE = "";
                string orgEntityIDs = "";
                if (d != null)
                    selectedOE = d.SelectedItem.Text;

                DBData db = DBData.getDBData(Session);
                db.connect();
                try {
                    if (selectedOE != "") {
                        DataTable oeTable = db.getDataTable("select ID from ORGENTITY where ROOT_ID in (select ORGENTITY_ID from ORGANISATION where MAINORGANISATION=1) and "+db.langAttrName("ORGANISATION","TITLE")+"='" + selectedOE + "'");
                        bool isFirst = true;
                        foreach (DataRow row in oeTable.Rows) {
                            if (isFirst)
                                isFirst = false;
                            else
                                orgEntityIDs += ",";
                            orgEntityIDs += row[0].ToString();
                        }
                        CheckBox cb = (CheckBox) searchTab.FindControl("OE_RECURSIVE");
                        bool searchRecursive = false;
                        if (cb != null) {
                            searchRecursive = cb.Checked;
                        }
                        if (searchRecursive) {
                            orgEntityIDs = db.Orgentity.addAllSubOEIDs(orgEntityIDs);
                        }
                    }

                    string src = base.getSql(_table, searchTab);
                    string match1 = "select distinct * from " + _TABLE;
                    string match2 = " where LOWER";
                    string match3 = "(PERIOD) = ";
                    string match4 = " and LOWER";
                    string match5 = "(PERIODII) = ";

                    switch (ContextSearch) {
                        case "subnavReportAverageOEPerformance":
                            src = src.Replace(match1,"");
                            src = src.Replace(match2,"");
                            src = src.Replace(match3,"");
                            Period = src.Replace("'","");
                            break;
                        case "subnavReportPerformanceChange":
                            src = src.Replace(match1,"");
                            src = src.Replace(match2,"");
                            src = src.Replace(match3,"");
                            src = src.Replace(match4,"");
                            src = src.Replace(match5,"");
                            src = src.Replace("'","");
                            if (src.Length < 8) {
                                src += src;
                            }
                            if (src != "") {
                                Period = src.Substring(0,4);
                                PeriodII = src.Substring(4,4);
                            }
                            else {
                                int cnt = 0;
                                foreach (DataRow row in _periodTable.Rows) {
                                    cnt++;
                                    if (cnt == 1) {
                                        Period = ch.psoft.Util.Validate.GetValid(row["PERIOD"].ToString(), "");
                                    }
                                    PeriodII = ch.psoft.Util.Validate.GetValid(row["PERIODII"].ToString(), "");
                                }
                            }
                            break;
                    }

                    string table = "";
                    string sql = "";

                    switch (ContextSearch) {
                        default:
                            table = "PERFORMANCERATINGOEV";
                            break;
                    }
                
                    if (sql == "") {
                        sql = "select * from " + table;
                    }
                    if (Period != "" || PeriodII != "") {
                        switch (ContextSearch) {
                            case "subnavReportAverageOEPerformance":
                                sql += " where RATING_DATE like '%" + Period + "%'";
                                break;
                            case "subnavReportPerformanceChange":
                                int p = Convert.ToInt16(Period);
                                int pII = Convert.ToInt16(PeriodII);
                                int diff = pII - p;
                                bool lowerFirst = true;
                                if (diff < 0) {
                                    diff = diff * -1;
                                    lowerFirst = false;
                                }
                                sql += " where " + ((diff == 0)? "":"(") + "RATING_DATE like '%" + p + "%'";
                                for (int i=1; i<=diff; i++) {
                                    sql += " or RATING_DATE like '%" + ((lowerFirst)?(p+i):(p-i)) + "%'";
                                }
                                sql += ((diff == 0)? "":")");
                                break;
                        }
                    }

                    sql = sqlAppendWhere(sql, "employment_ref in (" + db.Performance.getRateableEmploymentIDs() + ")");
                
                    if (orgEntityIDs != "")
                        sql = sqlAppendWhere(sql, "OE_ID in (" + orgEntityIDs + ")");

                    sql = sqlAppendWhere(sql, "IS_SELFRATING = 0");

                    OrgEntityIDs = orgEntityIDs;
                    Session["PerformanceRatingSQLSearch"] = sql;
                    Session["PerformanceRatingSQLTable"] = table;
                
                    _searchArgs.SearchSQL = sql;
                }
                catch (Exception ex) {
                    DoOnException(ex);
                }
                finally {
                    db.disconnect();
                }

                DoOnSearchClick(apply);
            }
		}

        private string sqlAppendWhere(string sql, string clause)
        {
            sql += ((sql.ToLower().IndexOf(" where ") > 0) ? " and " : " where ") + clause;
            return sql;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
        }
		#endregion
	}
}
