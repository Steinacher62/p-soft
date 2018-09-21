using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Report.Controls
{
    /// <summary>
    /// Summary description for ReportLayoutSelect
    /// </summary>
    public partial class ReportLayoutSelectCtrl : PSOFTListViewUserControl
	{
        protected DBData _db = null;

        public ReportLayoutSelectCtrl() : base()
        {
            DetailEnabled = true;
            DeleteEnabled = false;
            EditEnabled = false;
            HeaderEnabled = true;
            InfoBoxEnabled = true;
        }

        #region Properties
        public static string Path
        {
            get {return Global.Config.baseURL + "/Report/Controls/ReportLayoutSelectCtrl.ascx";}
        }

        protected string _backURL = "";
        public string BackURL
        {
            set { _backURL = value;}
            get { return _backURL;}
        }
        
        protected ReportModule.Target _target = ReportModule.Target.Undefined;
        public ReportModule.Target Target
        {
            set { _target = value;}
            get { return _target;}
        }

        protected string _types = ((int) ReportModule.ReportType.Undefined).ToString();
        public string Types
        {
            set { _types = value;}
            get { return _types;}
        }

        protected string _titleMnemo = "selectTitle";
        public string TitleMnemo
        {
            set { _titleMnemo = value;}
            get { return _titleMnemo;}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();

            _db = DBData.getDBData(Session);
            OrderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], "TITLE_MNEMO");
            OrderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], "asc");

            if (!IsPostBack)
            {
                Title.Text = _mapper.get("reportLayout", _titleMnemo);
            }

            loadList();
        }

        private void loadList()
        {
            listTab.Controls.Clear();

            try
            {
                _db.connect();
                string sql = "select * from REPORTLAYOUT";
                bool isFirst = true;
                if (_target != ReportModule.Target.Undefined)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        sql += " where";
                    }
                    sql += " TARGET=" + (int) _target;
                }
                if (!_types.Equals(((int) ReportModule.ReportType.Undefined).ToString()))
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        sql += " where";
                    }
                    else
                        sql += " and";
                    sql += " TYPE in (" + _types +")";
                }
                
                sql += " order by " + OrderColumn + " " + OrderDir;

                DataTable table = _db.getDataTableExt(sql, "REPORTLAYOUT");
                IDColumn = "ID";
                LoadList(_db, table, listTab);
            }
            finally
            {
                _db.disconnect();
            }
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell)
        {
            if (col != null)
            {
                switch(col.ColumnName)
                {
                    case "TITLE_MNEMO":
                        if (cell.Controls.Count > 0)
                        {
                            HyperLink l = cell.Controls[0] as HyperLink;
                            if (l != null)
                                l.Text = _mapper.get("reportLayout", l.Text);
                        }
                        break;
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
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

        }
		#endregion
	}
}
