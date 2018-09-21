namespace ch.appl.psoft.Knowledge.Controls
{
    using ch.appl.psoft.Wiki;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface.DBObjects;
    using LayoutControls;
    using System;
    using System.Collections;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    public partial class KnowledgeDetailCtrl : PSOFTListViewUserControl
    {
        protected System.Web.UI.WebControls.Table Table1;
        public System.Web.UI.WebControls.Table TableThemes 
        {
            get { return tableThemes; }
        }

        public System.Web.UI.WebControls.Table TableDocuments 
        {
            get { return tableDocuments; }
        }
        
        public System.Web.UI.WebControls.Table TableHistory 
        {
            get { return tableHistory; }
        }

		
        protected DBData _db = null;
        protected AutoNumbering _autoNumbering = new AutoNumbering();
        private TextBox reasonTextBox = new TextBox();
        private TextBox suggestionReasonTextBox = new TextBox();


        protected string _searchURL          = "";
        protected string _latestURL          = "";
        protected string _previousURL        = "";
        protected string _currentURL		 = "";
        protected string _confirmAllElements = "";
        protected string _errorDeleteEntry   = "";

        protected long   _latestId         = -1;
        protected long   _currentId        = -1; 

        bool hasEditCell = false; 
		
        public static string Path
        {
            get {return Global.Config.baseURL + "/Knowledge/Controls/KnowledgeDetailCtrl.ascx";}
        }

        private const int MAXTABLEOFCONTENTSLENGTH = 35;

        public KnowledgeDetailCtrl() : base()
        {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = true;
            DetailEnabled = true;
            InfoBoxEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
        }

		#region Properities

        private long _knowledgeID = -1;
        public long KnowledgeID
        {
            get {return _knowledgeID;}
            set {_knowledgeID = value;}
        }

        private long _suggestionExecutionId = -1;

        private bool isHistoryEntry = false;
        public bool ShowHistoryEntry 
        {
            get {return isHistoryEntry; }
            set {isHistoryEntry = value;}
        }


        private long _slaveCharacteristicID = -1;
        public long SlaveCharacteristicID
        {
            get {return _slaveCharacteristicID;}
            set {_slaveCharacteristicID = value;}
        }
			
        private LinkGroupControl _linkGroup = null;
        public LinkGroupControl LinkGroup
        {
            get {return _linkGroup;}
            set {_linkGroup = value;}
        }
		
        private ArrayList radiobuttons = new ArrayList();

        public ArrayList RadioButtons 
        {
            get { return radiobuttons; }

        }

        private bool firstHistoryRow = true;
		
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();

            _db = DBData.getDBData(Session);
            _db.connect();
            try 
            {
                IDColumn = "ID";
				
                _currentId   = KnowledgeID;
                _latestId    = _db.Knowledge.getLatestKnowledgeIdFromHistory(KnowledgeID);
				
                _confirmAllElements = "Wollen Sie wirklich das gesamte Wissenselement und dessen Versionsgeschichte löschen ?";
                
                //in case the entry cannot be deleted the following text is displayed to the user in a pup-up window. see javascript 
                //procedure deleteRowConfirm in KnowledgeDetailCtrl.ascx resp. procedure wsDeleteRowConfirmExt in PsoftService.js.
                _errorDeleteEntry   = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_POPUP_CANNOT_DELETE_WE); 
				
                long versionLatest  = _db.lookup("VERSION","KNOWLEDGE","ID="+ _latestId, -1L);           		           		
                long previousId	    = _db.lookup("TOP 1 ID","KNOWLEDGE","VERSION < " + versionLatest + " ORDER BY VERSION DESC ",-1L);

                if(Global.Config.isModuleEnabled("suggestion")) 
                {
                    _suggestionExecutionId = _db.lookup("SUGGESTION_EXECUTION_ID", "KNOWLEDGE","ID="+ _currentId, -1L);    //new for suggestions
                }                

                bool hasEditCell = true;
                if(Global.Config.isModuleEnabled("suggestion")) 
                {
                    hasEditCell = suggestionAdminRights(this._db, this._suggestionExecutionId);
                }

                // define the urls for the deleteRowConfirm javascript. look at the .ascx file
                _latestURL   = KnowledgeDetail.GetURL("knowledgeID",_latestId,"slaveCharID",SlaveCharacteristicID);
				
                if(previousId > 0)
                {
                    _previousURL = KnowledgeDetail.GetURL("knowledgeID",previousId,"slaveCharID",SlaveCharacteristicID);
                }
                else
                {
                    _previousURL = "";
                }
				
                _currentURL  = KnowledgeDetail.GetURL("knowledgeID",_latestId,"slaveCharID",SlaveCharacteristicID);;
                _searchURL   = Search.GetURL();			
                // description...
                ArrayList contentsEntries = new ArrayList();
//                labelDescription.Text = _db.Theme.text2HTML(_db.Knowledge.getDescription(_knowledgeID), _db, _db.ID2UID(_knowledgeID, Knowledge._TABLENAME), ref _autoNumbering, 0, ref contentsEntries);				
                labelDescription.Text = _db.Knowledge.getDescription(_knowledgeID);
  
                addLinks(contentsEntries);

                // themes...
                long baseThemeID = _db.Knowledge.getBaseThemeID(_knowledgeID);
                string sql = "select * from THEME where PARENT_ID=" + baseThemeID; 
				
                // 
                if(Global.isModuleEnabled("morph"))
                {
                    sql += " AND ID NOT IN (SELECT THEME_ID FROM SLAVE_CHARACTERISTIC WHERE THEME_ID IS NOT NULL)";
                }

                sql += "order by ORDNUMBER asc";
				
                DataTable table = _db.getDataTableExt(sql, "THEME");
                foreach (DataRow row in table.Rows)
                {
                    if (_db.hasRowAuthorisation(DBData.AUTHORISATION.READ, table, row, true, true))
                    {
                        addTheme(row, _suggestionExecutionId);
                    }
                }
			
                // Matrix characteristic if enabled
                if(Global.isModuleEnabled("morph"))
                {
                    if(_slaveCharacteristicID > 0 )
                    {
                        addSlaveCharacteristicTheme(_slaveCharacteristicID);
                    }
                }

                //Suggestion: Display rating
                if( isSuggestion() ) //this is a suggestion knowledge
                {
                    displaySuggestionRating(_suggestionExecutionId);
                }

                // documents...
                sql = "select ID, TITLE, FILENAME, DESCRIPTION, TYP, CREATED from DOCUMENT where KNOWLEDGE_ID=" + _knowledgeID + " and TYP=" + ((int)Document.DocType.Document) + " order by TITLE asc";
                table = _db.getDataTableExt(sql, "DOCUMENT");
                table.Columns["DESCRIPTION"].ExtendedProperties["Visibility"] = DBColumn.Visibility.LIST | DBColumn.Visibility.INFO;
                table.Columns["CREATED"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INFO;
                HeaderEnabled = true;
                InfoBoxEnabled = true;
                
                //DetailURL = Psoft.Document.Detail.GetURL("table","DOCUMENT", "xID","%ID", "registryEnable","false");
					
                DetailEnabled = false;
                if(isHistoryEntry)
                {
                    EditEnabled = false;
                }
                else
                {
                    EditURL = psoft.Document.Edit.GetURL("table","DOCUMENT", "xID","%ID", "registryEnable","false", "backURL",Request.RawUrl);				
                }

                if (LoadList(_db, table, tableDocuments) > 0)
                {
                    string title = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_ST_DOCUMENTS);
                    string number = _autoNumbering.GetNextNumber(0);
                    string anchor = number.Replace(".", "_");
                    addLink(new ContentsEntry(anchor, number, title));
                    labelDocuments.Text = "<br>" + " " + title;					
                    labelDocuments.Text += "<a name='" + anchor + "'/>";
                }
                else
                {
                    tableDocuments.Visible = false;
                }
				
                // History des Wissenelements
                Knowledge knowledge = new Knowledge(_db,Session);				
                long historyRootId = knowledge.getBaseKnowledgeId(_knowledgeID);

                sql = "select k.* from KNOWLEDGE_V k where k.ID =" + historyRootId + " or k.HISTORY_ROOT_ID =" + historyRootId + " order by VERSION desc ,CREATED desc";
				
                table = _db.getDataTableExt(sql, "KNOWLEDGE_V");
                DataTable personTab = _db.Person.getWholeNameMATable(true);											
                table.Columns["TRIGGER_UID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                if (Global.Config.isModuleEnabled("suggestion") && isSuggestion()) 
                {
                    table.Columns["SUGGESTION_EXECUTION_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    table.Columns["ISACTIVE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    table.Columns["TYPE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                }

                table.Columns[_db.langAttrName(Knowledge._VIEWNAME, "CREATOR_PERSON_ID")].ExtendedProperties["In"] = personTab;				
				
                DetailEnabled = true;
                if(hasEditCell) 
                {
                    EditEnabled = true;	
                    DeleteEnabled = true;	
                } 
                else 
                {
                    EditEnabled = false;	
                    DeleteEnabled = false;	
                }
				
                if(SlaveCharacteristicID > 0 )
                {
                    DetailURL = KnowledgeDetail.GetURL("context","history","knowledgeID","%ID","slaveCharID",SlaveCharacteristicID);
                }
                else
                {
                    DetailURL = KnowledgeDetail.GetURL("context","history","knowledgeID","%ID");
                }

                if(!IsPostBack && !isSuggestion())
                {
				
                    EditURL = EditHistory.GetURL("knowledgeID","%ID","mode","edit");
                    HeaderEnabled = true;
                    InfoBoxEnabled = true;
				
                    string oldView =  base.View;
                    View = "HISTORY";
                    CheckOrder = true;
                    if (LoadList(_db, table, tableHistory) > 0)
                    {
                        string title = "Versionsgeschichte";					
                        string number = _autoNumbering.GetNextNumber(0);
                        string anchor = number.Replace(".", "_");
                        addLink(new ContentsEntry(anchor, number, title));
                        labelHistory.Text = "<br>" +  title;
                        labelHistory.Text += "<a name='" + anchor + "'/>";
                    }
                    else
                    {
                        tableHistory.Visible = false;
                    }
 
                    CheckOrder = false;				
                    View = oldView;
                }

                // registry...
                string[] registryIDs = _db.Registry.getRegistryIDs(_knowledgeID.ToString(), "KNOWLEDGE").Split(',');
                ArrayList regPaths = new ArrayList();
                foreach (string registryID in registryIDs)
                {
                    if (registryID != "")
                    {
                        regPaths.Add(_db.Registry.getRegistryPath(Int64.Parse(registryID), " / ", false));
                    }
                }
                if (regPaths.Count > 0)
                {
                    string title = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_ST_REGISTRY);
                    labelRegistry.Text = "<br>" + title;
                    string number = _autoNumbering.GetNextNumber(0);
                    string anchor = number.Replace(".", "_");
                    addLink(new ContentsEntry(anchor, number, title));
                    labelDocuments.Text += "<a name='" + anchor + "'/>";
                    regPaths.Sort();
                    for(int i=0; i<regPaths.Count; i++)
                    {
                        TableRow registryRow = new TableRow();
                        tableRegistry.Rows.Add(registryRow);
                        TableCell registryCell = new TableCell();
                        registryRow.Cells.Add(registryCell);
                        registryCell.Text = " -- " + regPaths[i];
                    }
                }
            }
            catch (Exception ex) 
            {
                DoOnException(ex);
            }
            finally 
            {
                _db.disconnect();
            }
        }

        protected void addLink(ContentsEntry contentsEntry)
        {
            string title = contentsEntry._autoNumber + " " + contentsEntry._title;
            if(title.Length > MAXTABLEOFCONTENTSLENGTH)
                title = title.Substring(0,MAXTABLEOFCONTENTSLENGTH);
            _linkGroup.AddLink(_mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_CT_CONTENTS), title , "#" + contentsEntry._anchor);
        }

        protected void addLinks(ArrayList contentsEntries)
        {
            foreach(ContentsEntry contentsEntry in contentsEntries)
            {
                addLink(contentsEntry);
            }
        }
		
        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell c) 
        {
            if (col != null) 
            {
                switch (col.ColumnName) 
                {
                    case "FILENAME":
                        if (ch.psoft.Util.Validate.GetValid(c.Text) != "") 
                        {
                            HyperLink link = new HyperLink();
                            link.Text = DBColumn.GetValid(row[col], c.Text);
                            c.Controls.Clear(); 
                            c.Controls.Add(link);
                            link.CssClass = "List";
                            link.Target = "_blank";
                            link.NavigateUrl = psoft.Document.GetDocument.GetURL("documentID",row["ID"]);
                        }
                        break;					
                    default:
                        break;
                }
            }
			
            if(View == "HISTORY")
            {

                if(_knowledgeID == (long) row["ID"])
                {
                    c.CssClass = "List_selected";
                }
			
                if(!ListBuilder.IsEditCell(c) && !ListBuilder.IsDeleteCell(c) )
                {
                    if(!firstHistoryRow)
                    {
                        c.Font.Italic = true;
                    }
                    else
                    {
                        c.Font.Bold = true;
                    }
                }
                else
                {
                    if(ListBuilder.IsDeleteCell(c))
                    {
                        if(_latestId == (long) row["ID"])
                        {
                            c.Controls.Clear();
                        }
                        else
                        {
                            HyperLink l = (HyperLink) c.Controls[0];
                            l.NavigateUrl = l.NavigateUrl.Replace("KNOWLEDGE_V","KNOWLEDGE");
                        }
                    }
                }
            }
        }

        protected override void onAddRow (DataRow row, TableRow r) 
        {
            r.Style.Add("vertical-align", "top");
			
            if(View == "HISTORY")
            {             				
                firstHistoryRow = false;
            }
        }

        protected void addTheme(DataRow row, long suggestionId)
        {
            long themeID = DBColumn.GetValid(row["ID"], -1L);
            string title = DBColumn.GetValid(row["TITLE"], "");
            string number = _autoNumbering.GetNextNumber(0);
            string anchor = number.Replace(".", "_");
            addLink(new ContentsEntry(anchor, number, title));

            // title
            TableRow titleRow = new TableRow();
            tableThemes.Rows.Add(titleRow);
            titleRow.ID = "title_" + anchor;
            TableCell titleCell = new TableCell();
            titleRow.Cells.Add(titleCell);
            titleCell.Width = Unit.Percentage(100);
            HtmlAnchor titleAnchor = new HtmlAnchor();
            titleCell.Controls.Add(titleAnchor);
            titleAnchor.Name = anchor;
            Label titleLabel = new Label();
            titleCell.Controls.Add(titleLabel);
            titleLabel.Text = /* number + " " + */ title; //don't display the item number in the title
            titleRow.CssClass = titleLabel.CssClass = titleCell.CssClass = "themeTitle";

            // description
            ArrayList contentsEntries = new ArrayList();
            TableRow descriptionRow = new TableRow();
            tableThemes.Rows.Add(descriptionRow);
            descriptionRow.ID = "desc_" + anchor;
            TableCell descriptionCell = new TableCell();
            descriptionRow.Cells.Add(descriptionCell);
            descriptionCell.ColumnSpan = 4;
            Label descriptionLabel = new Label();
            descriptionCell.Controls.Add(descriptionLabel);
//            descriptionLabel.Text = _db.Theme.text2HTML(DBColumn.GetValid(row["DESCRIPTION"], ""), _db, _db.ID2UID(_knowledgeID, Knowledge._TABLENAME), ref _autoNumbering, 1, ref contentsEntries);
            descriptionLabel.Text = DBColumn.GetValid(row["DESCRIPTION"], "");

            descriptionRow.CssClass = descriptionLabel.CssClass = descriptionCell.CssClass = "themeText";
            addLinks(contentsEntries);

			
            // edit/delete...
            bool hasEditCell = true;
            if(Global.Config.isModuleEnabled("suggestion")) 
            {
                hasEditCell = suggestionAdminRights(this._db, suggestionId);
            }
            if (!isHistoryEntry && _db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "THEME", themeID, true, true))
            {
               if( hasEditCell )
               {
                   TableCell editCell = new TableCell();
                   titleRow.Cells.Add(editCell);
                   HyperLink editLink = new HyperLink();
                   editCell.Controls.Add(editLink);
                   editLink.Text = "E";
                   editLink.ToolTip = _mapper.get("edit");
                   editLink.NavigateUrl = psoft.Knowledge.EditTheme.GetURL("themeID",themeID, "backURL",Request.RawUrl, "mode","edit");
                   TableCell separatorCell = new TableCell();
                   titleRow.Cells.Add(separatorCell);
                   Label separatorLabel = new Label();
                   separatorCell.Controls.Add(separatorLabel);
                   separatorLabel.Text = "|";
               }
            {
                //print cell (new)
                TableCell printCell = new TableCell();
                titleRow.Cells.Add(printCell);
                HyperLink printLink = new HyperLink();
                printCell.Controls.Add(printLink);
                printLink.Text = "P";
                printLink.ToolTip = _mapper.get("print");
                printLink.NavigateUrl = psoft.Knowledge.KnowledgeDetail.GetURL("knowledgeID",_knowledgeID,"PRINT",themeID);
                printLink.Target = "_blank";
            }
            }
			
            if (!isHistoryEntry && _db.hasRowAuthorisation(DBData.AUTHORISATION.DELETE, "THEME", themeID, true, true))
            {
                if (hasEditCell)
                {
                    TableCell separatorCell = new TableCell();
                    titleRow.Cells.Add(separatorCell);
                    Label separatorLabel = new Label();
                    separatorCell.Controls.Add(separatorLabel);
                    separatorLabel.Text = "|";
                
                    TableCell deleteCell = new TableCell();
                    titleRow.Cells.Add(deleteCell);
                    HyperLink deleteLink = new HyperLink();
                    deleteCell.Controls.Add(deleteLink);
                    deleteLink.NavigateUrl = "javascript: listDeleteRowConfirm('"+descriptionRow.ClientID+"','"+themeID+"','THEME')";
                    deleteLink.Text = "D";
                    deleteLink.ToolTip = _mapper.get("delete");
                }
            }
        }

        protected void addSlaveCharacteristicTheme(long id)
        {	
            string sqlSlaveChar = "SELECT THEME_ID, SLAVE_ID, CHARACTERISTIC_ID FROM SLAVE_CHARACTERISTIC WHERE  ID = " + id ;
            DataTable result = _db.getDataTable(sqlSlaveChar);
            DataRow slaveData = result.Rows[0];

            string slaveTitle = DBColumn.GetValid(_db.lookup( "TITLE", "SLAVE", " ID = " + slaveData[1]),"");
            string title = "Ergänzungswissen " + DBColumn.GetValid(_db.lookup( "TITLE +' ' + TITLE2 + ' ' + TITLE3 AS TITEL  ", "CHARACTERISTIC", " ID = " + slaveData[2]),"") + " - " + slaveTitle;
			
            string desc =  "";
            long themeId = -1;
            if(!DBColumn.IsNull(slaveData["THEME_ID"]))
            {
                desc    = DBColumn.GetValid(_db.lookup( "DESCRIPTION", "THEME", " ID = " + slaveData["THEME_ID"]),"");
                themeId = DBColumn.GetValid(slaveData["THEME_ID"],-1L);
            }
			
            string number = _autoNumbering.GetNextNumber(0);
            string anchor = number.Replace(".", "_");
            addLink(new ContentsEntry(anchor, number, title));

            // title
            TableRow titleRow = new TableRow();
            tableThemes.Rows.Add(titleRow);
            titleRow.ID = "title_" + anchor;
            TableCell titleCell = new TableCell();
            titleRow.Cells.Add(titleCell);
            titleCell.Width = Unit.Percentage(100);
            HtmlAnchor titleAnchor = new HtmlAnchor();
            titleCell.Controls.Add(titleAnchor);
            titleAnchor.Name = anchor;
            Label titleLabel = new Label();
            titleCell.Controls.Add(titleLabel);
            titleLabel.Text = title;
            titleRow.CssClass = titleLabel.CssClass = titleCell.CssClass = "themeTitle";

            // description
            ArrayList contentsEntries = new ArrayList();
            TableRow descriptionRow = new TableRow();
            tableThemes.Rows.Add(descriptionRow);
            descriptionRow.ID = "desc_" + anchor;
            TableCell descriptionCell = new TableCell();
            descriptionRow.Cells.Add(descriptionCell);
            descriptionCell.ColumnSpan = 4;
            Label descriptionLabel = new Label();
            descriptionCell.Controls.Add(descriptionLabel);
            descriptionLabel.Text = desc;
            descriptionRow.CssClass = descriptionLabel.CssClass = descriptionCell.CssClass = "themeText";
			      
            hasEditCell = (_db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "SLAVE_CHARACTERISTIC", SlaveCharacteristicID, true, true));
                     
            if (hasEditCell)
            {
                TableCell editCell = new TableCell();
                titleRow.Cells.Add(editCell);
                HyperLink editLink = new HyperLink();
                editCell.Controls.Add(editLink);
                editLink.Text = "E";
                editLink.ToolTip = _mapper.get("edit");

                if (!DBColumn.IsNull(slaveData["THEME_ID"]))
                {
                    editLink.NavigateUrl = psoft.Knowledge.EditTheme.GetURL("slaveID", _slaveCharacteristicID, "backURL", Request.RawUrl, "mode", "edit");
                }
                else
                {
                    // themes...
                    long baseThemeID = _db.Knowledge.getBaseThemeID(_knowledgeID);
                    editLink.NavigateUrl = psoft.Knowledge.EditTheme.GetURL("parentThemeID", baseThemeID, "slaveID", _slaveCharacteristicID, "backURL", Request.RawUrl, "mode", "add");
                }

                /*
                TableCell separatorCell = new TableCell();
                titleRow.Cells.Add(separatorCell);
                //Label separatorLabel = new Label();
                separatorCell.Controls.Add(separatorLabel);
                separatorLabel.Text = "|";
            
                
                TableCell deleteCell = new TableCell();
                titleRow.Cells.Add(deleteCell);
                
                HyperLink deleteLink = new HyperLink();
                deleteCell.Controls.Add(deleteLink);
                //deleteLink.NavigateUrl = "javascript: listDeleteRowConfirm('"+descriptionRow.ClientID+"','"+themeID+"','THEME')";
                deleteLink.Text = "D";
                deleteLink.ToolTip = _mapper.get("delete"); */
            }

            if(_knowledgeID == _db.Knowledge.getLatestKnowledgeIdFromHistory(_knowledgeID))
            {
                // Ranking Title
                TableRow rankingRow = new TableRow();
                tableThemes.Rows.Add(rankingRow);
                rankingRow.ID = "ranking_" + anchor;
                TableCell rankingCell = new TableCell();
                rankingRow.Cells.Add(rankingCell);
                rankingCell.ColumnSpan = 4;
                Label rankingLabel = new Label();
                rankingCell.Controls.Add(rankingLabel);
                rankingLabel.Text = "Bewertung";
                rankingRow.CssClass = rankingLabel.CssClass = rankingCell.CssClass = "themeTitle";
					
                createRankingControls(tableThemes);
            }
            addLinks(contentsEntries);
        }
					
		/// <summary>
		/// 
		/// </summary>
		/// <param name="suggestionId">currently non used</param>
        protected void displaySuggestionRating(long suggestionId)
        {
            ArrayList contentsEntries = new ArrayList();
            //string descriptionlabel = _db.Theme.text2HTML("desc", _db, _db.ID2UID(_knowledgeID, Knowledge._TABLENAME), ref _autoNumbering, 1, ref contentsEntries);
            if(_knowledgeID == _db.Knowledge.getLatestKnowledgeIdFromHistory(_knowledgeID))
            {
                // Ranking Title
                
                string number = _autoNumbering.GetNextNumber(0);
                string anchor = number.Replace(".", "_");
                TableRow rankingRow = new TableRow();
                tableThemes.Rows.Add(rankingRow);
                rankingRow.ID = "ranking_" + anchor;
                TableCell rankingCell = new TableCell();
                rankingRow.Cells.Add(rankingCell);
                rankingCell.ColumnSpan = 4;
                Label rankingLabel = new Label();
                rankingCell.Controls.Add(rankingLabel);
                rankingLabel.Text = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_ST_RANKING_TITLE);
                rankingRow.CssClass = rankingLabel.CssClass = rankingCell.CssClass = "themeTitle";
					
                createSuggestionRankingControls(tableThemes);
            }
            addLinks(contentsEntries);
        }

        /// <summary>
        /// This is the ranking for WE in matrixes
        /// </summary>
        /// <param name="tableThemes"></param>
        public void createRankingControls(Table tableThemes)
        {		
            long slaveId = DBColumn.GetValid(_db.lookup("SLAVE_ID","SLAVE_CHARACTERISTIC","ID =" + SlaveCharacteristicID),-1L);					             							
            long matrixID = DBColumn.GetValid(_db.lookup("MATRIX_ID","SLAVE"," ID = " + slaveId),-1L);

            DataTable colTable = _db.getDataTable("select * from COLORATION where " + Matrix._TABLENAME + "_ID=" + matrixID + " order by ORDNUMBER" );
            if(colTable.Rows.Count > 0 )
            {
                TableRow row = new TableRow();             
				
                TableCell rootCell    = new TableCell();				
                rootCell.ColumnSpan = 4;
                row.Cells.Add(rootCell);

                tableThemes.Rows.Add(row);
								
                Table tableRoot = new Table();  
                tableRoot.Width = Unit.Percentage(100);
                tableRoot.BorderWidth = 0;
                rootCell.Controls.Add(tableRoot);
								
                row = new TableRow();             				
                TableCell tableCell    = new TableCell();				
                tableCell.Width = Unit.Percentage(50.0);
                row.Cells.Add(tableCell);
                			
                Table ratingTable = new Table();
                ratingTable.Width = Unit.Percentage(100.0);
                tableCell.Controls.Add(ratingTable);

                tableCell    = new TableCell();
                tableCell.Width = Unit.Percentage(50.0);
                row.Cells.Add(tableCell);

                Table table2 = new Table();
                table2.BorderWidth = 0;
                table2.Width  = Unit.Percentage(100.0);
                table2.Height = Unit.Percentage(100.0);
                tableCell.Controls.Add(table2);
                
                tableRoot.Rows.Add(row);

                long idLatestColoration = DBColumn.GetValid(_db.lookup("COLORATION_ID","COLORATION_HISTORY"," SLAVE_CHARACTERISTIC_ID = " + SlaveCharacteristicID + " order by GUELTIG_AB DESC"),-1L);
												
                string group = "rankingGroup";
                ratingTable.Rows.Clear();
                int radioId = 0;

                bool hasWriteRight = (_db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "SLAVE_CHARACTERISTIC", SlaveCharacteristicID, true, true));

                foreach(DataRow r in colTable.Rows)
                {
                    row = new TableRow();                
                    TableCell radioCell = new TableCell();
                
                    RadioButton radio = new RadioButton();                   
                    radio.GroupName = group;
                    radio.EnableViewState = true;
                    radio.ID = "" + DBColumn.GetValid(r["ID"],-1L);
                    radioCell.Controls.Add(radio);
                    radio.Text = "";
                    if (hasWriteRight)
                    {
                        radio.CheckedChanged += new EventHandler(changedRadioButton);
                    }
                   
                    radio.AutoPostBack = false;
                    radiobuttons.Add(radio);
                    radioId++;
							
                    if(DBColumn.GetValid(r["ID"],-1L) == idLatestColoration)
                        radio.Checked = true;
                    else
                        radio.Checked = false;
				
                    row.Cells.Add(radioCell);
				
                    TableCell cellColor = new TableCell();
                    cellColor.BackColor = _db.Characteristic.getColor(DBColumn.GetValid(r["ID"], -1L));
                    cellColor.ColumnSpan = 1;
                    cellColor.Width = Unit.Pixel(25);
				
                    row.Cells.Add(cellColor);		

                    TableCell cellTitle = new TableCell();
				   
                    cellTitle.Text = DBColumn.GetValid(r["Title"],"");

                    cellTitle.Width = Unit.Percentage(80);			
                    row.Cells.Add(cellTitle);		
									
                    ratingTable.Rows.Add(row);					
                }
			
                row = new TableRow();                
                TableCell buttonCell = new TableCell();
                row.Cells.Add(buttonCell);
             
                if (hasWriteRight)
                {
                    Button b = new Button();
                    b.Text = "Bewerten";
                    buttonCell.Controls.Add(b);
                }
                buttonCell.ColumnSpan = 3;
                ratingTable.Rows.Add(row);

                // Bewertungskommentar
                row = new TableRow();
                TableCell titleCell = new TableCell();
                Label l = new Label();
				
                l.Text = "Bemerkung";                
                titleCell.Width = Unit.Percentage(100.0);
                titleCell.Controls.Add(l); 
                row.Cells.Add(titleCell);

                table2.Rows.Add(row);
				
                row = new TableRow();
                TableCell cellReason = new TableCell();
                cellReason.RowSpan = colTable.Rows.Count;
                    

                string latestReasonText = DBColumn.GetValid(_db.lookup("TOP 1 TITLE","COLORATION_HISTORY"," SLAVE_CHARACTERISTIC_ID = " + SlaveCharacteristicID + " order by GUELTIG_AB DESC"),"");

                reasonTextBox = new TextBox();								
                reasonTextBox.TextMode = TextBoxMode.MultiLine;
                reasonTextBox.Text = latestReasonText;
                reasonTextBox.Rows = 4;
                reasonTextBox.Width = Unit.Percentage(100.0);
                cellReason.Controls.Add(reasonTextBox);						
                cellReason.ColumnSpan = 2;
                cellReason.Enabled = hasWriteRight;

                row.Cells.Add(cellReason);

                table2.Rows.Add(row);

            }
        }

        /// <summary>
        /// This is the ranking for suggestions.
        /// </summary>
        /// <param name="tableThemes"></param>
        private void createSuggestionRankingControls(Table tableThemes)
        {
            ///returns immediately if suggestion module is not enabled
            if(!Global.Config.isModuleEnabled("suggestion")) return;

            //all available stati (this is currently global)
            DataTable colTable = _db.getDataTable("select * from suggestion_execution_stati where suggestion_execution_id = " + this._suggestionExecutionId);

            if(colTable.Rows.Count > 0 )
            {
                TableRow row = new TableRow();             
				
                TableCell rootCell    = new TableCell();				
                rootCell.ColumnSpan = 4;
                row.Cells.Add(rootCell);

                tableThemes.Rows.Add(row);
								
                Table tableRoot = new Table();  
                tableRoot.Width = Unit.Percentage(100);
                tableRoot.BorderWidth = 0;
                rootCell.Controls.Add(tableRoot);
								
                row = new TableRow();             				
                TableCell tableCell    = new TableCell();				
                tableCell.Width = Unit.Percentage(50.0);
                row.Cells.Add(tableCell);
                			
                Table table = new Table();
                table.Width = Unit.Percentage(100.0);
                tableCell.Controls.Add(table);

                tableCell    = new TableCell();
                tableCell.Width = Unit.Percentage(50.0);
                row.Cells.Add(tableCell);

                Table table2 = new Table();
                table2.BorderWidth = 0;
                table2.Width  = Unit.Percentage(100.0);
                table2.Height = Unit.Percentage(100.0);
                tableCell.Controls.Add(table2);
                
                tableRoot.Rows.Add(row);

                string group = "rankingGroup";
                table.Rows.Clear();
                int radioId = 0;			

                //the currently set ranking (color)
                long idLatestColoration = DBColumn.GetValid(_db.lookup("ID","SUGGESTION_EXECUTION_STATI","SUGGESTION_EXECUTION_ID = " + this._suggestionExecutionId + " and CHOSEN = 1"), -1L);

                foreach(DataRow r in colTable.Rows)
                {
                    row = new TableRow();                
                    TableCell radioCell = new TableCell();
                
                    RadioButton radio = new RadioButton();
                    if(!this.suggestionAdminRights(_db, _suggestionExecutionId)) 
                    {
                        radio.Enabled = false;
                    }
                    radio.GroupName = group;
                    radio.EnableViewState = true;
                    radio.ID = "" + DBColumn.GetValid(r["ID"],-1L); //stati id
                    radioCell.Controls.Add(radio);
                    radio.Text = "";
                    radio.CheckedChanged += new EventHandler(changedSuggestionRadioButton);				
                    radio.AutoPostBack = false;
                    radiobuttons.Add(radio);
                    radioId++;
							
                    if((idLatestColoration != -1) && DBColumn.GetValid(r["ID"],-1L) == idLatestColoration)
                        radio.Checked = true;
                    else
                        radio.Checked = false;
				
                    row.Cells.Add(radioCell);
				
                    TableCell cellColor = new TableCell();
                   
                    int color32 = DBColumn.GetValid(r["COLOR"], 0xffffff); //directly read from the table (global)
                    cellColor.BackColor = Color.FromArgb(color32);
                   
                    cellColor.ColumnSpan = 1;
                    cellColor.Width = Unit.Pixel(25);
				
                    row.Cells.Add(cellColor);		

                    TableCell cellTitle = new TableCell();
				   
                    cellTitle.Text = DBColumn.GetValid(r["Title"],"");
            
                    cellTitle.Width = Unit.Percentage(80);			
                    row.Cells.Add(cellTitle);		
									
                    table.Rows.Add(row);					
                }
			
                row = new TableRow();                
                TableCell buttonCell = new TableCell();
                row.Cells.Add(buttonCell);
                Button b = new Button();	
			    
                b.Text = "Bewerten";
                b.Click += new System.EventHandler(ranking_Click);
                if(!this.suggestionAdminRights(_db, _suggestionExecutionId)) 
                {
                    b.Enabled = false;
                }
                buttonCell.Controls.Add(b);
                buttonCell.ColumnSpan = 3;
                table.Rows.Add(row);

                // Bewertungskommentar
                row = new TableRow();
                TableCell titleCell = new TableCell();
                Label l = new Label();
				
                l.Text = "Bemerkung";                
                titleCell.Width = Unit.Percentage(100.0);
                titleCell.Controls.Add(l); 
                row.Cells.Add(titleCell);

                table2.Rows.Add(row);
				
                row = new TableRow();
                TableCell cellReason = new TableCell();
                cellReason.RowSpan = colTable.Rows.Count;
                    

                string latestReasonText = DBColumn.GetValid(_db.lookup("REMARK","SUGGESTION_EXECUTION"," ID = " + this._suggestionExecutionId),"");

                suggestionReasonTextBox = new TextBox();								
                suggestionReasonTextBox.TextMode = TextBoxMode.MultiLine;
                suggestionReasonTextBox.Text = latestReasonText;
                suggestionReasonTextBox.Rows = 4;
                suggestionReasonTextBox.Width = Unit.Percentage(100.0);
                cellReason.Controls.Add(suggestionReasonTextBox);						
                cellReason.ColumnSpan = 2;
                row.Cells.Add(cellReason);
                if(!this.suggestionAdminRights(_db, _suggestionExecutionId)) 
                {
                    suggestionReasonTextBox.Enabled = false;
                }
                table2.Rows.Add(row);
            }
        }

        private void changedRadioButton(object sender, System.EventArgs e) 
        {			
			
            _db.connect();
            try
            {
                DataTable table = _db.getDataTableExt("select GUELTIG_AB,TITLE from COLORATION_HISTORY WHERE ID = 0", "COLORATION_HISTORY");
                RadioButton rb = (RadioButton) sender;
			
                string text = reasonTextBox.Text;
						
                StringBuilder sql = new StringBuilder("INSERT INTO COLORATION_HISTORY(COLORATION_ID, GUELTIG_AB,SLAVE_CHARACTERISTIC_ID,TITLE) VALUES(#1,#2,#3,#4)");		
                sql.Replace("#4",_db.dbColumn.AddToSql("",table.Columns["TITLE"],text));				
                sql.Replace("#2",_db.dbColumn.AddToSql("",table.Columns["GUELTIG_AB"],DateTime.Now.ToString()));
                sql.Replace("#3",""+ SlaveCharacteristicID);
                sql.Replace("#1",rb.ID);	
                long baseThemeID = _db.Knowledge.getBaseThemeID(_knowledgeID);
			
                _db.execute(sql.ToString());		
            }
            catch(Exception ex)
            {
                Logger.Log(ex,Logger.ERROR);
            }
            finally
            {
                _db.disconnect();
                Response.Redirect(this.Request.RawUrl,false);
            }
			
        }
	
        /// <summary>
        /// Called on postback, stores current ranking value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changedSuggestionRadioButton(object sender, System.EventArgs e) 
        {
            if(!Global.Config.isModuleEnabled("suggestion")) return;

            _db.connect();
            string text = suggestionReasonTextBox.Text;

            try
            {
                StringBuilder sqlclear = new StringBuilder("update SUGGESTION_EXECUTION_STATI set CHOSEN = 0 WHERE  SUGGESTION_EXECUTION_ID = #1");
                sqlclear.Replace("#1",this._suggestionExecutionId.ToString()); 

                StringBuilder sql      = new StringBuilder("update SUGGESTION_EXECUTION_STATI set CHOSEN = 1 WHERE  ID = #1");		
                RadioButton rb = (RadioButton) sender;
                sql.Replace("#1",rb.ID); // the id of the selected suggestion_execution_stati has been stored into the radio button id

                //remark set in the click button callback.
                _db.execute(sqlclear.ToString());		
                _db.execute(sql.ToString());	
            }
            catch(Exception ex)
            {
                Logger.Log(ex,Logger.ERROR);
            }
            finally
            {
                _db.disconnect();
                //redirect in the click button callback.
            }
        }



        /// <summary>
        /// Edit cell is activ depending on W.E. type and user identity. 
        /// Suggestions are activ for suggestions responsible only.
        /// </summary>
        /// <returns>true if the edit option must be displayed</returns>
        public bool suggestionAdminRights(DBData db, long suggestionExecutionId)
        {
            //test type
            if(suggestionExecutionId == -1) 
            {
                //not a suggestion entry, can be edited: return true
                return true;
            }       
            int type = ch.psoft.Util.Validate.GetValid(db.lookup("TYP", "PERSON", "ID=" + db.userId, false), 0);
            if( (type & Person.TYP.ADMINISTRATOR) > 0) 
            {
                //administrator.
                return true;
            }
        
            long suggestionId = DBColumn.GetValid(db.lookup("SUGGESTION_ID", "SUGGESTION_EXECUTION","ID=" + suggestionExecutionId), -1L);

            bool isAdministrable = db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "SUGGESTION", suggestionId, true, true);
            bool isModuleRights  = db.hasApplicationAuthorisation(DBData.AUTHORISATION.ADMIN, DBData.APPLICATION_RIGHT.MODULE_SUGGESTION,true);
            return (isAdministrable || isModuleRights);
        }

        private bool isSuggestion()
        {
            return (_suggestionExecutionId != -1);
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
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
		#endregion
    

 
        /// <summary>
        /// Update remark only (used by we for suggestions only)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ranking_Click(object sender, System.EventArgs e) 
        {	
            if(!Global.Config.isModuleEnabled("suggestion")) return;

            _db.connect();
            string text = suggestionReasonTextBox.Text;

            try
            {              
                StringBuilder sqlInsertRemark = new StringBuilder("update SUGGESTION_EXECUTION set REMARK = '#1' where ID = #2");
                sqlInsertRemark.Replace("#1",text);
                sqlInsertRemark.Replace("#2",this._suggestionExecutionId.ToString());
                _db.execute(sqlInsertRemark.ToString());	
            }
            catch(Exception ex)
            {
                Logger.Log(ex,Logger.ERROR);
            }
            finally
            {
                _db.disconnect();
                Response.Redirect(this.Request.RawUrl,false);
            }
        }

    }
}
