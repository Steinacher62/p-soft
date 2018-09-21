using ch.appl.psoft.Common;
using ch.appl.psoft.Energiedienst.Controls;
using ch.appl.psoft.LayoutControls;
using System;



namespace ch.appl.psoft.Energiedienst
{
    /// <summary>
    /// Summary description for PrintJobDescription.
    /// </summary>
    public partial class PrintJobDescription : PsoftDetailPage
    {
        protected int _jobID = -1;
        protected long _employment_ID = -1;
        protected long _personId = -1;
        protected string _onloadString;
        protected long _funktionID = -1;
        protected int _groupNumber = 0;
        protected string _reportDate = DateTime.Now.ToString("d");
        protected bool isFirstGrp = true;
        protected string tmpSqltableName;

        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");

            // Setting breadcrumb caption
            BreadcrumbCaption = "Aufgabenbeschreibung";

            // Setting page-title
            ((PsoftPageLayout)PageLayoutControl).PageTitle = "Aufgabenbeschreibung";

            //Load and Setting content layout user controls
            PrintJobDescriptionCtrl pjdCtrl = (PrintJobDescriptionCtrl)this.LoadPSOFTControl(Global.Config.baseURL + "/Energiedienst/Controls/PrintJobDescriptionCtrl.ascx", "_jobDescPrint");
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, pjdCtrl);
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
