using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for SkillsCatalogReport.
    /// </summary>
    public class SkillsCatalogReport : PsoftPDFReport {
        private XmlNode _nTable;
        private XmlNode _nRow;
        private XmlNode _nCell;

        public SkillsCatalogReport(HttpSessionState Session, string imageDirectory) : base(Session, imageDirectory) {}

        public XmlNode AppendAdvancementTable() {
            return AppendTable("420,100");
        }

        public void createReport() {
            Interface.DBObjects.Tree dbTree = _db.Tree("SKILLGROUP");
            
            writeHeaderAndFooter(_map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_SKILLSCATALOG), DateTime.Now.ToString("d"));

            long rootID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "SKILLGROUP", "PARENT_ID is null", false), -1L);
            createGroup(rootID, "");

            appendVSpace(20);
            appendHLine(520);
        }

        private void createGroup(long groupID, String groupTitle) {
            DataTable skillTable = _db.getDataTableExt("select * from SKILL_VALIDITY_V where SKILLGROUP_ID=" + groupID + " and (VALID_FROM<=GetDate() and (VALID_TO>=GetDate() or VALID_TO is null)) order by ORDNUMBER, ID", "SKILL_VALIDITY_V");
            if (skillTable.Rows.Count > 0) {
                appendVSpace(20);
                _nTable = AppendAdvancementTable();
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createClassAttribute("title"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("colspan", "2"));
                _nCell.InnerText = groupTitle;
                
                int rowCnt = -1;
                foreach (DataRow skillRow in skillTable.Rows) {
                    rowCnt++;
                    appendVSpace(8);
                    if (rowCnt > 0) {
                        appendVSpace(8);
                    }                   
                    _nTable = AppendAdvancementTable();

                    if (SkillsModule.showNumTitleInReport){
                        _nRow = _nTable.AppendChild(createRow());
                        _nCell = _nRow.AppendChild(createCell());
                        _nCell.Attributes.Append(createClassAttribute("subTitle"));
                        _nCell.InnerText = ch.psoft.Util.Validate.GetValid(skillRow["NUMBER"].ToString(), "") + " " + ch.psoft.Util.Validate.GetValid(skillRow[_db.langAttrName(skillRow.Table.TableName, "TITLE")].ToString(), "");
                    }
                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    addCellText(_nCell, ch.psoft.Util.Validate.GetValid(skillRow[_db.langAttrName(skillRow.Table.TableName, "DESCRIPTION")].ToString(), ""));

                    String skillValidityID = ch.psoft.Util.Validate.GetValid(skillRow[_db.langAttrName(skillRow.Table.TableName, "ID")].ToString(), "");
                    DataTable detailTable = _db.getDataTableExt("select VALID_FROM, VALID_TO from SKILL_VALIDITY_V where ID =" + skillValidityID, "SKILL_VALIDITY_V");

                    _nTable = AppendDetailTable();
                    addDetail(_db, _nTable, detailTable);
                }
            }
            DataTable table = _db.getDataTable("select * from SKILLGROUP where PARENT_ID=" + groupID + " order by ORDNUMBER, ID", "SKILLGROUP");
            foreach (DataRow row in table.Rows) {
                long skillGroupID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1L);
                string skillGroupTitle = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "TITLE")].ToString(), "");
                if (groupTitle != "") {
                    skillGroupTitle = groupTitle + "/" + skillGroupTitle;
                }
                createGroup(skillGroupID, skillGroupTitle);
            }
        }
    }


}
