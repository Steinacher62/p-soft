using ch.appl.psoft.Common;
using ch.appl.psoft.Dispatch.Controls;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Dispatch
{
    /// <summary>
    /// Summary description for ManualMail.
    /// </summary>
    public partial class ManualMail : PsoftContentPage {

        private const string PAGE_URL = "/Dispatch/ManualMail.aspx";

        static ManualMail(){
            SetPageParams(PAGE_URL, "xID", "reportLayoutID", "mailingID", "useSameTemplate", "searchResultID", "backURL", "param0", "param1", "param2");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public ManualMail() : base() {
            PageURL = PAGE_URL;
        }

        /// <summary>
        /// Manual creation of a mailing, where the user can select templates, persons, etc depending on the context.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, System.EventArgs e) {
            BreadcrumbVisible = false;

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout) LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            
            // Setting detail parameters
            ManualMailCtrl mm = (ManualMailCtrl) LoadPSOFTControl(ManualMailCtrl.Path, "_mm");
            mm.xID = GetQueryValue("xID", mm.xID);
            mm.ReportLayoutID = GetQueryValue("reportLayoutID", mm.ReportLayoutID);
            mm.MailingID = GetQueryValue("mailingID", mm.MailingID);
            mm.UseSameTemplate = bool.Parse(GetQueryValue("useSameTemplate", mm.UseSameTemplate.ToString()));
            mm.SearchResultID = GetQueryValue("searchResultID", mm.SearchResultID);
            mm.BackURL = GetQueryValue("backURL", mm.BackURL);
            mm.SubstituteValues = new String[] {GetQueryValue("param0", ""), GetQueryValue("param1", ""), GetQueryValue("param2", "")};

            PsoftPageLayout.PageTitle = _mapper.get("reportLayout", mm.GetTitleMnemo());

            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, mm);
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
