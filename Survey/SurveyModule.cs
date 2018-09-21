using ch.appl.psoft.Interface;
using System;
using System.Collections;

namespace ch.appl.psoft.Survey
{
    /// <summary>
    /// Summary description for SurveyModule.
    /// </summary>
    public class SurveyModule : psoftModule 
    {
        public const string LANG_SCOPE_SURVEY                   = "survey";

        public const string LANG_MNEMO_SURVEY                   = "survey";
        public const string LANG_MNEMO_STEP                     = "step";
        public const string LANG_MNEMO_NEXT_STEP                = "nextStep";
        public const string LANG_MNEMO_PREVIOUS_STEP            = "previousStep";
        public const string LANG_MNEMO_FINISH                   = "finish";
        public const string LANG_MNEMO_NO_EXECUTION_TITLE       = "noExecutionTitle";
        
        //navigation-menu
        public const string LANG_MNEMO_SUBNAV_TITLE             = "subNavTitle";
        public const string LANG_MNEMO_SUBNAV_SEARCHSURVEY      = "subNavSearchSurvey";

        //validator-messages
        public const string LANG_MNEMO_VAL_ANSWER_REQUIRED      = "valAnswerRequired";
        public const string LANG_MNEMO_VAL_OUT_OF_RANGE         = "valOutOfRange";
        public const string LANG_MNEMO_VAL_RANGE_TEXT_MIN       = "valRangeTextMin";
        public const string LANG_MNEMO_VAL_RANGE_TEXT_MAX       = "valRangeTextMax";
        public const string LANG_MNEMO_VAL_RANGE_TEXT_MIN_MAX   = "valRangeTextMinMax";
        public const string LANG_MNEMO_VAL_RANGE_NUMBER_MIN     = "valRangeNumberMin";
        public const string LANG_MNEMO_VAL_RANGE_NUMBER_MAX     = "valRangeNumberMax";
        public const string LANG_MNEMO_VAL_RANGE_NUMBER_MIN_MAX = "valRangeNumberMinMax";
        public const string LANG_MNEMO_VAL_RANGE_DATE_MIN       = "valRangeDateMin";
        public const string LANG_MNEMO_VAL_RANGE_DATE_MAX       = "valRangeDateMax";
        public const string LANG_MNEMO_VAL_RANGE_DATE_MIN_MAX   = "valRangeDateMinMax";

        //breadcrumb captions
        public const string LANG_MNEMO_BC_SEARCHSURVEY          = "bcSearchSurvey";

        //controls title
        public const string LANG_MNEMO_CT_SURVEY_SEARCHRESULT   = "ctSurveySearchresult";
        public const string LANG_MNEMO_CT_SURVEY_SELECTION      = "ctSurveySelection";
        public const string LANG_MNEMO_CT_EXECUTABLE_SURVEYS    = "ctExecutableSurveys";
        public const string LANG_MNEMO_CT_SURVEY_EXECUTIONS     = "ctSurveyExecutions";
        public const string LANG_MNEMO_CT_EXECUTION_SELECTION   = "ctExecutionSelection";
        
        //context-menus
        public const string LANG_MNEMO_CM_EXECUTE_SURVEY        = "cmExecuteSurvey";
        public const string LANG_MNEMO_CM_SHOW_EXECUTIONS       = "cmShowExecutions";

        //context-menu title
        public const string LANG_MNEMO_CMT_SELECTED_SURVEY      = "cmtSelectedSurvey";
        public const string LANG_MNEMO_CMT_LISTED_SURVEYS       = "cmtListedSurveys";

        public SurveyModule() : base() 
        {
            m_NameMnemonic = "survey";
            m_SubNavMenuURL = "../Survey/SubNavMenu.aspx";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Survey/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public static ArrayList getBooleans(LanguageMapper map){
            return new ArrayList(map.getEnum("boolean", true));
        }

        public static bool showSearchSurveyMenu{
            get { return Global.Config.getModuleParam("survey", "showSearchSurveyMenu", "1") == "1"; }
        }
    }
}
