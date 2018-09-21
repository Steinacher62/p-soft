using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;
using System.Reflection;

namespace ch.appl.psoft.Performance.Controls
{
    public partial class PrintAveragePerformanceDetail : System.Web.UI.UserControl
    {
        protected int _performanceRatingID = -1;
        protected int _employmentID = -1;
        protected long _personID = -1;
        protected string _onloadString;
        protected int mbo = 0;
        protected long mboId = 0;
        protected long mboExists = 0;

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
            if (reportClassName == "ch.appl.psoft.Performance.AveragePerformanceReportRectangle")
            {
                //Response.Redirect(Global.Config.baseURL + "/Performance/AveragePerformanceReportRectangle.aspx?" + Request.QueryString,true);

                // export rectangle to PDF / 08.06.10 / mkr
                string reportfile = Server.MapPath(Global.Config.baseURL + "/crystalreports/PerformanceratingRectangle.rpt");
                _personID = ch.psoft.Util.Validate.GetValid((db.lookup("PERSON_ID", "EMPLOYMENT", "ID=" + _employmentID)).ToString(), -1L);

                ReportDocument rpt1 = ReportFactory.GetReport();

                rpt1.Load(reportfile);

                //set db logon for report
                ConnectionInfo connectionInfo = new ConnectionInfo();
                connectionInfo.ServerName = Global.Config.dbServer;
                connectionInfo.DatabaseName = Global.Config.dbName;
                connectionInfo.UserID = Global.Config.dbUser;
                connectionInfo.Password = Global.Config.dbPassword;

                SetDBLogonForReport(connectionInfo, rpt1);

                SetHeader(rpt1, db, map);
                SetPerformanceDiagramm(rpt1, db);
                SetExpectation(rpt1, db, map, connectionInfo);
                //if (mbo == 1 && mboId > 0)
                //{
                SetMbO(rpt1, db, map, connectionInfo);
                //}



                // export to PDF
                ExportOptions exportOpts = new ExportOptions();
                PdfRtfWordFormatOptions PDFOpts = new PdfRtfWordFormatOptions();
                DiskFileDestinationOptions diskOpts = new DiskFileDestinationOptions();
                exportOpts = rpt1.ExportOptions;
                exportOpts.ExportFormatOptions = PDFOpts;

                exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat;
                exportOpts.ExportDestinationType = ExportDestinationType.DiskFile;
                string rectangle_filename = fileName + "_rectangle_" + SessionData.getSessionID(Session).ToString() + ".pdf";
                diskOpts.DiskFileName = outputDirectory + "\\" + rectangle_filename;
                exportOpts.DestinationOptions = diskOpts;

                rpt1.Export();

                Response.Redirect(Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + rectangle_filename, false);
            }
            else
            {
                try
                {
                    db.connect();
                    Type reportClass = Type.GetType(reportClassName, true, false);
                    ConstructorInfo[] constr = reportClass.GetConstructors();
                    PReport report = (PReport)constr[0].Invoke(new object[] { Session, imageDirectory });

                    string pyramidName = outputDirectory + "/" + fileName + "_pyramid_" + SessionData.getSessionID(Session).ToString() + ".jpg";
                    report.createReport(new object[] { _employmentID, _performanceRatingID, pyramidName });

                    if (report != null)
                        report.saveReport(outputDirectory, fileName);

                    Response.Redirect(Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + report.PDFFilename, false);
                }

                catch (Exception ex)
                {
                    Logger.Log(ex, Logger.ERROR);
                }
                finally
                {
                    db.disconnect();
                }
            }
        }



        // code below copied from AveragePerformanceReportRectangle.aspx.cs, update if finished / don't make any changes here!
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

        private void SetHeader(ReportDocument rpt1, DBData db, LanguageMapper map)
        {
            TextObject Firstname = (TextObject)rpt1.ReportDefinition.ReportObjects["Firstname"];
            TextObject Pname = (TextObject)rpt1.ReportDefinition.ReportObjects["Pname"];
            TextObject Personnelnumber = (TextObject)rpt1.ReportDefinition.ReportObjects["Personnelnumber"];
            TextObject Dateofbirth = (TextObject)rpt1.ReportDefinition.ReportObjects["Dateofbirth"];
            TextObject Job = (TextObject)rpt1.ReportDefinition.ReportObjects["Job"];
            TextObject Ratingdat = (TextObject)rpt1.ReportDefinition.ReportObjects["Ratingdat"];
            
            Firstname.Text = db.lookup("FIRSTNAME", "PERSON", "ID=" + _personID).ToString();
            Pname.Text = db.lookup("PNAME", "PERSON", "ID=" + _personID).ToString();
            Personnelnumber.Text = db.lookup("PERSONNELNUMBER", "PERSON", "ID=" + _personID).ToString();
            Dateofbirth.Text = db.lookup("DATEOFBIRTH", "PERSON", "ID=" + _personID).ToString();
            if (Dateofbirth.Text != "")
            {
                Dateofbirth.Text = Dateofbirth.Text.Remove(11);
            }
            Job.Text = db.lookup("JOB_TITLE_" + map.LanguageCode, "PERFORMANCERATING", "ID=" + _performanceRatingID).ToString();
            Ratingdat.Text = db.lookup("RATING_DATE", "PERFORMANCERATING", "ID=" + _performanceRatingID).ToString().Remove(11);
            // TextObject name = (TextObject)rpt1.ReportDefinition.ReportObjects["Text8"];
            // name.Text = map.get("Performance", "pname");

        }

        private void SetPerformanceDiagramm(ReportDocument rpt1, DBData db)
        {
            LineObject topline = (LineObject)rpt1.ReportDefinition.ReportObjects["Line2"];
            LineObject baseline = (LineObject)rpt1.ReportDefinition.ReportObjects["Line10"];
            LineObject heighLine = (LineObject)rpt1.ReportDefinition.ReportObjects["Line3"];
            BoxObject left = (BoxObject)rpt1.ReportDefinition.ReportObjects["Box2"];
            LineObject totperformanceLine = (LineObject)rpt1.ReportDefinition.ReportObjects["Line11"];
            int space = 50;
            int startfirstx = (left.Right + space);
            int startx = startfirstx;
            int maxlength = 9729;
            int maxhight = (heighLine.Bottom - heighLine.Top);
            double totperformance = 0;


            //PictureObject pic1 = (PictureObject)rpt1.ReportDefinition.ReportObjects["picture1"];
            BoxObject box1 = (BoxObject)rpt1.ReportDefinition.ReportObjects["Box4"];
            TextObject lb1 = (TextObject)rpt1.ReportDefinition.ReportObjects["LB1"];
            BoxObject box2 = (BoxObject)rpt1.ReportDefinition.ReportObjects["Box5"];
            TextObject lb2 = (TextObject)rpt1.ReportDefinition.ReportObjects["LB2"];
            BoxObject box3 = (BoxObject)rpt1.ReportDefinition.ReportObjects["Box6"];
            TextObject lb3 = (TextObject)rpt1.ReportDefinition.ReportObjects["LB3"];
            BoxObject box4 = (BoxObject)rpt1.ReportDefinition.ReportObjects["Box7"];
            TextObject lb4 = (TextObject)rpt1.ReportDefinition.ReportObjects["LB4"];
            BoxObject box5 = (BoxObject)rpt1.ReportDefinition.ReportObjects["Box8"];
            TextObject lb5 = (TextObject)rpt1.ReportDefinition.ReportObjects["LB5"];
            BoxObject box6 = (BoxObject)rpt1.ReportDefinition.ReportObjects["Box9"];
            TextObject lb6 = (TextObject)rpt1.ReportDefinition.ReportObjects["LB6"];
            BoxObject box7 = (BoxObject)rpt1.ReportDefinition.ReportObjects["Box10"];
            TextObject lb7 = (TextObject)rpt1.ReportDefinition.ReportObjects["LB7"];
            BoxObject box8 = (BoxObject)rpt1.ReportDefinition.ReportObjects["Box11"];
            TextObject lb8 = (TextObject)rpt1.ReportDefinition.ReportObjects["LB8"];
            TextObject performanceratingtot = (TextObject)rpt1.ReportDefinition.ReportObjects["PerformanceRatingTot"];
            TextObject pcriteria1 = (TextObject)rpt1.ReportDefinition.ReportObjects["PCriteria1"];
            TextObject pcriteria2 = (TextObject)rpt1.ReportDefinition.ReportObjects["PCriteria2"];
            TextObject pcriteria3 = (TextObject)rpt1.ReportDefinition.ReportObjects["PCriteria3"];
            TextObject pcriteria4 = (TextObject)rpt1.ReportDefinition.ReportObjects["PCriteria4"];
            TextObject pcriteria5 = (TextObject)rpt1.ReportDefinition.ReportObjects["PCriteria5"];
            TextObject pcriteria6 = (TextObject)rpt1.ReportDefinition.ReportObjects["PCriteria6"];
            TextObject pcriteria7 = (TextObject)rpt1.ReportDefinition.ReportObjects["PCriteria7"];
            TextObject pcriteria8 = (TextObject)rpt1.ReportDefinition.ReportObjects["PCriteria8"];

            LineObject Line100Precent = (LineObject)rpt1.ReportDefinition.ReportObjects["Line6"];
            TextObject Text100Precent = (TextObject)rpt1.ReportDefinition.ReportObjects["Text100"];
            TextObject RatingText = (TextObject)rpt1.ReportDefinition.ReportObjects["RatingText"];
            TextObject TextLow = (TextObject)rpt1.ReportDefinition.ReportObjects["Text20"];
            TextObject TextBase = (TextObject)rpt1.ReportDefinition.ReportObjects["Text100"];
            TextObject TextHigh = (TextObject)rpt1.ReportDefinition.ReportObjects["Text21"];

            DataTable tableCriteria = db.getDataTableExt("select distinct CRITERIA_REF, " + db.langAttrName("PERFORMANCERATING_ITEMS", "CRITERIA_TITLE") + " from PERFORMANCERATING_ITEMS where PERFORMANCERATING_REF=" + _performanceRatingID, "PERFORMANCERATING_ITEMS");

            int zaehler = 1;
            double mbo_weight = 0;
            double mboAvgRating = 0;
            long jobId = 0;
            int counterCriteria = 1;
            bool criteria1 = false;
            bool criteria2 = false;
            bool criteria3 = false;
            bool criteria4 = false;
            bool criteria5 = false;
            bool criteria6 = false;
            bool criteria7 = false;
            bool criteria8 = false;

            double multiplikator = double.Parse(Global.Config.getModuleParam("performance", "performanceRatingBase", "100")) / 100;
            double rectangleRatingLow = double.Parse(Global.Config.getModuleParam("performance", "RectangleRatingLow", "0"));
            double rectangleRatingHigh = double.Parse(Global.Config.getModuleParam("performance", "RectangleRatingHigh", "100"));

            jobId = (long)db.lookup("job_id", "performancerating", "id = " + _performanceRatingID, 0L);
            mbo = (int)db.lookup("PERFORMANCERATING_MBO", "JOB", "ID = " + jobId, 0);

            //addd mbo if needed
            if (mbo == 1)
            {
                mbo_weight = (double)db.lookup("PERFORMANCERATING_MBO_WEIGHT", "JOB", "ID = " + jobId, 0.0D);
                int mboYear = Convert.ToInt16(db.lookup("OBJECTIVE_TURN.TITLE_DE", "PERFORMANCERATING INNER JOIN OBJECTIVE_TURN ON PERFORMANCERATING.OBJECTIVE_TURN_REF = dbo.OBJECTIVE_TURN.ID", "PERFORMANCERATING.ID = " + _performanceRatingID));
                long turnId = Convert.ToInt32(db.lookup("OBJECTIVE_TURN.ID", "PERFORMANCERATING INNER JOIN OBJECTIVE_TURN ON PERFORMANCERATING.OBJECTIVE_TURN_REF = dbo.OBJECTIVE_TURN.ID", "PERFORMANCERATING.ID = " + _performanceRatingID));
                mboAvgRating = Convert.ToInt32(db.lookup("TOP 1 Bewertung", "Objectiv_Rating", "Person_ID = " + _personID + " AND Turn = '" + mboYear + "' AND NOT Bewertung IS NULL", 0.0D));     //YEAR(Bewertungsdatum) IN (" + year + ", " + (year - 1) + ") ORDER BY Bewertungsdatum DESC"));
                DateTime ratingDate = Convert.ToDateTime(db.lookup("TOP 1 Bewertungsdatum", "Objectiv_Rating", "Turn = '" + mboYear + "'", DateTime.MinValue));                       //Convert.ToDateTime(db.lookup("TOP 1 Bewertungsdatum", "Objectiv_Rating", "Person_ID = " + _personID + " AND YEAR(Bewertungsdatum) IN (" + year + ", " + (year - 1) + ") ORDER BY Bewertungsdatum DESC"));
                if (mboYear > 1)
                {
                    mboId = Convert.ToUInt32(db.lookup("TOP 1 ID", "OBJECTIVE", "PERSON_ID = " + _personID + " AND OBJECTIVE_TURN_ID = " + turnId));
                }
                else
                {
                    mboId = 0;
                }
                DataRow mboRow;
                if (mboId > 0)
                {
                    //maxlength -= space;
                    mboRow = tableCriteria.NewRow();
                    mboRow["CRITERIA_REF"] = 0;
                    mboRow["CRITERIA_TITLE_DE"] = "Zielerreichungsgrad " + db.lookup("TITLE_DE", "OBJECTIVE_TURN", "ID =" + db.lookup("OBJECTIVE_TURN_ID", "OBJECTIVE", "ID =" + mboId.ToString()));
                    tableCriteria.Rows.Add(mboRow);
                }
                else
                {
                    mbo = 0;
                }
            }



            maxlength = maxlength - ((tableCriteria.Rows.Count - 1) * space);
            foreach (DataRow rowCriteria in tableCriteria.Rows)
            {
                long criteriaID = ch.psoft.Util.Validate.GetValid(rowCriteria["CRITERIA_REF"].ToString(), -1L);
                DataTable tableItems = new DataTable();

                if ((long)rowCriteria.ItemArray[0] > 0)
                {
                    tableItems = db.getDataTableExt("select ID from PERFORMANCERATING_ITEMS where PERFORMANCERATING_REF=" + _performanceRatingID + " and CRITERIA_REF=" + criteriaID, "PERFORMANCERATING_ITEMS");
                }
                else
                {
                    tableItems.Columns.Add("ID");
                    DataRow isMbO = tableItems.NewRow();
                    isMbO.ItemArray.SetValue(0, 0);
                    tableItems.Rows.Add(isMbO);
                }

                if (tableItems.Rows.Count > 0)
                {
                    foreach (DataRow itemsRow in tableItems.Rows)
                    {
                        String itemsID = "";
                        object[] values = null;
                        DataTable tableArgs;
                        if (!itemsRow[0].ToString().Equals(""))
                        {
                            itemsID = ch.psoft.Util.Validate.GetValid(itemsRow[db.langAttrName(itemsRow.Table.TableName, "ID")].ToString(), "");
                            DataTable detailTable = db.getDataTableExt("select " + db.langAttrName("PERFORMANCERATING_ITEMS", "EXPECTATION_DESCRIPTION") + " from PERFORMANCERATING_ITEMS where ID=" + itemsID, "PERFORMANCERATING_ITEMS");
                            values = db.lookup(
                            new string[] 
                                {
                                    "CRITERIA_WEIGHT",
                                    "RELATIV_WEIGHT",
                                    "LEVEL_TITLE_DE",
                                    db.langAttrName("PERFORMANCERATING_ITEMS", "EXPECTATION_DESCRIPTION")
                                },
                                    "PERFORMANCERATING_ITEMS",
                                    "ID = " + itemsID
                                    );

                            if (mbo == 1)
                            {

                                values[0] = System.Convert.ToDouble(values[0]) * ((100 - (float)mbo_weight) / 100);

                            }

                            tableArgs = db.getDataTableExt("select * from PERFORMANCERATING_ARGUMENTS where PERFORMANCERATING_REF=" + _performanceRatingID + " and PERFORMANCERATING_CRITERIA_REF=" + criteriaID + " and PERFORMANCERATING_ITEM_ID=" + itemsID, "PERFORMANCERATING_ARGUMENTS");
                        }
                        else
                        {

                            values = new string[]{               
                                    "CRITERIA_WEIGHT",
                                    "RELATIV_WEIGHT",
                                    "LEVEL_TITLE_DE",
                                    "EXPECTATION_DESCRIPTION"
                                 };
                            values[0] = mbo_weight.ToString();
                            values[2] = "Zielerreichungsgrad";
                            values[3] = "";
                            if (mboId > 0)
                            {
                                values[1] = mboAvgRating.ToString();
                            }
                            else
                            {
                                //default wenn keine Zielvereinbarung besteht
                                values[1] = "80";
                            }
                        }


                        bool showPerformanceLevelText = Convert.ToBoolean(Global.Config.getModuleParam("performance", "showPerformanceLevelsOnReport", "1") == "1");

                        if (counterCriteria == 1 && criteria1 == false)
                        {
                            double totPerformanceItem = 0;
                            for (int i = 0; i < tableItems.Rows.Count; i++)
                            {
                                totPerformanceItem += ((double)db.lookup("RELATIV_WEIGHT", "PERFORMANCERATING_ITEMS", "ID =" + tableItems.Rows[i][0].ToString()) / tableItems.Rows.Count);
                            }

                            box1.Bottom = (baseline.Bottom - 50);
                            box1.Left = (startfirstx);
                            box1.Right = (int)(box1.Left + maxlength / 100 * Convert.ToDouble(values[0]));
                            if (!itemsRow[0].ToString().Equals(""))
                            {
                                box1.Top = (box1.Bottom - (Int32)totPerformanceItem * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * totPerformanceItem;
                            }
                            else
                            {
                                box1.Top = (box1.Bottom - ((Int32)totPerformanceItem / 2) * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * (totPerformanceItem / 2);
                            }
                            lb1.Text = Convert.ToString(rowCriteria[1]);
                            lb1.Left = (box1.Left + ((box1.Right - box1.Left) / 2) - 150 - moveTextbox(lb1.Text));
                            startx = (box1.Right + space);
                            pcriteria1.Left = lb1.Left;
                            if (showPerformanceLevelText)
                            {
                                pcriteria1.Text = Convert.ToString(values[2]);
                            }
                            criteria1 = true;

                        }

                        else if (counterCriteria == 2 && criteria2 == false)
                        {
                            double totPerformanceItem = 0;
                            for (int i = 0; i < tableItems.Rows.Count; i++)
                            {
                                totPerformanceItem += ((double)db.lookup("RELATIV_WEIGHT", "PERFORMANCERATING_ITEMS", "ID =" + tableItems.Rows[i][0].ToString()) / tableItems.Rows.Count);
                            }

                            box2.Bottom = (baseline.Bottom - 50);
                            box2.Left = startx;
                            box2.Right = (int)(box2.Left + maxlength / 100 * Convert.ToDouble(values[0]));
                            if (!itemsRow[0].ToString().Equals(""))
                            {
                                box2.Top = (box1.Bottom - (Int32)totPerformanceItem * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * totPerformanceItem;
                            }
                            else
                            {
                                box2.Top = (box1.Bottom - ((Int32)totPerformanceItem / 2) * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * (totPerformanceItem / 2);
                            }
                            lb2.Text = Convert.ToString(rowCriteria[1]);
                            lb2.Left = (box2.Left + ((box2.Right - box2.Left) / 2) - 150 - moveTextbox(lb2.Text));
                            startx = (box2.Right + space);
                            pcriteria2.Left = lb2.Left;
                            if (showPerformanceLevelText)
                            {
                                pcriteria2.Text = Convert.ToString(values[2]);
                            }
                            criteria2 = true;
                        }
                        else if (counterCriteria == 3 && criteria3 == false)
                        {
                            double totPerformanceItem = 0;
                            for (int i = 0; i < tableItems.Rows.Count; i++)
                            {
                                totPerformanceItem += ((double)db.lookup("RELATIV_WEIGHT", "PERFORMANCERATING_ITEMS", "ID =" + tableItems.Rows[i][0].ToString()) / tableItems.Rows.Count);
                            }

                            box3.Bottom = (baseline.Bottom - 50);
                            box3.Left = startx;
                            box3.Right = (int)(box3.Left + maxlength / 100 * Convert.ToDouble(values[0]));
                            if (!itemsRow[0].ToString().Equals(""))
                            {
                                box3.Top = (box1.Bottom - (Int32)totPerformanceItem * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * totPerformanceItem;
                            }
                            else
                            {
                                box3.Top = (box1.Bottom - ((Int32)totPerformanceItem / 2) * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * (totPerformanceItem / 2);
                            }
                            lb3.Text = Convert.ToString(rowCriteria[1]);
                            lb3.Left = (box3.Left + ((box3.Right - box3.Left) / 2) - 150 - moveTextbox(lb3.Text));
                            startx = (box3.Right + space);
                            pcriteria3.Left = lb3.Left;
                            if (showPerformanceLevelText)
                            {
                                pcriteria3.Text = Convert.ToString(values[2]);
                            }

                            criteria3 = true;

                        }
                        else if (counterCriteria == 4 && criteria4 == false)
                        {
                            double totPerformanceItem = 0;
                            for (int i = 0; i < tableItems.Rows.Count; i++)
                            {
                                totPerformanceItem += ((double)db.lookup("RELATIV_WEIGHT", "PERFORMANCERATING_ITEMS", "ID =" + tableItems.Rows[i][0].ToString()) / tableItems.Rows.Count);
                            }

                            box4.Bottom = (baseline.Bottom - 50);
                            box4.Left = startx;
                            box4.Right = (int)(box4.Left + maxlength / 100 * Convert.ToDouble(values[0]));
                            if (!itemsRow[0].ToString().Equals(""))
                            {
                                box4.Top = (box1.Bottom - (Int32)totPerformanceItem * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * totPerformanceItem;
                            }
                            else
                            {
                                box4.Top = (box1.Bottom - ((Int32)totPerformanceItem / 2) * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * (totPerformanceItem / 2);
                            }
                            lb4.Text = Convert.ToString(rowCriteria[1]);
                            lb4.Left = (box4.Left + ((box4.Right - box4.Left) / 2) - 150 - moveTextbox(lb4.Text));
                            startx = (box4.Right + space);
                            pcriteria4.Left = lb4.Left;
                            if (showPerformanceLevelText)
                            {
                                pcriteria4.Text = Convert.ToString(values[2]);
                            }

                            criteria4 = true;

                        }
                        else if (counterCriteria == 5 && criteria5 == false)
                        {
                            double totPerformanceItem = 0;
                            for (int i = 0; i < tableItems.Rows.Count; i++)
                            {
                                totPerformanceItem += ((double)db.lookup("RELATIV_WEIGHT", "PERFORMANCERATING_ITEMS", "ID =" + tableItems.Rows[i][0].ToString()) / tableItems.Rows.Count);
                            }

                            box5.Bottom = (baseline.Bottom - 50);
                            box5.Left = startx;
                            box5.Right = (int)(box5.Left + maxlength / 100 * Convert.ToDouble(values[0]));
                            if (!itemsRow[0].ToString().Equals(""))
                            {
                                box5.Top = (box1.Bottom - (Int32)totPerformanceItem * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * totPerformanceItem;
                            }
                            else
                            {
                                box5.Top = (box1.Bottom - ((Int32)totPerformanceItem / 2) * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * (totPerformanceItem / 2);
                            }
                            lb5.Text = Convert.ToString(rowCriteria[1]);
                            lb5.Left = (box5.Left + ((box5.Right - box5.Left) / 2) - 150 - moveTextbox(lb5.Text));
                            startx = (box5.Right + space);
                            pcriteria5.Left = lb5.Left;
                            if (showPerformanceLevelText)
                            {
                                pcriteria5.Text = Convert.ToString(values[2]);
                            }

                            criteria5 = true;
                        }
                        else if (counterCriteria == 6 && criteria6 == false)
                        {
                            double totPerformanceItem = 0;
                            if (!itemsRow[0].ToString().Equals(""))
                            {
                                for (int i = 0; i < tableItems.Rows.Count; i++)
                                {
                                    totPerformanceItem += ((double)db.lookup("RELATIV_WEIGHT", "PERFORMANCERATING_ITEMS", "ID =" + tableItems.Rows[i][0].ToString()) / tableItems.Rows.Count);
                                }
                            }
                            else
                            {
                                totPerformanceItem = mboAvgRating;
                            }

                            box6.Bottom = (baseline.Bottom - 50);
                            box6.Left = startx;
                            box6.Right = (int)(box6.Left + maxlength / 100 * Convert.ToDouble(values[0]));

                            if (!itemsRow[0].ToString().Equals(""))
                            {
                                box6.Top = (box1.Bottom - (Int32)totPerformanceItem * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * totPerformanceItem;
                            }
                            else
                            {
                                box6.Top = (box1.Bottom - ((Int32)totPerformanceItem / 2) * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * (totPerformanceItem / 2);
                            }
                            lb6.Text = Convert.ToString(rowCriteria[1]);
                            lb6.Left = (box6.Left + ((box6.Right - box6.Left) / 2) - 150 - moveTextbox(lb6.Text));
                            startx = (box6.Right + space);

                            pcriteria6.Left = lb6.Left;
                            if (showPerformanceLevelText)
                            {
                                pcriteria6.Text = Convert.ToString(values[2]);
                            }

                            criteria6 = true;
                        }

                        else if (counterCriteria == 7 && criteria7 == false)
                        {
                            double totPerformanceItem = 0;
                            if (!itemsRow[0].ToString().Equals(""))
                            {
                                for (int i = 0; i < tableItems.Rows.Count; i++)
                                {
                                    totPerformanceItem += ((double)db.lookup("RELATIV_WEIGHT", "PERFORMANCERATING_ITEMS", "ID =" + tableItems.Rows[i][0].ToString()) / tableItems.Rows.Count);
                                }
                            }
                            else
                            {
                                totPerformanceItem = mboAvgRating;
                            }

                            box7.Bottom = (baseline.Bottom - 50);
                            box7.Left = startx;
                            box7.Right = (int)(box7.Left + maxlength / 100 * Convert.ToDouble(values[0]));

                            if (!itemsRow[0].ToString().Equals(""))
                            {
                                box7.Top = (box1.Bottom - (Int32)totPerformanceItem * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * totPerformanceItem;
                            }
                            else
                            {
                                box7.Top = (box1.Bottom - ((Int32)totPerformanceItem / 2) * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * (totPerformanceItem / 2);
                            }
                            lb7.Text = Convert.ToString(rowCriteria[1]);
                            lb7.Left = (box7.Left + ((box7.Right - box7.Left) / 2) - 150 - moveTextbox(lb7.Text));
                            startx = (box7.Right + space);

                            pcriteria7.Left = lb7.Left;
                            if (showPerformanceLevelText)
                            {
                                pcriteria7.Text = Convert.ToString(values[2]);
                            }

                            criteria7 = true;
                        }

                        else if (counterCriteria == 8 && criteria8 == false)
                        {
                            double totPerformanceItem = 0;
                            for (int i = 0; i < tableItems.Rows.Count; i++)
                            {
                                totPerformanceItem += ((double)db.lookup("RELATIV_WEIGHT", "PERFORMANCERATING_ITEMS", "ID =" + tableItems.Rows[i][0].ToString()) / tableItems.Rows.Count);
                            }

                            box8.Bottom = (baseline.Bottom - 50);
                            box8.Left = startx;
                            box8.Right = (int)(box8.Left + maxlength / 100 * Convert.ToDouble(values[0]));
                            if (!itemsRow[0].ToString().Equals(""))
                            {
                                box8.Top = (box1.Bottom - (Int32)totPerformanceItem * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * totPerformanceItem;
                            }
                            else
                            {
                                box8.Top = (box1.Bottom - ((Int32)totPerformanceItem / 2) * (maxhight / 100));
                                totperformance = totperformance + Convert.ToDouble(values[0]) * (totPerformanceItem / 2);
                            }
                            lb8.Text = Convert.ToString(rowCriteria[1]);
                            lb8.Left = (box8.Left + ((box8.Right - box8.Left) / 2) - 150 - moveTextbox(lb8.Text));
                            startx = (box8.Right + space);
                            pcriteria8.Left = lb8.Left;
                            if (showPerformanceLevelText)
                            {
                                pcriteria8.Text = Convert.ToString(values[2]);
                            }

                            criteria8 = true;
                        }
                        zaehler++;

                        if (zaehler == tableCriteria.Rows.Count)
                        {
                            for (int i = tableCriteria.Rows.Count + 1; i <= 8; i++)
                            {
                                if (i == 5)
                                {
                                    box6.Left = box6.Right;
                                    box6.LineStyle = LineStyle.NoLine;
                                }
                                if (i == 6)
                                {
                                    box6.Left = box6.Right;
                                    box6.LineStyle = LineStyle.NoLine;
                                }
                                if (i == 7)
                                {
                                    box7.Left = box7.Right;
                                    box7.LineStyle = LineStyle.NoLine;
                                }
                                if (i == 8)
                                {
                                    box8.Left = box8.Right;
                                    box8.LineStyle = LineStyle.NoLine;
                                }
                            }

                        }
                    }
                }
                counterCriteria++;

            }

            double ratingTot = Convert.ToDouble(box1.Bottom);
            int reight = totperformanceLine.Right;

            TextLow.Text = rectangleRatingLow.ToString() + "%";
            TextHigh.Text = rectangleRatingHigh.ToString() + "%";
            TextBase.Text = ((rectangleRatingHigh + rectangleRatingLow) / 2).ToString() + "%";
            if (Global.Config.getModuleParam("performance", "showLine", "1") == "1")
            {
                totperformanceLine.Right = totperformanceLine.Left;
                totperformanceLine.Bottom = Convert.ToInt32(ratingTot - (totperformance / 100 * (maxhight / 100)));
                totperformanceLine.Top = Convert.ToInt32(ratingTot - (totperformance / 100 * (maxhight / 100)));
                totperformanceLine.Right = reight;
                totperformanceLine.LineStyle = LineStyle.SingleLine;
                performanceratingtot.Top = Convert.ToInt32(ratingTot - 450 - (totperformance / 100 * (maxhight / 100)));

                performanceratingtot.Text = "Leistung " + "\r" + (Convert.ToInt16(totperformance) / rectangleRatingHigh * (rectangleRatingHigh / 100) + rectangleRatingLow).ToString() + "%";
                //performanceratingtot.Text = "Leistung " + "\r" + (Convert.ToInt16(totperformance) / 50).ToString() + "%";
                if (totperformance / 100 + 50 > 90 && totperformance / 100 + 50 < 106)
                {
                    Line100Precent.LineStyle = LineStyle.NoLine;
                    Text100Precent.Text = "";
                }

            }
            else
            {
                totperformanceLine.LineStyle = LineStyle.NoLine;
                performanceratingtot.Top = 9500;
                performanceratingtot.Width = 3000;
                performanceratingtot.Text = "Leistungsgrad " + Math.Round(((rectangleRatingHigh + rectangleRatingLow) * (totperformance / 10000)), 2).ToString() + "%";
            }



            // RatingText.Text = "Leistungswert " + (Convert.ToInt16(totperformance) / 100 + 50) + "% entspricht " + (Convert.ToInt16( multiplikator  * totperformance / 100 )) + "% lohnrelevantem Leistungsanteil.";

        }

        private int moveTextbox(string performanceCriteria)
        {
            int move = 0;
            if (performanceCriteria.Length > 33)
            {
                move = 150;
            }
            return move;
        }


        private void SetExpectation(ReportDocument rpt1, DBData db, LanguageMapper map, ConnectionInfo connectionInfo)
        {
            ReportDocument subrep = rpt1.OpenSubreport("PerformanzeratingExpectationSub");

            DataTable tableCriteria = db.getDataTableExt("select distinct CRITERIA_REF, " + db.langAttrName("PERFORMANCERATING_ITEMS", "CRITERIA_TITLE") + " from PERFORMANCERATING_ITEMS where PERFORMANCERATING_REF=" + _performanceRatingID, "PERFORMANCERATING_ITEMS");



            // delete temporary table if exists
            db.connect();
            string tbl_del = " IF NOT OBJECT_ID('tempdb..[##PerformanceratingDetail_%ratingid%]') IS NULL "
                              + "DROP TABLE [##PerformanceratingDetail_%ratingid%]";

            db.execute(tbl_del.Replace("%ratingid%", _performanceRatingID.ToString()));
            //create and fill temporary table
            string tbl_create = "CREATE TABLE [##PerformanceratingDetail_%ratingid%]("
                        + "[CRITERIA_TITLE] [varchar](128) NULL,"
                        + "[EXPECTATION_DESCRIPTION] [varchar](3000) NULL,"
                        + "[LEVEL_TITLE] [varchar](128) NULL,"
                        + "[ARGUMENTS] [varchar](3000) NULL,"
                        + "[MEASURE] [varchar](2000) NULL"
                        + ") ON [PRIMARY]";
            db.execute(tbl_create.Replace("%ratingid%", _performanceRatingID.ToString()));

            foreach (DataRow rowCriteria in tableCriteria.Rows)
            {
                long criteriaID = ch.psoft.Util.Validate.GetValid(rowCriteria["CRITERIA_REF"].ToString(), -1L);
                DataTable tableItems = db.getDataTableExt("select ID from PERFORMANCERATING_ITEMS where PERFORMANCERATING_REF=" + _performanceRatingID + " and CRITERIA_REF=" + criteriaID, "PERFORMANCERATING_ITEMS");
                if (tableItems.Rows.Count > 0)
                {

                    foreach (DataRow itemsRow in tableItems.Rows)
                    {
                        String itemsID = ch.psoft.Util.Validate.GetValid(itemsRow[db.langAttrName(itemsRow.Table.TableName, "ID")].ToString(), "");
                        String sql = "INSERT INTO [tempdb].[##PerformanceratingDetail_" + _performanceRatingID.ToString() + "] ([CRITERIA_TITLE], [EXPECTATION_DESCRIPTION], [LEVEL_TITLE], [ARGUMENTS], [MEASURE])";


                        sql = sql + " VALUES ('" + db.lookup("CRITERIA_TITLE_" + map.LanguageCode, "PERFORMANCERATING_ITEMS", "ID=" + itemsID, " ").ToString().Trim().Replace("'", "''") + "', '"
                                      + db.lookup("EXPECTATION_DESCRIPTION_" + map.LanguageCode, "PERFORMANCERATING_ITEMS", "ID=" + itemsID, " ").ToString().Trim().Replace("'", "''") + "', '"
                                      + db.lookup("LEVEL_TITLE_" + map.LanguageCode, "PERFORMANCERATING_ITEMS", "ID=" + itemsID, " ").ToString().Trim().Replace("'", "''") + "', '"
                                      + db.lookup("ARGUMENTS", "PERFORMANCERATING_ARGUMENTS", "PERFORMANCERATING_ITEM_ID = " + itemsID, " ").ToString().Trim().Replace("'", "''") + "', '"
                                      + db.lookup("MEASURE", "PERFORMANCERATING_ARGUMENTS", "PERFORMANCERATING_ITEM_ID = " + itemsID, " ").ToString().Trim().Replace("'", "''") + "')";

                        db.execute(sql);
                    }
                }


            }
            Tables tables = rpt1.Subreports[0].Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                tableLogonInfo.ConnectionInfo.DatabaseName = "TempDb";
                table.ApplyLogOnInfo(tableLogonInfo);
                table.Location = "##PerformanceratingDetail_" + _performanceRatingID.ToString();


            }

            //ChangeReportTable("LeistungsbewertungenOe_" + db.userId.ToString(), connectionInfo, rpt1);

        }
        private void SetMbO(ReportDocument rpt1, DBData db, LanguageMapper map, ConnectionInfo connectionInfo)
        {
            ReportDocument subrep = rpt1.OpenSubreport("SubRepPbMbO.rpt");

            string turnTitle = db.lookup("TITLE_DE", "OBJECTIVE_TURN", "ID = " + db.lookup("OBJECTIVE_TURN_ID", "OBJECTIVE", "ID = " + mboId, "0"), "").ToString();

            DataTable tableMbO = db.getDataTableExt("SELECT PERSON.ID AS PersonId, OBJECTIVE_1.ID AS ObectiveId,(SELECT TITLE FROM OBJECTIVE WHERE (OBJECTIVE_1.PARENT_ID = ID)) AS TitleParentObjective, OBJECTIVE_1.NUMBER AS ObjectiveNumber, OBJECTIVE_1.TITLE AS ObjectveTitle, "
                                                           + "OBJECTIVE_1.DESCRIPTION AS ObjectiveDescription, OBJECTIVE_1.STARTDATE AS ObjectiveStartdate, "
                                                           + "OBJECTIVE_1.DATEOFREACHING AS ObjectiveDateOfReaching, OBJECTIVE_1.MEMO AS ObjectiveMemo, OBJECTIVE_1.TARGETVALUE AS ObjectiveTargetvalue, "
                                                           + "OBJECTIVE_1.ACTIONNEED AS ObjectiveActionNeed, OBJECTIVE_1.MEASUREKRIT AS ObjectiveMeasureKrit, OBJECTIVE_1.TYP AS ObjectiveTyp, "
                                                           + "OBJECTIVE_TURN.TITLE_DE AS ObjectiveTurn, OBJECTIVE_PERSON_RATING.RATING_WEIGHT AS RatingWeight, "
                                                           + "MEASUREMENT_TYPE.TITLE_DE AS MeasurementType, OBJECTIVE_PERSON_RATING.RATING "
                                                           + "FROM   MEASUREMENT_TYPE INNER JOIN OBJECTIVE AS OBJECTIVE_1 INNER JOIN "
                                                           + "EMPLOYMENT INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID ON OBJECTIVE_1.PERSON_ID = PERSON.ID INNER JOIN "
                                                           + "OBJECTIVE_TURN ON OBJECTIVE_1.OBJECTIVE_TURN_ID = OBJECTIVE_TURN.ID ON "
                                                           + "MEASUREMENT_TYPE.ID = OBJECTIVE_1.MEASUREMENT_TYPE_ID LEFT OUTER JOIN "
                                                           + "OBJECTIVE_PERSON_RATING ON OBJECTIVE_1.ID = OBJECTIVE_PERSON_RATING.OBJECTIVE_ID "
                                                           + "WHERE PERSON.ID = " + _personID + " AND OBJECTIVE_TURN.TITLE_DE = '" + turnTitle + "'");

            // delete temporary table if exists
            db.connect();
            string tbl_del = " IF NOT OBJECT_ID('tempdb..[##MbOReport_%userid%]') IS NULL "
                              + "DROP TABLE [##MbOReport_%userid%]";
            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));
            //create and fill temporary table
            string tbl_create = "CREATE TABLE [##MbOReport_%userid%]("
                        + "[PersonId] [bigint] NULL,"
                        + "[ObectiveId] [bigint] NULL,"
                        + "[TitleParentObjective] [varchar](128) NULL,"
                        + "[ObjectiveNumber] [varchar](128) NULL,"
                        + "[ObjectveTitle] [varchar](128) NULL,"
                        + "[ObjectiveDescription] [varchar](2000) NULL,"
                        + "[ObjectiveStartdate] [datetime] NULL,"
                        + "[ObjectiveDateOfReaching] [datetime] NULL,"
                        + "[ObjectiveMemo] [varchar](3000) NULL,"
                        + "[ObjectiveTargetvalue] [varchar](128) NULL,"
                        + "[ObjectiveActionNeed] [varchar](1000) NULL,"
                        + "[ObjectiveMeasureKrit] [varchar](3000) NULL,"
                        + "[ObjectiveTyp] [int] NULL,"
                        + "[ObjectiveTurn] [varchar](128) NULL,"
                        + "[RatingWeight] [float] NULL,"
                        + "[MeasurementType] [varchar](128) NULL,"
                        + "[RATING] [int] NULL,"
                        + ") ON [PRIMARY]";
            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

            if (tableMbO.Rows.Count > 0)
            {

                foreach (DataRow itemsRow in tableMbO.Rows)
                {
                    String sql = "INSERT INTO [##MbOReport_" + db.userId.ToString() + "] ([PersonId], [ObectiveId], [TitleParentObjective], [ObjectiveNumber], [ObjectveTitle], [ObjectiveDescription], " +
                    "[ObjectiveStartdate], [ObjectiveDateOfReaching], [ObjectiveMemo], [ObjectiveTargetvalue], [ObjectiveActionNeed], [ObjectiveMeasureKrit],  [ObjectiveTyp], [ObjectiveTurn], [RatingWeight], [MeasurementType], [RATING])";

                    //try{
                    sql = sql + " VALUES ('" + itemsRow[0] + "','" + itemsRow[1] + "','" + itemsRow[2].ToString().Trim().Replace("'", "''") + "','" + itemsRow[3] + "','" + itemsRow[4].ToString().Trim().Replace("'", "''") + "','" + itemsRow[5].ToString().Trim().Replace("'", "''") +
                        "','" + ((DateTime)itemsRow[6]).ToString("MM/dd/yyyy") + "','" + ((DateTime)itemsRow[7]).ToString("MM/dd/yyyy") + "','" + itemsRow[8].ToString().Trim().Replace("'", "''") + "','" + itemsRow[9] + "','" + itemsRow[10].ToString().Trim().Replace("'", "''") + "','" + itemsRow[11].ToString().Trim().Replace("'", "''") + "','" + itemsRow[12] + "','" + itemsRow[13] + "','" + itemsRow[14] +
                        "','" + itemsRow[15].ToString().Trim().Replace("'", "''") + "','" + itemsRow[16] + "')";
                    db.execute(sql);
                    //}
                    //catch (InvalidCastException e)
                    //{
                    //    Response.Write("<script language='javascript'> { window.close();}</script>");
                    //    ClientScriptManager cs = Page.ClientScript;
                    //    cs.RegisterStartupScript(this.GetType(), "myalert", "alert('Der Report kann nicht erstellt werden da die Ziekvereinbarungen unvollständig erstellt sind!'); window.location = \"" + Global.Config.baseURL +"/Performance/EmploymentRating.aspx?performanceRatingID=444193&employmentID=3294&mode=edit&type=leader"  + "\";", true);
                    //}


                }
            }
            Tables tables = rpt1.Subreports[1].Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                tableLogonInfo.ConnectionInfo.DatabaseName = "TempDb";
                table.ApplyLogOnInfo(tableLogonInfo);
                table.Location = "##MbOReport_" + db.userId.ToString();
            }

        }
    }
}