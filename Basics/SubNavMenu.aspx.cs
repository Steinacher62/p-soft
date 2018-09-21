using ch.appl.psoft.Common;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Basics
{
    /// <summary>
    /// Summary description for navigation.
    /// </summary>
    public partial class SubNavMenu : PsoftMenuPage
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
            ctrl.Title = _mapper.get("navigation", "home");
            ctrl.TitleLink = Global.Config.baseURL + "/";
            ctrl.TitleTarget = "_top";
            if (Global.Config.ShowNews){
                ctrl.addMenuItem(null, "news", _mapper.get("news","news"), psoft.Basics.contentInit.GetURL("context",News.CONTEXT.NEWS));
                ctrl.addMenuItem(null, "subscriptions", _mapper.get("news","subscriptions"), psoft.Basics.contentInit.GetURL("context",News.CONTEXT.SUBSCRIPTION));
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
