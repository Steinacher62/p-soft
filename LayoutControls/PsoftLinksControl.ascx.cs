namespace ch.appl.psoft.LayoutControls
{
    using System;

    /// <summary>
    ///		Summary description for PsoftLinksControl.
    /// </summary>
    public partial  class PsoftLinksControl : PSOFTUserControl 
	{
		protected LinkGroupControl lLinks1;
		protected LinkGroupControl lLinks2;
		protected LinkGroupControl lLinks3;

		public static string Path
		{
			get {return Global.Config.baseURL + "/LayoutControls/PsoftLinksControl.ascx";}
		}

        protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		#region Properities
        public override bool Visible{
            get{return lLinks1.Visible || lLinks2.Visible || lLinks3.Visible;}
        }
        
        public LinkGroupControl LinkGroup1{
            get {return lLinks1;}
        }
        
        public LinkGroupControl LinkGroup2{
            get {return lLinks2;}
        }

        public LinkGroupControl LinkGroup3{
            get {return lLinks3;}
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
