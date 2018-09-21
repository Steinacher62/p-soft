using ch.appl.psoft.Common;
using ch.appl.psoft.FBS.Controls;
using ch.appl.psoft.LayoutControls;
using System;


namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for FunctionTree.
    /// </summary>
    public partial class FunctionCatalogTree : PsoftTreeViewPage {
        private long _id = 0;

        protected void Page_Load(object sender, System.EventArgs e) {
            _id = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"],0L);
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("fbs","functionCatalog");
            BreadcrumbCaption = _mapper.get("fbs","functionCatalog");

            // Setting content layout of page layout
            DDGLContentLayout contentLayout = (DDGLContentLayout) LoadPSOFTControl(DDGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            FunctionCatalogTreeCtrl tree = (FunctionCatalogTreeCtrl) LoadPSOFTControl(FunctionCatalogTreeCtrl.Path, "_tree");
            tree.id = _id;
            SetPageLayoutContentControl(DDGLContentLayout.DETAIL_LEFT, tree);

            if (_id > 0) {
                JobDescriptionCtrl detail = (JobDescriptionCtrl)this.LoadPSOFTControl(JobDescriptionCtrl.Path, "_detail");
                SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, detail);
                detail.FunktionID = _id;
            }
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
        }
		#endregion
    }
}
