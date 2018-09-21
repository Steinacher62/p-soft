using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.PsoftPartner
{
    public class PsoftPartnerModule : psoftModule 
    {
        public PsoftPartnerModule() : base() 
        {
            m_NameMnemonic = "PsoftPartner";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "PsoftPartner/XML/language_" + languageCode + ".xml", languageCode, false);
        }
    }
}
