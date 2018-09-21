namespace ch.appl.psoft.LayoutControls
{
    using System;
    using System.Web.UI;

    /// <summary>
    ///		Summary description for MainContentLayout.
    /// </summary>
    public partial class MainContentLayout : ContentLayoutControl
	{


		/// <summary>
		/// These are constants for all content place names 
		/// suported by this content layout.
		/// </summary>
		public const string LINKS = "LINKS";
		public const string LIST  = "LIST";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		public static string Path
		{
			get {return Global.Config.baseURL + "/LayoutControls/MainContentLayout.ascx";}
		}

		#region Properities

		public PSOFTUserControl ListControl
		{
			set {SetContentControl(LIST, value);}
		}

		public PSOFTUserControl LinksControl
		{
			set {SetContentControl(LINKS, value);}
		}
		#endregion

		#region Protected overrided method from parent class
		protected override Control DoGetContentPlace(string contentPlaceName) 
		{
			contentPlaceName = contentPlaceName.ToUpper();
			if (contentPlaceName == LIST)
			{
				return listeCell;
			}
			else if (contentPlaceName == LINKS)
			{
				return linksCell;
			}
			else throw new Exception("There is no content place with name: " + contentPlaceName + " on MainContentLayout.");
		}
		#endregion


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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
