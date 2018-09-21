using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Training
{
    /// <summary>
    /// Summary description for TrainingModule.
    /// </summary>
    public class TrainingModule : psoftModule {
        public const string LANG_SCOPE_TRAINING                  = "training";

        public const string LANG_MNEMO_SUBNAV_TITLE              = "subnavTitle";
        public const string LANG_MNEMO_SUBNAV_SEARCH             = "subnavSearch";

        //breadcrumb captions
        public const string LANG_MNEMO_BC_ADVANCEMENT            = "bcAdvancement";
        public const string LANG_MNEMO_BC_EDIT_ADVANCEMENT       = "bcEditAdvancement";
        public const string LANG_MNEMO_BC_ADD_ADVANCEMENT        = "bcAddAdvancement";
        public const string LANG_MNEMO_BC_SEARCH_ADVANCEMENT     = "bcSearchAdvancement";
        public const string LANG_MNEMO_BC_TRAINING_CATALOG       = "bcTrainingCatalog";

        //controls title
        public const string LANG_MNEMO_CT_TRAININGGROUP_TREE     = "ctTrainingGroupTree";
        public const string LANG_MENMO_CT_ADVANCEMENT_LIST       = "ctAdvancementList";
        public const string LANG_MNEMO_CT_ADVANCEMENT_DETAIL     = "ctAdvancementDetail";
        public const string LANG_MNEMO_CT_TRAINING_DETAIL        = "ctTrainingDetail";
        public const string LANG_MNEMO_CT_ADVANCEMENT_SEARCH_LIST = "ctAdvancementSearchList";
        public const string LANG_MNEMO_CT_TRAININGCATALOG_T_DETAIL  = "ctTrainingCatalogTrainingDetail";
        public const string LANG_MNEMO_CT_TRAININGCATALOG_G_DETAIL  = "ctTrainingCatalogGroupDetail";

        //context-menus
        public const string LANG_MENMO_CM_ADVANCEMENT         = "cmAdvancment";
        public const string LANG_MENMO_CM_EDIT_ADVANCEMENT    = "cmEditAdvancment";
        public const string LANG_MENMO_CM_ADD_ADVANCEMENT     = "cmAddAdvancement";
        public const string LANG_MENMO_CM_DELETE_ADVANCEMENT  = "cmDeleteAdvancement";

        //context-menu title
        public const string LANG_MNEMO_CMT_TRAINING           = "cmtTraining";

        //flag
        public const string LANG_MNEMO_FLAG_ADVANCEMENT_INHERIT_TREE = "flagAdvancementInheritTree";
        public const string LANG_MNEMO_SHOW_DONE_ADVANCEMENT = "flagShowDoneAdvancement";

        //state
        public const string LANG_MNEMO_STATE = "state";

        //labels
        public const string LANG_MNEMO_LBL_SUMMARY_TITLE            = "lblSummaryTitle";
        public const string LANG_MNEMO_LBL_COST_CURRENCY            = "lblCostCurrancy";
        public const string LANG_MNEMO_LBL_COST_INTERN              = "lblCostIntern";
        public const string LANG_MNEMO_LBL_COST_EXTERN              = "lblCostExtern";
        
        //report
        public const string LANG_MNEMO_REP_PERSONTRAINING           = "repPersonTraining";
        public const string LANG_MNEMO_REP_ADVACEMENT               = "repAdvancement";
        public const string LANG_MNEMO_REP_STANDALONE_ADVACEMENT    = "repStandaloneAdvancement";
        public const string LANG_MNEMO_REP_TRAININGCATALOG          = "repTrainingCatalog";
        public const string LANG_MNEMO_REP_CONTROLLING              = "repControlling";

        public TrainingModule() : base() {
            m_NameMnemonic = "training";
//            m_StartURL = "../Training/Search.aspx";
            m_SubNavMenuURL = "../Training/SubNavMenu.aspx";
            m_IsVisible = false;
        }

        public override void OnAfterLogin(HttpSessionState session){
            DBData db = DBData.getDBData(session);
            db.Training.refreshCacheEntriesAsynchronous();
        }
            
        public override bool IsVisible(HttpSessionState session)
        {
            // immer unsichtbar wegen neuem Menü
            return false;
            
            DBData db = DBData.getDBData(session);
            if (db.Training.hasTrainingPersons())
            {
                m_IsVisible = true;
            }
            else 
            {
                m_IsVisible = false;
            }
            return m_IsVisible;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Training/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public static DataTable getTrainingDemandTable(DBData db) {
            return db.getDataTable("select ID, " + db.langAttrName("TRAINING_DEMAND", "TITLE") + " from TRAINING_DEMAND order by NUMBER", "TRAINING_DEMAND");
        }
    }
}
