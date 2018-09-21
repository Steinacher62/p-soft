using ch.appl.psoft.Common.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{

    public partial class RegistryEntries : PsoftTreeViewPage {

        private const string PAGE_URL = "/Common/RegistryEntries.aspx";

        static RegistryEntries(){
            SetPageParams(PAGE_URL, "UID", "parentEntries");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        protected string _parentEntries = "";
        protected long _UID = -1L;

        public RegistryEntries() : base() {
            PageURL = PAGE_URL;
        }

        protected override void Initialize() {
            base.Initialize();
            BreadcrumbVisible = false;

            _parentEntries = GetQueryValue("parentEntries", _parentEntries);
            _UID = GetQueryValue("UID", _UID);

            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                PageTitle = _mapper.get("registry", "dialogTitle").Replace("#1", db.UID2NiceName(_UID, _mapper));
            }
            catch(Exception ex) {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally{
                db.disconnect();
            }
        }

        protected virtual void Page_Load(object sender, System.EventArgs e) {

            // Setting main page layout
            SimplePageLayout simplePageLayout = (SimplePageLayout) LoadPSOFTControl(SimplePageLayout.Path, "_pl");;
            PageLayoutControl = simplePageLayout;
            simplePageLayout.Height = Unit.Percentage(100);

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout) LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            contentLayout.Height = Unit.Percentage(100);

            // Setting detail parameters
            RegistryEntriesCtrl registryEntriesCtrl = (RegistryEntriesCtrl) LoadPSOFTControl(RegistryEntriesCtrl.Path, "_dokAbl");
            registryEntriesCtrl.UID = _UID;
            registryEntriesCtrl.ParentEntries = _parentEntries;

            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, registryEntriesCtrl);
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
