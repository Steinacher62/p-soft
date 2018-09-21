using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Document
{
    /// <summary>
    /// Summary description for GetDocument.
    /// </summary>
    public partial class GetFolder : PsoftPage {
        private const string PAGE_URL = "/Document/GetFolder.aspx";

        static GetFolder(){
            SetPageParams(PAGE_URL, "id");
        }

        public GetFolder() : base(){
            PageURL = PAGE_URL;
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        protected void Page_Load(object sender, System.EventArgs e) {
            string url = "";
            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                long id = GetQueryValue("id", -1);
                if (id > 0) {
                    int typ = DBColumn.GetValid(db.lookup("typ","folder","id="+id),0);
                    switch (typ) {
                    case 0:
                        url = psoft.Document.Detail.GetURL("xID",id, "Table","FOLDER", "selectedFolderID",id);
                        break;
                    case 1:
                        url = Global.Config.baseURL + "/Knowledge/Detail.aspx?ID="+id+"&context=forum";
                        break;
                    case 2:
                        url = Global.Config.baseURL + "/Knowledge/Detail.aspx?ID="+id+"&context=topic";
                        break;
                    default:
                        return;
                    }
                    Response.Redirect(url);
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
