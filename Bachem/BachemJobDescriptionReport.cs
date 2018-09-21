using ch.appl.psoft.db;
using ch.appl.psoft.FBS;
using ch.appl.psoft.Report;
using ch.appl.psoft.Skills;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Bachem
{
    /// <summary>
    /// SPZ-spezifischer Report
    /// Im Prinzip Stellenbeschreibungs- und Skillsreport
    /// Die Beschriftung der Spalte Kompetenzen bzw Anforderungsniveau
    /// wurden weggelassen, da diese nicht verwendet werden.
    /// Das Drucken der Spalten selbst ist noch vorhanden.
    /// </summary>
    public class BachemJobDescriptionReport : PsoftPDFReport
	{
        protected DataTable _competenceLevels = null;
        protected long _jobID = -1;
        protected long _funktionID = -1;
        protected int _groupNumber = 0;
        protected string _reportDate = "";
        protected XmlNode _nTable, _nRow, _nCell;
		protected HttpSessionState session = null;

        public BachemJobDescriptionReport(HttpSessionState session, string imageDirectory)
            : base(session, imageDirectory)
        {
			this.session = session;

			addImageToCatalog(_imageDirectory + ReportModule.headerLogoImage, "headerLogo");

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
            XmlNode nHeader = _rootNode.AppendChild(createPageHeader("*,80", "0.5cm"));
            nHeader.Attributes.Append(createClassAttribute("spzPageHeader"));
            XmlNode nRow = nHeader.AppendChild(createRow());
            nRow.Attributes.Append(createAttribute("vertical-align","bottom"));
            XmlNode nCell = nRow.AppendChild(createCell());

			nCell = nRow.AppendChild(createCell());
			nCell.AppendChild(createShowImage("headerLogo"));
            
            nRow = nHeader.AppendChild(createRow());
            nRow.Attributes.Append(createAttribute("padding-bottom", "10"));
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createAttribute("padding-top", "10"));
            
            XmlNode nFooter = _rootNode.AppendChild(createPageFooter());
            nFooter.Attributes.Append(createClassAttribute("pageFooter"));
            nRow = nFooter.AppendChild(createRow());
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createAttribute("align", "left"));
            //nCell.InnerText = reportDate;
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createAttribute("align","right"));
            nCell.InnerXml = _map.get("reportLayout", "page") + " <page-number/> " + _map.get("reportLayout", "pageFrom") + " <forward-reference name='total-pages'/>";
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

            spzWriteHeaderAndFooter(_map.get("bachem", "Title"), _reportDate);

            _nTable = AppendDutyTable();
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createClassAttribute("spzMainTitle"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.InnerText = _map.get("bachem", "JobDescription");

            // person
            _nTable = AppendDetailTable();
            _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
            _nTable.Attributes.Append(createAttribute("border-width-inner", ".5"));
            _nRow = _nTable.AppendChild(createRow());

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailName"));
            _nCell.InnerText = _map.get("bachem", "JobOwner");

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));

            if (personID > 0)
            {
                _nCell.InnerText = _db.Person.getWholeName(personID.ToString(), false, true, false);
            }
            // Engagement

			int engagement = ch.psoft.Util.Validate.GetValid(_db.lookup("ENGAGEMENT", "JOB", "ID=" + _jobID , ""), 0);

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

			//Funktionsbezeichnung
            _nRow = _nTable.AppendChild(createRow());

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailName"));
            _nCell.InnerText = _map.get("bachem", "Function");

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            _nCell.InnerText = funktionTitle;

			//Bezeichnung für externe Kommunikation (Anstellung)
			DataTable descriptiontab = _db.getDataTable("select e." + _db.langAttrName("EMPLOYMENT", "TITLE") + " as title from employment e where e.person_id=" + personID, Logger.VERBOSE);
			string description = ch.psoft.db.SQLColumn.GetValid(descriptiontab.Rows[0][0],"");

			_nRow = _nTable.AppendChild(createRow());

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailName"));
			_nCell.InnerText = _map.get("bachem", "externDescription");

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailValue"));
			_nCell.InnerText = description;

			//Beschäftigungsgrad
			_nRow = _nTable.AppendChild(createRow());

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailName"));
			_nCell.InnerText = _map.get("bachem", "Engagement");

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailValue"));
			_nCell.InnerText = ""+ engagement + " %";


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
            _nCell.InnerText = _map.get("bachem", "OE");

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
            string sql_getleader = "select p.firstname + ' ' + p.pname + '/' + f." + _db.langAttrName("FUNKTION", "TITLE") + " as fulldescription from funktion f, job j, employment e, person p where e.person_id=p.id and j.employment_id=e.id and j.funktion_id=f.id and f.id=dbo.GET_LEADERFUNCTIONID(" + personID + ")";
			DataTable tab = _db.getDataTable(sql_getleader, Logger.VERBOSE);
			string leaderfunction = "";
			if(tab.Rows.Count > 0) 
			{
				leaderfunction = ch.psoft.db.SQLColumn.GetValid(tab.Rows[0][0],"");
			}
			_nRow = _nTable.AppendChild(createRow());

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailName"));
			_nCell.InnerText = _map.get("bachem", "LeaderFunction");

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailValue"));
			_nCell.InnerText = leaderfunction;

			//Stellvertretung durch
			DataTable substitutedtab = _db.getDataTable("select p.firstname + ' ' + p.pname + '/' + f." + _db.langAttrName("FUNKTION", "TITLE") + " as fulldescription from person p, job j, funktion f where j.funktion_id=f.id and p.id=j.proxy_person_id and j.hauptfunktion=1 and j.id=" + _jobID, Logger.VERBOSE);
			string substituted = "";
			if(substitutedtab.Rows.Count > 0 ) 
			{
				substituted = ch.psoft.db.SQLColumn.GetValid(substitutedtab.Rows[0][0],"");
			}
			_nRow = _nTable.AppendChild(createRow());

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailName"));
			_nCell.InnerText = _map.get("bachem", "substituted");

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailValue"));
			_nCell.InnerText = substituted;

			//Stellvertretung von
			DataTable substitutestab = _db.getDataTable("select p.firstname + ' ' + p.pname + '/' + f." + _db.langAttrName("FUNKTION", "TITLE") + " as fulldescription from person p, job j, employment e, funktion f where j.funktion_id=f.id and p.id=e.person_id and e.id=j.employment_id and j.proxy_person_id=" + personID, Logger.VERBOSE);
			string substitutes = "";
			for(int i = 0; i < substitutestab.Rows.Count; i++) 
			{
				if(i > 0) 
				{
					substitutes += ", ";
				}
				substitutes += ch.psoft.db.SQLColumn.GetValid(substitutestab.Rows[i][0],"");
			}

			_nRow = _nTable.AppendChild(createRow());

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailName"));
			_nCell.InnerText = _map.get("bachem", "substitutes");

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailValue"));
			_nCell.InnerText = substitutes;
			//Ende erste Tabelle

            // Ziel der Stelle (funktion.description) - Stellenzweck
            appendVSpace(13);
            _nTable = AppendDutyTable();
            _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createClassAttribute("spzTitle"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.InnerText = "1. " + _map.get("bachem", "Overview");

            _nRow = _nTable.AppendChild(createRow());
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            addCellText(_nCell, funktionDescription);

			//Aufgaben
			appendVSpace(13);
			DataTable duties = _db.getDataTable("select CHAPTERNUMBER, " + _db.langAttrName("FUNKTION", "DESCRIPTION") + ", DEPTH, DUTY, SECTIONNUM from GET_DUTIESANDGROUPS(" + personID + ") order by ordnumber asc", Logger.VERBOSE);
			for(int i = 0; i < duties.Rows.Count; i++) 
			{
				int sectionnumber = ch.psoft.db.SQLColumn.GetValid(duties.Rows[i][4],0);
				if(ch.psoft.db.SQLColumn.GetValid(duties.Rows[i][3],0)==0) 
				{
					if(i>0 && i < duties.Rows.Count-1 && ch.psoft.db.SQLColumn.GetValid(duties.Rows[i-1][3],0)==1 && ch.psoft.db.SQLColumn.GetValid(duties.Rows[i-1][4],0)<sectionnumber) 
					{
						appendVSpace(13);
					}
					_nTable = AppendDutyTable();
					_nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
					_nTable.Attributes.Append(createAttribute("keep-together", "true"));
					_nRow = _nTable.AppendChild(createRow());
					_nRow.Attributes.Append(createClassAttribute("spzTitle"));
					_nCell = _nRow.AppendChild(createCell());
					_nCell.Attributes.Append(createAttribute("colspan", "2"));
					_nCell.InnerText = ch.psoft.db.SQLColumn.GetValid(duties.Rows[i][0],"") + ' ' + ch.psoft.db.SQLColumn.GetValid(duties.Rows[i][1],"");
				} 
				else 
				{
					_nRow = _nTable.AppendChild(createRow());
					_nCell = _nRow.AppendChild(createCell());
					_nCell.Attributes.Append(createAttribute("colspan", "2"));
					_nCell.Attributes.Append(createClassAttribute("detailValue"));
					addCellText(_nCell, ch.psoft.db.SQLColumn.GetValid(duties.Rows[i][1],""));
				}
			}

			/*_nTable = AppendDutyTable();
			_nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
			_nRow = _nTable.AppendChild(createRow());
			_nRow.Attributes.Append(createClassAttribute("spzTitle"));
			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createAttribute("colspan", "2"));
			_nCell.InnerText = "2. " + _map.get("bachem", "Duties");

            _competenceLevels = FBSModule.getCompetenceLevels(_db);*/

            // Detaillierte Aufgabenbeschreibungen
            
			

			/*DataTable grpTable = _db.getDataTable(
                    "select * from DUTYGROUP order by " + _db.langAttrName("DUTYGROUP", "TITLE"),
                    "DUTYGROUP"
                );

            bool isFirstGrp = true;

            foreach (DataRow grpRow in grpTable.Rows)
            {
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
            }*/

            // Schluss
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
			_nCell.InnerText = _map.get("bachem", "staffmember");
			_nCell = _nRow.AppendChild(createCell());
			_nCell.InnerText = _map.get("bachem", "datevisa");

			_nRow = _nTable.AppendChild(createRow());
			_nCell = _nRow.AppendChild(createCell());
			_nCell.InnerText = _map.get("bachem", "directchief");
			_nCell = _nRow.AppendChild(createCell());
			_nCell.InnerText = _map.get("bachem", "datevisa");

			_nRow = _nTable.AppendChild(createRow());
			_nCell = _nRow.AppendChild(createCell());
			_nCell.InnerText = _map.get("bachem", "nextchief");
			_nCell = _nRow.AppendChild(createCell());
			_nCell.InnerText = _map.get("bachem", "datevisa");

			_nRow = _nTable.AppendChild(createRow());
			_nCell = _nRow.AppendChild(createCell());
			_nCell.InnerText = _map.get("bachem", "humanresources");
			_nCell = _nRow.AppendChild(createCell());
			_nCell.InnerText = _map.get("bachem", "datevisa");

			//Druckdatum, cst, 071122
			_nTable = AppendDetailTable();
			_nTable.Attributes.Append(createAttribute("keep-together", "true"));
			_nRow = _nTable.AppendChild(createRow());

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailValue"));
			_nCell.InnerText = _map.get("bachem", "printDate");

			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createClassAttribute("detailValue"));
			_nCell.InnerText = DateTime.Now.ToString("dd.MM.yyyy");

			_nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createAttribute("padding-bottom", "36"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.Attributes.Append(createAttribute("padding-top", "36"));
			
            _nRow = _nTable.AppendChild(createRow());

            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailValue"));
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.InnerText = _map.get("bachem", "remark");
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
				else 
				{
					appendVSpace(13);
				}

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
                    _nCell.InnerText = "2." + _groupNumber + " "
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
                        _nCell.InnerText = ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "NUM_TITLE")].ToString(), "");
                    }

                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
					_nCell.Attributes.Append(createAttribute("colspan", "2"));
                    addCellText(_nCell, ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "DESCRIPTION")].ToString(), ""));
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
                    _nCell.InnerText = "4." + _groupNumber + " "
                        + ch.psoft.Util.Validate.GetValid(
                            grpRow[_db.langAttrName(grpRow.Table.TableName, "TITLE")].ToString(),
                            ""
                        );

					_groupNumber++;
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
