namespace ch.appl.psoft.LayoutControls
{
    using db;
    using System;
    using System.Web;

    /// <summary>
    ///		This is a layout with print, list and authorisations button in first row.
    ///		The second row is a user control with specified content layout
    ///		(BasePageLayout)
    /// </summary>
    public partial  class PsoftPageLayout : PageLayoutControl {
        protected bool _useImages = true;
        protected System.Web.UI.WebControls.WebControl ctrlPrint;
        protected System.Web.UI.WebControls.WebControl ctrlList;
        protected System.Web.UI.WebControls.WebControl ctrlAuthorisations;
        protected System.Web.UI.WebControls.WebControl ctrlExcel;
        protected System.Web.UI.WebControls.WebControl ctrlRegistryEntries;
        protected System.Web.UI.WebControls.WebControl ctrlVisio;
        

        public static string Path {
            get {return Global.Config.baseURL + "/LayoutControls/PsoftPageLayout.ascx";}
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            if (ctrlPrint != null && ctrlPrint.ToolTip == ""){
                ctrlPrint.ToolTip = _mapper.get("PsoftPageLayout", "printButtonToolTip");
            }

            if (ctrlList != null && ctrlList.ToolTip == ""){
                ctrlList.ToolTip = _mapper.get("PsoftPageLayout", "listButtonToolTip");
            }

            if (ctrlAuthorisations != null && ctrlAuthorisations.ToolTip == ""){
                ctrlAuthorisations.ToolTip = _mapper.get("PsoftPageLayout", "authorisationsButtonToolTip");
            }

            if (ctrlExcel != null && ctrlExcel.ToolTip == "") {
                ctrlExcel.ToolTip = _mapper.get("PsoftPageLayout", "excelButtonToolTip");
            }

            if (ctrlRegistryEntries != null && ctrlRegistryEntries.ToolTip == ""){
                ctrlRegistryEntries.ToolTip = _mapper.get("PsoftPageLayout", "registryEntriesButtonToolTip");
            }

            if (ctrlVisio != null && ctrlVisio.ToolTip == ""){
                ctrlVisio.ToolTip = _mapper.get("PsoftPageLayout", "visioButtonToolTip");
            }

        }

        protected void setButtonCtrls(){
            if (_useImages){
                ctrlPrint = imagePrint;
                ctrlList = imageList;
                ctrlAuthorisations = imageAuthorisations;
                ctrlExcel = imageExcel;
                ctrlRegistryEntries = imageRegistryEntries;
                ctrlVisio = imageVisio;
            }
            else{
                ctrlPrint = btnPrint;
                ctrlList = btnList;
                ctrlAuthorisations = btnAuthorisations;
                ctrlExcel = btnExcel;
                ctrlRegistryEntries = btnRegistryEntries;
                ctrlVisio = btnVisio;
            }
        }

		#region Events
        public event System.EventHandler ButtonPrintClick = null;
        public event System.EventHandler ButtonListClick = null;
        public event System.EventHandler ButtonAuthorisationsClick = null;
        public event System.EventHandler ButtonExcelClick = null;
        public event System.EventHandler ButtonRegistryEntriesClick = null;
		#endregion

		#region Properities
        public string PageTitle {
            get {return HttpUtility.HtmlDecode(titleLabel.Text);}
            set {titleLabel.Text = HttpUtility.HtmlEncode(value);}
        }

        public string PageTitleRight {
            get {return HttpUtility.HtmlDecode(titleRight.Text);}
            set {titleRight.Text = HttpUtility.HtmlEncode(value);}
        }

        /// <summary>
        /// Setting UseImageAsButton to true prevents the page from loading again, if just client-side scripting is used.
        /// Set UseImagesAsButton before setting any attribute on buttons.
        /// </summary>
        public bool UseImagesAsButton{
            get{return _useImages;}
            set{
                _useImages = value;
                setButtonCtrls();
            }
        }

        public bool ButtonPrintVisible
		{
			get {setButtonCtrls(); return ctrlPrint.Visible;}
			set {setButtonCtrls(); ctrlPrint.Visible = value;}
		}

		public bool ButtonPrintEnabled
		{
			get {setButtonCtrls(); return ctrlPrint.Enabled;}
			set {setButtonCtrls(); ctrlPrint.Enabled = value;}
		}

        public string ButtonPrintToolTip {
            get {setButtonCtrls(); return ctrlPrint.ToolTip;}
            set {setButtonCtrls(); ctrlPrint.ToolTip = value;}
        }

        public System.Web.UI.AttributeCollection ButtonPrintAttributes {
			get {setButtonCtrls(); return ctrlPrint.Attributes;}
		}

        public bool ButtonListVisible
		{
			get {setButtonCtrls(); return ctrlList.Visible;}
			set {setButtonCtrls(); ctrlList.Visible = value;}
		}

		public bool ButtonListEnabled
		{
			get {setButtonCtrls(); return ctrlList.Enabled;}
			set {setButtonCtrls(); ctrlList.Enabled = value;}
		}

        public string ButtonListToolTip {
            get {return ctrlList.ToolTip;}
            set {ctrlList.ToolTip = value;}
        }

        public System.Web.UI.AttributeCollection ButtonListAttributes {
            get {setButtonCtrls(); return ctrlList.Attributes;}
        }

        public bool ButtonAuthorisationsVisible {
            get {setButtonCtrls(); return ctrlAuthorisations.Visible;}
            set {setButtonCtrls(); ctrlAuthorisations.Visible = value;}
        }

        public bool ButtonAuthorisationsEnabled {
            get {setButtonCtrls(); return ctrlAuthorisations.Enabled;}
            set {setButtonCtrls(); ctrlAuthorisations.Enabled = value;}
        }

        public string ButtonAuthorisationsToolTip {
            get {setButtonCtrls(); return ctrlAuthorisations.ToolTip;}
            set {setButtonCtrls(); ctrlAuthorisations.ToolTip = value;}
        }

        public System.Web.UI.AttributeCollection ButtonAuthorisationsAttributes {
            get {setButtonCtrls(); return ctrlAuthorisations.Attributes;}
        }

        public bool ButtonExcelVisible
        {
            get {setButtonCtrls(); return ctrlExcel.Visible;}
            set {setButtonCtrls(); ctrlExcel.Visible = (Global.Config.getModuleParam("report", "enableExcel", "1") != "1") ? false : value;}
        }

        public bool ButtonExcelEnabled
        {
            get {setButtonCtrls(); return ctrlExcel.Enabled;}
            set {setButtonCtrls(); ctrlExcel.Enabled = value;}
        }

        public string ButtonExcelToolTip 
        {
            get {return ctrlExcel.ToolTip;}
            set {ctrlExcel.ToolTip = value;}
        }

        public System.Web.UI.AttributeCollection ButtonExcelAttributes 
        {
            get {setButtonCtrls(); return ctrlExcel.Attributes;}
        }

        public bool ButtonRegistryEntriesVisible {
            get {setButtonCtrls(); return ctrlRegistryEntries.Visible;}
            set {setButtonCtrls(); ctrlRegistryEntries.Visible = value;}
        }

        public bool ButtonRegistryEntriesEnabled {
            get {setButtonCtrls(); return ctrlRegistryEntries.Enabled;}
            set {setButtonCtrls(); ctrlRegistryEntries.Enabled = value;}
        }

        public string ButtonRegistryEntriesToolTip {
            get {setButtonCtrls(); return ctrlRegistryEntries.ToolTip;}
            set {setButtonCtrls(); ctrlRegistryEntries.ToolTip = value;}
        }

        public System.Web.UI.AttributeCollection ButtonRegistryEntriesAttributes {
            get {setButtonCtrls(); return ctrlRegistryEntries.Attributes;}
        }

        public bool ButtonVisioVisible
        {
            get { setButtonCtrls(); return ctrlVisio.Visible; }
            set { setButtonCtrls(); ctrlVisio.Visible = value; }
        }

        public bool ButtonVisioEnabled
        {
            get { setButtonCtrls(); return ctrlVisio.Enabled; }
            set { setButtonCtrls(); ctrlVisio.Enabled = value; }
        }

        public string ButtonVisioToolTip
        {
            get { return ctrlVisio.ToolTip; }
            set { ctrlVisio.ToolTip = value; }
        }


        public System.Web.UI.AttributeCollection ButtonVisioAttributes{
            get { setButtonCtrls(); return ctrlVisio.Attributes; }
        }

        public bool TitleRowVisible
		{
			get {return titleRow.Visible;}
			set {titleRow.Visible = value;}
		}
		#endregion

        #region Public methods
        public void ShowButtonAuthorisation(string tablename, long rowID){
            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                if (db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, tablename, rowID, true, true)){
                    ButtonAuthorisationsAttributes.Add("onClick", "javascript: openPopupWindow('" + Global.Config.baseURL + "/Common/Authorisations.aspx?tableName=" + tablename + "&rowID=" + rowID + "','400','420');");
                    ButtonAuthorisationsVisible = true;
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        }

        /// <summary>
        /// Shows the button to edit the registry-entries
        /// </summary>
        /// <param name="tablename">table-name of the SEEK-object</param>
        /// <param name="rowID">row-ID of the SEEK-Object</param>
        /// <param name="parentRegistryEntries">comma-separated list of registry-entries (usually from the parent) to show as selected and disabled</param>
        public void ShowButtonRegistryEntries(string tablename, long rowID, string parentRegistryEntries){
            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, tablename, rowID, true, true)){
                    ButtonRegistryEntriesAttributes.Add("onClick", "javascript: openPopupWindow('" + psoft.Common.RegistryEntries.GetURL("UID",db.ID2UID(rowID, tablename), "parentEntries",parentRegistryEntries) + "','500','600');");
                    ButtonRegistryEntriesVisible = true;
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        }
        #endregion

        #region Protected overridden methods from parent class
		protected override void DoSetContentLayoutControl(ContentLayoutControl layout)
		{
			baseLayoutCell.Controls.Add(layout);
		}

		protected override void DoSetErrorMessage(string message)
		{
			lblError.Text = message;
			lblError.Visible = ((message != null) && (message != ""));
		}
		#endregion

		#region Private methods
		private void btnPrint_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (ButtonPrintClick != null)
				ButtonPrintClick(sender, e);
		}

		private void btnList_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (ButtonListClick != null)
				ButtonListClick(sender, e);		
		}

        private void btnAuthorisations_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            if (ButtonAuthorisationsClick != null)
                ButtonAuthorisationsClick(sender, e);		
        }

        private void btnExcel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (ButtonExcelClick != null)
                ButtonExcelClick(sender, e);		
        }

        private void btnRegistryEntries_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
            if (ButtonRegistryEntriesClick != null)
                ButtonRegistryEntriesClick(sender, e);		
        }
        #endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

        }
		#endregion
	}
}
