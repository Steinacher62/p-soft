using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Lohn.Controls;
using System;
using System.Text;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// Anzeigen der unterstellten OE's für die Genehmigung der Anpassungen
    /// </summary>
    public partial class Approvement : PsoftContentPage
    {
        private const string PAGE_URL = "/Lohn/Approvement.aspx";

        static Approvement()
        {
            SetPageParams(PAGE_URL, "orgId", "salaryComponent", "action");
        }

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        public Approvement() : base()
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
            long orgentityId = GetQueryValue("orgId", (long)-1);
            string salaryComponent = GetQueryValue("salaryComponent", "");
            string action = GetQueryValue("action", "");
            string mnemonic = salaryComponent + "Approvement";

            base.ShowProgressBar = false;
            this.BreadcrumbCaption = _mapper.get(LohnModule.KundenModuleName, mnemonic);
            this.BreadcrumbName = "lohnApprovement" + salaryComponent;

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout
                = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            
            PsoftPageLayout.PageTitle = _mapper.get(LohnModule.KundenModuleName, mnemonic);
            DBData db = DBData.getDBData(Session);
            db.connect();

            try
            {
                string titleAttribute = db.langAttrName("ORGENTITY", "TITLE");
                string oeTitle = DBColumn.GetValid(
                        db.lookup(titleAttribute, "ORGENTITY","ID = " + orgentityId),
                        ""
                    );

                if (oeTitle != "")
                {
                    PsoftPageLayout.PageTitle += " - " + oeTitle;
                }
            }
            finally
            {
                db.disconnect();
            }

            // Setting content layout of page layout
            DGLContentLayout contentControl
                = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentControl;

            // Setting parameters
            ApprovementControl list
                = (ApprovementControl)this.LoadPSOFTControl(ApprovementControl.Path, "_detail");
            list.OrgentityId = orgentityId;
            list.SalaryComponent = salaryComponent;
            list.Action = action;

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
