using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Erstellt eine Vorlage-Kopie einer Pendenzenliste und zeigt die Vorlage an
    /// </summary>
    public partial class TasklistToTemplate : PsoftEditPage
	{
		private const string PAGE_URL = "/Tasklist/TasklistToTemplate.aspx";

		static TasklistToTemplate()
		{
			SetPageParams(PAGE_URL, "ID");
		}

		public static string GetURL(params object[] queryParams)
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		public TasklistToTemplate() : base()
		{
			PageURL = PAGE_URL;
		}

		protected long _ID = -1;
        
		protected void Page_Load(object sender, System.EventArgs e)
		{
			_ID = GetQueryValue("ID", _ID);

			DBData db = DBData.getDBData(Session);

			db.connect();
			db.beginTransaction();

			try 
			{
				_ID = db.Tasklist.copyAsTemplate(_ID);
				db.commit();
			}
			catch(Exception ex)
			{
				db.rollback();
				Logger.Log(ex, Logger.ERROR);
			}
			finally
			{
				db.disconnect();
			}

			string redirectURL
				= psoft.Tasklist.TaskDetail.GetURL("id", _ID, "context", "tasklisttemplate");
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
