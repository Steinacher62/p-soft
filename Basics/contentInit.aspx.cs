using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web;

namespace ch.appl.psoft.Basics
{
    /// <summary>
    /// Summary description for contentInit.
    /// </summary>
    public partial class contentInit : PsoftMainPage
	{
        private const string PAGE_URL = "/Basics/contentInit.aspx";

        static contentInit(){
            SetPageParams(PAGE_URL, "context");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        public contentInit() : base() {
            PageURL = PAGE_URL;
        }

        private string _context = News.CONTEXT.NEWS.ToString();

        protected override void Initialize() {
            // base initialize
            base.Initialize();
            _context = GetQueryValue("context", _context);
            base.ShowProgressBar = false;

        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
            // set culture in session / 12.01.10 / mkr
            Session["culture"] = SessionData.getDBColumn(Session).UserCultureName;

            this.BreadcrumbCaption = _mapper.get("home", "home");
			this.BreadcrumbLink = Global.Config.baseURL + "/Basics/contentInit.aspx";

            string myURL = this.Request.RawUrl;

			// Setting main page layout
			PageLayoutControl = (MainPageLayout)this.LoadPSOFTControl(MainPageLayout.Path, "_mainPl");
	
			// Setting content layout of page layout
			PageLayoutControl.ContentLayoutControl = (MainContentLayout)this.LoadPSOFTControl(MainContentLayout.Path, "_mainCl");

            // Creating image list control
            ImgListControl _imgList = (ImgListControl)this.LoadPSOFTControl(ImgListControl.Path, "_imgList");

			// Setting up all image buttons on image list
			// MyEntry
			ImgButtonControl _btn = (ImgButtonControl)this.LoadPSOFTControl(ImgButtonControl.Path);
			_btn.Caption = _mapper.get("home", "myEntry");
			_btn.ClickArgs.TargetUrl = psoft.Person.DetailFrame.GetURL("id",UserID, "mode","oe");
			_btn.ImageUrl = Global.Config.baseURL + "/images/pikto/mein_eintrag.gif";
			_btn.OnImageClick +=new ImgButtonClickEventHandler(ImgButtonClick);
			_imgList.Add(_btn);

			// TaskList
			if (Global.isModuleEnabled("tasklist")) 
			{
				_btn = (ImgButtonControl)this.LoadPSOFTControl(ImgButtonControl.Path);
				_btn.Caption = _mapper.get("home", "myTasks");
				_btn.ClickArgs.TargetUrl = psoft.Tasklist.MeasureDetail.GetURL("context","responsible", "xID",UserID);
				_btn.ImageUrl = Global.Config.baseURL + "/images/pikto/meine_pendenzen.gif";
				_btn.OnImageClick +=new ImgButtonClickEventHandler(ImgButtonClick);
				_imgList.Add(_btn);
			}

			// Project
			if (Global.isModuleEnabled("project"))
			{
				_btn = (ImgButtonControl)this.LoadPSOFTControl(ImgButtonControl.Path);
				_btn.Caption = _mapper.get("home", "myProjects");
				_btn.ClickArgs.TargetUrl = psoft.Project.ProjectDetail.GetURL("context","person", "xID",UserID);
				_btn.ImageUrl = Global.Config.baseURL + "/images/pikto/meine_projekte.gif";
				_btn.OnImageClick +=new ImgButtonClickEventHandler(ImgButtonClick);
				_imgList.Add(_btn);
			}

			// Contact
			if (Global.isModuleEnabled("contact")) 
			{
				_btn = (ImgButtonControl)this.LoadPSOFTControl(ImgButtonControl.Path);
				_btn.Caption = _mapper.get("home", "myContacts");
				_btn.ClickArgs.TargetUrl = Global.Config.baseURL + "/Contact/ContactGroupDetail.aspx";
				_btn.ImageUrl = Global.Config.baseURL + "/images/pikto/meine_kontakte.gif";
				_btn.OnImageClick +=new ImgButtonClickEventHandler(ImgButtonClick);
				_imgList.Add(_btn);
			}

            // Survey
            if (Global.isModuleEnabled("survey")) {
                _btn = (ImgButtonControl)this.LoadPSOFTControl(ImgButtonControl.Path);
                _btn.Caption = _mapper.get("home", "mySurveys");
                _btn.ClickArgs.TargetUrl = Global.Config.baseURL + "/Survey/SurveyDetail.aspx?context=person";
                _btn.ImageUrl = Global.Config.baseURL + "/images/pikto/meine_umfragen.gif";
                _btn.OnImageClick +=new ImgButtonClickEventHandler(ImgButtonClick);
                _imgList.Add(_btn);
            }

            // Suggestion
            if (Global.isModuleEnabled("suggestion")) 
            {
                _btn = (ImgButtonControl)this.LoadPSOFTControl(ImgButtonControl.Path);
                _btn.Caption = _mapper.get("home", "mySuggestions");
                _btn.ClickArgs.TargetUrl = Global.Config.baseURL + "/Suggestion/SuggestionDetail.aspx?context=person";
                _btn.ImageUrl = Global.Config.baseURL + "/images/pikto/meine_umfragen.gif";
                _btn.OnImageClick +=new ImgButtonClickEventHandler(ImgButtonClick);
                _imgList.Add(_btn);
            }

            SetPageLayoutContentControl(MainContentLayout.LINKS, _imgList);		

            if (Global.Config.ShowNews){
                ch.appl.psoft.Subscription.Controls.ListView list = (ch.appl.psoft.Subscription.Controls.ListView)this.LoadPSOFTControl(ch.appl.psoft.Subscription.Controls.ListView.Path, "_list");
                list.context = _context;
                list.backURL = myURL;
                list.Visible = true;
                list.title = _mapper.get("news",_context);
                list.OrderColumn = (_context == News.CONTEXT.SUBSCRIPTION.ToString() ? "TITLE" : _context+".CREATED");
                list.OrderDir = (_context == News.CONTEXT.SUBSCRIPTION.ToString() ?  "asc" : "desc");
                list.Visible = _context != "";
                SetPageLayoutContentControl(MainContentLayout.LIST, list);	
            }


            DBData db = DBData.getDBData(Session);
            string sql = "select FIRM.TITLE from FIRM,PERSON where PERSON.FIRM_ID=FIRM.ID and PERSON.ID=" + SessionData.getUserID(Session);
			string company = "";

            try
            {
                db.connect();
                DataTable table = db.getDataTable(sql);
                
                if (DBData.getNumberOfRows(table) > 0)
                {
                    company = " - " + SessionData.getDBColumn(Session).GetDisplayValue(table.Columns["TITLE"], DBData.getValue(table, 0, "TITLE"), true);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex,Logger.ERROR);
            }
            finally
            {
                db.disconnect();
            }
        }

        protected string UserID
        {
            get { return SessionData.getUserID(Session).ToString(); }
        }

		private void ImgButtonClick(object sender, ImgButtonClickArgs e)
		{
			if (!e.DisplayUserControl)
				this.Response.Redirect(e.TargetUrl);
			else
			{
				PSOFTUserControl _listControl = this.LoadPSOFTControl(e.UserControlPath);
				SetPageLayoutContentControl(MainContentLayout.LIST, _listControl);	
			}
		}

        private void setLanguage(string newCode)
        {
            string languageCode = "de";
            string regionCode = "ch";

            switch (newCode)
            {
                case "de":
                    languageCode = "de";
                    regionCode = "ch";
                    break;
                case "fr":
                    languageCode = "fr";
                    regionCode = "ch";
                    break;
                case "it":
                    languageCode = "it";
                    regionCode = "ch";
                    break;
                case "en":
                    languageCode = "en";
                    regionCode = "us";
                    break;
            }

            if (languageCode != "" && regionCode != "")
            {
                // reload LanguageMapper
                Global.reloadLanguageMapper(LanguageMapper.getLanguageMapper(Session), languageCode);

                // reload DBColumn
                SessionData.getDBColumn(Session).UserCultureName = languageCode + "-" + regionCode;

                // set properties
                if (Global.Config.authenticationMode != Config.AUTHENTICATION_MODE.ANONYMOUS)
                {
                    ch.appl.psoft.Interface.DBObjects.Person.setLanguageCode(SessionData.getUserID(Session), languageCode);
                    ch.appl.psoft.Interface.DBObjects.Person.setRegionCode(SessionData.getUserID(Session), regionCode);
                }
            }
            Response.Redirect("./FrameWork.aspx?contentFrameURL=" + HttpUtility.UrlEncode(((BreadcrumbItem)Session[BreadcrumbItem.BREAD_CRUMB_ITEM]).LastChild.Link), false);
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
	}
}
