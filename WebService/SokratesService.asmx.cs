using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.db;
using ch.psoft.Util;
using HiQPdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
namespace ch.appl.psoft.WebService
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für SokratesService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class SokratesService : System.Web.Services.WebService
    {
        public SokratesService()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
        }
        #region Component Designer generated code
        //Required by the Web Services Designer
        private IContainer components = null;
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string addNewSokrates(string template)
        {
            string sql;
            string matrixId = "0";
            string matrixUid = "0";
            //string dimensionId;
            string createDate = String.Format("{0:MM/dd/yyyy}", System.DateTime.Now);
            string createDateTime = String.Format("{0:MM/dd/yyyy hh:mm:ss}", System.DateTime.Now);
            string title = "Neue Sokrateskarte " + String.Format("{0:dd/MM/yyyy hh:mm:ss}", System.DateTime.Now);
            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                if (Global.Config.shopActive)
                {
                    int numCards = (int)db.lookup("NUMBER_OF_CARDS", "PERSON", "ID=" + db.userId);
                    if (numCards < 1)
                    {
                        Session.Add("OrderNeed", true);
                        return "OrderNeed";
                    }
                    else
                    {
                        db.execute("UPDATE PERSON SET NUMBER_OF_CARDS = " + (numCards - 1) + "WHERE ID=" + db.userId);
                    }
                }
                if (template.Equals("novis"))
                {
                    sql = "INSERT INTO MATRIX (TITLE,DESCRIPTION,SUBTITLE,CREATIONDATE,LASTCHANGE,REVISION,TITLECOL2,IS_NOVIS_TEMPLATE) VALUES('" + title + "','Beschreibung','Untertitel','" + createDate + "','" + createDate + "','" + createDate + "',1,1)";
                }
                else if (template.Equals("gfk"))
                {
                    sql = "INSERT INTO MATRIX (TITLE,DESCRIPTION,SUBTITLE,CREATIONDATE,LASTCHANGE,REVISION,TITLECOL2,IS_GFK_TEMPLATE) VALUES('" + title + "','Beschreibung','Untertitel','" + createDate + "','" + createDate + "','" + createDate + "',1,1)";
                }
                else if (template.Equals("sbs"))
                {
                    sql = "INSERT INTO MATRIX (TITLE,DESCRIPTION,SUBTITLE,CREATIONDATE,LASTCHANGE,REVISION,TITLECOL2,IS_SBS_TEMPLATE) VALUES('" + title + "','Beschreibung','Untertitel','" + createDate + "','" + createDate + "','" + createDate + "',1,1)";
                }
                else
                {
                    sql = "INSERT INTO MATRIX (TITLE,DESCRIPTION,SUBTITLE,CREATIONDATE,LASTCHANGE,REVISION,TITLECOL2) VALUES('" + title + "','Beschreibung','Untertitel','" + createDate + "','" + createDate + "','" + createDate + "',1)";
                }
                db.execute(sql);
                matrixId = db.lookup("ID", "MATRIX", "TITLE = '" + title + "'").ToString();
                matrixUid = db.lookup("UID", "MATRIX", "TITLE = '" + title + "'").ToString();
                if (template.Equals("novis"))
                {
                    sql = "UPDATE MATRIX SET NOVIS_ROOT_ID = " + matrixId + " WHERE ID = " + matrixId;
                    db.execute(sql);
                    sql = "UPDATE MATRIX SET SUBTITLE = 'Kundennummer' WHERE ID = " + matrixId;
                    db.execute(sql);
                }
                for (int i = 0; i < 8; i++)
                {
                    sql = "INSERT INTO DIMENSION (MATRIX_ID,ORDNUMBER,TITLEROWSPAN) VALUES (" + matrixId + "," + i + ",1)";
                    db.execute(sql);
                    //string lastDimensionId = db.lookup("MAX(ID)", "DIMENSION", "MATRIX_ID=" + matrixId).ToString();
                    //for (int i1 = 0; i1 < 9; i1++)
                    //{
                    //    db.execute("INSERT INTO CHARACTERISTIC (DIMENSION_ID, ORDNUMBER) VALUES (" + lastDimensionId + "," +i1 +")");
                    //}
                }
                //add 2 cells for each row
                DataTable rows = db.getDataTable("SELECT ID FROM DIMENSION WHERE MATRIX_ID = " + matrixId);
                foreach (DataRow row in rows.Rows)
                {
                    db.execute("INSERT INTO CHARACTERISTIC (DIMENSION_ID, ORDNUMBER) VALUES (" + row["ID"] + ", 0 )");
                    db.execute("INSERT INTO CHARACTERISTIC (DIMENSION_ID, ORDNUMBER) VALUES (" + row["ID"] + ", 1 )");
                }
                //add coloration
                if (Global.Config.isModuleEnabled("mks"))
                {
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (0," + matrixId + ",'OK, stabil gut und positiver Trend', -6029483)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (1," + matrixId + ",'Übererfüllung, irritierend gut', -12396288)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (2," + matrixId + ",'Probleme, Trend unklar, Monitoring nötig', -2724)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (3," + matrixId + ",'Noch nicht eingeschätzt, unklar', -1)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (4," + matrixId + ",'Grosse Probleme, Verbesserung dringend', -86472)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (5," + matrixId + ",'Derzeit (noch) nicht relevant', -2500135)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (6," + matrixId + ",'Gefährlich für das System, sofort eingreifen', -243619)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (7," + matrixId + ",'Idee, Vision, Projekt entwickeln', -7424797)";
                    db.execute(sql);
                }
                else
                {
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (0," + matrixId + ",'OK, stabil gut und positiver Trend', -2098243)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (1," + matrixId + ",'Übererfüllung, irritierend gut', -5846118)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (2," + matrixId + ",'Probleme, Trend unklar, Monitoring nötig', -1344)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (3," + matrixId + ",'Noch nicht eingeschätzt, unklar', -461063)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (4," + matrixId + ",'Grosse Probleme, Verbesserung dringend', -76910)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (5," + matrixId + ",'Derzeit (noch) nicht relevant', -1250323)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (6," + matrixId + ",'Gefährlich für das System, sofort eingreifen', -477503)";
                    db.execute(sql);
                    sql = "INSERT INTO COLORATION(ORDNUMBER,MATRIX_ID,TITLE,COLOR) VALUES (7," + matrixId + ",'Idee, Vision, Projekt entwickeln', -4141594)";
                    db.execute(sql);
                }
                //Wirkungspakete erstellen
                sql = "INSERT INTO WIRKUNGSPAKET(MATRIX_ID,ORDNUMBER,COLOR,NAME) VALUES (" + matrixId + ",1,-46505,'WP1')";
                db.execute(sql);
                sql = "INSERT INTO WIRKUNGSPAKET(MATRIX_ID,ORDNUMBER,COLOR,NAME) VALUES (" + matrixId + ",2,-10118427,'WP2')";
                db.execute(sql);
                sql = "INSERT INTO WIRKUNGSPAKET(MATRIX_ID,ORDNUMBER,COLOR,NAME) VALUES (" + matrixId + ",3,-31720,'WP3')";
                db.execute(sql);
                sql = "INSERT INTO WIRKUNGSPAKET(MATRIX_ID,ORDNUMBER,COLOR,NAME) VALUES (" + matrixId + ",4,-12462586,'WP4')";
                db.execute(sql);
                sql = "INSERT INTO WIRKUNGSPAKET(MATRIX_ID,ORDNUMBER,COLOR,NAME) VALUES (" + matrixId + ",5,-6843241,'WP5')";
                db.execute(sql);
                setMatrixrights(db, matrixId);
            }
            catch (Exception e)
            {
                Logger.Log(e, Logger.ERROR);
                db.rollback();
            }
            finally
            {
                db.disconnect();
            }
            return matrixId + "&UID=" + matrixUid;
        }
        private void setMatrixrights(DBData db, string matrixId)
        {
            long groupAccessorId = DBColumn.GetValid(db.lookup("ID", "ACCESSOR", "TITLE = 'Administratoren'"), (long)-1);
            if (Global.Config.shopActive && !db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true))
            {
                if (Global.Config.getModuleParam("morph", "permissionMultiuser", "0") == "1")
                {
                    db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, db.userAccessorID, "Matrix", Convert.ToInt32(matrixId));
                }
                else
                {
                    db.grantRowAuthorisation(DBData.AUTHORISATION.RUDI, db.userAccessorID, "Matrix", Convert.ToInt32(matrixId));
                }
            }
            else
            {
                db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, db.userAccessorID, "Matrix", Convert.ToInt32(matrixId));
            }
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string delSokrates(string matrixId)
        {
            DBData db = DBData.getDBData(Session);
            if (Global.isModuleEnabled("novis"))
            {
                DataTable submatrixCells = db.getDataTable("SELECT     CHARACTERISTIC.ID"
                       + " FROM         CHARACTERISTIC INNER JOIN"
                       + " DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID INNER JOIN"
                       + " MATRIX ON DIMENSION.MATRIX_ID = MATRIX.ID"
                       + " WHERE     (CHARACTERISTIC.DETAIL_MATRIX_ID <> '') AND (MATRIX.ID = " + matrixId + ")");
                for (int i = 0; i < submatrixCells.Rows.Count; i++)
                {
                    delsubMatrix(submatrixCells.Rows[i][0].ToString());
                }
            }
            dbUpdate("UPDATE CHARACTERISTIC SET DETAIL_MATRIX_ID = null WHERE DETAIL_MATRIX_ID = " + matrixId);
            DataTable submatrix = db.getDataTable("SELECT CHARACTERISTIC.ID FROM KNOWLEDGE INNER JOIN CHARACTERISTIC ON KNOWLEDGE.ID = CHARACTERISTIC.KNOWLEDGE_ID INNER JOIN DIMENSION ON dbo.CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID INNER JOIN MATRIX ON DIMENSION.MATRIX_ID = MATRIX.ID WHERE (MATRIX.ID = " + matrixId + " AND KNOWLEDGE.LOCAL = 1)");
            foreach (DataRow row in submatrix.Rows)
            {
                dbUpdate("UPDATE CHARACTERISTIC SET KNOPWLWDGW_ID = '' WHERE ID = " + row[0]);
            }
            dbUpdate("DELETE FROM Wirkungselement WHERE Wirkungspaket_ID IN (SELECT ID FROM Wirkungspaket WHERE MATRIX_ID = " + matrixId + " )");
            dbUpdate("DELETE FROM Wirkungspaket WHERE MATRIX_ID = " + matrixId);
            dbUpdate("DELETE FROM CHARACTERISTIC WHERE CHARACTERISTIC.DIMENSION_ID IN (SELECT ID FROM DIMENSION WHERE MATRIX_ID = " + matrixId + ")");
            dbUpdate("DELETE FROM DIMENSION WHERE MATRIX_ID = " + matrixId);
            dbUpdate("DELETE FROM COLORATION WHERE MATRIX_ID = " + matrixId);
            return dbUpdate("DELETE FROM MATRIX WHERE ID = " + matrixId);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getPermission(string MatrixId)
        {
            DBData data = DBData.getDBData(this.Session);
            bool authorisation = data.hasRowAuthorisation(4, "Matrix", Int64.Parse(MatrixId), true, true);
            string permission = "0";
            if (authorisation)
            {
                permission = "1";
            }
            return permission;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string rowSpan(string DimensionId, string rows)
        {
            if (Global.isModuleEnabled("novis"))
            {
                dbUpdate("UPDATE DIMENSION SET TITLEROWSPAN=" + rows + " FROM MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID  WHERE (MATRIX.NOVIS_TEMPLATE_ID IN (SELECT MATRIX_ID FROM DIMENSION WHERE ID=" + DimensionId + ") AND (DIMENSION.ORDNUMBER IN (SELECT ORDNUMBER FROM DIMENSION WHERE ID = " + DimensionId + ")))");
            }
            return dbUpdate("UPDATE DIMENSION SET TITLEROWSPAN = " + rows + " WHERE ID = " + DimensionId);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string changeHelpColumn(string MatrixId, string OnOff)
        {
            if (Global.isModuleEnabled("novis"))
            {
                dbUpdate("UPDATE MATRIX SET TITLECOL2 = " + OnOff + " WHERE NOVIS_TEMPLATE_ID = " + MatrixId);
            }
            return dbUpdate("UPDATE MATRIX SET TITLECOL2 = " + OnOff + " WHERE ID = " + MatrixId);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object loadSubmatrix()
        {
            DBData db = DBData.getDBData(Session);
            DataTable submatrix;
            if (Global.isModuleEnabled("gfk"))
            {
                submatrix = db.getDataTable("SELECT DISTINCT ID, TITLE FROM MATRIX " + db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " WHERE IS_GFK_TEMPLATE = 0 ORDER BY TITLE");
                DataTable submatrix1 = db.getDataTable("SELECT DISTINCT ID, TITLE FROM MATRIX " + db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.ADMIN, DBData.APPLICATION_RIGHT.COMMON, true, true) + " WHERE IS_GFK_TEMPLATE = 1 ORDER BY TITLE");
                submatrix.Merge(submatrix1);
                submatrix.DefaultView.Sort = "TITLE";
            }
            else
            {
                submatrix = db.getDataTable("SELECT DISTINCT ID, TITLE FROM MATRIX " + db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " ORDER BY TITLE DESC");
            }
            return GetList(submatrix);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object loadKnowledge(string MatrixId)
        {
            DBData db = DBData.getDBData(Session);
            DataTable submatrix = db.getDataTable("SELECT DISTINCT KNOWLEDGE.ID, THEME.TITLE FROM KNOWLEDGE " + db.getAccessRightsRowInnerJoinSQL("KNOWLEDGE", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " INNER JOIN THEME ON KNOWLEDGE.BASE_THEME_ID_DE = THEME.ID WHERE(" + db.userAccessorID + db.getAccessorIDsSQLInClause(db.userAccessorID) + ") AND (LOCAL = 0 or KNOWLEDGE.ID in (SELECT KNOWLEDGE.ID FROM MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID INNER JOIN KNOWLEDGE ON CHARACTERISTIC.KNOWLEDGE_ID = KNOWLEDGE.ID WHERE (MATRIX.ID = " + MatrixId + "))) ORDER BY THEME.TITLE DESC");
            return GetList(submatrix);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string dbColorChange(string MatrixId, string CellId, int NewColor)
        {
            MatrixId = MatrixId.Replace("&UID", "");
            if (MatrixId.StartsWith("Novis"))
            {
                MatrixId = MatrixId.Split('s')[1];
                DBData db = DBData.getDBData(Session);
                db.connect();
                String ordnr = db.lookup("ORDNUMBER", "CHARACTERISTIC", "ID=" + CellId).ToString();
                string Color = db.lookup("ID", "COLORATION", "MATRIX_ID = " + MatrixId + " AND ORDNUMBER = " + NewColor).ToString();
                string sql = "UPDATE Matrix_" + db.userId.ToString() + " SET CellColor_" + ordnr + " = " + Color + " WHERE CellId_" + ordnr + " = " + CellId;
                return dbUpdate(sql);
            }
            else
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                object colorObj = db.lookup("ID", "COLORATION", "MATRIX_ID = " + MatrixId + " AND ORDNUMBER = " + NewColor);
                string Color = colorObj != null ? colorObj.ToString() : "NULL";
                if (Global.isModuleEnabled("novis"))
                {
                    String ordnr = db.lookup("ORDNUMBER", "CHARACTERISTIC", "ID=" + CellId).ToString();
                    string sql = "UPDATE Matrix_" + db.userId.ToString() + " SET CellColor_" + ordnr + " = " + Color + " WHERE CellId_" + ordnr + " = " + CellId;
                    dbUpdate(sql);
                }
                db.disconnect();
                return dbUpdate("UPDATE CHARACTERISTIC SET COLOR_ID = " + Color + " WHERE ID = " + CellId);
            }
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string dbCellTextChange(String MatrixId, String RowId, String CellId, String Line0, String Line1, String Line2)
        {
            Line0 = Line0.Replace("'", "''");
            Line1 = Line1.Replace("'", "''");
            Line2 = Line2.Replace("'", "''");
            string sql = "";
            if (Global.isModuleEnabled("novis"))
            {
                //if (CellId == "Title0" || CellId == "Title1")
                //{
                //    if (CellId == "Title0")
                //    {
                //        sql = "UPDATE DIMENSION SET TITLE = '" + Line0 + "',TITLE2 = '" + Line1 + "' ,TITLE3 = '" + Line2 + "' WHERE ORDNUMBER IN (SELECT ORDNUMBER FROM DIMENSION WHERE ID ="+RowId+") AND MATRIX_ID IN (SELECT ID FROM MATRIX WHERE NOVIS_TEMPLATE_ID = "+MatrixId+")";
                //    }
                //    else
                //    {
                //        sql = "UPDATE DIMENSION SET SUBTITLE = '" + Line0 + "',SUBTITLE2 = '" + Line1 + "' ,SUBTITLE3 = '" + Line2 + "' WHERE ORDNUMBER IN (SELECT ORDNUMBER FROM DIMENSION WHERE ID =" + RowId + ") AND MATRIX_ID IN (SELECT ID FROM MATRIX WHERE NOVIS_TEMPLATE_ID = " + MatrixId + ")";
                //    }
                //}
                //else
                //{
                //sql = "UPDATE CHARACTERISTIC SET TITLE = '" + Line0 + "',TITLE2 = '" + Line1 + "' ,TITLE3 = '" + Line2 + "' WHERE ORDNUMBER IN (SELECT ORDNUMBER FROM CHARACTERISTIC WHERE ID =" + CellId + ") AND DIMENSION_ID IN (SELECT ID FROM DIMENSION WHERE MATRIX_ID IN ( SELECT ID FROM MATRIX WHERE NOVIS_TEMPLATE_ID =" + MatrixId + "))";
                DBData db = DBData.getDBData(Session);
                db.connect();
                String Index = db.lookup("ORDNUMBER", "CHARACTERISTIC", "ID=" + CellId).ToString();
                DataTable ids = db.getDataTable("SELECT ID FROM DIMENSION WHERE MATRIX_ID IN (SELECT ID FROM MATRIX WHERE NOVIS_TEMPLATE_ID IN (SELECT MATRIX_ID FROM DIMENSION WHERE ID =" + RowId + ")) AND ORDNUMBER IN (SELECT ORDNUMBER FROM DIMENSION WHERE ID =" + RowId + ")");
                for (int i = 0; i < ids.Rows.Count; i++)
                {
                    sql = "UPDATE CHARACTERISTIC SET TITLE = '" + Line0 + Line1 + Line2 + "' WHERE (DIMENSION_ID = " + ids.Rows[i][0].ToString() + " AND ORDNUMBER = " + Index + ")";
                    dbUpdate(sql);
                }
                db.disconnect();
                //}
                dbUpdate(sql);
            }
            //if (CellId == "Title0" || CellId == "Title1")
            //{
            //    if (CellId == "Title0")
            //    {
            //        sql = "UPDATE DIMENSION SET TITLE = '" + Line0 + "',TITLE2 = '" + Line1 + "' ,TITLE3 = '" + Line2 + "' WHERE ID = '" + RowId + "'";
            //    }
            //    else
            //    {
            //        sql = "UPDATE DIMENSION SET SUBTITLE = '" + Line0 + "',SUBTITLE2 = '" + Line1 + "' ,SUBTITLE3 = '" + Line2 + "' WHERE ID = '" + RowId + "'";
            //    }
            //}
            //else
            //{
            sql = "UPDATE CHARACTERISTIC SET TITLE = '" + Line0 + Line1 + Line2 + "' WHERE ID = '" + CellId + "'";
            //}
            return dbUpdate(sql);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string dbTextChange(String MatrixId, String CellId, String Text)
        {
            //CellId = "Title";
            //CellId = "Subtitle";
            //CellId = "Author";
            //CellId = "Date";
            //CellId = "LastChange";
            //CellId = "Revision";
            //CellId = "Legend"+row+"_"+cell
            //CellId = CellId für Supportzeile
            Text = Text.Replace("'", "''");
            string sql;
            int ordnumber = 0;
            int wirkungspaketId = 0;
            if (CellId.Contains("Legend"))
            {
                String[] Coords = CellId.Substring(6).Split('_');
                ordnumber = (Convert.ToInt32(Coords[0]) - 1) * 2 + Convert.ToInt32(Coords[1]);
                CellId = "Legend";
            }
            if (CellId.Contains("Wirkungspakete"))
            {
                String[] Coords = CellId.Split('_');
                wirkungspaketId = Convert.ToInt32(Coords[1]);
                CellId = "Wirkungspakete";
            }
            if (CellId.Equals("Date") || CellId.Equals("LastChange") || CellId.Equals("Revision"))
            {
                Text = String.Format("{0:MM/dd/yyyy}", System.DateTime.Now);
            }
            switch (CellId)
            {
                case "Title":
                    sql = "UPDATE MATRIX SET TITLE = '" + Text + "' WHERE ID = '" + MatrixId + "'";
                    break;
                case "Subtitle":
                    sql = "UPDATE MATRIX SET SUBTITLE = '" + Text + "' WHERE ID = '" + MatrixId + "'";
                    break;
                case "Author":
                    sql = "UPDATE MATRIX SET AUTHOR = '" + Text + "' WHERE ID = '" + MatrixId + "'";
                    break;
                case "Date":
                    sql = "UPDATE MATRIX SET CREATIONDATE = '" + Text + "' WHERE ID = '" + MatrixId + "'";
                    break;
                case "LastChange":
                    sql = "UPDATE MATRIX SET LASTCHANGE = '" + Text + "' WHERE ID = '" + MatrixId + "'";
                    break;
                case "Revision":
                    sql = "UPDATE MATRIX SET REVISION = '" + Text + "' WHERE ID = '" + MatrixId + "'";
                    break;
                case "Legend":
                    if (Global.isModuleEnabled("novis"))
                    {
                        sql = "UPDATE COLORATION SET TITLE = '" + Text + "' WHERE MATRIX_ID IN (SELECT ID FROM MATRIX WHERE NOVIS_TEMPLATE_ID = " + MatrixId + ") AND ORDNUMBER = " + ordnumber;
                        dbUpdate(sql);
                    }
                    sql = "UPDATE COLORATION SET TITLE = '" + Text + "' WHERE MATRIX_ID = '" + MatrixId + "' AND ORDNUMBER = " + ordnumber;
                    break;
                case "Wirkungspakete":
                    sql = "UPDATE Wirkungspaket SET NAME = '" + Text + "' WHERE MATRIX_ID = '" + MatrixId + "' AND ID = " + wirkungspaketId;
                    break;
                default:
                    sql = "UPDATE CHARACTERISTIC SET SUBTITLE = '" + Text + "' WHERE ID = '" + CellId + "'";
                    break;
            }
            return dbUpdate(sql);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string dbAddRow(String MatrixId, String Index)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            string sql;
            if (Global.isModuleEnabled("novis"))
            {
                DataTable ids = db.getDataTable("SELECT ID FROM MATRIX WHERE NOVIS_TEMPLATE_ID = " + MatrixId);
                for (int i = 0; i < ids.Rows.Count; i++)
                {
                    sql = "UPDATE DIMENSION SET ORDNUMBER = ORDNUMBER + 1 WHERE (MATRIX_ID = " + ids.Rows[i][0].ToString() + " AND ORDNUMBER >= " + Index + ")";
                    dbUpdate(sql);
                    sql = "INSERT INTO DIMENSION (MATRIX_ID, ORDNUMBER) VALUES(" + ids.Rows[i][0].ToString() + " , " + Index + ")";
                    dbUpdate(sql);
                }
            }
            sql = "UPDATE DIMENSION SET ORDNUMBER = ORDNUMBER + 1 WHERE (MATRIX_ID = " + MatrixId + " AND ORDNUMBER >= " + Index + ")";
            dbUpdate(sql);
            sql = "INSERT INTO DIMENSION (MATRIX_ID, ORDNUMBER) VALUES(" + MatrixId + " , " + Index + ")";
            dbUpdate(sql);
            string id = db.lookup("ID", "DIMENSION", "MATRIX_ID = " + MatrixId + " AND ORDNUMBER = " + Index).ToString();
            db.disconnect();
            return id;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string dbMoveRow(long[] rows)
        {
            string sql;
            int ordnumber = 0;
            DBData db = DBData.getDBData(Session);
            db.connect();
            foreach (long rowId in rows)
            {
                db.execute("UPDATE DIMENSION SET ORDNUMBER = " + ordnumber + " WHERE ID = " + rowId);
                ordnumber += 1;
            }

            db.disconnect();
            return "";

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string dbDeleteRow(String MatrixId, String RowId, String Index)
        {
            string sql;
            if (Global.isModuleEnabled("novis"))
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                DataTable submatrixCells = db.getDataTable("SELECT CHARACTERISTIC.ID FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID WHERE CHARACTERISTIC.DETAIL_MATRIX_ID !='' AND DIMENSION.ID =" + RowId);
                for (int i = 0; i < submatrixCells.Rows.Count; i++)
                {
                    delsubMatrix(submatrixCells.Rows[i][0].ToString());
                }
                DataTable ids = db.getDataTable("SELECT ID FROM MATRIX WHERE NOVIS_TEMPLATE_ID =" + MatrixId);
                for (int i = 0; i < ids.Rows.Count; i++)
                {
                    String RowIdTemp = db.lookup("ID", "DIMENSION", "MATRIX_ID =" + ids.Rows[i][0].ToString() + " AND ORDNUMBER =" + Index).ToString();
                    sql = "DELETE FROM CHARACTERISTIC WHERE DIMENSION_ID = " + RowIdTemp;
                    dbUpdate(sql);
                    sql = "UPDATE DIMENSION SET ORDNUMBER = ORDNUMBER - 1 WHERE (MATRIX_ID = " + ids.Rows[i][0].ToString() + " AND ORDNUMBER >= " + Index + ")";
                    dbUpdate(sql);
                    sql = "DELETE FROM DIMENSION WHERE ID = " + RowIdTemp;
                    dbUpdate(sql);
                }
            }
            sql = "DELETE FROM CHARACTERISTIC WHERE DIMENSION_ID = " + RowId;
            dbUpdate(sql);
            sql = "UPDATE DIMENSION SET ORDNUMBER = ORDNUMBER - 1 WHERE (MATRIX_ID = " + MatrixId + " AND ORDNUMBER >= " + Index + ")";
            dbUpdate(sql);
            sql = "DELETE FROM DIMENSION WHERE ID = " + RowId;
            return dbUpdate(sql);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string dbAddCell(String RowID, String Index)
        {
            string sql;
            DBData db = DBData.getDBData(Session);
            db.connect();
            if (Global.isModuleEnabled("novis"))
            {
                DataTable ids = db.getDataTable("SELECT ID FROM DIMENSION WHERE MATRIX_ID IN (SELECT ID FROM MATRIX WHERE NOVIS_TEMPLATE_ID IN (SELECT MATRIX_ID FROM DIMENSION WHERE ID =" + RowID + ")) AND ORDNUMBER IN (SELECT ORDNUMBER FROM DIMENSION WHERE ID =" + RowID + ")");
                for (int i = 0; i < ids.Rows.Count; i++)
                {
                    sql = "UPDATE CHARACTERISTIC SET ORDNUMBER = ORDNUMBER + 1 WHERE (DIMENSION_ID = " + ids.Rows[i][0].ToString() + " AND ORDNUMBER >= " + Index + ")";
                    dbUpdate(sql);
                    sql = "INSERT INTO CHARACTERISTIC (DIMENSION_ID, ORDNUMBER) VALUES(" + ids.Rows[i][0].ToString() + " , " + Index + ")";
                    dbUpdate(sql);
                }
            }
            sql = "UPDATE CHARACTERISTIC SET ORDNUMBER = ORDNUMBER + 1 WHERE (DIMENSION_ID = " + RowID + " AND ORDNUMBER >= " + Index + ")";
            dbUpdate(sql);
            sql = "INSERT INTO CHARACTERISTIC (DIMENSION_ID, ORDNUMBER) VALUES(" + RowID + " , " + Index + ")";
            dbUpdate(sql);
            string id = db.lookup("ID", "CHARACTERISTIC", "DIMENSION_ID = '" + RowID + "' AND ORDNUMBER = '" + Index + "'").ToString();
            db.disconnect();
            return id;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void dbMoveCell(String CellID, String OldRowID, String RowID, String OldIndex, String Index)
        {
            string sql;
            DBData db = DBData.getDBData(Session);
            db.connect();
            //if (Global.isModuleEnabled("novis"))
            //{
            //    DataTable ids = db.getDataTable("SELECT ID FROM DIMENSION WHERE MATRIX_ID IN (SELECT ID FROM MATRIX WHERE NOVIS_TEMPLATE_ID IN (SELECT MATRIX_ID FROM DIMENSION WHERE ID =" + RowID + ")) AND ORDNUMBER IN (SELECT ORDNUMBER FROM DIMENSION WHERE ID =" + RowID + ")");
            //    for (int i = 0; i < ids.Rows.Count; i++)
            //    {
            //        sql = "UPDATE CHARACTERISTIC SET ORDNUMBER = ORDNUMBER + 1 WHERE (DIMENSION_ID = " + ids.Rows[i][0].ToString() + " AND ORDNUMBER >= " + Index + ")";
            //        dbUpdate(sql);
            //        sql = "INSERT INTO CHARACTERISTIC (DIMENSION_ID, ORDNUMBER) VALUES(" + ids.Rows[i][0].ToString() + " , " + Index + ")";
            //        dbUpdate(sql);
            //    }
            //}

            sql = "UPDATE CHARACTERISTIC SET ORDNUMBER = ORDNUMBER - 1 WHERE (DIMENSION_ID = " + OldRowID + " AND ORDNUMBER >= " + OldIndex + ")";
            dbUpdate(sql);
            sql = "UPDATE CHARACTERISTIC SET ORDNUMBER = ORDNUMBER + 1 WHERE (DIMENSION_ID = " + RowID + " AND ORDNUMBER >= " + Index + ")";
            dbUpdate(sql);
            sql = "UPDATE CHARACTERISTIC SET DIMENSION_ID = " + RowID + " , ORDNUMBER = " + Index + " WHERE ID = " + CellID;
            dbUpdate(sql);

            db.disconnect();

        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string dbDeleteCell(String RowID, String CellId, String Index)
        {
            string sql;
            DBData db = DBData.getDBData(Session);
            db.connect();
            String test = db.lookup("DETAIL_MATRIX_ID", "CHARACTERISTIC", "ID=" + CellId).ToString();
            if (Global.isModuleEnabled("novis"))
            {
                if (!db.lookup("DETAIL_MATRIX_ID", "CHARACTERISTIC", "ID=" + CellId).ToString().Equals(""))
                {
                    delsubMatrix(CellId);
                }
                DataTable ids = db.getDataTable("SELECT ID FROM DIMENSION WHERE MATRIX_ID IN (SELECT ID FROM MATRIX WHERE NOVIS_TEMPLATE_ID IN (SELECT MATRIX_ID FROM DIMENSION WHERE ID =" + RowID + ")) AND ORDNUMBER IN (SELECT ORDNUMBER FROM DIMENSION WHERE ID =" + RowID + ")");
                for (int i = 0; i < ids.Rows.Count; i++)
                {
                    sql = "DELETE FROM CHARACTERISTIC WHERE DIMENSION_ID =" + ids.Rows[i][0].ToString() + " AND ORDNUMBER =" + Index;
                    dbUpdate(sql);
                    sql = "UPDATE CHARACTERISTIC SET ORDNUMBER = ORDNUMBER - 1 WHERE (DIMENSION_ID = " + ids.Rows[i][0].ToString() + " AND ORDNUMBER >= " + Index + ")";
                    dbUpdate(sql);
                }
            }
            sql = "UPDATE CHARACTERISTIC SET ORDNUMBER = ORDNUMBER - 1 WHERE (DIMENSION_ID = " + RowID + " AND ORDNUMBER >= " + Index + ")";
            dbUpdate(sql);
            sql = "DELETE FROM WIRKUNGSELEMENT WHERE ORIGIN_CHARACTERISTIC_ID = " + CellId + " OR GOAL_CHARACTERISTIC_ID = " + CellId;
            dbUpdate(sql);
            sql = "DELETE FROM CHARACTERISTIC WHERE ID = " + CellId;
            return dbUpdate(sql);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string dbDescriptionChange(String MatrixId, String Text)
        {
            Text = Text.Replace("'", "''");
            if (Global.isModuleEnabled("novis"))
            {
                dbUpdate("UPDATE MATRIX SET DESCRIPTION = '" + Text + "' WHERE NOVIS_TEMPLATE_ID = '" + MatrixId + "'");
            }
            return dbUpdate("UPDATE MATRIX SET DESCRIPTION = '" + Text + "' WHERE ID = '" + MatrixId + "'");
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ConvertToPDF(string sContent, string MatrixId, int port, double width, double height, string svg, double svgHeight, double svgWidth, string orientation, string browser, double zoomLevel)
        {
            double svgTop = 0;
            switch (browser)
            {
                case "IE":
                    svgTop = 7;
                    break;
                case "CHROME":
                    svgTop = 7;
                    if (zoomLevel > 1)
                    {
                        svgTop = svgTop + svgTop / zoomLevel;
                    }
                    if(zoomLevel < 1)
                    {
                        svgTop = svgTop + svgTop / zoomLevel;
                    }
                    
                    break;
                case "FIREFOX":
                    svgTop = 3;
                    break;
                case "SAFARI":
                    svgTop = 7;
                    break;
                case "OPERA":
                    svgTop = 7;
                    break;
                case "UNKNOWN":
                    svgTop = 7;
                    break;
            }
            string dir = Server.MapPath(Global.Config.baseURL + "/Morph/html");
            string header = ReadFile(dir + "\\start.html");
            header = header.Replace("top: 0", "top: " + svgTop.ToString());
            string footer = ReadFile(dir + "\\end.html");
            /* WriteFile(dir + "\\svg.svg", svg);
             int svgIndex = sContent.IndexOf("<svg");
             string svgImageString = "<img src=\"svg.svg\" type=\"image/svg+xml\" style=\"opacity:0.65; position: absolute; width:" + svgWidth + "px; height:" + svgHeight + "px; background-color:transparent; top:16px;\"></img>";
             sContent = sContent.Insert(svgIndex, svgImageString);
             */
            // fix image links
            sContent = sContent.Replace("uf_matrix.png", "uf_matrix.png");
            sContent = sContent.Replace("icon_excel.gif", "morph/white.png");
            string protocoll = "http://";
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                protocoll = "https://";
            }
            if (port == 80)
            {
                header = header.Replace("%baseURL%", protocoll + Global.Config.domain + Global.Config.baseURL);
                sContent = sContent.Replace("../images", protocoll + Global.Config.domain + Global.Config.baseURL + "/images");
                sContent = sContent.Replace("../Style", protocoll + Global.Config.domain + Global.Config.baseURL + "/Style");
            }
            else
            {
                // different port, we are in debug mode
                header = header.Replace("%baseURL%", protocoll + Global.Config.domain + ":" + port + Global.Config.baseURL);
                sContent = sContent.Replace("../images", protocoll + Global.Config.domain + ":" + port + Global.Config.baseURL + "/images");
                sContent = sContent.Replace("../Style", protocoll + Global.Config.domain + ":" + port + Global.Config.baseURL + "/Style");
                //sContent = "<img src=\"svg.svg\" type=\"image/svg+xml\" style=\"opacity:0.65; position: absolute; width:" + svgWidth + "px; height:" + svgHeight + "px; background-color:transparent;\"></img>" + sContent; // SVG Image hinzufügen
            }
            string export = ConvertUmlauts(header + "\r\n\r\n" + sContent + "\r\n\r\n" + footer);
            string filename = dir + "\\" + MatrixId + ".html";
            //WriteFile(filename,  export );
            StreamWriter Exporter = new StreamWriter(filename, false, Encoding.UTF8);
            //export = export.Replace((char)8209, (char)45);
            export = export.Replace("‑", "-");
            Exporter.Write(export);
            Exporter.Close();
            string pdf = Server.MapPath(Global.Config.baseURL + "/Morph/pdf") + "\\" + MatrixId + ".pdf";
            string converter = Server.MapPath(Global.Config.baseURL + "/Morph/pdfconvert") + "\\wkhtmltopdf.exe";
            HtmlToPdf pdfConverter = new HtmlToPdf();
            pdfConverter.SerialNumber = "5KyNtbSA-gqiNhpaF-lp3dytTE-1cTVxNDd-3NzE19XK-1dbK3d3d-3Q==";
            pdfConverter.Document.ImagesCompression = 0;
            pdfConverter.Document.Margins.Top = 5;
            pdfConverter.Document.Margins.Bottom = 5;
            pdfConverter.Document.Margins.Right = 5;
            pdfConverter.Document.Margins.Left = 5;
            pdfConverter.Document.PageOrientation = orientation == "1" ? PdfPageOrientation.Landscape : PdfPageOrientation.Portrait;
            pdfConverter.Document.FitPageHeight = true;
            pdfConverter.Document.FitPageWidth = true;
            pdfConverter.Document.Compress = true;
            pdfConverter.Document.PdfStandard = PdfStandard.Pdf;
            pdfConverter.Document.ImagesCutAllowed = false;
            //pdfConverter.Document.RenderImagesWithTransparency = true;
            //pdfConverter.Document.RenderWithTransparency = true;
            //pdfConverter.Document.DisplayMaskedImages = true;
            pdfConverter.ConvertUrlToFile(filename, pdf);
            return pdf;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string addKnowledge(string cellId, string knowledgeId)
        {
            return dbUpdate("UPDATE CHARACTERISTIC SET KNOWLEDGE_ID = " + knowledgeId + " WHERE ID = " + cellId);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string addsubMatrix(string cellId, string matrixId)
        {
            if (Global.isModuleEnabled("novis"))
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                DataTable idTable = db.getDataTable("SELECT DISTINCT CHARACTERISTIC.DETAIL_MATRIX_ID FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID WHERE   ( CHARACTERISTIC.DETAIL_MATRIX_ID IS NOT NULL) AND  (DIMENSION.MATRIX_ID = " + matrixId + ")");
                DataTable newIdTable = idTable;
                string rootId = "";
                // get all ids of submatrices
                while (newIdTable.Rows.Count > 0)
                {
                    idTable.Merge(newIdTable);
                    String sql = "SELECT DISTINCT CHARACTERISTIC.DETAIL_MATRIX_ID FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID WHERE  ( CHARACTERISTIC.DETAIL_MATRIX_ID IS NOT NULL) AND (DIMENSION.MATRIX_ID = ";
                    for (int i = 0; i < newIdTable.Rows.Count; i++)
                    {
                        if (i > 0)
                        {
                            sql += " OR DIMENSION.MATRIX_ID = ";
                        }
                        sql += newIdTable.Rows[i][0].ToString();
                    }
                    sql += ")";
                    newIdTable = db.getDataTable(sql);
                }
                rootId = db.lookup("MATRIX.NOVIS_ROOT_ID", "CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID INNER JOIN MATRIX ON DIMENSION.MATRIX_ID = MATRIX.ID", "CHARACTERISTIC.ID = " + cellId).ToString(); ;
                // set root id in matrices
                string sql2 = "UPDATE MATRIX SET NOVIS_ROOT_ID = " + rootId + " WHERE ID =" + matrixId;
                for (int i = 0; i < idTable.Rows.Count; i++)
                {
                    sql2 += " OR ID = " + idTable.Rows[i][0].ToString() + " ";
                }
                db.execute(sql2);
                if (db.lookup("IS_NOVIS_TEMPLATE", "MATRIX", "ID =" + matrixId).ToString().Equals("True"))
                {
                    DataTable cells = db.getDataTable("SELECT     CHARACTERISTIC.ID " +
                        " FROM  MATRIX INNER JOIN" +
                          " CHARACTERISTIC INNER JOIN" +
                          " DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN" +
                          " DIMENSION AS DIMENSION_1 INNER JOIN" +
                          " MATRIX AS MATRIX_1 ON DIMENSION_1.MATRIX_ID = MATRIX_1.ID INNER JOIN" +
                          " CHARACTERISTIC AS CHARACTERISTIC_1 ON DIMENSION_1.ID = CHARACTERISTIC_1.DIMENSION_ID ON MATRIX.NOVIS_TEMPLATE_ID = MATRIX_1.ID  AND " +
                          " CHARACTERISTIC.ORDNUMBER = CHARACTERISTIC_1.ORDNUMBER  AND DIMENSION.ORDNUMBER = DIMENSION_1.ORDNUMBER" +
                         " WHERE     (CHARACTERISTIC_1.ID = " + cellId + ")");
                    for (int i = 0; i < cells.Rows.Count; i++)
                    {
                        String derivedId = deriveSokrates(matrixId);
                        addsubMatrix(cells.Rows[i][0].ToString(), derivedId);
                    }
                    DataTable subMatricies = db.getDataTable("SELECT ID FROM MATRIX WHERE NOVIS_TEMPLATE_ID = " + matrixId + " AND NOVIS_ROOT_ID = ID");
                    for (int i = 0; i < subMatricies.Rows.Count; i++)
                    {
                        String derivedId = deriveSokrates(rootId);
                        derivedId = db.lookup("ID", "MATRIX", "NOVIS_ROOT_ID = " + derivedId + " AND NOVIS_TEMPLATE_ID IN (SELECT MATRIX.ID FROM MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID WHERE CHARACTERISTIC.ID =" + cellId + ")").ToString();
                        String tempCellId = db.lookup("ID", "CHARACTERISTIC", "DIMENSION_ID IN (SELECT ID FROM DIMENSION WHERE MATRIX_ID = " + derivedId + " AND ORDNUMBER IN (SELECT DIMENSION.ORDNUMBER FROM DIMENSION INNER JOIN CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID WHERE CHARACTERISTIC.ID=" + cellId + ")) AND ORDNUMBER IN (SELECT ORDNUMBER FROM CHARACTERISTIC WHERE ID =" + cellId + ")").ToString();
                        addsubMatrix(tempCellId, subMatricies.Rows[i][0].ToString());
                    }
                }
                db.disconnect();
            }
            return dbUpdate("UPDATE CHARACTERISTIC SET DETAIL_MATRIX_ID = " + matrixId + " WHERE ID = " + cellId);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string delKnowledge(string cellId)
        {
            return dbUpdate("UPDATE CHARACTERISTIC SET KNOWLEDGE_ID = NULL WHERE ID = " + cellId);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string delsubMatrix(string cellId)
        {
            if (Global.isModuleEnabled("novis"))
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                String matrixId = db.lookup("DETAIL_MATRIX_ID", "CHARACTERISTIC", "ID=" + cellId).ToString();
                if (!matrixId.Equals(""))
                {
                    DataTable idTable = db.getDataTable("SELECT DISTINCT CHARACTERISTIC.DETAIL_MATRIX_ID FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID WHERE   ( CHARACTERISTIC.DETAIL_MATRIX_ID IS NOT NULL) AND  (DIMENSION.MATRIX_ID = " + matrixId + ")");
                    DataTable newIdTable = idTable;
                    string rootId = "";
                    // get all ids of submatrices
                    while (newIdTable.Rows.Count > 0)
                    {
                        idTable.Merge(newIdTable);
                        String sql = "SELECT DISTINCT CHARACTERISTIC.DETAIL_MATRIX_ID FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID WHERE  ( CHARACTERISTIC.DETAIL_MATRIX_ID IS NOT NULL) AND (DIMENSION.MATRIX_ID = ";
                        for (int i = 0; i < newIdTable.Rows.Count; i++)
                        {
                            if (i > 0)
                            {
                                sql += " OR DIMENSION.MATRIX_ID = ";
                            }
                            sql += newIdTable.Rows[i][0].ToString();
                        }
                        sql += ")";
                        newIdTable = db.getDataTable(sql);
                    }
                    rootId = matrixId;
                    // set root id in matrices
                    string sql2 = "UPDATE MATRIX SET NOVIS_ROOT_ID = " + rootId + " WHERE ID =" + matrixId;
                    for (int i = 0; i < idTable.Rows.Count; i++)
                    {
                        sql2 += " OR ID = " + idTable.Rows[i][0].ToString() + " ";
                    }
                    db.execute(sql2);
                    string test = db.lookup("IS_NOVIS_TEMPLATE", "MATRIX", "ID =" + matrixId).ToString();
                    if (db.lookup("IS_NOVIS_TEMPLATE", "MATRIX", "ID =" + matrixId).ToString().Equals("True"))
                    {
                        DataTable cells = db.getDataTable("SELECT     CHARACTERISTIC.ID " +
                  " FROM  MATRIX INNER JOIN" +
                    " CHARACTERISTIC INNER JOIN" +
                    " DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN" +
                    " DIMENSION AS DIMENSION_1 INNER JOIN" +
                    " MATRIX AS MATRIX_1 ON DIMENSION_1.MATRIX_ID = MATRIX_1.ID INNER JOIN" +
                    " CHARACTERISTIC AS CHARACTERISTIC_1 ON DIMENSION_1.ID = CHARACTERISTIC_1.DIMENSION_ID ON MATRIX.NOVIS_TEMPLATE_ID = MATRIX_1.ID  AND " +
                    " CHARACTERISTIC.ORDNUMBER = CHARACTERISTIC_1.ORDNUMBER  AND DIMENSION.ORDNUMBER = DIMENSION_1.ORDNUMBER" +
                   " WHERE     (CHARACTERISTIC_1.ID = " + cellId + ")");
                        for (int i = 0; i < cells.Rows.Count; i++)
                        {
                            delsubMatrix(cells.Rows[i][0].ToString());
                        }
                    }
                }
                db.disconnect();
            }
            return dbUpdate("UPDATE CHARACTERISTIC SET DETAIL_MATRIX_ID = NULL WHERE ID = " + cellId);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string copySokratesMap(string matrixId, bool copyColoration, bool copyWirkungspakete)
        {
            string matrixIdNew;
            DBData db = DBData.getDBData(Session);
            db.connect();
            if (Global.Config.shopActive)
            {
                int numCards = (int)db.lookup("NUMBER_OF_CARDS", "PERSON", "ID=" + db.userId);
                if (numCards < 1)
                {
                    Session.Add("OrderNeed", true);
                    return "OrderNeed";
                }
                else
                {
                    db.execute("UPDATE PERSON SET NUMBER_OF_CARDS = " + (numCards - 1) + "WHERE ID=" + db.userId);
                }
            }
            db.executeProcedure("copy_sokrates_map ", 0, 180, new ParameterCtx("MATRIXID", matrixId), new ParameterCtx("USERID", db.userId), new ParameterCtx("copyColoration", copyColoration), new ParameterCtx("copyWirkungspakete", copyWirkungspakete));
            matrixIdNew = db.lookup("max(id)", "MATRIX", "id>1").ToString();
            //db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, db.userAccessorID, "Matrix", Convert.ToInt32(matrixIdNew));
            setMatrixrights(db, matrixIdNew);
            //set rights for new local knowlege elements
            DataTable localKnowlege = db.getDataTable("SELECT KNOWLEDGE.ID FROM KNOWLEDGE INNER JOIN CHARACTERISTIC ON KNOWLEDGE.ID = CHARACTERISTIC.KNOWLEDGE_ID INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID INNER JOIN MATRIX ON DIMENSION.MATRIX_ID = MATRIX.ID WHERE (KNOWLEDGE.LOCAL = 1) AND (MATRIX.ID = " + matrixIdNew + ")");
            foreach (DataRow row in localKnowlege.Rows)
            {
                db.execute("EXECUTE COPY_ACCESS_RIGHT_ROW MATRIX, " + matrixId + " ,KNOWLEDGE," + row[0]);
            }
            //set rights for Matrix if GFK template
            if ((bool)db.lookup("IS_GFK_TEMPLATE", "MATRIX", "ID = " + matrixId))
            {
                db.execute("EXECUTE COPY_ACCESS_RIGHT_ROW MATRIX, " + matrixId + " ,MATRIX," + matrixIdNew);
            }
            db.disconnect();
            return matrixIdNew;
        }
        private string dbUpdate(string sql)
        {
            DBData db = DBData.getDBData(Session);
            bool success = true;
            try
            {
                db.connect();
                db.beginTransaction();
                db.execute(sql);
                db.commit();
            }
            catch (Exception e)
            {
                Logger.Log(e, Logger.ERROR);
                success = false;
                db.rollback();
            }
            finally
            {
                db.disconnect();
            }
            return success.ToString();
        }
        protected string ReadFile(string filename)
        {
            TextReader r = new StreamReader(filename);
            string text = r.ReadToEnd();
            r.Close();
            return text;
        }
        protected void WriteFile(string filename, string content)
        {
            TextWriter w = new StreamWriter(filename, false, Encoding.ASCII);
            w.Write(content);
            w.Close();
        }
        protected string ConvertUmlauts(string content)
        {
            return content.Replace("ä", "&auml;").Replace("Ä", "&Auml;").Replace("ö", "&ouml;").Replace("Ö", "&Ouml;").Replace("ü", "&uuml;").Replace("Ü", "&Uuml;");
        }
        private static object GetList(DataTable Dt)
        {
            List<ListObj> dList = new List<ListObj>();
            foreach (DataRow row in Dt.Rows)
            {
                ListObj listItem = new ListObj { id = row[0].ToString(), title = row[1].ToString() };
                dList.Add(listItem);
            }
            ListObj listItemFirst = new ListObj { id = "0", title = " " };
            dList.Add(listItemFirst);
            return dList;
        }
        private class ListObj
        {
            public string id { get; set; }
            public string title { get; set; }
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string addWirkungselement(string cellId1, string cellId2, string wirkungspaketId)
        {
            string wirkungselementId;
            dbUpdate("INSERT INTO Wirkungselement (ORIGIN_CHARACTERISTIC_ID, GOAL_CHARACTERISTIC_ID, Wirkungspaket_ID) VALUES(" + cellId1 + " , " + cellId2 + " , " + wirkungspaketId + ")");
            DBData db = DBData.getDBData(Session);
            db.connect();
            wirkungselementId = db.lookup("max(id)", "Wirkungselement", "id>=0").ToString();
            db.disconnect();
            return wirkungselementId;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string removeWirkungselement(string cellId1, string cellId2, string wirkungspaketId)
        {
            return dbUpdate("DELETE FROM Wirkungselement WHERE (ORIGIN_CHARACTERISTIC_ID = " + cellId1 + " AND GOAL_CHARACTERISTIC_ID = " + cellId2 + " AND Wirkungspaket_ID = " + wirkungspaketId + ")");
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string removeWirkungspaket(string wirkungspaketId, string ordnumber)
        {
            dbUpdate("DELETE FROM Wirkungselement WHERE Wirkungspaket_ID = " + wirkungspaketId);
            return dbUpdate("UPDATE Wirkungspaket SET NAME = 'WP" + ordnumber + "' WHERE ID = '" + wirkungspaketId + "'");
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object loadWirkungselementArray(String matrixId)
        {
            DBData db = DBData.getDBData(Session);
            DataTable wirkungselementTable = db.getDataTable("SELECT     WIRKUNGSELEMENT.ID, WIRKUNGSELEMENT.ORIGIN_CHARACTERISTIC_ID, WIRKUNGSELEMENT.GOAL_CHARACTERISTIC_ID, WIRKUNGSELEMENT.WIRKUNGSPAKET_ID FROM WIRKUNGSELEMENT INNER JOIN WIRKUNGSPAKET ON WIRKUNGSELEMENT.WIRKUNGSPAKET_ID = WIRKUNGSPAKET.ID WHERE (WIRKUNGSPAKET.MATRIX_ID = " + matrixId + ")");
            List<WirkungselementListObj> dList = new List<WirkungselementListObj>();
            foreach (DataRow row in wirkungselementTable.Rows)
            {
                WirkungselementListObj listItem = new WirkungselementListObj { id = row[0].ToString(), ORIGIN_CHARACTERISTIC_ID = row[1].ToString(), GOAL_CHARACTERISTIC_ID = row[2].ToString(), Wirkungspaket_ID = row[3].ToString() };
                dList.Add(listItem);
            }
            return dList;
        }
        private class WirkungselementListObj
        {
            public string id { get; set; }
            public string ORIGIN_CHARACTERISTIC_ID { get; set; }
            public string GOAL_CHARACTERISTIC_ID { get; set; }
            public string Wirkungspaket_ID { get; set; }
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string addNewKnowledge(string cellId, string matrixId)
        {
            string newThemeId;
            string title = "Neues lokales Wissenselement " + String.Format("{0:dd/MM/yyyy hh:mm:ss}", System.DateTime.Now);
            DBData db = DBData.getDBData(Session);
            db.connect();
            string sql = "INSERT INTO THEME (TITLE,CREATOR_PERSON_ID) VALUES ('" + title + "'," + db.userId + ")";
            db.execute(sql);
            newThemeId = db.lookup("max(ID)", "THEME", "id>1").ToString();
            sql = "UPDATE THEME SET ROOT_ID = " + newThemeId + " WHERE ID = " + newThemeId;
            db.execute(sql);
            sql = "insert into KNOWLEDGE(BASE_THEME_ID_DE, REASON, LOCAL) values (" + newThemeId + ",'Initialversion', 1)";
            db.execute(sql);
            string newKnowledgeId = db.lookup("ID", "KNOWLEDGE", "BASE_THEME_ID_DE = " + newThemeId).ToString();
            // copy rights from sokrates map
            sql = "EXECUTE COPY_ACCESS_RIGHT_ROW MATRIX, " + matrixId + " ,KNOWLEDGE," + newKnowledgeId;
            db.execute(sql);
            db.disconnect();
            dbUpdate("UPDATE CHARACTERISTIC SET KNOWLEDGE_ID = " + newKnowledgeId + " WHERE ID = " + cellId);
            return newKnowledgeId;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string deriveSokrates(string matrixId)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable idTable = db.getDataTable("SELECT DISTINCT CHARACTERISTIC.DETAIL_MATRIX_ID FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID WHERE     (DIMENSION.MATRIX_ID = " + matrixId + ") AND CHARACTERISTIC.DETAIL_MATRIX_ID IS NOT NULL");
            DataTable newIdTable = idTable;
            String matrixIdNew;
            ArrayList newIds = new ArrayList();
            // get all ids of submatrices
            while (newIdTable.Rows.Count > 0)
            {
                idTable.Merge(newIdTable);
                String sql = "SELECT DISTINCT CHARACTERISTIC.DETAIL_MATRIX_ID FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID WHERE  ( CHARACTERISTIC.DETAIL_MATRIX_ID IS NOT NULL) AND (DIMENSION.MATRIX_ID = ";
                for (int i = 0; i < newIdTable.Rows.Count; i++)
                {
                    if (i > 0)
                    {
                        sql += " OR DIMENSION.MATRIX_ID = ";
                    }
                    sql += newIdTable.Rows[i][0].ToString();
                }
                sql += ")";
                newIdTable = db.getDataTable(sql);
            }
            // copy matrices bottom up + save new ids
            for (int i = idTable.Rows.Count - 1; i > -1; i--)
            {
                db.executeProcedure("copy_novis_sokrates_map ", 0, 180, new ParameterCtx("MATRIXID", idTable.Rows[i][0]), new ParameterCtx("USERID", db.userId));
                matrixIdNew = db.lookup("max(id)", "MATRIX", "id>1").ToString();
                db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, db.userAccessorID, "Matrix", Convert.ToInt32(matrixIdNew));
                db.grantRowAuthorisation(DBData.AUTHORISATION.READ, DBData.ACCESSOR.ALL, "Matrix", Convert.ToInt32(matrixIdNew));
                newIds.Add(matrixIdNew);
            }
            db.executeProcedure("copy_novis_sokrates_map ", 0, 180, new ParameterCtx("MATRIXID", matrixId), new ParameterCtx("USERID", db.userId));
            matrixIdNew = db.lookup("max(id)", "MATRIX", "id>1").ToString();
            db.grantRowAuthorisation(DBData.AUTHORISATION.RAUDI, db.userAccessorID, "Matrix", Convert.ToInt32(matrixIdNew));
            db.grantRowAuthorisation(DBData.AUTHORISATION.READ, DBData.ACCESSOR.ALL, "Matrix", Convert.ToInt32(matrixIdNew));
            newIds.Add(matrixIdNew);
            // set root id in all new matrices
            string sql2 = "UPDATE MATRIX SET NOVIS_ROOT_ID = " + matrixIdNew + " WHERE";
            for (int i = 0; i < newIds.Count; i++)
            {
                if (i > 0)
                {
                    sql2 += "OR";
                }
                sql2 += " ID = " + newIds[i] + " ";
            }
            db.execute(sql2);
            for (int i = 0; i < idTable.Rows.Count; i++)
            {
                sql2 = "UPDATE CHARACTERISTIC SET CHARACTERISTIC.DETAIL_MATRIX_ID =" + newIds[newIds.Count - i - 2] + " FROM CHARACTERISTIC INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID INNER JOIN MATRIX ON DIMENSION.MATRIX_ID = MATRIX.ID  WHERE  (MATRIX.NOVIS_ROOT_ID = " + matrixIdNew + ") AND (MATRIX.IS_NOVIS_TEMPLATE IS NULL) AND  CHARACTERISTIC.DETAIL_MATRIX_ID = " + idTable.Rows[i][0].ToString();
                db.execute(sql2);
            }
            db.disconnect();
            return matrixIdNew;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object loadNovisSubmatrix(String matrixId)
        {
            DBData db = DBData.getDBData(Session);
            DataTable submatrix = new DataTable();
            if (Global.isModuleEnabled("novis"))
            {
                submatrix = db.getDataTable("SELECT DISTINCT ID, TITLE FROM MATRIX " + db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " AND MATRIX.IS_NOVIS_TEMPLATE = 1  AND MATRIX.ID = MATRIX.NOVIS_ROOT_ID AND ID != " + matrixId + " AND MATRIX.ID NOT IN (SELECT NOVIS_ROOT_ID FROM MATRIX WHERE ID = " + matrixId + ")  ORDER BY TITLE DESC");
            }
            return GetList(submatrix);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object createNovisReport()
        {
            DBData db = DBData.getDBData(base.Session);
            SqlConnection sqlcon = (SqlConnection)db.connection;
            SqlBulkCopy StatusLeistungsbewertungenSPZ = new SqlBulkCopy(sqlcon.ConnectionString + "; Password=" + Global.Config.dbPassword);
            // delete temporary table if exists
            String tbl_del = " IF NOT OBJECT_ID('tempdb..[##NovisReport_%userid%]') IS NULL "
              + "DROP TABLE [##NovisReport_%userid%]";
            db.connect();
            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));
            // create table
            String tbl_create = "CREATE TABLE [##NovisReport_%userid%]("
                                    + "[PARENT_ID] [bigint] NULL"
                                   + ",[ID] [bigint] NULL"
                                    + ",[TITLE] [varchar](180) NULL"
                                  + ",[TITLE_4] [varchar](60) NULL"
                                  + ",[COLOR_NAME] [varchar](256)"
                                  + ",[COLOR] [int]"
                                  + ",[MATRIX_ID] [bigint] NULL"
                                  + ",[MATRIX_TITLE] [varchar](256) NULL"
                                  + ",[CUSTOMER_NR] [varchar](256) NULL"
                                  + ") ON [PRIMARY]";
            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));
            StatusLeistungsbewertungenSPZ.DestinationTableName = "##NovisReport_" + db.userId.ToString();
            DataTable result = new DataTable();
            DataTable matrix = db.getDataTable("SELECT * FROM MATRIX_" + db.userId);
            int cellnr = (matrix.Columns.Count - 6) / 5;
            for (int i = 0; i < matrix.Rows.Count; i = i + 2)
            {
                for (int j = 0; j < cellnr; j++)
                {
                    if (!matrix.Rows[i][("CellId_" + j)].ToString().Equals("") && !matrix.Rows[i][("CellColor_" + j)].ToString().Equals("0"))
                    {
                        String sql = "SELECT     CHARACTERISTIC.ID AS PARENT_ID, CHARACTERISTIC_1.ID, CHARACTERISTIC.TITLE,CHARACTERISTIC.SUBTITLE AS TITLE_4, COLORATION_1.TITLE AS COLOR_NAME, COLORATION_1.COLOR , DIMENSION_1.MATRIX_ID, " +
                            " MATRIX_1.TITLE AS MATRIX_TITLE, MATRIX_1.SUBTITLE AS CUSTOMER_NR" +
                            " FROM         COLORATION INNER JOIN" +
                            " COLORATION AS COLORATION_1 ON COLORATION.ORDNUMBER = COLORATION_1.ORDNUMBER INNER JOIN" +
                            " DIMENSION AS DIMENSION_1 INNER JOIN" +
                            " CHARACTERISTIC AS CHARACTERISTIC_1 INNER JOIN" +
                            " CHARACTERISTIC INNER JOIN" +
                            " DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID ON CHARACTERISTIC_1.ORDNUMBER = CHARACTERISTIC.ORDNUMBER INNER JOIN" +
                            " MATRIX AS MATRIX_1 ON DIMENSION.MATRIX_ID = MATRIX_1.NOVIS_TEMPLATE_ID ON DIMENSION_1.MATRIX_ID = MATRIX_1.ID AND " +
                            " DIMENSION_1.ORDNUMBER = DIMENSION.ORDNUMBER AND DIMENSION_1.ID = CHARACTERISTIC_1.DIMENSION_ID ON COLORATION.ID = CHARACTERISTIC_1.COLOR_ID" +
                           " WHERE     (CHARACTERISTIC.ID = " + matrix.Rows[i][("CellId_" + j)] + ") AND (COLORATION_1.ID = " + matrix.Rows[i][("CellColor_" + j)] + ") ";
                        result.Merge(db.getDataTable(sql));
                    }
                }
            }
            db.execute("UPDATE CHARACTERISTIC SET COLOR_ID = NULL WHERE DIMENSION_ID IN (SELECT DIMENSIONID FROM MATRIX_" + db.userId + ")");
            StatusLeistungsbewertungenSPZ.WriteToServer(result);
            return null;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DataTable getMatrxWithSubmatrixs(long matrixId)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable matrix = db.getDataTable("select matrixId from GET_SUBMATRIXS(" + matrixId + ")");
            db.disconnect();
            return matrix;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public long copyMatrixWithSubmatrixs(long toUserId, DataTable submatrixs, int authorisation, bool linkNewSubmatrixs)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            string newMatrixIds = "";
            DataColumn newMatrixId = new DataColumn("newMatrixId", typeof(long));
            submatrixs.Columns.Add(newMatrixId);
            long newRootMatrixId = 0;
            for (int i = 0; i < submatrixs.Rows.Count; i++)
            {
                //db.executeProcedure("copy_sokrates_map ", 0, 180, new ParameterCtx("MATRIXID", submatrixs.Rows[i]["matrixId"]), new ParameterCtx("USERID", toUserId), new ParameterCtx("copyColoration", true), new ParameterCtx("copyWirkungspakete", true));
                db.executeProcedure("copy_sokrates_map ", 0, 180, new ParameterCtx("MATRIXID", submatrixs.Rows[i]["matrixId"]), new ParameterCtx("USERID", toUserId), new ParameterCtx("copyColoration", true), new ParameterCtx("copyWirkungspakete", true));
                long newMatrixIdTmp = (long)db.lookup("max(id)", "MATRIX", "id>1");
                if (i == 0)
                {
                    newRootMatrixId = newMatrixIdTmp;
                }
                submatrixs.Rows[i]["newMatrixId"] = newMatrixIdTmp;
                newMatrixIds += newMatrixIdTmp.ToString() + ", ";
                db.grantRowAuthorisation(authorisation, toUserId, "Matrix", newMatrixIdTmp);
                //set rights for new local knowlege elements
                DataTable localKnowlege = db.getDataTable("SELECT KNOWLEDGE.ID FROM KNOWLEDGE INNER JOIN CHARACTERISTIC ON KNOWLEDGE.ID = CHARACTERISTIC.KNOWLEDGE_ID INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID INNER JOIN MATRIX ON DIMENSION.MATRIX_ID = MATRIX.ID WHERE (KNOWLEDGE.LOCAL = 1) AND (MATRIX.ID = " + newMatrixIdTmp + ")");
                foreach (DataRow row in localKnowlege.Rows)
                {
                    db.grantRowAuthorisation(authorisation, toUserId, "KNOWLEDGE", (long)row[0]);
                }
            }
            if (newMatrixIds.Length > 1 && linkNewSubmatrixs)
            {
                newMatrixIds = newMatrixIds.Remove(newMatrixIds.Length - 2, 2);
                DataTable SubMatrixs = db.getDataTable("SELECT CHARACTERISTIC.ID, CHARACTERISTIC.DETAIL_MATRIX_ID  FROM MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN "
                                                      + "CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID "
                                                      + "WHERE(MATRIX.ID IN(" + newMatrixIds + ")) AND(NOT(CHARACTERISTIC.DETAIL_MATRIX_ID IS NULL))");
                foreach (DataRow subMatrix in SubMatrixs.Rows)
                {
                    DataRow[] newMatrixTmp = submatrixs.Select("matrixId= " + (long)subMatrix["DETAIL_MATRIX_ID"]);
                    db.execute("UPDATE CHARACTERISTIC SET DETAIL_MATRIX_ID = " + newMatrixTmp[0]["newMatrixId"] + " WHERE ID= " + (long)subMatrix["ID"]);
                }
            }
            submatrixs.Columns.Remove("newMatrixId");
            db.disconnect();
            return newRootMatrixId;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public object getKnowledgeSimple(long knowledgeId)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable knowledgeTable = db.getDataTable("SELECT THEME_ID, THEME_TITLE, THEME_DESCRIPTION, THEME_ORDNUMBER FROM ThemeTree where KNOWLWDGE_ID = " + knowledgeId + " ORDER BY THEME_ORDNUMBER");

            List<SimpleKnowledgeList> knowledgeList = new List<SimpleKnowledgeList>();
            SimpleKnowledgeList listItem = new SimpleKnowledgeList();
            bool firstItem = true;
            foreach (DataRow row in knowledgeTable.Rows)
            {
                if (firstItem)
                {
                    listItem.id = row[0].ToString();
                    listItem.title = row[1].ToString();
                    if (row[2].ToString().Length > 0)
                    {
                        listItem.description = row[2].ToString();
                    }
                    firstItem = false;
                }
                else
                {
                    listItem.description += "<hr/><p>&nbsp;</p><p><strong><span style = \"font-size: 16px;\" >" + row[1].ToString() + "</span></strong></p>";
                    if (row[2].ToString().Length > 0)
                    {
                        listItem.description += "<p>&nbsp;</p>" + row[2].ToString();
                    }
                }

            }

            long KnowledgeUid = (long)db.lookup("UID", "KNOWLEDGE", "ID=" + knowledgeId);

            DataTable docs = db.getDataTable("select ID, UID, TITLE from DOCUMENT where KNOWLEDGE_ID=" + knowledgeId + " and TYP= 0  order by TITLE");
            DataTable documentLinks = db.getDataTable("SELECT ID, UID, TITLE FROM DOCUMENT WHERE UID IN (SELECT TO_UID FROM UID_ASSIGNMENT WHERE FROM_UID=" + KnowledgeUid + ")");
            if (docs.Rows.Count > 0 || documentLinks.Rows.Count > 0)
            {
                listItem.description += "<p>&nbsp;</p><strong>Dokumnete:</strong><br>";
            }


            if (docs.Rows.Count > 0)
            {
                foreach (DataRow docRow in docs.Rows)
                {
                    listItem.description += "<a href = \"" + Global.Config.baseURL + "/goto.aspx?uid=" + docRow["UID"].ToString() + "\" target = \"_blank\" class=\"bold\">" + docRow["TITLE"].ToString() + " </a>";
                    listItem.description += "<br>";
                }
            }

            if (documentLinks.Rows.Count > 0)
            {

                foreach (DataRow linkRow in documentLinks.Rows)
                {
                    listItem.description += "<a href = \"" + Global.Config.baseURL + "/goto.aspx?uid=" + linkRow["UID"].ToString() + "\" target = \"_blank\" class=\"bold\">" + linkRow["TITLE"].ToString() + " </a>";
                    listItem.description += "<br>";
                }
            }

            db.disconnect();
            knowledgeList.Add(listItem);
            return knowledgeList;
        }

        private class SimpleKnowledgeList
        {
            public string id { get; set; }
            public string title { get; set; }
            public string description { get; set; }

            public int themeOrdnumber { get; set; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string saveKnowledgeSimple(long knowledgeId, string title, string description)
        {
            string error = "";
            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                db.beginTransaction();
                db.execute("UPDATE THEME SET DESCRIPTION = '" + description + "', TITLE = '" + title + "' WHERE ID =" + knowledgeId);
                db.commit();
            }
            catch (Exception e)
            {
                Logger.Log(e, Logger.ERROR);
                error = e.ToString();
                db.rollback();
            }
            finally
            {
                db.disconnect();
            }
            return error;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string deleteKnowledgeSimple(long knowledgeId)
        {
            string error = "";
            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                db.beginTransaction();
                long SourceID = Convert.ToUInt32(db.lookup("ROOT_ID", "THEME", "ID =" + knowledgeId));
                db.execute("DELETE FROM KNOWLEDGE WHERE ID =" + SourceID);
                db.commit();
            }
            catch (Exception e)

            {
                Logger.Log(e, Logger.ERROR);
                error = e.ToString();
                db.rollback();
            }
            finally
            {
                db.disconnect();
            }
            return error;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string translate(string text, string scope)
        {
            string txt = "";
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            txt = _map.get(scope, text);
            return txt;
        }
    }
}
