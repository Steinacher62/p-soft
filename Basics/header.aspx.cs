using System;
using System.Web;
using System.Web.UI.WebControls;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Common;

namespace ch.appl.psoft.Basics
{
    using Interface.DBObjects;

    /// <summary>
    /// Summary description for header.
    /// </summary>
    public partial class header : System.Web.UI.Page
	{


        protected string _backgroundImage = "";
        protected bool _isLogged = false;
        private const string _defaultStyle = "cursor:default;";
        protected string _elementsScript = "";

        protected void Page_Load(object sender, System.EventArgs e)
		{
            _isLogged = Request.IsAuthenticated && !bool.Parse(ch.psoft.Util.Validate.GetValid(Request.QueryString["logout"], "false"));

            if (_isLogged)
            {
                LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
                TableCell cell;

                spacerCell.Style.Add("padding","0px 161px 0px 0px");
                headerTable.Style.Add("border-top","solid 1px #B0B9D6");
                headerTable.Style.Add("border-bottom","solid 1px #B0B9D6");

                foreach (psoftModule module in Global.s_ModulesDict.Values)
                {
                    if (module.IsVisible(Session))
                    {
                        cell = new TableCell();
                        cell.ID = module.Name + "Cell";
                        cell.CssClass = "header";
                        cell.VerticalAlign = VerticalAlign.Middle;
                        _elementsScript += "elementIDs[elementIDs.length] = '" + cell.ID + "';\r\n";

                        HyperLink link = new HyperLink();
                        link.ID = module.Name + "Link";
                        link.Text = module.GetLocalName(map);
                        if (_isLogged)
                            link.Attributes.Add("onClick", "HighlightModule('" + module.Name + "');");
                        _elementsScript += "elementIDs[elementIDs.length] = '" + link.ID + "';\r\n";

                        if (_isLogged)
                        {
                            link.CssClass = "header";

                            switch (module.Name)
                            {
                                case "knowledge":
                                    link.Target = "subNavFrame";
                                    link.NavigateUrl = Global.Config.baseURL + "/Basics/leftMenu.aspx?module=knowledge";
                                    break;
                                case "organisation":
                                    link.Target = "subNavFrame";
                                    link.NavigateUrl = Global.Config.baseURL + "/Basics/leftMenu.aspx?module=organisation";
                                    break;
                                //case "rsr":
                                    // don't show menu entry, module enabled for new reports, but does not need to be visible
                                    //break;
                                default:
                                    if (module.StartURL != ""){
                                    link.Target = "contentFrame";
                                    link.NavigateUrl = module.StartURL;
                                    }
                                    else{
                                        link.Target = "subNavFrame";
                                        link.NavigateUrl = "javascript: top.reloadSubNavigation('" + module.SubNavMenu + "');";
                                    }
                                    break;
                             }
                            
                        }
                        else
                            link.CssClass = "header_inactive";
                        
                        cell.Controls.Add(link);
                        headerRow.Cells.Add(cell);
                    }
                }

                cell = new TableCell();
                cell.CssClass = "header";
                cell.VerticalAlign = VerticalAlign.Middle;
                headerRow.Cells.Add(cell);

                if (Global.Config.showLanguageSelector){
                    langGerman.CssClass = langFrench.CssClass = langItalian.CssClass = langEnglish.CssClass = "header_language";

                    LinkButton lb = null;

                    switch(LanguageMapper.getLanguageMapper(Session).LanguageCode) {
                        case "de":
                            lb = langGerman;
                            break;
                        case "fr":
                            lb = langFrench;
                            break;
                        case "it":
                            lb = langItalian;
                            break;
                        case "en":
                            lb = langEnglish;
                            break;
                    }

                    if (lb != null) {
                        lb.CssClass = "header_language_selected";
                        lb.Enabled = false;
                        lb.Attributes.Add("style", _defaultStyle);
                    }

					string availableLanguages = Global.Config.availableLanguages;
					if(availableLanguages.IndexOf("de") < 0) 
					{
						languageSelector.Controls.Remove(langGerman);
					}
					if(availableLanguages.IndexOf("en") < 0) 
					{
						languageSelector.Controls.Remove(langEnglish);
					}
					if(availableLanguages.IndexOf("fr") < 0) 
					{
						languageSelector.Controls.Remove(langFrench);
					}
					if(availableLanguages.IndexOf("it") < 0) 
					{
						languageSelector.Controls.Remove(langItalian);
					}

                }
                else{
                    languageSelector.Visible = false;
                }

                // Logout-Button
                logoutLink.ID = "logoutLink";
                logoutLink.Text = map.get("navigation", "logout");
                logoutLink.CssClass = "header";
                logoutLink.Target = "_top";
                logoutLink.NavigateUrl = Global.Config.baseURL + "/Basics/default.aspx?logout=true";

                _backgroundImage = "../images/header_top.jpg";
            }
            else
            {
                headerDIV.Visible = false;
                _backgroundImage = "../images/header_infospeed_with_line.jpg";
            }
		}

        public string LoginInfo
        {
            get
            {
                return ((Global.Config.authenticationMode != Config.AUTHENTICATION_MODE.ANONYMOUS) && _isLogged)? Session["UserInitials"].ToString() : "";
            }
        }

        protected string Logo{
            get{
                if (Global.Config.headerLogo != ""){
                    return Global.Config.baseURL + Global.Config.headerLogo;
                }
                return "../images/transparent.gif";
            }
        }

        protected string LogoURL{
            get { return Global.Config.headerLogoURL; }
        }

        private void MapButtonMethods()
        {
            langGerman.Click += new System.EventHandler(langGerman_Click);
            langEnglish.Click += new System.EventHandler(langEnglish_Click);
            langFrench.Click += new System.EventHandler(langFrench_Click);
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
            MapButtonMethods();
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

        private void setLanguage(string newCode)
        {
            string languageCode = "de";
            string regionCode = "ch";

            switch(newCode)
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
                SessionData.getDBColumn(Session).UserCultureName = languageCode+"-"+regionCode;

                // set properties
                if (Global.Config.authenticationMode != Config.AUTHENTICATION_MODE.ANONYMOUS){
                    Person.setLanguageCode(SessionData.getUserID(Session), languageCode);
                    Person.setRegionCode(SessionData.getUserID(Session), regionCode);
                }
            }
            Response.Redirect("../FrameWork.aspx?contentFrameURL=" + HttpUtility.UrlEncode(((BreadcrumbItem)Session[BreadcrumbItem.BREAD_CRUMB_ITEM]).LastChild.Link),false);
        }

        private void langGerman_Click(object sender, EventArgs e)
        {
            if (LanguageMapper.getLanguageMapper(Session).LanguageCode != "de")
                setLanguage("de");
        }

        private void langEnglish_Click(object sender, EventArgs e)
        {
            if (LanguageMapper.getLanguageMapper(Session).LanguageCode != "en")
                setLanguage("en");
        }

        private void langFrench_Click(object sender, EventArgs e)
        {
            if (LanguageMapper.getLanguageMapper(Session).LanguageCode != "fr")
                setLanguage("fr");
        }
    }
}
