using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Laufenburg
{
    /// <summary>
    /// Summary description for LaufenburgModule.
    /// </summary>
    public class LaufenburgModule : psoftModule
    {
        public LaufenburgModule() : base()
        {
            m_NameMnemonic = "laufenburg";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(
                AppDomain.CurrentDomain.BaseDirectory.ToString() + "Laufenburg/XML/language_" + languageCode + ".xml",
                languageCode,
                false
            );
        }
    }
}
