using ch.appl.psoft.Common;
using System;
using System.Web.UI;

namespace ch.appl.psoft
{
    /// <summary>
    /// Summary description for GotoCMSPage.
    /// </summary>
    public partial class GotoCMSPage : PsoftContentPage
	{
        public const string PAGE_URL = "/gotoCMSPage.aspx";

        static GotoCMSPage(){
            SetPageParams(PAGE_URL, "URL", "noFrames");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }

        protected string _URL = "";

        protected override void Initialize()
        {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _URL = GetQueryValue("URL", _URL);
        }

        public GotoCMSPage()
        {
            ShowProgressBar = false;
            PageURL = PAGE_URL;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
            BreadcrumbCaption = "CMS";

            if (_URL == "")
            {
                Response.Redirect(psoft.NotFound.GetURL());
            }
            else
            {
                // load and display content of cms page / 05.01.10 / mkr
                // heigt change IE7 error /22.12.10 /msr
                string iframe = "<iframe id=\"contentIFrame\" src=\"" + Global.Config.cmsRootURL + "/" + _URL + "\" border=0 width=\"100%\" height=\"100%\"\">Ihr Browser unterst&uuml;tzt keine iframes.</iframe>";

                // add javascript to set height of iframe (IE 7 does not respect % settings) / 11.01.11 / mkr
                iframe += "<script type=\"text/javascript\">var iframe = document.getElementById('contentIFrame'); var div = document.getElementById('content'); var divHeight = div.offsetHeight; iframe.height = divHeight - 5;</script>";


                LiteralControl lc = new LiteralControl(iframe);
                this.Master.FindControl("ContentPlaceHolder1").Controls.Add(lc);
            }
            
        }

        // disabled, does not work with masterpage / 05.01.10 / mkr
        //protected override void AppendBodyOnLoad(StringBuilder bodyOnLoad)
        //{
        //    base.AppendBodyOnLoad(bodyOnLoad);
        //    if (_URL != "")
        //    {
        //        // redirect is done this way to ensure that the page is loaded and the progress-bar is not shown.
        //        // CMS documents are not derived from PsoftContentPage!
        //        bodyOnLoad.Append("document.location.href='" + Global.Config.cmsRootURL + "/" + _URL + "';");
        //    }
        //}

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
