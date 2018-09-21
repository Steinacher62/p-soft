using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Lohn.Controls;
using System;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// Anzeigen der OE's für den Aufruf von SalaryAdjustment.aspx.
    /// Wird im ersten Schritt nur angezeigt, wenn es mehrere sind.
    /// Erst innerhalb von SalaryAdjustment.aspx wird die Liste auch mit nur einer OE
    /// angezeigt.
    /// </summary>
    public partial class Main : PsoftContentPage
    {
        private const string PAGE_URL = "/Lohn/Main.aspx";

        static Main()
        {
            SetPageParams(PAGE_URL, "salaryComponent", "budgettypId", "context");
        }

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        public Main() : base()
        {
            PageURL = PAGE_URL;
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.BreadcrumbCaption = _mapper.get("lohn", "division");
            this.BreadcrumbName = "lohnMain";

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout
                = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("lohn", "division");

            // Setting content layout of page layout
            DGLContentLayout contentControl
                = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentControl;

            // Setting parameters
            MainControl list
                = (MainControl)this.LoadPSOFTControl(MainControl.Path, "_detail");
            list.ParentId = -1;
            list.Kontext = GetQueryValue("context", "").ToLower();
            list.SalaryComponent = GetQueryValue("salaryComponent", "");
            list.BudgettypId = GetQueryValue("budgettypId", (long)-1);

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
            this.ID = "lohnMain";
        }
		#endregion
    }
}
