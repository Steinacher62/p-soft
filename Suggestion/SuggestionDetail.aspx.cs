using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Suggestion.Controls;
using ch.psoft.Util;
using System;


namespace ch.appl.psoft.Suggestion
{

    public partial class SuggestionDetail : PsoftDetailPage
	{
        public const string PAGE_URL = "/Suggestion/SuggestionDetail.aspx";

        protected string _context = "person";
        protected long _xID = -1;
		protected long _executionID = -1;
		protected long _suggestionID = -1;
		protected string _orderColumn = "CREATED";
		protected string _orderDir = "desc";


        static SuggestionDetail()
        {
            SetPageParams(PAGE_URL, "suggestionID", "executionID");
        } 

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        #region Protected overridden methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();

            // Setting navigation-menu
            SubNavMenuUrl = "/Suggestion/SubNavMenu.aspx";
        }

		#endregion
        
        protected void Page_Load(object sender, System.EventArgs e)
		{
            _context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"], _context);
            _xID = ch.psoft.Util.Validate.GetValid(Request.QueryString["xID"], _xID);
			_executionID = ch.psoft.Util.Validate.GetValid(Request.QueryString["executionID"], _executionID);
			_orderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], _orderColumn);
			_orderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], _orderDir);
			_suggestionID = ch.psoft.Util.Validate.GetValid(Request.QueryString["ID"],_suggestionID);

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            
            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            DBData db = DBData.getDBData(Session);
            db.connect();
 
            try{
                
                PsoftLinksControl links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                links.LinkGroup1.Caption = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_CMT_SELECTED_SUGGESTION);

                switch (_context){
                    case "all":
                    case "person":
                    case "matrix":
                        if (_xID <= 0){
                            _xID = db.userId;
                        }
                        string SuggestionIdString = db.Suggestion.getActiveSuggestion();
                        if(SuggestionIdString == "") 
                        {
                            //currently no active suggestion found
                            _suggestionID = -1;
                        }
                        else if (_suggestionID <= 0)
                        {
                           _suggestionID = long.Parse(SuggestionIdString);
                        }
                        break;
                    case "searchresult":
                        if (_suggestionID <= 0){
                            _suggestionID =  ch.psoft.Util.Validate.GetValid(db.lookup("row_id", "searchresult", "ID=" + _xID, false), -1);
                        }
                        break;
                }

				if (_suggestionID > 0 && _executionID <= 0) 
				{
					if(Context.Equals("all")) 
					{
						_executionID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SUGGESTION_DETAIL_V", "SUGGESTION_ID=" + _suggestionID + " order by " + _orderColumn + " " + _orderDir, false), -1);
					} 
					else 
					{
						_executionID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SUGGESTION_DETAIL_V", "SUGGESTION_ID=" + _suggestionID + " and PERSON_ID=" + db.userId + " order by " + _orderColumn + " " + _orderDir, false), -1);
					}
				}

                switch (_context)
                {
                    case "matrix":
                    {
                        //matrix control is loaded
                        SuggestionEvaluationMatrixCtrl matrix = (SuggestionEvaluationMatrixCtrl) LoadPSOFTControl(SuggestionEvaluationMatrixCtrl.Path, "_det");
                        matrix.SuggestionID = _suggestionID;
                        matrix.Mapper = _mapper;

                        // Setting content layout user controls
                        SetPageLayoutContentControl(DGLContentLayout.DETAIL, matrix);
                    }
                    break;
                    default:
                    {
                        SuggestionDetailCtrl detail = (SuggestionDetailCtrl) LoadPSOFTControl(SuggestionDetailCtrl.Path, "_det");
                        detail.SuggestionID = _suggestionID;
                        detail.ExecutionID = _executionID;
                        detail.OrderColumn = _orderColumn;
                        detail.OrderDir = _orderDir;
                        detail.Kontext = _context=="person"?ExecutionList.CONTEXT_SUGGESTION:ExecutionList.CONTEXT_ALLRESULT;
                        detail.Links = links;
                        detail.Mapper = _mapper;

                        if(_suggestionID > 0) 
                        {
                            //TODO: Keine Liste anzeigen, wenn kein Vorschlagswesen aktiv ist bei normalen Rechten oder noch kein Vorschlagswesen gemacht wurde
                            ExecutionList executionList = (ExecutionList) LoadPSOFTControl(ExecutionList.Path, "_list");
                            executionList.xID = _suggestionID;
                            executionList.Kontext = _context=="person"?ExecutionList.CONTEXT_SUGGESTION:ExecutionList.CONTEXT_ALLRESULT;
                            executionList.OrderColumn = _orderColumn;
                            executionList.OrderDir = _orderDir;
                            executionList.SortURL = Global.Config.baseURL + "/Suggestion/SuggestionDetail.aspx?context=" + _context;
                            executionList.DetailURL = Global.Config.baseURL + "/Suggestion/SuggestionDetail.aspx?suggestionID=" + _suggestionID + "&executionID=%ID&context=" + _context + "&orderDir=" + _orderDir + "&orderColumn=" + _orderColumn;
                            //executionList.DetailURL = Global.Config.baseURL + "/Suggestion/Execute.aspx?executionID=%ID&suggestionID=" + _suggestionID;
                            executionList.DetailEnabled = true;
                            executionList.PostDeleteURL = Request.RawUrl;
                            executionList.ExecutionID = _executionID;
                            SetPageLayoutContentControl(DGLContentLayout.GROUP, executionList);
                        }//if

                        // Setting content layout user controls
                        SetPageLayoutContentControl(DGLContentLayout.DETAIL, detail);

                        //links setting
                        bool isExecutable = db.Suggestion.isExecutable(_suggestionID);
                        if (isExecutable)
                        {
                            links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_CM_EXECUTE_SUGGESTION), "/Suggestion/Execute.aspx?suggestionID=" + _suggestionID + "&context=" + _context);
                        }
                        SetPageLayoutContentControl(DGLContentLayout.LINKS, links);
                    }
                    break;
                }


                BreadcrumbCaption = PsoftPageLayout.PageTitle = db.lookup("TITLE", "SUGGESTION", "ID=" + _suggestionID, false);
            }
            catch(Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally{
                db.disconnect();
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

	}
}
