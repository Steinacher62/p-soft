using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Document.Controls;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Text;

namespace ch.appl.psoft.Document
{
    using Interface.DBObjects;
    /// <summary>
	/// Summary description for Edit.
	/// </summary>
	public partial class Edit : PsoftTreeViewPage
	{
        private const string PAGE_URL = "/Document/Edit.aspx";

        static Edit(){
            SetPageParams(PAGE_URL, "xID", "clipboardID", "ownerTable", "selectedFolderID", "table", "registryEnable", "backURL");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public Edit() : base()
        {
            PageURL = PAGE_URL;
        }

		protected override void AppendBodyOnLoad(StringBuilder bodyOnLoad)
		{
			base.AppendBodyOnLoad (bodyOnLoad);
			bodyOnLoad.Append("onLoadTask();");
		}

		protected void Page_Load(object sender, System.EventArgs e) 
		{
			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
			
			String table = Request.QueryString["table"];

			if(table == "EXCHANGE") 
			{
				MailingListEditCtrl _mlEdit = (MailingListEditCtrl) this.LoadPSOFTControl(MailingListEditCtrl.Path, "_mlEdit");
				_mlEdit.XID = GetQueryValue("xID", 0);
				
				//Setting content layout user controls
				SetPageLayoutContentControl(DGLContentLayout.DETAIL, _mlEdit);		
			} 
			else 
			{
				// Setting parameters
				DokAblageEdit _dokAbl = (DokAblageEdit)this.LoadPSOFTControl(DokAblageEdit.Path, "_dokAbl");
				_dokAbl.XID = GetQueryValue("xID", 0);
				_dokAbl.ClipboardID = GetQueryValue("clipboardID", 0);
				_dokAbl.OwnerTable = GetQueryValue("ownerTable", "");
				_dokAbl.FolderID = GetQueryValue("selectedFolderID", 0);
				_dokAbl.TableName = GetQueryValue("table", _dokAbl.TableName);
				_dokAbl.RegistryEnable = bool.Parse(GetQueryValue("registryEnable","true"));
				_dokAbl.BackURL = GetQueryValue("backURL","");

				//Setting content layout user controls
				SetPageLayoutContentControl(DGLContentLayout.DETAIL, _dokAbl);		

				DBData db = DBData.getDBData(Session);
				db.connect();
				try
				{
					switch (_dokAbl.TableName) 
					{
						case "DOCUMENT":
							Document.DocType docType = (Document.DocType) ch.psoft.Util.Validate.GetValid(db.lookup("TYP", "DOCUMENT", "ID=" + _dokAbl.XID, false), (int) Document.DocType.Document);
							BreadcrumbCaption = _mapper.get("document", "edit" + docType);
							break;
						case "FOLDER":
							BreadcrumbCaption = _mapper.get("document", "editFolder");
							break;
					}

					PsoftPageLayout.ShowButtonAuthorisation(_dokAbl.TableName, _dokAbl.XID);
					PsoftPageLayout.PageTitle = BreadcrumbCaption + " - " + db.lookup("TITLE", "FOLDERDOCUMENTV", "ID=" + _dokAbl.XID, false);
				}
				catch(Exception ex) 
				{
					Logger.Log(ex, Logger.ERROR);
					ShowError(ex.Message);
				}
				finally 
				{
					db.disconnect();
				}
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
