using ch.appl.psoft.Report;
using System;
using System.Collections;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Training
{
    /// <summary>
    /// Summary description for TrainingCatalogReport.
    /// </summary>
    public class TrainingCatalogReport : PsoftPDFReport
    {
        private XmlNode _nTable;
        private XmlNode _nRow;
        private XmlNode _nCell;

        public TrainingCatalogReport(HttpSessionState Session, string imageDirectory) : base(Session, imageDirectory) {}

        public XmlNode AppendAdvancementTable()
        {
            return AppendTable("420,100");
        }

        public void createReport()
        {
            Interface.DBObjects.Tree dbTree = _db.Tree("TRAININGGROUP");
            Hashtable groupNames = new Hashtable();
            
            writeHeaderAndFooter(_map.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_REP_TRAININGCATALOG), DateTime.Now.ToString("d"));

            long rootID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "TRAININGGROUP", "PARENT_ID is null", false), -1L);
            createGroup(rootID, "");

            appendVSpace(20);
            appendHLine(520);
        }

        private void createGroup(long groupID, String groupTitle) {
            DataTable advancementTable = _db.getDataTableExt("select * from TRAINING where TRAININGGROUP_ID=" + groupID + "order by ORDNUMBER, ID", "TRAINING");
            if (advancementTable.Rows.Count > 0) 
            {
                appendVSpace(20);
                _nTable = AppendAdvancementTable();
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createClassAttribute("title"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("colspan", "2"));
                _nCell.InnerText = groupTitle;
                
                int rowCnt = -1;
                foreach (DataRow elemRow in advancementTable.Rows)
                {
                    rowCnt++;
                    appendVSpace(8);
                    if (rowCnt > 0)
                    {
                        appendVSpace(8);
                    }                   
                    _nTable = AppendAdvancementTable();

                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("subTitle"));
                    _nCell.InnerText = ch.psoft.Util.Validate.GetValid(elemRow[_db.langAttrName(elemRow.Table.TableName, "TITLE")].ToString(), "");

                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    addCellText(_nCell, ch.psoft.Util.Validate.GetValid(elemRow[_db.langAttrName(elemRow.Table.TableName, "DESCRIPTION")].ToString(), ""));

                    String trainingID = ch.psoft.Util.Validate.GetValid(elemRow[_db.langAttrName(elemRow.Table.TableName, "ID")].ToString(), "");
                    DataTable detailTable = _db.getDataTableExt("select VALID_FROM, VALID_TO, COST_EXTERNAL, COST_INTERNAL, LOCATION, INSTRUCTOR from TRAINING where ID =" + trainingID, "TRAINING");

                    appendVSpace(10);
                    _nTable = AppendDetailTable();
                    addDetail(_db, _nTable, detailTable);
                    
                }
            }
            DataTable table = _db.getDataTable("select * from TRAININGGROUP where PARENT_ID=" + groupID + " order by ORDNUMBER, ID", "TRAININGGROUP");            
            foreach (DataRow row in table.Rows) {
                long trainingGroupID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1L);
                string trainingGroupName = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "TITLE")].ToString(), "");
                if (groupTitle != "") {
                    trainingGroupName = groupTitle + "/" + trainingGroupName;
                }
                createGroup(trainingGroupID, trainingGroupName);
            }
        }
    }


}
