using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Web.SessionState;
using Excel = Microsoft.Office.Interop.Excel;

namespace ch.appl.psoft.Report
{
    /// <summary>
    /// Summary description for PDFList.
    /// </summary>
    public class EXCELList : XLSList {
        protected DataTable _layout = new DataTable();
        protected DataTable _header = new DataTable();
        protected DataTable _column = new DataTable();
        protected DataTable _data = new DataTable();
        protected LanguageMapper _map = null;
        private ReportModule.Layout _listLayout = ReportModule.Layout.Uniform;
        private int _indexRowFromTop = 0;
        private static string[] COLORS = {"0","15"}; // palette index
        private string _bgColor = COLORS[0];
        private static double CM2P_FACTOR = 5.1425;
        private static double OFFSET = -0.71; // bill gates told so
        private double _areaWidth = 0;
        private ArrayList _columnWidthsList = new ArrayList();
        private double _columnWidthsSum = 0;
        private int _columnVariableWidthsCount = 0;

        public EXCELList() {
        }
        public void writeList(string title, string path, LanguageMapper map, DBData db, DataTable data, HttpSessionState session, params string[] substituteValues) {
            int id = (int) db.lookup("id","reportlayout","title='"+title+"'");
            writeList(id,path,"",map,db,data,session,substituteValues);
        }

        public override void writeList(long id, string path, string imagePath, LanguageMapper map, DBData db, DataTable data, HttpSessionState session, params string[] substituteValues) 
		{
            _indexRowFromTop = 1;

            try {

                base.open(id,path,db,session);
                _map = map;
                _data = data;
                if (!base.prepareLayout(id, out _layout, out _header, out _column, ref _data, substituteValues)) return;

                if ((ReportModule.ReportType) _layout.Rows[0]["type"] != ReportModule.ReportType.ListExcel) return;

                _listLayout = (ReportModule.Layout) _layout.Rows[0]["layout"];

                // pageSetup
                /* Some systemconfigurations may cause a mysterious effect when PageSetup is used.
                 * The error is hardly to reproduce. Because this feature is near to nice-to-have, it 
                 * is now commented out.                
                _worksheet.PageSetup.PaperSize = Excel.XlPaperSize.xlPaperA4;
                _worksheet.PageSetup.LeftMargin = CM2P_FACTOR * 0.5;
                _worksheet.PageSetup.RightMargin = CM2P_FACTOR * 0.5;
                _worksheet.PageSetup.TopMargin = CM2P_FACTOR * 0.2;
                _worksheet.PageSetup.BottomMargin = CM2P_FACTOR * 0.5;
                _worksheet.PageSetup.FirstPageNumber = 1;
                */
                switch (DBColumn.GetValid(_layout.Rows[0]["format"],0))
                {
                    case (int) ReportModule.Format.Portrait:
                        /*_worksheet.PageSetup.Orientation = Excel.XlPageOrientation.xlPortrait;*/
                        _areaWidth = CM2P_FACTOR * 21;
                        break;
                    case (int) ReportModule.Format.Landscape:
                        /*_worksheet.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;*/
                        _areaWidth = CM2P_FACTOR * 29.7;
                        break;
                }
                /* Due language dependency of the Excel:"Formatting Codes for Headers and Footers"
                 * the footer should not be set without proper languagesettings and programversions handling (serverside).
                 * 
                 * int languagecode = _excelApplication.LanguageSettings.get_LanguageID(Microsoft.Office.Core.MsoAppLanguageID.msoLanguageIDUI);
                 * 1031: german [&Seite]/[&Seiten] = &s/&a
                 * 1033: english ..                = &p/&n
                 * 
                 * 
                _worksheet.PageSetup.LeftFooter = "&8" + _map.get("reportLayout", _layout.Rows[0]["title_mnemo"].ToString()) + " \u00a9 p-soft";
                _worksheet.PageSetup.CenterFooter = "&8" + _map.get("reportLayout", "stand") + ": " + DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                _worksheet.PageSetup.RightFooter = "&8" + _map.get("reportLayout", "page") + " &s/&a";
                */


                // header
                writeHeader(substituteValues);              

				// body
                string w = writeColumnHeader();               
                writeList(w);
			
				base.saveXLS();
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
                throw e;
            }
            finally {
                base.close();
            }
        }

        public void AddItemToSpreadsheet(int row, int column, Excel.Worksheet ws, string item, string font, string fontsize, ReportModule.HAlign halign , ReportModule.VAlign valign, string rowheight, string columnwidth)
        {
            ((Excel.Range)ws.Cells[row, column]).Value2 = item;
            if (font != "") 
            {
                ((Excel.Range)ws.Cells[row, column]).Font.Name = font;
                if (font.ToLower().EndsWith("bold")) ((Excel.Range)ws.Cells[row, column]).Font.Bold = true;
                if (font.ToLower().EndsWith("italic")) ((Excel.Range)ws.Cells[row, column]).Font.Italic = true;
                if (font.ToLower().EndsWith("strikeout")) ((Excel.Range)ws.Cells[row, column]).Font.Strikethrough = true;
                if (font.ToLower().EndsWith("subscript")) ((Excel.Range)ws.Cells[row, column]).Font.Subscript = true;
                if (font.ToLower().EndsWith("superscript")) ((Excel.Range)ws.Cells[row, column]).Font.Superscript = true;
            }
            if (fontsize != "") ((Excel.Range)ws.Cells[row, column]).Font.Size = fontsize;
            switch(halign)
            {
                case ReportModule.HAlign.Left:
                    ((Excel.Range)ws.Cells[row, column]).HorizontalAlignment = Excel.Constants.xlLeft;
                    break;
                case ReportModule.HAlign.Center:
                    ((Excel.Range)ws.Cells[row, column]).HorizontalAlignment = Excel.Constants.xlCenter;
                    break;
                case ReportModule.HAlign.Right:
                    ((Excel.Range)ws.Cells[row, column]).HorizontalAlignment = Excel.Constants.xlRight;
                    break;
            }
            switch(valign)
            {
                case ReportModule.VAlign.Top:
                    ((Excel.Range)ws.Cells[row, column]).VerticalAlignment = Excel.Constants.xlTop;
                    break;
                case ReportModule.VAlign.Middle:
                    ((Excel.Range)ws.Cells[row, column]).VerticalAlignment = Excel.Constants.xlCenter;
                    break;
                case ReportModule.VAlign.Bottom:
                    ((Excel.Range)ws.Cells[row, column]).VerticalAlignment = Excel.Constants.xlBottom;
                    break;
            }
            if (rowheight != "") ((Excel.Range)ws.Cells[row, column]).RowHeight = rowheight;

//            if (columnwidth != "" && columnwidth != "*")
//            {
//                bool isCm = false;
//                if (columnwidth.ToLower().EndsWith("cm"))
//                {
//                    columnwidth = columnwidth.ToLower().Replace("cm","");
//                    isCm = true;                   
//                }
//                double cw = -1;
//                try 
//                {
//                    cw = System.Double.Parse(columnwidth);
//                } 
//                catch(Exception e){
//                    e.ToString();
//                }
//                if (cw > -1) 
//                {
//                    if (isCm)
//                    {
//                        cw = -0.71 + CM2P_FACTOR * cw; // bill gates told so
//                    }
//                    columnwidth = cw.ToString();
//                    ((Excel.Range)ws.Cells[row, column]).ColumnWidth = columnwidth; // points (1/72 of an inch)
//                }   
//            }           
            if (columnwidth != "" && _columnWidthsList.Count >= column)
            {
                int idx = column-1;
                string cW = _columnWidthsList[idx].ToString();
                double cw = System.Double.Parse(cW);
                cw = cw + OFFSET;
                cw = Math.Round(cw,2);
                Logger.Log("EXCEL: ColWidth="+cw,Logger.DEBUG);
                ((Excel.Range)ws.Cells[row, column]).ColumnWidth = cw.ToString();
            }

            if (_listLayout == ReportModule.Layout.Computer)
            {
                ((Excel.Range)ws.Cells[row, column]).Interior.ColorIndex = _bgColor;
            }
           
        }

        

        public void AddItemToSpreadsheet(int row, int column, Excel.Worksheet ws, string item, string font, string fontsize, ReportModule.HAlign halign , ReportModule.VAlign valign, string rowheight)
        {
            AddItemToSpreadsheet(row, column, ws, item, font, fontsize, halign, valign, rowheight, "");
        }

		public void AddItemToSpreadsheet(int row, int column, Excel.Worksheet ws, string item)
		{
            AddItemToSpreadsheet(row, column, ws, item, "", "", ReportModule.HAlign.Left, ReportModule.VAlign.Top, "");            
        }


        private void writeHeader(string[] substituteValues) 
        {
            int rownum = -1;
            _bgColor = COLORS[0];
            for (int r = 0; r < _header.Rows.Count; r++) 
            {
                DataRow row = _header.Rows[r];

                if ((int) row["rownumber"] != rownum) 
                {
                    rownum = (int) row["rownumber"];
                    for (int rr = r; rr < _header.Rows.Count; rr++) 
                    {
                        DataRow tmpRow = _header.Rows[rr];

                        if ((int) tmpRow["rownumber"] != rownum) break;
                    }
                }
                string font = DBColumn.GetValid(row["font"],"");
                string fontsize = DBColumn.GetValid(row["fontsize"],"");
                string rowheight = DBColumn.GetValid(row["rowheight"],"");
                ReportModule.HAlign halign = (ReportModule.HAlign) row["halign"];
                ReportModule.VAlign valign = (ReportModule.VAlign) row["valign"];

                _indexRowFromTop += rownum;
                AddItemToSpreadsheet(_indexRowFromTop
                    ,1
                    ,_worksheet
                    ,base.substitute(_map.get("reportLayout", row["text_mnemo"].ToString()),substituteValues)
                    ,font
                    ,fontsize
                    ,halign
                    ,valign
                    ,rowheight);

            }
        }

		private string writeColumnHeader() {
            int indexColumn = 1;
            _indexRowFromTop++;
            _bgColor = COLORS[0];
            foreach (DataRow row in _column.Rows) 
            {
                if (!DBColumn.IsNull(row["headername_mnemo"])) 
                {
                    prepareColumnWidths(DBColumn.GetValid(row["columnwidth"],"*"));
                }
            }
            calculateColumnWidths();
            foreach (DataRow row in _column.Rows) 
            {
                if (!DBColumn.IsNull(row["headername_mnemo"])) 
                {
                    string font = DBColumn.GetValid(row["headerfont"],"");
                    string fontsize = DBColumn.GetValid(row["headerfontsize"],"");
                    string rowheight = DBColumn.GetValid(row["headerrowheight"],"");
                    ReportModule.HAlign halign = (ReportModule.HAlign) row["headerhalign"];
                    ReportModule.VAlign valign = (ReportModule.VAlign) row["headervalign"];
                    string columnwidth = DBColumn.GetValid(row["columnwidth"],"");

                    AddItemToSpreadsheet(_indexRowFromTop
                        ,indexColumn++
                        ,_worksheet
                        ,_map.get("reportLayout", row["headername_mnemo"].ToString())
                        ,font
                        ,fontsize
                        ,halign
                        ,valign
                        ,rowheight
                        ,columnwidth);
                 }
            }
			return "";
        }

        private void prepareColumnWidths(string columnwidth)
        {
            if (columnwidth != "*")
            {
                bool isCm = false;
                if (columnwidth.ToLower().EndsWith("cm"))
                {
                    columnwidth = columnwidth.ToLower().Replace("cm","");
                    isCm = true;                   
                }
                double cw = -1;
                try 
                {
                    cw = System.Double.Parse(columnwidth,DBColumn.DBCulture);
                    Logger.Log("EXCEL: prepareColumnWidthsParse="+cw,Logger.DEBUG);
                } 
                catch(Exception e)
                {
                    Logger.Log("EXCEL: prepareColumnWidths="+e.Message.ToString(),Logger.ERROR);
                }
                if (cw > -1) 
                {
                    if (isCm)
                    {
                        cw = CM2P_FACTOR * cw; // cm to point
                        Logger.Log("EXCEL: prepareColumnWidthsMultiplication="+cw,Logger.DEBUG);
                   
                    }
                    columnwidth = cw.ToString();
                    _columnWidthsSum += cw;
                }   
            }
            else 
            {
                _columnVariableWidthsCount++;
            }
            _columnWidthsList.Add(columnwidth);
        }

        private void calculateColumnWidths()
        {
            if (_columnVariableWidthsCount > 0)
            {
                double diff = _areaWidth - _columnWidthsSum;
                double width = (diff > 0)? (diff / _columnVariableWidthsCount - 1) : (CM2P_FACTOR);

                for(int c=0; c < _columnWidthsList.Count; c++)
                {
                    string w = _columnWidthsList[c].ToString();                   
                    if (w == "*")
                    {
                        _columnWidthsList.RemoveAt(c);
                        _columnWidthsList.Insert(c,width.ToString());
                    }
                }
            }
        }

		private void writeList(string widths) {
            int rowNum = 0;
            foreach(DataRow row in _data.Rows)
            {
                _indexRowFromTop++;              
                _bgColor = COLORS[rowNum % 2];
                rowNum++;
                int indexColumn = 0;
                foreach (DataRow cell in _column.Rows) 
                {
                    string font = DBColumn.GetValid(cell["font"],"");
                    string fontsize = DBColumn.GetValid(cell["fontsize"],"");
                    string rowheight = DBColumn.GetValid(cell["rowheight"],"");
                    ReportModule.HAlign halign = (ReportModule.HAlign) cell["halign"];
                    ReportModule.VAlign valign = (ReportModule.VAlign) cell["valign"];
                    int idx = 0;
                    string pattern = DBColumn.IsNull(cell["formatpattern"]) ? "" : cell["formatpattern"].ToString();
                    string text = pattern;

                    foreach (string attr in cell[_db.langAttrName("reportcolumn", "attributname")].ToString().Split(',')) 
                    {
                        if (!DBColumn.IsNull(row[attr])) 
                        {
                            string display = _db.dbColumn.GetDisplayValue(_data.Columns[attr],row[attr],false);
                            if (pattern == "") text += display;
                            else text = text.Replace("$"+idx,display);
                        }
                        else if (pattern != "") 
                        {
                            int i = text.IndexOf("$"+idx);
                            int len = text.Length;

                            while (i >= 0) 
                            {
                                text = text.Remove(i,1);
                                if (text.Length == i || text[i] == '$') break;
                            }
                        }
                        idx++;
                    }
                    indexColumn++;
                    AddItemToSpreadsheet(_indexRowFromTop
                        ,indexColumn
                        ,_worksheet
                        ,text
                        ,font
                        ,fontsize
                        ,halign
                        ,valign
                        ,rowheight);
                }
                    
            }
        }
    }
}
