using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report;
using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Dispatch.Controls
{
    /// <summary>
    /// Summary description for ManualMailCtrl.
    /// </summary>
    public partial class ManualMailCtrl : PSOFTMapperUserControl {
        protected class State{
            public long _xID = -1L;
            public long _mailingID = -1L;

            public State(long xID, long mailingID){
                _xID = xID;
                _mailingID = mailingID;
            }
        }

        protected const string FIELD_EMAIL_ADDRESS = "EMAIL";
        protected const string FIELD_EMAIL_SUBJECT = "TB_SUBJECT";

        protected System.Web.UI.WebControls.HyperLink HyperLink1;

        protected ReportModule.Target _target = ReportModule.Target.Undefined;
        protected ReportModule.ReportType _type = ReportModule.ReportType.Undefined;
        protected string _mailingSQL = "";
        protected string _mailingTable = "";
        protected bool _doEmail = false;
        protected bool _doLetter = false;
        protected string _loadingMnemo = "loadingCombined";
        protected long _journalID = -1;


        protected DBData _db = null;
        protected System.Web.UI.HtmlControls.HtmlGenericControl attachmentsFrame;
        protected System.Web.UI.HtmlControls.HtmlTableRow rowSeparator1;
        protected ListBuilder _listBuilder = null;
        protected string _testEmailAddress;

        #region Properties
        protected long _reportLayoutID = -1;
        public long ReportLayoutID {
            get {return _reportLayoutID;}
            set {_reportLayoutID = value;}
        }

        protected long _xID = -1;
        public long xID {
            get {return _xID;}
            set {_xID = value;}
        }

        protected long _mailingID = -1;
        public long MailingID {
            get {return _mailingID;}
            set {_mailingID = value;}
        }

        protected long _mailTemplateID = -1;
        public long MailTemplateID {
            get {return _mailTemplateID;}
            set {_mailTemplateID = value;}
        }

        protected long _letterTemplateID = -1;
        public long LetterTemplateID {
            get {return _letterTemplateID;}
            set {_letterTemplateID = value;}
        }

        protected long _searchResultID = -1;
        public long SearchResultID {
            get {return _searchResultID;}
            set {_searchResultID = value;}
        }

        protected bool _useSameTemplate = true;
        public bool UseSameTemplate {
            get {return _useSameTemplate;}
            set {_useSameTemplate = value;}
        }

        protected string _backURL = "";
        public string BackURL {
            get {return _backURL;}
            set {_backURL = value;}
        }

        protected string[] _substituteValues = null;
        public string[] SubstituteValues{
            get {return _substituteValues;}
            set {_substituteValues = value;}
        }
        #endregion

        public static string Path {
            get {return Global.Config.baseURL + "/Dispatch/Controls/ManualMailCtrl.ascx";}
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        /// <summary>
        /// Manual creation of a mailing, where the user can select templates, persons, etc depending on the context.
        /// 
        /// Queryparameter:
        ///     - ID : the identifier of the record. The record-type depends on the context.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void DoExecute() {
            base.DoExecute ();
            _db = DBData.getDBData(Session);
            _listBuilder = new ListBuilder();
            _testEmailAddress = Global.Config.getModuleParam("dispatch", "testEmailAddress", "");

            mergeDocTable.Visible = false;
            mergeDocText.Visible = mergeDocTitle.Visible = mailFinishedText.Visible = mailFinishedTitle.Visible = false;
            testTable.Visible = false;

            if (_reportLayoutID <= 0)
                Response.Redirect(psoft.Report.ReportLayoutSelect.GetURL("nextURL",psoft.Dispatch.ManualMail.GetURL("reportLayoutID","%ID", "xID",_xID)));


            if (!IsPostBack) {
                LabelTarget.Text = _mapper.get("dispatch", "LabelTarget");
                ValueTarget.Text = _searchResultID > 0? _mapper.get("dispatch", "targetSelection") : _mapper.get("dispatch", "targetAll");
                selectTarget.Text = _mapper.get("dispatch", "selectTarget");
                LabelFrom.Text = _mapper.get("dispatch", "LabelFrom");
                LabelTo.Text = _mapper.get("dispatch", "LabelTo");
                LabelSubject.Text = _mapper.get("dispatch", "LabelSubject");
                LabelMailTemplate.Text = _mapper.get("dispatch", "LabelMailTemplate");
                cbUseSame.Text = _mapper.get("dispatch", "radioUseSame");
                LabelLetterTemplate1.Text = _mapper.get("dispatch", "LabelLetterTemplate");
                LabelAttachments.Text = _mapper.get("dispatch", "LabelAttachments");
                LabelTest.Text = _mapper.get("dispatch", "LabelTest");
                CBTestEmailAddress.Text = _mapper.get("dispatch", "CBTestEmailAddress");
                CBTestEmailAddress.Checked = true;

                addAttachment.Text = _mapper.get("dispatch", "addAttachment");
                //                addLetterTemplate.Text = _mapper.get("dispatch", "addLetterTemplate");
                addLetterTemplate.Text = _mapper.get("dispatch", "selectLetterTemplate");
                //                addMailTemplate.Text = _mapper.get("dispatch", "addMailTemplate");
                addMailTemplate.Text = _mapper.get("dispatch", "selectMailTemplate");

                selectLetterTemplate.Text = _mapper.get("dispatch", "selectLetterTemplate");
                selectLetterTemplate.Visible = false;
                selectMailTemplate.Text = _mapper.get("dispatch", "selectMailTemplate");
                selectMailTemplate.Visible = false;

                TBTestEmailAddress.Text = TBFrom.Text = _testEmailAddress == ""? Global.Config.getModuleParam("dispatch", "eMailAddressFrom", "") : _testEmailAddress;
                
                backMerge.Text = _mapper.get("back");
                send.Text = _mapper.get("dispatch", "send");
                //                send.Attributes.Add("onClick", "startLoading();");
                testDocTitle.Text = _mapper.get("dispatch", "testDocTitleCombined");
                testDocText.Text = _mapper.get("dispatch", "testDocTextCombined");
                mergeDocTitle.Text = _mapper.get("dispatch", "mergeDocTitle");
                mergeDocText.Text = _mapper.get("dispatch", "mergeDocText");
                mailFinishedTitle.Text = _mapper.get("dispatch", "mailFinishedTitle");
                mailFinishedText.Text = _mapper.get("dispatch", "mailFinishedText");
                back.Text = _mapper.get("back");
                ok.Text = _mapper.get("next");
                ok.Attributes.Add("onClick", "startLoading();");
                next.Text = _mapper.get("next");

                cbUseSame.Checked = _useSameTemplate;

                try {
                    _db.connect();

                    if (_mailingID <= 0){
                        _mailingID = _db.Mailing.create(_reportLayoutID, true, ""); // create temporary mailing entry
                    }
                    else{
                        TBFrom.Text = (string) Session["Mailing_From_" + _mailingID];
                        TBSubject.Text = (string) Session["Mailing_Subject_" + _mailingID];
                        TBTestEmailAddress.Text = (string) Session["Mailing_TestEmailAddress_" + _mailingID];
                    }
                    
                    Session["MailingID"] = _mailingID.ToString();
                }
                catch(Exception ex) {
                    DoOnException(ex);
                }
                finally {
                    _db.disconnect();
                }
            }
            else {
                _mailingID = ch.psoft.Util.Validate.GetValid(Session["MailingID"].ToString(), -1L);
            }

            loadDocuments();
            setButtonLinks();
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
		
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {    

        }
		#endregion

        private void setButtonLinks() {
            string backURL = HttpUtility.UrlEncode(psoft.Dispatch.ManualMail.GetURL("reportLayoutID",_reportLayoutID, "xID",_xID, "mailingID",_mailingID, "useSameTemplate",cbUseSame.Checked.ToString(), "searchResultID",_searchResultID, "backURL",_backURL, "param0",_substituteValues[0], "param1",_substituteValues[1], "param2",_substituteValues[2]));
            addAttachment.Attributes.Add("onClick", "openPopupWindow('DocumentAdd.aspx?mailingID=" + _mailingID + "&type=" + (int) DispatchDocument.DOCUMENT_TYPE.MAIL_ATTACHMENT + "&backURL=" + backURL + "', 450, 100);");
            addLetterTemplate.Attributes.Add("onClick", "openPopupWindow('DocumentAdd.aspx?asEmailTemplate=false&mailingID=" + _mailingID + "&type=" + (int) DispatchDocument.DOCUMENT_TYPE.MAILING_TEMPLATE + "&backURL=" + backURL + "', 450, 100);");
            addMailTemplate.Attributes.Add("onClick", "openPopupWindow('DocumentAdd.aspx?asEmailTemplate=true&mailingID=" + _mailingID + "&type=" + (int) DispatchDocument.DOCUMENT_TYPE.MAILING_TEMPLATE + "&backURL=" + backURL + "', 450, 100);");
            selectMailTemplate.Attributes.Add("onClick", "openPopupWindow('DocumentSelect.aspx?asEmailTemplate=true&mailingID=" + _mailingID + "&type=" + (int) DispatchDocument.DOCUMENT_TYPE.MAILING_TEMPLATE + "&backURL=" + backURL + "', 450, 550);");
            selectLetterTemplate.Attributes.Add("onClick", "openPopupWindow('DocumentSelect.aspx?asEmailTemplate=false&mailingID=" + _mailingID + "&type=" + (int) DispatchDocument.DOCUMENT_TYPE.MAILING_TEMPLATE + "&backURL=" + backURL + "', 450, 550);");
            selectTarget.Attributes.Add("onClick", "openPopupWindow('TargetSelect.aspx?reportLayoutID=" + _reportLayoutID + "&xID=" + _xID +"&searchResultID=" + _searchResultID + "&backURL=" + backURL + "&param0=" + _substituteValues[0] + "&param1=" + _substituteValues[1] + "&param2=" + _substituteValues[2] + "', 700, 550);");
        }

        private void loadDocuments() {
            attachmentTab.Controls.Clear();

            try {
                _db.connect();
                DataTable table = _db.getDataTableExt("select ATTACHMENT.ID, DISPATCHDOCUMENT.FILENAME from DISPATCHDOCUMENT inner join ATTACHMENT on DISPATCHDOCUMENT.ID=ATTACHMENT.DISPATCHDOCUMENT_ID where MAILING_ID=" + _mailingID, "DISPATCHDOCUMENT");

                _listBuilder.rowsPerPage = SessionData.getRowsPerListPage(Session);
                _listBuilder.detailEnable = false;
                _listBuilder.deleteEnable = true;
                _listBuilder.editEnable = false;
                _listBuilder.headerEnable = false;
                _listBuilder.infoBoxEnable = false;
                _listBuilder.idColumn = table.Columns["ID"];
                int numRec = _listBuilder.load(_db, table, attachmentTab, _mapper);
                if (numRec == 0) {
                    TableRow r = new TableRow();
                    TableCell c = new TableCell();
                    c.Text = _mapper.get("dispatch", "noAttachments");
                    c.CssClass = "List";
                    r.Cells.Add(c);
                    attachmentTab.Rows.Add(r);
                    attachmentTab.CellPadding = 0;
                }
                else
                    attachmentTab.CellPadding = 3;

                table = _db.getDataTable("select * from MAILING where ID=" + _mailingID);
                if (table.Rows.Count > 0) {
                    DataRow row = table.Rows[0];
                    _mailTemplateID = ch.psoft.Util.Validate.GetValid(row["EMAIL_TEMPLATE_DISPATCHDOCUMENT_ID"].ToString(), -1);
                    _letterTemplateID = ch.psoft.Util.Validate.GetValid(row["LETTER_TEMPLATE_DISPATCHDOCUMENT_ID"].ToString(), -1);
                    string mailTemplate = _db.DispatchDocument.getFileName(_mailTemplateID);
                    ValueMailTemplate.Text = ch.psoft.Util.Validate.GetValid(mailTemplate, _mapper.get("dispatch", "noAttachments"));

                    string letterTemplate = _db.DispatchDocument.getFileName(_letterTemplateID);
                    ValueLetterTemplate.Text = ch.psoft.Util.Validate.GetValid(letterTemplate, _mapper.get("dispatch", "noAttachments"));

                    send.Enabled = mailTemplate != "" || letterTemplate != "";
                }

                table = _db.getDataTable("select * from REPORTLAYOUT where ID=" + _reportLayoutID);
                if (table.Rows.Count > 0) {
                    DataRow row = table.Rows[0];
                    _mailingSQL = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName("REPORTLAYOUT", "SQL")].ToString(), "").Replace("%25ID","%ID").Replace("%ID", _xID.ToString());
                    if (_substituteValues != null){
                        for (int i=0; i<_substituteValues.Length; i++){
                            _mailingSQL = _mailingSQL.Replace("$"+i, _substituteValues[i]);
                        }
                    }
                    if (_searchResultID > 0)
                        _mailingSQL += " and ID in (select ROW_ID from SEARCHRESULT where ID=" + _searchResultID + ")";
                    _mailingSQL += " order by NAME asc";
                    _mailingTable = ch.psoft.Util.Validate.GetValid(row["DBTABLE"].ToString(), "");
                    _target = (ReportModule.Target) ch.psoft.Util.Validate.GetValid(row["TARGET"].ToString(), (int) _target);
                    _type = (ReportModule.ReportType) ch.psoft.Util.Validate.GetValid(row["TYPE"].ToString(), (int) _type);
                    _db.Mailing.setTitle(_mailingID, _mapper.get("reportLayout", _db.GetDisplayValue(table.Columns["TITLE_MNEMO"], row["TITLE_MNEMO"].ToString(), false)));
                }

                switch (_type) {
                    case ReportModule.ReportType.Email:
                        LabelMailTemplates.Text = _mapper.get("dispatch", "LabelMailTemplates");
                        LabelMailTemplate.Text = "";
                        _loadingMnemo = "loadingEmail";
                        testDocTitle.Text = _mapper.get("dispatch", "testDocTitleEmail");
                        testDocText.Text = _mapper.get("dispatch", "testDocTextEmail");
                        send.Text = _mapper.get("dispatch", "send");
                        _doEmail = true;
                        break;

                    case ReportModule.ReportType.EmailAndLetter:
                        LabelMailTemplates.Text = _mapper.get("dispatch", "LabelTemplates");
                        _loadingMnemo = "loadingCombined";
                        testDocTitle.Text = _mapper.get("dispatch", "testDocTitleCombined");
                        testDocText.Text = _mapper.get("dispatch", "testDocTextCombined");
                        send.Text = _mapper.get("dispatch", "send");
                        _doEmail = true;
                        _doLetter = true;
                        break;
                    
                    case ReportModule.ReportType.Letter:
                        LabelLetterTemplates2.Text = _mapper.get("dispatch", "LabelLetterTemplates");
                        cbUseSame.Checked = false;
                        CBTestEmailAddress.Checked = false;
                        _loadingMnemo = "loadingLetter";
                        testDocTitle.Text = _mapper.get("dispatch", "testDocTitleLetter");
                        testDocText.Text = _mapper.get("dispatch", "testDocTextLetter");
                        send.Text = _mapper.get("dispatch", "generate");
                        _doLetter = true;
                        break;
                }
                rowAttachments.Visible = rowFrom.Visible = rowTo.Visible = rowSubject.Visible = rowMailTemplate.Visible = rowTestEmail.Visible = rowSeparator3.Visible = rowSeparator4.Visible = rowSeparator5.Visible = rowSeparator7.Visible = rowSeparator8.Visible = _doEmail;
                rowLetterTemplate1.Visible = _doEmail && _doLetter;
                rowLetterTemplate2.Visible = _doLetter;

                rowTo.Visible = rowSeparator4.Visible = false;
            }
            catch(Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        private void sendMail(bool onlyTestMail) {
            try {
                _db.connect();
                long mergeDocId = -1;
                string mergedDocFilename = "";
                string letterTemplateFilename = "";
                string letterOrigFilename = "";
                long templateID = _mailTemplateID;
                string mailTemplateFilename = _db.DispatchDocument.getServerFileName(templateID);

                if (!cbUseSame.Checked)
                    templateID = _letterTemplateID;

                letterTemplateFilename = _db.DispatchDocument.getServerFileName(templateID);
                letterOrigFilename = _db.DispatchDocument.getFileName(templateID);

                if ((_doEmail && mailTemplateFilename == "") || (_doLetter && letterTemplateFilename == ""))
                    throw new Exception(_mapper.get("dispatch", "selectTemplate"));

                string smtpServer = Global.Config.getModuleParam("dispatch", "smtpServer", "");
                string eMailAddressFrom = TBFrom.Text;

                MergeAndMail mailMerge = null;
                MergeAndMail letterMerge = null;

                DataTable table = _db.getDataTableExt(_mailingSQL, _mailingTable);
                ArrayList columnNames = new ArrayList();
                foreach (DataColumn col in table.Columns) {
                    columnNames.Add(col.ColumnName);
                }
                if (_doEmail){
                    columnNames.Add(FIELD_EMAIL_SUBJECT);
                }

                int recordCount = table.Rows.Count;
                const int setSize = 50;
                int lowerBound = 0;
                int upperBound = recordCount;

                if (_doEmail && !_doLetter)
                    upperBound = Math.Min(setSize, recordCount);
                
                //                if (_doEmail && !onlyTestMail)
                //                    _db.Mailing.setTemporary(_mailingID, false); //make it persistent
                
                _journalID = -1;

                while (lowerBound < upperBound) {
                    if (_doEmail) {
                        mailMerge = new MergeAndMail(_db.DispatchDocument.DocumentsPath + mailTemplateFilename, (string[]) columnNames.ToArray(typeof(string)));
                        mailMerge.CheckFields();
                        if (_doEmail && !onlyTestMail){
                            mailMerge.sendMail += new SendMailHandler(onSendMail);
                        }
                    }
                    if (_doLetter) {
                        letterMerge = new MergeAndMail(_db.DispatchDocument.DocumentsPath + letterTemplateFilename, (string[]) columnNames.ToArray(typeof(string)));
                        letterMerge.CheckFields();
                    }

                    for (int i=lowerBound; i<upperBound; i++) {
                        DataRow row = table.Rows[i];
                        int xID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1);
                        ArrayList columnValues = new ArrayList();
                        foreach (string columnName in columnNames) {
                            if (_doEmail && columnName == FIELD_EMAIL_ADDRESS)
                                columnValues.Add(onlyTestMail? TBTestEmailAddress.Text : (_testEmailAddress == ""? _db.GetDisplayValue(table.Columns[columnName], row[columnName], false) : _testEmailAddress));
                            else if (_doEmail && columnName == FIELD_EMAIL_SUBJECT)
                                columnValues.Add(TBSubject.Text);
                            else
                                columnValues.Add(_db.GetDisplayValue(table.Columns[columnName], row[columnName], false));
                        }

                        if (_doEmail) {
                            mailMerge.AddRecord(new State(xID, _mailingID), (string[]) columnValues.ToArray(typeof(string)));
                        }

                        if (_doLetter) {
                            letterMerge.AddRecord(new State(xID, _mailingID), (string[]) columnValues.ToArray(typeof(string)));
                            if (!onlyTestMail){
//                                _db.Mailing.addMailingPerson(_mailingID, personID, Mailing.MAILING_PERSON_TYPE.LETTER);
                                addJournalEntry(xID);
                            }
                        }

                        if (onlyTestMail) {
                            lowerBound = recordCount;
                            break;
                        }
                    }

                    if (_doLetter) {
                        mergedDocFilename = _db.DispatchDocument.DocumentsPath + _target + _xID + "_" + System.IO.Path.GetFileNameWithoutExtension(letterOrigFilename) + ".doc";
                        letterMerge.CreateMergeDoc(mergedDocFilename);
                        mergeDocId = _db.Mailing.addGeneratedDocument(_mailingID, mergedDocFilename, true);
                    }

                    if (_doEmail) {
                        DataTable attTable = _db.getDataTable("select SERVER_FILENAME from DISPATCHDOCUMENT inner join ATTACHMENT on DISPATCHDOCUMENT.ID=ATTACHMENT.DISPATCHDOCUMENT_ID where MAILING_ID=" + _mailingID);
                        foreach (DataRow attRow in attTable.Rows) {
                            mailMerge.AddAttachment(_db.DispatchDocument.DocumentsPath + attRow["SERVER_FILENAME"].ToString());
                        }
                        mailMerge.ExecuteMailing(smtpServer, eMailAddressFrom, FIELD_EMAIL_ADDRESS, FIELD_EMAIL_SUBJECT);
                    }

                    if (!onlyTestMail) {
                        lowerBound = upperBound;
                        upperBound = Math.Min(upperBound + setSize, recordCount);
                    }
                }

                //                if (!onlyTestMail)
                //                    _db.Mailing.setTemporary(mailingID, false); //make it persistent

                mergeMailTable.Visible = false;
                if (mergeDocId > 0) {
                    if (onlyTestMail) {
                        testTable.Visible = true;
                        testDocLink.NavigateUrl = _db.DispatchDocument.DocumentsURL + _db.DispatchDocument.getServerFileName(mergeDocId);
                        testDocLink.Text = System.IO.Path.GetFileName(mergedDocFilename);
                    }
                    else {
                        mergeDocTable.Visible = mergeDocText.Visible = mergeDocTitle.Visible = true;
                        mergeDocLink.NavigateUrl = _db.DispatchDocument.DocumentsURL + _db.DispatchDocument.getServerFileName(mergeDocId);
                        mergeDocLink.Text = System.IO.Path.GetFileName(mergedDocFilename);
                    }
                }
                else {
                    if (onlyTestMail) {
                        testTable.Visible = true;
                    }
                    else{
                        mergeDocTable.Visible = mailFinishedText.Visible = mailFinishedTitle.Visible = true;
                    }
                }
            }
            catch(Exception ex) {
                if (ex.GetType() == typeof(FieldNotFoundException)){
                    ex = new Exception(_mapper.get("error", "mergeFieldNotFound") + ex.Message, ex);
                }
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected void popubButton_Click(object sender, System.EventArgs e) {
            Session["Mailing_From_" + _mailingID] = TBFrom.Text;
            Session["Mailing_Subject_" + _mailingID] = TBSubject.Text;
            Session["Mailing_TestEmailAddress_" + _mailingID] = TBTestEmailAddress.Text;
        }

        protected void send_Click(object sender, System.EventArgs e) {
            sendMail(CBTestEmailAddress.Checked);
        }

        protected void back_Click(object sender, System.EventArgs e) {
            mergeMailTable.Visible = true;
            mergeDocTable.Visible = false;
            testTable.Visible = false;
        }

        protected void backMerge_Click(object sender, System.EventArgs e) {
            if (BackURL != "")
                Response.Redirect(BackURL);
            else
                ((PsoftContentPage) Page).RedirectToPreviousPage();
        }

        protected void ok_Click(object sender, System.EventArgs e) {
            sendMail(false);
        }

        protected void next_Click(object sender, System.EventArgs e) {
            if (BackURL != "")
                Response.Redirect(BackURL);
            else
                ((PsoftContentPage) Page).RedirectToPreviousPage();
        }

        protected void cbUseSame_CheckedChanged(object sender, System.EventArgs e) {
            ValueLetterTemplate.Enabled = addLetterTemplate.Enabled = selectLetterTemplate.Enabled = !cbUseSame.Checked;
            setButtonLinks();
        }

        protected void CBTestEmailAddress_CheckedChanged(object sender, System.EventArgs e) {
            TBTestEmailAddress.Enabled = CBTestEmailAddress.Checked;
        }
    
        public void onSendMail(object state){
            if (state != null && state is State){
                addJournalEntry((state as State)._xID);
            }
        }

        protected void addJournalEntry(long contactID){
            if (Global.isModuleEnabled("contact")){
                try{
                    _db.beginTransaction();
                    if (_journalID <= 0){
                        string title = _doEmail? TBSubject.Text + " (" + ValueMailTemplate.Text + ")" : ValueLetterTemplate.Text;
                        _journalID = _db.Journal.create(title, _doEmail? "SERIALEMAIL" : "SERIALLETTER");
                    }
                    _db.Journal.assignToContact(_journalID, contactID);
                    _db.commit();
                }
                catch (Exception ex) {
                    DoOnException(ex);
                    _db.rollback();
                }
            }
        }

        public string GetTitleMnemo() {
            DBData db = DBData.getDBData(Session);

            try {
                db.connect();
                return db.lookup("TITLE_MNEMO", "REPORTLAYOUT", "ID=" + _reportLayoutID, false);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
            return "";
        }
    }
}
