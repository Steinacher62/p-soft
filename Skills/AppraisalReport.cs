using ch.appl.psoft.db;
using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for AppraisalReport.
    /// </summary>
    public class AppraisalReport : PsoftPDFReport {
        protected long _appraisalID = -1;
        XmlNode _nTable, _nRow, _nCell;

        public AppraisalReport(HttpSessionState Session, string imageDirectory) : base (Session, imageDirectory) {}

        public XmlNode AppendSkillTable()
        {
            return AppendTable("420,100");
        }

        public void createReport(int appraisalID) {
            _appraisalID = appraisalID;
            long personID = ch.psoft.Util.Validate.GetValid(_db.lookup("PERSON_ID", "SKILLS_APPRAISAL", "ID=" + _appraisalID, false), -1);

            writeHeaderAndFooter(_map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_SKILLSAPPRAISAL).Replace("#1", _db.Person.getWholeName(personID)), DateTime.Now.ToString("d"));

            // overall-rating
            double overallRating = DBColumn.GetValid(_db.lookup("avg(rating_level_percentage)","skill_rating","skills_appraisal_id = " + appraisalID), 0);
            _nTable = AppendDetailTable();
            _nRow = _nTable.AppendChild(createRow());

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailName"));
            _nCell.InnerText = _map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_OVERALL_RATING);

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            _nCell.InnerText = overallRating.ToString() + "%";
            appendVSpace(10);

            appendHLine(520);

            _nTable = AppendTable("390,130");
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createAttribute("padding-top", "10"));
            _nCell = _nRow.AppendChild(createCell());
            _nRow.Attributes.Append(createClassAttribute("sectionHeader"));
            _nCell.InnerText = _map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_SKILL);
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("sectionHeader"));
            _nCell.Attributes.Append(createAttribute("align", "right"));
            _nCell.InnerText = _map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_RATINGLEVEL);

            DataTable grpTable = _db.getDataTable("select * from SKILLGROUP order by " + _db.langAttrName("SKILLGROUP", "TITLE"), "SKILLGROUP");

            bool isFirstGrp = true;
            foreach (DataRow grpRow in grpTable.Rows) 
            {
                long skillGroupID = ch.psoft.Util.Validate.GetValid(grpRow["ID"].ToString(), -1L);                          
                            
                string sql = "";
                string sqlOrdr = " order by SKILL_NUMBER, " + _db.langAttrName("SKILL_RATING", "SKILL_TITLE");
                DataTable table = null;
                if (isFirstGrp)
                {
                    sql = "select * from SKILL_RATING where SKILLS_APPRAISAL_ID=" + _appraisalID + " and SKILLGROUP_ID  is null";
                    sql += sqlOrdr;
                    table = _db.getDataTable(sql, "SKILL_RATING");
                    addGroup(table, ref isFirstGrp, null);
                }

                sql = "select * from SKILL_RATING where SKILLS_APPRAISAL_ID=" + _appraisalID + " and SKILLGROUP_ID="+skillGroupID;
                sql += sqlOrdr;
                table = _db.getDataTable(sql, "SKILL_RATING");
                addGroup(table, ref isFirstGrp, grpRow);

            }
        }

        private void addGroup(DataTable table, ref bool isFirstGrp, DataRow grpRow)
        {
            if (table.Rows.Count > 0) 
            {
                if (isFirstGrp)
                {
                    isFirstGrp = false;
                }
                appendVSpace(20);
                _nTable = AppendSkillTable();
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createClassAttribute("title"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("colspan", "2"));
                if (grpRow != null)
                {
                    _nCell.InnerText = ch.psoft.Util.Validate.GetValid(grpRow[_db.langAttrName(grpRow.Table.TableName, "TITLE")].ToString(), "");
                }
                else 
                {
                    _nCell.InnerText = _map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_SKILLGROUP_FREE);
                }
                appendVSpace(8);
                _nTable = AppendSkillTable();

                foreach (DataRow dcvRow in table.Rows)
                {
                    if (SkillsModule.showNumTitleInReport)
                    {
                        _nRow = _nTable.AppendChild(createRow());
                        _nCell = _nRow.AppendChild(createCell());
                        _nCell.Attributes.Append(createClassAttribute("subTitle"));
                        _nCell.InnerText = ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "SKILL_TITLE")].ToString(), "");
                    }

                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    addCellText(_nCell, ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "DEMAND_LEVEL_TITLE")].ToString(), "") + ": " + ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "SKILL_DESCRIPTION")].ToString(), ""));

                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    _nCell.Attributes.Append(createAttribute("align", "right"));
                    _nCell.InnerText = ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "RATING_LEVEL_TITLE")].ToString(), "");

                }
            }
        }

    }
}
