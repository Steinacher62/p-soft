using ch.appl.psoft.db;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Project
{
    public class ProjectReport : PDFReport {
        protected DBData _db;
        protected string _imageDirectory;
        protected ReportDetailHandler _projectDetailHandler;
        protected long _projectID = -1;
        private HttpRequest _request;

        public ProjectReport(HttpSessionState Session, HttpRequest Request, string imageDirectory) {
            _db = DBData.getDBData(Session);
            _request = Request;
            _imageDirectory = imageDirectory + "/";
            _projectDetailHandler = new ReportDetailHandler(onProjectAddRow);
            init(Session);
            setLandscapeOrientation(true);
            addImageToCatalog(_imageDirectory + ReportModule.headerLogoImage, "headerLogo");
            addImageToCatalog(_imageDirectory + "ampelBlau.gif", "ampelBlau");
            addImageToCatalog(_imageDirectory + "ampelGrau.gif", "ampelGrau");
            addImageToCatalog(_imageDirectory + "ampelGruen.gif", "ampelGruen");
            addImageToCatalog(_imageDirectory + "ampelOrange.gif", "ampelOrange");
            addImageToCatalog(_imageDirectory + "ampelRot.gif", "ampelRot");
            appendStyle(".pageHeader", "helvetica-bold", 18, "left");
            appendStyle(".pageFooter", "helvetica-bold", 10, "left");
            appendStyle(".normal", "helvetica", 10, "left");
            appendStyle(".title", "helvetica-bold", 12, "left");
            appendStyle(".detailName", "helvetica-bold", 10, "left");
            appendStyle(".detailValue", "helvetica", 10, "left");
        }

        public void createReport(long projectID) {
            _projectID = projectID;
            _db.connect();
            try {
                addScoreCardToCatalog();
                writeHeaderAndFooter(_map.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_REPORT_HEADER), DateTime.Now.ToString("d"));
                appendVSpace(20);
                appendProjectDetail(false);

                appendScoreCard();
                appendVSpace(10);

                appendMeasures("", _db.lookup("TASKLIST_ID", "PROJECT", "ID=" + projectID, -1L), true);

                string sql = "select * from PHASE where PROJECT_ID=" + _projectID;
                DataTable table = _db.getDataTableExt(sql, "PHASE");

                foreach (DataRow row in table.Rows) {
                    if (_db.hasRowAuthorisation(DBData.AUTHORISATION.READ, table, row, true, true)) {
                        appendPhaseDetail(row, true);
                    }
                }
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                _db.disconnect();
            }
        }

        protected void addScoreCardToCatalog(){
            ScoreCard scoreCard = new ScoreCard(_db, _map, _projectID, "");
            addImageToCatalog(ScoreCardImage.SaveImage(scoreCard.GetImage(), _projectID, _session, _request), "scoreCard");
        }

        protected void writeHeaderAndFooter(string header, string footer) {
            XmlNode nHeader = _rootNode.AppendChild(createPageHeader("70,*", "0.5cm", -1, 1));
            nHeader.Attributes.Append(createClassAttribute("pageHeader"));
            XmlNode nRow = nHeader.AppendChild(createRow());
            nRow.Attributes.Append(createAttribute("vertical-align","bottom"));
            XmlNode nCell = nRow.AppendChild(createCell());
            nCell.AppendChild(createShowImage("headerLogo"));
            nCell = nRow.AppendChild(createCell());
            nCell.InnerText = header;
            nCell.Attributes.Append(createAttribute("padding-left", "20"));

            nRow = nHeader.AppendChild(createRow());
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createAttribute("padding-top", "20"));
            addHLine(nCell, _pageWidth);

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

        protected bool appendProjectDetail(bool border) {
            bool retValue = true;

            XmlNode nTable = _rootNode.AppendChild(createTable());
            nTable.Attributes.Append(createAttribute("widths", "120,400"));
            nTable.Attributes.Append(createAttribute("keep-together", "true"));
            if (border) {
                nTable.Attributes.Append(createAttribute("border-width-inner", "0.2"));
                nTable.Attributes.Append(createAttribute("border-width-outer", "1"));
            }
            else {
                nTable.Attributes.Append(createAttribute("border-width-inner", "0"));
                nTable.Attributes.Append(createAttribute("border-width-outer", "0"));
            }
            nTable.Attributes.Append(createAttribute("padding-all", "2"));
            nTable.Attributes.Append(createAttribute("align", "left"));

            
            string columnOrder = "ID, TITLE, STARTDATE, DUEDATE, SPEC_MODIFY_DATE";
            DataTable table = _db.getDataTableExt("select " + columnOrder + " from PROJECT where ID=" + _projectID, "PROJECT");
            table.Columns["SPEC_MODIFY_DATE"].ExtendedProperties["Views"] = "PROJECT";
            detailRow += _projectDetailHandler;
            addDetail(_db, nTable, table, "PROJECT", true);
            detailRow -= _projectDetailHandler;

            long[] personIDs = _db.Project.getLeaderPersonIDs(_projectID);
            if (personIDs.Length > 0){
                XmlNode nRow = nTable.AppendChild(createRow());

                XmlNode nCell = nRow.AppendChild(createCell());
                nCell.Attributes.Append(createClassAttribute("detailName"));
                nCell.InnerText = _map.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_LEADERS);

                nCell = nRow.AppendChild(createCell());
                nCell.Attributes.Append(createClassAttribute("detailValue"));
                string detailValue = "";
                bool isFirst = true;
                foreach(long personID in personIDs){
                    if (isFirst){
                        isFirst = false;
                    }
                    else{
                        detailValue += ", ";
                    }
                    detailValue += _db.Person.getWholeName(personID);
                }
                addCellText(nCell, detailValue);
            }

            return retValue;
        }

        protected void appendScoreCard(){
            XmlNode nImage = createShowImage("scoreCard", _pageWidth);
            nImage.Attributes.Append(createAttribute("space-before", "5"));
            _rootNode.AppendChild(nImage);
        }

        protected void onProjectAddRow(DataRow dataRow, DataColumn dataColumn, XmlNode rowNode) {
            if (dataColumn.ColumnName == "TITLE") {
                XmlNode nCellClone = rowNode.ChildNodes[1].Clone();
                rowNode.ChildNodes[1].InnerText = "";
                XmlNode nTable = rowNode.ChildNodes[1].AppendChild(createTable());
                nTable.Attributes.Append(createAttribute("widths", "15,*"));
                XmlNode nRow = nTable.AppendChild(createRow());
                XmlNode nCell = nRow.AppendChild(createCell());
                string className = "ampelGrau";
                switch (_db.Project.getSemaphore(_projectID, true)) {
                    case 0:
                        className = "ampelRot";
                        break;
                    case 1:
                        className = "ampelOrange";
                        break;
                    case 2:
                        className = "ampelGruen";
                        break;
                    case 3:
                        className = "ampelGrau";
                        break;
                    case 4:
                        className = "ampelBlau";
                        break;
                }
                nCell.AppendChild(createShowImage(className));
                nRow.AppendChild(nCellClone);
            }
        }

        protected bool appendPhaseDetail(DataRow row, bool border) {
            bool retValue = true;

            string title = DBColumn.GetValid(row["TITLE"], "");
//            XmlNode nTable = _rootNode.AppendChild(createTable());
//            nTable.Attributes.Append(createAttribute("widths", "100,400"));
//            nTable.Attributes.Append(createAttribute("keep-together", "true"));
//            if (border) {
//                nTable.Attributes.Append(createAttribute("border-width-inner", "0.2"));
//                nTable.Attributes.Append(createAttribute("border-width-outer", "1"));
//            }
//            else {
//                nTable.Attributes.Append(createAttribute("border-width-inner", "0"));
//                nTable.Attributes.Append(createAttribute("border-width-outer", "0"));
//            }
//            nTable.Attributes.Append(createAttribute("padding-all", "2"));
//            nTable.Attributes.Append(createAttribute("align", "left"));
//
            appendMeasures(title, DBColumn.GetValid(row["TASKLIST_ID"], -1L), border);

            return retValue;
        }

        private void appendCell(XmlNode nRow, string text){
            appendCell(nRow, text, createClassAttribute("detailValue"));
        }
        
        private void appendCell(XmlNode nRow, string text, XmlAttribute attribute){
            XmlNode nCell = createCell();
            nRow.AppendChild(nCell);
            nCell.InnerText = text;
            nCell.Attributes.Append(attribute);
        }

        protected bool appendMeasures(string phaseTitle, long tasklistID, bool border){
            bool retValue = true;

            DataTable table = _db.getDataTableExt("select * from MEASURE where TASKLIST_ID=" + tasklistID + " and STATE=0", "MEASURE");
            if (table.Rows.Count > 0){
                if (phaseTitle != ""){
                    XmlNode nBlock = appendTextBlock(phaseTitle);
                    nBlock.Attributes.Append(createClassAttribute("title"));
                    appendVSpace(10);
                }

                XmlNode nTable = _rootNode.AppendChild(createTable());
                nTable.Attributes.Append(createAttribute("widths", "110,50,*,110,110,70,20"));
                if (border) {
                    nTable.Attributes.Append(createAttribute("border-width-inner", "0.2"));
                    nTable.Attributes.Append(createAttribute("border-width-outer", "1"));
                }
                else {
                    nTable.Attributes.Append(createAttribute("border-width-inner", "0"));
                    nTable.Attributes.Append(createAttribute("border-width-outer", "0"));
                }
                nTable.Attributes.Append(createAttribute("padding-all", "2"));
                nTable.Attributes.Append(createAttribute("align", "left"));

                XmlNode nRow = createRow();
                nTable.AppendChild(nRow);
                appendCell(nRow, _map.get("MEASURE", "TITLE"), createClassAttribute("detailName"));
                appendCell(nRow, _map.get("MEASURE", "NUMMER"), createClassAttribute("detailName"));
                appendCell(nRow, _map.get("MEASURE", "DESCRIPTION"), createClassAttribute("detailName"));
                appendCell(nRow, _map.get("MEASURE", "AUTHOR_PERSON_ID"), createClassAttribute("detailName"));
                appendCell(nRow, _map.get("MEASURE", "RESPONSIBLE_PERSON_ID"), createClassAttribute("detailName"));
                appendCell(nRow, _map.get("MEASURE", "DUEDATE"), createClassAttribute("detailName"));
                appendCell(nRow, "", createClassAttribute("detailName"));

                int criticalDays = _db.Tasklist.getCriticalDays(tasklistID);

                foreach(DataRow row in table.Rows){
                    if (_db.hasRowAuthorisation(DBData.AUTHORISATION.READ, table, row, true, true)) {
                        nRow = createRow();
                        nTable.AppendChild(nRow);
                        
                        appendCell(nRow, DBColumn.GetValid(row["TITLE"], ""));
                        appendCell(nRow, DBColumn.GetValid(row["NUMMER"], ""));
                        appendCell(nRow, DBColumn.GetValid(row["DESCRIPTION"], ""));
                        appendCell(nRow, _db.Person.getWholeName(DBColumn.GetValid(row["AUTHOR_PERSON_ID"], -1L)));
                        appendCell(nRow, _db.Person.getWholeName(DBColumn.GetValid(row["RESPONSIBLE_PERSON_ID"], -1L)));
                        DateTime dueDate = DBColumn.GetValid(row["DUEDATE"], DateTime.MinValue);
                        appendCell(nRow, dueDate > DateTime.MinValue? dueDate.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern, _db.dbColumn.UserCulture.DateTimeFormat) : "");

                        string className = "";
                        switch (_db.Measure.getSemaphore(DBColumn.GetValid(row["ID"], -1L), criticalDays)) {
                            case 0:
                                className = "ampelRot";
                                break;
                            case 1:
                                className = "ampelOrange";
                                break;
                            case 2:
                                className = "ampelGruen";
                                break;
                            case 3:
                                className = "ampelGrau";
                                break;
                        }

                        XmlNode nCell = createCell();
                        nRow.AppendChild(nCell);
                        nCell.AppendChild(createShowImage(className));
                    }
                }
            }
            
            return retValue;
        }

    }
}
