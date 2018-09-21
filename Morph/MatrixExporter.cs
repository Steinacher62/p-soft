using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.db;
using ch.psoft.Util;
using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.SessionState;
namespace ch.appl.psoft.Morph
{
    public class MatrixExporter
    {
        // Fields
        protected DBData _db;
        private LanguageMapper _mapper;
        private long _matrixId = -1L;
        private long _maxCellNumber = -1L;
        private long _slaveId = -1L;

        // Methods
        public MatrixExporter(HttpSessionState Session, long matrixId, long slaveId)
        {
            this._db = DBData.getDBData(Session);
            this._matrixId = matrixId;
            this._slaveId = slaveId;
            this._mapper = LanguageMapper.getLanguageMapper(Session);
        }

        protected void addCharacteristic(long characteristicID, Worksheet sheet, long row, long column)
        {
            sheet.Cells[row, column] = this._db.Characteristic.getTitle(characteristicID);
            Range range = (Range)sheet.Cells[row, column];
            if (this._slaveId > 0L)
            {
                range.Interior.Color = ColorTranslator.ToOle(this._db.Characteristic.getSlaveColor(this._slaveId, characteristicID));
            }
            range.RowHeight = 60;
            range.ColumnWidth = 60;
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = XlVAlign.xlVAlignCenter;
            range.WrapText = true;
            range.UseStandardWidth = true;
            range.UseStandardHeight = true;
        }

        protected void addDimension(long dimensionID, Worksheet sheet, long currentRow, bool isFirstRow)
        {
            sheet.Cells[currentRow, 1] = this._db.Dimension.getTitle(dimensionID);
            int valid = SQLColumn.GetValid(this._db.lookup("HEADER_STYLES", "MATRIX", "ID=" + this._matrixId), 0);
            int argb = SQLColumn.GetValid(this._db.lookup("HEADER_COLOR", "MATRIX", "ID=" + this._matrixId), 0xffffff);
            if ((valid == 2) && isFirstRow)
            {
                ((Range)sheet.Cells[currentRow, 1]).Interior.Color = ColorTranslator.ToOle(Color.FromArgb(argb));
            }
            else
            {
                int num3 = SQLColumn.GetValid(this._db.lookup("DIMENSION_COLOR", "MATRIX", "ID=" + this._matrixId), 0xffffff);
                ((Range)sheet.Cells[currentRow, 1]).Interior.Color = ColorTranslator.ToOle(Color.FromArgb(num3));
            }
            ((Range)sheet.Cells[currentRow, 1]).RowHeight = 60;
            ((Range)sheet.Cells[currentRow, 1]).ColumnWidth = 60;
            ((Range)sheet.Cells[currentRow, 1]).WrapText = true;
            ((Range)sheet.Cells[currentRow, 1]).HorizontalAlignment = XlHAlign.xlHAlignLeft;
            ((Range)sheet.Cells[currentRow, 1]).VerticalAlignment = XlVAlign.xlVAlignCenter;
            ((Range)sheet.Cells[currentRow, 1]).UseStandardWidth = true;
            ((Range)sheet.Cells[currentRow, 1]).UseStandardHeight = true;
            string selectStatement = "select ID from CHARACTERISTIC where DIMENSION_ID=" + dimensionID + "order by ORDNUMBER";
            System.Data.DataTable table = this._db.getDataTable(selectStatement, new object[0]);
            int num4 = 2;
            foreach (DataRow row in table.Rows)
            {
                this.addCharacteristic(SQLColumn.GetValid(row["ID"], (long)(-1L)), sheet, currentRow, (long)num4);
                if ((valid == 2) && isFirstRow)
                {
                    ((Range)sheet.Cells[currentRow, num4]).Interior.Color = ColorTranslator.ToOle(Color.FromArgb(argb));
                }
                num4++;
            }
            if (table.Rows.Count > this._maxCellNumber)
            {
                this._maxCellNumber = table.Rows.Count;
            }
        }

        protected void createCustomPalette(Workbook wb, System.Data.DataTable colTable, System.Data.DataTable colTitleTable)
        {
            wb.set_Colors(1, Color.White.ToArgb());
            int index = 2;
            foreach (DataRow row in colTable.Rows)
            {
                wb.set_Colors(index, ColorTranslator.ToOle(Color.FromArgb(SQLColumn.GetValid(row["COLOR"], -1))));
                index++;
            }
            foreach (DataRow row2 in colTitleTable.Rows)
            {
                wb.set_Colors(index, ColorTranslator.ToOle(Color.FromArgb(SQLColumn.GetValid(row2["HEADER_COLOR"], -1))));
                index++;
                wb.set_Colors(index, ColorTranslator.ToOle(Color.FromArgb(SQLColumn.GetValid(row2["DIMENSION_COLOR"], -1))));
                index++;
            }
        }

        public int drawLegend(Workbook wb, Worksheet sheet, System.Data.DataTable colTable, int startColumn)
        {
            sheet.Cells[1, startColumn] = this._mapper.get("suggestion_execution", "legend");
            int num = 2;
            foreach (DataRow row in colTable.Rows)
            {
                Range range = (Range)sheet.Cells[num, startColumn];
                range.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(SQLColumn.GetValid(row["COLOR"], -1)));
                sheet.Cells[num, startColumn + 1] = SQLColumn.GetValid(row["TITLE"], "");
                num++;
            }
            return num;
        }

        public void export(string fullFileName)
        {
            this._db.connect();
            try
            {
                object o = null;
                object readOnlyRecommended = false;
                object addToMru = true;
                object template = Missing.Value;
                object filename = fullFileName;
                GC.Collect();
                ApplicationClass class2 = new ApplicationClass();
                class2.Visible = false;
                class2.DisplayAlerts = false;
                Workbook wb = class2.Workbooks.Add(template);
                Worksheet activeSheet = (Worksheet)wb.ActiveSheet;
                System.Data.DataTable colTable = this._db.getDataTable("select * from COLORATION where MATRIX_ID=" + this._matrixId + " order by ORDNUMBER", new object[0]);
                System.Data.DataTable colTitleTable = this._db.getDataTable("select * from MATRIX where ID=" + this._matrixId, new object[0]);
                this.createCustomPalette(wb, colTable, colTitleTable);
                activeSheet.Cells[1, 1] = "Title";
                if (this._slaveId > 0L)
                {
                    this._matrixId = SQLColumn.GetValid(this._db.lookup("MATRIX_ID", "SLAVE", " ID = " + this._slaveId), (long)(-1L));
                    activeSheet.Cells[2, 1] = this._db.Matrix.getSlaveDescription(this._slaveId);
                }
                else
                {
                    activeSheet.Cells[2, 1] = this._db.Matrix.getDescription(this._matrixId);
                }
                this._db.getDataTable("select DIMENSION_ID from MATRIX where ID=" + this._matrixId + " and DIMENSION_ID is not NULL ", new object[0]);
                System.Data.DataTable table3 = this._db.getDataTable("select ID from DIMENSION where MATRIX_ID=" + this._matrixId + " order by ORDNUMBER", new object[0]);
                string[] attributs = new string[] { "HEADER_STYLES", "HEADER_COLOR", "DIMENSION_COLOR" };
                object[] objArray = this._db.lookup(attributs, "MATRIX", " ID=" + this._matrixId);
                Color white = Color.White;
                Color color2 = Color.White;
                int valid = SQLColumn.GetValid(objArray[1], -1);
                if (valid != -1)
                {
                    Color.FromArgb(valid);
                }
                int argb = SQLColumn.GetValid(objArray[2], -1);
                if (argb != -1)
                {
                    Color.FromArgb(argb);
                }
                int num4 = this._db.lookup("COUNT(*)", "COLORATION", " MATRIX_ID = " + this._matrixId, -1) + 3;
                int num5 = num4;
                foreach (DataRow row in table3.Rows)
                {
                    this.addDimension(SQLColumn.GetValid(row["ID"], (long)(-1L)), activeSheet, (long)num5, num5 == num4);
                    num5++;
                }
                this.drawLegend(wb, activeSheet, colTable, (int)this._maxCellNumber);
                Range range = (Range)activeSheet.Cells[1, 1];
                Range range2 = (Range)activeSheet.Cells[1, this._maxCellNumber - 2L];
                Range range3 = activeSheet.get_Range(range, range2);
                range3.MergeCells = true;
                o = range;
                this.ReleaseCom(ref o);
                o = range2;
                this.ReleaseCom(ref o);
                o = range3;
                this.ReleaseCom(ref o);
                GC.Collect();
                range3 = activeSheet.get_Range(activeSheet.Cells[2, 1], activeSheet.Cells[num4 - 2, this._maxCellNumber - 2L]);
                range3.MergeCells = true;
                range3.Merge(template);
                range3 = null;
                activeSheet.get_Range(activeSheet.Cells[num4, 1], activeSheet.Cells[(num4 + table3.Rows.Count) - 1, this._maxCellNumber + 1L]).Borders.LineStyle = XlLineStyle.xlContinuous;
                range3 = null;
                GC.Collect();
                wb.SaveAs(filename, template, template, template, readOnlyRecommended, template, XlSaveAsAccessMode.xlExclusive, template, addToMru, template, template, template);
                wb.Saved = true;
                wb.Close(readOnlyRecommended, template, template);
                o = activeSheet;
                this.ReleaseCom(ref o);
                o = wb;
                this.ReleaseCom(ref o);
                class2.Quit();
                o = class2;
                this.ReleaseCom(ref o);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, Logger.ERROR);
            }
            finally
            {
                this._db.disconnect();
            }
        }

        private void ReleaseCom(ref object o)
        {
            try
            {
                while (Marshal.ReleaseComObject(o) > 0)
                {
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception, Logger.ERROR);
            }
            finally
            {
                o = null;
            }
        }
    }
}