using ch.psoft.Util;
using System;
using System.Collections;
using System.Web.Mail;
using Word = Microsoft.Office.Interop.Word;

namespace ch.appl.psoft.Dispatch
{
    public delegate void SendMailHandler(object state);

    public class FieldNotFoundException : Exception
    {
        public FieldNotFoundException(string message) : base(message) {}
    }

    public class MailMergeRecord
    {
        public object _state;
        public string [] _values;

        public MailMergeRecord(object state, params string[] values)
        {
            _state = state;
            _values = values;
        }
    }

	/// <summary>
	/// Summary description for MergeAndMail.
	/// </summary>
	public class MergeAndMail
	{
        public event SendMailHandler sendMail = null;

        protected string _template = "";
        protected string [] _headers = null;

        protected ArrayList _records = null;
        protected ArrayList _attachments = null;
        // used to consider only those fields for the merge-process that are found in the template.
        protected ArrayList _mergeFieldIndexes = null;
 
        protected object oFalse = false;
        protected object oTrue = true;
        protected object oMissing = System.Reflection.Missing.Value;

        private MergeAndMail() {}

		public MergeAndMail(string template, params string[] fields)
		{
            _template = template;
            _headers = fields;
            _records = new ArrayList();
            _attachments = new ArrayList();
            _mergeFieldIndexes = new ArrayList();
        }

        public void AddRecord(object state, params string [] values)
        {
            _records.Add(new MailMergeRecord(state, values));
        }

        public void AddAttachment(string fileName)
        {
            _attachments.Add(fileName);
        }

        public void ExecuteMailing(string smtpServer, string fromAddress, string emailAddressField, string emailSubjectField)
        {
            CreateMailMergeDoc("", smtpServer, fromAddress, emailAddressField, emailSubjectField);
        }

        public void CreateMergeDoc(string mergeFileName)
        {
            CreateMailMergeDoc(mergeFileName, "", "", "", "");
        }

        public void CheckFields()
        {
            bool fieldsOk = true;
            string notFoundFields = "";

            Word.ApplicationClass wordApplication = new Word.ApplicationClass();
            Word.Document templateDoc = null;

            _mergeFieldIndexes.Clear();

            try
            {
                // open template document
                object oTemplate = _template;
#if WORD_V11
                templateDoc = wordApplication.Documents.Open(ref oTemplate, ref oFalse, ref oTrue, ref oFalse, ref oMissing, ref oMissing, ref oTrue, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oFalse, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
#else
                templateDoc = wordApplication.Documents.Open(ref oTemplate, ref oFalse, ref oTrue, ref oFalse, ref oMissing, ref oMissing, ref oTrue, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oFalse, ref oMissing, ref oMissing, ref oMissing);
#endif
                // check if the according fields exist in the template
                foreach (Word.Field field in templateDoc.Fields)
                {
                    if (field.Type != Word.WdFieldType.wdFieldMergeField)
                        continue;

                    string fieldString = field.Result.Text.Replace("«", "").Replace("»","");
                    bool found = false;
                    for (int i=0; i<_headers.Length; i++) {
                        string header = _headers[i];
                        if (header == fieldString) {
                            found = true;
                            if (_mergeFieldIndexes.IndexOf(i) < 0){
                                _mergeFieldIndexes.Add(i);
                            }
                            break;
                        }
                    }
                    if (!found) {
                        if (fieldsOk)
                            fieldsOk = false;
                        else
                            notFoundFields += ", ";

                        notFoundFields += fieldString;
                    }
                }
            }
            finally
            {
                if (templateDoc != null)
                    templateDoc.Close(ref oFalse, ref oMissing, ref oMissing);

                wordApplication.Application.Quit(ref oMissing, ref oMissing, ref oMissing);
            }

            if (!fieldsOk)
                throw new FieldNotFoundException(notFoundFields);
        }

        protected void CreateMailMergeDoc(string mergeFileName, string smtpServer, string fromAddress, string emailAddressField, string emailSubjectField)
        {
            Word.ApplicationClass wordApplication = new Word.ApplicationClass();
            Word.Document templateDoc = null;
            Word.Document dataSourceDoc = null;
            Word.Document mergeDoc = null;
            string dataSourceFileName = System.IO.Path.GetTempFileName();
            
            bool mailIt = emailAddressField != "";

            if (smtpServer != "")
                SmtpMail.SmtpServer = smtpServer;

            wordApplication.Options.CheckGrammarAsYouType = false;
            wordApplication.Options.CheckSpellingAsYouType = false;
            wordApplication.Options.FormatScanning = false;
            wordApplication.Options.ShowFormatError = false;
            wordApplication.Options.SaveInterval = 0;

            try
            {
                // open template document
                object oTemplate = _template;
#if WORD_V11
                templateDoc = wordApplication.Documents.Open(ref oTemplate, ref oFalse, ref oTrue, ref oFalse, ref oMissing, ref oMissing, ref oTrue, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oFalse, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
#else
                templateDoc = wordApplication.Documents.Open(ref oTemplate, ref oFalse, ref oTrue, ref oFalse, ref oMissing, ref oMissing, ref oTrue, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oFalse, ref oMissing, ref oMissing, ref oMissing);
#endif

                CheckFields();

                // create data-source doc
                string headerList = "";
                bool isFirst = true;
                for (int i=0; i<_mergeFieldIndexes.Count; i++){
                    if (isFirst)
                        isFirst = false;
                    else
                        headerList += ";";

                    headerList += _headers[(int) _mergeFieldIndexes[i]];
                }
                object oHeaderRecord = headerList;
                object oDataSourceName = dataSourceFileName;
                Logger.Log("MailMerge: Creating DataSource '" + dataSourceFileName + "'...", Logger.DEBUG);
                templateDoc.MailMerge.CreateDataSource(ref oDataSourceName, ref oMissing, ref oMissing, ref oHeaderRecord, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
#if WORD_V11
                dataSourceDoc = wordApplication.Documents.Open(ref oDataSourceName, ref oMissing, ref oMissing, ref oFalse, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
#else
                dataSourceDoc = wordApplication.Documents.Open(ref oDataSourceName, ref oMissing, ref oMissing, ref oFalse, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
#endif

                isFirst = true;
                Word.Table table = dataSourceDoc.Tables[1];
                Word.Rows rows = table.Rows;
                Word.Row row = rows[2];
                Word.Cells cells = null;
                int recCount = 1;
                foreach (MailMergeRecord record in _records) {
                    Logger.Log("MailMerge: adding record " + recCount++ + ".", Logger.DEBUG);
                    if (isFirst)
                        isFirst = false;
                    else
                        row = rows.Add(ref oMissing);

                    cells = row.Cells;

                    for (int i=0; i<_mergeFieldIndexes.Count; i++){
                        cells[i+1].Range.Text = record._values[(int) _mergeFieldIndexes[i]];
                    }
                }

                dataSourceDoc.Save();
                dataSourceDoc.Saved = true;
                dataSourceFileName = dataSourceDoc.FullName;
                Logger.Log("MailMerge: ...DataSource '" + dataSourceFileName + "' saved.", Logger.DEBUG);

                // perform mail-merge
                Logger.Log("MailMerge: Executing merge-process...", Logger.DEBUG);
                templateDoc.MailMerge.Destination = Word.WdMailMergeDestination.wdSendToNewDocument;
                templateDoc.MailMerge.Execute(ref oFalse);
                Logger.Log("MailMerge: ...Merge-process done.", Logger.DEBUG);

                templateDoc.Saved = true;

                foreach (Word.Document doc in wordApplication.Documents)
                {
                    if (!doc.Saved)
                        mergeDoc = doc;
                }
                if (mergeDoc != null)
                {
                    object oMergeFileName = mergeFileName;

                    if (mailIt)
                    {
                        // get number of sections in template
                        int nrOfSections = templateDoc.Sections.Count;

                        // get email-address and -subject field-index
                        int emailAddressIndex = -1;
                        int emailSubjectIndex = -1;
                        for (int i=0; i<_headers.Length; i++)
                        {
                            if (_headers[i] == emailAddressField)
                                emailAddressIndex = i;
                            if (_headers[i] == emailSubjectField)
                                emailSubjectIndex = i;
                        }

                        if (emailAddressIndex >= 0)
                        {
                            Logger.Log("MailMerge: Starting mailing...", Logger.DEBUG);
                            for (int recNr=0; recNr<_records.Count; recNr++)
                            {
                                MailMergeRecord record = (_records[recNr] as MailMergeRecord);
                                string emailAddress = record._values[emailAddressIndex];
                                if (emailAddress != "")
                                {
                                    object startRange = mergeDoc.Sections[recNr * nrOfSections + 1].Range.Start;
                                    object endRange = mergeDoc.Sections[recNr * nrOfSections + nrOfSections].Range.End;
                                    string mailText = mergeDoc.Range(ref startRange, ref endRange).Text.Replace("\r", "\r\n");
                                    
                                    // let's mail the plain-text...
                                    MailMessage mail = new MailMessage();

                                    if (emailSubjectIndex >= 0){
                                        mail.Subject = record._values[emailSubjectIndex];
                                    }
                                    mail.From = fromAddress;
                                    mail.To = emailAddress;
                                    mail.Body = mailText;
                                    foreach (string attachment in _attachments)
                                    {
                                        mail.Attachments.Add(new MailAttachment(attachment));
                                    }

                                    Logger.Log("MailMerge: Sending Email to '" + emailAddress + "'.", Logger.DEBUG);
                                    SmtpMail.Send(mail);
                                    if (sendMail != null)
                                        sendMail(record._state);
                                }
                            }
                            Logger.Log("MailMerge: ...Mailing finished.", Logger.DEBUG);
                        }
                        else
                        {
                            Logger.Log("MailMerge: Email field not found!", Logger.ERROR);
                        }
                    }
                    else
                    {
                        Logger.Log("MailMerge: Saving merged document '" + mergeFileName + "'.", Logger.DEBUG);
                        mergeDoc.SaveAs(ref oMergeFileName, ref oMissing, ref oMissing, ref oMissing, ref oFalse, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                        mergeDoc.Saved = true;
                    }
                }
            }
            finally
            {
                // close documents
                if (mergeDoc != null)
                    mergeDoc.Close(ref oFalse, ref oMissing, ref oMissing);
                if (templateDoc != null)
                    templateDoc.Close(ref oFalse, ref oMissing, ref oMissing);
                if (dataSourceDoc != null)
                    dataSourceDoc.Close(ref oFalse, ref oMissing, ref oMissing);

                wordApplication.Application.Quit(ref oMissing, ref oMissing, ref oMissing);

                // delete temporary data-source file
                if (System.IO.File.Exists(dataSourceFileName))
                    System.IO.File.Delete(dataSourceFileName);
            }
        }
	}
}
