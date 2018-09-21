using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.FBS.Controls;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Data;


namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for FunctionSearch.
    /// </summary>
    public partial class FunctionDetail : PsoftContentPage {
        private FunctionListCtrl _list = null;
        private string _context = "SEARCH";
        private long _contextId = 0;
        private long _id = 0;
        private long _jobId = 0;
        private long _OEId = 0;
        private long _listId = 0;

        protected void Page_Load(object sender, System.EventArgs e) {
            _listId = ch.psoft.Util.Validate.GetValid(Request.QueryString["listId"], 0L);
            _id = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"], 0L);
            _jobId = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobId"], 0L);
            _OEId = ch.psoft.Util.Validate.GetValid(Request.QueryString["OEId"], 0L);
            _context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"], _context);
            _contextId = ch.psoft.Util.Validate.GetValid(Request.QueryString["contextId"], 0L);

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            if (_jobId > 0) BreadcrumbCaption = _mapper.get("fbs","bcJobDescription");
            else BreadcrumbCaption = _mapper.get("fbs","functionDescription");

            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_dl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            JobDescriptionCtrl detail = (JobDescriptionCtrl)this.LoadPSOFTControl(JobDescriptionCtrl.Path, "_detail");
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, detail);

         
            DBData db = DBData.getDBData(Session);
            db.connect();
            try {
                if (_context == "SELECTION") {
                    _list = (FunctionListCtrl)this.LoadPSOFTControl(FunctionListCtrl.Path, "_list");
                    _list.Visible = true;
                    _list.context = _context;
                    _list.contextId = _contextId;
                    if (_id <= 0 && _jobId <= 0) {
                        DataTable table = _list.loadTable(1);
                        if (table != null && table.Rows.Count > 0) {
                            _id = (long) table.Rows[0]["FID"];
                            _listId = (long) table.Rows[0]["JOIN_ID"];
                        }
                    }
                    _list.id = _listId;
                    SetPageLayoutContentControl(DGLContentLayout.GROUP, _list);
                }
                detail.FunktionID = _id; 
                detail.JobID = _jobId; 
                detail.OEID = _OEId; 
                if (_id > 0) {
                    PsoftPageLayout.PageTitle = db.lookup(db.langAttrName("FUNCGROUPV","FTITLE"), "FUNCGROUPV", "FID=" + _id, true);

                    PsoftLinksControl links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                    links.LinkGroup1.Caption = _mapper.get("importantLinks");
                    if (Global.Config.isModuleEnabled("fbw")) links.LinkGroup1.AddLink(_mapper.get("show"), _mapper.get("fbs", "functionRating"), "/FBW/FunctionRating.aspx?functionID="+_id);
                    SetPageLayoutContentControl(DGLContentLayout.LINKS, links);		
                }
                else if (_jobId > 0) PsoftPageLayout.PageTitle = db.lookup("TITLE", "JOBPERSONV", "ID=" + _jobId, true);
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
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
