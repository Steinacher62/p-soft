using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Summary description for SubNavMenu.
    /// </summary>
    /// <param name="colorItem">context for the sub-menu.</param>
    public partial class SubNavMenu : PsoftMenuPage
    {
        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            MenuPageLayout pageLayout = (MenuPageLayout) LoadPSOFTControl(MenuPageLayout.Path, "_pl");;
            PageLayoutControl = pageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout)this.LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting detail parameters
            MenuControl ctrl = (MenuControl)this.LoadPSOFTControl(MenuControl.Path, "_ctrl");
            ctrl.Title = _mapper.get("tasklist","tasklist");
            ctrl.addMenuItem(
				null,
				"searchTasklist",
				_mapper.get("tasklist","searchtasklist"),
				psoft.Tasklist.Search.GetURL("base", "tasklist")
			);
            ctrl.addMenuItem(
				null,
				"searchMeasure",
				_mapper.get("tasklist","searchmeasure"),
				psoft.Tasklist.Search.GetURL("base", "measure")
			);
            ctrl.addMenuItem(
				null,
				"newTasklist",
				_mapper.get("tasklist","newtasklist"),
				psoft.Tasklist.AddTasklist.GetURL()
			);
            ctrl.addMenuItem(
				null,
				"searchTasklistTemplate",
				_mapper.get("tasklist","searchtasklisttemplate"),
				psoft.Tasklist.Search.GetURL("base", "tasklist", "template", "true")
			);

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
