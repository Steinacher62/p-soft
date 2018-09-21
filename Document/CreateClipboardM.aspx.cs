using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Document.Controls;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Document
{
    /// <summary>
	/// Summary description for CreateClipboardM.
	/// </summary>
	public partial class CreateClipboardM : PsoftTreeViewPage
	{
        private const string PAGE_URL = "/Document/CreateClipboardM.aspx";

        static CreateClipboardM(){
            SetPageParams(PAGE_URL, "nextURL", "ownerTable", "ownerID", "registryEnable", "type", "accessorID", "triggerUID");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public CreateClipboardM() : base()
        {
            PageURL = PAGE_URL;
        }

		protected void Page_Load(object sender, System.EventArgs e) 
		{
			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			DokAblageCreate _dokAbl = (DokAblageCreate)this.LoadPSOFTControl(DokAblageCreate.Path, "_dokAbl");
            _dokAbl.NextUrl = GetQueryValue("nextURL","");
            _dokAbl.OwnerTable = GetQueryValue("ownerTable", "");
            _dokAbl.OwnerID = GetQueryValue("ownerID", -1L);
            _dokAbl.TriggerUID = GetQueryValue("triggerUID", -1L);
            _dokAbl.RegistryEnable = bool.Parse(GetQueryValue("registryEnable", "true"));
            _dokAbl.ClipboardType = GetQueryValue("type", Interface.DBObjects.Clipboard.TYPE_PUBLIC);

            //Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, _dokAbl);		

            PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get("createClipboard");

            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                _dokAbl.AccessorID = GetQueryValue("accessorID", db.userAccessorID);
            }
            catch(Exception ex) {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally {
                db.disconnect();
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
