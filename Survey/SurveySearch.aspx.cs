using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Survey.Controls;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Survey
{
    public partial class SurveySearch : PsoftSearchPage {

        private string _orderColumn = "TITLE";
        private string _orderDir = "asc";

        private SurveySearchCtrl _sSearch;
        private SurveyList _sList;

        #region Protected overridden methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _orderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], _orderColumn);
            _orderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], _orderDir);
        }

		#endregion

		#region Properities
		#endregion
		
        protected void Page_Load(object sender, System.EventArgs e) {
            //clear session information from previous search 
            Session["SurveySQLSearch"] = "";

            // Setting default breadcrumb caption
            BreadcrumbCaption = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_BC_SEARCHSURVEY);

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_sPL");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = BreadcrumbCaption;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (SearchContentLayout) LoadPSOFTControl(SearchContentLayout.Path, "_sCL");
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            _sSearch = (SurveySearchCtrl) LoadPSOFTControl(SurveySearchCtrl.Path, "_sS");
            _sSearch.OnSearchClick += new SearchClickHandler(ButtonSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, _sSearch);

            _sList = (SurveyList) LoadPSOFTControl(SurveyList.Path, "_pL");
            _sList.Mode = SurveyList.MODE_SEARCHRESULT;
            _sList.OrderColumn = _orderColumn;
            _sList.OrderDir = _orderDir;
            _sList.Visible = false;
            _sList.DetailURL = Global.Config.baseURL + "/Survey/SurveyDetail.aspx?id=%ID";
            _sList.DetailEnabled = true;
            _sList.DeleteEnabled = false;
            _sList.EditEnabled = false;
            _sList.BackURL = Global.Config.baseURL + "/Survey/SurveyDetail.aspx?context=" + SurveyList.CONTEXT_SEARCHRESULT + "&xID=%SearchResultID";
            _sList.OnNextClick += new NextEventHandler(ButtonNextClick);
            SetPageLayoutContentControl(SearchContentLayout.LIST, _sList);
        }

        private void ButtonSearchClick(object Sender, SearchEventArgs e) {
            _sList.Reload = e.ReloadList;
            _sList.SearchSQL = e.SearchSQL;
            _sList.Visible = true;
            _sList.Execute();
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50);
        }

        private void ButtonNextClick(object Sender, NextEventArgs e) {
            Response.Redirect(e.LoadUrl, false);
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
