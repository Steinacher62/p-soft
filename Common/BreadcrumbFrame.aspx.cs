using System;


namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for breadcrumbFrame.
    /// </summary>
    public partial class BreadcrumbFrame : System.Web.UI.Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if (Session[BreadcrumbItem.BREAD_CRUMB_ITEM] != null)
			{
				BreadcrumbItem _item = (BreadcrumbItem)Session[BreadcrumbItem.BREAD_CRUMB_ITEM];
				breadcrumbCell.Text = _item.ToString();
			}
			else
			{
				breadcrumbCell.Text = "---";
			}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
