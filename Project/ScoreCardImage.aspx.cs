using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace ch.appl.psoft.Project
{

    public partial class ScoreCardImage : System.Web.UI.Page {
        public static string IMAGE_EXTENSION   = ".jpg";
        public static string IMAGE_CONTENTTYPE = "image/jpeg";
        public static long   IMAGE_QUALITY     = 100L;  // Quality 100% means almost lossless compression
        public static string REPORT_PREFIX     = "ScoreCard_";

        protected void Page_Load(object sender, System.EventArgs e) {
            
            if (Session["ScoreCardImage"] != null) {         
                Response.ContentType = IMAGE_CONTENTTYPE;
                System.Drawing.Image map = (System.Drawing.Image) Session["ScoreCardImage"];

                long projectID = ch.psoft.Util.Validate.GetValid(Request.QueryString["projectID"], -1L);
                DBData db = DBData.getDBData(Session);
                db.connect();
                try{
                    if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "PROJECT", projectID, true, true)){
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
                        SaveImage(map, projectID, Session, Request);
                        map.Dispose();
                    }
                }
                catch(Exception ex) {
                    Logger.Log(ex, Logger.ERROR);
                }
                finally{
                    db.disconnect();
                }
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType) {
            foreach (ImageCodecInfo encoder in ImageCodecInfo.GetImageEncoders()) {
                if(encoder.MimeType == mimeType)
                    return encoder;
            }
            return null;
        }

        public static string SaveImage(System.Drawing.Image image, long projectID, HttpSessionState session, HttpRequest request){
            string fullName = "";
            if (projectID > 0) {
                EncoderParameters eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(Encoder.Quality, IMAGE_QUALITY);
                ImageCodecInfo ici = GetEncoderInfo(IMAGE_CONTENTTYPE);
                fullName = request.MapPath("~" + ReportModule.REPORTS_DIRECTORY + "/" + REPORT_PREFIX + projectID + "_" + SessionData.getSessionID(session) + IMAGE_EXTENSION);
                Stream stream = File.Open(fullName,FileMode.Create,FileAccess.Write);
                image.Save(stream, ici, eps);
                stream.Close();
            }
            return fullName;
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
