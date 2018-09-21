using ch.appl.psoft.db;
using System;
using System.Data;

namespace ch.appl.psoft.Report
{
    public partial class SelectJobTitle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);

            //list jobs
            DataTable tblFkt = db.getDataTableExt("SELECT TITLE_DE FROM FUNKTION", new object[0]);
            foreach (DataRow aktRow in tblFkt.Rows)
            {
                lstFunktion.Items.Add(aktRow["TITLE_DE"].ToString());
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            string job = lstFunktion.SelectedItem.Text.Replace("'", "''");
            
            DBData db = DBData.getDBData(Session);
            db.connect();

            // delete temporary table if exists
            string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stellenbeschreibung_%userid%]') "
                              + "AND type in (N'U')) "
                                + "DROP TABLE [dbo].[Stellenbeschreibung_%userid%]";
            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

            DataTable jobDescriptionTable = db.getDataTable("SELECT DISTINCT  FUNKTION.TITLE_DE AS Stellenbezeichnung,  DUTY_VALIDITY.DESCRIPTION_DE AS Aufgabe, "+
                                                                 "DUTY_VALIDITY.VALID_FROM AS Gültig_ab,  DUTY_VALIDITY.VALID_TO AS gültig_bis,  DUTY.ORDNUMBER AS [Reihenfolge Aufgabe], "+
                                                                 "DUTY.DUTYGROUP_ID,  HirarchieAufgabengruppen.ORDNUMBER,  HirarchieAufgabengruppen.TITLE_DE,  HirarchieAufgabengruppen.[LEVEL], "+
                                                                 "HirarchieAufgabengruppen.ID,  HirarchieAufgabengruppen.PARENT_ID,  HirarchieAufgabengruppen.Ob_Gruppenname, "+
                                                                 "FUNKTION.DESCRIPTION_DE AS Stellenzweck, DUTY_COMPETENCE_VALIDITY.ID AS DutyCompetenceValidityId "+
                                                             "FROM JOB INNER JOIN DUTY_VALIDITY INNER JOIN DUTY ON  DUTY_VALIDITY.DUTY_ID =  DUTY.ID INNER JOIN FUNKTION INNER JOIN "+
                                                                 "DUTY_COMPETENCE_VALIDITY ON  FUNKTION.ID =  DUTY_COMPETENCE_VALIDITY.FUNKTION_ID ON "+
                                                                 "DUTY.ID =  DUTY_COMPETENCE_VALIDITY.DUTY_ID ON  JOB.FUNKTION_ID =  FUNKTION.ID INNER JOIN "+
                                                                 "HirarchieAufgaben ON  DUTY.DUTYGROUP_ID =  HirarchieAufgaben.ID INNER JOIN "+
                                                                 "HirarchieAufgabengruppen ON  HirarchieAufgaben.ID_TEMP =  HirarchieAufgabengruppen.ID "+                           
                                                            "WHERE ( HirarchieAufgabengruppen.[LEVEL] = 2) AND ( FUNKTION.TITLE_DE = '" + job +"') "+
                                                            "ORDER BY  HirarchieAufgabengruppen.[LEVEL],  HirarchieAufgabengruppen.ORDNUMBER, [Reihenfolge Aufgabe]");
           
            
            string tbl_create = "CREATE TABLE [dbo].[Stellenbeschreibung_%userid%]("
                                + "[Stellenbezeichnung] [varchar](128) NULL, " 
                                + "[Aufgabe] [varchar](1500) NULL, "
                                + "[Gültig_ab] [datetime] NULL, "
                                + "[Gültig_bis] [datetime] NULL, "
                                + "[Reihenfolge Aufgabe] [float] NULL, "
                                + "[DUTYGROUP_ID] [bigint] NULL, "
                                + "[ORDNUMBER] [float] NULL, "
                                + "[LEVEL] [float] NULL, "
                                + "[ID] [bigint] NULL, "
                                + "[PARENT_ID] [bigint] NULL, "
                                + "[Stellenzweck] [varchar](2500) NULL, "
                                + "[Title_de] [varchar](128) NULL, "
                                + "[Ob_Gruppenname] [varchar](128) NULL, "
                                + "[Kompetenz] [varchar] (128) NULL)"
                                + "ON [PRIMARY];";

            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

            string kompetenz;
            string sql;
            DataTable competencTable;
            foreach (DataRow jobDescriptionRow in jobDescriptionTable.Rows)
            {
              competencTable = db.getDataTable("SELECT COMPETENCE_LEVEL_ID FROM COMPETENCE WHERE DUTY_COMPETENCE_VALIDITY_ID = " + jobDescriptionRow["DutyCompetenceValidityId"].ToString());
              kompetenz = "";
              foreach (DataRow competencTableRow in competencTable.Rows)
              {
                  kompetenz = kompetenz + db.lookup("MNEMO_DE","COMPETENCE_LEVEL", "ID= " + competencTableRow["COMPETENCE_LEVEL_ID"],  " ") + " "; 
              }
            

            sql = "INSERT INTO Stellenbeschreibung_" + db.userId.ToString() + " VALUES ('" + jobDescriptionRow["Stellenbezeichnung"] + "',"
                                 + " '" + jobDescriptionRow["Aufgabe"].ToString().Replace("'", "''") + "', '" + DateTime.Parse(jobDescriptionRow["Gültig_ab"].ToString()).ToString("MM.dd.yyyy") + "',"
                                 + " '" + DateTime.Parse(jobDescriptionRow["Gültig_bis"].ToString()).ToString("MM.dd.yyyy") + "', '" + jobDescriptionRow["Reihenfolge Aufgabe"] + "',"
                                 + " '" + jobDescriptionRow["DUTYGROUP_ID"] + "', '" + jobDescriptionRow["ORDNUMBER"] + "',"
                                 + " '" + jobDescriptionRow["LEVEL"] + "', '" + jobDescriptionRow["ID"] + "', '" + jobDescriptionRow["PARENT_ID"] + "', '"
                                 + jobDescriptionRow["Stellenzweck"].ToString().Replace("'", "''") + "', '" + jobDescriptionRow["Title_de"].ToString().Replace("'", "''") + "', '" + jobDescriptionRow["Ob_Gruppenname"] + "', '" + kompetenz + "')";
            db.execute(sql);
            }
            //redirect to report


            if (job != "")
            {
                Response.Redirect("CrystalReportViewer.aspx?alias=Stellenbeschreibung&param0=" + "Stellenbeschreibung_" + db.userId.ToString(),true);
            }
        }
    }
}
