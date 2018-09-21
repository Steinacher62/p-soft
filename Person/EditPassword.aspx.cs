using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Person.Controls;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Person
{
    /// <summary>
    /// Summary description for EditPassword.
    /// </summary>
    public partial class EditPassword : PsoftEditPage
    {
        private const string PAGE_URL = "/Person/EditPassword.aspx";

        static EditPassword(){
            SetPageParams(PAGE_URL, "backURL", "ID");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public EditPassword() : base() {
            PageURL = PAGE_URL;
        }

		protected void Page_Load(object sender, System.EventArgs e)
        {
			this.BreadcrumbCaption = _mapper.get("administration","editPassword");

			// Setting main page layout
			PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            (PageLayoutControl as PsoftPageLayout).PageTitle = _mapper.get("administration","editPassword");

            // Check rights...
            DBData db = DBData.getDBData(Session);
            db.connect();
            try {
                if (!db.Person.canChangePassword(db.userId)){
                    RedirectToPreviousPage();
                    return;
                }
            }
            catch (Exception ex) {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally {
                db.disconnect();
            }

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			PasswordEdit _detail = (PasswordEdit)this.LoadPSOFTControl(PasswordEdit.Path, "_detail");
			_detail.BackUrl = GetQueryValue("backURL", "");
			_detail.IDParam = GetQueryValue("ID", "-1");

			// Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, _detail);		
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
			this.ID = "Edit";

		}
		#endregion
    }
}
