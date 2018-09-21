using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Report
{
    public partial class SelectApplRight : System.Web.UI.Page
    {
        DBData db;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            // connect to database
            db = DBData.getDBData(Session);
            db.connect();
            
            // apply language
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
            if (map == null)
            {
                map = LanguageMapper.getLanguageMapper(Application);
            }
            lblSelPerson.Text = map.get("ApplRights", "selPerson");
            lblShowFor.Text = map.get("ApplRights", "ShowFor");
            lblOR.Text = map.get("ApplRights", "OR");
            lblOE.Text = map.get("ApplRights", "OE");
            lblPerson.Text = map.get("ApplRights", "Person");
            cmdOk.Text = map.get("ApplRights", "showReport");

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
                DataTable tblPersons = db.getDataTableExt("SELECT ID,PNAME + ' ' + FIRSTNAME AS NAME FROM PERSON WHERE LEAVING IS NULL AND NOT FIRSTNAME IS NULL ORDER BY PNAME,FIRSTNAME", new object[0]);
                foreach (DataRow personRow in tblPersons.Rows)
                {
                    lstSelPerson.Items.Add(new ListItem(personRow["NAME"].ToString(), personRow["ID"].ToString()));
                    lstPerson.Items.Add(new ListItem(personRow["NAME"].ToString(), personRow["ID"].ToString()));
                }
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            //save results in temporary table, then call report

            // delete temporary table if exists
            string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[berechtigungen_%userid%]') "
                           + "AND type in (N'U')) "
                           + "DROP TABLE [dbo].[berechtigungen_%userid%]";
            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

            // create table
            string tbl_create = "CREATE TABLE [dbo].[berechtigungen_%userid%]("
                                + "[AcessorId] [bigint] NULL,"
                                + "[AcessorPnr] [varchar](64) NULL,"
                                + "[AcessorName] [varchar](64) NULL,"
                                + "[AcessorVorname] [varchar](64) NULL,"
                                + "[Gruppe] [varchar](80) NULL,"
                                + "[Rechte] [int] NULL,"
                                + "[Applikationsrecht] [int] NULL,"
                                + "[Job] [varchar](128) NULL,"
                                + "[IdPerson] [bigint] NULL,"
                                + "[Pnr] [varchar](64) NULL,"
                                + "[NamePerson] [varchar](64) NULL,"
                                + "[VornamePerson] [varchar](64) NULL,"
                                + "[IDAbteilung] [bigint] NULL,"
                                + "[Abteilung] [varchar](128) NULL,"
                                + "[AcessorMeberId][bigint] NULL"
                                + ") ON [PRIMARY]";
            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

            //fill table
            db.execute("INSERT INTO berechtigungen_" + db.userId.ToString() + " SELECT * FROM f_Berechtigungen(" + lstSelPerson.SelectedValue + ", " + lstOE.SelectedValue + ", " + lstPerson.SelectedValue + " )");

            //save application rights summary in seperate table

            //delete temporary table if exists
            string tblAppl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[applikationsrechte_%userid%]') "
                           + "AND type in (N'U')) "
                           + "DROP TABLE [dbo].[applikationsrechte_%userid%]";
            db.execute(tblAppl_del.Replace("%userid%", db.userId.ToString()));

            //create table
            string tblAppl_create = "CREATE TABLE [dbo].[applikationsrechte_%userid%]("
                                + "[IdPerson] [bigint] NULL"
                                //+ "[Applikationsrecht] [int] NULL"
                                //+ "[Recht] [int] NULL"
                                + ") ON [PRIMARY]";
            db.execute(tblAppl_create.Replace("%userid%", db.userId.ToString()));

            //fill table
            db.execute("INSERT INTO applikationsrechte_" + db.userId.ToString() + " SELECT IdPerson FROM berechtigungen_" + db.userId.ToString() + " GROUP BY IdPerson");

            //add columns for applications
            string tblAppl_add = "ALTER TABLE applikationsrechte_%userid% "
                                + "ADD [RechteAusbildung] [varchar](1) NULL,"
                                + " [RechteFunktionsbewertung] [varchar](1) NULL,"
                                + " [RechteLeistungsbewertung] [varchar](1) NULL,"
                                + " [RechteLohn] [varchar](1) NULL,"
                                + " [RechteMbO] [varchar](1) NULL,"
                                + " [RechteSkills] [varchar](1) NULL,"
                                + " [RechteStellenbeschreibung] [varchar](1) NULL,"
                                + " [RechteStellenerwartungen] [varchar](1) NULL";
            db.execute(tblAppl_add.Replace("%userid%", db.userId.ToString()));

            //insert application rights
            string sqlAppl = "UPDATE applikationsrechte_%userid% "
                            + "SET %col% = "
                            + "CASE "
                            + "WHEN (select max(rechte) as recht from berechtigungen_%userid% where IdPerson = %IdPerson% and Applikationsrecht = %ApplRight%) > 2 "
                            + "THEN 'B' "
                            + "WHEN (select max(rechte) as recht from berechtigungen_%userid% where IdPerson = %IdPerson% and Applikationsrecht = %ApplRight%) = 2 "
                            + "THEN 'L' "
                            + "ELSE ' ' "
                            + "END "
                            + "WHERE IdPerson = %IdPerson%";

            DataTable personTable = db.getDataTable("SELECT IdPerson FROM applikationsrechte_" + db.userId.ToString());
            foreach (DataRow personRow in personTable.Rows)
            {
                string IdPerson = personRow["IdPerson"].ToString();
                string personSql = sqlAppl.Replace("%userid%", db.userId.ToString()).Replace("%IdPerson%", IdPerson);
                db.execute(personSql.Replace("%col%", "RechteAusbildung").Replace("%ApplRight%", "41"));
                db.execute(personSql.Replace("%col%", "RechteFunktionsbewertung").Replace("%ApplRight%", "51"));
                db.execute(personSql.Replace("%col%", "RechteLeistungsbewertung").Replace("%ApplRight%", "11"));
                db.execute(personSql.Replace("%col%", "RechteLohn").Replace("%ApplRight%", "110"));
                db.execute(personSql.Replace("%col%", "RechteMbO").Replace("%ApplRight%", "60"));
                db.execute(personSql.Replace("%col%", "RechteSkills").Replace("%ApplRight%", "31"));
                db.execute(personSql.Replace("%col%", "RechteStellenbeschreibung").Replace("%ApplRight%", "21"));
                db.execute(personSql.Replace("%col%", "RechteStellenerwartungen").Replace("%ApplRight%", "12"));
            }

            Response.Redirect("CrystalReportViewer.aspx?alias=AppliRights",true);
        }
    }
}
