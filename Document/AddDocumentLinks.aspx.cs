using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;

namespace ch.appl.psoft.Document
{
    using Interface.DBObjects;

    public partial class AddDocumentLinks : PsoftEditPage {

        private const string PAGE_URL = "/Document/AddDocumentLinks.aspx";

        static AddDocumentLinks(){
            SetPageParams(PAGE_URL, "searchResultID", "folderID", "nextURL");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public AddDocumentLinks() : base() {
            ShowProgressBar = false;
            BreadcrumbVisible = false;
            PageURL = PAGE_URL;
        }

        protected void Page_Load(object sender, System.EventArgs e) 
        {
            long searchResultID = GetQueryValue("searchResultID", -1);
            long folderID = GetQueryValue("folderID", -1);
            string nextURL = GetQueryValue("nextURL", "");

            if (searchResultID > 0 && folderID > 0) {
                DBData db = DBData.getDBData(Session);
                db.connect();
                try {
                    db.beginTransaction();
                    // add the selected documents-links to the folder
                    DataTable table = db.getDataTable("select ROW_ID from SEARCHRESULT where ID=" + searchResultID + " and TABLENAME='DOCUMENT'");
                    bool isFirst = true;
                    string linkPrefix = _mapper.get("document", "linkPrefix");
                    foreach (DataRow row in table.Rows) {
                        string rowID = row["ROW_ID"].ToString();
                        long newID = db.newId("DOCUMENT");
                        string sql = "insert into DOCUMENT (ID, FOLDER_ID, LINKED_DOCUMENT_ID, TYP, INHERIT, TITLE) select " + newID + ", " + folderID + ", " + rowID + ", " + (int)Document.DocType.Document_Link + ", 0, '" + linkPrefix + "' + TITLE from DOCUMENT where ID=" + rowID;
                        db.executeProcedure("MODIFYTABLEROW",
                            new ParameterCtx("ROWS", null, ParameterDirection.Output,typeof(int)),
                            new ParameterCtx("USERID", db.userId),
                            new ParameterCtx("TABLENAME", "DOCUMENT"),
                            new ParameterCtx("ROWID", newID),
                            new ParameterCtx("MODIFY", sql, ParameterDirection.Input,typeof(string),true),
                            new ParameterCtx("INHERIT", 1)
                        );
                        if (isFirst){
                            isFirst = false;
                            nextURL = nextURL.Replace("%25ID","%ID").Replace("%ID", newID.ToString());
                        }
                    }
                    db.commit();
                    Response.Redirect(nextURL, false);
                }
                catch (Exception ex) {
                    Logger.Log(ex, Logger.ERROR);
                    ShowError(ex.Message);
                }
                finally {
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