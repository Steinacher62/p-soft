using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace ch.appl.psoft.Organisation
{
    /// <summary>
    /// Summary description for NavigationTree.
    /// </summary>
    public partial class NavigationTree : System.Web.UI.Page {
        public static string IMAGE_EXTENSION   = ".jpg";
        public static string IMAGE_CONTENTTYPE = "image/jpeg";
        public static long   IMAGE_QUALITY     = 100L;  // Quality 100% means almost lossless compression
        public static string REPORT_PREFIX     = "Organigram_";

        /// <summary>
        /// Tabelle laden
        /// </summary>
        protected void Page_Load(object sender, System.EventArgs e) {
            
            if (this.Session["OrganisationImage"] != null) {         
                System.Drawing.Image map = (System.Drawing.Image) this.Session["OrganisationImage"];
                long id = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"], -1L);
                
                Response.ContentType = IMAGE_CONTENTTYPE;
                EncoderParameters eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(Encoder.Quality, IMAGE_QUALITY);
                ImageCodecInfo ici = GetEncoderInfo(IMAGE_CONTENTTYPE);
                try{
                    map.Save(Response.OutputStream, ici, eps);
                    Response.Flush();
                }
                catch (Exception ex){
                    Logger.Log(ex, Logger.ERROR);
                }
                if (id > 0) {
                    String fullName = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY + "/" + REPORT_PREFIX + id + "_" + SessionData.getSessionID(Session) + IMAGE_EXTENSION);
                    Stream stream = File.Open(fullName,FileMode.Create,FileAccess.Write);
                    //Stream stream = File.OpenRead(fullName);
                    map.Save(stream, ici, eps);
                    stream.Close();
                }
                map.Dispose();
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType) {
            foreach (ImageCodecInfo encoder in ImageCodecInfo.GetImageEncoders()) {
                if(encoder.MimeType == mimeType)
                    return encoder;
            }
            return null;
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
