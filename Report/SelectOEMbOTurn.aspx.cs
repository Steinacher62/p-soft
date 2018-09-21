using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Report
{
    public partial class SelectOEMbOTurn : System.Web.UI.Page
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

            chkSubOEs.Text = map.get("SelectOEDateRange", "SubOEs");
            cmdOk.Text = map.get("SelectOEDateRange", "showReport");
            lblTurn.Text = map.get("SelectOrgentityMbo", "Turn");

            //check rights
            long accessorID = SessionData.getUserAccessorID(Session);
            string accessorSQL = db.getAccessorIDsSQLInClause(accessorID);
            string UserJobId = db.lookup("ID", "JOB", "ID=(SELECT JOB.ID FROM JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID WHERE (HAUPTFUNKTION = 1 AND PERSON.ID = " + db.userId.ToString() + "))","0").ToString();
            DataTable tblJobs = db.getDataTableExt("select distinct JOB.ID from JOB inner join ORGENTITY on JOB.ORGENTITY_ID=ORGENTITY.ID inner join ORGANISATION on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID and ORGANISATION.MAINORGANISATION=1 inner join ACCESS_RIGHT_RT on ACCESS_RIGHT_RT.TABLENAME='JOB' and (ACCESS_RIGHT_RT.ROW_ID=JOB.ID or ACCESS_RIGHT_RT.ROW_ID=0) and ACCESS_RIGHT_RT.APPLICATION_RIGHT=11 and (ACCESS_RIGHT_RT.AUTHORISATION&4)=4 and ACCESS_RIGHT_RT.ACCESSOR_ID  " + accessorSQL, new object[0]);
            if (Global.isModuleEnabled("energiedienst"))
            {
               
                DataRow[] isJobIncl = tblJobs.Select("ID=" + UserJobId);
                if (isJobIncl.Count() == 0)
                {
                    DataRow UserJobRow = tblJobs.NewRow();
                    UserJobRow["ID"] = UserJobId;
                    tblJobs.Rows.Add(UserJobRow);
                }
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

            //list OEs
            DataTable tblOE = db.getDataTableExt("SELECT DISTINCT ORGENTITY.ID, ORGENTITY.TITLE_DE FROM ORGENTITY INNER JOIN JOB ON ORGENTITY.ID = JOB.ORGENTITY_ID WHERE JOB.ID " + jobsSQL + " ORDER BY ORGENTITY.TITLE_DE", new object[0]);
            foreach (DataRow aktRow in tblOE.Rows)
            {
                lstOE.Items.Add(new ListItem(aktRow["TITLE_DE"].ToString(), aktRow["ID"].ToString()));
            }
            
            //list Objective Turns
            DataTable tblTurn = db.getDataTableExt("SELECT ID," + db.langAttrName("OBJECTIVE_TURN", "TITLE") + " FROM OBJECTIVE_TURN ORDER BY TITLE_DE DESC", new object[0]);
            foreach (DataRow aktRow in tblTurn.Rows)
            {
                lstTurn.Items.Add(new ListItem(aktRow["TITLE_DE"].ToString(), aktRow["ID"].ToString()));
            }
            }
            else
            {
                cmdOk.Enabled = false;
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            //redirect to report
            string oe = lstOE.SelectedItem.Value;
            string turn = lstTurn.SelectedItem.Value;



            if (chkSubOEs.Checked && oe != "")
            {
                DBData db = DBData.getDBData(Session);

                oe = db.Orgentity.addAllSubOEIDs(oe);
            }
            
            if (oe != "")
            {
                string oe_enc = Global.EncodeTo64(oe);
                string jobSql_enc = Global.EncodeTo64(jobsSQL);
                Session["oe_enc"] = oe_enc;
                Session["jobSql_en"] = jobSql_enc;

                if (Request.QueryString.Get("alias") == "EnergiedienstStatusMbO")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=EnergiedienstStatusMbO&param1=" + Global.EncodeTo64(turn));
                }
                if (Request.QueryString.Get("alias") == "EnergiedienstRatingMbO")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=EnergiedienstRatingMbO&param1=" + Global.EncodeTo64(turn));
                }
                if (Request.QueryString.Get("alias") == "ZielerreichungFoamPartner")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=ZielerreichungFoamPartner&param0="+ oe_enc + "&param1=" + Global.EncodeTo64(turn));
                }


            }
        }

    }
}
