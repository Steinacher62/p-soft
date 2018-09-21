using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Report
{
    public partial class SelectNameJob : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);

            //check rights
            long accessorID = SessionData.getUserAccessorID(Session);
            string accessorSQL = db.getAccessorIDsSQLInClause(accessorID);
            string userJobId = db.lookup("ID", "JOB", "ID = (SELECT JOB.ID FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID WHERE HAUPTFUNKTION = 1 AND PERSON.ID = " + db.userId.ToString() + ")","0").ToString();
            DataTable tblJobs = db.getDataTableExt("select distinct JOB.ID from JOB inner join ORGENTITY on JOB.ORGENTITY_ID=ORGENTITY.ID inner join ORGANISATION on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID and ORGANISATION.MAINORGANISATION=1 inner join ACCESS_RIGHT_RT on ACCESS_RIGHT_RT.TABLENAME='JOB' and (ACCESS_RIGHT_RT.ROW_ID=JOB.ID or ACCESS_RIGHT_RT.ROW_ID=0) and ACCESS_RIGHT_RT.APPLICATION_RIGHT=11 and (ACCESS_RIGHT_RT.AUTHORISATION&4)=4 and ACCESS_RIGHT_RT.ACCESSOR_ID  " + accessorSQL, new object[0]);
            if (tblJobs.Select("ID= " + userJobId).Count() == 0 )
            {
                string[] userjob = new string[] { userJobId};
                tblJobs.Rows.Add(userjob);
            }
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

            //list employees
            //caution: view "rep_stellenerwartungen" is used also used for "ausbildungsmassnahmen" / 20.04.10 / mkr
            DataTable tblMA = db.getDataTableExt("SELECT DISTINCT PersonenID,[Name],Vorname,[Bezeichnung Job] FROM Rep_Stellenerwartungen WHERE JobID " + jobsSQL + " ORDER BY [Name]", new object[0]);
            foreach (DataRow aktRow in tblMA.Rows)
            {
                ListItem aktItem = new ListItem(aktRow["Name"] + " " + aktRow["Vorname"] + " / " + aktRow["Bezeichnung Job"], aktRow["PersonenID"].ToString());
                lstMA.Items.Add(aktItem);
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            //redirect to report
            string ma = lstMA.SelectedItem.Value;

            //which report?
            string report = Request.QueryString["report"];
            if (Global.isModuleEnabled("frauenfeld") && report.Equals("leistungswertema"))
            {
                report = "LeistungswerteMaFrauenfeld";
            }

            if (ma != "")
            {
                string job_enc = Global.EncodeTo64(ma);
                    Response.Redirect("CrystalReportViewer.aspx?alias=" + report + "&param0=" + job_enc,true);
            }
        }

        //private string EncodeTo64(string toEncode)
        //{
        //    byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.Unicode.GetBytes(toEncode);

        //    string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

        //    return returnValue;
        //}
    }
}
