namespace ch.appl.psoft.LayoutControls
{
    using System;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for SimplePageLayout.
    /// </summary>
    public partial  class SimplePageLayout : PageLayoutControl
	{


		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		public static string Path
		{
			get {return Global.Config.baseURL + "/LayoutControls/SimplePageLayout.ascx";}
		}

        public Unit Height{
            get {return simpleLayoutTable.Height;}
            set {
                simpleLayoutTable.Height = value;
                simpleCell.Height = value;
            }
        }

		#region Protected overrided methods from parent class
		protected override void DoSetContentLayoutControl(ContentLayoutControl layout)
		{
			simpleCell.Controls.Add(layout);
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
