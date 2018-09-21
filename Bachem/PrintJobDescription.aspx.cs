using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Bachem
{
    /// <summary>
    /// Summary description for PrintJobDescription.
    /// </summary>
    public partial class PrintJobDescription : System.Web.UI.Page
    {
        protected int _jobID = -1;
        protected string _onloadString;
        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            _jobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], -1);

            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY); 
            string imageDirectory = Request.MapPath("~/images");
            
            DBData db = DBData.getDBData(Session);
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);

            try
            {
                db.connect();

                BachemJobDescriptionReport report = new BachemJobDescriptionReport(Session, imageDirectory);

                fileName = "job" + _jobID;
                report.createReport(_jobID);

                if (report != null)
                    report.saveReport(outputDirectory, fileName);

                Response.Redirect(Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + report.PDFFilename, false);
            }
            catch (Exception ex)
            {
                Logger.Log(ex,Logger.ERROR);
            }
            finally
            {
                db.disconnect();
            }

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
