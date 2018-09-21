using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.FoamPartner
{
    /// <summary>
    /// Summary description for FoampartnerModule.
    /// </summary>
    public class FoampartnerModule : psoftModule
    {
        public FoampartnerModule()
            : base()
        {
            m_NameMnemonic = "foampartner";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(
                AppDomain.CurrentDomain.BaseDirectory.ToString() + "FoamPartner/XML/language_" + languageCode + ".xml",
                languageCode,
               false
           );
        }
    }
}