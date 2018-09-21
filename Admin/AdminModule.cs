using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ch.appl.psoft.admin
{
    public class AdminModule : psoftModule
    {
        public AdminModule()
            : base()
        {
            m_NameMnemonic = "administration";
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(
                AppDomain.CurrentDomain.BaseDirectory.ToString() + "Admin/XML/language_" + languageCode + ".xml",
                languageCode,
               false
           );
        }
    }
}