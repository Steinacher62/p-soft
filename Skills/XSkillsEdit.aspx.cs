using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Skills.Controls;
using System;


namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for XSkillsEdit.
    /// </summary>
    public partial class XSkillsEdit : PsoftTreeViewPage {
        private PsoftLinksControl _links = null;
        protected bool _actionNew = false;

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting parameters
            XSkillsEditCtrl xsCtrl = (XSkillsEditCtrl) LoadPSOFTControl(XSkillsEditCtrl.Path, "_xSkillsEdit");
            xsCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], xsCtrl.JobID);
            xsCtrl.PersonID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], xsCtrl.PersonID);
            xsCtrl.SkillID = ch.psoft.Util.Validate.GetValid(Request.QueryString["skillID"], xsCtrl.SkillID);
            xsCtrl.SkillLevelValidityID = ch.psoft.Util.Validate.GetValid(Request.QueryString["skillLevelValidityID"], xsCtrl.SkillLevelValidityID);

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, xsCtrl);
	
            _actionNew = ch.psoft.Util.Validate.GetValid(Request.QueryString["action"], "").ToLower() == "new";

            _links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
            
            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                if (xsCtrl.JobID > 0){
                    BreadcrumbCaption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_EDIT_JOBSKILLS);
                    _links.LinkGroup1.Caption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CMT_JOBSKILLS);
                    PsoftPageLayout.PageTitle = db.lookup("TITLE", "JOBPERSONV", "ID=" + xsCtrl.JobID, false);
                }
                else if (xsCtrl.PersonID > 0){
                    BreadcrumbCaption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_EDIT_ACTUALSKILLS);
                    _links.LinkGroup1.Caption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CMT_ACUTALSKILLS);
                    PsoftPageLayout.PageTitle = db.Person.getWholeName(xsCtrl.PersonID) + " - " + _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_ACTUALSKILLS);
                }

                if (xsCtrl.SkillID <= 0)
                    xsCtrl.SkillID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILLGROUPTREEV", "PARENT_ID is null", false), -1);

                if (xsCtrl.SkillID > 0) {
                    if (xsCtrl.SkillLevelValidityID <= 0 && !_actionNew) {
                        if (xsCtrl.JobID > 0){
                            xsCtrl.SkillLevelValidityID = getJobSkillLevelValidityID(db, xsCtrl.SkillID, xsCtrl.JobID);
                        }
                        else if (xsCtrl.PersonID > 0){
                            xsCtrl.SkillLevelValidityID = getActualSkillLevelValidityID(db, xsCtrl.SkillID, xsCtrl.PersonID);
                        }
                    }
                    if (xsCtrl.SkillLevelValidityID > 0) {
                        SkillLevelValidityList slvList = (SkillLevelValidityList) LoadPSOFTControl(SkillLevelValidityList.Path, "_slvList");
                        slvList.SkillID = xsCtrl.SkillID;
                        slvList.JobID = xsCtrl.JobID;
                        slvList.PersonID = xsCtrl.PersonID;
                        slvList.SkillLevelValidityID = xsCtrl.SkillLevelValidityID;
                        slvList.DetailURL = Global.Config.baseURL + "/Skills/XSkillsEdit.aspx?skillLevelValidityID=%ID&personID=" + xsCtrl.PersonID + "&jobID=" + xsCtrl.JobID + "&skillID=" + xsCtrl.SkillID;
                        slvList.DetailEnabled = true;
                        SetPageLayoutContentControl(DGLContentLayout.GROUP, slvList);
                        _links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CM_NEW_SKILL_LEVEL), "/Skills/XSkillsEdit.aspx?personID=" + xsCtrl.PersonID + "&jobID=" + xsCtrl.JobID + "&skillID=" + xsCtrl.SkillID + "&action=new");
                    }
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

        public static long getJobSkillLevelValidityID(DBData db, long skillID, long jobID) {
            long retValue = -1;
            long funktionID = ch.psoft.Util.Validate.GetValid(db.lookup("FUNKTION_ID", "JOB", "ID=" + jobID, false), -1);
            retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and VALID_FROM<=GetDate() and VALID_TO>=GetDate() and FUNKTION_ID=" + funktionID, false), -1);
            if (retValue <= 0) {
                // if no function-skill get job-skill
                retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and VALID_FROM<=GetDate() and VALID_TO>=GetDate() and JOB_ID=" + jobID, false), -1);
                if (retValue <= 0) {
                    // if no skill-level specified, get first job-skill
                    retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and JOB_ID=" + jobID + " order by VALID_FROM asc", false), -1);
                    if (retValue <= 0) {
                        // if no first job-skill get first function-skill
                        retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and FUNKTION_ID=" + funktionID + " order by VALID_FROM asc", false), -1);
                    }
                }
            }
            
            return retValue;
        }

        public static long getActualSkillLevelValidityID(DBData db, long skillID, long personID) {
            long retValue = -1;
            retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and VALID_FROM<=GetDate() and VALID_TO>=GetDate() and PERSON_ID=" + personID, false), -1);
            if (retValue <= 0) {
                // if no actual skill, get first actual-skill
                retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and PERSON_ID=" + personID + " order by VALID_FROM asc", false), -1);
            }
            
            return retValue;
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
