using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;

namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for SkillsModule.
    /// </summary>
    public class SkillsModule : psoftModule {
        public const string LANG_SCOPE_SKILLS                  = "skills";

        //breadcrumb captions
        public const string LANG_MNEMO_BC_JOBSKILLS            = "bcJobSkills";
        public const string LANG_MNEMO_BC_EDIT_JOBSKILLS       = "bcEditJobSkills";
        public const string LANG_MNEMO_BC_ACTUALSKILLS         = "bcActualSkills";
        public const string LANG_MNEMO_BC_EDIT_ACTUALSKILLS    = "bcEditActualSkills";
        public const string LANG_MNEMO_BC_SKILLSAPPRAISAL      = "bcSkillsAppraisal";
        public const string LANG_MNEMO_BC_SKILLS_CATALOG       = "bcSkillsCatalog";
        public const string LANG_MNEMO_BC_ADD_SKILL_LEVEL      = "bcAddSkillLevel";
        public const string LANG_MNEMO_BC_ADD_SKILL_LEVEL_FREE = "bcAddSkillLevelFree";
        public const string LANG_MNEMO_BC_EDIT_SKILL_LEVEL     = "bcEditSkillLevel";
        public const string LANG_MNEMO_BC_EDIT_SKILL_FREE      = "bcEditSkillFree";


        //controls title
        public const string LANG_MNEMO_CT_SKILLGROUP_TREE      = "ctSkillGroupTree";
        public const string LANG_MNEMO_CT_SKILL_TREE           = "ctSkillTree";
        public const string LANG_MNEMO_CT_DEMAND_LEVEL_LIST    = "ctDemandLevelList";
        public const string LANG_MNEMO_CT_DEMAND_LEVEL         = "ctDemandLevel";
        public const string LANG_MNEMO_CT_SLV_LIST             = "ctSLVList";
        public const string LANG_MNEMO_CT_APPRAISAL_LIST       = "ctAppraisalList";
        public const string LANG_MNEMO_CT_SKILLRATING_LIST     = "ctSkillRatingList";
        public const string LANG_MNEMO_CT_SKILLSCATALOG_S_DETAIL = "ctSkillsCatalogSkillDetail";
        public const string LANG_MNEMO_CT_SKILL               = "ctSkill";

        //context-menus
        public const string LANG_MNEMO_CM_EDIT_JOBSKILLS       = "cmEditJobSkills";
        public const string LANG_MNEMO_CM_EDIT_ACTUALSKILLS    = "cmEditActualSkills";
        public const string LANG_MNEMO_CM_NEW_SKILL_LEVEL      = "cmNewSkillLevel";
        public const string LANG_MNEMO_CM_EDIT_SKILLSAPPRAISAL = "cmEditSkillsAppraisal";
        public const string LANG_MNEMO_CM_NEW_SKILLSAPPRAISAL  = "cmNewSkillsAppraisal";
        public const string LANG_MNEMO_CM_ADD_SKILL_LEVEL      = "cmAddSkillLevel";
        public const string LANG_MNEMO_CM_ADD_SKILL_LEVEL_FREE = "cmAddSkillLevelFree";
        public const string LANG_MNEMO_CM_EDIT_SKILL_FREE      = "cmEditSkillFree";

        //context-menu title
        public const string LANG_MNEMO_CMT_JOBSKILLS           = "cmtJobSkills";
        public const string LANG_MNEMO_CMT_ACUTALSKILLS        = "cmtActualSkills";
        public const string LANG_MNEMO_CMT_SKILLSAPPRAISAL     = "cmtSkillsAppraisal";
        public const string LANG_MNEMO_CMT_NEWSKILLSAPPRAISAL  = "cmtNewSkillsAppraisal";

        //report
        public const string LANG_MNEMO_REP_JOBSKILLS           = "repJobSkills";
        public const string LANG_MNEMO_REP_PERSONSKILLS        = "repPersonSkills";
        public const string LANG_MNEMO_REP_DEMANDLEVEL         = "repDemandLevel";
        public const string LANG_MNEMO_REP_SKILL               = "repSkill";
        public const string LANG_MNEMO_REP_SKILLSAPPRAISAL     = "repSkillsAppraisal";
        public const string LANG_MNEMO_REP_RATINGLEVEL         = "repRatingLevel";
        public const string LANG_MNEMO_REP_JOB_OWNER           = "repJobOwner";
        public const string LANG_MNEMO_REP_PERSON              = "repPerson";
        public const string LANG_MNEMO_REP_OVERALL_RATING      = "repOverallRating";
        public const string LANG_MNEMO_REP_SKILLSCATALOG       = "repSkillsCatalog";
        public const string LANG_MNEMO_REP_SKILLGROUP_FREE     = "repSkillGroupFree";

        //checkbox
        public const string LANG_MNEMO_SHOW_VALID_ONLY         = "cbShowValidOnly";

        //skill types
        public const int JSKILL = 0;
        public const int PSKILL = 1;
        public const int FSKILL = 2;

        public SkillsModule() : base() {
            m_NameMnemonic = "skills";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Skills/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public static DataTable getDemandLevels(DBData db) {
            return db.getDataTable("select ID, " + db.langAttrName("DEMAND_LEVEL", "TITLE") + " from DEMAND_LEVEL order by NUMBER", "DEMAND_LEVEL");
        }

        public static DataTable getRatingLevels(DBData db) {
            return db.getDataTable("select ID, " + db.langAttrName("RATING_LEVEL", "TITLE") + ", PERCENTAGE from RATING_LEVEL order by PERCENTAGE", "RATING_LEVEL");
        }
 
        public static bool showNumTitleInReport{
            get { return Global.Config.getModuleParam("skills", "showNumTitleInReport", "1") == "1"; }
        }
    }
}
