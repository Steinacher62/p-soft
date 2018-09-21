using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Payment
{
    /// <summary>
    /// Summary description for PaymentModule.
    /// </summary>
    public class PaymentModule : psoftModule
    {
        public PaymentModule() : base()
        {
            m_NameMnemonic = "payment";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(
                AppDomain.CurrentDomain.BaseDirectory.ToString() + "Payment/XML/language_" + languageCode + ".xml",
                languageCode,
                false
            );
        }
    }
}
