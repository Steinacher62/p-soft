using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using Telerik.Web.Spreadsheet;
using Telerik.Web.UI;



namespace ch.appl.psoft.GFK
{
    public partial class FormelEditor : PsoftEditPage
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

            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;			
            PsoftPageLayout.PageTitle = BreadcrumbCaption = "GFK Formeleditor";

            // rowId
            int rowId = int.Parse(Request.QueryString["Row_ID"]);

            if (FileNameBox.Text == "" && ExcelFileBox.Text =="")
            {
                

                // get data from db
                RowData rowData;
                rowData = new RowData(rowId, Session);

                // get filename
                string filename = GetFileName();
                
                FileNameBox.Text = filename;
                ExcelFileBox.Text = filename;

                string filename2 = GetFileName2();

                FileNameBox0.Text = filename2;
                ExcelFileBox0.Text = filename2;

                // save rowdata in session
                Session.Add("RowData", rowData);

                // write to form
                if (rowData.secondTableEnabled)
                {
                    if (rowData.secondIsQVW)
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
                if (filename != "" && rowData.product != "" && rowData.isQvw)
                {

                    QVWFileData qvwData = new QVWFileData(FileNameBox.Text);
                    Session.Add("QVWFileData", qvwData);
                    WriteToForm(rowData);
                }
                else if (filename != "" && rowData.excelSheet != "" && !rowData.isQvw)
                {
                    ApplyExcelFile_Click(null, null);
                    ApplyTablenames_Click(null, null);
                    WriteToForm(rowData);
                }
                
            }
        }

        // toggle second Value
        protected void EnableSecondValue_CheckedChanged(object sender, EventArgs e)
        {
            // enable input for second value
            FormulaBox2.Enabled =       EnableSecondValue.Checked;
            FieldsList2.Enabled =       EnableSecondValue.Checked;
            UnitBox2.Enabled    =       EnableSecondValue.Checked;
            EnableCount.Enabled = !EnableSecondValue.Checked;

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

        protected void AddToFormula3_Click(object sender, EventArgs e)
        {
            FormulaBox1.Text += FieldsList5.SelectedValue;
        }

        protected void AddToFormula4_Click(object sender, EventArgs e)
        {
            FormulaBox2.Text += FieldsList6.SelectedValue;
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
                RowData rowData = (RowData)Session["RowData"];

                string filename;
                rowData.rowID = int.Parse(Request.QueryString["Row_ID"]);

                rowData.isQvw = WebTab1.SelectedIndex == 0;

                if (rowData.isQvw)
                {
                    filename = FileNameBox.Text;
                }
                else
                {
                    filename = ExcelFileBox.Text;
                }
                string filename2;
                rowData.secondIsQVW = WebTab2.SelectedIndex == 0;
                rowData.previousYearEnabled = PreviousYearBox.Checked;
                rowData.secondTableEnabled = SecondTableEnabledBox.Checked;

                if (rowData.secondIsQVW)
                {
                    filename2 = FileNameBox0.Text;
                }
                else
                {
                    filename2 = ExcelFileBox0.Text;
                }


                rowData.excelSheet = ExcelSheetList.SelectedValue;
                rowData.title1 = Title1Box.Text;
                rowData.title2 = Title2Box.Text;

                rowData.reportTyp = ReportTypList.SelectedValue;
                rowData.product = ProductsList.SelectedValue;
                rowData.segment = SegmentsList.SelectedValue;
                rowData.fact = FactsList.SelectedValue;

                rowData.excelSheet2 = ExcelSheetList0.SelectedValue;
                rowData.title12 = Title1Box0.Text;
                rowData.title22 = Title2Box0.Text;

                rowData.reportTyp2 = ReportTypList0.SelectedValue;
                rowData.product2 = ProductsList0.SelectedValue;
                rowData.segment2 = SegmentsList0.SelectedValue;
                rowData.fact2 = FactsList0.SelectedValue;

                // formula Input
                rowData.unit1 = UnitBox1.Text;
                rowData.unit2 = UnitBox2.Text;
                rowData.unitTotal = UnitBoxTotal.Text; 
                rowData.secondUnitTotal = UnitBoxTotal0.Text;
                rowData.TotalEnabled = EnableTotal.Checked;
                rowData.secondTotalEnabled = SecondTotalEnabledBox.Checked;
                rowData.countEnabled = EnableCount.Checked;
                rowData.numberOfEntries = NumberOfEntriesBox.Text;
                rowData.secondValueEnabled = EnableSecondValue.Checked;
                rowData.secondTotalHidden = EnableSecondTotalHidden.Checked;

                // sortation Inputs
                rowData.borderEnabled = BorderEnabledBax.Checked;
                rowData.sortationOnSecondValue = SortingOnSecondValueBox.Checked;
                rowData.descendantSorted = SortedHighestFirst.Checked;
                rowData.upperBorder = HighestValueBox.Text;
                rowData.lowerBorder = LowestValueBox.Text;


                // color inputs
                rowData.colorationOnSecondValue = ColorationOnSecondValueBox.Checked;
                rowData.highestValueIsGreen = HighestIsGreen.Checked;
                rowData.borderGreenLightgreen = greenLightgreenBox.Text;
                rowData.borderLightgreenYellow = lightgreenYellowBox.Text;
                rowData.borderYellowOrange = yellowOrangeBox.Text;
                rowData.borderOrangeRed = orangeRedBox.Text;

                rowData.borderOrangeTotal = orangeRedBoxTotal.Text;
                rowData.borderGreenTotal = greenLightgreenBoxTotal.Text;
                rowData.borderLightgreenTotal = lightgreenYellowBoxTotal.Text;
                rowData.borderYellowTotal = yellowOrangeBoxTotal.Text;

               
                rowData.colorationOnSecondTotal = ColorationSecondTotalBox.Checked;

                // rewrite formulas
                String[] columnNames = GetColumnNames();

                rowData.formula1 = FormulaBox1.Text;
                rowData.formula2 = FormulaBox2.Text;
                rowData.formulaTotal = FormulaBoxTotal.Text;
                rowData.secondFormulaTotal = FormulaBoxTotal0.Text;

                for (int columnNr = 0; columnNr < columnNames.Length; columnNr++)
                {
                    rowData.formula1 = rowData.formula1.Replace(columnNames[columnNr], "[1." + columnNr + "]");
                    if (EnableSecondValue.Checked)
                    {
                        rowData.formula2 = rowData.formula2.Replace(columnNames[columnNr], "[1." + columnNr + "]");
                    }
                    if (EnableTotal.Checked)
                    {
                        rowData.formulaTotal = rowData.formulaTotal.Replace(columnNames[columnNr], "[1." + columnNr + "]");
                    }
                    if (SecondTotalEnabledBox.Checked)
                    {
                        rowData.secondFormulaTotal = rowData.secondFormulaTotal.Replace(columnNames[columnNr], "[1." + columnNr + "]");
                    }

                }

                if (rowData.secondTableEnabled)
                {
                    columnNames = GetColumnNames2();
                    for (int columnNr = 0; columnNr < columnNames.Length; columnNr++)
                    {
                        rowData.formula1 = rowData.formula1.Replace(columnNames[columnNr], "[2." + columnNr + "]");
                        if (EnableSecondValue.Checked)
                        {
                            rowData.formula2 = rowData.formula2.Replace(columnNames[columnNr], "[2." + columnNr + "]");
                        }
                        if (EnableTotal.Checked)
                        {
                            rowData.formulaTotal = rowData.formulaTotal.Replace(columnNames[columnNr], "[2." + columnNr + "]");
                        }
                         
                    if (SecondTotalEnabledBox.Checked)
                    {
                        rowData.secondFormulaTotal = rowData.secondFormulaTotal.Replace(columnNames[columnNr], "[2." + columnNr + "]");
                    }

                    }
                }

                // save filename to db
                
                DBData db = DBData.getDBData(Session);
                db.connect();
                string matrixId = db.lookup("MATRIX.ID", "MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID", "DIMENSION.ID = " + Request.QueryString["Row_ID"]).ToString();
                db.execute("UPDATE MATRIX SET GFK_FILE_NAME ='" + filename + "' , SECOND_FILE_NAME ='" + filename2 + "' WHERE ID =" + matrixId);
                db.disconnect();

                // write rowData to db
                rowData.WriteToDB();
                Response.Redirect(Global.Config.baseURL + "/GFK/SaveSuccessful.aspx");
                }
        }

       // check if all requiered fields are filled
        Boolean RequieredFieldsFull()
        {
            bool secondTotal = (!SecondTotalEnabledBox.Checked || (FormulaBoxTotal0.Text != "" && EnableTotal.Checked));
            bool totalSelection = (!EnableTotal.Checked || ((FormulaBoxTotal.Text != "" && greenLightgreenBoxTotal.Text != "" && lightgreenYellowBoxTotal.Text != "" && yellowOrangeBoxTotal.Text != "" && orangeRedBoxTotal.Text != "") || (FormulaBoxTotal.Text != "" && greenLightgreenBoxTotal.Text == "" && lightgreenYellowBoxTotal.Text == "" && yellowOrangeBoxTotal.Text == "" && orangeRedBoxTotal.Text == "")));
            bool tableSelection = (WebTab1.SelectedIndex == 0 && ReportTypList.SelectedValue != "" && ProductsList.SelectedValue != "" && SegmentsList.SelectedValue != "" && FactsList.SelectedValue != "" && FileNameBox.Text != "") || (WebTab1.SelectedIndex == 1 && ExcelFileBox.Text != "" && ExcelSheetList.SelectedValue != "" && Title1Box.Text != "" && Title2Box.Text != "");
            return (secondTotal && totalSelection && tableSelection && (FormulaBox1.Text != "" || EnableCount.Checked) && ((greenLightgreenBox.Text != "" && lightgreenYellowBox.Text != "" && yellowOrangeBox.Text != "" && orangeRedBox.Text != "") || (greenLightgreenBox.Text == "" && lightgreenYellowBox.Text == "" && yellowOrangeBox.Text == "" && orangeRedBox.Text == "")) && (!EnableSecondValue.Checked || FormulaBox2.Text != ""));
        }

        // get filename from db
        string GetFileName()
        {
            // get rowId
            int rowID = int.Parse(Request.QueryString["Row_ID"]);

            // get filename from db
            DBData db = DBData.getDBData(Session);
            db.connect();
            string fileName = db.lookup("GFK_FILE_NAME", "MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID", "DIMENSION.ID = " + rowID).ToString();         
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
            
            string[] columnNames = qvwData.GetColumns("1.",ReportTypList.SelectedValue.ToString(), ProductsList.SelectedValue.ToString(), SegmentsList.SelectedValue.ToString(),FactsList.SelectedValue.ToString());
            FieldsList1.DataSource = columnNames;
            FieldsList1.DataBind();
            FieldsList2.DataSource = columnNames;
            FieldsList2.DataBind();
            FieldsList3.DataSource = columnNames;
            FieldsList3.DataBind();
            FieldsList7.DataSource = columnNames;
            FieldsList7.DataBind();
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

            string[] columnNames = qvwData.GetColumns("2.", ReportTypList0.SelectedValue.ToString(), ProductsList0.SelectedValue.ToString(), SegmentsList0.SelectedValue.ToString(), FactsList0.SelectedValue.ToString());
            
            FieldsList8.DataSource = columnNames;
            FieldsList8.DataBind();    
            FieldsList6.DataSource = columnNames;
            FieldsList6.DataBind();
            FieldsList5.DataSource = columnNames;
            FieldsList5.DataBind();
            FieldsList4.DataSource = columnNames;
            FieldsList4.DataBind();

        }

        // Excel buttons
        protected void ApplyExcelFile_Click(object sender, EventArgs e)
        {
            if(ExcelFileBox.Text != ""){
                ExcelData excelData = new ExcelData(ExcelFileBox.Text);
                Session.Add("ExcelData", excelData);
                string[] worksheets = new string[excelData.wb.Sheets.Count];
                int i = 0;
                foreach (Worksheet ws in excelData.wb.Sheets){
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
           
               
                int i=0;
                while ((ws.Rows[i].Cells[0].Value == null || !ws.Rows[i].Cells[0].Value.ToString().Trim().Equals( Title1Box.Text.Trim()) )||(ws.Rows[i+1].Cells[0].Value==null || !ws.Rows[i+1].Cells[0].Value.ToString().Trim().Equals( Title2Box.Text.Trim())))
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
                FieldsList3.DataSource = columnNames.ToArray(typeof(string));
                FieldsList3.DataBind();
                FieldsList7.DataSource = columnNames.ToArray(typeof(string));
                FieldsList7.DataBind();
            }
        }


        protected void ApplyExcelFile2_Click(object sender, EventArgs e)
        {
            if (ExcelFileBox0.Text != "")
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
                    columnNames.Add("[2." + (j - 1) + "] " + ws.Rows[i].Cells[j].Value);
                    j++;
                }

                FieldsList8.DataSource = columnNames.ToArray(typeof(string));
                FieldsList8.DataBind();
                
                FieldsList6.DataSource = columnNames.ToArray(typeof(string));
                FieldsList6.DataBind();
                FieldsList5.DataSource = columnNames.ToArray(typeof(string));
                FieldsList5.DataBind();
                FieldsList4.DataSource = columnNames.ToArray(typeof(string));
                FieldsList4.DataBind();
            }
        }


        string[] GetColumnNames()
        {
            RowData rowData = (RowData)Session["RowData"];
            if (rowData.isQvw)
            {
                ApplyFact_Click(null,null);
            }
            else
            {
                ApplyTablenames_Click(null, null);
            }

            return (string[])FieldsList1.DataSource;
        }

        string[] GetColumnNames2()
        {
            RowData rowData = (RowData)Session["RowData"];
            if (SecondTableEnabledBox.Checked)
            {
                if (rowData.secondIsQVW)
                {
                    ApplyFact2_Click(null, null);
                }
                else
                {
                    ApplyTablenames2_Click(null, null);
                }
            }
            return (string[])FieldsList4.DataSource;
        }


        // get filename from db
        string GetFileName2()
        {
            // get rowId
            int rowID = int.Parse(Request.QueryString["Row_ID"]);

            // get filename from db
            DBData db = DBData.getDBData(Session);
            db.connect();
            string fileName = db.lookup("SECOND_FILE_NAME", "MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID", "DIMENSION.ID = " + rowID).ToString();
            db.disconnect();

            return fileName;
        }



        // write data into form
        void WriteToForm(RowData rowData)
        {
            

            // write all data
            WebTab1.SelectedIndex = rowData.isQvw ? 0 : 1;


            ExcelSheetList.DataSource = new string[1] { rowData.excelSheet};
            ExcelSheetList.DataBind();
            Title1Box.Text = rowData.title1;
            Title2Box.Text = rowData.title2;

            SecondTableEnabledBox.Checked = rowData.secondTableEnabled;
            PreviousYearBox.Checked = rowData.previousYearEnabled;
            WebTab2.SelectedIndex = rowData.secondIsQVW ? 0 : 1;

            ReportTypList.DataSource = new string[1]{rowData.reportTyp};
            ReportTypList.DataBind();
            ProductsList.DataSource = new string[1]{rowData.product};
            ProductsList.DataBind();
            SegmentsList.DataSource = new string[1]{rowData.segment};
            SegmentsList.DataBind();
            FactsList.DataSource = new string[1]{rowData.fact};
            FactsList.DataBind();

            ReportTypList0.DataSource = new string[1] { rowData.reportTyp2 };
            ReportTypList0.DataBind();
            ProductsList0.DataSource = new string[1] { rowData.product2 };
            ProductsList0.DataBind();
            SegmentsList0.DataSource = new string[1] { rowData.segment2 };
            SegmentsList0.DataBind();
            FactsList0.DataSource = new string[1] { rowData.fact2 };
            FactsList0.DataBind();
            
            EnableCount.Checked = rowData.countEnabled;
            
            EnableSecondValue.Checked = rowData.secondValueEnabled;
            FormulaBox2.Enabled = rowData.secondValueEnabled;
            UnitBox2.Enabled = rowData.secondValueEnabled;

            EnableTotal.Checked = rowData.TotalEnabled;
            FormulaBoxTotal.Enabled = rowData.TotalEnabled;
            UnitBoxTotal.Enabled = rowData.TotalEnabled;

            EnableTotal.Enabled = !EnableCount.Checked;
            EnableSecondValue.Enabled = !EnableCount.Checked;
            EnableCount.Enabled = !EnableSecondValue.Checked && !EnableTotal.Checked;
            NumberOfEntriesBox.Text = rowData.numberOfEntries;

            FormulaBoxTotal0.Text = rowData.secondFormulaTotal;
            UnitBoxTotal0.Text = rowData.secondUnitTotal;
            SecondTotalEnabledBox.Checked = rowData.secondTotalEnabled;
            EnableSecondTotalHidden.Checked = rowData.secondTotalHidden;

            SortingOnSecondValueBox.Checked = rowData.sortationOnSecondValue;
            BorderEnabledBax.Checked = rowData.borderEnabled;
            SortedHighestFirst.Checked = rowData.descendantSorted;
            SortedLowestFirst.Checked = !rowData.descendantSorted;

            HighestValueBox.Text = rowData.upperBorder;
            LowestValueBox.Text = rowData.lowerBorder;

            ColorationOnSecondValueBox.Checked = rowData.colorationOnSecondValue;
            HighestIsGreen.Checked = rowData.highestValueIsGreen;
            LowestIsGreen.Checked = !rowData.highestValueIsGreen;

            greenLightgreenBox.Text = rowData.borderGreenLightgreen;
            lightgreenYellowBox.Text = rowData.borderLightgreenYellow;
            yellowOrangeBox.Text = rowData.borderYellowOrange;
            orangeRedBox.Text = rowData.borderOrangeRed;

            greenLightgreenBoxTotal.Text = rowData.borderGreenTotal;
            lightgreenYellowBoxTotal.Text = rowData.borderLightgreenTotal;
            yellowOrangeBoxTotal.Text = rowData.borderYellowTotal;
            orangeRedBoxTotal.Text = rowData.borderOrangeTotal;

           
            ColorationSecondTotalBox.Checked = rowData.colorationOnSecondTotal;

            string[] columnNames = GetColumnNames();

            String formula1 = rowData.formula1;
            string formula2 = rowData.formula2;
            string formulaTotal = rowData.formulaTotal;
            string formulaTotal2 = rowData.secondFormulaTotal;

            // decode formulas
            for (int i = 0; i < columnNames.Length; i++)
            {
                formula1 = formula1.Replace("[1." + i + "]", columnNames[i]);
                formula2 = formula2.Replace("[1." + i + "]", columnNames[i]);
                formulaTotal = formulaTotal.Replace("[1." + i + "]", columnNames[i]);
                formulaTotal2 = formulaTotal2.Replace("[1." + i + "]", columnNames[i]);
            }
            columnNames = GetColumnNames2();

            // decode formulas
            if (rowData.secondTableEnabled)
            {
                for (int i = 0; i < columnNames.Length; i++)
                {
                    formula1 = formula1.Replace("[2." + i + "]", columnNames[i]);
                    formula2 = formula2.Replace("[2." + i + "]", columnNames[i]);
                    formulaTotal = formulaTotal.Replace("[2." + i + "]", columnNames[i]);
                    formulaTotal2 = formulaTotal2.Replace("[2." + i + "]", columnNames[i]);
                }
            }

            FormulaBox1.Text = formula1;
            FormulaBox2.Text = formula2;
            FormulaBoxTotal.Text = formulaTotal;
            FormulaBoxTotal0.Text = formulaTotal2;

            UnitBox1.Text = rowData.unit1;
            UnitBox2.Text = rowData.unit2;
            UnitBoxTotal.Text = rowData.unitTotal;
            UnitBoxTotal0.Text = rowData.secondUnitTotal;

        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            UnitBoxTotal.Enabled =  EnableTotal.Checked;
            FormulaBoxTotal.Enabled = EnableTotal.Checked;
            EnableCount.Enabled = !EnableTotal.Checked;        
        }

        protected void AddToFormulaTotal_Click(object sender, EventArgs e)
        {
            FormulaBoxTotal.Text += FieldsList3.SelectedValue.ToString();
        }

        protected void AddToFormulaTotal2_Click(object sender, EventArgs e)
        {
            FormulaBoxTotal.Text += FieldsList4.SelectedValue.ToString();
        }

        protected void EnableCount_CheckedChanged(object sender, EventArgs e)
        {
            EnableTotal.Enabled = !EnableCount.Checked;
            EnableSecondValue.Enabled = !EnableCount.Checked;
        }

        protected void AddToFormulaTotal12_Click(object sender, EventArgs e)
        {
            FormulaBoxTotal0.Text += FieldsList7.SelectedValue.ToString();
        }

        protected void AddToFormulaTotal22_Click(object sender, EventArgs e)
        {
            FormulaBoxTotal0.Text += FieldsList8.SelectedValue.ToString();
        }
        



    }

    //class has all data of 1 xml table
   public class XmlData               
    {
       // public String[] rowNames;                   // Array with row Names
       // public DataTable datatable;                 // Datatable from xml
       string[] columnNames;
        // constructor
        public XmlData(String path, String tableName)
        {
            
            int tableNr = 0;

            // load xml file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            
            Boolean tableFound = false;
            foreach (XmlElement tab in xmlDoc.GetElementsByTagName("tab"))
            {
                if (tab.Attributes["repname"].InnerText.Equals(tableName))
                {
                    tableFound = true;
                    break;
                }
                tableNr++;
            }
            if (tableFound)
            {
                //create Datatable
               // datatable = new DataTable();

                //get Rows from XML
                XmlNodeList xmlRows = xmlDoc.GetElementsByTagName("tab")[tableNr].SelectNodes("descendant::row");
                String firstRow = xmlRows[0].SelectSingleNode("descendant::data").InnerText;

                //create Columns
                int numberOfColumns = firstRow.Split(",".ToCharArray()).Length;         // get number of Columns
                //DataColumn[] tableColumns = new DataColumn[numberOfColumns];            // Column Array
                columnNames = new string[numberOfColumns];

                for (int i = 0; i < numberOfColumns; i++)
                {
                    // create each Column
                    //tableColumns[i] = new DataColumn();
                    String[] columnRefIds = new String[xmlDoc.GetElementsByTagName("tab")[tableNr].SelectNodes("cols/descendant::col/descendant::itmrefs")[i].InnerText.Split(",".ToCharArray()).Length];
                    columnRefIds = xmlDoc.GetElementsByTagName("tab")[tableNr].SelectNodes("cols/descendant::col/descendant::itmrefs")[i].InnerText.Split(",".ToCharArray());

                    // name Column
                    String columnName = "[";
                    foreach (String itmRefId in columnRefIds)
                    {
                        if (columnName != "")
                        {
                            columnName += " ";            // string between Column Titles
                        }
                        columnName += xmlDoc.GetElementsByTagName("tab")[tableNr].SelectNodes("cols/descendant::itm")[Convert.ToInt32(itmRefId)].SelectNodes("txt")[2].InnerText;
                    }
                    //tableColumns[i].ColumnName = columnName;
                    //datatable.Columns.Add(tableColumns[i]);
                    columnNames[i] = columnName + "]";
                }


                // create Rows
                //foreach (XmlNode row in xmlRows)
                //{
                //    DataRow newRow = datatable.NewRow();                                                    // create New Row
                //    String[] rowData = new string[row.SelectSingleNode("descendant::data").InnerText.Split(",".ToCharArray()).Length];
                //    rowData = row.SelectSingleNode("descendant::data").InnerText.Split(",".ToCharArray());  // row Data Array
                //    int i = 0;                                                                              // counter
                //    foreach (DataColumn column in tableColumns)
                //    {
                //        //fill each field of Row
                //        newRow.SetField(column, rowData[i]);
                //        i++;
                //    }
                //    datatable.Rows.Add(newRow);
                //}


                // get row Names
                //rowNames = new String[xmlRows.Count];                                              // Row Name Array (Brands)
                //int rowNumber = 0;                                                                          // counter
                //foreach (XmlNode row in xmlRows)
                //{
                //    String[] itmRefIds = new String[row.SelectSingleNode("descendant::itmrefs").InnerText.Split(",".ToCharArray()).Length];   // itmrefids
                //    itmRefIds = row.SelectSingleNode("descendant::itmrefs").InnerText.Split(",".ToCharArray());
                //    rowNames[rowNumber] = "";
                //    foreach (String itmRefId in itmRefIds)
                //    {
                //        if (rowNames[rowNumber] != "")
                //        {
                //            rowNames[rowNumber] += " ";                                                   // string between Row Titles
                //        }
                //        rowNames[rowNumber] += xmlDoc.GetElementsByTagName("tab")[tableNr].SelectNodes("rows/descendant::itm")[Convert.ToInt32(itmRefId)].SelectNodes("txt")[2].InnerText;
                //    }
                //    rowNumber++;
                //}
            }
        }
        
        // get names of columns in string array
        public String[] getColumnnames()
        {
            //String[] columnNames = new String[datatable.Columns.Count];
            //int columncount = 0;
            //foreach( DataColumn column in datatable.Columns)
            //{
            //    columnNames[columncount] = "["+column.ColumnName.Replace("<", "&lt").Replace(">", "&gt")+"]";
            //    columncount++;
            //}
            return columnNames;
        }      
        
    }

   //  class has data of 1 sokrates row
   public class RowData
   {
       public int rowID;
       System.Web.SessionState.HttpSessionState Session;

       public Boolean isQvw;
       // parameters to get table
       public Boolean TotalEnabled;
       public string formulaTotal;
       public string unitTotal;
      

       public Boolean countEnabled;
       public string numberOfEntries;

       public Boolean secondTotalHidden;
       public Boolean secondTotalEnabled;
       public string secondFormulaTotal;
       public string secondUnitTotal;

       public string reportTyp;
       public string product;
       public string segment;
       public string fact;
       public string excelSheet;
       public string title1;
       public string title2;

       public string reportTyp2;
       public string product2;
       public string segment2;
       public string fact2;
       public string excelSheet2;
       public string title12;
       public string title22;

       public Boolean secondTableEnabled;
       public Boolean previousYearEnabled;
       public Boolean secondIsQVW;


       public string formula1;             // formula for 1st value
       public string unit1;                // unit of first value

       public string formula2;
       public string unit2;
       public Boolean secondValueEnabled;  // is second value enabled

       public Boolean borderEnabled;
       public Boolean sortationOnSecondValue;
       public Boolean descendantSorted;    // is sortation descendant
       public string upperBorder;          // highest value shown
       public string lowerBorder;          // lowest value shown

       public Boolean highestValueIsGreen; // is highest value green
       public Boolean colorationOnSecondValue;
       public Boolean colorationOnSecondTotal;
       // bordervalues between colors
       public string borderGreenLightgreen;
       public string borderLightgreenYellow;
       public string borderYellowOrange;
       public string borderOrangeRed;

       public string borderGreenTotal;
       public string borderLightgreenTotal;
       public string borderYellowTotal;
       public string borderOrangeTotal;

       public RowData(int rowId, System.Web.SessionState.HttpSessionState Session)
       {
           rowID = rowId;
           this.Session = Session;
           UpdateFromDB();
       }

       public void UpdateFromDB()
       {
           DBData db = DBData.getDBData(Session);
           db.connect();

           // does entry in db exist?
           if (db.lookup("ID", "GFK_FORMULA", "ROW_ID=" + rowID,"").ToString() == "")
           {
               string sql = "INSERT INTO GFK_FORMULA (ROW_ID) VALUES("+rowID+")";
               db.execute(sql);
           }// read data from db

               reportTyp                = db.lookup("REPORT_TYP",               "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               product                  = db.lookup("PRODUCT",                  "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               segment                  = db.lookup("SEGMENT",                  "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               fact                     = db.lookup("FACT",                     "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               excelSheet               = db.lookup("EXCELSHEET",               "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               title1                   = db.lookup("TABLENAME1",               "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               title2                   = db.lookup("TABLENAME2",               "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               formula1                 = db.lookup("FORMULA1",                 "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               unit1                    = db.lookup("UNIT1",                    "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               formula2                 = db.lookup("FORMULA2",                  "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               unit2                    = db.lookup("UNIT2",                    "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               formulaTotal             = db.lookup("FORMULA_TOTAL",            "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               unitTotal                = db.lookup("UNIT_TOTAL",               "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               upperBorder              = db.lookup("UPPER_BORDER",             "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               lowerBorder              = db.lookup("LOWER_BORDER",             "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               borderGreenLightgreen    = db.lookup("BORDER_GREEN_LIGHTGREEN",  "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               borderLightgreenYellow   = db.lookup("BORDER_LIGHTGREEN_YELLOW", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               borderYellowOrange       = db.lookup("BORDER_YELLOW_ORANGE",     "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               borderOrangeRed          = db.lookup("BORDER_ORANGE_RED",        "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               borderGreenTotal         = db.lookup("BORDER_GREEN_LIGHTGREEN_T","GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               borderLightgreenTotal    = db.lookup("BORDER_LIGHTGREEN_YELLOW_T","GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               borderYellowTotal        = db.lookup("BORDER_YELLOW_ORANGE_T",   "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               borderOrangeTotal        = db.lookup("BORDER_ORANGE_RED_T",      "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               numberOfEntries          = db.lookup("NUMBER_OF_ENTRIES",        "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               reportTyp2               = db.lookup("SECOND_REPORT_TYP",        "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               product2                 = db.lookup("SECOND_PRODUCT",           "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               segment2                 = db.lookup("SECOND_SEGMENT",           "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               fact2                    = db.lookup("SECOND_FACT",              "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               excelSheet2              = db.lookup("SECOND_EXCELSHEET",        "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               title12                  = db.lookup("SECOND_TABLENAME1",        "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               title22                  = db.lookup("SECOND_TABLENAME2",        "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               secondFormulaTotal       = db.lookup("SECOND_FORMULA_TOTAL",     "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
               secondUnitTotal          = db.lookup("SECOND_UNIT_TOTAL",        "GFK_FORMULA", "ROW_ID= " + rowID).ToString();

               countEnabled             = db.lookup("COUNT_ENABLED",            "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               TotalEnabled             = db.lookup("TOTAL_VALUE_ENABLED",      "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               secondValueEnabled       = db.lookup("SECOND_VALUE_ENABLED",     "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               descendantSorted         = db.lookup("DESCENDANT_SORTED",        "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               highestValueIsGreen      = db.lookup("HIGHEST_VALUE_IS_GREEN",   "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               isQvw                    = db.lookup("IS_QVW",                   "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               colorationOnSecondValue  = db.lookup("COLORATION_SECOND_VALUE",  "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               sortationOnSecondValue   = db.lookup("SORTATION_SECOND_VALUE",   "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               secondIsQVW              = db.lookup("SECOND_IS_QVW",            "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               secondTableEnabled       = db.lookup("SECOND_TABLE_ENABLED",     "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               previousYearEnabled      = db.lookup("PREVIOUS_YEAR_ENABLED",    "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               borderEnabled            = db.lookup("ENABLE_BORDER",            "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               colorationOnSecondTotal  = db.lookup("COLORATION_SECOND_TOTAL",  "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               secondTotalEnabled       = db.lookup("SECOND_TOTAL_ENABLED",     "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
               secondTotalHidden        = db.lookup("SECOND_TOTAL_HIDDEN",      "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
           db.disconnect();
       }

       // write data to database
       public void WriteToDB()
       {
           DBData db = DBData.getDBData(Session);
           db.connect();
           string isQvwOut                  = isQvw                 ? "1" : "0";
           string secondValueEnabledOut     = secondValueEnabled    ? "1" : "0";
           string descendantOrder           = descendantSorted      ? "1" : "0";
           string highestIsGreenOut         = highestValueIsGreen   ? "1" : "0";
           string enableTotalOut            = TotalEnabled          ? "1" : "0";
           string countEnabledOut           = countEnabled          ? "1" : "0";
           string borderEnabledOut          = borderEnabled         ? "1" : "0";
           string sortSecondValueOut        = sortationOnSecondValue ? "1" : "0";
           string colorSecondValueOut       = colorationOnSecondValue ? "1" : "0";
           string secondIsQVWOut            = secondIsQVW           ? "1" : "0";
           string secondTableEnabledOut     = secondTableEnabled    ? "1" : "0";
           string previousYearEnabledOut    = previousYearEnabled   ? "1" : "0";
           string colorationSecondTotalOut  = colorationOnSecondTotal ? "1" : "0";
           string secondTotalEnabledOut      = secondTotalEnabled     ? "1" : "0";
           string secondTotalHiddenOut      = secondTotalHidden     ? "1" : "0";

           upperBorder = upperBorder == "" ? "NULL" : upperBorder;
           lowerBorder = lowerBorder == "" ? "NULL" : lowerBorder;
           string sql = "UPDATE GFK_FORMULA SET SECOND_TOTAL_HIDDEN = '" + secondTotalHiddenOut + "' , SECOND_FORMULA_TOTAL = '" + secondFormulaTotal + "' , SECOND_UNIT_TOTAL = '" + secondUnitTotal + "' , SECOND_TOTAL_ENABLED = '" + secondTotalEnabledOut + "' , COLORATION_SECOND_TOTAL = '" + colorationSecondTotalOut + "' , SECOND_IS_QVW = '" + secondIsQVWOut + "' , PREVIOUS_YEAR_ENABLED = '" + previousYearEnabledOut + "' , SECOND_TABLE_ENABLED = '" + secondTableEnabledOut + "' , SORTATION_SECOND_VALUE = '" + sortationOnSecondValue + "' , COLORATION_SECOND_VALUE = '" + colorSecondValueOut + "' , ENABLE_BORDER = '" + borderEnabledOut + "' , UNIT_TOTAL ='" + unitTotal + "' , FORMULA_TOTAL = '" + formulaTotal + "' , COUNT_ENABLED ='" + countEnabledOut + "' , NUMBER_OF_ENTRIES= '" + numberOfEntries + "' , TOTAL_VALUE_ENABLED = '" + enableTotalOut + "' , BORDER_GREEN_LIGHTGREEN_T = '" + borderGreenTotal + "' , BORDER_LIGHTGREEN_YELLOW_T = '" + borderLightgreenTotal + "' , BORDER_YELLOW_ORANGE_T = '" + borderYellowTotal + "' , BORDER_ORANGE_RED_T = '" + borderOrangeTotal + "' , IS_QVW = '" + isQvwOut + "' , EXCELSHEET = '" + excelSheet + "' ,TABLENAME1 = '" + title1 + "' ,TABLENAME2 = '" + title2 + "' , REPORT_TYP = '" + reportTyp + "' , PRODUCT = '" + product + "' , SEGMENT = '" + segment + "' , FACT = '" + fact + "' , SECOND_EXCELSHEET = '" + excelSheet2 + "' ,SECOND_TABLENAME1 = '" + title12 + "' ,SECOND_TABLENAME2 = '" + title22 + "' , SECOND_REPORT_TYP = '" + reportTyp2 + "' , SECOND_PRODUCT = '" + product2 + "' , SECOND_SEGMENT = '" + segment2 + "' , SECOND_FACT = '" + fact2 + "' , FORMULA1 = '" + formula1 + "', UNIT1 = '" + unit1 + "', FORMULA2 = '" + formula2 + "', UNIT2 = '" + unit2 + "', SECOND_VALUE_ENABLED = '" + secondValueEnabledOut + "', DESCENDANT_SORTED = '" + descendantOrder + "', UPPER_BORDER = " + upperBorder + ", LOWER_BORDER = " + lowerBorder + ", HIGHEST_VALUE_IS_GREEN = '" + highestIsGreenOut + "' , BORDER_GREEN_LIGHTGREEN = '" + borderGreenLightgreen + "', BORDER_LIGHTGREEN_YELLOW = '" + borderLightgreenYellow + "', BORDER_YELLOW_ORANGE = '" + borderYellowOrange + "', BORDER_ORANGE_RED = '" + borderOrangeRed + "' WHERE ROW_ID = '" + rowID + "'";
           db.execute(sql);
           db.disconnect();
       }
   }

   //  class with data from qvw file
   public class QVWFileData
   {
       public ArrayList reports;
       public List<string> reportTypes;
       public List<string> Products;
       public List<string> Segments;
       public List<string> Facts;
       public string[] columnNames;
       public string path; 

       public QVWFileData(string filename)
       {
           reportTypes = new List<string>();
           Products = new List<string>();
           Segments = new List<string>();
           Facts = new List<string>();

           String qvwPath = Global.Config.getModuleParam("gfk", "GFKDataSourcePath", "");

           // get latest file
           var directory = new DirectoryInfo(qvwPath);
           var qvwFile = (from f in directory.GetFiles(filename + "*") orderby f.LastWriteTime descending select f).First();

           qvwPath = qvwFile.FullName;
           path = qvwPath;

           StreamReader reader = new StreamReader(qvwPath);
           string buffer;
           reports = new ArrayList();
           int tabLine = 0;
           int line = 0;
           Report rep = new Report(0);
           while (!reader.EndOfStream)
           {
               buffer = reader.ReadLine();
               line++;
               tabLine++;
               if (buffer.Substring(0, 2) == "TS") // Table Start
               {
                   rep = new Report(line);
                   tabLine = 0;
               }
               else if (buffer.Substring(0, 2) == "TE") // table end
               {
                   rep.endLine = line;
                   reports.Add(rep);
               }
               else
               {
                   switch (tabLine)
                   {
                       case 1:
                           string s = buffer.Substring(3);
                           if (s.Substring(0, 7) == "RUNNING")
                           {
                               rep.repTyp = 1;
                               rep.name = s;
                           }
                           else if (s.Substring(0, 10) == "MANAGEMENT")
                           {
                               rep.repTyp = 2;
                               rep.name = s;
                           }
                           else if (s.Substring(0, 8) == "STANDARD")
                           {
                               rep.repTyp = 3;
                               rep.name = s;
                           }
                           else if (s.Substring(0, 7) == "SEGMENT")
                           {
                               rep.repTyp = 4;
                               rep.name = s;
                           }
                           else if (s.Substring(0, 7) == "HITLIST")
                           {
                               rep.repTyp = 5;
                               rep.name = s;
                           }
                           else
                           {
                               rep.repTyp = 0;
                               rep.name = s;
                           }
                           break;
                       case 2:
                           rep.product = buffer.Substring(3);
                           break;
                       case 3:
                           rep.kanal = buffer.Substring(3);
                           break;
                       case 4:
                           rep.fact = buffer.Substring(3);
                           rep.startLine = line;
                           break;
                       default:
                           break;

                   }
               }
           }

           reader.Close();
           
           foreach (Report curRep in reports)
           {
               if (!reportTypes.Contains(curRep.name)){
                   reportTypes.Add(curRep.name);
               }
           }
       }

       // get list of products
       public void GetProducts(string reportTyp)
       {
           if (reportTyp != "")
           {
               foreach (Report curRep in reports)
               {
                   if (curRep.name == reportTyp && !Products.Contains(curRep.product))
                   {
                       Products.Add(curRep.product);
                   }
               }
           }
       }

       // get list of Segments
       public void GetSegments(string reportTyp, string product)
       {
           if (reportTyp != "" && product != "")
           {
               foreach (Report curRep in reports)
               {
                   if (curRep.name == reportTyp && curRep.product == product && !Segments.Contains(curRep.kanal))
                   {
                       Segments.Add(curRep.kanal);
                   }
               }
           }

       }


       // get list of Segments
       public void GetFacts(string reportTyp, string product, string Segment)
       {
           if (reportTyp != "" && product != "" && Segment != "")
           {
               foreach (Report curRep in reports)
               {
                   if (curRep.name == reportTyp && curRep.product == product && curRep.kanal == Segment && !Facts.Contains(curRep.fact))
                   {
                       Facts.Add(curRep.fact);
                   }
               }
           }

       }


       public string[] GetColumns(string first , string reportTyp, string product, string segment, string fact)
       {
           if (reportTyp != "" && product != "" && segment != "" && fact != "")
           {
               Report tableReport = new Report(0);
               foreach (Report curRep in reports)
               {
                   if (curRep.name == reportTyp && curRep.product == product && curRep.kanal == segment && curRep.fact == fact)
                   {
                       tableReport = curRep;
                       break;
                   }
               }

               StreamReader reader = new StreamReader(path);
               int i = 1;

               while (i < tableReport.startLine)
               {
                   reader.ReadLine();
                   i++;
               }

               string buffer = reader.ReadLine();
               while (buffer.Substring(0, 2) != "CH")
               {
                   buffer = reader.ReadLine();
               }

               string[] returnValue = new string[buffer.Split("\t".ToCharArray()).Length - 3];
               Array.Copy(buffer.Split("\t".ToCharArray()), 3, returnValue, 0, returnValue.Length);
               for (int j = 0; j < returnValue.Length; j++)
               {
                   returnValue[j] = "[" + first + j + "] " + returnValue[j].Replace("<", "&lt").Replace(">", "&gt");
               }
               columnNames = returnValue;
               return returnValue;

           }
           else
           {
               return new string[0];
           }
           
       }
   }

    // class with info of 1 report
   public class Report
   {
       public long startLine;
       public long endLine;
       public int repTyp;
       public string name;
       public string fact;
       public string kanal;
       public string product;
       public int druckSt;
       public string srNr;
       public string warengruppe;
       public string periode;

       public Report(int startPos)
       {
           startLine = startPos;
       }

   }

   public class ExcelData
   {
       public Workbook wb;

       public ExcelData(string filename)
       {
           String excelPath = Global.Config.getModuleParam("gfk", "GFKDataSourcePath", "");

           // get latest file
           var directory = new DirectoryInfo(excelPath);
           var File = (from f in directory.GetFiles(filename + "*") orderby f.LastWriteTime descending select f).First();

           excelPath = File.FullName;

           wb = new Workbook();
           wb = Workbook.Load(excelPath);

       }

   }
}