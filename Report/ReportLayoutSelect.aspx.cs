using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report.Controls;
using System;

namespace ch.appl.psoft.Report
{
    /// <summary>
    /// Summary description for ReportLayoutSelect
    /// </summary>
    public partial class ReportLayoutSelect : PsoftMainPage
	{
        private const string PAGE_URL = "/Report/ReportLayoutSelect.aspx";

        static ReportLayoutSelect(){
            SetPageParams(PAGE_URL, "target", "type", "titleMnemo", "backURL", "nextURL");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public ReportLayoutSelect() : base() {
            PageURL = PAGE_URL;
        }

        protected ReportLayoutSelectCtrl _selectCtrl = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            BreadcrumbVisible = false;

            // Setting main page layout
            PageLayoutControl = (SimplePageLayout)this.LoadPSOFTControl(SimplePageLayout.Path, "_pl");

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (SimpleContentLayout)this.LoadPSOFTControl(SimpleContentLayout.Path, "_Cl");

            // Setting the report-select control of simple content layout
            _selectCtrl = (ReportLayoutSelectCtrl)this.LoadPSOFTControl(ReportLayoutSelectCtrl.Path, "_repSel");
            _selectCtrl.DetailURL = GetQueryValue("nextURL", _selectCtrl.DetailURL);
            _selectCtrl.BackURL = GetQueryValue("backURL", _selectCtrl.BackURL);
            _selectCtrl.Target = (ReportModule.Target) GetQueryValue("target", (int) _selectCtrl.Target);
            _selectCtrl.Types = GetQueryValue("type", _selectCtrl.Types);
            _selectCtrl.TitleMnemo = GetQueryValue("titleMnemo", _selectCtrl.TitleMnemo);
            _selectCtrl.SortURL = psoft.Report.ReportLayoutSelect.GetURL("target",(int)_selectCtrl.Target, "type",_selectCtrl.Types, "nextURL",_selectCtrl.DetailURL, "backURL",_selectCtrl.BackURL);

            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, _selectCtrl);
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
