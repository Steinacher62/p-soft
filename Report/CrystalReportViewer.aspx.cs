using ch.appl.psoft.db;
using ch.appl.psoft.Energiedienst;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Payment;
using ch.psoft.Util;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace ch.appl.psoft.Report
{
    public partial class CrystalReportViewer : System.Web.UI.Page
    {
        private bool saveReport = false;
        private string reportPath = "";
        private ConnectionInfo connectionInfo;
        private ReportDocument rpt1;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                setParameter();
            }
        }

        protected void setParameter()
        {
            string alias = Request.QueryString["alias"];
            if (alias == "StatusLeistungsbewertungSPZ.rpt")
            {
                SetParameter("von", Global.DecodeFrom64(Request.QueryString["param1"]));
                SetParameter("bis", Global.DecodeFrom64(Request.QueryString["param2"]));
            }
            if (alias == "PersonalentwicklungsbedarfEnergiedienst")
            {
                SetParameter("AdvacementId", Request.QueryString["param0"]);
                SetParameter("LeaderName", Request.QueryString["param1"]);
            }

            if (alias == "PersonWithoutPerformancerating")
            {
                if (Request.QueryString["From"] != null && Request.QueryString["From"] != "")
                {
                    SetParameter("from", Request.QueryString["From"]);
                }

                if (Request.QueryString["To"] != null && Request.QueryString["To"] != "")
                {
                    SetParameter("to", Request.QueryString["To"]);
                }
            }

            if (alias == "DurchschnittlicheLeistungsanteile")
            {
                SetParameter("MaxLeistungsanteil", Request.QueryString["performanceRatingBase"].ToString());
            }

            if (alias == "Leistungswerte")
            {
                SetParameter("RatingBase", Convert.ToInt32(Global.Config.getModuleParam("performance", "performanceRatingBase", "0")));
            }

            if (alias == "MbOAbteilung" || alias == "MbOAbteilungDetail" || alias == "MbOUnternehmensziel" || alias == "MbOUnternehmenszielDetail")
            {
                ParameterFields fields = CrystalReportViewer1.ParameterFieldInfo;
                if (alias == "MbOAbteilung" || alias == "MbOAbteilungDetail")
                {
                    SetParameter("OrgentityId", this.Session["param0"].ToString());
                }
                else
                {
                    SetParameter("FirmObjectiveId", this.Session["param0"].ToString());
                    SetParameter("FirmObjectiveTitle", this.Session["param3"].ToString());
                }
                SetParameter("TurnId", this.Session["param1"].ToString());
            }


            if (alias == "LeistungswerteFoamPartner")
            {
                SetParameter("ratingBase", 60);

            }

            if (alias == "Lohnsimulation")

            {
                if (true || Session["WertPunkt"].ToString() != "0")
                {
                    //SetParameter("Punktwert", Convert.ToDouble(Session["WertPunkt"].ToString()));
                    SetParameter("Punktwert", Convert.ToString(Session["WertPunkt"]));
                }

                if (true || Session["LohnsummenKorrRel"].ToString() != "0")
                {
                    //SetParameter("LohnsummenKorrRel", Convert.ToDouble(Session["LohnsummenKorrRel"]));
                    SetParameter("LohnsummenKorrRel", Convert.ToString(Session["LohnsummenKorrRel"]));

                }

                if (true || Session["LohnsummenKorrAbs"].ToString() != "0")
                {
                    SetParameter("LohnsummenKorrAbs", Convert.ToDouble(Session["LohnsummenKorrAbs"].ToString()));
                }


                if (true || Session["LohnaendAbs"].ToString() != "0")
                {
                    SetParameter("LohnaendAbs", Convert.ToDouble(Session["LohnaendAbs"].ToString().Replace(",", ".")));
                }

                if (true || Session["LohnaendRel"].ToString() != "0")
                {
                    SetParameter("LohnaendRel", Convert.ToDouble(Session["LohnaendRel"].ToString()));
                }

                if (true || Session["ErhoeBasislohn"].ToString() != "0")
                {
                    SetParameter("ErhoeBasislohn", Convert.ToDouble(Session["ErhoeBasislohn"].ToString()));
                }

                SetParameter("ExportAdressen", Session["ExportAdresse"].ToString());
                SetParameter("Runden", Session["Runden"].ToString());
            }


            if (alias == "SimulationEinstelllohn")
            {
                SetParameter("Name", Request.QueryString["name"]);
                SetParameter("Vorname", Request.QueryString["vorname"]);
                SetParameter("Lohnvorstellung", Convert.ToDouble(Request.QueryString["lohnvorstellung"].ToString().Replace(",", ".")));
                SetParameter("Leistungsanteil%", Convert.ToDouble(Request.QueryString["leistungsanteil"].ToString().Replace(",", ".")));
                SetParameter("Alter", Convert.ToDouble(Request.QueryString["alter"].ToString().Replace(",", ".")));
                SetParameter("Erfahrung%", Convert.ToDouble(Request.QueryString["erfahrung"].ToString().Replace(",", ".")));
                SetParameter("MaxLeistungsanteil%", Convert.ToDouble(Request.QueryString["maxleistungsanteil"].ToString().Replace(",", ".")));
            }

            if (alias == "UnterdurchschnittlicheLeistungswerte" && Request.QueryString["startdat"] != null && Request.QueryString["startdat"] != "")
            {
                SetParameter("Startdatum", DateTime.Parse(Request.QueryString["startdat"]));
                SetParameter("Enddatum", DateTime.Parse(Request.QueryString["enddat"]));
                SetParameter("HalberMaxLeistungswert", Convert.ToInt32(Global.Config.getModuleParam("performance", "performanceRatingBase", "40")) / 2);
            }

            if (alias == "NegativeLeistungsentwicklung" && Request.QueryString["startdat"] != null && Request.QueryString["startdat"] != "")
            {

                SetParameter("LBFrom", DateTime.Parse(Request.QueryString["startdat"]));
                SetParameter("LBTo", DateTime.Parse(Request.QueryString["enddat"]));
                SetParameter("rangeLimit", Request.QueryString["rangeLimit"].ToString());
            }

            if (alias == "BonusNauer.rpt")
            {
                SetParameter("DurchschnittlicherLeistungsgrad", double.Parse(Request.QueryString["AvgPerformance"]));
                SetParameter("Korrekturfaktor", double.Parse(Request.QueryString["korrfakt"]));
                SetParameter("LbDatVon", Request.QueryString["datVon"]);
                SetParameter("LbDatBis", Request.QueryString["datBis"]);
            }
            if (alias == "JournalList")
            {
                // SetParameter("von", Global.DecodeFrom64(Request.QueryString["param1"]));
                // SetParameter("bis", Global.DecodeFrom64(Request.QueryString["param2"]));
            }

            if (alias == "Journal.rpt")
            {
                SetParameter("JournalId", Convert.ToInt32(Request.QueryString["param1"]));
            }

            if (alias == "Invoice")
            {
                SetParameter("OrderId", Convert.ToInt32(Session["OrderId"]));
                SetParameter("SellerAddressId", Convert.ToInt32(Session["SellerAddressId"]));
                SetParameter("SellerFirmId", Convert.ToInt32(Session["SellerFirmId"]));
                SetParameter("PaymentType", Session["PaymentType"].ToString());
            }
            if (alias == "SokratesList")
            {
                SetParameter("MatrixId", Convert.ToInt32(Request.QueryString["param0"]));
            }
            if (alias == "PraemieLaufenburg.rpt")
            {
                DateTime von = Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["paramVon"]));
                DateTime bis = Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["paramBis"]));
                double p1 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP1"]));
                double p2 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP2"]));
                double p3 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP3"]));
                double p4 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP4"]));
                double p5 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP5"]));
                double p6 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP6"]));
                double p7 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP7"]));
                double p8 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP8"]));
                double p9 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP9"]));
                double p10 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP10"]));
                double p11 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP11"]));
                double p12 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP12"]));

                SetParameter("von", Convert.ToDateTime(von).ToString("dd.MM.yyyy"));
                SetParameter("bis", Convert.ToDateTime(bis).ToString("dd.MM.yyyy"));
                SetParameter("Prämie1Pkt", p1);
                SetParameter("Prämie2Pkt", p2);
                SetParameter("Prämie3Pkt", p3);
                SetParameter("Prämie4Pkt", p4);
                SetParameter("Prämie5Pkt", p5);
                SetParameter("Prämie6Pkt", p6);
                SetParameter("Prämie7Pkt", p7);
                SetParameter("Prämie8Pkt", p8);
                SetParameter("Prämie9Pkt", p9);
                SetParameter("Prämie10Pkt", p10);
                SetParameter("Prämie11Pkt", p11);
                SetParameter("Prämie12Pkt", p12);
            }

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            {
                ////set IE9 compatibility
                //HtmlMeta metakey = new HtmlMeta();
                //metakey.HttpEquiv = "X-UA-Compatible";
                //metakey.Content = "IE=9";
                //Page.Header.Controls.AddAt(2, metakey);
                ////get report alias for db lookup
                string alias = Request.QueryString["alias"];

                if (alias == "")
                {
                    //no alias submitted
                    lblOutput.Text = "kein Report angegeben!";
                }
                else
                {
                    DBData db = DBData.getDBData(Session);

                    string filename = ch.psoft.Util.Validate.GetValid(db.lookup("filename", "CRYSTALREPORTS", "alias='" + alias + "'", ""));
                    if (filename == "" && !alias.EndsWith(".rpt"))
                    {
                        //report not found
                        lblOutput.Text = "Report wurde nicht gefunden!";
                    }
                    else
                    {
                        //get or set full filename
                        if (alias.EndsWith(".rpt"))
                        {
                            filename = Server.MapPath(Global.Config.baseURL + "/crystalreports/" + alias);
                        }
                        else
                        {
                            filename = Server.MapPath(Global.Config.baseURL + "/crystalreports/" + filename);
                        }
                        //set db logon for report
                        connectionInfo = new ConnectionInfo();
                        connectionInfo.ServerName = Global.Config.dbServer;
                        connectionInfo.DatabaseName = Global.Config.dbName;
                        connectionInfo.UserID = Global.Config.dbUser;
                        connectionInfo.Password = Global.Config.dbPassword;

                        //ReportDocument rpt1 = new ReportDocument();
                        rpt1 = ReportFactory.GetReport();
                        rpt1.Load(filename);
                        SetLogo(rpt1);
                        CrystalReportViewer1.ReportSource = rpt1;

                        if (alias == "NovisReport.rpt")
                        {
                            SqlConnection sqlcon = (SqlConnection)db.connection;
                            SqlBulkCopy Novisreport = new SqlBulkCopy(sqlcon.ConnectionString + "; Password=" + Global.Config.dbPassword);
                            Novisreport.DestinationTableName = "##Novisreport_" + db.userId.ToString();

                            SetTableName("##Novisreport_" + db.userId.ToString());
                        }

                        if (alias == "Invoice")
                        {
                            SetParameter("OrderId", Convert.ToInt32(Session["OrderId"]));
                            SetParameter("SellerAddressId", Convert.ToInt32(Session["SellerAddressId"]));
                            SetParameter("SellerFirmId", Convert.ToInt32(Session["SellerFirmId"]));
                            SetParameter("PaymentType", Session["PaymentType"].ToString());
                            saveReport = true;
                            reportPath = Request.MapPath("~" + "\\Payment\\Invoice\\Invoice_" + Session["OrderId"].ToString() + ".pdf");


                        }

                        if (alias == "JournalList")
                        {
                            string von = Global.DecodeFrom64(Request.QueryString["param1"]);
                            string bis = Global.DecodeFrom64(Request.QueryString["param2"]);
                            string oe = Global.DecodeFrom64(Session["oe_enc"].ToString());

                            DataTable journals = db.getDataTable("SELECT PERSON.ID AS PersonId, PERSON.PERSONNELNUMBER, PERSON.PNAME, PERSON.FIRSTNAME, JOB.ID AS JobId, JOB.TITLE_DE AS JobTitle, "
                                                                       + "ORGENTITY.ID AS OrgentityId, ORGENTITY.TITLE_DE AS OrgentityTitle, PERSON_JOURNAL.TITLE AS JournalTitle, "
                                                                       + "PERSON_JOURNAL.DESCRIPTION AS JournalDescription, PERSON_JOURNAL.CREATED AS JournalCreated "
                                                                + "FROM PERSON INNER JOIN "
                                                                       + "EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                                       + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                                       + "PERSON_JOURNAL ON PERSON.ID = PERSON_JOURNAL.PERSON_ID INNER JOIN "
                                                                       + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID WHERE PERSON_JOURNAL.CREATED > '" + Convert.ToDateTime(von).ToString("yyyy.MM.dd") + "' AND PERSON_JOURNAL.CREATED < '" + Convert.ToDateTime(bis).ToString("yyyy.MM.dd") + "' AND ORGENTITY.ID IN(" + oe + ")");
                            // delete temporary table if exists

                            string tbl_del = " IF NOT OBJECT_ID(N'tempdb..##JournalList_%userid%') IS NULL "
                              + "DROP TABLE ##JournalList_%userid%";
                            db.connect();
                            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                            // create table
                            string tbl_create = "CREATE TABLE [##JournalList_%userid%]("
                                                + "[PersonId] [bigint] NULL,"
                                                + "[PERSONNELNUMBER] [varchar](64) NULL,"
                                                + "[PNAME] [varchar](64) NULL,"
                                                + "[FIRSTNAME] [varchar](64) NULL,"
                                                + "[JobId] [bigint] NULL,"
                                                + "[JOBTITLE] [varchar](64) NULL,"
                                                + "[ORGENTITYId] [bigint] NULL,"
                                                + "[ORGENTITYTITLE] [varchar](128) NULL,"
                                                + "[JOURNALTITLE] [varchar](128) NULL,"
                                                + "[JOURNALDESCRIPTION] [varchar](4096) NULL,"
                                                + "[JOURNALCREATED] [datetime] NULL,"
                                                + ") ON [PRIMARY]";
                            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                            SqlConnection sqlcon = (SqlConnection)db.connection;
                            SqlBulkCopy JournalList = new SqlBulkCopy(sqlcon.ConnectionString + "; Password=" + Global.Config.dbPassword);
                            JournalList.DestinationTableName = "##JournalList_" + db.userId.ToString();
                            JournalList.WriteToServer(journals);
                            SetTableName("##JournalList_" + db.userId.ToString());
                            //SetParameter("von", Global.DecodeFrom64(Request.QueryString["param1"]));
                            //SetParameter("bis", Global.DecodeFrom64(Request.QueryString["param2"]));


                        }

                        if (alias == "JournalListStatus")
                        {

                        }



                            if (alias == "Journal.rpt")
                        {
                            SetParameter("JournalId", Convert.ToInt32(Request.QueryString["param1"]));
                        }


                        if (alias == "StatusLeistungsbewertungSPZ.rpt")
                        {
                            if (!IsPostBack)
                            {
                                string von = Global.DecodeFrom64(Request.QueryString["param1"]);
                                string bis = Global.DecodeFrom64(Request.QueryString["param2"]);
                                string oe = Global.DecodeFrom64(Session["Oes"].ToString());

                                DataTable persons = db.getDataTable("SELECT DISTINCT PERSON.ID AS PersonId, PERSON.PERSONNELNUMBER, PERSON.PNAME, PERSON.FIRSTNAME, PERSON.PFS, JOB.TITLE_DE AS JobTitle, "
                                                                      + "ORGENTITY.ID AS OrgentityId, ORGENTITY.TITLE_DE AS OrgentityTitle, PERFORMANCERATING.ID AS PreformanceRatingId, "
                                                                      + "PERFORMANCERATING.RATING_DATE, PERFORMANCERATING.INTERVIEW_DONE "
                                                                + "FROM PERSON INNER JOIN "
                                                                          + "EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                                          + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                                          + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID LEFT OUTER JOIN "
                                                                          + "PERFORMANCERATING ON JOB.ID = PERFORMANCERATING.JOB_ID "
                                                                + "WHERE PERSON.PFS = 'Ja' AND (ORGENTITY.ID IN (" + oe + ")) AND (PERFORMANCERATING.RATING_DATE > '" + Convert.ToDateTime(von).ToString("yyyy.MM.dd") + "' AND "
                                                                          + "PERFORMANCERATING.RATING_DATE < '" + Convert.ToDateTime(bis).ToString("yyyy.MM.dd") + "' ) "
                                                            + "UNION "
                                                            + "SELECT DISTINCT PERSON.ID AS PersonId, PERSON.PERSONNELNUMBER, PERSON.PNAME, PERSON.FIRSTNAME, PERSON.PFS, JOB.TITLE_DE AS JobTitle, "
                                                                      + "ORGENTITY.ID AS OrgentityId, ORGENTITY.TITLE_DE AS OrgentityTitle, 0 AS dummy1, 1 AS dummy2, 2 AS dummy3 "
                                                                + "FROM PERSON INNER JOIN "
                                                                          + "EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                                          + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                                          + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID "
                                                                + "WHERE PERSON.PFS = 'Ja' AND (ORGENTITY.ID IN (" + oe + ")) AND NOT PERSON.ID IN (SELECT DISTINCT    PERSON.ID "
                                                                + "FROM PERSON INNER JOIN "
                                                                          + "EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                                          + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                                          + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID LEFT OUTER JOIN "
                                                                          + "PERFORMANCERATING ON JOB.ID = PERFORMANCERATING.JOB_ID "
                                                                + "WHERE PERSON.PFS = 'Ja' AND (ORGENTITY.ID IN (" + oe + ")) AND (PERFORMANCERATING.RATING_DATE > '" + Convert.ToDateTime(von).ToString("yyyy.MM.dd") + "' AND "
                                                                          + "PERFORMANCERATING.RATING_DATE < '" + Convert.ToDateTime(bis).ToString("yyyy.MM.dd") + "'))");
                                // delete temporary table if exists

                                string tbl_del = " IF NOT OBJECT_ID(N'tempdb..##StatusLeistungsbewertungenSPZ_%userid%') IS NULL "
                                  + "DROP TABLE ##StatusLeistungsbewertungenSPZ_%userid%";
                                db.connect();
                                db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                                // create table
                                string tbl_create = "CREATE TABLE [##StatusLeistungsbewertungenSPZ_%userid%]("
                                                    + "[PersonId] [bigint] NULL,"
                                                    + "[PERSONNELNUMBER] [varchar](64) NULL,"
                                                    + "[PNAME] [varchar](64) NULL,"
                                                    + "[FIRSTNAME] [varchar](64) NULL,"
                                                    + "[PFS] [varchar](10) NULL,"
                                                    + "[Jobtitle] [varchar](128) NULL,"
                                                    + "[IdOrgentitiy] [int] NULL,"
                                                    + "[Orgentity] [varchar](64) NULL,"
                                                    + "[IdPerformancerating] [bigint] NULL,"
                                                    + "[Ratingdate] [datetime] NULL,"
                                                    + "[InterviewDate] [datetime] NULL"
                                                    + ") ON [PRIMARY]";
                                db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                                SqlConnection sqlcon = (SqlConnection)db.connection;
                                SqlBulkCopy StatusLeistungsbewertungenSPZ = new SqlBulkCopy(sqlcon.ConnectionString + "; Password=" + Global.Config.dbPassword);
                                StatusLeistungsbewertungenSPZ.DestinationTableName = "##StatusLeistungsbewertungenSPZ_" + db.userId.ToString();
                                StatusLeistungsbewertungenSPZ.WriteToServer(persons);


                                SetParameter("von", Global.DecodeFrom64(Request.QueryString["param1"]));
                                SetParameter("bis", Global.DecodeFrom64(Request.QueryString["param2"]));

                            }


                            SetTableName("##StatusLeistungsbewertungenSPZ_" + db.userId.ToString());

                        }

                        ChangeLogo changeLogoEnergiedienst = new ChangeLogo();

                        if (alias == "PraemieLaufenburg.rpt")
                        {
                            DateTime von = Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["paramVon"]));
                            DateTime bis = Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["paramBis"]));
                            double p1 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP1"]));
                            double p2 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP2"]));
                            double p3 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP3"]));
                            double p4 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP4"]));
                            double p5 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP5"]));
                            double p6 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP6"]));
                            double p7 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP7"]));
                            double p8 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP8"]));
                            double p9 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP9"]));
                            double p10 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP10"]));
                            double p11 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP11"]));
                            double p12 = Convert.ToDouble(Global.DecodeFrom64(Request.QueryString["paramP12"]));
                            if (!IsPostBack)
                            {
                                string tbl_del = " IF NOT OBJECT_ID(N'tempdb..##LeistungswerteLaufenburg_%userid%') IS NULL "
                                  + "DROP TABLE ##LeistungswerteLaufenburg_%userid%";
                                db.connect();
                                db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                                // create table
                                string tbl_create = "CREATE TABLE [##LeistungswerteLaufenburg_%userid%]("
                                                    + "[PersonID] [bigint] NULL,"
                                                    + "[Name] [varchar](64) NULL,"
                                                    + "[Vorname] [varchar](64) NULL,"
                                                    + "[Abteilung] [varchar](128) NULL,"
                                                    + "[Pkt_Leistungsbewertung] [float] NULL,"
                                                    + "[LeistungsbewertungsId] [bigint] NULL,"
                                                    + "[Bewertungsdatum] [datetime] NULL,"
                                                    + "[Istlohn] float NULL"
                                                    + ") ON [PRIMARY]";
                                db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                                long accessorID = SessionData.getUserAccessorID(Session);
                                string accessorSQL = db.getAccessorIDsSQLInClause(accessorID);

                                DataTable tmpPr = db.getDataTable("SELECT PERSON.ID AS PersonId, PERSON.PNAME AS Name, PERSON.FIRSTNAME AS Vorname, ORGENTITY.TITLE_DE AS Abteilung, 0 AS Pkt_Leistungsbewertung, "
                                        + "(SELECT TOP (1) ID FROM PERFORMANCERATING WHERE (RATING_DATE >= '" + von.ToString("yyyy.MM.dd") + "') AND (RATING_DATE <= '" + bis.ToString("yyyy.MM.dd") + "') AND (PERSON_ID = PERSON.ID) ORDER BY RATING_DATE DESC) AS LeistungsbewertungsId, "
                                        + "(SELECT TOP (1) RATING_DATE FROM PERFORMANCERATING AS PERFORMANCERATING_1 WHERE (RATING_DATE >= '" + von.ToString("yyyy.MM.dd") + "') AND (RATING_DATE <= '" + bis.ToString("yyyy.MM.dd") + "') AND (PERSON_ID = PERSON.ID) "
                                        + "ORDER BY RATING_DATE DESC) AS Bewertungsdatum, LOHN.ISTLOHN AS Istlohn, JOB.ID "
                                  + "FROM PERSON INNER JOIN "
                                        + "EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                        + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                        + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN "
                                        + "LOHN ON EMPLOYMENT.ID = LOHN.EMPLOYMENT_ID INNER JOIN "
                                        + "ACCESS_RIGHT_RT ON ACCESS_RIGHT_RT.TABLENAME = 'JOB' AND (ACCESS_RIGHT_RT.ROW_ID = JOB.ID OR "
                                        + "ACCESS_RIGHT_RT.ROW_ID = 0) AND ACCESS_RIGHT_RT.APPLICATION_RIGHT = 11 AND ACCESS_RIGHT_RT.AUTHORISATION & 4 = 4 AND ACCESS_RIGHT_RT.ACCESSOR_ID " + accessorSQL
                                + "WHERE (JOB.HAUPTFUNKTION = 1) "
                                + "ORDER BY Name ");

                                tmpPr.Columns["Pkt_Leistungsbewertung"].ReadOnly = false;

                                foreach (DataRow row in tmpPr.Rows)
                                {

                                    int totPkt = 0;
                                    string bewDat = "";
                                    if (row["Bewertungsdatum"].ToString().Length > 0)
                                    {

                                        bewDat = Convert.ToDateTime(row["Bewertungsdatum"]).ToString(("MM.dd.yyyy"));
                                    }

                                    if (row["LeistungsbewertungsId"].ToString().Length > 0)
                                    {
                                        DataTable tmpRatingValues = db.getDataTable("SELECT ID, CRITERIA_WEIGHT, RELATIV_WEIGHT FROM PERFORMANCERATING_ITEMS WHERE PERFORMANCERATING_REF =" + (long)row["LeistungsbewertungsId"]);

                                        foreach (DataRow rowRating in tmpRatingValues.Rows)
                                        {
                                            switch (Convert.ToInt16(rowRating["RELATIV_WEIGHT"]))
                                            {
                                                case 0:
                                                    if (Convert.ToInt16(rowRating["CRITERIA_WEIGHT"]) == 50)
                                                        totPkt += -2;
                                                    else
                                                        totPkt += -1;
                                                    break;
                                                case 25:
                                                    totPkt += 0;
                                                    break;
                                                case 50:
                                                    if (Convert.ToInt16(rowRating["CRITERIA_WEIGHT"]) == 50)
                                                        totPkt += 2;
                                                    else
                                                        totPkt += 1;
                                                    break;
                                                case 75:
                                                    if (Convert.ToInt16(rowRating["CRITERIA_WEIGHT"]) == 50)
                                                        totPkt += 4;
                                                    else
                                                        totPkt += 2;
                                                    break;
                                                case 100:
                                                    if (Convert.ToInt16(rowRating["CRITERIA_WEIGHT"]) == 50)
                                                        totPkt += 6;
                                                    else
                                                        totPkt += 3;
                                                    break;
                                                default:
                                                    totPkt = 0;
                                                    break;

                                            }

                                            row["Pkt_Leistungsbewertung"] = totPkt;

                                        }
                                    }

                                    db.execute("INSERT INTO ##LeistungswerteLaufenburg_" + db.userId.ToString() + "(PersonID, Name, Vorname, Abteilung, Pkt_Leistungsbewertung, LeistungsbewertungsId, Bewertungsdatum, Istlohn)"
                                                                                                                + " VALUES ('" + row[0].ToString().Replace("'", "''") + "', '" + row[1].ToString().Replace("'", "''") + "', '" + row[2].ToString().Replace("'", "''") + "', '" + row[3].ToString() + "', '" + row[4].ToString() + "', '" + row[5].ToString() + "', '" + bewDat + "', '" + row[7].ToString() + "')");
                                }
                                SetParameter("von", Convert.ToDateTime(von).ToString("dd.MM.yyyy"));
                                SetParameter("bis", Convert.ToDateTime(bis).ToString("dd.MM.yyyy"));
                                SetParameter("Prämie1Pkt", p1);
                                SetParameter("Prämie2Pkt", p2);
                                SetParameter("Prämie3Pkt", p3);
                                SetParameter("Prämie4Pkt", p4);
                                SetParameter("Prämie5Pkt", p5);
                                SetParameter("Prämie6Pkt", p6);
                                SetParameter("Prämie7Pkt", p7);
                                SetParameter("Prämie8Pkt", p8);
                                SetParameter("Prämie9Pkt", p9);
                                SetParameter("Prämie10Pkt", p10);
                                SetParameter("Prämie11Pkt", p11);
                                SetParameter("Prämie12Pkt", p12);
                            }
                            SetTableName("##LeistungswerteLaufenburg_" + db.userId.ToString());
                        }

                        if (alias == "EnergiedienstRatingMbO")
                        {
                            if (!IsPostBack || !ExistTmpTable("EnergiedienstRatingMbO"))
                            {
                                db.connect();
                                // delete temporary table if exists
                                string tbl_del = " IF NOT OBJECT_ID(N'tempdb..[##EnergiedienstRatingMbO_%userid%]') IS NULL "
                                               + "DROP TABLE [##EnergiedienstRatingMbO_%userid%]";
                                db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                                //create and fill temporary table
                                string tbl_create = "CREATE TABLE [##EnergiedienstRatingMbO_%userid%]("
                                + "[OBJECTIVEID] [bigint] NULL,"
                                + "[CURRENTVALUE] [varchar](128) NULL,"
                                + "[VALUEIMPLICIT] [int] NULL,"
                                + "[STARTDATE] [datetime] NULL,"
                                + "[STATEDATE] [datetime] NULL,"
                                + "[VIEWED] [datetime] NULL,"
                                + "[INTERVIEW_DONE] [datetime] NULL,"
                                + "[PERSONID] [bigint] NULL,"
                                + "[PNAME] [varchar](64) NULL,"
                                + "[FIRSTNAME] [varchar](64) NULL,"
                                + "[PERSONNELNUMBER][varchar](64) NULL,"
                                + "[PERSON_GROUP_TITLE] [varchar](256) NULL,"
                                + "[PERSON_CYRCLE_TITLE] [varchar](256) NULL,"
                                + "[ORGENTITYID]  [bigint] NULL,"
                                + "[ORGENTITY] [varchar](128) NULL,"
                                + "[OBJECTIVE_TURN_ID] [bigint] NULL,"
                                + "[TURN] [varchar](128) NULL,"
                                + "[RATINGTOT] [float] NULL,"
                                + "[MNEMONIC_DE] [varchar](20) NULL"
                                + ") ON [PRIMARY]";
                                db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                                db.execute("INSERT INTO ##EnergiedienstRatingMbO_" + db.userId.ToString() + " SELECT DISTINCT OBJECTIVE.ID AS ObjectveId, OBJECTIVE.CURRENTVALUE, OBJECTIVE.VALUEIMPLICIT, OBJECTIVE.STARTDATE, OBJECTIVE.STATEDATE, "
                                                                                                                    + "OBJECTIVE.VIEWED, OBJECTIVE.INTERVIEW_DONE, PERSON.ID AS PersonId, PERSON.PNAME, PERSON.FIRSTNAME, "
                                                                                                                    + "PERSON.PERSONNELNUMBER, PERSON.PERSON_GROUP_TITLE, PERSON.PERSON_CIRCLE_TITLE, ORGENTITY.ID AS OrgentityId, ORGENTITY.TITLE_DE AS Orgentity, OBJECTIVE.OBJECTIVE_TURN_ID, "
                                                                                                                    + "OBJECTIVE_TURN.TITLE_DE AS Turn, SUM(dbo.OBJECTIVE_PERSON_RATING.RATING * dbo.OBJECTIVE_PERSON_RATING.RATING_WEIGHT / 100) AS RatingTot, ORGENTITY.MNEMONIC_DE "
                                                                                                            + "FROM EMPLOYMENT INNER JOIN "
                                                                                                                    + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                                                                                    + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN "
                                                                                                                    + "PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID INNER JOIN "
                                                                                                                    + "OBJECTIVE_TURN INNER JOIN "
                                                                                                                    + "OBJECTIVE ON OBJECTIVE_TURN.ID = OBJECTIVE.OBJECTIVE_TURN_ID INNER JOIN "
                                                                                                                    + "OBJECTIVE_PERSON_RATING INNER JOIN "
                                                                                                                    + "OBJECTIVE AS OBJECTIVE_1 ON OBJECTIVE_PERSON_RATING.OBJECTIVE_ID = OBJECTIVE_1.ID ON "
                                                                                                                    + "OBJECTIVE_TURN.ID = OBJECTIVE_1.OBJECTIVE_TURN_ID AND OBJECTIVE.PERSON_ID = OBJECTIVE_1.PERSON_ID ON "
                                                                                                                    + "PERSON.ID = OBJECTIVE.PERSON_ID "
                                                                                                                    + "WHERE JOB.ID " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND ORGENTITY.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ") AND ((OBJECTIVE.OBJECTIVE_TURN_ID = '" + Global.DecodeFrom64(Request.QueryString["param1"]) + "' AND NOT OBJECTIVE.STARTDATE IS NULL) OR OBJECTIVE.OBJECTIVE_TURN_ID IS NULL) "
                                                                                                            + "GROUP BY OBJECTIVE.ID, OBJECTIVE.CURRENTVALUE, OBJECTIVE.VALUEIMPLICIT, OBJECTIVE.STARTDATE, OBJECTIVE.STATEDATE, "
                                                                                                                    + "OBJECTIVE.VIEWED, OBJECTIVE.INTERVIEW_DONE, PERSON.ID, PERSON.PNAME, PERSON.FIRSTNAME, PERSON.PERSONNELNUMBER,PERSON.PERSON_GROUP_TITLE, PERSON.PERSON_CIRCLE_TITLE, "
                                                                                                                    + "ORGENTITY.ID, ORGENTITY.TITLE_DE, OBJECTIVE.OBJECTIVE_TURN_ID, OBJECTIVE_TURN.TITLE_DE, ORGENTITY.MNEMONIC_DE");

                            }
                            SetTableName("##EnergiedienstRatingMbO_" + db.userId.ToString());

                            changeLogoEnergiedienst.setLogoEnergiedienstOe(db, CrystalReportViewer1, Global.DecodeFrom64(Session["oe_enc"].ToString()));

                        }

                        if (alias == "EnergiedienstStatusMbO")
                        {

                            if (!IsPostBack || !ExistTmpTable("EnergiedienstStatusMbO"))
                            {
                                db.connect();
                                // delete temporary table if exists
                                string tbl_del = " IF NOT OBJECT_ID(N'tempdb..##EnergiedienstStatusMbO_%userid%') IS NULL "
                                               + "DROP TABLE ##EnergiedienstStatusMbO_%userid%";
                                db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                                //create and fill temporary table
                                string tbl_create = "CREATE TABLE [##EnergiedienstStatusMbO_%userid%]("
                               + "[OBJECTIVEID] [bigint] NULL,"
                               + "[CURRENTVALUE] [varchar](128) NULL,"
                               + "[VALUEIMPLICIT] [int] NULL,"
                               + "[STARTDATE] [datetime] NULL,"
                               + "[STATEDATE] [datetime] NULL,"
                               + "[VIEWED] [datetime] NULL,"
                               + "[INTERVIEW_DONE] [datetime] NULL,"
                               + "[PERSONID] [bigint] NULL,"
                               + "[PNAME] [varchar](64) NULL,"
                               + "[FIRSTNAME] [varchar](64) NULL,"
                               + "[PERSONNELNUMBER][varchar](64) NULL,"
                               + "[ORGENTITYID]  [bigint] NULL,"
                               + "[ORGENTITY] [varchar](128) NULL,"
                               + "[OBJECTIVE_TURN_ID] [bigint] NULL,"
                               + "[TURN] [varchar](128) NULL,"
                               + "[RATINGTOT] [float] NULL,"
                               + "[MNEMONIC_DE] [varchar](20) NULL"
                               + ") ON [PRIMARY]";
                                db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                                db.execute("INSERT INTO ##EnergiedienstStatusMbO_" + db.userId.ToString() + " SELECT OBJECTIVE.ID AS ObjectveId, OBJECTIVE.CURRENTVALUE, OBJECTIVE.VALUEIMPLICIT, OBJECTIVE.STARTDATE, OBJECTIVE.STATEDATE, "
                                                                                                                                      + "OBJECTIVE.VIEWED, OBJECTIVE.INTERVIEW_DONE, PERSON.ID AS PersonId, PERSON.PNAME, PERSON.FIRSTNAME, "
                                                                                                                                      + "PERSON.PERSONNELNUMBER, ORGENTITY.ID AS OrgentityId, ORGENTITY.TITLE_DE AS Orgentity, OBJECTIVE.OBJECTIVE_TURN_ID, "
                                                                                                                                      + "OBJECTIVE_TURN.TITLE_DE AS Turn, SUM(OBJECTIVE_PERSON_RATING.RATING * OBJECTIVE_PERSON_RATING.RATING_WEIGHT / 100) AS RatingTot, "
                                                                                                                                      + "ORGENTITY.MNEMONIC_DE "
                                                                                                                + "FROM EMPLOYMENT INNER JOIN "
                                                                                                                                      + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                                                                                                      + "PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID INNER JOIN "
                                                                                                                                      + "OBJECTIVE_TURN INNER JOIN "
                                                                                                                                      + "OBJECTIVE ON OBJECTIVE_TURN.ID = OBJECTIVE.OBJECTIVE_TURN_ID INNER JOIN "
                                                                                                                                      + "OBJECTIVE_PERSON_RATING INNER JOIN "
                                                                                                                                      + "OBJECTIVE AS OBJECTIVE_1 ON OBJECTIVE_PERSON_RATING.OBJECTIVE_ID = OBJECTIVE_1.ID ON "
                                                                                                                                      + "OBJECTIVE_TURN.ID = OBJECTIVE_1.OBJECTIVE_TURN_ID AND OBJECTIVE.PERSON_ID = OBJECTIVE_1.PERSON_ID ON "
                                                                                                                                      + "PERSON.ID = OBJECTIVE.PERSON_ID INNER JOIN "
                                                                                                                                      + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID "
                                                                                                                + "WHERE (JOB.ID  " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND (ORGENTITY.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ")) AND (OBJECTIVE.OBJECTIVE_TURN_ID = '" + Global.DecodeFrom64(Request.QueryString["param1"]) + "') AND "
                                                                                                                                      + "(NOT (OBJECTIVE.STARTDATE IS NULL)) OR "
                                                                                                                                      + "(OBJECTIVE.OBJECTIVE_TURN_ID IS NULL)) "
                                                                                                                + "GROUP BY OBJECTIVE.ID, OBJECTIVE.CURRENTVALUE, OBJECTIVE.VALUEIMPLICIT, OBJECTIVE.STARTDATE, OBJECTIVE.STATEDATE, "
                                                                                                                                      + "OBJECTIVE.VIEWED, OBJECTIVE.INTERVIEW_DONE, PERSON.ID, PERSON.PNAME, PERSON.FIRSTNAME, PERSON.PERSONNELNUMBER, "
                                                                                                                                      + "ORGENTITY.ID, ORGENTITY.TITLE_DE, OBJECTIVE.OBJECTIVE_TURN_ID, OBJECTIVE_TURN.TITLE_DE, ORGENTITY.MNEMONIC_DE "
                                                                                                                + "UNION "
                                                                                                                + "SELECT DISTINCT '' AS ObjectveId, '' AS Turn, '' AS RatingTot, '' AS Expr1, '' AS Expr2, '' AS Expr3, '' AS Expr4, PERSON.ID AS PersonId, PERSON.PNAME, "
                                                                                                                                      + "PERSON.FIRSTNAME, PERSON.PERSONNELNUMBER, ORGENTITY.ID AS OrgentityId, ORGENTITY.TITLE_DE AS Orgentity, "
                                                                                                                                      + " '' AS OBJECTIVE_TURN_ID, '' AS Expr5, '' AS Expr6, ORGENTITY.MNEMONIC_DE "
                                                                                                                + "FROM OBJECTIVE_TURN INNER JOIN "
                                                                                                                                      + "OBJECTIVE ON OBJECTIVE_TURN.ID = OBJECTIVE.OBJECTIVE_TURN_ID RIGHT OUTER JOIN "
                                                                                                                                      + "ORGENTITY INNER JOIN "
                                                                                                                                      + "EMPLOYMENT INNER JOIN "
                                                                                                                                      + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                                                                                                      + "PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID ON ORGENTITY.ID = JOB.ORGENTITY_ID ON "
                                                                                                                                      + "OBJECTIVE.PERSON_ID = PERSON.ID "
                                                                                                                + "WHERE (JOB.ID  " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND (ORGENTITY.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ")) AND (NOT (PERSON.ID IN "
                                                                                                                                          + "(SELECT PERSON_1.ID "
                                                                                                                                            + "FROM EMPLOYMENT AS EMPLOYMENT_1 INNER JOIN "
                                                                                                                                                                   + "JOB AS JOB_1 ON EMPLOYMENT_1.ID = JOB_1.EMPLOYMENT_ID INNER JOIN "
                                                                                                                                                                   + "PERSON AS PERSON_1 ON EMPLOYMENT_1.PERSON_ID = PERSON_1.ID INNER JOIN "
                                                                                                                                                                   + "OBJECTIVE_TURN AS OBJECTIVE_TURN_1 INNER JOIN "
                                                                                                                                                                   + "OBJECTIVE AS OBJECTIVE_2 ON OBJECTIVE_TURN_1.ID = OBJECTIVE_2.OBJECTIVE_TURN_ID INNER JOIN "
                                                                                                                                                                   + "OBJECTIVE_PERSON_RATING AS OBJECTIVE_PERSON_RATING_1 INNER JOIN "
                                                                                                                                                                   + "OBJECTIVE AS OBJECTIVE_1 ON OBJECTIVE_PERSON_RATING_1.OBJECTIVE_ID = OBJECTIVE_1.ID ON "
                                                                                                                                                                   + "OBJECTIVE_TURN_1.ID = OBJECTIVE_1.OBJECTIVE_TURN_ID AND OBJECTIVE_2.PERSON_ID = OBJECTIVE_1.PERSON_ID ON "
                                                                                                                                                                   + "PERSON_1.ID = OBJECTIVE_2.PERSON_ID INNER JOIN "
                                                                                                                                                                   + "ORGENTITY AS ORGENTITY_1 ON JOB_1.ORGENTITY_ID = ORGENTITY_1.ID "
                                                                                                                                            + "WHERE (JOB_1.ID " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND (ORGENTITY_1.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ")) AND (OBJECTIVE_2.OBJECTIVE_TURN_ID = '" + Global.DecodeFrom64(Request.QueryString["param1"]) + "') AND "
                                                                                                                                                                   + "(NOT (OBJECTIVE_2.STARTDATE IS NULL)) OR "
                                                                                                                                                                   + "(OBJECTIVE_2.OBJECTIVE_TURN_ID IS NULL)) "
                                                                                                                                            + "GROUP BY PERSON_1.ID, PERSON_1.PNAME)))) "
                                                                                                                + "GROUP BY OBJECTIVE.ID, OBJECTIVE.CURRENTVALUE, OBJECTIVE.VALUEIMPLICIT, OBJECTIVE.STARTDATE, OBJECTIVE.STATEDATE, "
                                                                                                                                      + "OBJECTIVE.VIEWED, OBJECTIVE.INTERVIEW_DONE, PERSON.ID, PERSON.PNAME, PERSON.FIRSTNAME, PERSON.PERSONNELNUMBER, "
                                                                                                                                      + "ORGENTITY.ID, ORGENTITY.TITLE_DE, OBJECTIVE.OBJECTIVE_TURN_ID, OBJECTIVE_TURN.TITLE_DE, ORGENTITY.MNEMONIC_DE");

                            }
                            SetTableName("##EnergiedienstStatusMbO_" + db.userId.ToString());
                            changeLogoEnergiedienst.setLogoEnergiedienstOe(db, CrystalReportViewer1, Global.DecodeFrom64(Session["oe_enc"].ToString()));

                        }

                        if (alias == "EnergiedienstRatingKompetenzbeurteilung")
                        {
                            if (!IsPostBack || !ExistTmpTable("EnergiedienstRatingKompetenzbeurteilung"))
                            {
                                db.connect();
                                // delete temporary table if exists
                                string tbl_del = " IF NOT OBJECT_ID(N'tempdb..##EnergiedienstRatingKompetenzbeurteilung_%ratingid%') IS NULL "
                                               + "DROP TABLE ##EnergiedienstRatingKompetenzbeurteilung_%ratingid%";
                                db.execute(tbl_del.Replace("%ratingid%", db.userId.ToString()));

                                //create and fill temporary table
                                string tbl_create = "CREATE TABLE [##EnergiedienstRatingKompetenzbeurteilung_%userid%]("
                               + "[PERSONNELNUMBER] [varchar](64) NULL,"
                               + "[FIRSTNAME] [varchar](64) NULL,"
                               + "[NAME] [varchar](64) NULL,"
                               + "[PERSON_GROUP_TITLE] [varchar](256) NULL,"
                               + "[PERSON_CYRCLE_TITLE] [varchar](256) NULL,"
                               + "[JOB] [varchar](128) NULL,"
                               + "[ORGENTITYID] [bigint] NULL,"
                               + "[ORGENTITY] [varchar](128) NULL,"
                               + "[MNEMONIC_DE] [varchar](128) NULL,"
                               + "[RATING_DATE] [datetime] NULL,"
                               + "[VIEWED] [datetime] NULL,"
                               + "[INTERVIEW_DONE] [datetime] NULL,"
                               + "[PERFORMANCE_YEAR] [int] NULL,"
                               + "[RATING] [float] NULL"
                               + ") ON [PRIMARY]";
                                db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                                db.execute("INSERT INTO ##EnergiedienstRatingKompetenzbeurteilung_" + db.userId.ToString() + " SELECT PERSON.PERSONNELNUMBER, PERSON.FIRSTNAME, PERSON.PNAME AS Name, PERSON.PERSON_GROUP_TITLE, PERSON.PERSON_CIRCLE_TITLE, JOB.TITLE_DE AS Job, ORGENTITY.ID AS OrgentityId, "
                                                                                                                               + "ORGENTITY.TITLE_DE AS Orgentity, ORGENTITY.MNEMONIC_DE, PERFORMANCERATING.RATING_DATE, PERFORMANCERATING.VIEWED, "
                                                                                                                               + "PERFORMANCERATING.INTERVIEW_DONE, PERFORMANCERATING.PERFORMANCE_YEAR, CASE COUNT(PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF) "
                                                                                                                                + "WHEN 7 THEN (SELECT ROUND(AVG(RELATIV_WEIGHT/20),0) FROM PERFORMANCERATING_ITEMS AS TEMPDB WHERE TEMPDB.PERFORMANCERATING_REF =  PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF AND TEMPDB.CRITERIA_REF = 533) * 0.4 + "
                                                                                                                                            + "(SELECT ROUND(AVG(RELATIV_WEIGHT/20),0) FROM PERFORMANCERATING_ITEMS AS TEMPDB WHERE TEMPDB.PERFORMANCERATING_REF =  PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF AND TEMPDB.CRITERIA_REF = 537) * 0.3 + "
                                                                                                                                            + "(SELECT ROUND(AVG(RELATIV_WEIGHT/20),0) FROM PERFORMANCERATING_ITEMS AS TEMPDB WHERE TEMPDB.PERFORMANCERATING_REF =  PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF AND TEMPDB.CRITERIA_REF = 541) * 0.3 "
                                                                                                                                + "WHEN 11 THEN (SELECT ROUND(AVG(RELATIV_WEIGHT/20),0) FROM PERFORMANCERATING_ITEMS AS TEMPDB WHERE TEMPDB.PERFORMANCERATING_REF =  PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF AND TEMPDB.CRITERIA_REF = 533) * 0.3 + "
                                                                                                                                            + "(SELECT ROUND(AVG(RELATIV_WEIGHT/20),0) FROM PERFORMANCERATING_ITEMS AS TEMPDB WHERE TEMPDB.PERFORMANCERATING_REF =  PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF AND TEMPDB.CRITERIA_REF = 537) * 0.2 +  "
                                                                                                                                            + "(SELECT ROUND(AVG(RELATIV_WEIGHT/20),0) FROM PERFORMANCERATING_ITEMS AS TEMPDB WHERE TEMPDB.PERFORMANCERATING_REF =  PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF AND TEMPDB.CRITERIA_REF = 541) * 0.2+ "
                                                                                                                                            + "(SELECT ROUND(AVG(RELATIV_WEIGHT/20),0) FROM PERFORMANCERATING_ITEMS AS TEMPDB WHERE TEMPDB.PERFORMANCERATING_REF =  PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF AND TEMPDB.CRITERIA_REF = 545) * 0.3	 "
                                                                                                                                + "END "
                                                                                                                         + "AS Rating "
                                                                                                                        + "FROM   PERSON INNER JOIN "
                                                                                                                               + "EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                                                                                               + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                                                                                               + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN "
                                                                                                                               + "PERFORMANCERATING ON PERSON.ID = PERFORMANCERATING.PERSON_ID INNER JOIN "
                                                                                                                               + "PERFORMANCERATING_ITEMS ON PERFORMANCERATING.ID = PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF "
                                                                                                                               + "WHERE ORGENTITY.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ") AND JOB.ID " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND ((PERFORMANCERATING.RATING_DATE >= '" + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param1"])).ToString("yyyy.MM.dd") + "' AND PERFORMANCERATING.RATING_DATE <= '" + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param2"])).ToString("yyyy.MM.dd") + "')) "
                                                                                                                        + "GROUP BY PERSON.PERSONNELNUMBER, PERSON.FIRSTNAME, PERSON.PNAME, PERSON.PERSON_GROUP_TITLE, PERSON.PERSON_CIRCLE_TITLE, JOB.TITLE_DE, ORGENTITY.ID, ORGENTITY.TITLE_DE, "
                                                                                                                               + "ORGENTITY.MNEMONIC_DE, PERFORMANCERATING.RATING_DATE, PERFORMANCERATING.VIEWED, PERFORMANCERATING.INTERVIEW_DONE, "
                                                                                                                               + "PERFORMANCERATING.PERFORMANCE_YEAR, PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF ");
                            }
                            SetTableName("##EnergiedienstRatingKompetenzbeurteilung_" + db.userId.ToString());
                            changeLogoEnergiedienst.setLogoEnergiedienstOe(db, CrystalReportViewer1, Global.DecodeFrom64(Session["oe_enc"].ToString()));

                        }

                        if (alias == "EnergiedienstStatusPersonalentwicklungsbedarf")
                        {
                            if (!IsPostBack || !ExistTmpTable("EnergiedienstStatusPersonalentwicklungsbedarf"))
                            {
                                db.connect();

                                // delete temporary table if exists
                                string tbl_del = " IF NOT OBJECT_ID(N'tempdb..##EnergiedienstStatusPersonalentwicklungsbedarf_%userid%') IS NULL "
                                                   + "DROP TABLE ##EnergiedienstStatusPersonalentwicklungsbedarf_%userid%";
                                db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                                //create and fill temporary table
                                string tbl_create = "CREATE TABLE [##EnergiedienstStatusPersonalentwicklungsbedarf_%userId%]("
                                + "[PERSONNELNUMBER] [varchar](64) NULL,"
                                + "[FIRSTNAME] [varchar](64) NULL,"
                                + "[NAME] [varchar](64) NULL,"
                                + "[ORGENTITY] [varchar](128) NULL,"
                                + "[MNEMONIC_DE] [varchar](128) NULL,"
                                + "[VALID_FROM] [datetime] NULL,"
                                + "[RELEASE] [datetime] NULL,"
                                + "[CHECKED] [datetime] NULL"
                                + ") ON [PRIMARY]";
                                db.execute(tbl_create.Replace("%userId%", db.userId.ToString()));

                                db.execute("INSERT INTO ##EnergiedienstStatusPersonalentwicklungsbedarf_" + db.userId.ToString() + " SELECT PERSON.PERSONNELNUMBER, PERSON.FIRSTNAME, PERSON.PNAME AS NAME, ORGENTITY.TITLE_DE AS ORGENTITY, ORGENTITY.MNEMONIC_DE, TRAINING_ADVANCEMENT.CREATED AS VALID_FROM, TRAINING_ADVANCEMENT.RELEASE, TRAINING_ADVANCEMENT.VIEWED AS CHECKED "
                                                    + "FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID LEFT OUTER JOIN TRAINING_ADVANCEMENT ON PERSON.ID = TRAINING_ADVANCEMENT.PERSON_ID "
                                                    + "WHERE JOB.ID " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND JOB.HAUPTFUNKTION = 1 AND (ORGENTITY.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ") AND ((TRAINING_ADVANCEMENT.CREATED >= '" + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param1"])).ToString("yyyy.MM.dd") + "' AND TRAINING_ADVANCEMENT.CREATED <= '" + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param2"])).ToString("yyyy.MM.dd") + "'))) "
                                                    + "UNION "
                                                    + "SELECT PERSON.PERSONNELNUMBER, PERSON.FIRSTNAME, PERSON.PNAME AS NAME, ORGENTITY.TITLE_DE AS ORGENTITY, ORGENTITY.MNEMONIC_DE, NULL AS VALID_FROM, NULL AS RELEASE, NULL AS CHECKED "
                                                    + "FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID LEFT OUTER JOIN TRAINING_ADVANCEMENT ON PERSON.ID = TRAINING_ADVANCEMENT.PERSON_ID "
                                                    + "WHERE JOB.ID " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND JOB.HAUPTFUNKTION = 1 AND (ORGENTITY.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ") AND NOT JOB.ID IN(SELECT JOB.ID FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID LEFT OUTER JOIN TRAINING_ADVANCEMENT ON PERSON.ID = TRAINING_ADVANCEMENT.PERSON_ID "
                                                    + "WHERE JOB.ID " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND JOB.HAUPTFUNKTION = 1 AND (ORGENTITY.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ") AND ((TRAINING_ADVANCEMENT.CREATED >= '" + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param1"])).ToString("yyyy.MM.dd") + "' AND TRAINING_ADVANCEMENT.CREATED <= '" + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param2"])).ToString("yyyy.MM.dd") + "')))))");
                            }

                            SetTableName("##EnergiedienstStatusPersonalentwicklungsbedarf_" + db.userId.ToString());
                            changeLogoEnergiedienst.setLogoEnergiedienstOe(db, CrystalReportViewer1, Global.DecodeFrom64(Session["oe_enc"].ToString()));

                        }

                        if (alias == "EnergiedienstStatusAufgabenbeschreibung")
                        {
                            if (!IsPostBack || !ExistTmpTable("EnergiedienstStatusAufgabenbeschreibung"))
                            {
                                db.connect();

                                // delete temporary table if exists
                                string tbl_del = " IF NOT OBJECT_ID(N'tempdb..##EnergiedienstStatusAufgabenbeschreibung_%userId%') IS NULL "
                                                  + "DROP TABLE ##EnergiedienstStatusAufgabenbeschreibung_%userId%";

                                db.execute(tbl_del.Replace("%userId%", db.userId.ToString()));

                                //create and fill temporary table
                                string tbl_create = "CREATE TABLE [##EnergiedienstStatusAufgabenbeschreibung_%userid%]("
                                + "[PERSONNELNUMBER] [varchar](64) NULL,"
                                + "[FIRSTNAME] [varchar](64) NULL,"
                                + "[NAME] [varchar](64) NULL,"
                                + "[ORGENTITY] [varchar](128) NULL,"
                                + "[MNEMONIC_DE] [varchar](128) NULL,"
                                + "[VALID_FROM] [datetime] NULL,"
                                + "[RELEASE] [datetime] NULL,"
                                + "[CHECKED] [datetime] NULL"
                                + ") ON [PRIMARY]";
                                db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                                db.execute("INSERT INTO ##EnergiedienstStatusAufgabenbeschreibung_" + db.userId.ToString() + " SELECT PERSON.PERSONNELNUMBER, PERSON.FIRSTNAME, PERSON.PNAME AS Name, ORGENTITY.TITLE_DE AS Orgentity, "
                                                    + "ORGENTITY.MNEMONIC_DE, DUTY_VALIDITY.VALID_FROM, JOB.JOB_DESCRIPTION_RELEASE, "
                                                    + "JOB.JOB_DESCRIPTION_CHECKED "
                                                + "FROM PERSON INNER JOIN "
                                                    + "EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                    + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                    + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID RIGHT OUTER JOIN "
                                                    + "DUTY_VALIDITY ON DUTY.ID = (SELECT DUTY_ID FROM DUTY_COMETENCE_VALIDITY WHERE JOB_ID = JOB.ID)"
                                                + "WHERE JOB.ID " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND(ORGENTITY.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ") "
                                                    + "AND ((DUTY_VALIDITY.VALID_FROM >= '"
                                                    + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param1"])).ToString("yyyy.MM.dd")
                                                    + "' AND DUTY_VALIDITY.VALID_FROM <= '" + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param2"])).ToString("yyyy.MM.dd") + "'))) "
                                            + "UNION "

                                            + "SELECT distinct  PERSON.PERSONNELNUMBER, PERSON.FIRSTNAME, PERSON.PNAME AS Name, ORGENTITY.TITLE_DE AS Orgentity, "
                                                    + "ORGENTITY.MNEMONIC_DE, '' AS VALID_FROM, NULL AS_RELEASE, "
                                                    + "NULL AS JOB_DESCRIPTION_CHECKED "
                                                + "FROM PERSON INNER JOIN "
                                                    + "EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                    + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                    + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID, DUTY_COMPETENCE_VALIDITY "
                                                + "WHERE JOB.ID " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND(ORGENTITY.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ")) AND  "
                                                    + "NOT JOB.ID IN (SELECT JOB_ID FROM DUTY_VALIDITY) AND "
                                                    + " NOT JOB.ID IN(SELECT JOB.ID  FROM PERSON INNER JOIN "
                                                    + "EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                    + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                    + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID RIGHT OUTER JOIN "
                                                     + "DUTY_VALIDITY ON DUTY.ID = (SELECT DUTY_ID FROM DUTY_COMETENCE_VALIDITY WHERE JOB_ID = JOB.ID)"
                                                + "WHERE JOB.ID " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND(ORGENTITY.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ") "
                                                    + "AND ((DUTY_VALIDITY.VALID_FROM >= '"
                                                    + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param1"])).ToString("yyyy.MM.dd")
                                                    + "' AND DUTY_VALIDITY.VALID_FROM <= '" + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param2"])).ToString("yyyy.MM.dd") + "'))))");


                            }
                            SetTableName("##EnergiedienstStatusAufgabenbeschreibung_" + db.userId.ToString());
                            changeLogoEnergiedienst.setLogoEnergiedienstOe(db, CrystalReportViewer1, Global.DecodeFrom64(Session["oe_enc"].ToString()));
                        }

                        if (alias == "KompetenzbewertungStatusEnergiedienst")
                        {
                            if (!IsPostBack || !ExistTmpTable("KompetenzbewertungStatusEnergiedienst"))
                            {
                                db.connect();

                                // delete temporary table if exists

                                string tbl_del = " IF NOT OBJECT_ID(N'tempdb..##KompetenzbewertungStatusEnergiedienst_%userid%') IS NULL "
                             + "DROP TABLE ##KompetenzbewertungStatusEnergiedienst_%userid%";
                                db.connect();
                                db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                                //create and fill temporary table
                                string tbl_create = "CREATE TABLE dbo.[##KompetenzbewertungStatusEnergiedienst_%userid%]("
                                + "[PERSONNELNUMBER] [varchar](64) NULL,"
                                + "[FIRSTNAME] [varchar](64) NULL,"
                                + "[NAME] [varchar](64) NULL,"
                                + "[JOB] [varchar](128) NULL,"
                                + "[ORGENTITYID] [bigint] NULL,"
                                + "[ORGENTITY] [varchar](128) NULL,"
                                + "[MNEMONIC_DE] [varchar](128) NULL,"
                                + "[RATING_DATE] [datetime] NULL,"
                                + "[VIEWED] [datetime] NULL,"
                                + "[INTERVIEW_DONE] [datetime] NULL,"
                                + "[PERFORMANCE_YEAR] [int] NULL"
                                + ") ON [PRIMARY]";
                                db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                                db.execute("INSERT INTO ##KompetenzbewertungStatusEnergiedienst_" + db.userId.ToString() + " SELECT PERSON.PERSONNELNUMBER, PERSON.FIRSTNAME, PERSON.PNAME AS Name, JOB.TITLE_DE AS Job, ORGENTITY.ID AS OrgentityId, "
                                                                                                                       + "ORGENTITY.TITLE_DE AS Orgentity, ORGENTITY.MNEMONIC_DE, PERFORMANCERATING.RATING_DATE, PERFORMANCERATING.VIEWED, "
                                                                                                                       + "PERFORMANCERATING.INTERVIEW_DONE, PERFORMANCERATING.PERFORMANCE_YEAR "
                                                                                                                + "FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                                                                                       + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                                                                                       + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID LEFT OUTER JOIN "
                                                                                                                       + "PERFORMANCERATING ON PERSON.ID = PERFORMANCERATING.PERSON_ID "
                                                                                                                       + "WHERE JOB.ID " + Global.DecodeFrom64(Session["jobSql_en"].ToString()) + " AND ORGENTITY.ID IN (" + Global.DecodeFrom64(Session["oe_enc"].ToString()) + ") AND ((PERFORMANCERATING.RATING_DATE >= '" + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param1"])).ToString("yyyy.MM.dd") + "' AND PERFORMANCERATING.RATING_DATE <= '" + Convert.ToDateTime(Global.DecodeFrom64(Request.QueryString["param2"])).ToString("yyyy.MM.dd") + "' OR PERSON.ID NOT IN (SELECT PERSON_ID FROM PERFORMANCERATING)))");



                            }
                            SetTableName("##KompetenzbewertungStatusEnergiedienst_" + db.userId.ToString());
                            changeLogoEnergiedienst.setLogoEnergiedienstOe(db, CrystalReportViewer1, Global.DecodeFrom64(Session["oe_enc"].ToString()));

                        }

                        if (alias == "PersonalentwicklungsbedarfEnergiedienst")
                        {
                            SetParameter("AdvacementId", Request.QueryString["param0"]);
                            SetParameter("LeaderName", Request.QueryString["param1"]);
                            ChangeLogo changeLogo = new ChangeLogo();
                            SetParameter("LogoPath", Global.Config.logoImageDirectory + "\\" + changeLogo.getLogoFilename(db, Convert.ToInt32(Request.QueryString["param2"])));
                        }

                        if (alias == "Leistungswerte")
                        {

                            SetParameter("RatingBase", Convert.ToInt32(Global.Config.getModuleParam("performance", "performanceRatingBase", "0")));
                            if (!IsPostBack)
                            {
                                string orgenties = Global.DecodeFrom64(Request.QueryString["param0"]);
                                string dateOf = Global.DecodeFrom64(Request.QueryString["param1"]);
                                string dateTo = Global.DecodeFrom64(Request.QueryString["param2"]);
                                db.connect();

                                // delete temporary table if exists

                                string tbl_del = " IF NOT OBJECT_ID(N'tempdb..##LeistungswerteMa_%userid%') IS NULL "
                                + "DROP TABLE ##LeistungswerteMa_%userid%";
                                db.connect();
                                db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                                //create and fill temporary table
                                string tbl_create = "CREATE TABLE dbo.[##LeistungswerteMa_%userid%]("
                                + "[PERSONNELNUMBER] [varchar](64) NULL,"
                                + "[NAME] [varchar](64) NULL,"
                                + "[FIRSTNAME] [varchar](64) NULL,"
                                + "[JOB] [varchar](128) NULL,"
                                + "[COSTCENTER] [varchar](64) NULL,"
                                + "[ORGENTITY] [varchar](128) NULL,"
                                + "[RATING_DATE] [datetime] NULL,"
                                + "[RATING] [float] NULL,"
                                + "[JOB_ID] [bigint] NULL,"
                                + "[OE_ID] [bigint] NULL"
                                + ") ON [PRIMARY]";
                                db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                                db.execute("INSERT INTO ##LeistungswerteMa_" + db.userId.ToString() + " SELECT PERSON.PERSONNELNUMBER AS [P-Nr.], PERSON.PNAME AS Name, PERSON.FIRSTNAME AS Vorname, JOB.TITLE_DE, "
                                                                                                           + "PERSON.COSTCENTER AS Kst, ORGENTITY.TITLE_DE AS Abteilung, SolllohnLeistungswert.Bewertungsdatum, SolllohnLeistungswert.Leistungswert, "
                                                                                                           + "JOB.ID AS Job, ORGENTITY.ID AS Oe_Id "
                                                                                                        + "FROM PERSON INNER JOIN "
                                                                                                           + "EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                                                                           + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                                                                           + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN "
                                                                                                           + "SolllohnLeistungswert ON PERSON.ID = SolllohnLeistungswert.PERSON_ID "
                                                                                                        + "WHERE SolllohnLeistungswert.Leistungswert > 0 AND ORGENTITY.ID IN (" + orgenties + ") AND  SolllohnLeistungswert.Bewertungsdatum >= '" + Convert.ToDateTime(dateOf).ToString("yyyy.MM.dd") + "' AND  SolllohnLeistungswert.Bewertungsdatum <= '" + Convert.ToDateTime(dateTo).ToString("yyyy.MM.dd") + "'");
                            }

                            SetTableName("##LeistungswerteMa_" + db.userId.ToString());
                        }

                        if (alias == "LeistungswerteFoamPartner")
                        {
                            SetParameter("ratingBase", 60);

                        }

                        if (alias == "PersonWithoutPerformancerating")
                        {
                            ChangeReportTable("PersonWithoutPerformancerating_" + db.userId.ToString(), connectionInfo, rpt1);
                            ParameterFields fields = CrystalReportViewer1.ParameterFieldInfo;

                            if (Request.QueryString["From"] != null && Request.QueryString["From"] != "")
                            {
                                SetParameter("from", Request.QueryString["From"]);
                            }

                            if (Request.QueryString["To"] != null && Request.QueryString["To"] != "")
                            {
                                SetParameter("to", Request.QueryString["To"]);
                            }
                        }

                        // fill temporary table for LeistungswerteOE
                        if (alias == "LeistungswerteOE")
                        {
                            db.connect();

                            // delete temporary table if exists
                            string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeistungsbewertungenOe_%userid%]') "
                                              + "AND type in (N'U')) "
                                              + "DROP TABLE [dbo].[LeistungsbewertungenOe_%userid%]";
                            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                            //create and fill temporary table
                            string tbl_create = "CREATE TABLE [dbo].[LeistungsbewertungenOe_%userid%]("
                            + "[Personalnummer] [varchar](64) NULL,"
                            + "[Nachname] [varchar](64) NOT NULL,"
                            + "[Vorname] [varchar](64) NULL,"
                            + "[Austritt] [datetime] NULL,"
                            + "[Stellenbezeichnung] [varchar](128) NOT NULL,"
                            + "[Leistunsbewertung_REF] [bigint] NULL,"
                            + "[DatumLeistungsbewertung] [datetime] NOT NULL,"
                            + "[Leistungswert] [float] NOT NULL,"
                            + "[OE] [varchar](128) NOT NULL,"
                            + "[ID] [bigint] NOT NULL"
                            + ") ON [PRIMARY]";
                            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                            string performanceRatingBase = Global.Config.getModuleParam("performance", "performanceRatingBase", "40");
                            db.execute("INSERT INTO LeistungsbewertungenOe_" + db.userId.ToString() + " SELECT * FROM f_LeistungsbewertungenOe(" + performanceRatingBase + ")");

                            db.disconnect();

                            // set tablename and db logon
                            ChangeReportTable("LeistungsbewertungenOe_" + db.userId.ToString(), connectionInfo, rpt1);
                        }

                        if (alias == "DurchschnittlicheLeistungsanteile")
                        {
                            SetParameter("MaxLeistungsanteil", Request.QueryString["performanceRatingBase"].ToString());
                        }

                        if (alias == "MbOAbteilung" || alias == "MbOAbteilungDetail" || alias == "MbOUnternehmensziel" || alias == "MbOUnternehmenszielDetail")
                        {
                            ParameterFields fields = CrystalReportViewer1.ParameterFieldInfo;
                            if (alias == "MbOAbteilung" || alias == "MbOAbteilungDetail")
                            {
                                SetParameter("OrgentityId", this.Session["param0"].ToString());
                            }
                            else
                            {
                                SetParameter("FirmObjectiveId", this.Session["param0"].ToString());
                                SetParameter("FirmObjectiveTitle", this.Session["param3"].ToString());
                            }
                            SetParameter("JobId", this.Session["param2"].ToString());
                            SetParameter("TurnId", this.Session["param1"].ToString());
                        }

                        // fill temporary table for Solllohn
                        if (alias == "Solllohn")
                        {
                            db.connect();

                            // fill temporary person table which contains only persons in selected OEs
                            // first delete all entries in table
                            // currently Solllohn has all persons selected
                            db.execute("DELETE FROM PERSON_temp");

                            // fill table
                            db.execute("INSERT INTO PERSON_temp "
                            + "SELECT dbo.PERSON.ID, dbo.PERSON.EXTERNAL_REF, dbo.PERSON.PNAME, "
                            + "dbo.PERSON.FIRSTNAME, dbo.PERSON.PERSONNELNUMBER, dbo.PERSON.DATEOFBIRTH, "
                            + "dbo.PERSON.ENTRY, dbo.PERSON.LEAVING, dbo.PERSON.TYP, dbo.PERSON.BERUFSERFAHRUNG, "
                            + "dbo.PERSON.COSTCENTER, dbo.PERSON.COSTCENTER_TITLE, dbo.PERSON.BESCH_GRAD, dbo.PERSON.SALUTATION_ADDRESS, dbo.PERSON.SALUTATION_LETTER  "
                            + "FROM dbo.PERSON");

                            // delete temporary table if exists
                            string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[solllohn_%userid%]') "
                                              + "AND type in (N'U')) "
                                              + "DROP TABLE [dbo].[solllohn_%userid%]";
                            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                            //create and fill temporary table
                            string tbl_create = "CREATE TABLE [dbo].[solllohn_%userid%]("
                            + "[Personalnummer] [varchar](64) NULL,"
                            + "[Name] [varchar](64) NULL,"
                            + "[Vorname] [varchar](64) NULL,"
                            + "[Geburtsdatum] [datetime] NULL,"
                            + "[BERUFSERFAHRUNG] [int] NULL,"
                            + "[OeID] [int] NULL,"
                            + "[Oe-Name] [varchar](128) NULL,"
                            + "[Kostenstelle] [varchar](64) NULL,"
                            + "[Eintritt] [datetime] NULL,"
                            + "[Austritt] [datetime] NULL,"
                            + "[Anrede] [varchar](64) NULL,"
                            + "[Anrede Brief] [varchar](128) NULL,"
                            + "[Adresse1] [varchar](128) NULL,"
                            + "[Adresse2] [varchar](128) NULL,"
                            + "[Postleitzahl] [varchar](32) NULL,"
                            + "[Ort] [varchar](64) NULL,"
                            + "[IstLohn] [float] NULL,"
                            + "[Funktionsbewertung] [float] NULL,"
                            + "[Ausschluss_Ab] [int] NULL,"
                            + "[AUSSCHLUSS_BIS] [datetime] NULL,"
                            + "[AUSSCHLUSS_LOHN] [int] NULL,"
                            + "[Stellenbezeichnung] [varchar](128) NULL,"
                            + "[ID] [bigint] NULL,"
                            + "[Bewertungsdatum] [datetime] NULL,"
                            + "[Leistungswert] [float] NULL,"
                            + "[Erfahrung] [numeric](3, 1) NULL,"
                            + "[ENGAGEMENT] [varchar](64) NULL,"
                            + "[PunkteInklErfLeist] [float] NULL,"
                            + "[SummePunkteInklErfLeist] [float] NULL,"
                            + "[LohnsummeIst] [float] NULL,"
                            + "[WertProPunkt] [float] NULL,"
                            + "[SollBasislohn] [float] NULL,"
                            + "[SollLeistung] [float] NULL,"
                            + "[SollErfahrung] [float] NULL,"
                            + "[Sollohn] [float] NULL,"
                            + "[fehler] [varchar](64) NULL"
                            + ") ON [PRIMARY]";
                            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                            string performanceRatingBase = Global.Config.getModuleParam("performance", "performanceRatingBase", "100");
                            string anzMonatsloehne = Global.Config.getModuleParam("report", "anzMonatsloehne", "13");
                            string maxErfahrung = Global.Config.getModuleParam("report", "maxErfahrung", "10");
                            string glaetten = Global.Config.getModuleParam("report", "glaetten", "0").ToString();
                            string fbwMin = "''";
                            string fbwMax = "''";
                            string lohnMin = "''";
                            string lohnMax = "''";

                            if (glaetten.Equals("1"))
                            {
                                fbwMin = (db.lookup("FbwMin", "FBW_LOHN_MIN_MAX_V", "")).ToString();
                                fbwMax = (db.lookup("FbwMax", "FBW_LOHN_MIN_MAX_V", "")).ToString();
                                lohnMin = Global.Config.getModuleParam("report", "minSolllohn", "0").ToString();
                                lohnMax = (db.lookup("LohnMax", "FBW_LOHN_MIN_MAX_V", "")).ToString();
                            }
                            db.execute("INSERT INTO solllohn_" + db.userId.ToString() + " SELECT * FROM f_Lohnsimulation3(" + performanceRatingBase + ", " + anzMonatsloehne + ", " + maxErfahrung + ", " + glaetten + ", " + fbwMin + ", " + fbwMax + ", " + lohnMin + ", " + lohnMax + " )", 600, 600);

                            // add columns for calculation
                            string tbl_alter = "ALTER TABLE solllohn_%userid% "
                                               + "ADD [AbwProzent] [float] NULL,"
                                               + "[AbwCHF] [float] NULL";
                            db.execute(tbl_alter.Replace("%userid%", db.userId.ToString()));

                            // calculate differences
                            db.execute("UPDATE solllohn_" + db.userId.ToString() + " SET AbwCHF = IstLohn - Sollohn");
                            db.execute("UPDATE solllohn_" + db.userId.ToString() + " SET AbwProzent = AbwCHF / IstLohn * 100");

                            db.disconnect();

                            // set tablename and db logon
                            ChangeReportTable("solllohn_" + db.userId.ToString(), connectionInfo, rpt1);
                        }

                        string sql = "";

                        //HACK: check rights for LeistungsbewertungenUnvollstaendig and Solllohn
                        if (alias == "Solllohn")
                        {
                            //check rights
                            long accessorID = SessionData.getUserAccessorID(Session);
                            string accessorSQL = db.getAccessorIDsSQLInClause(accessorID);
                            DataTable tblJobs = db.getDataTableExt("select distinct JOB.ID from JOB inner join ORGENTITY on JOB.ORGENTITY_ID=ORGENTITY.ID inner join ORGANISATION on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID and ORGANISATION.MAINORGANISATION=1 inner join ACCESS_RIGHT_RT on ACCESS_RIGHT_RT.TABLENAME='JOB' and (ACCESS_RIGHT_RT.ROW_ID=JOB.ID or ACCESS_RIGHT_RT.ROW_ID=0) and ACCESS_RIGHT_RT.APPLICATION_RIGHT=11 and (ACCESS_RIGHT_RT.AUTHORISATION&4)=4 and ACCESS_RIGHT_RT.ACCESSOR_ID  " + accessorSQL, new object[0]);
                            bool start = true;
                            foreach (DataRow aktJob in tblJobs.Rows)
                            {
                                if (start == true)
                                {
                                    start = false;
                                }
                                else
                                {
                                    sql += " OR ";
                                }

                                sql += "{Sollloehne.ID}=" + aktJob["ID"];

                            }
                        }
                        else
                        {
                            sql = ch.psoft.Util.Validate.GetValid(db.lookup("sql", "CRYSTALREPORTS", "alias='" + alias + "'", ""));
                        }

                        if (sql != "")
                        {
                            if (alias == "NegativeLeistungsentwicklung")
                            {
                                //sql = sql.Replace("%param0%", Session["NegativeLeistungsentwicklung_param0"].ToString());
                            }
                            else if (alias == "UnterdurchschnittlicheLeistungswerte")
                            {
                                //sql = sql.Replace("%param0%", Session["UnterdurchschnittlicheLeistungswerte_param0"].ToString());
                            }
                            else if (alias == "MbOAbteilung" || alias == "MbOAbteilungDetail" || alias == "MbOUnternehmensziel" || alias == "MbOUnternehmenszielDetail")
                            {
                                sql = sql.Replace("%param0%", this.Session["param0"].ToString());
                                sql = sql.Replace("%param1%", this.Session["param1"].ToString());
                                sql = sql.Replace("%param2%", this.Session["param2"].ToString());
                            }
                            else
                            {
                                sql = replaceParameters(sql);
                            }
                            CrystalReportViewer1.SelectionFormula = sql;
                        }

                        //HACK: submit parameters for Lohnsimulation
                        if (alias == "Lohnsimulation")
                        {
                            if (true || Session["WertPunkt"].ToString() != "0")
                            {
                                //SetParameter("Punktwert", Convert.ToDouble(Session["WertPunkt"].ToString()));
                                SetParameter("Punktwert", Convert.ToString(Session["WertPunkt"]));
                            }

                            if (true || Session["LohnsummenKorrRel"].ToString() != "0")
                            {
                                //SetParameter("LohnsummenKorrRel", Convert.ToDouble(Session["LohnsummenKorrRel"]));
                                SetParameter("LohnsummenKorrRel", Convert.ToString(Session["LohnsummenKorrRel"]));

                            }

                            if (true || Session["LohnsummenKorrAbs"].ToString() != "0")
                            {
                                SetParameter("LohnsummenKorrAbs", Convert.ToDouble(Session["LohnsummenKorrAbs"].ToString()));
                            }


                            if (true || Session["LohnaendAbs"].ToString() != "0")
                            {
                                SetParameter("LohnaendAbs", Convert.ToDouble(Session["LohnaendAbs"].ToString().Replace(",", ".")));
                            }

                            if (true || Session["LohnaendRel"].ToString() != "0")
                            {
                                SetParameter("LohnaendRel", Convert.ToDouble(Session["LohnaendRel"].ToString()));
                            }

                            if (true || Session["ErhoeBasislohn"].ToString() != "0")
                            {
                                SetParameter("ErhoeBasislohn", Convert.ToDouble(Session["ErhoeBasislohn"].ToString()));
                            }

                            SetParameter("ExportAdressen", Session["ExportAdresse"].ToString());
                            SetParameter("Runden", Session["Runden"].ToString());

                            // set tablename and db logon
                            ChangeReportTable("lohnsimulation_" + db.userId.ToString(), connectionInfo, rpt1);


                        }

                        //HACK: submit parameters for SimulationEinstelllohn
                        else if (alias == "SimulationEinstelllohn")
                        {
                            SetParameter("Name", Request.QueryString["name"]);
                            SetParameter("Vorname", Request.QueryString["vorname"]);
                            SetParameter("Leistungsanteil%", Convert.ToDouble(Request.QueryString["leistungsanteil"].ToString().Replace(",", ".")));
                            SetParameter("Lohnvorstellung", Convert.ToDouble(Request.QueryString["lohnvorstellung"].ToString().Replace(",", ".")));
                            SetParameter("Alter", Convert.ToDouble(Request.QueryString["alter"].ToString().Replace(",", ".")));
                            SetParameter("Erfahrung%", Convert.ToDouble(Request.QueryString["erfahrung"].ToString().Replace(",", ".")));
                            SetParameter("MaxLeistungsanteil%", Convert.ToDouble(Request.QueryString["maxleistungsanteil"].ToString().Replace(",", ".")));
                            // set tablename and db logon
                            ChangeReportTable("einstelllohn_" + db.userId.ToString(), connectionInfo, rpt1);
                        }

                        //HACK: submit parameters for unterdurchschnittliche Leistungswerte
                        else if (alias == "UnterdurchschnittlicheLeistungswerte" && Request.QueryString["startdat"] != null && Request.QueryString["startdat"] != "")
                        {
                            SetParameter("Startdatum", DateTime.Parse(Request.QueryString["startdat"]));
                            SetParameter("Enddatum", DateTime.Parse(Request.QueryString["enddat"]));
                            SetParameter("HalberMaxLeistungswert", Convert.ToInt32(Global.Config.getModuleParam("performance", "performanceRatingBase", "40")) / 2);

                            // set tablename and db logon
                            ChangeReportTable("leistungswerte_" + db.userId.ToString(), connectionInfo, rpt1);
                        }

                        //HACK: submit parameters for schlechtere Leistungswerte
                        else if (alias == "NegativeLeistungsentwicklung" && Request.QueryString["startdat"] != null && Request.QueryString["startdat"] != "")
                        {

                            SetParameter("LBFrom", DateTime.Parse(Request.QueryString["startdat"]));
                            SetParameter("LBTo", DateTime.Parse(Request.QueryString["enddat"]));
                            SetParameter("rangeLimit", Request.QueryString["rangeLimit"].ToString());

                            // set tablename and db logon
                            ChangeReportTable("leistungswerte_" + db.userId.ToString(), connectionInfo, rpt1);
                        }




                        //HACK: change table for "AppliRights"
                        if (alias == "AppliRights")
                        {
                            ChangeReportTableNamed("BerechtigungenEffektiv", "berechtigungen_" + db.userId.ToString(), connectionInfo, rpt1);
                            ChangeReportTableNamed("applikationsrechte_64", "applikationsrechte_" + db.userId.ToString(), connectionInfo, rpt1);
                        }

                        //HACK: submit parameters for Ausbildungsmassnahmen AHB
                        if (alias == "AusbildungsmassnahmenAHB")
                        {
                            ChangeReportTable("Ausbildungsmassnahmen_" + db.userId.ToString(), connectionInfo, rpt1);
                        }


                        if (alias == "BonusNauer.rpt")
                        {
                            SetParameter("DurchschnittlicherLeistungsgrad", double.Parse(Request.QueryString["AvgPerformance"]));
                            SetParameter("Korrekturfaktor", double.Parse(Request.QueryString["korrfakt"]));
                            SetParameter("LbDatVon", Request.QueryString["datVon"]);
                            SetParameter("LbDatBis", Request.QueryString["datBis"]);

                            SetTableName("##BonusNauer_" + db.userId.ToString());
                        }

                        if (alias == "LeistungsbewertungReisgies")
                        {
                            ChangeReportTable("Rep_Rating", connectionInfo, rpt1);
                        }

                        if (alias == "Knowledge.rpt")
                        {
                            db.connect();

                            // delete temporary table if exists
                            string tbl_del = " IF NOT OBJECT_ID(N'tempdb..##Knowledge_%userid%') IS NULL "
                                + "DROP TABLE ##Knowledge_%userid%";
                            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                            //create and fill temporary table
                            string tbl_create = "CREATE TABLE [dbo].[##Knowledge_%userid%]("
                            + "[KNOWLWDGE_ID] [bigint],"
                            + "[KNOWLEDGE_CREATED] [datetime] NULL,"
                            + "[KNOWLEDGE_TITLE] [varchar](256) NULL,"
                            + "[KNOWLEDGE_DESCRIPTION] [text] NULL,"
                            + "[ORDNUMBER] [int] NULL,"
                            + "[KNOWLEDGE_CREATOR_PERSON] [varchar](128) NULL,"
                            + "[THEME_ID] [bigint] NULL,"
                            + "[THEME_TITLE] [varchar](256) NULL,"
                            + "[THEME_DESCRIPTION] [text] NULL,"
                            + "[THEME_ORDNUMBER] [int] NULL,"
                            + "[THEME_CREATOR_PERSONTHEME] [varchar](128) NULL,"
                            + "[THEMECREATED] [datetime] NULL,"
                            + "[THEME_PARENT_ID] [bigint] NULL)";

                            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                            db.execute("INSERT INTO ##Knowledge_" + db.userId.ToString()
                                + " SELECT KNOWLEDGE.ID AS KNOWLWDGE_ID, KNOWLEDGE.CREATED AS KNOWLEDGE_CREATED, THEME.TITLE AS KNOWLEDGE_TITLE, THEME.DESCRIPTION AS KNOWLEDGE_DESCRIPTION, THEME.ORDNUMBER, "
                                + "(SELECT ISNULL(FIRSTNAME, '')+ ' ' + ISNULL(PNAME, ' ')  FROM PERSON WHERE(ID = THEME.CREATOR_PERSON_ID)) AS KNOWLEDGE_CREATOR_PERSON, THEME_1.ID AS THEME_ID, THEME_1.TITLE AS THEME_TITLE, THEME_1.DESCRIPTION AS THEME_DESCRIPTION, THEME_1.ORDNUMBER AS THEME_ORDNUMBER, "
                                + "(SELECT ISNULL(FIRSTNAME, '')+ ' ' + ISNULL(PNAME, ' ')  FROM PERSON WHERE(ID = THEME_1.CREATOR_PERSON_ID)) AS THEME_CREATOR_PERSONTHEME, THEME_1.CREATED AS THEMECREATED,THEME_1.PARENT_ID AS THEME_PARENT_ID "
                                + "FROM KNOWLEDGE INNER JOIN "
                                + "THEME ON KNOWLEDGE.BASE_THEME_ID_DE = THEME.ID INNER JOIN "
                                + "THEME AS THEME_1 ON THEME.ID = THEME_1.ROOT_ID INNER JOIN "
                                + "PERSON AS PERSON_1 ON THEME.CREATOR_PERSON_ID = PERSON_1.ID "
                                + "WHERE KNOWLEDGE.ID = " + Request.QueryString["param0"]);
                            db.execute("ALTER TABLE ##Knowledge_" + db.userId.ToString() + " ADD DescriptionKnowledgeImage image NULL");
                            db.execute("ALTER TABLE ##Knowledge_" + db.userId.ToString() + " ADD DescriptionThemeImage image NULL");

                            DataTable tblKnowledge = db.getDataTable("SELECT THEME_ID, KNOWLEDGE_DESCRIPTION, THEME_DESCRIPTION FROM  ##Knowledge_" + db.userId.ToString());
                            for (int i = 0; i < tblKnowledge.Rows.Count; i++)
                            {
                                System.Drawing.Image knowledeImage;
                                System.Drawing.Image themeImage;
                                if (!tblKnowledge.Rows[i]["KNOWLEDGE_DESCRIPTION"].ToString().Equals(""))
                                {
                                    knowledeImage = ConvertHtmlToImage(tblKnowledge.Rows[i]["KNOWLEDGE_DESCRIPTION"].ToString());
                                }
                                else
                                {
                                    knowledeImage = new System.Drawing.Bitmap(1, 1);
                                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(knowledeImage);
                                    g.Clear(System.Drawing.Color.White);

                                }
                                if (!tblKnowledge.Rows[i]["THEME_DESCRIPTION"].ToString().Equals(""))
                                {
                                    themeImage = ConvertHtmlToImage(tblKnowledge.Rows[i]["THEME_DESCRIPTION"].ToString());
                                }
                                else
                                {
                                    themeImage = new System.Drawing.Bitmap(1, 1);
                                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(themeImage);
                                    g.Clear(System.Drawing.Color.White);

                                }
                                db.execute("UPDATE  ##Knowledge_" + db.userId.ToString() + " SET DescriptionKnowledgeImage =" + "0x" + BitConverter.ToString(imageToByteArray(knowledeImage)).Replace("-", "") + ", DescriptionthemeImage =" + "0x" + BitConverter.ToString(imageToByteArray(themeImage)).Replace("-", "") + " WHERE THEME_ID ='" + tblKnowledge.Rows[i]["THEME_ID"] + "'");
                            }



                            SetTableName("##Knowledge_" + db.userId.ToString());
                        }

                        if (alias == "SokratesList")
                        {
                            db.connect();

                            // delete temporary table if exists
                            string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SokratesListTitles_%userid%]') "
                                              + "AND type in (N'U')) "
                                              + "DROP TABLE [dbo].[SokratesListTitles_%userid%]";
                            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                            tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SokratesListCells_%userid%]') "
                                              + "AND type in (N'U')) "
                                              + "DROP TABLE [dbo].[SokratesListCells_%userid%]";
                            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                            //create and fill temporary tables
                            string tbl_create = "CREATE TABLE [dbo].[SokratesListTitles_%userid%]("
                            + "[MatrixId] [bigint] NULL,"
                            + "[DimensionId] [bigint] NULL,"
                            + "[CellId] [bigint] NULL,"
                            + "[OrdnumberDimension] [int] NULL,"
                            + "[OrdnumberReport][int] NULL,"
                            + "[TitleRowSpan] [int] NULL,"
                            + "[OrdnumberCharacteristic] [int] NULL,"
                            + "[CellTitle] [varchar](256) NULL,"
                            + "[CellSubtitle] [varchar](60) NULL,"
                            + "[Color_Red] [int] NULL,"
                            + "[Color_Green] [int] NULL,"
                            + "[Color_Blue] [int] NULL,"
                            + "[KNOWLEDGE_ID] [bigint] NULL,"
                            + "[THEME_ID] [bigint] NULL,"
                            + "[THEME_TITLE] [varchar](256) NULL,"
                            + "[THEME_DESCRIPTION] [text] NULL,"
                            + ") ON [PRIMARY]";
                            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                            tbl_create = "CREATE TABLE [dbo].[SokratesListCells_%userid%]("
                            + "[MatrixId] [bigint] NULL,"
                            + "[DimensionId] [bigint] NULL,"
                            + "[OrdnumberDimension] [int] NULL,"
                            + "[TitleRowSpan] [int] NULL,"
                            + "[CellID] [bigint] NULL,"
                            + "[OrdnumberCharacteristic] [int] NULL,"
                            + "[CellTitle] [varchar](256) NULL,"
                            + "[CellSubtitle] [varchar](60) NULL,"
                            + "[Color_Red] [int] NULL,"
                            + "[Color_Green] [int] NULL,"
                            + "[Color_Blue] [int] NULL,"
                            + "[KNOWLEDGE_ID] [bigint] NULL,"
                            + "[THEME_ID] [bigint] NULL,"
                            + "[THEME_TITLE] [varchar](256) NULL,"
                            + "[THEME_DESCRIPTION] [text] NULL,"
                            + ") ON [PRIMARY]";
                            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                            db.execute("INSERT INTO SokratesListTitles_" + db.userId.ToString() + " SELECT MATRIX.ID AS MatrixId, DIMENSION.ID AS DimensionId, CHARACTERISTIC.Id AS CellId, DIMENSION.ORDNUMBER AS OrdnumberDimension,  CAST(CAST(DIMENSION.ORDNUMBER AS VARCHAR(10)) + CAST(CHARACTERISTIC.ORDNUMBER AS VARCHAR(10)) AS INT) AS OrdnumberReport, DIMENSION.TITLEROWSPAN, CHARACTERISTIC.ORDNUMBER AS OrdnumberCharacteristic, "
                                                                               + "CHARACTERISTIC.TITLE, CHARACTERISTIC.SUBTITLE, CAST(CAST(CAST(COLORATION.COLOR AS BINARY) & 16777215 AS int) / 256 / 256 AS int) AS Color_Red, CAST(CAST(CAST(COLORATION.COLOR AS BINARY) & 16777215 AS int) / 256 & 255 AS int) AS Color_Green, "
                                                                               + "CAST(CAST(CAST(COLORATION.COLOR AS BINARY) & 16777215 AS int) & 255 AS int) AS Color_Blue, ThemeTree.KNOWLWDGE_ID, ThemeTree.THEME_ID, ThemeTree.THEME_TITLE, ThemeTree.THEME_DESCRIPTION "
                                                                        + "FROM MATRIX INNER JOIN "
                                                                               + "DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN "
                                                                               + "CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID LEFT OUTER JOIN "
                                                                               + "ThemeTree ON CHARACTERISTIC.KNOWLEDGE_ID = ThemeTree.KNOWLWDGE_ID LEFT OUTER JOIN "
                                                                               + "COLORATION ON CHARACTERISTIC.COLOR_ID = COLORATION.ID "
                                                                        + "WHERE(MATRIX.ID = " + Request.QueryString["param0"] + ") AND(CHARACTERISTIC.ORDNUMBER = 0 OR "
                                                                               + "CHARACTERISTIC.ORDNUMBER = 1) AND(NOT(CHARACTERISTIC.TITLE IS NULL)) AND(CHARACTERISTIC.TITLE > ' ') "
                                                                        + "ORDER BY OrdnumberDimension, OrdnumberCharacteristic ");

                            db.execute("INSERT INTO SokratesListCells_" + db.userId.ToString() + " SELECT MATRIX.ID AS MatrixId, DIMENSION.ID AS DimensionId, DIMENSION.ORDNUMBER AS OrdnumberDimension, DIMENSION.TITLEROWSPAN, CHARACTERISTIC.ID AS CellId, CHARACTERISTIC.ORDNUMBER AS OrdnumberCharacteristic, "
                                                                               + "CHARACTERISTIC.TITLE, CHARACTERISTIC.SUBTITLE, CAST(CAST(CAST(COLORATION.COLOR AS BINARY) & 16777215 AS int) / 256 / 256 AS int) AS Color_Red, CAST(CAST(CAST(COLORATION.COLOR AS BINARY) & 16777215 AS int) / 256 & 255 AS int) AS Color_Green, "
                                                                               + "CAST(CAST(CAST(COLORATION.COLOR AS BINARY) & 16777215 AS int) & 255 AS int) AS Color_Blue, ThemeTree.KNOWLWDGE_ID, ThemeTree.THEME_ID, ThemeTree.THEME_TITLE, ThemeTree.THEME_DESCRIPTION "
                                                                        + "FROM MATRIX INNER JOIN "
                                                                               + "DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN "
                                                                               + "CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID LEFT OUTER JOIN "
                                                                               + "ThemeTree ON CHARACTERISTIC.KNOWLEDGE_ID = ThemeTree.KNOWLWDGE_ID LEFT OUTER JOIN "
                                                                               + "COLORATION ON CHARACTERISTIC.COLOR_ID = COLORATION.ID "
                                                                        + "WHERE(MATRIX.ID = " + Request.QueryString["param0"] + ")  "
                                                                               + " AND(NOT(CHARACTERISTIC.TITLE IS NULL)) AND(CHARACTERISTIC.TITLE > ' ') "
                                                                        + "ORDER BY OrdnumberDimension, OrdnumberCharacteristic ");


                            db.execute("ALTER TABLE SokratesListTitles_" + db.userId.ToString() + " ADD DescriptionThemeImage image NULL");
                            db.execute("ALTER TABLE SokratesListTitles_" + db.userId.ToString() + " ADD HasCell1 int NULL");

                            db.execute("ALTER TABLE SokratesListCells_" + db.userId.ToString() + " ADD DescriptionThemeImage image NULL");

                            DataTable tblTitleCells = db.getDataTable("SELECT * FROM SokratesListTitles_" + db.userId.ToString());

                            for (int i = 0; i < tblTitleCells.Rows.Count; i++)
                            {
                                System.Drawing.Image img;
                                if (!tblTitleCells.Rows[i]["THEME_DESCRIPTION"].ToString().Equals(""))
                                {
                                    img = ConvertHtmlToImage(tblTitleCells.Rows[i]["THEME_DESCRIPTION"].ToString());
                                }
                                else
                                {
                                    img = new System.Drawing.Bitmap(1, 1);
                                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img);
                                    g.Clear(System.Drawing.Color.White);

                                }

                                if (tblTitleCells.Rows[i]["OrdnumberCharacteristic"].ToString().Equals("0"))
                                {
                                    DataRow[] hasCell = tblTitleCells.Select("DimensionId = '" + tblTitleCells.Rows[i]["DimensionID"].ToString() + "' AND OrdnumberCharacteristic = 1");
                                    if (hasCell.Length > 0)
                                    {
                                        db.execute("UPDATE  SokratesListTitles_" + db.userId.ToString() + " SET HasCell1 = 1 WHERE CellId = " + tblTitleCells.Rows[i]["CellId"].ToString());
                                    }
                                }

                                db.execute("UPDATE  SokratesListTitles_" + db.userId.ToString() + " SET DescriptionThemeImage =" + "0x" + BitConverter.ToString(imageToByteArray(img)).Replace("-", "") + " WHERE THEME_ID ='" + tblTitleCells.Rows[i]["THEME_ID"] + "'");
                            }

                            DataTable tblCells = db.getDataTable("SELECT * FROM SokratesListCells_" + db.userId.ToString());

                            for (int i = 0; i < tblCells.Rows.Count; i++)
                            {
                                System.Drawing.Image img;
                                if (!tblCells.Rows[i]["THEME_DESCRIPTION"].ToString().Equals(""))
                                {
                                    img = ConvertHtmlToImage(tblCells.Rows[i]["THEME_DESCRIPTION"].ToString());
                                }
                                else
                                {
                                    img = new System.Drawing.Bitmap(1, 1);
                                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img);
                                    g.Clear(System.Drawing.Color.White);

                                }

                                db.execute("UPDATE  SokratesListCells_" + db.userId.ToString() + " SET DescriptionThemeImage =" + "0x" + BitConverter.ToString(imageToByteArray(img)).Replace("-", "") + " WHERE THEME_ID ='" + tblCells.Rows[i]["THEME_ID"] + "'");
                            }
                            db.disconnect();

                            ChangeReportTableNamed(rpt1.Database.Tables[1].Name.ToString(), "SokratesListTitles_" + db.userId.ToString(), connectionInfo, rpt1);
                            ChangeReportTableNamed(rpt1.Database.Tables[2].Name.ToString(), "SokratesListCells_" + db.userId.ToString(), connectionInfo, rpt1);
                            //ChangeReportTableNamed(rpt1.Subreports[1].Database.Tables[0].Name.ToString(), "SokratesListTitles_" + db.userId.ToString(), connectionInfo, rpt1.Subreports[1]);
                            //ChangeReportTableNamed(rpt1.Subreports[2].Database.Tables[0].Name.ToString(), "SokratesListCells_" + db.userId.ToString(), connectionInfo, rpt1.Subreports[2]);
                            SetParameter("MatrixId", Convert.ToInt32(Request.QueryString["param0"]));
                        }

                        if (alias == "FBWDetail.rpt")
                        {
                            int anzahlMerkmale = 6;
                            db.connect();

                            // delete temporary table if exists
                            string tbl_del = " IF NOT OBJECT_ID(N'tempdb..##FBWDetail_%userid%') IS NULL "
                                + "DROP TABLE ##FBWDetail_%userid%";
                            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                            //create and fill temporary table
                            string tbl_create = "CREATE TABLE [dbo].[##FBWDetail_%userid%]("
                            + "[FBWId] [bigint],"
                            + "[FunctionTitle] [varchar](256) NULL,"
                            + "[FunctionValue] [float] NULL)";
                            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                            DataTable fbwKriterium = db.getDataTable("SELECT ID, LABEL, ORDNUMBER FROM FBW_KRITERIUM");

                            var ColumnNames = new List<string>();
                            int rowNo = 1;
                            foreach (DataRow row in fbwKriterium.Rows)
                            {
                                db.execute("ALTER TABLE ##FBWDetail_" + db.userId.ToString() + " ADD [Datafield" + rowNo.ToString() + "] [float] NULL ");
                                ColumnNames.Add("Datafield" + rowNo.ToString());
                                rowNo++;
                            }
                            if (ColumnNames.Count < anzahlMerkmale)
                            {
                                for (int i = ColumnNames.Count; i < anzahlMerkmale; i++)
                                {
                                    db.execute("ALTER TABLE ##FBWDetail_" + db.userId.ToString() + " ADD [Datafield" + (i + 1).ToString() + "] [float] NULL ");
                                    ColumnNames.Add("Datafield" + i.ToString());
                                }
                            }
                            DataTable FBW = db.getDataTable("SELECT FUNKTIONSBEWERTUNG.ID AS FBWId, FUNKTION.TITLE_DE AS FunctionTitle, FUNKTIONSBEWERTUNG.FUNKTIONSWERT AS Value "
                                                                + "FROM   FUNKTION INNER JOIN FUNKTIONSBEWERTUNG ON FUNKTION.ID = FUNKTIONSBEWERTUNG.FUNKTION_ID "
                                                                + "WHERE  (FUNKTIONSBEWERTUNG.GUELTIG_AB <= GETDATE()) AND (FUNKTIONSBEWERTUNG.GUELTIG_BIS >= GETDATE()) "
                                                                + "ORDER BY FunctionTitle ");
                            DataTable FBWDetail = db.getDataTable("SELECT ANFORDERUNGDETAILV.FUNKTIONSBEWERTUNG_ID AS FBWId, SUM(ANFORDERUNGDETAILV.STUFE_PUNKTEZAHL) AS PunkteDetail, FBW_KRITERIUM.BEZEICHNUNG, FBW_KRITERIUM.ORDNUMBER "
                                                                + "FROM ANFORDERUNGDETAILV INNER JOIN FBW_KRITERIUM ON ANFORDERUNGDETAILV.KRITERIUM_ID = FBW_KRITERIUM.ID "
                                                                + "GROUP BY ANFORDERUNGDETAILV.FUNKTIONSBEWERTUNG_ID, FBW_KRITERIUM.BEZEICHNUNG, FBW_KRITERIUM.ORDNUMBER, FBW_KRITERIUM.PERFORMANCE_CRITERIA_REF ORDER BY FBW_KRITERIUM.ORDNUMBER ");

                            foreach (DataRow row in FBW.Rows)
                            {
                                string sqlDetailColumns = "";
                                string detailValues = "";
                                DataRow[] rowtmp = FBWDetail.Select("FBWId =" + row[0].ToString());
                                if (rowtmp.Length > 0)
                                {
                                    DataTable detailData = rowtmp.CopyToDataTable();

                                    if (detailData.Rows.Count < anzahlMerkmale)
                                    {
                                        for (int i = detailData.Rows.Count; i < anzahlMerkmale; i++)
                                        {
                                            detailData.Rows.Add();
                                            detailData.Rows[i]["FBWId"] = 1;
                                            detailData.Rows[i]["PunkteDetail"] = System.DBNull.Value;
                                            detailData.Rows[i]["BEZEICHNUNG"] = "Datafield" + i.ToString();
                                            detailData.Rows[i]["ORDNUMBER"] = i + 1;
                                        }

                                    }
                                    int i1 = 1;
                                    foreach (DataRow rowDetail in detailData.Rows)
                                    {
                                        sqlDetailColumns += ", [Datafield" + i1.ToString() + "]";
                                        if (rowDetail[1].ToString().Equals(""))
                                            detailValues += ", NULL";
                                        else
                                            detailValues += ", " + rowDetail[1].ToString().Replace(",", ".");

                                        SetParameter("FieldName" + i1.ToString(), rowDetail.ItemArray[2].ToString());
                                        i1++;


                                    }

                                    db.execute("INSERT INTO ##FBWDetail_" + db.userId.ToString() + "(FBWId, FunctionTitle, FunctionValue" + sqlDetailColumns + ") VALUES(" + row[0] + ", '" + row[1] + "', " + row[2].ToString().Replace(",", ".") + detailValues + ")");
                                }

                                SetTableName("##FBWDetail_" + db.userId.ToString());
                            }



                        }

                        if (alias == "Stellenbeschreibung")
                        {
                            ChangeReportTable("Stellenbeschreibung_" + db.userId.ToString(), connectionInfo, rpt1);
                        }
                        if (alias == "LeistungsbewertungenUnvollstaendig")
                        {
                            //check rights
                            sql = "";
                            long accessorID = SessionData.getUserAccessorID(Session);
                            string accessorSQL = db.getAccessorIDsSQLInClause(accessorID);
                            DataTable tblJobs = db.getDataTableExt("select distinct JOB.ID from JOB inner join ORGENTITY on JOB.ORGENTITY_ID=ORGENTITY.ID inner join ORGANISATION on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID and ORGANISATION.MAINORGANISATION=1 inner join ACCESS_RIGHT_RT on ACCESS_RIGHT_RT.TABLENAME='JOB' and (ACCESS_RIGHT_RT.ROW_ID=JOB.ID or ACCESS_RIGHT_RT.ROW_ID=0) and ACCESS_RIGHT_RT.APPLICATION_RIGHT=11 and (ACCESS_RIGHT_RT.AUTHORISATION&4)=4 and ACCESS_RIGHT_RT.ACCESSOR_ID  " + accessorSQL, new object[0]);
                            bool start = true;
                            foreach (DataRow aktJob in tblJobs.Rows)
                            {
                                if (start == true)
                                {
                                    start = false;
                                }
                                else
                                {
                                    sql += " OR ";
                                }

                                sql += "JobID=" + aktJob["ID"];
                            }
                            db.connect();
                            db.execute("DELETE FROM PersonWithIncompletePerformancerating_" + db.userId.ToString() + " WHERE NOT (" + sql + ")");
                            db.disconnect();

                            ChangeReportTable("PersonWithIncompletePerformancerating_" + db.userId.ToString(), connectionInfo, rpt1);
                        }
                        else
                        {
                            SetDBLogonForReport(connectionInfo, rpt1);

                        }

                        if (alias == "EigeneinschaetzungSPV" || alias == "EigeneinschaetzungSPVKader")
                        {
                            string personIds = "";
                            if (alias == "EigeneinschaetzungSPV")
                            {
                                sql = "{SelfReatingSPV.PersonID} IN  [param0]";
                            }
                            else
                            {
                                sql = "{SelfReatingSPVKader.PersonID} IN  [param0]";
                            }
                            //get persons IDs
                            if (Request.QueryString["param0"] == "oe")
                            {

                                DataTable persons;
                                if (alias == "EigeneinschaetzungSPV")
                                {
                                    persons = db.getDataTable("SELECT PersonID FROM SelfReatingSPV WHERE OrgentityID IN (" + Request.QueryString["param1"] + ")");
                                }
                                else
                                {
                                    persons = db.getDataTable("SELECT PersonID FROM SelfReatingSPVKader WHERE OrgentityID IN (" + Request.QueryString["param1"] + ")");
                                }

                                foreach (DataRow person in persons.Rows)
                                {
                                    personIds += person["PersonID"].ToString() + ",";
                                }
                            }
                            else
                            {
                                personIds = Request.QueryString["param1"];
                            }

                            personIds = personIds.TrimEnd(new char[] { ',' });


                            sql = sql.Replace("param0", personIds);
                            CrystalReportViewer1.SelectionFormula = sql;
                        }




                    }
                }
            }
        }

        private void PerisitImage(object p1, string p2)
        {
            throw new NotImplementedException();
        }

        private string replaceParameters(string sql)
        {
            for (int idx = 0; idx < 10; idx++)
            {
                if (Request.QueryString["param" + idx] != "" && Request.QueryString["param" + idx] != null)
                {
                    string param = Request.QueryString["param" + idx];

                    //decode base64 if necessary
                    if (param.Length > 2 && param.Substring(0, 3) != "pwd")
                    {
                        //try to decode
                        param = Global.DecodeFrom64(param);

                        //try
                        //{
                        //    byte[] encodedDataAsBytes = System.Convert.FromBase64String(param);
                        //    param = System.Text.ASCIIEncoding.Unicode.GetString(encodedDataAsBytes);
                        //}
                        //catch (FormatException e)
                        //{
                        //    //invalid!
                        //    param = "no_data";
                        //}
                    }
                    else if (param.Length > 2 && param.Substring(0, 3) == "pwd")
                    {
                        //remove pwd
                        param = param.Substring(3, param.Length - 3);
                    }
                    else
                    {
                        //invalid!
                        param = "no_data";
                    }

                    sql = sql.Replace("%param" + idx + "%", param);
                }
                else
                {
                    return sql;
                }
            }

            return sql;
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

            //for (int i = 0; i < rpt1.DataSourceConnections.Count; i++)
            //{
            //    NameValuePairs2 lp = rpt1.DataSourceConnections[i].LogonProperties;
            //    lp.Set("Connection Timeout", "1");
            //    rpt1.DataSourceConnections[i].SetLogonProperties(lp);
            //}

            if (saveReport)
            {
                SaveInvoce();
            }

        }

        private void SaveInvoce()
        {
            try
            {
                rpt1.SetParameterValue(0, Convert.ToInt32(Session["OrderId"]));
                rpt1.SetParameterValue(1, Convert.ToInt32(Session["SellerAddressId"]));
                rpt1.SetParameterValue(2, Convert.ToInt32(Session["SellerFirmId"]));
                rpt1.SetParameterValue(3, Session["PaymentType"].ToString());
                rpt1.ExportToDisk(ExportFormatType.PortableDocFormat, reportPath);

                BraintreeService bService = new BraintreeService();
                bService.sendInvoice(reportPath);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
        }

        private void ChangeReportTable(string newTable, ConnectionInfo connectionInfo, ReportDocument rpt1)
        {
            Tables tables = rpt1.Database.Tables;

            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(tableLogonInfo);
                table.Location = connectionInfo.DatabaseName + ".dbo." + newTable;
            }
        }

        private void ChangeReportTableNamed(string oldTable, string newTable, ConnectionInfo connectionInfo, ReportDocument rpt1)
        {
            Tables tables = rpt1.Database.Tables;

            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                if (oldTable == table.Name)
                {
                    TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                    tableLogonInfo.ConnectionInfo = connectionInfo;
                    table.ApplyLogOnInfo(tableLogonInfo);
                    table.Location = connectionInfo.DatabaseName + ".dbo." + newTable;
                }
            }
        }

        private void SetLogo(ReportDocument rpt)
        {
            foreach (ReportObject rObj in rpt.ReportDefinition.ReportObjects)
            {
                if (rObj.Name.Equals("Logo"))
                {
                    ParameterFields pFields = CrystalReportViewer1.ParameterFieldInfo;

                    string logoName = Global.Config.getModuleParam("report", "headerLogoImage", "PsoftDogBlack.gif"); ;
                    ParameterField parameterImagePath = new ParameterField();
                    parameterImagePath.Name = "LogoPath";
                    ParameterDiscreteValue parameterImagePath_value = new ParameterDiscreteValue();
                    string protocoll = "http://";
                    if (HttpContext.Current.Request.IsSecureConnection)
                    {
                        protocoll = "https://";
                    }
                    parameterImagePath_value.Value = protocoll + Global.Config.domain + Global.Config.baseURL + "/images/" + logoName;
                    parameterImagePath.CurrentValues.Add(parameterImagePath_value);
                    pFields.Add(parameterImagePath);
                }
            }
        }

        protected override void InitializeCulture()
        {
            // set culture according to selected language / 12.01.10 / mkr
            string cultureInfo = "de-CH";

            if (Session["culture"] != null && Session["culture"].ToString() != "")
            {
                cultureInfo = Session["culture"].ToString();
            }

            CultureInfo culture = new CultureInfo(cultureInfo);

            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

            base.InitializeCulture();
        }

        private void SetParameter(string fieldName, string parameter)
        {
            ParameterFields fields = CrystalReportViewer1.ParameterFieldInfo;
            ParameterField field = new ParameterField();
            field.Name = fieldName;
            ParameterDiscreteValue field_value = new ParameterDiscreteValue();
            field_value.Value = parameter;
            field.CurrentValues.Add(field_value);
            fields.Add(field);
        }

        private void SetParameter(string fieldName, int parameter)
        {
            ParameterFields fields = CrystalReportViewer1.ParameterFieldInfo;
            ParameterField field = new ParameterField();
            field.Name = fieldName;
            ParameterDiscreteValue field_value = new ParameterDiscreteValue();
            field_value.Value = parameter;
            field.CurrentValues.Add(field_value);
            fields.Add(field);
        }

        private void SetParameter(string fieldName, double parameter)
        {
            ParameterFields fields = CrystalReportViewer1.ParameterFieldInfo;
            ParameterField field = new ParameterField();
            field.Name = fieldName;
            ParameterDiscreteValue field_value = new ParameterDiscreteValue();
            field_value.Value = parameter;
            field.CurrentValues.Add(field_value);
            fields.Add(field);
        }

        private void SetParameter(string fieldName, DateTime parameter)
        {
            ParameterFields fields = CrystalReportViewer1.ParameterFieldInfo;
            ParameterField field = new ParameterField();
            field.Name = fieldName;
            ParameterDiscreteValue field_value = new ParameterDiscreteValue();
            field_value.Value = parameter;
            field.CurrentValues.Add(field_value);
            fields.Add(field);
        }

        private void SetTableName(string tableName)
        {
            Tables tables = rpt1.Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                tableLogonInfo.ConnectionInfo.DatabaseName = "tempdb";
                table.ApplyLogOnInfo(tableLogonInfo);
                table.Location = tableName;
            }
        }

        private bool ExistTmpTable(string tableName)
        {
            bool tableExist = false;
            DBData dbt = DBData.getDBData(Session);
            dbt.connect();
            DataTable testTable = dbt.getDataTable("SELECT OBJECT_ID(N'tempdb..##" + tableName + "_" + dbt.userId.ToString() + "')");
            dbt.disconnect();
            if (!testTable.Rows[0][0].ToString().Equals(""))
                tableExist = true;
            return tableExist;
        }

        public System.Drawing.Image ConvertHtmlToImage(String html)

        {
            //add style
            String style = "<html><head> <link id=\"cssLink\" href=\"" + Server.MapPath("../Style/Psoft.css") + "\" type=\"text/css\" rel=\"stylesheet\">"
                            + "<link id=\"cssFontHeight\" href=\"" + Server.MapPath("../Style/PsoftFontsize4.css") + "\" type=\"text/css\" rel=\"stylesheet\">"
                            + "<link id=\"cssLayoutLink\" href=\"" + Server.MapPath("../Style/layout.css") + "\" media=\"screen\" rel=\"stylesheet\" type=\"text/css\">"
                            + "</head><body>";
            html = style + html + " </body></html>";
            System.Drawing.Image image;
            //convert
            try
            {
                if (Global.Config.baseURL.Length > 0)
                {
                    html = Regex.Replace(html, Global.Config.baseURL, Page.Request.Url.Scheme + "://" + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port) + Global.Config.baseURL, RegexOptions.IgnoreCase);
                }
                else
                {
                    html = Regex.Replace(html, Global.Config.domain, Page.Request.Url.Scheme + "://" + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port), RegexOptions.IgnoreCase);
                }

                image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage(html, 669);
            }
            catch
            {
                image = new System.Drawing.Bitmap(1, 1);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
                g.Clear(System.Drawing.Color.White);
            }
            //save image
            image.Save(Server.MapPath("test.png"));
            return image;
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }

        public static void PerisitImage(byte[] imgBytes, String tableName, IDbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {


                command.CommandText = "INSERT INTO " + tableName + "(DescriptionThemeImage) VALUES (:payload)";
                IDataParameter par = command.CreateParameter();
                par.ParameterName = "payload";
                par.DbType = DbType.Binary;
                par.Value = imgBytes;
                command.Parameters.Add(par);
                command.ExecuteNonQuery();
            }
        }

    }
}
