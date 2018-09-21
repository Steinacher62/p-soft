using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;

namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for FBSModule.
    /// </summary>
    public class FBSModule : psoftModule {
        public const string LANG_SCOPE_FBS                       = "fbs";

        //breadcrumb captions
        public const string LANG_MNEMO_BC_JOBDESCRIPTION         = "bcJobDescription";
        public const string LANG_MNEMO_BC_EDIT_JOBDESCRIPTION    = "bcEditJobDescription";
        public const string LANG_MNEMO_BC_DUTY_CATALOG           = "bcDutyCatalog";
        public const string LANG_MNEMO_BC_ADD_DUTY_COMPETENCE    = "bcAddDutyCompetence";
        public const string LANG_MNEMO_BC_ADD_DUTY_COMPETENCE_FREE    = "bcAddDutyCompetenceFree";
        public const string LANG_MNEMO_BC_EDIT_DUTY_COMPETENCE   = "bcEditDutyCompetence";
        public const string LANG_MNEMO_BC_EDIT_DUTY_FREE         = "bcEditDutyFree";

        //controls title
        public const string LANG_MNEMO_CT_DUTYGROUP_TREE         = "ctDutyGroupTree";
        public const string LANG_MNEMO_CT_DUTY_TREE              = "ctDutyTree";
        public const string LANG_MNEMO_CT_COMPETENCE_LIST        = "ctCompetenceList";
        public const string LANG_MNEMO_CT_COMPETENCES            = "ctCompetences";
        public const string LANG_MNEMO_CT_DCV_LIST               = "ctDCVList";
        public const string LANG_MNEMO_CT_DUTYCATALOG_D_DETAIL   = "ctDutyCatalogDutyDetail";
        public const string LANG_MNEMO_CT_COMPETENCE             = "ctCompetence";
        public const string LANG_MNEMO_CT_DUTY                   = "ctDuty";

        //context-menus
        public const string LANG_MNEMO_CM_EDIT_JOBDESCRIPTION    = "cmEditJobDescription";
        public const string LANG_MNEMO_CM_NEW_DUTY_COMPETENCE    = "cmNewDutyCompetence";
        public const string LANG_MNEMO_CM_ADD_DUTY_COMPETENCE    = "cmAddDutyCompetence";
        public const string LANG_MNEMO_CM_ADD_DUTY_COMPETENCE_FREE    = "cmAddDutyCompetenceFree";
        public const string LANG_MNEMO_CM_EDIT_DUTY_FREE      = "cmEditDutyFree";

        //context-menu title
        public const string LANG_MNEMO_CMT_JOBDESCRIPTION        = "cmtJobDescription";

        //report
        public const string LANG_MNEMO_REP_JOBDESCRIPTION        = "repJobDescription";
        public const string LANG_MNEMO_REP_COMPETENCES           = "repCompetences";
        public const string LANG_MNEMO_REP_DUTY                  = "repDuty";
        public const string LANG_MNEMO_REP_COMPETENCES_RULES     = "repCompetencesRules";
        public const string LANG_MNEMO_REP_JOB_OWNER             = "repJobOwner";
        public const string LANG_MNEMO_REP_DUTYCATALOG           = "repDutyCatalog";
        public const string LANG_MNEMO_REP_DUTYGROUP_FREE        = "repDutyGroupFree";

        //checkbox
        public const string LANG_MNEMO_SHOW_VALID_ONLY           = "cbShowValidOnly";

        public FBSModule() : base() {
            m_NameMnemonic = "fbs";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(
                AppDomain.CurrentDomain.BaseDirectory.ToString()
                    + "FBS/XML/language_" + languageCode + ".xml",
                languageCode,
                false
            );
        }

        public static DataTable getCompetenceLevels(DBData db) {
            return db.getDataTable("select ID, " + db.langAttrName("COMPETENCE_LEVEL", "TITLE") + ", " + db.langAttrName("COMPETENCE_LEVEL", "MNEMO") + " from COMPETENCE_LEVEL order by NUMBER", "COMPETENCE_LEVEL");
        }

        public static bool showNumTitleInReport{
            get { return Global.Config.getModuleParam("fbs", "showNumTitleInReport", "1") == "1"; }
        }
    }
}
