using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report.Controls;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Report
{
    /// <summary>
    /// Summary description for ReportListing.
    /// </summary>
    public partial class ReportListing : PsoftMainPage
    {
        private const string PAGE_URL = "/Report/ReportListing.aspx";

        static ReportListing(){
            SetPageParams(PAGE_URL, "backURL", "reportLayoutID", "param0", "param1", "param2", "filterQuery");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        protected ReportListingCtrl _listingCtrl = null;

        public ReportListing() : base()
        {
            UseWebService = false;
            ShowProgressBar = false;
            PageURL = PAGE_URL;
        }
        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Setting default breadcrumb caption
            this.BreadcrumbCaption = _mapper.get("REPORTLAYOUT", "TITLE_MNEMO");

            // Setting main page layout
            PageLayoutControl = (SimplePageLayout)this.LoadPSOFTControl(SimplePageLayout.Path, "_pl");

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (SimpleContentLayout)this.LoadPSOFTControl(SimpleContentLayout.Path, "_Cl");

            // Setting the report-select control of simple content layout
            _listingCtrl = (ReportListingCtrl)this.LoadPSOFTControl(ReportListingCtrl.Path, "_repList");
            _listingCtrl.BackURL        = GetQueryValue("backURL", _listingCtrl.BackURL);
            _listingCtrl.ReportLayoutID = GetQueryValue("reportLayoutID", _listingCtrl.ReportLayoutID);
            _listingCtrl.Param0         = GetQueryValue("param0", _listingCtrl.Param0);
            _listingCtrl.Param1         = GetQueryValue("param1", _listingCtrl.Param1);
            _listingCtrl.Param2         = GetQueryValue("param2", _listingCtrl.Param2);
            _listingCtrl.FilterQuery    = GetQueryValue("filterQuery", _listingCtrl.FilterQuery);

            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, _listingCtrl);

            if (_listingCtrl.ReportLayoutID > 0)
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                try
                {
                    this.BreadcrumbCaption = _mapper.get("reportLayout", db.lookup("title_mnemo","reportlayout","id="+_listingCtrl.ReportLayoutID,false));
                }
                catch (Exception ex) 
                {
                    Logger.Log(ex, Logger.ERROR);
                    ShowError(ex.Message);
                }
                finally 
                {
                    db.disconnect();
                }
            }
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) 
        {
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
        private void InitializeComponent() 
        {    

        }
		#endregion
    }
}
