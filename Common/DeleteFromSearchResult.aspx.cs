using ch.appl.psoft.db;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Deletes a record (parameter rowID) from a searchresult (parameters searchResultID and tablename) and redirects to the previous page.
    /// </summary>
    public partial class DeleteFromSearchResult : PsoftEditPage {
        private const string PAGE_URL = "/Common/DeleteFromSearchResult.aspx";

        static DeleteFromSearchResult(){
            SetPageParams(PAGE_URL, "searchResultID", "rowID", "tablename");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public DeleteFromSearchResult() : base() {
            ShowProgressBar = false;
            PageURL = PAGE_URL;
        }

        protected void Page_Load(object sender, System.EventArgs e) 
        {
            long searchResultID = GetQueryValue("searchResultID", -1);
            long rowID = GetQueryValue("rowID", -1);
            string tablename = GetQueryValue("tablename", "");

            if (searchResultID > 0 && rowID > 0 && tablename != "") {
                DBData db = DBData.getDBData(Session);
                db.connect();
                try {
                    db.beginTransaction();
                    db.execute("delete from SEARCHRESULT where ID=" + searchResultID + " and TABLENAME='" + tablename + "' and ROW_ID=" + rowID);
                    db.commit();
                }
                catch (Exception ex) {
                    Logger.Log(ex, Logger.ERROR);
                    ShowError(ex.Message);
                }
                finally {
                    db.disconnect();
                }
                RedirectToPreviousPage();
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