using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ch.psoft.Util;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report;
using Infragistics.WebUI.WebHtmlEditor;
using SautinSoft;


namespace ch.appl.psoft.Energiedienst.Controls
{
    public partial class PrintJobDescriptionCtrl : PSOFTMapperUserControl
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

        public static string Path
        {
            get { return Global.Config.baseURL + "/Energiedienst/Controls/PrintJobDescriptionCtrl.ascx"; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY);
            _jobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], -1);
            DBData db = DBData.getDBData(Session);
            _employment_ID = (long)db.lookup("EMPLOYMENT_ID", "JOB", "ID = " + _jobID, -1L);
            _personId = (long)db.lookup("PERSON_ID", "EMPLOYMENT", "ID = " + _employment_ID, -1L);
            _funktionID = (long)db.lookup("FUNKTION_ID", "JOB", "ID = " + _jobID, -1L);

            FunctionJobDescriptionReport report = new FunctionJobDescriptionReport(Session, "");
            report.writeHead(_jobID, _personId, _employment_ID, _funktionID, outputDirectory, Server.MapPath(Global.Config.baseURL + "/images/logoEnergiedienst.jpg"));
            string imageDirectory = Request.MapPath("~/images");
            string reportfile = Server.MapPath(Global.Config.baseURL + "/crystalreports/StellenbeschreibungEnergiedienst.rpt");
            fileName = "averagePerformance" + _funktionID;

            report.saveReport(outputDirectory, fileName);

            pdfFrame.Attributes.Add("src", Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + fileName + ".pdf");


            FBSGeprueft.Text = "Eingesehen und geprüft von " + db.lookup("Firstname", "Person", "ID = " + _personId).ToString() + " " + db.lookup("PNAME", "Person", "ID = " + _personId).ToString();
            if (db.lookup("JOB_DESCRIPTION_CHECKED", "JOB", "ID=" + _jobID).ToString().Equals(""))
            {
                FBSGeprueft.Checked = false;
            }
            else
            {
                FBSGeprueft.Checked = true;
            }
            FBSGeprueft.CheckedChanged += new EventHandler(FBW_Checked);
            if (db.userId == _personId && FBSGeprueft.Checked == false)
            {
                FBSGeprueft.Enabled = true;
            }
            else
            {
                FBSGeprueft.Enabled = false;
            }
        }

        protected void FBW_Checked(object sender, System.EventArgs e)
        {
            DateTime time = System.DateTime.Now;
            DBData db = DBData.getDBData(Session);
            db.connect();
            db.execute("UPDATE JOB SET JOB_DESCRIPTION_CHECKED = '" + time.ToString("MM.dd.yyyy") + "' WHERE ID = " + _jobID);
            db.disconnect();
            Response.Redirect(Request.RawUrl);
        }




        protected string getRestriction()
        {
            if (SessionData.showValidDutyCompOnly(Session))
                return "VALID_FROM<=GetDate() and VALID_TO>=GetDate() and DUTY_VALIDITY_VALID_FROM<=GetDate() and DUTY_VALIDITY_VALID_TO>=GetDate() and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
            else
                return "DUTY_VALIDITY_VALID_FROM<=GetDate() and DUTY_VALIDITY_VALID_TO>=GetDate() and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
        }

    }
}