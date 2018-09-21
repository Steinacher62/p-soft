namespace ch.appl.psoft.Performance.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using ch.psoft.Util;
    using Common;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PerformanceRatingSearch.
    /// </summary>
    public partial class PerformanceRatingSearch : PSOFTSearchUserControl
	{
        public const string PARAM_CONTEXT = "PARAM_CONTEXT";
        public const string PARAM_FROM_DATE = "PARAM_FROM_DATE";
        public const string PARAM_TO_DATE = "PARAM_TO_DATE";

        private const string _TABLE = "PERFORMANCERATINGOEV";
        protected DataTable _table;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Performance/Controls/PerformanceRatingSearch.ascx";}
		}

		#region Properities
        public string ContextSearch
        {
            get {return GetString(PARAM_CONTEXT);}
            set {SetParam(PARAM_CONTEXT, value);}
        }
        public String FromDate 
        {
            get {return GetString(PARAM_FROM_DATE);}
            set {SetParam(PARAM_FROM_DATE, value);}
        }
        public String ToDate 
        {
            get {return GetString(PARAM_TO_DATE);}
            set {SetParam(PARAM_TO_DATE, value);}
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

                switch (ContextSearch) 
                {
                    default:
                        DataTable personTable = db.Person.getWholeNameMATable(false);
                        _table.Columns["RATING_PERSON_REF"].ExtendedProperties["In"] = personTable;
                        _table.Columns["RATING_PERSON_REF"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                        DataTable employmentTable = db.getDataTable(EmploymentRating.employmentTableSql(db));
                        _table.Columns["EMPLOYMENT_REF"].ExtendedProperties["In"] = employmentTable;
                        _table.Columns["EMPLOYMENT_REF"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                        ArrayList boolList = new ArrayList(_mapper.getEnum("boolean", true));
                        _table.Columns["IS_SELFRATING"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                        _table.Columns["IS_SELFRATING"].ExtendedProperties["In"] = boolList;

                        _table.Columns["OE_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                        break;
                    case "subnavSearchPersonWithoutRating":
                        _table.Columns["RATING_PERSON_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                        _table.Columns["EMPLOYMENT_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                        _table.Columns["IS_SELFRATING"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                        
                        _table.Columns["OE_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
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

                    string sql = base.getSql(_table, searchTab);
                    string table = _TABLE;

                    switch (ContextSearch) {
                        case "subnavSearchPersonWithoutRating":
                            //SEEKIII-254
                            table = "PERSONOEV";
                            if (sql == "")
                            {
                                //should never be reached
                                sql = "select distinct e.person_id from employment e, job j where e.id not in " +
                                      "(select employment_ref from performancerating where job_id is not null) " +
                                      " and e.id = j.employment_id";
                                if (orgEntityIDs != "")
                                    sql = sqlAppendWhere(sql, "e.orgentity_id" + " in (" + orgEntityIDs + ")");
                                sql = sqlAppendWhere(sql, "j.ID in (" + db.Performance.getRateableJobIDs() + ")");
                            }
                            else
                            {
                                string newsql = "select distinct e.person_id from employment e, job j where e.id not in " +
                                                     "(%PERFCONDITION) " +
                                                     "and e.id = j.employment_id";
                                
                                string performanceCondition = "select employment_ref from performancerating";

                                //parse date
                                System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(sql, @"RATING_DATE[ ]*>=[ ]*'[0-9: /APM]+'");
                                if (m.Success)
                                {
                                    string mstr = m.ToString();
                                    FromDate = GetValid(mstr.Substring(mstr.IndexOf("'") + 1, mstr.Length - mstr.IndexOf("'") - 2), DateTime.Now);
                                    performanceCondition = sqlAppendWhere(performanceCondition, mstr);
                                }
                                else
                                {
                                    FromDate = GetValid(null, DateTime.Now);
                                }

                                m = System.Text.RegularExpressions.Regex.Match(sql, @"RATING_DATE[ ]*<=[ ]*'[0-9: /APM]+'");
                                if (m.Success)
                                {
                                    string mstr = m.ToString();
                                    ToDate = GetValid(mstr.Substring(mstr.IndexOf("'") + 1, mstr.Length - mstr.IndexOf("'") - 2), DateTime.Now);
                                    performanceCondition = sqlAppendWhere(performanceCondition, mstr);
                                }
                                else
                                {
                                    ToDate = GetValid(null, DateTime.Now);
                                }
                                // performance for employer with a new job is not considered
                                performanceCondition = sqlAppendWhere(performanceCondition, "job_id is not null");

                                sql = newsql.Replace("%PERFCONDITION", performanceCondition);

                                if (orgEntityIDs != "")
                                {
                                    sql = sqlAppendWhere(sql, "e.orgentity_id" + " in (" + orgEntityIDs + ")");
                                }
                                sql = sqlAppendWhere(sql, "j.ID in (" + db.Performance.getRateableJobIDs() + ")");
                            }

                            if (Global.isModuleEnabled("spz"))
                            {
                                sql = "select ID, pname, firstname, mnemo from person where id in (" + sql + ") and pfs = 'ja'";
                            }
                            else
                            {
                                sql = "select ID, pname, firstname, mnemo from person where id in (" + sql + ")";
                            }

                            break;
                         default:
                            //filter out "self" of others and jobs with other leaders and add "self" of the current logged-in person
                            if (sql == "") {
                                sql = "select distinct * from " + table;
                            }
                            if (orgEntityIDs != "")
                            {
                                sql = sqlAppendWhere(sql, "oe_id in (" + orgEntityIDs + ")");
                            }

                            // constraint (TODO!!! for admin this does not work!)
                            string departmentConstraint = "";// "and oe_id in (select distinct orgentity_id from employment where person_id = " + db.userId + ")";

                            //string sqlr = "select distinct a.AUTHORISATION from ACCESS_RIGHT_RT a where a.TABLENAME = 'JOB' and a.ROW_ID = " + "job_id" 
                            //             + " and a.APPLICATION_RIGHT=11 and a.ACCESSOR_ID ";
                            //sqlr += db.getAccessorIDsSQLInClause(db.userAccessorID);                                                    
                            
                            sql = sqlAppendWhere(sql, "(employment_ref in (" 
                                  + db.Performance.getRateableEmploymentIDs() + ")"
                                  + "and is_selfrating = 0 "
                                  + departmentConstraint
                                  //+ sqlr
                                  + " or employment_ref in (select distinct ID from EMPLOYMENT where PERSON_ID = " + db.userId + ")"// and ORGENTITY_ID is not null)"
                                  + ")"
                                  );
                            
                            break;
                   }
                    
          
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

        public static string GetValid(string str, DateTime dflt) 
        {
            //only for displaying
            if (str == null || str.Equals("")) return dflt.ToString("dd'.'MM'.'yyyy");
            else 
            {
                try 
                {
                    return DateTime.Parse(str,SQLColumn.DBCulture).ToString("dd'.'MM'.'yyyy");
                }
                catch(Exception ex)
                {
                    Logger.Log(ex, Logger.ERROR);
                    return dflt.ToString("dd'.'MM'.'yyyy");
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
