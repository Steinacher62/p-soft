using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report.Controls;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Report
{
    /// <summary>
    /// Summary description for Report LeistungswerteNegVeränderung.
    /// </summary>  

    public partial class LeistungswerteNegVeraenderung : PsoftContentPage
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("LBNegVeraenderung", "title");
            BreadcrumbCaption = _mapper.get("LBNegVeraenderung", "title");

            // Setting content layout of page layout
            SearchContentLayout contentLayout = (SearchContentLayout)LoadPSOFTControl(SearchContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            ((SearchContentLayout)PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            LeistungswerteUnterdurchschnCtrl search = (LeistungswerteUnterdurchschnCtrl)this.LoadPSOFTControl(LeistungswerteUnterdurchschnCtrl.Path, "_search");
            search.OnSearchClick += new SearchClickHandler(onSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, search);

            Control custom = Page.FindControl("_ctrl2");

            Control ctrl1;
            foreach (Control ctrl in Page.Controls)
            {
                ctrl1 = FindControlRecursive(ctrl, "_search");
            }
        }

        private static Control FindControlRecursive(Control root, string Id)
        {
            //if (root.ID == Id)
            if (root.ID != null && root.ID.Contains(Id))
            {
                return root;
            }

            foreach (System.Web.UI.Control Ctl in root.Controls)
            {
                System.Web.UI.Control temp = FindControlRecursive(Ctl, Id);

                if (temp != null)
                {
                    return temp;
                }
            }
            return null;
        }




        private void onSearchClick(object Sender, SearchEventArgs e)
        {
            Control ctrl1;
            DateTime LbFrom = new DateTime(1900, 01, 01);
            DateTime LbTo = new DateTime(2099, 12, 31);
            Double dataRange = 0;
            foreach (Control ctrl in Page.Controls)
            {
                ctrl1 = FindControlRecursive(ctrl, "_search");
                if (ctrl1 != null)
                {

                    if (((ctrl1 as LeistungswerteUnterdurchschnCtrl).LBFrom) != "")
                        LbFrom = DateTime.Parse((ctrl1 as LeistungswerteUnterdurchschnCtrl).LBFrom);
                    if (((ctrl1 as LeistungswerteUnterdurchschnCtrl).LBTo) != "")
                        LbTo = DateTime.Parse((ctrl1 as LeistungswerteUnterdurchschnCtrl).LBTo);
                    if (((ctrl1 as LeistungswerteUnterdurchschnCtrl).DataRange) != "")
                        dataRange = Double.Parse((ctrl1 as LeistungswerteUnterdurchschnCtrl).DataRange);
                }
            }

            DBData db = DBData.getDBData(Session);
            db.connect();

            // get persons
            DataTable personTable = db.getDataTable("SELECT PERSONNELNUMBER FROM PERSON WHERE PERSONNELNUMBER IS NOT NULL AND LEAVING IS NULL;");

            string performanceRatingBase = Global.Config.getModuleParam("performance", "performanceRatingBase", "40");

            // use temporary table (needed for calculation of leistungswert) / 04.11.09 / mkr
            // delete temporary table if exists
            string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[leistungswerte_%userid%]') "
                              + "AND type in (N'U')) "
                                + "DROP TABLE [dbo].[leistungswerte_%userid%]";
            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

            // create table
            string tbl_create = "CREATE TABLE [dbo].[leistungswerte_%userid%]("
                                + "[PNr] [varchar](64) NULL, "
                                + "[Name] [varchar](64) NULL, "
                                + "[Vorname] [varchar](64) NULL, "
                                + "[TITLE_DE] [varchar](128) NULL, "
                                + "[Kst] [varchar](64) NULL, "
                                + "[Abteilung] [varchar](128) NULL, "
                                + "[Bewertungsdatum1] [datetime] NULL, "
                                + "[Leistungswert1] [float] NULL, "
                                + "[Bewertungsdatum2] [datetime] NULL, "
                                + "[Leistungswert2] [float] NULL, "
                                + "[Bewertungsdatum3] [datetime] NULL, "
                                + "[Leistungswert3] [float] NULL, "
                                +" [Bewertungsdatum4] [datetime] NULL, "
                                + "[Leistungswert4] [float] NULL, "
                                + "[Bewertungsdatum5] [datetime] NULL, "
                                + "[Leistungswert5] [float] NULL)"
                                + "ON [PRIMARY];";

            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

            // Report "Leistungswerte schlechter als Vorjahr"
            foreach (DataRow personRow in personTable.Rows)
            {
                string pnr = personRow[0].ToString();

                bool bad = false;

                // get value of person between startdate and enddate
                DataTable thisYearTable = db.getDataTable("SELECT * FROM f_LeistungswerteJahr(" + performanceRatingBase + ", '" + LbFrom.ToString("MM.dd.yyyy") + "', '" + LbTo.ToString("MM.dd.yyyy") + "') WHERE pnr = '" + pnr + "';");

                float lbEins = 0;
                float lbZwei = 0;
                float lbDrei = 0;
                float lbVier = 0;
                float lbFünf = 0;
                DateTime DatEins = new DateTime(1900, 01, 01);
                DateTime DatZwei = new DateTime(1900, 01, 01);
                DateTime DatDrei = new DateTime(1900, 01, 01);
                DateTime DatVier = new DateTime(1900, 01, 01);
                DateTime DatFünf = new DateTime(1900, 01, 01);

                DataRow[] foundRows;
                foreach(DataRow row in thisYearTable.Rows)
                {
                    foundRows = thisYearTable.Select("Jahr = " + row["Jahr"].ToString());
                    if (foundRows.Length > 1)
                    {
                        row.Delete();
                    }
                }
                thisYearTable.AcceptChanges();

                if (thisYearTable.Rows.Count >= 3)
                {

                    if (thisYearTable.Rows.Count == 3)
                    {
                        float.TryParse(thisYearTable.Rows[2]["Leistungswert"].ToString(), out lbEins);
                        DatEins = DateTime.Parse(thisYearTable.Rows[2]["Bewertungsdatum"].ToString());
                        float.TryParse(thisYearTable.Rows[1]["Leistungswert"].ToString(), out lbZwei);
                        DatZwei = DateTime.Parse(thisYearTable.Rows[1]["Bewertungsdatum"].ToString());
                        float.TryParse(thisYearTable.Rows[0]["Leistungswert"].ToString(), out lbDrei);
                        DatDrei = DateTime.Parse(thisYearTable.Rows[0]["Bewertungsdatum"].ToString());
                    }
                    if (thisYearTable.Rows.Count == 4)
                    {
                        float.TryParse(thisYearTable.Rows[3]["Leistungswert"].ToString(), out lbEins);
                        DatEins = DateTime.Parse(thisYearTable.Rows[3]["Bewertungsdatum"].ToString());
                        float.TryParse(thisYearTable.Rows[2]["Leistungswert"].ToString(), out lbZwei);
                        DatZwei = DateTime.Parse(thisYearTable.Rows[2]["Bewertungsdatum"].ToString());
                        float.TryParse(thisYearTable.Rows[1]["Leistungswert"].ToString(), out lbDrei);
                        DatDrei = DateTime.Parse(thisYearTable.Rows[1]["Bewertungsdatum"].ToString());
                        float.TryParse(thisYearTable.Rows[0]["Leistungswert"].ToString(), out lbVier);
                        DatVier = DateTime.Parse(thisYearTable.Rows[0]["Bewertungsdatum"].ToString());
                    }
                    if (thisYearTable.Rows.Count == 5)
                    {
                        float.TryParse(thisYearTable.Rows[4]["Leistungswert"].ToString(), out lbEins);
                        DatEins = DateTime.Parse(thisYearTable.Rows[4]["Bewertungsdatum"].ToString());
                        float.TryParse(thisYearTable.Rows[3]["Leistungswert"].ToString(), out lbZwei);
                        DatZwei = DateTime.Parse(thisYearTable.Rows[3]["Bewertungsdatum"].ToString());
                        float.TryParse(thisYearTable.Rows[2]["Leistungswert"].ToString(), out lbDrei);
                        DatDrei = DateTime.Parse(thisYearTable.Rows[2]["Bewertungsdatum"].ToString());
                        float.TryParse(thisYearTable.Rows[1]["Leistungswert"].ToString(), out lbVier);
                        DatFünf = DateTime.Parse(thisYearTable.Rows[1]["Bewertungsdatum"].ToString());
                        float.TryParse(thisYearTable.Rows[0]["Leistungswert"].ToString(), out lbFünf);
                        DatFünf = DateTime.Parse(thisYearTable.Rows[0]["Bewertungsdatum"].ToString());
                    }

                    if(lbEins + dataRange - lbDrei < 0 || (lbZwei + dataRange - lbVier < 0 && lbVier > 0) || (lbDrei + dataRange - lbFünf < 0 && lbFünf >0))
                        bad = true;
                    else
                    {
                        lbEins = 0;
                        lbZwei = 0;
                        lbDrei = 0;
                        lbVier = 0;
                        lbFünf = 0;
                        bad = false;
                    }
                }
                
                if (bad)
                {
                    // performance got worse
                    // insert into temporary table
                    string sql = "INSERT INTO leistungswerte_" + db.userId.ToString() + " VALUES ('" + thisYearTable.Rows[0]["PNr"] + "',"
                                 + " '" + thisYearTable.Rows[0]["Name"] + "', '" + thisYearTable.Rows[0]["Vorname"] + "',"
                                 + " '" + thisYearTable.Rows[0]["TITLE_DE"] + "', '" + thisYearTable.Rows[0]["Kst"] + "',"
                                 + " '" + thisYearTable.Rows[0]["Abteilung"] + "', '" + DatEins.ToString("MM.dd.yyyy") + "',"
                                 + " '" + lbEins.ToString().Replace(",",".") + "', '" + DatZwei.ToString("MM.dd.yyyy") + "', '" + lbZwei.ToString().Replace(",", ".") + "', '"
                                 + DatDrei.ToString("MM.dd.yyyy") + "', '" + lbDrei.ToString().Replace(",", ".") + "', '"
                                 + DatVier.ToString("MM.dd.yyyy") + "', '" + lbVier.ToString().Replace(",", ".") + "', '"
                                 + DatFünf.ToString("MM.dd.yyyy") + "', '" + lbFünf.ToString().Replace(",", ".") + "')";
                    db.execute(sql);
                    string a = Culture.ToString();
                }
            }
            Response.Redirect("CrystalReportViewer.aspx?alias=NegativeLeistungsentwicklung&startdat=" + LbFrom.ToShortDateString() + "&enddat=" + LbTo.ToShortDateString() + "&rangeLimit=" + dataRange.ToString(),true);

            db.disconnect();
        }
    }
}
