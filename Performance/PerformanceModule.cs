using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for SkillsModule.
    /// </summary>
    public class PerformanceModule : psoftModule {
        public const string LANG_SCOPE_PERFORMANCE                     = "performance";
        
        public const string LANG_MNEMO_SUBNAV_TITLE                     = "subnavTitle";
        public const string LANG_MNEMO_SUBNAV_SEARCH                    = "subnavSearch";
        public const string LANG_MNEMO_SUBNAV_SEARCH_PERSONWITHOUTRATING   = "subnavSearchPersonWithoutRating";
        public const string LANG_MNEMO_SUBNAV_REPORT_AVERAGEEOPERFORMANCE    = "subnavReportAverageOEPerformance";
        public const string LANG_MNEMO_SUBNAV_REPORT_PERFORMANCECHANGE    = "subnavReportPerformanceChange";

		public const string LANG_MNEMO_JOBEXPECTATION                  = "jobExpectation";
		public const string LANG_MNEMO_PERFORMANCERATING               = "performanceRating";
        public const string LANG_MNEMO_PERFORMANCERATING_SELF          = "performanceRatingSelf";
		public const string LANG_MNEMO_PERFORMANCERATING_REPORT        = "performanceRatingGlobalReport";

        //breadcrumb captions
        public const string  LANG_MNEMO_BC_SEARCH_PERFORMANCERATING   = "bcSearchPerformanceRating";
        public const string  LANG_MNEMO_BC_SEARCH_PERFORMANCERATING_PERSONWITHOUTRATING   = "bcSearchPerformanceRatingPersonWithoutRating";
        public const string  LANG_MNEMO_BC_REPORT_AVERAGEPERFORMANCE   = "bcReportAveragePerformance";
        public const string  LANG_MNEMO_BC_REPORT_PERFORMANCECHANGE   = "bcReportPerformanceChange";
        public const string  LANG_MNEMO_BC_REPORT_GLOBALPERFORMANCE_SELECTDATE   = "bcGlobalPerformanceSelectDate";
        public const string  LANG_MNEMO_BC_COPYJOBEXPECTATIONS   = "bcCopyJobExpectations";

        //controls title
        public const string  LANG_MNEMO_CT_PERFORMANCERATING_SEARCH_LIST    = "ctPerformanceRatingSearchList";
        public const string LANG_MNEMO_CT_PERFORMANCERATING_SELF_SEARCH_LIST = "ctPerformanceRatingSelfSearchList";
        public const string  LANG_MNEMO_CT_PERFORMANCERATING_SEARCH_LIST_PERSONWITHOUTRATING    = "ctPerformanceRatingSearchListPersonWithoutRating";
        public const string  LANG_MNEMO_CT_EXPECTATION_COPY    = "ctExpectationCopy";

        public const string  LANG_MNEMO_REP_AVERAGEPERFORMANCE  = "reportAveragePerformance";
        public const string  LANG_MNEMO_REP_AVERAGEPERFORMANCESELF  = "reportAveragePerformanceSelf";
        public const string  LANG_MNEMO_REP_AVERAGEPERFORMANCES  = "reportAveragePerformances";
        public const string  LANG_MNEMO_REP_PERFORMANCECRITERIA  = "reportPerformanceCriteria";
        public const string  LANG_MNEMO_REP_AVERAGEEOPERFORMANCE  = "reportAverageOEPerformance";
        public const string  LANG_MNEMO_REP_PERFORMANCECHANGE  = "reportPerformanceChange";
        public const string  LANG_MNEMO_REP_AVERAGEEOPERFORMANCE_PROPORTION  = "reportAverageOEPerformanceProportion";
        public const string  LANG_MNEMO_REP_DATE  = "reportDate";
        public const string  LANG_MNEMO_REP_SIGN  = "reportSign";
        public const string  LANG_MNEMO_REP_SUPERVISOR  = "reportSupervisor";
        public const string  LANG_MNEMO_REP_CLERK  = "reportClerk";
        public const string  LANG_MNEMO_REP_SUPERVISOR_RATING  = "reportSupervisorRating";
        public const string  LANG_MNEMO_REP_CLERK_RATING  = "reportClerkRating";
        public const string  LANG_MNEMO_REP_RATING_DATE  = "reportRatingDate";
        public const string  LANG_MNEMO_REP_EMPLOYMENT_PERSON  = "reportEmploymentPerson";
        public const string  LANG_MNEMO_REP_PERFORMANCE_PROPORTION  = "reportPerformanceProportion";
        public const string  LANG_MNEMO_REP_YEAR  = "reportYear";
        public const string  LANG_MNEMO_REP_TOTAL = "reportTotal";
        public const string  LANG_MNEMO_REP_RATINGPERIODS = "reportRatingPeriods";
        public const string  LANG_MNEMO_REP_GLOBALPERFORMANCE  = "reportPerformanceRatingGlobal";
        

        public PerformanceModule() : base() {
            m_NameMnemonic = "performance";
//            m_StartURL = "../Performance/Search.aspx";
            m_SubNavMenuURL = "../Performance/SubNavMenu.aspx";
            m_IsVisible = false;
        }

        public override void OnAfterLogin(HttpSessionState session){
            DBData db = DBData.getDBData(session);
            db.Performance.refreshCacheEntriesAsynchronous();
        }
            
        public override bool IsVisible(HttpSessionState session)
        {
            // immer unsichtbar wegen neuem Menü
            return false;
            
            DBData db = DBData.getDBData(session);
            if (db.Performance.hasRateableJobs())
            {
                m_IsVisible = true;
            }
            else 
            {
                m_IsVisible = false;
            }

            return m_IsVisible;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Performance/XML/language_" + languageCode + ".xml", languageCode, false);
        }

		public static bool showGlobalPerformanceReport
		{
			get { return Global.Config.getModuleParam("performance", "showGlobalPerformanceReport", "1") == "1"; }
		}

        public static bool showMeasure
        {
            get { return Global.Config.getModuleParam("performance", "showMeasure", "1") == "1"; }
        }

        public static bool showGlobalComment
        {
            get { return Global.Config.getModuleParam("performance", "showGlobalComment", "1") == "1"; }
        }
		public static string getGlobalPerformanceReportClassName
		{
			get { return Global.Config.getModuleParam("performance", "globalPerformanceReportClass", "ch.appl.psoft.Performance.GlobalPerformanceReport"); }
		}

		public static string getAveragePerfomanceReportClassName
		{
			get { return Global.Config.getModuleParam("performance", "averagePerformanceReportClass", "ch.appl.psoft.Performance.AveragePerformanceReport"); }
		}

        public static bool showPyramidWeight{
            get { return Global.Config.getModuleParam("performance", "showPyramidWeight", "1") == "1"; }
        }

        public static bool showPyramidFooter {
            get { return Global.Config.getModuleParam("performance", "showPyramidFooter", "1") == "1"; }
        }

        public static bool showLine
        {
            get { return Global.Config.getModuleParam("performance", "showLine", "1") == "1"; }
        }

		public static int basePercentage 
		{
			get { 	return Int32.Parse(Global.Config.getModuleParam("performance", "performanceRatingBase", "100")); }
		}
		
		public static double recalcToBase(double val) 
		{
			return val*basePercentage/100;
		}

        public static string getLogoOrientation
        {
            get { return Global.Config.getModuleParam("performance", "logoOrientation", "left"); }
        }

        public static bool ShowSign2Up
        {
            get { return Global.Config.getModuleParam("performance", "ShowSign2Up", "1")== "1"; }
        }

        public static DataTable GetPersonJobs(string jobId, DBData db)
        {
            DataTable jobTable;

            if (Global.isModuleEnabled("spz"))
            {
                //get jobname
                string jobName = ch.psoft.Util.Validate.GetValid(db.lookup("TITLE_DE", "JOB", "ID = " + jobId).ToString(), "-1");
                //get jobs with same name
                jobTable = db.getDataTable("SELECT id,EMPLOYMENT_ID FROM JOB WHERE TITLE_DE = '" + jobName + "'");
            }

            else
            {
                //get function id
                string funktion_Id = ch.psoft.Util.Validate.GetValid(db.lookup("FUNKTION_ID", "JOB", "ID = " + jobId).ToString(), "-1");
                //get jobs with same function
                jobTable = db.getDataTable("SELECT id,EMPLOYMENT_ID FROM JOB WHERE FUNKTION_ID = " + funktion_Id);
            }
            return jobTable;
        }


        public static void CopyJobExpectationFunktion(DBData db, DataTable table)
        {
            int ordNumber = (int)db.lookup("count(*)", "JOB_EXPECTATION", "CRITERIA_REF = " + table.Rows[0]["CRITERIA_REF"].ToString() + " and JOB_REF = " + table.Rows[0]["JOB_REF"].ToString());
            String funktion_Id = ch.psoft.Util.Validate.GetValid(db.lookup("FUNKTION_ID", "JOB", "ID = " + table.Rows[0]["JOB_REF"].ToString()).ToString());
            db.executeNative("COPY_JOBEXPECTATION_FUNKTION " + table.Rows[0]["JOB_REF"].ToString() + ", " + funktion_Id + ", " + table.Rows[0]["CRITERIA_REF"].ToString()+ ", " + ordNumber.ToString());
        }

        public static void CopyJobExpectationJobName(DBData db, DataTable table)
        {
            String jobName = ch.psoft.Util.Validate.GetValid(db.lookup("TITLE_DE", "JOB", "ID = " + table.Rows[0]["JOB_REF"].ToString()).ToString());
            db.executeNative("COPY_JOBEXPECTATION_JOBNAME " + table.Rows[0]["JOB_REF"].ToString() + ", " + "'" + jobName + "'" + ", " + table.Rows[0]["CRITERIA_REF"].ToString());
        }
    }
}
