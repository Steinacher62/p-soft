using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Tasklist.Controls;
using System;

namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Summary description for Search.
    /// </summary>
    public partial class AddTasklist : PsoftEditPage
	{
		private const string PAGE_URL = "/Tasklist/AddTasklist.aspx";

		static AddTasklist()
		{
			SetPageParams(
				PAGE_URL,
				"backURL",
				"nextURL",
				"parentId",
				"ownerTable",
				"ownerID",
				"type",
				"triggerUID",
                "template"
			);
		}

		public static string GetURL(params object[] queryParams)
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		public AddTasklist() : base()
		{
			PageURL = PAGE_URL;
		}
       
		#region Protected overrided methods from parent class
		protected override void Initialize()
		{
			// base initialize
			base.Initialize();
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.BreadcrumbCaption = _mapper.get("tasklist","newtasklist");

			// Setting main page layout
			PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            (PageLayoutControl as PsoftPageLayout).PageTitle = _mapper.get("tasklist","newtasklist");

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			PendenzenTaskAdd _detail = (PendenzenTaskAdd)this.LoadPSOFTControl(PendenzenTaskAdd.Path, "_detail");
			_detail.BackUrl = GetQueryValue("backURL", "");
            _detail.NextUrl = GetQueryValue("nextURL", "");
            _detail.ParentTasklist = GetQueryValue("parentId", (long)0);
            _detail.OwnerTable = GetQueryValue("ownerTable", "");
            _detail.OwnerID = GetQueryValue("ownerID", (long)-1);
            _detail.Typ = GetQueryValue("type", Interface.DBObjects.Tasklist.TYPE_PUBLIC);
            _detail.TriggerUID = GetQueryValue("triggerUID", (long)0);
            _detail.Template = Boolean.Parse(GetQueryValue("template", "false"));

			// Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, _detail);		
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
			this.ID = "Add";

		}
		#endregion

	}
}
