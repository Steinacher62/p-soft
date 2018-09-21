using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for PrintAppraisal.
    /// </summary>
    public partial class PrintAppraisal : System.Web.UI.Page {
        protected int _appraisalID = -1;
        protected string _onloadString;
        
        protected void Page_Load(object sender, System.EventArgs e) {
            _appraisalID = ch.psoft.Util.Validate.GetValid(Request.QueryString["appraisalID"], -1);

            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY); 
            string imageDirectory = Request.MapPath("~/images");
            
            DBData db = DBData.getDBData(Session);
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);

            try {
                db.connect();

                AppraisalReport report = new AppraisalReport(Session, imageDirectory);

                fileName = "appraisal" + _appraisalID;
                report.createReport(_appraisalID);

                if (report != null)
                    report.saveReport(outputDirectory, fileName);

                Response.Redirect(Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + report.PDFFilename, false);
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                db.disconnect();
            }

        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
        private void InitializeComponent() {    
        }
		#endregion
    }
}
