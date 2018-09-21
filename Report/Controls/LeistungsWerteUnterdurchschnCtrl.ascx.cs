namespace ch.appl.psoft.Report.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;
    using Telerik.Web.UI;


    public partial class LeistungswerteUnterdurchschnCtrl : PSOFTSearchUserControl
    {

        private DBData _db = null;
        private DataTable _table = null;
        private static string _LBFrom;
        private static string _LBTo;
        private static string _dataRange;


        public static string Path
        {
            get { return Global.Config.baseURL + "/Report/Controls/LeistungswerteUnterdurchschnCtrl.ascx"; }
        }
        public string LBFrom
        {
            get {return (_LBFrom);} 
        }
        public string LBTo
        {
            get {return (_LBTo); }
        }

        public string DataRange
        {
            get { return (_dataRange); }
        }
        #region Properities

        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }
        protected override void DoExecute()
        {
            base.DoExecute();

            if (!IsPostBack)
            {
                apply.Text = _mapper.get("search");
            }
            if (Visible) loadDetail();
        }
        private void loadDetail()
        {
            _db = DBData.getDBData(Session);
            _db.connect();
            try
            {
                // load details of tasklist
                detailTab.Rows.Clear();
                _table = _db.getDataTableExt("select ID, RATING_DATE from Performancerating", "PERFORMANCERATING");
                if (_table.Columns.Contains("ID") && !Global.isModuleEnabled("performance")) _table.Columns["ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                base.LoadInput(_db, _table, detailTab);


                // Inputlabel Datarange % 

                if (System.Text.RegularExpressions.Regex.IsMatch(Request.AppRelativeCurrentExecutionFilePath, "LeistungswerteNegVeraenderung")== true)
                {

                    TableRow r = new TableRow();
                    TableCell c = new TableCell();
                    detailTab.Rows.Add(r);
                    c.Text = _mapper.get("LBNegVeraenderung", "datarange");
                    c.CssClass = "InputMask_Label";
                    r.Cells.Add(c);

                    TableCell ce = new TableCell();
                    r.Cells.Add(ce);
                    ce.Text = "%";

                    TableCell c1 = new TableCell();
                    c1.CssClass = "InputMask_Label";
                    r.Cells.Add(c1);
                    RadNumericTextBox tb = new RadNumericTextBox();
                    tb.Type = NumericType.Percent;
                    tb.ID = "DATARANGE";
                    c1.Controls.Add(tb);
                }
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
        private void mapControls()
        {
            apply.Click += new System.EventHandler(apply_Click);
        }

        private void apply_Click(object sender, System.EventArgs e)
        {
            if (!base.checkInputValue(_table, detailTab))
                return;

            string sql = base.getSql(_table, detailTab);

            // Setting search event args
            _searchArgs.ReloadList = true;
            _searchArgs.SearchSQL = sql;

            System.Web.UI.Control ctrl;
            TextBox tb = new TextBox();
            foreach (TableRow r in detailTab.Rows)
            {
                TableCell prevCell = null;
                foreach (TableCell cell in r.Cells)
                {


                    if (cell.HasControls())
                    {

                        if (cell.Controls[0].ID == "DATARANGE")
                        {
                            ctrl = cell.Controls[0];
                            _dataRange = (ctrl as RadNumericTextBox).Text;
                            
                        }
                        else
                        {
                            if (cell.Controls[0].Controls[0].Controls[0].Controls[0].ID == "Input-PERFORMANCERATING-RATING_DATE-From")
                            {
                                ctrl = cell.Controls[0].Controls[0].Controls[0].Controls[0];
                                _LBFrom = (ctrl as TextBox).Text;
                            }
                            else if (cell.Controls[0].Controls[0].Controls[0].Controls[0].ID == "Input-PERFORMANCERATING-RATING_DATE-To")
                            {
                                ctrl = cell.Controls[0].Controls[0].Controls[0].Controls[0];
                                _LBTo = (ctrl as TextBox).Text;
                            }
                        }
                    }
                }
            }          
            
            {
                DoOnSearchClick(apply);
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
