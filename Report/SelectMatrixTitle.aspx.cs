using ch.appl.psoft.db;
using System;
using System.Data;

namespace ch.appl.psoft.Report
{
    public partial class SelectMatrixTitle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);

            //list matrixes
            DataTable tblMatrix = db.getDataTable("SELECT DISTINCT TITLE FROM MATRIX " + db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " ORDER BY TITLE ASC");                       //    db.getDataTableExt("SELECT TITLE FROM MATRIX ORDER BY TITLE", new object[0]);
            foreach (DataRow aktRow in tblMatrix.Rows)
            {
                lstMatrix.Items.Add(aktRow["TITLE"].ToString());
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            //redirect to report
            string matrix = lstMatrix.SelectedItem.Text.Replace("'", "''");

            if (matrix != "")
            {
                string matrix_enc = Global.EncodeTo64(matrix);
                DBData db = DBData.getDBData(Session);
                string id = db.lookup("id", "matrix", "Title ='" + matrix + "'", "");
                Response.Redirect("CrystalReportViewer.aspx?alias=SokratesList&param0=" + id, true);
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
