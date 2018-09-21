using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.FBS.Controls;
using ch.appl.psoft.LayoutControls;
using System;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.FBS
{

    public partial class DutyCatalog : PsoftTreeViewPage {

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DDGLContentLayout contentLayout = (DDGLContentLayout) LoadPSOFTControl(DDGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            contentLayout.DetailLeftWidth = Unit.Percentage(40);

            DutyCatalogTreeCtrl tree = (DutyCatalogTreeCtrl) LoadPSOFTControl(DutyCatalogTreeCtrl.Path, "_tree");
            tree.DutyValidityID = ch.psoft.Util.Validate.GetValid(Request.QueryString["dutyValidityID"], tree.DutyValidityID);     
            tree.LeafNodeUrl = "DutyCatalog.aspx?dutyValidityID=%ID";
            tree.PerserveState = 1;

            DutyCatalogDetailCtrl detail = (DutyCatalogDetailCtrl) LoadPSOFTControl(DutyCatalogDetailCtrl.Path, "_duty");
            detail.DutyValidityID = tree.DutyValidityID;
           
            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                if (tree.DutyValidityID <= 0) {
                    tree.DutyValidityID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "DUTYGROUP", "PARENT_ID is null and EXTERNAL_REF like 'dg_root'", false), -1L);
                }
                if (tree.DutyValidityID > 0){
                    PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('PrintDutyCatalog.aspx?userID=" + db.userId + "');");
                    PsoftPageLayout.ButtonPrintVisible = true;
//                    PsoftPageLayout.ButtonExcelAttributes.Add("onClick", "javascript: window.open('PrintMatrixDutyCatalog.aspx?userID=" + db.userId + "');");
//                    PsoftPageLayout.ButtonExcelVisible = true;
                    BreadcrumbCaption = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_BC_DUTY_CATALOG);
                    PsoftPageLayout.PageTitle = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_BC_DUTY_CATALOG);
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
