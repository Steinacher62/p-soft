using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Report
{
    public partial class SelectNameOE : System.Web.UI.Page
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
            lblTitle.Text = map.get("SelectNameOE", "Title");
            lblOR.Text = map.get("SelectNameOE", "OR");
            lblOE.Text = map.get("SelectNameOE", "OE");
            chkSubOEs.Text = map.get("SelectNameOE", "SubOEs");
            lblPerson.Text = map.get("SelectNameOE", "Person");
            cmdOk.Text = map.get("SelectNameOE", "showReport");

            if (!Page.IsPostBack)
            {
                //add empty item to OE list
                lstOE.Items.Add(new ListItem("", "-1"));

                //list OEs
                DataTable tblOE = db.getDataTableExt("SELECT ID," + db.langAttrName("ORGENTITY", "TITLE") + " FROM ORGENTITY ORDER BY TITLE_DE", new object[0]);
                foreach (DataRow aktRow in tblOE.Rows)
                {
                    lstOE.Items.Add(new ListItem(aktRow["TITLE_DE"].ToString(), aktRow["ID"].ToString()));
                }

                //add empty item to person list
                lstPerson.Items.Add(new ListItem("", "-1"));

                //list persons
                DataTable tblPersons = null;
                if(Request.QueryString["alias"] != "EigeneinschaetzungSPVKader.rpt")
                {
                    tblPersons = db.getDataTableExt("SELECT ID,PNAME + ' ' + FIRSTNAME AS NAME FROM PERSON WHERE (LEAVING < { fn NOW() } OR LEAVING IS NULL) AND NOT FIRSTNAME IS NULL ORDER BY PNAME,FIRSTNAME", new object[0]);
                }
                else
                {
                    tblPersons = db.getDataTableExt("SELECT PERSON.ID, PNAME + ' ' + FIRSTNAME AS NAME "
                                                        + "FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN "
                                                                    + "JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN "
                                                                    + "FUNKTION ON JOB.FUNKTION_ID = FUNKTION.ID INNER JOIN "
                                                                    + "FUNKTION_TYP ON FUNKTION.FUNKTION_TYP_ID = FUNKTION_TYP.ID "
                                                        + "WHERE ((FUNKTION_TYP.EXTERNAL_REF = '1') OR (FUNKTION_TYP.EXTERNAL_REF = '2')) AND (LEAVING < { fn NOW() } OR LEAVING IS NULL) AND NOT FIRSTNAME IS NULL "
                                                        + "ORDER BY PNAME,FIRSTNAME ");
                }
                
                foreach (DataRow personRow in tblPersons.Rows)
                {
                    lstPerson.Items.Add(new ListItem(personRow["NAME"].ToString(), personRow["ID"].ToString()));
                }


            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {

            string selected = "";
            string Ids = "";
            
            //OE or person selected?
            if (lstOE.SelectedIndex > 0)
            {
                //OE
                selected = "oe";
                Ids = lstOE.SelectedValue;
                if (chkSubOEs.Checked)
                {
                    //also list Sub-OEs 
                    Ids = db.Orgentity.addAllSubOEIDs(Ids);
                }
            }
            else
            {
                //person
                selected = "person";
                Ids = lstPerson.SelectedValue;
            }
            if (Request.QueryString["alias"] != "EigeneinschaetzungSPVKader.rpt")
            {
                if (Ids != "-1" && (db.lookup("count(PersonID)", "SelfReatingSPV", "OrgentityID IN (" + Ids + ")", 0) > 0) && selected == "oe" || selected == "person")
                {
                    string Ids_enc = Global.EncodeTo64(Ids);

                    Response.Redirect("CrystalReportViewer.aspx?alias=EigeneinschaetzungSPV&param0=" + selected + "&param1=" + Ids, true);
                }
                else
                {
                    //nothing selected or no persons in selected OE
                    lblNotFound.Text = map.get("SelectNameOE", "NotFound");
                }
            }
            else
            {
                if (Ids != "-1" && (db.lookup("count(PersonID)", "SelfReatingSPVKader", "OrgentityID IN (" + Ids + ")", 0) > 0) && selected == "oe" || selected == "person")
                {
                    string Ids_enc = Global.EncodeTo64(Ids);

                    Response.Redirect("CrystalReportViewer.aspx?alias=EigeneinschaetzungSPVKader&param0=" + selected + "&param1=" + Ids, true);
                }
                else
                {
                    //nothing selected or no persons in selected OE
                    lblNotFound.Text = map.get("SelectNameOE", "NotFound");
                }
            }
        }
    }
}
