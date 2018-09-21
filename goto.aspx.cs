using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Web;

namespace ch.appl.psoft
{
    /// <summary>
    /// Summary description for Goto.
    /// </summary>
    public partial class Goto : PsoftPage
	{
        public const string PAGE_URL = "/goto.aspx";

        static Goto(){
            SetPageParams(PAGE_URL, "uid", "alias", "noFrames");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }

        public Goto() : base(){
            PageURL = PAGE_URL;
        }

        protected long _uID = -1;
        protected string _alias = "";

		protected void Page_Load(object sender, System.EventArgs e)
		{
            _uID = GetQueryValue("uid", _uID);
            _alias = GetQueryValue("alias", _alias);
            DBData db = DBData.getDBData(Session);
            string URL = "";
            try
            {
                db.connect();

                if (_alias != "")
                    _uID = db.alias2UID(_alias, _uID);

                URL = db.UID2DetailURL(_uID);
                if (URL != "")
                {
                    URL = Global.Config.baseURL + URL;
                    bool isFirst = URL.IndexOf("?") < 0;
                    foreach (string key in Request.QueryString.AllKeys)
                    {
                        if (key != "uid" && key != "alias")
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
                    URL = psoft.NotFound.GetURL("uid",_uID, "alias",_alias);
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
