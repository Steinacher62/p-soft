using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for PsoftPage
    /// </summary>
    public abstract class PsoftPage : PSOFTPage
    {
        protected LanguageMapper _mapper = null;
		protected Config _config = Global.Config;

        public PsoftPage() : base() {}
		public PsoftPage(string method) : base(method) {}


        #region Properities
        private string _pageURL = "";
        /// <summary>
        /// The page's base URL (i.e. "/MyModule/MyPage.aspx").
        /// </summary>
        public string PageURL {
            get {return _pageURL;}
            set {_pageURL = value;}
        }
        #endregion

        #region Page-parameters functionality
        private static Hashtable PageParams = new Hashtable();

        /// <summary>
        /// Defines the page's URL parameters.
        /// </summary>
        /// <param name="pageURL">The page's base-URL (i.e. "/MyModule/MyPage.aspx")</param>
        /// <param name="pageParams">All the valid URL parameters</param>
        protected static void SetPageParams(string pageURL, params string[] pageParams){
            if (PageParams[pageURL.ToLower()] == null){
                Hashtable pageParamsLower = null;
                if (pageParams.Length > 0){
                    pageParamsLower = new Hashtable();
                    foreach(string pageParam in pageParams){
                        pageParamsLower.Add(pageParam.ToLower(), pageParam);
                    }
                }
                PageParams.Add(pageURL.ToLower(), pageParamsLower);
            }
            else{
                Logger.Log("PsoftPage.SetPageParams: The parameters for page '" + pageURL + "' are already defined.", Logger.ERROR);
            }
        }

        private static Hashtable GetPageParams(string pageURL){
            return PageParams[pageURL.ToLower()] as Hashtable;
        }

        private static string GetPageParam(string pageURL, Hashtable pageParams, string paramName){
            if (paramName.IndexOf('=') >= 0){
                Logger.Log("PsoftPage.GetPageParam: The parameter '" + paramName + "' for page '" + pageURL + "' contains a '='. Maybe this was not intended.", Logger.WARNING);
            }
            if (pageParams != null){
                return pageParams[paramName.ToLower()] as string;
            }
            return null;
        }

        private static string GetPageParamString(string pageURL, Hashtable pageParams, string paramName, string paramValue){
            string paramString = "";
            string param = GetPageParam(pageURL, pageParams, paramName);
            if (param != null){
                paramString = param + "=" + HttpUtility.UrlEncode(paramValue);
            }
            else{
                Logger.Log("PsoftPage.CreateURL: The parameter '" + paramName + "' is not defined on page '" + pageURL + "'.", Logger.ERROR);
            }
            return paramString;
        }

        /// <summary>
        /// Creates a URL with query parameters for a certain page.
        /// The parameter-values will be URL-encoded.
        /// </summary>
        /// <param name="pageURL">The page's base-URL (i.e. "/MyModule/MyPage.aspx")</param>
        /// <param name="queryParams">All the query-parameters. Even parameters (0,2,4,..) identify the parameter-names, odd parameters (1,3,..) are the parameter-values</param>
        /// <returns>The entire URL (i.e. "/Psoft/MyModule/MyPage.aspx?ID=1234&mode=myMode")</returns>
        public static string CreateURL(string pageURL, params object[] queryParams){
            string URL = Global.Config.baseURL + pageURL;
            Hashtable pageParams = GetPageParams(pageURL);
            if (pageParams != null){
                bool isFirst = true;
                for(int i=0; i<queryParams.Length; i+=2){
                    if (i+1 < queryParams.Length){
                        string paramString = GetPageParamString(pageURL, pageParams, queryParams[i].ToString(), queryParams[i+1].ToString());
                        if (paramString != ""){
                            if (isFirst){
                                URL += "?";
                                isFirst = false;
                            }
                            else{
                                URL += "&";
                            }
                            URL += paramString;
                        }
                    }
                }
            }
            else if (queryParams != null && queryParams.Length > 0)
            {
                Logger.Log("PsoftPage.CreateURL: The page '" + pageURL + "' has no parameters defined.", Logger.ERROR);
            }
            return URL;
        }

        /// <summary>
        /// Returns the query-string for a certain URL parameter.
        /// </summary>
        /// <param name="paramName">The name of the URL-parameter</param>
        /// <returns>The query-string or null if not found.</returns>
        private string getRequestQueryString(string paramName){
            string param = GetPageParam(_pageURL, GetPageParams(_pageURL), paramName);
            if (param != null){
                return Request.QueryString[param];
            }
            else{
                Logger.Log("PsoftPage.getRequestQueryString: The parameter '" + paramName + "' is not defined on page '" + _pageURL + "'.", Logger.ERROR);
                return null;
            }
        }

        /// <summary>
        /// Returns the query-string as string, or the default-value if invalid.
        /// </summary>
        /// <param name="queryParam">The name of the query-string parameter</param>
        /// <param name="defaultValue">The default value to return if invalid</param>
        /// <returns></returns>
        public string GetQueryValue(string queryParam, string defaultValue){
            return ch.psoft.Util.Validate.GetValid(getRequestQueryString(queryParam), defaultValue);
        }

        /// <summary>
        /// Returns the query-string as double, or the default-value if invalid.
        /// </summary>
        /// <param name="queryParam">The name of the query-string parameter</param>
        /// <param name="defaultValue">The default value to return if invalid</param>
        /// <returns></returns>
        public double GetQueryValue(string queryParam, double defaultValue){
            return ch.psoft.Util.Validate.GetValid(getRequestQueryString(queryParam), defaultValue);
        }

        /// <summary>
        /// Returns the query-string as long, or the default-value if invalid.
        /// </summary>
        /// <param name="queryParam">The name of the query-string parameter</param>
        /// <param name="defaultValue">The default value to return if invalid</param>
        /// <returns></returns>
        public long GetQueryValue(string queryParam, long defaultValue){
            return ch.psoft.Util.Validate.GetValid(getRequestQueryString(queryParam), defaultValue);
        }

        /// <summary>
        /// Returns the query-string as integer, or the default-value if invalid.
        /// </summary>
        /// <param name="queryParam">The name of the query-string parameter</param>
        /// <param name="defaultValue">The default value to return if invalid</param>
        /// <returns></returns>
        public int GetQueryValue(string queryParam, int defaultValue){
            return ch.psoft.Util.Validate.GetValid(getRequestQueryString(queryParam), defaultValue);
        }
        #endregion

        #region Protected overridden methods from parent class
        protected override void Initialize() {
            base.Initialize();
            _mapper = LanguageMapper.getLanguageMapper(this.Session);
        }

        protected override void AppendCSSLink(StringBuilder cssLinks)
        {
            base.AppendCSSLink(cssLinks);
            cssLinks.Append("<LINK href=\"");
            cssLinks.Append(Global.Config.baseURL);
            cssLinks.Append("/Style/Psoft.css\" type=\"text/css\" rel=\"stylesheet\">");
        }
        #endregion
    }

	/// <summary>
	/// Summary description for PsoftContentPage.
	/// </summary>
	public class PsoftContentPage : PsoftPage
	{
        public PsoftContentPage() : base() {}
		public PsoftContentPage(string method) : base(method) {}

		#region Properities
        private string _breadcrumbCaption = "";
        public string BreadcrumbCaption
        {
            get {return _breadcrumbCaption;}
            set {_breadcrumbCaption = value;}
        }

        private string _breadcrumbName = "";
        public string BreadcrumbName
        {
            get{return _breadcrumbName;}
            set{_breadcrumbName = value;}
        }

        private string _breadcrumbLink = "";
        public string BreadcrumbLink
        {
            get { return _breadcrumbLink;}
            set { _breadcrumbLink = value;}
        }

        private bool _breadcrumbVisible = true;
        public bool BreadcrumbVisible
        {
            get {return _breadcrumbVisible;}
            set {_breadcrumbVisible = value;}
        }
        protected string _subNavMenuUrl = "";
		public string SubNavMenuUrl
		{
			get {return _subNavMenuUrl;}
			set {_subNavMenuUrl = value;}
		}

        protected bool _useWebService = true;
        public bool UseWebService
        {
            get {return _useWebService;}
            set {_useWebService = value;}
        }

        protected bool _showProgressBar = true;
        public bool ShowProgressBar
        {
            get {return _showProgressBar;}
            set {_showProgressBar = value;}
        }
		#endregion

        #region public methods
        public void RemoveBreadcrumbItem()
        {
            ((BreadcrumbItem)Session[BreadcrumbItem.BREAD_CRUMB_ITEM]).RemoveLastChild();
        }

        public void RedirectToPreviousPage()
        {
            Response.Redirect(((BreadcrumbItem)Session[BreadcrumbItem.BREAD_CRUMB_ITEM]).LastChild.Link);
        }

        /// <summary>
        /// Marks a control to get the focus when page is loaded.
        /// </summary>
        /// <param name="control">Control to receive the focus.</param>
        /// <param name="setFocus">True, if focus should be set.</param>
        public static void SetFocusControl(WebControl control, bool setFocus){
            if (control != null){
                control.Attributes.Add("PsoftFocusCtrl", setFocus? "1" : "0");
            }
        }
        #endregion

        #region Protected overridden methods from parent class
        protected override void Initialize() {
            base.Initialize();
            _breadcrumbCaption = this.GetType().Name;
            _breadcrumbName = this.GetType().Name;
            _breadcrumbLink = this.Request.RawUrl;
        }

        protected override void OnPreRender( System.EventArgs e )
        {
            // Create breadcrumb menu item
            if (BreadcrumbVisible)
            {
                BreadcrumbItem newItem = new BreadcrumbItem(BreadcrumbName, BreadcrumbCaption, BreadcrumbLink);
                
                if (Session[BreadcrumbItem.BREAD_CRUMB_ITEM] != null)
                {
                    BreadcrumbItem sessionItem = (BreadcrumbItem)Session[BreadcrumbItem.BREAD_CRUMB_ITEM];
                    sessionItem.SetBreadcrumbItem(newItem);
					while(sessionItem.Count > _config.MaxBreadcrumbItems)
						sessionItem.RemoveFirstChild();
                }
                else
                    Session[BreadcrumbItem.BREAD_CRUMB_ITEM] = newItem;
            }

            base.OnPreRender(e);
        }
        
        protected override void AppendJavaScripts(StringBuilder javaScripts)
		{
            // disabled, now handled by masterpage / 31.08.10 / mkr
            
            //base.AppendJavaScripts(javaScripts);
            //javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/PropertyBox.js'></script>\r\n");
            //if (_useWebService)
            //    javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/PsoftService.js'></script>\r\n");
            //javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/Common.js'></script>\r\n");
            //javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/PopupWindow.js'></script>\r\n");
            //javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/SortTable.js'></script>\r\n");
            //javaScripts.Append("<script language=\"JavaScript\" type='text/javascript'>");
            //javaScripts.Append("propertyBox_Width = 350;");
            //javaScripts.Append("propertyBox_Height = 300;");
            //javaScripts.Append("propertyBox_Path = \"" + Global.Config.baseURL + "/Common/\";");
            //if (_useWebService)
            //    javaScripts.Append("webServicePath = \"" + Global.Config.baseURL + "/\";");
            //javaScripts.Append("deleteConfirmMessage = \"" + PSOFTConvert.ToJavascript(_mapper.get("MESSAGES", "deleteConfirm")) + "\";");
            //javaScripts.Append("</script>");
		}

		protected override void AppendBodyOnLoad(StringBuilder bodyOnLoad)
		{
			base.AppendBodyOnLoad(bodyOnLoad);
			if (_subNavMenuUrl != "")
			{
				bodyOnLoad.Append("if (top.reloadSubNavigation) top.reloadSubNavigation('");
				bodyOnLoad.Append(Global.Config.baseURL);
				bodyOnLoad.Append(_subNavMenuUrl);
				bodyOnLoad.Append("');");
			}
            
            //if (_useWebService)
            //    bodyOnLoad.Append("usePsoftService();");
            
            bodyOnLoad.Append("if (top.reloadBreadcrumbFrame) top.reloadBreadcrumbFrame();");
            bodyOnLoad.Append("if (top.hideProgressBar) top.hideProgressBar();");
            bodyOnLoad.Append("if (window.setPsoftFocusCtrl) setPsoftFocusCtrl();");
        }

		protected override void AppendBodyOnBeforeUnload(StringBuilder bodyOnLoad)
		{
			base.AppendBodyOnBeforeUnload(bodyOnLoad);
            if (_showProgressBar)
            {
                int msDelay = 1500;
                bodyOnLoad.Append("if (top.showProgressBarDelayed) top.showProgressBarDelayed(" + msDelay + ");");
            }
            bodyOnLoad.Append("erasePropertyBox();");
		}

		protected override void AppendAdditionalLiteralControls(StringBuilder literalControls)
		{
			base.AppendAdditionalLiteralControls(literalControls);
            if (_useWebService)
            {
                literalControls.Append("<div id=\"service\" style=\"BEHAVIOR: url(");
                literalControls.Append(Global.Config.baseURL);
                literalControls.Append("/JavaScript/webservice.htc)\"></div>");
            }
		}
		#endregion
	}

    /// <summary>
    /// Summary description for PsoftMenuPage.
    /// </summary>
    public class PsoftMenuPage : PsoftPage
    {
        public PsoftMenuPage() : base() {}

		#region Properities
		#endregion

		#region Protected overrided methods from parent class
        protected override void AppendJavaScripts(StringBuilder javaScripts)
        {
            base.AppendJavaScripts(javaScripts);
            javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/Common.js'></script>\r\n");
            javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/SubNavMenu.js'></script>\r\n");
        }
		#endregion
    }


	/// <summary>
	/// Summary description for PsoftDetailPage.
	/// </summary>
	public class PsoftDetailPage : PsoftContentPage
	{

		public PsoftDetailPage() : base() {}
	}

	/// <summary>
	/// 
	/// </summary>
	public class PsoftEditPage : PsoftContentPage
	{

		public PsoftEditPage() : base() {}

        protected override void AppendJavaScripts(StringBuilder javaScripts) {
            base.AppendJavaScripts(javaScripts);
            javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/WikiBoxTools.js'></script>\r\n");
            javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/Editor.js'></script>\r\n");

        }
    }

	/// <summary>
	/// 
	/// </summary>
	public class PsoftSearchPage : PsoftContentPage
	{
		public PsoftSearchPage() : base(){}
		public PsoftSearchPage(string method) : base(method) {}

	}

	/// <summary>
	/// 
	/// </summary>
	public class PsoftMainPage : PsoftContentPage
	{
		public PsoftMainPage() : base(){}

	}

	/// <summary>
	/// Summary description for PsoftTreeViewPage.
	/// </summary>
	public class PsoftTreeViewPage : PsoftContentPage
	{
		public PsoftTreeViewPage() : base() {
            ShowProgressBar = false;
        }

		public PsoftTreeViewPage(string method) : base(method) {
			ShowProgressBar = false;
		}

		protected override void AppendJavaScripts(StringBuilder javaScripts)
		{
			base.AppendJavaScripts(javaScripts);
			javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/Registry.js'></script>\r\n");
			javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/Tree/ua.js'></script>\r\n");
			javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/Tree/ftiens4.js'></script>\r\n");
		}
	}
}
