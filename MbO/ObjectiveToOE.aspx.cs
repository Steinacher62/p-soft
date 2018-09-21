using ch.appl.psoft.db;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.MbO
{
    /// <summary>
    /// Summary description for ObjectiveToOE.
    /// </summary>
    public partial class ObjectiveToOE : System.Web.UI.Page {
        protected void Page_Load(object sender, System.EventArgs e) {
            long oeId = ch.psoft.Util.Validate.GetValid(Request.QueryString["oeId"],0L);
            long id = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"],0L);
            string backURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["backURL"],"Search.aspx");
            DBData db = DBData.getDBData(Session);

            db.connect();
            try {
                db.Objective.addObjectiveToOE(id,oeId);
                Response.Redirect(backURL,false);
            }
            catch (Exception ex) {
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
