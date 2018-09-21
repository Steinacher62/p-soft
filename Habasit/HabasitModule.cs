using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Habasit
{
    /// <summary>
    /// Summary description for ReportModule.
    /// </summary>
    public class HabasitModule : psoftModule 
    {
		public const string LANG_SCOPE_HABASIT                     = "habasit";

		public const string  LANG_MNEMO_REP_AVERAGEPERFORMANCE  = "reportAveragePerformance";
		public const string  LANG_MNEMO_REP_DATE  = "reportDate";
		public const string  LANG_MNEMO_REP_SIGN  = "reportSign";
		public const string  LANG_MNEMO_REP_SUPERVISOR  = "reportSupervisor";
		public const string  LANG_MNEMO_REP_CLERK  = "reportClerk";
		public const string  LANG_MNEMO_REP_SUPERVISOR_RATING  = "reportSupervisorRating";
		public const string  LANG_MNEMO_REP_CLERK_RATING  = "reportClerkRating";
		public const string  LANG_MNEMO_REP_RATING_DATE  = "reportRatingDate";
		public const string  LANG_MNEMO_REP_HIGHERSUPERVISOR = "reportHigherSupervisor";
		public const string  LANG_MNEMO_REP_EMPL_REMARKS = "reportEmplRemarks";



        public HabasitModule() : base() 
        {
            m_NameMnemonic = "habasit";
            m_IsVisible = false;
        }



        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Habasit/XML/language_" + languageCode + ".xml", languageCode, false);
        }
    }
}
