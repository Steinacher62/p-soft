using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Collections;
using Telerik.Web.Spreadsheet;


namespace ch.appl.psoft.GFK
{
    public partial class FormelEditorZelle : PsoftEditPage
    {
        protected long _contextID = -1;
        protected long _journalID = -1;
        protected string _backURL = "";

        #region Protected overridden methods from parent class
        protected override void Initialize()
        {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _contextID = ch.psoft.Util.Validate.GetValid(Request.QueryString["contextID"], -1);
            _journalID = ch.psoft.Util.Validate.GetValid(Request.QueryString["journalID"], _journalID);
            _backURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["backURL"], _backURL);
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            // Setting main page layout

            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl"); ;
            PsoftPageLayout.PageTitle = BreadcrumbCaption = "GFK Formeleditor";

            // rowId
            int cellId = int.Parse(Request.QueryString["Cell_ID"]);

            if (FileNameBox.Text == "" && ExcelFileBox.Text == "")
            {


                // get data from db
                CellData cellData;
                cellData = new CellData(cellId, Session);
                
                // get filename
                string filename = GetFileName();

                FileNameBox.Text = filename;
                ExcelFileBox.Text = filename;

                string filename2 = GetFileName2();

                FileNameBox0.Text = filename2;
                ExcelFileBox0.Text = filename2;

                // save rowdata in session
                Session.Add("CellData", cellData);

                // write to form
                if (cellData.secondTableEnabled)
                {
                    if (cellData.secondIsQVW)
                    {
                        QVWFileData qvwData = new QVWFileData(FileNameBox0.Text);
                        Session.Add("SecondQVWFileData", qvwData);
                    }
                    else
                    {
                        ApplyExcelFile2_Click(null, null);
                        ApplyTablenames2_Click(null, null);
                    }
                }
                if (filename != "" && cellData.product != "" && cellData.isQvw)
                {

                    QVWFileData qvwData = new QVWFileData(FileNameBox.Text);
                    Session.Add("QVWFileData", qvwData);
                    WriteToForm(cellData);
                }
                else if (filename != "" && cellData.excelSheet != "" && !cellData.isQvw)
                {
                    ApplyExcelFile_Click(null, null);
                    ApplyTablenames_Click(null, null);
                    WriteToForm(cellData);
                }

            }
        }

        // toggle second Value
        protected void EnableSecondValue_CheckedChanged(object sender, EventArgs e)
        {
            // enable input for second value
            FormulaBox2.Enabled = EnableSecondValue.Checked;
            FieldsList2.Enabled = EnableSecondValue.Checked;
            UnitBox2.Enabled = EnableSecondValue.Checked;
            

        }



        // get xml data and bind columnnames to form
        //void GetXmlData(string filename, string tablename)
        //{
        //    if (filename != "" && tablename != "")
        //    {
        //        String xmlPath = Global.Config.getModuleParam("gfk", "GFKDataSourcePath", "");

        //        // get latest file
        //        var directory = new DirectoryInfo(xmlPath);
        //        var xmlFile = (from f in directory.GetFiles(FileNameBox.Text + "*") orderby f.LastWriteTime descending select f).First();

        //        xmlPath = xmlFile.FullName;


        //        XmlData xmlData;

        //        xmlData = new XmlData(xmlPath, tablename);            // load Data from Xml
        //        Session.Remove("xmlData");
        //        Session.Add("xmlData", xmlData);                   // save xmlData in session

        //        string[] columnNames;
        //        columnNames = xmlData.getColumnnames();            // get column names

        //        FieldsList1.DataSource = columnNames;
        //        FieldsList1.DataBind();
        //        FieldsList2.DataSource = columnNames;
        //        FieldsList2.DataBind();
        //    }
        //}

        // add fieldname to formula 1
        protected void AddToFormula1_Click(object sender, EventArgs e)
        {
            FormulaBox1.Text += FieldsList1.SelectedValue;
        }

        // add fieldname to formula 2
        protected void AddToFormula2_Click(object sender, EventArgs e)
        {
            FormulaBox2.Text += FieldsList2.SelectedValue;
        }

        // save
        protected void OkButton_Click(object sender, EventArgs e)
        {
            if (!RequieredFieldsFull())
            {
                OutputLabel.Text = "Please fill in all required data";
            }
            else
            {
                OutputLabel.Text = "";
                // get rowdata
               CellData cellData = (CellData)Session["CellData"];

                string filename;
                cellData.cellID = int.Parse(Request.QueryString["Cell_ID"]);
                cellData.isQvw = WebTab1.SelectedIndex == 0;

                if (cellData.isQvw)
                {
                    filename = FileNameBox.Text;
                }
                else
                {
                    filename = ExcelFileBox.Text;
                }

                string filename2;

                cellData.secondIsQVW = WebTab2.SelectedIndex == 0;
                cellData.previousYearEnabled = PreviousYearBox.Checked;
                cellData.secondTableEnabled = SecondTableEnabledBox.Checked;



                if (cellData.secondIsQVW)
                {
                    filename2 = FileNameBox0.Text;
                }
                else
                {
                    filename2 = ExcelFileBox0.Text;
                }

                cellData.excelSheet = ExcelSheetList.SelectedValue;
                cellData.title1 = Title1Box.Text;
                cellData.title2 = Title2Box.Text;

                cellData.reportTyp = ReportTypList.SelectedValue;
                cellData.product = ProductsList.SelectedValue;
                cellData.segment = SegmentsList.SelectedValue;
                cellData.fact = FactsList.SelectedValue;

                cellData.rowName = RowNameBox.Text;

                cellData.excelSheet2 = ExcelSheetList0.SelectedValue;
                cellData.title12 = Title1Box0.Text;
                cellData.title22 = Title2Box0.Text;

                cellData.reportTyp2 = ReportTypList0.SelectedValue;
                cellData.product2 = ProductsList0.SelectedValue;
                cellData.segment2 = SegmentsList0.SelectedValue;
                cellData.fact2 = FactsList0.SelectedValue;

                cellData.rowName2 = RowNameBox0.Text;

                // formula Input
                cellData.unit1 = UnitBox1.Text;
                cellData.unit2 = UnitBox2.Text;

                cellData.secondValueEnabled = EnableSecondValue.Checked;
            
                // color inputs
                cellData.highestValueIsGreen = HighestIsGreen.Checked;
                cellData.borderGreenLightgreen = greenLightgreenBox.Text;
                cellData.borderLightgreenYellow = lightgreenYellowBox.Text;
                cellData.borderYellowOrange = yellowOrangeBox.Text;
                cellData.borderOrangeRed = orangeRedBox.Text;
                cellData.colorationOnSecondValue = ColorationOnSecondValueBox.Checked;
                // rewrite formulas
                String[] columnNames = GetColumnNames();

                cellData.formula1 = FormulaBox1.Text;
                cellData.formula2 = FormulaBox2.Text;              

                for (int columnNr = 0; columnNr < columnNames.Length; columnNr++)
                {
                    cellData.formula1 = cellData.formula1.Replace(columnNames[columnNr], "[1." + columnNr + "]");
                    if (EnableSecondValue.Checked)
                    {
                        cellData.formula2 = cellData.formula2.Replace(columnNames[columnNr], "[1." + columnNr + "]");
                    }
                   
                }
                if (cellData.secondTableEnabled)
                {
                    columnNames = GetColumnNames2();
                    for (int columnNr = 0; columnNr < columnNames.Length; columnNr++)
                    {
                        cellData.formula1 = cellData.formula1.Replace(columnNames[columnNr], "[2." + columnNr + "]");
                        if (EnableSecondValue.Checked)
                        {
                            cellData.formula2 = cellData.formula2.Replace(columnNames[columnNr], "[2." + columnNr + "]");
                        }

                    }
                }

                // save filename to db

                DBData db = DBData.getDBData(Session);
                db.connect();
                string matrixId = db.lookup("MATRIX.ID", "MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN CHARACTERISTIC ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID", "CHARACTERISTIC.ID = " + Request.QueryString["Cell_ID"]).ToString();
                db.execute("UPDATE MATRIX SET GFK_FILE_NAME ='" + filename + "' , SECOND_FILE_NAME ='"+filename2+"' WHERE ID =" + matrixId);
                db.disconnect();

                // write rowData to db
                cellData.WriteToDB();
                Response.Redirect(Global.Config.baseURL + "/GFK/SaveSuccessful.aspx");
            }
        }

        // check if all requiered fields are filled
        Boolean RequieredFieldsFull()
        {
            bool secondTable = (!SecondTableEnabledBox.Checked || (WebTab2.SelectedIndex == 0 && ReportTypList0.SelectedValue != "" && ProductsList0.SelectedValue != "" && SegmentsList0.SelectedValue != "" && FactsList0.SelectedValue != "" && FileNameBox0.Text != "") || (WebTab2.SelectedIndex == 1 && ExcelFileBox0.Text != "" && ExcelSheetList0.SelectedValue != "" && Title1Box0.Text != "" && Title2Box0.Text != ""));
            bool tableSelection = (WebTab1.SelectedIndex == 0 && ReportTypList.SelectedValue != "" && ProductsList.SelectedValue != "" && SegmentsList.SelectedValue != "" && FactsList.SelectedValue != "" && FileNameBox.Text != "") || (WebTab1.SelectedIndex == 1 && ExcelFileBox.Text != "" && ExcelSheetList.SelectedValue != "" && Title1Box.Text != "" && Title2Box.Text != "");
            return (secondTable && tableSelection && (FormulaBox1.Text != "") && ((greenLightgreenBox.Text != "" && lightgreenYellowBox.Text != "" && yellowOrangeBox.Text != "" && orangeRedBox.Text != "") || (greenLightgreenBox.Text == "" && lightgreenYellowBox.Text == "" && yellowOrangeBox.Text == "" && orangeRedBox.Text == "")) && (!EnableSecondValue.Checked || FormulaBox2.Text != ""));
        }

        // get filename from db
        string GetFileName()
        {
            // get rowId
          int cellID = int.Parse(Request.QueryString["Cell_ID"]);

            // get filename from db
            DBData db = DBData.getDBData(Session);
            db.connect();
            string fileName = db.lookup("GFK_FILE_NAME", "MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID", "CHARACTERISTIC.ID = " + cellID).ToString();
            db.disconnect();

            return fileName;
        }

        string GetFileName2()
        {
            // get rowId
            int cellID = int.Parse(Request.QueryString["Cell_ID"]);

            // get filename from db
            DBData db = DBData.getDBData(Session);
            db.connect();
            string fileName = db.lookup("SECOND_FILE_NAME", "MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID", "CHARACTERISTIC.ID = " + cellID).ToString();
            db.disconnect();

            return fileName;
        }

        // quickview buttons
        protected void Button1_Click(object sender, EventArgs e)
        {


            //GetXmlData(FileNameBox.Text, TableNameBox.Text);
            QVWFileData qvwData = new QVWFileData(FileNameBox.Text);
            ReportTypList.DataSource = qvwData.reportTypes;
            ReportTypList.DataBind();
            Session.Add("QVWFileData", qvwData);
        }

        protected void GetProducts_Click(object sender, EventArgs e)
        {
            QVWFileData qvwData = ((QVWFileData)Session["QVWFileData"]);
            qvwData.GetProducts(ReportTypList.SelectedValue.ToString());
            ProductsList.DataSource = qvwData.Products;
            ProductsList.DataBind();
        }

        protected void GetSegments_Click(object sender, EventArgs e)
        {
            QVWFileData qvwData = ((QVWFileData)Session["QVWFileData"]);
            qvwData.GetSegments(ReportTypList.SelectedValue.ToString(), ProductsList.SelectedValue.ToString());
            SegmentsList.DataSource = qvwData.Segments;
            SegmentsList.DataBind();
        }

        protected void GetFacts_Click(object sender, EventArgs e)
        {
            QVWFileData qvwData = ((QVWFileData)Session["QVWFileData"]);
            qvwData.GetFacts(ReportTypList.SelectedValue.ToString(), ProductsList.SelectedValue.ToString(), SegmentsList.SelectedValue.ToString());
            FactsList.DataSource = qvwData.Facts;
            FactsList.DataBind();
        }

        protected void ApplyFact_Click(object sender, EventArgs e)
        {
            QVWFileData qvwData = ((QVWFileData)Session["QVWFileData"]);

            string[] columnNames = qvwData.GetColumns("1.",ReportTypList.SelectedValue.ToString(), ProductsList.SelectedValue.ToString(), SegmentsList.SelectedValue.ToString(), FactsList.SelectedValue.ToString());
            FieldsList1.DataSource = columnNames;
            FieldsList1.DataBind();
            FieldsList2.DataSource = columnNames;
            FieldsList2.DataBind();

        }

        protected void Apply2_Click(object sender, EventArgs e)
        {


            //GetXmlData(FileNameBox.Text, TableNameBox.Text);
            QVWFileData qvwData = new QVWFileData(FileNameBox0.Text);
            ReportTypList0.DataSource = qvwData.reportTypes;
            ReportTypList0.DataBind();
            Session.Add("SecondQVWFileData", qvwData);
        }

        protected void GetProducts2_Click(object sender, EventArgs e)
        {
            QVWFileData qvwData = ((QVWFileData)Session["SecondQVWFileData"]);
            qvwData.GetProducts(ReportTypList0.SelectedValue.ToString());
            ProductsList0.DataSource = qvwData.Products;
            ProductsList0.DataBind();
        }

        protected void GetSegments2_Click(object sender, EventArgs e)
        {
            QVWFileData qvwData = ((QVWFileData)Session["SecondQVWFileData"]);
            qvwData.GetSegments(ReportTypList0.SelectedValue.ToString(), ProductsList0.SelectedValue.ToString());
            SegmentsList0.DataSource = qvwData.Segments;
            SegmentsList0.DataBind();
        }

        protected void GetFacts2_Click(object sender, EventArgs e)
        {
            QVWFileData qvwData = ((QVWFileData)Session["SecondQVWFileData"]);
            qvwData.GetFacts(ReportTypList0.SelectedValue.ToString(), ProductsList0.SelectedValue.ToString(), SegmentsList0.SelectedValue.ToString());
            FactsList0.DataSource = qvwData.Facts;
            FactsList0.DataBind();
        }

        protected void ApplyFact2_Click(object sender, EventArgs e)
        {
            QVWFileData qvwData = ((QVWFileData)Session["SecondQVWFileData"]);

            string[] columnNames = qvwData.GetColumns("2.",ReportTypList0.SelectedValue.ToString(), ProductsList0.SelectedValue.ToString(), SegmentsList0.SelectedValue.ToString(), FactsList0.SelectedValue.ToString());
            FieldsList3.DataSource = columnNames;
            FieldsList3.DataBind();
            FieldsList4.DataSource = columnNames;
            FieldsList4.DataBind();

        }

        // Excel buttons
        protected void ApplyExcelFile_Click(object sender, EventArgs e)
        {
            if (ExcelFileBox.Text != "")
            {
                ExcelData excelData = new ExcelData(ExcelFileBox.Text);
                Session.Add("ExcelData", excelData);
                string[] worksheets = new string[excelData.wb.Sheets.Count];
                int i = 0;
                foreach (Worksheet ws in excelData.wb.Sheets)
                {
                    worksheets[i] = ws.Name;
                    i++;
                }
                ExcelSheetList.DataSource = worksheets;
                ExcelSheetList.DataBind();
            }
        }

        protected void ApplyTablenames_Click(object sender, EventArgs e)
        {
            if (ExcelFileBox.Text != "" && ExcelSheetList.SelectedValue != "" && Title1Box.Text != "" && Title2Box.Text != "")
            {
                ExcelData excelData = (ExcelData)Session["ExcelData"];
                Worksheet ws = excelData.wb.Sheets[Convert.ToInt16(ExcelSheetList.SelectedValue)];


                int i = 0;
                while ((ws.Rows[i].Cells[0].Value == null || !ws.Rows[i].Cells[0].Value.ToString().Trim().Equals(Title1Box.Text.Trim())) || (ws.Rows[i + 1].Cells[0].Value == null || !ws.Rows[i + 1].Cells[0].Value.ToString().Trim().Equals(Title2Box.Text.Trim())))
                {
                    i++;
                }
                i = i + 2;
                while (ws.Rows[i].Cells[0].Value != null)
                {
                    i++;
                }
                while (ws.Rows[i].Cells[0].Value == null)
                {
                    i++;
                }
                int j = 1;
                ArrayList columnNames = new ArrayList();

                while (ws.Rows[i].Cells[j].Value != null)
                {
                    columnNames.Add("[1." + (j - 1) + "] " + ws.Rows[i].Cells[j].Value.ToString().Replace("<", "&lt").Replace(">", "&gt"));
                    j++;
                }

                FieldsList1.DataSource = columnNames.ToArray(typeof(string));
                FieldsList1.DataBind();
                FieldsList2.DataSource = columnNames.ToArray(typeof(string));
                FieldsList2.DataBind();
            }
        }

        protected void ApplyExcelFile2_Click(object sender, EventArgs e)
        {
            if (ExcelFileBox.Text != "")
            {
                ExcelData excelData = new ExcelData(ExcelFileBox0.Text);
                Session.Add("SecondExcelData", excelData);
                string[] worksheets = new string[excelData.wb.Sheets.Count];
                int i = 0;
                foreach (Worksheet ws in excelData.wb.Sheets)
                {
                    worksheets[i] = ws.Name;
                    i++;

                }
                ExcelSheetList0.DataSource = worksheets;
                ExcelSheetList0.DataBind();
            }
        }

        protected void ApplyTablenames2_Click(object sender, EventArgs e)
        {
            if (ExcelFileBox0.Text != "" && ExcelSheetList0.SelectedValue != "" && Title1Box0.Text != "" && Title2Box0.Text != "")
            {
                ExcelData excelData = (ExcelData)Session["SecondExcelData"];
                Worksheet ws = excelData.wb.Sheets[Convert.ToInt16(ExcelSheetList0.SelectedValue)];


                int i = 0;
                while ((ws.Rows[i].Cells[0].Value == null || !ws.Rows[i].Cells[0].Value.ToString().Trim().Equals(Title1Box0.Text.Trim())) || (ws.Rows[i + 1].Cells[0].Value == null || !ws.Rows[i + 1].Cells[0].Value.ToString().Trim().Equals(Title2Box0.Text.Trim())))
                {
                    i++;
                }
                i = i + 2;
                while (ws.Rows[i].Cells[0].Value != null)
                {
                    i++;
                }
                while (ws.Rows[i].Cells[0].Value == null)
                {
                    i++;
                }
                int j = 1;
                ArrayList columnNames = new ArrayList();

                while (ws.Rows[i].Cells[j].Value != null)
                {
                    columnNames.Add("[2." + (j - 1) + "] " + ws.Rows[i].Cells[j].Value.ToString().Replace("<", "&lt").Replace(">", "&gt"));
                    j++;
                }

                FieldsList3.DataSource = columnNames.ToArray(typeof(string));
                FieldsList3.DataBind();
                FieldsList4.DataSource = columnNames.ToArray(typeof(string));
                FieldsList4.DataBind();
            }
        }

        string[] GetColumnNames()
        {
            CellData cellData = (CellData)Session["CellData"];
            if (cellData.isQvw)
            {
                ApplyFact_Click(null, null);
            }
            else
            {
                ApplyTablenames_Click(null, null);
            }

            return (string[])FieldsList1.DataSource;
        }

        string[] GetColumnNames2()
        {
            CellData cellData = (CellData)Session["CellData"];
            if (SecondTableEnabledBox.Checked)
            {
                if (cellData.secondIsQVW)
                {
                    ApplyFact2_Click(null, null);
                }
                else
                {
                    ApplyTablenames2_Click(null, null);
                }
            }
            return (string[])FieldsList3.DataSource;
        }

        // write data into form
        void WriteToForm(CellData cellData)
        {


            // write all data
            WebTab1.SelectedIndex = cellData.isQvw ? 0 : 1;


            ExcelSheetList.DataSource = new string[1] { cellData.excelSheet };
            ExcelSheetList.DataBind();
            Title1Box.Text = cellData.title1;
            Title2Box.Text = cellData.title2;

            ReportTypList.DataSource = new string[1] { cellData.reportTyp };
            ReportTypList.DataBind();
            ProductsList.DataSource = new string[1] { cellData.product };
            ProductsList.DataBind();
            SegmentsList.DataSource = new string[1] { cellData.segment };
            SegmentsList.DataBind();
            FactsList.DataSource = new string[1] { cellData.fact };
            FactsList.DataBind();


            RowNameBox.Text = cellData.rowName;

            SecondTableEnabledBox.Checked = cellData.secondTableEnabled;
            PreviousYearBox.Checked = cellData.previousYearEnabled;
            WebTab2.SelectedIndex = cellData.secondIsQVW ? 0 : 1;

            ExcelSheetList0.DataSource = new string[1] { cellData.excelSheet2 };
            ExcelSheetList0.DataBind();
            Title1Box0.Text = cellData.title12;
            Title2Box0.Text = cellData.title22;

            ReportTypList0.DataSource = new string[1] { cellData.reportTyp2 };
            ReportTypList0.DataBind();
            ProductsList0.DataSource = new string[1] { cellData.product2 };
            ProductsList0.DataBind();
            SegmentsList0.DataSource = new string[1] { cellData.segment2 };
            SegmentsList0.DataBind();
            FactsList0.DataSource = new string[1] { cellData.fact2 };
            FactsList0.DataBind();

            RowNameBox0.Text = cellData.rowName2;

            EnableSecondValue.Checked = cellData.secondValueEnabled;
            FormulaBox2.Enabled = cellData.secondValueEnabled;
            UnitBox2.Enabled = cellData.secondValueEnabled;
       

            HighestIsGreen.Checked = cellData.highestValueIsGreen;
            LowestIsGreen.Checked = !cellData.highestValueIsGreen;
            ColorationOnSecondValueBox.Checked = cellData.colorationOnSecondValue;

            greenLightgreenBox.Text = cellData.borderGreenLightgreen;
            lightgreenYellowBox.Text = cellData.borderLightgreenYellow;
            yellowOrangeBox.Text = cellData.borderYellowOrange;
            orangeRedBox.Text = cellData.borderOrangeRed;

            string[] columnNames = GetColumnNames();

            String formula1 = cellData.formula1;
            string formula2 = cellData.formula2;

            // decode formulas
            for (int i = 0; i < columnNames.Length; i++)
            {
                formula1 = formula1.Replace("[1." + i + "]", columnNames[i]);
                formula2 = formula2.Replace("[1." + i + "]", columnNames[i]);
            }

         columnNames = GetColumnNames2();

            // decode formulas
         if (cellData.secondTableEnabled)
         {
             for (int i = 0; i < columnNames.Length; i++)
             {
                 formula1 = formula1.Replace("[2." + i + "]", columnNames[i]);
                 formula2 = formula2.Replace("[2." + i + "]", columnNames[i]);
             }
         }
            FormulaBox1.Text = formula1;
            FormulaBox2.Text = formula2;

            UnitBox1.Text = cellData.unit1;
            UnitBox2.Text = cellData.unit2;

        }

        protected void AddToFormula3_Click(object sender, EventArgs e)
        {
            FormulaBox1.Text += FieldsList3.SelectedValue;
        }

        protected void AddToFormula4_Click(object sender, EventArgs e)
        {
            FormulaBox2.Text += FieldsList4.SelectedValue;
        }
    }

    //  class has data of 1 sokrates cell
    public class CellData
    {
        public int cellID;
        System.Web.SessionState.HttpSessionState Session;

        public Boolean isQvw;
        // parameters to get table     

        public string reportTyp;
        public string product;
        public string segment;
        public string fact;
        public string excelSheet;
        public string title1;
        public string title2;

        public string rowName;


        public string reportTyp2;
        public string product2;
        public string segment2;
        public string fact2;
        public string excelSheet2;
        public string title12;
        public string title22;

        public string rowName2;

        public Boolean secondTableEnabled;
        public Boolean previousYearEnabled;
        public Boolean secondIsQVW;


        public string formula1;             // formula for 1st value
        public string unit1;                // unit of first value

        public string formula2;
        public string unit2;
        public Boolean secondValueEnabled;  // is second value enabled

        public Boolean highestValueIsGreen; // is highest value green
        public Boolean colorationOnSecondValue;

        // bordervalues between colors
        public string borderGreenLightgreen;
        public string borderLightgreenYellow;
        public string borderYellowOrange;
        public string borderOrangeRed;

        public CellData(int cellId, System.Web.SessionState.HttpSessionState Session)
        {
            cellID = cellId;
            this.Session = Session;
            UpdateFromDB();
        }

        public void UpdateFromDB()
        {
            DBData db = DBData.getDBData(Session);
            db.connect();

            // does entry in db exist?
            if (db.lookup("ID", "GFK_FORMULA_CELL", "CELL_ID=" + cellID, "").ToString() == "")
            {
                string sql = "INSERT INTO GFK_FORMULA_CELL (CELL_ID) VALUES(" + cellID + ")";
                db.execute(sql);
            }// read data from db

            rowName = db.lookup("ROW_NAME", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString().Replace("&lt", "<").Replace("&gt", ">");
            reportTyp = db.lookup("REPORT_TYP", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            product = db.lookup("PRODUCT", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            segment = db.lookup("SEGMENT", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            fact = db.lookup("FACT", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            excelSheet = db.lookup("EXCELSHEET", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            title1 = db.lookup("TABLENAME1", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            title2 = db.lookup("TABLENAME2", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            rowName2 = db.lookup("SECOND_ROW_NAME", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString().Replace("&lt" , "<").Replace("&gt" , ">");
            reportTyp2 = db.lookup("SECOND_REPORT_TYP", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            product2 = db.lookup("SECOND_PRODUCT", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            segment2 = db.lookup("SECOND_SEGMENT", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            fact2 = db.lookup("SECOND_FACT", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            excelSheet2 = db.lookup("SECOND_EXCELSHEET", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            title12 = db.lookup("SECOND_TABLENAME1", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            title22 = db.lookup("SECOND_TABLENAME2", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            formula1 = db.lookup("FORMULA1", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            unit1 = db.lookup("UNIT1", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            formula2 = db.lookup("FORMULA2", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            unit2 = db.lookup("UNIT2", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            borderGreenLightgreen = db.lookup("BORDER_GREEN_LIGHTGREEN", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            borderLightgreenYellow = db.lookup("BORDER_LIGHTGREEN_YELLOW", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            borderYellowOrange = db.lookup("BORDER_YELLOW_ORANGE", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();
            borderOrangeRed = db.lookup("BORDER_ORANGE_RED", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString();

            secondValueEnabled = db.lookup("SECOND_VALUE_ENABLED", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString() == "True";
            highestValueIsGreen = db.lookup("HIGHEST_VALUE_IS_GREEN", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString() == "True";
            isQvw = db.lookup("IS_QVW", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString() == "True";
            colorationOnSecondValue = db.lookup("COLORATION_SECOND_VALUE", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString() == "True";
            secondIsQVW = db.lookup("SECOND_IS_QVW", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString() == "True";
            secondTableEnabled = db.lookup("SECOND_TABLE_ENABLED", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString() == "True";
            previousYearEnabled = db.lookup("PREVIOUS_YEAR_ENABLED", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString() == "True";
            db.disconnect();
        }

        // write data to database
        public void WriteToDB()
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            string isQvwOut = isQvw ? "1" : "0";
            string secondValueEnabledOut = secondValueEnabled ? "1" : "0";
            string highestIsGreenOut = highestValueIsGreen ? "1" : "0";
            string colorSecondValueOut = colorationOnSecondValue ? "1" : "0";
            string secondIsQVWOut = secondIsQVW ? "1" : "0";
            string secondTableEnabledOut = secondTableEnabled ? "1" : "0";
            string previousYearEnabledOut = previousYearEnabled ? "1" : "0";

            string sql = "UPDATE GFK_FORMULA_CELL SET SECOND_ROW_NAME = '" + rowName2.Replace("<", "&lt").Replace(">", "&gt") + "' , SECOND_IS_QVW = '" + secondIsQVWOut + "' , PREVIOUS_YEAR_ENABLED = '" + previousYearEnabledOut + "' , SECOND_TABLE_ENABLED = '" + secondTableEnabledOut + "' , COLORATION_SECOND_VALUE = '" + colorSecondValueOut + "' , ROW_NAME = '" + rowName.Replace("<", "&lt").Replace(">", "&gt") + "' , IS_QVW = '" + isQvwOut + "' , EXCELSHEET = '" + excelSheet + "' ,TABLENAME1 = '" + title1 + "' ,TABLENAME2 = '" + title2 + "' , REPORT_TYP = '" + reportTyp + "' , PRODUCT = '" + product + "' , SEGMENT = '" + segment + "' , FACT = '" + fact + "' , SECOND_EXCELSHEET = '" + excelSheet2 + "' ,SECOND_TABLENAME1 = '" + title12 + "' ,SECOND_TABLENAME2 = '" + title22 + "' , SECOND_REPORT_TYP = '" + reportTyp2 + "' , SECOND_PRODUCT = '" + product2 + "' , SECOND_SEGMENT = '" + segment2 + "' , SECOND_FACT = '" + fact2 + "' , FORMULA1 = '" + formula1 + "', UNIT1 = '" + unit1 + "', FORMULA2 = '" + formula2 + "', UNIT2 = '" + unit2 + "', SECOND_VALUE_ENABLED = '" + secondValueEnabledOut + "', HIGHEST_VALUE_IS_GREEN = '" + highestIsGreenOut + "' , BORDER_GREEN_LIGHTGREEN = '" + borderGreenLightgreen + "', BORDER_LIGHTGREEN_YELLOW = '" + borderLightgreenYellow + "', BORDER_YELLOW_ORANGE = '" + borderYellowOrange + "', BORDER_ORANGE_RED = '" + borderOrangeRed + "' WHERE CELL_ID = '" + cellID + "'";
            db.execute(sql);
            db.disconnect();
        }
    }

}