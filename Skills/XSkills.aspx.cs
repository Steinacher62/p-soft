using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Skills.Controls;
using System;


namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for XSkills.
    /// </summary>
    public partial class XSkills : PsoftDetailPage {
        private PsoftLinksControl _links = null;

        public XSkills() : base() {
            ShowProgressBar = false;
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting parameters
            XSkillsCtrl xsCtrl = (XSkillsCtrl) LoadPSOFTControl(XSkillsCtrl.Path, "_xSkills");
            xsCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], xsCtrl.JobID);
            xsCtrl.PersonID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], xsCtrl.PersonID);

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, xsCtrl);
	
            _links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
            
            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                if (xsCtrl.JobID > 0) {
                    BreadcrumbCaption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_JOBSKILLS);
                    _links.LinkGroup1.Caption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CMT_JOBSKILLS);
                    string printPage = "PrintXSkills";

                    if (Global.isModuleEnabled("spz"))
                    {
                        // Zusammengefasster Report. Nur mit JobID, nicht mit PersonID
                        printPage = "../SPZ/PrintJobDescription";
                    }

                    PsoftPageLayout.ButtonPrintAttributes.Add(
                        "onClick",
                        "javascript: window.open('" + printPage+ ".aspx?jobID=" + xsCtrl.JobID + "');"
                    );
                    PsoftPageLayout.ButtonPrintVisible = true;
                    PsoftPageLayout.PageTitle = db.lookup("TITLE", "JOBPERSONV", "ID=" + xsCtrl.JobID, false) + " - " + _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_JOBSKILLS);
                    if (xsCtrl.hasAuthorisation(db, SkillsModule.JSKILL, DBData.AUTHORISATION.INSERT))
                    {
                        _links.LinkGroup1.AddLink("", _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CM_ADD_SKILL_LEVEL),"/Skills/AddSkillLevelValidity.aspx?jobID=" + xsCtrl.JobID);
                        _links.LinkGroup1.AddLink("", _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CM_ADD_SKILL_LEVEL_FREE),"/Skills/AddSkillLevelValidityHandsFree.aspx?jobID=" + xsCtrl.JobID);
                    }
                }
                else if (xsCtrl.PersonID > 0){
                    BreadcrumbCaption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_ACTUALSKILLS);
                    _links.LinkGroup1.Caption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CMT_ACUTALSKILLS);
                    PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('PrintXSkills.aspx?personID=" + xsCtrl.PersonID + "');");
                    PsoftPageLayout.ButtonPrintVisible = true;
                    PsoftPageLayout.PageTitle = db.Person.getWholeName(xsCtrl.PersonID) + " - " + _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_ACTUALSKILLS);

                    if (xsCtrl.hasAuthorisation(db, SkillsModule.PSKILL, DBData.AUTHORISATION.INSERT))
                    {
                        _links.LinkGroup1.AddLink("", _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CM_ADD_SKILL_LEVEL),"/Skills/AddSkillLevelValidity.aspx?personID=" + xsCtrl.PersonID);
                        _links.LinkGroup1.AddLink("", _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CM_ADD_SKILL_LEVEL_FREE),"/Skills/AddSkillLevelValidityHandsFree.aspx?personID=" + xsCtrl.PersonID);
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
