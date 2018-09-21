using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Survey
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
            ctrl.Title = _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_SUBNAV_TITLE);
            ctrl.TitleLink = Global.Config.baseURL + "/Survey/SurveyDetail.aspx?context=person";
            if (SurveyModule.showSearchSurveyMenu){
                ctrl.addMenuItem(null, "searchSurvey", _mapper.get(SurveyModule.LANG_SCOPE_SURVEY, SurveyModule.LANG_MNEMO_SUBNAV_SEARCHSURVEY), Global.Config.baseURL + "/Survey/SurveySearch.aspx");
            }

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
