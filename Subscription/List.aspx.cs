using System;

namespace ch.appl.psoft.Subscription
{
    using ch.appl.psoft.LayoutControls;
    using ch.appl.psoft.Subscription.Controls;
    using Common;
    using Interface.DBObjects;
    /// <summary>
    /// Summary description for List.
    /// </summary>
    /// <param name="context">can be person, oe, job</param>
    /// <param name="id">ID to which the objective is assigned</param>
    public partial class List : PsoftContentPage {
        private const string PAGE_URL = "/Subscription/List.aspx";

        static List(){
            SetPageParams(PAGE_URL, "ID", "context", "orderColumn", "orderDir");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public List() : base() {
            PageURL = PAGE_URL;
        }

        private string _context = News.CONTEXT.SUBSCRIPTION.ToString();
        private long _id = 0;

        #region Protected overrided methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();
            base.ShowProgressBar = false;
            _context = GetQueryValue("context",_context);
            _id = GetQueryValue("id", 0L);

        }
		#endregion
        protected void Page_Load(object sender, System.EventArgs e) {
            string myURL = this.Request.RawUrl;

            base.BreadcrumbCaption = base._mapper.get("news",_context.ToLower());
            base.BreadcrumbName += _context;
            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            (PageLayoutControl as PsoftPageLayout).PageTitle = base._mapper.get("news",_context.ToLower());
            //
            // Setting content layout of page layout
            SimpleContentLayout control = (SimpleContentLayout)this.LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = control;
            ListView list = (ListView)this.LoadPSOFTControl(ListView.Path, "_list");
            list.context = _context;
            list.id = _id;
            list.backURL = myURL;
            list.Visible = true;
            list.OrderColumn = GetQueryValue("orderColumn", _context+".VALID_FROM");
            list.OrderDir = GetQueryValue("orderDir", "asc");
            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, list);	

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
