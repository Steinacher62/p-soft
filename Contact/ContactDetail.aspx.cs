using ch.appl.psoft.Common;
using ch.appl.psoft.Contact.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Contact
{
    /// <summary>
    /// Summary description for ContactDetail.
    /// </summary>
    public partial class ContactDetail : PsoftDetailPage
    {
        public const string MODE_CONTACTGROUP = "contactgroup";
        public const string MODE_SEARCHRESULT = "searchresult";
        public const string MODE_FIRM = "firm";
        public const string MODE_OE = "oe";

        // User controls variables
        private PsoftLinksControl _links = null;

        // Query string variables
        private long _ID = -1;
        private long _UID = -1;
        private long _xID = -1;
        private long _xUID = -1;
        private string _mode = "";
        private string _orderColumn = "NAME";
        private string _orderDir = "asc";

        #region Protected overridden methods from parent class
        protected override void Initialize()
        {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _ID = ch.psoft.Util.Validate.GetValid(Request.QueryString["ID"], -1);
            _UID = ch.psoft.Util.Validate.GetValid(Request.QueryString["UID"], -1);
            _xID = ch.psoft.Util.Validate.GetValid(Request.QueryString["xID"], -1);
            _xUID = ch.psoft.Util.Validate.GetValid(Request.QueryString["xUID"], -1);
            _mode = ch.psoft.Util.Validate.GetValid(Request.QueryString["mode"],"").ToLower();
            _orderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], _orderColumn);
            _orderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], _orderDir);
        }

		#endregion
		
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Setting default breadcrumb caption
            BreadcrumbCaption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_BC_EMPTYCONTACTGROUP);
            BreadcrumbName += _mode;

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            ((DGLContentLayout) PageLayoutControl.ContentLayoutControl).DetailHeight = Unit.Pixel(210);

            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();

                if (_UID > 0)
                    _ID = db.UID2ID(_UID);

                if (_xUID > 0)
                    _xID = db.UID2ID(_xUID);

                switch (_mode){
                    case MODE_OE:
                        _mode = MODE_CONTACTGROUP;
                        _xID = ch.psoft.Util.Validate.GetValid(db.lookup("CONTACT_GROUP_ID", "ORGENTITY", "ID=" + _xID, false), -1L);
                        break;
                }

                if (_ID > 0){
                    //check if ID is valid...
                    _ID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "CONTACTV", "ID=" + _ID, false), -1L);
                }

                if (_ID <= 0)
                {
                    switch (_mode)
                    {
                        case MODE_CONTACTGROUP:
                            _ID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "CONTACT_GROUP_CONTACT_V", "CONTACT_GROUP_ID=" + _xID + " order by " + _orderColumn + " " + _orderDir, false),-1);
                            break;

                        case MODE_SEARCHRESULT:
                            _ID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "CONTACTV", "ID IN (select ROW_ID from SEARCHRESULT where ID=" + _xID + ") order by " + _orderColumn + " " + _orderDir, false),-1);
                            break;
                        
                        case MODE_FIRM:
                            _ID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "CONTACTV", "FIRM_ID=" + _xID + " order by NAME asc", false),-1);
                            break;
                    }
                }

                // Setting links control
                _links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                SetPageLayoutContentControl(DGLContentLayout.LINKS, _links);

                if (_xID > 0) {
                    switch(_mode){
                        case MODE_CONTACTGROUP:
                            _links.LinkGroup2.Caption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_GROUP_CONTACTS);

                            //     add to group
                            _links.LinkGroup2.AddLink(_mapper.get("actions"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_ADD_TO_GROUP), psoft.Contact.ContactSearch.GetURL("nextURL","AddToGroup.aspx?searchResultID=%SearchResultID&contactGroupID=" + _xID));
                            break;

                        case MODE_SEARCHRESULT:
                            _links.LinkGroup2.Caption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_SELECTION_CONTACTS);

                            //     add to selection
                            _links.LinkGroup2.AddLink(_mapper.get("actions"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_ADD_TO_SELECTION), psoft.Contact.ContactSearch.GetURL("searchResultID",_xID));
                            break;
                    }
                }

                if (_ID > 0) {
                    // Loading and setting properities of content user controls
                    PSOFTUserControl detailControl = null;
                    string type = ch.psoft.Util.Validate.GetValid(db.lookup("TABLENAME", "CONTACTV", "ID=" + _ID, false), "").ToLower();

                    //     new journal
                    _links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_NEW_JOURNAL), "/Contact/JournalAdd.aspx?contactID=" + _ID + "&type=" + type);

                    //     edit contact
                    _links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_EDIT_CONTACT), "/Contact/ContactEdit.aspx?ID=" + _ID + "&type=" + type + "&nextURL=" + HttpUtility.UrlEncode("ContactDetail.aspx?ID=" + _ID + "&xID=" + _xID + "&mode=" + _mode + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir));

                    //     delete contact
                    if (db.hasRowAuthorisation(DBData.AUTHORISATION.DELETE, type, _ID, true, true)){
                        _links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_DELETE_CONTACT), "javascript: listDeleteRowConfirm('', " + _ID + ",'CONTACTV');"); 
                    }

                    //     list journal
                    _links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_LIST_JOURNAL), "/Contact/JournalDetail.aspx?contactID=" + _ID + "&type=" + type);

                    switch (type) {
                        case ContactModule.TYPE_PERSON:
                            detailControl = LoadPSOFTControl(ContactPersonDetail.Path, "_pd");
                            ((ContactPersonDetail) detailControl).PersonID = _ID;

                            // Setting breadcrumb caption
                            BreadcrumbCaption = ((ContactPersonDetail) detailControl).PersonName;

                            _links.LinkGroup1.Caption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_CONTACTPERSON);
                            break;
                        case ContactModule.TYPE_FIRM:
                            detailControl = LoadPSOFTControl(ContactFirmDetail.Path, "_pd");
                            ((ContactFirmDetail) detailControl).FirmID = _ID;
                            if (_mode == ""){
                                _mode = MODE_FIRM;
                                _xID = _ID;
                            }

                            // Setting breadcrumb caption
                            BreadcrumbCaption = ((ContactFirmDetail) detailControl).FirmName;

                            _links.LinkGroup1.Caption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_CONTACTFIRM);

                            //     new contact-person
                            _links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_NEW_CONTACTPERSON), "/Contact/ContactAdd.aspx?type=" + ContactModule.TYPE_PERSON + "&firmID=" + _ID + "&nextURL=" + HttpUtility.UrlEncode("ContactDetail.aspx?ID=%ID&xID=" + _ID + "&mode=" + MODE_FIRM));

                            //     list contact-persons
                            _links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_LIST_CONTACTPERSONS), "/Contact/ContactDetail.aspx?mode=" + MODE_FIRM + "&xID=" + _ID + "&type=" + ContactModule.TYPE_PERSON);
                            break;
                    }

                    switch (_mode) {
                        case MODE_CONTACTGROUP:
                            _links.LinkGroup2.Caption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_GROUP_CONTACTS);
                            
                            //     delete from group
                            _links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_DELETE_FROM_GROUP), "/Contact/DeleteFromGroup.aspx?contactID=" + _ID + "&contactGroupID=" + _xID);

                            //     new journal
                            _links.LinkGroup2.AddLink(_mapper.get("new"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_NEW_JOURNAL), "/Contact/JournalAdd.aspx?xID=" + _xID + "&mode=" + _mode);

                            if (Global.isModuleEnabled("dispatch")){
                                //     serial letter
                                _links.LinkGroup2.AddLink(_mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_DISPATCH), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_SERIAL_LETTER), psoft.Report.ReportLayoutSelect.GetURL("titleMnemo","contactGroupLetter", "target",(int)ReportModule.Target.ContactGroup, "type",(int)ReportModule.ReportType.Letter, "nextURL",psoft.Dispatch.ManualMail.GetURL("reportLayoutID","%ID", "xID",_xID)));

                                //     serial e-mail
                                _links.LinkGroup2.AddLink(_mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_DISPATCH), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_SERIAL_EMAIL), psoft.Report.ReportLayoutSelect.GetURL("titleMnemo","contactGroupEmail", "target",(int)ReportModule.Target.ContactGroup, "type",(int)ReportModule.ReportType.Email, "nextURL",psoft.Dispatch.ManualMail.GetURL("reportLayoutID","%ID", "xID",_xID)));
                            }
                            break;
                        case MODE_SEARCHRESULT:
                            _links.LinkGroup2.Caption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_SELECTION_CONTACTS);

                            //     delete from selection
                            _links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_DELETE_FROM_SELECTION), psoft.Common.DeleteFromSearchResult.GetURL("searchResultID",_xID, "rowID",_ID, "tablename",type));

                            //     new journal
                            _links.LinkGroup2.AddLink(_mapper.get("new"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_NEW_JOURNAL), "/Contact/JournalAdd.aspx?xID=" + _xID + "&mode=" + _mode);

                            //     save selection as contact-group
                            _links.LinkGroup2.AddLink(_mapper.get("new"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_SAVE_AS_GROUP), "/Contact/ContactGroupAdd.aspx?searchresultID=" + _xID + "&personID=" + SessionData.getUserID(Session) + "&nextURL=" + HttpUtility.UrlEncode("ContactDetail.aspx?xID=%ID&mode=" + MODE_CONTACTGROUP + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir));

                            if (Global.isModuleEnabled("dispatch")){
                                //     serial letter
                                _links.LinkGroup2.AddLink(_mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_DISPATCH), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_SERIAL_LETTER), psoft.Report.ReportLayoutSelect.GetURL("titleMnemo","contactsLetter", "target",(int)ReportModule.Target.ContactSelection, "type",(int)ReportModule.ReportType.Letter, "nextURL",psoft.Dispatch.ManualMail.GetURL("reportLayoutID","%ID", "xID",_xID)));

                                //     serial e-mail
                                _links.LinkGroup2.AddLink(_mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_DISPATCH), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_SERIAL_EMAIL), psoft.Report.ReportLayoutSelect.GetURL("titleMnemo","contactsEmail", "target",(int)ReportModule.Target.ContactSelection, "type",(int)ReportModule.ReportType.Email, "nextURL",psoft.Dispatch.ManualMail.GetURL("reportLayoutID","%ID", "xID",_xID)));
                            }
                            break;
                        case MODE_FIRM:
                            _links.LinkGroup2.Caption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_FIRM_CONTACTS);

                            //     new journal
                            _links.LinkGroup2.AddLink(_mapper.get("new"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_NEW_JOURNAL), "/Contact/JournalAdd.aspx?xID=" + _xID + "&mode=" + _mode);

                            if (Global.isModuleEnabled("dispatch")){
                                //     serial letter
                                _links.LinkGroup2.AddLink(_mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_DISPATCH), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_SERIAL_LETTER), psoft.Report.ReportLayoutSelect.GetURL("titleMnemo","firmLetter", "target",(int)ReportModule.Target.ContactFirm, "type",(int)ReportModule.ReportType.Letter, "nextURL",psoft.Dispatch.ManualMail.GetURL("reportLayoutID","%ID", "xID",_xID)));

                                //     serial e-mail
                                _links.LinkGroup2.AddLink(_mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_DISPATCH), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_SERIAL_EMAIL), psoft.Report.ReportLayoutSelect.GetURL("titleMnemo","firmEmail", "target",(int)ReportModule.Target.ContactFirm, "type",(int)ReportModule.ReportType.Email, "nextURL",psoft.Dispatch.ManualMail.GetURL("reportLayoutID","%ID", "xID",_xID)));
                            }
                            break;
                    }

                    //     clipboard
                    object[] objs = db.lookup(new string[] {"CLIPBOARD_ID","TASKLIST_ID","UID"}, type, "ID=" + _ID);
                    long clipboardID = DBColumn.GetValid(objs[0],-1L);
                    long triggerUID = DBColumn.GetValid(objs[2],-1L);
                    if (clipboardID > 0) {
                        if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "CLIPBOARD", clipboardID, true, true)){
                            //     open clipboard
                            _links.LinkGroup1.AddLink(_mapper.get("shelf"), _mapper.get("clipboard"), psoft.Document.Clipboard.GetURL("ID",clipboardID, "ownerTable",type));
                        }
                    }
                    else {
                        //     create clipboard
                        _links.LinkGroup1.AddLink(_mapper.get("shelf"), _mapper.get("createClipboard"), Document.CreateClipboardM.GetURL("type",Clipboard.TYPE_PRIVATE, "ownerTable",type, "ownerID",_ID, "accessorID",DBData.ACCESSOR.ALL, "triggerUID",triggerUID));
                    }

                    //     tasklist
                    long tasklistID = DBColumn.GetValid(objs[1],-1L);
                   
                    if (tasklistID > 0) {
                        //     open tasklist
                        _links.LinkGroup1.AddLink(
                            _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_TASKLISTS),
                            db.lookup("TITLE", "TASKLIST", "ID=" + tasklistID, false),
                            psoft.Tasklist.TaskDetail.GetURL("ID", tasklistID, "ownerTable", type)
                            );
                    }
                    else {
                        //     create tasklist
                        _links.LinkGroup1.AddLink(
                            _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_TASKLISTS),
                            _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_CREATE_TASKLIST),
                            psoft.Tasklist.AddTasklist.GetURL(
                            "type", Interface.DBObjects.Tasklist.TYPE_PRIVATE,
                            "ownerTable", type,
                            "ownerID", _ID,
                            "triggerUID", triggerUID
                            )
                            );
                    }

                    //     tasklists of contained contact-groups
                    ArrayList tasklists = db.lookup(new string [] {"t.ID", "t.TITLE"}, "CONTACT_GROUP_CONTACT_V cgv inner join TASKLIST t on t.CONTACT_GROUP_ID=cgv.CONTACT_GROUP_ID", "cgv.ID=" + _ID + " and t.template=0", "t.TITLE", false);
                    if (tasklists != null){
                        foreach (string [] tasklist in tasklists){
                            tasklistID = ch.psoft.Util.Validate.GetValid(tasklist[0], -1);
                            
                            if (db.Tasklist.getSemaphore(tasklistID, true) != 3) {
                                //     open tasklist
                                _links.LinkGroup1.AddLink(
                                    _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_TASKLISTS),
                                    tasklist[1],
                                    psoft.Tasklist.TaskDetail.GetURL("ID", tasklistID)
                                    );
                            }
                        }
                    }

                    //  oe
                    _links.LinkGroup1.AddLink(_mapper.get("organisation","oe"), _mapper.get("contact","oeAssignContactGroup"), "/Organisation/AssignGroups.aspx?ownerTable=CONTACTV&ownerID=" + _ID + "&backURL=" + HttpUtility.UrlEncode(Request.RawUrl) + "&nextURL=" + HttpUtility.UrlEncode(Request.RawUrl));

                    //Setting content layout user controls
                    if (detailControl != null)
                        SetPageLayoutContentControl(DGLContentLayout.DETAIL, detailControl);
                }
                else{
                    Response.Redirect(psoft.Contact.ContactSearch.GetURL(), false);
                }

                PsoftPageLayout.PageTitle = BreadcrumbCaption;
            }
            catch(Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally
            {
                db.disconnect();
            }

            if ((_mode != "") || (_xID != -1))
            {
                ContactList cList = (ContactList)this.LoadPSOFTControl(ContactList.Path, "_clst");
                cList.contactID = _ID;
                cList.xID = _xID;
                cList.Mode = _mode;
                cList.OrderColumn = _orderColumn;
                cList.OrderDir = _orderDir;
                cList.SortURL = Global.Config.baseURL + "/Contact/ContactDetail.aspx?id=" + _ID + "&xID=" + _xID + "&mode=" + _mode;
                cList.DetailURL = Global.Config.baseURL + "/Contact/ContactDetail.aspx?id=%ID&xID=" + _xID + "&mode=" + _mode + "&type=" + (_mode == MODE_FIRM? ContactModule.TYPE_PERSON : "%TABLENAME") + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir;
                cList.DetailEnabled = true;
                
                //Setting content layout user controls
                SetPageLayoutContentControl(DGLContentLayout.GROUP, cList);			
            }
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {    

        }
		#endregion
    }
}
