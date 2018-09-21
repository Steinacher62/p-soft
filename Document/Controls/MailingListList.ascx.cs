namespace ch.appl.psoft.Document.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;

    public partial class MailingListList : PSOFTListViewUserControl 
	{
		
		private DBData _db = null;
		
		protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        //never used, thus commented
        //private string href = "";

		public static string Path 
		{
			get {return Global.Config.baseURL + "/Document/Controls/MailingListList.ascx";}
		}

		public MailingListList() : base() 
		{
			HeaderEnabled = true;
			DeleteEnabled = false;
			EditEnabled = false;
			UseJavaScriptToSort = false;
			UseFirstLetterAsPageSelector = true;
			
		}

		#region Properties
		public long _exchangeFolderID 
		{
			get {return GetLong("xID");}
			set {SetParam("xID", value);}
		}
		
		public long _messageID 
		{
			get {return GetLong("contextID");}
			set {SetParam("contextID", value);}
		}
		#endregion	

       
		protected void Page_Load(object sender, System.EventArgs e) 
		{
			Execute();
		}

		protected override void DoExecute() 
		{
			base.DoExecute();

			_db = DBData.getDBData(Session);
      
			try {
                _db.connect();
				
				string sql = "select ID, MESSAGENAME, EML_FROM, CREATIONDATE, NROFATTACH, FOLDERNAME from EXCHANGE_MESSAGES_TMP where SESSIONID='" + Session.SessionID + "'"; // AND FOLDERNAME='" + ef.Title + "'";
			    
				if(OrderColumn != "") 
				{
					sql += " order by " + OrderColumn + " " + OrderDir;
				}

				IDColumn = "ID";

				DataTable messages = _db.getDataTableExt(sql, "EXCHANGE_MESSAGES_TMP");

				if(messages.Rows.Count > 0) 
				{
					pageTitle.Text = messages.Rows[0][messages.Columns["FOLDERNAME"]].ToString();

					if (_messageID >= 0) 
					{
						HighlightRecordID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "EXCHANGE_MESSAGES_TMP", "ID=" + _messageID + " and SESSIONID='" + Session.SessionID + "'", false), -1);
					}//if

					LoadList(_db, messages, listTab);
				}//if
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
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
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}
