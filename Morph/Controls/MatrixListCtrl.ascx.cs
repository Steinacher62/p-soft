using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.psoft.db;
using System;
using System.Data;
namespace ch.appl.psoft.Morph.Controls
{
    public partial class MatrixListCtrl : PSOFTSearchListUserControl
    {
        // Fields
        protected DBData _db;
        public const string PARAM_BACK_URL = "PARAM_BACK_URL";
        public const string PARAM_MODE = "PARAM_MODE";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_QUERY = "PARAM_QUERY";
        public const string PARAM_RELOAD = "PARAM_RELOAD";
        public const string PARAM_SQL = "PARAM_SQL";
        public const string PARAM_SURVEY_ID = "PARAM_SURVEY_ID";
        public const string PARAM_X_ID = "PARAM_X_ID";

        // Methods
        protected override void DoExecute()
        {
            base.DoExecute();
            this.listTab.Rows.Clear();
            this._db = DBData.getDBData(base.Session);
            try
            {
                string str2;
                this._db.connect();
                if (this.NextURL != "")
                {
                    this.next.Visible = true;
                    this.next.Text = base._mapper.get("next");
                    base.CheckBoxEnabled = true;
                }
                string selectStatement = "";
                string likeStatement = "";
                if (this.Query != "")
                {
                    selectStatement = this.Query;
                    int first = selectStatement.IndexOf("LOWER");
                    int last = selectStatement.Length;
                    likeStatement = selectStatement.Substring(first, last - first);


                }
                DataTable dataTable = null;
                if (((str2 = this.Mode) != null) && (str2 == "slave"))
                {
                    this.pageTitle.Text = base._mapper.get("morph", "bcSlaveList");
                    if (selectStatement == "")
                    {
                        selectStatement = "select * from SLAVE";
                        string str3 = selectStatement;
                        selectStatement = str3 + " order by " + base.OrderColumn + " " + base.OrderDir;
                    }
                    base.View = "SLAVE";
                    dataTable = this._db.getDataTableExt(selectStatement, new object[] { "SLAVE" });
                }
                else
                {
                    this.pageTitle.Text = base._mapper.get("morph", "bcMatrixList");
                    if (Page.IsPostBack)
                    {
                        if (!Global.isModuleEnabled("gfk"))
                        {
                            if (likeStatement == "")
                                dataTable = _db.getDataTableExt("SELECT DISTINCT ID, TITLE, DESCRIPTION FROM MATRIX " + _db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " ORDER BY TITLE ASC", new object[] { "MATRIX" });
                            else
                                dataTable = _db.getDataTableExt("SELECT DISTINCT ID, TITLE, DESCRIPTION FROM MATRIX " + _db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " AND " + likeStatement + " ORDER BY TITLE ASC", new object[] { "MATRIX" });
                        }
                        else
                        {
                            string sql = "";
                            string sql1 = "";
                            if (likeStatement == "")
                            {
                                sql = "SELECT DISTINCT ID, TITLE, DESCRIPTION FROM MATRIX " + _db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " WHERE IS_GFK_TEMPLATE = 0 ORDER BY TITLE DESC";
                                sql1 = "SELECT DISTINCT * FROM MATRIX " + _db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.ADMIN, DBData.APPLICATION_RIGHT.COMMON, true, true) + " WHERE IS_GFK_TEMPLATE = 1 ORDER BY TITLE DESC";
                            }
                            else
                            {
                                sql = "SELECT DISTINCT ID, TITLE, DESCRIPTION FROM MATRIX " + _db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " WHERE IS_GFK_TEMPLATE = 0 AND " + likeStatement + " ORDER BY TITLE DESC";
                                sql1 = "SELECT DISTINCT ID, TITLE, DESCRIPTION FROM MATRIX " + _db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.ADMIN, DBData.APPLICATION_RIGHT.COMMON, true, true) + " WHERE IS_GFK_TEMPLATE = 1 AND " + likeStatement + " ORDER BY TITLE DESC";
                            }
                            dataTable = _db.getDataTableExt(sql, new object[] { "MATRIX" });
                            DataTable submatrix1 = _db.getDataTableExt(sql1, new object[] { "MATRIX" });
                            if (submatrix1.Rows.Count > 0 && dataTable.Rows.Count > 0)
                            {
                                dataTable.Merge(submatrix1);
                            }
                            //dataTable.DefaultView.Sort = "TITLE";
                        }

                        base.View = "matrix";
                        base.IDColumn = "ID";
                        dataTable.Columns["TITLE"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.LIST;
                        dataTable.Columns["DESCRIPTION"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.LIST;
                        this.LoadList(this._db, dataTable, this.listTab);
                    }
                }
            }
            catch (Exception exception)
            {
                base.DoOnException(exception);
            }
            finally
            {
                this._db.disconnect();
            }
        }

        private void InitializeComponent()
        {
        }

        private void mapControls()
        {
            this.next.Click += new EventHandler(this.next_Click);
        }

        private void next_Click(object sender, EventArgs e)
        {
            string str;
            long num = -1L;
            if (((str = this.Mode) != null) && (str == "slave"))
            {
                num = base.SaveInSearchResult(this.listTab, "SLAVE");
            }
            else
            {
                num = base.SaveInSearchResult(this.listTab, "MATRIX");
            }
            this.NextURL = this.NextURL.Replace("%25SearchResultID", "%SearchResultID").Replace("%SearchResultID", num.ToString());
            base._nextArgs.LoadUrl = this.NextURL;
            base.DoOnNextClick(this.next);
        }

        protected override void OnInit(EventArgs e)
        {
            this.mapControls();
            this.InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Execute();
        }

        // Properties
        public string BackURL
        {
            get
            {
                return base.GetString("PARAM_BACK_URL");
            }
            set
            {
                base.SetParam("PARAM_BACK_URL", value);
            }
        }

        public string Mode
        {
            get
            {
                return base.GetString("PARAM_MODE");
            }
            set
            {
                base.SetParam("PARAM_MODE", value);
            }
        }

        public string NextURL
        {
            get
            {
                return base.GetString("PARAM_NEXT_URL");
            }
            set
            {
                base.SetParam("PARAM_NEXT_URL", value);
            }
        }

        public static string Path
        {
            get
            {
                return (Global.Config.baseURL + "/Morph/Controls/MatrixListCtrl.ascx");
            }
        }

        public string Query
        {
            get
            {
                return base.GetString("PARAM_QUERY");
            }
            set
            {
                base.SetParam("PARAM_QUERY", value);
            }
        }

        public bool Reload
        {
            get
            {
                return base.GetBool("PARAM_RELOAD");
            }
            set
            {
                base.SetParam("PARAM_RELOAD", value);
            }
        }

        public string SearchSQL
        {
            get
            {
                return base.GetString("PARAM_SQL");
            }
            set
            {
                base.SetParam("PARAM_SQL", value);
            }
        }

        public long SurveyID
        {
            get
            {
                return base.GetLong("PARAM_SURVEY_ID");
            }
            set
            {
                base.SetParam("PARAM_SURVEY_ID", value);
            }
        }

        public long xID
        {
            get
            {
                return base.GetLong("PARAM_X_ID");
            }
            set
            {
                base.SetParam("PARAM_X_ID", value);
            }
        }
    }
}