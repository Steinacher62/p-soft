using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;



namespace ch.appl.psoft
{
    public partial class Framework : System.Web.UI.MasterPage
    {
        protected string _backgroundImage = "";
        protected string _banner_backgroundImage = "";
        protected bool _isLogged = false;
        protected string _vDir = "";
        private const string _defaultStyle = "cursor:default;";
        protected string _elementsScript = "";

        protected string _contentFrameURL = psoft.Basics.contentInit.GetURL();
        protected string _noFramesURL = "";

        protected int timeout;
        protected int cssCharactersize;
        protected void Page_Load(object sender, EventArgs e)
        {
            // load vDir and adjust paths
            DBData db = DBData.getDBData(Session);
            if (db.userId > -1)
                cssCharactersize = (int)db.lookup("CHARACTER_SIZE", "PERSON", "ID = " + db.userId);
            _vDir = Global.Config.baseURL;
            cssLink.Href = _vDir + "/Style/Psoft.css";
            if (cssCharactersize > 0)
                cssFontHeight.Href = _vDir + "/Style/PsoftFontsize" + cssCharactersize + ".css";
            cssLayoutLink.Href = _vDir + "/Style/layout.css";
            //lblJS.Text = "<script language=\"JavaScript\" src='%vDir%/JavaScript/PsoftService.js' type='text/javascript'></script>"
            lblJS.Text = "<script language=\"JavaScript\" type='text/javascript' src='%vDir%/JavaScript/Common.js'></script>"
            + "<script language=\"JavaScript\" type='text/javascript' src='%vDir%/JavaScript/SortTable.js'></script>"
            + "<script language=\"JavaScript\" type='text/javascript' src='%vDir%/JavaScript/PopupWindow.js'></script>"
            + "<script language=\"JavaScript\" type='text/javascript' src='%vDir%/JavaScript/PsoftService.js'></script>"
            + "<script language=\"JavaScript\" type='text/javascript' src='%vDir%/Scripts/jquery-3.3.1.min.js'></script>"
            + "<script language=\"JavaScript\" type='text/javascript' src='%vDir%/JavaScript/jquery.json-2.2.min.js'></script>"
            + "<script language=\"JavaScript\" type='text/javascript' src='%vDir%/JavaScript/PropertyBox.js'></script>"
            + "<script language=\"JavaScript\" type='text/javascript'>webServicePath = \"" + Global.Config.baseURL + "/\";</script>";
            //+ "<div id=\"service\" style=\"BEHAVIOR: url(%vDir%/JavaScript/webservice.htc)\"></div>";
            lblJS.Text = lblJS.Text.Replace("%vDir%", _vDir);

            _contentFrameURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["contentFrameURL"], _contentFrameURL);
            _noFramesURL = PSOFTConvert.ToJavascript(ch.psoft.Util.Validate.GetValid(Request.QueryString["noFramesURL"], _noFramesURL));


            _isLogged = Request.IsAuthenticated && !bool.Parse(ch.psoft.Util.Validate.GetValid(Request.QueryString["logout"], "false"));
            LanguageMapper map = new LanguageMapper();
            if (_isLogged)
            {
                map = LanguageMapper.getLanguageMapper(Session);
                TableCell cell;

                if (Global.isModuleEnabled("payment"))
                {
                    orderLink.ID = "orderLink";
                    orderLink.Text = map.get("navigation", "orderNow");
                    orderLink.CssClass = "header";
                    orderLink.NavigateUrl = Global.Config.baseURL + "/Payment/Order.aspx";
                }


                if (cssCharactersize < 4 && Global.Config.SelectFontSize)
                {
                    ImageButtonMore.Visible = true;
                    ImageButtonMore.ImageUrl = Global.Config.baseURL + "/images/format_font_size_more.png";
                    ImageButtonMore.Attributes.Add("onmouseover", "javascript:this.src='" + Global.Config.baseURL + "/images/format_font_size_More_Hover.png'");
                    ImageButtonMore.Attributes.Add("onmouseout", "javascript:this.src=' " + Global.Config.baseURL + "/images/format_font_size_More.png'");
                    ImageButtonMore.Command += new CommandEventHandler(ImageButtonMoreClick);
                }
                else
                    ImageButtonMore.Visible = false;

                if (cssCharactersize > 0 && Global.Config.SelectFontSize)
                {
                    ImageButtonLess.Visible = true;
                    ImageButtonLess.ImageUrl = Global.Config.baseURL + "/images/format_font_size_less.png";
                    ImageButtonLess.Attributes.Add("onmouseover", "javascript:this.src='" + Global.Config.baseURL + "/images/format_font_size_less_Hover.png'");
                    ImageButtonLess.Attributes.Add("onmouseout", "javascript:this.src=' " + Global.Config.baseURL + "/images/format_font_size_less.png'");
                    ImageButtonLess.Command += new CommandEventHandler(ImageButtonLessClick);
                }
                else
                    ImageButtonLess.Visible = false;

                //add confirm message
                lblJS.Text += "<script language=\"JavaScript\" type='text/javascript'>deleteConfirmMessage = \"" + PSOFTConvert.ToJavascript(map.get("MESSAGES", "deleteConfirm")) + "\";</script>";

                //spacerCell.Style.Add("padding", "0px 161px 0px 0px");
                headerTable.Style.Add("border-top", "solid 1px #B0B9D6");
                headerTable.Style.Add("border-bottom", "solid 1px #B0B9D6");

                foreach (psoftModule module in Global.s_ModulesDict.Values)
                {
                    if (module.IsVisible(Session))
                    {
                        //if Shop achtive do not add knowledege menu
                        if (!(module.Name.Equals("knowledge") && (Global.Config.shopActive || Global.Config.getModuleParam("morph", "permissionMultiuser", "0") == "1")))
                        {
                            //if MultiUser achtive do not add administration menu
                            if (!(module.Name.Equals("organisation") && Global.Config.getModuleParam("morph", "permissionMultiuser", "0") == "1"))
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
                                            if (module.StartURL != "")
                                            {
                                                link.Target = "contentFrame";
                                                link.NavigateUrl = module.StartURL;
                                            }
                                            else
                                            {

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
                    }
                }

                cell = new TableCell();
                cell.CssClass = "header";
                cell.VerticalAlign = VerticalAlign.Middle;
                headerRow.Cells.Add(cell);

                if (Global.Config.showLanguageSelector)
                {
                    langGerman.CssClass = langFrench.CssClass = langItalian.CssClass = langEnglish.CssClass = "header_language";

                    LinkButton lb = null;

                    switch (LanguageMapper.getLanguageMapper(Session).LanguageCode)
                    {
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

                    if (lb != null)
                    {
                        lb.CssClass = "header_language_selected";
                        lb.Enabled = false;
                        lb.Attributes.Add("style", _defaultStyle);
                    }

                    string availableLanguages = Global.Config.availableLanguages;
                    if (availableLanguages.IndexOf("de") < 0)
                    {
                        languageSelector.Controls.Remove(langGerman);
                    }
                    if (availableLanguages.IndexOf("en") < 0)
                    {
                        languageSelector.Controls.Remove(langEnglish);
                    }
                    if (availableLanguages.IndexOf("fr") < 0)
                    {
                        languageSelector.Controls.Remove(langFrench);
                    }
                    if (availableLanguages.IndexOf("it") < 0)
                    {
                        languageSelector.Controls.Remove(langItalian);
                    }

                }
                else
                {
                    languageSelector.Visible = false;
                }

                // Logout-Button
                logoutLink.ID = "logoutLink";
                logoutLink.Text = map.get("navigation", "logout");
                logoutLink.CssClass = "header";
                logoutLink.Target = "_top";
                logoutLink.NavigateUrl = Global.Config.baseURL + "/Basics/default.aspx?logout=true";

                // Helpfile
                if (Global.Config.ShowHelpFile)
                {
                    HelpFileLink.ID = "helpFileLink";
                    HelpFileLink.Text = map.get("navigation", "help");
                    HelpFileLink.CssClass = "header";
                    HelpFileLink.Target = "_blank";
                    HelpFileLink.NavigateUrl = Global.Config.baseURL + "/helpfile.pdf";
                }

                _backgroundImage = Global.Config.baseURL + "/images/Kundenspezifisch/" + Global.Config.ApplicationName + "/header_top.png";
                _banner_backgroundImage = Global.Config.baseURL + "/images/Kundenspezifisch/" + Global.Config.ApplicationName + "/header_top_background.jpg";

            }
            else
            {
                // should not be reached, different masterpage for login / 26.01.10 / mkr
                _backgroundImage = Global.Config.baseURL + "/images/header_infospeed_with_line.jpg";
            }

            //load menu on every request (otherwise the menu isn't shown after click on button on PsoftPage) / 26.10.09
            if (!Page.IsPostBack)
            {
                LanguageMapper _mapper = LanguageMapper.getLanguageMapper(this.Session);
                //bool _isLogged = Request.IsAuthenticated && !bool.Parse(ch.psoft.Util.Validate.GetValid(Request.QueryString["logout"], "false"));

                //load treeMenu for user

                //home
                RadMenuItem home;
                if (!Global.isModuleEnabled("energiedienst"))
                {
                    if (!Global.isModuleEnabled("SBS"))
                    {
                        if (Global.Config.startUpURL.Length > 0)
                        {
                            if (Global.Config.startUpURL != "myPage")
                            {
                                home = new RadMenuItem("Home", Global.Config.baseURL + Global.Config.startUpURL.ToString());
                            }
                            else
                            {
                                home = new RadMenuItem("Home", Global.Config.baseURL + "/Person/DetailFrame.aspx?ID=" + db.userId + "&mode=oe");
                            }
                        }
                        else
                        {
                            home = new RadMenuItem("Home", Global.Config.baseURL + "/default.html");
                        }
                    }
                    else
                    {
                        home = new RadMenuItem("Home", Global.Config.baseURL + "/sbs/sbsstartpage.aspx");
                    }
                }
                else
                {
                    home = new RadMenuItem("Home", Global.Config.baseURL + "/Basics/contentInit.aspx");
                }
                home.CssClass = "rmHome";
                leftMenu.Items.Add(home);

                //Administration
                if (Global.Config.ShowAdmin)
                {
                    if ((int)db.getApplicationAuthorisations(1, true) > 0)
                    {
                        RadMenuItem admin = new RadMenuItem(map.get("application", "administration"));
                        leftMenu.Items.Add(admin);

                        RadMenuItem organisation = new RadMenuItem(map.get("application", "Organisation"), Global.Config.baseURL + "/admin/Organisation.aspx");
                        admin.Items.Add(organisation);

                        if ((int)db.getApplicationAuthorisations(1, true) >= 32)
                        {
                            RadMenuItem permissions = new RadMenuItem(map.get("application", "Permission"));
                            RadMenuItem parentItem = leftMenu.Items.FindItemByText(map.get("application", "administration"));
                            parentItem.Items.Add(permissions);
                            RadMenuItem applicationPermissions = new RadMenuItem(map.get("application", "ApplicationPermissions"), Global.Config.baseURL + "/Admin/Authorisation/ApplicationPermissions.aspx");
                            permissions.Items.Add(applicationPermissions);
                            RadMenuItem accessorGroups = new RadMenuItem(map.get("application", "AccessorGroups"), Global.Config.baseURL + "/Admin/Authorisation/UserGroups.aspx");
                            permissions.Items.Add(accessorGroups);
                        }

                        if (Global.isModuleEnabled("training") && (int)db.getApplicationAuthorisations(40, true) > 1)
                        {
                            RadMenuItem training = new RadMenuItem(map.get("application", "training"));
                            RadMenuItem parentItem = leftMenu.Items.FindItemByText(map.get("application", "administration"));
                            parentItem.Items.Add(training);
                            RadMenuItem trainingCatalog = new RadMenuItem(map.get("application", "trainingCatalog"), Global.Config.baseURL + "/admin/Training/TrainingCatalog.aspx");
                            training.Items.Add(trainingCatalog);
                            RadMenuItem trainingDemand = new RadMenuItem(map.get("application", "trainingDemand"), Global.Config.baseURL + "/admin/Training/TrainingDemand.aspx");
                            training.Items.Add(trainingDemand);
                        }

                        RadMenuItem function = new RadMenuItem(map.get("application", "Function"));
                        admin.Items.Add(function);
                        RadMenuItem functionCatalog = new RadMenuItem(map.get("function", "functionCatalog"), Global.Config.baseURL + "/admin/Functions/FunctionCatalog.aspx");
                        function.Items.Add(functionCatalog);

                        if (Global.isModuleEnabled("administration"))
                        {
                            RadMenuItem organigramm = new RadMenuItem(map.get("application", "OrgCharts"));
                            RadMenuItem parentItem = leftMenu.Items.FindItemByText(map.get("application", "administration"));
                            parentItem.Items.Add(organigramm);
                            RadMenuItem ortgChart = new RadMenuItem(map.get("application", "OrgChart"), Global.Config.baseURL + "/admin/Chart/Chart.aspx");
                            organigramm.Items.Add(ortgChart);
                            RadMenuItem orgentityLayout = new RadMenuItem(map.get("application", "OrgentityLayout"), Global.Config.baseURL + "/admin/Chart/OrgentityLayout.aspx");
                            organigramm.Items.Add(orgentityLayout);
                            RadMenuItem textLayout = new RadMenuItem(map.get("application", "TextLayout"), Global.Config.baseURL + "/admin/Chart/TextLayout.aspx");
                            organigramm.Items.Add(textLayout);
                        }

                        if (Global.isModuleEnabled("fbs") && (int)db.getApplicationAuthorisations(20, true) > 1)
                        {
                            RadMenuItem functionDescriptionFolder = new RadMenuItem(map.get("functionDescription", "name"));
                            function.Items.Add(functionDescriptionFolder);
                            RadMenuItem functionDescription = new RadMenuItem(map.get("functionDescription", "name"), Global.Config.baseURL + "/admin/Functions/FunctionDescription.aspx");
                            functionDescriptionFolder.Items.Add(functionDescription);
                            RadMenuItem dutyCatalog = new RadMenuItem(map.get("functionDescription", "dutyCatalog"), Global.Config.baseURL + "/admin/Functions/DutyCatalog.aspx");
                            functionDescriptionFolder.Items.Add(dutyCatalog);
                            RadMenuItem competenceLevel = new RadMenuItem(map.get("functionDescription", "competenceLevel"), Global.Config.baseURL + "/admin/Functions/CompetenceLevel.aspx");
                            functionDescriptionFolder.Items.Add(competenceLevel);
                        }
                        if (Global.isModuleEnabled("fbw") && (int)db.getApplicationAuthorisations(50, true) > 1)
                        {
                            RadMenuItem functionRatingFolder = new RadMenuItem(map.get("functionRating", "name"));
                            function.Items.Add(functionRatingFolder);
                            RadMenuItem functionRating = new RadMenuItem(map.get("functionRating", "name"), Global.Config.baseURL + "/admin/Functions/FunctionRating.aspx");
                            functionRatingFolder.Items.Add(functionRating);
                            RadMenuItem functionRatingCatalog = new RadMenuItem(map.get("functionRating", "catalogOptions"), Global.Config.baseURL + "/admin/Functions/FunctionRatingCatalogOptions.aspx");
                            functionRatingFolder.Items.Add(functionRatingCatalog);
                            RadMenuItem functionRatingFunctionTyps = new RadMenuItem(map.get("functionRating", "functionTyps"), Global.Config.baseURL + "/admin/Functions/FunctionRatingFunctionTyps.aspx");
                            functionRatingFolder.Items.Add(functionRatingFunctionTyps);
                        }
                        if (Global.isModuleEnabled("performance") && (int)db.getApplicationAuthorisations(10, true) > 1)
                        {
                            RadMenuItem performanceratingFolder = new RadMenuItem(map.get("performanceRating", "name"));
                            RadMenuItem parentItem = leftMenu.Items.FindItemByText(map.get("application", "administration"));
                            parentItem.Items.Add(performanceratingFolder);
                            RadMenuItem lockDate = new RadMenuItem(map.get("performanceRating", "lockdate"), Global.Config.baseURL + "/Admin/Performancerating/Lockdate.aspx");
                            performanceratingFolder.Items.Add(lockDate);
                            RadMenuItem defaultJobExpectation = new RadMenuItem(map.get("performanceRating", "defaultJobExpectation"), Global.Config.baseURL + "/Admin/Performancerating/DefaultJobExpectation.aspx");
                            performanceratingFolder.Items.Add(defaultJobExpectation);
                            RadMenuItem ratingCriterias = new RadMenuItem(map.get("performanceRating", "ratingCriterias"), Global.Config.baseURL + "/Admin/Performancerating/RatingCriterias.aspx");
                            performanceratingFolder.Items.Add(ratingCriterias);
                            RadMenuItem ratingLevels = new RadMenuItem(map.get("performanceRating", "ratingLevels"), Global.Config.baseURL + "/Admin/Performancerating/RatingLevel.aspx");
                            performanceratingFolder.Items.Add(ratingLevels);
                            RadMenuItem functionCriterias = new RadMenuItem(map.get("performanceRating", "functionCriterias"), Global.Config.baseURL + "/Admin/Performancerating/FunctionCriterias.aspx");
                            performanceratingFolder.Items.Add(functionCriterias);
                        }
                        if (Global.isModuleEnabled("skills") && (int)db.getApplicationAuthorisations(30, true) > 1)
                        {
                            RadMenuItem skillsManagementFolder = new RadMenuItem(map.get("skills", "skillsManagement"));
                            admin.Items.Add(skillsManagementFolder);
                            RadMenuItem functionSkills = new RadMenuItem(map.get("skills", "functionSkills"), Global.Config.baseURL + "/admin/Skills/FunctionSkills.aspx");
                            skillsManagementFolder.Items.Add(functionSkills);
                            RadMenuItem skillsCatalog = new RadMenuItem(map.get("skills", "skillsCatalog"), Global.Config.baseURL + "/admin/Skills/SkillsCatalog.aspx");
                            skillsManagementFolder.Items.Add(skillsCatalog);
                            RadMenuItem competenceLevel = new RadMenuItem(map.get("skills", "competenceLevel"), Global.Config.baseURL + "/admin/Skills/CompetenceLevel.aspx");
                            skillsManagementFolder.Items.Add(competenceLevel);
                            RadMenuItem ratingLevels = new RadMenuItem(map.get("skills", "ratingLevels"), Global.Config.baseURL + "/Admin/Skills/RatingLevel.aspx");
                            skillsManagementFolder.Items.Add(ratingLevels);
                        }
                        if (Global.isModuleEnabled("Lohn") && (int)db.getApplicationAuthorisations(110, true) > 1)
                        {
                            RadMenuItem wage = new RadMenuItem(map.get("application", "wage"));
                            RadMenuItem parentItem = leftMenu.Items.FindItemByText(map.get("application", "administration"));
                            parentItem.Items.Add(wage);
                            RadMenuItem wageVariant = new RadMenuItem(map.get("application", "wageVariant"), Global.Config.baseURL + "/admin/Wage/WageVariant.aspx");
                            wage.Items.Add(wageVariant);
                            RadMenuItem wageList = new RadMenuItem(map.get("application", "wageList"), Global.Config.baseURL + "/admin/Wage/Wagelist.aspx");
                            wage.Items.Add(wageList);
                            RadMenuItem wageCorrection = new RadMenuItem(map.get("application", "wageCorrection"), Global.Config.baseURL + "/admin/Wage/WageCorrection.aspx");
                            wage.Items.Add(wageCorrection);
                        }

                        RadMenuItem menus = new RadMenuItem(map.get("application", "Menus"));
                        admin.Items.Add(menus);
                        RadMenuItem orgMenu = new RadMenuItem(map.get("application", "OrgMenu"), Global.Config.baseURL + "/admin/Menus/OrganisationMenu.aspx");
                        menus.Items.Add(orgMenu);
                        RadMenuItem knowledgeMenu = new RadMenuItem(map.get("application", "KnowledgeMenu"), Global.Config.baseURL + "/admin/Menus/KnowledgeMenu.aspx");
                        menus.Items.Add(knowledgeMenu);

                        if (Global.isModuleEnabled("mbo") && (int)db.getApplicationAuthorisations(60, true) > 1)
                        {
                            RadMenuItem mbo = new RadMenuItem(map.get("application", "mbo"));
                            RadMenuItem parentItem = leftMenu.Items.FindItemByText(map.get("application", "administration"));
                            parentItem.Items.Add(mbo);
                            RadMenuItem objectiveRound = new RadMenuItem(map.get("application", "objectiveRound"), Global.Config.baseURL + "/admin/Mbo/ObjectiveRound.aspx");
                            mbo.Items.Add(objectiveRound);
                            RadMenuItem mboAdministration = new RadMenuItem(map.get("application", "mboAdministration"), Global.Config.baseURL + "/admin/Mbo/MboAdministration.aspx");
                            mbo.Items.Add(mboAdministration);
                        }

                        RadMenuItem helpMenu = new RadMenuItem(map.get("application", "HelpMenu"), Global.Config.baseURL + "/admin/Help/Index.htm");
                        helpMenu.Target = "_blank";
                        admin.Items.Add(helpMenu);

                    }

                }


                //organisation
                if (Request.QueryString["module"] != "knowledge")
                {
                    if (!Global.Config.shopActive && Global.Config.getModuleParam("morph", "permissionMultiuser", "0") == "0")
                    {
                        loadFromDB("ORGANISATION");
                    }
                    if (Global.isModuleEnabled("SBS"))
                    {
                        DataTable userSeminars = new DataTable();
                        userSeminars = db.getDataTable("SELECT SBS_SEMINARS.ID, SBS_SEMINARS.NAME "
                                                    + "FROM   SBS_USER_SEMINARS INNER JOIN SBS_SEMINARS ON SBS_USER_SEMINARS.SEMINAR_REF = SBS_SEMINARS.ID "
                                                    + "WHERE  SBS_USER_SEMINARS.USER_REF = " + db.userId + " ORDER BY ID DESC");
                        if (userSeminars.Rows.Count > 1)
                        {
                            RadMenuItem seminars = new RadMenuItem("Seminare");
                            RadMenuItem parentItem;
                            if (leftMenu.Items.FindItemByText("Contenu") == null)
                            {
                                parentItem = leftMenu.Items.FindItemByText("Inhalt");
                            }
                            else
                            {
                                parentItem = leftMenu.Items.FindItemByText("Contenu");
                            }
                            foreach (DataRow seminarRow in userSeminars.Rows)
                            {
                                RadMenuItem seminar = new RadMenuItem(seminarRow[1].ToString(), Global.Config.baseURL + "/sbs/sbsstartpage.aspx?seminarID=" + seminarRow[0].ToString());
                                seminars.Items.Add(seminar);
                            }

                            parentItem.Items.Add(seminars);
                        }

                    }
                }

                //modules
                foreach (psoftModule module in Global.s_ModulesDict.Values)
                {
                    if (module.IsVisible(Session) && _isLogged && module.Name != "organisation" && module.Name != "undefined")
                    {
                        RadMenuItem moduleItem;
                        if (!(module.Name.Equals("knowledge") && Global.Config.shopActive) && Global.Config.getModuleParam("morph", "permissionMultiuser", "0") == "0")
                        {
                            if ((module.StartURL != "" || Global.isModuleEnabled("energiedienst")) && (!Global.isModuleEnabled("SBS") || LoginInfo == "admin"))
                            {
                                string displayname = getDisplayName(module.GetLocalName(_mapper));

                                moduleItem = new RadMenuItem(displayname, module.StartURL);
                                leftMenu.Items.Add(moduleItem);

                                switch (module.GetLocalName(_mapper))
                                {

                                    case "MORPHOLOGIE":

                                        moduleItem.Text = "Karten";
                                        moduleItem.NavigateUrl = "";
                                        RadMenuItem AddMap = new RadMenuItem("Neu", "Morph/AddNewMap.aspx?template=0");
                                        moduleItem.Items.Add(AddMap);
                                        RadMenuItem SearchMap = new RadMenuItem("Suche", module.StartURL);
                                        moduleItem.Items.Add(SearchMap);
                                        if (Global.isModuleEnabled("gfk"))
                                        {
                                            long groupAccessorId = DBColumn.GetValid(db.lookup("ID", "ACCESSOR", "TITLE = 'GFKAdministratoren'"), (long)-1);
                                            if (db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true))
                                            {
                                                RadMenuItem AddMapTemplate = new RadMenuItem("Neues GFK Template", "Morph/AddNewMap.aspx?template=gfk");
                                                moduleItem.Items.Add(AddMapTemplate);
                                            }
                                        }
                                        if (Global.isModuleEnabled("novis"))
                                        {
                                            RadMenuItem AddMapTemplate = new RadMenuItem("Neues Template", "Morph/AddNewMap.aspx?template=novis");
                                            moduleItem.Items.RemoveAt(0);
                                            moduleItem.Items.Add(AddMapTemplate);
                                            RadMenuItem CreateReport = new RadMenuItem("Report erstellen", "Novis/CreateNovisReport.aspx");
                                            moduleItem.Items.Add(CreateReport);
                                        }
                                        if (Global.isModuleEnabled("SBS"))
                                        {
                                            RadMenuItem AddMapTemplate = new RadMenuItem("Neues Template", "Morph/AddNewMap.aspx?template=sbs");
                                            moduleItem.Items.RemoveAt(0);
                                            moduleItem.Items.Add(AddMapTemplate);
                                        }

                                        break;

                                    case "WISSEN":
                                        // Query-String Check disabled
                                        if ((true || Request.QueryString["module"] == "knowledge") && Global.Config.getModuleParam("morph", "permissionMultiuser", "0") == "0")

                                        {
                                            DBData db_knowledge = DBData.getDBData(Session);
                                            db_knowledge.connect();
                                            string baseID = db_knowledge.lookup("ID", "MENUGROUP", "MNEMO = 'KNOWLEDGE'", false);
                                            RadMenuItem kItem1 = new RadMenuItem(_mapper.get(ch.appl.psoft.Knowledge.KnowledgeModule.LANG_SCOPE_KNOWLEDGE, ch.appl.psoft.Knowledge.KnowledgeModule.LANG_MNEMO_MI_SEARCH_KNOWLEDGE), psoft.Knowledge.Search.GetURL());
                                            moduleItem.Items.Add(kItem1);
                                            RadMenuItem kItem2 = new RadMenuItem(_mapper.get(ch.appl.psoft.Knowledge.KnowledgeModule.LANG_SCOPE_KNOWLEDGE, ch.appl.psoft.Knowledge.KnowledgeModule.LANG_MNEMO_MI_NEW_KNOWLEDGE), psoft.Knowledge.EditKnowledge.GetURL("mode", "add", "backURL", psoft.Knowledge.KnowledgeDetail.GetURL("knowledgeID", "%ID")));
                                            moduleItem.Items.Add(kItem2);
                                            RadMenuItem kItem3 = new RadMenuItem(_mapper.get(ch.appl.psoft.Knowledge.KnowledgeModule.LANG_SCOPE_KNOWLEDGE, ch.appl.psoft.Knowledge.KnowledgeModule.LANG_MNEMO_MI_PRINT_KNOWLEDGE), Global.Config.baseURL + "/report/selectKnowledge.aspx");                        //psoft.Knowledge.Print.GetURL());
                                            moduleItem.Items.Add(kItem3);
                                            getMenuItems(db_knowledge, baseID, moduleItem);
                                        }
                                        break;
                                    case "ORGANISATION":
                                        loadFromDB("ORGANISATION");
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                moduleItem = new RadMenuItem(module.GetLocalName(_mapper), "javascript: top.reloadSubNavigation('" + module.SubNavMenu + "');");
                            }


                            // QueryString-Check disabled
                            if ((module.GetLocalName(_mapper) != "WISSEN" || Request.QueryString["module"] == "knowledge") && module.GetLocalName(_mapper) != "MORPHOLOGIE")
                            {
                                leftMenu.Items.Add(moduleItem);
                            }
                        }
                    }

                    // Module einzeln durchgehen weil Sichtbarkeit generell deaktiviert

                    // QueryString-Check disabled
                    if (!module.IsVisible(Session) && _isLogged && module.Name != "organisation" && (true || Request.QueryString["module"] != "knowledge"))
                    {
                        switch (module.Name)
                        {
                            case "mbo":
                                if (!Global.isModuleEnabled("energiedienst"))
                                {
                                    RadMenuItem MbOTitleItem = new RadMenuItem(getDisplayName(module.GetLocalName(_mapper)));
                                    RadMenuItem MbOSearchItem;
                                    if (!Global.isModuleEnabled("spz"))
                                    {
                                        MbOSearchItem = new RadMenuItem(_mapper.get("mbo", "objectiveSearch"), Global.Config.baseURL + "/mbo/Search.aspx");
                                    }
                                    else
                                    {
                                        MbOSearchItem = new RadMenuItem(_mapper.get("mbo", "objectiveSearch"), Global.Config.baseURL + "/SPZ/Search.aspx");
                                    }
                                    MbOTitleItem.Items.Add(MbOSearchItem);

                                    string context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"], "");
                                    long contextId = ch.psoft.Util.Validate.GetValid(Request.QueryString["contextId"], 0L);

                                    string url;
                                    if (!Global.isModuleEnabled("spz"))
                                    {
                                        url = Global.Config.baseURL + "/mbo/Detail.aspx?view=add";
                                    }
                                    else
                                    {
                                        url = Global.Config.baseURL + "/spz/Detail.aspx?view=add";
                                    }
                                    url += context == ch.appl.psoft.Interface.DBObjects.Objective.SEARCH ? "&typ=" : "&context=" + context + "&contextId=" + contextId + "&typ=";

                                    try
                                    {
                                        int typ = db.Orgentity.getEmployment(db.userId);
                                        if (!db.Objective.isPersonFilterOnly)
                                        {
                                            if (db.Objective.hasAuthorisation(DBData.AUTHORISATION.UPDATE))
                                            {
                                                RadMenuItem MbOAddItem = new RadMenuItem(_mapper.get("mbo", "addObjective"), url + ch.appl.psoft.Interface.DBObjects.Objective.ORGANISATION_TYP);
                                                MbOTitleItem.Items.Add(MbOAddItem);
                                            }
                                            else if (typ == 1)
                                            {
                                                if (db.Orgentity.hasOEs(db.userId))
                                                {
                                                    RadMenuItem MbOAddOEObjectiveItem = new RadMenuItem(_mapper.get("mbo", "addOEObjective"), url + ch.appl.psoft.Interface.DBObjects.Objective.ORGENTITY_TYP);
                                                    MbOTitleItem.Items.Add(MbOAddOEObjectiveItem);
                                                }
                                            }
                                        }
                                        // add Link Firmenziele
                                        db.connect();
                                        long groupAccessorId = DBColumn.GetValid(
                                        db.lookup("ID", "ACCESSOR", "TITLE = 'HR'"),
                                        (long)-1);

                                        if (Global.isModuleEnabled("spz") & (db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true)))
                                        {
                                            RadMenuItem MbOAddFirmObjectiveItem = new RadMenuItem(_mapper.get("mbo", "addOrgObjective"), url + ch.appl.psoft.Interface.DBObjects.Objective.ORGANISATION_TYP);
                                            MbOTitleItem.Items.Add(MbOAddFirmObjectiveItem);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Log(ex, Logger.ERROR);
                                    }

                                    leftMenu.Items.Add(MbOTitleItem);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                //document search
                if (!Global.isModuleEnabled("energiedienst") && Global.Config.showDocumentSearch)
                {
                    RadMenuItem documentSearch = new RadMenuItem(_mapper.get("navigation", "documentSearch"), psoft.Document.DocumentSearch.GetURL());
                    leftMenu.Items.Add(documentSearch);
                }
            }

            //go back to login page after session timeout / 25.01.11
            timeout = 0;
            if (_isLogged)
            {
                timeout = this.Session.Timeout * 60 * 1000;
                string timeoutScript = "<script type=\"text/javascript\">"
                                        + "// go back to login page after session timeout\r\n"
                                        + "var timeoutInterval;\r\n"
                                        + "function goBack()\r\n"
                                        + "{\r\n"
                                        + "$find('ctl00_TimeoutWindow').show();\r\n"
                                        //+ "window.location = '" + _vDir + "/Basics/login.aspx" + "';\r\n"
                                        + "}\r\n"
                                        + "var timeout = " + timeout + ";\r\n"
                                        + "if(timeout > 0)\r\n"
                                        + "{\r\n"
                                        + "timeoutInterval = window.setInterval('goBack()', timeout);\r\n"
                                        + "}\r\n"
                                        + "</script>\r\n";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "timeoutScript", timeoutScript);

                string redirectLogin = "<script type =\"text/javascript\">"
                                          + "function redirectToLogin(e)\r\n"
                                          + "{\r\n"
                                          + "window.location = '" + _vDir + "/Basics/login.aspx" + "';\r\n"
                                          + "e.preventDefault();\r\n"
                                          + "}\r\n"
                                          + "</script>\r\n";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "redirectLogin", redirectLogin);
            }
        }

        public string LoginInfo
        {
            get
            {
                //some errors on page reload if not logged in, return empty string / 26.10.09 / mkr
                try
                {
                    return ((Global.Config.authenticationMode != Config.AUTHENTICATION_MODE.ANONYMOUS) && _isLogged) ? Session["UserInitials"].ToString() : "";
                }
                catch
                {
                    return "";
                }
            }
        }

        //get LoginInfo with link (if admin) / 24.03.10
        public string LoginInfoLink
        {
            get
            {
                return LoginInfo;
            }
        }

        protected string Logo
        {
            get
            {
                if (Global.Config.headerLogo != "")
                {
                    return Global.Config.baseURL + Global.Config.headerLogo;
                }
                return Global.Config.baseURL + "/images/transparent.gif";
            }
        }

        protected string LogoURL
        {
            get { return Global.Config.headerLogoURL; }
        }


        private void MapButtonMethods()
        {
            langGerman.Click += new System.EventHandler(langGerman_Click);
            langEnglish.Click += new System.EventHandler(langEnglish_Click);
            langFrench.Click += new System.EventHandler(langFrench_Click);
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
            Response.Redirect(Request.RawUrl);
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

        private void ImageButtonMoreClick(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            if (cssCharactersize < 4)
                db.execute("update person set CHARACTER_SIZE = CHARACTER_SIZE + 1 where id = " + db.userId);
            db.disconnect();
            Response.Redirect(Request.Url.ToString());

        }
        private void ImageButtonLessClick(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            db.execute("update person set CHARACTER_SIZE = CHARACTER_SIZE -1 where id = " + db.userId);
            db.disconnect();
            Response.Redirect(Request.Url.ToString());
        }


        protected void loadFromDB(string module)
        {
            // load menu items from db
            DBData db = DBData.getDBData(Session);
            db.connect();

            string baseID = db.lookup("ID", "MENUGROUP", "MNEMO = '" + module + "'", false);
            string titel = db.lookup("TITLE", "MENUGROUP", "ID = " + baseID, false);

            RadMenuItem mItem = new RadMenuItem(titel);
            getMenuItems(db, baseID, mItem);
            leftMenu.Items.Add(mItem);
        }

        protected void getMenuItems(DBData db, string parentID, RadMenuItem parentNode)
        {
            string titleColumnName = db.langAttrName("MENUV", "TITLE");
            long groupAccessorId = DBColumn.GetValid(
                    db.lookup("ID", "ACCESSOR", "TITLE = 'Administratoren'"),
                    (long)-1
                    );
            string sql = "";
            if (Convert.ToBoolean(db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true)))
            {
                sql = "SELECT * FROM MENUV WHERE PARENT_ID = " + parentID;
            }
            else
            {
                sql = "SELECT * FROM MENUV WHERE PARENT_ID = " + parentID + " AND ID IN (SELECT ROW_ID  FROM ACCESS_RIGHT_RT where (TABLENAME = 'MENUV') and (ACCESSOR_ID in (SELECT ACCESSOR_GROUP_ID FROM ACCESSOR_GROUP_ASSIGNMENT where ACCESSOR_MEMBER_ID = " + db.userId + ") or ACCESSOR_ID = " + db.userId + ")) ORDER BY ORDNUMBER ASC, " + titleColumnName + " ASC";
            }

            DataTable table = db.getDataTable(sql, "MENUV");

            foreach (DataRow row in table.Rows)
            {

                string ID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), "");
                string title = ch.psoft.Util.Validate.GetValid(row[titleColumnName].ToString(), "");
                string tablename = ch.psoft.Util.Validate.GetValid(row["TABLENAME"].ToString(), "");

                switch (tablename)
                {
                    case "MENUGROUP":
                        RadMenuItem subNode = new RadMenuItem(title);
                        //subNode.ImageUrl = "ig_menuTri.gif";
                        //subNode.ImageToolTip = "ig_menuTri.gif";
                        if (title == "Sokrateskarten" && Global.Config.isModuleEnabled("SBS") && LoginInfo != "admin")
                        {
                            DataTable sokratesKarten = db.getDataTable("SELECT      MATRIX.ID,  MATRIX.UID, MATRIX.TITLE "
                                + "FROM MATRIX INNER JOIN ACCESS_RIGHT_RT ON MATRIX.ID = ACCESS_RIGHT_RT.ROW_ID "
                                + "WHERE (ACCESS_RIGHT_RT.TABLENAME = 'MATRIX') AND (ACCESS_RIGHT_RT.AUTHORISATION > 1) AND (ACCESS_RIGHT_RT.ACCESSOR_ID = " + db.userId + ") ");
                            foreach (DataRow r in sokratesKarten.Rows)
                            {
                                subNode.Items.Add(new RadMenuItem(r["TITLE"].ToString(), Global.Config.baseURL + "/Morph/MatrixDetail.aspx?matrixID=" + r["ID"].ToString() + "&UID=" + r["UID"].ToString()));
                            }
                        }
                        getMenuItems(db, ID, subNode);
                        parentNode.Items.Add(subNode);

                        break;
                    case "MENUITEM":
                        string url = ch.psoft.Util.Validate.GetValid(row["LINK"].ToString(), "");
                        RadMenuItem menuItem = new RadMenuItem();
                        menuItem.Text = title;
                        menuItem.NavigateUrl = Global.Config.baseURL + url;
                        menuItem.PostBack = false;

                        parentNode.Items.Add(menuItem);
                        break;
                }

            }
        }

        protected string getDisplayName(string name)
        {
            // returns the lowered string with first character capitalized
            string displayName = name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1).ToLower();
            return displayName;
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            if (Session[BreadcrumbItem.BREAD_CRUMB_ITEM] != null)
            {
                BreadcrumbItem _item = (BreadcrumbItem)Session[BreadcrumbItem.BREAD_CRUMB_ITEM];
                breadcrumbDd.DataSource = _item.Items;
                breadcrumbDd.DataTextField = "Caption";
                breadcrumbDd.DataValueField = "Link";
                breadcrumbDd.DataBind();
            }
            base.OnPreRender(e);
        }

        override protected void OnInit(EventArgs e)
        {
            MapButtonMethods();
            base.OnInit(e);
        }
    }
}
