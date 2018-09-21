using ch.appl.psoft.db;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Report
{
    /// <summary>
    /// Summary description for PsoftPDFReport.
    /// </summary>
    public class PsoftPDFReport : PDFReport
	{
        protected DBData _db;
        protected string _imageDirectory;

        public PsoftPDFReport(HttpSessionState Session, string imageDirectory)
        {
            _db = DBData.getDBData(Session);
            _imageDirectory = imageDirectory + "\\";
            init(Session);
            addImageToCatalog(_imageDirectory + ReportModule.headerLogoImage, "headerLogo");
            appendStyle(".pageHeader", "helvetica-bold", 18, "left");
            appendStyle(".pageFooter", "helvetica-bold", 10, "left");
            appendStyle(".normal", "helvetica", 10, "left");
            appendStyle(".sectionHeader", "helvetica-bold", 14, "left");
            appendStyle(".title", "helvetica-bold", 14, "left").Attributes.Append(createAttribute("fill-color", "lightgray"));
            appendStyle(".subTitle", "helvetica-bold", 11, "left");
            appendStyle(".detailName", "helvetica-bold", 10, "left");
            appendStyle(".detailValue", "helvetica", 10, "left");
        }

        protected virtual XmlNode CreateTable(string widths){
            XmlNode nTable = createTable();
            nTable.Attributes.Append(createAttribute("widths", widths));
            nTable.Attributes.Append(createAttribute("keep-together", "false"));
            nTable.Attributes.Append(createAttribute("border-width-inner", "0"));
            nTable.Attributes.Append(createAttribute("border-width-outer", "0"));
            nTable.Attributes.Append(createAttribute("padding-all", "2"));
            //nTable.Attributes.Append(createAttribute("padding-all", "0"));
            nTable.Attributes.Append(createAttribute("align", "left"));
            return nTable;
        }

        protected virtual XmlNode AppendTable(string widths){
            return _rootNode.AppendChild(CreateTable(widths));
        }

        protected virtual XmlNode CreateDetailTable(){
            return CreateTable("120,400");
        }

        protected virtual XmlNode AppendDetailTable(){
            return _rootNode.AppendChild(CreateDetailTable());
        }

        protected void writeHeaderAndFooter(string header, string footer) 
        {
            // get logo orientation from config, default: left / 21.10.09 / mkr
            string orientation = ch.appl.psoft.Performance.PerformanceModule.getLogoOrientation;

            XmlNode nHeader;
            if (orientation != "right")
            {
                nHeader = _rootNode.AppendChild(createPageHeader("70,*", "0.5cm"));
            }
            else
            {
                nHeader = _rootNode.AppendChild(createPageHeader("*,70", "0.5cm"));
            }

            nHeader.Attributes.Append(createClassAttribute("pageHeader"));
            XmlNode nRow = nHeader.AppendChild(createRow());
            nRow.Attributes.Append(createAttribute("vertical-align", "bottom"));

            XmlNode nCell;
            if (orientation != "right")
            {
                nCell = nRow.AppendChild(createCell());
                nCell.AppendChild(createShowImage("headerLogo"));
                nCell = nRow.AppendChild(createCell());
                nCell.InnerText = header;
                // Logo breite Beschränkung aus
                //nCell.Attributes.Append(createAttribute("padding-left", "20"));
            }
            else
            {
                nCell = nRow.AppendChild(createCell());
                nCell.InnerText = header;
                nCell = nRow.AppendChild(createCell());
                nCell.AppendChild(createShowImage("headerLogo"));
                // Logo breite Beschränkung aus
                //nCell.Attributes.Append(createAttribute("padding-left", "20"));
            }
            
            nRow = nHeader.AppendChild(createRow());
            nRow.Attributes.Append(createAttribute("padding-bottom", "10"));
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createAttribute("padding-top", "20"));
            addHLine(nCell, 520);

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
    }
}
