using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Morph
{
    public class MorphModule : psoftModule
    {
        // Fields
        public const string LANG_MNEMO_BC_XXX = "bcXxx";
        public const string LANG_MNEMO_CL_XXX = "clXxxx";
        public const string LANG_MNEMO_CT_MATRIX = "ctMatrix";
        public const string LANG_SCOPE_MORPH = "morph";

        // Methods
        public MorphModule()
        {
            base.m_NameMnemonic = "morph";
            base.m_IsVisible = true;
            base.m_StartURL = Global.Config.baseURL + "/Morph/MatrixSearch.aspx";
            //base.m_StartURL = "";
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Morph/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        
    }
}

