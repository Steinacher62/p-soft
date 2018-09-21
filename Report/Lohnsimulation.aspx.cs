using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Report
{
    public partial class Lohnsimulation : System.Web.UI.Page
    {
        DBData db;
        private int selects = 0;
        private int updates = 0;
        private Dictionary<string, float> selectCache = new Dictionary<string, float>();
        private double averageWage;

        protected void Page_Load(object sender, EventArgs e)
        {
            // connect to database
            db = DBData.getDBData(Session);
            db.connect();

            // apply language
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
            if (map == null)
            {
                map = LanguageMapper.getLanguageMapper(Application);
            }
            paramSimLabel.Text = map.get("Lohnsimulation", "paramSim");
            chkStandard.Text = map.get("Lohnsimulation", "stdSim");
            chkWertPunkt.Text = map.get("Lohnsimulation", "valPoint");
            valPointUnitLabel.Text = map.get("Lohnsimulation", "Unit");
            totalSumCorrLabel.Text = map.get("Lohnsimulation", "totalSumCorr");
            chkKorrekturabsolut.Text = map.get("Lohnsimulation", "abs");
            absUnitLabel.Text = map.get("Lohnsimulation", "Unit");
            chkKorrekturrelativ.Text = map.get("Lohnsimulation", "rel");
            fixChangeLabel.Text = map.get("Lohnsimulation", "fixChange");
            diffNomActLabel.Text = map.get("Lohnsimulation", "diffNomAct");
            changePercentLabel.Text = map.get("Lohnsimulation", "changePercent");
            incBaseSalaryLabel.Text = map.get("Lohnsimulation", "incBaseSalary");
            cmdOk.Text = map.get("Lohnsimulation", "showReport");
            chkSubOEs.Text = map.get("Lohnsimulation", "subOEs");
            expotrOptions.Text = map.get("Lohnsimulation", "exportOptions");
            maxIncreaseLabel.Text = map.get("Lohnsimulation", "maxIncrease");
            increaseUnitLabel.Text = map.get("Lohnsimulation", "Unit");
            roundLabel.Text = map.get("Lohnsimulation", "round");
            roundUnitLabel.Text = map.get("Lohnsimulation", "Unit");
            checkBoxCorrectionExport.Text = map.get("Lohnsimulation", "correctionExport");
            checkBoxAddressExport.Text = map.get("Lohnsimulation", "addressExport");

            if (!Page.IsPostBack)
            {
                //list OEs
                DataTable tblOE = db.getDataTableExt("SELECT ID,TITLE_DE FROM ORGENTITY ORDER BY TITLE_DE", new object[0]);
                foreach (DataRow aktRow in tblOE.Rows)
                {
                    lstOE.Items.Add(new ListItem(aktRow["TITLE_DE"].ToString(), aktRow["ID"].ToString()));
                }

                //get and select top OE
                try
                {
                    DataTable topOE = db.getDataTableExt("SELECT ID FROM ORGENTITY WHERE PARENT_ID IS NULL", new object[0]);
                    lstOE.SelectedValue = topOE.Rows[0][0].ToString();
                }
                catch
                {
                }

                // show multiple input boxes for fix salary change?
                string abwProzent = Global.Config.getModuleParam("report", "abwProzent", "0");
                if (abwProzent == "0")
                {
                    pFix.Visible = true;
                    pRel.Visible = false;
                }
                else
                {
                    pFix.Visible = false;
                    pRel.Visible = true;
                }

                double fixPointValue = Convert.ToDouble(db.lookup("FIX_POINT_VALUE", "VARIANTE", "HAUPTVARIANTE = 1"));
                if(fixPointValue > 0)
                {
                    chkWertPunkt.Checked = true;
                    txtWertPunkt.Text = fixPointValue.ToString();

                }
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            // delete error message
            lblFehler.Text = "";

            // check lock
            string filename = Server.MapPath(Global.Config.baseURL + "/tmp/lohnsimulation_lock.txt");

            if (File.Exists(filename))
            {
                // there is a lock, check if it is older than 2 minutes
                TextReader rLock = new StreamReader(filename);
                DateTime lockTime = DateTime.Parse(rLock.ReadLine());
                rLock.Close();
                TimeSpan tLock = DateTime.Now.Subtract(lockTime);
                if (tLock.Minutes < 2)
                {
                    lblFehler.Text = "Es wird schon eine Lohnsimulation durchgeführt! Bitte versuchen Sie es in ein paar Minuten nochmal!";
                    return;
                }
                File.Delete(filename);
            }

            TextWriter wLock = new StreamWriter(filename);
            wLock.Write(DateTime.Now);
            wLock.Close();

            // fill temporary person table which contains only persons in selected OEs
            // first delete all entries in table
            db.execute("DELETE FROM PERSON_temp");

            // get list of selected OEs
            string oe = lstOE.SelectedValue;
            if (chkSubOEs.Checked)
            {
                oe = db.Orgentity.addAllSubOEIDs(oe);
            }

            // fill table
            db.execute("INSERT INTO PERSON_temp "
                + "SELECT dbo.PERSON.ID, dbo.PERSON.EXTERNAL_REF, dbo.PERSON.PNAME, "
                + "dbo.PERSON.FIRSTNAME, dbo.PERSON.PERSONNELNUMBER, dbo.PERSON.DATEOFBIRTH, "
                + "dbo.PERSON.ENTRY, dbo.PERSON.LEAVING, dbo.PERSON.TYP, dbo.PERSON.BERUFSERFAHRUNG, "
                + "dbo.PERSON.COSTCENTER, dbo.PERSON.COSTCENTER_TITLE, dbo.PERSON.BESCH_GRAD, dbo.PERSON.SALUTATION_ADDRESS, dbo.PERSON.SALUTATION_LETTER "
                + "FROM       dbo.PERSON INNER JOIN "
                + "dbo.OEPERSONV ON dbo.PERSON.ID = dbo.OEPERSONV.ID "
                + "WHERE dbo.OEPERSONV.OE_ID IN (" + oe + ")");

            if (db.lookup("count(ID)", "PERSON_TEMP", "", 0) == 0)
            {
                lblFehler.Text = "Ihre Datenselektion enthält keine Daten. Bitte ändern sie die Datenselektion";
                File.Delete(filename); 
                return;
            }

            // initialize session
            Session["Lohnsimulation"] = "1";
            Session["WertPunkt"] = "0";
            Session["LohnsummenKorrRel"] = "0";
            Session["LohnsummenKorrAbs"] = "0";
            Session["LohnaendAbs"] = "0";
            Session["LohnaendRel"] = "0";
            Session["ErhoeBasislohn"] = "0";
            Session["MaxIndErhoehung"] = "0";
            Session["Runden"] = "0";
            Session["ExportAdresse"] = "0";
            Session["ExportKorrekturen"] = "0";

            if (chkWertPunkt.Checked)
            {
                Session["WertPunkt"] = txtWertPunkt.Text;
            }

            if (chkKorrekturrelativ.Checked)
            {
                Session["LohnsummenKorrRel"] = txtKorrekturrelativ.Text;
            }

            if (chkKorrekturabsolut.Checked)
            {
                Session["LohnsummenKorrAbs"] = txtKorrekturabsolut.Value;
            }

            if (chkAbsolut.Checked)
            {
                Session["LohnaendAbs"] = txtAbsolut.Value;
            }

            if (chkRelativ.Checked)
            {
                Session["LohnaendRel"] = txtRelativ.Value;
            }

            Session["ErhoeBasislohn"] = txtBasislohn.Value;
            Session["ExportAdresse"] = checkBoxAddressExport.Checked;
            Session["ExportKorrekturen"] = checkBoxCorrectionExport.Checked;
            Session["MaxIndErhoehung"] = txtMaxIncrease.Text;
            Session["Runden"] = txtRunden.Text;

            // delete temporary table if exists
            string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[lohnsimulation_%userid%]') "
                              + "AND type in (N'U')) "
                                + "DROP TABLE [dbo].[lohnsimulation_%userid%]";
            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

            // create table
            string tbl_create = "CREATE TABLE [dbo].[lohnsimulation_%userid%]("
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
                                + "[LMCode] [bigint] NULL,"
                                + "[ExtReflohn] [float] NULL,"
                                + "[Funktionsbewertung] [float] NULL,"
                                + "[Ausschluss_Ab] [int] NULL,"
                                + "[AUSSCHLUSS_BIS] [datetime] NULL,"
                                + "[AUSSCHLUSS_LOHN] [int] NULL,"
                                + "[Stellenbezeichnung] [varchar](128) NULL,"
                                + "[ID] [bigint] NULL,"
                                + "[Bewertungsdatum] [datetime] NULL,"
                                + "[Leistungswert] [float] NULL,"
                                + "[Erfahrung] [float] NULL,"
                                + "[ENGAGEMENT] [varchar](64) NULL,"
                                + "[PunkteInklErfLeist] [float] NULL,"
                                + "[SummePunkteInklErfLeist] [float] NULL,"
                                + "[LohnsummeIst] [float] NULL,"
                                + "[WertProPunkt] [float] NULL,"
                                + "[SollBasislohn] [float] NULL,"
                                + "[SollLeistung] [float] NULL,"
                                + "[SollErfahrung] [float] NULL,"
                                + "[Solllohnkorrektur] [float] NULL,"
                                + "[fehler] [varchar](64) NULL"
                                + ") ON [PRIMARY]";
            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

            // fill table
            string performanceRatingBase = Global.Config.getModuleParam("performance", "performanceRatingBase", "100");
            string anzMonatsloehne = Global.Config.getModuleParam("report", "anzMonatsloehne", "13");
            string maxErfahrung = Global.Config.getModuleParam("report", "maxErfahrung", "10");
            string glaetten = Global.Config.getModuleParam("report", "glaetten","0").ToString();
            string inklZiele = Global.Config.getModuleParam("report", "inclObjectives", "0").ToString();
            string fbwMin ="''";
            string fbwMax = "''";
            string lohnMin = "''";
            string lohnMax = "''";

            if(glaetten.Equals("1"))
            {
                fbwMin = (db.lookup("FbwMin", "FBW_LOHN_MIN_MAX_V", "")).ToString();
                fbwMax = (db.lookup("FbwMax", "FBW_LOHN_MIN_MAX_V", "")).ToString();
                lohnMin = Global.Config.getModuleParam("report", "minSolllohn", "0").ToString();
                lohnMax = (db.lookup("LohnMax", "FBW_LOHN_MIN_MAX_V", "")).ToString();
            }

            //if (Global.isModuleEnabled("spz"))
            //{
            //    db.execute("INSERT INTO lohnsimulation_" + db.userId.ToString() + " SELECT * FROM f_Lohnsimulation3(" + performanceRatingBase + ", " + anzMonatsloehne + ", " + maxErfahrung + ", " + glaetten + ", " + fbwMin + ", " + fbwMax + ", " + lohnMin + ", " + lohnMax + ", " + inklZiele + " )");
            //}
            //else
            //{
                db.execute("INSERT INTO lohnsimulation_" + db.userId.ToString() + " SELECT * FROM f_Lohnsimulation3(" + performanceRatingBase + ", " + anzMonatsloehne + ", " + maxErfahrung + ", " + glaetten + ", " + fbwMin + ", " + fbwMax + ", " + lohnMin + ", " + lohnMax  + ")", 600, 600);
               
            //}
            // add columns for calculation
            string tbl_alter = "ALTER TABLE lohnsimulation_%userid% "
                               + "ADD [Basislohn] [float] NULL,"
                               + "[Erfahrung_b] [float] NULL,"
                               + "[Leistung] [float] NULL,"
                               + "[Leistung_p] [float] NULL,"
                               + "[Solllohn_b] [float] NULL,"
                               + "[genAend] [float] NULL,"
                               + "[genAend_p] [float] NULL,"
                               + "[indAend] [float] NULL,"
                               + "[indAend_p] [float] NULL,"
                               + "[ErhTot] [float] NULL,"
                               + "[Erh_p] [float] NULL,"
                               + "[Lohn_neu] [float] NULL,"
                               + "[Leistungvorjahr] [float] NULL,"
                               + "[Leistungvorletztesjahr] [float] NULL";
            db.execute(tbl_alter.Replace("%userid%", db.userId.ToString()));

            // update performacerating previous years
            DataTable lastYearTable = db.getDataTable("SELECT * FROM f_LeistungswerteJahr(" + performanceRatingBase + ", '01.01." + (DateTime.Now.Year - 1).ToString() + "', '12.31." + (DateTime.Now.Year - 1).ToString() + "');");
            
            foreach (DataRow persnr_r in lastYearTable.Rows)
            {
                string persnr = persnr_r[0].ToString();
                lohnSimUpdate("Leistungvorjahr", persnr_r["Leistungswert"].ToString(), persnr.ToString());
            }

            lastYearTable = db.getDataTable("SELECT * FROM f_LeistungswerteJahr(" + performanceRatingBase + ", '01.01." + (DateTime.Now.Year - 2).ToString() + "', '12.31." + (DateTime.Now.Year - 2).ToString() + "');");

            foreach (DataRow persnr_r in lastYearTable.Rows)
            {
                string persnr = persnr_r[0].ToString();
                lohnSimUpdate("Leistungvorletztesjahr", persnr_r["Leistungswert"].ToString(), persnr.ToString());
            }
            // update table with calculated results
            DataTable persnrs = db.getDataTable("SELECT Personalnummer FROM lohnsimulation_" + db.userId.ToString());
            
            // calculate how much we have to distribute
            lohnsumme = float.Parse(db.lookup("LohnsummeIst", "lohnsimulation_" + db.userId.ToString(), "").ToString());

            // If module Wohlen enabled calculate median


            if (Global.isModuleEnabled("wohlen")) 
            {
                //averageWage = Convert.ToDouble(db.lookup("AVG(Istlohn)", "lohnsimulation_" + db.userId.ToString(), ""));
                averageWage = Convert.ToDouble(db.lookup("MAX(Istlohn)", "(SELECT TOP 50 PERCENT Istlohn FROM lohnsimulation_" + db.userId.ToString() + " ORDER BY Istlohn) AS H1", ""));
                
            }

            if (chkKorrekturrelativ.Checked)
            {
                zu_verteilen = lohnsumme / 100 * (float)txtKorrekturrelativ.Value;
            }
            else
            {
                zu_verteilen = (float)txtKorrekturabsolut.Value;
            }

            // calculate how much we can lift the "solllohn"
            float soll_total = 0;
            float ist_total = 0;

            foreach (DataRow persnr_r in persnrs.Rows)
            {
                string persnr = persnr_r[0].ToString();

                ist_total += lohnSimLookup("IstLohn", persnr);
                if (!fix(persnr))
                {
                    soll_total += Solllohn_b(persnr);
                }
                else
                {
                    soll_total += lohnSimLookup("IstLohn", persnr);
                }
            }

            float korr = 0;
            if (chkKorrekturrelativ.Checked)
            {
                korr = lohnsumme / 100 * (float)txtKorrekturrelativ.Value; 
            }
            else
            {
                korr = (float)txtKorrekturabsolut.Value;
            }

            if ((ist_total + korr) > soll_total)
            {
                soll_p = (ist_total + korr) / soll_total;
            }

            // first cycle to calculate negative difference and how much we distribute "fix"
            foreach (DataRow persnr_r in persnrs.Rows)
            {
                string persnr = persnr_r[0].ToString();

                // Basislohn
                lohnSimUpdate("Basislohn", Basislohn(persnr), persnr);

                // Erfahrung
                lohnSimUpdate("Erfahrung_b", Erfahrung(persnr), persnr);

                // Leistung
                lohnSimUpdate("Leistung", Leistung(persnr), persnr);

                // Leistung %
                lohnSimUpdate("Leistung_p", Leistung_p(persnr), persnr);

                // Solllohn
                lohnSimUpdate("Solllohn_b", Solllohn_b(persnr), persnr);

                // Fehler eintragen (test)
                //lohnSimUpdate("fehler", "'test_" + persnr + "'", persnr);

                if (!fix(persnr))
                {
                    NegAbweichung(persnr);
                }

                verteilt_fix += genAend(persnr);
            }

            // do we have enough money to distribute what we want?
            if ((zu_verteilen - verteilt_fix) < 0)
            {
                // would it be enough if correction factor would be 0?
                if (zu_verteilen - (verteilt_fix / soll_p) < 0)
                {
                    // no chance
                    float missing = zu_verteilen - (verteilt_fix / soll_p);
                    lblFehler.Text = "Es stehen nicht genug Mittel für die Verteilung der fixen Erh&ouml;hungen zur Verf&uuml;gung! Fehlbetrag: " + missing.ToString() + " CHF";
                    return;
                }
                else
                {
                    // ok, set correction factor to 1
                    soll_p = 1;

                    // we have to calculate the last round again
                    SummeNegAbweichungen = 0;
                    foreach (DataRow persnr_r in persnrs.Rows)
                    {
                        string persnr = persnr_r[0].ToString();

                        // Basislohn
                        lohnSimUpdate("Basislohn", Basislohn(persnr), persnr);

                        // Erfahrung
                        lohnSimUpdate("Erfahrung_b", Erfahrung(persnr), persnr);

                        // Leistung
                        lohnSimUpdate("Leistung", Leistung(persnr), persnr);

                        // Leistung %
                        lohnSimUpdate("Leistung_p", Leistung_p(persnr), persnr);

                        // Solllohn
                        lohnSimUpdate("Solllohn_b", Solllohn_b(persnr), persnr);

                        if (!fix(persnr))
                        {
                            NegAbweichung(persnr);
                        }
                    }
                }
            }

            zu_verteilen -= verteilt_fix;

            // second cycle to calculate missing values
            foreach (DataRow persnr_r in persnrs.Rows)
            {
                string persnr = persnr_r[0].ToString();

                // gen. Aend.
                lohnSimUpdate("genAend", genAend(persnr), persnr);

                // gen. Aend. %
                lohnSimUpdate("genAend_p", genAend_p(persnr), persnr);

                // ind. Aend.
                lohnSimUpdate("indAend", indAend(persnr), persnr);

                // ind. Aend. %
                lohnSimUpdate("indAend_p", indAend_p(persnr), persnr);

                // ErhTot
                lohnSimUpdate("ErhTot", ErhTot(persnr), persnr);

                // Erh. %
                lohnSimUpdate("Erh_p", Erh_p(persnr), persnr);

                // Lohn neu
                lohnSimUpdate("Lohn_neu", Lohn_neu(persnr), persnr);
            }


            if ((double)txtMaxIncrease.Value > 0)
            {
                double korrMittelBegrenzung = zu_verteilen;
                double zusaetzlicheMittel = 0;
                foreach (DataRow persnr_r in persnrs.Rows)
                {
                    string persnr = persnr_r[0].ToString();
                    float erhInd = lohnSimLookup("IndAend", persnr);

                    if (txtMaxIncrease.Value < erhInd)
                    {
                        double negAbw = lohnSimLookup("IstLohn", persnr) - lohnSimLookup("Solllohn_b", persnr);
                        korrMittelBegrenzung -= Convert.ToDouble(txtMaxIncrease.Value);
                        zusaetzlicheMittel += erhInd - Convert.ToDouble(txtMaxIncrease.Value);
                    }

                }

                double korrekturfaktor = korrMittelBegrenzung / (korrMittelBegrenzung - zusaetzlicheMittel);
                negAbweichungBegrenzung((float)korrekturfaktor);


                //wenn Mittel aus begrenzten Lohnerhöhungen zur verfügung  stehen oder Frauenfeld nochmals rechnen



                foreach (DataRow persnr_r in persnrs.Rows)
                {
                    string persnr = persnr_r[0].ToString();

                    // gen. Aend.
                    lohnSimUpdate("genAend", genAend(persnr), persnr);

                    // gen. Aend. %
                    lohnSimUpdate("genAend_p", genAend_p(persnr), persnr);

                    // ind. Aend.
                    lohnSimUpdate("indAend", indAend(persnr, 2), persnr);

                    // ind. Aend. %
                    lohnSimUpdate("indAend_p", indAend_pFinish(persnr), persnr);

                    // ErhTot
                    lohnSimUpdate("ErhTot", ErhTotLastRound(persnr), persnr);

                    // Erh. %
                    lohnSimUpdate("Erh_p", Erh_pFinal(persnr), persnr);

                    // Lohn neu
                    lohnSimUpdate("Lohn_neu", Lohn_neu(persnr), persnr);
                }
            }

            // remove lock
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            db.disconnect();
       
            Response.Redirect("CrystalReportViewer.aspx?alias=Lohnsimulation", true);
        }

        protected float lohnSimLookup(string column, string persnr)
        {
            // is data in cache?
            float wert;
            if (!selectCache.TryGetValue(column + "_" + persnr, out wert))
            {
                // not in cache
                wert = float.Parse(db.lookup(column, "lohnsimulation_" + db.userId.ToString(), "Personalnummer = '" + persnr + "'").ToString());

                selectCache.Add(column + "_" + persnr, wert);

                selects++;
            }

            return wert;
        }

        protected void lohnSimUpdate(string column, string value, string persnr)
        {
            db.execute("UPDATE lohnsimulation_" + db.userId.ToString() + " SET " + column + "=" + value.Replace(",",".") + " WHERE Personalnummer='" + persnr + "'");

            updates++;
        }

        protected void lohnSimUpdate(string column, float value, string persnr)
        {
            lohnSimUpdate(column, value.ToString(), persnr);
        }

        float soll_p = 1;
        float verteilt_fix = 0;
        float zu_verteilen = 0;
        float lohnsumme = 0;

        // Funktionen aus Crystal übernommen
        protected float SummeNegAbweichungen = 0;

        protected float Basislohn(string persnr)
        {
            
            float Punktwert = (float)txtWertPunkt.Value;
            //float Punktwert = float.Parse(txtWertPunkt.Text);
            if (chkWertPunkt.Checked)
            {
                return lohnSimLookup("Funktionsbewertung", persnr) * Punktwert;
            }
            else
            {
                return lohnSimLookup("SollBasislohn", persnr);
            }
        }

        protected float Erfahrung(string persnr)
        {
            float Punktwert = (float)txtWertPunkt.Value;
            if (chkWertPunkt.Checked)
            {
                return lohnSimLookup("Funktionsbewertung", persnr) * Punktwert / 100 * lohnSimLookup("Erfahrung", persnr);
            }
            else
            {
                return lohnSimLookup("SollErfahrung", persnr);
            }
        }

        protected float Erh_p(string persnr)
        {
            return ErhTot(persnr) / (lohnSimLookup("IstLohn", persnr) / 100);
        }

        protected float Erh_pFinal(string persnr)
        {
            return (float)(double)db.lookup("IndAend", "lohnsimulation_" + db.userId.ToString(), "Personalnummer = '" + persnr + "'") / (lohnSimLookup("IstLohn", persnr) / 100);
        }

        protected float ErhTot(string persnr)
        {
            //if (!fix(persnr))
            //{
            return genAend(persnr) + indAend(persnr);
            //}
            //else
            //{
            //    return 0;
            //}
        }

        protected float ErhTotLastRound(string persnr)
        {
            //if (!fix(persnr))
            //{
            return genAend(persnr) + (float)(double)db.lookup("IndAend", "lohnsimulation_" + db.userId.ToString(), "Personalnummer = '" + persnr + "'");
            //}
            //else
            //{
            //    return 0;
            //}
        }




        protected void negAbweichungBegrenzung(float negAbweichung)
        {
            SummeNegAbweichungen = SummeNegAbweichungen / negAbweichung;
        }


        protected float genAend(string persnr)
        {

            //Wenn Person "Lohn einfrieren" keine fixe Lohnerhöhung
            //if (fix(persnr))
            //{
            //    return 0;
            //}


            float LohnAendAbs = float.Parse(txtAbsolut.Text);
            if (LohnAendAbs > 0)
            {
                return LohnAendAbs;
            }
            else
            {
                string abwProzent = Global.Config.getModuleParam("report", "abwProzent", "0");
                if (abwProzent == "0")
                {
                    float LohnAendRel = float.Parse(txtRelativ.Text);
                    if (!Global.isModuleEnabled("wohlen"))
                    {
                        return lohnSimLookup("IstLohn", persnr) / 100 * LohnAendRel;
                    }
                    else
                    {
                        return (float)averageWage / 100 * LohnAendRel;
                    }
                }
                else
                {
                    float LohnAendProzent = AendRelProzent(persnr);
                    return lohnSimLookup("IstLohn", persnr) / 100 * LohnAendProzent;
                }
            }
        }

        protected float genAend_p(string persnr)
        {
            float genLohnaenderung = 0;
            if (Global.isModuleEnabled("wohlen") && genAend(persnr) > 0)
            {
                genLohnaenderung = (float)averageWage /100 / (lohnSimLookup("IstLohn", persnr) / 100);
            }
            else
            {
                genLohnaenderung = genAend(persnr) / (lohnSimLookup("IstLohn", persnr) / 100);
            }
            return genLohnaenderung;
        }

        protected float indAend(string persnr)
        {
            if (fix(persnr))
            {
                return 0;
            }

            else if (Solllohn_b(persnr) > lohnSimLookup("IstLohn", persnr))
            {
                //Bei frauenfeld keine individuelle Erhöhung wenn Erfahrung > 10%
                if (Global.isModuleEnabled("frauenfeld"))
                {
                    if ((float)lohnSimLookup("Erfahrung", persnr) > 10)
                    {
                        return 0;
                    }
                }


                if ((double)txtMaxIncrease.Value > 0)
                {
                    if (!db.lookup("IndAend", "lohnsimulation_" + db.userId.ToString(), "Personalnummer = '" + persnr + "'").ToString().Equals(""))
                    {
                        double erhInd = (double)db.lookup("IndAend", "lohnsimulation_" + db.userId.ToString(), "Personalnummer = '" + persnr + "'");
                        if (txtMaxIncrease.Value < erhInd)
                        {
                            return (float)txtMaxIncrease.Value;
                        }
                    }
                }

                return ZuVerteilenInd(persnr) / SummeNegAbweichung(persnr) * (Solllohn_b(persnr) - lohnSimLookup("IstLohn", persnr));

            }
            else
            {
                return 0;
            }
        }

        protected float indAend(string persnr, int cycle)
        {
            if (fix(persnr))
            {
                return 0;
            }

            else if (Solllohn_b(persnr) > lohnSimLookup("IstLohn", persnr))
            {
                //Bei frauenfeld keine individuelle Erhöhung wenn Erfahrung > 10%
                if (Global.isModuleEnabled("frauenfeld"))
                {
                    if ((float)lohnSimLookup("Erfahrung", persnr) > 10)
                    {
                        return 0;
                    }
                }




                float erhInd = ZuVerteilenInd(persnr) / SummeNegAbweichung(persnr) * (Solllohn_b(persnr) - lohnSimLookup("IstLohn", persnr));
                if (Convert.ToDouble(txtMaxIncrease.Value) == 0)
                {
                    return erhInd;
                }
                else
                {
                    if (txtMaxIncrease.Value >= erhInd)
                    {
                        return erhInd;
                    }
                    else
                    {
                        return (float)txtMaxIncrease.Value;
                    }
                }
            }
            else
            {
                return 0;
            }
        }

        protected float indAend_p(string persnr)
        {
            return indAend(persnr) / (lohnSimLookup("IstLohn", persnr) / 100);
        }


        protected float indAend_pFinish(string persnr)
        {
            return (lohnSimLookup("indAend", persnr) / (lohnSimLookup("IstLohn", persnr) / 100));
        }

        protected float Leistung(string persnr)
        {
            float Punktwert = (float)txtWertPunkt.Value;
            if (Punktwert > 0 && chkWertPunkt.Checked)
            {
                return lohnSimLookup("Funktionsbewertung", persnr) * Punktwert / 100 * lohnSimLookup("SollLeistung", persnr) / (lohnSimLookup("SollBasisLohn", persnr) / 100);
            }
            else
            {
                return lohnSimLookup("SollLeistung", persnr);
            }
        }

        protected float Leistung_p(string persnr)
        {
            return lohnSimLookup("SollLeistung", persnr) / (lohnSimLookup("SollBasislohn", persnr) / 100);
        }

        protected float Lohn_neu(string persnr)
        {
            return lohnSimLookup("IstLohn", persnr) + ErhTot(persnr);
        }

        protected float Lohn_neuFinal(string persnr)
        {
            return lohnSimLookup("IstLohn", persnr) + ErhTotLastRound(persnr);
        }

        protected void NegAbweichung(string persnr)
        {
            if (Solllohn_b(persnr) > lohnSimLookup("IstLohn", persnr))
            {
                SummeNegAbweichungen += (Solllohn_b(persnr) - lohnSimLookup("IstLohn", persnr));
                if (Global.isModuleEnabled("frauenfeld") && (float)lohnSimLookup("Erfahrung", persnr) > 10)
                {
                    SummeNegAbweichungen -= (Solllohn_b(persnr) - lohnSimLookup("IstLohn", persnr));
                }
            }
        }

        protected float Solllohn_b(string persnr)
        {
            return (Basislohn(persnr) + Erfahrung(persnr) + Leistung(persnr) + korrektur(persnr)) * soll_p;
        }

        protected float SummeNegAbweichung(string persnr)
        {
            return SummeNegAbweichungen;
        }

        protected float ZuVerteilenInd(string persnr)
        {
            return zu_verteilen;
        }
        // Ende Crystal

        protected float AendRelProzent(string persnr)
        {
            // gets the relative change of salary
            float solllohn = Solllohn_b(persnr);
            float istlohn = lohnSimLookup("IstLohn", persnr);
            float diff = istlohn - solllohn;

            float level0 = (float)txtAbw0.Value;
            float level0p = (float)txtAbw0p.Value;
            float level1 = (float)txtAbw1.Value;
            float level1p = (float)txtAbw1p.Value;
            float level2 = (float)txtAbw2.Value;
            float level2p = (float)txtAbw2p.Value;
            float level3 = (float)txtAbw3.Value;
            float level3p = (float)txtAbw3p.Value;

            if (istlohn < solllohn)
            {
                return level0p;
            }
            else
            {
                float abw = (diff / istlohn) * 100;

                if (abw < level0)
                {
                    return level0p;
                }
                else if (abw < level1)
                {
                    return level1p;
                }
                else if (abw < level2)
                {
                    return level2p;
                }
                else if (abw < level3)
                {
                    return level3p;
                }
                return 0;
            }
        }

        protected float korrektur(string persnr)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();

            float korr = 0;

            object objKorr = db.lookup("Korr1 + Korr2 + Korr3 + Korr4 AS korrektur", "Solllohnkorrektur", "PERSONNELNUMBER = '" + persnr + "'");

            if (objKorr != null && objKorr.ToString() != "")
            {
                korr = float.Parse(objKorr.ToString());
            }

            db.disconnect();

            return korr;
        }

        protected bool fix(string persnr)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();

            bool isFix = false;

            object objFix = db.lookup("fix", "Solllohnkorrektur", "PERSONNELNUMBER = '" + persnr + "'");

            if (objFix != null && objFix.ToString() != "" && objFix.ToString().Equals("True"))
            {
                isFix = true;
            }

            db.disconnect();

            return isFix;
        }


        // temp: override rendering to show how masterpage is integrated / 26.10.09 / mkr
        protected override void Render(HtmlTextWriter writer)
        {
            // *** Write the HTML into this string builder

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            StringWriter sw = new StringWriter(sb);

            ControlCollection temp = this.Controls;

            HtmlTextWriter hWriter = new HtmlTextWriter(sw);

            base.Render(hWriter);

            // *** store to a string

            string PageResult = sb.ToString();

            // *** Write it back to the server

            writer.Write(PageResult);

        }


    }
}
