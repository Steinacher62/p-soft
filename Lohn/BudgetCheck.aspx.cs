using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Lohn.Controls;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// Page für Eingaben und Übersichten, für beide DLA-Komponenten 'salaire' und 'prime'
    /// </summary>
    public partial class BudgetCheck : PsoftContentPage
    {
        private const string PAGE_URL = "/Lohn/BudgetCheck.aspx";

        static BudgetCheck()
        {
            SetPageParams(
                PAGE_URL, 
                "orgId", 
                "salaryComponent",
                "budgettypId",
                "orderColumn",
                "orderDir"
            );
        }

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        public BudgetCheck() : base()
        {
            PageURL = PAGE_URL;
        }

        #region Protected overrided methods from parent class
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) 
        {
            long budgettypId = GetQueryValue("budgettypId", (long)-1);
            long orgentityId = GetQueryValue("orgId", (long)-1);
            string salaryComponent = GetQueryValue("salaryComponent", "");
            string mnemonic = salaryComponent + "BudgetCheck";

            this.BreadcrumbCaption = _mapper.get(LohnModule.KundenModuleName, mnemonic);
            this.BreadcrumbName = "budgetAllocation" + salaryComponent;

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout
                = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            
            PsoftPageLayout.PageTitle = _mapper.get(LohnModule.KundenModuleName, mnemonic);
            DBData db = DBData.getDBData(Session);
            db.connect();

            try
            {
                string budgettyp = DBColumn.GetValid(
                        db.lookup("BEZEICHNUNG", "BUDGETTYP", "ID = " + budgettypId),
                        ""
                    );

                if (budgettyp != "")
                {
                    PsoftPageLayout.PageTitle += " - " + budgettyp;
                }
            }
            finally
            {
                db.disconnect();
            }

            // Setting content layout of page layout
            DGLContentLayout contentControl
                = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            contentControl.DetailHeight = Unit.Percentage(30);
            PageLayoutControl.ContentLayoutControl = contentControl;

            // Setting parameters
            BudgetCheckControl detail
                = (BudgetCheckControl)this.LoadPSOFTControl(BudgetCheckControl.Path, "_detail");
            detail.OrgentityId = orgentityId;
            detail.SalaryComponent = salaryComponent;
            detail.BudgettypId = budgettypId;

            // Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, detail);	

            // Setting parameters
            BudgetCheckListControl budgetList
                = (BudgetCheckListControl)this.LoadPSOFTControl(BudgetCheckListControl.Path, "_budgetList");
            budgetList.SalaryComponent = salaryComponent;
            budgetList.OrgentityId = orgentityId;
            budgetList.BudgettypId = budgettypId;
            budgetList.OrderColumn = GetQueryValue("orderColumn", "");
            budgetList.OrderDir = GetQueryValue("orderDir", "");

            // Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.GROUP, budgetList);	
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
            this.ID = "BudgetCheck";
        }
		#endregion
    }
}
