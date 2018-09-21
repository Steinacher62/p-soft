using System;

namespace ch.appl.psoft.Common
{

    /// <summary>
    /// Download can be used to download any type of file.
    /// The user has the possibility to save the file on its machine or open it with the assigned application (outside the browser).
    /// 
    /// URL querystring parameters:
    ///  - file: The file to download. Must be a relative URL within the webapplication.
    ///  - contentType: The content-type (mime-type) of the file.
    /// </summary>
    public partial class Download : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            string file = Server.MapPath(ch.psoft.Util.Validate.GetValid(Request.QueryString["file"], ""));
            string filename = file.Substring(file.LastIndexOf("\\") + 1);
            string contentType = ch.psoft.Util.Validate.GetValid(Request.QueryString["contentType"], "");

            Response.Clear();
            Response.ClearHeaders();

            if (contentType != ""){
                Response.AddHeader("content-type", contentType);
            }
            Response.AddHeader("content-disposition", "attachment; filename=" + filename);

            Response.WriteFile(file);
            Response.Flush();
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
