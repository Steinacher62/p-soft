using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report.Controls;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Report
{
    public partial class SearchPersonWithoutPerformanceRating : PsoftSearchPage
    {
        private SearchPersonWithoutPerformanceRatingCtrl _search;
        protected PsoftPageLayout PsoftPageLayout;

        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);

            // Setting main page layout
            PsoftPageLayout = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
            PageLayoutControl = PsoftPageLayout;
            BreadcrumbCaption = PsoftPageLayout.PageTitle = _mapper.get("SelectOEDateRange", "searchTitle");

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (SearchContentLayout)this.LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout)PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            _search = (SearchPersonWithoutPerformanceRatingCtrl)this.LoadPSOFTControl(SearchPersonWithoutPerformanceRatingCtrl.Path, "_search");
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, _search);
        }
    }
}
