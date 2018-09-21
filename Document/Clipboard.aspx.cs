using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Document.Controls;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;


namespace ch.appl.psoft.Document
{
    /// <summary>
    /// Summary description for Clipboard.
    /// </summary>
    public partial class Clipboard : PsoftDetailPage 
	{
        private const string PAGE_URL = "/Document/Clipboard.aspx";

        static Clipboard(){
            SetPageParams(PAGE_URL, "ownerTable", "orderColumn", "orderDir", "documentAddEnable", "registryEnable", "ID", "selectedFolderID");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        private PsoftLinksControl _links = null;
        protected string _ownerTable = "";

        public Clipboard() : base()
        {
            ShowProgressBar = false;
            PageURL = PAGE_URL;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get("clipboard");
            _ownerTable = GetQueryValue("ownerTable","");

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			DokAblageTreeview _dokAbl = (DokAblageTreeview)this.LoadPSOFTControl(DokAblageTreeview.Path, "_dokAbl");
			_dokAbl.OrderColumn = GetQueryValue("orderColumn","TITLE");
			_dokAbl.OrderDir = GetQueryValue("orderDir","asc");
			_dokAbl.DocumentAddEnable = bool.Parse(GetQueryValue("documentAddEnable","true"));
			_dokAbl.RegistryEnable = bool.Parse(GetQueryValue("registryEnable","true"));
            _dokAbl.ClipboardID = GetQueryValue("ID", _dokAbl.ClipboardID);
            _dokAbl.OwnerTable = _ownerTable;
            _dokAbl.SelectedFolderID = GetQueryValue("selectedFolderID", _dokAbl.SelectedFolderID);
            _dokAbl.ActiveXErrorTooltip = PSOFTConvert.ToJavascript(_mapper.get("document","ActiveXErrorTooltip"));

			//Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, _dokAbl);		
	
			_links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
            
            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                if (_dokAbl.ClipboardID > 0)
                {
                    PsoftPageLayout.PageTitle = _mapper.get("clipboard");
                    if (_ownerTable != "")
                    {
                        long ownerID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", _ownerTable, "CLIPBOARD_ID=" + _dokAbl.ClipboardID, false), -1);
                        long uid = db.ID2UID(ownerID, _ownerTable);
                        string niceName = db.UID2NiceName(uid, _mapper, false);
                        if (niceName != "")
                            PsoftPageLayout.PageTitle += " " + niceName;
                    }
                }
                    
                if (_dokAbl.SelectedFolderID <= 0)
                    _dokAbl.SelectedFolderID = ch.psoft.Util.Validate.GetValid(db.lookup("FOLDER_ID", "CLIPBOARD", "ID=" + _dokAbl.ClipboardID, false), 0);

                if (_dokAbl.SelectedFolderID > 0 && db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "FOLDER", _dokAbl.SelectedFolderID, true, true))
                {
                    _links.LinkGroup1.Caption = "";
                    if (db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "FOLDER", _dokAbl.SelectedFolderID, true, true))
                    {
                        _links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get("document","newDocument"), _dokAbl.NewDocumentPath);
                        _links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get("document","newDocument_Link"), _dokAbl.NewDocumentLinkPath);
                        _links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get("document","newFolder"), _dokAbl.NewFolderPath);
						if(Global.Config.isModuleEnabled("module_exchange")) 
						{
							_links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get("document","newPublicEXCHGFolder"), _dokAbl.NewExchangeFolderPath);
						}//if
                    }
					if (db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "FOLDER", _dokAbl.SelectedFolderID, true, true))
					{
                        _links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get("document","editFolder"), _dokAbl.EditFolderPath);
                    }
                    _links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get("document","get"), psoft.Document.ShelfLocalCopy.GetURL("ID",_dokAbl.SelectedFolderID), "id=\"getDocumentLink\"");

                    _links.LinkGroup1.AddAboLinks("FOLDER", _dokAbl.SelectedFolderID, _mapper);

                    PsoftPageLayout.PageTitle += " - " + db.lookup("TITLE", "FOLDER", "ID=" + _dokAbl.SelectedFolderID, false);

                    PsoftPageLayout.ShowButtonAuthorisation("FOLDER", _dokAbl.SelectedFolderID);
                    PsoftPageLayout.ShowButtonRegistryEntries("FOLDER", _dokAbl.SelectedFolderID, db.Registry.getRegistryIDs(DBData.BuildSQLArray(db.Folder.getParentFolderIDList(_dokAbl.SelectedFolderID, false).ToArray()), "FOLDER"));
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                db.disconnect();
            }

			SetPageLayoutContentControl(DGLContentLayout.LINKS, _links);		
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
