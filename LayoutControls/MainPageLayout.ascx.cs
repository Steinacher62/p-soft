namespace ch.appl.psoft.LayoutControls
{
    using System;
    //using System.Drawing;


    /// <summary>
    ///		Summary description for MainPageLayout.
    /// </summary>
    public partial class MainPageLayout : PageLayoutControl
	{


		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (Global.Config.BackgroundStartpage.Length > 0)
            {
                mainLayoutTable.Style.Add("background-image", ".."+ Global.Config.BackgroundStartpage);
                mainLayoutTable.Style.Add("background-size", "100%");
                //mainLayoutTable.Style.Add("opacity", "0.1");
                //mainLayoutTable.Style.Add("filter", "alpha(opacity = 50)");
            }
        }

		public static string Path
		{
			get {return Global.Config.baseURL + "/LayoutControls/MainPageLayout.ascx";}
		}

		#region Protected overrided methods from parent class
		protected override void DoSetContentLayoutControl(ContentLayoutControl layout)
		{
			mainCell.Controls.Add(layout);
		}

		protected override void DoSetErrorMessage(string message)
		{
			lblError.Text = message;
			lblError.Visible = ((message != null) && (message != ""));
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
