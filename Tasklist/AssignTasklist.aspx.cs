using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;

namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Summary description for AssignTasklist.
    /// </summary>
    public partial class AssignTasklist : PsoftPage
    {
		private const string PAGE_URL = "/Tasklist/AssignTasklist.aspx";

		static AssignTasklist()
		{
			SetPageParams(PAGE_URL, "rootID", "parentID", "searchResultID");
		}

		public static string GetURL(params object[] queryParams)
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		public AssignTasklist() : base()
		{
			PageURL = PAGE_URL;
		}

		protected long _rootID = -1;
        protected long _parentID = -1;
        protected long _searchResultID = -1;
        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            _rootID = GetQueryValue("rootID", _rootID);
            _parentID = GetQueryValue("parentID", _parentID);
            _searchResultID = GetQueryValue("searchResultID", _searchResultID);
            string redirectURL = "TaskDetail.aspx";
            DBData db = DBData.getDBData(Session);

            try
            {
                db.connect();
                DataTable table = db.getDataTable("select distinct ROW_ID from SEARCHRESULT where ID=" + _searchResultID);
                string assignedTasklistID = "";
                foreach (DataRow row in table.Rows){
                    assignedTasklistID = row[0].ToString();
                    db.execute("insert into TASKLIST_ASSIGNMENT (PARENT_TASKLIST_ID, ASSIGNED_TASKLIST_ID) values (" + _parentID + "," + assignedTasklistID + ")");
                }

                if (_rootID <= 0){
                    _rootID = ch.psoft.Util.Validate.GetValid(db.lookup("ROOT_ID", "TASKLIST", "ID=" + _parentID, false), -1L);
                }
                redirectURL += "?id=" + assignedTasklistID + "&rootID=" + _rootID;
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
