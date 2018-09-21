using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using NotesFor.HtmlToOpenXml;
using System;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;


namespace ch.appl.psoft.Energiedienst
{
    public partial class ConvertDBToXML : System.Web.UI.Page
    {

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

        protected Boolean skipCurrent = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            _db = DBData.getDBData(Session);
         
            DataTable IDs = _db.getDataTable("SELECT [ID] FROM[dbo].[JOB]");
            int fails = 0;
            String failedid = "";
            for (int i = 0; i < IDs.Rows.Count; i++)
            {
                try
                {
                    _jobID = (long)IDs.Rows[i].ItemArray.GetValue(0);
                    skipCurrent = false;
                    _funktionID = (long)_db.lookup("FUNKTION_ID", "JOB", "ID = " + _jobID, -1L);

                    //from printJobDesc-




                    DBData db = DBData.getDBData(Session);
                    _employment_ID = (long)db.lookup("EMPLOYMENT_ID", "JOB", "ID = " + _jobID, -1L);
                    _personId = (long)db.lookup("PERSON_ID", "EMPLOYMENT", "ID = " + _employment_ID, -1L);
                    _funktionID = (long)db.lookup("FUNKTION_ID", "JOB", "ID = " + _jobID, -1L);
                    employeeName = _db.Person.getWholeName(_personId.ToString(), false, true, false).Replace(" ", "");

                    String path = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY);

                    String docxPath = path + "\\template.docx";
                    String tempPath = path + "\\temp.docx";

                    if (File.Exists(docxPath))
                    {
                        File.Delete(docxPath);
                    }

                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }

                    File.Copy(Request.MapPath("JobDescription.docx"), docxPath);
                    WordprocessingDocument document = WordprocessingDocument.Open(docxPath, true);
                    generateWordDoc(document);
                    

                    //not empty and not already converted
                    if (!skipCurrent)
                    {
                        mergeDocuments("template");
                        document = WordprocessingDocument.Open(docxPath, true);
                        updateDatabase(document);
                        document.Close();
                    }
                }catch(Exception ex)
                {
                    fails++;
                    failedid += _jobID + " ";
                }
            }
            lblMessage.Text = "conversion complete! fails: " + fails +" failed job IDs: "+failedid;
        }


        private WordprocessingDocument generateWordDoc(WordprocessingDocument document)
        {
            OpenXmlPart part = document.MainDocumentPart.GetPartById("rId1");
            XDocument cDoc = part.LoadXDocument();

            _db = DBData.getDBData(Session);
            _personId = (long)_db.lookup("EMPLOYMENT.PERSON_ID", "JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID", "JOB.ID = " + _jobID);
            object[] values = _db.lookup(new string[] { "O.ID", "O." + _db.langAttrName("FUNKTION", "TITLE"), "O." + _db.langAttrName("ORGENTITY", "MNEMONIC") }, "ORGENTITY O inner join JOB J on O.ID = J.ORGENTITY_ID", "J.ID = " + _jobID);



            //fill word document from DB
            foreach (var c in cDoc.Root.Elements())
            {
                switch (c.Name.ToString())
                {

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
                        c.Value = c.Value.Trim();
                        if (String.IsNullOrEmpty(c.Value) || c.Value.StartsWith("<?xml")){
                            skipCurrent = true;
                            break;
                        }
                        convertHtmlToXml(c.Value);
                        c.Value = "";
                        break;

                    default:
                        break;
                }
            }


            //save
            using (Stream str = part.GetStream(
            FileMode.Create, FileAccess.Write))
            using (XmlWriter xw = XmlWriter.Create(str))
                cDoc.Save(xw);


            document.Close();
            return document;
        }

        private String convertHtmlToXml(String html)
        {
            try
            {
                string filename = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY) + "/temp.docx";


                if (File.Exists(filename)) File.Delete(filename);

                using (MemoryStream generatedDocument = new MemoryStream())
                {
                    String xml = "";
                    using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = package.MainDocumentPart;
                        if (mainPart == null)
                        {
                            mainPart = package.AddMainDocumentPart();
                            Body body = new Body();
                            new DocumentFormat.OpenXml.Wordprocessing.Document(body).Save(mainPart);
                        }

                        HtmlConverter converter = new HtmlConverter(mainPart);
                        converter.ParseHtml(html);

                        mainPart.Document.Save();
                    }
                    File.WriteAllBytes(filename, generatedDocument.ToArray());



                    return xml;
                }

            }
            catch (Exception e)
            {
                return html;
            }


        }

        public void mergeDocuments(String filename)
        {
            Microsoft.Office.Interop.Word.ApplicationClass wordObject = new Microsoft.Office.Interop.Word.ApplicationClass();




            object destination = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY) + "\\" + filename + ".docx"; //this is the path
            object source = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY) + "\\temp.docx";

           // Microsoft.Office.Interop.Word.Application wordobject = new Microsoft.Office.Interop.Word.Application();
           // wordObject.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsNone;
            Microsoft.Office.Interop.Word._Document docs = wordObject.Documents.Open(ref source);
            docs.ActiveWindow.Selection.WholeStory();
            docs.ActiveWindow.Selection.Copy();
            docs.Close();
            docs = wordObject.Documents.Open(ref destination);

            Microsoft.Office.Interop.Word.Range rangeStory;
            foreach (Microsoft.Office.Interop.Word.Range range in docs.StoryRanges)
            {

                rangeStory = range;
                do
                {
                    foreach (Word.ContentControl cc in rangeStory.ContentControls)
                    {
                        if (cc.XMLMapping.XPath == "/jobDescription[1]/jobDescription[1]")
                        {
                            cc.Range.Paste();
                        }
                    }

                    rangeStory = rangeStory.NextStoryRange;

                }
                while (rangeStory != null);
            }
            docs.Close();

            wordObject.Quit();

            //if (docs != null)
            //    System.Runtime.InteropServices.Marshal.ReleaseComObject(docs);
            //if (wordobject != null)
            //    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordobject);      
            //docs = null;
            //wordobject = null;
            //GC.Collect(); // force final cleanup!
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
                        //System.Text.Encoding.Convert()
                        String sql = "UPDATE DUTY_VALIDITY SET DESCRIPTION_DE = N'" + c.Value.Replace("'", "''") + "' WHERE ID = " + _dutyValidityId;
                        _db.execute(sql);
                        break;

                    default:
                        break;
                }
            }
            

            _db.disconnect();

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