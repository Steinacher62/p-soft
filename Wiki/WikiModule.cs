using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Web;

namespace ch.appl.psoft.Wiki
{

    public class WikiModule : psoftModule {

        public const string LANG_SCOPE_WIKI = "wiki";
        public const string LANG_MNEMO_WIKI = "wiki";

        //breadcrumb captions
        public const string LANG_MNEMO_BC_ADD_IMAGE = "bcAddImage";

        //contextlink titles
        public const string LANG_MNEMO_CT_XXX       = "ctXxx";

        //context links
        public const string LANG_MNEMO_CL_ADD_IMAGE = "clAddImage";

        //section titles
        public const string LANG_MNEMO_ST_ADD_IMAGE = "stAddImage";
        public const string LANG_MNEMO_ST_IMAGES    = "stImages";

        public WikiModule() : base() {
            m_NameMnemonic = LANG_SCOPE_WIKI;
            m_StartURL = "";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Wiki/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public override void Application_End(HttpApplicationState application) {
            DBData db = DBData.getDBData();

            db.connect();
            try {
                // Delete all temporary pictures
                db.WikiImage.deleteAllToBeDeleted();
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                db.disconnect();
            }
        }
    }
}
