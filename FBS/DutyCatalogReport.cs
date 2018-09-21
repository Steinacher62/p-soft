using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for DutyCatalogReport.
    /// </summary>
    public class DutyCatalogReport : PsoftPDFReport {
        private XmlNode _nTable;
        private XmlNode _nRow;
        private XmlNode _nCell;

        public DutyCatalogReport(HttpSessionState Session, string imageDirectory) : base(Session, imageDirectory) {}

        public XmlNode AppendAdvancementTable() {
            return AppendTable("420,100");
        }

        public void createReport() {
            Interface.DBObjects.Tree dbTree = _db.Tree("DUTYGROUP");
            
            writeHeaderAndFooter(_map.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_REP_DUTYCATALOG), DateTime.Now.ToString("d"));

            long rootID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "DUTYGROUP", "PARENT_ID is null", false), -1L);
            createGroup(rootID, "");

            appendVSpace(20);
            appendHLine(520);
        }

        private void createGroup(long groupID, String groupTitle) {
            DataTable dutyTable = _db.getDataTableExt("select * from DUTY_VALIDITY_V where DUTYGROUP_ID=" + groupID + " and (VALID_FROM<=GetDate() and (VALID_TO>=GetDate() or VALID_TO is null)) order by ORDNUMBER, ID", "DUTY_VALIDITY_V");
            if (dutyTable.Rows.Count > 0) {
                appendVSpace(20);
                _nTable = AppendAdvancementTable();
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createClassAttribute("title"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("colspan", "2"));
                _nCell.InnerText = groupTitle;
                
                int rowCnt = -1;
                foreach (DataRow dutyRow in dutyTable.Rows) {
                    rowCnt++;
                    appendVSpace(8);
                    if (rowCnt > 0) {
                        appendVSpace(8);
                    }                   
                    _nTable = AppendAdvancementTable();

                    if (FBSModule.showNumTitleInReport){
                        _nRow = _nTable.AppendChild(createRow());
                        _nCell = _nRow.AppendChild(createCell());
                        _nCell.Attributes.Append(createClassAttribute("subTitle"));
                        _nCell.InnerText = ch.psoft.Util.Validate.GetValid(dutyRow["NUMBER"].ToString(), "") + " " + ch.psoft.Util.Validate.GetValid(dutyRow[_db.langAttrName(dutyRow.Table.TableName, "TITLE")].ToString(), "");
                    }
                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    addCellText(_nCell, ch.psoft.Util.Validate.GetValid(dutyRow[_db.langAttrName(dutyRow.Table.TableName, "DESCRIPTION")].ToString(), ""));

                    String dutyValidityID = ch.psoft.Util.Validate.GetValid(dutyRow[_db.langAttrName(dutyRow.Table.TableName, "ID")].ToString(), "");
                    DataTable detailTable = _db.getDataTableExt("select VALID_FROM, VALID_TO from DUTY_VALIDITY_V where ID =" + dutyValidityID, "DUTY_VALIDITY_V");

                    _nTable = AppendDetailTable();
                    addDetail(_db, _nTable, detailTable);
                }
            }
            DataTable table = _db.getDataTable("select * from DUTYGROUP where PARENT_ID=" + groupID + " order by ORDNUMBER, ID", "DUTYGROUP");
            foreach (DataRow row in table.Rows) {
                long dutyGroupID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1L);
                string dutyGroupTitle = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "TITLE")].ToString(), "");
                if (groupTitle != "") {
                    dutyGroupTitle = groupTitle + "/" + dutyGroupTitle;
                }
                createGroup(dutyGroupID, dutyGroupTitle);
            }
        }
    }


}
