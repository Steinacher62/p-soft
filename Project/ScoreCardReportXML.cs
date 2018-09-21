using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Report;
using System.Web.SessionState;
using System.Xml;


namespace ch.appl.psoft.Project
{

    /// <summary>
    /// Used to generate an XML file representation of the Score Card Report. 
    /// The Xml file can then be converted in any format.
    /// </summary>
    public class ScoreCardReportXML : XMLReport
    {

      
        DBData DB { get; set; }


        public ScoreCardReportXML(DBData db, HttpSessionState session)
            : base(session)
        {
            this.DB = db;
            this.Title = Mapper.get("project", "reportHeader");
        }


        public XmlElement createXML(XmlDocument doc, long projectId)
        {
            XmlElement elm = null;
            try
            {
                DB.connect();
                
                DataToXml projectDxml = DataToXml.createRoot("PROJECT",DB, this.Mapper, "project"); //root
               
                projectDxml.addNodeValue("TITLE", "title");
                projectDxml.addNodeValue("NUMBER", "number");
                //projectDxml.addNodeValue("DESCRIPTION", "description");
                
                projectDxml.addNodeValue("STARTDATE", "startDate").convertToShortDate();
                projectDxml.addNodeValue("DUEDATE", "dueDate").convertToShortDate();
                projectDxml.addNodeValue("SPEC_MODIFY_DATE", "specModifyDate").convertToShortDate(); //Nachführungsdatum
                //projectDxml.addNodeValue("STATE", "state").convertUsingEnum(Mapper.getEnum(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_ENUM_STATE_PROJECT, true));
                //projectDxml.addNodeValue("ORDNUMBER", "ordNummer");
                //projectDxml.addNodeValue("CRITICALDAYS", "criticalDays");
                projectDxml.addIdNode("leaders", resolveLeaders).setCustomTranslation(Mapper.get("PHASE", "LEADER_PERSON_ID"));

                projectDxml.addNodeValue("COST_INTERNAL_NOMINAL", "costInternalNominal");
                projectDxml.addNodeValue("COST_INTERNAL_ACTUAL", "costInternalActual");
                projectDxml.addNodeValue("COST_EXTERNAL_NOMINAL", "costExternalNominal");
                projectDxml.addNodeValue("COST_EXTERNAL_ACTUAL", "costExternalActual");
                projectDxml.addIdNode("costInternalDifference", costInternalDifference).setCustomTranslation(Mapper.get("project","costDifference"));
                projectDxml.addIdNode("costExternalDifference", costExternalDifference).setCustomTranslation(Mapper.get("project","costDifference")); ;
                projectDxml.addNodeValue("IS_MAIN_OBJECTIVE", "isMainObjective").convertUsingEnum(Mapper.getEnum("project", "isMainObjective",true));
                

                //DataToXml orgDxml = projectDxml.createChildOne("ORGANISATION_ID", "ORGANISATION", "organisation");
                //orgDxml.addNodeValue("TITLE_DE", "titleDe");

                //DataToXml taskListDxml = projectDxml.createChildOne("TASKLIST_ID", "TASKLIST", "tasklist");
                //taskListDxml.addNodeValue("TITLE", "title");
                //taskListDxml.addNodeValue("DESCRIPTION", "description");

                //DataToXml authorPersonDxml = taskListDxml.createChildOne("AUTHOR_PERSON_ID", "PERSON", "leader");
                //authorPersonDxml.addNodeValue("PNAME", "name");
                //authorPersonDxml.addNodeValue("FIRSTNAME", "firstName");
                //authorPersonDxml.addNodeValue("MNEMO", "alias");

                DataToXml phaseDxml = projectDxml.createChildMany("PROJECT_ID", "PHASE", "phase", "phases");
                phaseDxml.addNodeValue("TITLE", "title");
                phaseDxml.addNodeValue("STARTDATE", "startDate").convertToShortDate();
                phaseDxml.addNodeValue("DUEDATE", "dueDate").convertToShortDate();
                phaseDxml.addNodeValue("STATE", "state").convertUsingEnum(Mapper.getEnum(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_ENUM_STATE_PHASE, true)); //color: Offen: gruen
                phaseDxml.addIdNode("SemaphoreCode", convertToPhaseCode);
                phaseDxml.addNodeValue("HAS_MILESTONE", "hasMilestone").convertUsingEnum(Mapper.getEnum("phase", "hasMilestone",true));

                //phaseDxml.addNodeValue("STATE", "state").convertUsingEnum(Mapper.getEnum(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_ENUM_STATE_PHASE, true));
                //DataToXml leaderPersonDxml = phaseDxml.createChildOne("LEADER_PERSON_ID", "PERSON", "leader");
                //leaderPersonDxml.addNodeValue("PNAME", "name");
                //leaderPersonDxml.addNodeValue("FIRSTNAME", "firstName");
                //leaderPersonDxml.addNodeValue("MNEMO", "alias");


                XmlElement data = projectDxml.extractData(doc, projectId);
                XmlElement translation = projectDxml.generateTranslationStructure(doc);
                elm = DataToXml.mergeDataWithTranslation(doc, data, translation, "scoreCard");
            }
            finally
            {
                DB.disconnect();
            }
            return elm;
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
        /// <param name="value"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private string convertToPhaseCode(string value, DataToXml current)
        {
            //current contains the phase id.  "value" is not used
            int criticalDays = DB.lookup("CRITICALDAYS", "PROJECT", "ID = " + current.Parent.CurrentId, 1);
            return DB.Phase.getSemaphore(current.CurrentId, criticalDays).ToString();
        }

        private string costInternalDifference(string pid, DataToXml current)
        {
            //current contains the phase id.  "value" is not used
            double costInternalActual = DB.lookup("COST_INTERNAL_ACTUAL", "PROJECT", "ID = " + pid, 0.0);
            double costInternalNominal = DB.lookup("COST_INTERNAL_NOMINAL", "PROJECT", "ID = " + pid, 0.0);
            double costInternalDifference = (costInternalActual / costInternalNominal - 1) * 100;
            return costInternalDifference.ToString("0.0");
        }

        private string costExternalDifference(string pid, DataToXml current)
        {
            //current contains the phase id.  "value" is not used
            double costExternalActual = DB.lookup("COST_EXTERNAL_ACTUAL", "PROJECT", "ID = " + pid, 0.0);
            double costExternalNominal = DB.lookup("COST_EXTERNAL_NOMINAL", "PROJECT", "ID = " + pid, 0.0);

            double costExternalDifference = (costExternalActual / costExternalNominal - 1) * 100;
            return costExternalDifference.ToString("0.0");
        }
    
    }
}
