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
    /// Summary description for PrintAveragePerformance.
    /// </summary>
    public partial class PrintAveragePerformance : System.Web.UI.Page
    {
        protected int _performanceRatingID = -1;
        protected int _employmentID = -1;
        protected long _personID = -1;
        protected string _onloadString;
        protected int mbo = 0;
        protected long mboId = 0;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY);
            string imageDirectory = Request.MapPath("~/images");

            DBData db = DBData.getDBData(Session);
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);

            _performanceRatingID = ch.psoft.Util.Validate.GetValid(Request.QueryString["performanceRatingID"], -1);
            _employmentID = ch.psoft.Util.Validate.GetValid(Request.QueryString["employmentID"], -1);

            fileName = "averagePerformance" + _employmentID;

            string reportClassName = PerformanceModule.getAveragePerfomanceReportClassName;
            string reportfile = Server.MapPath(Global.Config.baseURL + "/crystalreports/PerformanceratingEnergiedienst.rpt");
            _personID = ch.psoft.Util.Validate.GetValid((db.lookup("PERSON_ID", "EMPLOYMENT", "ID=" + _employmentID)).ToString(), -1L);

            DataTable tablePerformancerating = db.getDataTableExt("SELECT PERSON.PNAME AS Name, PERSON.FIRSTNAME AS Firstname, JOB.TITLE_DE AS JobTitle, ORGENTITY.TITLE_DE AS Orgentity, "
                                                   + "PERFORMANCERATING.ID AS PerformanceRatingId, PERFORMANCERATING.RATING_DATE AS RatingDate, PERFORMANCERATING.viewed AS Viewed, PERFORMANCERATING.INTERVIEW_DONE, "
                                                   + "PERSON_1.PNAME AS RatingPersonName, PERSON_1.FIRSTNAME AS RatingPersonFirstname, PERFORMANCERATING.PERFORMANCE_YEAR "
                                                   + "FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                   + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN PERFORMANCERATING ON PERSON.ID = PERFORMANCERATING.PERSON_ID INNER JOIN "
                                                   + "PERSON AS PERSON_1 ON PERFORMANCERATING.RATING_PERSON_REF = PERSON_1.ID "
                                                   + "WHERE (JOB.HAUPTFUNKTION = 1) AND (PERFORMANCERATING.ID = " + _performanceRatingID.ToString() + ")");

            // delete temporary table if exists 
            db.connect();
            string tbl_del = " IF NOT OBJECT_ID('tempdb..[##PerformanceratingEnergiedienst_%performanceRatingID%]') IS NULL "
                                + "DROP TABLE [##PerformanceratingEnergiedienst_%performanceRatingID%]";
            db.execute(tbl_del.Replace("%performanceRatingID%", _performanceRatingID.ToString()));
            //create and fill temporary table
            string tbl_create = "CREATE TABLE [##PerformanceratingEnergiedienst_%performanceRatingID%]("
                        + "[Name] [varchar](64) NULL,"
                        + "[Firstname] [varchar](64) NULL,"
                        + "[JobTitle] [varchar](128) NULL,"
                        + "[Orgentity] [varchar](128) NULL,"
                        + "[PerformanceRatingId] [bigint] NULL,"
                        + "[RatingDate] [datetime] NULL,"
                        + "[Viewed] [datetime] NULL,"
                        + "[RatingPersonName] [varchar](64) NULL,"
                        + "[RatingPersonFirstname] [varchar](64) NULL,"
                        + "[InterviewDone] [datetime] NULL,"
                        + "[PERFORMANCE_YEAR] [int] NULL"
                        + ") ON [PRIMARY]";
            db.execute(tbl_create.Replace("%performanceRatingID%", _performanceRatingID.ToString()));

            foreach (DataRow itemsRow in tablePerformancerating.Rows)
            {
                if (DBNull.Value.Equals(itemsRow["Viewed"]))
                {
                    itemsRow["Viewed"] = "01.01.1900 00:00:00";
                }
                if (DBNull.Value.Equals(itemsRow["Interview_Done"]))
                {
                    itemsRow["Interview_Done"] = "01.01.1900 00:00:00";
                }
                String sql = "INSERT INTO [##PerformanceratingEnergiedienst_" + _performanceRatingID.ToString() + "] ([Name], [Firstname], [JobTitle], [Orgentity], [PerformanceRatingId], [RatingDate], [Viewed], [RatingPersonName], [RatingPersonFirstname], [InterviewDone], [PERFORMANCE_YEAR])";
                sql = sql + " VALUES ('" + itemsRow[0].ToString().Trim().Replace("'", "''") + "','" + itemsRow[1].ToString().Trim().Replace("'", "''") + "','" + itemsRow[2].ToString().Trim().Replace("'", "''") + "','" + itemsRow[3].ToString().Trim().Replace("'", "''") + "','" + itemsRow[4].ToString() + "','" + ((DateTime)itemsRow[5]).ToString("MM/dd/yyyy") + "','" + ((DateTime)itemsRow[6]).ToString("MM/dd/yyyy") + "','" + itemsRow[8].ToString().Trim().Replace("'", "''") + "','" + itemsRow[9].ToString().Trim().Replace("'", "''") +"','"+ ((DateTime)itemsRow[7]).ToString("MM/dd/yyyy") +"','"+ itemsRow[10].ToString() +"')";

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
                table.Location = "##PerformanceratingEnergiedienst_" + _performanceRatingID.ToString();
            }

            SetDBLogonForReport(connectionInfo, rpt1);
            ChangeLogo changeLogo = new ChangeLogo();
            changeLogo.setLogoEnergiedienstPerson(db, rpt1, _personID);

            SetPerformanceDiagramm(rpt1, db);


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

        private void SetPerformanceDiagramm(ReportDocument rpt1, DBData db)
        {
            TextObject A1, A2, A3, A4, B1, B2, C1, D1, D2, D3, D4;
            TextObject AComment = (TextObject)rpt1.ReportDefinition.ReportObjects["AComment"];
            TextObject BComment = (TextObject)rpt1.ReportDefinition.ReportObjects["BComment"];
            TextObject CComment = (TextObject)rpt1.ReportDefinition.ReportObjects["CComment"];
            TextObject DComment = (TextObject)rpt1.ReportDefinition.ReportObjects["DComment"];
            TextObject GComment = (TextObject)rpt1.ReportDefinition.ReportObjects["GComment"];
            TextObject AT, BT, CT, DT, GT;
            double Aaverage=0, Baverage=0, Caverage=0, Daverage=0, Gaverage=0;
            string[] performanceLevel = new string[] { "E", "D", "C", "B", "A" };

            DataTable criteriaCount = db.getDataTable("SELECT COUNT(PERFORMANCERATING_ITEMS.CRITERIA_TITLE_DE) AS Expr1, PERFORMANCERATING_ITEMS.CRITERIA_REF "
                                                       + "FROM PERFORMANCERATING INNER JOIN PERFORMANCERATING_ITEMS ON PERFORMANCERATING.ID = PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF "
                                                       + "GROUP BY PERFORMANCERATING.ID, PERFORMANCERATING_ITEMS.CRITERIA_REF "
                                                       + "HAVING PERFORMANCERATING.ID = " + _performanceRatingID.ToString());

            DataTable rating = db.getDataTableExt("SELECT PERFORMANCERATING.ID, PERFORMANCERATING_ITEMS.CRITERIA_WEIGHT, PERFORMANCERATING_ITEMS.Id, PERFORMANCERATING_ITEMS.CRITERIA_REF, PERFORMANCERATING_ITEMS.CRITERIA_TITLE_DE, PERFORMANCERATING_ITEMS.EXPECTATION_TITLE_DE,"
                                              + "PERFORMANCERATING_ITEMS.RELATIV_WEIGHT, PERFORMANCERATING.ARGUMENTS AS totArguments, PERFORMANCERATING_ARGUMENTS.ARGUMENTS AS Arguments "
                                              + "FROM PERFORMANCERATING INNER JOIN PERFORMANCERATING_ITEMS ON PERFORMANCERATING.ID = PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF INNER JOIN "
                                              + "PERFORMANCERATING_ARGUMENTS ON PERFORMANCERATING_ITEMS.ID = PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID "
                                              + "WHERE PERFORMANCERATING.ID = " + _performanceRatingID.ToString());
            int counter = 0;
            string ratingItem = rating.Rows[0][4].ToString();
            foreach (DataRow ratingRow in rating.Rows)
            {
                string ratingField = "";
                if (ratingRow[4].ToString().Equals(ratingItem))
                {
                    counter += 1;
                }
                else
                {
                    counter = 1;
                    ratingItem = ratingRow[4].ToString();
                }

                
               
                switch (ratingRow[4].ToString())
                {
                    case "Fachliche Kompetenz":
                        ratingField = "A";
                        Aaverage += (Convert.ToInt32(ratingRow[6]) / 20);
                        break;
                    case "Soziale Kompetenz":
                        ratingField = "B";
                        Baverage += Convert.ToInt32(ratingRow[6]) / 20;
                        break;
                    case "Selbstkompetenz":
                        ratingField = "C";
                        Caverage += Convert.ToInt32(ratingRow[6]) / 20;
                        break;
                    case "Führungskompetenz":
                        ratingField = "D";
                        Daverage +=Convert.ToInt32(ratingRow[6]) / 20;
                        break;
                }

                ratingField += counter.ToString();
                if (Convert.ToInt16(ratingRow[6]) > -1)
                {

                    ratingField += performanceLevel[(Convert.ToInt32(ratingRow[6]) / 20 - 1)];


                    switch (ratingField.Substring(0, 1))
                    {
                        case "A":
                            GComment.Text = ratingRow[7].ToString();
                            switch (ratingField.Substring(1, 1))
                            {
                                case "1":
                                    A1 = (TextObject)rpt1.ReportDefinition.ReportObjects[ratingField];
                                    SetReportRating(A1, ratingField);
                                    AComment.Text = ratingRow[8].ToString();
                                    break;
                                case "2":
                                    A2 = (TextObject)rpt1.ReportDefinition.ReportObjects[ratingField];
                                    SetReportRating(A2, ratingField);
                                    break;
                                case "3":
                                    A3 = (TextObject)rpt1.ReportDefinition.ReportObjects[ratingField];
                                    SetReportRating(A3, ratingField);
                                    break;
                                case "4":
                                    A4 = (TextObject)rpt1.ReportDefinition.ReportObjects[ratingField];
                                    SetReportRating(A4, ratingField);
                                    break;
                            }
                            break;
                        case "B":
                            BComment.Text = ratingRow[8].ToString();
                            switch (ratingField.Substring(1, 1))
                            {
                                case "1":
                                    B1 = (TextObject)rpt1.ReportDefinition.ReportObjects[ratingField];
                                    SetReportRating(B1, ratingField);
                                    break;
                                case "2":
                                    B2 = (TextObject)rpt1.ReportDefinition.ReportObjects[ratingField];
                                    SetReportRating(B2, ratingField);
                                    break;
                            }
                            break;
                        case "C":
                            CComment.Text = ratingRow[8].ToString();
                            C1 = (TextObject)rpt1.ReportDefinition.ReportObjects[ratingField];
                            SetReportRating(C1, ratingField);
                            break;

                        case "D":
                            DComment.Text = ratingRow[8].ToString();
                            switch (ratingField.Substring(1, 1))
                            {
                                case "1":
                                    D1 = (TextObject)rpt1.ReportDefinition.ReportObjects[ratingField];
                                    SetReportRating(D1, ratingField);
                                    break;
                                case "2":
                                    D2 = (TextObject)rpt1.ReportDefinition.ReportObjects[ratingField];
                                    SetReportRating(D2, ratingField);
                                    break;
                                case "3":
                                    D3 = (TextObject)rpt1.ReportDefinition.ReportObjects[ratingField];
                                    SetReportRating(D3, ratingField);
                                    break;
                                case "4":
                                    D4 = (TextObject)rpt1.ReportDefinition.ReportObjects[ratingField];
                                    SetReportRating(D4, ratingField);
                                    break;
                            }
                            break;
                    }
                }
            }

            if (criteriaCount.Rows.Count > 0 && Aaverage > 0)
            {
                Aaverage = Convert.ToInt32(Math.Round((Aaverage) / Convert.ToInt32(criteriaCount.Rows[0][0]), 0, MidpointRounding.AwayFromZero) - 1);
                AT = (TextObject)rpt1.ReportDefinition.ReportObjects["A" + performanceLevel[(int)Aaverage].ToString()];
                AT.Text = performanceLevel[(int)Aaverage];
                AT.Border.BackgroundColor = System.Drawing.Color.FromArgb(128, 255, 0); ;
            }
            if (criteriaCount.Rows.Count > 0 && Baverage > 0)
            {
                Baverage = Convert.ToInt32(Math.Round((Baverage) / Convert.ToInt32(criteriaCount.Rows[1][0]), 0, MidpointRounding.AwayFromZero) - 1);
                BT = (TextObject)rpt1.ReportDefinition.ReportObjects["B" + performanceLevel[(int)Baverage].ToString()];
                BT.Text = performanceLevel[(int)Baverage];
                BT.Border.BackgroundColor = System.Drawing.Color.FromArgb(128, 255, 0); ;
            }
            if (criteriaCount.Rows.Count > 1 && Caverage > 0)
            {
                Caverage = Convert.ToInt32(Math.Round((Caverage) / Convert.ToInt32(criteriaCount.Rows[2][0]), 0, MidpointRounding.AwayFromZero) - 1);
                CT = (TextObject)rpt1.ReportDefinition.ReportObjects["C" + performanceLevel[(int)Caverage].ToString()];
                CT.Text = performanceLevel[(int)Caverage];
                CT.Border.BackgroundColor = System.Drawing.Color.FromArgb(128, 255, 0); ;
            }
            if (criteriaCount.Rows.Count > 2 && Daverage > 0)
            {
                Daverage = Convert.ToInt32(Math.Round((Daverage) / Convert.ToInt32(criteriaCount.Rows[3][0]), 0, MidpointRounding.AwayFromZero) - 1);
                DT = (TextObject)rpt1.ReportDefinition.ReportObjects["D" + performanceLevel[(int)Daverage].ToString()];
                DT.Text = performanceLevel[(int)Daverage];
                DT.Border.BackgroundColor = System.Drawing.Color.FromArgb(128, 255, 0);
                Section leadership = (Section)rpt1.ReportDefinition.Areas["DetailArea1"].Sections["DetailSection2"];
                leadership.SectionFormat.EnableSuppress = false;
            }

            if (criteriaCount.Rows.Count == 3)
            {
                Aaverage *= 0.4;
                Baverage *= 0.3;
                Caverage *= 0.3;
            }
            else
            {
                Aaverage *= 0.3;
                Baverage *= 0.2;
                Caverage *= 0.2;
                Daverage *= 0.3;
            }
            int totAverage = Convert.ToInt32(Math.Round((Aaverage + Baverage + Caverage + Daverage), 0, MidpointRounding.AwayFromZero));
            if (totAverage > 0)
            {
                GT = (TextObject)rpt1.ReportDefinition.ReportObjects["G" + performanceLevel[totAverage].ToString()];
                GT.Text = performanceLevel[totAverage];
                GT.Border.BackgroundColor = System.Drawing.Color.FromArgb(128, 255, 0);
            }
        }
        
        private void SetReportRating(TextObject reportBox, string field)
        {
            reportBox.Text = char.ConvertFromUtf32(164);
        }



        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
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
        private void InitializeComponent()
        {
        }
        #endregion
    }

}
