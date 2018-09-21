using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Web.UI;

namespace ch.appl.psoft.Report.Controls
{
    public partial class SelectKnowledgeCtrl : PSOFTSearchUserControl
    {

        public static string Path
        {
            get { return Global.Config.baseURL + "/Report/Controls/SelectKnowledgeCtrl.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DBData db = DBData.getDBData(Session);

                //list knowledge
                DataTable tblMatrix = db.getDataTableExt("Select id, (SELECT TITLE FROM THEME WHERE  ROOT_ID = KNOWLEDGE.BASE_THEME_ID_DE AND PARENT_ID IS NULL AND HISTORY_ROOT_ID IS NULL) AS TITLE from KNOWLEDGE WHERE HISTORY_ROOT_ID IS NULL ORDER BY TITLE", new object[0]);
                lstKnowledge.Items.Clear();
                foreach (DataRow row in tblMatrix.Rows)
                {
                    if (!db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "KNOWLEDGE", Convert.ToInt32(row["ID"]), true, false))
                    {
                        row.Delete();
                    }
                }
                tblMatrix.AcceptChanges();
                foreach (DataRow aktRow in tblMatrix.Rows)
                {
                    lstKnowledge.Items.Add(aktRow["TITLE"].ToString());
                    lstKnowledge.Items[lstKnowledge.Items.Count - 1].Value = aktRow["ID"].ToString();
                }

                lbSelectKnowledge.Text = _mapper.get("SelectKnowledge", "selectKnowledgeElement");
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            Response.Redirect("CrystalReportViewer.aspx?alias=Knowledge.rpt&param0=" + lstKnowledge.SelectedValue, true);
        }
    }
}