using ch.appl.psoft.db;
using ch.psoft.Util;
/// TODO: language
///
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.SessionState;
using Excel = Microsoft.Office.Interop.Excel;

namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for DutyCatalogReport.
    /// </summary>
    public class ResponsabilitiesMatrixReport 
    {

        //attributes

        const string FILE_EXTENSION = ".xls";


        protected HttpSessionState _session;
        protected string _imageDirectory;
        protected DBData _db; 

        string _filename = "";


        Hashtable dutyGroups = new Hashtable();


        Function[] funtionList = null;
        //int[] maxSizes;


        MatrixReportInterface reportInterface = new MatrixReportInterface();

 
        long orgUnitID;
        string orgUnitTitle = "Alle Organisationseinheit"; 


        public string Filename
        {
            get{ return _filename;}
        }


        /// <summary>
        /// constructor
        /// </summary>        
        public ResponsabilitiesMatrixReport(HttpSessionState Session, string imageDirectory, long orgUnitId) 
        {
            this._session = Session;
            this._imageDirectory = imageDirectory;
            this.orgUnitID = orgUnitId;

            _db = DBData.getDBData(Session);

            ///all function ids         
            string allfuncIdsSql = "select f.id from duty_competence_validity cv, funktion f, job j where cv.funktion_id = f.id and j.funktion_id = f.id  group by f.id";
            if(orgUnitId > 0) 
            { //organisation unit restriction
                allfuncIdsSql = "select f.id from duty_competence_validity cv, funktion f, job j where cv.funktion_id = f.id and j.funktion_id = f.id and j.orgentity_id = " + orgUnitId + "group by f.id";
                orgUnitTitle = (string)_db.lookup(_db.langAttrName("ORGENTITY", "TITLE"),"ORGENTITY","id = " + orgUnitId);
            }
            DataTable allFunctionIdsTable = _db.getDataTableExt(allfuncIdsSql, "funktion");
            funtionList = new Function[allFunctionIdsTable.Rows.Count];
            for(int k = 0; k < allFunctionIdsTable.Rows.Count; k++)
            {
                long curFktId = (long)allFunctionIdsTable.Rows[k][0];    
                string curDes = (string)_db.lookup("title_de", "funktion","id="+curFktId);
                Function curfkt = new Function(curDes,curFktId);
                funtionList[k] = curfkt;
            }


            //all duties
            string alldutiesSql= "select dv.id, dv.description_de, dg.id as dgId, dg.title_de, d.id as DUTY_ID from duty d, duty_validity dv, dutygroup dg where dv.duty_id = d.id and d.dutygroup_id = dg.id  and d.id in (select duty_id from duty_competence_validity where funktion_id in ( " + allfuncIdsSql + " ) group by duty_id) order by dg.title_de";
            DataTable allDutyIdsTable = _db.getDataTableExt(alldutiesSql, "duty");
            for(int k = 0; k < allDutyIdsTable.Rows.Count; k++)
            {
                long dutyGroupkey = (long)allDutyIdsTable.Rows[k][2];
                long dutyKey = (long)allDutyIdsTable.Rows[k][4]; //not the duty_validity key but the duty key
                if ( !this.dutyGroups.ContainsKey(dutyGroupkey) ) 
                {
                    this.dutyGroups.Add(dutyGroupkey,new ItemGroup((string)allDutyIdsTable.Rows[k][3],dutyGroupkey));
                }
                ((ItemGroup)this.dutyGroups[dutyGroupkey]).addItem(new Duty((string)allDutyIdsTable.Rows[k][1],dutyKey));
            }

        }

      

        public void createReport() 
        {
            //1. step: Add title
            MatrixReportInterface.Cell[] title = new MatrixReportInterface.Cell [1];
            title[0] = new MatrixReportInterface.Cell(this.orgUnitTitle,true);
            reportInterface.addRow(title, false);
            reportInterface.addRow(new MatrixReportInterface.Cell [1], true);


            //2. step: Add all funtions
            MatrixReportInterface.Cell[] cells = new MatrixReportInterface.Cell [this.funtionList.Length + 1];
            //first cell is empty or contains a description
            cells[0] = new MatrixReportInterface.Cell("", true); //Kompetence/Funktion
            for(int l = 1; l < cells.Length; l++) 
            {
                cells[l] = new MatrixReportInterface.Cell(this.funtionList.GetValue(l-1).ToString(),true);
            }
            reportInterface.addRow(cells,true);

            //3. step: Add all competence and values
            foreach(DictionaryEntry entry in this.dutyGroups)
            {
                ItemGroup dutyGroup = (ItemGroup)entry.Value;
                //Title (Parent Aufgabe)
                MatrixReportInterface.Cell[] aufgabeTitles = new MatrixReportInterface.Cell [this.funtionList.Length + 1];
                aufgabeTitles[0] = new MatrixReportInterface.Cell(dutyGroup.ToString(),true);
                for(int m = 1; m <  aufgabeTitles.Length; m++) aufgabeTitles[m] = new MatrixReportInterface.Cell("",false);
                reportInterface.addRow(aufgabeTitles,false);
                //Aufgaben
                reportItems(dutyGroup.getList());

            }
         }

      void reportItems(ExcelItem[] dutyList) 
      {
            for(int k = 0; k < dutyList.Length; k++) 
            {
                MatrixReportInterface.Cell[] cells = new MatrixReportInterface.Cell [this.funtionList.Length + 1];
                Duty cpt = (Duty)dutyList.GetValue(k);
                cells[0] = new MatrixReportInterface.Cell(cpt.ToString(), false);       

                for(int l = 0; l < this.funtionList.Length; l++) 
                {
                    cells[l+1] = new MatrixReportInterface.Cell(calculateValue( (Function)this.funtionList.GetValue(l), (Duty)dutyList.GetValue(k) ) , false );
                }
                reportInterface.addRow(cells,true);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputDirectory"></param>
        /// <param name="fileName"></param>
        /// <returns>path where the file has been saved</returns>
        public string saveReport(string outputDirectory, string fileName) 
        {
            this._filename = fileName + "_" + ch.appl.psoft.Interface.SessionData.getSessionID(_session) + FILE_EXTENSION;
            reportInterface.printToExcel(outputDirectory + "/" + this._filename);
            /*
            System.IO.FileStream targetFile = new System.IO.FileStream(outputDirectory + "/" + this._filename, System.IO.FileMode.OpenOrCreate);
            reportInterface.printToFile(targetFile);
            */
            return outputDirectory + "/" + this._filename;
        }

        public string calculateValue(Function f, Duty c) 
        {
            long fktId = f.ID;
            long dutyId = c.ID; 
            string ret = "";
            string sql = "select id from DUTY_COMPETENCE_VALIDITY where funktion_id = " +  fktId  + "and duty_id = " + dutyId;
            DataTable data = _db.getDataTableExt(sql, "DUTY_COMPETENCE_VALIDITY");
            if(data.Rows.Count > 0) 
            {
                sql = "select c.*, cl.* from competence c, competence_level cl where cl.id = c.competence_level_id and duty_competence_validity_id = " + data.Rows[0][0];
                data = _db.getDataTableExt(sql, "competence");
                for(int k = 0; k < data.Rows.Count; k++) 
                {
                    ret += (string) data.Rows[k]["MNEMO_DE"]; 
                    if(k < data.Rows.Count -1) { ret += "/"; }
                }
            }
            return ret;
        }      
    }




    /// <summary>
    /// Class performing writing on document
    /// </summary>
    public class MatrixReportInterface
    {

        System.Collections.ArrayList listOfRows = new System.Collections.ArrayList();

        public MatrixReportInterface() 
        {
        }


        public class Cell 
        {
            public Cell(string text, bool bold) 
            {
                this.text = text;
                this.isBold = bold;
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

    //=========================================================================
    //utility classes:

    /// <summary>
    /// 
    /// </summary>
    public class ExcelItem 
    {
        protected string description; 

        protected long id;

        public ExcelItem(string description, long id)
        {
            this.description = description;
            this.id = id;
        }

        public long ID 
        {
            get { return this.id; }
        }

        override public string ToString() 
        {
            return description;
        }
      
    }

    /// <summary>
    /// 
    /// </summary>
    public class Duty : ExcelItem
    {
        public Duty(string description, long id)
            : base(description, id)
        {
        }


    }

    /// <summary>
    /// 
    /// </summary>
    class ItemGroup  : ExcelItem
    {
        ArrayList items = new ArrayList();

        public ItemGroup( string description, long id) 
            : base(description, id)
        {        
        }
        public void addItem(ExcelItem item ) 
        {
            items.Add(item);
        }

        public ExcelItem[] getList() 
        {
            return (ExcelItem[])this.items.ToArray(Type.GetType("ch.appl.psoft.FBS.ExcelItem"));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Function : ExcelItem
    {
     
        public Function(string description, long id)  : base(description, id)
        {  
        }
        override public string ToString() 
        {
            return description;
        }

    }

}
