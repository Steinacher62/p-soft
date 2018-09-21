namespace ch.appl.psoft.LayoutControls
{
    using System;

    /// <summary>
    ///		Summary description for MenuPageLayout.
    /// </summary>
    public partial  class MenuPageLayout : PageLayoutControl
	{


		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		public static string Path
		{
			get {return Global.Config.baseURL + "/LayoutControls/MenuPageLayout.ascx";}
		}

        #region Protected overridden methods from parent class
		protected override void DoSetContentLayoutControl(ContentLayoutControl layout)
		{
			menuCell.Controls.Add(layout);
		}

		protected override void DoSetErrorMessage(string message)
		{
			lblError.Text = message;
			rowError.Visible = ((message != null) && (message != ""));
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
