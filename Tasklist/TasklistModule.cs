using ch.appl.psoft.Interface;
using System;
using System.Web.SessionState;

namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Summary description for TaskListModule.
    /// </summary>
    public class TaskListModule : psoftModule
    {
        public TaskListModule() : base()
        {
            m_NameMnemonic = "tasklist";
            m_StartURL = psoft.Tasklist.Search.GetURL();
            m_IsVisible = true;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Tasklist/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public static string excelstylesheet
        {
            get { return Global.Config.getModuleParam("tasklist", "excelstylesheet", DefaultValues.TasklistExcelXSLT); }
        }

		public static string getSemaphoreMeasureComment(HttpSessionState session, int state)
		{
			return getSemaphoreMeasureComment(session, state, -1);
		}

        public static string getSemaphoreMeasureComment(HttpSessionState session, int state, int criticalDays)
        {
            string retValue = "";
			string ct = criticalDays >=0 ? criticalDays.ToString() : Global.Config.getModuleParam("tasklist", "criticalDays","5");

            LanguageMapper map = LanguageMapper.getLanguageMapper(session);
			
            switch (state)
            {
                case 0:
                    retValue = map.get("tasklist", "redMeasure");
                    break;

                case 1:
                    retValue = map.get("tasklist", "orangeMeasure1") + ct + map.get("tasklist", "orangeMeasure2");
                    break;

                case 2:
                    retValue = map.get("tasklist", "greenMeasure");
                    break;

                case 3:
                    retValue = map.get("tasklist", "doneMeasure");
                    break;
            }

            return retValue;
        }

        public static string getSemaphoreTasklistComment(HttpSessionState session, int state)
        {
            string retValue = "";

            LanguageMapper map = LanguageMapper.getLanguageMapper(session);

            switch (state)
            {
                case 0:
                    retValue = map.get("tasklist", "redTasklist");
                    break;

                case 1:
                    retValue = map.get("tasklist", "orangeTasklist");
                    break;

                case 2:
                    retValue = map.get("tasklist", "greenTasklist");
                    break;

                case 3:
                    retValue = map.get("tasklist", "doneTasklist");
                    break;
            }

            return retValue;
        }
    }
}
