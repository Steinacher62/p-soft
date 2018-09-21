using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using System.Data;
using Telerik.Web.UI;




namespace ch.appl.psoft.SBS
{

    public partial class Seminars : PsoftDetailPage
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            /// Setting breadcrumb caption
            BreadcrumbCaption = _mapper.get(SbsModule.LANG_SCOPE_SBS, SbsModule.LANG_MNEMO_SEMINARMANAGEMENT);
            // Setting page-title
            //((PsoftPageLayout)PageLayoutControl).PageTitle = _mapper.get(SbsModule.LANG_SCOPE_SBS, SbsModule.LANG_MNEMO_ADD_EDIT_SEMINARS);

            if (!IsPostBack)
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                DataTable seminars = db.getDataTable("Select ID, NAME from SBS_SEMINARS");
                CBSeminars.DataSource = seminars;
                CBSeminars.DataBind();
                db.disconnect();
            }

            if (!IsPostBack && Request.QueryString["SEMINAR"] == null)
            {
                string[] paths = new string[] { "~" + Global.Config.getModuleParam("SBS", "SeminarsURl", "/").ToString() };
                seminarId.Value = "0";
                setPaths(paths);
            }
            if (!IsPostBack && Request.QueryString["SEMINAR"] != null)
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                string actSeminar = db.lookup("NAME", "SBS_SEMINARS", "ID=" + Request.QueryString["SEMINAR"]).ToString();
                string[] paths = new string[] { "~" + Global.Config.getModuleParam("SBS", "SeminarsURl", "/").ToString() };
                paths[0] += "/" + actSeminar;
                setPaths(paths);
                CBSeminars.SelectedValue = Request.QueryString["SEMINAR"];
                seminarId.Value = Request.QueryString["SEMINAR"].ToString();
                seminarUID.Value = db.lookup("UID", "SBS_SEMINARS", "ID=" + Request.QueryString["SEMINAR"]).ToString();
                db.disconnect();
            }


            FileExplorer.EnableCopy = false;

            FileExplorer.ToolBar.CssClass = ("FileExplorerToolbar");
            RadToolBarButton SeminarAddButton = new RadToolBarButton("Neues Seminar");
            //SeminarAddButton.CssClass = "icnUpload rtbWrap";
            SeminarAddButton.Value = "Neues Seminar";
            SeminarAddButton.CommandName = "newSeminarClicked";
            SeminarAddButton.CommandArgument = "newSeminar";
            SeminarAddButton.PostBack = false;
            FileExplorer.ToolBar.Items.Add(SeminarAddButton);

            RadToolBarButton SeminarPreview = new RadToolBarButton("Vorschau");
            //SeminarPreview.CssClass = "icnUpload rtbWrap";
            SeminarPreview.Value = "Vorschau";
            SeminarPreview.CommandName = "SeminarPreviewClicked";
            SeminarPreview.CommandArgument = "preview";
            SeminarPreview.PostBack = false;

            FileExplorer.ToolBar.Items.Add(SeminarPreview);
            FileExplorer.ToolBar.AutoPostBack = false;
            FileExplorer.ToolBar.OnClientButtonClicked = "FileExplorerButtonClicked";
            FileExplorer.ToolBar.Items[3].Visible = false;
            FileExplorer.AsyncUpload.OnClientFileUploaded = "FileExplorerFileAdded";
            FileExplorer.AsyncUpload.OnClientFileUploadRemoving = "FileExplorerFileAddedCancel";
            FileExplorer.OnClientMove = "FileExplorerNodeEdited";
            FileExplorer.TreeView.OnClientContextMenuShowing = "FileExplorerContextMenueShowing";
            FileExplorer.TreeView.EnableDragAndDrop = false;
            FileExplorer.TreeView.EnableDragAndDropBetweenNodes = false;
            FileExplorer.Grid.ClientSettings.AllowRowsDragDrop = true;
            FileExplorer.Grid.ClientSettings.AllowDragToGroup = true;
            FileExplorer.Grid.MasterTableView.Height = 20;



            ClientDataSourceDataForm.DataSource.WebServiceDataSourceSettings.BaseUrl = Global.Config.baseURL + "/WebService/Test.asmx/";

            if (!IsPostBack)
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                DataTable seminars = db.getDataTable("Select ID, NAME from SBS_SEMINARS");
                CBSeminars.DataSource = seminars;
                CBSeminars.DataBind();
                db.disconnect();
            }

            if (Request.Browser.MSDomVersion.Major == 0) // Non IE Browser?
            {
                Response.Cache.SetNoStore(); // No client side cashing for non IE browsers
            }

        }



        private void setPaths(string[] paths)
        {
            FileExplorer.Configuration.ViewPaths = paths;
            FileExplorer.Configuration.UploadPaths = paths;
            FileExplorer.Configuration.DeletePaths = paths;
        }
    }
}

