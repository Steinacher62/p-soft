using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Training.Controls;
using System;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Training
{
    /// <summary>
    /// Summary description for SearchFrame.
    /// </summary>
    public partial class TrainingSearch : PsoftSearchPage
	{
		private AdvancementList _list;
		// Query string variables
		private string _mode = "";

		#region Protected overrided methods from parent class
		protected override void Initialize()
		{
			// base initialize
			base.Initialize();

        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e)
		{
			this.BreadcrumbCaption = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING,TrainingModule.LANG_MNEMO_BC_SEARCH_ADVANCEMENT);
			this.BreadcrumbName = "AdvancementSearch";

            DBData db = DBData.getDBData(Session);
			_mode = ch.psoft.Util.Validate.GetValid(Request.QueryString["mode"], "");
			if(_mode.Equals("clear"))
				Session["AdvancementSQLSearch"] = null;

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
			PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING,TrainingModule.LANG_MNEMO_BC_SEARCH_ADVANCEMENT);

			// Setting content layout of page layout
			PageLayoutControl.ContentLayoutControl = (SearchContentLayout)this.LoadPSOFTControl(SearchContentLayout.Path, "_sC");
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

			TrainingAdvancementSearch _search = (TrainingAdvancementSearch)this.LoadPSOFTControl(TrainingAdvancementSearch.Path, "_search");
			_search.OnSearchClick +=new SearchClickHandler(_search_OnSearchClick);
			SetPageLayoutContentControl(SearchContentLayout.SEARCH, _search);

			_list = (AdvancementList)this.LoadPSOFTControl(AdvancementList.Path, "_list");
            _list.ContextList = "search";
			_list.Visible = false;
            _list.DetailURL = Global.Config.baseURL + "/Training/Advancement.aspx?advancementID=%ID";
            _list.DetailEnabled = true;
            _list.OrderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["OrderColumn"], _list.OrderColumn);
            _list.OrderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["OrderDir"], _list.OrderDir);
            _list.SortURL = Global.Config.baseURL + "/Training/Search.aspx?";
            SetPageLayoutContentControl(SearchContentLayout.LIST, _list);

            if (ch.psoft.Util.Validate.GetValid(Request.QueryString["OrderColumn"], "") != ""){
                _list.Visible = true;
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

		private void _search_OnSearchClick(object Sender, SearchEventArgs e)
		{
		    _list.Visible = true;
            _list.Execute();
            ((SearchContentLayout) PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(50);
        }

        private void nextClick(object Sender, NextEventArgs e) {
            this.Response.Redirect(e.LoadUrl, true);
        }

	}
}
