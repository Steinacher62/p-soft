using ch.appl.psoft.Interface;
using System;
using System.Xml;
using System.Xml.Xsl;

namespace ch.appl.psoft.Common
{
    class XSLTransformer
    {
        #region Properties

        bool Indent { set; get; }
        bool OmitXmlDeclaration { set; get; }
        string StyleSheet { get; set; }

        #endregion

        #region Constructors

        public XSLTransformer()
        {
            Indent = true;
            OmitXmlDeclaration = false;
        }

        public XSLTransformer(bool indent) : this()
        {
            this.Indent = indent;
        }

        public XSLTransformer(string stylesheet, bool indent)
            : this(indent)
        {
            this.StyleSheet = getStyleSheet(stylesheet);
        }

        #endregion

        #region Public Methods

        public void transform(XmlDocument xmlDocument, string output)
        {
            transform(xmlDocument, this.StyleSheet, output);
        }

        public void transform(XmlDocument xmlDocument, string stylesheet, string output)
        {
                XslCompiledTransform xslTrans = new XslCompiledTransform();

                //load the Xsl 
                xslTrans.Load(stylesheet, XsltSettings.TrustedXslt, new XmlUrlResolver());
                
                //create the output stream
                XmlWriter xmlWriter = XmlWriter.Create(output, getSettings());

                //do the actual transform of Xml
                xslTrans.Transform(xmlDocument, xmlWriter);

                xmlWriter.Close();
        }

        public XmlWriterSettings getSettings()
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = Indent;
            xmlSettings.OmitXmlDeclaration = OmitXmlDeclaration;
            return xmlSettings;
        }

        public string getStyleSheet(string stylesheetfilename)
        {
            return AppDomain.CurrentDomain.BaseDirectory + Global.Config.getCommonSetting("stylesheetdir", DefaultValues.StylesheetDirectory) + stylesheetfilename;
        }

        #endregion
    }
}
