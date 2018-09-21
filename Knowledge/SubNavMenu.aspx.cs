using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Knowledge
{
    /// <summary>
    /// Summary description for SubNavMenu.
    /// </summary>
    public partial class SubNavMenu : PsoftMenuPage {

        private const string PAGE_URL = "/Knowledge/SubNavMenu.aspx";

        static SubNavMenu(){
            SetPageParams(PAGE_URL);
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public SubNavMenu() : base(){
            PageURL = PAGE_URL;
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            MenuPageLayout pageLayout = (MenuPageLayout) LoadPSOFTControl(MenuPageLayout.Path, "_pl");;
            PageLayoutControl = pageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout)this.LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting detail parameters
            MenuControl ctrl = (MenuControl)this.LoadPSOFTControl(MenuControl.Path, "_ctrl");
            ctrl.Title = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_KNOWLEDGE);
            ctrl.addMenuItem(null, KnowledgeModule.LANG_MNEMO_MI_SEARCH_KNOWLEDGE, _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_MI_SEARCH_KNOWLEDGE), psoft.Knowledge.Search.GetURL());
            ctrl.addMenuItem(null, KnowledgeModule.LANG_MNEMO_MI_NEW_KNOWLEDGE, _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_MI_NEW_KNOWLEDGE), psoft.Knowledge.EditKnowledge.GetURL("mode","add", "backURL",psoft.Knowledge.KnowledgeDetail.GetURL("knowledgeID","%ID")));
            ctrl.addMenuItem(null, KnowledgeModule.LANG_MNEMO_MI_PRINT_KNOWLEDGE, _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_MI_PRINT_KNOWLEDGE), psoft.Knowledge.Print.GetURL() );


			ctrl.Mnemo = "KNOWLEDGE";
			ctrl.LoadFromDB = true;

            //Setting content layout user controls
            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, ctrl);
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
