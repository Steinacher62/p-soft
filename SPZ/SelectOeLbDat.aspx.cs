using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.SPZ
{
    public partial class SelectOeLbDat : PsoftDetailPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");

            // Setting breadcrumb caption
            BreadcrumbCaption = "Datenselektion Report";

            // Setting page-title
            pageTitle.Text = "Datenselektion Report";

            if (!Page.IsPostBack)
            {
                txtVon.Clear();
                txtVon.SelectedDate = Convert.ToDateTime("01.01." + DateTime.Now.Year);
                txtBis.Clear();
                txtBis.SelectedDate = Convert.ToDateTime("31.12." + DateTime.Now.Year);
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
            //redirect to report
            string oe = lstOE.SelectedItem.Value;
            string von = txtVon.SelectedDate.ToString();
            string bis = txtBis.SelectedDate.ToString();
            DBData db = DBData.getDBData(Session); ;
            if (chkSubOEs.Checked && oe != "")
            {
                oe = db.Orgentity.addAllSubOEIDs(oe);
            }
            
            if (oe != "")
            {
                string oe_enc = Global.EncodeTo64(oe);
                string von_enc = Global.EncodeTo64(von);
                string bis_enc = Global.EncodeTo64(bis);
                Session.Add("Oes", oe_enc);
                Response.Redirect("../report/CrystalReportViewer.aspx?alias=StatusLeistungsbewertungSPZ.rpt&param1=" + von_enc + "&param2=" + bis_enc, true);
            }
            }
        }
    }
