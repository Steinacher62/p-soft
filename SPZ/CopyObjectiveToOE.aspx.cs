using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.SPZ
{
    public partial class CopyObjectiveToOE : System.Web.UI.Page
    {
        protected DBData db;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            db = DBData.getDBData(Session);
            if (!Page.IsPostBack)
            {
                // apply language
                LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
                if (map == null)
                {
                    map = LanguageMapper.getLanguageMapper(Application);
                }
                lblSelectOE.Text = map.get("mbo", "selectOE");
                lblObjectiveNr.Text = map.get("mbo", "number");

                cmdOk.Text = map.get("mbo", "copyObjective");

                db = DBData.getDBData(Session);

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


                //objective number

                tbxNumber.Text = db.lookup("NUMBER", "OBJECTIVE", "ID = " + Request.QueryString["Id"]).ToString();

                //list objective typ
                //LanguageMapper _mapper = LanguageMapper.getLanguageMapper(this.Session);
                //ArrayList objectiveTyp = new ArrayList(_mapper.getEnum("mbo", "typ4", false));
                //int i = 0;
                //foreach (Object obj in objectiveTyp)
                //{
                //    lstTyp.Items.Add(new ListItem(objectiveTyp[i].ToString()));
                //    i++;
                //}
                //int oldTyp = (int)db.lookup("TYP", "OBJECTIVE", "ID = " + Request.QueryString["Id"]);
                //if (oldTyp == 1)
                //{
                //    lstTyp.SelectedIndex = 0;
                //}
                //else
                //{
                //    lstTyp.SelectedIndex = oldTyp - 2;
                //}
            }  
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            //list employees in OE, without leader
            DataTable employees = db.getDataTable("SELECT PERSON_ID FROM JOB_PERS_FUNC_V WHERE JOB_TYP = 0 AND ORGENTITY_ID = " + lstOE.SelectedValue);

            string objectiveId = "-1";

            if (Request.QueryString["Id"] != null && Request.QueryString["Id"] != "")
            {
                objectiveId = Request.QueryString["Id"];
            }

            //copy objective
            db = DBData.getDBData(Session);
            db.connect();
            string turnId = db.lookup("WERT", "PROPERTY", "TITLE = 'turn'").ToString();
            string title = db.lookup("TITLE", "OBJECTIVE", "ID = " + objectiveId).ToString();
            string templateSQL = "INSERT INTO OBJECTIVE (PARENT_ID, NUMBER,TITLE, PERSON_ID, DESCRIPTION, OBJECTIVE_TURN_ID, MEASUREMENT_TYPE_ID, CURRENTVALUE, VALUEIMPLICIT, TARGETVALUE, STARTDATE, DATEOFREACHING, CURRENTDATE, ARGUMENT, BSC, STATE, STATEDATE, TYP, FLAG, ACTIONNEED, MEASUREKRIT) SELECT "
                               + "PARENT_ID, '%number%' AS NUMBER, TITLE, '%personID%' AS PERSON_ID, DESCRIPTION, '" + turnId + "' AS OBJECTIVE_TURN_ID, MEASUREMENT_TYPE_ID, CURRENTVALUE, VALUEIMPLICIT, TARGETVALUE, STARTDATE, DATEOFREACHING, CURRENTDATE, ARGUMENT, BSC, STATE, STATEDATE, '%type%' AS TYP, FLAG, ACTIONNEED, MEASUREKRIT "
                               + "FROM OBJECTIVE where ID=" + objectiveId;

            foreach (DataRow employee in employees.Rows)
            {
                string personId = employee["PERSON_ID"].ToString();
                
                //copy only if objective does not already exist in same turn for employee / 23.11.10 / mkr
                int exists = db.lookup("COUNT(ID)", "OBJECTIVE", "PERSON_ID = " + personId + " AND TITLE = '" + title + "' AND OBJECTIVE_TURN_ID = " + turnId, 0);

                if (exists == 0)
                {
                    templateSQL = templateSQL.Replace("%number%", tbxNumber.Text);
                    templateSQL = templateSQL.Replace("%type%", ("5"));
                    db.execute(templateSQL.Replace("%personID%", personId));
                }
            }
            db.disconnect();

            // show message
            lblDone.Text = "Ziele wurden erfolgreich kopiert";
        }
    }
}
