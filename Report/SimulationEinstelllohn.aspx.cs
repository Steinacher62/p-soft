using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Report
{
    public partial class SimulationEinstelllohn : System.Web.UI.Page
    {
        DBData db;

        private Dictionary<string, float> selectCache = new Dictionary<string, float>();

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
            titleLabel.Text = map.get("Lohnsimulation", "titleEinstelllohn");
            chkSubOEs.Text = map.get("Lohnsimulation", "subOEs");
            jobDescriptionLabel.Text = map.get("Lohnsimulation", "jobDescription");
            cmdOk.Text = map.get("Lohnsimulation", "showReport");
            nameLabel.Text = map.get("Lohnsimulation", "nameLabel");
            vornameLabel.Text = map.get("Lohnsimulation", "vornameLabel");
            lohnvorstellungLabel.Text = map.get("Lohnsimulation", "lohnvorstellungLabel");
            leistungsanteilLabel.Text = map.get("Lohnsimulation", "leistungsanteilLabel");
            alterLabel.Text = map.get("Lohnsimulation", "alterLabel");


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

                //list Jobs
                DataTable tblJobs = db.getDataTableExt("SELECT DISTINCT TITLE_DE FROM JOB ORDER BY TITLE_DE", new object[0]);
                foreach (DataRow aktRow in tblJobs.Rows)
                {
                    if (aktRow["TITLE_DE"].ToString() != "")
                    {
                        lstJobs.Items.Add(aktRow["TITLE_DE"].ToString());
                    }
                }
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            // delete error message
            lblFehler.Text = "";

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
                return;
            }


            // delete temporary table if exists
            string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[einstelllohn_%userid%]') "
                              + "AND type in (N'U')) "
                                + "DROP TABLE [dbo].[einstelllohn_%userid%]";
            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

            // create table
            string tbl_create = "CREATE TABLE [dbo].[einstelllohn_%userid%]("
                                + "[Stellenbezeichnung] [varchar](256) NULL,"
                                + "[Funktionsbewertung] [float] NULL,"
                                + "[Basislohn] [float] NULL,"
                                + "[AnzahlStellen] [int] NULL,"
                                + "[MinLohn] [float] NULL,"
                                + "[MaxLohn] [float] NULL,"
                                + "[DurchschnIstLohn] [float] NULL,"
                                + "[DurchschnAbwSollIst] [float] NULL,"
                                + "[Solllohnkorrektur] [float] NULL"
                                + ") ON [PRIMARY]";
            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

            // fill table
            string performanceRatingBase = Global.Config.getModuleParam("performance", "performanceRatingBase", "100");
            string anzMonatsloehne = Global.Config.getModuleParam("report", "anzMonatsloehne", "13");
            string Stellenbezeichnung = lstJobs.SelectedItem.Text;
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
            db.execute("INSERT INTO einstelllohn_" + db.userId.ToString() + " SELECT * FROM f_SimulationEinstelllohn(" + performanceRatingBase + ", " + anzMonatsloehne + ", " + maxErfahrung + ", '" + Stellenbezeichnung + "', " + glaetten + ", " + fbwMin + ", " + fbwMax + ", " + lohnMin + ", " + lohnMax + " )");

            // get value from agetable
            string erfahrung = db.lookup("Wert", "Lebensalter", "Schluessel = " + txtAlter.Text, "0");

            double fixPointValue = Convert.ToDouble(db.lookup("FIX_POINT_VALUE", "VARIANTE", "HAUPTVARIANTE = 1"));
            if(fixPointValue > 0)
            {
                db.execute("UPDATE einstelllohn_" + db.userId.ToString() + " SET Basislohn = " + fixPointValue * Convert.ToDouble(db.lookup("Funktionsbewertung", "einstelllohn_" + db.userId.ToString(), "")));
            }

            db.disconnect();
            Response.Redirect("CrystalReportViewer.aspx?alias=SimulationEinstelllohn&name=" + txtName.Text + "&vorname=" + txtVorname.Text + "&lohnvorstellung=" + txtLohnvorstellung.Text + "&leistungsanteil=" + txtLeistungsanteil.Text + "&alter=" + txtAlter.Text + "&erfahrung=" + erfahrung + "&maxleistungsanteil=" + performanceRatingBase,true);
        }
    }
}
