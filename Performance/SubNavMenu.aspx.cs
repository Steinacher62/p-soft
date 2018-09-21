using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Performance
{
    public partial class SubNavMenu : PsoftMenuPage
	{
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Setting main page layout
            MenuPageLayout pageLayout = (MenuPageLayout) LoadPSOFTControl(MenuPageLayout.Path, "_pl");;
            PageLayoutControl = pageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout) LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting detail parameters
            MenuControl ctrl = (MenuControl) LoadPSOFTControl(MenuControl.Path, "_ctrl");
            ctrl.Title = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_SUBNAV_TITLE);
            ctrl.addMenuItem(null, "search", _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_SUBNAV_SEARCH), Global.Config.baseURL + "/Performance/Search.aspx");
            ctrl.addMenuItem(null, "subnavSearchPersonWithoutRating", _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_SUBNAV_SEARCH_PERSONWITHOUTRATING), Global.Config.baseURL + "/Performance/Search.aspx?context=subnavSearchPersonWithoutRating");
            ctrl.addMenuItem(null, "subnavReportAverageOEPerformance", _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_SUBNAV_REPORT_AVERAGEEOPERFORMANCE), Global.Config.baseURL + "/Performance/SelectPeriod.aspx?context=subnavReportAverageOEPerformance");
            ctrl.addMenuItem(null, "subnavReportPerformanceChange", _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_SUBNAV_REPORT_PERFORMANCECHANGE), Global.Config.baseURL + "/Performance/SelectPeriod.aspx?context=subnavReportPerformanceChange");
            ctrl.StartPageLink = Global.Config.baseURL + "/Performance/Search.aspx";

            //Setting content layout user controls
            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, ctrl);
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
