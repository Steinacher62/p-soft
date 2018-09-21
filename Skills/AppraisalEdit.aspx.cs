using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Skills.Controls;
using System;


namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for AppraisalEdit.
    /// </summary>
    public partial class AppraisalEdit : PsoftTreeViewPage {

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting parameters
            AppraisalEditCtrl aeCtrl = (AppraisalEditCtrl) LoadPSOFTControl(AppraisalEditCtrl.Path, "_ad");
            aeCtrl.AppraisalID = ch.psoft.Util.Validate.GetValid(Request.QueryString["appraisalID"], aeCtrl.AppraisalID);

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, aeCtrl);
	
            DBData db = DBData.getDBData(Session);
            try {
                db.connect();

                if (aeCtrl.AppraisalID > 0) {
                    BreadcrumbCaption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_SKILLSAPPRAISAL);
                    long personID = ch.psoft.Util.Validate.GetValid(db.lookup("PERSON_ID", "SKILLS_APPRAISAL", "ID=" + aeCtrl.AppraisalID, false), -1L);
                    PsoftPageLayout.PageTitle = db.Person.getWholeName(personID) + " - " + _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_SKILLSAPPRAISAL) + " (" + db.lookup("APPRAISALDATE", "SKILLS_APPRAISAL", "ID=" +aeCtrl.AppraisalID) + ")";
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
