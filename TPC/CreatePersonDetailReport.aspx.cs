using ch.appl.psoft.Common;
using System;
namespace ch.appl.psoft.TPC
{
    public partial class CreatePersonDetailReport : PsoftPage
    {
        // Fields
        private const string PAGE_URL = "/TPC/CreatePersonDetailReport.aspx";

        // Methods
        static CreatePersonDetailReport()
        {
            PsoftPage.SetPageParams("/TPC/CreatePersonDetailReport.aspx", new string[] { "lohnId", "salaryComponent" });
        }

        public CreatePersonDetailReport()
        {
            base.PageURL = "/TPC/CreatePersonDetailReport.aspx";
        }

        public static string GetURL(params object[] queryParams)
        {
            return PsoftPage.CreateURL("/TPC/CreatePersonDetailReport.aspx", queryParams);
        }

        private void InitializeComponent()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            long queryValue = base.GetQueryValue("lohnId", (long)(-1L));
            string salaryKomponent = base.GetQueryValue("salaryComponent", "");
            string outputDirectory = base.Request.MapPath("~/Reports");
            string pdfName = "TPC_" + queryValue;
            TPCPersonDetailReport report = new TPCPersonDetailReport(this.Session);
            report.createPersonDetailReport(queryValue, salaryKomponent);
            report.saveReport(outputDirectory, pdfName);
            base.Response.Redirect(Global.Config.baseURL + "/Reports/" + report.PDFFilename, false);
        }
    }
}