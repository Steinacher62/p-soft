using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Kaiser
{
    public class KaiserModule : psoftModule
    {
        public KaiserModule() : base()
        {
            m_NameMnemonic = "kaiser";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Kaiser/XML/language_" + languageCode + ".xml", languageCode, false);
        }
    }
}
