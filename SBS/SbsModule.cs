using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.SBS
{
    public class SbsModule : psoftModule
    {
        public const string LANG_SCOPE_SBS = "sbs";
        public const string LANG_MNEMO_ADD_EDIT_USER = "addEditUser";
        public const string LANG_MNEMO_USERMANAGEMENT = "userManagement";
        public const string LANG_MNEMO_ADD_EDIT_SEMINARS = "addEditSeminar";
        public const string LANG_MNEMO_SEMINARMANAGEMENT = "seminarManagement";
        public const string LANG_MNEMO_STARTPAGE = "startPage";
       
        public SbsModule()
            : base()
        {
            m_NameMnemonic = "sbs";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "SBS/XML/language_" + languageCode + ".xml", languageCode, false);
        }
    }
}

