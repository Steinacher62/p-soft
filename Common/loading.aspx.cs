using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for loading.
    /// </summary>
    public partial class loading : System.Web.UI.Page
	{

        protected string _interval = "200";
        protected string _mnemo = "loadingData";
        protected int _nrOfCells = 20;
        protected string _onloadString = "StartAnimation();";
        protected bool _autoload = true;
    
        protected void Page_Load(object sender, System.EventArgs e)
        {
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
            LoadingBar.ForeColor = Global.HighlightColor;
            _mnemo = ch.psoft.Util.Validate.GetValid(Request.QueryString["mnemo"] as string, _mnemo);
            LoadingLabel.Text = map.get(_mnemo);
            _interval = ch.psoft.Util.Validate.GetValid(Request.QueryString["interval"] as string, _interval);
            _nrOfCells = ch.psoft.Util.Validate.GetValid(Request.QueryString["nrOfCells"] as string, _nrOfCells);
            LoadingBar.NrOfCells = _nrOfCells;
            LoadingBar.Width = _nrOfCells * 15;
            _autoload = bool.Parse(ch.psoft.Util.Validate.GetValid(Request.QueryString["autoload"] as string, "true"));
            if (!_autoload)
            {
                _onloadString = "";
            }
#if DEBUG
            close.Value = map.get("show");
#else
            close.Visible = false;
#endif
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
