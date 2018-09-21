namespace ch.appl.psoft.Report.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using ch.psoft.Util;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PerformanceRatingSearch.
    /// </summary>
    public partial class SearchPersonWithoutPerformanceRatingCtrl : PSOFTSearchUserControl
	{
        public const string PARAM_CONTEXT = "PARAM_CONTEXT";
        public const string PARAM_FROM_DATE = "PARAM_FROM_DATE";
        public const string PARAM_TO_DATE = "PARAM_TO_DATE";

        private const string _TABLE = "PERFORMANCERATINGOEV";
        protected DataTable _table;

		public static string Path
		{
            get { return Global.Config.baseURL + "/Report/Controls/SearchPersonWithoutPerformanceRatingCtrl.ascx"; }
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
                    
                _table.Columns["RATING_PERSON_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["EMPLOYMENT_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["IS_SELFRATING"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                
                _table.Columns["OE_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
             
				base.CheckOrder = true;
                base.View = _TABLE;
				base.LoadInput(db,_table,searchTab);


                DataTable OETable;
                
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
                //if (!IsPostBack)
                //{
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

                    //get and select top OE
                    try
                    {
                        DataTable topOE = db.getDataTableExt("SELECT " + db.langAttrName("ORGENTITY", "TITLE") + " FROM ORGENTITY WHERE PARENT_ID IS NULL", new object[0]);
                        d.SelectedValue = topOE.Rows[0][0].ToString();
                    }
                    catch
                    {
                    }
                //}

                // Checkbox 'Orgentity recursive'
                c = new TableCell();
                c.CssClass = "InputMask_Label";
                r.Cells.Add(c);
                CheckBox cb = new CheckBox();
                cb.Text = _mapper.get("person", "orgEntityRecursive");
                cb.ID = "OE_RECURSIVE";
                cb.Checked = true;
                c.Controls.Add(cb);
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

                    //parse date
                    string sql = base.getSql(_table, searchTab);
                    System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(sql, @"RATING_DATE[ ]*>=[ ]*'[0-9: /APM]+'");
                    if (m.Success)
                    {
                        string mstr = m.ToString();
                        FromDate = GetValid(mstr.Substring(mstr.IndexOf("'") + 1, mstr.Length - mstr.IndexOf("'") - 2), DateTime.Now, true);
                    }
                    else
                    {
                        FromDate = GetValid(null, DateTime.Now, true);
                    }

                    m = System.Text.RegularExpressions.Regex.Match(sql, @"RATING_DATE[ ]*<=[ ]*'[0-9: /APM]+'");
                    if (m.Success)
                    {
                        string mstr = m.ToString();
                        ToDate = GetValid(mstr.Substring(mstr.IndexOf("'") + 1, mstr.Length - mstr.IndexOf("'") - 2), DateTime.Now, true);
                    }
                    else
                    {
                        ToDate = GetValid(null, DateTime.Now, true);
                    }

                    //redirect to report
                    string von = this.FromDate;
                    string bis = this.ToDate;

                    // delete temporary table if exists
                    string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PersonWithoutPerformancerating_%userid%]') "
                                      + "AND type in (N'U')) "
                                      + "DROP TABLE [dbo].[PersonWithoutPerformancerating_%userid%]";
                    db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                    //create and fill temporary table
                    string tbl_create = "CREATE TABLE [dbo].[PersonWithoutPerformancerating_%userid%]("
                    + "[Person_ID] [bigint] NULL,"
                    + "[Name] [varchar](64) NULL,"
                    + "[Firstname] [varchar](64) NULL,"
                    + "[Personnelnumber] [varchar](64) NULL,"
                    + "[OE_ID] [bigint] NULL,"
                    + "[Oe_Title_De] [varchar](128) NULL,"
                    + "[Oe_Title_Fr] [varchar](128) NULL,"
                    + "[Oe_Title_En] [varchar](128) NULL,"
                    + "[Oe_Title_It] [varchar](128) NULL,"
                    + "[Leaving] [datetime] NULL )";

                    db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                    if (Global.isModuleEnabled("spz"))
                    {
                        db.execute("INSERT INTO PersonWithoutPerformancerating_" + db.userId.ToString() + " SELECT * FROM f_PersonWithoutPerformanceratingSPZ('" + von + "','" + bis + "')where oe_id in (" + orgEntityIDs + " )");
                    }
                    else
                    {
                        db.execute("INSERT INTO PersonWithoutPerformancerating_" + db.userId.ToString() + " SELECT * FROM f_PersonWithoutPerformancerating('" + von + "','" + bis + "')where oe_id in (" + orgEntityIDs + " )");
                    }
                    db.disconnect();

                    Response.Redirect("CrystalReportViewer.aspx?alias=PersonWithoutPerformancerating&from=" + GetValid(FromDate, DateTime.Now, false) + "&to=" + GetValid(ToDate, DateTime.Now, false),true);
          
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

        public static string GetValid(string str, DateTime dflt, bool db) 
        {
            //only for displaying
            if (str == null || str.Equals("")) return dflt.ToString("dd'.'MM'.'yyyy");
            else 
            {
                try 
                {
                    if (db)
                    {
                        return DateTime.Parse(str, SQLColumn.DBCulture).ToString("MM'.'dd'.'yyyy");
                    }
                    else
                    {
                        return DateTime.Parse(str,SQLColumn.DBCulture).ToString("dd'.'MM'.'yyyy");
                    }
                    
                }
                catch(Exception ex)
                {
                    Logger.Log(ex, Logger.ERROR);

                    if (db)
                    {
                        return dflt.ToString("MM'.'dd'.'yyyy");
                    }
                    else
                    {
                        return dflt.ToString("dd'.'MM'.'yyyy");
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
