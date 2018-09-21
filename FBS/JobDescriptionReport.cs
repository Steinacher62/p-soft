using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for JobDescriptionReport.
    /// </summary>
    public class JobDescriptionReport : PsoftPDFReport
	{
        protected DataTable _competenceLevels = null;
        protected long _jobID = -1;
        protected long _funktionID = -1;
        protected XmlNode _nTable, _nRow, _nCell;

        public JobDescriptionReport(HttpSessionState Session, string imageDirectory) : base(Session, imageDirectory) {
        }

        public XmlNode AppendDutyTable(){
            return AppendTable("420,100");
        }

        protected string getRestriction()
        {
                return "VALID_FROM<=GetDate() and VALID_TO>=GetDate() and DUTY_VALIDITY_VALID_FROM<=GetDate() and DUTY_VALIDITY_VALID_TO>=GetDate() and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
        }


		bool isFirstGrp = true;

        public void createReport(int jobID)
        {
            _jobID = jobID;
            _funktionID = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "JOB", "ID=" + _jobID, false), -1);
            long personID = ch.psoft.Util.Validate.GetValid(_db.lookup("PERSON_ID", "JOBPERSONV", "ID=" + _jobID, false), -1);

            writeHeaderAndFooter(_map.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_REP_JOBDESCRIPTION), DateTime.Now.ToString("d"));

            // person
            if (personID > 0){
                _nTable = AppendDetailTable();
                _nRow = _nTable.AppendChild(createRow());

                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createClassAttribute("detailName"));
                _nCell.InnerText = _map.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_REP_JOB_OWNER);

                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createClassAttribute("detailValue"));
                _nCell.InnerText = _db.Person.getWholeName(personID.ToString(), false, true, false);
                appendVSpace(10);
            }

            // job detail
            _nTable = AppendDetailTable();
            DataTable table = _db.getDataTableExt("select ID, " + _db.langAttrName("JOB", "TITLE") + ", " + _db.langAttrName("JOB", "DESCRIPTION") + " from JOB where ID=" + _jobID, "JOB");
            addDetail(_db, _nTable, table, "JOB", false);
            appendVSpace(10);
            _nTable = AppendDetailTable();
            table = _db.getDataTableExt("select ID, " + _db.langAttrName("FUNKTION", "TITLE") + ", " + _db.langAttrName("FUNKTION", "DESCRIPTION") + " from FUNKTION where ID=" + _funktionID, "FUNKTION");
            addDetail(_db, _nTable, table, "FUNKTION", false);

            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createAttribute("padding-top", "10"));
            _nCell = _nRow.AppendChild(createCell());
            appendHLine(520);
            appendVSpace(10);

            _nTable = AppendDutyTable();
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createClassAttribute("sectionHeader"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.InnerText = _map.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_REP_DUTY);
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("align", "right"));
            _nCell.InnerText = _map.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_REP_COMPETENCES);

            _competenceLevels = FBSModule.getCompetenceLevels(_db);

			// Neue Version sortiert die Skills dem Skillskatalog entsprechend (Tiefensuche)
			long rootId =  ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "DUTYGROUP", "PARENT_ID is null",""), -1);                   					
			addGroupsDepthFirst(null,rootId);

			/* Alte Version sortiert nach Titel 
            DataTable grpTable = _db.getDataTable("select * from DUTYGROUP order by " + _db.langAttrName("DUTYGROUP", "TITLE"), "DUTYGROUP");

            bool isFirstGrp = true;
            foreach (DataRow grpRow in grpTable.Rows) {
                long dutyGroupID = ch.psoft.Util.Validate.GetValid(grpRow["ID"].ToString(), -1L);

                string sql = "";
                string sqlOrdr = " order by " + _db.langAttrName("DUTY_COMPETENCE__DUTY_VALIDITY_V", "NUM_TITLE");
                DataTable table2 = null;
                if (isFirstGrp)
                {
                    sql = "select * from DUTY_COMPETENCE__DUTY_VALIDITY_V where " + getRestriction() + " and DUTYGROUP_ID is null";
                    sql += sqlOrdr;
                    table2 = _db.getDataTable(sql, "DUTY_COMPETENCE__DUTY_VALIDITY_V");
                    addGroup(table2, ref isFirstGrp, null);
                }

                sql = "select * from DUTY_COMPETENCE__DUTY_VALIDITY_V where " + getRestriction() + " and DUTYGROUP_ID="+dutyGroupID;
                sql += sqlOrdr;
                table2 = _db.getDataTable(sql, "DUTY_COMPETENCE__DUTY_VALIDITY_V");
                addGroup(table2, ref isFirstGrp, grpRow);
            }
			*/

            appendVSpace(20);
            appendHLine(520);
            appendVSpace(20);

            _nTable = AppendTable("20,500");
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createClassAttribute("subTitle"));
            _nRow.Attributes.Append(createAttribute("padding-bottom", "10"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.InnerText = _map.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_REP_COMPETENCES_RULES);
            table = _db.getDataTable("select * from COMPETENCE_LEVEL order by " + _db.langAttrName("COMPETENCE_LEVEL", "MNEMO"), "COMPETENCE_LEVEL");
            foreach (DataRow row in table.Rows) {
                _nRow = _nTable.AppendChild(createRow());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createClassAttribute("detailName"));
                _nCell.InnerText = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "MNEMO")].ToString(), "");

                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createClassAttribute("detailValue"));
                _nCell.InnerText = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "TITLE")].ToString(), "");
            }


            // Schluss hinzugefügt 09.05.14 M.Steinacher

            if (!Global.isModuleEnabled("ahb"))
            {
                _nTable = AppendDetailTable();
                _nTable.Attributes.Append(createAttribute("keep-together", "true"));

                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createAttribute("padding-bottom", "25"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("colspan", "2"));
                _nCell.Attributes.Append(createAttribute("padding-top", "25"));

                //Datum/Visum
                _nTable = AppendDetailTable();
                _nTable.Attributes.Append(createAttribute("keep-together", "true"));
                _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
                _nTable.Attributes.Append(createAttribute("widths", "350,*"));
                _nTable.Attributes.Append(createAttribute("name", ".spzTitle"));
                _nTable.Attributes.Append(createAttribute("font-name", "helvetica"));
                _nTable.Attributes.Append(createAttribute("font-size", 10.ToString()));
                _nTable.Attributes.Append(createAttribute("align", "left"));
                _nRow = _nTable.AppendChild(createRow());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.InnerText = _map.get("fbs", "staffmember");
                _nCell = _nRow.AppendChild(createCell());
                _nCell.InnerText = _map.get("fbs", "datevisa");

                _nRow = _nTable.AppendChild(createRow());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.InnerText = _map.get("fbs", "directchief");
                _nCell = _nRow.AppendChild(createCell());
                _nCell.InnerText = _map.get("fbs", "datevisa");

                _nRow = _nTable.AppendChild(createRow());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.InnerText = _map.get("fbs", "nextchief");
                _nCell = _nRow.AppendChild(createCell());
                _nCell.InnerText = _map.get("fbs", "datevisa");
            }

            if (Global.isModuleEnabled("ahb"))
            {
                XmlNode nTable = createTable();

                nTable.Attributes.Append(createAttribute("widths", "100,80,100,200,100"));
                nTable.Attributes.Append(createAttribute("keep-together", "false"));
                nTable.Attributes.Append(createAttribute("border-width-inner", "0"));
                nTable.Attributes.Append(createAttribute("border-width-outer", "0"));
                nTable.Attributes.Append(createAttribute("padding-all", "2"));
                nTable.Attributes.Append(createAttribute("align", "left"));
                nTable.Attributes.Append(createClassAttribute("detailValue"));
                _nTable.Attributes.Append(createAttribute("font-name", "helvetica"));
  
                _nRow = _nTable.AppendChild(createRow());
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("align", "center"));
                _nCell.InnerText = "Datum";
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("align", "center"));
                _nCell.InnerText = "Name";
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("align", "center"));
                _nCell.InnerText = "Visum";
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createAttribute("padding-bottom", "15"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell.InnerText ="Vorgesetzter";
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createAttribute("padding-bottom", "15"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell.InnerText = "Mitarbeiter";
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
            }
        }

		public void addGroupsDepthFirst(DataRow parentGrp, long parentId)
		{			
			long dutyGroupID = parentId;                    
                            
			string sql = "";
			string sqlOrdr = " order by ORDNUMBER";
			DataTable table2 = null;
			if (parentGrp == null)
			{
				sql = "select DUTY_COMPETENCE__DUTY_VALIDITY_V.*, DUTY.ORDNUMBER from DUTY_COMPETENCE__DUTY_VALIDITY_V INNER JOIN DUTY ON DUTY.ID = DUTY_ID where " + getRestriction() + " and DUTY_COMPETENCE__DUTY_VALIDITY_V.DUTYGROUP_ID is null";
				sql += sqlOrdr;
				table2 = _db.getDataTable(sql, "DUTY_COMPETENCE__DUTY_VALIDITY_V");
				addGroup(table2, ref isFirstGrp, null);
			}

			sql = "select DUTY_COMPETENCE__DUTY_VALIDITY_V.*, DUTY.ORDNUMBER from DUTY_COMPETENCE__DUTY_VALIDITY_V INNER JOIN DUTY ON DUTY.ID = DUTY_ID where " + getRestriction() + " and DUTY_COMPETENCE__DUTY_VALIDITY_V.DUTYGROUP_ID="+dutyGroupID;
			sql += sqlOrdr;
			table2 = _db.getDataTable(sql, "DUTY_COMPETENCE__DUTY_VALIDITY_V");
			addGroup(table2, ref isFirstGrp, parentGrp);
			
			DataTable grpTable = _db.getDataTable("select * from DUTYGROUP where PARENT_ID =" + parentId + " order by ORDNUMBER", "DUTYGROUP");
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
                _nTable = AppendDutyTable();
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
                    _nCell.InnerText = _map.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_REP_DUTYGROUP_FREE);
                }
                appendVSpace(8);
                _nTable = AppendDutyTable();

                foreach (DataRow dcvRow in table.Rows)
                {
                    if (FBSModule.showNumTitleInReport)
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
                    _nCell.Attributes.Append(createAttribute("align", "right"));
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    bool isFirst = true;
                    foreach (DataRow clRow in _competenceLevels.Rows) 
                    {
                        if (ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "COMPETENCE", "DUTY_COMPETENCE_VALIDITY_ID=" + dcvRow["ID"] + " and COMPETENCE_LEVEL_ID=" + clRow["ID"], false), -1) > 0)
                        {
                            if (isFirst)
                            {
                                isFirst = false;
                            }
                            else
                            {
                                _nCell.InnerText += " ";
                            }
                            _nCell.InnerText += clRow[_db.langAttrName(clRow.Table.TableName, "MNEMO")].ToString();
                        }
                    }
                }
            }
        }
    }
}
