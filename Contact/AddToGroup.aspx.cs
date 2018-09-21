using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;

namespace ch.appl.psoft.Contact
{
    /// <summary>
    /// Summary description for AddToGroup.
    /// </summary>
    public partial class AddToGroup : PsoftEditPage {

        public AddToGroup() : base() {
            ShowProgressBar = false;
        }

        /// <summary>
        /// Load page
        /// </summary>
        protected void Page_Load(object sender, System.EventArgs e) 
        {
            long searchResultID = ch.psoft.Util.Validate.GetValid(Request.QueryString["searchResultID"], -1);
            long contactGroupID = ch.psoft.Util.Validate.GetValid(Request.QueryString["contactGroupID"], -1);

            if (searchResultID > 0 && contactGroupID > 0) {
                DBData db = DBData.getDBData(Session);
                db.connect();
                try {
                    db.beginTransaction();
                    // assign the selected contacts to group
                    DataTable table = db.getDataTable("select TABLENAME, ROW_ID from SEARCHRESULT where ID=" + searchResultID);
                    foreach (DataRow row in table.Rows) {
                        string tablename = row["TABLENAME"].ToString();
                        string rowID = row["ROW_ID"].ToString();
                        db.execute("if not exists(select ID from CONTACT_GROUP_" + tablename + " where CONTACT_GROUP_ID=" + contactGroupID + " and " + tablename + "_ID=" + rowID + ") insert into CONTACT_GROUP_" + tablename + " (CONTACT_GROUP_ID, " + tablename + "_ID) values (" + contactGroupID + ", " + rowID + ")");
                    }
                    db.commit();
                    Response.Redirect("ContactDetail.aspx?mode=" + ContactDetail.MODE_CONTACTGROUP + "&xID=" + contactGroupID, false);
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