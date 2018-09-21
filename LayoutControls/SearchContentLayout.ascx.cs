namespace ch.appl.psoft.LayoutControls
{
    using Interface;
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		This is a Search layout content
    ///		--------------------------
    ///		|                        |
    ///		|       SEARCH           |
    ///		|------------------------|
    ///		|       LIST             |
    ///		|                        |
    ///		--------------------------
    /// </summary>
    public partial  class SearchContentLayout : ContentLayoutControl
	{



		/// <summary>
		/// These are constants for all content place names 
		/// suported by this content layout.
		/// </summary>
		public const string SEARCH  = "SEARCH";
		public const string LIST  = "LIST";

        protected override void OnPreRender( System.EventArgs e ) {
            if (searchRow.Visible){
                if (searchCellDiv.Controls.Count > 0){
                    searchRow.Visible = searchCellDiv.Controls[0].Visible;
                }
                else{
                    searchRow.Visible = false;
                }
            }
            base.OnPreRender(e);
        }
        
        public static string Path
		{
			get {return Global.Config.baseURL + "/LayoutControls/SearchContentLayout.ascx";}
		}

		#region Properities
		public PSOFTUserControl SearchControl
		{
			set {SetContentControl(SEARCH, value);}
		}

		public PSOFTUserControl ListControl
		{
			set {SetContentControl(LIST, value);}
		}

        public Unit SearchHeight{
            get{ return searchRow.Height; }
            set{ searchRow.Height = value; }
        }

        public Unit ListHeight{
            get{ return listRow.Height; }
            set{ listRow.Height = value; }
        }
		#endregion

		#region Protected overrided method from parent class
		protected override Control DoGetContentPlace(string contentPlaceName) 
		{
			contentPlaceName = contentPlaceName.ToUpper();
			if (contentPlaceName == SEARCH)
			{
				return searchCellDiv;
			}
			else if (contentPlaceName == LIST)
			{
				return listCell;
			}
			else throw new Exception("There is no content place with name: " + contentPlaceName + " on SearchContentLayout.");
		}
		#endregion


		protected void Page_Load(object sender, System.EventArgs e)
		{
            searchMinimizerImg.ImageUrl = Global.Config.baseURL + "/images/minimizer.jpg";
            searchMinimizerImg.ToolTip = LanguageMapper.getLanguageMapper(Session).get("hideShow");
            searchMinimizerImg.Attributes.Add("onclick", "if (" + searchRow.ClientID + ".style.height != '3px'){"
                + searchRow.ClientID + ".origHeight=" + searchRow.ClientID + ".style.height;"
                + searchRow.ClientID + ".style.height='3px';"
                + listRow.ClientID + ".origHeight=" + listRow.ClientID + ".style.height;"
                + listRow.ClientID + ".style.height='';"
                + "} else {"
                + searchRow.ClientID + ".style.height=" + searchRow.ClientID + ".origHeight;"
                + listRow.ClientID + ".style.height=" + listRow.ClientID + ".origHeight;"
                + "}");
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
