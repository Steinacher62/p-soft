namespace ch.appl.psoft.LayoutControls
{
    using System;

    /// <summary>
    ///		Summary description for ImgListControl.
    /// </summary>
    public partial class ImgListControl : PSOFTUserControl
	{

		private ImgButtonList _imgList = new ImgButtonList();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			imgListCell.Controls.Add(_imgList.Parse());
		}

		public static string Path
		{
			get {return Global.Config.baseURL + "/LayoutControls/ImgListControl.ascx";}
		}

		public int Count
		{
			get {return _imgList.Count;}
		}

		public int Add(ImgButtonControl imageButtonControl)
		{
			return _imgList.Add(imageButtonControl);
		}

		public void Clear()
		{
			_imgList.Clear();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
