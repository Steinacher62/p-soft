using ch.appl.psoft.Interface;
using System;
using System.Web.SessionState;
using System.Xml;



namespace ch.appl.psoft.Report
{
    public class XMLReport
    {
        #region Properties

        protected XmlDocument XMLDoc { get; set; }
        protected LanguageMapper Mapper { get; set; }
        protected HttpSessionState Session { get; set; }
        protected string Title { get; set; }

        #endregion

        #region Constructors

        protected XMLReport()
        {
            this.XMLDoc = new XmlDocument();
        }

        protected XMLReport(HttpSessionState session) 
            : this()
        {
            this.Session = session;
            this.Mapper = LanguageMapper.getLanguageMapper(session);
        }

        protected XMLReport(HttpSessionState session, string title)
            : this(session)
        {
            this.Title = title;
        }

        #endregion

        #region Protected Methods

        protected XmlElement appendChild(XmlElement rootelement, string elementname)
        {
            XmlElement child = this.XMLDoc.CreateElement(elementname);
            rootelement.AppendChild(child);
            return child;
        }

        protected XmlElement appendChild(XmlElement rootelement, string elementname, string innertext)
        {
            XmlElement child = appendChild(rootelement, elementname);
            child.InnerText = innertext;
            return child;
        }

        protected XmlElement appendChild(XmlElement rootelement, string elementname, object innertext)
        {
            return appendChild(rootelement, elementname, innertext.ToString());
        }

        protected XmlElement appendChild(XmlElement rootelement, string elementname, string attributename, string attributevalue, string innertext)
        {
            XmlElement child = appendChild(rootelement, elementname, innertext);
            child.SetAttribute(attributename, attributevalue);
            return child;
        }

        protected XmlElement appendChild(XmlElement rootelement, string elementname, string attributename, string attributevalue, object innertext)
        {
            return appendChild(rootelement, elementname, attributename, attributevalue, innertext.ToString());
        }

        #endregion

        static private string getOutputPath(string title)
        {
            return Global.Config.getCommonSetting("tmpdir", DefaultValues.TmpDirectory) + title + "_" + DateTime.Now.ToString("yyMMdd") + ".xml";
        }

        static public string getOutputfileRelativePath(string title)
        {
            return Global.Config.baseURL + getOutputPath(title);
        }

        static public string getOutputfileAbsolutePath(string title)
        {
            return AppDomain.CurrentDomain.BaseDirectory + getOutputPath(title);
        }

        static public string getWebPath(string title)
        {
            return Global.Config.baseURL + getOutputfileRelativePath(title);
        }

    }
}
