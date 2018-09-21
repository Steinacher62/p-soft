namespace ch.appl.psoft.LayoutControls
{
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for MainContentLayout.
    /// </summary>
    public partial  class SimpleContentLayout : ContentLayoutControl
	{


        /// <summary>
		/// These are constants for all content place names 
		/// suported by this content layout.
		/// </summary>
		public const string CONTENT = "CONTENT";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		public static string Path
		{
			get {return Global.Config.baseURL + "/LayoutControls/SimpleContentLayout.ascx";}
		}

		#region Properities

		public PSOFTUserControl ContentControl
		{
			set {SetContentControl(CONTENT, value);}
		}

        public Unit Height{
            get {return simpleContentTable.Height;}
            set {
                simpleContentTable.Height = value;
                contentCell.Height = value;
            }
        }
		#endregion

        #region Protected overridden method from parent class
		protected override Control DoGetContentPlace(string contentPlaceName) 
		{
			contentPlaceName = contentPlaceName.ToUpper();
			if (contentPlaceName == CONTENT)
			{
				return contentCell;
			}
			else throw new Exception("There is no content place with name: " + contentPlaceName + " on SimpleContentLayout.");
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
