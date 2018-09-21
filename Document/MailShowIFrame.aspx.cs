using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;

namespace ch.appl.psoft.Document
{
    /// <summary>
    /// Summary description for WebForm1.
    /// </summary>
    public partial class MailShowIFrame : System.Web.UI.Page
	{
		private DBData _db;
		private DataTable _table;
		private string href = "";

		#region Properities
		public long _messageID {
			get {return Convert.ToInt64(ch.psoft.Util.Validate.GetValid(Request.QueryString["contextID"],"-1"));}
		}

		public long _exchangeFolderID 
		{
			get {return Convert.ToInt64(ch.psoft.Util.Validate.GetValid(Request.QueryString["xID"],"-1"));}
		}

		//public string _style
		//{
		//	get {return ch.psoft.Util.Validate.GetValid(Request.QueryString["style"],"html");}
		//}
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{

			_db = DBData.getDBData(Session);

			try 
			{
				_db.connect();

				string sql = "select HREF from EXCHANGE_MESSAGES_TMP where " 
					+ "SESSIONID='" + Session.SessionID + "' AND ID=" + _messageID;
				
				_table = _db.getDataTableExt(sql, "EXCHANGE_MESSAGES_TMP");
				href = _table.Rows[0][_table.Columns["HREF"]].ToString();

				ExchangeHelper eh = ExchangeHelper.getExchangeHelper(Session, _exchangeFolderID);
				ExchangeMessage em = eh.getMessage(href);
				
				string emailtext = "";
				//if((_style == "" || _style == "html") && em != null) 
				//{
					emailtext = em.getAdditionalAttribute(ExchangeMessage.HTMLDESCRIPTION);
				//	Session["style"] = "html";
				//} 
				//else if (_style == "text" && em != null)
				//{
				//	emailtext = em.getAdditionalAttribute(ExchangeMessage.TEXTDESCRIPTION).Replace("\n","<br />");
				//	Session["style"] = "text";
				//}



				Response.Write(emailtext.ToCharArray(),0,emailtext.Length);
				Response.Flush();
				Response.End();	
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
