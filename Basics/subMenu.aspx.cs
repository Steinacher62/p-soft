using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Basics
{
    /// <summary>
    /// Summary description for SubNavMenu.
    /// </summary>
    public partial class subMenu : PsoftMenuPage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            // Setting main page layout
            MenuPageLayout pageLayout = (MenuPageLayout) LoadPSOFTControl(MenuPageLayout.Path, "_pl");;
            PageLayoutControl = pageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout)this.LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting detail parameters
            MenuControl ctrl = (MenuControl)this.LoadPSOFTControl(MenuControl.Path, "_ctrl");
//            ctrl.addMenuItem(null, "help", _mapper.get("navigation","help"), "", "_blank");
            if (Global.Config.showDocumentSearch){
                ctrl.addMenuItem(null, "documentSearch", _mapper.get("navigation","documentSearch"), psoft.Document.DocumentSearch.GetURL());
            }
            //ctrl.addMenuItem(null, "logout", _mapper.get("navigation","logout"), Global.Config.baseURL + "/Basics/default.aspx?logout=true", "_top");

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
