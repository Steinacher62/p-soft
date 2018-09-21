using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;

namespace ch.appl.psoft.MbO.Controls
{
    public partial class CopySearchView : PSOFTSearchUserControl
    {
        protected DataTable _table;

        public static string Path
        {
            get { return Global.Config.baseURL + "/MbO/Controls/CopySearchView.ascx"; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            string sql = "select * from OBJECTIVE where id = -1";

            try
            {
                if (!IsPostBack)
                {
                    //apply.Text = _mapper.get("search");
                    //title.Text = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_CT_EXPECTATION_COPY);

                    //TODO: get texts from language file
                    apply.Text = "suchen";
                    title.Text = "Ziele suchen";

                    ShowRelation = false;
                }

                db.connect();

                _table = db.getDataTableExt(sql, "OBJECTIVE");

                _table.Columns["STARTDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["TITLE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["NUMBER"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                //DataTable personTab = db.Person.getWholeNameMATable(false);
                _table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.SEARCH;
                _table.Columns["PERSON_ID"].ExtendedProperties["In"] = perstab(db);
                _table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                //TODO: get column from language mapper
                string sqlTurns = "SElECT ID, TITLE_DE FROM OBJECTIVE_TURN ORDER BY ID DESC";
                DataTable turnTable = db.getDataTable(sqlTurns);
                _table.Columns["OBJECTIVE_TURN_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.SEARCH;
                _table.Columns["OBJECTIVE_TURN_ID"].ExtendedProperties["In"] = turnTable;
                _table.Columns["OBJECTIVE_TURN_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                CheckOrder = true;
                LoadInput(db, _table, searchTab);
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

        private void mapControls()
        {
            apply.Click += new System.EventHandler(apply_Click);
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

        private void apply_Click(object sender, System.EventArgs e)
        {
            if (!checkInputValue(_table, searchTab))
                return;

            _searchArgs.SearchSQL = getSql(_table, searchTab);
            _searchArgs.ReloadList = true;

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