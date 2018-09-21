using ch.appl.psoft.Common;
using ch.appl.psoft.FBS.Controls;
using ch.appl.psoft.LayoutControls;
using System;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for FunctionSearch.
    /// </summary>
    public partial class FunctionSearch : PsoftContentPage {
        private FunctionListCtrl _list = null;

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("fbs","functionSearch");
            BreadcrumbCaption = _mapper.get("fbs","functionSearch");

            // Setting content layout of page layout
            SearchContentLayout contentLayout = (SearchContentLayout) LoadPSOFTControl(SearchContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            FunctionSearchCtrl search = (FunctionSearchCtrl)this.LoadPSOFTControl(FunctionSearchCtrl.Path, "_search");
            search.OnSearchClick += new SearchClickHandler(onSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, search);

            _list = (FunctionListCtrl)this.LoadPSOFTControl(FunctionListCtrl.Path, "_list");
            _list.Visible = false;
            _list.context = "SEARCH";
            _list.OnNextClick += new NextEventHandler(nextClick);
            SetPageLayoutContentControl(SearchContentLayout.LIST, _list);
            
        }
        private void onSearchClick(object Sender, SearchEventArgs e) {
            _list.Visible = true;
            _list.query = e.SearchSQL;
            _list.Execute();
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = 150;
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
