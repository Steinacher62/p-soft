using ch.appl.psoft.db;
using ch.appl.psoft.Report;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;

namespace ch.appl.psoft.Laufenburg
{
    public class JobDescriptionReport : System.Web.UI.Page
    {
        protected long _jobID = -1;
        protected long _funktionID = -1;
        protected int _groupNumber = 0;
        protected string _reportDate = DateTime.Now.ToString("d");
        protected void Page_Load(object sender, System.EventArgs e)
        {
            
            string imageDirectory = Request.MapPath("~/images");
            string reportfile = Server.MapPath(Global.Config.baseURL + "/crystalreports/StellenbeschreibungLaufenburg.rpt");
            DBData db = DBData.getDBData(Session);
            ReportDocument rpt1 = ReportFactory.GetReport();

            rpt1.Load(reportfile);

            //set db logon for report
            ConnectionInfo connectionInfo = new ConnectionInfo();
            connectionInfo.ServerName = Global.Config.dbServer;
            connectionInfo.DatabaseName = Global.Config.dbName;
            connectionInfo.UserID = Global.Config.dbUser;
            connectionInfo.Password = Global.Config.dbPassword;

            SetDBLogonForReport(connectionInfo, rpt1);
            SetHeader(rpt1, db);

        }

        private void SetDBLogonForReport(ConnectionInfo connectionInfo, ReportDocument rpt1)
        {
            Tables tables = rpt1.Database.Tables;

            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(tableLogonInfo);
                table.Location = connectionInfo.DatabaseName + ".dbo." + table.Location.Substring(table.Location.LastIndexOf(".") + 1);
            }
        }

        private void SetHeader(ReportDocument rpt1, DBData db)
        {
            TextObject Stelleninhaber = (TextObject)rpt1.ReportDefinition.ReportObjects["Stelleninhaber"];
            TextObject BG = (TextObject)rpt1.ReportDefinition.ReportObjects["BG"];
            TextObject Geburtsdatum = (TextObject)rpt1.ReportDefinition.ReportObjects["Geburtsdatum"];
            TextObject Bereich = (TextObject)rpt1.ReportDefinition.ReportObjects["Bereich"];
            TextObject Vorgesetzter = (TextObject)rpt1.ReportDefinition.ReportObjects["Vorgesetzter"];
            TextObject Stellvertretung = (TextObject)rpt1.ReportDefinition.ReportObjects["Stellvertretung"];
            TextObject UnterstellteBereiche = (TextObject)rpt1.ReportDefinition.ReportObjects["UnterstellteBereiche"];
            TextObject Stellenziele = (TextObject)rpt1.ReportDefinition.ReportObjects["Stellenziele"];

            //Stelleninhaber.Text = db.Person.getWholeName(personID.ToString(), false, true, false);
            //Pname.Text = db.lookup("PNAME", "PERSON", "ID=" + _personID).ToString();
            //Personnelnumber.Text = db.lookup("PERSONNELNUMBER", "PERSON", "ID=" + _personID).ToString();
            //Dateofbirth.Text = db.lookup("DATEOFBIRTH", "PERSON", "ID=" + _personID).ToString();
            //if (Dateofbirth.Text != "")
            //{
            //    Dateofbirth.Text = Dateofbirth.Text.Remove(11);
            //}
            //Job.Text = db.lookup("JOB_TITLE_" + map.LanguageCode, "PERFORMANCERATING", "ID=" + _performanceRatingID).ToString();
            //Ratingdat.Text = db.lookup("RATING_DATE", "PERFORMANCERATING", "ID=" + _performanceRatingID).ToString().Remove(11);
            // TextObject name = (TextObject)rpt1.ReportDefinition.ReportObjects["Text8"];
            // name.Text = map.get("Performance", "pname");

        }
        
        
        
        
        
        
        
        
        
    
        
        
        //    protected DataTable _competenceLevels = null;
    //    protected long _jobID = -1;
    //    protected long _funktionID = -1;
    //    protected int _groupNumber = 0;
    //    protected string _reportDate = "";
    //    protected XmlNode _nTable, _nRow, _nCell;
    //    protected HttpSessionState session = null;

    //    public JobDescriptionReport(HttpSessionState session, string imageDirectory)
    //        : base(session, imageDirectory)
    //    {
    //        this.session = session;
    //        // Für Laufenburg eigene Styles
    //        appendStyle(".spzPageHeader", "arial-bold", 14, "left");
    //        XmlNode node = appendStyle(".spzMainTitle", "arial-bold", 14, "left");
    //        node.Attributes.Append(createAttribute("fill-color", "#fbf5ad"));
    //        node.Attributes.Append(createAttribute("border-width-outer", ".5"));
    //        node.Attributes.Append(createAttribute("padding-top", "6"));
    //        node.Attributes.Append(createAttribute("padding-bottom", "8"));
    //        node = appendStyle(".spzTitle", "arial-bold", 11, "left");
    //        node.Attributes.Append(createAttribute("fill-color", "#fbf5ad"));
    //        node.Attributes.Append(createAttribute("border-width-bottom", ".5"));
    //        node.Attributes.Append(createAttribute("padding-top", "4"));
    //        node.Attributes.Append(createAttribute("padding-bottom", "6"));
    //    }

    //    public XmlNode AppendDutyTable()
    //    {
    //        return AppendTable("510, 10"); // 2 Spalten
    //    }

    //    public XmlNode AppendSkillTable()
    //    {
    //        return AppendTable("420, 100"); // 2 Spalten
    //    }

    //    /// <summary>
    //    /// Eigene Methode um Laufenburg-spezifische Styles einzubringen
    //    /// </summary>
    //    /// <param name="reportTitle"></param>
    //    /// <param name="reportDate"></param>
    //    protected void spzWriteHeaderAndFooter(string reportTitle, string reportDate)
    //    {
    //        XmlNode nHeader = _rootNode.AppendChild(createPageHeader("100,*", "2.0cm"));
    //        nHeader.Attributes.Append(createClassAttribute("spzPageHeader"));
    //        XmlNode nRow = nHeader.AppendChild(createRow());
    //        nRow.Attributes.Append(createAttribute("vertical-align", "bottom"));
    //        XmlNode nCell = nRow.AppendChild(createCell());
    //        //nCell.InnerText = reportTitle;
    //        //nCell.Attributes.Append(createAttribute("padding-left", "20"));

    //        nCell = nRow.AppendChild(createCell());
    //        nCell.Attributes.Append(createAttribute("align", "right"));
    //        nCell.AppendChild(createShowImage("headerLogo"));

    //        nRow = nHeader.AppendChild(createRow());
    //        nRow.Attributes.Append(createAttribute("padding-bottom", "10"));
    //        nCell = nRow.AppendChild(createCell());
    //        nCell.Attributes.Append(createAttribute("padding-top", "10"));

    //        XmlNode nFooter = _rootNode.AppendChild(createPageFooter());
    //        nFooter.Attributes.Append(createClassAttribute("pageFooter"));
    //        nRow = nFooter.AppendChild(createRow());
    //        nCell = nRow.AppendChild(createCell());
    //        nCell.Attributes.Append(createAttribute("align", "left"));
    //        nCell.InnerText = reportDate;
    //        nCell = nRow.AppendChild(createCell());
    //        nCell.Attributes.Append(createAttribute("align", "right"));
    //        nCell.InnerXml = _map.get("reportLayout", "page") + " <page-number/>/<forward-reference name='total-pages'/>";
    //    }

    //    protected string getRestriction()
    //    {
    //        return "VALID_FROM <= GetDate() and VALID_TO >= GetDate()"
    //            + " and DUTY_VALIDITY_VALID_FROM <= GetDate() and DUTY_VALIDITY_VALID_TO >= GetDate()"
    //            + " and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
    //    }

    //    protected string getSkillRestriction()
    //    {
    //        return "VALID_FROM <= GetDate() and VALID_TO >= GetDate()"
    //            + " and SKILL_VALIDITY_VALID_FROM <= GetDate() and SKILL_VALIDITY_VALID_TO >= GetDate()"
    //            + " and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
    //    }

    //    /// <summary>
    //    /// Hauptmethode (keine Übergabe der Personen-Id)
    //    /// </summary>
    //    /// <param name="jobID"></param>
    //    public void createReport(int jobID)
    //    {
    //        //_groupNumber = 0;
    //        _reportDate = DateTime.Now.ToString("d");
    //        _jobID = jobID;
    //        _funktionID = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "JOB", "ID=" + _jobID, false), -1);
    //        long personID = ch.psoft.Util.Validate.GetValid(_db.lookup("PERSON_ID", "JOBPERSONV", "ID=" + _jobID, false), -1);

    //        spzWriteHeaderAndFooter(_map.get("laufenburg", "Title"), _reportDate);

    //        _nTable = AppendDutyTable();
    //        _nRow = _nTable.AppendChild(createRow());
    //        _nRow.Attributes.Append(createClassAttribute("spzMainTitle"));
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("colspan", "2"));
    //        _nCell.InnerText = _map.get("laufenburg", "JobDescription");

    //        // person

    //        _nTable = AppendDetailTable();
    //        _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
    //        _nTable.Attributes.Append(createAttribute("border-width-inner", ".5"));
    //        _nRow = _nTable.AppendChild(createRow());

    //        // auskommentieren, wenn Name auf Report nicht angezeigt werden soll
    //        // ---

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailName"));
    //        _nCell.InnerText = _map.get("laufenburg", "JobOwner");

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));

    //        if (personID > 0)
    //        {
    //            _nCell.InnerText = _db.Person.getWholeName(personID.ToString(), false, true, false);
    //        }
    //        // ---

    //        // Engagement

    //        int engagement = ch.psoft.Util.Validate.GetValid(_db.lookup("ENGAGEMENT", "JOB", "ID=" + _jobID, ""), 0);

    //        _nRow = _nTable.AppendChild(createRow());

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailName"));
    //        _nCell.InnerText = _map.get("laufenburg", "Engagement");

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //        _nCell.InnerText = "" + engagement + " %";

    //        // date of birth

    //        _nRow = _nTable.AppendChild(createRow());
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailName"));
    //        _nCell.InnerText = _map.get("laufenburg", "DateOfBirth");

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));

    //        if (!_db.lookup("DATEOFBIRTH", "PERSON", "ID = " + personID.ToString()).ToString().Equals(""))
    //        {
    //            _nCell.InnerText = _db.lookup("DATEOFBIRTH", "PERSON", "ID = " + personID.ToString()).ToString().Substring(1, 10);
    //        }


    //        // funktion
    //        object[] values = _db.lookup(
    //                new string[] 
    //                {
    //                    _db.langAttrName("FUNKTION", "TITLE"),
    //                    _db.langAttrName("FUNKTION", "DESCRIPTION")
    //                },
    //                "FUNKTION",
    //                "ID = " + _funktionID
    //            );
    //        string funktionTitle = DBColumn.GetValid(values[0], "");
    //        string funktionDescription = DBColumn.GetValid(values[1], "");

    //        _nRow = _nTable.AppendChild(createRow());

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailName"));
    //        _nCell.InnerText = _map.get("laufenburg", "Function");

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //        _nCell.InnerText = funktionTitle;

    //        // Firma/Bereich
    //        values = _db.lookup(
    //                new string[] { "O.ID", "O." + _db.langAttrName("FUNKTION", "TITLE") },
    //                "ORGENTITY O inner join JOB J on O.ID = J.ORGENTITY_ID",
    //                "J.ID = " + _jobID
    //            );
    //        string bereich = DBColumn.GetValid(values[1], "");
    //        long orgentityId = DBColumn.GetValid(values[0], (long)-1);
    //        string firma = getFirma(orgentityId);

    //        _nRow = _nTable.AppendChild(createRow());

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailName"));
    //        _nCell.InnerText = _map.get("laufenburg", "OE");

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));

    //        if (firma == "")
    //        {
    //            _nCell.InnerText = bereich;
    //        }
    //        else if (firma == bereich)
    //        {
    //            _nCell.InnerText = firma;
    //        }
    //        else
    //        {
    //            _nCell.InnerText = firma + "/" + bereich;
    //        }

    //        // Vorgesetzte Funktion
    //        _nRow = _nTable.AppendChild(createRow());

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailName"));
    //        _nCell.InnerText = _map.get("laufenburg", "LeaderFunction");

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //        XmlNode subTable = _nCell.AppendChild(createTable());

    //        DataTable tab = _db.getDataTable("select dbo.GET_LEADERFUNCTIONID(" + personID + ")", Logger.VERBOSE);
    //        long leaderFunctionID = ch.psoft.db.SQLColumn.GetValid(tab.Rows[0][0], 0L);

    //        string sqlFunction = "select distinct F." + _db.langAttrName("FUNKTION", "TITLE")
    //                    + " from FUNKTION F WHERE"
    //                    + " F.ID = " + leaderFunctionID;

    //        DataTable funktionTable = _db.getDataTable(
    //                sqlFunction,
    //                "FUNKTION"
    //            );

    //        foreach (DataRow row in funktionTable.Rows)
    //        {
    //            if (row[0].ToString() != funktionTitle)
    //            {
    //                _nRow = subTable.AppendChild(createRow());
    //                _nCell = _nRow.AppendChild(createCell());
    //                _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //                _nCell.InnerText = row[0].ToString();
    //            }
    //        }

    //        // Stellvertretung

    //        long proxyPersonId = (long)_db.lookup("PROXY_PERSON_ID", "JOB", "ID = " + _jobID.ToString(), 0L);
    //        if (proxyPersonId > 0)
    //        {
    //            _nRow = _nTable.AppendChild(createRow());

    //            _nCell = _nRow.AppendChild(createCell());
    //            _nCell.Attributes.Append(createClassAttribute("detailName"));
    //            _nCell.InnerText = _map.get("laufenburg", "Representation");
    //            _nCell = _nRow.AppendChild(createCell());
    //            _nCell.Attributes.Append(createClassAttribute("detailValue"));

    //            _nCell.InnerText = _db.Person.getWholeName(proxyPersonId.ToString(), false, true, false);

    //        }

    //        // Unterstellte Bereiche
    //        if (ch.psoft.Util.Validate.GetValid(_db.lookup("TYP", "JOB", "ID=" + _jobID, ""), 0) == 1)
    //        {
    //            _nRow = _nTable.AppendChild(createRow());

    //            _nCell = _nRow.AppendChild(createCell());
    //            _nCell.Attributes.Append(createClassAttribute("detailName"));
    //            _nCell.InnerText = _map.get("laufenburg", "SubOEs");

    //            _nCell = _nRow.AppendChild(createCell());
    //            _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //            _nTable = _nCell.AppendChild(createTable());

    //            DataTable subOETable = _db.getDataTable(
    //                    "select " + _db.langAttrName("ORGENTITY", "TITLE")
    //                        + " from ORGENTITY"
    //                        + " where PARENT_ID = " + orgentityId
    //                        + " order by ORDNUMBER",
    //                    "ORGENTITY"
    //                );



    //            foreach (DataRow row in subOETable.Rows)
    //            {
    //                _nRow = _nTable.AppendChild(createRow());
    //                _nCell = _nRow.AppendChild(createCell());
    //                _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //                _nCell.InnerText = row[0].ToString();
    //            }
    //        }


    //        // Ziel der Stelle (funktion.description)
    //        appendVSpace(13);
    //        _nTable = AppendDutyTable();
    //        _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
    //        _nRow = _nTable.AppendChild(createRow());
    //        _nRow.Attributes.Append(createClassAttribute("spzTitle"));
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("colspan", "2"));

    //        _nCell.InnerText = _map.get("laufenburg", "Overview");

    //        _nRow = _nTable.AppendChild(createRow());
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("colspan", "2"));
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //        addCellText(_nCell, funktionDescription);

    //        _competenceLevels = FBSModule.getCompetenceLevels(_db);

    //        // Detaillierte Aufgabenbeschreibungen
    //        DataTable dutysandgroupsTable = _db.getDataTable("select ID, DUTY, " + _db.langAttrName("FUNKTION", "DESCRIPTION") + ", " + _db.langAttrName("DUTY_COMPETENCE__DUTY_VALIDITY_V", "NUM_TITLE") + " from GET_DUTIESANDGROUPS(" + personID + ") order by ordnumber");

    //        bool isFirstGrp = true;

    //        for (int i = 0; i < dutysandgroupsTable.Rows.Count; i++)
    //        {
    //            long dutyid = ch.psoft.db.SQLColumn.GetValid(dutysandgroupsTable.Rows[i][0], -1L);
    //            int isduty = ch.psoft.db.SQLColumn.GetValid(dutysandgroupsTable.Rows[i][1], -1);

    //            int isdutynext = 1;
    //            if (i < dutysandgroupsTable.Rows.Count - 1)
    //            {
    //                isdutynext = ch.psoft.db.SQLColumn.GetValid(dutysandgroupsTable.Rows[i + 1][1], -1);
    //            }

    //            string description = ch.psoft.db.SQLColumn.GetValid(dutysandgroupsTable.Rows[i][2], "");
    //            string numtitle = ch.psoft.db.SQLColumn.GetValid(dutysandgroupsTable.Rows[i][3], "");

    //            //if is dutygroup
    //            if (isduty == 0 && isdutynext != 0)
    //            {
    //                appendVSpace(13);
    //                _nTable = AppendDutyTable();
    //                _nTable.Attributes.Append(createAttribute("keep-together", "true"));
    //                _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
    //                _nRow = _nTable.AppendChild(createRow());
    //                _nRow.Attributes.Append(createClassAttribute("spzTitle"));
    //                _nCell = _nRow.AppendChild(createCell());
    //                _nCell.Attributes.Append(createAttribute("colspan", "2"));
    //                _nCell.InnerText = description;
    //            }
    //            else if (isduty == 1)
    //            {
    //                if (FBSModule.showNumTitleInReport)
    //                {
    //                    _nRow = _nTable.AppendChild(createRow());
    //                    _nCell = _nRow.AppendChild(createCell());
    //                    _nCell.Attributes.Append(createClassAttribute("subTitle"));
    //                    _nCell.Attributes.Append(createAttribute("colspan", "2"));
    //                    _nCell.InnerText = numtitle;
    //                }

    //                _nRow = _nTable.AppendChild(createRow());
    //                _nCell = _nRow.AppendChild(createCell());
    //                _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //                addCellText(_nCell, description);

    //                _nCell = _nRow.AppendChild(createCell());
    //                _nCell.Attributes.Append(createAttribute("align", "right"));
    //                _nCell.Attributes.Append(createClassAttribute("detailValue"));

    //                bool isFirst = true;
    //                foreach (DataRow clRow in _competenceLevels.Rows)
    //                {
    //                    if (ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "COMPETENCE", "DUTY_COMPETENCE_VALIDITY_ID=" + dutyid + " and COMPETENCE_LEVEL_ID=" + clRow["ID"], false), -1) > 0)
    //                    {
    //                        if (isFirst)
    //                        {
    //                            isFirst = false;
    //                        }
    //                        else
    //                        {
    //                            _nCell.InnerText += " ";
    //                        }

    //                        _nCell.InnerText += clRow[_db.langAttrName(clRow.Table.TableName, "MNEMO")].ToString();
    //                    }
    //                }
    //            }
    //        }




    //        // Schluss
    //        _nTable = AppendDetailTable();
    //        _nTable.Attributes.Append(createAttribute("keep-together", "true"));

    //        _nRow = _nTable.AppendChild(createRow());
    //        _nRow.Attributes.Append(createAttribute("padding-bottom", "20"));
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("colspan", "2"));
    //        _nCell.Attributes.Append(createAttribute("padding-top", "20"));




    //        XmlNode nTable = createTable();
    //        //nTable.Attributes.Append(createAttribute("widths", "20%,20%,30%,30%"));
    //        nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
    //        nTable.Attributes.Append(createAttribute("border-width-bottom", ".5"));
    //        nTable.Attributes.Append(createAttribute("keep-together", "false"));
         

    //        nTable.Attributes.Append(createAttribute("padding-all", "2"));
    //        nTable.Attributes.Append(createAttribute("align", "left"));
    //        nTable.Attributes.Append(createClassAttribute("detailValue"));
    //        _nTable.Attributes.Append(createAttribute("font-name", "arial"));

    //        _nRow = _nTable.AppendChild(createRow());
    //        //_nCell = _nRow.AppendChild(createCell());
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("align", "center"));
    //        _nCell.InnerText = "Datum";
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("align", "center"));
    //        _nCell.InnerText = "Name";
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("align", "center"));
    //        _nCell.InnerText = "Visum";
    //        _nRow = _nTable.AppendChild(createRow());
    //        _nRow.Attributes.Append(createAttribute("padding-bottom", "10"));
    //        //_nCell = _nRow.AppendChild(createCell());
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("border-width-all", "0.5"));
    //        _nCell.InnerText = "Vorgesetzter";
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("border-width-all", "0.5"));
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("border-width-all", "0.5"));
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("border-width-all", "0.5"));
    //        _nRow = _nTable.AppendChild(createRow());
    //        _nRow.Attributes.Append(createAttribute("padding-bottom", "10"));
    //        //_nCell = _nRow.AppendChild(createCell());
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("border-width-all", "0.5"));
    //        _nCell.InnerText = "Mitarbeiter";
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("border-width-all", "0.5"));
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("border-width-all", "0.5"));
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("border-width-all", "0.5"));


    //        //Erstellungsdatum
    //        _nRow = _nTable.AppendChild(createRow());

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //        _nCell.InnerText = _map.get("laufenburg", "reportDate");

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //        _nCell.InnerText = _reportDate;

    //        //Druckdatum, cst, 071122
    //        _nRow = _nTable.AppendChild(createRow());

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //        _nCell.InnerText = _map.get("laufenburg", "printDate");

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //        _nCell.InnerText = DateTime.Now.ToString("dd.MM.yyyy");


    //        _nRow = _nTable.AppendChild(createRow());
    //        _nRow.Attributes.Append(createAttribute("padding-bottom", "36"));
    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createAttribute("colspan", "2"));
    //        _nCell.Attributes.Append(createAttribute("padding-top", "36"));

    //        _nRow = _nTable.AppendChild(createRow());

    //        _nCell = _nRow.AppendChild(createCell());
    //        _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //        _nCell.Attributes.Append(createAttribute("colspan", "2"));
    //        _nCell.InnerText = _map.get("laufenburg", "remark");
    //    }

    //    /// <summary>
    //    /// Firma ist OE unter Root. (Gemäss M. Steinacher, 22.9.2006)
    //    /// </summary>
    //    /// <param name="orgentityId"></param>
    //    /// <returns></returns>
    //    private string getFirma(long orgentityId)
    //    {
    //        string returnValue = "";
    //        long id = orgentityId;
    //        long root_id = -1;
    //        object[] values = null;

    //        while (id != root_id && id != -1)
    //        {
    //            values = _db.lookup(
    //                    new string[] { "PARENT_ID", "ROOT_ID", _db.langAttrName("FUNKTION", "TITLE") },
    //                    "ORGENTITY",
    //                    "ID = " + id
    //                );
    //            id = DBColumn.GetValid(values[0], (long)-1);
    //            root_id = DBColumn.GetValid(values[1], (long)-1);
    //        }

    //        if (values != null)
    //        {
    //            returnValue = DBColumn.GetValid(values[2], "");
    //        }

    //        return returnValue;
    //    }

    //    /// <summary>
    //    /// Aufgabengruppe
    //    /// </summary>
    //    /// <param name="table"></param>
    //    /// <param name="isFirstGrp"></param>
    //    /// <param name="grpRow"></param>
    //    private void addGroup(DataTable table, ref bool isFirstGrp, DataRow grpRow)
    //    {
    //        if (table.Rows.Count > 0)
    //        {
    //            if (isFirstGrp)
    //            {
    //                isFirstGrp = false;
    //            }

    //            appendVSpace(13);
    //            _nTable = AppendDutyTable();
    //            _nTable.Attributes.Append(createAttribute("keep-together", "true"));
    //            _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
    //            _nRow = _nTable.AppendChild(createRow());
    //            _nRow.Attributes.Append(createClassAttribute("spzTitle"));
    //            _nCell = _nRow.AppendChild(createCell());
    //            _nCell.Attributes.Append(createAttribute("colspan", "2"));

    //            if (grpRow != null)
    //            {

    //                _nCell.InnerText = " "
    //                    + ch.psoft.Util.Validate.GetValid(
    //                        grpRow[_db.langAttrName(grpRow.Table.TableName, "TITLE")].ToString(),
    //                        ""
    //                    );
    //            }
    //            else
    //            {
    //                _nCell.InnerText = _map.get(
    //                        FBSModule.LANG_SCOPE_FBS,
    //                        FBSModule.LANG_MNEMO_REP_DUTYGROUP_FREE
    //                    );
    //            }

    //            foreach (DataRow dcvRow in table.Rows)
    //            {
    //                if (FBSModule.showNumTitleInReport)
    //                {
    //                    _nRow = _nTable.AppendChild(createRow());
    //                    _nCell = _nRow.AppendChild(createCell());
    //                    _nCell.Attributes.Append(createClassAttribute("subTitle"));
    //                    _nCell.Attributes.Append(createAttribute("colspan", "2"));
    //                    _nCell.InnerText = ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "NUM_TITLE")].ToString(), "");
    //                }

    //                _nRow = _nTable.AppendChild(createRow());
    //                _nCell = _nRow.AppendChild(createCell());
    //                _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //                addCellText(_nCell, ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "DESCRIPTION")].ToString(), ""));

    //                _nCell = _nRow.AppendChild(createCell());
    //                _nCell.Attributes.Append(createAttribute("align", "right"));
    //                _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //                bool isFirst = true;

    //                foreach (DataRow clRow in _competenceLevels.Rows)
    //                {
    //                    if (ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "COMPETENCE", "DUTY_COMPETENCE_VALIDITY_ID=" + dcvRow["ID"] + " and COMPETENCE_LEVEL_ID=" + clRow["ID"], false), -1) > 0)
    //                    {
    //                        if (isFirst)
    //                        {
    //                            isFirst = false;
    //                        }
    //                        else
    //                        {
    //                            _nCell.InnerText += " ";
    //                        }

    //                        _nCell.InnerText += clRow[_db.langAttrName(clRow.Table.TableName, "MNEMO")].ToString();
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Gruppe im Anforderungsprofil
    //    /// </summary>
    //    /// <param name="table"></param>
    //    /// <param name="isFirstGrp"></param>
    //    /// <param name="grpRow"></param>
    //    private void addSkillGroup(DataTable table, ref bool isFirstGrp, DataRow grpRow)
    //    {
    //        if (table.Rows.Count > 0)
    //        {
    //            if (isFirstGrp)
    //            {
    //                isFirstGrp = false;
    //            }
    //            else
    //            {
    //                appendVSpace(13);
    //            }

    //            _nTable = AppendSkillTable();
    //            _nTable.Attributes.Append(createAttribute("keep-together", "true"));
    //            _nTable.Attributes.Append(createAttribute("border-width-outer", ".5"));
    //            _nRow = _nTable.AppendChild(createRow());
    //            _nRow.Attributes.Append(createClassAttribute("spzTitle"));
    //            _nCell = _nRow.AppendChild(createCell());
    //            _nCell.Attributes.Append(createAttribute("colspan", "2"));

    //            if (grpRow != null)
    //            {
    //                _nCell.InnerText = " "
    //                    + ch.psoft.Util.Validate.GetValid(
    //                        grpRow[_db.langAttrName(grpRow.Table.TableName, "TITLE")].ToString(),
    //                        ""
    //                    );


    //            }
    //            else
    //            {
    //                _nCell.InnerText = _map.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_SKILLGROUP_FREE);
    //            }

    //            foreach (DataRow dcvRow in table.Rows)
    //            {
    //                if (SkillsModule.showNumTitleInReport)
    //                {
    //                    _nRow = _nTable.AppendChild(createRow());
    //                    _nCell = _nRow.AppendChild(createCell());
    //                    _nCell.Attributes.Append(createClassAttribute("subTitle"));
    //                    _nCell.Attributes.Append(createAttribute("colspan", "2"));
    //                    _nCell.InnerText = ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "NUM_TITLE")].ToString(), "");
    //                }
    //                //cst, 071120 - gar kein Titel, wenn der Parameter 0 ist anzeigen
    //                /*
    //                else
    //                {
    //                    _nRow = _nTable.AppendChild(createRow());
    //                    _nCell = _nRow.AppendChild(createCell());
    //                    _nCell.Attributes.Append(createClassAttribute("subTitle"));
    //                    _nCell.Attributes.Append(createAttribute("colspan", "2"));
    //                    _nCell.InnerText = ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "TITLE")].ToString(), "");
    //                }*/

    //                _nRow = _nTable.AppendChild(createRow());
    //                _nCell = _nRow.AppendChild(createCell());
    //                _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //                addCellText(_nCell, ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "DESCRIPTION")].ToString(), ""));

    //                _nCell = _nRow.AppendChild(createCell());
    //                _nCell.Attributes.Append(createClassAttribute("detailValue"));
    //                _nCell.Attributes.Append(createAttribute("align", "right"));
    //                _nCell.InnerText = ch.psoft.Util.Validate.GetValid(dcvRow[_db.langAttrName(dcvRow.Table.TableName, "DEMAND_LEVEL_TITLE")].ToString(), "");

    //            }
    //        }
    //    }
    }
}
