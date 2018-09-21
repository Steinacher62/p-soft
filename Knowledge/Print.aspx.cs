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

    public partial class Print : PsoftTreeViewPage {

        private const string PAGE_URL = "/Knowledge/Print.aspx";

        static Print(){
            SetPageParams(PAGE_URL, "orderColumn", "orderDir", "nextURL", "PrintSelection");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public Print() : base(){
            PageURL = PAGE_URL;
        }

        private KnowledgePrintListCtrl _list = null;
        private KnowledgeSearchPrintCtrl _search = null;

        //never used, thus commented
        //int _print = 0;

		#region Protected overrided methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();
            SubNavMenuUrl = "/Knowledge/SubNavMenu.aspx";
        }
		#endregion

      
        public delegate void PrintClickHandler(bool v);

        protected void Page_Load(object sender, System.EventArgs e) {

 
      
            BreadcrumbCaption = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_BC_PRINT_KNOWLEDGE);

            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
            (PageLayoutControl as PsoftPageLayout).PageTitle = BreadcrumbCaption;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (SearchContentLayout) LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            _search = (KnowledgeSearchPrintCtrl) LoadPSOFTControl(KnowledgeSearchPrintCtrl.Path, "_search");
            _search.OnSearchClick += new SearchClickHandler(onSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, _search);

            _list = (KnowledgePrintListCtrl) LoadPSOFTControl(KnowledgePrintListCtrl.Path, "_list");
            _list.OnNextClick += new NextEventHandler(onNextClick);
            _list.Visible = false;
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
                _list.OrderColumn = GetQueryValue("orderColumn", db.langAttrName("KNOWLEDGE_V", "TITLE"));
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
            _list.Kontext = "search";
            _list.Query = e.SearchSQL;
            _list.NextURL = GetQueryValue("nextURL", "");
            
            //Custom data
            if(e.CustomData.Equals("Wissen")) 
            {
                _list.DetailURL = psoft.Knowledge.KnowledgeDetail.GetURL("knowledgeID","%ID"); 
                _list.IsWissenElementGrouped = true;
                _list.ListKontext = Knowledge._VIEWNAME;
            }
            else 
            {
                _list.DetailURL = psoft.Knowledge.KnowledgeDetail.GetURL("knowledgeID","%K_ID","themeID","%ID"); 
                _list.IsWissenElementGrouped = false;
                _list.ListKontext = ch.appl.psoft.Interface.DBObjects.Theme._TABLENAME;
            }
            _list.Execute();
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50);
            
        }

        /// <summary>
        /// callback
        /// </summary>
        private void onNextClick(object Sender, NextEventArgs e)
        {
            _list.IsWissenElementGrouped = this._search.IsCheckBoxKnowledgeChecked;
        }   
 

    }
}
