using ch.appl.psoft.Interface;
using System;
using System.Collections;

namespace ch.appl.psoft.Suggestion
{
    /// <summary>
    /// Summary description for SuggestionModule.
    /// </summary>
    public class SuggestionModule : psoftModule 
    {
        public const string LANG_SCOPE_SUGGESTION               = "suggestion";

        public const string LANG_MNEMO_SUGGESTION               = "suggestion";
        public const string LANG_MNEMO_STEP                     = "step";
        public const string LANG_MNEMO_NEXT_STEP                = "nextStep";
        public const string LANG_MNEMO_PREVIOUS_STEP            = "previousStep";
        public const string LANG_MNEMO_FINISH                   = "finish";
        public const string LANG_MNEMO_SENDTOKNOWLEDGE          = "sendToKnowledge";
        public const string LANG_MNEMO_NO_EXECUTION_TITLE       = "noExecutionTitle";
        
        //navigation-menu
        public const string LANG_MNEMO_SUBNAV_TITLE             = "subNavTitle";
        public const string LANG_MNEMO_SUBNAV_SEARCHSUGGESTION  = "subNavSearchSuggestion";
        public const string LANG_MNEMO_SUBNAV_ALLSUGGESTIONEXECUTIONS = "subNavAllSuggestionExecutions";
		public const string LANG_MNEMO_SUBNAV_OWNSUGGESTIONEXECUTIONS = "subNavOwnSuggestionExecutions";
        public const string LANG_MNEMO_SUBNAV_ALLSUGGESTIONEXECUTIONSMATRIX = "subNavAllSuggestionExecutionsMatrix";

        //information messages
        public const string LANG_MNEMO_NO_ACTIVE_SUGGESTION_FOUND = "ciNoActiveSuggestionFound";
        public const string LANG_MNEMO_NO_ACTIVE_SUBMITTED_EXECUTION_FOUND = "ciNoActiveSubmittedExecutionFound";
        public const string LANG_MNEMO_VAL_INFO_RANGE_NUMBER_MIN_MAX = "valInfoRangeNumberMinMax";
        public const string LANG_MNEMO_VAL_INFO_RANGE_NUMBER_MIN     = "valInfoRangeNumberMin";
        public const string LANG_MNEMO_VAL_INFO_RANGE_NUMBER_MAX     = "valInfoRangeNumberMax";
        public const string LANG_MNEMO_VAL_INFO_RANGE_TEXT_MIN_MAX   = "valInfoRangeTextMinMax";
        public const string LANG_MNEMO_VAL_INFO_RANGE_DATE_MIN_MAX   = "valInfoRangeDateMinMax";
        public const string LANG_MNEMO_VAL_INFO_RANGE_DATE_MIN       = "valInfoRangeDateMin";
        public const string LANG_MNEMO_VAL_INFO_RANGE_DATE_MAX       = "valInfoRangeDateMax";

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
        public const string LANG_MNEMO_BC_SEARCHSUGGESTION      = "bcSearchSuggestion";

        //controls title
        public const string LANG_MNEMO_CT_SUGGESTION_SEARCHRESULT   = "ctSuggestionSearchresult";
        public const string LANG_MNEMO_CT_SUGGESTION_SELECTION      = "ctSuggestionSelection";
        public const string LANG_MNEMO_CT_EXECUTABLE_SUGGESTIONS    = "ctExecutableSuggestions";
        public const string LANG_MNEMO_CT_SUGGESTION_EXECUTIONS     = "ctSuggestionExecutions";
        public const string LANG_MNEMO_CT_EXECUTION_SELECTION   = "ctExecutionSelection";

        //context-menus
        public const string LANG_MNEMO_CM_EXECUTE_SUGGESTION        = "cmExecuteSuggestion";
		public const string LANG_MNEMO_CM_EDIT_SUGGESTION        = "cmEditSuggestion";
        public const string LANG_MNEMO_CM_SHOW_EXECUTIONS       = "cmShowExecutions";
		public const string LANG_MNEMO_CM_LINK_KNOWLEDGE       = "cmLinkToKnowledge";

        //context-menu title
        public const string LANG_MNEMO_CMT_SELECTED_SUGGESTION      = "cmtSelectedSuggestion";
        public const string LANG_MNEMO_CMT_LISTED_SUGGESTIONS       = "cmtListedSuggestions";

        public SuggestionModule() : base() 
        {
            m_NameMnemonic = "suggestion";
            m_SubNavMenuURL = "../Suggestion/SubNavMenu.aspx";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Suggestion/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public static ArrayList getBooleans(LanguageMapper map){
            return new ArrayList(map.getEnum("boolean", true));
        }

        public static bool debugXML
        {
            get { return Global.Config.getModuleParam(LANG_SCOPE_SUGGESTION, "debugXML", DefaultValues.DebugXML) == "1"; }
        }

        public static string excelstylesheet
        {
            get { return Global.Config.getModuleParam(LANG_SCOPE_SUGGESTION, "excelstylesheet", DefaultValues.SuggestionExcelXSLT); }
        }

    }
}
