using ch.appl.psoft.Interface;
using System;

public class psoftApplModule : psoftModule
{
    // Methods
    public psoftApplModule()
    {
        base.m_NameMnemonic = "PsoftPSOFT";
        base.m_IsVisible = false;
    }

    public override void LoadLanguageFile(LanguageMapper map, string languageCode)
    {
        map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "PsoftPSOFT/XML/language_" + languageCode + ".xml", languageCode, false);
    }
}

