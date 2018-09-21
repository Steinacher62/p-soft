using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Document
{
    /// <summary>
    /// Summary description for Add.
    /// </summary>
    public partial class CreateClipboard : PsoftEditPage {
        private const string PAGE_URL = "/Document/CreateClipboard.aspx";

        static CreateClipboard(){
            SetPageParams(PAGE_URL, "ownerTable", "ownerID", "type", "accessorID");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public CreateClipboard() : base()
        {
            ShowProgressBar = false;
            BreadcrumbVisible = false;
            PageURL = PAGE_URL;
        }

        /// <summary>
        /// Load page
        /// </summary>
        protected void Page_Load(object sender, System.EventArgs e) 
        {
            string ownerTable = GetQueryValue("ownerTable", "");
            long ownerID = GetQueryValue("ownerID", -1);
            int typ = GetQueryValue("type", Interface.DBObjects.Clipboard.TYPE_PUBLIC);

            if (ownerTable != "" && ownerID > 0)
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                try
                {
                    db.beginTransaction();
                    string folderName = _mapper.get("clipboard");
                    string clipboardName = _mapper.get("clipboard") + " " + ownerTable + "_" + ownerID;
                    long folderID = db.newId("FOLDER");
                    db.execute("insert into FOLDER (ID, ROOT_ID, TITLE) values(" + folderID + ", " + folderID + ", '" + folderName + "')");
                    long clipboardID = db.newId("CLIPBOARD");
                    db.execute("insert into CLIPBOARD (ID, FOLDER_ID, TITLE, TYP) values(" + clipboardID + ", " + folderID + ", '" + clipboardName + "', " + typ + ")");
                    db.execute("update " + ownerTable + " set CLIPBOARD_ID=" + clipboardID + " where ID=" + ownerID);

                    // Setting the access-rights on the clipboard. Default: the currently logged user.
                    long accessorID = GetQueryValue("accessorID", db.userAccessorID);
                    if (accessorID > 0){
                        db.grantRowAuthorisation(DBData.AUTHORISATION.FULL_ACCESS, accessorID, "CLIPBOARD", clipboardID);
                    }
                    db.commit();
                    Response.Redirect(Clipboard.GetURL("ID",clipboardID, "ownerTable",ownerTable), false);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, Logger.ERROR);
                    ShowError(ex.Message);
                }
                finally
                {
                    db.disconnect();
                }
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
            this.ID = "Add";

        }
		#endregion
    }
}