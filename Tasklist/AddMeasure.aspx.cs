using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Tasklist.Controls;
using System;

namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Summary description for Search.
    /// </summary>
    public partial class AddMeasure : PsoftEditPage
	{
		private const string PAGE_URL = "/Tasklist/AddMeasure.aspx";

		static AddMeasure()
		{
			SetPageParams(PAGE_URL, "backURL", "assignPerson", "tasklistID", "triggerUID", "template", "hideTasklist");
		}

		public static string GetURL(params object[] queryParams)
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		public AddMeasure() : base()
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

        protected void Page_Load(object sender, System.EventArgs e) {
            this.BreadcrumbCaption = _mapper.get("tasklist","newmeasure");

            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            (PageLayoutControl as PsoftPageLayout).PageTitle = _mapper.get("tasklist", "newmeasure");

            // Setting content layout of page layout
            DGLContentLayout contentControl = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentControl;

			// Setting parameters
			PendenzenMeasureAdd _detail = (PendenzenMeasureAdd)this.LoadPSOFTControl(PendenzenMeasureAdd.Path, "_detail");
			_detail.BackUrl = GetQueryValue("backURL", "");
            _detail.AssignPerson = GetQueryValue("assignPerson", "enable") == "enable";
            _detail.TaskListId = GetQueryValue("tasklistID", (long)0);
            _detail.TriggerUID = GetQueryValue("triggerUID", (long)0);
            _detail.Template = Boolean.Parse(GetQueryValue("template", "false"));
			_detail.HideTasklist = Boolean.Parse(GetQueryValue("hideTasklist", "false"));

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
			this.ID = "Edit";

		}
		#endregion
	}
}