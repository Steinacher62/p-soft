using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Summary description for TasklistMeasureReport.
    /// </summary>
    public class TasklistMeasureReport : PDFReport
	{
        protected DBData _db;
        protected string _imageDirectory;
        protected ReportDetailHandler _tasklistDetailHandler;
        protected ReportDetailHandler _measureDetailHandler;
        protected long _currentTasklistID = -1;
        protected long _currentMeasureID = -1;
        private HttpSessionState session;

        public TasklistMeasureReport(HttpSessionState Session, string imageDirectory)
        {
            _db = DBData.getDBData(Session);
            session = Session;
            _imageDirectory = imageDirectory + "/";
            _tasklistDetailHandler = new ReportDetailHandler(onTasklistAddRow);
            _measureDetailHandler = new ReportDetailHandler(onMeasureAddRow);
            init(Session);
            addImageToCatalog(_imageDirectory + ReportModule.headerLogoImage, "headerLogo");
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

        public void createTasklistReport(int tasklistID)
        {
            writeHeaderAndFooter(_map.get("tasklist", "tasklistdetail"), DateTime.Now.ToString("d"));
            appendVSpace(20);
            appendTasklistDetail(tasklistID, false);

            appendVSpace(20);
            XmlNode nBlock = appendTextBlock(_map.get("tasklist", "measures"));
            nBlock.Attributes.Append(createClassAttribute("title"));

			int showsubs = 0;
			if(SessionData.showSubMeasures(session)) showsubs = 1;

            string sql = "select ID from MEASURE where TASKLIST_ID in (select * from get_tasklist(" + tasklistID + "," + showsubs + "))";
            DataTable table = _db.getDataTable(sql);

            foreach (DataRow row in table.Rows)
            {
                long measureID = long.Parse(row[0].ToString());
                if (_db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "MEASURE", measureID, true, true))
                {
                    appendVSpace(10);
                    appendMeasureDetail(measureID, true);;
                }
            }
        }

        public void createMeasureReport(int measureID)
        {
        }

        protected void writeHeaderAndFooter(string header, string footer) 
        {
            XmlNode nHeader = _rootNode.AppendChild(createPageHeader("70,*", "0.5cm"));
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

        protected bool appendTasklistDetail(int tasklistID, bool border)
        {
            bool retValue = true;

            _currentTasklistID = tasklistID;

            XmlNode nTable = _rootNode.AppendChild(createTable());
            nTable.Attributes.Append(createAttribute("widths", "100,400"));
            nTable.Attributes.Append(createAttribute("keep-together", "true"));
            if (border)
            {
                nTable.Attributes.Append(createAttribute("border-width-inner", "0.2"));
                nTable.Attributes.Append(createAttribute("border-width-outer", "1"));
            }
            else
            {
                nTable.Attributes.Append(createAttribute("border-width-inner", "0"));
                nTable.Attributes.Append(createAttribute("border-width-outer", "0"));
            }
            nTable.Attributes.Append(createAttribute("padding-all", "2"));
            nTable.Attributes.Append(createAttribute("align", "left"));

            
            try
            {
                _db.connect();
                string columnOrder = "ID, TITLE, TYPE, DESCRIPTION, AUTHOR_PERSON_ID, CREATIONDATE, STARTDATE, DUEDATE, TEMPLATE";
                DataTable table = _db.getDataTableExt("select " + columnOrder + " from TASKLIST where ID=" + tasklistID, "TASKLIST");
                table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                detailRow += _tasklistDetailHandler;
                addDetail(_db, nTable, table, "TASKLIST_DETAIL", true);
                detailRow -= _tasklistDetailHandler;
            }
            catch (Exception ex)
            {
                Logger.Log(ex,Logger.ERROR);
            }
            finally
            {
                _db.disconnect();
            }

            return retValue;
        }

        protected void onTasklistAddRow(DataRow dataRow, DataColumn dataColumn, XmlNode rowNode)
        {
            if (dataColumn.ColumnName == "TITLE")
            {
                XmlNode nCellClone = rowNode.ChildNodes[1].Clone();
                rowNode.ChildNodes[1].InnerText = "";
                XmlNode nTable = rowNode.ChildNodes[1].AppendChild(createTable());
                nTable.Attributes.Append(createAttribute("widths", "15,*"));
                XmlNode nRow = nTable.AppendChild(createRow());
                XmlNode nCell = nRow.AppendChild(createCell());
                string className = "ampelGrau";
                switch (_db.Tasklist.getSemaphore(_currentTasklistID,true))
                {
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
                nCell.AppendChild(createShowImage(className));
                nRow.AppendChild(nCellClone);
            }
        }

        protected bool appendMeasureDetail(long measureID, bool border)
        {
            bool retValue = true;

            _currentMeasureID = measureID;

            XmlNode nTable = _rootNode.AppendChild(createTable());
            nTable.Attributes.Append(createAttribute("widths", "100,400"));
            nTable.Attributes.Append(createAttribute("keep-together", "true"));
            if (border)
            {
                nTable.Attributes.Append(createAttribute("border-width-inner", "0.2"));
                nTable.Attributes.Append(createAttribute("border-width-outer", "1"));
            }
            else
            {
                nTable.Attributes.Append(createAttribute("border-width-inner", "0"));
                nTable.Attributes.Append(createAttribute("border-width-outer", "0"));
            }
            nTable.Attributes.Append(createAttribute("padding-all", "2"));
            nTable.Attributes.Append(createAttribute("align", "left"));

            try
            {
                _db.connect();
                string columnOrder = "ID, TITLE, STATE, NUMMER, DESCRIPTION, AUTHOR_PERSON_ID, RESPONSIBLE_PERSON_ID, CREATIONDATE, STARTDATE, DUEDATE, TEMPLATE, TASKLIST_ID";
                DataTable table = _db.getDataTableExt("select " + columnOrder + " from MEASURE where ID=" + measureID + (SessionData.showDoneMeasures(session)? "" : " and STATE=0"), "MEASURE");
                
                DataTable personTable = _db.Person.getWholeNameMATable(true);
                table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = personTable;
                table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["In"] = personTable;
                table.Columns["TASKLIST_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

//                ArrayList states = new ArrayList(_map.getEnum("tasklist", "state", true));
//                table.Columns["STATE"].ExtendedProperties["In"] = states;
                table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                detailRow += _measureDetailHandler;
                addDetail(_db, nTable, table, "MEASURE_DETAIL", true);
                detailRow -= _measureDetailHandler;
            }
            catch (Exception ex)
            {
                Logger.Log(ex,Logger.ERROR);
            }
            finally
            {
                _db.disconnect();
            }

            return retValue;
        }

        protected void onMeasureAddRow(DataRow dataRow, DataColumn dataColumn, XmlNode rowNode)
        {
            if (dataColumn.ColumnName == "TITLE")
            {
                XmlNode nCellClone = rowNode.ChildNodes[1].Clone();
                rowNode.ChildNodes[1].InnerText = "";
                XmlNode nTable = rowNode.ChildNodes[1].AppendChild(createTable());
                nTable.Attributes.Append(createAttribute("widths", "15,*"));
                XmlNode nRow = nTable.AppendChild(createRow());
                XmlNode nCell = nRow.AppendChild(createCell());
                string className = "ampelGrau";
                switch (_db.Measure.getSemaphore(_currentMeasureID, _db.Tasklist.getCriticalDays(DBColumn.GetValid(dataRow["TASKLIST_ID"], -1L))))
                {
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
                nCell.AppendChild(createShowImage(className));
                nRow.AppendChild(nCellClone);
            }
        }

    }
}
