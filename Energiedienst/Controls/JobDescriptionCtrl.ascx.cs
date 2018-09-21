namespace ch.appl.psoft.Energiedienst.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.appl.psoft.Report;
    using ch.psoft.Util;
    using db;
    using DocumentFormat.OpenXml.Packaging;
    using Interface;
    using System;
    using System.Data;
    using System.IO;
    using System.Net.Mail;
    using System.Runtime.InteropServices;
    using System.Web;
    using System.Web.UI;
    using System.Xml;
    using System.Xml.Linq;
    using Word = Microsoft.Office.Interop.Word;

    //using ch.appl.psoft.Dispatch;  

    /// <summary>
    ///		Summary description for JobDescriptionCtrl.
    /// </summary>
    public partial class JobDescriptionCtrl : PSOFTMapperUserControl
    {
        public const string PARAM_DUTYGROUP_ID = "PARAM_DUTYGROUP_ID";
        protected Config _config = null;
        protected DBData _db = null;
        protected DocumentFormat.OpenXml.Drawing.Charts.DataTable _competenceLevels = null;
        protected long _funktionID = -1L;
        protected bool _showFunktion = false;
        protected bool _showSalary = false;
        protected long _jobID = -1L;
        protected long _OEID = -1L;
        protected long _dutyValidityId = 0L;
        protected long _personId;


        protected long _employment_ID = -1;
        protected string _onloadString;
        protected int _groupNumber = 0;
        protected string _reportDate = DateTime.Now.ToString("d");
        protected bool isFirstGrp = true;
        protected string tmpSqltableName;
        protected string employeeName;
        protected string imagename;
        public static string Path
        {
            get { return Global.Config.baseURL + "/Energiedienst/Controls/JobDescriptionCtrl.ascx"; }
        }

        #region Properities
        public long JobID
        {
            get { return _jobID; }
            set
            {
                _jobID = value;
                if (_jobID > 0)
                {
                    _funktionID = -1L;
                    _showFunktion = false;
                    _showSalary = false;
                }
            }
        }
        public long FunktionID
        {
            get { return _funktionID; }
            set
            {
                _funktionID = value;
                if (_funktionID > 0)
                {
                    _jobID = -1L;
                    _showFunktion = Global.Config.isModuleEnabled("fbw");
                    _showSalary = Global.Config.isModuleEnabled("lohn"); ;
                }
            }
        }
        public long OEID
        {
            get { return _OEID; }
            set { _OEID = value; }
        }
        public string deleteMessage
        {
            get { return _mapper.get("MESSAGES", "deleteConfirm"); }
        }

        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {

            
                _db = DBData.getDBData(Session);
                _personId = (long)_db.lookup("EMPLOYMENT.PERSON_ID", "JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID", "JOB.ID = " + JobID);
                _jobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], -1);
                _funktionID = (long)_db.lookup("FUNKTION_ID", "JOB", "ID = " + _jobID, -1L);

                //from printJobDesc



                _jobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], -1);
                DBData db = DBData.getDBData(Session);
                _employment_ID = (long)db.lookup("EMPLOYMENT_ID", "JOB", "ID = " + _jobID, -1L);
                _personId = (long)db.lookup("PERSON_ID", "EMPLOYMENT", "ID = " + _employment_ID, -1L);
                _funktionID = (long)db.lookup("FUNKTION_ID", "JOB", "ID = " + _jobID, -1L);
                employeeName = _db.Person.getWholeName(_personId.ToString(), false, true, false).Replace(" ", "");
                ChangeLogo logo = new ChangeLogo();
                imagename = logo.getLogoFilename(db, _personId).Split('.')[0];

            if (!Page.IsPostBack)
            {
                updatePdfView();

            }

            FBSGeprueft.Text = "Eingesehen und geprüft von " + db.lookup("Firstname", "Person", "ID = " + _personId).ToString() + " " + db.lookup("PNAME", "Person", "ID = " + _personId).ToString();
                if (db.lookup("JOB_DESCRIPTION_CHECKED", "JOB", "ID=" + _jobID).ToString().Equals(""))
                {
                    FBSGeprueft.Checked = false;
                }
                else
                {
                    FBSGeprueft.Checked = true;
                }
                FBSGeprueft.CheckedChanged += new EventHandler(FBW_Checked);
                if (db.userId == _personId && FBSGeprueft.Checked == false)
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

            File.Copy(pdfPath, config.documentSaveDirectory + "\\" + xFilename + ".pdf");

            string sql = "insert into document (ID, FOLDER_ID,INHERIT,TITLE,DESCRIPTION,AUTHOR,FILENAME,XFILENAME,VERSION,CREATED,CHECKOUT_STATE,CHECKIN_PERSON_ID,TYP,NUMOFDOCVERSIONS)  VALUES(" + xid + "," + folder_id + ",'1','" + title + "','','','" + file + ".pdf" + "','" + xFilename + ".pdf" + "','1','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','0','" + _db.userId + "','0','1')";
            _db.execute(sql);

            _db.disconnect();

            sendMail("Statusänderung Aufgabenbeschreibung", "<font face=\"Arial\" size=\"3\">sender hat ihre/seine Aufgabenbeschreibung visiert.<br><br></font><a href=\"https://srv132/p-flow\"><font face=\"Arial\" size=\"3\">Einstiegslink p-flow</font></a><br><br><font face=\"Arial\" size=\"3\">Besten Dank für Ihre Unterstützung.</font>", toID);
    
        }

        protected void FBW_RELEASE_CLICKED(object sender, System.EventArgs e)
        {
            DateTime time = System.DateTime.Now;
            _db.connect();
            _db.execute("UPDATE JOB SET JOB_DESCRIPTION_RELEASE = '" + time.ToString("MM.dd.yyyy") + "' WHERE ID = " + JobID);
            _db.execute("UPDATE JOB SET JOB_DESCRIPTION_RELEASE_PERSON = " + _db.userId + " WHERE ID = " + JobID);
            _db.execute("UPDATE JOB SET JOB_DESCRIPTION_CHECKED = null WHERE ID = " + JobID);
            
            string toID = _db.lookup("ID", "Person", " id = (SELECT PERSON.ID FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID WHERE (JOB.ID = " + JobID + "))").ToString();
            sendMail("Statusänderung Aufgabenbeschreibung", "<font face=\"Arial\" size=\"3\">Ihre Augabenbeschreibung wurde von sender geändert.<br><br>Bitte überprüfen Sie Ihre Aufgabenbeschreibung und visieren Sie diese.<br><br></font><a href=\"https://srv132/p-flow\"><font face=\"Arial\" size=\"3\">Einstiegslink p-flow</font></a><br><br><a href=\"http://intranet.master.kwl.de/edintranet/userfiles/File/Service_Tools/Interne%20Auftr%c3%a4ge%20und%20Bestellungen/Ablauf%20Aufgabenbeschreibungen_19.10.2016.pdf\"><font face=\"Arial\" size=\"3\">Ablauf Aufgabenbeschreibungen</font></a><br><br><font face=\"Arial\" size=\"3\">Besten Dank für Ihre Unterstützung.</font>", toID);

            _db.disconnect();
            
        }

       

        protected string getRestriction()
        {
            if (SessionData.showValidDutyCompOnly(Session))
                return "VALID_FROM<=GetDate() and VALID_TO>=GetDate() and DUTY_VALIDITY_VALID_FROM<=GetDate() and DUTY_VALIDITY_VALID_TO>=GetDate() and (JOB_ID=" + JobID + " or FUNKTION_ID=" + _funktionID + ")";
            else
                return "DUTY_VALIDITY_VALID_FROM<=GetDate() and DUTY_VALIDITY_VALID_TO>=GetDate() and (JOB_ID=" + JobID + " or FUNKTION_ID=" + _funktionID + ")";
        }

        protected override void DoExecute()
        {
            base.DoExecute();
        }

        protected void sendMail(string subject, string message, string toID)
        {
            _db = DBData.getDBData(Session);
            _db.connect();
            string _personId = toID;

            MailMessage myMessage = new MailMessage();
            try
            {
                myMessage.From = new MailAddress(_db.lookup("EMAIL", "PERSON", "ID = " + _db.userId).ToString(), _db.lookup("FIRSTNAME", "PERSON", "ID = " + _db.userId).ToString() + " " + _db.lookup("PNAME", "PERSON", "ID = " + _db.userId, "").ToString());
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

        protected void btnDownload_Click(object sender, EventArgs e)
        {

            string fileName = _db.Person.getWholeName(_personId.ToString(), false, true, false).Replace(" ", "");
            String path = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY);

            String docxPath = path + "\\" + fileName + ".docx";
          
            if (File.Exists(docxPath))
            {
                File.Delete(docxPath);
            }

            File.Copy(Request.MapPath("JobDescription"+imagename+".docx"), docxPath);
            WordprocessingDocument document = WordprocessingDocument.Open(docxPath, true);

            document = generateWordDoc(document);
            

            byte[] byteArray = File.ReadAllBytes(docxPath);

            if (byteArray != null)
            {
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                string fileNameDownload = "Aufgabenbeschreibung_" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ".docx";
                Response.AddHeader("Content-Disposition",
                    String.Format("attachment; filename={0}", fileNameDownload));
                Response.BinaryWrite(byteArray);
                Response.Flush();
                Response.End();
            }
           
        }

        private void updatePdfView()
        {
            string fileName = _db.Person.getWholeName(_personId.ToString(), false, true, false).Replace(" ", "");
            String path =  Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY);

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

            File.Copy(Request.MapPath("JobDescription"+imagename+".docx"), docxPath);
            WordprocessingDocument document = WordprocessingDocument.Open(docxPath, true);

           generateWordDoc(document);
            Convert(docxPath, pdfPath, Word.WdSaveFormat.wdFormatPDF);
          
            pdfFrame.Attributes.Add("src", Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + fileName + ".pdf");

        }
        private WordprocessingDocument generateWordDoc(WordprocessingDocument document)
        {
            try
            {
                OpenXmlPart part = document.MainDocumentPart.GetPartById("rId1");
                XDocument cDoc = part.LoadXDocument();

                _db = DBData.getDBData(Session);
                _personId = (long)_db.lookup("EMPLOYMENT.PERSON_ID", "JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID", "JOB.ID = " + JobID);
                object[] values = _db.lookup(new string[] { "O.ID", "O." + _db.langAttrName("FUNKTION", "TITLE"), "O." + _db.langAttrName("ORGENTITY", "MNEMONIC") }, "ORGENTITY O inner join JOB J on O.ID = J.ORGENTITY_ID", "J.ID = " + JobID);

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
                            c.Value = _db.lookup("KEYCONTACTS", "JOB", "ID = " + JobID).ToString();
                            break;

                        case "shortJobDescription":
                            c.Value = _db.lookup("JOB_DESCRIPTION_SHORT", "JOB", "ID = " + JobID).ToString();
                            break;

                        case "jobDescription":
                            _dutyValidityId = (long)_db.lookup("TOP (1) DUTY_VALIDITY.ID", "DUTY_VALIDITY INNER JOIN DUTY ON DUTY_VALIDITY.DUTY_ID = DUTY.ID INNER JOIN DUTY_COMPETENCE_VALIDITY ON DUTY.ID = DUTY_COMPETENCE_VALIDITY.DUTY_ID INNER JOIN JOB ON DUTY_COMPETENCE_VALIDITY.JOB_ID = dbo.JOB.ID", "JOB.ID = " + JobID, 0L);
                            if (_dutyValidityId == 0)
                            {
                                _db.connect();
                                _db.execute("INSERT INTO DUTY(ORDNUMBER) VALUES(1)");
                                long lastInserted = (long)_db.lookup("MAX(ID)", "DUTY", "");
                                _db.execute("INSERT INTO DUTY_VALIDITY(DUTY_ID,NUMBER,VALID_FROM, VALID_TO)VALUES(" + lastInserted + ", 1,GETDATE(),'2099-12-31')");
                                _db.execute("INSERT INTO DUTY_COMPETENCE_VALIDITY (VALID_FROM, VALID_TO,DUTY_ID,JOB_ID) VALUES (GETDATE(),'2099-12-31'," + lastInserted + "," + JobID + ")");

                                _db.disconnect();
                                _dutyValidityId = (long)_db.lookup("ID", "DUTY_VALIDITY", "DUTY_ID = " + lastInserted);
                            }
                            String jD = (string)_db.lookup("TOP (1) DUTY_VALIDITY.DESCRIPTION_DE", "DUTY_VALIDITY INNER JOIN DUTY ON DUTY_VALIDITY.DUTY_ID = DUTY.ID INNER JOIN DUTY_COMPETENCE_VALIDITY ON DUTY.ID = DUTY_COMPETENCE_VALIDITY.DUTY_ID INNER JOIN JOB ON DUTY_COMPETENCE_VALIDITY.JOB_ID = dbo.JOB.ID", "JOB.ID = " + JobID);
                            if (!String.IsNullOrEmpty(jD))
                            {
                                c.Value = jD.Replace("<w:lang w:val=\"en-US\" w:eastAsia=\"en-US\" w:bidi=\"ar-SA\"/>", "<w:lang w:val=\"de-CH\"/>");
                            }
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


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);

        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion


        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (FileUploadControl.HasFile)
                try
                {
                    string fileNameFromUser = FileUploadControl.FileName;

                    var fiFileName = new FileInfo(fileNameFromUser);
                    if (fiFileName.Extension == ".docx")
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            memoryStream.Write(FileUploadControl.FileBytes, 0,
                                FileUploadControl.FileBytes.Length);

                            // Open and close the WordprocessingML document to make sure
                            // that the SDK can open it.

                            WordprocessingDocument wDoc = null;
                            try
                            {
                                wDoc = WordprocessingDocument.Open(memoryStream, true);
                                updateDatabase(wDoc);
                            }
                            finally
                            {
                                if (wDoc != null)
                                {
                                    wDoc.Close();
                                    wDoc = null;
                                }
                            }
                            lblMessage.Text = "Daten wurden erfolgreich übernommen";
                            updatePdfView();
                        }
                    }
                    else
                    {
                        lblMessage.Text = "ERROR: Datei ist nicht im docx Format";
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "ERROR: " + ex.Message.ToString();
                }
            else
            {
                lblMessage.Text = "Bitte wählen Sie eine Datei";
            }
        }

        private void updateDatabase(WordprocessingDocument wDoc)
        {
            OpenXmlPart part = wDoc.MainDocumentPart.GetPartById("rId1");
            XDocument cDoc = part.LoadXDocument();
            _db = DBData.getDBData(Session);
            _db.connect();

            //fill word document from DB
            foreach (var c in cDoc.Root.Elements())
            {
                switch (c.Name.ToString())
                {

                    case "substitute":
                        _db.execute("UPDATE PERSON SET PROXY_ME = '" + c.Value.Replace("'", "''") + "' WHERE ID = " + _personId);
                        break;

                    case "substituteOf":
                        _db.execute("UPDATE PERSON SET PROXY_TO = '" + c.Value.Replace("'", "''") + "' WHERE ID = " + _personId);
                        break;

                    case "keyContacts":
                        _db.execute("UPDATE JOB SET KEYCONTACTS = '" + c.Value.Replace("'", "''") + "' WHERE ID = " + JobID);
                        break;

                    case "shortJobDescription":
                        _db.execute("UPDATE JOB SET JOB_DESCRIPTION_SHORT = '" + c.Value.Replace("'", "''") + "' WHERE ID = " + JobID);
                        break;

                    case "jobDescription":
                        _dutyValidityId = (long)_db.lookup("TOP (1) DUTY_VALIDITY.ID", "DUTY_VALIDITY INNER JOIN DUTY ON DUTY_VALIDITY.DUTY_ID = DUTY.ID INNER JOIN DUTY_COMPETENCE_VALIDITY ON DUTY.ID = DUTY_COMPETENCE_VALIDITY.DUTY_ID INNER JOIN JOB ON DUTY_COMPETENCE_VALIDITY.JOB_ID = dbo.JOB.ID", "JOB.ID = " + JobID, 0L);
                        if (_dutyValidityId == 0)
                        {
                            _db.connect();
                            _db.execute("INSERT INTO DUTY(ORDNUMBER) VALUES(1)");
                            long lastInserted = (long)_db.lookup("MAX(ID)", "DUTY", "");
                            _db.execute("INSERT INTO DUTY_VALIDITY(DUTY_ID,NUMBER,VALID_FROM, VALID_TO)VALUES(" + lastInserted + ", 1,GETDATE(),'2099-12-31')");
                            _db.execute("INSERT INTO DUTY_COMPETENCE_VALIDITY (VALID_FROM, VALID_TO,DUTY_ID,JOB_ID) VALUES (GETDATE(),'2099-12-31'," + lastInserted + "," + JobID + ")");

                            _db.disconnect();
                            _dutyValidityId = (long)_db.lookup("ID", "DUTY_VALIDITY", "DUTY_ID = " + lastInserted);
                        }
               
                        //System.Text.Encoding.Convert()
                        String sql = "UPDATE DUTY_VALIDITY SET DESCRIPTION_DE = N'" + c.Value.Replace("'", "''") + "' WHERE ID = " + _dutyValidityId;
                        _db.execute(sql);
                        break;


                }
            }
                _db.execute("UPDATE JOB SET JOB_DESCRIPTION_CHECKED = NULL WHERE ID = " + JobID);
                _db.execute("UPDATE JOB SET JOB_DESCRIPTION_RELEASE = null WHERE ID = " + JobID);
                _db.execute("UPDATE JOB SET JOB_DESCRIPTION_RELEASE_PERSON = null WHERE ID = " + JobID);

            _db.disconnect();
            
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


    public static class Extensions
    {
        public static XDocument LoadXDocument(this OpenXmlPart part)
        {
            XDocument xdoc;
            using (StreamReader streamReader = new StreamReader(part.GetStream()))
                xdoc = XDocument.Load(XmlReader.Create(streamReader));
            return xdoc;
        }
       
    }
}
