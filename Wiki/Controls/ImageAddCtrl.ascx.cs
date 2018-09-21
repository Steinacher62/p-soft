using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Wiki.Controls
{
    public partial class ImageAddCtrl : PSOFTInputViewUserControl {
        protected System.Web.UI.WebControls.Table imagesTab;
        protected DBData _db = null;

        #region Properties

        public const string PARAM_BACK_URL = "PARAM_BACK_URL";
        public string BackURL {
            get {return GetString(PARAM_BACK_URL);}
            set {SetParam(PARAM_BACK_URL, value);}
        }

        public const string PARAM_OWNER_UID = "PARAM_OWNER_UID";
        public long OwnerUID {
            get {return GetLong(PARAM_OWNER_UID);}
            set {SetParam(PARAM_OWNER_UID, value);}
        }

        public const string PARAM_IMAGE_TITLE = "PARAM_IMAGE_TITLE";
        public string ImageTitle {
            get {return GetString(PARAM_IMAGE_TITLE);}
            set {SetParam(PARAM_IMAGE_TITLE, value);}
        }
        #endregion
        
        public static string Path {
            get {return Global.Config.baseURL + "/Wiki/Controls/ImageAddCtrl.ascx";}
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _db = DBData.getDBData(Session);

            if (!IsPostBack) {
                ok.Text = _mapper.get("next");
                title.Text = _mapper.get(WikiModule.LANG_SCOPE_WIKI, WikiModule.LANG_MNEMO_ST_ADD_IMAGE);
                lblImage.Text = _mapper.get(WikiImage._TABLENAME, "FILENAME");
                lblTitle.Text = _mapper.get(WikiImage._TABLENAME, "TITLE");
                lblDescription.Text = _mapper.get(WikiImage._TABLENAME, "DESCRIPTION");
                tbTitle.Text = ImageTitle;
            }
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

        protected void ok_Click(object sender, System.EventArgs e) {
            _db.connect();
            try {
                if (tbTitle.Text == ""){
                    tbTitle.Text = System.IO.Path.GetFileName(newImage.PostedFile.FileName);
                }
                _db.WikiImage.addPicture(newImage.PostedFile, tbTitle.Text, tbDescription.Text, OwnerUID);
            }
            catch(Exception ex) {
                Logger.Log(ex, Logger.ERROR);
            }
            finally {
                _db.disconnect();
            }
            if (BackURL != ""){
                Response.Redirect(BackURL);
            }
            else{
                ((PsoftContentPage)Page).RemoveBreadcrumbItem();
                ((PsoftContentPage)Page).RedirectToPreviousPage();
            }
        }
    }
}
