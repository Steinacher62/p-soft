using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Feller
{
    /// <summary>
    /// Summary description for ReportModule.
    /// </summary>
    public class FellerModule : psoftModule 
    {
        public enum Target 
        {
            Undefined = 0,
            Overall,
            OrgEntity,        // selected organisation entity
            ContactSelection  // selected contacts
        }

        public enum ReportType 
        {
            Undefined = 0,
            List,
            Email,
            Letter,
            EmailAndLetter
        }
        public enum Format 
        {
            Portrait,
            Landscape
        }
        public enum Layout 
        {
            Uniform,
            Computer,
            Grid
        }
        public enum HAlign 
        {
            Left,
            Center,
            Right
        }
        public enum VAlign 
        {
            Top,
            Middle,
            Bottom
        }

        public FellerModule() : base() 
        {
            m_NameMnemonic = "feller";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Feller/XML/language_" + languageCode + ".xml", languageCode, false);
        }
    }
}
