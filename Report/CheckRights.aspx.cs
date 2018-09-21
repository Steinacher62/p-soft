using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;

namespace ch.appl.psoft.Report
{
    public partial class CheckRights : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);

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

            //which report?
            string report = Request.QueryString["report"];

            //string job_enc = EncodeTo64(ma);

            Response.Redirect("CrystalReportViewer.aspx?alias=" + report + "&param0=" + jobsSQL,true);
        }
    }
}
