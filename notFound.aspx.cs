using ch.appl.psoft.Common;
using System;

namespace ch.appl.psoft
{
    /// <summary>
    /// Summary description for NotFound.
    /// </summary>
    public partial class NotFound : PsoftDetailPage
	{
        private const string PAGE_URL = "/notFound.aspx";

        static NotFound(){
            SetPageParams(PAGE_URL, "uid", "alias");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }

        public NotFound() : base(){
            PageURL = PAGE_URL;
        }

        protected long _uID = -1;
        protected string _alias = "";

        protected void Page_Load(object sender, System.EventArgs e)
		{
            _uID = GetQueryValue("uid", _uID);
            _alias = GetQueryValue("alias", _alias);
            if (_uID > 0)
                notFoundComment.Text += "UID " + _uID + "<br>";
            if (_alias != "")
                notFoundComment.Text += "Alias '" + _alias + "'<br>";
            notFoundComment.Text += _mapper.get("linkNotFound");
            BreadcrumbCaption = _mapper.get("linkNotFound");
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
