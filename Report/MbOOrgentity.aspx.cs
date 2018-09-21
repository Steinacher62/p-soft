using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Report
{
    public partial class MbOOrgentity : System.Web.UI.Page
    {
        DBData db;
        LanguageMapper map;

        protected void Page_Load(object sender, EventArgs e)
        {
            // connect to database
            db = DBData.getDBData(Session);
            db.connect();

            // apply language
            map = LanguageMapper.getLanguageMapper(Session);
            if (map == null)
            {
                map = LanguageMapper.getLanguageMapper(Application);
            }

            lblTitle.Text = map.get("SelectOrgentityMbo", "Title");
            lblTurn.Text = map.get("SelectOrgentityMbo", "Turn");

            if (Request.QueryString["alias"] == "MbOAbteilungsziel.rpt" || Request.QueryString["alias"] == "MbOAbteilungszielDetail.rpt")
            {
                lblOE.Text = map.get("SelectOrgentityMbo", "OE");
                chkSubOEs.Text = map.get("SelectOrgentityMbo", "SubOEs");
            }
            else
            {
                lblTitle.Text = map.get("SelectOrgentityMbo", "Title");
                lblOE.Text = map.get("SelectOrgentityMbo", "CompanyObjective");
                chkSubOEs.Visible = false;
                lstTurn.AutoPostBack = true;
            }





            cmdOk.Text = map.get("SelectOrgentityMbo", "showReport");

            //if (Page.IsPostBack && (Request.QueryString["alias"] == "MbOUnternehmensziel.rpt" || Request.QueryString["alias"] == "MbOUnternehmenszielDetail.rpt"))
            //{
            //    lstOE.Items.Clear();
            //    lstOE.Text = "";
               
            //    DataTable tblCompanyGoal = db.getDataTableExt("SELECT ID, TITLE FROM OBJECTIVE WHERE TYP = 1 AND OBJECTIVE_TURN_ID = " + lstTurn.SelectedValue.ToString() + " ORDER BY TITLE", new object[0]);
            //    foreach (DataRow aktRow in tblCompanyGoal.Rows)
            //    {
            //        lstOE.Items.Add(new ListItem(aktRow["TITLE"].ToString(), aktRow["ID"].ToString()));
            //    }
            //}

            if (!Page.IsPostBack)
            {
                DataTable tblTurn = db.getDataTableExt("SELECT ID," + db.langAttrName("OBJECTIVE_TURN", "TITLE") + " FROM OBJECTIVE_TURN ORDER BY TITLE_DE DESC", new object[0]);
                foreach (DataRow aktRow in tblTurn.Rows)
                {
                    lstTurn.Items.Add(new ListItem(aktRow["TITLE_DE"].ToString(), aktRow["ID"].ToString()));
                }
                
                setObjetiveList();
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            string Ids = "-1";

            //OE or person selected?
            if (lstOE.SelectedIndex > 0 && (Request.QueryString["alias"] == "MbOAbteilungsziel.rpt" || Request.QueryString["alias"] == "MbOAbteilungszielDetail.rpt"))
            {
                //OE

                Ids = lstOE.SelectedValue;
                if (chkSubOEs.Checked)
                {
                    //also list Sub-OEs 
                    Ids = db.Orgentity.addAllSubOEIDs(Ids);
                }
            }

            string turnId = "";
            if (lstTurn.SelectedIndex > -1)
            {
                turnId = lstTurn.SelectedValue.ToString();
            }



            //check rights
            long accessorID = SessionData.getUserAccessorID(Session);
            string accessorSQL = db.getAccessorIDsSQLInClause(accessorID);
            DataTable tblJobs;
            if (Request.QueryString["alias"] == "MbOAbteilungsziel.rpt" || Request.QueryString["alias"] == "MbOAbteilungszielDetail.rpt")
            {
                tblJobs = db.getDataTableExt("SELECT DISTINCT JOB.ID FROM PERSON INNER JOIN OBJECTIVE ON PERSON.ID = OBJECTIVE.PERSON_ID INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB INNER JOIN ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN ORGANISATION ON ORGANISATION.ORGENTITY_ID = ORGENTITY.ROOT_ID AND ORGANISATION.MAINORGANISATION = 1 INNER JOIN ACCESS_RIGHT_RT ON ACCESS_RIGHT_RT.TABLENAME = 'JOB' AND (ACCESS_RIGHT_RT.ROW_ID = JOB.ID OR ACCESS_RIGHT_RT.ROW_ID = 0) AND dbo.ACCESS_RIGHT_RT.APPLICATION_RIGHT = 60 AND dbo.ACCESS_RIGHT_RT.AUTHORISATION & 4 = 4 AND ACCESS_RIGHT_RT.ACCESSOR_ID  " + accessorSQL + "ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID WHERE(OBJECTIVE.OBJECTIVE_TURN_ID = " + turnId.ToString() + " AND ORGENTITY.ID IN (" + Ids + "))", new object[0]);
            }
            else
            {
                tblJobs = db.getDataTableExt("SELECT DISTINCT JOB.ID FROM PERSON INNER JOIN OBJECTIVE ON PERSON.ID = OBJECTIVE.PERSON_ID INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB INNER JOIN ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN ORGANISATION ON ORGANISATION.ORGENTITY_ID = ORGENTITY.ROOT_ID AND ORGANISATION.MAINORGANISATION = 1 INNER JOIN ACCESS_RIGHT_RT ON ACCESS_RIGHT_RT.TABLENAME = 'JOB' AND (ACCESS_RIGHT_RT.ROW_ID = JOB.ID OR ACCESS_RIGHT_RT.ROW_ID = 0) AND dbo.ACCESS_RIGHT_RT.APPLICATION_RIGHT = 60 AND dbo.ACCESS_RIGHT_RT.AUTHORISATION & 4 = 4 AND ACCESS_RIGHT_RT.ACCESSOR_ID  " + accessorSQL + "ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID WHERE(OBJECTIVE.OBJECTIVE_TURN_ID = " + turnId.ToString() + " AND (OBJECTIVE.PARENT_ID = " + lstOE.SelectedValue.ToString() + " OR OBJECTIVE.ID = " + lstOE.SelectedValue.ToString() +"))", new object[0]);
                if (tblJobs.Rows.Count > 0)
                {
                    Ids = "1";
                }
            }

            string jobsSQL = "0";
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
            //jobsSQL += ")";



            if (Ids != "-1")
            {
                //redirect to report
                if (Request.QueryString["alias"] == "MbOAbteilungsziel.rpt" || Request.QueryString["alias"] == "MbOAbteilungszielDetail.rpt")
                {
                    this.Session["param0"] = Ids;
                }
                else
                {
                    this.Session["param0"] = lstOE.SelectedValue.ToString();
                }
                this.Session["param1"] = turnId;
                this.Session["param2"] = jobsSQL;
                this.Session["param3"] = db.lookup("title", "objective", "id= " + lstOE.SelectedValue.ToString());

                if (Request.QueryString["alias"] == "MbOAbteilungszielDetail.rpt")
                {
                    Response.Redirect(Global.Config.baseURL + "/report/CrystalReportViewer.aspx?alias=MbOAbteilungDetail", true);
                }
                else if (Request.QueryString["alias"] == "MbOAbteilungsziel.rpt")
                {
                    Response.Redirect(Global.Config.baseURL + "/report/CrystalReportViewer.aspx?alias=MbOAbteilung", true);
                }
                else if (Request.QueryString["alias"] == "MbOUnternehmensziel.rpt")
                {
                    Response.Redirect(Global.Config.baseURL + "/report/CrystalReportViewer.aspx?alias=MbOUnternehmensziel", true);
                }
                else if (Request.QueryString["alias"] == "MbOUnternehmenszielDetail.rpt")
                {
                    Response.Redirect(Global.Config.baseURL + "/report/CrystalReportViewer.aspx?alias=MbOUnternehmenszielDetail", true);
                }
            }
            else
            {
                //nothing selected or no persons in selected OE
                lblNotFound.Text = map.get("SelectOrgentityMbo", "NotFound");
            }
        }

        protected void lstTurnChanged(object sender, EventArgs e)
        {
            setObjetiveList();
        }

        protected void setObjetiveList()
        {


            if (Request.QueryString["alias"] == "MbOAbteilungsziel.rpt" || Request.QueryString["alias"] == "MbOAbteilungszielDetail.rpt")
            {
                lstOE.Items.Add(new ListItem("", "-1"));

                //list OEs
                DataTable tblOE = db.getDataTableExt("SELECT ID," + db.langAttrName("ORGENTITY", "TITLE") + " FROM ORGENTITY ORDER BY TITLE_DE", new object[0]);
                foreach (DataRow aktRow in tblOE.Rows)
                {
                    lstOE.Items.Add(new ListItem(aktRow["TITLE_DE"].ToString(), aktRow["ID"].ToString()));
                }
            }
            else
            {
                lstOE.Items.Clear();
                // add empty item to objective list
                lstOE.Items.Add(new ListItem("", "-1"));
                DataTable tblCompanyGoal = db.getDataTableExt("SELECT ID, TITLE FROM OBJECTIVE WHERE TYP = 1 AND OBJECTIVE_TURN_ID = " + lstTurn.SelectedValue.ToString() + " ORDER BY TITLE", new object[0]);
                foreach (DataRow aktRow in tblCompanyGoal.Rows)
                {
                    lstOE.Items.Add(new ListItem(aktRow["TITLE"].ToString(), aktRow["ID"].ToString()));
                }
            }




        }
    }
}