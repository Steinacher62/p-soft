using ch.appl.psoft.Common;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Person.Controls;
using System;
using System.Web;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Person
{
    /// <summary>
    /// Summary description for SearchFrame.
    /// </summary>
    /// <param name="singleResultRecord">true: radio-buttons / false: check-boxes</param>
    /// <param name="backURL">URL to go redirect afterwards</param>
    /// <param name="backTarget">Target for backURL</param>
    /// </param>
    public partial class SearchFrame : PsoftSearchPage {
        private const string PAGE_URL = "/Person/SearchFrame.aspx";

        static SearchFrame(){
            SetPageParams(PAGE_URL, "singleResultRecord", "backURL", "backTarget", "orderColumn", "orderDir", "searchResultID");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public SearchFrame() : base("get") {
            PageURL = PAGE_URL;
            this.PreInit += new EventHandler(SearchFrame_PreInit);
        }

        private string _backURL = "";
		private string _backTarget = "_self";
		private string _orderColumn;
		private string _orderDir;
        private bool _singleResultRecord = false;
        private long _searchResultID = -1L;

		private PersonSearch _pSearch;
		private PersonSearchList _pList;

        #region Protected overridden methods from parent class
		protected override void Initialize()
		{
			// base initialize
			base.Initialize();

			// Retrieving query string values
			_singleResultRecord = bool.Parse(GetQueryValue("singleResultRecord","false"));
			_backURL = GetQueryValue("backURL", psoft.Person.DetailFrame.GetURL("mode","searchResult", "xID","%SearchResultID"));
			_backTarget = GetQueryValue("backTarget","_parent");
			_orderColumn = GetQueryValue("orderColumn", "PNAME");
            _orderDir = GetQueryValue("orderDir", "asc");
            _searchResultID = GetQueryValue("searchResultID", -1L);
		}

		#endregion

		#region Properities
		public string SingleResultRecord 
		{
			get { return _singleResultRecord.ToString(); }
		}

		public string BackURL 
		{
			get { return HttpUtility.UrlEncode(_backURL); }
		}

		public string BackTarget 
		{
			get { return _backTarget; }
		}
		#endregion

        void SearchFrame_PreInit(object sender, EventArgs e)
        {
            // Setting main page layout
            //PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
            //PageLayoutControl = PsoftPageLayout;
            ////PsoftPageLayout.PageTitle = _mapper.get("person", "pageTitleSearchPerson");

            //// Setting content layout of page layout
            //PageLayoutControl.ContentLayoutControl = (SearchContentLayout)this.LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            //((SearchContentLayout)PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            //_pSearch = (PersonSearch)this.LoadPSOFTControl(PersonSearch.Path, "_pS");
            //_pSearch.OnSearchClick += new SearchClickHandler(ButtonSearchClick);
            //SetPageLayoutContentControl(SearchContentLayout.SEARCH, _pSearch);

            //_pList = (PersonSearchList)this.LoadPSOFTControl(PersonSearchList.Path, "_pL");
            //_pList.SingleResultRecord = _singleResultRecord;
            //_pList.BackURL = _backURL;
            //_pList.BackTarget = _backTarget;
            //_pList.OrderColumn = _orderColumn;
            //_pList.OrderDir = _orderDir;
            //_pList.SearchResultID = _searchResultID;
            //_pList.Visible = false;
            //_pList.DetailURL = Psoft.Person.DetailFrame.GetURL("mode", "oe", "id", "%ID");
            //_pList.DetailEnabled = true;
            //_pList.OnNextClick += new NextEventHandler(ButtonNextClick);
            //SetPageLayoutContentControl(SearchContentLayout.LIST, _pList);
        }

        protected void Page_Init(object sender, System.EventArgs e)
        {
            
        }
        
        protected void Page_Load(object sender, System.EventArgs e) 
		{
            //set language based on URL query string
            if (Request.QueryString["languageCode"] != null && Request.QueryString["languageCode"] != "")
            {
                Global.reloadLanguageMapper(LanguageMapper.getLanguageMapper(Session), Request.QueryString["languageCode"]);
            }
            
            //clear session information from previous search 
            Session["PersonSQLSearch"] = "";
            int anz = this.Controls.Count;
			// Setting default breadcrumb caption
			this.BreadcrumbCaption = _mapper.get("navigation","searchPerson");

            //// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("person", "pageTitleSearchPerson");

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (SearchContentLayout)this.LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout)PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            _pSearch = (PersonSearch)this.LoadPSOFTControl(PersonSearch.Path, "_pS");
            _pSearch.OnSearchClick += new SearchClickHandler(ButtonSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, _pSearch);

            _pList = (PersonSearchList)this.LoadPSOFTControl(PersonSearchList.Path, "_pL");
            _pList.SingleResultRecord = _singleResultRecord;
            _pList.BackURL = _backURL;
            _pList.BackTarget = _backTarget;
            _pList.OrderColumn = _orderColumn;
            _pList.OrderDir = _orderDir;
            _pList.SearchResultID = _searchResultID;
            _pList.Visible = false;
            _pList.DetailURL = psoft.Person.DetailFrame.GetURL("mode", "oe", "id", "%ID");
            _pList.DetailEnabled = true;
            _pList.OnNextClick += new NextEventHandler(ButtonNextClick);
            SetPageLayoutContentControl(SearchContentLayout.LIST, _pList);

            //this.Master.FindControl("ContentPlaceHolder1").Controls.Add(_pSearch);
            //this.Master.FindControl("ContentPlaceHolder1").Controls.Add(_pList);

            //TextBox txtTest = new TextBox();
            //this.Master.FindControl("ContentPlaceHolder1").Controls.Add(txtTest);
		}

		private void ButtonSearchClick(object Sender, SearchEventArgs e)
		{
			_pList.Reload = e.ReloadList;
			_pList.SearchSQL = e.SearchSQL;
			_pList.Visible = true;
			_pList.Execute();
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50);
        }

		private void ButtonNextClick(object Sender, NextEventArgs e)
		{
			this.Response.Redirect(e.LoadUrl, true);
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
        private void InitializeComponent() {    
        }
		#endregion
    }
}
