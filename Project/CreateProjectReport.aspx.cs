using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;
using System.Xml;

namespace ch.appl.psoft.Project
{

    public partial class CreateProjectReport : PsoftPage {

        /// <summary>
        /// url argument name for the project id.
        /// </summary>
        public const string ARGNAME_URL_ID = "projectID";

        /// <summary>
        /// url argument name for the context.
        /// </summary>
        public const string ARGNAME_URL_CONTEXT = "context";

 

        /// <summary>
        /// This page.
        /// </summary>
        private const string PAGE_URL = "/Project/CreateProjectReport.aspx";


        static CreateProjectReport() {
            SetPageParams(PAGE_URL, ARGNAME_URL_ID, ARGNAME_URL_CONTEXT);
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public CreateProjectReport() : base() {
            PageURL = PAGE_URL;
        }
        protected long _projectID = -1;
        protected string _context = "";
        
        protected void Page_Load(object sender, System.EventArgs e) {
            _projectID = GetQueryValue(ARGNAME_URL_ID, -1);
            _context = GetQueryValue(ARGNAME_URL_CONTEXT, "");

            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY); 
            string imageDirectory = Request.MapPath("~/images");
            
            DBData db = DBData.getDBData(Session);
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);

            try {
                db.connect();
                switch (_context)
                {
                    default:
                        ProjectReport report = new ProjectReport(Session, Request, imageDirectory);

                        if (Global.Config.getModuleParam("project", "scoreCardPrintAsPdf", "0").Equals("1"))
                        {
                            fileName = "project" + _projectID;
                            report.createReport(_projectID);

                            if (report != null)
                                report.saveReport(outputDirectory, fileName);

                            Response.Redirect(Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + report.PDFFilename, false);
                        }
                        else
                        {
                            // xml report 
                            XmlDocument doc = new XmlDocument();
                            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

                            //report
                            ScoreCardReportXML scoreCardReport = new ScoreCardReportXML(db, Session);
                            XmlElement elm = scoreCardReport.createXML(doc, _projectID);

                            doc.AppendChild(elm);
                            string filenameAbs = XMLReport.getOutputfileAbsolutePath("scoreCardReport");
                            string filenameAbsOrg = XMLReport.getOutputfileAbsolutePath("scoreCardReportOrg");
                            doc.Save(filenameAbsOrg);
                            string filenameRel = XMLReport.getOutputfileRelativePath("scoreCardReport");

                            //stylesheet {  
                            XSLTransformer transformer = new XSLTransformer(ProjectModule.EXCEL_STYLE_SHEET_SCORE_CARD, ProjectModule.debugXML);
                            transformer.transform(doc, filenameAbs);
                            Response.ContentType = "application/vnd.ms-excel";
                            // }

                            Response.Redirect(filenameRel,false);
                        }
                        break;
                }
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
