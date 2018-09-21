using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Document
{
    /// <summary>
    /// Summary description for GetDocument.
    /// </summary>
    public partial class GetDocument : PsoftPage {

        private const string PAGE_URL = "/Document/GetDocument.aspx";

        static GetDocument(){
            SetPageParams(PAGE_URL, "documentID", "historyID", "checkout");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }

        public GetDocument() : base(){
            PageURL = PAGE_URL;
        }
        
        protected void Page_Load(object sender, System.EventArgs e) {
            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                string url = "";
                string file = "";
                long documentID = GetQueryValue("documentID", -1);
                bool checkout = bool.Parse(GetQueryValue("checkout", "false"));
                string downloadName = "";
                while (documentID > 0 && db.Document.getDocType(documentID) == Interface.DBObjects.Document.DocType.Document_Link){
                    documentID = ch.psoft.Util.Validate.GetValid(db.lookup("LINKED_DOCUMENT_ID", "DOCUMENT", "ID=" + documentID, false), -1L);
                }

                if (documentID > 0){
                    if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "DOCUMENT", documentID, true, true)){
                        url = psoft.Document.Detail.GetURL("table","DOCUMENT", "xID",documentID);
                        object[] objs = db.lookup(new string[] {"TYP","XFILENAME","FILENAME"}, "DOCUMENT", "ID=" + documentID);
                        int typ = DBColumn.GetValid(objs[0],0);
                        switch (typ){
                            case (int) Interface.DBObjects.Document.DocType.Document:
                                file = DBColumn.GetValid(objs[1],"");
                                downloadName = DBColumn.GetValid(objs[2], "unknown");
                                if (file != "") url = Global.Config.documentSaveURL + "/" + file;
                                break;
                                
                            default:
                                break;
                        }
                    }
                }
                else {
                    long historyID = GetQueryValue("historyID", -1);
                    if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "DOCUMENT_HISTORY", historyID, true, true)){
                        object[] historyData = db.lookup(new string[] { "HFILENAME", "FILENAME" }, "DOCUMENT_HISTORY", "ID=" + historyID, false);
                        url = Global.Config.documentHistoryURL + "/" + DBColumn.GetValid(historyData[0],"");
                        downloadName = DBColumn.GetValid(historyData[1],"unknown");
                    }
                }
                if (url != ""){
                    if (checkout)
                      db.Document.checkOut(documentID);
                    //Response.Redirect(url, false);
                    //Response.ContentType = "image/jpeg";
                    Response.AppendHeader("Content-Disposition","attachment; filename="+downloadName);
                    Response.TransmitFile(url);
                    Response.End();
                }
                else{
                    Response.Redirect(psoft.NotFound.GetURL(), false);
                }
            }
            catch(Exception ex){
                Logger.Log(ex, Logger.ERROR); 
            }
            finally{
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
