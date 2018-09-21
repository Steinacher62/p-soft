using ch.appl.psoft.Interface;

namespace ch.appl.psoft.Document
{
    /// <summary>
    /// Summary description for ExchangeModule.
    /// </summary>
    public class ExchangeModule: psoftModule
	{
		public ExchangeModule() : base()
		{
			m_NameMnemonic = "exchange";
			m_IsVisible = false;
		}

		public override void LoadLanguageFile(LanguageMapper map, string languageCode)
		{
			//map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Feller/XML/language_" + languageCode + ".xml", languageCode, false);
		}
	}
}
