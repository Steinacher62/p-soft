using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Suggestion
{
    public partial class SubNavMenu : PsoftMenuPage
	{
        //never used, thus commented
        //MenuControl ctrl = null; 

        protected void Page_Load(object sender, System.EventArgs e)
        {
            DBData db = DBData.getDBData(Session);

            long suggestionID = long.Parse(db.Suggestion.getActiveSuggestion());

            // Setting main page layout
            MenuPageLayout pageLayout = (MenuPageLayout) LoadPSOFTControl(MenuPageLayout.Path, "_pl");;
            PageLayoutControl = pageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout) LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting detail parameters
            MenuControl ctrl = (MenuControl) LoadPSOFTControl(MenuControl.Path, "_ctrl");
            ctrl.Title = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_SUBNAV_TITLE);
            ctrl.TitleLink = Global.Config.baseURL + "/Suggestion/SuggestionDetail.aspx?context=person";

		    ctrl.addMenuItem(null, "ownSuggestionExecutions", _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_SUBNAV_OWNSUGGESTIONEXECUTIONS), Global.Config.baseURL + "/Suggestion/SuggestionDetail.aspx?context=person");
            if(showResponsibleMenuItem()) 
            {
                ctrl.addMenuItem(null, "allSuggestionExecutions", _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_SUBNAV_ALLSUGGESTIONEXECUTIONS), Global.Config.baseURL + "/Suggestion/SuggestionDetail.aspx?context=all");
            }
            //Übersicht immer sichtbar
            ctrl.addMenuItem(null, "allSuggestionExecutionsMatrix", _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_SUBNAV_ALLSUGGESTIONEXECUTIONSMATRIX), Global.Config.baseURL + "/Suggestion/SuggestionDetail.aspx?context=matrix&suggestionID=" + suggestionID);
        
//            if (SuggestionModule.showSearchSuggestionMenu){
//                ctrl.addMenuItem(null, "searchSuggestion", _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_SUBNAV_SEARCHSUGGESTION), Global.Config.baseURL + "/Suggestion/SuggestionSearch.aspx");
//            }

            //Setting content layout user controls
            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, ctrl);
        }


        public bool showResponsibleMenuItem() 
        {
            DBData db = DBData.getDBData(Session);
            bool isRowRights = false;
            bool isModuleRights = false;
            bool isAdministrable = false;
            try 
            {
                db.connect();
                //only one suggestion is active at a time
                
                long suggestionId = DBColumn.GetValid(db.lookup("ID", "SUGGESTION","ISACTIVE=1"), -1L);
                isRowRights = db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "SUGGESTION", suggestionId, true, true);
                isModuleRights = db.hasApplicationAuthorisation(DBData.AUTHORISATION.ADMIN, DBData.APPLICATION_RIGHT.MODULE_SUGGESTION, true);
                isAdministrable = (isRowRights || isModuleRights);
                
            }
            catch (Exception ex) 
            {
                //error
                string str = ex.Message;
            }
            finally 
            {
                db.disconnect();
            }
            return isAdministrable;
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
