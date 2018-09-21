using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Lohn.Controls;
using System;
using System.Text;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// Administration der Stati
    /// </summary>
    public partial class Admin : PsoftEditPage
    {
        private const string PAGE_URL = "/Lohn/Admin.aspx";

        static Admin()
        {
            SetPageParams(PAGE_URL, "salaryComponent", "orderColumn", "orderDir");
        }

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        public Admin() : base()
        {
            PageURL = PAGE_URL;
        }

        #region Protected overrided methods from parent class
        protected override void AppendBodyOnLoad(StringBuilder bodyOnLoad)
        {
            base.AppendBodyOnLoad (bodyOnLoad);
            bodyOnLoad.Append("listTabOnLoad();");
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) 
        {
            this.BreadcrumbCaption = _mapper.get("lohn", "admin");
            this.BreadcrumbName = "lohnAdmin";

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout
                = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("lohn", "admin");

            // Setting content layout of page layout
            DGLContentLayout contentControl
                = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentControl;

            // Setting parameters
            AdminControl list = (AdminControl)this.LoadPSOFTControl(AdminControl.Path, "_list");
            list.SalaryComponent = GetQueryValue("salaryComponent", "");
            list.OrderColumn = GetQueryValue("orderColumn", "");
            list.OrderDir = GetQueryValue("orderDir", "asc");

            // Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, list);	
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
            this.ID = "Admin";

        }
		#endregion
    }
}
