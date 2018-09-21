using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.FoamPartner
{
    public partial class SelectOEParam  : PsoftDetailPage
    {
        DBData db;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");

            // Setting breadcrumb caption
            BreadcrumbCaption = "Datenselektion Bonusliste";

            // Setting page-title
            pageTitle.Text = "Datenselektion Bonusliste";

            if (!Page.IsPostBack)
            {
                //fill data fields
                Von.DateInput.SelectedDate = Convert.ToDateTime("01.01." + DateTime.Now.Year);
                Bis.DateInput.SelectedDate = Convert.ToDateTime("31.12." + DateTime.Now.Year);
            }

            DBData db = DBData.getDBData(Session);

            // apply language
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
            if (map == null)
            {
                map = LanguageMapper.getLanguageMapper(Application);
            }


            lblOE.Text = map.get("SelectOEDateRange", "selOE");
            lblDateRange.Text = map.get("SelectOEDateRange", "selDateRangeBonus");
            lblFrom.Text = map.get("SelectOEDateRange", "from");
            lblTo.Text = map.get("SelectOEDateRange", "to");
            chkSubOEs.Text = map.get("SelectOEDateRange", "SubOEs");
            cmdOk.Text = map.get("SelectOEDateRange", "showReport");

            //check rights
            long accessorID = SessionData.getUserAccessorID(Session);
            string accessorSQL = db.getAccessorIDsSQLInClause(accessorID);
            DataTable tblJobs = db.getDataTableExt("select distinct JOB.ID from JOB inner join ORGENTITY on JOB.ORGENTITY_ID=ORGENTITY.ID inner join ORGANISATION on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID and ORGANISATION.MAINORGANISATION=1 inner join ACCESS_RIGHT_RT on ACCESS_RIGHT_RT.TABLENAME='JOB' and (ACCESS_RIGHT_RT.ROW_ID=JOB.ID or ACCESS_RIGHT_RT.ROW_ID=0) and ACCESS_RIGHT_RT.APPLICATION_RIGHT=11 and (ACCESS_RIGHT_RT.AUTHORISATION&4)=4 and ACCESS_RIGHT_RT.ACCESSOR_ID  " + accessorSQL, new object[0]);
            string jobsSQL = "IN (";
            bool start = true;

            foreach (DataRow aktJob in tblJobs.Rows)
            {
                if (start == true)
                {
                    start = false;
                }
                else
                {
                    jobsSQL += ", ";
                }
                jobsSQL += aktJob["ID"];
            }
            jobsSQL += ")";

            //list OEs
            DataTable tblOE = db.getDataTableExt("SELECT DISTINCT ORGENTITY.ID, ORGENTITY.TITLE_DE FROM ORGENTITY INNER JOIN JOB ON ORGENTITY.ID = JOB.ORGENTITY_ID WHERE JOB.ID " + jobsSQL + " ORDER BY ORGENTITY.TITLE_DE", new object[0]);
            foreach (DataRow aktRow in tblOE.Rows)
            {
                lstOE.Items.Add(new ListItem(aktRow["TITLE_DE"].ToString(), aktRow["ID"].ToString()));
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            //string personIds = "";

            //OE or person selected?
            if (lstOE.SelectedIndex > 0)
            {
                db = DBData.getDBData(Session);
                db.connect();
                //OE
                string oeIds = lstOE.SelectedValue;
                if (chkSubOEs.Checked)
                {
                    //also list Sub-OEs 
                    oeIds = db.Orgentity.addAllSubOEIDs(oeIds);
                }

                //get persons in OEs
                double ratingBase = double.Parse(Global.Config.getModuleParam("performance", "performanceRatingBase", "100"));
                double numbWages = double.Parse(Global.Config.getModuleParam("report", "anzMonatsloehne", "13"));

                DataTable persons = db.getDataTable("SELECT PERSON.ID AS PersonId, PERSON.PERSONNELNUMBER, PERSON.PNAME, PERSON.FIRSTNAME, "
                                                          +"JOB.TITLE_DE AS Jobtitle, ORGENTITY.ID AS IdOrgentitiy, ORGENTITY.TITLE_DE AS Orgentity, JOB.ENGAGEMENT, LOHN.ISTLOHN / "+ numbWages +" AS Wages, "
                                                          + "MAX(SolllohnLetzteLeistungsbewertung.Bewertungsdatum) AS Ratingdate, SolllohnLeistungswert_1.Leistungswert / 10000 * " + ratingBase + " AS Rating, "
                                                          +"LOHN.Bonusanteil AS Bonuslevel "
                                                    +"FROM PERSON INNER JOIN "
                                                          +"EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                          +"JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                          +"ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN "
                                                          +"LOHN ON EMPLOYMENT.ID = LOHN.EMPLOYMENT_ID INNER JOIN "
                                                          +"SolllohnLetzteLeistungsbewertung ON PERSON.ID = SolllohnLetzteLeistungsbewertung.PERSON_ID INNER JOIN "
                                                          +"SolllohnLeistungswert AS SolllohnLeistungswert_1 ON SolllohnLetzteLeistungsbewertung.PERSON_ID = SolllohnLeistungswert_1.PERSON_ID AND "
                                                          +"SolllohnLetzteLeistungsbewertung.Bewertungsdatum = SolllohnLeistungswert_1.Bewertungsdatum "
                                                    +"GROUP BY PERSON.ID, PERSON.PERSONNELNUMBER, PERSON.PNAME, PERSON.FIRSTNAME, JOB.TITLE_DE, ORGENTITY.ID, "
                                                          +"ORGENTITY.TITLE_DE, JOB.ENGAGEMENT, LOHN.ISTLOHN, SolllohnLetzteLeistungsbewertung.Bewertungsdatum, "
                                                          +"SolllohnLeistungswert_1.Leistungswert, LOHN.Bonusanteil "
                                                    +"HAVING (SolllohnLetzteLeistungsbewertung.Bewertungsdatum < '" + Von.SelectedDate.Value.ToString("MM-dd-yyyy") + "' AND "
                                                          + "(MAX(SolllohnLetzteLeistungsbewertung.Bewertungsdatum) > '" + Bis.SelectedDate.Value.ToString("MM-dd-yyyy") + "'))");

                persons.Columns.Add("RatingAvg", typeof(double));
                persons.Columns.Add("Bonus%", typeof(double));

                persons.Columns["RatingAvg"].Expression = "Avg(Rating)";
                persons.Columns["Bonus%"].Expression = "(Rating / RatingAvg) *100";

                // delete temporary table if exists

                string tbl_del = " IF NOT OBJECT_ID('tempdb..[##BonusNauer_%userid%]') IS NULL "
                  + "DROP TABLE [##BonusNauer_%userid%]";

                db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                // create table
                string tbl_create = "CREATE TABLE [##BonusNauer_%userid%]("
                                    + "[PersonId] [bigint] NULL,"
                                    + "[PERSONNELNUMBER] [varchar](64) NULL,"
                                    + "[PNAME] [varchar](64) NULL,"
                                    + "[FIRSTNAME] [varchar](64) NULL,"
                                    + "[Jobtitle] [varchar](128) NULL,"
                                    + "[IdOrgentitiy] [int] NULL,"
                                    + "[Orgentity] [varchar](64) NULL,"
                                    + "[ENGAGEMENT] [int] NULL,"
                                    + "[Wages] [float] NULL,"
                                    + "[Ratingdate] [datetime] NULL," 
                                    + "[Rating] [float] NULL,"
                                    + "[Bonuslevel] [float] NULL,"
                                    + "[RatingAvg] [float] NULL,"
                                    + "[Bonus%] [float] NULL"
                                    + ") ON [PRIMARY]";
                db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                SqlConnection sqlcon = (SqlConnection)db.connection;
                SqlBulkCopy BonusUpdate = new SqlBulkCopy(sqlcon.ConnectionString + "; Password=" + Global.Config.dbPassword);
                BonusUpdate.DestinationTableName = "##BonusNauer_" + db.userId.ToString();
                BonusUpdate.WriteToServer(persons);
                string AveragePerformance = db.lookup("min(RatingAvg)", "##BonusNauer_" + db.userId.ToString(), "").ToString();
                Response.Redirect(Global.Config.baseURL.ToString() + "/Report//CrystalReportViewer.aspx?alias=BonusNauer.rpt&korrfakt=" + korrekturfaktor.Value.ToString() + "&datVon=" + Von.SelectedDate.Value.ToString("MM-dd-yyyy") + "&datBis=" + Bis.SelectedDate.Value.ToString("MM-dd-yyyy") + "&AvgPerformance="+AveragePerformance);
            }

        }
    }
}
