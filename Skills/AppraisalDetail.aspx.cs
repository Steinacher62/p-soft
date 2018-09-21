using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Skills.Controls;
using System;


namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for AppraisalDetail.
    /// </summary>
    public partial class AppraisalDetail : PsoftTreeViewPage {
        private PsoftLinksControl _links = null;

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting parameters
            AppraisalDetailCtrl adCtrl = (AppraisalDetailCtrl) LoadPSOFTControl(AppraisalDetailCtrl.Path, "_ad");
            adCtrl.AppraisalID = ch.psoft.Util.Validate.GetValid(Request.QueryString["appraisalID"], adCtrl.AppraisalID);
            adCtrl.PersonID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], adCtrl.PersonID);

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, adCtrl);
	
            _links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
            
            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                if (adCtrl.PersonID <= 0){
                    adCtrl.PersonID = ch.psoft.Util.Validate.GetValid(db.lookup("PERSON_ID", "SKILLS_APPRAISAL", "ID=" + adCtrl.AppraisalID, false), -1L);
                }

                if (adCtrl.AppraisalID <= 0) {
                    adCtrl.AppraisalID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILLS_APPRAISAL", "PERSON_ID=" + adCtrl.PersonID + " order by APPRAISALDATE desc", false), -1L);
                }

                bool hasDeleteRight = false;
                bool hasUpdateRight = false;
                bool hasInsertRight = false;
                if (adCtrl.AppraisalID > 0) {
                    BreadcrumbCaption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_SKILLSAPPRAISAL);
                    PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('PrintAppraisal.aspx?appraisalID=" + adCtrl.AppraisalID + "');");
                    PsoftPageLayout.ButtonPrintVisible = true;
                    PsoftPageLayout.PageTitle = db.Person.getWholeName(adCtrl.PersonID) + " - " + _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_SKILLSAPPRAISAL) + " (" + db.lookup("APPRAISALDATE", "SKILLS_APPRAISAL", "ID=" +adCtrl.AppraisalID) + ")";
                    hasUpdateRight = adCtrl.hasAuthorisation(db, DBData.AUTHORISATION.UPDATE);
                    hasInsertRight = adCtrl.hasAuthorisation(db, DBData.AUTHORISATION.INSERT);
                    hasDeleteRight = adCtrl.hasAuthorisation(db, DBData.AUTHORISATION.DELETE);
                    if (hasUpdateRight || hasInsertRight){
                        _links.LinkGroup1.Caption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CMT_SKILLSAPPRAISAL);
                        if (hasUpdateRight){
                            _links.LinkGroup1.AddLink("", _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CM_EDIT_SKILLSAPPRAISAL), "/Skills/AppraisalEdit.aspx?appraisalID=" + adCtrl.AppraisalID);
                        }
                        if (hasInsertRight){
                            _links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CM_NEW_SKILLSAPPRAISAL), "/Skills/AppraisalAdd.aspx?personID=" + adCtrl.PersonID);
                        }
                    }
                }

                if (adCtrl.PersonID > 0){
                    AppraisalListCtrl aList = (AppraisalListCtrl) LoadPSOFTControl(AppraisalListCtrl.Path, "_aList");
                    aList.PersonID=adCtrl.PersonID;
                    aList.AppraisalID = adCtrl.AppraisalID;
                    aList.DetailURL = Global.Config.baseURL + "/Skills/AppraisalDetail.aspx?appraisalID=%ID";
                    aList.DetailEnabled = true;
                    aList.DeleteEnabled = hasDeleteRight;
                    aList.EditEnabled = hasUpdateRight;
                    aList.EditURL = Global.Config.baseURL + "/Skills/AppraisalEdit.aspx?appraisalID=%ID";
                    SetPageLayoutContentControl(DGLContentLayout.GROUP, aList);
                }
            }
            catch (Exception ex) {
                ShowError(ex.Message);
            }
            finally {
                db.disconnect();
            }

            SetPageLayoutContentControl(DGLContentLayout.LINKS, _links);		
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
