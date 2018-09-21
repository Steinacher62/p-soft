using ch.appl.psoft.Common;
using ch.appl.psoft.Document.Controls;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Document
{
    using Interface.DBObjects;
    /// <summary>
    /// Summary description for Add. !
    /// </summary>
    public partial class Add : PsoftTreeViewPage {

        private const string PAGE_URL = "/Document/Add.aspx";

        static Add(){
            SetPageParams(PAGE_URL, "XID", "clipboardID", "ownerTable", "table", "registryEnable", "backURL", "docType", "context", "triggerUID");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public Add() : base()
        {
            PageURL = PAGE_URL;
        }

        protected virtual void Page_Load(object sender, System.EventArgs e) 
        {

			
			String table = Request.QueryString["table"];
			DokAblageAdd _dokAbl = null;

			// Setting main page layout
			PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;
			PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
			DGLContentLayout contentLayout = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
			PageLayoutControl.ContentLayoutControl = contentLayout;

			if(table == "EXCHANGE_FOLDER") 
			{
				MailingListAddCtrl _mlAdd = (MailingListAddCtrl) this.LoadPSOFTControl(MailingListAddCtrl.Path, "_mlAdd");
				_mlAdd.XID = GetQueryValue("XID", 0);
				
				//Setting content layout user controls
				SetPageLayoutContentControl(DGLContentLayout.DETAIL, _mlAdd);		
			} 
			else 
			{
				// Setting parameters
				_dokAbl = (DokAblageAdd)this.LoadPSOFTControl(DokAblageAdd.Path, "_dokAbl");
				_dokAbl.Kontext = GetQueryValue("context", "clipboard"); // mode can be 'clipboard' or 'knowledge'
				_dokAbl.XID = GetQueryValue("XID", 0);
				_dokAbl.TriggerUID = GetQueryValue("triggerUID", 0);
				_dokAbl.ClipboardID = GetQueryValue("clipboardID", 0);
				_dokAbl.OwnerTable = GetQueryValue("ownerTable", "");
				_dokAbl.TableName = GetQueryValue("table", _dokAbl.TableName);
				_dokAbl.RegistryEnable = bool.Parse(GetQueryValue("registryEnable", _dokAbl.RegistryEnable.ToString()));
				_dokAbl.BackURL = GetQueryValue("backURL","");
				_dokAbl.DocumentType = (Document.DocType) GetQueryValue("docType",0);

				//Setting content layout user controls
				SetPageLayoutContentControl(DGLContentLayout.DETAIL, _dokAbl);		
			}
			
            switch (table)
            {
                case "DOCUMENT":
                    (PageLayoutControl as PsoftPageLayout).PageTitle = _mapper.get("document", "new"+_dokAbl.DocumentType);
                    this.BreadcrumbCaption = _mapper.get("document", "new"+_dokAbl.DocumentType);
                    break;
                case "FOLDER":
                    (PageLayoutControl as PsoftPageLayout).PageTitle = _mapper.get("document", "newFolder");
                    this.BreadcrumbCaption = _mapper.get("document", "newFolder");
                    break;
				case "EXCHANGE_FOLDER":
					(PageLayoutControl as PsoftPageLayout).PageTitle = _mapper.get("document", "newPublicEXCHGFolder");
					this.BreadcrumbCaption = _mapper.get("document", "newPublicEXCHGFolder");
					break;
            }
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
        private void InitializeComponent() {    
            this.ID = "Add";

        }
		#endregion
    }
}