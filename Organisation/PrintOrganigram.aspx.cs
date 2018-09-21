using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;
using System.Data;
using System.IO;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Organisation
{
    /// <summary>
    /// Summary description for PrintOrganigram.
    /// </summary>
    public partial class PrintOrganigram : System.Web.UI.Page {
        // A4
        private static int MaxW = 596;
        private static int MaxH = 842;

        protected void Page_Load(object sender, System.EventArgs e) {
            long id = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"], -1L);
            int imgW = (int) this.Session["OrganisationImageWidth"];
            int imgH = (int) this.Session["OrganisationImageHeight"];

            if (id <= 0) return;

            String imageName =  NavigationTree.REPORT_PREFIX + id + "_" + SessionData.getSessionID(Session);
            String imageFullName = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY + "/" + imageName + NavigationTree.IMAGE_EXTENSION);

            if (!File.Exists(imageFullName)) return;

            DBData db = DBData.getDBData(Session);
            db.connect();
            try {
                PdfOrganigram report = new PdfOrganigram();
                XmlTextWriter writer = report.open(id, Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY + "/"), db, Session);
                string title = db.lookup("title","chart","id="+id,false);

                writer.WriteStartElement("document");
                writer.WriteAttributeString("margin-left","5");
                writer.WriteAttributeString("margin-right","5");
                writer.WriteAttributeString("margin-top","20");
                writer.WriteAttributeString("margin-bottom","5");
                int lrBorder = 10;
                int tbBorder = 100;
                int w = MaxW - lrBorder;
                int h = MaxH - tbBorder;
                if (imgW > w && imgW > imgH) {
                    writer.WriteAttributeString("orientation","landscape");
                    w = MaxH - lrBorder;
                    h = MaxW - tbBorder;
                }

                writer.WriteStartElement("images");
                writer.WriteStartElement("image");
                writer.WriteAttributeString("file-name",imageFullName);
                writer.WriteAttributeString("image-name",imageName);
                writer.WriteAttributeString("anti-aliasing","false");
                writer.WriteAttributeString("quality","100");
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("page-footer");
                writer.WriteStartElement("row");
                writer.WriteStartElement("cell");
                writer.WriteAttributeString("font-name","helvetica");
                writer.WriteAttributeString("font-size","10");
                writer.WriteAttributeString("align","center");
                writer.WriteString(DateTime.Now.ToShortDateString());
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("table");
                writer.WriteAttributeString("widths","100%");
                writer.WriteStartElement("row");
                writer.WriteStartElement("cell");
                writer.WriteAttributeString("font-name","helvetica-bold");
                writer.WriteAttributeString("font-size","14");
                writer.WriteAttributeString("align","center");
                writer.WriteString(title);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("row");
                writer.WriteStartElement("cell");
                writer.WriteAttributeString("min-height","20");
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("row");
                writer.WriteStartElement("cell");
                writer.WriteAttributeString("align","center");

                writer.WriteStartElement("show-image");
                writer.WriteAttributeString("image-name",imageName);

                double scale = (double) w / (double) imgW;
                if (scale < 1.0) {
                    if (((double) h / (double) imgH) < scale) writer.WriteAttributeString("scale-height",h.ToString());
                    else writer.WriteAttributeString("scale-width",w.ToString());
                }                
                else if (imgH > h) writer.WriteAttributeString("scale-height",h.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndElement();

                report.close();
                /*MemoryStream stream = new MemoryStream();
                report.writePDF(stream);
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length",stream.Length.ToString());
                Response.BinaryWrite(stream.ToArray());
                Response.End();*/
                report.savePDF();
                Response.Redirect(Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + report.pdfFilename, false);
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
    class PdfOrganigram : XMLPDFList {
        public new XmlTextWriter open(long id, string path, DBData db, HttpSessionState session) {
            base._name = NavigationTree.REPORT_PREFIX + id;
            base.open(id,path,db, session);
            return _writer;
        }
        public new void close() {
            base.close();
        }
        public new bool savePDF() {
            return base.savePDF();
        }
        public new bool writePDF(Stream stream) {
            return base.writePDF(stream);
        }
        public override void writeList(long id, string path, string imagePath, LanguageMapper map, DBData db, DataTable data, HttpSessionState session, params string[] substituteValues) {
        }
    }

}
