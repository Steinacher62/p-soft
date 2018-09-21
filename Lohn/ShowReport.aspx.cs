using ch.appl.psoft.Common;
using System;
using System.Text;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// Anzeigen des Excel-Outputs
    /// Der Reportname muss inklusive relativem Pfad bezüglich dieser Seite
    /// angegeben sein.
    /// </summary>
    public partial class ShowReport : PsoftPage
    {
        private const string PAGE_URL = "/Lohn/ShowReport.aspx";
        static ShowReport()
        {
            SetPageParams(PAGE_URL, "reportName");
        }

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        public ShowReport() : base()
        {
            PageURL = PAGE_URL;
        }

        #region Protected overrided methods from parent class
        protected override void Initialize()
        {
            // base initialize
            base.Initialize();
        }

        /// <summary>
        /// Der Inhalt wird mit einem onLoad-Javascript eingefüllt!
        /// </summary>
        /// <param name="bodyOnLoad"></param>
        protected override void AppendBodyOnLoad(StringBuilder bodyOnLoad)
        {
            base.AppendBodyOnLoad (bodyOnLoad);
            bodyOnLoad.Append("window.location.href='" + GetQueryValue("reportName", "") + "';");
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) 
        {
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
            this.ID = "ShowReport";

        }
		#endregion
    }
}
