using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Report
{
    public partial class SelectOE : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //fill year field
                int anfang = DateTime.Now.Year - 9;

                for (int idx = 0; idx < 10; idx++)
                {
                    lstJahr.Items.Add((anfang + idx).ToString());
                }

                //add "alle" field
                lstJahr.Items.Add("alle");

                lstJahr.SelectedIndex = lstJahr.Items.Count - 3;

                //show SubOE-Checkbox if report is "AusbildungsmassnahmenOE"
                if (Request.QueryString["report"] == "AusbildungsmassnahmenOE")
                {
                    chkSubOEs.Visible = true;
                }
            }
            
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

            //list OEs
            DataTable tblOE = db.getDataTableExt("SELECT DISTINCT ORGENTITY.ID, ORGENTITY.TITLE_DE FROM ORGENTITY INNER JOIN JOB ON ORGENTITY.ID = JOB.ORGENTITY_ID WHERE JOB.ID " + jobsSQL + " ORDER BY ORGENTITY.TITLE_DE", new object[0]);
            foreach (DataRow aktRow in tblOE.Rows)
            {
                lstOE.Items.Add(new ListItem(aktRow["TITLE_DE"].ToString(), aktRow["ID"].ToString()));
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            //redirect to report
            string oe = lstOE.SelectedItem.Value;
            string year = lstJahr.SelectedItem.Text;

            if (Request.QueryString["report"] == "AusbildungsmassnahmenOE" && chkSubOEs.Checked && oe != "")
            {
                DBData db = DBData.getDBData(Session);

                oe = db.Orgentity.addAllSubOEIDs(oe);
            }
            
            if (oe != "")
            {
                string oe_enc = Global.EncodeTo64(oe);
                string year_enc = Global.EncodeTo64(year);

                if (Request.QueryString["report"] == "" || Request.QueryString["report"] == null)
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=LeistungswerteOE&param0=" + oe_enc + "&param1=" + year_enc,true);
                }
                else if (Request.QueryString["report"] == "AusbildungsmassnahmenOE" && lstJahr.SelectedItem.Text == "alle")
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=AusbildungsmassnahmenOEalle&param0=" + oe_enc,true);
                }
                else
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=" + Request.QueryString["report"] + "&param0=" + oe_enc + "&param1=" + year_enc);
                }
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
