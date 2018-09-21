using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.RPB
{
    /// <summary>
    /// Summary description for SPZModule.
    /// </summary>
    public class RPBModule : psoftModule
    {
        public RPBModule() : base()
        {
            m_NameMnemonic = "RPB";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(
                AppDomain.CurrentDomain.BaseDirectory.ToString() + "RPB/XML/language_" + languageCode + ".xml",
                languageCode,
                false
            );
        }
    }
}
