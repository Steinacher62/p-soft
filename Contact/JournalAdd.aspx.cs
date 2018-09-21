using ch.appl.psoft.Common;
using ch.appl.psoft.Contact.Controls;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Contact
{

    public partial class JournalAdd : PsoftEditPage {

        protected long _xID = -1;
        protected long _contactID = -1;
        protected string _mode = "";
        protected string _nextURL = "";

        #region Protected overridden methods from parent class
        protected override void Initialize()
        {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _xID = ch.psoft.Util.Validate.GetValid(Request.QueryString["xID"], -1);
            _contactID = ch.psoft.Util.Validate.GetValid(Request.QueryString["contactID"], -1);
            _mode = ch.psoft.Util.Validate.GetValid(Request.QueryString["mode"],"").ToLower();
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
            PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_BC_ADDJOURNAL);

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)LoadPSOFTControl(DGLContentLayout.Path, "_cl");
			PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting control
            JournalAddCtrl ja = (JournalAddCtrl)LoadPSOFTControl(JournalAddCtrl.Path, "_ja");
            ja.xID = _xID;
            ja.ContactID = _contactID;
            ja.NextURL = _nextURL;
            ja.Mode = _mode;

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, ja);		
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