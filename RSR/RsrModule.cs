using ch.appl.psoft.Interface;
using System;
namespace ch.appl.psoft.RSR
{
    public class RsrModule : psoftModule
    {
        // Fields
        public const string BONUS = "prime";
        public const string FORFAIT = "forfait B";
        public const string FUNCTION_BONUS = "indemnite de fonction";
        public const string SALARY = "salaire";
        private static string[] salaryComponentList = new string[] { "salaire", "prime", "forfait B", "indemnite de fonction" };

        // Methods
        public RsrModule()
            : base()
        {
            base.m_NameMnemonic = "rsr";
            // set to invisible because needed only for new reports, no menu entry
            base.m_IsVisible = false;
            //base.m_IsVisible = true;
            base.m_SubNavMenuURL = ch.appl.psoft.Lohn.SubNavMenu.GetURL(new object[] {
                        "moduleName",
                        "rsr"});
        }
        static RsrModule()
        {
            RsrModule.salaryComponentList = new string[] {
                    "salaire",
                    "prime",
                    "forfait B",
                    "indemnite de fonction"};
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "RSR/XML/language_" + languageCode + ".xml", languageCode, false);
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