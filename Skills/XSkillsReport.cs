using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for XSkillsReport.
    /// </summary>
    public class XSkillsReport : PsoftPDFReport {
        protected long _jobID = -1;
        protected long _personID = -1;
        protected long _funktionID = -1;
        protected XmlNode _nTable, _nRow, _nCell;

        public XSkillsReport(HttpSessionState Session, string imageDirectory) : base (Session, imageDirectory) {}

        public XmlNode AppendSkillTable()
        {
            return AppendTable("420,100");
        }

        protected string getRestriction()
        {
            string retValue = "VALID_FROM<=GetDate() and VALID_TO>=GetDate() and SKILL_VALIDITY_VALID_FROM<=GetDate() and SKILL_VALIDITY_VALID_TO>=GetDate()";
            if (_jobID > 0)
                retValue += " and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
            else if (_personID > 0)
                retValue += " and PERSON_ID=" + _personID;
            return retValue;
        }

		bool isFirstGrp = true;

        public void createReport(int jobID, int personID) {
            DataTable table;
            _jobID = jobID;
            _personID = personID;
            if (_jobID > 0){
                _funktionID = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "JOB", "ID=" + _jobID, false), -1);
                _personID = ch.psoft.Util.Validate.GetValid(_db.lookup("PERSON_ID", "JOBPERSONV", "ID=" + _jobID, false), -1);
                writeHeaderAndFooter(_map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_JOBSKILLS), DateTime.Now.ToString("d"));
            }
            else if (_personID > 0){
                writeHeaderAndFooter(_map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_PERSONSKILLS), DateTime.Now.ToString("d"));
            }

            // person
            if (_personID > 0){
                _nTable = AppendDetailTable();
                _nRow = _nTable.AppendChild(createRow());

                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createClassAttribute("detailName"));
                _nCell.InnerText = _map.get(SkillsModule.LANG_SCOPE_SKILLS, _jobID > 0? SkillsModule.LANG_MNEMO_REP_JOB_OWNER : SkillsModule.LANG_MNEMO_REP_PERSON);

                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createClassAttribute("detailValue"));
                _nCell.InnerText = _db.Person.getWholeName(_personID.ToString(), false, true, false);
                appendVSpace(10);
            }

            // job detail
            if (_jobID > 0){
                _nTable = AppendDetailTable();
                table = _db.getDataTableExt("select ID, " + _db.langAttrName("JOB", "TITLE") + " from JOB where ID=" + _jobID, "JOB");
                addDetail(_db, _nTable, table, "JOB", false);
                appendVSpace(10);
                _nTable = AppendDetailTable();
                table = _db.getDataTableExt("select ID, " + _db.langAttrName("FUNKTION", "TITLE") + " from FUNKTION where ID=" + _funktionID, "FUNKTION");
                addDetail(_db, _nTable, table, "FUNKTION");
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createAttribute("padding-top", "10"));
                _nCell = _nRow.AppendChild(createCell());
            }

            appendHLine(520);

            _nTable = AppendTable("390,130");
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createAttribute("padding-top", "10"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("sectionHeader"));
            _nCell.InnerText = _map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_SKILL);
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("sectionHeader"));
            _nCell.Attributes.Append(createAttribute("align", "right"));
            _nCell.InnerText = _map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_DEMANDLEVEL);

            
            // Neue Version sortiert die Skills dem Skillskatalog entsprechend (Tiefensuche)
			long rootId =  ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "SKILLGROUP", "PARENT_ID is null",""), -1);                   					
			addGroupsDepthFirst(null,rootId);
            
			/* Alte Version sortiert nach Titel 
			 			 
			DataTable grpTable = _db.getDataTable("select * from SKILLGROUP order by " + _db.langAttrName("SKILLGROUP", "TITLE"), "SKILLGROUP");
            foreach (DataRow grpRow in grpTable.Rows) 
            {
                long skillGroupID = ch.psoft.Util.Validate.GetValid(grpRow["ID"].ToString(), -1L);

                string sql = "";
                string sqlOrdr = " order by " + _db.langAttrName("SKILL_LEVEL__SKILL_VALIDITY_V", "NUM_TITLE");
                DataTable table2 = null;
                if (isFirstGrp)
                {
                    sql = "select * from SKILL_LEVEL__SKILL_VALIDITY_V where " + getRestriction() + " and SKILLGROUP_ID is null";
                    sql += sqlOrdr;
                    table2 = _db.getDataTable(sql, "SKILL_LEVEL__SKILL_VALIDITY_V");
                    addGroup(table2, ref isFirstGrp, null);
                }

                sql = "select * from SKILL_LEVEL__SKILL_VALIDITY_V where " + getRestriction() + " and SKILLGROUP_ID="+skillGroupID;
                sql += sqlOrdr;
                table2 = _db.getDataTable(sql, "SKILL_LEVEL__SKILL_VALIDITY_V");
                addGroup(table2, ref isFirstGrp, grpRow);
            }
			*/
        }

		public void addGroupsDepthFirst(DataRow parentGrp, long parentId)
		{
			
			long skillGroupID = parentId;                    
                            
			string sql = "";
			string sqlOrdr = " order by ORDNUMBER";
			DataTable table = null;

			if (parentGrp == null)
			{
				sql = "select SKILL_LEVEL__SKILL_VALIDITY_V.*,SKILL.ORDNUMBER from SKILL_LEVEL__SKILL_VALIDITY_V INNER JOIN SKILL ON SKILL.ID = SKILL_ID where " + getRestriction() + " and SKILL_LEVEL__SKILL_VALIDITY_V.SKILLGROUP_ID is null";
				sql += sqlOrdr;
				table = _db.getDataTable(sql, "SKILL_LEVEL__SKILL_VALIDITY_V");
				addGroup(table, ref isFirstGrp, null);
			}
			
			sql = "select SKILL_LEVEL__SKILL_VALIDITY_V.*,SKILL.ORDNUMBER from SKILL_LEVEL__SKILL_VALIDITY_V INNER JOIN SKILL ON SKILL.ID = SKILL_ID where " + getRestriction() + " and SKILL_LEVEL__SKILL_VALIDITY_V.SKILLGROUP_ID="+skillGroupID;
			sql += sqlOrdr;
			table = _db.getDataTable(sql, "SKILL_LEVEL__SKILL_VALIDITY_V");
			addGroup(table, ref isFirstGrp, parentGrp);		

			DataTable grpTable = _db.getDataTable("select * from SKILLGROUP where PARENT_ID =" + parentId + " order by ORDNUMBER", "SKILLGROUP");
			foreach(DataRow child in grpTable.Rows)
			{
				addGroupsDepthFirst(child, ch.psoft.Util.Validate.GetValid(child["ID"].ToString(), -1L));
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
                        _nCell.InnerText = ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "NUM_TITLE")].ToString(), "");
                    }

                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    addCellText(_nCell, ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "DESCRIPTION")].ToString(), ""));

                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    _nCell.Attributes.Append(createAttribute("align", "right"));
                    _nCell.InnerText = ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "DEMAND_LEVEL_TITLE")].ToString(), "");

                }
            }
        }

    }
}
