using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using HiQPdf;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ch.appl.psoft.Energiedienst
{
    /// <summary>
    /// Summary description for PersonObjectiveReport.
    /// </summary>
    public class FunctionJobDescriptionReport : PsoftPDFReport
    {
        private long _jobID = 0;
        private string _dateFormat = "";
        private string _imagePath = "";
        private DataTable _data = null;
        protected long _employment_ID = -1;
        protected long _personId = -1;
        protected string _onloadString; 
        protected long _funktionID = -1;
        protected int _groupNumber = 0;
        protected string _reportDate = DateTime.Now.ToString("d");
        protected bool isFirstGrp = true;
        protected string tmpSqltableName;
        private HtmlGenericControl baseDiv;

        public FunctionJobDescriptionReport(HttpSessionState Session, string imagePath) : base(Session, imagePath) {
               
        }

           public void writeHead(long jobId, long personId, long employmentId, long funktionId, string path, string imagepath)
           { //DataTable data, string turnid
            _jobID = jobId;
            _personId = personId;
            _employment_ID = employmentId;
            _funktionID = funktionId;
            _imagePath = imagepath;

            _dateFormat = _db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern;

            

            writeHeader();                        
           }

        private void writeHeader() {
            // Name
            String stelleninhaber = _db.Person.getWholeName(_personId.ToString(), false, true, false);
            //Gültig ab
            String gueltigAb = _db.lookup("MAX(LAST_CHANGE)", "DUTY_VALIDITY", "DUTY_ID IN (SELECT DUTY_ID FROM DUTY_COMPETENCE_VALIDITY WHERE JOB_ID = " + _jobID + ")").ToString();
            if (!gueltigAb.Equals(""))
            {
                gueltigAb = gueltigAb.Substring(0, 10);
            }
            //Beschäftigungsgrad
            //BG.Text = ch.psoft.Util.Validate.GetValid(_db.lookup("ENGAGEMENT", "JOB", "ID=" + _jobID, ""), 0).ToString() +"%";
            //Geburtsdatum
            //if (!_db.lookup("DATEOFBIRTH", "PERSON", "ID = " + _personId.ToString()).ToString().Equals(""))
            //{
            //    Geburtsdatum.Text = _db.lookup("DATEOFBIRTH", "PERSON", "ID = " + _personId.ToString()).ToString().Substring(0, 10);
            //}
            //Funktion und Stellenziele
            object[] values = _db.lookup(
                   new string[] 
                    {
                        _db.langAttrName("FUNKTION", "TITLE"),
                        _db.langAttrName("FUNKTION", "DESCRIPTION")
                    },
                   "FUNKTION",
                   "ID = " + _funktionID
               );
            String funktion = DBColumn.GetValid(values[0], "");
            //Stellenziele.Text = _dbColumn.GetValid(values[1], "");
            //Arbeitort /Bereich / Bereichskürzel / Kst

            String arbeitsort = _db.lookup("LOCATION", "PERSON", "ID = " + _personId).ToString();
            values = _db.lookup(new string[] { "O.ID", "O." + _db.langAttrName("FUNKTION", "TITLE"), "O." + _db.langAttrName("ORGENTITY", "MNEMONIC") }, "ORGENTITY O inner join JOB J on O.ID = J.ORGENTITY_ID", "J.ID = " + _jobID);
            String bereich = DBColumn.GetValid(values[1], "");
            String bereichskuerzel = DBColumn.GetValid(values[2], "");
            String kst = _db.lookup("COSTCENTER_TITLE", "PERSON", "ID=" + _personId).ToString();
            long orgentityId = DBColumn.GetValid(values[0], (long)-1);
            //Vorgesetzte Funktion
            DataTable tab = _db.getDataTable("select dbo.GET_LEADERFUNCTIONID(" + _personId + ")", Logger.VERBOSE);
            DataTable tab1 = _db.getDataTable("select dbo.GET_LEADERPERSONID(" + _personId + ")", Logger.VERBOSE);
            long leaderFunctionID = ch.psoft.db.SQLColumn.GetValid(tab.Rows[0][0], 0L);
            String LeaderfullName = _db.Person.getWholeName(ch.psoft.db.SQLColumn.GetValid(tab1.Rows[0][0], "").ToString(), false, true, false);

            string sqlFunction = "select distinct F." + _db.langAttrName("FUNKTION", "TITLE")
                        + " from FUNKTION F WHERE"
                        + " F.ID = " + leaderFunctionID;

            //

            DataTable funktionTable = _db.getDataTable(
                    sqlFunction,
                    "FUNKTION"
                );
            String vorgesetzter = LeaderfullName;
            //foreach (DataRow dRow in funktionTable.Rows)
            //{
            //    if (!object.Equals(dRow[0].ToString(), funktion))
            //    {
            //       vorgesetzter = LeaderfullName + " / " + dRow[0].ToString();
            //    }
            //}

            //Vertritt

            String vertritt = _db.lookup("PROXY_TO", "PERSON", "ID = " + _personId).ToString().Replace("\r\n","<br />");

            //Stellvertretung

            String stellvertretung = _db.lookup("PROXY_ME", "PERSON", "ID = " + _personId).ToString().Replace("\r\n", "<br />");
            
            //Unterstellte Bereiche
            String unterstellteBereiche = "";
            if (ch.psoft.Util.Validate.GetValid(_db.lookup("TYP", "JOB", "ID=" + _jobID, ""), 0) == 1)
            {
                DataTable subOETable = _db.getDataTable(
                        "select " + _db.langAttrName("ORGENTITY", "TITLE")
                            + " from ORGENTITY"
                            + " where PARENT_ID = " + orgentityId + "OR ID = " + orgentityId
                            + " order by ORDNUMBER",
                        "ORGENTITY"
                    );


                string UB = "";
                foreach (DataRow dRow in subOETable.Rows)
                {
                    UB += dRow[0].ToString() + " \r\n";
                }
                UB = UB.Substring(0, UB.Length - 3);
                unterstellteBereiche = UB.Replace("\r\n", "<br />");

              
            }

            //Schlüsselkontakte / Stellenkurzbeschreibung
            String schluesselkontakte = _db.lookup("KEYCONTACTS", "JOB", "ID = " + _jobID).ToString().Replace("\r\n", "<br />");
            String stellenkurzbeschreibung = _db.lookup("JOB_DESCRIPTION_SHORT", "JOB", "ID = " + _jobID).ToString();

            base.appendTextBlock("Aufgabenbeschreibung");

            baseDiv = new HtmlGenericControl("div");

            HtmlImage logo = new HtmlImage();
            logo.Src = _imagePath;
            logo.Attributes.Add("Class", "Logo");
            baseDiv.Controls.Add(logo);

            HtmlGenericControl title = new HtmlGenericControl("h1");
            title.InnerText = "Aufgabenbeschreibung";
            baseDiv.Controls.Add(title);

            HtmlTable table = new HtmlTable();
            HtmlTableRow row = new HtmlTableRow();
           
            HtmlTableCell cell = new HtmlTableCell();
           
            table.Style.Add("border-width-bottom", "1");
            baseDiv.Controls.Add(table);


            // person name
           
            table.Controls.Add(row);
             row.Controls.Add(cell);
            cell.Attributes.Add("Class","detailName");
            cell.Style.Add("font-size", "10");
            cell.InnerText = "Name:";
            cell = new HtmlTableCell();  row.Controls.Add(cell);
            cell.Attributes.Add("Class","detailValue");
            cell.Style.Add("font-size", "10");
            cell.Style.Add("column-span", "3");
            cell.InnerText = stelleninhaber;


            // Function + date
            row = headerLine("Funktion:", funktion, "Gültig ab:", gueltigAb);
            table.Controls.Add(row);

            // Bereich
            row = headerLine("Bereich:", bereich, "Bereichskürzel:", bereichskuerzel);
            table.Controls.Add(row);

            // Arbeitsort + Kostenstelle
            row = headerLine("Arbeitsort:", arbeitsort, "Kostenstelle:", kst);
            table.Controls.Add(row);



            // second part of header
            table =new HtmlTable();
            baseDiv.Controls.Add(table);

            table.Style.Add("border-width-bottom", "1");

            row = headerLine2("ist überstellt:", unterstellteBereiche);
            table.Controls.Add(row);

            row = headerLine2("ist unterstellt:", vorgesetzter);
            table.Controls.Add(row);

            row = headerLine2("wird vertreten durch:", stellvertretung);
            table.Controls.Add(row);

            row = headerLine2("vertritt:", vertritt);
            table.Controls.Add(row);

            row = new HtmlTableRow();
            table.Controls.Add(row);
            cell = new HtmlTableCell();  row.Controls.Add(cell);
            cell.Attributes.Add("Class","detailName");
            cell.Style.Add("font-size", "10");
           // cell.Style.Add("height", "5px");
            cell.InnerText = "Schlüsselkontakte (extern):";
            cell = new HtmlTableCell();  row.Controls.Add(cell);
            cell.Attributes.Add("Class","detailValue");
            cell.Style.Add("font-size", "10");           
            cell.RowSpan=2;
            cell.InnerHtml = schluesselkontakte;
            row = new HtmlTableRow();
            row.Style.Add("height", "100%");
            table.Controls.Add(row);
            cell = new HtmlTableCell();  row.Controls.Add(cell);
            cell.Attributes.Add("Class","detailValue");
            cell.Style.Add("font-size", "8");
            cell.Style.Add("padding-top", "0px");
            cell.Style.Add("height", "100%");
            cell.InnerText = "wo von unternehmerischer Bedeutung";

            row = headerLine2("Stellenkurzbeschreibung:", stellenkurzbeschreibung);
            table.Controls.Add(row);
             
            

            var sb = new StringBuilder();
           baseDiv.RenderControl(new HtmlTextWriter(new StringWriter(sb)));
           string style = File.ReadAllText(Global.Config.directory+"../../Energiedienst/JobDescriptionReportStyle.css");

            // style
            string s = "<head><style type=\"text/css\">"+style+"</style></head><div>";

            

            // content
            s+=    sb.ToString();

            // Aufgaben/ Hauptverantwortlichkeiten
            s += "<h2>Aufgaben / Hauptverantwortlichkeiten</h2>";
            s += Convert.ToString(_db.lookup("TOP (1) DUTY_VALIDITY.DESCRIPTION_DE", "DUTY_VALIDITY INNER JOIN DUTY ON DUTY_VALIDITY.DUTY_ID = DUTY.ID INNER JOIN DUTY_COMPETENCE_VALIDITY ON DUTY.ID = DUTY_COMPETENCE_VALIDITY.DUTY_ID INNER JOIN JOB ON DUTY_COMPETENCE_VALIDITY.JOB_ID = dbo.JOB.ID", "JOB.ID = " + _jobID))+"</div>";
            
            
            baseDiv =new HtmlGenericControl("div");
            baseDiv.InnerHtml = s;


            //Footer
            String Erstellungsdatum = _db.lookup("JOB_DESCRIPTION_RELEASE", "JOB", "ID =" + _jobID).ToString();
            if (Erstellungsdatum.Length > 0)
            {
                Erstellungsdatum = Erstellungsdatum.Substring(0, 10);
            }
            String Ersteller = "";
            String ErstellerVisum = "";
            String EingesehenDatum = "";
            String StelleninhaberName = "";
            String StelleninhaberVisum = "";

            string leaderId = _db.lookup("JOB_DESCRIPTION_RELEASE_PERSON","JOB","ID =" + _jobID).ToString();
            if (!Erstellungsdatum.Equals("")&& !leaderId.Equals(""))
            {
                Ersteller = _db.lookup("FIRSTNAME", "PERSON", "ID = " + leaderId) + " " + _db.lookup("PNAME", "PERSON", "ID = " + leaderId);
                ErstellerVisum = _db.lookup("MNEMO", "PERSON", "ID = " + leaderId).ToString();
            }

            if (!_db.lookup("JOB_DESCRIPTION_CHECKED", "JOB", "ID = " + _jobID).ToString().Equals(""))
            {
                EingesehenDatum = _db.lookup("JOB_DESCRIPTION_CHECKED", "JOB", "ID = " + _jobID).ToString().Substring(0, 10);
                StelleninhaberName = _db.lookup("FIRSTNAME", "PERSON", "ID = " + _personId) + " " + _db.lookup("PNAME", "PERSON", "ID = " + _personId);
                StelleninhaberVisum = _db.lookup("MNEMO", "PERSON", "ID = " + _personId).ToString();
            }
            HtmlGenericControl footerDiv = new HtmlGenericControl("div");
            footerDiv.Attributes.Add("Class", "FooterDiv");
            baseDiv.Controls.Add(footerDiv);
            table = new HtmlTable();
            table.Attributes.Add("Class", "FooterTable");
            footerDiv.Controls.Add(table);
            row = new HtmlTableRow();
            table.Controls.Add(row);
            cell = new HtmlTableCell();
            row.Controls.Add(cell);
            cell = new HtmlTableCell();
            cell.InnerText = "Datum";
            row.Controls.Add(cell);
            cell = new HtmlTableCell();
            row.Controls.Add(cell);
            cell.InnerText = "Name";
            cell = new HtmlTableCell();
            row.Controls.Add(cell);
            cell.InnerText = "Visum";

            row = new HtmlTableRow();
            row.Attributes.Add("Class", "FooterContentRow");
            table.Controls.Add(row);
            cell = new HtmlTableCell();
            cell.InnerText = "Vorgesetzte/r";
            row.Controls.Add(cell);
            cell = new HtmlTableCell();
            cell.InnerText = Erstellungsdatum;
            row.Controls.Add(cell);
            cell = new HtmlTableCell();
            row.Controls.Add(cell);
            cell.InnerText = Ersteller;
            cell = new HtmlTableCell();
            row.Controls.Add(cell);
            cell.InnerText = ErstellerVisum;

            row = new HtmlTableRow();
            row.Attributes.Add("Class", "FooterContentRow");
            table.Controls.Add(row);
            cell = new HtmlTableCell();
            cell.InnerText = "Mitarbeiter/in";
            row.Controls.Add(cell);
            cell = new HtmlTableCell();
            cell.InnerText = EingesehenDatum;
            row.Controls.Add(cell);
            cell = new HtmlTableCell();
            row.Controls.Add(cell);
            cell.InnerText = StelleninhaberName;
            cell = new HtmlTableCell();
            row.Controls.Add(cell);
            cell.InnerText = StelleninhaberVisum;

            HtmlGenericControl footerTextDiv = new HtmlGenericControl("div");
            footerTextDiv.Attributes.Add("Class", "FooterTextDiv");
            footerTextDiv.InnerHtml = "* Die Zielvereinbarung erfolgt im Zuge des Orientierungsgesprächs<br />Bei der Aufgabenerfüllung steht das Gesamtinteresse des Unternehmens im Vordergrund. Dazu gehören die<br />Übernahme von weiteren Aufgaben, die Mitwirkung in Projekten, die bereichsübergreifende<br />Zusammenarbeit und die Anpassung der Aufgabenbeschreibung an neue Bedürfnisse.";
            footerDiv.Controls.Add(footerTextDiv);
        } 
        
        public bool saveReport(string outputDirectory, string pdfName)
        {
            
            HtmlToPdf pdfConverter = new HtmlToPdf();
            var sb = new StringBuilder();
            baseDiv.RenderControl(new HtmlTextWriter(new StringWriter(sb)));
            String html = sb.ToString();
            pdfConverter.SerialNumber = "5KyNtbSA-gqiNhpaF-lp3dytTE-1cTVxNDd-3NzE19XK-1dbK3d3d-3Q==";
            pdfConverter.BrowserWidth = 595;
            pdfConverter.Document.ImagesCompression = 0;
            pdfConverter.Document.Compress = false;
            //pdfConverter.Document.ForceFitPageWidth = true;
            pdfConverter.Document.ImagesCutAllowed = false;
            pdfConverter.Document.Margins.Bottom = 20;
            pdfConverter.Document.Margins.Top = 20;
            pdfConverter.Document.Margins.Left = 20;
            pdfConverter.Document.Margins.Right = 20;
            pdfConverter.Document.PageSize = PdfPageSize.A4;
            pdfConverter.Document.ForceFitPageWidth = true;
           
            SetFooter(pdfConverter.Document);
            PdfDocument duty = pdfConverter.ConvertHtmlToPdfDocument(html,null);
            
            duty.WriteToFile(outputDirectory + "\\" + pdfName + ".pdf");
            return true;
        }
        private HtmlTableRow headerLine(string name1, string value1, string name2, string value2)
        {
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell;
            cell = new HtmlTableCell();  row.Controls.Add(cell);
            cell.Attributes.Add("Class","detailName");
            cell.Style.Add("font-size", "10");
            cell.InnerText = name1;
            cell = new HtmlTableCell();  row.Controls.Add(cell);
            cell.Attributes.Add("Class","detailValue");
            cell.Style.Add("font-size", "10");
            cell.InnerText = value1;
            cell = new HtmlTableCell();  row.Controls.Add(cell);
            cell.Attributes.Add("Class","detailName");
            cell.Style.Add("font-size", "10");
            cell.InnerText = name2;
            cell = new HtmlTableCell();  row.Controls.Add(cell);
            cell.Attributes.Add("Class","detailValue");
            cell.Style.Add("font-size", "10");
            cell.InnerText = value2;
            return row;
        }

        private HtmlTableRow headerLine2(string name1, string value1)
        {
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell;
            cell = new HtmlTableCell();  row.Controls.Add(cell);
            cell.Attributes.Add("Class","detailName");
            cell.Style.Add("font-size", "10");
          
            cell.InnerText = name1;
            cell = new HtmlTableCell();  row.Controls.Add(cell);
            cell.Attributes.Add("Class","detailValue");
            cell.Style.Add("font-size", "10");
            cell.InnerHtml = value1;
            return row;
        }

        

        private void SetFooter(PdfDocumentControl htmlToPdfDocument)
        {
            // enable footer display
            htmlToPdfDocument.Footer.Enabled = true;

            // set footer height
            htmlToPdfDocument.Footer.Height = 20;

            // set footer background color
           

            float pdfPageWidth = 
                    htmlToPdfDocument.PageOrientation == PdfPageOrientation.Portrait ?
                    htmlToPdfDocument.PageSize.Width : htmlToPdfDocument.PageSize.Height;

            float footerWidth = pdfPageWidth - htmlToPdfDocument.Margins.Left - htmlToPdfDocument.Margins.Right;
            float footerHeight = htmlToPdfDocument.Footer.Height;

            // layout HTML in footer
            PdfHtml footerHtml = new PdfHtml(0, 0, "<span style=\"font-size: 20px; font-family:Verdana;\">" + System.DateTime.Now.ToString("d. MMMM yyyy", new System.Globalization.CultureInfo("de-DE")) + "</span>", null);
            footerHtml.FitDestHeight = true;
            htmlToPdfDocument.Footer.Layout(footerHtml);

            System.Drawing.Font pageNumberingFont =
                              new System.Drawing.Font(new System.Drawing.FontFamily("Verdana"),
                                          6, System.Drawing.GraphicsUnit.Point);
            PdfText pageNumberingText = new PdfText(0, 2,
                    "Seite {CrtPage} von {PageCount}", pageNumberingFont);
            pageNumberingText.HorizontalAlign = PdfTextHAlign.Right;
            pageNumberingText.EmbedSystemFont = true;
            htmlToPdfDocument.Footer.Layout(pageNumberingText);

            // create a border for footer
            PdfRectangle borderRectangle = new PdfRectangle(0, 0, footerWidth, 0);
            borderRectangle.LineStyle.LineWidth = 0.5f;
            borderRectangle.ForeColor = System.Drawing.Color.Black;
            htmlToPdfDocument.Footer.Layout(borderRectangle);    
        }

        public void saveHistorie(long jobId, long personId)
        {
            _db.connect();
            Config config = Global.Config;
            ChangeLogo logo = new ChangeLogo();

            writeHead(jobId, personId, (long)_db.lookup("EMPLOYMENT_ID", "JOB", "ID = " + jobId, -1L), (long)_db.lookup("FUNKTION_ID", "JOB", "ID = " + jobId, -1L), config.ftpDocumentSaveDirectory, config.logoImageDirectory +"//"+ logo.getLogoFilename(_db, personId));

            long folderRootId = _db.lookup("folder_id", "clipboard", "id =" + _db.lookup("CLIPBOARD_ID", "PERSON", "ID=" + personId, 0L), 0L);
            long folder_id = _db.lookup("id", "folder", "(root_id ='" + folderRootId + "') and (Title = 'Historie Aufgabenbeschreibung')", 0L);
            string title = "Aufgabenbeschreibung " + DateTime.Now.ToString("yy.MM.dd");
            string file = "Aufgabenbeschreibung" + DateTime.Now.ToString("yy-MM-dd");
            long xid = _db.newId("Document");
            string xFilename = _db.Document.EncodeXFileName(xid, file);
            saveReport(config.documentSaveDirectory, xFilename);

            string sql = "insert into document (ID, FOLDER_ID,INHERIT,TITLE,DESCRIPTION,AUTHOR,FILENAME,XFILENAME,VERSION,CREATED,CHECKOUT_STATE,CHECKIN_PERSON_ID,TYP,NUMOFDOCVERSIONS)  VALUES(" + xid + "," + folder_id + ",'1','" + title + "','','','" + file + ".pdf" + "','" + xFilename + ".pdf" + "','1','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','0','" + _db.userId + "','0','1')";
            _db.execute(sql);
            _db.disconnect();

        }
     }

}