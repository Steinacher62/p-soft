using System;

namespace ch.appl.psoft.Common
{
    using ch.appl.psoft.Common.Controls;
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.Util;
    using db;
    using Interface;

    public partial class ManageUIDAssignments : PsoftDetailPage {
        public const string PAGE_URL = "/Common/ManageUIDAssignments.aspx";

        static ManageUIDAssignments(){
            SetPageParams(PAGE_URL, "fromUID", "orderColumn", "orderDir");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }

        public ManageUIDAssignments() : base(){
            PageURL = PAGE_URL;
        }

        private long _fromUID = -1L;
        
        protected override void Initialize() {
            base.Initialize();

            _fromUID = GetQueryValue("fromUID", -1L);
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            DBData db = DBData.getDBData(Session);

            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");

            // Setting content layout of page layout
            DGLContentLayout dglControl = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = dglControl;

            db.connect();
            try {
                if (_fromUID > 0L){
                    (PageLayoutControl as PsoftPageLayout).PageTitle = BreadcrumbCaption = _mapper.get("uidAssignment", "manageAssignments");
                    (PageLayoutControl as PsoftPageLayout).PageTitle += " - " + db.UID2NiceName(_fromUID, _mapper);

                    UIDAssignmentsListCtrl list = (UIDAssignmentsListCtrl) LoadPSOFTControl(UIDAssignmentsListCtrl.Path, "_list");
                    list.FromUID = _fromUID;
                    list.OrderColumn = GetQueryValue("orderColumn", db.langAttrName("UID_ASSIGNMENT_V", "TO_NICENAME"));
                    list.OrderDir = GetQueryValue("orderDir", "asc");
                    list.SortURL = psoft.Common.ManageUIDAssignments.GetURL("fromUID",_fromUID);
                    list.RowsPerPage = SessionData.getRowsPerListPage(Session) * 2;

                    SetPageLayoutContentControl(DGLContentLayout.DETAIL, list);

                    PsoftLinksControl links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                    links.LinkGroup1.Caption = _mapper.get("uidAssignment", "ctManageAssignments");
                    links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get("uidAssignment", "newAssignment"), psoft.Common.AddUIDAssignments.GetURL("fromUID",_fromUID, "nextURL",Request.RawUrl));

                    SetPageLayoutContentControl(DGLContentLayout.LINKS, links);	
                }
            }
            catch (Exception ex) {
                ShowError(ex.Message);
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                db.disconnect();
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

        }
		#endregion
    }
}
