using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Morph.Controls;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Morph
{
    public partial class MatrixSearch : PsoftTreeViewPage
    {
        // Fields
        private MatrixListCtrl _list;
        private MatrixSearchCtrl _search;
        public const string MODE_SLAVE = "slave";
        public const string ORDER_COLUMN = "TITLE";
        private const string PAGE_URL = "/Morph/MatrixSearch.aspx";

        // Methods
        static MatrixSearch()
        {
            PsoftPage.SetPageParams("/Morph/MatrixSearch.aspx", new string[] { "type", "nextURL" });
        }

        public MatrixSearch()
        {
            base.PageURL = "/Morph/MatrixSearch.aspx";
        }

        public static string GetURL(params object[] queryParams)
        {
            return PsoftPage.CreateURL("/Morph/MatrixSearch.aspx", queryParams);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        private void InitializeComponent()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

       

        protected void Page_Load(object sender, EventArgs e)
        {
            base.BreadcrumbCaption = base._mapper.get("morph", "bcSearchMatrix");
            base.PageLayoutControl = (PsoftPageLayout)base.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
            base.PageLayoutControl.ContentLayoutControl = (SearchContentLayout)base.LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout)base.PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100.0);
            this._search = (MatrixSearchCtrl)base.LoadPSOFTControl(MatrixSearchCtrl.Path, "_search");
            this._search.Mode = base.GetQueryValue("type", "");
            //this._search.OnSearchClick += new SearchClickHandler(this.onSearchClick);
            this._search.OnSearchClick += new SearchClickHandler(onSearchClick);
            base.SetPageLayoutContentControl("SEARCH", this._search);
            this._list = (MatrixListCtrl)base.LoadPSOFTControl(MatrixListCtrl.Path, "_list");
            this._list.OrderColumn = "TITLE";
            this._list.NextURL = base.GetQueryValue("nextURL", "");
            this._list.Mode = this._search.Mode;
            this._list.OnNextClick += new NextEventHandler(this.onNextClick);
            this._list.Visible = false;
            if (this._list.Mode != "slave")
            {
                base.BreadcrumbCaption = base._mapper.get("morph", "bcSearchMatrix");
            }
            else
            {
                base.BreadcrumbCaption = base._mapper.get("morph", "bcSearchSlave");
            }
            (base.PageLayoutControl as PsoftPageLayout).PageTitle = base.BreadcrumbCaption;
            base.SetPageLayoutContentControl("LIST", this._list);
        }

        private void onNextClick(object Sender, NextEventArgs e)
        {
            base.Response.Redirect(e.LoadUrl, true);
        }

        private void onSearchClick(object Sender, SearchEventArgs e)
        {
            this._list.Visible = true;
            this._list.OrderDir = base.GetQueryValue("orderDir", "asc");
            this._list.DetailEnabled = true;
            if (this._list.Mode == "slave")
            {
                this._list.DetailURL = MatrixDetail.GetURL(new object[] { "matrixID", "%ID", "slave", "true" });
            }
            else
            {
                this._list.DetailURL = MatrixDetail.GetURL(new object[] { "matrixID", "%ID" });
            }
            this._list.Query = e.SearchSQL;
            this._list.NextURL = base.GetQueryValue("nextURL", "");
            this._list.Execute();
            ((SearchContentLayout)base.PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50.0);
        }
    }
}