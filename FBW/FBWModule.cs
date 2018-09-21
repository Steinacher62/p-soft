using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.FBW
{
    /// <summary>
    /// Summary description for FBWModule.
    /// </summary>
    public class FBWModule : psoftModule {
        public const string LANG_SCOPE_FBW                     = "fbw";

        //breadcrumb captions
        public const string LANG_MNEMO_BC_FBW_CATALOG          = "bcFBWCatalog";
        public const string LANG_MNEMO_BC_FUNCTIONRATING       = "bcFunctionRating";

        //controls title
        public const string LANG_MNEMO_CT_CRITERIA_TREE        = "ctCriteriaTree";
        public const string LANG_MNEMO_CT_ARGUMENT_DETAIL      = "ctArgumentDetail";
        public const string LANG_MNEMO_CT_ANFORDERUNGLIST      = "ctAnforderungList";
        public const string LANG_MNEMO_CT_ANFORDERUNGEN        = "ctAnforderungen";

        //page title
        public const string LANG_MNEMO_PT_FUNCTIONRATING       = "ptFunctionRating";

        //context-menus
        public const string LANG_MNEMO_CM_CRITERIAS            = "cmCriterias";

        //context-menu title
        public const string LANG_MNEMO_CMT_CRITERIAS           = "cmtCriterias";
        public const string LANG_MNEMO_CMT_FUNCTIONRATING      = "cmtFunctionRating";

        //report
        public const string LANG_MNEMO_REP_FUNCTIONRATING      = "repFunctionRating";
        public const string LANG_MNEMO_REP_RATING              = "repRating";

        
        public FBWModule() : base() {
            m_NameMnemonic = "FBW";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "FBW/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public static string getCatalogPath(DBData db, long catalogID, bool includeCriterium){
            return getCatalogPath(db, catalogID, includeCriterium, false);
        }

        public static string getCatalogPath(DBData db, long catalogID, bool includeCriterium, bool useCriteriumLabel){
            string retValue = "";

            while (catalogID > 0) {
                string [] lookups = db.lookup(new string[] {"BEZEICHNUNG", "PARENT_ID", "ID"}, "FBW_ARGUMENT_KATALOG", "ID=" + catalogID, false);
                catalogID = ch.psoft.Util.Validate.GetValid(lookups[1], -1L);
                if (catalogID <= 0) {
                    if (includeCriterium){
                        if (useCriteriumLabel){
                            retValue = db.lookup("LABEL", "FBW_KRITERIUM", "FBW_ARGUMENT_KATALOG_ID=" + lookups[2], false) + " \\ " + retValue;
                        }
                        else{
                            retValue = lookups[0] + " \\ " + retValue;
                        }
                    }
                }
                else{
                    retValue = lookups[0] + " \\ " + retValue;
                }

            }

            return retValue;
        }

    }

}
