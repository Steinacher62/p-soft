namespace ch.appl.psoft.SPZ.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for SearchView.
    /// </summary>
    public partial class SearchView : PSOFTSearchUserControl
    {
        protected string _backURL = "";

        private DBData _db = null;
        private DataTable _table;
        string _typFilter = Objective.ALL_TYP_S;


        public static string Path
        {
            get { return Global.Config.baseURL + "/SPZ/Controls/SearchView.ascx"; }
        }
        #region Properities
        /// <summary>
        /// Get/Set back url
        /// </summary>
        public string backURL
        {
            get { return _backURL; }
            set { _backURL = value; }
        }

        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }
        protected override void DoExecute()
        {
            base.DoExecute();
            // load settings on every request / 26.10.09 / mkr
            if (true || !IsPostBack)
            {
                apply.Text = _mapper.get("search");
                semaphore.Text = _mapper.get("mbo", "semaphore");
                rbAlle.Text = _mapper.get("all");
                rbAlle.Checked = true;
                imRed.ToolTip = _mapper.get("mbo", "red");
                imOrange.ToolTip = _mapper.get("mbo", "orange");
                imGreen.ToolTip = _mapper.get("mbo", "green");
                imGray.ToolTip = _mapper.get("mbo", "gray");
                tdGray.Attributes.Add("onclick", rbGray.ClientID + ".checked=true");
                tdGreen.Attributes.Add("onclick", rbGreen.ClientID + ".checked=true");
                tdOrange.Attributes.Add("onclick", rbOrange.ClientID + ".checked=true");
                tdRed.Attributes.Add("onclick", rbRed.ClientID + ".checked=true");
            }
            _db = DBData.getDBData(Session);
            _db.connect();
            try
            {
                loadDetail();
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

        private void loadDetail()
        {
            // load details of tasklist
            detailTab.Rows.Clear();
            _table = _db.getDataTableExt("select * from OBJECTIVEV where ID=-1", "OBJECTIVE");
            _typFilter = _db.Objective.objectiveFilter;
            switch (_typFilter)
            {
                case Objective.UNDEFINED_TYP_S:
                case Objective.ORGANISATION_TYP_S:
                case Objective.ORGENTITY_TYP_S:
                case Objective.PERSON_TYP_S:
                case Objective.PROJECT_TYP_S:
                case Objective.JOB_TYP_S:
                    // exact one typ
                    _table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    break;
                default:
                    ArrayList typList = new ArrayList(_mapper.getEnum("mbo", "typ", true));
                    if (_typFilter != Objective.ALL_TYP_S)
                    {
                        string filter = "," + _typFilter;
                        if (filter.IndexOf("," + Objective.UNDEFINED_TYP) < 0) typList[Objective.UNDEFINED_TYP] = null;
                        if (filter.IndexOf("," + Objective.ORGANISATION_TYP) < 0) typList[Objective.ORGANISATION_TYP] = null;
                        if (filter.IndexOf("," + Objective.ORGENTITY_TYP) < 0) typList[Objective.ORGENTITY_TYP] = null;
                        if (filter.IndexOf("," + Objective.PERSON_TYP) < 0) typList[Objective.PERSON_TYP] = null;
                        if (filter.IndexOf("," + Objective.PROJECT_TYP) < 0) typList[Objective.PROJECT_TYP] = null;
                        if (filter.IndexOf("," + Objective.JOB_TYP) < 0) typList[Objective.JOB_TYP] = null;
                    }
                    _table.Columns["TYP"].ExtendedProperties["InputControlType"] = typeof(BitsetNumberCtrl);
                    _table.Columns["TYP"].ExtendedProperties["In"] = typList;
                    break;
            }
            //_table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(BitsetNumberCtrl);
            //_table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo","state",false));
            _table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            _table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.SEARCH;
            _table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
            _table.Columns["PERSON_ID"].ExtendedProperties["In"] = perstab(_db);
            //_table.Columns["ARGUMENT_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
            //_table.Columns["ARGUMENT_ID"].ExtendedProperties["In"] = _db.Objective.arguments;
            _table.Columns["ARGUMENT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            _table.Columns["CURRENTVALUE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            string ids = _db.Orgentity.addAllSubOEIDs(_db.lookup("ORGENTITY_ID", "ORGANISATION", "MAINORGANISATION=1", false));
            if (ids != "")
            {
                DataTable oeTab = _db.getDataTable("select distinct id," + _db.langAttrName("ORGENTITY", "TITLE") + " from ORGENTITY where ID in (" + ids + ") order by " + _db.langAttrName("ORGENTITY", "TITLE"));
                _table.Columns["ORGENTITY_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["ORGENTITY_ID"].ExtendedProperties["In"] = oeTab;
            }
            else _table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

            base.View = "OBJECTIVE_OE";
            base.LoadInput(_db, _table, detailTab);
        }
        private CheckBox _subNodes = null;
        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r)
        {

            if (col != null)
            {
                BitsetCtrl bs = null;
                switch (col.ColumnName)
                {
                    case "TYP":
                    case "STATE":
                        bs = (BitsetCtrl)r.Cells[2].Controls[0];
                        bs.repeatDirection = RepeatDirection.Horizontal;
                        bs.numberOfGroupItems = 6;
                        bs.columnWidth = 110;
                        break;
                    case "ORGENTITY_ID":
                        _subNodes = new CheckBox();
                        _subNodes.Text = _mapper.get("person", "orgEntityRecursive"); ;
                        r.Cells[2].Controls.Add(_subNodes);
                        break;
                }
            }

        }

        private DataTable perstab(DBData db)
        {
            //check rights
            long accessorID = SessionData.getUserAccessorID(Session);
            string accessorSQL = db.getAccessorIDsSQLInClause(accessorID);
            DataTable tblJobs = db.getDataTableExt("select distinct JOB.ID from JOB inner join ORGENTITY on JOB.ORGENTITY_ID=ORGENTITY.ID inner join ORGANISATION on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID and ORGANISATION.MAINORGANISATION=1 inner join ACCESS_RIGHT_RT on ACCESS_RIGHT_RT.TABLENAME='JOB' and (ACCESS_RIGHT_RT.ROW_ID=JOB.ID or ACCESS_RIGHT_RT.ROW_ID=0) and ACCESS_RIGHT_RT.APPLICATION_RIGHT = 60 and (ACCESS_RIGHT_RT.AUTHORISATION&4)=4 and ACCESS_RIGHT_RT.ACCESSOR_ID  " + accessorSQL, new object[0]);
            string jobsSQL = "IN (";
            bool start = true;
            foreach (DataRow aktJob in tblJobs.Rows)
            {
                if (start == true)
                {
                    start = false;
                }
                else
                {
                    jobsSQL += ", ";
                }
                jobsSQL += aktJob["ID"];
            }
            jobsSQL += ")";

            //list employees
            return db.getDataTableExt("SELECT DISTINCT PersonenID AS PERSON_ID,[Name] + ' ' + Vorname + ', ' + [Bezeichnung Job] AS NAME FROM Rep_Stellenerwartungen WHERE JobID " + jobsSQL + " ORDER BY [Name]", new object[0]);

        }
        protected override void onBuildSQL(StringBuilder build, System.Web.UI.Control control, DataColumn col, object val)
        {
            if (col.ColumnName == "ORGENTITY_ID")
            {
                //string sql = build.ToString();
                //int idx = sql.IndexOf("ORGENTITY_ID");
                //build.Length = 0;
                //build.Append(sql.Substring(0,idx));
                string sql;
                if (!_subNodes.Checked)
                {
                    sql = " (SELECT ID from OEPERSONV WHERE OE_ID = " + val + ")";
                }
                else
                {
                    string ids = _db.Orgentity.addAllSubOEIDs(val.ToString());
                    sql = " (SELECT ID from OEPERSONV WHERE OE_ID IN (" + ids +"))";

                }
                build.Replace("ORGENTITY_ID =", "PERSON_ID IN");
                build.Append(sql);
                
            }


                //SELECT OBJECTIVE.ID
            //FROM   OBJECTIVE INNER JOIN JOBEMPLOYMENTV ON OBJECTIVE.PERSON_ID = dbo.JOBEMPLOYMENTV.PERSON_ID

            else _inputBuilder.dbColumn.AddToSql(build, col, val);
        }
        private void mapControls()
        {
            apply.Click += new System.EventHandler(apply_Click);
        }

        private void apply_Click(object sender, System.EventArgs e)
        {
            if (!base.checkInputValue(_table, detailTab))
                return;

            string sql = base.getSql(_table, detailTab);

            string inStr = "-";
            if (rbRed.Checked)
            {
                inStr = _db.Objective.getObjectiveBySemaphore(Objective.RED);
            }
            else if (rbOrange.Checked)
            {
                inStr = _db.Objective.getObjectiveBySemaphore(Objective.ORANGE);
            }
            else if (rbGreen.Checked)
            {
                inStr = _db.Objective.getObjectiveBySemaphore(Objective.GREEN);
            }
            else if (rbGray.Checked)
            {
                inStr = _db.Objective.getObjectiveBySemaphore(Objective.GRAY);
            }

            if (sql == "") sql = "select * from OBJECTIVE where OBJECTIVE_TURN_ID=" + _db.Objective.turnId;
            else sql = sql.Replace(" where ", " where OBJECTIVE_TURN_ID=" + _db.Objective.turnId + " and ");

            if (inStr != "-")
            {
                if (inStr == "") inStr = "null";
                sql += " and ID in (" + inStr + ")";
            }
            //if (_typFilter != Objective.ALL_TYP_S) sql += " and TYP in ("+_typFilter+")";

            // Setting search event args
            _searchArgs.ReloadList = true;
            _searchArgs.SearchSQL = sql;

            DoOnSearchClick(apply);

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
