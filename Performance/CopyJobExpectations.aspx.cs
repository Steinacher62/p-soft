using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Performance.Controls;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Performance
{
    public partial class CopyJobExpectations : PsoftSearchPage {
        private ExpectationListView _listCtrl;
        private ExpectationsSearch _searchCtrl;
        
        // Query string variables
        private long _jobID = -1;
        private string _backURL = "";

		#region Protected overrided methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _jobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], _jobID);
            _backURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["backURL"], _backURL);
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            DBData db = DBData.getDBData(Session);
            db.connect();
            
            try {
                // Setting main page layout
                PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
                PageLayoutControl = PsoftPageLayout;

                // Setting content layout of page layout
                PageLayoutControl.ContentLayoutControl = (SearchContentLayout) LoadPSOFTControl(SearchContentLayout.Path, "_cl");
                ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

                // Setting breadcrumb caption
                BreadcrumbCaption = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_BC_COPYJOBEXPECTATIONS);

                // Setting page-title
                ((PsoftPageLayout)PageLayoutControl).PageTitle = BreadcrumbCaption + " - " + db.lookup(db.langAttrName("JOB","TITLE"), "JOB", "ID=" + _jobID, false);

                _searchCtrl = (ExpectationsSearch) LoadPSOFTControl(ExpectationsSearch.Path, "_sc");
                _searchCtrl.OnSearchClick +=new SearchClickHandler(onSearchClick);
                SetPageLayoutContentControl(SearchContentLayout.SEARCH, _searchCtrl);

                _listCtrl = (ExpectationListView)this.LoadPSOFTControl(ExpectationListView.Path, "_lt");
                _listCtrl.BackURL = _backURL;
                _listCtrl.Visible = false;
                _listCtrl.Mode = "search";
                _listCtrl.DetailEnabled = false;
                _listCtrl.DeleteEnabled = false;
                _listCtrl.EditEnabled = false;
                _listCtrl.OrderColumn = "VALID_DATE";
                _listCtrl.OrderDir = "desc";
                _listCtrl.OnNextClick += new NextEventHandler(onNextClick);
                SetPageLayoutContentControl(SearchContentLayout.LIST, _listCtrl);
            }
            catch(Exception ex) {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally {
                db.disconnect();
            }
        }

        private void onSearchClick(object Sender, SearchEventArgs e) {
            _listCtrl.Reload = e.ReloadList;
            _listCtrl.SearchSQL = e.SearchSQL;
            _listCtrl.Visible = true;
            _listCtrl.Execute();
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50);
        }

        private void onNextClick(object Sender, NextEventArgs e) {
            DBData db = DBData.getDBData(Session);

            db.connect();
            try {
                db.beginTransaction();
                string descriptionAttrs = db.langExpand("DESCRIPTION%LANG%", "JOB_EXPECTATION", "DESCRIPTION");
				string titleAttrs = db.langExpand("TITLE%LANG%", "JOB_EXPECTATION", "TITLE");
                string templateSQL = "insert into JOB_EXPECTATION (CRITERIA_REF, JOB_REF, " + descriptionAttrs + "," + titleAttrs + ", VALID_DATE) select CRITERIA_REF, " + _jobID + "," + descriptionAttrs + "," + titleAttrs+ ",getdate() from JOB_EXPECTATION where ID=";
                DataTable table = db.getDataTable("select ROW_ID from SEARCHRESULT where ID=" + e.SearchResultID);
                foreach (DataRow row in table.Rows){
                    string sql = templateSQL + DBColumn.GetValid(row[0], -1L);
                    db.execute(sql);
                }
                db.commit();
            }
            catch (Exception ex) {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally {
                db.disconnect();
            }

            Response.Redirect(e.LoadUrl, true);
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
