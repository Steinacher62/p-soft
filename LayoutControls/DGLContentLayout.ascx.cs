namespace ch.appl.psoft.LayoutControls
{
    using ch.appl.psoft.Interface;
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		This is a Detail-Group-Links layout page
    ///		----------------------------
    ///		|              |           |
    ///		|              |           |
    ///		|  DETAIL      |           |
    ///		|--------------| LINKS     |
    ///		|              |           |
    ///		|  GROUP       |           |
    ///		----------------------------
    /// </summary>
    public partial class DGLContentLayout : ContentLayoutControl
	{


		/// <summary>
		/// These are constants for all content place names 
		/// suported by this content layout.
		/// </summary>
		public const string DETAIL = "DETAIL";
		public const string GROUP  = "GROUP";
		public const string LINKS  = "LINKS";

        protected override void OnPreRender( System.EventArgs e ) {
            if (groupRow.Visible){
                if (groupCellDiv.Controls.Count > 0){
                    groupRow.Visible = groupCellDiv.Controls[0].Visible;
                }
                else{
                    groupRow.Visible = false;
                }
            }
            if (linksCell.Visible){
                if (linksCellDiv.Controls.Count > 0){
                    linksCell.Visible = linksMinimizerCell.Visible = linksCellDiv.Controls[0].Visible;
                }
                else{
                    linksCell.Visible = linksMinimizerCell.Visible = false;
                }
            }
            base.OnPreRender(e);
        }
        
        protected void Page_Load(object sender, System.EventArgs e)
		{
            groupMinimizerImg.ImageUrl = Global.Config.baseURL + "/images/minimizer.jpg";
            groupMinimizerImg.ToolTip = LanguageMapper.getLanguageMapper(Session).get("hideShow");
            groupMinimizerImg.Attributes.Add("onclick", "if (" + groupRow.ClientID + ".style.height != '3px'){"
                                                            + groupRow.ClientID + ".origHeight=" + groupRow.ClientID + ".style.height;"
                                                            + groupRow.ClientID + ".style.height='3px';"
                                                            + detailRow.ClientID + ".origHeight=" + detailRow.ClientID + ".style.height;"
                                                            + detailRow.ClientID + ".style.height='';"
                                                        + "} else {"
                                                            + groupRow.ClientID + ".style.height=" + groupRow.ClientID + ".origHeight;"
                                                            + detailRow.ClientID + ".style.height=" + detailRow.ClientID + ".origHeight;"
                                                        + "}");

            linksMinimizerImg.ImageUrl = Global.Config.baseURL + "/images/minimizerVertical.jpg";
            linksMinimizerImg.ToolTip = LanguageMapper.getLanguageMapper(Session).get("hideShow");
            linksMinimizerImg.Style.Add("margin-left", "1px");
            linksMinimizerImg.Attributes.Add("onclick", "if (" + linksCell.ClientID + ".style.display != 'none'){"
                                                            + linksCell.ClientID + ".style.display='none';"
                                                        + "} else {"
                                                            + linksCell.ClientID + ".style.display='';"
                                                        + "}");
        }

		public static string Path
		{
			get {return Global.Config.baseURL + "/LayoutControls/DGLContentLayout.ascx";}
		}

		#region Properities
		public PSOFTUserControl DetailControl
		{
			set {SetContentControl(DETAIL, value);}
		}

		public PSOFTUserControl GroupControl
		{
			set {SetContentControl(GROUP, value);}
		}

		public PSOFTUserControl LinksControl
		{
			set {SetContentControl(LINKS, value);}
		}

        public bool GroupWide{
            get{ return linksCell.RowSpan == 1;}
            set{
                linksCell.RowSpan = linksMinimizerCell.RowSpan = value? 1 : 2;
                groupCell.ColumnSpan = value? 3 : 1;
            }
        }

        public Unit DetailHeight{
            get{ return detailRow.Height; }
            set{ detailRow.Height = value; }
        }

        public Unit GroupHeight{
            get{ return groupRow.Height; }
            set{ groupRow.Height = value; }
        }
        #endregion

		#region Protected overrided method from parent class
		protected override Control DoGetContentPlace(string contentPlaceName) 
		{
			contentPlaceName = contentPlaceName.ToUpper();
			if (contentPlaceName == DETAIL)
			{
				return detailCellDiv;
			}
			else if (contentPlaceName == GROUP)
			{
				return groupCellDiv;
			}
			else if (contentPlaceName == LINKS)
			{
				return linksCellDiv;
			}
			else throw new Exception("There is no content place with name: " + contentPlaceName + " on DGLContentLayout.");
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

        }
		#endregion
	}
}
