using ch.appl.psoft.db;
using ch.appl.psoft.Report;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// Schreibt und speichert den Lohndatenreport als PDF
    /// 
    /// Bemerkung:
    /// In diesem Report wird eins zu eins dargestellt, was in PersonDetailControl
    /// angezeigt wird. Es handelt sich weitgehend um eine Kopie. (doppelte Arbeit bei der
    /// Überarbeitung!)
    /// </summary>
    public class PersonDetailReport : PDFReport
	{
        protected DBData _db;
        protected double _salaryDivisor = 1;

        public PersonDetailReport(HttpSessionState Session)
        {
            _db = DBData.getDBData(Session);
            init(Session);
            appendStyle(".pageHeader", "helvetica-bold", 18, "left");
            appendStyle(".pageFooter", "helvetica-bold", 10, "left");
            appendStyle(".normal", "helvetica", 10, "left");
            appendStyle(".title", "helvetica-bold", 12, "left");
            appendStyle(".detailName", "helvetica-bold", 10, "left");
            appendStyle(".detailValue", "helvetica", 10, "left");
        }

        protected XmlNode createDetailLabelCell(string innerText)
        {
            return createCell(createClassAttribute("detailName"), innerText, 1, "");
        }

        protected XmlNode createDetailValueCell(string innerText)
        {
            return createDetailValueCell(innerText, 1);
        }

        protected XmlNode createDetailValueCell(string innerText, string align)
        {
            return createDetailValueCell(innerText, 1, align);
        }

        protected XmlNode createDetailValueCell(string innerText, int colspan)
        {
            return createDetailValueCell(innerText, colspan, "");
        }

        protected XmlNode createDetailValueCell(string innerText, int colspan, string align)
        {
            return createCell(createClassAttribute("detailValue"), innerText, colspan, align);
        }

        protected XmlNode createCell(XmlAttribute classAttribute, string innerText, int colspan, string align)
        {
            XmlNode nCell = createCell();
            nCell.Attributes.Append(classAttribute);
            if (colspan > 1){
                nCell.Attributes.Append(createAttribute("colspan",colspan.ToString()));
            }
            if (align != ""){
                nCell.Attributes.Append(createAttribute("align", align));
            }
            nCell.InnerXml = innerText;
            return nCell;
        }

        protected void writeHeaderAndFooter(string header, string footer) 
        {
            XmlNode nHeader = _rootNode.AppendChild(createPageHeader("70,*", "0.5cm"));
            nHeader.Attributes.Append(createClassAttribute("pageHeader"));
            XmlNode nRow = nHeader.AppendChild(createRow());
            nRow.Attributes.Append(createAttribute("vertical-align","bottom"));
            XmlNode nCell = nRow.AppendChild(createCell());
            nCell = nRow.AppendChild(createCell());
            nCell.InnerText = header;
            
            nRow = nHeader.AppendChild(createRow());
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createAttribute("padding-top", "20"));
            addHLine(nCell, 500);

            XmlNode nFooter = _rootNode.AppendChild(createPageFooter());
            nFooter.Attributes.Append(createClassAttribute("pageFooter"));
            nRow = nFooter.AppendChild(createRow());
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createAttribute("align", "left"));
            nCell.InnerText = footer;
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createAttribute("align","right"));
            nCell.InnerXml = _map.get("reportLayout", "page") + " <page-number/>/<forward-reference name='total-pages'/>";
        }

        protected double getPartSalary(double salary) 
        {
            return salary / _salaryDivisor;
        }
    }
}
