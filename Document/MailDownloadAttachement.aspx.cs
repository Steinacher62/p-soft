using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;
using System.IO;

namespace ch.appl.psoft.Document
{
    /// <summary>
    /// Summary description for MailDownloadAttachement.
    /// </summary>
    public partial class MailDownloadAttachement : System.Web.UI.Page
	{

		private DBData _db;
		private DataTable _table;
		private string href = "";
		private string mimetype = "";
		private string filename = "";

		#region Properities
		public long _messageID 
		{
			get {return Convert.ToInt64(ch.psoft.Util.Validate.GetValid(Request.QueryString["messageID"],"-1"));}
		}

		public long _attachementID 
		{
			get {return Convert.ToInt64(ch.psoft.Util.Validate.GetValid(Request.QueryString["attachID"],"-1"));}
		}

		public long _exchangeFolderID
		{
			get {return Convert.ToInt64(ch.psoft.Util.Validate.GetValid(Request.QueryString["xID"],"-1"));}
		}
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			_db = DBData.getDBData(Session);

			try 
			{
				_db.connect();

				string sql = "select HREF, MIMETYPE, FILENAME from EXCHANGE_ATTACHEMENTS_TMP where " 
					+ "SESSIONID='" + Session.SessionID + "' AND ID=" + _attachementID + " AND MESSAGE_ID=" + _messageID;
				
				_table = _db.getDataTableExt(sql, "EXCHANGE_MESSAGES_TMP");
				href = _table.Rows[0][_table.Columns["HREF"]].ToString();
				mimetype = _table.Rows[0][_table.Columns["MIMETYPE"]].ToString();
				filename = _table.Rows[0][_table.Columns["FILENAME"]].ToString();

				ExchangeHelper eh = ExchangeHelper.getExchangeHelper(Session, _exchangeFolderID);
				Stream fileStream = eh.getFileStream(href);
				
				Response.ContentType = mimetype;
				Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);

				byte [] buffer = new byte [4096];
				int len = 0;

				while ((len = fileStream.Read(buffer,0,buffer.Length)) > 0 )
				{
					Response.Flush();
					Response.OutputStream.Write(buffer,0,len);
			
				}//while
				Response.End();

				fileStream.Close();
			}
			catch (Exception) 
			{
				//ignore
			}
			finally 
			{
				_db.disconnect();
			}
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
