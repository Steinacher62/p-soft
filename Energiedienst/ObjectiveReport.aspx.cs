using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;

namespace ch.appl.psoft.Energiedienst
{
    /// <summary>
    /// Summary description for ObjectiveReport.
    /// </summary>
    public partial class ObjectiveReport : System.Web.UI.Page {
        protected string onLoad = "";

        protected void Page_Load(object sender, System.EventArgs e) {
            long turnId = ch.psoft.Util.Validate.GetValid(Request.QueryString["turnId"], 0L);
            long personId = ch.psoft.Util.Validate.GetValid(Request.QueryString["contextId"], 0L);
            DBData db = DBData.getDBData(Session);
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
            string sql = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY); 
            string imageDirectory = Request.MapPath("~/images");
            string reportfile = Server.MapPath(Global.Config.baseURL + "/crystalreports/MbOEnergiedienst.rpt");
            string fileName = "";

            db.connect(); 
            DataTable tableObjectives = db.getDataTableExt("SELECT OBJECTIVE.ID AS ObjectiveId, OBJECTIVE.PERSON_ID AS PersonId, OBJECTIVE.NUMBER AS Nr, OBJECTIVE.STRATEGIE_RELATION AS Strategiebezug, " 
                                                              +"OBJECTIVE.DESCRIPTION AS Beschreibung, OBJECTIVE.ACTIONNEED AS Massnahmen, OBJECTIVE.TERMIN AS Termin, " 
                                                              +"OBJECTIVE.ARGUMENT AS Etappengespraech, OBJECTIVE.MEASUREKRIT AS Erfolgskriterien, OBJECTIVE.MEMO AS AufwandNutzen, "
                                                              +"OBJECTIVE.TASKLIST AS NegativeFolgen, OBJECTIVE_PERSON_RATING.RATING_WEIGHT AS Gewichutung, "
                                                              +"OBJECTIVE_PERSON_RATING.RATING AS Zielerreichung, PERSON.PNAME AS Name, PERSON.FIRSTNAME AS Vorname, "
                                                              + "JOB.TITLE_DE AS Stellenbezeichnung, OBJECTIVE.STATEDATE AS LetzteAenderung, OBJECTIVE.STARTDATE AS Erstellungsdatum, PERSON_1.PNAME AS NameVorgesetzter, "
                                                              + "PERSON_1.FIRSTNAME AS [Vorname Vorgesetzter], OBJECTIVE_TURN.TITLE_DE AS Zielrunde, OBJECTIVE.VIEWED AS Eingesehen, OBJECTIVE.INTERVIEW_DONE AS Freigegeben "
                                                            +"FROM EMPLOYMENT INNER JOIN "
                                                              +"PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID INNER JOIN "
                                                              +"JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                              +"OBJECTIVE ON PERSON.ID = OBJECTIVE.PERSON_ID INNER JOIN "
                                                              +"OBJECTIVE_TURN ON OBJECTIVE.OBJECTIVE_TURN_ID = OBJECTIVE_TURN.ID INNER JOIN "
                                                              +"PERSON AS PERSON_1 ON OBJECTIVE.CREATOR = PERSON_1.ID LEFT OUTER JOIN "
                                                              +"OBJECTIVE_PERSON_RATING ON OBJECTIVE.ID = OBJECTIVE_PERSON_RATING.OBJECTIVE_ID "
                                                              +"WHERE OBJECTIVE_TURN.ID = " + turnId + " AND OBJECTIVE.PERSON_ID = " + personId + " AND (dbo.JOB.HAUPTFUNKTION = 1)");

            // delete temporary table if exists 
            db.connect();
            string tbl_del = " IF NOT OBJECT_ID('tempdb..[##Objectives_%personId%]') IS NULL "
                                + "DROP TABLE [##Objectives_%personId%]";
            db.execute(tbl_del.Replace("%personId%", personId.ToString()));
            //create and fill temporary table
            string tbl_create = "CREATE TABLE [##Objectives_%personId%]("
                        + "[ObjectiveId] [bigint] NULL,"
                        + "[PersonId] [bigint] NULL,"
                        + "[Nr] [varchar](128) NULL,"
                        + "[Strategiebezug] [varchar](200) NULL,"
                        + "[Beschreibung] [varchar](2000) NULL,"
                        + "[Massnahmen] [varchar](1000) NULL,"
                        + "[Termin] [varchar](50) NULL,"
                        + "[Etappengespraech] [varchar](1000) NULL,"
                        + "[Erfolgskriterien] [varchar](3000) NULL,"
                        + "[AufwandNutzen] [varchar](3000) NULL,"
                        + "[NegativeFolgen] [varchar](1000) NULL,"
                        + "[Gewichutung] [float] NULL,"
                        + "[Zielerreichung] [int] NULL,"
                        + "[Name] [varchar](64) NULL,"
                        + "[Vorname] [varchar](64) NULL,"
                        + "[Stellenbezeichnung] [varchar](128) NULL,"
                        + "[LetzteAenderung] [datetime] NULL,"
                        + "[Erstellungsdatum] [datetime] NULL,"
                        + "[NameVorgesetzter] [varchar](64) NULL,"
                        + "[VornameVorgesetzter] [varchar](64) NULL,"
                        + "[Zielrunde] [varchar](128) NULL,"
                        + "[Eingesehen] [datetime] NULL,"
                        + "[Freigegeben] [datetime] NULL"
                        + ") ON [PRIMARY]";
            db.execute(tbl_create.Replace("%personId%", personId.ToString()));

            string erstellungsdatum;
            string eingesehen;
            string freigegeben;

            if (DBNull.Value.Equals(tableObjectives.Rows[0]["Erstellungsdatum"]))
            {
                erstellungsdatum = "01.01.1900 00:00:00";
            }
            else
            {
                erstellungsdatum = tableObjectives.Rows[0]["Erstellungsdatum"].ToString();
            }
             
            if (DBNull.Value.Equals(tableObjectives.Rows[0]["Eingesehen"]))
            {
                eingesehen = "01.01.1900 00:00:00";
            }
            else
            {
                eingesehen = tableObjectives.Rows[0]["Eingesehen"].ToString();
            }

            if (DBNull.Value.Equals(tableObjectives.Rows[0]["Freigegeben"]))
            {
                freigegeben = "01.01.1900 00:00:00";
            }
            else
            {
                freigegeben = tableObjectives.Rows[0]["Freigegeben"].ToString();
            }
            
            foreach (DataRow itemsRow in tableObjectives.Rows)
            {
                if (DBNull.Value.Equals(itemsRow["Erstellungsdatum"]))
                {
                    itemsRow["Erstellungsdatum"] = erstellungsdatum;
                }
                if (DBNull.Value.Equals(itemsRow["Eingesehen"]))
                {
                    itemsRow["Eingesehen"] = eingesehen;
                }
                if (DBNull.Value.Equals(itemsRow["Freigegeben"]))
                {
                    itemsRow["Freigegeben"] = freigegeben;
                }

                sql = "INSERT INTO [##Objectives_" + personId.ToString() + "] ([ObjectiveId],[PersonId],[Nr],[Strategiebezug],[Beschreibung],[Massnahmen],[Termin],[Etappengespraech],[Erfolgskriterien],[AufwandNutzen],[NegativeFolgen],[Gewichutung],[Zielerreichung],[Name],[Vorname],[Stellenbezeichnung],[LetzteAenderung],[Erstellungsdatum],[NameVorgesetzter],[VornameVorgesetzter],[Zielrunde],[Eingesehen],[Freigegeben])";
                sql = sql + " VALUES ('" + itemsRow[0].ToString() + "','" + itemsRow[1].ToString() + "','" + itemsRow[2].ToString().Trim().Replace("'", "''") + "','"
                                         + itemsRow[3].ToString().Trim().Replace("'", "''") + "','" + itemsRow[4].ToString().Trim().Replace("'", "''") + "','" + itemsRow[5].ToString().Trim().Replace("'", "''") + "','"
                                         + itemsRow[6].ToString().Trim().Replace("'", "''") + "','" + itemsRow[7].ToString().Trim().Replace("'", "''") + "','" + itemsRow[8].ToString().Trim().Replace("'", "''") + "','"
                                         + itemsRow[9].ToString().Trim().Replace("'", "''") + "','" + itemsRow[10].ToString().Trim().Replace("'", "''") + "','"
                                         + itemsRow[11].ToString() + "','" + itemsRow[12].ToString() + "','" + itemsRow[13].ToString().Trim().Replace("'", "''") + "','" + itemsRow[14].ToString().Trim().Replace("'", "''") + "','"
                                         + itemsRow[15].ToString().Trim().Replace("'", "''") + "','" + ((DateTime)itemsRow[16]).ToString("MM/dd/yyyy") + "','" + ((DateTime)itemsRow[17]).ToString("MM/dd/yyyy") + "','" + itemsRow[18].ToString().Trim().Replace("'", "''") + "','" + itemsRow[19].ToString().Trim().Replace("'", "''") + "','" + itemsRow[20].ToString().Trim().Replace("'", "''") + "','" + ((DateTime)itemsRow[21]).ToString("MM/dd/yyyy") + "','" + ((DateTime)tableObjectives.Rows[0][22]).ToString("MM/dd/yyyy") + "')";

                db.execute(sql);
            }

            ReportDocument rpt1 = ReportFactory.GetReport();
            //set db logon for report
            ConnectionInfo connectionInfo = new ConnectionInfo();
            connectionInfo.ServerName = Global.Config.dbServer;
            connectionInfo.DatabaseName = Global.Config.dbName;
            connectionInfo.UserID = Global.Config.dbUser;
            connectionInfo.Password = Global.Config.dbPassword;

            rpt1.Load(reportfile);


            Tables tables = rpt1.Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                tableLogonInfo.ConnectionInfo.DatabaseName = "TempDb";
                table.ApplyLogOnInfo(tableLogonInfo);
                table.Location = "[##Objectives_" + personId.ToString();
            }
            db.disconnect();
            
            SetDBLogonForReport(connectionInfo, rpt1);

            ChangeLogo changeLogo = new ChangeLogo();
            changeLogo.setLogoEnergiedienstPerson(db, rpt1, personId);



            // export to PDF
            ExportOptions exportOpts = new ExportOptions();
            PdfRtfWordFormatOptions PDFOpts = new PdfRtfWordFormatOptions();
            DiskFileDestinationOptions diskOpts = new DiskFileDestinationOptions();
            exportOpts = rpt1.ExportOptions;
            exportOpts.ExportFormatOptions = PDFOpts;

            exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat;
            exportOpts.ExportDestinationType = ExportDestinationType.DiskFile;
            fileName += SessionData.getSessionID(Session).ToString() + ".pdf";
            diskOpts.DiskFileName = outputDirectory + "\\" + fileName;
            exportOpts.DestinationOptions = diskOpts;

            rpt1.Export();
            Response.Redirect(Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + fileName, false);
                
        }

        private void SetDBLogonForReport(ConnectionInfo connectionInfo, ReportDocument rpt1)
        {
            Tables tables = rpt1.Database.Tables;

            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(tableLogonInfo);
                table.Location = connectionInfo.DatabaseName + ".dbo." + table.Location.Substring(table.Location.LastIndexOf(".") + 1);
            }
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
		
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {    
        }
		#endregion
    }
}
