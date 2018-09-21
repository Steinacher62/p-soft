using ch.appl.psoft.db;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;

namespace ch.appl.psoft.Report
{
    public partial class export : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //export report to Excel
            
            DBData db = DBData.getDBData(Session);

            string pfad = Server.MapPath(Global.Config.baseURL + "/crystalreports/wordexport");
            //string pfad = @"\\HCHS0003\source\data\HR\Ruc\Stellenbeschreibungen";
            //string pfad = @"x:\appl\Psoft Zentral\Stellenbeschreibungen";

            string report = Request.QueryString["report"];
            string filename = Server.MapPath(Global.Config.baseURL + "/crystalreports/" + report + ".rpt");

            //Netzlaufwerk verbinden
            System.Diagnostics.Process.Start("net", @"use x: /delete /yes");
            System.Diagnostics.Process.Start("net", @"use x: \\hchs0003\source /user:habasitch.local\x-p-soft 161006");

            //set db logon for report
            ConnectionInfo connectionInfo = new ConnectionInfo();
            connectionInfo.ServerName = Global.Config.dbServer;
            connectionInfo.DatabaseName = Global.Config.dbName;
            connectionInfo.UserID = Global.Config.dbUser;
            connectionInfo.Password = Global.Config.dbPassword;

            ReportDocument rpt1 = ReportFactory.GetReport();
            rpt1.Load(filename);
            SetDBLogonForReport(connectionInfo, rpt1);

            switch (report)
            {
                case "TestStellenbeschreibung":
                    break;
                case "StellenbeschreibungOE":
                    string oe = Request.QueryString["oe"];
                    string r = Request.QueryString["r"];

                    DataTable tblJobs = new DataTable();
                    if (r == "1")
                    {
                        tblJobs = db.getDataTableExt("SELECT JobId,Stellenbezeichnung FROM StellenbeschreibungOE WHERE OEId IN (" + db.Orgentity.addAllSubOEIDs(oe) + ")", new object[0]);
                    }
                    else
                    {
                        tblJobs = db.getDataTableExt("SELECT JobId,Stellenbezeichnung FROM StellenbeschreibungOE WHERE OEId = " + oe, new object[0]);
                    }

                    foreach (DataRow aktRow in tblJobs.Rows)
                    {
                        ParameterFieldDefinition parameter = rpt1.DataDefinition.ParameterFields["JobId"];
                        ParameterValues currentValues = parameter.CurrentValues;
                        currentValues.Clear();
                        ParameterDiscreteValue newValue = new ParameterDiscreteValue();
                        newValue.Value = Convert.ToInt32(aktRow["JobId"]);
                        currentValues.Add(newValue);
                        parameter.ApplyCurrentValues(currentValues);

                        string rtf = aktRow["Stellenbezeichnung"].ToString() + ".xls";
                        rtf = rtf.Replace("\\", "");
                        rtf = rtf.Replace("'", "");
                        rtf = rtf.Replace(":", "");
                        rtf = rtf.Replace("/", "");
                        string speicherort = pfad + "\\" + rtf;
                        try
                        {
                            ExportOptions exportOpts = new ExportOptions();
                            ExcelDataOnlyFormatOptions excelDataFormatOpts = new ExcelDataOnlyFormatOptions();
                            DiskFileDestinationOptions diskOpts = new DiskFileDestinationOptions();
                            exportOpts = rpt1.ExportOptions;

                            excelDataFormatOpts.MaintainRelativeObjectPosition = true;
                            exportOpts.ExportFormatType = ExportFormatType.ExcelRecord;
                            exportOpts.FormatOptions = excelDataFormatOpts;

                            exportOpts.ExportDestinationType = ExportDestinationType.DiskFile;
                            diskOpts.DiskFileName = speicherort;
                            exportOpts.DestinationOptions = diskOpts;

                            rpt1.Export();
                        }
                        catch(Exception fehler)
                        {
                            lblOutput.Text = "Fehler: " + fehler.Message + "<br><br>" + fehler.Source;
                        }
                    }

                    break;
                default:
                    break;
            }

            lblOutput.Text += "Export abgeschlossen!";
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
