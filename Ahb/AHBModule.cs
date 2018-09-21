using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.AHB
{
    /// <summary>
    /// Summary description for AHBModule.
    /// </summary>
    public class AHBModule : psoftModule
    {
        public AHBModule()
            : base()
        {
            m_NameMnemonic = "ahb";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(
                AppDomain.CurrentDomain.BaseDirectory.ToString() + "Ahb/XML/language_" + languageCode + ".xml",
                languageCode,
               false
           );
        }
    }
}