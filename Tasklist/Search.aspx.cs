using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Tasklist.Controls;
using System;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Summary description for SearchFrame.
    /// </summary>
    public partial class Search : PsoftSearchPage
	{
		private const string PAGE_URL = "/Tasklist/Search.aspx";

		static Search()
		{
			SetPageParams(
				PAGE_URL,
				"base",
				"mode",
				"xID",
				"rootID",
				"template",
				"ID",
				"showContacts",
				"orderColumn",
				"orderDir",
				"assignMeasure",
				"assignPerson",
				"subMenu",
                "nextURL"
			);
		}

		public static string GetURL(params object[] queryParams)
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		public Search() : base()
		{
			PageURL = PAGE_URL;
		}
		
		private string _base = "tasklist";
        private string _mode = "";
        private long _xID = -1;
		private long _rootID = -1;
		private bool _template = false;

		private PendenzenTaskList _pList;
		private PendenzenMeasureList _mList;

		#region Protected overrided methods from parent class
		protected override void Initialize()
		{
			// base initialize
			base.Initialize();

			// Retrieving query string values
			_base = GetQueryValue("base", "tasklist").ToLower();
			_mode = GetQueryValue("mode", "").ToLower();
			_xID = GetQueryValue("xID", (long)-1);
			_rootID = GetQueryValue("rootID", (long)-1);
			_template = Boolean.Parse(GetQueryValue("template", "false"));

            SubNavMenuUrl = "/Tasklist/SubNavMenu.aspx','search" + _base + _mode;
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e)
		{
			string pageTitleId = "search" + _base + _mode + (_template ? "template" : "");
			this.BreadcrumbCaption = _mapper.get("tasklist", pageTitleId);
			this.BreadcrumbName = "Tasklist" + pageTitleId;

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
			PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("tasklist", pageTitleId);

			// Setting content layout of page layout
			PageLayoutControl.ContentLayoutControl = (SearchContentLayout)this.LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

			PendenzenSearch _pSearch = (PendenzenSearch)this.LoadPSOFTControl(PendenzenSearch.Path, "_pS");
			_pSearch.Base = _base;
            _pSearch.Mode = _mode;
			_pSearch.Template = _template;
            _pSearch.XID = _xID;
            _pSearch.OnSearchClick +=new SearchClickHandler(_pSearch_OnSearchClick);
			SetPageLayoutContentControl(SearchContentLayout.SEARCH, _pSearch);

			if (_base == "tasklist")
			{
				_pList = (PendenzenTaskList)this.LoadPSOFTControl(PendenzenTaskList.Path, "_pList");
				_pList.Visible = false;
                _pList.OnNextClick += new NextEventHandler(nextClick);
				_pList.DeleteURL = Request.RawUrl;
                SetPageLayoutContentControl(SearchContentLayout.LIST, _pList);
			}
			else if (_base == "measure")
			{
				_mList = (PendenzenMeasureList)this.LoadPSOFTControl(PendenzenMeasureList.Path, "_mList");
				_mList.Visible = false;
                _mList.OnNextClick += new NextEventHandler(nextClick);
                _mList.XID = _xID;
				_mList.DeleteURL = Request.RawUrl;
				SetPageLayoutContentControl(SearchContentLayout.LIST, _mList);
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

		private void _pSearch_OnSearchClick(object Sender, SearchEventArgs e)
		{
			if (_base == "tasklist")
			{
				_pList.Visible = true;
				_pList.SelectedID = GetQueryValue("ID", (long)-1);
				_pList.XID = _xID;
                _pList.RootID = _rootID;
                _pList.ShowContacts = bool.Parse(GetQueryValue("showContacts", "false"));
				_pList.Kontext = "search" + _mode;
				_pList.OrderColumn = GetQueryValue("orderColumn", "TITLE");
				_pList.OrderDir = GetQueryValue("orderDir", "asc");
                _pList.SortEnabled = false;
                _pList.Template = _template;
                _pList.NextURL = GetQueryValue("nextURL", "");
                _pList.Execute();
			}
			else if (_base == "measure")
			{
				_mList.Visible = true;
				_mList.Kontext = "search";
				_mList.SelectedID = GetQueryValue("ID", (long)-1);
				_mList.OrderColumn = GetQueryValue("orderColumn", "DUEDATE");
				_mList.OrderDir = GetQueryValue("orderDir", "asc");
				_mList.AssignMeasure = GetQueryValue("assignMeasure", "enable") == "enable";
				_mList.AssignPerson = GetQueryValue("assignPerson", "enable");
				_mList.SubMenuEnable = GetQueryValue("subMenu", "enable") == "enable";
                _mList.SortEnabled = false;
                _mList.Template = _template;
                _mList.NextURL = GetQueryValue("nextURL", "");
                _mList.Execute();
			}
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50);
        }

        private void nextClick(object Sender, NextEventArgs e) {
            this.Response.Redirect(e.LoadUrl, true);
        }

	}
}
