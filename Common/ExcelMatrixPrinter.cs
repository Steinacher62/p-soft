using ch.psoft.Util;
/// ===========================================================================
/// Author:       Mattia Jermini
/// Date:         6. Februar 2008
/// Description:  Given a matrix print out an excel sheet.
/// ===========================================================================

using System;
using System.Collections;
using System.Drawing;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;



namespace ch.appl.psoft.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class ExcelMatrixPrinter 
    {

     
        
        string _url = "";
        public string Url 
        {
            get { return this._url; }
        }

        string _path= "";
        public string Path 
        {
            get { return this._path; }
        }

        string _filename = "";

        public string Filename 
        {
            get { return this._filename; }
        }
      
        const string FILE_EXTENSION = ".xls";

 
        /// <summary>
        /// 
        /// </summary>
        MatrixReportInterface reportInterface = new MatrixReportInterface();

        /// <summary>
        /// constructor
        /// </summary>        
        public ExcelMatrixPrinter() 
        {
        }


        public void addTitle(MatrixReportInterface.Cell title) 
        {
            //title row
            MatrixReportInterface.Cell[] cellTitle = new MatrixReportInterface.Cell [1];
            cellTitle[0] = title;

            reportInterface.addRow(cellTitle, false);
            reportInterface.addRow(new MatrixReportInterface.Cell [1], true);
        }
 
        public void addItems(MatrixReportInterface.Cell[] items, int numberOfItemsPerRow)
        {
            int rows = items.Length / numberOfItemsPerRow;
            for(int l = 0; l < rows; l++)
            {
                MatrixReportInterface.Cell[] cells = new MatrixReportInterface.Cell [numberOfItemsPerRow];
                for( int k = 0; k < numberOfItemsPerRow; k++ ) 
                {
                    cells[k] = items[l*numberOfItemsPerRow+k];
                }
                reportInterface.addRow(cells,false);
            }
        }
 

        public void addEmptyRow() 
        {
            reportInterface.addRow(new MatrixReportInterface.Cell [1], true);
        }
      
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputDirectory"></param>
        /// <param name="fileName"></param>
        /// <returns>path where the file has been saved</returns>
        public ExcelMatrixPrinter saveReport(System.Web.HttpRequest request, int sessionId, string fileName) 
        {
            string outputDirectory = request.MapPath("~" + ch.appl.psoft.Report.ReportModule.REPORTS_DIRECTORY); 

            _filename = fileName + "_" + sessionId + FILE_EXTENSION;
            _path = outputDirectory + "/" + _filename;
            reportInterface.printToExcel(_path);
   
            _url = Global.Config.baseURL + ch.appl.psoft.Report.ReportModule.REPORTS_DIRECTORY + "/" + this.Filename;
            return this;
        }

    }




    /// <summary>
    /// Class performing writing on document
    /// </summary>
    public class MatrixReportInterface
    {

        System.Collections.ArrayList listOfRows = new System.Collections.ArrayList();

        System.Collections.ArrayList colors = new System.Collections.ArrayList();


        public MatrixReportInterface() 
        {
        }


        public class Cell 
        {
            public Cell(string text)
            {
                this.text = text;
                this.isBold = false;
                this.color = Color.White;

            }

            public Cell(string text, bool bold, Color color) 
            {
                this.text = text;
                this.isBold = bold;
                this.color = color;
            }

            public bool IsBold 
            {
                get { return this.isBold; }
            }

            override public string ToString() 
            {
                return text;
            }
            bool isBold = false;
            string text;
            Color color;
            public Color ColorValue 
            {
                get { return color; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        class Row 
        {
            public Row(Cell[] cells, bool downMargin) 
            { 
                this.cells = cells; 
                this.numberOfCells = cells.Length;
                this.merged = false;
                this.downMargin = downMargin; 
            }

            public Row(Cell cell, int numberOfCells, bool downMargin) 
            { 
                this.cells = new Cell[1];
                this.cells[0] = cell; 
                this.numberOfCells = numberOfCells;
                this.merged = true;
                this.downMargin = downMargin; 
            }

            public bool IsMerged 
            {
                get { return this.merged; }
            }
            public Cell[] Cells 
            {
                get { return this.cells; }
            }
            public int NrOfCells 
            {
                get { return this.numberOfCells; }
            }
            public bool HasDownMargin 
            {
                get { return this.downMargin; }
            }

    


            Cell[] cells;
            int numberOfCells;
            bool downMargin = false;
            bool merged = false;
   
        }


  
        public void addRow(Cell[] cells, bool downMargin) 
        {
            //add
            listOfRows.Add(new Row(cells,downMargin));

            //automatically fill in color palette array (used later by 
            //createCustomPalette)
            foreach(Cell cell in cells) 
            {
                if(cell != null && cell.ColorValue != Color.White && !colors.Contains(cell.ColorValue)) 
                {
                   colors.Add(cell.ColorValue);
                }
            }
        }

        
        /// <summary>
        /// Set the custom color palette for this excel sheet
        /// </summary>
        /// <param name="wb"></param>
        protected void createCustomPalette(Excel.Workbook wb)
        {
            if(colors == null) return;
            wb.set_Colors(1,Color.White.ToArgb());           
            int i = 2;
            foreach (Color color in colors)
            {
                wb.set_Colors(i,ColorTranslator.ToOle(color));
                i++;
                //it is possible to define up to 56 colors in excel.
                if(i == 56) break;
            }
        }

        /// <summary>
        /// Print to an excel file using MS automation.
        /// </summary>
        /// <param name="fileName"></param>
        public void printToExcel(string fileName) 
        {
            Excel.ApplicationClass excel = new Excel.ApplicationClass();
            excel.Visible = false;				
            excel.DisplayAlerts = false;
            //Get a new workbook.
            Excel.Workbook wb = (Excel.Workbook)(excel.Workbooks.Add(oMissing));	
            createCustomPalette(wb);
            try
            {         
                object oMergeFileName = fileName;
				
                Excel.Worksheet sheet = (Excel.Worksheet) wb.ActiveSheet;
                
                
                // --I-- first calculate column width
                ArrayList columnSizes = new ArrayList();
                for(int k = 0; k < this.listOfRows.Count; k++) 
                {
                    Row row = (Row)this.listOfRows[k];
                    for(int j = 0; j < row.Cells.Length; j++) 
                    {
                        double ffactor = 0.1;
                        double fs = (double)sheet.Cells.Font.Size * ffactor;
                        double cw = (double)sheet.Cells.ColumnWidth;
                        //consider the longest line only
                        if(row.Cells[j] == null) continue; 
                        string str = row.Cells[j].ToString();
                        if(str == null) continue;
                        string[] stra = System.Text.RegularExpressions.Regex.Split(str,@"[\r\n]+");
                        int len = 0;
                        for (int l = 0; l < stra.Length; l++) 
                        {
                            len = len<stra[l].Length?stra[l].Length:len;
                        }
                        
                        double wi = (len * fs) > cw?(len * fs):cw;
                        if(j >= columnSizes.Count) 
                        {
                            columnSizes.Add( wi );
                        }
                        else 
                        {
                            if( ((double)columnSizes[j]) < wi )  columnSizes[j] = wi;
                        }
                    }
                }
                object[] allsizes = columnSizes.ToArray();
                for(int i = 0; i<allsizes.Length; i++) //column 
                {
                    double size = (double)allsizes[i];
                    //resize column in order to fit
                    
                    //Spaltenbreite muss zwischen 0 und 255 Zeichen liegen
                    ((Excel.Range) sheet.Cells[1,1+i]).ColumnWidth = size<255?size:255;

                }

                
                
                // --II-- then write to the sheet
                for(int k = 0; k < this.listOfRows.Count; k++) 
                {
                    Row row = (Row)this.listOfRows[k];
                    if(row.HasDownMargin) 
                    {
                        //TODO margin currently unused
                    }
                    for(int j = 0; j < row.Cells.Length; j++) 
                    {
                        if(row.Cells[j] == null) continue;
                        if(row.Cells[j].IsBold) 
                        {
                            Excel.Range cell = (Excel.Range) sheet.Cells[k+1,j+1];
                            cell.Font.Bold = true;
                        }  
                        Excel.Range cell_ = (Excel.Range) sheet.Cells[k+1,j+1];
                        Color tempColor = row.Cells[j].ColorValue;
                        if(tempColor != Color.White) 
                        {
                            // excel uses its own color logic
//                            int excelcolor = tempColor.B << 16;
//                            excelcolor = excelcolor | tempColor.G << 8;
//                            excelcolor = excelcolor | tempColor.R;
//                            cell_.Interior.Color = (double)excelcolor;
                            cell_.Interior.Color = ColorTranslator.ToOle(tempColor);
                        }
                        //System.Drawing.KnownColor
                        sheet.Cells[k+1,j+1] = row.Cells[j].ToString();  
                    }
                }

                // --III-- save the file
                wb.SaveAs(oMergeFileName, oMissing, oMissing, oMissing, oFalse, oMissing, Excel.XlSaveAsAccessMode.xlExclusive, oMissing, oTrue, oMissing, oMissing, oMissing);
                wb.Saved = true;
                object rel = sheet;
                ReleaseCom(ref rel);
            }
            catch(Exception e) 
            {
                Logger.Log(e, Logger.WARNING);
            }
            finally 
            {
                wb.Close(oFalse, oMissing, oMissing);
                excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject (excel);
                excel = null;
                GC.Collect();
            }

        }

        /// <summary>
        /// Procedure to generate an CSV file (no automation required).
        /// </summary>
        /// <param name="output"></param>
        public void printToFile(System.IO.FileStream output)
        {
           
            foreach(Row row in this.listOfRows) 
            {
                foreach(Cell cell in row.Cells) 
                {
                    byte[] info = (new UTF8Encoding(true)).GetBytes(cell.ToString() + ";");
                    output.Write( info,0,info.Length);
                }
                output.WriteByte((byte)'\n');
            }
            output.Close();
        }


        protected object oFalse = false;
        protected object oTrue = true;
        protected object oMissing = System.Reflection.Missing.Value;

        private void ReleaseCom(ref object o) 
        { 
            try 
            { 
                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(o) > 0); 
            } 
            catch (Exception ex)
            {
                Logger.Log(ex,Logger.ERROR);
            } 
            finally 
            { 
                o = null; 
            } 
        } 
    }






 
}
