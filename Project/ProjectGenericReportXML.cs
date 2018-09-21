using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.SessionState;


namespace ch.appl.psoft.Project
{
    /// <summary>
    /// XML structure of a project depending on the requested type
    /// </summary>
    public class ProjectGenericReportXML : XMLReport
    {
        public enum ReportType
        {
            SUMMARY, //Auswertung 
            OVERVIEW_EXPORT
        }

        private ReportType RequestedReport { get; set; }


        private DBData DB { get; set; }


        public ProjectGenericReportXML(DBData db, HttpSessionState session, ReportType requestedReport)
            : base(session)
        {
            this.DB = db;
            this.Title = Mapper.get("project", "reportHeader");
            this.RequestedReport = requestedReport;
        }


   


        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public DataToXml createDataToXML()
        {

            DataToXml dxml = DataToXml.createRoot("PROJECT", DB, this.Mapper, "project"); //root

            switch (RequestedReport)
            {
                case ReportType.SUMMARY:
                    this.fillInSummaryStructure(dxml);
                    break;
                case ReportType.OVERVIEW_EXPORT:
                    this.fillInOverviewExportStructure(dxml);
                    break;
                default:
                    this.fillInAllStructure(dxml);
                    break;
            }

            return dxml;
        }

        #region private methods

        private string convertCriticalDays(string value, DataToXml current)
        {
            //current contains the project id. "value" is not used
            return DB.Project.getSemaphore(current.CurrentId, true).ToString();
        }

        private string convertToPhaseCode(string value, DataToXml current)
        {
            //current contains the phase id.  "value" is not used
            int criticalDays = DB.lookup("CRITICALDAYS", "PROJECT", "ID = " + current.Parent.CurrentId, 1);
            return DB.Phase.getSemaphore(current.CurrentId, criticalDays).ToString();
        }

        private void fillInSummaryStructure(DataToXml dxml)
        {
            this.fillInBaseSummary(dxml);
            this.fillInPhaseSummary(dxml);
        }


        private void fillInOverviewExportStructure(DataToXml dxml)
        {
            dxml.addNodeValue(DB.langAttrName(Interface.DBObjects.Project.TableName, "TITLE"), "title");
            dxml.addIdNode("department", this.resolveDepartment).setCustomTranslation("Abteilung");
            if (Global.Config.getModuleParam("project", "enableMainObjectiveField", "0").Equals("1"))
            {
                dxml.addNodeValue("IS_MAIN_OBJECTIVE", "isMainObjective").convertUsingEnum(new string[] { "Nein", "Ja" });
            }

            dxml.addNodeValue("ID", "semaphoreCode").convertUsingDelegate(convertCriticalDays);
            dxml.addNodeValue("COST_INTERNAL_NOMINAL", "sollInternal");
            dxml.addNodeValue("COST_INTERNAL_ACTUAL", "istInternal");
            dxml.addNodeValue("COST_EXTERNAL_NOMINAL", "sollExternal");
            dxml.addNodeValue("COST_EXTERNAL_ACTUAL", "istExternal");

        }

        /// <summary>
        /// callback
        /// </summary>
        /// <param name="idValue"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        private string resolveDepartment(string idValue, DataToXml from)
        {
            try
            {
                long projectId = long.Parse(idValue);
                string sql = "select "
                             + this.DB.langAttrName("REGISTRY", "TITLE")
                             + " from REGISTRY where ID in (select REGISTRY_ID from REGISTRY_ENTRY where OBJECT_UID in (select UID from project where id = "
                             + projectId + ") )";
                DataTable data = this.DB.getDataTable(sql);
                if (data.Rows.Count == 0)
                {
                    return "";
                }
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (DataRow elm in data.Rows)
                {
                    sb.Append(elm[0].ToString());
                    sb.Append(", ");
                }
                if (sb.Length > 2)
                {
                    sb.Remove(sb.Length - 2, 2);
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                ch.psoft.Util.Logger.Log(e, ch.psoft.Util.Logger.ERROR);
                return "";
            }
        }

        /// <summary>
        /// extract leaders names. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        private string resolveLeaders(string id, DataToXml from)
        {
            long[] projectLeaders = DB.Project.getLeaderPersonIDs(long.Parse(id));
            System.Text.StringBuilder ret = new System.Text.StringBuilder();
            foreach (long leader in projectLeaders)
            {
                if (leader > 0)
                {
                    string sname = DB.lookup("PNAME", "PERSON", "ID = " + leader, "");
                    string fname = DB.lookup("FIRSTNAME", "PERSON", "ID = " + leader, "");
                    ret.Append(",");
                    ret.Append(fname);
                    ret.Append(" ");
                    ret.Append(sname);
                }
            }
            if (ret.Length > 0) ret.Remove(0, 1);
            return ret.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dxml"></param>
        /// <returns></returns>
        private DataToXml fillInBaseSummary(DataToXml dxml)
        {
            //Auswertung
            dxml.addNodeValue("TITLE", "title");
            dxml.addNodeValue("STARTDATE", "startDate").convertToShortDate();
            dxml.addNodeValue("DUEDATE", "dueDate").convertToShortDate();
            dxml.addNodeValue("STATE", "state").convertUsingEnum(Mapper.getEnum(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_ENUM_STATE_PROJECT, true));
            dxml.addNodeValue("CRITICALDAYS", "criticalValue");
            //color: Auftrag: blue, genehmigt: green, ...
            dxml.addIdNode("semaphoreCode", convertCriticalDays);

            //Leaders using callback on project id
            dxml.addIdNode("leaders", resolveLeaders).setCustomTranslation(Mapper.get("PHASE", "LEADER_PERSON_ID"));

            //add recursion
            dxml.createChildManyRecursive("PARENT_ID", "PROJECT", "project", "subProjects");

            return dxml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dxml"></param>
        /// <returns></returns>
        private DataToXml fillInPhaseSummary(DataToXml dxml)
        {
            //Auswertung

            //Project->Phase
            DataToXml phaseDxml = dxml.createChildMany("PROJECT_ID", "PHASE", "phase", "phases");
            phaseDxml.addNodeValue("TITLE", "title");
            phaseDxml.addNodeValue("STARTDATE", "startDate").convertToShortDate();
            phaseDxml.addNodeValue("DUEDATE", "dueDate").convertToShortDate();
            phaseDxml.addNodeValue("STATE", "state").convertUsingEnum(Mapper.getEnum(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_ENUM_STATE_PHASE, true)); //color: Offen: gruen
            phaseDxml.addNodeValue("ID", "SemaphoreCode").convertUsingDelegate(convertToPhaseCode);

            //Project->Phase.Leader
            DataToXml phaseLeaderPersonDxml = phaseDxml.createChildOne("LEADER_PERSON_ID", "PERSON", "leader");
            phaseLeaderPersonDxml.addNodeValue("PNAME", "name");
            phaseLeaderPersonDxml.addNodeValue("FIRSTNAME", "firstName");
            phaseLeaderPersonDxml.addNodeValue("MNEMO", "alias");

            return phaseDxml;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bnode"></param>
        private void fillInAllStructure(DataToXml bnode)
        {
            DataToXml dxml = this.fillInBaseSummary(bnode);
            dxml.addNodeValue("NUMBER", "number");
            dxml.addNodeValue("DESCRIPTION", "description");
            dxml.addNodeValue("CREATIONDATE", "creationDate").convertToShortDate();
            dxml.addNodeValue("ORDNUMBER", "ordNummer");
            dxml.addNodeValue("CRITICALDAYS", "criticalDays");
            dxml.addNodeValue("COST_INTERNAL_NOMINAL", "costInternalNominal");
            dxml.addNodeValue("COST_INTERNAL_ACTUAL", "costInternalActual");
            dxml.addNodeValue("COST_EXTERNAL_NOMINAL", "costExternalNominal");
            dxml.addNodeValue("COST_EXTERNAL_ACTUAL", "costExternalActual");
            dxml.addNodeValue("IS_MAIN_OBJECTIVE", "isMainObjective");

            //Project.Organisation
            DataToXml orgDxml = dxml.createChildOne("ORGANISATION_ID", "ORGANISATION", "organisation");
            orgDxml.addNodeValue("TITLE_DE", "titleDe");

            //Project.Task
            DataToXml taskListDxml = dxml.createChildOne("TASKLIST_ID", "TASKLIST", "tasklist");
            taskListDxml.addNodeValue("TITLE", "title");
            taskListDxml.addNodeValue("DESCRIPTION", "description");


            //Project.Task.Author
            DataToXml authorPersonDxml = taskListDxml.createChildOne("AUTHOR_PERSON_ID", "PERSON", "leader");
            authorPersonDxml.addNodeValue("PNAME", "name");
            authorPersonDxml.addNodeValue("FIRSTNAME", "firstName");
            authorPersonDxml.addNodeValue("MNEMO", "alias");

            DataToXml phaseDxml = this.fillInPhaseSummary(dxml);
            phaseDxml.addNodeValue("CREATIONDATE", "creationDate").convertToShortDate();
            phaseDxml.addNodeValue("CRITICALDAYS", "criticalDays");
            phaseDxml.addNodeValue("COST_INTERNAL_NOMINAL", "costInternalNominal");
            phaseDxml.addNodeValue("COST_INTERNAL_ACTUAL", "costInternalActual");
            phaseDxml.addNodeValue("COST_EXTERNAL_NOMINAL", "costExternalNominal");
            phaseDxml.addNodeValue("COST_EXTERNAL_ACTUAL", "costExternalActual");
            phaseDxml.addNodeValue("CHANCES", "chances");
            phaseDxml.addNodeValue("RISKS", "risks");
            phaseDxml.addNodeValue("SPEC_PROBLEM", "specProblem");
            phaseDxml.addNodeValue("SPEC_COMMENT", "specComment");
            phaseDxml.addNodeValue("SPEC_MODIFY_DATE", "specModifyDate");


            //Project->Phase.ProjectType
            DataToXml projectTypePersonDxml = phaseDxml.createChildOne("PROJECT_TYPE_ID", "PROJECT_TYPE", "projectType");
            projectTypePersonDxml.addNodeValue("TITLE_DE", "title");
            projectTypePersonDxml.addNodeValue("DESCRIPTION_DE", "description");

        }

        #endregion

    }
}
