using ch.appl.psoft.Common;
using ch.appl.psoft.FBS.Controls;
using ch.appl.psoft.LayoutControls;
///TODO
///

using System;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for PrintResponsabilityMatrix.
    /// </summary>
    public partial class PrintResponsabilityMatrix : PsoftContentPage 
    {
        public const string PAGE_URL = "/FBS/PrintResponsabilityMatrix.aspx";

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        protected void Page_Load(object sender, System.EventArgs e) 
        {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("fbs","responsabilityMatrixTitle");
            BreadcrumbCaption = _mapper.get("fbs","organisationUnitSelection"); 

            // Setting content layout of page layout
            SearchContentLayout contentLayout = (SearchContentLayout) LoadPSOFTControl(SearchContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            OrganisationUnitSearchCtrl search = (OrganisationUnitSearchCtrl)this.LoadPSOFTControl(OrganisationUnitSearchCtrl.Path, "_search");
            search.ReportType = OrganisationUnitSearchCtrl.ReportTypeEnum.RESPONSABILITY;
            search.OnSearchClick += new SearchClickHandler(onSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, search);

            
        }
        private void onSearchClick(object Sender, SearchEventArgs e) 
        {
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = 150;
        }

        private void nextClick(object Sender, NextEventArgs e) 
        {
            this.Response.Redirect(e.LoadUrl, true);
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
