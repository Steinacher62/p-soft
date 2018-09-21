/// ===========================================================================
/// Created 16.10.2007
/// =========================================================================== 
namespace ch.appl.psoft.Knowledge.Controls
{
    using ch.psoft.Util;
    using db;
    using Interface.DBObjects;
    using LayoutControls;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    /// List of printable "Themen"
    /// 
    /// </summary>
    public partial  class KnowledgePrintListCtrl : PSOFTSearchListUserControl 
    {

        protected DBData _db = null;


        private String _tablename = "KNOWLEDGE_V"; 

        public String TableName 
        {
            set { _tablename = value; }
        }

        public static string Path 
        {
            get {return Global.Config.baseURL + "/Knowledge/Controls/KnowledgePrintListCtrl.ascx";}
        }

        public class KnowledgeThemeIdPair 
        {
            public KnowledgeThemeIdPair(int knowledgeId, int themeId) 
            {
                this.knowledgeId = knowledgeId;
                this.themeId = themeId;
            }
            public int knowledgeId;
            public int themeId;
        }

        #region Properties
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public string NextURL {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public const string PARAM_QUERY = "PARAM_QUERY";
        public string Query {
            get {return GetString(PARAM_QUERY);}
            set {SetParam(PARAM_QUERY, value);}
        }

        public const string PARAM_KNOWLEDGE_ID = "PARAM_KNOWLEDGE_ID";
        public long KnowledgeID {
            get {return GetLong(PARAM_KNOWLEDGE_ID);}
            set {SetParam(PARAM_KNOWLEDGE_ID, value);}
        }

        public const string LIST_KONTEXT = "LIST_KONTEXT";
        public string ListKontext 
        {
            get {return GetString(LIST_KONTEXT);}
            set {SetParam(LIST_KONTEXT, value);}
        }

        protected System.Web.UI.WebControls.Button printSelection;

        public const string PARAM_KONTEXT = "PARAM_KONTEXT"; 
        public string Kontext {
            get {return GetString(PARAM_KONTEXT);}
            set {SetParam(PARAM_KONTEXT, value);}
        }

        public const string PARAM_X_ID = "PARAM_X_ID";
        public long XID {
            get {return GetLong(PARAM_X_ID);}
            set {SetParam(PARAM_X_ID, value);}
        }

        public const string IS_WISSENELEMENT_GROUPED = "IS_WISSENELEMENT_GROUPED";
        public bool IsWissenElementGrouped 
        {
            get {return GetBool(IS_WISSENELEMENT_GROUPED);}
            set {SetParam(IS_WISSENELEMENT_GROUPED, value);}
        }

        #endregion
    
        public KnowledgePrintListCtrl() : base() {
            HeaderEnabled = true;
            DeleteEnabled = false;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            CheckOrder = true;
            CheckBoxEnabled = true;
            // SEEKIII-239: we lower down the number of rows per page so that 
            // all elements can be displayed without scrolling 
            RowsPerPage = 10; 
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            next.Text = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_BT_PRINT_SELECTION);
            Execute();

        }
        protected override void DoExecute() {
            _db = DBData.getDBData(Session);
            if (Visible)
                loadList();
        }

        private void loadList() {
            _tablename = this.ListKontext;
            _db.connect();
            try {
                string sql = "select * from " + _tablename;
                
                pageTitle.Text = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_BC_PRINT_KNOWLEDGE);

                switch(Kontext){
                    case "search":
                        if (NextURL != ""){
                            next.Visible = true;
                            next.Text = _mapper.get("next"); 
                            CheckBoxEnabled = true;
                        }
                        if (Query != ""){
                            sql = Query;
                        }
                        break;
                }

                if(sql.ToLower().IndexOf("order by") > 0) 
                {
                    sql = sql.Insert(sql.ToLower().IndexOf("order by")+"order by".Length," " + OrderColumn + " " + OrderDir + ",");
                }
                else 
                {
                    sql += " order by " + OrderColumn + " " + OrderDir;
                }
                
                //Using KNOWLEDGE_V, the ID of KNOWLEDGE_V must be returned by the SQL query.
                DataTable table = _db.getDataTableExt(sql,_tablename); 

                if(ListKontext.Equals(Knowledge._VIEWNAME))  // whole wissenelemente 
                {
                    DataTable personTab = _db.Person.getWholeNameMATable(true);
                    table.Columns[_db.langAttrName(Knowledge._VIEWNAME, "CREATOR_PERSON_ID")].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.LIST;
                    table.Columns[_db.langAttrName(Knowledge._VIEWNAME, "CREATOR_PERSON_ID")].ExtendedProperties["In"] = personTab;
                    table.Columns[_db.langAttrName(Knowledge._VIEWNAME, "CREATOR_PERSON_ID")].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%" + _db.langAttrName(Knowledge._VIEWNAME, "CREATOR_PERSON_ID"), "mode","oe");

                    table.Columns[_db.langAttrName(Knowledge._VIEWNAME, "TITLE")].ExtendedProperties["ContextLink"] = psoft.Knowledge.KnowledgeDetail.GetURL("knowledgeID","%K_ID","themeID","%ID");
                    table.Columns[_db.langAttrName(Knowledge._VIEWNAME, "TITLE")].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.LIST;
  
                    if(Global.Config.isModuleEnabled("suggestion")) 
                    {
                        table.Columns["TYPE"].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.LIST + (int) DBColumn.Visibility.INFO;
                        table.Columns["TYPE"].ExtendedProperties["In"] = new System.Collections.ArrayList(_mapper.getEnum("KNOWLEDGE", "types", true));
                        table.Columns["ISACTIVE"].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.LIST + (int) DBColumn.Visibility.INFO;
                        table.Columns["ISACTIVE"].ExtendedProperties["In"] = new System.Collections.ArrayList(_mapper.getEnum("KNOWLEDGE", "isActiveSuggestion", true));
                    }
                }
                else 
                {
                    table.Columns["DESCRIPTION"].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.LIST;
                    DataTable personTab = _db.Person.getWholeNameMATable(true);
                    table.Columns["CREATOR_PERSON_ID"].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.LIST;
                    table.Columns["CREATOR_PERSON_ID"].ExtendedProperties["In"] = personTab;
                    table.Columns["CREATOR_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%" + "CREATOR_PERSON_ID", "mode","oe");
                }
				CheckOrder = true;
                listTable.Rows.Clear();
                LoadList(_db, table, listTable);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        public string lookup(DataColumn col, object id, bool http) {
            string retValue = "";

            if (col != null && !(id is System.DBNull)) {
                switch (col.ColumnName) {
                    case "TRIGGER_UID":
                        retValue = _db.UID2NiceName((long)id,_mapper);
                        break;
                }
            }
            return retValue;
        }


        /// <summary>
        /// Event handler for the 'next' button. In this case a "print" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void next_Click(object sender, System.EventArgs e) 
        {
            DoOnNextClick(sender);

            //fill in the table
            long searchResultID = 0;
            if(ListKontext.Equals(Knowledge._VIEWNAME))  // whole wissenelemente 
            {
                searchResultID = SaveInSearchResult(listTable, Knowledge._VIEWNAME, 0);
            }
            else 
            {
                searchResultID = SaveInSearchResult(listTable, Theme._TABLENAME, 0);
            }

            //read from the table (could also be done directly reading the checkboxes)
            DBData db = DBData.getDBData(Session);

            string fileName = "selection" + DateTime.Now.Ticks + ".doc";
            db.connect();
            try
            {
                if(IsWissenElementGrouped)  // whole wissenelemente 
                {
                    wissenElements(db, searchResultID, fileName);
                } 
                else 
                {
                    themenList(db, searchResultID, fileName);
                }


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
            //InputStream - lesen
            System.IO.FileStream emlStream = null;
            try 
            {
                string path = Global.Config.documentSaveDirectory + "\\" + KnowledgeDetail.PRINT_TARGET_DIRECTORY + "\\" + fileName;         
                emlStream = new System.IO.FileStream(path, System.IO.FileMode.Open);

                //Content-Type setzen
                Response.ContentType = "application/msword";

                //Content-Disposition setzen und Filenam 
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);

                byte [] buffer = new byte [4096];
                int len = 0;
                while ((len = emlStream.Read(buffer,0,buffer.Length)) > 0 )
                {
                    //erst flushen
                    Response.Flush();
                    //rausschreiben
                    Response.OutputStream.Write(buffer,0,len);
                }              
                Response.End();
            }
            catch(Exception ex) 
            {
                Logger.Log(ex, Logger.ERROR);
            }
            finally 
            {
                if(emlStream!=null) 
                {
                    emlStream.Close();
                    emlStream = null;
                }
            }
        }

 
        private void wissenElements(DBData db, long searchResultID, string fileName)
        {
            const string ROW_ID = "ROW_ID";

            Morph.MorphToWord  morphToWord = new Morph.MorphToWord(Morph.MorphToWord.outputFile(fileName, KnowledgeDetail.THEME_ID), db, 0);
            morphToWord.Mapper = this._mapper;

            morphToWord.printIndex();

            string sqlt = "select * from SEARCHRESULT where ID=" + searchResultID;
            DataTable tabt = db.getDataTable(sqlt,Logger.VERBOSE);
            int  N = tabt.Rows.Count;
            for(int k = 0; k < N; k++)
            {
//                long themeId = (long)tabt.Rows[k][ROW_ID];             
//                long rootId = (long)db.lookup("ROOT_ID","THEME","ID="+(themeId));
                long knowledgeid = (long)tabt.Rows[k][ROW_ID];  //(long)db.lookup("ID","KNOWLEDGE",db.langAttrName("KNOWLEDGE", "base_theme_id") + "=" +(rootId));
                morphToWord.OwnerId = db.ID2UID(knowledgeid, Knowledge._TABLENAME);
                printWholeWissenElement(db,morphToWord,knowledgeid);
 
            }
            morphToWord.close();      

        }
        private void printWholeWissenElement(DBData db, Morph.MorphToWord  morphToWord, long knowledgeID)
        {
            //Wissenelement and theme
            Morph.MorphToWord.TitleWikiPair wissenElement = prepareWissenElement(db, knowledgeID);
            morphToWord.printWissenElement(wissenElement);
            morphToWord.printNewLine();
            Morph.MorphToWord.TitleWikiPair[] themenElement = Morph.MorphToWord.prepareThemen(db,knowledgeID);
            morphToWord.printAllThemen(themenElement);
            morphToWord.printNewLine();

            //-----------------------------------------------------------------
            //Document table  
            //TODO: document table has a column in the wrong position.
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = true;
            DetailEnabled = true;
            InfoBoxEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            CheckBoxEnabled = false;
            CheckOrder = true;

            string docSql = "select ID, TITLE, FILENAME, DESCRIPTION, TYP, CREATED from DOCUMENT where KNOWLEDGE_ID=" + knowledgeID + " and TYP=" + ((int)Document.DocType.Document) + " order by TITLE asc";
            DataTable docTable = db.getDataTableExt(docSql, "DOCUMENT");
            docTable.Columns["DESCRIPTION"].ExtendedProperties["Visibility"] = DBColumn.Visibility.LIST | DBColumn.Visibility.INFO;
            docTable.Columns["CREATED"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INFO;
            Table docListTable = new Table();
            DetailEnabled = false;
            InfoBoxEnabled = true;
            if(this.LoadList(db,docTable, docListTable) > 0)
            {
                morphToWord.printTable("Dokumente", docListTable, 2, KnowledgeDetail.DOCTABLE_PROP_PAIR_ARRAY);
            }
            //History table
            string memView =  base.View;
            View = "HISTORY";
            DetailEnabled = true;

            //
            string hystorySql = "select k.* from KNOWLEDGE_V k where k.ID =" + knowledgeID + " or k.HISTORY_ROOT_ID =6945 order by VERSION desc ,CREATED desc";
            DataTable hystoryTable = db.getDataTableExt(hystorySql, "KNOWLEDGE_V");
            DataTable personTab = db.Person.getWholeNameMATable(true);											
            hystoryTable.Columns["TRIGGER_UID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;				
            hystoryTable.Columns[_db.langAttrName(Knowledge._VIEWNAME, "CREATOR_PERSON_ID")].ExtendedProperties["In"] = personTab;	
            Table hystoryListTable = new Table();
            if(this.LoadList(db,hystoryTable, hystoryListTable) > 0)
            {
                morphToWord.printTable("Versionsgeschichte", hystoryListTable, 2, KnowledgeDetail.HISTORYTABLE_PROP_PAIR_ARRAY);
            }
            base.View = memView;
            CheckBoxEnabled = true;

            //-----------------------------------------------------------------
            
            string[] registryIDs = db.Registry.getRegistryIDs(knowledgeID.ToString(), "KNOWLEDGE").Split(',');
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

            morphToWord.printRegistratur("Registratur", regPathsDest, 1);

            morphToWord.pageBreak();
   
        }

        

        private Morph.MorphToWord.TitleWikiPair prepareWissenElement(DBData db, long knowledgeID)
        {
            string ktitle = db.Knowledge.getTitle(knowledgeID);
            string kdes = db.Knowledge.getDescription(knowledgeID);
            return new Morph.MorphToWord.TitleWikiPair(ktitle,kdes);
        }

        private void themenList(DBData db, long searchResultID, string fileName)
        {
            const string ROW_ID = "ROW_ID";

            Morph.MorphToWord  morphToWord = new Morph.MorphToWord(Morph.MorphToWord.outputFile(fileName, KnowledgeDetail.PRINT_TARGET_DIRECTORY), db, 0);
            morphToWord.Mapper = this._mapper;

            string sqlt = "select * from SEARCHRESULT where ID=" + searchResultID;
            DataTable tabt = db.getDataTable(sqlt,Logger.VERBOSE);
            int  N = tabt.Rows.Count;
            long curKnowledgeid = 0;
            for(int k = 0; k < N; k++)
            {
                long themeId = (long)tabt.Rows[k][ROW_ID];             
                long rootId = (long)db.lookup("ROOT_ID","THEME","ID="+(themeId));
                long knowledgeid = (long)db.lookup("ID","KNOWLEDGE",db.langAttrName("KNOWLEDGE", "base_theme_id") + "=" +(rootId));
                if(curKnowledgeid == 0 || curKnowledgeid != knowledgeid) 
                {
                    //new knowledge id: handle knowledge
                    if(curKnowledgeid!=0) 
                    {
                        morphToWord.pageBreak();
                    }
                    string knowledgeTitle = (string)db.lookup(db.langAttrName("KNOWLEDGE", "TITLE"),"KNOWLEDGE_V",db.langAttrName("KNOWLEDGE", "base_theme_id") + "=" +(rootId));
                    //morphToWord.printParagraph(new iTextSharp.text.Paragraph("<" + knowledgeTitle + ">"));
                    curKnowledgeid = knowledgeid;
                }
                if(rootId == themeId) //this is a Wissenelement root Theme
                {
                    //handle root element if necessary
                }
                morphToWord.OwnerId = db.ID2UID(knowledgeid, Knowledge._TABLENAME);
                Morph.MorphToWord.TitleWikiPair theme = Morph.MorphToWord.prepareTheme(db,(int)themeId);
                morphToWord.printWiki(theme,1);
            }
            morphToWord.close();      
        }

        protected override void onAddHeaderCell (DataRow row, DataColumn col, TableRow r, TableCell cell)
        {
            if (col != null)
            {
                if (col.ColumnName.Equals("DESCRIPTION")) 
                {
                    cell.Width=200;
                    cell.Text = _mapper.get("knowledge", "thColumnKnowledgeTitle");
                }
            }
        }


		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

  
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion


    }
}
