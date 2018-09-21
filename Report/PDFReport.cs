using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Report
{
    public delegate void ReportDetailHandler(DataRow dataRow, DataColumn dataColumn, XmlNode rowNode);
    /// <summary>
    /// Base class for all Reports.
    /// </summary>
    public abstract class PDFReport
    {
        public const int PORTRAIT_WIDTH = 500;
        public const int LANDSCAPE_WIDTH = 770;
        protected event ReportDetailHandler detailRow = null;
        protected HttpSessionState _session;
        protected LanguageMapper _map;
        protected XmlDocument _xmlDoc;
        protected XmlNode _rootNode;
        protected XmlNode _imageNode;
        protected string _filename = "";
        protected const string CR = "\r";
        protected const string LF = "\n";
        protected int _pageWidth = PORTRAIT_WIDTH;

        public string PDFFilename{
            get{ return _filename + ".pdf";}
        }

        protected void init(HttpSessionState session)
        {
            _session = session;
            _map = LanguageMapper.getLanguageMapper(session);
            _xmlDoc = new XmlDocument();
            _xmlDoc.LoadXml("<document></document>");
            _rootNode = _xmlDoc.LastChild;
            _imageNode = _rootNode.AppendChild(createElement("images"));
        }

        protected void setLandscapeOrientation(bool isLandscape){
            _rootNode.Attributes.Append(createAttribute("orientation", isLandscape? "landscape" : "portrait"));
            _pageWidth = isLandscape? LANDSCAPE_WIDTH : PORTRAIT_WIDTH;
        }

        protected void addImageToCatalog(string fileName, string imageName)
        {
            XmlNode nImage = _imageNode.AppendChild(createElement("image"));
            nImage.Attributes.Append(createAttribute("file-name", fileName));
            nImage.Attributes.Append(createAttribute("image-name", imageName));
        }

        protected XmlNode appendStyle(string className, string fontName, int fontSize, string align)
        {
            XmlNode nStyle = _rootNode.AppendChild(createElement("style"));
            nStyle.Attributes.Append(createAttribute("name", className));
            nStyle.Attributes.Append(createAttribute("font-name", fontName));
            nStyle.Attributes.Append(createAttribute("font-size", fontSize.ToString()));
            nStyle.Attributes.Append(createAttribute("align", align));
            return nStyle;
        }

        protected XmlNode createShowImage(string imageName) {
            return createShowImage(imageName, -1);
        }
        
        protected XmlNode createShowImage(string imageName, int width)
        {
            XmlNode nShowImage = createElement("show-image");
            nShowImage.Attributes.Append(createAttribute("image-name", imageName));
            if (width > 0){
                nShowImage.Attributes.Append(createAttribute("scale-width", width.ToString()));
            }
            return nShowImage;
        }

        protected XmlAttribute createAttribute(string attrName, string attrValue)
        {
            XmlAttribute attr = _xmlDoc.CreateAttribute(attrName);
            attr.Value = attrValue;
            
            return attr;
        }

        protected XmlNode createElement(string nodeName)
        {
            return _xmlDoc.CreateElement(nodeName);
        }

        protected XmlNode createPageHeader(string widths, string spaceAfter) {
            return createPageHeader(widths, spaceAfter, -1, -1);
        }
        
        protected XmlNode createPageHeader(string widths, string spaceAfter, int firstPage, int lastPage)
        {
            XmlNode node = createElement("page-header");

            if (widths != "")
                node.Attributes.Append(createAttribute("widths", widths));
            
            if (spaceAfter != "")
                node.Attributes.Append(createAttribute("space-after", spaceAfter));

            if (firstPage > 0){
                node.Attributes.Append(createAttribute("first-page", firstPage.ToString()));
            }

            if (lastPage > 0){
                node.Attributes.Append(createAttribute("last-page", lastPage.ToString()));
            }

            return node;
        }

        protected XmlNode createPageFooter()
        {
            return createElement("page-footer");

            //XmlNode node = createElement("page-footer");

            //node.Attributes.Append(createAttribute("space-before", "5cm"));

            //return node;
        }

        protected XmlNode createTable()
        {
            return createElement("table");
        }

        protected XmlNode createRow()
        {
            return createElement("row");
        }

        protected XmlNode createHeader()
        {
            return createElement("header");
        }

        protected XmlNode createCell()
        {
            return createElement("cell");
        }

        protected XmlNode createBlock()
        {
            return createElement("block");
        }

        protected XmlAttribute createClassAttribute(string styleName)
        {
            return createAttribute("class", styleName);
        }

        protected XmlNode appendTextBlock(string textBlock)
        {
            XmlNode nBlock = _rootNode.AppendChild(createBlock());
            nBlock.InnerText = textBlock;
            return nBlock;
        }

        /// <summary>
        /// Adds a text to a table-cell. If the text contains line-feeds a nested table will be created.
        /// </summary>
        /// <param name="cell">Table-cell, where the text will be added.</param>
        /// <param name="text">The text to add.</param>
        protected void addCellText(XmlNode cell, string text){
            int size = 1;
            int pos = text.IndexOf(LF, 0);
            while (pos >= 0){
                size++;
                pos = text.IndexOf(LF, pos+1);
            }

            if (size > 1){
                XmlNode nTable, nRow, nCell;
                nTable = cell.AppendChild(createTable());
                nTable.Attributes.Append(createAttribute("padding-all", "0"));
                int start = 0;
                int i = 0;
                pos = text.IndexOf(LF, start);
                while (pos >= 0){
                    nRow = nTable.AppendChild(createRow());
                    nCell = nRow.AppendChild(createCell());
                    nCell.InnerText = text.Substring(start, pos-start).Trim();
                    start = pos+1;
                    pos = text.IndexOf(LF, start);
                    i++;
                }
                nRow = nTable.AppendChild(createRow());
                nCell = nRow.AppendChild(createCell());
                nCell.InnerText = text.Substring(start).Trim();
            }
            else{
                cell.InnerText = text;
            }
        }

        protected void addDetail(DBData db, XmlNode nTable, DataTable table) {
            addDetail(db, nTable, table, "");
        }

        protected void addDetail(DBData db, XmlNode nTable, DataTable table, string view) {
            addDetail(db, nTable, table, view, true);
        }

        protected void addDetail(DBData db, XmlNode nTable, DataTable table, string view, bool showIfEmpty) {
            if (table.Rows.Count > 0) 
            {
                DataRow row = table.Rows[0];
                XmlNode nRow, nCell;
                DBColumn.Visibility visi;
                object dbViews;
                string viewName = view == "" ? table.TableName : view;

                foreach (DataColumn col in table.Columns) 
                {
                    visi = (DBColumn.Visibility) col.ExtendedProperties["Visibility"];
                    dbViews = col.ExtendedProperties["Views"];
                    if (visi >= DBColumn.Visibility.DETAIL && DBColumn.IsInView(view,dbViews) && db.hasColumnAuthorisation(DBData.AUTHORISATION.READ, table, col.ColumnName, true, true)) 
                    {
                        if (showIfEmpty || (row[col] != null && row[col].ToString() != "")){
                            nRow = nTable.AppendChild(createRow());

                            nCell = nRow.AppendChild(createCell());
                            nCell.Attributes.Append(createClassAttribute("detailName"));
                            nCell.InnerText = _map.get(viewName, col.ColumnName);

                            nCell = nRow.AppendChild(createCell());
                            nCell.Attributes.Append(createClassAttribute("detailValue"));
                            string detailValue = "";
                            if (col.ExtendedProperties["In"] != null) 
                                detailValue = DBColumn.LookupIn(col,row[col],false);
                            else
                                detailValue = db.GetDisplayValue(col,row[col],false);
                            if (col.ExtendedProperties["Unit"] != null && detailValue != "")
                            {
                                detailValue += " " + col.ExtendedProperties["Unit"];
                            }
                        
                            addCellText(nCell, detailValue);
                            onDetailRow(row,col,nRow);
                        }
                    }
                }
            }
        }

        private void onDetailRow(DataRow dataRow, DataColumn dataColumn, XmlNode rowNode) 
        {
            if (detailRow != null) 
            {
                // Invokes the delegates. 
                detailRow(dataRow, dataColumn, rowNode);
            }
        }


        /// <summary>
        /// Vertical space in report
        /// </summary>
        /// <param name="space">space</param>
        protected void appendVSpace(int space) 
        {
            XmlNode nBlock = _rootNode.AppendChild(createBlock());
            nBlock.Attributes.Append(createAttribute("space-after", space.ToString()));
            nBlock.InnerText = " ";
        }
        
        /// <summary>
        /// Draw horizontal line
        /// </summary>
        /// <param name="length">Line length</param>
        protected void addHLine(XmlNode node, int length) 
        {
            XmlNode nGraphic = node.AppendChild(createElement("graphic"));
            XmlNode nBoxes = nGraphic.AppendChild(createElement("boxes"));
            nBoxes.Attributes.Append(createAttribute("box-width",length.ToString()));
            nBoxes.Attributes.Append(createAttribute("box-height","1"));
            nBoxes.Attributes.Append(createAttribute("number","1"));
            nBoxes.Attributes.Append(createAttribute("line-width","1"));
        }
        
        protected void appendHLine(int length)
        {
            addHLine(_rootNode, length);
        }

        /// <summary>
        /// End of report, the pdf file will be generated.
        /// </summary>
        public bool saveReport(string outputDirectory, string pdfName) 
        {
            _filename = pdfName + "_" + SessionData.getSessionID(_session);

            string pdfFileName = outputDirectory + "/" + _filename + ".pdf";
            string xmlFileName = outputDirectory + "/" + _filename + ".xml";
            _xmlDoc.Save(xmlFileName);
            
            xmlpdf.licensing.Generator.setRuntimeKey("7BA376EB0DE0301372636518B401115DJPZT+ODUV3SK4ZMIDYRSRW==");
            xmlpdf.PDFDocument converter = new xmlpdf.PDFDocument();

            //ibex4.licensing.Generator.setRuntimeKey("7BA376EB0DE0301372636518B401115DJPZT+ODUV3SK4ZMIDYRSRW==");
            //ibex4.FODocument foconvert = new ibex4.FODocument();
            
            try 
            {
                converter.ValidationType = System.Xml.ValidationType.DTD;
                converter.generate(xmlFileName, pdfFileName);

                //foconvert.generate(xmlFileName, pdfFileName);

                //File.Delete(xmlFileName);
                return true;
            }
            catch (Exception ex) 
            {
                Logger.Log(ex,Logger.ERROR);
                return false;
            }
        }
    }
}
