using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Lohn.Controls;
using System;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// Prompt fürs Abschliessen der Lohnmassnahme
    /// </summary>
    public partial class BudgetDoneCheck : PsoftEditPage
    {
        private const string PAGE_URL = "/Lohn/BudgetDoneCheck.aspx";

        static BudgetDoneCheck()
        {
            SetPageParams(PAGE_URL, "backURL", "orgId", "salaryComponent","budgettypId");
        }

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        public BudgetDoneCheck() : base()
        {
            PageURL = PAGE_URL;
        }

        protected void Page_Load(object sender, System.EventArgs e) 
        {
            this.BreadcrumbCaption = _mapper.get("lohn", "terminatePrompt");
            this.BreadcrumbName = "lohnBudgetDoneCheck";

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout
                = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("lohn", "terminatePrompt");

            // Setting content layout of page layout
            DGLContentLayout contentControl
                = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentControl;

            // Setting parameters
            BudgetDoneCheckControl detail
                = (BudgetDoneCheckControl)this.LoadPSOFTControl(BudgetDoneCheckControl.Path, "_detail");
            detail.SalaryComponent = GetQueryValue("salaryComponent", "");
            detail.OrgentityId = GetQueryValue("orgId", (long)-1);
            detail.BackURL = GetQueryValue("backURL", "");
            detail.BudgettypId = GetQueryValue("budgettypId", 0L);

            // Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, detail);	
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
            this.ID = "BudgetDoneCheck";

        }
		#endregion
    }
}
