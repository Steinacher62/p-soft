using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;

namespace ch.appl.psoft.Laufenburg
{
    /// <summary>
    /// Summary description for PrintJobDescription.
    /// </summary>
    public partial class PrintJobDescription : System.Web.UI.Page
    {
        protected int _jobID = -1;
        protected long _employment_ID = -1;
        protected long _personId = -1;
        protected string _onloadString;
        protected long _funktionID = -1;
        protected int _groupNumber = 0;
        protected string _reportDate = DateTime.Now.ToString("d");
        protected bool isFirstGrp = true;
        protected string tmpSqltableName;

        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY);
            _jobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], -1); 
            DBData db = DBData.getDBData(Session);
            _employment_ID = (long)db.lookup("EMPLOYMENT_ID", "JOB", "ID = " + _jobID, -1L);
            _personId = (long)db.lookup("PERSON_ID","EMPLOYMENT", "ID = " + _employment_ID, -1L);
            _funktionID = (long)db.lookup("FUNKTION_ID", "JOB", "ID = " + _jobID, -1L);

            string imageDirectory = Request.MapPath("~/images");
            string reportfile = Server.MapPath(Global.Config.baseURL + "/crystalreports/StellenbeschreibungLaufenburg.rpt");
            fileName = "averagePerformance" + _funktionID;

            ReportDocument rpt1 = ReportFactory.GetReport();

            rpt1.Load(reportfile);

            //set db logon for report
            ConnectionInfo connectionInfo = new ConnectionInfo();
            connectionInfo.ServerName = Global.Config.dbServer;
            connectionInfo.DatabaseName = Global.Config.dbName;
            connectionInfo.UserID = Global.Config.dbUser;
            connectionInfo.Password = Global.Config.dbPassword;



            // delete temporary table if exists
            db.connect();
            string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stellenbeschreibung_%userid%]') "
                              + "AND type in (N'U')) "
                              + "DROP TABLE [dbo].[Stellenbeschreibung_%userid%]";
            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

            //create and fill temporary table
            tmpSqltableName = "Stellenbeschreibung_" + db.userId.ToString();
            string tbl_create = "CREATE TABLE [dbo].[Stellenbeschreibung_%userid%]("
            + "[ID] [bigint] IDENTITY,"
            + "[ORDNUMBERGROUP] [int] NULL,"
            + "[GROUPTITLE] [varchar](64) NOT NULL,"
            + "[ORDNUMBERDUTY] [int] NULL,"
            + "[DUTY] [varchar](1500) NULL"
            + ") ON [PRIMARY]";
            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));
            db.disconnect();

            SetDBLogonForReport(connectionInfo, rpt1);
            SetHeader(rpt1, db);

            long rootId = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "DUTYGROUP", "PARENT_ID is null", ""), -1);

            GetDutyGroups(db, null, rootId);


            //ParameterFields fields = CrystalReportViewer1.ParameterFieldInfo;
            //ParameterField field1 = new ParameterField();
            //field1.Name = "FunktionId";
            //ParameterDiscreteValue field1_value = new ParameterDiscreteValue();
            //field1_value.Value = _funktionID;
            //field1.CurrentValues.Add(field1_value);
            //fields.Add(field1);

            Tables tables = rpt1.Database.Tables;

            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(tableLogonInfo);
                table.Location = connectionInfo.DatabaseName + ".dbo.Stellenbeschreibung_" + db.userId.ToString();
            }

            CrystalReportViewer1.ReportSource = rpt1;


        }

        private void SetDBLogonForReport(ConnectionInfo connectionInfo, ReportDocument rpt1)
        {
            Tables tables = rpt1.Database.Tables;

            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(tableLogonInfo);
                table.Location = connectionInfo.DatabaseName + ".dbo." + table.Location.Substring(table.Location.LastIndexOf(".") + 1);
            }
        }

        private void SetHeader(ReportDocument rpt1, DBData db)
        {
            TextObject Stelleninhaber = (TextObject)rpt1.ReportDefinition.ReportObjects["Stelleninhaber"];
            TextObject BG = (TextObject)rpt1.ReportDefinition.ReportObjects["BG"];
            TextObject Geburtsdatum = (TextObject)rpt1.ReportDefinition.ReportObjects["Geburtsdatum"];
            TextObject Funktion = (TextObject)rpt1.ReportDefinition.ReportObjects["Funktion"];
            TextObject Bereich = (TextObject)rpt1.ReportDefinition.ReportObjects["Bereich"];
            TextObject Vorgesetzter = (TextObject)rpt1.ReportDefinition.ReportObjects["Vorgesetzter"];
            TextObject Stellvertretung = (TextObject)rpt1.ReportDefinition.ReportObjects["Stellvertretung"];
            TextObject UnterstellteBereiche = (TextObject)rpt1.ReportDefinition.ReportObjects["UnterstellteBereiche"];
            TextObject Stellenziele = (TextObject)rpt1.ReportDefinition.ReportObjects["Stellenziele"];

            // Name
            Stelleninhaber.Text = db.Person.getWholeName(_personId.ToString(), false, true, false);
            //Beschäftigungsgrad
            BG.Text = ch.psoft.Util.Validate.GetValid(db.lookup("ENGAGEMENT", "JOB", "ID=" + _jobID, ""), 0).ToString();
            //Geburtsdatum
            if (!db.lookup("DATEOFBIRTH", "PERSON", "ID = " + _personId.ToString()).ToString().Equals(""))
            {
                Geburtsdatum.Text = db.lookup("DATEOFBIRTH", "PERSON", "ID = " + _personId.ToString()).ToString().Substring(0, 10);
            }
            //Funktion und Stellenziele
             object[] values = db.lookup(
                    new string[] 
                    {
                        db.langAttrName("FUNKTION", "TITLE"),
                        db.langAttrName("FUNKTION", "DESCRIPTION")
                    },
                    "FUNKTION",
                    "ID = " + _funktionID
                );
            Funktion.Text = DBColumn.GetValid(values[0], "");
            Stellenziele.Text = DBColumn.GetValid(values[1], "");
            //Bereich
            values = db.lookup(new string[] { "O.ID", "O." + db.langAttrName("FUNKTION", "TITLE") }, "ORGENTITY O inner join JOB J on O.ID = J.ORGENTITY_ID", "J.ID = " + _jobID);
            Bereich.Text = DBColumn.GetValid(values[1], "");
            long orgentityId = DBColumn.GetValid(values[0], (long)-1);
            //Vorgesetzte Funktion
            DataTable tab = db.getDataTable("select dbo.GET_LEADERFUNCTIONID(" + _personId + ")", Logger.VERBOSE);
            long leaderFunctionID = ch.psoft.db.SQLColumn.GetValid(tab.Rows[0][0], 0L);

            string sqlFunction = "select distinct F." + db.langAttrName("FUNKTION", "TITLE")
                        + " from FUNKTION F WHERE"
                        + " F.ID = " + leaderFunctionID;

            DataTable funktionTable = db.getDataTable(
                    sqlFunction,
                    "FUNKTION"
                );

            foreach (DataRow row in funktionTable.Rows)
            {
                if ( !object.Equals(row[0].ToString(),Funktion))
                {
                    Vorgesetzter.Text = row[0].ToString();
                }
            }

            //Stellvertretung
            long proxyPersonId = (long)db.lookup("PROXY_PERSON_ID", "JOB", "ID = " + _jobID.ToString(), 0L);
            if (proxyPersonId > 0)
            {
                Stellvertretung.Text = db.Person.getWholeName(proxyPersonId.ToString(), false, true, false);
            }

            //Unterstellte Bereiche
            if (ch.psoft.Util.Validate.GetValid(db.lookup("TYP", "JOB", "ID=" + _jobID, ""), 0) == 1)
            {
                DataTable subOETable = db.getDataTable(
                        "select " + db.langAttrName("ORGENTITY", "TITLE")
                            + " from ORGENTITY"
                            + " where PARENT_ID = " + orgentityId + "OR ID = " + orgentityId
                            + " order by ORDNUMBER",
                        "ORGENTITY"
                    );


                string UB = "";
                foreach (DataRow row in subOETable.Rows)
                {
                    UB += row[0].ToString() +" \r\n";
                }
                UB = UB.Substring(0, UB.Length - 3);
                UnterstellteBereiche.Text = UB;

                rpt1.ReportDefinition.ReportObjects["UnterstellteBereicheTitel"].Height = subOETable.Rows.Count * 300;
                rpt1.ReportDefinition.ReportObjects["UnterstellteBereiche"].Height = subOETable.Rows.Count * 300;
            }


        }
        private void GetDutyGroups(DBData db,DataRow parentGrp, long parentId)
        {
            long dutyGroupID = parentId;

            string sql = "";
            string sqlOrdr = " order by ORDNUMBER";
            DataTable table2 = null;
            if (parentGrp == null)
            {
                sql = "select DUTY_COMPETENCE__DUTY_VALIDITY_V.*, DUTY.ORDNUMBER from DUTY_COMPETENCE__DUTY_VALIDITY_V INNER JOIN DUTY ON DUTY.ID = DUTY_ID where " + getRestriction() + " and DUTY_COMPETENCE__DUTY_VALIDITY_V.DUTYGROUP_ID is null";
                sql += sqlOrdr;
                table2 = db.getDataTable(sql, "DUTY_COMPETENCE__DUTY_VALIDITY_V");
                addGroup(db, table2, ref isFirstGrp, null);
            }

            sql = "select DUTY_COMPETENCE__DUTY_VALIDITY_V.*, DUTY.ORDNUMBER from DUTY_COMPETENCE__DUTY_VALIDITY_V INNER JOIN DUTY ON DUTY.ID = DUTY_ID where " + getRestriction() + " and DUTY_COMPETENCE__DUTY_VALIDITY_V.DUTYGROUP_ID=" + dutyGroupID;
            sql += sqlOrdr;
            table2 = db.getDataTable(sql, "DUTY_COMPETENCE__DUTY_VALIDITY_V");
            addGroup(db, table2, ref isFirstGrp, parentGrp);

            DataTable grpTable = db.getDataTable("select * from DUTYGROUP where PARENT_ID =" + parentId + " order by ORDNUMBER", "DUTYGROUP");
            foreach (DataRow child in grpTable.Rows)
            {
                GetDutyGroups(db,child, ch.psoft.Util.Validate.GetValid(child["ID"].ToString(), -1L));
            }	
        }

        protected string getRestriction()
        {
            if (SessionData.showValidDutyCompOnly(Session))
                return "VALID_FROM<=GetDate() and VALID_TO>=GetDate() and DUTY_VALIDITY_VALID_FROM<=GetDate() and DUTY_VALIDITY_VALID_TO>=GetDate() and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
            else
                return "DUTY_VALIDITY_VALID_FROM<=GetDate() and DUTY_VALIDITY_VALID_TO>=GetDate() and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
        }

        private void addGroup(DBData db, DataTable table, ref bool isFirstGrp, DataRow grpRow)
        {
            string sql;
            string groupName;
            db.connect();
            if (grpRow != null)
            {
                groupName =  ch.psoft.Util.Validate.GetValid(grpRow[db.langAttrName(grpRow.Table.TableName, "TITLE")].ToString().Replace("'","''"));
            }
            else
            {
                groupName = "Freie Aufgaben";
            }

            foreach (DataRow row in table.Rows)
            {
                sql = "INSERT INTO " + tmpSqltableName + "(ORDNUMBERGROUP,GROUPTITLE,ORDNUMBERDUTY,DUTY) VALUES (0,'" + grpRow[6].ToString().Replace("'", "''") + "','" + row[24] + "','" + row[17].ToString().Replace("'", "''") + "')";
               db.execute(sql); 
            }
            db.disconnect();
        }
        
		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
		
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {    
        }
		#endregion
    }
}
