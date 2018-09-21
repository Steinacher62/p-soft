using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for PrintAverageOEPerformance.
    /// </summary>
    public partial class PrintAverageOEPerformance : System.Web.UI.Page
    {
        protected int _personID = -1;
        protected string _period = "";
        protected string _onloadString;
        protected string _orgentityIDs;
        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            _personID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], -1);
            _period = ch.psoft.Util.Validate.GetValid(Request.QueryString["period"], "");
            _orgentityIDs = ch.psoft.Util.Validate.GetValid(Request.QueryString["orgentityIDs"], "");
            
                         

            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY); 
            string imageDirectory = Request.MapPath("~/images");
            
            
            
            DBData db = DBData.getDBData(Session);
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);

            try
            {
                db.connect();


                AverageOEPerformanceReport report = new AverageOEPerformanceReport(Session, imageDirectory);

                fileName = "averageOEPerformance" + _personID;
                report.createReport(_personID,_period,_orgentityIDs);
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
