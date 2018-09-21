using ch.appl.psoft.db;
using ch.appl.psoft.FBS;
using ch.appl.psoft.Report;
using ch.appl.psoft.Skills;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.SPZ
{
    /// <summary>
    /// SPZ-spezifischer Report
    /// Im Prinzip Stellenbeschreibungs- und Skillsreport
    /// Die Beschriftung der Spalte Kompetenzen bzw Anforderungsniveau
    /// wurden weggelassen, da diese nicht verwendet werden.
    /// Das Drucken der Spalten selbst ist noch vorhanden.
    /// </summary>
    public class JobDescriptionReport : PsoftPDFReport
	{
        protected DataTable _competenceLevels = null;
        protected long _jobID = -1;
        protected long _funktionID = -1;
        protected int _groupNumber = 0;
        protected string _reportDate = "";
        protected XmlNode _nTable, _nRow, _nCell;

        public JobDescriptionReport(HttpSessionState session, string imageDirectory)
            : base(session, imageDirectory)
        {
            // Für SPZ eigene Styles
            appendStyle(".spzPageHeader", "helvetica-bold", 14, "left");
            XmlNode node = appendStyle(".spzMainTitle", "helvetica-bold", 14, "left");
            node.Attributes.Append(createAttribute("fill-color", "#fbf5ad"));
            node.Attributes.Append(createAttribute("border-width-outer", ".5"));
            node.Attributes.Append(createAttribute("padding-top", "6"));
            node.Attributes.Append(createAttribute("padding-bottom", "8"));
            node = appendStyle(".spzTitle", "helvetica-bold", 11, "left");
            node.Attributes.Append(createAttribute("fill-color", "#fbf5ad"));
            node.Attributes.Append(createAttribute("border-width-bottom", ".5"));
            node.Attributes.Append(createAttribute("padding-top", "4"));
            node.Attributes.Append(createAttribute("padding-bottom", "6"));
        }

        public XmlNode AppendDutyTable()
        {
            return AppendTable("420, 100"); // 2 Spalten
        }

        public XmlNode AppendSkillTable()
        {
            return AppendTable("420, 100"); // 2 Spalten
        }

        /// <summary>
        /// Eigene Methode um SPZ-spezifische Styles einzubringen
        /// </summary>
        /// <param name="reportTitle"></param>
        /// <param name="reportDate"></param>
        protected void spzWriteHeaderAndFooter(string reportTitle, string reportDate) 
        {
            XmlNode nHeader = _rootNode.AppendChild(createPageHeader("120,*", "0.5cm"));
            nHeader.Attributes.Append(createClassAttribute("spzPageHeader"));
            XmlNode nRow = nHeader.AppendChild(createRow());
            nRow.Attributes.Append(createAttribute("vertical-align","bottom"));
            XmlNode nCell = nRow.AppendChild(createCell());
            nCell.AppendChild(createShowImage("headerLogo"));
            nCell = nRow.AppendChild(createCell());
            nCell.InnerText = reportTitle;
            nCell.Attributes.Append(createAttribute("padding-left", "20"));
            
            nRow = nHeader.AppendChild(createRow());
            nRow.Attributes.Append(createAttribute("padding-bottom", "10"));
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createAttribute("padding-top", "10"));
            
            XmlNode nFooter = _rootNode.AppendChild(createPageFooter());
            nFooter.Attributes.Append(createClassAttribute("pageFooter"));
            nRow = nFooter.AppendChild(createRow());
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createAttribute("align", "left"));
            nCell.InnerText = reportDate;
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createAttribute("align","right"));
            nCell.InnerXml = _map.get("reportLayout", "page") + " <page-number/>/<forward-reference name='total-pages'/>";
        }

        protected string getRestriction()
        {
                return "VALID_FROM <= GetDate() and VALID_TO >= GetDate()"
                    + " and DUTY_VALIDITY_VALID_FROM <= GetDate() and DUTY_VALIDITY_VALID_TO >= GetDate()"
                    + " and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
        }

        protected string getSkillRestriction()
        {
            return "VALID_FROM <= GetDate() and VALID_TO >= GetDate()"
                + " and SKILL_VALIDITY_VALID_FROM <= GetDate() and SKILL_VALIDITY_VALID_TO >= GetDate()"
                + " and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
         }

        /// <summary>
        /// Hauptmethode (keine Übergabe der Personen-Id)
        /// </summary>
        /// <param name="jobID"></param>
        public void createReport(int jobID)
        {
            _groupNumber = 0;
            _reportDate = DateTime.Now.ToString("d");
            _jobID = jobID;
            _funktionID = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "JOB", "ID=" + _jobID, false), -1);
            long personID = ch.psoft.Util.Validate.GetValid(_db.lookup("PERSON_ID", "JOBPERSONV", "ID=" + _jobID, false), -1);

            spzWriteHeaderAndFooter(_map.get("spz", "Title"), _reportDate);

            _nTable = AppendDutyTable();
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createClassAttribute("spzMainTitle"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.InnerText = "1.  " + _map.get("spz", "JobDescription");

            // person
            _nTable = AppendDetailTable();
            _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
            _nTable.Attributes.Append(createAttribute("border-width-inner", ".5"));
            _nRow = _nTable.AppendChild(createRow());

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailName"));
            _nCell.InnerText = _map.get("spz", "JobOwner");

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));

            if (personID > 0)
            {
                _nCell.InnerText = _db.Person.getWholeName(personID.ToString(), false, true, false);
            }

            // funktion
            object [] values = _db.lookup(
                    new string [] 
                    {
                        _db.langAttrName("FUNKTION", "TITLE"),
                        _db.langAttrName("FUNKTION", "DESCRIPTION")
                    },
                    "FUNKTION",
                    "ID = " + _funktionID
                );
            string funktionTitle = DBColumn.GetValid(values[0], "");
            string funktionDescription = DBColumn.GetValid(values[1], "");

            _nRow = _nTable.AppendChild(createRow());

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailName"));
            _nCell.InnerText = _map.get("spz", "Function");

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            _nCell.InnerText = funktionTitle;

            // Firma/Bereich
            values = _db.lookup(
                    new string [] {"O.ID", "O." + _db.langAttrName("FUNKTION", "TITLE")},
                    "ORGENTITY O inner join JOB J on O.ID = J.ORGENTITY_ID",
                    "J.ID = " + _jobID
                );
            string bereich = DBColumn.GetValid(values[1], "");
            long orgentityId = DBColumn.GetValid(values[0], (long)-1);
            string firma = getFirma(orgentityId);

            _nRow = _nTable.AppendChild(createRow());

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailName"));
            _nCell.InnerText = _map.get("spz", "OE");

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            
            if (firma == "")
            {
                _nCell.InnerText = bereich;
            }
            else if (firma == bereich)
            {
                _nCell.InnerText = firma;
            }
            else
            {
                _nCell.InnerText = firma + "/" + bereich;
            }

            // Vorgesetzte Funktion
            _nRow = _nTable.AppendChild(createRow());

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailName"));
            _nCell.InnerText = _map.get("spz", "LeaderFunction");

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            XmlNode subTable  = _nCell.AppendChild(createTable());

            DataTable funktionTable = _db.getDataTable(
                    "select distinct F." + _db.langAttrName("FUNKTION", "TITLE")
                        + " from FUNKTION F, JOB J"
                        + " where J.ORGENTITY_ID = " + orgentityId
                        + " and J.TYP = 1"
                        + " and F.ID = J.FUNKTION_ID",
                    "FUNKTION"
                );

            foreach (DataRow row in funktionTable.Rows) 
            {
                if (row[0].ToString() != funktionTitle)
                {
                    _nRow = subTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    _nCell.InnerText = row[0].ToString();
                }
            }

            // Unterstellte Bereiche
            _nRow = _nTable.AppendChild(createRow());

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailName"));
            _nCell.InnerText = _map.get("spz", "SubOEs");

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            _nTable  = _nCell.AppendChild(createTable());

            DataTable subOETable = _db.getDataTable(
                    "select " + _db.langAttrName("ORGENTITY", "TITLE")
                        + " from ORGENTITY"
                        + " where PARENT_ID = " + orgentityId
                        + " order by ORDNUMBER",
                    "ORGENTITY"
                );

            foreach (DataRow row in subOETable.Rows) 
            {
                _nRow = _nTable.AppendChild(createRow());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createClassAttribute("detailValue"));
                _nCell.InnerText = row[0].ToString();
            }

            // Ziel der Stelle (funktion.description)
            appendVSpace(13);
            _nTable = AppendDutyTable();
            _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createClassAttribute("spzTitle"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _groupNumber++;
            _nCell.InnerText = "1." + _groupNumber + " " + _map.get("spz", "Overview");

            _nRow = _nTable.AppendChild(createRow());
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            addCellText(_nCell, funktionDescription);

            _competenceLevels = FBSModule.getCompetenceLevels(_db);

            // Detaillierte Aufgabenbeschreibungen
            DataTable grpTable = _db.getDataTable(
                    "select * from DUTYGROUP order by " + _db.langAttrName("DUTYGROUP", "TITLE"),
                    "DUTYGROUP"
                );

            bool isFirstGrp = true;

            foreach (DataRow grpRow in grpTable.Rows)
            {
                long dutyGroupID = ch.psoft.Util.Validate.GetValid(grpRow["ID"].ToString(), -1L);

                string sql = "";
                string sqlOrdr = " order by "
                    + _db.langAttrName("DUTY_COMPETENCE__DUTY_VALIDITY_V", "NUM_TITLE");
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

            // Kompetenzenlegende
            appendVSpace(20);
            DataTable table = _db.getDataTable(
                    "select * from COMPETENCE_LEVEL order by " + _db.langAttrName("COMPETENCE_LEVEL", "MNEMO"),
                    "COMPETENCE_LEVEL"
                );

            if (table.Rows.Count > 0)
            {
                _nTable = AppendTable("20,500");
                _nTable.Attributes.Append(createAttribute("keep-together", "true"));
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createClassAttribute("subTitle"));
                _nRow.Attributes.Append(createAttribute("padding-bottom", "10"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("colspan", "2"));
                _nCell.InnerText
                    = _map.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_REP_COMPETENCES_RULES);

                foreach (DataRow row in table.Rows) 
                {
                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailName"));
                    _nCell.InnerText = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "MNEMO")].ToString(), "");

                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    _nCell.InnerText = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "TITLE")].ToString(), "");
                }
            }

            // Anforderungsprofil
            if (Global.isModuleEnabled("skills"))
            {
                appendVSpace(20);
                _groupNumber = 0;
                _nTable = AppendDutyTable();
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createClassAttribute("spzMainTitle"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("colspan", "2"));
                _nCell.InnerText = "2.  " + _map.get("spz", "Skills");

                grpTable = _db.getDataTable(
                        "select * from SKILLGROUP order by " + _db.langAttrName("SKILLGROUP", "TITLE"),
                        "SKILLGROUP"
                    );
                isFirstGrp = true;
                
                foreach (DataRow grpRow in grpTable.Rows) 
                {
                    long skillGroupID = ch.psoft.Util.Validate.GetValid(grpRow["ID"].ToString(), -1L);

                    string sql = "";
                    string sqlOrdr = " order by " + _db.langAttrName("SKILL_LEVEL__SKILL_VALIDITY_V", "NUM_TITLE");
                    DataTable table2 = null;

                    if (isFirstGrp)
                    {
                        sql = "select * from SKILL_LEVEL__SKILL_VALIDITY_V"
                            + " where " + getSkillRestriction() + " and SKILLGROUP_ID is null";
                        sql += sqlOrdr;
                        table2 = _db.getDataTable(sql, "SKILL_LEVEL__SKILL_VALIDITY_V");
                        addSkillGroup(table2, ref isFirstGrp, null);
                    }

                    sql = "select * from SKILL_LEVEL__SKILL_VALIDITY_V"
                        + " where " + getSkillRestriction() + " and SKILLGROUP_ID = " + skillGroupID;
                    sql += sqlOrdr;
                    table2 = _db.getDataTable(sql, "SKILL_LEVEL__SKILL_VALIDITY_V");
                    addSkillGroup(table2, ref isFirstGrp, grpRow);
                }
            }

            // Schluss
            _nTable = AppendDetailTable();
            _nTable.Attributes.Append(createAttribute("keep-together", "true"));
            
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createAttribute("padding-bottom", "25"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.Attributes.Append(createAttribute("padding-top", "25"));

            _nRow = _nTable.AppendChild(createRow());

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            _nCell.InnerText = _map.get("spz", "reportDate");

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            _nCell.InnerText = _reportDate;

            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createAttribute("padding-bottom", "36"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.Attributes.Append(createAttribute("padding-top", "36"));

            _nRow = _nTable.AppendChild(createRow());

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.InnerText = _map.get("spz", "remark");
        }

        /// <summary>
        /// Firma ist OE unter Root. (Gemäss M. Steinacher, 22.9.2006)
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <returns></returns>
        private string getFirma(long orgentityId)
        {
            string returnValue = "";
            long id = orgentityId;
            long root_id = -1;
            object [] values = null;

            while (id != root_id  && id != -1)
            {
                values = _db.lookup(
                        new string [] {"PARENT_ID", "ROOT_ID", _db.langAttrName("FUNKTION", "TITLE")},
                        "ORGENTITY",
                        "ID = " + id
                    );
                id = DBColumn.GetValid(values[0], (long)-1);
                root_id = DBColumn.GetValid(values[1], (long)-1);
            }

            if (values != null)
            {
               returnValue = DBColumn.GetValid(values[2], "");
            }

            return returnValue;
        }

        /// <summary>
        /// Aufgabengruppe
        /// </summary>
        /// <param name="table"></param>
        /// <param name="isFirstGrp"></param>
        /// <param name="grpRow"></param>
        private void addGroup(DataTable table, ref bool isFirstGrp, DataRow grpRow)
        {
            if (table.Rows.Count > 0) 
            {
                if (isFirstGrp)
                {
                    isFirstGrp = false;
                }

                appendVSpace(13);
                _nTable = AppendDutyTable();
                _nTable.Attributes.Append(createAttribute("keep-together", "true"));
                _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createClassAttribute("spzTitle"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("colspan", "2"));

                if (grpRow != null)
                {
                    _groupNumber++;
                    _nCell.InnerText = "1." + _groupNumber + " "
                        + ch.psoft.Util.Validate.GetValid(
                            grpRow[_db.langAttrName(grpRow.Table.TableName, "TITLE")].ToString(),
                            ""
                        );
                }
                else 
                {
                    _nCell.InnerText = _map.get(
                            FBSModule.LANG_SCOPE_FBS,
                            FBSModule.LANG_MNEMO_REP_DUTYGROUP_FREE
                        );
                }

                foreach (DataRow dcvRow in table.Rows)
                {
                    if (FBSModule.showNumTitleInReport)
                    {
                        _nRow = _nTable.AppendChild(createRow());
                        _nCell = _nRow.AppendChild(createCell());
                        _nCell.Attributes.Append(createClassAttribute("subTitle"));
                        _nCell.Attributes.Append(createAttribute("colspan", "2"));
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

        /// <summary>
        /// Gruppe im Anforderungsprofil
        /// </summary>
        /// <param name="table"></param>
        /// <param name="isFirstGrp"></param>
        /// <param name="grpRow"></param>
        private void addSkillGroup(DataTable table, ref bool isFirstGrp, DataRow grpRow)
        {
            if (table.Rows.Count > 0) 
            {
                if (isFirstGrp)
                {
                    isFirstGrp = false;
                }
                else
                {
                    appendVSpace(13);
                }

                _nTable = AppendSkillTable();
                _nTable.Attributes.Append(createAttribute("keep-together", "true"));
                _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createClassAttribute("spzTitle"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("colspan", "2"));

                if (grpRow != null)
                {
                    _nCell.InnerText = "2." + _groupNumber + " "
                        + ch.psoft.Util.Validate.GetValid(
                            grpRow[_db.langAttrName(grpRow.Table.TableName, "TITLE")].ToString(),
                            ""
                        );
                }
                else 
                {
                    _nCell.InnerText = _map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_SKILLGROUP_FREE);
                }

                foreach (DataRow dcvRow in table.Rows)
                {
                    if (SkillsModule.showNumTitleInReport)
                    {
                        _nRow = _nTable.AppendChild(createRow());
                        _nCell = _nRow.AppendChild(createCell());
                        _nCell.Attributes.Append(createClassAttribute("subTitle"));
                        _nCell.Attributes.Append(createAttribute("colspan", "2"));
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
