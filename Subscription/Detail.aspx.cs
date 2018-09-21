using System;

namespace ch.appl.psoft.Subscription
{
    using ch.appl.psoft.LayoutControls;
    using ch.appl.psoft.Subscription.Controls;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface.DBObjects;
    /// <summary>
    /// 
    /// </summary>
    public partial class Detail : PsoftTreeViewPage {
        private const string PAGE_URL = "/Subscription/Detail.aspx";

        static Detail(){
            SetPageParams(PAGE_URL, "ID", "context", "view", "triggerName", "triggerView", "triggerID", "backURL");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public Detail() : base() {
            PageURL = PAGE_URL;
        }

        private string _context = News.CONTEXT.SUBSCRIPTION.ToString();
        private string _view = "detail";
        private long _id = 0L;
        private long _triggerId = 0L;
        private string _triggerName = "";
        private string _triggerView = "";
        private string _myURL = "";
        private DBData _db = null;

        protected void Page_Load(object sender, System.EventArgs e) {
            _context = GetQueryValue("context",_context);
            _view = GetQueryValue("view",_view);
            _id = GetQueryValue("id", 0L);
            _triggerName = GetQueryValue("triggerName", "");
            _triggerView = GetQueryValue("triggerView", "");
            _triggerId = GetQueryValue("triggerId", 0L);
            _myURL = GetQueryValue("backURL","");

            base.BreadcrumbCaption = base._mapper.get("news",_view+_context);
            base.BreadcrumbName += _view;
            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            (PageLayoutControl as PsoftPageLayout).PageTitle = base._mapper.get("news",_view+_context);

            _db = DBData.getDBData(Session);
            _db.connect();
            try	{
                // Setting content layout of page layout
                DGLContentLayout control = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
                PageLayoutControl.ContentLayoutControl = control;
                if (_view == "detail") {
                    // detail
                    DetailView detail = (DetailView)this.LoadPSOFTControl(DetailView.Path, "_detail");
                    detail.id = _id;
                    detail.backURL = _myURL;
                    SetPageLayoutContentControl(DGLContentLayout.DETAIL, detail);		
                }
                else { // edit, add
                    EditView edit = (EditView)this.LoadPSOFTControl(EditView.Path, "_detail");
                    edit.action = _view;
                    edit.id = _id;
                    edit.triggerId = _triggerId;
                    edit.triggerName = _triggerName;
                    edit.triggerView = _triggerView;
                    edit.id = _id;
                    edit.context = _context;
                    edit.backURL = _myURL;
                    SetPageLayoutContentControl(DGLContentLayout.DETAIL, edit);	
                }
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                _db.disconnect(); 
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
