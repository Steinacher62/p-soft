using ch.appl.psoft.db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ch.appl.psoft.Morph
{
    public class MatrixDataContainer
    {
        private DataTable matrixData;
        public DataTable dimensionData;
        private DataTable characteristicData;

        public int rowCount;
        public int colCount;
        public int firstcolumns;

        public CharacteristicDataContainer[,] matrixTable;

        public MatrixDataContainer(long matrixID, DBData db)
        {
            db.connect();
           
            matrixData = db.getDataTable("SELECT MATRIX.* FROM MATRIX WHERE(ID = "+matrixID+")");
            dimensionData = db.getDataTable("SELECT DIMENSION.* FROM DIMENSION WHERE(MATRIX_ID = " + matrixID + ") ORDER BY ORDNUMBER");
            characteristicData = db.getDataTable("SELECT DIMENSION.ORDNUMBER AS DimOrdNr, CHARACTERISTIC.*, DIMENSION.MATRIX_ID FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID WHERE(DIMENSION.MATRIX_ID = "+matrixID+") ORDER BY DIMENSION.ORDNUMBER , CHARACTERISTIC.ORDNUMBER");

            rowCount = dimensionData.Rows.Count;

            colCount = 0;
            foreach (DataRow row in characteristicData.Rows) {
                if ((int)row["ORDNUMBER"] > colCount)
                {
                    colCount = (int)row["ORDNUMBER"];
                }
            }
            colCount++;

            if (colCount < 10) colCount = 10;

            firstcolumns = (int)matrixData.Rows[0]["TITLECOL2"];

            matrixTable = new CharacteristicDataContainer[rowCount, colCount];

            generateMatrixText();

            db.disconnect();
        }


        public MatrixDataContainer(long matrixID, DBData db, Boolean isNovisReport)
        {
            if (isNovisReport) {
                db.execute("EXEC GET_Matrix " + matrixID + ", " + db.userId + ", 1");
                matrixData = db.getDataTable("SELECT MATRIX.* FROM MATRIX WHERE (NOVIS_ROOT_ID = " + matrixID + ")");
                string matrixIdList = "(";
                foreach(DataRow row in matrixData.Rows)
                {
                    matrixIdList += row["ID"]+", " ;
                }
                matrixIdList = matrixIdList.Substring(0, matrixIdList.Length - 2) + ")";
                dimensionData = db.getDataTable("SELECT DIMENSION.* FROM DIMENSION WHERE(MATRIX_ID = " + matrixID + ") ORDER BY MATRIX_ID,ORDNUMBER");
                dimensionData.Merge( db.getDataTable("SELECT DIMENSION.* FROM DIMENSION WHERE(MATRIX_ID IN " + matrixIdList + " AND MATRIX_ID <> "+matrixID+") ORDER BY MATRIX_ID,ORDNUMBER"));
                characteristicData = db.getDataTable("SELECT DIMENSION.ORDNUMBER AS DimOrdNr, CHARACTERISTIC.*, DIMENSION.MATRIX_ID FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID WHERE(DIMENSION.MATRIX_ID IN " + matrixIdList + " AND MATRIX_ID = " + matrixID+") ORDER BY DIMENSION.MATRIX_ID, DIMENSION.ORDNUMBER , CHARACTERISTIC.ORDNUMBER");
                characteristicData.Merge(db.getDataTable("SELECT DIMENSION.ORDNUMBER AS DimOrdNr, CHARACTERISTIC.*, DIMENSION.MATRIX_ID FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID WHERE(DIMENSION.MATRIX_ID IN " + matrixIdList + " AND MATRIX_ID <> " + matrixID+") ORDER BY DIMENSION.MATRIX_ID, DIMENSION.ORDNUMBER , CHARACTERISTIC.ORDNUMBER"));
                colCount = 0;
                int counter = 0;
                long dimId = (long)characteristicData.Rows[0]["DIMENSION_ID"];
                foreach(DataRow row in characteristicData.Rows)
                {
                    if((long)row["DIMENSION_ID"]!= dimId)
                    {
                        counter++;
                        dimId = (long)row["DIMENSION_ID"];
                    }
                    row["DimOrdNr"] = counter;

                    if ((int)row["ORDNUMBER"] > colCount)
                    {
                        colCount = (int)row["ORDNUMBER"];
                    }
                    row["COLOR_ID"] = 0;
                }

                rowCount = dimensionData.Rows.Count;

                colCount++;

                if (colCount < 10) colCount = 10;

                firstcolumns = (int)matrixData.Rows[0]["TITLECOL2"];

                matrixTable = new CharacteristicDataContainer[rowCount, colCount];


                foreach (DataRow row in characteristicData.Rows)
                {
                    int colnr = (int)row["ORDNUMBER"];
                    matrixTable[(int)row["DimOrdNr"], colnr] = new CharacteristicDataContainer(row, colnr == 0 || colnr == 1 && firstcolumns == 1);

                }

                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        if (matrixTable[i, j] == null)
                        {
                            matrixTable[i, j] = new CharacteristicDataContainer(j == 0 || j == 1 && firstcolumns == 1, (long)dimensionData.Rows[i]["ID"]);
                        }else { 
                         
                        db.execute("UPDATE Matrix_" + db.userId + " SET CellColor_" + j + " = '0' WHERE DimensionId = " + (long)dimensionData.Rows[i]["ID"] + " and Line = 1");
                        }
                    }
                }

                db.disconnect();
            } else{
                
        }
        }

        private void generateMatrixText()
        {

           
            foreach (DataRow row in characteristicData.Rows)
            {
                int colnr = (int)row["ORDNUMBER"];
                matrixTable[(int)row["DimOrdNr"], colnr] = new CharacteristicDataContainer(row, colnr == 0 || colnr == 1 && firstcolumns == 1);

            }

            for(int i=0; i<rowCount; i++)
            {
                for (int j =0; j< colCount; j++)
                {
                    if (matrixTable[i, j] == null)
                    {
                        matrixTable[i, j] = new CharacteristicDataContainer(j == 0 || j == 1 && firstcolumns == 1, (long)dimensionData.Rows[i]["ID"]);
                    }
                }
            }
        }
    }

    public class CharacteristicDataContainer
    {
        public bool isFirstCol;
        public String title;
        public String subtitle;
        public long colorID;
        public string ID;
        public long UID;
        public long subMatrixID;
        public long knowledgeID;
        public long dimensionID;
        public CharacteristicDataContainer(bool isFirstCol, long dimension_ID)
        {
            this.isFirstCol = isFirstCol;
            title = "";
            subtitle = "";
            colorID = 0;
            ID = "";
            UID = 0;
            subMatrixID = 0;
            dimensionID = dimension_ID;
            knowledgeID = 0;
        }
        public CharacteristicDataContainer(DataRow row, Boolean isFirstCol)
        {
            this.isFirstCol = isFirstCol;
            title = row["TITLE"] == DBNull.Value ? "" : (string)row["TITLE"];
            subtitle = row["SUBTITLE"] == DBNull.Value ? "" : (string)row["SUBTITLE"];
            colorID = row["COLOR_ID"] == DBNull.Value ? 0 : (long)row["COLOR_ID"];
            ID =  ((long)row["ID"]).ToString();
            UID =  (long)row["UID"];
            subMatrixID = row["DETAIL_MATRIX_ID"] == DBNull.Value ? 0 : (long)row["DETAIL_MATRIX_ID"];
            dimensionID = row["DIMENSION_ID"] == DBNull.Value ? 0 : (long)row["DIMENSION_ID"];
            knowledgeID = row["KNOWLEDGE_ID"] == DBNull.Value ? 0 : (long)row["KNOWLEDGE_ID"];
            
    }

    }
 }