using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;

namespace ch.appl.psoft.Energiedienst.Controls
{
    public partial class PrintJobDescriptionCtrl : PSOFTMapperUserControl
    {

        protected int _jobID = -1;
        protected long _employment_ID = -1;
        protected long _personId = -1;
        protected string _onloadString;
        protected long _funktionID = -1;
        protected int _groupNumber = 0;
        protected string _reportDate = DateTime.Now.ToString("d");
        protected bool isFirstGrp = true;
        protected string tmpSqltableName;
        protected long _dutyValidityId = 0L;
        protected string employeeName;
        protected string imagename;
        protected DBData _db;
        public static string Path
        {
            get { return Global.Config.baseURL + "/Energiedienst/Controls/PrintJobDescriptionCtrl.ascx"; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY);
            _jobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], -1);
            _db = DBData.getDBData(Session);
            _employment_ID = (long)_db.lookup("EMPLOYMENT_ID", "JOB", "ID = " + _jobID, -1L);
            _personId = (long)_db.lookup("PERSON_ID", "EMPLOYMENT", "ID = " + _employment_ID, -1L);
            _funktionID = (long)_db.lookup("FUNKTION_ID", "JOB", "ID = " + _jobID, -1L);

            //FunctionJobDescriptionReport report = new FunctionJobDescriptionReport(Session, "");
            //ChangeLogo logo = new ChangeLogo();
            //report.writeHead(_jobID, _personId, _employment_ID, _funktionID, outputDirectory, Server.MapPath(Global.Config.baseURL + "/images/"+ logo.getLogoFilename(db,_personId)));
            //string imageDirectory = Request.MapPath("~/images");
            //string reportfile = Server.MapPath(Global.Config.baseURL + "/crystalreports/StellenbeschreibungEnergiedienst.rpt");
            //fileName = "averagePerformance" + _funktionID;

            //report.saveReport(outputDirectory, fileName);
            ChangeLogo logo = new ChangeLogo();
            imagename = logo.getLogoFilename(_db, _personId).Split('.')[0];

            if (!Page.IsPostBack)
            {
                updatePdfView();
            }

            //pdfFrame.Attributes.Add("src", Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + fileName + ".pdf");


            FBSGeprueft.Text = "Eingesehen und geprüft von " + _db.lookup("Firstname", "Person", "ID = " + _personId).ToString() + " " + _db.lookup("PNAME", "Person", "ID = " + _personId).ToString();
            if (_db.lookup("JOB_DESCRIPTION_CHECKED", "JOB", "ID=" + _jobID).ToString().Equals(""))
            {
                FBSGeprueft.Checked = false;
            }
            else
            {
                FBSGeprueft.Checked = true;
            }
            FBSGeprueft.CheckedChanged += new EventHandler(FBW_Checked);
            if (_db.userId == _personId && FBSGeprueft.Checked == false)
            {
                FBSGeprueft.Enabled = true;
            }
            else
            {
                FBSGeprueft.Enabled = false;
            }
        }

        protected void FBW_Checked(object sender, System.EventArgs e)
        {
            DateTime time = System.DateTime.Now;
            _db.connect();

            _db.execute("UPDATE JOB SET JOB_DESCRIPTION_CHECKED = '" + time.ToString("MM.dd.yyyy") + "' WHERE ID = " + _jobID);
            string toID = _db.lookup("ID", "Person", " id = (SELECT JOB_DESCRIPTION_RELEASE_PERSON FROM JOB WHERE ID =" + _jobID + ")").ToString();
            long personId = (long)_db.lookup("ID", "Person", " id = (SELECT PERSON.ID FROM JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID  WHERE JOB.ID =" + _jobID + ")");

            string fileName = _db.Person.getWholeName(_personId.ToString(), false, true, false).Replace(" ", "");
            String path = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY);

            String pdfPath = path + "\\" + fileName + ".pdf";

            Config config = Global.Config;

            long folderRootId = _db.lookup("folder_id", "clipboard", "id =" + _db.lookup("CLIPBOARD_ID", "PERSON", "ID=" + _personId, 0L), 0L);
            long folder_id = _db.lookup("id", "folder", "(root_id ='" + folderRootId + "') and (Title = 'Historie Aufgabenbeschreibung')", 0L);
            string title = "Aufgabenbeschreibung " + DateTime.Now.ToString("yy.MM.dd");
            string file = "Aufgabenbeschreibung" + DateTime.Now.ToString("yy-MM-dd");
            _db.disconnect();
            _db = DBData.getDBData(Session);
            _db.connect();
            long xid = _db.newId("Document");
            string xFilename = _db.Document.EncodeXFileName(xid, file);

            File.Copy(pdfPath, config.documentSaveDirectory + "\\" + xFilename+".pdf");

            string sql = "insert into document (ID, FOLDER_ID,INHERIT,TITLE,DESCRIPTION,AUTHOR,FILENAME,XFILENAME,VERSION,CREATED,CHECKOUT_STATE,CHECKIN_PERSON_ID,TYP,NUMOFDOCVERSIONS)  VALUES(" + xid + "," + folder_id + ",'1','" + title + "','','','" + file + ".pdf" + "','" + xFilename + ".pdf" + "','1','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','0','" + _db.userId + "','0','1')";
            _db.execute(sql);

            _db.disconnect();

            sendMail("Statusänderung Aufgabenbeschreibung", "<font face=\"Arial\" size=\"3\">sender hat ihre/seine Aufgabenbeschreibung visiert.<br><br></font><a href=\"https://srv132/p-flow\"><font face=\"Arial\" size=\"3\">Einstiegslink p-flow</font></a><br><br><font face=\"Arial\" size=\"3\">Besten Dank für Ihre Unterstützung.</font>", toID);

        }


        protected void sendMail(string subject, string message, string toID)
        {
            
            
            string _personId = toID;

            MailMessage myMessage = new MailMessage();
            try
            {
            myMessage.From = new MailAddress(_db.lookup("EMAIL", "PERSON", "ID = " + _db.userId).ToString(), _db.lookup("FIRSTNAME", "PERSON", "ID = " + _db.userId).ToString() + " " + _db.lookup("PNAME", "PERSON", "ID = " + _db.userId,"").ToString());
            myMessage.To.Add(_db.lookup("EMAIL", "PERSON", "ID = " + toID).ToString());
                        }
            catch (System.Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
            myMessage.Subject = subject;
            myMessage.IsBodyHtml = true;

            string senderName = _db.lookup("FIRSTNAME", "PERSON", "ID = " + _db.userId).ToString() + " " + _db.lookup("PNAME", "PERSON", "ID = " + _db.userId).ToString();
            string receiverName = _db.lookup("PNAME", "PERSON", "ID = " + _personId).ToString() + " " + _db.lookup("FIRSTNAME", "PERSON", "ID = " + _personId).ToString();
            message = message.Replace("sender", senderName);
            message = message.Replace("receiver", receiverName);

            myMessage.Body = message;
            SmtpClient mySmtpClient = new SmtpClient();
            System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential(Global.Config.getModuleParam("dispatch", "UserName", ""), Global.Config.getModuleParam("dispatch", "passwordFrom", ""));
            mySmtpClient.Host = Global.Config.getModuleParam("dispatch", "smtpServer", "");
            mySmtpClient.UseDefaultCredentials = false;
            mySmtpClient.Credentials = myCredential;
            mySmtpClient.ServicePoint.MaxIdleTime = 1;

            try
            {
                mySmtpClient.Send(myMessage);
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(this.GetType(), "myalert", "alert('Eine e-Mail Benachrichtigung mit der Statusänderung wurde erfolgreich versandt.'); window.location = \"" + Request.Url.AbsoluteUri + "\";", true);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
        }



        protected string getRestriction()
        {
            if (SessionData.showValidDutyCompOnly(Session))
                return "VALID_FROM<=GetDate() and VALID_TO>=GetDate() and DUTY_VALIDITY_VALID_FROM<=GetDate() and DUTY_VALIDITY_VALID_TO>=GetDate() and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
            else
                return "DUTY_VALIDITY_VALID_FROM<=GetDate() and DUTY_VALIDITY_VALID_TO>=GetDate() and (JOB_ID=" + _jobID + " or FUNKTION_ID=" + _funktionID + ")";
        }

        private WordprocessingDocument generateWordDoc(WordprocessingDocument document)
        {
            try { 
            OpenXmlPart part = document.MainDocumentPart.GetPartById("rId1");
            XDocument cDoc = part.LoadXDocument();

            _db = DBData.getDBData(Session);
            _personId = (long)_db.lookup("EMPLOYMENT.PERSON_ID", "JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID", "JOB.ID = " + _jobID);
            object[] values = _db.lookup(new string[] { "O.ID", "O." + _db.langAttrName("FUNKTION", "TITLE"), "O." + _db.langAttrName("ORGENTITY", "MNEMONIC") }, "ORGENTITY O inner join JOB J on O.ID = J.ORGENTITY_ID", "J.ID = " + _jobID);

            String Erstellungsdatum = _db.lookup("JOB_DESCRIPTION_RELEASE", "JOB", "ID =" + _jobID).ToString();
            if (Erstellungsdatum.Length > 0)
            {
                Erstellungsdatum = Erstellungsdatum.Substring(0, 10);
            }
            String Ersteller = " ";
            String ErstellerVisum = " ";
            String EingesehenDatum = " ";
            String StelleninhaberName = " ";
            String StelleninhaberVisum = " ";

            string leaderId = _db.lookup("JOB_DESCRIPTION_RELEASE_PERSON", "JOB", "ID =" + _jobID).ToString();
            if (!Erstellungsdatum.Equals("") && !leaderId.Equals(""))
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

            //fill word document from DB
            foreach (var c in cDoc.Root.Elements())
            {
                switch (c.Name.ToString())
                {
                    case "name":
                        c.Value = _db.Person.getWholeName(_personId.ToString(), false, true, false);
                        break;

                    case "gueltigAb":
                        String gueltigAb = _db.lookup("MAX(LAST_CHANGE)", "DUTY_VALIDITY", "DUTY_ID IN (SELECT DUTY_ID FROM DUTY_COMPETENCE_VALIDITY WHERE JOB_ID = " + _jobID + ")").ToString();
                        if (!gueltigAb.Equals(""))
                        {
                            gueltigAb = gueltigAb.Substring(0, 10);
                        }
                        c.Value = gueltigAb;
                        break;

                    case "function":
                        //Funktion und Stellenziele
                        object[] fvalues = _db.lookup(
                               new string[]
                                {
                        _db.langAttrName("FUNKTION", "TITLE"),
                        _db.langAttrName("FUNKTION", "DESCRIPTION")
                                },
                               "FUNKTION",
                               "ID = " + _funktionID
                           );
                        c.Value = DBColumn.GetValid(fvalues[0], "");
                        break;

                    case "area":
                        c.Value = DBColumn.GetValid(values[1], "");
                        break;

                    case "areaShort":
                        c.Value = DBColumn.GetValid(values[2], "");
                        break;

                    case "workplace":
                        c.Value = _db.lookup("LOCATION", "PERSON", "ID = " + _personId).ToString();
                        break;

                    case "costCenter":
                        c.Value = _db.lookup("COSTCENTER_TITLE", "PERSON", "ID=" + _personId).ToString();
                        break;

                    case "isSuperiorOf":
                        if (ch.psoft.Util.Validate.GetValid(_db.lookup("TYP", "JOB", "ID=" + _jobID, ""), 0) == 1)
                        {
                            DataTable subOETable = _db.getDataTable(
                                    "select " + _db.langAttrName("ORGENTITY", "TITLE")
                                        + " from ORGENTITY"
                                        + " where PARENT_ID = " + DBColumn.GetValid(values[0], "") + "OR ID = " + DBColumn.GetValid(values[0], "")
                                        + " order by ORDNUMBER",
                                    "ORGENTITY"
                                );


                            string UB = "";
                            foreach (DataRow row in subOETable.Rows)
                            {
                                UB += row[0].ToString() + ", \n";
                            }
                            if (UB.Length > 3)
                            {
                                UB = UB.Substring(0, UB.Length - 3);
                            }
                            c.Value = UB;
                        }
                        break;

                    case "superior":
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

                        foreach (DataRow row in funktionTable.Rows)
                        {

                            c.Value = LeaderfullName;// +" / " + row[0].ToString();

                        }
                        break;

                    case "substitute":
                        c.Value = _db.lookup("PROXY_ME", "PERSON", "ID = " + _personId).ToString();
                        break;

                    case "substituteOf":
                        c.Value = _db.lookup("PROXY_TO", "PERSON", "ID = " + _personId).ToString();
                        break;

                    case "keyContacts":
                        c.Value = _db.lookup("KEYCONTACTS", "JOB", "ID = " + _jobID).ToString();
                        break;

                    case "shortJobDescription":
                        c.Value = _db.lookup("JOB_DESCRIPTION_SHORT", "JOB", "ID = " + _jobID).ToString();
                        break;

                    case "jobDescription":
                        _dutyValidityId = (long)_db.lookup("TOP (1) DUTY_VALIDITY.ID", "DUTY_VALIDITY INNER JOIN DUTY ON DUTY_VALIDITY.DUTY_ID = DUTY.ID INNER JOIN DUTY_COMPETENCE_VALIDITY ON DUTY.ID = DUTY_COMPETENCE_VALIDITY.DUTY_ID INNER JOIN JOB ON DUTY_COMPETENCE_VALIDITY.JOB_ID = dbo.JOB.ID", "JOB.ID = " + _jobID, 0L);
                        if (_dutyValidityId == 0)
                        {
                            _db.connect();
                            _db.execute("INSERT INTO DUTY(ORDNUMBER) VALUES(1)");
                            long lastInserted = (long)_db.lookup("MAX(ID)", "DUTY", "");
                            _db.execute("INSERT INTO DUTY_VALIDITY(DUTY_ID,NUMBER,VALID_FROM, VALID_TO)VALUES(" + lastInserted + ", 1,GETDATE(),'2099-12-31')");
                            _db.execute("INSERT INTO DUTY_COMPETENCE_VALIDITY (VALID_FROM, VALID_TO,DUTY_ID,JOB_ID) VALUES (GETDATE(),'2099-12-31'," + lastInserted + "," + _jobID + ")");

                            _db.disconnect();
                            _dutyValidityId = (long)_db.lookup("ID", "DUTY_VALIDITY", "DUTY_ID = " + lastInserted);
                        }

                        c.Value = (string)_db.lookup("TOP (1) DUTY_VALIDITY.DESCRIPTION_DE", "DUTY_VALIDITY INNER JOIN DUTY ON DUTY_VALIDITY.DUTY_ID = DUTY.ID INNER JOIN DUTY_COMPETENCE_VALIDITY ON DUTY.ID = DUTY_COMPETENCE_VALIDITY.DUTY_ID INNER JOIN JOB ON DUTY_COMPETENCE_VALIDITY.JOB_ID = dbo.JOB.ID", "JOB.ID = " + _jobID);

                        break;

                    case "erstellDatum":
                        c.Value = Erstellungsdatum;
                        break;
                    case "ersteller":
                        c.Value = Ersteller;
                        break;
                    case "erstellerVisum":
                        c.Value = ErstellerVisum;
                        break;
                    case "eingesehenDatum":
                        c.Value = EingesehenDatum;
                        break;
                    case "stelleninhaber":
                        c.Value = StelleninhaberName;
                        break;
                    case "stelleninhaberVisum":
                        c.Value = StelleninhaberVisum;
                        break;



                }
                if (String.IsNullOrEmpty(c.Value))
                {
                    c.Value = " ";
                }
            }


            //save
            using (Stream str = part.GetStream(
            FileMode.Create, FileAccess.Write))
            using (XmlWriter xw = XmlWriter.Create(str))
                cDoc.Save(xw);
            document.Close();
            document = null;

        }
            finally
            {
                if (document != null)
                {
                    document.Close();
                    document = null;
                }
}
            
            return document;
        }

        private void updatePdfView()
        {
            string fileName = _db.Person.getWholeName(_personId.ToString(), false, true, false).Replace(" ", "");
            String path = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY);

            String docxPath = path + "\\" + fileName + ".docx";
            String pdfPath = path + "\\" + fileName + ".pdf";

            if (File.Exists(docxPath))
            {
                File.Delete(docxPath);
            }

            if (File.Exists(pdfPath))
            {
                File.Delete(pdfPath);
            }

            File.Copy(Request.MapPath("JobDescription" + imagename + ".docx"), docxPath);
            WordprocessingDocument document = WordprocessingDocument.Open(docxPath, true);

            generateWordDoc(document);
            Convert(docxPath, pdfPath, Word.WdSaveFormat.wdFormatPDF);

            pdfFrame.Attributes.Add("src", Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + fileName + ".pdf");

        }


        private static void Convert(string input, string output, Word.WdSaveFormat format)
        {
            // Create an instance of Word.exe
            Word._Application oWord = new Word.Application();
            Word._Document oDoc = null;

            // Interop requires objects.
            object oMissing = System.Reflection.Missing.Value;
            object isVisible = true;
            object readOnly = false;
            object oInput = input;
            object oOutput = output;
            object oFormat = format;

            try
            {
                // Make this instance of word invisible (Can still see it in the taskmgr).
                oWord.Visible = false;



                // Load a document into our instance of word.exe
                oDoc = oWord.Documents.Open(ref oInput);

                // Make this document the active document.
                oDoc.Activate();
                
                System.Threading.Thread.Sleep(1000);
                // Save this document in Word 2003 format.
                oDoc.ExportAsFixedFormat(output, Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF);
                oDoc.Close();
                Marshal.ReleaseComObject(oDoc);
                oDoc = null;
                // Always close Word.exe.
                oWord.Quit(ref oMissing, ref oMissing, ref oMissing);
                Marshal.ReleaseComObject(oWord);
                oWord = null;
            }
            finally
            {
                if (oWord != null)
                {
                    if (oDoc != null)
                    {
                        oDoc.Close();
                        Marshal.ReleaseComObject(oDoc);
                        oDoc = null;
                    }
                    oWord.Quit(ref oMissing, ref oMissing, ref oMissing);
                    Marshal.ReleaseComObject(oWord);
                    oWord = null;
                }
            }
        }


    }


}
