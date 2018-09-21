using ch.appl.psoft.Common;
using ch.appl.psoft.Contact.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;
using System.Web;

namespace ch.appl.psoft.Contact
{
    /// <summary>
    /// Summary description for ContactGroupDetail.
    /// </summary>
    public partial class ContactGroupDetail : PsoftDetailPage
    {
        // User controls variables
        private PsoftLinksControl _links = null;

        // Query string variables
        private long _ID = -1;
        private long _UID = -1;
        private string _orderColumn = "TITLE";
        private string _orderDir = "asc";

        #region Protected overridden methods from parent class
        protected override void Initialize()
        {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _ID = ch.psoft.Util.Validate.GetValid(Request.QueryString["ID"], -1);
            _UID = ch.psoft.Util.Validate.GetValid(Request.QueryString["UID"], -1);
            _orderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], _orderColumn);
            _orderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], _orderDir);

            // Setting SubNavMenu URL
            SubNavMenuUrl = "/Contact/SubNavMenu.aspx";
        }

		#endregion
		
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Setting default breadcrumb caption
            BreadcrumbCaption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_BC_NOCONTACTGROUP);

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");

            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();

                if (_UID > 0)
                    _ID = db.UID2ID(_UID);

                if (_ID <= 0)
                {
                    _ID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "CONTACT_GROUP", "PERSON_ID=" + SessionData.getUserID(Session) + " order by " + _orderColumn + " " + _orderDir, false),-1);
                }

                if (_ID > 0)
                {
                    // Loading and setting properities of content user controls
                    ContactGroupDetailCtrl cgd = (ContactGroupDetailCtrl) LoadPSOFTControl(ContactGroupDetailCtrl.Path, "_cgd");
                    cgd.xID = _ID;

                    // Setting breadcrumb caption
                    BreadcrumbCaption = cgd.getTitle();

                    // Setting links control for given contact
                    _links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                    _links.LinkGroup1.Caption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_CONTACTGROUP);
                    _links.LinkGroup2.Caption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_ALLCONTACTGROUPS);

                    //     edit contact-group
                    _links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_EDIT_CONTACTGROUP), "/Contact/ContactGroupEdit.aspx?ID=" + _ID + "&nextURL=" + HttpUtility.UrlEncode("ContactGroupDetail.aspx?ID=" + _ID + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir));

                    //     list contacts
                    _links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_LIST_CONTACTS), "/Contact/ContactDetail.aspx?mode=" + ContactDetail.MODE_CONTACTGROUP + "&xID=" + _ID);

                    //     new journal
                    _links.LinkGroup2.AddLink(_mapper.get("new"), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_NEW_JOURNAL), "/Contact/JournalAdd.aspx?xID=" + _ID + "&mode=" + ContactDetail.MODE_CONTACTGROUP);

                    if (Global.isModuleEnabled("dispatch")){
                        //     serial letter
                        _links.LinkGroup1.AddLink(_mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_DISPATCH), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_SERIAL_LETTER), psoft.Report.ReportLayoutSelect.GetURL("titleMnemo","contactGroupLetter", "target",(int)ReportModule.Target.ContactGroup, "type",(int)ReportModule.ReportType.Letter, "nextURL",psoft.Dispatch.ManualMail.GetURL("reportLayoutID","%ID", "xID",_ID)));

                        //     serial e-mail
                        _links.LinkGroup1.AddLink(_mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CMT_DISPATCH), _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CM_SERIAL_EMAIL), psoft.Report.ReportLayoutSelect.GetURL("titleMnemo","contactGroupEmail", "target",(int)ReportModule.Target.ContactGroup, "type",(int)ReportModule.ReportType.Email, "nextURL",psoft.Dispatch.ManualMail.GetURL("reportLayoutID","%ID", "xID",_ID)));
                    }

                    //Setting content layout user controls
                    SetPageLayoutContentControl(DGLContentLayout.DETAIL, cgd);
                    SetPageLayoutContentControl(DGLContentLayout.LINKS, _links);
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

            ContactGroupList cgList = (ContactGroupList) LoadPSOFTControl(ContactGroupList.Path, "_cglst");
            cgList.ContactGroupID = _ID;
            cgList.PersonID = SessionData.getUserID(Session);
            cgList.OrderColumn = _orderColumn;
            cgList.OrderDir = _orderDir;
            cgList.SortURL = Global.Config.baseURL + "/Contact/ContactGroupDetail.aspx?id=" + _ID;
            cgList.DetailURL = Global.Config.baseURL + "/Contact/ContactGroupDetail.aspx?id=%ID" + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir;
            cgList.DetailEnabled = true;
            
            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.GROUP, cgList);			
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
