namespace ch.appl.psoft.LayoutControls
{
    using System;

    /// <summary>
    ///		Summary description for ImgButtonControl.
    /// </summary>
    public partial class ImgButtonControl : PSOFTUserControl
	{
		private ImgButtonClickArgs _args = new ImgButtonClickArgs();


		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		public static string Path
		{
			get {return Global.Config.baseURL + "/LayoutControls/ImgButtonControl.ascx";}
		}

		#region Events
		public event ImgButtonClickEventHandler OnImageClick;
		#endregion

		#region Properities
		public string Caption
		{
			get {return imageLbl.Text;}
			set {imageLbl.Text = value;}
		}

		public bool CaptionVisible 
		{
			get {return imageLbl.Visible;}
			set {imageLbl.Visible = value;}
		}

		public string ImageUrl
		{
			get {return imageBtn.ImageUrl;}
			set {imageBtn.ImageUrl = value;}
		}

		public bool ImageVisible
		{
			get {return imageBtn.Visible;}
			set {imageBtn.Visible = value;}
		}

		public ImgButtonClickArgs ClickArgs
		{
			get {return _args;}
		}
		#endregion

		#region Private methods
		protected void imageBtn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (OnImageClick != null)
			{
				_args.ImageClickArgs = e;
				OnImageClick(sender, _args);
			}
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

	public class ImgButtonClickArgs
	{
		private bool _displayUserControl = false;
		private string _userControlPath = "";
		private string _targetUrl = "";
		private System.Web.UI.ImageClickEventArgs _imageClickArgs = null;

		public ImgButtonClickArgs(){}

		public bool DisplayUserControl
		{
			get {return _displayUserControl;}
			set {_displayUserControl = value;}
		}

		public string UserControlPath
		{
			get {return _userControlPath;}
			set {_userControlPath = value;}
		}

		public string TargetUrl
		{
			get {return _targetUrl;}
			set {_targetUrl = value;}
		}

		public System.Web.UI.ImageClickEventArgs ImageClickArgs
		{
			get {return _imageClickArgs;}
			set {_imageClickArgs = value;}
		}
	}

	public delegate void ImgButtonClickEventHandler (object sender, ImgButtonClickArgs e);
}