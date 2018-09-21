using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.SPZ
{
    /// <summary>
    /// Summary description for SPZModule.
    /// </summary>
    public class SPZModule : psoftModule
    {
        public SPZModule() : base()
        {
            m_NameMnemonic = "spz";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(
                AppDomain.CurrentDomain.BaseDirectory.ToString() + "SPZ/XML/language_" + languageCode + ".xml",
                languageCode,
                false
            );
        }
    }
}
