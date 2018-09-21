using ch.appl.psoft.Common;
using ch.appl.psoft.Contact.Controls;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Contact
{
    /// <summary>
    /// Summary description for ContactEdit.
    /// </summary>
    public partial class ContactEdit : PsoftEditPage {

        protected long _ID = -1;
        protected string _type = "";
        protected string _nextURL = "";

        #region Protected overridden methods from parent class
        protected override void Initialize()
        {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _ID = ch.psoft.Util.Validate.GetValid(Request.QueryString["ID"], -1);
            _type = ch.psoft.Util.Validate.GetValid(Request.QueryString["type"],"").ToLower();
            _nextURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["nextURL"], "");
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
            PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, _type == ContactModule.TYPE_PERSON ? ContactModule.LANG_MNEMO_BCEDITCONTACT : ContactModule.LANG_MNEMO_BCEDITFIRM);

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)LoadPSOFTControl(DGLContentLayout.Path, "_cl");
			PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting control
            ContactEditCtrl ce = (ContactEditCtrl)LoadPSOFTControl(ContactEditCtrl.Path, "_ce");
            ce.xID = _ID;
            ce.NextURL = _nextURL;
            ce.Type = _type;

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, ce);		
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