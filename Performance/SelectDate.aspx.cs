using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Performance.Controls;
using System;


namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for SelectDate.
    /// </summary>
    public partial class SelectDate : PsoftContentPage
	{

        // Query string variables
        private string _context = "";
        private int _employmentID = -1;
        private GlobalPerformanceSelectDate _search = null;
        private PsoftPageLayout _PsoftPageLayout = null;

        public SelectDate() : base()
        {
            UseWebService = false;
            ShowProgressBar = false;
        }

		#region Protected overrided methods from parent class
		protected override void Initialize()
		{
			// base initialize
			base.Initialize();

            // Retrieving query string values
            _context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"], _context);
            _employmentID = ch.psoft.Util.Validate.GetValid(Request.QueryString["employmentID"], _employmentID);
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
                default:
                    this.BreadcrumbCaption = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_BC_REPORT_GLOBALPERFORMANCE_SELECTDATE);
                    _PsoftPageLayout.PageTitle = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_BC_REPORT_GLOBALPERFORMANCE_SELECTDATE);
                    this.BreadcrumbName = "GlobalPerformanceSelectDate";
                    break;
            }

			// Setting content layout of page layout
			PageLayoutControl.ContentLayoutControl = (SearchContentLayout)this.LoadPSOFTControl(SearchContentLayout.Path, "_sC");

			/*GlobalPerformanceSelectDate*/ _search = (GlobalPerformanceSelectDate)this.LoadPSOFTControl(GlobalPerformanceSelectDate.Path, "_search");
            _search.ContextSearch = _context;
            _search.EmploymentID = _employmentID;
			SetPageLayoutContentControl(SearchContentLayout.SEARCH, _search);
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


        private void nextClick(object Sender, NextEventArgs e) {
            this.Response.Redirect(e.LoadUrl, true);
        }

	}
}
