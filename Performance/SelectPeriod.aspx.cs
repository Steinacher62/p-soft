using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Performance.Controls;
using System;


namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for SelectPeriod.
    /// </summary>
    public partial class SelectPeriod : PsoftSearchPage
	{

        // Query string variables
        private string _context = "";
        private PerformanceRatingSelectPeriod _search = null;
        private PerformanceRatingList _list = null;
        private PsoftPageLayout _PsoftPageLayout = null;

		#region Protected overrided methods from parent class
		protected override void Initialize()
		{
			// base initialize
			base.Initialize();

            // Retrieving query string values
            _context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"], _context);
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e)
		{

            DBData db = DBData.getDBData(Session);

			// Setting main page layout
            /*PsoftPageLayout*/ _PsoftPageLayout = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
			PageLayoutControl = _PsoftPageLayout;

            switch (_context) 
            {
                case "subnavReportAverageOEPerformance":
                    BreadcrumbCaption = _PsoftPageLayout.PageTitle = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_BC_REPORT_AVERAGEPERFORMANCE);
                    BreadcrumbName = "ReportAverageOEPerformance";
                    break;
                case "subnavReportPerformanceChange":
                    BreadcrumbCaption = _PsoftPageLayout.PageTitle = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_BC_REPORT_PERFORMANCECHANGE);
                    BreadcrumbName = "ReportPerformanceChange";
                    break;
            }

			// Setting content layout of page layout
			PageLayoutControl.ContentLayoutControl = (SearchContentLayout)this.LoadPSOFTControl(SearchContentLayout.Path, "_sC");

			/*PerformanceRatingSelectPeriod*/ _search = (PerformanceRatingSelectPeriod)this.LoadPSOFTControl(PerformanceRatingSelectPeriod.Path, "_search");
			_search.OnSearchClick +=new SearchClickHandler(_search_OnSearchClick);
            _search.ContextSearch = _context;
			SetPageLayoutContentControl(SearchContentLayout.SEARCH, _search);

			_list = (PerformanceRatingList)this.LoadPSOFTControl(PerformanceRatingList.Path, "_list");
			_list.Visible = false;
            _list.DetailEnabled = true;
            _list.DetailURL = Global.Config.baseURL + "/Performance/EmploymentRating.aspx?performanceRatingID=%ID&type=leader";
            _list.OrderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["OrderColumn"], _list.OrderColumn);
            _list.OrderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["OrderDir"], _list.OrderDir);
            _list.SortURL = Global.Config.baseURL + "/Performance/SelectPeriod.aspx?context=" + _context;
            _list.ContextSearch = _search.ContextSearch;
            SetPageLayoutContentControl(SearchContentLayout.LIST, _list);

            if (ch.psoft.Util.Validate.GetValid(Request.QueryString["OrderColumn"], "") != ""){
                _list.Visible = true;
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
//            mapControls();
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

        }
		#endregion

		private void _search_OnSearchClick(object Sender, SearchEventArgs e)
		{
            if (_search.Period != null)
            {
                string target = "";
                switch (_context) 
                {
                    case "subnavReportAverageOEPerformance":
                        target = "PrintAverageOEPerformance.aspx?personID=" + DBData.getDBData(Session).userId.ToString() + "&period=" + _search.Period.ToString() + "&orgentityIDs=" + _search.OrgEntityIDs;
                        break;
                    case "subnavReportPerformanceChange":
                        target = "PrintPerformanceChange.aspx?personID=" + DBData.getDBData(Session).userId.ToString() + "&period=" + _search.Period.ToString()+ "&periodII=" + _search.PeriodII.ToString() + "&orgentityIDs=" + _search.OrgEntityIDs;
                        break;
                }
                target = "javascript: window.open('" + target + "');";
                _PsoftPageLayout.ButtonPrintAttributes.Add("onClick", target);
                _PsoftPageLayout.ButtonPrintVisible = true;

            }
		    _list.Visible = true;
            _list.Execute();
		}

        private void nextClick(object Sender, NextEventArgs e) {
            this.Response.Redirect(e.LoadUrl, true);
        }

	}
}
