using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Summary description for RemoveTasklistAssignment.
    /// </summary>
    public partial class RemoveTasklistAssignment : PsoftPage
    {
		private const string PAGE_URL = "/Tasklist/RemoveTasklistAssignment.aspx";

		static RemoveTasklistAssignment()
		{
			SetPageParams(PAGE_URL, "ID", "rootID");
		}

		public static string GetURL(params object[] queryParams)
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		public RemoveTasklistAssignment() : base()
		{
			PageURL = PAGE_URL;
		}

		protected long _ID = -1;
        protected long _rootID = -1;
        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            _ID = GetQueryValue("ID", _ID);
            _rootID = GetQueryValue("rootID", _rootID);
            string redirectURL = psoft.Tasklist.TaskDetail.GetURL("id", _rootID, "rootID", _rootID);
            DBData db = DBData.getDBData(Session);

            try
            {
                db.connect();
                long parentTasklistID = db.Tasklist.getAssignedParentTasklist(_ID, _rootID);
                db.execute("delete from TASKLIST_ASSIGNMENT where PARENT_TASKLIST_ID=" + parentTasklistID + " and ASSIGNED_TASKLIST_ID=" + _ID);
                redirectURL = psoft.Tasklist.TaskDetail.GetURL("id", parentTasklistID, "rootID", _rootID);
            }
            catch (Exception ex)
            {
                Logger.Log(ex,Logger.ERROR);
            }
            finally
            {
                db.disconnect();
            }

            Response.Redirect(redirectURL);
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
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
        private void InitializeComponent()
        {    
        }
		#endregion
    }
}
