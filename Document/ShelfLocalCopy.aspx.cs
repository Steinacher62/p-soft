using ch.appl.psoft.Common;
using ch.appl.psoft.Document.Controls;
using ch.appl.psoft.LayoutControls;
using System;
using System.Text;

namespace ch.appl.psoft.Document
{
    /// <summary>
    /// Summary description for ShelfLocalCopy.
    /// </summary>
    public partial class ShelfLocalCopy : PsoftDetailPage
	{
        private const string PAGE_URL = "/Document/ShelfLocalCopy.aspx";

        static ShelfLocalCopy(){
            SetPageParams(PAGE_URL, "ID");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public ShelfLocalCopy() : base()
        {
            ShowProgressBar = false;
            PageURL = PAGE_URL;
        }

        protected override void AppendBodyOnLoad(StringBuilder bodyOnLoad)
        {
            base.AppendBodyOnLoad(bodyOnLoad);
            bodyOnLoad.Append("Go();");
        }

        protected void Page_Load(object sender, System.EventArgs e)
		{
            BreadcrumbCaption = _mapper.get("document", "get");

            // Setting main page layout
            SimplePageLayout pageLayout = (SimplePageLayout) LoadPSOFTControl(SimplePageLayout.Path, "_pl");;
            PageLayoutControl = pageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout)this.LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting detail parameters
            ShelfLocalCopyCtrl ctrl = (ShelfLocalCopyCtrl)this.LoadPSOFTControl(ShelfLocalCopyCtrl.Path, "_ctrl");
            ctrl.FolderID = GetQueryValue("ID", 0);

            //Setting content layout user controls
            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, ctrl);
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
