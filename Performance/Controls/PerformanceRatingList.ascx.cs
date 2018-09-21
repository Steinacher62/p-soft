using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Collections;
using System.Data;

namespace ch.appl.psoft.Performance.Controls
{


    public partial class PerformanceRatingList : PSOFTSearchListUserControl/*PSOFTListViewUserControl*/ {
        public const string PARAM_CONTEXT = "PARAM_CONTEXT";

        public const string PERSON_ID = "PERSON_ID";

        protected System.Web.UI.WebControls.Label summary;

        public static string Path {
            get {return Global.Config.baseURL + "/Performance/Controls/PerformanceRatingList.ascx";}
        }

        public PerformanceRatingList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = false;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            OrderColumn = "RATING_DATE";
            OrderDir = "asc";
        }

		#region Properties
        public string ContextSearch
        {
            get {return GetString(PARAM_CONTEXT);}
            set {SetParam(PARAM_CONTEXT, value);}
        }

        public long PersonID
        {
            get { return GetLong(PERSON_ID); }
            set { SetParam(PERSON_ID, value); }
        }

		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            CBShowDone.Visible = false;
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();
            if (Visible){
                loadList();
            }
        }

        private void loadList()
        {

            DBData db = DBData.getDBData(Session);
            try 
            {
                db.connect();
                               
                listTab.Rows.Clear();
                DataTable table = getTable(db);

                if (table != null)
                {

                    switch (ContextSearch) 
                    {
                        default:
                            pageTitle.Text = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_CT_PERFORMANCERATING_SEARCH_LIST);
                           
                            table.Columns["RATING_PERSON_REF"].ExtendedProperties["In"] = db.Person.getWholeNameMATable(true);
                            table.Columns["RATING_PERSON_REF"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%RATING_PERSON_REF", "mode","oe");

                            table.Columns["EMPLOYMENT_REF"].ExtendedProperties["In"] = getEmploymentTable(db);
                            table.Columns["EMPLOYMENT_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.LIST;

                            table.Columns["IS_SELFRATING"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("boolean", true));

                            table.Columns["OE_ID"].ExtendedProperties["In"] = getOeTable(db);
                            break;
                        case "subnavSearchPersonWithoutRating":
                            pageTitle.Text = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_CT_PERFORMANCERATING_SEARCH_LIST_PERSONWITHOUTRATING);
                            
                            table.Columns["MNEMO"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            
                            base.View = "PERSONOEV_VIEW";
                            break;
                        case "subnavReportAverageOEPerformance":
                            pageTitle.Text = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_CT_PERFORMANCERATING_SEARCH_LIST);

                            table.Columns["EMPLOYMENT_REF"].ExtendedProperties["In"] = getEmploymentTable(db);
                            table.Columns["EMPLOYMENT_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.LIST;

                            table.Columns["RATING_PERSON_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            
                            table.Columns["IS_SELFRATING"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            
                            table.Columns["OE_ID"].ExtendedProperties["In"] = getOeTable(db);
                            break;
                        case "subnavReportPerformanceChange":
                            table.Columns["RATING_PERSON_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            pageTitle.Text = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_CT_PERFORMANCERATING_SEARCH_LIST);

                            table.Columns["EMPLOYMENT_REF"].ExtendedProperties["In"] = getEmploymentTable(db);
                            table.Columns["EMPLOYMENT_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.LIST;

                            table.Columns["IS_SELFRATING"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            table.Columns["RATING_PERSON_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                            table.Columns["OE_ID"].ExtendedProperties["In"] = getOeTable(db);
                            break;
                        case "subnavHistoryLeaderPersonWithRating":
                            pageTitle.Text = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_CT_PERFORMANCERATING_SEARCH_LIST);

                            table.Columns["EMPLOYMENT_REF"].ExtendedProperties["In"] = getEmploymentTable(db);
                            table.Columns["RATING_PERSON_REF"].ExtendedProperties["In"] = db.Person.getWholeNameMATable(true);
                            table.Columns["RATING_PERSON_REF"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id", "%RATING_PERSON_REF", "mode", "oe");

                            table.Columns["EMPLOYMENT_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.LIST;

                            table.Columns["IS_SELFRATING"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                            table.Columns["OE_ID"].ExtendedProperties["In"] = getOeTable(db);
                            break;
                        case "subnavHistorySelfPersonWithRating":
                            pageTitle.Text = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_CT_PERFORMANCERATING_SELF_SEARCH_LIST);

                            table.Columns["EMPLOYMENT_REF"].ExtendedProperties["In"] = getEmploymentTable(db);
                            table.Columns["RATING_PERSON_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                            table.Columns["EMPLOYMENT_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.LIST;
                            
                            table.Columns["IS_SELFRATING"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            
                            table.Columns["OE_ID"].ExtendedProperties["In"] = getOeTable(db);
                            break;
                    }

                    base.LoadList(db, table, listTab);
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

        private DataTable getEmploymentTable(DBData db){
            return db.getDataTable(EmploymentRating.employmentTableSql(db));
        }

        private DataTable getOeTable(DBData db){
            return db.getDataTableExt("select ID, " + db.langAttrName("ORGENTITY", "TITLE") + " from ORGENTITY where ROOT_ID in (select ORGENTITY_ID from ORGANISATION where MAINORGANISATION=1)","ORGENTITY");
        }

        private DataTable getTable(DBData db)
        {
            string sql = null;
            DataTable table = null;
            sql = Session["PerformanceRatingSQLSearch"] as string;
            String tableName = Session["PerformanceRatingSQLTable"] as string;

            switch (ContextSearch) 
            {
                case "subnavSearchPersonWithoutRating":
                    if (tableName != "PERSONOEV")
                    {
                        sql = null;
                    }
                    break;
                case "subnavHistoryLeaderPersonWithRating":
                    tableName = "PERFORMANCERATINGOEV";
                    sql = "select * from " + tableName + " where PERSON_ID = " + PersonID + " and IS_SELFRATING = 0";
                    break;
                case "subnavHistorySelfPersonWithRating":
                    tableName = "PERFORMANCERATINGOEV";
                    sql = "select * from " + tableName + " where PERSON_ID = " + PersonID + " and IS_SELFRATING = 1";
                    break;
                default:
                    if (tableName != "PERFORMANCERATINGOEV")
                    {
                        sql = null;
                    }
                    break;
            }

         
            if (sql != null)
            {
                sql += " order by " + OrderColumn + " " + OrderDir;
                table = db.getDataTableExt(sql, tableName);
            }
           
            return table;
        }

        public long saveSearchResult() 
        {
            string tableName = "PERFORMANCERATING";
            switch (ContextSearch) 
            {
                case "subnavSearchPersonWithoutRating":
                    tableName = "PERSON";
                    break;
            }
            return ch.psoft.Util.Validate.GetValid(SaveInSearchResult(listTab, tableName).ToString(),(long)-1);            
        }
 

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
		#endregion
    }
}
