using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Person.Controls;
using System;

namespace ch.appl.psoft.Person
{

    public partial class PersonJournalEdit : PsoftEditPage {

        protected long _contextID = -1;
        protected long _journalID = -1;
        protected string _backURL = "";

        #region Protected overridden methods from parent class
        protected override void Initialize()
        {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _contextID = ch.psoft.Util.Validate.GetValid(Request.QueryString["contextID"], -1);
            _journalID = ch.psoft.Util.Validate.GetValid(Request.QueryString["journalID"], _journalID);
            _backURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["backURL"], _backURL);
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
            PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get("Journal", "Eintrag bearbeiten");

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)LoadPSOFTControl(DGLContentLayout.Path, "_cl");
			PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting control
            PersonJournalEditCtrl ja = (PersonJournalEditCtrl) LoadPSOFTControl(PersonJournalEditCtrl.Path, "_ja");
            ja.contextID = _contextID;
            ja.journalID = _journalID;
            ja.BackUrl = _backURL;

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