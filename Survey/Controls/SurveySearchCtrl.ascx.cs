namespace ch.appl.psoft.Survey.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;
    using System.Text;

    public partial class SurveySearchCtrl : PSOFTSearchUserControl {

        private DataTable _table;

        protected System.Web.UI.WebControls.Table Table1;
       

        public static string Path {
            get {return Global.Config.baseURL + "/Survey/Controls/SurveySearchCtrl.ascx";}
        }

		#region Properities
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();
            DBData db = DBData.getDBData(Session);

            try {
                if (!IsPostBack) {
                    apply.Text = _mapper.get("search");
                }

                db.connect();
                _table = db.getDataTableExt("select * from SURVEY where ID=-1", "SURVEY");

                LoadInput(db, _table, searchTab);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        }

        private void mapControls () {
            apply.Click += new System.EventHandler(apply_Click);
        }

        private string sqlAppendWhere(string sql, string clause) {
            sql += ((sql.ToLower().IndexOf(" where ") > 0) ? " and " : " where ") + clause;
            return sql;
        }

        private void apply_Click(object sender, System.EventArgs e) {
            if (checkInputValue(_table, searchTab)){
                StringBuilder sqlEx = getSql(_table, searchTab, true);
                extendSql(sqlEx, _table, "ISPUBLIC", 1);
                string sql = endExtendSql(sqlEx);
                
                // Setting search event args
                _searchArgs.ReloadList = true;
                _searchArgs.SearchSQL = sql;

                DoOnSearchClick(apply);
            }
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            mapControls();
        }
		
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {    
            ID = "Search";

        }
		#endregion
    }
}