using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Knowledge.Controls;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Knowledge
{
    using Interface.DBObjects;

    public partial class Search : PsoftTreeViewPage {

        private const string PAGE_URL = "/Knowledge/Search.aspx";

        static Search(){
            SetPageParams(PAGE_URL, "orderColumn", "orderDir", "nextURL");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public Search() /*: base("get")*/ {
            PageURL = PAGE_URL;
        }

        private KnowledgeListCtrl _list = null;
        private KnowledgeSearchCtrl _search = null;

		#region Protected overrided methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();

            //SubNavMenuUrl = "/Knowledge/SubNavMenu.aspx";
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            BreadcrumbCaption = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_BC_SEARCH_KNOWLEDGE);

            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
            (PageLayoutControl as PsoftPageLayout).PageTitle = BreadcrumbCaption;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (SearchContentLayout) LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            _search = (KnowledgeSearchCtrl) LoadPSOFTControl(KnowledgeSearchCtrl.Path, "_search");
            _search.OnSearchClick += new SearchClickHandler(onSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, _search);

            _list = (KnowledgeListCtrl) LoadPSOFTControl(KnowledgeListCtrl.Path, "_list");
            _list.OnNextClick += new NextEventHandler(onNextClick);
            _list.Visible = false;
            //_list.NextURL = GetQueryValue("nextURL", "");
            SetPageLayoutContentControl(SearchContentLayout.LIST, _list);
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

        private void onSearchClick(object Sender, SearchEventArgs e) {
            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                _list.OrderColumn = GetQueryValue("orderColumn", db.langAttrName(Knowledge._VIEWNAME, "TITLE"));
            }
            catch (Exception ex) {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally {
                db.disconnect();
            }
            _list.Visible = true;
            _list.OrderDir = GetQueryValue("orderDir", "asc");
            _list.DetailEnabled = true;
            _list.DetailURL = psoft.Knowledge.KnowledgeDetail.GetURL("knowledgeID","%ID");
            _list.Kontext = "search";
            _list.Query = e.SearchSQL;
            _list.NextURL = GetQueryValue("nextURL", "");
            _list.Execute();
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50);
        }

        private void onNextClick(object Sender, NextEventArgs e) {
            Response.Redirect(e.LoadUrl, true);
        }

    }
}
