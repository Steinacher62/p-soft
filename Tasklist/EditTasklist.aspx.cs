using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Tasklist.Controls;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Summary description for Search.
    /// </summary>
    public partial class EditTasklist : PsoftEditPage
    {
		private const string PAGE_URL = "/Tasklist/EditTasklist.aspx";

		static EditTasklist()
		{
			SetPageParams(PAGE_URL, "backURL", "ID");
		}

		public static string GetURL(params object[] queryParams)
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		public EditTasklist() : base()
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
            long id = GetQueryValue("ID", (long)-1);
            bool template = false;
            DBData db = DBData.getDBData(Session);
            db.connect();
            
            try
            {
                template = db.lookup("TEMPLATE", "TASKLIST", "ID=" + id, false) == "1";
            }
            catch(Exception ex) 
            {
                Logger.Log(ex, Logger.ERROR);
            }
            finally
            {
                db.disconnect();
            }

            string title = _mapper.get("tasklist", template ? "edittasklisttemplate" : "edittasklist");
            this.BreadcrumbCaption = title;

			// Setting main page layout
			PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            (PageLayoutControl as PsoftPageLayout).PageTitle = title;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			PendenzenTaskEdit _detail = (PendenzenTaskEdit)this.LoadPSOFTControl(PendenzenTaskEdit.Path, "_detail");
			_detail.Template = template;
			_detail.BackUrl = GetQueryValue("backURL", "");
			_detail.id = id;

			// Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, _detail);		
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
        private void InitializeComponent() {    
			this.ID = "Edit";

		}
		#endregion
    }
}
