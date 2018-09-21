using System;

namespace ch.appl.psoft.Wiki
{
    using ch.appl.psoft.Wiki.Controls;
    using ch.psoft.Util;
    using Common;
    using db;
    using LayoutControls;
    /// <summary>
    /// 
    /// </summary>
    public partial class ImageAdd : PsoftDetailPage {
        public const string PAGE_URL = "/Wiki/ImageAdd.aspx";

        public const string OWNER_UID = "ownerUID";
        public const string BACK_URL = "backURL";
        public const string IMAGE_TITLE = "imageTitle";
        
        static ImageAdd(){
            SetPageParams(PAGE_URL, OWNER_UID, BACK_URL, IMAGE_TITLE);
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public ImageAdd() : base(){
            PageURL = PAGE_URL;
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            DBData db = DBData.getDBData(Session);

            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");

            // Setting content layout of page layout
            DGLContentLayout dglControl = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = dglControl;

            long ownerUID = GetQueryValue(OWNER_UID, -1L);
            db.connect();
            try {
                string tablename = db.UID2Tablename(ownerUID);
                long rowID = db.UID2ID(ownerUID);
                if (ownerUID > 0L && db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, tablename, rowID, true, true)){
                    BreadcrumbCaption = _mapper.get(WikiModule.LANG_SCOPE_WIKI, WikiModule.LANG_MNEMO_BC_ADD_IMAGE);
                    (PageLayoutControl as PsoftPageLayout).PageTitle = BreadcrumbCaption + " - " + db.UID2NiceName(ownerUID, _mapper, true);

                    // Setting parameters
                    ImageAddCtrl add = (ImageAddCtrl) LoadPSOFTControl(ImageAddCtrl.Path, "_add");
                    add.OwnerUID = ownerUID;
                    add.BackURL = GetQueryValue(BACK_URL, "");
                    add.ImageTitle = GetQueryValue(IMAGE_TITLE, "");

                    ImageListCtrl list = (ImageListCtrl) LoadPSOFTControl(ImageListCtrl.Path, "_list");
                    list.OwnerUID = ownerUID;
                    // Setting content layout user controls
                    SetPageLayoutContentControl(DGLContentLayout.DETAIL, add);
                    SetPageLayoutContentControl(DGLContentLayout.GROUP, list);
                }
                else{
                    BreadcrumbVisible = false;
                    Response.Redirect(NotFound.GetURL(), false);
                }
            }
            catch (Exception ex) {
                ShowError(ex.Message);
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                db.disconnect();
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
    }
}
