using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;
using System.Linq;
using Telerik.Web.UI;


namespace ch.appl.psoft.Report
{
    public partial class SelectOEDateRange : System.Web.UI.Page
    {
        private string jobsSQL = "IN (";

        protected void Page_Load(object sender, EventArgs e)
        {


            DBData db = DBData.getDBData(Session);

            // apply language
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
            if (map == null)
            {
                map = LanguageMapper.getLanguageMapper(Application);
            }


            lblOE.Text = map.get("SelectOEDateRange", "selOE");
            lblDateRange.Text = map.get("SelectOEDateRange", "selDateRange");
            lblFrom.Text = map.get("SelectOEDateRange", "from");
            lblTo.Text = map.get("SelectOEDateRange", "to");
            chkSubOEs.Text = map.get("SelectOEDateRange", "SubOEs");
            cmdOk.Text = map.get("SelectOEDateRange", "showReport");

            if (!Page.IsPostBack)
            {
                DatVon.Clear();
                DatVon.SelectedDate = Convert.ToDateTime("01.01." + DateTime.Now.Year);
                DatBis.Clear();
                DatBis.SelectedDate = Convert.ToDateTime("31.12." + DateTime.Now.Year);
            }

            //check rights
            long accessorID = SessionData.getUserAccessorID(Session);
            string accessorSQL = db.getAccessorIDsSQLInClause(accessorID);
            string UserJobId = db.lookup("ID", "JOB", "ID=(SELECT JOB.ID FROM JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID WHERE (HAUPTFUNKTION = 1 AND PERSON.ID = " + db.userId.ToString() + "))", "0").ToString();
            DataTable tblJobs;
            if (Global.isModuleEnabled("energiedienst"))
            {
                tblJobs = db.getDataTableExt("select distinct JOB.ID from JOB inner join ORGENTITY on JOB.ORGENTITY_ID=ORGENTITY.ID inner join ORGANISATION on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID and ORGANISATION.MAINORGANISATION=1 inner join ACCESS_RIGHT_RT on ACCESS_RIGHT_RT.TABLENAME='JOB' and (ACCESS_RIGHT_RT.ROW_ID=JOB.ID or ACCESS_RIGHT_RT.ROW_ID=0) and ACCESS_RIGHT_RT.APPLICATION_RIGHT=11 and (ACCESS_RIGHT_RT.AUTHORISATION&4)=4 and ACCESS_RIGHT_RT.ACCESSOR_ID  " + accessorSQL, new object[0]);
                DataRow[] isJobIncl = tblJobs.Select("ID=" + UserJobId);
                if (isJobIncl.Count() == 0)
                {
                    DataRow UserJobRow = tblJobs.NewRow();
                    UserJobRow["ID"] = UserJobId;
                    tblJobs.Rows.Add(UserJobRow);
                }
            }
            else
            {
                tblJobs = db.getDataTableExt("select distinct JOB.ID from JOB inner join ORGENTITY on JOB.ORGENTITY_ID=ORGENTITY.ID inner join ORGANISATION on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID and ORGANISATION.MAINORGANISATION=1 inner join ACCESS_RIGHT_RT on ACCESS_RIGHT_RT.TABLENAME='JOB' and (ACCESS_RIGHT_RT.ROW_ID=JOB.ID or ACCESS_RIGHT_RT.ROW_ID=0) and ACCESS_RIGHT_RT.APPLICATION_RIGHT=11 and (ACCESS_RIGHT_RT.AUTHORISATION&4)=4 and ACCESS_RIGHT_RT.ACCESSOR_ID  " + accessorSQL, new object[0]);
            }
            bool start = true;
            if (tblJobs.Rows.Count > 0)
            {
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
                if (!Page.IsPostBack)
                {
                    //list OEs
                    this.lstOE.Items.Add(new RadComboBoxItem(" ", "0"));
                    DataTable tblOE = db.getDataTableExt("SELECT DISTINCT ORGENTITY.ID, ORGENTITY.TITLE_DE FROM ORGENTITY INNER JOIN JOB ON ORGENTITY.ID = JOB.ORGENTITY_ID WHERE JOB.ID " + jobsSQL + " ORDER BY ORGENTITY.TITLE_DE", new object[0]);
                    foreach (DataRow aktRow in tblOE.Rows)
                    {
                        this.lstOE.Items.Add(new RadComboBoxItem(aktRow["TITLE_DE"].ToString(), aktRow["ID"].ToString()));

                    }
                }
                else
                {
                    cmdOk.Enabled = false;
                }
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            //redirect to report
            string oe = lstOE.SelectedValue.ToString();
            string von = DatVon.SelectedDate.ToString();
            string bis = DatBis.SelectedDate.ToString();

            if (chkSubOEs.Checked && oe != "")
            {
                DBData db = DBData.getDBData(Session);

                oe = db.Orgentity.addAllSubOEIDs(oe);
            }

            if (oe != "")
            {
                string oe_enc = Global.EncodeTo64(oe);
                string von_enc = Global.EncodeTo64(von);
                string bis_enc = Global.EncodeTo64(bis);
                string jobSql_enc = Global.EncodeTo64(jobsSQL);
                Session["oe_enc"] = oe_enc;
                Session["jobSql_en"] = jobSql_enc;

                if (Request.QueryString.Get("context") == "subnavReportAverageOEPerformance")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=DurchschnittlicheLeistungsanteile&param0=" + oe_enc + "&param1=" + von_enc + "&param2=" + bis_enc + "&performanceRatingBase=" + Global.Config.getModuleParam("performance", "performanceRatingBase", "100"), true);
                }
                if (Request.QueryString.Get("report") == "LeistungswerteMa")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=Leistungswerte&param0=" + oe_enc + "&param1=" + von_enc + "&param2=" + bis_enc, true);
                }
                if (Request.QueryString.Get("alias") == "EnergiedienstKompetenzbewertungStatus")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=KompetenzbewertungStatusEnergiedienst&param1=" + von_enc + "&param2=" + bis_enc, true);
                }
                if (Request.QueryString.Get("alias") == "EnergiedienstRatingKompetenzbeurteilung")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=EnergiedienstRatingKompetenzbeurteilung&param1=" + von_enc + "&param2=" + bis_enc, true);
                }
                if (Request.QueryString.Get("alias") == "EnergiedienstStatusAufgabenbeschreibung")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=EnergiedienstStatusAufgabenbeschreibung&param1=" + von_enc + "&param2=" + bis_enc, true);
                }
                if (Request.QueryString.Get("alias") == "EnergiedienstStatusPersonalentwicklungsbedarf")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=EnergiedienstStatusPersonalentwicklungsbedarf&param1=" + von_enc + "&param2=" + bis_enc, true);
                }
                if (Request.QueryString.Get("alias") == "Journalliste")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=JournalList&param1=" + von_enc + "&param2=" + bis_enc, true);
                }
                if (Request.QueryString.Get("alias") == "LeistungswerteFoamPartner")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=LeistungswerteFoamPartner&param0=" + oe_enc + "&param1=" + von_enc + "&param2=" + bis_enc, true);
                }
                if (Request.QueryString.Get("alias") == "PotentialFoamPartner")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=PotentialEinschätzungFoamPartner&param0=" + oe_enc + "&param1=" + von_enc + "&param2=" + bis_enc, true);
                }
                if (Request.QueryString.Get("alias") == "JournalListStatus")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=JournalListStatus&param0=" + oe_enc + "&param1=" + von_enc + "&param2=" + bis_enc, true);
                }
            }
        }


    }
}
