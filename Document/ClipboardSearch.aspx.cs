using ch.appl.psoft.Common;
using ch.appl.psoft.Document.Controls;
using ch.appl.psoft.LayoutControls;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Document
{
    public partial class ClipboardSearch : PsoftTreeViewPage {

        private const string PAGE_URL = "/Document/ClipboardSearch.aspx";

        static ClipboardSearch(){
            SetPageParams(PAGE_URL, "orderColumn", "orderDir", "nextURL", "includeLinks");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public ClipboardSearch() : base(){
            PageURL = PAGE_URL;
        }

        private string _orderColumn;
        private string _orderDir;
        private string _nextURL;

        private ClipboardSearchCtrl _dSearch;
        private ClipboardList _dList;

        #region Protected overridden methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _orderColumn = GetQueryValue("orderColumn", "TITLE");
            _orderDir = GetQueryValue("orderDir", "asc");
            _nextURL = GetQueryValue("nextURL", psoft.Document.Detail.GetURL("table","DOCUMENT", "context","searchresult", "contextID","%SearchResultID"));
        }

		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting default breadcrumb caption
            BreadcrumbCaption = _mapper.get("clipboard","search");

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = BreadcrumbCaption;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (SearchContentLayout) LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            _dSearch = (ClipboardSearchCtrl) LoadPSOFTControl(ClipboardSearchCtrl.Path, "_dS");
            _dSearch.IncludeLinks = Boolean.Parse(GetQueryValue("includeLinks", "true"));
            _dSearch.OnSearchClick += new SearchClickHandler(ButtonSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, _dSearch);

            _dList = (ClipboardList) LoadPSOFTControl(ClipboardList.Path, "_dL");
            _dList.Mode = DocumentList.MODE_SEARCHRESULT;
            _dList.OrderColumn = _orderColumn;
            _dList.OrderDir = _orderDir;
            _dList.Visible = false;
            _dList.DetailURL = psoft.Document.Clipboard.GetURL("ID","%ID");
            _dList.DetailEnabled = true;
            _dList.DeleteEnabled = false;
            _dList.EditEnabled = false;
            _dList.NextURL = _nextURL;
            _dList.OnNextClick += new NextEventHandler(ButtonNextClick);
            SetPageLayoutContentControl(SearchContentLayout.LIST, _dList);
        }

        private void ButtonSearchClick(object Sender, SearchEventArgs e) {
            _dList.Reload = e.ReloadList;
            _dList.SearchSQL = e.SearchSQL;
            _dList.Contents = e.CustomData;
            _dList.Visible = true;
            _dList.Execute();
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50);
        }

        private void ButtonNextClick(object Sender, NextEventArgs e) {
            this.Response.Redirect(e.LoadUrl, true);
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
