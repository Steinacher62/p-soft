using ch.appl.psoft.Common;
using System;
namespace ch.appl.psoft.RSR
{
    public partial class CreatePersonDetailReport : PsoftPage
    {
        // Fields
        protected long _lohnId = -1L;
        private const string PAGE_URL = "/Rsr/CreatePersonDetailReport.aspx";

        // Methods
        static CreatePersonDetailReport()
        {
            PsoftPage.SetPageParams("/Rsr/CreatePersonDetailReport.aspx", new string[] { "lohnId" });
        }

        public CreatePersonDetailReport()
        {
            base.PageURL = "/Rsr/CreatePersonDetailReport.aspx";
        }

        public static string GetURL(params object[] queryParams)
        {
            return PsoftPage.CreateURL("/Rsr/CreatePersonDetailReport.aspx", queryParams);
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
            this._lohnId = base.GetQueryValue("lohnId", (long)(-1L));
            string outputDirectory = base.Request.MapPath("~/Reports");
            string pdfName = "RSR_" + this._lohnId;
            RSRPersonDetailReport report = new RSRPersonDetailReport(this.Session);
            report.createPersonDetailReport(this._lohnId);
            report.saveReport(outputDirectory, pdfName);
            base.Response.Redirect(Global.Config.baseURL + "/Reports/" + report.PDFFilename, false);
        }
    }
}
