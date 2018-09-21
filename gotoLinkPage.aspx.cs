using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Web;

namespace ch.appl.psoft
{
    /// <summary>
    /// Summary description for GotoLinkPage.
    /// </summary>
    public partial class GotoLinkPage : System.Web.UI.Page
	{
        protected long _ID = -1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            _ID = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"],_ID);
            DBData db = DBData.getDBData(Session);
            string URL = "";
            try
            {
                db.connect();
                URL = ch.psoft.Util.Validate.GetValid(db.lookup("URL", "LINKPAGE", "ID=" + _ID, false), "");
                if (URL != "")
                {
                    URL = Global.Config.baseURL + URL;
                    bool isFirst = URL.IndexOf("?") < 0;
                    foreach (string key in Request.QueryString.AllKeys)
                    {
                        if (key != "id")
                        {
                            if (isFirst)
                            {
                                URL += "?";
                                isFirst = false;
                            }
                            else
                                URL += "&";
                            URL += key + "=" + HttpUtility.UrlEncode(Request.QueryString[key].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                URL = "";
            }
            finally
            {
                db.disconnect();
                if (URL == "")
                    URL = psoft.NotFound.GetURL();
                Response.Redirect(URL);
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
