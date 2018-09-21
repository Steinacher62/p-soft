using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Lohn.Controls;
using System;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// Import- und Exportmenü
    /// </summary>
    public partial class ImportExport : PsoftEditPage
    {
        private const string PAGE_URL = "/Lohn/ImportExport.aspx";

        static ImportExport()
        {
            SetPageParams(PAGE_URL);
        }

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        public ImportExport() : base()
        {
            PageURL = PAGE_URL;
        }

        protected void Page_Load(object sender, System.EventArgs e) 
        {
            this.BreadcrumbCaption = _mapper.get("lohn", "importExport");
            this.BreadcrumbName = "lohnImportExport";

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout
                = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("lohn", "importExport");

            // Setting content layout of page layout
            DGLContentLayout contentControl
                = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentControl;

            // Setting parameters
            ImportExportControl detail
                = (ImportExportControl)this.LoadPSOFTControl(ImportExportControl.Path, "_detail");

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
            this.ID = "ImportExport";

        }
		#endregion
    }
}
