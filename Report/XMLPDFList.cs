using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Report
{

    /// <summary>
    /// Base class for all PDF Lists.
    /// </summary>
    public abstract class XMLPDFList : List{
        protected XmlTextWriter _writer = null;
        protected Encoding _encode = null;
        protected string _fileName = "";
        protected string _name = "";
 
        public string pdfFilename {
            get { return _name+".pdf"; }
        }
        public string xmlFilename {
            get { return _name+".xml"; }
        }
        public abstract void writeList(long id, string path, string imagePath, LanguageMapper map, DBData db, DataTable data, HttpSessionState session, params string[] substituteValues);

        protected virtual void open(long id, string path, DBData db, HttpSessionState session) {
            if (!_append) {
                base.open(db);
                if (db != null && _name == "") _name = db.lookup("title_mnemo","reportlayout","id="+id,false);
                _name += "_" + SessionData.getSessionID(session);
                _encode = Encoding.GetEncoding("ISO-8859-1");
                _fileName = path+"\\"+_name;
                _writer = new XmlTextWriter(_fileName+".xml",_encode);
                _writer.Formatting = Formatting.Indented;
                _writer.QuoteChar = '\'';
                _writer.Indentation = 2;
                _writer.WriteStartDocument();
            }
        }
        protected virtual new void close() {
            if (!_extend) {
                if (_writer != null) {
                    _writer.WriteEndDocument();
                    _writer.Close();
                    _writer = null;
                }
                base.close();
            }
        }

        /// <summary>
        /// The pdf file will be generated to file.
        /// </summary>
        protected bool savePDF() {
            string pdfFileName = _fileName + ".pdf";
            string xmlFileName = _fileName + ".xml";
            return generatePDF(xmlFileName,pdfFileName);
        }
        /// <summary>
        /// The pdf file will be generated to stream.
        /// </summary>
        protected bool writePDF(Stream stream) {
            string xmlFileName = _fileName + ".xml";
            return generatePDF(xmlFileName,stream);
        }
        /// <summary>
        /// pdf will be generated.
        /// </summary>
        private bool generatePDF(string xmlFileName, object output) {
            if (_extend) return false;

            System.GC.Collect();
            xmlpdf.licensing.Generator.setRuntimeKey("7BA376EB0DE0301372636518B401115DJPZT+ODUV3SK4ZMIDYRSRW==");
            xmlpdf.PDFDocument converter = new xmlpdf.PDFDocument();			

            try {
                converter.ValidationType = System.Xml.ValidationType.DTD;
                if (output is string) converter.generate(xmlFileName, output as string);
                else if (output is Stream) converter.generate(xmlFileName, output as Stream);
                else return false;
                return true;
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            return false;
        }

     }
}
