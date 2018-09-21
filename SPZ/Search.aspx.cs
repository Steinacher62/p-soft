using ch.appl.psoft.Common;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.SPZ.Controls;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.SPZ
{
    /// <summary>
    /// Summary description for Search.
    /// </summary>
    public partial class Search : PsoftTreeViewPage {
        private ch.appl.psoft.SPZ.Controls.ListView _list = null;
        private SearchView _search = null;

		#region Protected overrided methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();
            //base.SubNavMenuUrl = "/MbO/SubNavMenu.aspx?context="+Objective.SEARCH+"','search";
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            this.BreadcrumbCaption = _mapper.get("mbo","objectiveSearch");
            this.BreadcrumbName = "ObjectiveSearch";

            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
            (PageLayoutControl as PsoftPageLayout).PageTitle = _mapper.get("mbo","objectiveSearch");

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (SearchContentLayout)this.LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            _search = (SearchView)this.LoadPSOFTControl(SearchView.Path, "_search");
            _search.OnSearchClick +=new SearchClickHandler(onSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, _search);

            _list = (ch.appl.psoft.SPZ.Controls.ListView)this.LoadPSOFTControl(ch.appl.psoft.SPZ.Controls.ListView.Path, "_list");
            _list.context = Objective.SEARCH;
            _list.Visible = false;
            _list.backURL = Request.RawUrl;
            _list.OrderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], "TITLE");
            //_list.OrderColumn = "PERSON_ID";
            _list.OrderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], "asc");
            _list.OnNextClick += new NextEventHandler(nextClick);
            SetPageLayoutContentControl(SearchContentLayout.LIST, _list);
        }
        private void onSearchClick(object Sender, SearchEventArgs e) {
            _list.Visible = true;
            _list.query = e.SearchSQL;
            _list.Execute();
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50);
        }
        private void nextClick(object Sender, NextEventArgs e) {
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
