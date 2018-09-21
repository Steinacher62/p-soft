using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.psoft.db;
using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Morph.Controls
{
    public partial class MatrixSearchCtrl : PSOFTSearchUserControl
    {
        // Fields
        private DataTable _table;
        private const string SQL_SEARCH_MATRIX = "matrix";
        private const string SQL_SEARCH_MATRIX_QUERY = "select * from MATRIX where ID=-1";
        private const string SQL_SEARCH_SLAVE = "SLAVE";
        private const string SQL_SEARCH_SLAVE_QUERY = "select * from SLAVE where ID=-1";
        protected Table Table1;

        // Methods
        private void apply_Click(object sender, EventArgs e)
        {
            if (base.checkInputValue(this._table, this.searchTab))
            {
                StringBuilder sql = this.getSql(this._table, this.searchTab, true);
                string str = base.endExtendSql(sql);
                base._searchArgs.ReloadList = true;
                base._searchArgs.SearchSQL = str;
                base.DoOnSearchClick(this.apply);
            }
        }

        protected override void DoExecute()
        {
            base.DoExecute();
            DBData db = DBData.getDBData(base.Session);
            try
            {
                string str;
                if (!base.IsPostBack)
                {
                    this.apply.Text = base._mapper.get("search");
                }
                db.connect();
                if (((str = this.Mode) != null) && (str == "slave"))
                {
                    this._table = db.getDataTableExt("select * from SLAVE where ID=-1", new object[] { "SLAVE" });
                    this._table.Columns["TITLE"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.SEARCH;
                    base.View = "SLAVE";
                }
                else
                {
                    this._table = db.getDataTableExt("select * from MATRIX where ID=-1", new object[] { "MATRIX" });
                    if (Global.isModuleEnabled("novis"))
                    {
                        this._table.Columns["SUBTITLE"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.SEARCH;
                      
                    }
                    base.View = "matrix";
                }
                this.LoadInput(db, this._table, this.searchTab);
            }
            catch (Exception exception)
            {
                base.DoOnException(exception);
            }
            finally
            {
                db.disconnect();
            }
        }

        private void InitializeComponent()
        {
        }

        private void mapControls()
        {
            this.apply.Click += new EventHandler(this.apply_Click);
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

        private string sqlAppendWhere(string sql, string clause)
        {
            sql = sql + ((sql.ToLower().IndexOf(" where ") > 0) ? " and " : " where ") + clause;
            return sql;
        }

        // Properties
        public string Mode
        {
            get
            {
                return base.GetString("PARAM_MODE_SEARCH");
            }
            set
            {
                base.SetParam("PARAM_MODE_SEARCH", value);
            }
        }

        public static string Path
        {
            get
            {
                return (Global.Config.baseURL + "/Morph/Controls/MatrixSearchCtrl.ascx");
            }
        }
    }
}