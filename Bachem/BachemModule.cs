using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Bachem
{
    /// <summary>
    /// Summary description for SPZModule.
    /// </summary>
    public class BachemModule : psoftModule
    {
        public BachemModule() : base()
        {
            m_NameMnemonic = "bachem";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(
                AppDomain.CurrentDomain.BaseDirectory.ToString() + "Bachem/XML/language_" + languageCode + ".xml",
                languageCode,
                false
            );
        }
    }
}
