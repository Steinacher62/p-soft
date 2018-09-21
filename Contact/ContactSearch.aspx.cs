using ch.appl.psoft.Common;
using ch.appl.psoft.Contact.Controls;
using ch.appl.psoft.LayoutControls;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Contact
{
    /// <summary>
    /// Summary description for ContactSearch.
    /// </summary>
    /// <param name="backURL">URL to go redirect afterwards</param>
    /// <param name="backTarget">Target for backURL</param>
    /// </param>
    public partial class ContactSearch : PsoftSearchPage {
        public const string PAGE_URL = "/Contact/ContactSearch.aspx";
        
        static ContactSearch(){
            SetPageParams(PAGE_URL, "nextURL", "orderColumn", "orderDir", "searchResultID");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public ContactSearch() : base(){
            PageURL = PAGE_URL;
        }

        private string _nextURL = "";
		private string _orderColumn;
		private string _orderDir;
        private long _searchResultID = -1L;

		private Controls.ContactSearch _pSearch;
		private Controls.ContactSearchList _pList;

        #region Protected overridden methods from parent class
		protected override void Initialize()
		{
			// base initialize
			base.Initialize();

			// Retrieving query string values
			_nextURL = GetQueryValue("nextURL", "ContactDetail.aspx?mode=" + ContactDetail.MODE_SEARCHRESULT + "&xID=%SearchResultID");
			_orderColumn = GetQueryValue("orderColumn", "NAME");
            _orderDir = GetQueryValue("orderDir", "asc");
            _searchResultID = GetQueryValue("searchResultID", _searchResultID);

			// Setting SubNavMenu URL
			SubNavMenuUrl = "/Contact/SubNavMenu.aspx','searchContact";
		}

		#endregion

		protected void Page_Load(object sender, System.EventArgs e) 
		{
			// Setting default breadcrumb caption
			BreadcrumbCaption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_BCSEARCHCONTACT);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
			PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_PTSEARCHCONTACT);					

			// Setting content layout of page layout
			PageLayoutControl.ContentLayoutControl = (SearchContentLayout) LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

			_pSearch = (Contact.Controls.ContactSearch) LoadPSOFTControl(Contact.Controls.ContactSearch.Path, "_pS");
			_pSearch.OnSearchClick += new SearchClickHandler(ButtonSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, _pSearch);

			_pList = (ContactSearchList) LoadPSOFTControl(ContactSearchList.Path, "_pL");
			_pList.NextURL = _nextURL;
            _pList.SearchResultID = _searchResultID; // if a searchResultID is provided the selection will be added to the existing searchresult.
			_pList.OrderColumn = _orderColumn;
			_pList.OrderDir = _orderDir;
			_pList.Visible = false;
            _pList.DetailURL = Global.Config.baseURL + "/Contact/ContactDetail.aspx?id=%ID&type=%TABLENAME";
            _pList.DetailEnabled = true;
			_pList.OnNextClick += new NextEventHandler(ButtonNextClick);
            SetPageLayoutContentControl(SearchContentLayout.LIST, _pList);
		}

		private void ButtonSearchClick(object Sender, SearchEventArgs e)
		{
			_pList.Reload = e.ReloadList;
			_pList.SearchSQL = e.SearchSQL;
			_pList.Visible = true;
			_pList.Execute();
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = 230;
        }

		private void ButtonNextClick(object Sender, NextEventArgs e)
		{
			Response.Redirect(e.LoadUrl, true);
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
