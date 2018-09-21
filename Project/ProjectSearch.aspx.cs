using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Project
{
    public partial class ProjectSearch : PsoftTreeViewPage {
        private const string PAGE_URL = "/Project/ProjectSearch.aspx";

        static ProjectSearch() {
            SetPageParams(PAGE_URL, "nextURL", "orderColumn", "orderDir");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public ProjectSearch() : base() {
            PageURL = PAGE_URL;
        }

        private string _orderColumn;
        private string _orderDir;

        private ProjectSearchCtrl _pSearch;
        private ProjectList _pList;

        #region Protected overridden methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _orderColumn = GetQueryValue("orderColumn", "TITLE");
            _orderDir = GetQueryValue("orderDir", "asc");
        }
		#endregion
		
        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting default breadcrumb caption
            BreadcrumbCaption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_SEARCHPROJECT);

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = BreadcrumbCaption;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (SearchContentLayout) LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            _pSearch = (ProjectSearchCtrl) LoadPSOFTControl(ProjectSearchCtrl.Path, "_pS");
            _pSearch.OnSearchClick += new SearchClickHandler(ButtonSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, _pSearch);

            _pList = (ProjectList) LoadPSOFTControl(ProjectList.Path, "_pL");
            _pList.Mode = ProjectList.MODE_SEARCHRESULT;
            _pList.OrderColumn = _orderColumn;
            _pList.OrderDir = _orderDir;
            _pList.Visible = false;
            _pList.DetailURL = psoft.Project.ProjectDetail.GetURL("id","%ID");
            //_pList.SortURL = Psoft.Project.ProjectDetail.GetURL("id", "%ID");
            _pList.DetailEnabled = true;
            _pList.DeleteEnabled = false;
            _pList.EditEnabled = false;
            _pList.NextURL = GetQueryValue("nextURL", psoft.Project.ProjectDetail.GetURL("context",ProjectList.CONTEXT_SEARCHRESULT, "xID","%SearchResultID"));
            _pList.OnNextClick += new NextEventHandler(ButtonNextClick);
            SetPageLayoutContentControl(SearchContentLayout.LIST, _pList);
        }

        private void ButtonSearchClick(object Sender, SearchEventArgs e) {
            _pList.Reload = e.ReloadList;
            _pList.SearchSQL = e.SearchSQL;
            _pList.Visible = true;
            _pList.Execute();
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
