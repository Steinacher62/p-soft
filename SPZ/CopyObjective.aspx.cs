using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.SPZ.Controls;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.SPZ
{
    public partial class CopyObjective : PsoftSearchPage
    {
        private ch.appl.psoft.SPZ.Controls.CopyListView _list;
        private CopySearchView _searchCtrl;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            // save referer in session if coming from Detail.aspx
            if (Request.Headers["Referer"].Contains("Detail.aspx"))
            {
                Session["CopyObjectiveRef"] = Request.Headers["Referer"];
            }
            
            DBData db = DBData.getDBData(Session);
            db.connect();

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (SearchContentLayout)LoadPSOFTControl(SearchContentLayout.Path, "_cl");
            ((SearchContentLayout)PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            // Setting breadcrumb caption
            //BreadcrumbCaption = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_BC_COPYJOBEXPECTATIONS);
            BreadcrumbCaption = "Ziel von anderem Mitarbeiter kopieren";

            // Setting page-title
            //((PsoftPageLayout)PageLayoutControl).PageTitle = BreadcrumbCaption + " - " + db.lookup(db.langAttrName("JOB", "TITLE"), "JOB", "ID=" + _jobID, false);
            ((PsoftPageLayout)PageLayoutControl).PageTitle = "Ziel kopieren";

            _searchCtrl = (CopySearchView)LoadPSOFTControl(CopySearchView.Path, "_sc");
            _searchCtrl.OnSearchClick += new SearchClickHandler(onSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, _searchCtrl);

            _list = (ch.appl.psoft.SPZ.Controls.CopyListView)this.LoadPSOFTControl(ch.appl.psoft.SPZ.Controls.CopyListView.Path, "_list");
            _list.context = Objective.SEARCH;
            _list.Visible = false;
            _list.backURL = Request.RawUrl;
            //_list.OrderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], "TITLE");
            //_list.OrderColumn = "PERSON_ID";
            //_list.OrderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], "asc");
            _list.OnNextClick += new NextEventHandler(nextClick);
            SetPageLayoutContentControl(SearchContentLayout.LIST, _list);
        }

        private void onSearchClick(object Sender, SearchEventArgs e)
        {
            _list.Visible = true;
            _list.query = e.SearchSQL;
            _list.Execute();
            ((SearchContentLayout)PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50);
        }
        private void nextClick(object Sender, NextEventArgs e)
        {
            //this.Response.Redirect(e.LoadUrl, true);

            //return to page we came from / 23.11.10 / mkr
            //this.Response.Redirect(Request.Headers["Referer"], true);
        }
    }
}
