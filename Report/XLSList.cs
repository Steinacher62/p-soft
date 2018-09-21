using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.SessionState;
using Excel = Microsoft.Office.Interop.Excel;

namespace ch.appl.psoft.Report
{

    /// <summary>
    /// Base class for all Reports.
    /// </summary>
    public abstract class XLSList : List{
		protected Encoding _encode = null;
		protected Excel.ApplicationClass _excelApplication = null;
		protected Excel.Workbook _workbook = null;
		protected Excel.Worksheet _worksheet = null;

        protected string _fileName = "";
        protected string _name = "";
 
        public string excelFilename {
            get { return _name+".xls"; }
        }

		public abstract void writeList(long id, string path, string imagePath, LanguageMapper map, DBData db, DataTable data, HttpSessionState session, params string[] substituteValues);

        protected virtual void open(long id, string path, DBData db, HttpSessionState session) {
            if (!_append) {
                base.open(db);
                if (db != null && _name == "") _name = db.lookup("title_mnemo","reportlayout","id="+id,false);
                _name += "_" + SessionData.getSessionID(session);
                _encode = Encoding.GetEncoding("ISO-8859-1");
                _fileName = path+"\\"+_name;
				_excelApplication = new Excel.ApplicationClass();
				_workbook = _excelApplication.Workbooks.Add(Type.Missing);    // add a new workbook
				_worksheet = (Excel.Worksheet)_excelApplication.ActiveSheet;  // activate the active worksheet in the workbook
				_excelApplication.DisplayAlerts = false;
				_worksheet.Activate();
            }
        }

        protected virtual new void close() {
            _workbook.Close(false, "", false);
			_excelApplication.DisplayAlerts = true;

			_excelApplication.Quit();

			Marshal.ReleaseComObject(_worksheet);
			Marshal.ReleaseComObject(_workbook);
			Marshal.ReleaseComObject(_excelApplication);
			System.GC.Collect();
			GC.WaitForPendingFinalizers();

			base.close();
        }

        /// <summary>
        /// The xls file will be saved.
        /// </summary>
        protected bool saveXLS() {
            string xlsFileName = _fileName + ".xls";

            if (_extend) return false;

            System.GC.Collect();

            try {
				_workbook.SaveAs(xlsFileName, Excel.XlFileFormat.xlWorkbookNormal, "", "",
					false, false,
					Excel.XlSaveAsAccessMode.xlNoChange,
					Excel.XlSaveConflictResolution.xlUserResolution,
					false, "", "", null);
                return true;
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
                return false;
            }
        }

     }
}
