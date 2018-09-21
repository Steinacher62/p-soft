using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.IO;
using Telerik.Web.Spreadsheet;
using Telerik.Web.UI;
using Telerik.Windows.Documents.FormatProviders.OpenXml;

namespace ch.appl.psoft.SPZ
{
    public partial class UploadPerformanceRating : PsoftDetailPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            BreadcrumbCaption = "Daten aus Excel Importieren";

        }

        protected void AsyncUpload1_FileUploaded(object sender, FileUploadedEventArgs e)
        {
            string path = Server.MapPath("~/SPZ/TempFiles/");
            e.File.SaveAs(path + e.File.GetName());

            try
            {
                Workbook wb = new Workbook();
                
                wb = Workbook.Load(path + e.File.GetName());

                int length = wb.Sheets.Count;
                int indexOfSheet = 0;
                for (int i = 0; i < length; i++)
                {
                    if (wb.Sheets[i].Name.Equals("LB-Daten"))
                    {
                        indexOfSheet = i;
                        break;
                    }

                }

                Worksheet ws = wb.Sheets[indexOfSheet];


                DBData db = DBData.getDBData(Session);
                db.connect();
                DataTable errorTable = new DataTable();
                DataColumn RowNumber = new DataColumn("Zeile");
                DataColumn Personnelnumber = new DataColumn("Personalnummer");
                DataColumn EmploymentId = new DataColumn("ID-Anstellung");
                DataColumn JobId = new DataColumn("ID-Job");
                DataColumn PerformanceratingDat = new DataColumn("Leistungsbewertungsdatum");
                DataColumn Rating = new DataColumn("Bewertung");
                errorTable.Columns.Add(RowNumber);
                errorTable.Columns.Add(Personnelnumber);
                errorTable.Columns.Add(EmploymentId);
                errorTable.Columns.Add(JobId);
                errorTable.Columns.Add(PerformanceratingDat);
                errorTable.Columns.Add(Rating);
                long pNr = 0;
                double performancerating = 0;
                DateTime ratingDate = Convert.ToDateTime("01.01.1900");
                long employmentId = 0;
                long jobID = 0;
                long personId = 0;
                int rowNumber = 0;
                foreach (Row lbRow in ws.Rows)
                {
                    try
                    {
                        pNr = Convert.ToInt32(lbRow.Cells[0].Value);

                        employmentId = Convert.ToInt32(db.lookup("ID", "EMPLOYMENT", "EXTERNAL_REF =" + pNr));
                        jobID = Convert.ToInt32(db.lookup("ID", "JOB", "EMPLOYMENT_ID =" + employmentId));
                        personId = Convert.ToInt32(db.lookup("ID", "PERSON", "PERSONNELNUMBER =" + pNr));
                        performancerating = Convert.ToDouble(lbRow.Cells[1].Value);
                        int ratingDate_t = Convert.ToInt32(lbRow.Cells[2].Value);
                        if (ratingDate_t > 59) ratingDate_t -= 1; //Excel/Lotus 2/29/1900 bug   
                        ratingDate = new DateTime(1899, 12, 31).AddDays(ratingDate_t);
                        if (ratingDate < new DateTime(2000, 01, 01))
                        {
                            ratingDate = new DateTime();
                        }
                        db.execute("INSERT INTO PERFORMANCERATING (EMPLOYMENT_REF, RATING_DATE, JOB_ID, PERSON_ID) VALUES(" + employmentId + ", '" + ratingDate.ToString("MM.dd.yyyy") + "', " + jobID + ", " + personId + ")");
                        long performanceRatingId = Convert.ToInt32(db.lookup("max(ID)", "PERFORMANCERATING", ""));
                        db.execute("INSERT INTO PERFORMANCERATING_ITEMS (PERFORMANCERATING_REF, CRITERIA_REF, CRITERIA_WEIGHT, CRITERIA_TITLE_DE, RELATIV_WEIGHT) VALUES(" + performanceRatingId + ", 48442, 100, 'Umsetzung der gestellten Aufgaben'," + performancerating / 2 + ")");

                        pNr = 0;
                        performancerating = 0;
                        ratingDate = Convert.ToDateTime("01.01.1900");
                        employmentId = 0;
                        jobID = 0;
                        personId = 0;
                        rowNumber += 1;
                    }
                    catch
                    {
                        if (pNr != 0)
                        {
                            DataRow newRow = errorTable.NewRow();
                            newRow["Zeile"] = rowNumber;
                            newRow["Personalnummer"] = pNr;
                            newRow["ID-Anstellung"] = employmentId;
                            newRow["ID-Job"] = jobID;
                            newRow["Leistungsbewertungsdatum"] = ratingDate;
                            newRow["Bewertung"] = performancerating;
                            errorTable.Rows.Add(newRow);
                        }
                        pNr = 0;
                        performancerating = 0;
                        ratingDate = Convert.ToDateTime("01.01.1900");
                        employmentId = 0;
                        jobID = 0;
                        personId = 0;
                        rowNumber += 1;



                    }

                    ErrorTable.DataSource = errorTable;
                    ErrorTable.DataBind();
                }

                db.disconnect();


            }
            catch
            {
                errorMsg.Style["Display"] = "Visible";
                return;
            }
        }


        public void btnDummy_Click(object sender, EventArgs e)
        {

        }
    }
}
