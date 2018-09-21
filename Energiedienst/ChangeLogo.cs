using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;
using System;
namespace ch.appl.psoft.Energiedienst
{
    public class ChangeLogo : psoftModule
    {

        public string getLogoFilename(DBData db,long personId)
        {
            Int32 maMainJobId = Convert.ToInt32(db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + personId + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString());
            long maJobOrgId = Convert.ToInt32(db.lookup("ORGENTITY_ID", "JOB", "ID =" + maMainJobId.ToString()));
            string LogoFilename = Global.Config.getModuleParam("report", "headerLogoImage", "PsoftDogBlack.gif"); 
            if (db.Orgentity.addAllSubOEIDs("25585").IndexOf(maJobOrgId.ToString(), 0) > -1) 
            {
                LogoFilename = "logoednetze.bmp";
            }
            return LogoFilename;
        }


        public void setLogoEnergiedienstPerson(DBData db, ReportDocument report, long personId)
        {
            Int32 maMainJobId = Convert.ToInt32(db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + personId + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString());
            long maJobOrgId = Convert.ToInt32(db.lookup("ORGENTITY_ID", "JOB", "ID =" + maMainJobId.ToString()));
            ParameterDiscreteValue pathLogoField_value = new ParameterDiscreteValue();
            if (db.Orgentity.addAllSubOEIDs("25585").IndexOf(maJobOrgId.ToString(), 0) > -1)
            {
                pathLogoField_value.Value = Global.Config.logoImageDirectory + "\\LogoEdNetze.bmp";
            }
            else
            {
                pathLogoField_value.Value = Global.Config.logoImageDirectory + "\\" + Global.Config.getModuleParam("report", "headerLogoImage", "PsoftDogBlack.gif").ToString();
            }
            report.SetParameterValue("LogoPath", pathLogoField_value.Value);
        }

        public void setLogoEnergiedienstOe(DBData db, CrystalReportViewer CRViewer, string Oes)
        {
            if (db.Orgentity.addAllSubOEIDs("25585").IndexOf(Oes, 0) > -1) //if first orgentity in Energiedienst Netze GmbH 
            {
                setNewLogo(CRViewer);
            }
        }

        private void setNewLogo(CrystalReportViewer CRViewer)
        {
            ParameterFields pFields = CRViewer.ParameterFieldInfo;
            ParameterField parameterImagePath = new ParameterField();
            parameterImagePath.Name = "LogoPath";
            ParameterDiscreteValue parameterImagePath_value = new ParameterDiscreteValue();
            parameterImagePath_value.Value = Global.Config.logoImageDirectory  +"\\LogoEdNetze.bmp";
            parameterImagePath.CurrentValues.Add(parameterImagePath_value);
            pFields.Add(parameterImagePath);
        }
    }
}