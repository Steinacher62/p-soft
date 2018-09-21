using ch.appl.psoft.Common;
using ch.appl.psoft.db;
//using Telerik.Windows.Documents.Spreadsheet.Model;

using ch.psoft.Util;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using Telerik.Web.Spreadsheet;
using Telerik.Web.UI;

namespace ch.appl.psoft.MKS
{
    public partial class UploadExcel : PsoftDetailPage
    {
        public int RowNumber;
        protected void Page_Load(object sender, EventArgs e)
        {
            BreadcrumbCaption = "Daten aus Excel Importieren";

        }

        protected void AsyncUpload1_FileUploaded(object sender, FileUploadedEventArgs e)
        {
            string path = Server.MapPath("~/MKS/TempFiles/");
            e.File.SaveAs(path+e.File.GetName());
            String matrixId = Request.QueryString["MatrixId"];
            try
            {
                Workbook wb = new Workbook();
                wb = Workbook.Load(path + e.File.GetName());
                int length = wb.Sheets.Count;
                int indexOfSheet = 0;
                for (int i = 0; i < length; i++)
                {
                    if (wb.Sheets[i].Name.Equals("Sokrates-Schnittstelle"))
                    {
                        indexOfSheet = i;
                        break;
                    }

                }
                wb.Sheets[indexOfSheet].FrozenColumns = 100;
                Worksheet ws = wb.Sheets[indexOfSheet];
                

                DBData db = DBData.getDBData(Session);
                db.connect();

                DataTable rowIds = db.getDataTable("SELECT ID FROM DIMENSION WHERE MATRIX_ID =" + matrixId + " ORDER BY ORDNUMBER", "row_IDs");
                DataTable colorIds = db.getDataTable("SELECT ID FROM COLORATION WHERE MATRIX_ID = " + matrixId, "color_Ids");

                foreach (DataRow row in rowIds.Rows)
                {
                    db.execute("DELETE FROM CHARACTERISTIC WHERE DIMENSION_ID = " + row[0].ToString() +" AND ORDNUMBER > 1");
                }


                int rowInd = 0;

                NumberFormatInfo swissNumberFormatInfo = CultureInfo.GetCultureInfo("de-CH").NumberFormat;

                for (int cell = 2; ws.Rows.Count > cell && ws.Rows[cell].Cells[0].Value != null; cell++)
                {
                    RowNumber = cell+1;
                    while (Int32.Parse(ws.Rows[cell].Cells[0].Value.ToString()) - 1 > rowInd)
                    {
                        rowInd++;
                        if (rowInd > rowIds.Rows.Count)
                        {
                            db.execute("INSERT INTO DIMENSION (MATRIX_ID) VALUE (" + matrixId + ")");
                            rowIds = db.getDataTable("SELECT ID FROM DIMENSION WHERE MATRIX_ID =" + matrixId + " ORDER BY ORDNUMBER", "row_IDs");
                        }
                    }

                    String z1 = "";
                    String z2 = "";
                    String z3 = "";
                    String z4 = "";
                    String bew = "4";


                    int ordnumber = Convert.ToInt16(ws.Rows[cell].Cells[1].Value.ToString()) -1;
                    if (ws.Rows[cell].Cells[2].Value != null)
                    {
                       z1 = ws.Rows[cell].Cells[2].Value.ToString().Replace("'", "''");
                    }
                    if (ws.Rows[cell].Cells[3].Value != null)
                    {
                        z2 = "\n" +  Convert.ToInt32( ws.Rows[cell].Cells[3].Value).ToString("#,#", new NumberFormatInfo { NumberGroupSeparator = "'" }).Replace("'","''");
                    }
                    if (ws.Rows[cell].Cells[4].Value != null)
                    {
                       z3 = "\n" + Convert.ToDouble( ws.Rows[cell].Cells[4].Value).ToString("#0.##%", swissNumberFormatInfo).Replace("'", "''");
                    }
                    if (ws.Rows[cell].Cells.Count > 5 && ws.Rows[cell].Cells[5].Value != null)
                    {
                        z4 = ws.Rows[cell].Cells[5].Value.ToString().Replace("'", "''");
                    }
                    if (ws.Rows[cell].Cells.Count > 6 && ws.Rows[cell].Cells[6].Value != null)
                    {
                        bew = ws.Rows[cell].Cells[6].Value.ToString();
                    }

                    String sql = "INSERT INTO CHARACTERISTIC (ORDNUMBER, DIMENSION_ID , TITLE, SUBTITLE ";
                    String val = "VALUES ('" + ordnumber   + "', '" + rowIds.Rows[rowInd].ItemArray[0].ToString() + "', '" + z1 + z2  + z3 + "', '" + z4 + "'";

                    int color = 0;
                    if (Int32.TryParse(bew, out color))
                    {
                        sql += ", COLOR_ID";
                        val += ", '" + colorIds.Rows[color - 1].ItemArray[0].ToString() + "'";
                    }
                    sql += ")";
                    val += ")";
                    sql += val;
                    Logger.Log(sql, 0);
                    db.execute(sql);
                }
               
            }
            catch
            {
                errorMsg.Style["Display"] = "Visible";

                DBData db = DBData.getDBData(Session);
                db.connect();
                DataTable rowIds = db.getDataTable("SELECT ID FROM DIMENSION WHERE MATRIX_ID =" + matrixId + " ORDER BY ORDNUMBER", "row_IDs");

                foreach (DataRow row in rowIds.Rows)
                {
                    db.execute("DELETE FROM CHARACTERISTIC WHERE DIMENSION_ID = " + row[0].ToString());
                }
                File.Delete(path + e.File.GetName());
                return;
            }

            File.Delete(path + e.File.GetName());
            Response.Redirect("../Morph/MatrixDetail.aspx?matrixID=" + matrixId,true);
        }


        public void btnDummy_Click(object sender, EventArgs e)
        {
            
        }
    }
}
