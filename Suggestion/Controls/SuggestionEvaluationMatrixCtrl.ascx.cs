namespace ch.appl.psoft.Suggestion.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Collections;
    using System.Data;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml;

    public partial class SuggestionEvaluationMatrixCtrl : PSOFTListViewUserControl {

        const string  LANG_MNEMO_DESCR      = "ttMatrixTitle";
        const string  LANG_MNEMO_CREATION   = "ttMatrixDate";
        const string  LANG_MNEMO_RATING     = "ttMatrixRating";
        const string  LANG_MNEMO_REMARK     = "ttMatrixRemark";

        /// <summary>
        /// Represents a matrix elements, matrix data must be first stored in this 
        /// internal representation before being output.
        /// </summary>
        class MatrixElement 
        {
            
            /// <summary>
            /// Implicit conversion operator to convert this class in an excel item.
            /// </summary>
            /// <param name="typ"></param>
            /// <returns></returns>
            public static implicit operator MatrixReportInterface.Cell(MatrixElement typ) 
            {
                if (typ.mtype == Type.TITLE)
                {
                    return new MatrixReportInterface.Cell(typ.text, true, typ.color);
                }
                else
                {
                    return new MatrixReportInterface.Cell(typ.text, false, typ.color);
                }
            }

            public enum Type 
            {
                VALUE,
                TITLE,
                NULL
            }

            public enum DetailRedirection
            {
                SUGGESTION_DETAIL,
                KNOWLEDGE_DETAIL
            }

            public MatrixElement() 
            {
                this.mtype = Type.NULL; //null value
            }

            public MatrixElement(string text, Color color, long suggestionId, long suggestionExecutionId, string tooltyp) 
            {
                this.text = text;
                this.color = color;
                this.suggestionID = suggestionId;
                this.suggestionExecutionId = suggestionExecutionId;
                this.ToolTyp = tooltyp;
                this.mtype = Type.VALUE;
                
            }
            public MatrixElement(string text) 
            {
                this.text = text;
                this.color = Color.White;
                this.mtype = Type.TITLE;
            }

            /// <summary>
            /// represents the link of the current matrix element. 
            /// </summary>
            /// <returns></returns>
            public string toUrlLink(DetailRedirection detailType)  
            {
                if(detailType == DetailRedirection.SUGGESTION_DETAIL) 
                {
                    return toSuggestionDetailLink();
                } 
                else 
                {
                    return toKnowledgeDetailLink();
                }
            }

            private string toKnowledgeDetailLink() 
            {
                return ch.appl.psoft.Knowledge.KnowledgeDetail.GetURL("context","history","suggestionExecutionID",this.suggestionExecutionId);
            }

            private string toSuggestionDetailLink() 
            {
                return ch.appl.psoft.Suggestion.SuggestionDetail.GetURL("suggestionID",suggestionID,"executionID",this.suggestionExecutionId);
            }

            public bool isNull() 
            {
                return this.mtype == Type.NULL; 
            }

            public string ToolTyp { get; private set; }

            public string text;
            public Color color = Color.White;

            public MatrixElement.Type mtype = Type.VALUE;

            private long suggestionID;
            private long suggestionExecutionId;
            

        }

        /// <summary>
        /// List containing all matrix elements. 
        /// </summary>
        private ArrayList matrixElements = new ArrayList();
        /// <summary>
        /// The number of column of a table.
        /// </summary>
        private int numberOfExecutions;

        /// <summary>
        /// 
        /// </summary>
        private int numberOfPeople;

//        int colorationCounter = 0;
        string suggestionTitle = "";


        public const string CSS_CLASS_ID_INFO = "suggestionInfo";

		
        protected DBData _db = null;

        
        public SuggestionEvaluationMatrixCtrl()
        {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = true;
            DetailEnabled = true;
            InfoBoxEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;            
        }

        public static string Path {
            get {return Global.Config.baseURL + "/Suggestion/Controls/SuggestionEvaluationMatrixCtrl.ascx";}
        }

        #region Properties

        private long _suggestionId = -1;
        public long SuggestionID
        {
            get {return _suggestionId;}
            set {_suggestionId = value;}
        }

        public LanguageMapper Mapper 
        {
            get {return _mapper;}
            set {_mapper = value;}
        }

        private long _slaveId = -1;
        public long SlaveID
        {
            get {return _slaveId;}
            set {_slaveId = value;}
        }



        #endregion

        private Color dimensionColor = Color.White; 

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();
			_db = DBData.getDBData(Session);
			_db.connect();

            suggestiondescription.Text = DBColumn.GetValid(_db.lookup(_db.langAttrName("SUGGESTION", "DESCRIPTION"),"SUGGESTION","ID = " + this._suggestionId), ""); 

			if(Request.Params.Get("PRINT") == null) 
			{
				excelButton.Visible = true;
				excelButton.ImageUrl = "../../images/icon_excel.gif";
				excelButton.ToolTip = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, "btnGenerateExcelFile");
				string paramString =  Request.QueryString + "&PRINT=1";
				excelButton.Attributes.Add("onClick", "javascript: window.open('" + Request.Path + "?" + paramString + "','_blank')");

					if(SuggestionID <= 0) 
					{
						//currently no active suggestions
						TableRow r = new TableRow();
						TableCell c = new TableCell();
						r.Cells.Add(c);
						matrixTable.Rows.Add(r);

						Label l = new System.Web.UI.WebControls.Label();
						l.Text = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_NO_ACTIVE_SUGGESTION_FOUND);
						l.CssClass = CSS_CLASS_ID_INFO;
						c.Controls.Add(l);
                        this.excelButton.Visible = false;
						return; 
					}				

                    if(fillInData() == 0) 
                    {
                        //currently no active suggestions
                        TableRow r = new TableRow();
                        TableCell c = new TableCell();
                        r.Cells.Add(c);
                        matrixTable.Rows.Add(r);

                        Label l = new System.Web.UI.WebControls.Label();
                        l.Text = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, SuggestionModule.LANG_MNEMO_NO_ACTIVE_SUBMITTED_EXECUTION_FOUND);
                        l.CssClass = CSS_CLASS_ID_INFO;
                        c.Controls.Add(l);
                        this.excelButton.Visible = false;
                        return; 
                    }
                    else 
                    {
                        printOutHtmlTable();
                    }
			
			} 
			else 
			{
                this.suggestionTitle = DBColumn.GetValid(_db.lookup(_db.langAttrName("SUGGESTION", "TITLE"),"SUGGESTION","ID = " + this._suggestionId), ""); 
                
                XmlDocument overview = getOverviewAsXML(this.suggestionTitle);
                XSLTransformer transformer = new XSLTransformer(SuggestionModule.excelstylesheet, SuggestionModule.debugXML);
                string outputfile_relative = Global.Config.getCommonSetting("tmpdir", DefaultValues.TmpDirectory) + this.suggestionTitle + "_" + DateTime.Now.ToString("yyMMdd") + ".xls";
                string outputfile_absolute = AppDomain.CurrentDomain.BaseDirectory + outputfile_relative;
                transformer.transform(overview, outputfile_absolute);

                Response.ContentType = "application/vnd.ms-excel";
                Response.Redirect(Global.Config.baseURL + outputfile_relative, false);
			}
        }

        /// <summary>
        /// 
        /// </summary>
        private int fillInData() 
        {
            int numberOfRows = 0;
            try 
            {
                this.numberOfPeople = DBColumn.GetValid(_db.lookup("count (distinct PERSON_ID)","SUGGESTION_EXECUTION","ISFINISHED = 2 and SUGGESTION_ID in (select ID from SUGGESTION where ISACTIVE = 1)"), 0);
                numberOfRows = this.numberOfPeople;

                string sqlPersonIds = "select PERSON_ID from SUGGESTION_EXECUTION where ISFINISHED = 2 and SUGGESTION_ID in (select ID from SUGGESTION where ISACTIVE = 1) group by PERSON_ID";
                DataTable personIds = _db.getDataTable(sqlPersonIds); //personId
	
                string restriction = "PERSON_ID = #1 and ISFINISHED = 2 and SUGGESTION_ID in (select ID from SUGGESTION where ISACTIVE = 1)";
                foreach(DataRow row in personIds.Rows) 
                {
                    int nr = DBColumn.GetValid(_db.lookup("count (*)", "SUGGESTION_EXECUTION",restriction.Replace("#1",""+row[0])), 0);

                    if(nr > this.numberOfExecutions) 
                    {
                        this.numberOfExecutions = nr;
                    }
                }

               
                foreach(DataRow row in personIds.Rows) 
                {
                    //find out author
                    string author = DBColumn.GetValid(_db.lookup("FIRSTNAME","PERSON","ID = " + row[0]),"");
                    author += " " + DBColumn.GetValid(_db.lookup("PNAME","PERSON","ID = " + row[0]),"");
                     
                    this.matrixElements.Add(new MatrixElement(author));
                    string sqlex = "select * from SUGGESTION_EXECUTION where " + restriction.Replace("#1",""+row[0]) + " order by CREATED";
                    DataTable executions = _db.getDataTable(sqlex);
                    int nrex = 0;
                    foreach(DataRow rowex in executions.Rows) 
                    {
                        int coloration = DBColumn.GetValid(_db.lookup("COLOR","SUGGESTION_EXECUTION_STATI","SUGGESTION_EXECUTION_ID = " + rowex["ID"] + " and CHOSEN = 1"), -1);    
                        string rating  = DBColumn.GetValid(_db.lookup("TITLE","SUGGESTION_EXECUTION_STATI","SUGGESTION_EXECUTION_ID = " + rowex["ID"] + " and CHOSEN = 1"), "");          

                        Color cellcolor = coloration==-1?Color.White:Color.FromArgb(coloration);
                        string tooltyp = createTooltyp(rowex, rating);
                        this.matrixElements.Add(new MatrixElement((string)rowex["TITLE"], cellcolor, this.SuggestionID, (long)rowex["ID"], tooltyp));
                        nrex++;
                    }
                    for(int k = nrex; k < this.numberOfExecutions; k++) 
                    {
                        this.matrixElements.Add(new MatrixElement());
                    }
                }
                this.suggestionTitle = DBColumn.GetValid(_db.lookup(_db.langAttrName("SUGGESTION", "TITLE"),"SUGGESTION","ID = " + this._suggestionId), ""); 
            }
            catch (Exception ex) 
            {
                DoOnException(ex);
            }               
            finally 
            {
                _db.disconnect();
            }
            return numberOfRows;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowex"></param>
        /// <param name="rating"></param>
        /// <returns></returns>
        private string createTooltyp(DataRow rowex, string rating)
        {
            string descr_s =    _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, LANG_MNEMO_DESCR);
            string creation_s = _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, LANG_MNEMO_CREATION);
            string rating_s =   _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, LANG_MNEMO_RATING);
            string remark_s =   _mapper.get(SuggestionModule.LANG_SCOPE_SUGGESTION, LANG_MNEMO_REMARK);
            
            string created = rowex["CREATED"].ToString();
            created = created.Substring(0,created.IndexOf(" ", 0, created.Length));

            string tooltyp = descr_s + ": " + rowex["TITLE"] + "\n" + creation_s + ": " + created + "\n" + rating_s + ": " + rating + "\n" + remark_s + ": " + rowex["REMARK"];

            return tooltyp;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void printOutHtmlTable()
        {
            int rows =  this.numberOfExecutions + 1;
            int columns = this.numberOfPeople;
            const int SPACE_WIDTH = 1;
            const int CELL_PADDING = 2;

            matrixTable.BorderStyle = BorderStyle.Solid;
            
            matrixTable.BorderWidth = 2;
            matrixTable.BorderColor = Color.Gray;
            matrixTable.CellSpacing = 0;
            matrixTable.CellPadding = 0;

            //header
            // header(rows, SPACE_WIDTH);
                    
            //content                 
            for (int i = 0; i < columns; i++)// MatrixElement elm in this.matrixElements ) 
            {
                TableRow row = new TableRow();
                matrixTable.Rows.Add(row);
                matrixTable.CellPadding = CELL_PADDING;
                for(int k = 0; k < rows; k++) 
                {
                    MatrixElement melm = ((MatrixElement)matrixElements[i*rows + k]);
                    TableCell cell = new TableCell();
                    

                    if(melm.mtype == MatrixElement.Type.VALUE) 
                    {
                        HyperLink link = new HyperLink();            
                        link.Text = melm.text;    
                        link.CssClass = "Characteristic";
                        link.NavigateUrl = melm.toUrlLink(MatrixElement.DetailRedirection.KNOWLEDGE_DETAIL);
                        cell.Controls.Add(link);
                        cell.ToolTip = melm.ToolTyp;
                    }
                    else 
                    {
                        cell.Text = melm.text;
                        cell.Font.Bold = true;
                    }
                    cell.BackColor = melm.color;
                    
                    
                    cell.BorderWidth = 1;
                    cell.BorderColor = Color.Gray;			
                    cell.CssClass = "Characteristic";
                    row.Cells.Add(cell);

                    if(k==0) 
                    {
                        //add space 
                        TableCell spacingCell = new TableCell();
                        
                        spacingCell.Width =  Unit.Pixel(SPACE_WIDTH);
                        //spacingCell.Text = " ";
                        spacingCell.BorderStyle = BorderStyle.None;
                        spacingCell.ID = "border" + matrixTable.Rows.GetRowIndex(row);
                        row.Cells.Add(spacingCell);
                    }
                }
            }



            //addColorationLegend(_suggestionId);

        }

        /// <summary>
        /// Matrix header inclusive separation. Deprecated.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="spacew"></param>
        private void header(int rows, int spacew)
        {
            //header
            TableRow rowheader = new TableRow();
            //rowheader.Width = 5;
            matrixTable.Rows.Add(rowheader);
            for(int k = 0; k < rows; k++) 
            {
                TableCell cell = new TableCell();
                if(k==0) 
                {
                    cell.Text = "Person";
                }
                else 
                {
                    cell.Text = "Ausführung " + k;
                }
                cell.Font.Bold = true;
                cell.HorizontalAlign = HorizontalAlign.Center;
                                
                cell.BorderWidth = 1;
                cell.BorderColor = Color.Gray;			
                cell.CssClass = "Characteristic";
                rowheader.Cells.Add(cell);
                if(k==0) 
                {
                    //add space       
                    TableCell spacingCell = new TableCell();
                    spacingCell.Width =  Unit.Pixel(spacew);
                    //spacingCell.Text = " ";
                    spacingCell.BorderStyle = BorderStyle.None;
                    spacingCell.ID = "border" + matrixTable.Rows.GetRowIndex(rowheader);          
                    rowheader.Cells.Add(spacingCell);
                }
            }
          //  add border cells
            TableRow hborder = new TableRow(); 
            hborder.Height =  Unit.Pixel(5); 
            hborder.BorderStyle = BorderStyle.None;
            //hborder.ID = "border" + matrixTable.Rows.GetRowIndex(rowheader);
            matrixTable.Rows.AddAt(1,hborder);
            for(int celk = 0;celk<rows+2; celk++) 
            {   
                hborder.Cells.Add(new TableCell());
            }
            
//            labelDescription.Text = ""; //"Anzahl Durchführungen: " + totnrexe;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrixID"></param>
/*        private void addColorationLegend(long matrixID)
        {
			//the current coloration is considered. any execution may have a different coloration			
            DataTable colTable = _db.getDataTable("select * from SUGGESTION_STATI where " + "SUGGESTION_ID=" + this.SuggestionID + " order by ORDNUMBER" );

            if(colTable.Rows.Count > 0)
            {
                TableRow colorationRow = new TableRow();
                colorationTable.Rows.Add(colorationRow);
			
                TableCell colorationCell = new TableCell();
							
                colorationCell.BorderWidth = 0;
                colorationCell.BorderColor = Color.Gray;			
                colorationCell.Text = "Legende"; //TODO use XML file
                colorationCell.Font.Bold = true;
                colorationRow.Cells.Add(colorationCell);
                colorationCell.ColumnSpan = 2;			        
                colorationTable.CellSpacing = 2;

                colorationCounter = 0;
                foreach (DataRow row in colTable.Rows)
                {				
                    addColorationVertical(colorationCounter,row,DBColumn.GetValid(row["ID"], -1L));
                    colorationCounter++;
                }
            }
            else
            {
                colorationTable.Visible = false;
            }
        }
*/		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="dataRow"></param>
        /// <param name="colorationID"></param>
/*        private void addColorationVertical(int counter, DataRow dataRow, long colorationID)
        {	
            TableRow row = null;
            if(colorationTable.Rows.Count < 4)
            {
                row = new TableRow();
                colorationTable.Rows.Add(row);
            }
            else
            {
                row = colorationTable.Rows[(counter % 3) + 1];
            }
			
            TableCell colorationCell = new TableCell();
            colorationCell.BorderStyle = BorderStyle.Solid;
            colorationCell.BorderWidth = 0;
            colorationCell.BorderColor = Color.Gray;
            colorationCell.Attributes.Add("width","25");
            colorationCell.Attributes.Add("height","12");
		
            int coloration = (int)dataRow["COLOR"];
            
            colorationCell.BackColor = Color.FromArgb(coloration);
			
            TableCell textCell = new TableCell();
            textCell.BorderWidth = 0;
            textCell.BorderColor = Color.Gray;
            textCell.Text =  DBColumn.GetValid(dataRow["Title"],"");
	
            row.Cells.Add(colorationCell);
            row.Cells.Add(textCell);
        }
*/

        protected XmlDocument getOverviewAsXML(string title)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("suggestions");
            root.SetAttribute("name", title);

            try
            {
                string restriction = "PERSON_ID = #1 and ISFINISHED = 2 and SUGGESTION_ID in (select ID from SUGGESTION where ISACTIVE = 1)";

                string sqlPersonIds = "select PERSON_ID from SUGGESTION_EXECUTION where ISFINISHED = 2 and SUGGESTION_ID in (select ID from SUGGESTION where ISACTIVE = 1) group by PERSON_ID";
                DataTable personIds = _db.getDataTable(sqlPersonIds); //personId


                foreach (DataRow row in personIds.Rows)
                {
                    //find out author
                    string authorname = DBColumn.GetValid(_db.lookup("FIRSTNAME", "PERSON", "ID = " + row[0]), "");
                    authorname += " " + DBColumn.GetValid(_db.lookup("PNAME", "PERSON", "ID = " + row[0]), "");

                    //add Author to XML-Document
                    XmlElement author = xmlDoc.CreateElement("author");
                    author.SetAttribute("name", authorname);
                    
                    string sqlex = "select * from SUGGESTION_EXECUTION where " + restriction.Replace("#1", "" + row[0]) + " order by CREATED";
                    DataTable executions = _db.getDataTable(sqlex);
                    foreach (DataRow rowex in executions.Rows)
                    {
                        int coloration = DBColumn.GetValid(_db.lookup("COLOR", "SUGGESTION_EXECUTION_STATI", "SUGGESTION_EXECUTION_ID = " + rowex["ID"] + " and CHOSEN = 1"), -1);
                        string suggestiontitle = DBColumn.GetValid(_db.lookup("TITLE", "SUGGESTION_EXECUTION", "ID = " + rowex["ID"]), "default");

                        Color cellcolor = coloration == -1 ? Color.White : Color.FromArgb(coloration);

                        XmlElement suggestion = xmlDoc.CreateElement("suggestion");
                        suggestion.InnerText = suggestiontitle;
                        suggestion.SetAttribute("color", PSOFTConvert.ColorToHex(cellcolor));
                        author.AppendChild(suggestion);
                    }

                    root.AppendChild(author);
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

            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
            xmlDoc.AppendChild(root);

            /*
            string outputDirectory = Request.MapPath("~" + ch.appl.psoft.Report.ReportModule.REPORTS_DIRECTORY);
            string _filename = "suggestion.xml";
            string _path = outputDirectory + "/" + _filename;
            xmlDoc.Save(_path);

            string _url = Global.Config.baseURL + ch.appl.psoft.Report.ReportModule.REPORTS_DIRECTORY + "/" + _filename;

            return _url;
             */

            return xmlDoc;

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

        protected void legend_OnDataBound(Object sender, EventArgs e)
        {
            GridView gv = (GridView)sender;
            
            if (gv.HeaderRow != null)
            {
                gv.HeaderRow.Cells[0].Text = _mapper.get("SUGGESTION_STATI", "COLOR");
                gv.HeaderRow.Cells[1].Text = _mapper.get("SUGGESTION_STATI", "TITLE");
            }
            else
            {
                btnInfo.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
        }

        protected void btnInfo_OnLoad(Object sender, EventArgs e)
        {
            LinkButton lb = (LinkButton)sender;
            lb.Text = _mapper.get("suggestion_execution", "legend");
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }

        protected int getRandom()
        {
            return new Random().Next(-100,100);
        }
		#endregion
    }
}
