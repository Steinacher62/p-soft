using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Training.Controls;
using System;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Training
{
    /// <summary>
    /// Summary description for Advancement.
    /// </summary>
    public partial class TrainingCatalog : PsoftTreeViewPage {

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DDGLContentLayout contentLayout = (DDGLContentLayout) LoadPSOFTControl(DDGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            contentLayout.DetailLeftWidth = Unit.Percentage(40);

            TrainingCatalogTreeCtrl tree = (TrainingCatalogTreeCtrl) LoadPSOFTControl(TrainingCatalogTreeCtrl.Path, "_tree");
            tree.TrainingID = ch.psoft.Util.Validate.GetValid(Request.QueryString["trainingID"], tree.TrainingID);     
            tree.LeafNodeUrl = "TrainingCatalog.aspx?trainingID=%ID";
            tree.PerserveState = 1;

            TrainingCatalogDetailCtrl detail = (TrainingCatalogDetailCtrl) LoadPSOFTControl(TrainingCatalogDetailCtrl.Path, "_training");
            detail.TrainingID = tree.TrainingID;
                  
	
           
            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                if (tree.TrainingID <= 0)
                {
                    tree.TrainingID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "TRAININGGROUP", "PARENT_ID is null and EXTERNAL_REF like 'tg_root'", false), -1L);
                }
                if (tree.TrainingID > 0){
                    PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('PrintTrainingCatalog.aspx?userID=" + db.userId + "');");
                    PsoftPageLayout.ButtonPrintVisible = true;
                    BreadcrumbCaption = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_BC_TRAINING_CATALOG);
                    PsoftPageLayout.PageTitle = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_BC_TRAINING_CATALOG);
                    SetPageLayoutContentControl(DDGLContentLayout.DETAIL_LEFT, tree);
                    SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, detail);
                }

            }
            catch (Exception ex) {
                ShowError(ex.Message);
            }
            finally {
                db.disconnect();
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
