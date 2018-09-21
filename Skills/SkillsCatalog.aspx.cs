using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Skills.Controls;
using System;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Skills
{

    public partial class SkillsCatalog : PsoftTreeViewPage {

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DDGLContentLayout contentLayout = (DDGLContentLayout) LoadPSOFTControl(DDGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            contentLayout.DetailLeftWidth = Unit.Percentage(40);

            SkillsCatalogTreeCtrl tree = (SkillsCatalogTreeCtrl) LoadPSOFTControl(SkillsCatalogTreeCtrl.Path, "_tree");
            tree.SkillValidityID = ch.psoft.Util.Validate.GetValid(Request.QueryString["skillValidityID"], tree.SkillValidityID);     
            tree.LeafNodeUrl = "SkillsCatalog.aspx?skillValidityID=%ID";
            tree.PerserveState = 1;

            SkillsCatalogDetailCtrl detail = (SkillsCatalogDetailCtrl) LoadPSOFTControl(SkillsCatalogDetailCtrl.Path, "_skills");
            detail.SkillValidityID = tree.SkillValidityID;
                  
	
           
            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                if (tree.SkillValidityID <= 0) {
                    tree.SkillValidityID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILLGROUP", "PARENT_ID is null and EXTERNAL_REF like 'sg_root'", false), -1L);
                }
                if (tree.SkillValidityID > 0){
                    PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('PrintSkillsCatalog.aspx?userID=" + db.userId + "');");
                    PsoftPageLayout.ButtonPrintVisible = true;
                    BreadcrumbCaption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_SKILLS_CATALOG);
                    PsoftPageLayout.PageTitle = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_SKILLS_CATALOG);
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
