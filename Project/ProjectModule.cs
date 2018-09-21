using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Web.SessionState;

namespace ch.appl.psoft.Project
{
    /// <summary>
    /// Summary description for ProjectModule.
    /// </summary>
    public class ProjectModule : psoftModule 
    {
        public const string LANG_SCOPE_PROJECT                  = "project";

        public const string LANG_MNEMO_PROJECT                  = "project";
		public const string LANG_MNEMO_PHASE                    = "phase";

        public const string LANG_MNEMO_BILLING_PROJECT          = "billing";


        public const string LANG_MNEMO_PHASE_RED                = "phaseRed";
        public const string LANG_MNEMO_PHASE_ORANGE             = "phaseOrange";
        public const string LANG_MNEMO_PHASE_GREEN              = "phaseGreen";
        public const string LANG_MNEMO_PHASE_DONE               = "phaseDone";
        public const string LANG_MNEMO_PROJECT_RED              = "projectRed";
        public const string LANG_MNEMO_PROJECT_ORANGE           = "projectOrange";
        public const string LANG_MNEMO_PROJECT_GREEN            = "projectGreen";
        public const string LANG_MNEMO_PROJECT_DONE             = "projectDone";
		public const string LANG_MNEMO_PROJECT_BLUE             = "projectBlue";

        public const string LANG_ENUM_STATE_PROJECT             = "stateEnumProject";
		public const string LANG_ENUM_STATE_PHASE               = "stateEnumPhase";

        public const string LANG_MNEMO_SEMAPHORE                = "semaphore";
        public const string LANG_MNEMO_SEMAPHORE_ALL            = "semaphoreAll";

        public const string LANG_MNEMO_PROJECT_COMMITEE         = "projectCommitee";
        public const string LANG_MNEMO_PROJECT_LEADERS          = "projectLeaders";
        public const string LANG_MNEMO_PROJECT_MEMBERS          = "projectMembers";

        public const string LANG_MNEMO_COST                     = "cost";
        public const string LANG_MNEMO_COST_NOMINAL             = "costNominal";
        public const string LANG_MNEMO_COST_ACTUAL              = "costActual";
        public const string LANG_MNEMO_COST_DIFFERENCE          = "costDifference";
        public const string LANG_MNEMO_COST_INTERNAL            = "costInternal";
        public const string LANG_MNEMO_COST_EXTERNAL            = "costExternal";
        public const string LANG_MNEMO_PROJECT_STATE            = "projectState";

        public const string LANG_MNEMO_REPORT_HEADER            = "reportHeader";
        public const string LANG_MNEMO_REPORT_PHASES            = "reportPhases";

        //navigation-menu
        public const string LANG_MNEMO_SUBNAV_TITLE             = "subNavTitle";
        public const string LANG_MNEMO_SUBNAV_SEARCHPROJECT     = "subNavSearchProject";
        public const string LANG_MNEMO_SUBNAV_NEWPROJECT        = "subNavNewProject";

        //breadcrumb captions
        public const string LANG_MNEMO_BC_ADDPROJECT            = "bcAddProject";
        public const string LANG_MNEMO_BC_SEARCHPROJECT         = "bcSearchProject";
        public const string LANG_MNEMO_BC_EDITPROJECT           = "bcEditProject";
        public const string LANG_MNEMO_BC_ADDPHASE              = "bcAddPhase";
        public const string LANG_MNEMO_BC_EDITPHASE             = "bcEditPhase";
		public const string LANG_MNEMO_BC_ADDPHASEDEPENDENCY    = "bcAddPhaseDependency";
        public const string LANG_MNEMO_BC_PROJECT_SUMMARY       = "bcProjectSummary";
        public const string LANG_MNEMO_BC_PROJECT_TEAM          = "bcProjectTeam";
        public const string LANG_MNEMO_BC_ADD_TEAMMEMBER        = "bcAddTeamMember";
        public const string LANG_MNEMO_BC_EDIT_TEAMMEMBER       = "bcEditTeamMember";
        public const string LANG_MNEMO_BC_SPEC		            = "bcSpec";
        public const string LANG_MNEMO_BC_SCORECARD             = "bcScoreCard";
        public const string LANG_MNEMO_BC_ADDBILLING            = "bcAddBilling";
        public const string LANG_MNEMO_BC_EDITBILLING           = "bcEditBilling";


        //controls title
        public const string LANG_MNEMO_CT_PHASELIST             = "ctPhaseList";
        public const string LANG_MNEMO_CT_PROJECT_SEARCHRESULT  = "ctProjectSearchresult";
        public const string LANG_MNEMO_CT_PROJECT_SELECTION     = "ctProjectSelection";
        public const string LANG_MNEMO_CT_INVOLVED_PROJECTS     = "ctInvolvedProjects";
        public const string LANG_MNEMO_CT_PROJECT_GROUP         = "ctProjectGroup";
        
        public const string LANG_MNEMO_CT_BILLING_LIST          = "ctBillingList";

        //context-menus
        public const string LANG_MNEMO_CM_ADDPHASE              = "cmAddPhase";
        public const string LANG_MNEMO_CM_ADDPROJECT            = "cmAddProject";
        public const string LANG_MNEMO_CM_DELETEPROJECT         = "cmDeleteProject";
        public const string LANG_MNEMO_CM_EDITPROJECT           = "cmEditProject";
        public const string LANG_MNEMO_CM_EDITPHASE             = "cmEditPhase";
        public const string LANG_MNEMO_CM_OPEN_TASKLIST         = "cmOpenTasklist";
        public const string LANG_MNEMO_CM_CREATE_TASKLIST       = "cmCreateTasklist";
        public const string LANG_MNEMO_CM_PROJECT_SUMMARY       = "cmProjectSummary";
        public const string LANG_MNEMO_CM_PROJECT_ORGANISATION  = "cmProjectOrganisation";
        public const string LANG_MNEMO_CM_MANAGETEAM            = "cmManageTeam";
        public const string LANG_MNEMO_CM_SHOWTEAM              = "cmShowTeam";
        public const string LANG_MNEMO_CM_ADD_TEAMMEMBER        = "cmAddTeamMember";
        public const string LANG_MNEMO_CM_SPEC					= "cmSpec";
        public const string LANG_MNEMO_CM_SCORECARD             = "cmScoreCard";
		public const string LANG_MNEMO_CM_DELETEPHASE           = "cmDeletePhase";
		public const string LANG_MNEMO_CM_PHASEDEPENDENCY       = "cmPhaseDependency";
		public const string LANG_MNEMO_CM_PHASEDEPENDENCIES     = "cmPhaseDependencies";
        public const string LANG_MNEMO_CM_PHASEDEPENDENCY_LIST  = "cmPhaseDependencyList";
        public const string LANG_MNEMO_CM_EXPORTPROJECTOVERVIEW = "cmExportProjectOverview";
        public const string LANG_MNEMO_CM_COSTSCONTROL          = "cmProjectCostsControl";
        public const string LANG_MNEMO_CM_NEW_BILLING           = "cmNewBilling";
        public const string LANG_MNEMO_CM_PROJECT_BILLING_LIST  = "cmProjectBillingList";

        //context-menu title
        public const string LANG_MNEMO_CMT_SELECTED_PROJECT     = "cmtSelectedProject";
        public const string LANG_MNEMO_CMT_SELECTED_PHASE       = "cmtSelectedPhase";
        public const string LANG_MNEMO_CMT_TASKLIST             = "cmtTasklist";
        public const string LANG_MNEMO_CMT_LISTED_PROJECTS      = "cmtListedProjects";
        public const string LANG_MNEMO_CMT_ORGANIGRAMS          = "cmtOrganigrams";
        public const string LANG_MNEMO_CMT_PROJECTTEAM          = "cmtProjectTeam";
		
		//check-box, buttons
		public const string LANG_MNEMO_SHOWINACTIVEPROJS		= "showInactiveProjects";
        public const string LANG_MNEMO_BT_PRINT_BILLING_REPORT  = "btPrintBillingReport";
        public const string LANG_MNEMO_CB_PRINT_SUBPROJECTS     = "cbPrintSubprojects";

        //custom translations (for the xml report)
        public const string LANG_MNEMO_TT_CREDIT_BALANCE = "ttCreditBalance";
        public const string LANG_MNEMO_TT_CREDITOR_BALANCE = "ttCreditorBalance";
        public const string LANG_MNEMO_TT_DEBITOR_BALANCE = "ttDebitorBalance";
        public const string LANG_MNEMO_TT_SURPLUS = "ttSurplus";
        public const string LANG_MNEMO_TT_DEFICIT = "ttDefinit";
        public const string LANG_MNEMO_TT_DEPARTMENT = "ttDepartment";
        public const string LANG_MNEMO_TT_CREDIT_EXCL = "ttCreditExclusiveVat";
        public const string LANG_MNEMO_TT_CREDITOR_EXCL = "ttCreditorExclusiveVat";
        public const string LANG_MNEMO_TT_DEBITOR_EXCL = "ttDebitorExclusiveVat";



		//index
		public const int INDEX_PROJECT_STATE_START				= 1;

        public ProjectModule() : base() 
        {
            m_NameMnemonic = "project";
//            m_StartURL = Psoft.Project.ProjectSearch.GetURL();
            m_SubNavMenuURL = "../Project/SubNavMenu.aspx";
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Project/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public static string getSemaphorePhaseComment(HttpSessionState session, int state, int criticalDays) {
            string retValue = "";
            string strCriticalDays = criticalDays >=0 ? criticalDays.ToString() : "x";

            LanguageMapper map = LanguageMapper.getLanguageMapper(session);

            switch (state) {
                case 0:
                    retValue = map.get(LANG_SCOPE_PROJECT, LANG_MNEMO_PHASE_RED);
                    break;

                case 1:
                    retValue = map.get(LANG_SCOPE_PROJECT, LANG_MNEMO_PHASE_ORANGE).Replace("#1", strCriticalDays);
                    break;

                case 2:
                    retValue = map.get(LANG_SCOPE_PROJECT, LANG_MNEMO_PHASE_GREEN).Replace("#1", strCriticalDays);
                    break;

                case 3:
                    retValue = map.get(LANG_SCOPE_PROJECT, LANG_MNEMO_PHASE_DONE);
                    break;
            }

            return retValue;
        }

        public static string getSemaphoreProjectComment(HttpSessionState session, int state, int criticalDays) {
            string retValue = "";
            string strCriticalDays = criticalDays >=0 ? criticalDays.ToString() : "x";

            LanguageMapper map = LanguageMapper.getLanguageMapper(session);

            switch (state) {
                case 0:
                    retValue = map.get(LANG_SCOPE_PROJECT, LANG_MNEMO_PROJECT_RED);
                    break;

                case 1:
                    retValue = map.get(LANG_SCOPE_PROJECT, LANG_MNEMO_PROJECT_ORANGE).Replace("#1", strCriticalDays);
                    break;

                case 2:
                    retValue = map.get(LANG_SCOPE_PROJECT, LANG_MNEMO_PROJECT_GREEN).Replace("#1", strCriticalDays);
                    break;

                case 3:
                    retValue = map.get(LANG_SCOPE_PROJECT, LANG_MNEMO_PROJECT_DONE);
                    break;

				case 4:
					retValue = map.get(LANG_SCOPE_PROJECT, LANG_MNEMO_PROJECT_BLUE);
					break;
            }

            return retValue;
        }


        public static ArrayList getStates(LanguageMapper mapper, string enumState){
            return new ArrayList(mapper.getEnum(LANG_SCOPE_PROJECT, enumState, true));
        }


        public static string EXCEL_STYLE_SHEET_SCORE_CARD
        {
            get { return Global.Config.getModuleParam(LANG_SCOPE_PROJECT, "excelstylesheet_scorecard", DefaultValues.ProjectScoreCardExcelXSLT); }
        }
        
        public static string EXCEL_STYLE_SHEET_COST_CONTROL
        {
            get { return Global.Config.getModuleParam(LANG_SCOPE_PROJECT, "excelstylesheet_costcontrol", DefaultValues.ProjectCostControlExcelXSLT); }
        }
        
        public static string EXCEL_STYLE_SHEET_CONTROLLING
        {
            get { return Global.Config.getModuleParam(LANG_SCOPE_PROJECT, "excelstylesheet_controlling", DefaultValues.ProjectControllingExcelXSLT); }
        }

        public static bool debugXML
        {
            get { return Global.Config.getModuleParam(LANG_SCOPE_PROJECT, "debugXML", DefaultValues.DebugXML) == "1"; }
        }
    }
}
