using ch.appl.psoft.Morph;
using System;
using System.Collections;
using System.Data;
using System.Drawing;

namespace ch.appl.psoft.Knowledge
{
    using ch.appl.psoft.Knowledge.Controls;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface.DBObjects;
    using LayoutControls;
    /// <summary>
    /// 
    /// </summary>
    public partial class KnowledgeDetail : PsoftDetailPage {
        public const string PAGE_URL = "/Knowledge/KnowledgeDetail.aspx";

        public const string KNOWLEDGE_ID = "knowledgeID";
        public const string THEME_ID = "themeID";
        public const string SUGGESTION_EXECUTION_ID = "suggestionExecutionID";

		public const string SLAVE_CHARACTERISTIC_ID = "slaveCharID";
        
        // possible contexts: searchResult
        public const string CONTEXT = "context";

        // ID of object identified by parameter context.
        public const string XID = "xID";

		// ID of object identified by parameter context.
		public const string PRINT = "PRINT";

        // The target directory of generated documents (print).
        public const string PRINT_TARGET_DIRECTORY = "x";

        //set up special table widths in percent.
        public static Morph.TableToWord.ProportionPair[] DOCTABLE_PROP_PAIR_ARRAY = { new Morph.TableToWord.ProportionPair(2,50) };
        public static Morph.TableToWord.ProportionPair[] HISTORYTABLE_PROP_PAIR_ARRAY = { new Morph.TableToWord.ProportionPair(0,12),
                                                                                          new Morph.TableToWord.ProportionPair(1,12),
                                                                                          new Morph.TableToWord.ProportionPair(3,30)
                                                                                        };



        /// <summary>
        /// word printer
        /// </summary>
        private MorphToWord morphToWord;

        static KnowledgeDetail(){
            SetPageParams(PAGE_URL, KNOWLEDGE_ID, CONTEXT, XID,SLAVE_CHARACTERISTIC_ID,PRINT, THEME_ID, SUGGESTION_EXECUTION_ID);
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }

        private long _knowledgeID = -1L;
		private long _slaveCharacteristicID = -1L;
        private long _suggestionExecutionID = -1L;
		private int _print = 0;
        

		KnowledgeDetailCtrl detail = null;

        public KnowledgeDetail() : base(){
            PageURL = PAGE_URL;
        }

        protected override void Initialize() {
            base.Initialize();

            _knowledgeID = GetQueryValue(KNOWLEDGE_ID, -1L);
            if(Global.Config.isModuleEnabled("suggestion")) 
            {
                _suggestionExecutionID = GetQueryValue(SUGGESTION_EXECUTION_ID, -1L);
            }
            else 
            {
                _suggestionExecutionID = -1L;
            }
			_slaveCharacteristicID = GetQueryValue(SLAVE_CHARACTERISTIC_ID, -1L);
            _print =  GetQueryValue(PRINT, 0);

            if(_suggestionExecutionID > 0L && _knowledgeID < 0L)  //find out the knowledge id if the suggestion execution id is given
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                try 
                {
                    _knowledgeID = db.lookup("ID", "KNOWLEDGE", "SUGGESTION_EXECUTION_ID = " + _suggestionExecutionID, -1L);
                }
                catch (Exception ex) 
                {
                    ShowError(ex.Message);
                    Logger.Log(ex,Logger.ERROR);
                }
                finally 
                {
                    db.disconnect();
                }               
            }
    
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            DBData db = DBData.getDBData(Session);

			PsoftPageLayout pageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pL");
			PageLayoutControl = pageLayout;

            if ( _knowledgeID > 0L ) {
                db.connect();
                try {
                    PageTitle = BreadcrumbCaption = db.Knowledge.getTitle(_knowledgeID);
                    BreadcrumbName += _knowledgeID;
                }
                catch ( Exception ex ) {
                    ShowError( ex.Message );
                    Logger.Log( ex, Logger.ERROR );
                }
                finally {
                    db.disconnect();
                }
            }

            // Setting content layout of page layout
            DGLContentLayout dglControl = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = dglControl;

            string context = GetQueryValue(CONTEXT, "");
            long xID = GetQueryValue(XID, -1L);
            string myURL = Request.RawUrl;

            db.connect();
            try {
                if (_knowledgeID > 0L && db.hasRowAuthorisation(DBData.AUTHORISATION.READ, Knowledge._TABLENAME, _knowledgeID, true, true)){
                    string PageTitleExtention;
                    if ((int)db.lookup("LOCAL", "KNOWLEDGE", "ID = " + _knowledgeID) == 1)
                    {
                        PageTitleExtention = "(lokal)  " + BreadcrumbCaption;
                    }
                    else
                    {
                        PageTitleExtention = "(global)  " + BreadcrumbCaption;
                    }
                    (PageLayoutControl as PsoftPageLayout).PageTitle = PageTitleExtention;
                    long knowledgeUID = db.ID2UID(_knowledgeID, Knowledge._TABLENAME);
 
					 // Drucken von Wissenselementen ist noch nicht produktiv einsetzbar 
					pageLayout.ButtonPrintEnabled = true;
					pageLayout.ButtonPrintVisible = true;
                    pageLayout.ButtonPrintToolTip = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE,KnowledgeModule.LANG_MNEMO_TT_GENERATE_WORDDOC);
					string paramString =  Request.QueryString["knowledgeID"];
					pageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('../report/CrystalReportViewer.aspx?alias=Knowledge.rpt&param0=" + paramString + "','_target')");
                    

                    Knowledge knowledge = new Knowledge(db,Session);
					bool isLatestEntry = _knowledgeID == knowledge.getLatestKnowledgeIdFromHistory(_knowledgeID);

                    // Setting parameters
                    /*KnowledgeDetailCtrl*/ detail = (KnowledgeDetailCtrl) LoadPSOFTControl(KnowledgeDetailCtrl.Path, "_detail");
                    detail.KnowledgeID = _knowledgeID;
                    detail.SlaveCharacteristicID = _slaveCharacteristicID;
					detail.ShowHistoryEntry = !isLatestEntry;
										
					// Setting content layout user controls
                    SetPageLayoutContentControl(DGLContentLayout.DETAIL, detail);		

                    PsoftLinksControl links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                    links.LinkGroup1.Caption = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_CT_KNOWLEDGE);
                    links.LinkGroup2.Caption = _mapper.get("uidAssignment", "assignments");
                    detail.LinkGroup = links.LinkGroup1;

                    // context-links...
                    if(_suggestionExecutionID < 0L && Global.Config.isModuleEnabled("suggestion")) 
                    {
                        _suggestionExecutionID  = db.lookup("SUGGESTION_EXECUTION_ID", "KNOWLEDGE","ID="+ _knowledgeID, -1L);    //new for suggestions
                    }
                    bool adminRights = false;
                    int type = 0; //default
                    if(Global.Config.isModuleEnabled("suggestion")) 
                    {
                        adminRights = this.detail.suggestionAdminRights(db, _suggestionExecutionID);
                        type = db.lookup("TYPE", "KNOWLEDGE","ID="+ _knowledgeID, 0);    //new for suggestions
                    }
                    if( type != 1 || adminRights) // not a suggestion
                    {

                        if (isLatestEntry && db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, Knowledge._TABLENAME, _knowledgeID, true, true) &&
                            (type != 1) ) // not a suggestion
                        {
                            links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_CL_EDIT_KNOWLEDGE), psoft.Knowledge.EditKnowledge.GetURL("mode","edit", "knowledgeID",_knowledgeID, "backURL",Request.RawUrl));
                        }
					
                        if (isLatestEntry && db.hasRowAuthorisation(DBData.AUTHORISATION.DELETE, Knowledge._TABLENAME, _knowledgeID, true, true))
                        {
                            links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_CL_DELETE_KNOWLEDGE), "javascript: listDeleteRowConfirm('','" + _knowledgeID + "','" + Knowledge._TABLENAME + "')");
                        }
                        if (isLatestEntry && db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, Knowledge._TABLENAME, _knowledgeID, true, true))
                        {
                            links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_CL_ADD_THEME), psoft.Knowledge.EditTheme.GetURL("parentThemeID",db.Knowledge.getBaseThemeID(_knowledgeID), "backURL",Request.RawUrl, "mode","add"));
                            if( type != 1 ) 
                            {
                                links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_CL_ADD_DOCUMENT), psoft.Document.Add.GetURL("table","DOCUMENT", "XID",_knowledgeID, "context","knowledge", "triggerUID",knowledgeUID, "registryEnable","false", "backURL",Request.RawUrl, "docType",(int) Document.DocType.Document));
                                //links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_CL_ADD_IMAGE), Psoft.Wiki.ImageAdd.GetURL("ownerUID",knowledgeUID, "backURL",Request.RawUrl), "");
                                links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_BC_ADD_HISTORY),psoft.Knowledge.EditHistory.GetURL("knowledgeID", _knowledgeID, "mode","add")); 						
                            }
                            
                        }

                        if(isLatestEntry&&
                            ( type != 1 ) )
                        {   // disable abonnenments until implemented correctly
                            //links.LinkGroup1.AddAboLinks(Knowledge._TABLENAME, _knowledgeID, _mapper);
                        }
                    }
                    if (isLatestEntry && db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, Knowledge._TABLENAME, _knowledgeID, true, true)&&
                        (type != 1))
                    {
                        // TODO: link und reportlayout für Serienemail definieren
                        links.LinkGroup1.AddLink(_mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_CT_ADMINISTRATION), _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_CL_INVITE_AUTHORS),
                            psoft.Dispatch.ManualMail.GetURL("reportLayoutID",db.ReportLayout.getIDbyTitleMnemo("SerialEmailAccessor"),
                                                            "param0",PSOFTConvert.Join(",", db.getRowAccessorIDs(Knowledge._TABLENAME, _knowledgeID, DBData.AUTHORISATION.UPDATE, DBData.APPLICATION_RIGHT.COMMON)),
                                                            "param1",DBColumn.toSql("http://" + Global.Config.domain + psoft.Goto.GetURL("uid",knowledgeUID, "noFrames","true")),
                                                            "param2",DBColumn.toSql(BreadcrumbCaption),
                                                            "backURL",Request.RawUrl));
                    }
                   
                    if(isLatestEntry &&
                        (_suggestionExecutionID == -1))
					{
						links.LinkGroup2.AddLink(_mapper.get("actions"), _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_CL_NEW_KNOWLEDGE), psoft.Knowledge.EditKnowledge.GetURL("mode","add", "linkingKnowledgeID",_knowledgeID, "backURL",Request.RawUrl));
						links.LinkGroup2.AddLink(_mapper.get("actions"), _mapper.get("uidAssignment", "newAssignment"), psoft.Common.AddUIDAssignments.GetURL("fromUID",knowledgeUID));
						links.LinkGroup2.AddLink(_mapper.get("actions"), _mapper.get("uidAssignment", "manageAssignments"), psoft.Common.ManageUIDAssignments.GetURL("fromUID",knowledgeUID));
					}

					links.LinkGroup2.AddUIDAssignmentLinks(_mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_CT_ASSIGNED_KNOWLEDGES), Knowledge._TABLENAME, _knowledgeID, new string[]{Knowledge._TABLENAME}, null, DBData.ASSIGNMENT.ANY, -1L, true, true, _mapper, false);
					links.LinkGroup2.AddUIDAssignmentLinks(_mapper.get("uidAssignment", "assignedTo"), Knowledge._TABLENAME, _knowledgeID, null, new string[]{Knowledge._TABLENAME}, DBData.ASSIGNMENT.ANY, -1L, true, true, _mapper, true);

                    if (context == "searchResult"){
                        // Load list control
                        KnowledgeListCtrl list = (KnowledgeListCtrl) LoadPSOFTControl(KnowledgeListCtrl.Path, "_list");
                        list.XID = xID;
                        list.Kontext = context;
                        list.OrderColumn = GetQueryValue("orderColumn", "TITLE");
                        list.OrderDir = GetQueryValue("orderDir", "asc");
                        //list.SortURL = "../Detail.aspx?ID="+id;

                        // Setting content layout
                        SetPageLayoutContentControl(DGLContentLayout.GROUP, list);
                    }

                    ((PsoftPageLayout)PageLayoutControl).ShowButtonAuthorisation(Knowledge._TABLENAME, _knowledgeID);
                    ((PsoftPageLayout)PageLayoutControl).ShowButtonRegistryEntries(Knowledge._TABLENAME, _knowledgeID, "");

                    SetPageLayoutContentControl(DGLContentLayout.LINKS, links);	
                }
                else{
                    BreadcrumbVisible = false;
                    Response.Redirect(NotFound.GetURL(), false);
                }
            }
            catch (Exception ex) {
                ShowError(ex.Message);
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                db.disconnect();
            }
        }

		protected void printClick(object sender, System.EventArgs e) 
		{
            //not used (commented out code removed from revision 184).							
		}

		protected override void OnPreRender( System.EventArgs e ) 
		{	   
            if(_print == 1)
            {
                DBData db = DBData.getDBData(Session);

                string fileName = "wissenelement" + DateTime.Now.Ticks + ".doc";         
                //string url = Global.Config.documentSaveURL + "\\" + PRINT_TARGET_DIRECTORY + "\\" + fileName;
                string url = "c:\temp\test.doc";
                db.connect();
                try
                {
                    long owner_uid = db.ID2UID(_knowledgeID, Knowledge._TABLENAME);

                    string test = MorphToWord.outputFile(fileName, PRINT_TARGET_DIRECTORY);
                    
                    this.morphToWord = new MorphToWord(MorphToWord.outputFile(fileName, PRINT_TARGET_DIRECTORY), db, owner_uid);  //opened and ready
                    morphToWord.Mapper = this._mapper;

                    this.morphToWord.printIndex();
                    //Wissenelement and theme
                    MorphToWord.TitleWikiPair wissenElement = prepareWissenElement(db);
                    this.morphToWord.printWissenElement(wissenElement);
                    this.morphToWord.printNewLine();
                    MorphToWord.TitleWikiPair[] themenElement = MorphToWord.prepareThemen(db, this._knowledgeID);
                    this.morphToWord.printAllThemen(themenElement);
                    this.morphToWord.printNewLine();

                    string totalString = "<p><h1>" + wissenElement.title + "</h1>" + wissenElement.wikiText + "</p>";
                    foreach (MorphToWord.TitleWikiPair theme in themenElement)
                    {                    
                       //convert strings
                        totalString += "<p><h2>" + theme.title + "</h2>\b" + theme.wikiText + "<br/></p><p>";
                    }

                    //Themes table
                    this.morphToWord.printTable("Themen", detail.TableThemes);
                    //Document table
                    this.morphToWord.printTable("Dokumente", detail.TableDocuments, DOCTABLE_PROP_PAIR_ARRAY);
                    //History table
                    this.morphToWord.printTable("Versionsgeschichte", detail.TableHistory, HISTORYTABLE_PROP_PAIR_ARRAY);

                    string[] registryIDs = db.Registry.getRegistryIDs(_knowledgeID.ToString(), "KNOWLEDGE").Split(',');
                    string[] regPaths = new string[registryIDs.Length];
                    int k = 0;
                    foreach (string registryID in registryIDs)
                    {
                        if (registryID != "")
                        {
                            string path = db.Registry.getRegistryPath(Int64.Parse(registryID), " / ", false);
                            regPaths[k] = path;
                            k++;
                        }                  
                    }
                    string[] regPathsDest = new string[k];
                    Array.Copy(regPaths,0,regPathsDest,0,k);

                    this.morphToWord.printRegistratur("Registratur", regPathsDest);

                    if(Global.Config.isModuleEnabled("suggestion")) 
                    {
                        ArrayList ranklist = new ArrayList();
                        long suggestionExecutionId = ch.psoft.Util.Validate.GetValid(db.lookup("SUGGESTION_EXECUTION_ID" , "KNOWLEDGE", "ID=" + _knowledgeID, false), -1L);
                        if(suggestionExecutionId!=-1) 
                        {
                            string remark = ch.psoft.Util.Validate.GetValid(db.lookup("REMARK" , "SUGGESTION_EXECUTION", "ID=" + suggestionExecutionId, false), "");
                            DataTable colTable = db.getDataTable("select * from suggestion_execution_stati where suggestion_execution_id = " + suggestionExecutionId);
                            if(colTable.Rows.Count > 0 )
                            {
                                foreach(DataRow r in colTable.Rows)
                                {
                                    int color32 = DBColumn.GetValid(r["COLOR"], 0xffffff); //directly read from the table (global)
                                    System.Drawing.Color color = Color.FromArgb(color32);
                                    string text = DBColumn.GetValid(r["Title"],"");
                                    bool selected = (DBColumn.GetValid(r["CHOSEN"],0) == 1);
                                    ranklist.Add(new MorphToWord.RankingField(selected, color, text));
                                }
                            }
                            string title = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_ST_RANKING_TITLE);
                            this.morphToWord.printRanking(title, remark, ranklist, 1);
                        }
                    }

                    this.morphToWord.close();
                    //				MorphTextReport mr = new MorphTextReport();		
                    //				mr.create(db, db.ID2UID(_knowledgeID, Knowledge._TABLENAME), MorphTextReport.outputFile(fileName), db.Knowledge.getDescription(_knowledgeID));


                    Response.Redirect(url, false);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex,Logger.ERROR);
                    Response.Redirect(NotFound.GetURL(), false);
                }
                finally
                {
                    db.disconnect();							
                }		
            }
            else if(_print > 1) 
            {
                int themeId = _print;
                DBData db = DBData.getDBData(Session);

                string fileName = "thema" + DateTime.Now.Ticks + ".doc";
                string url = Global.Config.documentSaveURL + "\\" + PRINT_TARGET_DIRECTORY + "\\" + fileName;
                db.connect();
                try
                {
                    this.morphToWord = new MorphToWord(MorphToWord.outputFile(fileName, PRINT_TARGET_DIRECTORY), db, db.ID2UID(_knowledgeID, Knowledge._TABLENAME));  //opened and ready
                    morphToWord.Mapper = this._mapper;

                    MorphToWord.TitleWikiPair theme = MorphToWord.prepareTheme(db,themeId);
                     
                    this.morphToWord.printWiki(theme,1);

                    this.morphToWord.close();

                    Response.Redirect(url, false);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex,Logger.ERROR);
                    Response.Redirect(NotFound.GetURL(), false);
                }
                finally
                {
                    db.disconnect();							
                }		
            }

            //Do not forget! (breadcrumb element is loaded here)
            base.OnPreRender( e );
		}

        private MorphToWord.TitleWikiPair prepareWissenElement(DBData db)
        {
            string ktitle = db.Knowledge.getTitle(_knowledgeID);
            string kdes = db.Knowledge.getDescription(_knowledgeID);
            return new MorphToWord.TitleWikiPair(ktitle,kdes);
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
