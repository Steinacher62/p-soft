using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Dispatch
{
    /// <summary>
    /// Summary description for DispatchModule.
    /// </summary>
    public class DispatchModule : psoftModule
	{
		public DispatchModule() : base()
		{
			m_NameMnemonic = "dispatch";
            m_IsVisible = false;
		}
        
        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Dispatch/XML/language_" + languageCode + ".xml", languageCode, false);
        }
    }
}
