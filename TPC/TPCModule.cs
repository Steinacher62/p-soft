using ch.appl.psoft.Interface;
using System;
namespace ch.appl.psoft.TPC
{
    public class TPCModule : psoftModule
    {
        // Fields
        public const string BONUS = "prime";
        public const string REQUIREMENT_BONUS = "anforderungszulage";
        public const string SALARY = "salaire";
        private static string[] salaryComponentList = new string[] { "salaire", "prime", "anforderungszulage" };

        // Methods
        public TPCModule()
        {
            base.m_NameMnemonic = "tpc";
            base.m_IsVisible = true;
            base.m_SubNavMenuURL = ch.appl.psoft.Lohn.SubNavMenu.GetURL(new object[] { "moduleName", "tpc" });
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "TPC/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        // Properties
        public static string[] SalaryComponentList
        {
            get
            {
                return salaryComponentList;
            }
        }
    }
}