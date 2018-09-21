using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Energiedienst
{
    public class EnergiedienstModule : psoftModule 
    {
        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Energiedienst/XML/language_" + languageCode + ".xml", languageCode, false);
        }
    }
}
