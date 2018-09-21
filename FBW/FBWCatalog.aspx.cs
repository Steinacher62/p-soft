using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.FBW.Controls;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;


namespace ch.appl.psoft.FBW
{
    /// <summary>
    /// Summary description for FBWCatalog.
    /// </summary>
    public partial class FBWCatalog : PsoftTreeViewPage {

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting parameters
            CatalogCtrl catCtrl = (CatalogCtrl) LoadPSOFTControl(CatalogCtrl.Path, "_cat");
            catCtrl.KriteriumID = ch.psoft.Util.Validate.GetValid(Request.QueryString["KriteriumID"], catCtrl.KriteriumID);
            catCtrl.ArgumentID = ch.psoft.Util.Validate.GetValid(Request.QueryString["ArgumentID"], catCtrl.ArgumentID);

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, catCtrl);
	
            PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get(FBWModule.LANG_SCOPE_FBW, FBWModule.LANG_MNEMO_BC_FBW_CATALOG);

            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                if (catCtrl.KriteriumID <= 0){
                    catCtrl.KriteriumID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "FBW_KRITERIUM", "FBW_ARGUMENT_KATALOG_ID is not null order by LABEL asc", false), -1L);
                }

                if (catCtrl.KriteriumID > 0){
                    PsoftPageLayout.PageTitle += " - " + db.lookup("LABEL", "FBW_KRITERIUM", "ID=" + catCtrl.KriteriumID, false);
                    DataTable kriterien = db.getDataTable("select ID, LABEL from FBW_KRITERIUM where ID<>" + catCtrl.KriteriumID);
                    PsoftLinksControl links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                    foreach (DataRow row in kriterien.Rows){
                        links.LinkGroup1.AddLink(_mapper.get(FBWModule.LANG_SCOPE_FBW, FBWModule.LANG_MNEMO_CM_CRITERIAS), row["LABEL"].ToString(), "/FBW/FBWCatalog.aspx?KriteriumID=" + row["ID"]);
                    }
                    links.LinkGroup1.Caption = _mapper.get(FBWModule.LANG_SCOPE_FBW, FBWModule.LANG_MNEMO_CMT_CRITERIAS);
                    SetPageLayoutContentControl(DGLContentLayout.LINKS, links);		
                }
            }
            catch (Exception ex) {
                ShowError(ex.Message);
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
