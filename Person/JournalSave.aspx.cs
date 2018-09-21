using ch.appl.psoft.db;
using ch.appl.psoft.Report;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;


//Journal_PFS_PersonalNr_Datum

namespace ch.appl.psoft.Person
{
    public partial class JournalSave : System.Web.UI.Page
    {
        protected long _journalID = -1;
        protected long _personID = -1;
        protected string _createdOn = "";
        protected string _personnelnumber = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData _db = null;
            _db = DBData.getDBData(Session);
            _db.connect();
            DataTable ratings = _db.getDataTable("SELECT ID, PERSON_ID, CREATED  FROM PERSON_JOURNAL");

            foreach (DataRow rating in ratings.Rows)
            {
                _journalID = Convert.ToInt32(rating["ID"]);
                _personID = Convert.ToInt32(rating["PERSON_ID"]);
                _createdOn = rating["CREATED"].ToString().Remove(10, 9);
                _personnelnumber = _db.lookup("PERSONNELNUMBER", "PERSON", "ID=" + _personID).ToString();
                export();
            }
        }

        private void export()
        {
            string fileName = "Journal_PFS_" + _personnelnumber + "_" + _createdOn;
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY);
            string imageDirectory = Request.MapPath("~/images");

            DBData db = DBData.getDBData(Session);

            string reportfile = Server.MapPath(Global.Config.baseURL + "/crystalreports/Journal.rpt");

            ReportDocument rpt1 = ReportFactory.GetReport();

            rpt1.Load(reportfile);
         

            //set db logon for report
            ConnectionInfo connectionInfo = new ConnectionInfo();
            connectionInfo.ServerName = Global.Config.dbServer;
            connectionInfo.DatabaseName = Global.Config.dbName;
            connectionInfo.UserID = Global.Config.dbUser;
            connectionInfo.Password = Global.Config.dbPassword;
            SetDBLogonForReport(connectionInfo, rpt1);

            ////ParameterFields fields = CrystalReportViewer1.ParameterFieldInfo;
            //ParameterField field = new ParameterField();
            //field.Name = "@JournalId";
            //ParameterDiscreteValue field_value = new ParameterDiscreteValue();
            //field_value.Value = _journalID;
            //field.CurrentValues.Add(field_value);
            ////fields.Add(field);
            //rpt1.ParameterFields.Add(field);


            rpt1.SetParameterValue("JournalId", _journalID);


            // export to PDF
            ExportOptions exportOpts = new ExportOptions();
            PdfRtfWordFormatOptions PDFOpts = new PdfRtfWordFormatOptions();
            DiskFileDestinationOptions diskOpts = new DiskFileDestinationOptions();
            exportOpts = rpt1.ExportOptions;
            exportOpts.ExportFormatOptions = PDFOpts;
            

            exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat;
            exportOpts.ExportDestinationType = ExportDestinationType.DiskFile;
            string journal_filename = fileName + ".pdf";
            diskOpts.DiskFileName = outputDirectory + "\\" + journal_filename;
            exportOpts.DestinationOptions = diskOpts;

            rpt1.Export();

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
    }
}