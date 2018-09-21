using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Performance.Controls;
using System;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for SearchFrame.
    /// </summary>
    public partial class Search : PsoftSearchPage
	{
		private PerformanceRatingList _list;
        private PerformanceRatingSearch _search;
        protected PsoftPageLayout PsoftPageLayout;

        // Query string variables
        private string _context = "";

		#region Protected overrided methods from parent class
		protected override void Initialize()
		{
			// base initialize
			base.Initialize();

            // Retrieving query string values
            _context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"], _context);
        }
		#endregion

		/*public Search() : base("get") {}*/

        protected void Page_Load(object sender, System.EventArgs e)
		{

            DBData db = DBData.getDBData(Session);

			// Setting main page layout
            /*PsoftPageLayout*/ PsoftPageLayout = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
			PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
			PageLayoutControl.ContentLayoutControl = (SearchContentLayout)this.LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

			_search = (PerformanceRatingSearch)this.LoadPSOFTControl(PerformanceRatingSearch.Path, "_search");
			_search.OnSearchClick +=new SearchClickHandler(_search_OnSearchClick);
            _search.ContextSearch = _context;
			SetPageLayoutContentControl(SearchContentLayout.SEARCH, _search);

			_list = (PerformanceRatingList)this.LoadPSOFTControl(PerformanceRatingList.Path, "_list");
			_list.Visible = false;
            _list.DetailEnabled = true;
            _list.OrderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["OrderDir"], _list.OrderDir);
            _list.SortURL = Global.Config.baseURL + "/Performance/Search.aspx?context=" + _context;
            _list.ContextSearch = _search.ContextSearch;
            SetPageLayoutContentControl(SearchContentLayout.LIST, _list);

            switch (_context) 
            {
                default:
                    BreadcrumbCaption = PsoftPageLayout.PageTitle = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_BC_SEARCH_PERFORMANCERATING);
                    BreadcrumbName = "PerformanceRatingSearch";
                    _list.DetailURL = Global.Config.baseURL + "/Performance/EmploymentRating.aspx?performanceRatingID=%ID&type=%IS_SELFRATING";
                    _list.OrderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["OrderColumn"], _list.OrderColumn);
                    break;
                case "subnavSearchPersonWithoutRating":
                    BreadcrumbCaption = PsoftPageLayout.PageTitle = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_BC_SEARCH_PERFORMANCERATING_PERSONWITHOUTRATING);
                    BreadcrumbName = "PersonWithoutRatingSearch";
                    _list.DetailURL = psoft.Person.DetailFrame.GetURL("ID","%ID", "mode","oe");
                    _list.OrderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["OrderColumn"], "PNAME");
                    break;
            }

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
		    _list.Visible = true;
            _list.Execute();

            long searchResultID = _list.saveSearchResult();
            string alias = "PerformanceratingListSearchResult";
            string from;
            string to;
            switch (_context) 
            {
                default:
                    PsoftPageLayout.ButtonListAttributes.Add("onClick", "javascript: window.open('" + psoft.Goto.GetURL("alias",alias) + "&param0=" + searchResultID + "');");
                    PsoftPageLayout.ButtonListVisible = true;
                    break;
                case "subnavSearchPersonWithoutRating":
                    alias = "PersonWithoutPerformanceratingListSearchResult";
                    from = _search.FromDate.ToString();
                    to = _search.ToDate.ToString();
                    PsoftPageLayout.ButtonListAttributes.Add("onClick", "javascript: window.open('" + psoft.Goto.GetURL("alias",alias) + "&param0=" + searchResultID + "&param1=" + from + "&param2=" + to +"');");
                    PsoftPageLayout.ButtonListVisible = true;
                    break;
                case "subnavReportAverageOEPerformance":
                    from = _search.FromDate.ToString();
                    to = _search.ToDate.ToString();
                    alias = "PrintAverageOEPerformance.aspx?personID=" + DBData.getDBData(Session).userId.ToString() + "&period=" + from + "&orgentityIDs=" + "_search.OrgEntityIDs";
                    break;
            }
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50);
		}

        private void nextClick(object Sender, NextEventArgs e) {
            this.Response.Redirect(e.LoadUrl, true);
        }

	}
}
