using ch.appl.psoft.Common;
using ch.appl.psoft.Contact.Controls;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Contact
{
    /// <summary>
    /// Summary description for ContactGroupAdd.
    /// </summary>
    public partial class ContactGroupAdd : PsoftEditPage {

        protected long _searchResultID = -1;
        protected string _nextURL = "";
        protected long _personID = -1;
        protected string _ownerTable = "";
        protected long _ownerID = -1;

        #region Protected overridden methods from parent class
        protected override void Initialize()
        {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _searchResultID = ch.psoft.Util.Validate.GetValid(Request.QueryString["searchresultID"], -1);
            _nextURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["nextURL"], "");
            _personID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], -1);
            _ownerTable = ch.psoft.Util.Validate.GetValid(Request.QueryString["ownerTable"], "");
            _ownerID = ch.psoft.Util.Validate.GetValid(Request.QueryString["ownerID"], -1);
        }
		#endregion

        /// <summary>
        /// Load page
        /// </summary>
        protected void Page_Load(object sender, System.EventArgs e) 
        {
			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;
			PageLayoutControl = PsoftPageLayout;
	
            PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_BC_ADDCONTACTGROUP);

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)LoadPSOFTControl(DGLContentLayout.Path, "_cl");
			PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting control
            ContactGroupAddCtrl cga = (ContactGroupAddCtrl)LoadPSOFTControl(ContactGroupAddCtrl.Path, "_cga");
            cga.SearchresultID = _searchResultID;
            cga.NextURL = _nextURL;
            cga.PersonID = _personID;
            cga.OwnerTable = _ownerTable;
            cga.OwnerID = _ownerID;

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, cga);		
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
            this.ID = "Add";

        }
		#endregion
    }
}