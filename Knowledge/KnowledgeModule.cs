using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Knowledge
{
    /// <summary>
    /// Summary description for KnowledgeModule.
    /// </summary>
    public class KnowledgeModule : psoftModule {

        public const string LANG_SCOPE_KNOWLEDGE = "knowledge";
        public const string LANG_MNEMO_KNOWLEDGE = "knowledge";

        //menuitems
        public const string LANG_MNEMO_MI_SEARCH_KNOWLEDGE = "miSearchKnowledge";
        public const string LANG_MNEMO_MI_NEW_KNOWLEDGE = "miNewKnowledge";
        public const string LANG_MNEMO_MI_PRINT_KNOWLEDGE = "miPrintKnowledge";

        //breadcrumb captions
        public const string LANG_MNEMO_BC_SEARCH_KNOWLEDGE = "bcSearchKnowledge";
        public const string LANG_MNEMO_BC_PRINT_KNOWLEDGE = "bcPrintKnowledge";
        public const string LANG_MNEMO_BC_EDIT_THEME       = "bcEditTheme";
        public const string LANG_MNEMO_BC_ADD_THEME        = "bcAddTheme";
        public const string LANG_MNEMO_BC_EDIT_KNOWLEDGE   = "bcEditKnowledge";
        public const string LANG_MNEMO_BC_ADD_KNOWLEDGE    = "bcAddKnowledge";

		public const string LANG_MNEMO_BC_EDIT_HISTORY       = "bcEditHistory";
		public const string LANG_MNEMO_BC_ADD_HISTORY        = "bcAddHistory";
    
        //Tooltyps, button text
        public const string LANG_MNEMO_TT_GENERATE_WORDDOC = "ttGenerateWordDocument";
        public const string LANG_MNEMO_BT_PRINT_SELECTION  = "btPrintSelection";

        //contextlink titles
        public const string LANG_MNEMO_CT_CONTENTS            = "ctContents";
        public const string LANG_MNEMO_CT_KNOWLEDGE           = "ctKnowledge";
        public const string LANG_MNEMO_CT_ASSIGNED_KNOWLEDGES = "ctAssignedKnowledges";
        public const string LANG_MNEMO_CT_ADMINISTRATION      = "ctAdministration";

        //context links
        public const string LANG_MNEMO_CL_EDIT_KNOWLEDGE   = "clEditKnowledge";
        public const string LANG_MNEMO_CL_NEW_KNOWLEDGE    = "clNewKnowledge";
        public const string LANG_MNEMO_CL_DELETE_KNOWLEDGE = "clDeleteKnowledge";
        public const string LANG_MNEMO_CL_ADD_THEME        = "clAddTheme";
        public const string LANG_MNEMO_CL_ADD_DOCUMENT     = "clAddDocument";
        public const string LANG_MNEMO_CL_ADD_IMAGE        = "clAddImage";
        public const string LANG_MNEMO_CL_INVITE_AUTHORS   = "clInviteAuthors";

        //section titles
        public const string LANG_MNEMO_ST_DOCUMENTS = "stDocuments";
        public const string LANG_MNEMO_ST_REGISTRY  = "stRegistry";
        public const string LANG_MNEMO_ST_RANKING_TITLE = "stSuggestionRanking";

        //popup menu text
        public const string LANG_MNEMO_POPUP_CANNOT_DELETE_WE  = "popupCannotDeleteKnowledge"; 


        public KnowledgeModule() : base() {
            m_NameMnemonic = LANG_SCOPE_KNOWLEDGE;
            m_StartURL = psoft.Knowledge.Search.GetURL();
        }

        public static bool simpleKnowledge
        {
            get { return Global.Config.getModuleParam("knowledge", "simpleKnowledge", "0") == "1"; }
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Knowledge/XML/language_" + languageCode + ".xml", languageCode, false);
        }
    }
}
