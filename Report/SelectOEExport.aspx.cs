using ch.appl.psoft.db;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Report
{
    public partial class wordexport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            
            //list OEs
            DataTable tblOE = db.getDataTableExt("SELECT DISTINCT ORGENTITY.ID, ORGENTITY.TITLE_DE FROM ORGENTITY INNER JOIN JOB ON ORGENTITY.ID = JOB.ORGENTITY_ID ORDER BY ORGENTITY.TITLE_DE ASC", new object[0]);
            foreach (DataRow aktRow in tblOE.Rows)
            {
                lstOE.Items.Add(new ListItem(aktRow["TITLE_DE"].ToString(), aktRow["ID"].ToString()));
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            //forward to export.aspx
            string r = "0";
            if (chkSubOEs.Checked)
            {
                r = "1";
            }

            Response.Redirect(Global.Config.baseURL + "/Report/export.aspx?report=StellenbeschreibungOE&oe=" + lstOE.SelectedValue + "&r=" + r);
        }
    }
}
