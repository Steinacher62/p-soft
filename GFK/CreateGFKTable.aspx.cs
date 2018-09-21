using ch.appl.psoft.db;
using ch.appl.psoft.WebService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Telerik.Web.Spreadsheet;
using Telerik.Web.UI;

namespace ch.appl.psoft.GFK.XMLData
{
    public partial class CreateGFKTable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            // get matrix id
            string matrixId = Request.QueryString["matrixID"];

            // connect to db
            DBData db = DBData.getDBData(Session);
            db.connect();

            //get row ids
            DataTable rowIds = db.getDataTable("SELECT ID FROM DIMENSION WHERE MATRIX_ID ="+matrixId, "row_IDs");
            List<string> usedRows = new List<string>();

            // get color ids
            DataTable colorIds = db.getDataTable("SELECT ID FROM COLORATION WHERE MATRIX_ID = " + matrixId, "color_Ids");

            // get filenames
            string fileName = db.lookup("GFK_FILE_NAME", "MATRIX", "ID = " + matrixId).ToString();
            string fileName2 = db.lookup("SECOND_FILE_NAME", "MATRIX", "ID = " + matrixId).ToString();
            string pathBase = Global.Config.getModuleParam("gfk", "GFKDataSourcePath", "");
            var directory = new DirectoryInfo(pathBase);
            string tablename = db.lookup("TITLE", "MATRIX", "ID=" + matrixId).ToString();

            var file = (from f in directory.GetFiles(fileName + "*") orderby f.LastWriteTime descending select f).First();
            string path = file.FullName;

            FileInfo file2;
            string path2 = "";
            if (fileName2 != "")
            {
                file2 = (from f in directory.GetFiles(fileName2 + "*") orderby f.LastWriteTime descending select f).First();

                path2 = file2.FullName;

            }
            //get new name
            
            string date = file.Name.Split('_').Last().Split('.')[0];

            // correct date format
            string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string[] monthNr = new string[] { "M01", "M02", "M03", "M04", "M05", "M06", "M07", "M08", "M09", "M10", "M11", "M12" };
            for (int month = 0; month < 12; month++)
            {
                date = date.Replace(months[month], monthNr[month]);
            }
            string newName = date + " " + tablename;

            //check if table exists
            if (!db.exists("MATRIX", "TITLE = '" + newName + "'"))
            {

                string[] allowedFields = null;
                string enabledBorderRow = null;

                // for each row--------------------------------------
                foreach (DataRow row in rowIds.Rows)
                {
                    int rowId = int.Parse(row.ItemArray[0].ToString());
                    string sql;
                    string totalvalue1 = "n.a.";
                    string totalvalue2 = "n.a.";
                    

                    // get row Data from db
                    if (db.exists("GFK_FORMULA", "ROW_ID = " + rowId))
                    {
                        RowData rowData = new RowData(rowId, Session);

                        if (rowData.reportTyp != "" || rowData.excelSheet != "")
                        {
                            

                            // delete all cells in this row
                            sql = "DELETE FROM CHARACTERISTIC WHERE DIMENSION_ID = " + rowId;
                            db.execute(sql);

                            usedRows.Add(rowId.ToString());

                            NeededData nd = new NeededData();

                            if (rowData.isQvw)
                            {
                                nd = GetDataFromQVW(rowData, path);
                            }
                            else
                            {
                                nd = GetDataFromExcel(rowData, path);
                            }


                            NeededData nd2 = new NeededData();
                            if (rowData.secondTableEnabled)
                            {
                                string temppath = path2;
                                if (rowData.previousYearEnabled)
                                {
                                    int year = int.Parse(temppath.Split('_').Last().Split('.')[0].Substring(temppath.Split('_').Last().Split('.')[0].Length - 2, 2)) - 1;
                                    temppath = temppath.Replace(temppath.Split('_').Last().Split('.')[0], temppath.Split('_').Last().Split('.')[0].Replace((year + 1).ToString(), year.ToString()));
                                }
                                if (rowData.secondIsQVW && rowData.fact2 != "")
                                {
                                    nd2 = GetDataFromQVW2(rowData, path2);
                                }
                                else if (rowData.excelSheet2 != "")
                                {
                                    nd2 = GetDataFromExcel2(rowData, path2);
                                }

                                 
                                // selects same cells from 1st and second table
                                for (int i = 1; i < nd.rowNames.Length; i++)
                                {
                                  if(!nd2.rowNames.Contains(nd.rowNames[i])){
                                      string[] newIndicesArray = new string[nd.rowNames.Length - 1];

                                      int a = 0;
                                      int j = 0;
                                      while (a < nd.rowNames.Length)
                                      {
                                          if (a != i)
                                          {
                                              newIndicesArray[j] = nd.rowNames[a];
                                              j++;
                                          }

                                          a++;
                                      }
                                      nd.rowNames = newIndicesArray;
                                      nd.datatable.Rows[i].Delete();
                                      i--;
                                  }
                                }
                                for (int i = 1; i < nd2.rowNames.Length; i++)
                                {
                                    if (!nd.rowNames.Contains(nd2.rowNames[i]))
                                    {
                                        string[] newIndicesArray = new string[nd2.rowNames.Length - 1];

                                        int a = 0;
                                        int j = 0;
                                        while (a < nd2.rowNames.Length)
                                        {
                                            if (a != i)
                                            {
                                                newIndicesArray[j] = nd2.rowNames[a];
                                                j++;
                                            }

                                            a++;
                                        }
                                        nd2.rowNames = newIndicesArray;
                                        nd2.datatable.Rows[i].Delete();
                                        i--;
                                    }
                                }
                            }

                            

                            // arrays for value1 and row text
                            string[][] rowText = new string[nd.datatable.Rows.Count][];
                            float[] rowValue1 = new float[nd.datatable.Rows.Count];        // only needed to color cells     
                            float[] rowSortValue = new float[nd.datatable.Rows.Count];  // only needed to sort cells     
                            
                            // count entries
                            if (rowData.countEnabled)
                            {
                                int numberOfEntries = 0;
                                int.TryParse(rowData.numberOfEntries, out numberOfEntries);
                                LinkedList<String> entries = new LinkedList<string>();
                                if (rowData.formula1 != "")
                                {
                                    
                                    for (int i = 1; i < numberOfEntries+1; i++)
                                    {
                                        string formula = rowData.formula1;
                                        for (int j = 0; j < nd.datatable.Columns.Count; j++)
                                        {
                                            formula = formula.Replace("[1." + j + "]", nd.datatable.Rows[i].ItemArray[j].ToString());
                                        }
                                        entries.AddLast(formula);
                                    }
                                }
                                else
                                {
                                    for (int i = 1; i < numberOfEntries+1; i++)
                                    {
                                        entries.AddLast(nd.rowNames[i]);
                                    }
                                }
                                int numberOfElements = 0;
                                LinkedList<String> used = new LinkedList<string>();
                                while (entries.Count != 0)
                                {
                                    string next = entries.First.Value;
                                    if (!used.Contains(next))
                                    {
                                        rowText[numberOfElements] = new string[3];
                                        rowText[numberOfElements][0] = next;
                                        used.AddLast(next);
                                        LinkedListNode<String> node = entries.First;
                                        while (node.Next != null)
                                        {
                                            if (node.Value == next)
                                            {
                                                rowValue1[numberOfElements]++;
                                                rowSortValue[numberOfElements]++;
                                                node = node.Next;
                                                entries.Remove(node.Previous);
                                            }
                                            else
                                            {
                                                node = node.Next;
                                            }
                                        }
                                        if (node.Value == next)
                                        {
                                            rowValue1[numberOfElements]++;
                                            rowSortValue[numberOfElements]++;
                                            entries.Remove(node);
                                        }
                                        rowText[numberOfElements][1] = rowValue1[numberOfElements].ToString();
                                        numberOfElements++;
                                    }
                                }

                                // correct array length
                                string[][] rowTextTemp = new string [used.Count][];
                                float[] rowValueTemp = new float[used.Count];
                                float[] rowSortTemp = new float[used.Count];
                                for (int i = 0; i < used.Count; i++)
                                {
                                    rowTextTemp[i] = new string[3];
                                    for (int j = 0; j < 3; j++)
                                    {
                                        rowTextTemp[i][j] = rowText[i][j];
                                    }
                                    rowValueTemp[i] = rowValue1[i];
                                    rowSortTemp[i] = rowValueTemp[i];

                                }
                                rowValue1 = rowValueTemp;
                                rowText = rowTextTemp;
                                rowSortValue = rowSortTemp;

                            }
                            else
                            {
                                //Add Total Value
                                if (rowData.TotalEnabled)
                                {
                                    rowText[0] = new string[3];
                                    string formulaTotal = rowData.formulaTotal;

                                    // fill in formula from 1st table
                                    for (int j = 0; j < nd.datatable.Columns.Count; j++)
                                    {
                                        formulaTotal = formulaTotal.Replace("[1." + j + "]", nd.datatable.Rows[0].ItemArray[j].ToString());
                                    }
                                    if (rowData.secondTableEnabled)
                                    {

                                        int itemp = 0;
                                        // search total value in 2nd table
                                        while (itemp < nd2.rowNames.Length - 1 && nd2.rowNames[itemp].ToString().Trim() != nd.rowNames[0].ToString().Trim())
                                        {
                                            itemp++;
                                        }

                                        // fill in values from 2nd table
                                        if (nd2.rowNames[itemp].ToString().Trim() == nd.rowNames[0].ToString().Trim())
                                        {
                                            for (int j = 0; j < nd2.datatable.Columns.Count; j++)
                                            {
                                                formulaTotal = formulaTotal.Replace("[2." + j + "]", nd2.datatable.Rows[itemp].ItemArray[j].ToString());
                                            }
                                        }
                                    }

                                    float value1;
                                    // calculate equation
                                    try
                                    {
                                        var loDataTable = new DataTable();
                                        var loDataColumn = new DataColumn("Eval", typeof(float), formulaTotal);
                                        loDataTable.Columns.Add(loDataColumn);
                                        loDataTable.Rows.Add(0);



                                        if (float.TryParse((loDataTable.Rows[0]["Eval"]).ToString(), out value1))
                                        {
                                            rowText[0][1] = Math.Round(value1, 1, MidpointRounding.ToEven).ToString() + rowData.unitTotal;
                                            rowValue1[0] = value1;
                                            totalvalue1 = value1.ToString();
                                        }
                                    }
                                    catch
                                    {
                                        rowText[0][1] = "n.a."; // no value
                                        totalvalue1 = "n.a.";
                                        rowValue1[0] = rowData.descendantSorted ? float.NegativeInfinity : float.PositiveInfinity;  // value if value can not be calculated
                                    }

                                    rowText[0][0] = nd.rowNames[0].Trim();



                                
                                //Add 2nd Total Value
                                    if (rowData.secondTotalEnabled)
                                    {
                                        
                                        string formulaTotal2 = rowData.secondFormulaTotal;

                                        // fill in formula from 1st table
                                        for (int j = 0; j < nd.datatable.Columns.Count; j++)
                                        {
                                            formulaTotal2 = formulaTotal2.Replace("[1." + j + "]", nd.datatable.Rows[0].ItemArray[j].ToString());
                                        }
                                        if (rowData.secondTableEnabled)
                                        {

                                            int itemp = 0;
                                            // search total value in 2nd table
                                            while (itemp < nd2.rowNames.Length - 1 && nd2.rowNames[itemp].ToString().Trim() != nd.rowNames[0].ToString().Trim())
                                            {
                                                itemp++;
                                            }

                                            // fill in values from 2nd table
                                            if (nd2.rowNames[itemp].ToString().Trim() == nd.rowNames[0].ToString().Trim())
                                            {
                                                for (int j = 0; j < nd2.datatable.Columns.Count; j++)
                                                {
                                                    formulaTotal2 = formulaTotal2.Replace("[2." + j + "]", nd2.datatable.Rows[itemp].ItemArray[j].ToString());
                                                }
                                            }
                                        }
                                        

                                        // calculate equation
                                        try
                                        {
                                            var loDataTable = new DataTable();
                                            var loDataColumn = new DataColumn("Eval", typeof(float), formulaTotal2);
                                            loDataTable.Columns.Add(loDataColumn);
                                            loDataTable.Rows.Add(0);



                                            if (float.TryParse((loDataTable.Rows[0]["Eval"]).ToString(), out value1))
                                            {
                                                rowText[0][1] += " / " + Math.Round(value1, 1, MidpointRounding.ToEven).ToString() + rowData.secondUnitTotal;
                                                if (rowData.colorationOnSecondTotal)
                                                {
                                                    rowValue1[0] = value1;
                                                }
                                                totalvalue2 = value1.ToString();
                                            }
                                        }
                                        catch
                                        {
                                            rowText[0][1] += " / n.a."; // no value
                                            if (rowData.colorationOnSecondTotal)
                                            {
                                                rowValue1[0] = rowData.descendantSorted ? float.NegativeInfinity : float.PositiveInfinity;  // value if value can not be calculated
                                            }
                                            totalvalue2 = "n.a.";
                                        }

                                        rowText[0][0] = nd.rowNames[0].Trim();


                                    }
                                }

                                // calculate 1st values
                                for (int i = 1; i < nd.datatable.Rows.Count; i++)
                                {
                                    rowText[i] = new string[3];
                                    string formula1 = rowData.formula1;

                                    // fill in values from 1st table
                                    for (int j = 0; j < nd.datatable.Columns.Count; j++)
                                    {
                                        formula1 = formula1.Replace("[1." + j + "]", nd.datatable.Rows[i].ItemArray[j].ToString());
                                    }
                                    if (rowData.secondTableEnabled)
                                    {

                                        // search row in second table
                                        int itemp = 0;
                                        while (itemp<nd2.rowNames.Length && nd2.rowNames[itemp].ToString().Trim() != nd.rowNames[i].ToString().Trim() )
                                        {
                                            itemp++;
                                        }

                                        // fill in values from 2nd table
                                        if (nd2.rowNames[itemp].ToString().Trim() == nd.rowNames[i].ToString().Trim())
                                        {
                                            for (int j = 0; j < nd2.datatable.Columns.Count; j++)
                                            {
                                                formula1 = formula1.Replace("[2." + j + "]", nd2.datatable.Rows[itemp].ItemArray[j].ToString());
                                            }
                                        }
                                    }

                                    // fill in total value
                                    if (rowData.TotalEnabled)
                                    {
                                        formula1 = formula1.Replace("[Total]", totalvalue1);
                                    }
                                    if (rowData.TotalEnabled)
                                    {
                                        formula1 = formula1.Replace("[Total2]", totalvalue2);
                                    }
                                    float value1;

                                    // calculate equation
                                    try
                                    {
                                        var loDataTable = new DataTable();
                                        var loDataColumn = new DataColumn("Eval", typeof(float), formula1);
                                        loDataTable.Columns.Add(loDataColumn);
                                        loDataTable.Rows.Add(0);



                                        if (float.TryParse((loDataTable.Rows[0]["Eval"]).ToString(), out value1))
                                        {
                                            rowText[i][1] = Math.Round(value1, 1, MidpointRounding.ToEven).ToString() + rowData.unit1;
                                            rowValue1[i] = value1;
                                            rowSortValue[i] = value1;
                                        }
                                    }
                                    catch
                                    {
                                        rowText[i][1] = "n.a."; // no value
                                        rowValue1[i] = rowData.descendantSorted ? float.NegativeInfinity : float.PositiveInfinity;  // value if value can not be calculated
                                        rowSortValue[i] = rowData.descendantSorted ? float.NegativeInfinity : float.PositiveInfinity;
                                    }

                                    rowText[i][0] = nd.rowNames[i].Trim();

                                }

                                // add second value
                                if (rowData.secondValueEnabled)
                                {
                                    float totalvalue = rowValue1[0];
                                    for (int i = 1 ; i < nd.datatable.Rows.Count; i++)
                                    {

                                        string formula2 = rowData.formula2;
                                        float value2;
                                        
                                        // fill in values from 1st table   
                                        for (int j = 0; j < nd.datatable.Columns.Count; j++)
                                        {
                                            formula2 = formula2.Replace("[1." + j + "]", nd.datatable.Rows[i].ItemArray[j].ToString());
                                        }
                                        if (rowData.secondTableEnabled)
                                        {
                                            // search row in second table
                                            int itemp = 0;
                                            while (itemp < nd2.rowNames.Length - 1 && nd2.rowNames[itemp].ToString().Trim() != nd.rowNames[i].ToString().Trim())
                                            {
                                                itemp++;
                                            }

                                            // fill in values from second table
                                            if (nd2.rowNames[itemp].ToString().Trim() == nd.rowNames[i].ToString().Trim())
                                            {
                                                for (int j = 0; j < nd2.datatable.Columns.Count; j++)
                                                {
                                                    formula2 = formula2.Replace("[2." + j + "]", nd2.datatable.Rows[itemp].ItemArray[j].ToString());
                                                }
                                            }
                                        }

                                        // fill in total value
                                        if (rowData.TotalEnabled)
                                        {
                                            formula2 = formula2.Replace("[Total]", totalvalue1);
                                        }
                                        if (rowData.TotalEnabled)
                                        {
                                            formula2 = formula2.Replace("[Total2]", totalvalue2);
                                        }
                                        // calculate equation
                                        try
                                        {
                                            var loDataTable = new DataTable();
                                            var loDataColumn = new DataColumn("Eval", typeof(float), formula2);
                                            loDataTable.Columns.Add(loDataColumn);
                                            loDataTable.Rows.Add(0);

                                            if (float.TryParse((loDataTable.Rows[0]["Eval"]).ToString(), out value2))
                                            {
                                                rowText[i][1] += " / " + Math.Round(value2, 1, MidpointRounding.ToEven).ToString() + rowData.unit2;
                                                if (rowData.colorationOnSecondTotal && i == 0)
                                                {
                                                    rowValue1[i] = value2;
                                                }
                                                if (rowData.colorationOnSecondValue && i != 0)
                                                {
                                                    rowValue1[i] = value2;
                                                }
                                                if (rowData.sortationOnSecondValue && i != 0)
                                                {
                                                    rowSortValue[i] = value2;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            rowText[i][1] += " / n.a."; // no value  
                                            
                                            if (rowData.colorationOnSecondValue)
                                            {
                                                rowValue1[i] = rowData.descendantSorted ? float.NegativeInfinity : float.PositiveInfinity;  // value if value can not be calculated
                                            }
                                            if (rowData.sortationOnSecondValue)
                                            {
                                                rowSortValue[i] = rowData.descendantSorted ? float.NegativeInfinity : float.PositiveInfinity;  
                                            }
                                        }

                                    }
                                }

                                // 
                                if (!rowData.TotalEnabled)
                                {
                                    string[][] rowTextTemp = new string[rowValue1.Length-1][];
                                    float[] rowValueTemp = new float[rowValue1.Length - 1];
                                    float[] rowSortValueTemp = new float[rowValue1.Length - 1];
                                    for (int i = 0; i < rowValue1.Length-1; i++)
                                    {
                                        rowTextTemp[i] = new string[3];
                                        for (int j = 0; j < 3; j++)
                                        {
                                            rowTextTemp[i][j] = rowText[i+1][j];
                                        }
                                        rowValueTemp[i] = rowValue1[i+1];
                                        rowSortValueTemp[i] = rowSortValue[i + 1];

                                    }
                                    rowValue1 = rowValueTemp;
                                    rowText = rowTextTemp;
                                    rowSortValue = rowSortValueTemp;
                                }
                                    
                                    if (rowData.descendantSorted && rowData.TotalEnabled)
                                    {
                                      
                                        rowSortValue[0] = float.PositiveInfinity;
                                    }
                                    else if (rowData.TotalEnabled)
                                    {
                                        
                                        rowSortValue[0] = float.NegativeInfinity;
                                    }
                                
                            }

                            // add color id
                            if (rowData.highestValueIsGreen)
                            {
                                for (int i = rowData.TotalEnabled?  1 : 0 ; i < rowText.Length; i++)
                                {
                                    if (float.Parse(rowData.borderGreenLightgreen) == 0 && float.Parse(rowData.borderLightgreenYellow) == 0 && float.Parse(rowData.borderYellowOrange) == 0 && float.Parse(rowData.borderOrangeRed) == 0)
                                    {
                                        rowText[i][2] = colorIds.Rows[5][0].ToString();
                                    }
                                    else 
                                    if (rowValue1[i] == float.PositiveInfinity || rowValue1[i] == float.NegativeInfinity)
                                    {
                                        rowText[i][2] = colorIds.Rows[3][0].ToString();
                                    }
                                    else if (rowValue1[i] >= float.Parse(rowData.borderGreenLightgreen))
                                    {
                                        rowText[i][2] = colorIds.Rows[1][0].ToString();
                                    }
                                    else if (rowValue1[i] >= float.Parse(rowData.borderLightgreenYellow))
                                    {
                                        rowText[i][2] = colorIds.Rows[0][0].ToString();
                                    }
                                    else if (rowValue1[i] >= float.Parse(rowData.borderYellowOrange))
                                    {
                                        rowText[i][2] = colorIds.Rows[2][0].ToString();
                                    }
                                    else if (rowValue1[i] >= float.Parse(rowData.borderOrangeRed))
                                    {
                                        rowText[i][2] = colorIds.Rows[4][0].ToString();
                                    }
                                    else
                                    {
                                        rowText[i][2] = colorIds.Rows[6][0].ToString();
                                    }
                                }
                            }
                            else
                            {
                                for (int i = rowData.TotalEnabled ? 1 : 0; i < rowText.Length; i++)
                                {
                                    if (float.Parse(rowData.borderGreenLightgreen) == 0 && float.Parse(rowData.borderLightgreenYellow) == 0 && float.Parse(rowData.borderYellowOrange) == 0 && float.Parse(rowData.borderOrangeRed) == 0)
                                    {
                                        rowText[i][2] = colorIds.Rows[5][0].ToString();
                                    }
                                    else 
                                    if (rowValue1[i] == float.PositiveInfinity || rowValue1[i] == float.NegativeInfinity)
                                    {
                                        rowText[i][2] = colorIds.Rows[3][0].ToString();
                                    }
                                    else if (rowValue1[i] <= float.Parse(rowData.borderGreenLightgreen))
                                    {
                                        rowText[i][2] = colorIds.Rows[1][0].ToString();
                                    }
                                    else if (rowValue1[i] <= float.Parse(rowData.borderLightgreenYellow))
                                    {
                                        rowText[i][2] = colorIds.Rows[0][0].ToString();
                                    }
                                    else if (rowValue1[i] <= float.Parse(rowData.borderYellowOrange))
                                    {
                                        rowText[i][2] = colorIds.Rows[2][0].ToString();
                                    }
                                    else if (rowValue1[i] <= float.Parse(rowData.borderOrangeRed))
                                    {
                                        rowText[i][2] = colorIds.Rows[4][0].ToString();
                                    }
                                    else
                                    {
                                        rowText[i][2] = colorIds.Rows[6][0].ToString();
                                    }
                                }
                            }

                            // color total value
                            if(rowData.TotalEnabled)
                            {

                            if (rowData.highestValueIsGreen)
                            {
                                int i = 0;
                                if (float.Parse(rowData.borderGreenTotal) == 0 && float.Parse(rowData.borderLightgreenTotal) == 0 && float.Parse(rowData.borderYellowTotal) == 0 && float.Parse(rowData.borderOrangeTotal) == 0)
                                {
                                    rowText[i][2] = colorIds.Rows[5][0].ToString();
                                }
                                else
                                    if (rowValue1[i] == float.PositiveInfinity || rowValue1[i] == float.NegativeInfinity)
                                    {
                                        rowText[i][2] = colorIds.Rows[3][0].ToString();
                                    }
                                    else if (rowValue1[i] >= float.Parse(rowData.borderGreenTotal))
                                    {
                                        rowText[i][2] = colorIds.Rows[1][0].ToString();
                                    }
                                    else if (rowValue1[i] >= float.Parse(rowData.borderLightgreenTotal))
                                    {
                                        rowText[i][2] = colorIds.Rows[0][0].ToString();
                                    }
                                    else if (rowValue1[i] >= float.Parse(rowData.borderYellowTotal))
                                    {
                                        rowText[i][2] = colorIds.Rows[2][0].ToString();
                                    }
                                    else if (rowValue1[i] >= float.Parse(rowData.borderOrangeTotal))
                                    {
                                        rowText[i][2] = colorIds.Rows[4][0].ToString();
                                    }
                                    else
                                    {
                                        rowText[i][2] = colorIds.Rows[6][0].ToString();
                                    }

                            }
                            else
                            {
                                int i=0;
                                  if (float.Parse(rowData.borderGreenTotal) == 0 && float.Parse(rowData.borderLightgreenTotal) == 0 && float.Parse(rowData.borderYellowTotal) == 0 && float.Parse(rowData.borderOrangeTotal) == 0)
                                    {
                                        rowText[i][2] = colorIds.Rows[5][0].ToString();
                                    }
                                    else
                                        if (rowValue1[i] == float.PositiveInfinity || rowValue1[i] == float.NegativeInfinity)
                                        {
                                            rowText[i][2] = colorIds.Rows[3][0].ToString();
                                        }
                                        else if (rowValue1[i] <= float.Parse(rowData.borderGreenTotal))
                                        {
                                            rowText[i][2] = colorIds.Rows[1][0].ToString();
                                        }
                                        else if (rowValue1[i] <= float.Parse(rowData.borderLightgreenTotal))
                                        {
                                            rowText[i][2] = colorIds.Rows[0][0].ToString();
                                        }
                                        else if (rowValue1[i] <= float.Parse(rowData.borderYellowTotal))
                                        {
                                            rowText[i][2] = colorIds.Rows[2][0].ToString();
                                        }
                                        else if (rowValue1[i] <= float.Parse(rowData.borderOrangeTotal))
                                        {
                                            rowText[i][2] = colorIds.Rows[4][0].ToString();
                                        }
                                        else
                                        {
                                            rowText[i][2] = colorIds.Rows[6][0].ToString();
                                        }
                                }
                            
                        }

                            if (rowData.secondTotalEnabled && rowData.secondTotalHidden)
                            {
                                rowText[0][1] = rowText[0][1].Split('/')[0];
                            }

                            // sort line
                            Array.Sort(rowSortValue, rowText);
                            if (rowData.descendantSorted)
                            {
                                Array.Reverse(rowSortValue);
                                Array.Reverse(rowText);
                            }

                          
                            // write to database;
                            int numberOfCells = 0;
                            SokratesService sokratesServ = new SokratesService();
                            for (int i = 0; i < rowText.Length; i++)
                            {
                                if ((i==0 && rowData.TotalEnabled) || ((rowData.upperBorder == "" || rowSortValue[i] <= float.Parse(rowData.upperBorder))) && ((rowData.lowerBorder == "" || rowSortValue[i] >= float.Parse(rowData.lowerBorder))))
                                {
                                    rowText[i][0] = rowText[i][0].Trim();
                                    // split text
                                    string title1 = rowText[i][0].Length > 50 ? rowText[i][0].Substring(0, 50) : rowText[i][0];
                                    string title2 = rowText[i][0].Length > 100 ? rowText[i][0].Substring(51, 100) : rowText[i][0].Length > 50 ? rowText[i][0].Substring(51) : "";
                                    string title3 = rowText[i][0].Length > 100 ? rowText[i][0].Substring(101) : "";
                                    // replace <>
                                    title1 = title1.Replace("<", "&lt").Replace(">", "&gt");
                                    title2 = title2.Replace("<", "&lt").Replace(">", "&gt");
                                    title3 = title3.Replace("<", "&lt").Replace(">", "&gt");
                                    string newCellId = sokratesServ.dbAddCell(rowId.ToString(), numberOfCells.ToString());
                                    sql = "UPDATE CHARACTERISTIC SET TITLE = '"+title1+"' , TITLE2 ='"+title2+"' , TITLE3 = '"+title3+"' , TITLE4 = '" + rowText[i][1] + "' , COLOR_ID = '" + rowText[i][2] + "' WHERE ID = "+newCellId;
                                    db.execute(sql);
                                    numberOfCells++;
                                }
                            }

                            // set border on this row
                            if (rowData.borderEnabled)
                            {
                                allowedFields = new string[rowValue1.Length];
                                for (int i = 0; i < allowedFields.Length; i++)
                                {
                                    allowedFields[i] = rowText[i][0];
                                    enabledBorderRow = rowId.ToString();
                                }
                            }

                        }


                    }

                } 
                    // add single cells---------------------
                // get cell ids
                DataTable CellIds = db.getDataTable("SELECT CELL_ID FROM GFK_FORMULA_CELL WHERE CELL_ID IN (SELECT CHARACTERISTIC.ID FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID WHERE DIMENSION.MATRIX_ID = " + matrixId+")");
                
                for (int cellNr = 0; cellNr < CellIds.Rows.Count; cellNr++ ) // for each cell
                {
                    string cellId = CellIds.Rows[cellNr].ItemArray[0].ToString();

                    // get data from db
                    CellData cellData = new CellData(int.Parse(cellId), Session);
                    NeededData nd = null;
                    NeededData nd2 = null;
                    // get data from files
                    if (cellData.isQvw && cellData.fact!="")
                    {
                        nd = GetDataFromQVWCell(cellData, path);
                    }
                    else if(cellData.excelSheet !="")
                    {
                        nd = GetDataFromExcelCell(cellData, path);
                    }
                    if (cellData.secondTableEnabled)
                    {
                        string temppath = path2;
                        // set previous year
                        if (cellData.previousYearEnabled)
                        {
                            int year = int.Parse(temppath.Split('_').Last().Split('.')[0].Substring(temppath.Split('_').Last().Split('.')[0].Length-2, 2)) - 1;
                            temppath = temppath.Replace(temppath.Split('_').Last().Split('.')[0], temppath.Split('_').Last().Split('.')[0].Replace((year + 1).ToString(), year.ToString()));
                        }
                        if (cellData.secondIsQVW && cellData.fact2 != "")
                        {
                            nd2 = GetDataFromQVWCell2(cellData, temppath);
                        }
                        else if (cellData.excelSheet2 != "")
                        {
                            nd2 = GetDataFromExcelCell2(cellData, temppath);
                        }

                    }
                    
                    
                    
                    if (nd != null)
                    {
                        if (nd.datatable.Rows.Count == 0) // no data in row
                        {
                            db.execute("UPDATE CHARACTERISTIC SET COLOR_ID = " + colorIds.Rows[3][0].ToString() + " WHERE ID = " + cellId);
                            db.execute("UPDATE CHARACTERISTIC SET TITLE4 = 'n.a.' WHERE ID = " + cellId);
                        }
                        else
                        {
                            // fill values in formula
                            int i = 0;
                            string cellValue = "";
                            string formula1 = cellData.formula1;
                            for (int j = 0; j < nd.datatable.Columns.Count; j++)
                            {
                                formula1 = formula1.Replace("[1." + j + "]", nd.datatable.Rows[i].ItemArray[j].ToString());
                            }
                            if (cellData.secondTableEnabled)
                            {
                                for (int j = 0; j < nd2.datatable.Columns.Count; j++)
                                {
                                    formula1 = formula1.Replace("[2." + j + "]", nd2.datatable.Rows[i].ItemArray[j].ToString());
                                }
                            }
                            float value1;

                            // calculate equation
                            try
                            {
                                var loDataTable = new DataTable();
                                var loDataColumn = new DataColumn("Eval", typeof(float), formula1);
                                loDataTable.Columns.Add(loDataColumn);
                                loDataTable.Rows.Add(0);



                                if (float.TryParse((loDataTable.Rows[0]["Eval"]).ToString(), out value1))
                                {
                                    cellValue = Math.Round(value1, 1, MidpointRounding.ToEven).ToString() + cellData.unit1;

                                }
                            }
                            catch
                            {
                                cellValue = "n.a."; // no value
                                value1 = float.NegativeInfinity; // value if value can not be calculated
                            }



                            // add second value
                            if (cellData.secondValueEnabled)
                            {


                                string formula2 = cellData.formula2;
                                float value2; 
                                // fill in values from tables into formula
                                for (int j = 0; j < nd.datatable.Columns.Count; j++)
                                {
                                    formula2 = formula2.Replace("[1." + j + "]", nd.datatable.Rows[i].ItemArray[j].ToString());
                                }
                                if (cellData.secondTableEnabled)
                                {
                                    for (int j = 0; j < nd2.datatable.Columns.Count; j++)
                                    {
                                        formula2 = formula2.Replace("[2." + j + "]", nd2.datatable.Rows[i].ItemArray[j].ToString());
                                    }
                                }
                                // calculate equation
                                try
                                {
                                    var loDataTable = new DataTable();
                                    var loDataColumn = new DataColumn("Eval", typeof(float), formula2);
                                    loDataTable.Columns.Add(loDataColumn);
                                    loDataTable.Rows.Add(0);

                                    if (float.TryParse((loDataTable.Rows[0]["Eval"]).ToString(), out value2))
                                    {
                                        cellValue += " / " + Math.Round(value2, 1, MidpointRounding.ToEven).ToString() + cellData.unit2;
                                        if (cellData.colorationOnSecondValue)
                                        {
                                            value1 = value2;
                                        }
                                    }
                                } 
                                catch
                                {
                                    cellValue += " / n.a."; // no value  
                                    if (cellData.colorationOnSecondValue)
                                    {
                                        value1 = float.NegativeInfinity;  // value if value can not be calculated
                                    }
                                }
                            }
                            // write values to db
                            db.execute("UPDATE CHARACTERISTIC SET TITLE4 = '" + cellValue + "' WHERE ID = " + cellId);

                            // get color id for cell
                            string colorId;
                            if (float.Parse(cellData.borderGreenLightgreen) == 0 && float.Parse(cellData.borderLightgreenYellow) == 0 && float.Parse(cellData.borderYellowOrange) == 0 && float.Parse(cellData.borderOrangeRed) == 0)
                            {
                                colorId = colorIds.Rows[5][0].ToString();
                            }
                            else if (cellData.highestValueIsGreen)
                            {
                                if (value1 == float.PositiveInfinity || value1 == float.NegativeInfinity)
                                {
                                    colorId = colorIds.Rows[3][0].ToString();
                                }
                                else if (value1 >= float.Parse(cellData.borderGreenLightgreen))
                                {
                                    colorId = colorIds.Rows[1][0].ToString();
                                }
                                else if (value1 >= float.Parse(cellData.borderLightgreenYellow))
                                {
                                    colorId = colorIds.Rows[0][0].ToString();
                                }
                                else if (value1 >= float.Parse(cellData.borderYellowOrange))
                                {
                                    colorId = colorIds.Rows[2][0].ToString();
                                }
                                else if (value1 >= float.Parse(cellData.borderOrangeRed))
                                {
                                    colorId = colorIds.Rows[4][0].ToString();
                                }
                                else
                                {
                                    colorId = colorIds.Rows[6][0].ToString();
                                }

                            }
                            else
                            {
                                if (value1 == float.PositiveInfinity || value1 == float.NegativeInfinity)
                                {
                                    colorId = colorIds.Rows[3][0].ToString();
                                }
                                else if (value1 <= float.Parse(cellData.borderGreenLightgreen))
                                {
                                    colorId = colorIds.Rows[1][0].ToString();
                                }
                                else if (value1 <= float.Parse(cellData.borderLightgreenYellow))
                                {
                                    colorId = colorIds.Rows[0][0].ToString();
                                }
                                else if (value1 <= float.Parse(cellData.borderYellowOrange))
                                {
                                    colorId = colorIds.Rows[2][0].ToString();
                                }
                                else if (value1 <= float.Parse(cellData.borderOrangeRed))
                                {
                                    colorId = colorIds.Rows[4][0].ToString();
                                }
                                else
                                {
                                    colorId = colorIds.Rows[6][0].ToString();
                                }

                            }
                            // write  color to db
                            db.execute("UPDATE CHARACTERISTIC SET COLOR_ID = " + colorId + " WHERE ID = " + cellId);
                        }
                    }
                }
                
                SokratesService sServ = new SokratesService();

                // delete not allowed cells 
                if (allowedFields != null)
                {
                    DataTable allowedFieldsTable = db.getDataTable("SELECT TITLE, TITLE2, TITLE3 FROM CHARACTERISTIC WHERE DIMENSION_ID = "+enabledBorderRow);
                    allowedFields = new string[allowedFieldsTable.Rows.Count];
                    int number = 0;
                    foreach (DataRow cell in allowedFieldsTable.Rows)
                    {
                        allowedFields[number] = allowedFieldsTable.Rows[number][0].ToString() + allowedFieldsTable.Rows[number][1].ToString() + allowedFieldsTable.Rows[number][2].ToString();
                        number++;
                    }
                    foreach (string rowId in usedRows)
                    {
                        Boolean hasTotal = db.lookup("TOTAL_VALUE_ENABLED","GFK_FORMULA","ROW_ID = "+rowId).ToString() =="True";
                        DataTable cellTable = db.getDataTable("SELECT ID, TITLE, TITLE2, TITLE3, ORDNUMBER FROM CHARACTERISTIC WHERE DIMENSION_ID = " + rowId);
                        number = 0;
                        foreach (DataRow cell in cellTable.Rows)
                        {
                            if (!(number == 0 && hasTotal))
                            {
                                if (!allowedFields.Contains(cell[1].ToString() + cell[2].ToString() + cell[3].ToString()))
                                {
                                    sServ.dbDeleteCell(rowId, cell[0].ToString(), cell[4].ToString());
                                    
                                    foreach (DataRow cell2 in cellTable.Rows)
                                    {
                                        if (int.Parse(cell2[4].ToString()) > int.Parse(cell[4].ToString()))
                                        {
                                            cell2[4] = int.Parse(cell2[4].ToString()) - 1;
                                        }
                                       
                                    }
                                }
                            }
                            number++;
                        }
                    }
                }

                // copy map
                string newId = sServ.copySokratesMap(matrixId, true, true);
                db.execute("UPDATE MATRIX SET TITLE = '" + newName + "' WHERE ID = " + newId);

                // delete filled in data
                foreach (string rowIdout in usedRows)
                {
                    db.execute("DELETE FROM CHARACTERISTIC WHERE DIMENSION_ID ='" + rowIdout + "'");
                }

                for (int i = 0; i < CellIds.Rows.Count; i++)
                {
                    db.execute("UPDATE CHARACTERISTIC SET TITLE4 = '', COLOR_ID = " + colorIds.Rows[3][0].ToString() + " WHERE ID = " + CellIds.Rows[i][0].ToString());
                }
            }
            
            db.disconnect();
        }

        //Get Needed Data from excel
        NeededData GetDataFromExcel(RowData rowData, string path)
        {
            NeededData nData = new NeededData();
            Workbook wb = new Workbook();
            wb = Workbook.Load(path);

            Worksheet ws = wb.Sheets[Convert.ToInt16(rowData.excelSheet)];


            int i = 0;
            while ((ws.Rows[i].Cells[0].Value == null || !ws.Rows[i].Cells[0].Value.ToString().Trim().Equals(rowData.title1.Trim())) || (ws.Rows[i + 1].Cells[0].Value == null || !ws.Rows[i + 1].Cells[0].Value.ToString().Trim().Equals(rowData.title2.Trim())))
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

            int startRow = i;           

            ArrayList rowNameList = new ArrayList();
            DataTable dataTable = new DataTable();
            int j = 1;

            //create Columns
            while (ws.Rows[i].Cells[j].Value != null)
            {
                dataTable.Columns.Add("[1."+j+"]"+ws.Rows[i].Cells[j].Value.ToString());
                j++;
            }

            // create rows
            i++;
            while (ws.Rows[i].Cells[0].Value == null)
            {
                i++;
            }
            while (ws.Rows[i].Cells[0].Value != null && ws.Rows[i].Cells[0].Value.ToString() != "0")
            {
                rowNameList.Add(ws.Rows[i].Cells[0].Value.ToString());
                DataRow newRow = dataTable.NewRow();
                

                //create Columns
                for(j=1; j<dataTable.Columns.Count; j++)
                {
                    newRow.SetField(j-1, ws.Rows[i].Cells[j].Value == null ? "n.a." : ws.Rows[i].Cells[j].Value.ToString());                  
                }
                dataTable.Rows.Add(newRow);
                i++;
            }

            nData.datatable=dataTable;
            nData.rowNames = (string[])rowNameList.ToArray(typeof(string));

            return nData;
        }

        //Get Needed Data from excel for second table
        NeededData GetDataFromExcel2(RowData rowData, string path)
        {
            NeededData nData = new NeededData();
            Workbook wb = new Workbook();
            wb = Workbook.Load(path);

            Worksheet ws = wb.Sheets[Convert.ToInt16(rowData.excelSheet2)];


            int i = 0;
            while ((ws.Rows[i].Cells[0].Value == null || !ws.Rows[i].Cells[0].Value.ToString().Trim().Equals(rowData.title12.Trim())) || (ws.Rows[i + 1].Cells[0].Value == null || !ws.Rows[i + 1].Cells[0].Value.ToString().Trim().Equals(rowData.title22.Trim())))
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

            int startRow = i;

            ArrayList rowNameList = new ArrayList();
            DataTable dataTable = new DataTable();
            int j = 1;

            //create Columns
            while (ws.Rows[i].Cells[j].Value != null)
            {
                dataTable.Columns.Add("[2." + j + "]" + ws.Rows[i].Cells[j].Value.ToString());
                j++;
            }

            // create rows
            i++;
            while (ws.Rows[i].Cells[0].Value == null)
            {
                i++;
            }
            while (ws.Rows[i].Cells[0].Value != null && ws.Rows[i].Cells[0].Value.ToString() != "0")
            {
                rowNameList.Add(ws.Rows[i].Cells[0].Value.ToString());
                DataRow newRow = dataTable.NewRow();


                //create Columns
                for (j = 1; j < dataTable.Columns.Count; j++)
                {
                    newRow.SetField(j - 1, ws.Rows[i].Cells[j].Value == null ? "n.a." : ws.Rows[i].Cells[j].Value.ToString());
                }
                dataTable.Rows.Add(newRow);
                i++;
            }

            nData.datatable = dataTable;
            nData.rowNames = (string[])rowNameList.ToArray(typeof(string));

            return nData;
        }


        //Get Needed Data from excel for Cell
        NeededData GetDataFromExcelCell(CellData cellData, string path)
        {
            NeededData nData = new NeededData();
            Workbook wb = new Workbook();
            wb = Workbook.Load(path);

            Worksheet ws = wb.Sheets[Convert.ToInt16(cellData.excelSheet)];


            int i = 0;
            while ((ws.Rows[i].Cells[0].Value == null || !ws.Rows[i].Cells[0].Value.ToString().Trim().Equals(cellData.title1.Trim())) || (ws.Rows[i + 1].Cells[0].Value == null || !ws.Rows[i + 1].Cells[0].Value.ToString().Trim().Equals(cellData.title2.Trim())))
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
            int startRow = i;

            ArrayList rowNameList = new ArrayList();
            DataTable dataTable = new DataTable();
            int j = 1;

            //create Columns
            while (ws.Rows[i].Cells[j].Value != null)
            {
                dataTable.Columns.Add("[1." + j + "]" + ws.Rows[i].Cells[j].Value.ToString());
                j++;
            }

            // create rows
            i++;
            while (ws.Rows[i].Cells[0].Value == null)
            {
                i++;
            }
            while (ws.Rows[i].Cells[0].Value != null && ws.Rows[i].Cells[0].Value.ToString() != "0")
            {
                if (ws.Rows[i].Cells[0].Value.ToString().ToLower() == cellData.rowName.ToLower())
                {
                    rowNameList.Add(ws.Rows[i].Cells[0].Value.ToString().Trim());
                    DataRow newRow = dataTable.NewRow();
                    j = 1;

                    //create Columns
                    while (ws.Rows[i].Cells[j].Value != null)
                    {
                        newRow.SetField(j - 1, ws.Rows[i].Cells[j].Value.ToString());
                        j++;
                    }
                    dataTable.Rows.Add(newRow);
                }
                i++;
            }

            nData.datatable = dataTable;
            nData.rowNames = (string[])rowNameList.ToArray(typeof(string));

            return nData;
        }

        //Get Needed Data from excel for Cell Second table
        NeededData GetDataFromExcelCell2(CellData cellData, string path2)
        {
            NeededData nData = new NeededData();
            Workbook wb = new Workbook();
            wb = Workbook.Load(path2);

            Worksheet ws = wb.Sheets[Convert.ToInt16(cellData.excelSheet2)];


            int i = 0;
            while ((ws.Rows[i].Cells[0].Value == null || !ws.Rows[i].Cells[0].Value.ToString().Trim().Equals(cellData.title12.Trim())) || (ws.Rows[i + 1].Cells[0].Value == null || !ws.Rows[i + 1].Cells[0].Value.ToString().Trim().Equals(cellData.title22.Trim())))
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
            int startRow = i;

            ArrayList rowNameList = new ArrayList();
            DataTable dataTable = new DataTable();
            int j = 1;

            //create Columns
            while (ws.Rows[i].Cells[j].Value != null)
            {
                dataTable.Columns.Add("[1." + j + "]" + ws.Rows[i].Cells[j].Value.ToString());
                j++;
            }

            // create rows
            i++;
            while (ws.Rows[i].Cells[0].Value == null)
            {
                i++;
            }
            while (ws.Rows[i].Cells[0].Value != null && ws.Rows[i].Cells[0].Value.ToString() != "0")
            {
                if (ws.Rows[i].Cells[0].Value.ToString().ToLower() == cellData.rowName2.ToLower())
                {
                    rowNameList.Add(ws.Rows[i].Cells[0].Value.ToString().Trim());
                    DataRow newRow = dataTable.NewRow();
                    j = 1;

                    //create Columns
                    while (ws.Rows[i].Cells[j].Value != null)
                    {
                        newRow.SetField(j - 1, ws.Rows[i].Cells[j].Value.ToString());
                        j++;
                    }
                    dataTable.Rows.Add(newRow);
                }
                i++;
            }

            nData.datatable = dataTable;
            nData.rowNames = (string[])rowNameList.ToArray(typeof(string));

            return nData;
        }
        
        
        // get Needed Data From quickview
        NeededData GetDataFromQVW(RowData rowData, string path)
        {
            NeededData nd = new NeededData();
            StreamReader reader = new StreamReader(path);
            string buffer = "";

            
            while (!reader.EndOfStream)
            {
                buffer = reader.ReadLine();
                if (buffer.Substring(0, 2) == "DC")
                {
                    if (buffer.Substring(3) == rowData.reportTyp)
                    {
                        buffer = reader.ReadLine();
                        if (buffer.Substring(3) == rowData.product)
                        {
                            buffer = reader.ReadLine();
                            if (buffer.Substring(3) == rowData.segment)
                            {
                                buffer = reader.ReadLine();
                                if (buffer.Substring(3) == rowData.fact)
                                {
                                    break;
                                }
                            }
                        }
                    }

                }
            }

            ArrayList rowNameList = new ArrayList();
            DataTable dataTable = new DataTable();
            while (buffer.Substring(0, 2) != "CH")
            {
                buffer = reader.ReadLine();
            }
            string[] dataLine = buffer.Split("\t".ToCharArray());
            for (int i = 3; i < dataLine.Length; i++)
            {
                dataTable.Columns.Add("[1."+(i-3)+"]"+dataLine[i]);
            }

            buffer = reader.ReadLine();
            while (buffer.Substring(0, 2) == "RO")
            {
                DataRow row = dataTable.NewRow();              
                dataLine = buffer.Split("\t".ToCharArray());

                for (int i = 3; i < dataLine.Length; i++)
                {
                    row.SetField(i-3, dataLine[i]);
                }
                buffer = reader.ReadLine();
                rowNameList.Add(dataLine[2].Trim());
                dataTable.Rows.Add(row);

            }

            nd.rowNames = (string[])rowNameList.ToArray(typeof(string));
            nd.datatable = dataTable;
            return nd;
        }

        // get Needed Data From quickview for second table
        NeededData GetDataFromQVW2(RowData rowData, string path)
        {
            NeededData nd = new NeededData();
            StreamReader reader = new StreamReader(path);
            string buffer = "";


            while (!reader.EndOfStream)
            {
                buffer = reader.ReadLine();
                if (buffer.Substring(0, 2) == "DC")
                {
                    if (buffer.Substring(3) == rowData.reportTyp2)
                    {
                        buffer = reader.ReadLine();
                        if (buffer.Substring(3) == rowData.product2)
                        {
                            buffer = reader.ReadLine();
                            if (buffer.Substring(3) == rowData.segment2)
                            {
                                buffer = reader.ReadLine();
                                if (buffer.Substring(3) == rowData.fact2)
                                {
                                    break;
                                }
                            }
                        }
                    }

                }
            }

            ArrayList rowNameList = new ArrayList();
            DataTable dataTable = new DataTable();
            while (buffer.Substring(0, 2) != "CH")
            {
                buffer = reader.ReadLine();
            }
            string[] dataLine = buffer.Split("\t".ToCharArray());
            for (int i = 3; i < dataLine.Length; i++)
            {
                dataTable.Columns.Add("[2." + (i - 3) + "]" + dataLine[i]);
            }

            buffer = reader.ReadLine();
            while (buffer.Substring(0, 2) == "RO")
            {
                DataRow row = dataTable.NewRow();
                dataLine = buffer.Split("\t".ToCharArray());

                for (int i = 3; i < dataLine.Length; i++)
                {
                    row.SetField(i - 3, dataLine[i]);
                }
                buffer = reader.ReadLine();
                rowNameList.Add(dataLine[2].Trim());
                dataTable.Rows.Add(row);

            }

            nd.rowNames = (string[])rowNameList.ToArray(typeof(string));
            nd.datatable = dataTable;
            return nd;
        }


        // get Needed Data From quickview for Cell
        NeededData GetDataFromQVWCell(CellData cellData, string path)
        {
            NeededData nd = new NeededData();
            StreamReader reader = new StreamReader(path);
            string buffer = "";


            while (!reader.EndOfStream)
            {
                buffer = reader.ReadLine();
                if (buffer.Substring(0, 2) == "DC")
                {
                    if (buffer.Substring(3) == cellData.reportTyp)
                    {
                        buffer = reader.ReadLine();
                        if (buffer.Substring(3) == cellData.product)
                        {
                            buffer = reader.ReadLine();
                            if (buffer.Substring(3) == cellData.segment)
                            {
                                buffer = reader.ReadLine();
                                if (buffer.Substring(3) == cellData.fact)
                                {
                                    break;
                                }
                            }
                        }
                    }

                }
            }

            ArrayList rowNameList = new ArrayList();
            DataTable dataTable = new DataTable();
            while (buffer.Substring(0, 2) != "CH")
            {
                buffer = reader.ReadLine();
            }
            string[] dataLine = buffer.Split("\t".ToCharArray());
            for (int i = 3; i < dataLine.Length; i++)
            {
                dataTable.Columns.Add("[1." + i + "]" + dataLine[i]);
            }

            buffer = reader.ReadLine();
            while (buffer.Substring(0, 2) == "RO")
            {
               
                
                dataLine = buffer.Split("\t".ToCharArray());
                if (dataLine[2].Trim().ToLower() == cellData.rowName.ToLower())
                {
                    DataRow row = dataTable.NewRow();
                    for (int i = 3; i < dataLine.Length; i++)
                    {
                        row.SetField(i - 3, dataLine[i]);
                    }
                   
                    rowNameList.Add(dataLine[2]);
                    dataTable.Rows.Add(row);
                }
                buffer = reader.ReadLine();

            }

            nd.rowNames = (string[])rowNameList.ToArray(typeof(string));
            nd.datatable = dataTable;
        
            return nd;
        }

        // get Needed Data From quickview for Cell Second table
        NeededData GetDataFromQVWCell2(CellData cellData, string path2)
        {
            NeededData nd = new NeededData();
           
            if (cellData.secondTableEnabled)
            {
                StreamReader reader2 = new StreamReader(path2);
                string buffer2 = "";


                while (!reader2.EndOfStream)
                {
                    buffer2 = reader2.ReadLine();
                    if (buffer2.Substring(0, 2) == "DC")
                    {
                        if (buffer2.Substring(3) == cellData.reportTyp2)
                        {
                            buffer2 = reader2.ReadLine();
                            if (buffer2.Substring(3) == cellData.product2)
                            {
                                buffer2 = reader2.ReadLine();
                                if (buffer2.Substring(3) == cellData.segment2)
                                {
                                    buffer2 = reader2.ReadLine();
                                    if (buffer2.Substring(3) == cellData.fact2)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                    }
                }

                ArrayList rowNameList2 = new ArrayList();
                DataTable dataTable2 = new DataTable();
                while (buffer2.Substring(0, 2) != "CH")
                {
                    buffer2 = reader2.ReadLine();
                }
                string[] dataLine2 = buffer2.Split("\t".ToCharArray());
                for (int i = 3; i < dataLine2.Length; i++)
                {
                    dataTable2.Columns.Add("[2." + i + "]" + dataLine2[i]);
                }

                buffer2 = reader2.ReadLine();
                while (buffer2.Substring(0, 2) == "RO")
                {

                    dataLine2 = buffer2.Split("\t".ToCharArray());
                    if (dataLine2[2].Trim().ToLower() == cellData.rowName2.ToLower())
                    {
                        DataRow row = dataTable2.NewRow();
                        for (int i = 3; i < dataLine2.Length; i++)
                        {
                            row.SetField(i - 3, dataLine2[i]);
                        }

                        rowNameList2.Add(dataLine2[2]);
                        dataTable2.Rows.Add(row);
                    }
                    buffer2 = reader2.ReadLine();

                }

                nd.rowNames = (string[])rowNameList2.ToArray(typeof(string));
                nd.datatable = dataTable2;
            }
            return nd;
        }
    }

    // has data from quickview or excel
    public class NeededData
    {
        public String[] rowNames;                   // Array with row Names
        public DataTable datatable;                 // Datatable with values and columnnames
    }

    ////class has all data of 1 xml table
    //public class XmlData
    //{
    //    public String[] rowNames;                   // Array with row Names
    //    public DataTable datatable;                 // Datatable from xml
        
    //    //constructor
    //    public XmlData(String path, String tableName)
    //    {
    //        int tableNr = 0;

    //        // load xml file
    //        XmlDocument xmlDoc = new XmlDocument();
    //        xmlDoc.Load(path);

    //        Boolean tableFound = false;
    //        foreach (XmlElement tab in xmlDoc.GetElementsByTagName("tab"))
    //        {
    //            if (tab.Attributes["repname"].InnerText.Equals(tableName))
    //            {
    //                tableFound = true;
    //                break;
    //            }
    //            tableNr++;
    //        }
    //        if (tableFound)
    //        {
    //            //create Datatable
    //            datatable = new DataTable();

    //            //get Rows from XML
    //            XmlNodeList xmlRows = xmlDoc.GetElementsByTagName("tab")[tableNr].SelectNodes("descendant::row");
    //            String firstRow = xmlRows[0].SelectSingleNode("descendant::data").InnerText;

    //            //create Columns
    //            int numberOfColumns = firstRow.Split(",".ToCharArray()).Length;         // get number of Columns
    //            DataColumn[] tableColumns = new DataColumn[numberOfColumns];            // Column Array
              

    //            for (int i = 0; i < numberOfColumns; i++)
    //            {
    //                // create each Column
    //                tableColumns[i] = new DataColumn();
    //                String[] columnRefIds = new String[xmlDoc.GetElementsByTagName("tab")[tableNr].SelectNodes("cols/descendant::col/descendant::itmrefs")[i].InnerText.Split(",".ToCharArray()).Length];
    //                columnRefIds = xmlDoc.GetElementsByTagName("tab")[tableNr].SelectNodes("cols/descendant::col/descendant::itmrefs")[i].InnerText.Split(",".ToCharArray());

    //                // name Column
    //                String columnName = "";
    //                foreach (String itmRefId in columnRefIds)
    //                {
    //                    if (columnName != "")
    //                    {
    //                        columnName += " ";            // string between Column Titles
    //                    }
    //                    columnName += xmlDoc.GetElementsByTagName("tab")[tableNr].SelectNodes("cols/descendant::itm")[Convert.ToInt32(itmRefId)].SelectNodes("txt")[2].InnerText;
    //                }
    //                tableColumns[i].ColumnName = columnName;
    //                datatable.Columns.Add(tableColumns[i]);
                    
    //            }


    //            // create Rows
    //            foreach (XmlNode row in xmlRows)
    //            {
    //                DataRow newRow = datatable.NewRow();                                                    // create New Row
    //                String[] rowData = new string[row.SelectSingleNode("descendant::data").InnerText.Split(",".ToCharArray()).Length];
    //                rowData = row.SelectSingleNode("descendant::data").InnerText.Split(",".ToCharArray());  // row Data Array
    //                int i = 0;                                                                              // counter
    //                foreach (DataColumn column in tableColumns)
    //                {
    //                    //fill each field of Row
    //                    newRow.SetField(column, rowData[i]);
    //                    i++;
    //                }
    //                datatable.Rows.Add(newRow);
    //            }


    //            // get row Names
    //            rowNames = new String[xmlRows.Count];                                              // Row Name Array (Brands)
    //            int rowNumber = 0;                                                                          // counter
    //            foreach (XmlNode row in xmlRows)
    //            {
    //                String[] itmRefIds = new String[row.SelectSingleNode("descendant::itmrefs").InnerText.Split(",".ToCharArray()).Length];   // itmrefids
    //                itmRefIds = row.SelectSingleNode("descendant::itmrefs").InnerText.Split(",".ToCharArray());
    //                rowNames[rowNumber] = "";
    //                foreach (String itmRefId in itmRefIds)
    //                {
    //                    if (rowNames[rowNumber] != "")
    //                    {
    //                        rowNames[rowNumber] += " ";                                                   // string between Row Titles
    //                    }
    //                    rowNames[rowNumber] += xmlDoc.GetElementsByTagName("tab")[tableNr].SelectNodes("rows/descendant::itm")[Convert.ToInt32(itmRefId)].SelectNodes("txt")[2].InnerText;
    //                }
    //                rowNumber++;
    //            }
    //        }
    //    }

    //    // get names of columns in string array
    //    public String[] getColumnnames()
    //    {
    //        String[] columnNames = new String[datatable.Columns.Count];
    //        int columncount = 0;
    //        foreach (DataColumn column in datatable.Columns)
    //        {
    //            columnNames[columncount] = "[" + column.ColumnName.Replace("<", "&lt").Replace(">", "&gt") + "]";
    //            columncount++;
    //        }
    //        return columnNames;
    //    }

    //}

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
            if (db.lookup("ID", "GFK_FORMULA", "ROW_ID=" + rowID, "").ToString() == "")
            {
                string sql = "INSERT INTO GFK_FORMULA (ROW_ID) VALUES(" + rowID + ")";
                db.execute(sql);
            }// read data from db

            reportTyp = db.lookup("REPORT_TYP", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            product = db.lookup("PRODUCT", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            segment = db.lookup("SEGMENT", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            fact = db.lookup("FACT", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            excelSheet = db.lookup("EXCELSHEET", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            title1 = db.lookup("TABLENAME1", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            title2 = db.lookup("TABLENAME2", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            formula1 = db.lookup("FORMULA1", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            unit1 = db.lookup("UNIT1", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            formula2 = db.lookup("FORMULA2", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            unit2 = db.lookup("UNIT2", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            formulaTotal = db.lookup("FORMULA_TOTAL", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            unitTotal = db.lookup("UNIT_TOTAL", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            upperBorder = db.lookup("UPPER_BORDER", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            lowerBorder = db.lookup("LOWER_BORDER", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            borderGreenLightgreen = db.lookup("BORDER_GREEN_LIGHTGREEN", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            borderLightgreenYellow = db.lookup("BORDER_LIGHTGREEN_YELLOW", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            borderYellowOrange = db.lookup("BORDER_YELLOW_ORANGE", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            borderOrangeRed = db.lookup("BORDER_ORANGE_RED", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            borderGreenTotal = db.lookup("BORDER_GREEN_LIGHTGREEN_T", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            borderLightgreenTotal = db.lookup("BORDER_LIGHTGREEN_YELLOW_T", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            borderYellowTotal = db.lookup("BORDER_YELLOW_ORANGE_T", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            borderOrangeTotal = db.lookup("BORDER_ORANGE_RED_T", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            numberOfEntries = db.lookup("NUMBER_OF_ENTRIES", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            reportTyp2 = db.lookup("SECOND_REPORT_TYP", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            product2 = db.lookup("SECOND_PRODUCT", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            segment2 = db.lookup("SECOND_SEGMENT", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            fact2 = db.lookup("SECOND_FACT", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            excelSheet2 = db.lookup("SECOND_EXCELSHEET", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            title12 = db.lookup("SECOND_TABLENAME1", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            title22 = db.lookup("SECOND_TABLENAME2", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            secondFormulaTotal = db.lookup("SECOND_FORMULA_TOTAL", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();
            secondUnitTotal = db.lookup("SECOND_UNIT_TOTAL", "GFK_FORMULA", "ROW_ID= " + rowID).ToString();

            countEnabled = db.lookup("COUNT_ENABLED", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            TotalEnabled = db.lookup("TOTAL_VALUE_ENABLED", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            secondValueEnabled = db.lookup("SECOND_VALUE_ENABLED", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            descendantSorted = db.lookup("DESCENDANT_SORTED", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            highestValueIsGreen = db.lookup("HIGHEST_VALUE_IS_GREEN", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            isQvw = db.lookup("IS_QVW", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            colorationOnSecondValue = db.lookup("COLORATION_SECOND_VALUE", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            sortationOnSecondValue = db.lookup("SORTATION_SECOND_VALUE", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            secondIsQVW = db.lookup("SECOND_IS_QVW", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            secondTableEnabled = db.lookup("SECOND_TABLE_ENABLED", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            previousYearEnabled = db.lookup("PREVIOUS_YEAR_ENABLED", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            borderEnabled = db.lookup("ENABLE_BORDER", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            colorationOnSecondTotal = db.lookup("COLORATION_SECOND_TOTAL", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            secondTotalEnabled = db.lookup("SECOND_TOTAL_ENABLED", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            secondTotalHidden = db.lookup("SECOND_TOTAL_HIDDEN", "GFK_FORMULA", "ROW_ID= " + rowID).ToString() == "True";
            db.disconnect();
        }

        // write data to database
        public void WriteToDB()
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            string isQvwOut = isQvw ? "1" : "0";
            string secondValueEnabledOut = secondValueEnabled ? "1" : "0";
            string descendantOrder = descendantSorted ? "1" : "0";
            string highestIsGreenOut = highestValueIsGreen ? "1" : "0";
            string enableTotalOut = TotalEnabled ? "1" : "0";
            string countEnabledOut = countEnabled ? "1" : "0";
            string borderEnabledOut = borderEnabled ? "1" : "0";
            string sortSecondValueOut = sortationOnSecondValue ? "1" : "0";
            string colorSecondValueOut = colorationOnSecondValue ? "1" : "0";
            string secondIsQVWOut = secondIsQVW ? "1" : "0";
            string secondTableEnabledOut = secondTableEnabled ? "1" : "0";
            string previousYearEnabledOut = previousYearEnabled ? "1" : "0";
            string colorationSecondTotalOut = colorationOnSecondTotal ? "1" : "0";
            string secondTotalEnabledOut = secondTotalEnabled ? "1" : "0";
            string secondTotalHiddenOut = secondTotalHidden ? "1" : "0";

            upperBorder = upperBorder == "" ? "NULL" : upperBorder;
            lowerBorder = lowerBorder == "" ? "NULL" : lowerBorder;
            string sql = "UPDATE GFK_FORMULA SET SECOND_TOTAL_HIDDEN = '" + secondTotalHiddenOut + "' , SECOND_FORMULA_TOTAL = '" + secondFormulaTotal + "' , SECOND_UNIT_TOTAL = '" + secondUnitTotal + "' , SECOND_TOTAL_ENABLED = '" + secondTotalEnabledOut + "' , COLORATION_SECOND_TOTAL = '" + colorationSecondTotalOut + "' , SECOND_IS_QVW = '" + secondIsQVWOut + "' , PREVIOUS_YEAR_ENABLED = '" + previousYearEnabledOut + "' , SECOND_TABLE_ENABLED = '" + secondTableEnabledOut + "' , SORTATION_SECOND_VALUE = '" + sortationOnSecondValue + "' , COLORATION_SECOND_VALUE = '" + colorSecondValueOut + "' , ENABLE_BORDER = '" + borderEnabledOut + "' , UNIT_TOTAL ='" + unitTotal + "' , FORMULA_TOTAL = '" + formulaTotal + "' , COUNT_ENABLED ='" + countEnabledOut + "' , NUMBER_OF_ENTRIES= '" + numberOfEntries + "' , TOTAL_VALUE_ENABLED = '" + enableTotalOut + "' , BORDER_GREEN_LIGHTGREEN_T = '" + borderGreenTotal + "' , BORDER_LIGHTGREEN_YELLOW_T = '" + borderLightgreenTotal + "' , BORDER_YELLOW_ORANGE_T = '" + borderYellowTotal + "' , BORDER_ORANGE_RED_T = '" + borderOrangeTotal + "' , IS_QVW = '" + isQvwOut + "' , EXCELSHEET = '" + excelSheet + "' ,TABLENAME1 = '" + title1 + "' ,TABLENAME2 = '" + title2 + "' , REPORT_TYP = '" + reportTyp + "' , PRODUCT = '" + product + "' , SEGMENT = '" + segment + "' , FACT = '" + fact + "' , SECOND_EXCELSHEET = '" + excelSheet2 + "' ,SECOND_TABLENAME1 = '" + title12 + "' ,SECOND_TABLENAME2 = '" + title22 + "' , SECOND_REPORT_TYP = '" + reportTyp2 + "' , SECOND_PRODUCT = '" + product2 + "' , SECOND_SEGMENT = '" + segment2 + "' , SECOND_FACT = '" + fact2 + "' , FORMULA1 = '" + formula1 + "', UNIT1 = '" + unit1 + "', FORMULA2 = '" + formula2 + "', UNIT2 = '" + unit2 + "', SECOND_VALUE_ENABLED = '" + secondValueEnabledOut + "', DESCENDANT_SORTED = '" + descendantOrder + "', UPPER_BORDER = " + upperBorder + ", LOWER_BORDER = " + lowerBorder + ", HIGHEST_VALUE_IS_GREEN = '" + highestIsGreenOut + "' , BORDER_GREEN_LIGHTGREEN = '" + borderGreenLightgreen + "', BORDER_LIGHTGREEN_YELLOW = '" + borderLightgreenYellow + "', BORDER_YELLOW_ORANGE = '" + borderYellowOrange + "', BORDER_ORANGE_RED = '" + borderOrangeRed + "' WHERE ROW_ID = '" + rowID + "'";
            db.execute(sql);
            db.disconnect();
        }
    }

    //  class has data of 1 sokrates cell
    public class CellData
    {
        int cellID;
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
            rowName2 = db.lookup("SECOND_ROW_NAME", "GFK_FORMULA_CELL", "CELL_ID= " + cellID).ToString().Replace("&lt", "<").Replace("&gt", ">");
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