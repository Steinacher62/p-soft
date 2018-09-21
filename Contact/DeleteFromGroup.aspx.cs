using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Contact
{
    /// <summary>
    /// Summary description for DeleteFromGroup.
    /// </summary>
    public partial class DeleteFromGroup : PsoftEditPage {

        public DeleteFromGroup() : base() {
            ShowProgressBar = false;
        }

        /// <summary>
        /// Load page
        /// </summary>
        protected void Page_Load(object sender, System.EventArgs e) 
        {
            long contactID = ch.psoft.Util.Validate.GetValid(Request.QueryString["contactID"], -1);
            long contactGroupID = ch.psoft.Util.Validate.GetValid(Request.QueryString["contactGroupID"], -1);

            if (contactID > 0 && contactGroupID > 0) {
                DBData db = DBData.getDBData(Session);
                db.connect();
                try {
                    db.beginTransaction();
                    db.execute("delete from CONTACT_GROUP_FIRM where FIRM_ID=" + contactID + " and CONTACT_GROUP_ID=" + contactGroupID);
                    db.execute("delete from CONTACT_GROUP_PERSON where PERSON_ID=" + contactID + " and CONTACT_GROUP_ID=" + contactGroupID);
                    db.commit();
                    RedirectToPreviousPage();
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