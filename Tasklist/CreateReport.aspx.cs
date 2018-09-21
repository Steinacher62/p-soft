using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Summary description for CreateReport.
    /// </summary>
    public partial class CreateReport : PsoftPage
    {
		private const string PAGE_URL = "/Tasklist/CreateReport.aspx";

		static CreateReport()
		{
			SetPageParams(PAGE_URL, "context", "xID");
		}

		public static string GetURL(params object[] queryParams)
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		public CreateReport() : base()
		{
			PageURL = PAGE_URL;
		}
		protected string _context;
        protected int _xID = -1;
        protected string _onloadString;
        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            _context = GetQueryValue("context", "").ToLower();
            _xID = GetQueryValue("xID", -1);

            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY); 
            string imageDirectory = Request.MapPath("~/images");
            
            DBData db = DBData.getDBData(Session);
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);

            try
            {
                db.connect();

                TasklistMeasureReport report = new TasklistMeasureReport(Session, imageDirectory);

                if (_context == "tasklist")
                {
                    fileName = "tasklist" + _xID;
                    report.createTasklistReport(_xID);
                }
                else if (_context == "measure")
                {
                    fileName = "measure" + _xID;
                    report.createMeasureReport(_xID);
                }

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
